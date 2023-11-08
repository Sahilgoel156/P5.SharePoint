using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client.Publishing;
using P5.SharePoint.Core;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;
using P5.SharePoint.Core.Models.Search;
using P5.SharePoint.Core.Options;
using P5.SharePoint.Core.Services;
using P5.SharePoint.Data.Caching;
using P5.SharePoint.Data.Query;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.Settings;
using SP = Microsoft.SharePoint.Client;
using ValueType = P5.SharePoint.Data.Query.ValueType;

namespace P5.SharePoint.Data.Services
{
    public abstract partial class BaseSharePointService : ISharePointService
    {
        private const string RequestErrorMessage = "Error sending request to SharePoint service";
        private const string ThrottledExceptionTypeName = "Microsoft.SharePoint.SPQueryThrottledException";

        private readonly ConfidentialApplicationService _confidentialService;
        private readonly SharePointOptions _sharePointOptions;
        private readonly IPlatformMemoryCache _memoryCache;
        private readonly ISettingsManager _settingsManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;
        private readonly string _siteUrl;

        private static readonly ConcurrentDictionary<string, string> _modelCache = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _subModelCache = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _ataCache = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _effectivityCache = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _workingGroupCache = new ConcurrentDictionary<string, string>();

        private static readonly Expression<Func<SP.ListItem, object>>[] _listItemRetrievals =
        {
            item => item.Id,
            item => item.DisplayName,
            item => item.ContentType.Name,
            item => item.ContentType.StringId,
            item => item.File.UniqueId,
            item => item.File.Name,
            item => item.File.Length,
            item => item.FileSystemObjectType,
             item => item["Title"],
            item => item["FileRef"],
            item => item["FileDirRef"],
            item => item["Created"],
            item => item["Modified"],
            item => item["Author"],
            item => item["Editor"],
            item => item["Description"]

        };

        private static readonly IList<Library> _libraries = new[]
        {
            new Library("service", "Q Series", "QSeries/Service Document"),
            new Library("operator-message", "Q Series", "QSeries/Service Document", "All Operator Message (AOM) Document Set"),
            new Library("manual-status", "Q Series", "QSeries/Technical Publication", "Publication or Manual Document Set"),
            new Library("service-bulletin", "Q Series", "QSeries/Technical Publication", "Service Bulletin Document Set"),
            new Library("engineering", "Q Series", "QSeries/Engineering Document"),
            new Library("engineering-aircraft-databases", "Q Series", "QSeries/Engineering Document", "Engineering and Aircraft Databases Document Set"),
            new Library("action-register", "Q Series", "QSeries/Engineering Document", "Action Register Document Set"),
            new Library("technical-newsletter", "Q Series", "QSeries/Technical Newsletter"),
            new Library("training-video", "Q Series", "QSeries/Generic Document", "Training Generic Document Document Set"),
            new Library("event", "Q Series", "QSeries/Generic Document", "Conference or Trade show Documentation Document Set"),
            new Library("supplier", "Q Series", "QSeries/Suppliers Document")
        };

        private static readonly string[] _keywordRefiners =
        {
            "RefineListID",
            "ContentTypeName"
        };

        private static readonly Regex _errorKeywordRegex = new Regex(@"((?:Path:)|(?:ListID:)|(?:^\s*OR\s+))+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly IList<string> _defaultSerialNumbers = new[] { "0" };

        protected BaseSharePointService(
            ConfidentialApplicationService confidentialService,
            IOptions<SharePointOptions> sharePointOptions,
            IPlatformMemoryCache memoryCache,
            ISettingsManager settingsManager,
            IEventPublisher eventPublisher,
            ILoggerFactory loggerFactory)
        {
            _confidentialService = confidentialService;
            _sharePointOptions = sharePointOptions.Value;
            _siteUrl = _sharePointOptions.SiteUrl;
            _settingsManager = settingsManager;
            _eventPublisher = eventPublisher;
            _memoryCache = memoryCache;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        protected virtual Expression<Func<SP.ListItem, object>>[] ListItemRetrievals => _listItemRetrievals;
        protected virtual IList<Library> Libraries => _libraries;

        public async Task<Result<string>> AcquireTokenAsync()
        {
            if (string.IsNullOrEmpty(_siteUrl))
            {
                return Result.Failed<string>("url_empty", "SharePoint site URL is not set.");
            }

            try
            {
                var uri = new Uri(_siteUrl);

                // For SharePoint app only auth, the scope will be the Sharepoint tenant name followed by /.default
                var scopes = new[] { $"https://{uri.Authority}/.default" };

                var clientApp = _confidentialService.GetClientApplication();
                var authResult = await clientApp.AcquireTokenForClient(scopes).ExecuteAsync();

                return Result.Success(authResult.AccessToken);
            }
            catch (Exception ex)
            {
                const string message = "Error acquiring a token from the authority configured in the app";
                return GetExceptionResult<string>(ex, _logger, message);
            }
        }

        public async Task<Result<SpWeb>> GetSiteTitleAsync()
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetSiteTitleAsync));
            try
            {
                return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
                {
                    cacheEntry.AddExpirationToken(SharePointCacheRegion.CreateChangeToken());
                    cacheEntry.AbsoluteExpirationRelativeToNow = _sharePointOptions.LongCacheAbsoluteExpiration;

                    var tokenResult = await AcquireTokenAsync();
                    if (!tokenResult.Succeeded)
                    {
                        throw ResultException.Create(tokenResult);
                    }

                    using var ctx = GetClientContextWithAccessToken(_siteUrl, tokenResult.Value);

                    ctx.Load(ctx.Web, x => x.Id, x => x.Title, x => x.Description, x => x.Url, x => x.ServerRelativeUrl);
                    await ctx.ExecuteQueryAsync();

                    return Result.Success(new SpWeb
                    {
                        Id = ctx.Web.Id,
                        Title = ctx.Web.Title,
                        Description = ctx.Web.Description,
                        Url = ctx.Web.Url,
                        ServerRelativeUrl = ctx.Web.ServerRelativeUrl,
                        SchemeVersion = _sharePointOptions.SchemeVersion,
                        CheckNotificationSent = _sharePointOptions.CheckNotificationSent,
                        CheckSerialNumbers = _sharePointOptions.CheckSerialNumbers
                    });
                });
            }
            catch (Exception ex)
            {
                return GetExceptionResult<SpWeb>(ex, _logger);
            }
        }

        public async Task<Result<IList<SpList>>> GetListsAsync()
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetListsAsync));
            try
            {
                return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
                {
                    cacheEntry.AddExpirationToken(SharePointCacheRegion.CreateChangeToken());
                    cacheEntry.AbsoluteExpirationRelativeToNow = _sharePointOptions.LongCacheAbsoluteExpiration;

                    var tokenResult = await AcquireTokenAsync();
                    if (!tokenResult.Succeeded)
                    {
                        throw ResultException.Create(tokenResult);
                    }

                    using var ctx = GetClientContextWithAccessToken(_siteUrl, tokenResult.Value);

                    var lists = await GetListsInternal(ctx);
                    return Result.Success(lists);
                });
            }
            catch (Exception ex)
            {
                return GetExceptionResult<IList<SpList>>(ex, _logger);
            }
        }

        protected async Task<Result<IList<SpList>>> GetListsAsync(SP.ClientContext ctx)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetListsAsync));
            try
            {
                return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
                {
                    cacheEntry.AddExpirationToken(SharePointCacheRegion.CreateChangeToken());
                    cacheEntry.AbsoluteExpirationRelativeToNow = _sharePointOptions.LongCacheAbsoluteExpiration;

                    var lists = await GetListsInternal(ctx);
                    return Result.Success(lists);
                });
            }
            catch (Exception ex)
            {
                return GetExceptionResult<IList<SpList>>(ex, _logger);
            }
        }

        private static async Task<IList<SpList>> GetListsInternal(SP.ClientContext ctx)
        {
            var lists = ctx.LoadQuery(SP.ClientObjectQueryableExtension.Include(
                ctx.Web.Lists.Where(x => !x.Hidden),
                x => x.Id, x => x.Title));
            await ctx.ExecuteQueryAsync();

            var result = lists.Select(x => new SpList { Id = x.Id, Title = x.Title }).ToArray();

            return result;
        }

        public async Task<Result<IList<Library>>> GetLibrariesAsync()
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetLibrariesAsync));
            try
            {
                return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
                {
                    cacheEntry.AddExpirationToken(SharePointCacheRegion.CreateChangeToken());
                    cacheEntry.AbsoluteExpirationRelativeToNow = _sharePointOptions.LongCacheAbsoluteExpiration;

                    IList<Library> libraries = Libraries?.ToArray();
                    if (!(libraries?.Count > 0))
                    {
                        return Result.Success(libraries);
                    }

                    var tokenResult = await AcquireTokenAsync();
                    if (!tokenResult.Succeeded)
                    {
                        throw ResultException.Create(tokenResult);
                    }

                    using var ctx = GetClientContextWithAccessToken(_siteUrl, tokenResult.Value);

                    return await GetLibrariesInternal(ctx, libraries);
                });
            }
            catch (Exception ex)
            {
                return GetExceptionResult<IList<Library>>(ex, _logger);
            }
        }

        protected async Task<Result<IList<Library>>> GetLibrariesAsync(SP.ClientContext ctx)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetLibrariesAsync));
            try
            {
                return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
                {
                    cacheEntry.AddExpirationToken(SharePointCacheRegion.CreateChangeToken());
                    cacheEntry.AbsoluteExpirationRelativeToNow = _sharePointOptions.LongCacheAbsoluteExpiration;

                    IList<Library> libraries = Libraries?.ToArray();
                    return !(libraries?.Count > 0)
                        ? Result.Success(libraries)
                        : await GetLibrariesInternal(ctx, libraries);
                });
            }
            catch (Exception ex)
            {
                return GetExceptionResult<IList<Library>>(ex, _logger);
            }
        }

        private async Task<Result<IList<Library>>> GetLibrariesInternal(SP.ClientContext ctx, IList<Library> libraries)
        {
            var listsResult = await GetListsAsync(ctx);
            if (!listsResult.Succeeded)
            {
                return Result.Failed<IList<Library>>(listsResult.Errors);
            }

            foreach (var grp in libraries.GroupBy(x => x.ListTitle, StringComparer.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(grp.Key))
                {
                    continue;
                }

                var list = listsResult.Value?.FirstOrDefault(x => x.Title.EqualsInvariant(grp.Key));
                if (list == null)
                {
                    continue;
                }

                foreach (var library in grp)
                {
                    library.ListId = list.Id.ToString("D");
                }
            }

            return Result.Success(libraries);
        }

        public async Task<Result<SpFileResult>> GetFileAsync(Guid id)
        {
            try
            {
                var tokenResult = await AcquireTokenAsync();
                if (!tokenResult.Succeeded)
                {
                    throw ResultException.Create(tokenResult);
                }

                using var ctx = GetClientContextWithAccessToken(_siteUrl, tokenResult.Value);

                var file = ctx.Web.GetFileById(id);
                ctx.Load(file, x => x.Name, x => x.TimeLastModified, x => x.ETag, x => x.Length);

                var stream = file.OpenBinaryStream();

                await ctx.ExecuteQueryAsync();

                var resultStream = new MemoryStream((int)file.Length);
                await stream.Value.CopyToAsync(resultStream);
                resultStream.Seek(0L, SeekOrigin.Begin);

                return Result.Success(new SpFileResult
                {
                    Name = file.Name,
                    ContentType = MimeTypes.GetMimeType(file.Name),
                    LastModified = file.TimeLastModified,
                    ETag = file.ETag,
                    Stream = resultStream
                });
            }
            catch (Exception ex)
            {
                return GetExceptionResult<SpFileResult>(ex, _logger);
            }
        }

        public async Task<Result<ListItemSearchResult>> SearchListItemsAsync(ListItemSearchCriteria criteria)
        {            
            var cacheKey = CacheKey.With(GetType(), nameof(SearchListItemsAsync), criteria.GetCacheKey());
            try
            {
                return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
                {
                    cacheEntry.AddExpirationToken(SharePointCacheRegion.CreateChangeToken());
                    cacheEntry.AbsoluteExpirationRelativeToNow = _sharePointOptions.CacheAbsoluteExpiration;

                    var tokenResult = await AcquireTokenAsync();
                    if (!tokenResult.Succeeded)
                    {
                        throw ResultException.Create(tokenResult);
                    }

                    using var ctx = GetClientContextWithAccessToken(_siteUrl, tokenResult.Value);

                    return await SearchListItemsInternal(ctx, criteria);
                });
            }
            catch (Exception ex)
            {
                return GetExceptionResult<ListItemSearchResult>(ex, _logger);
            }
        }

        private async Task<Result<ListItemSearchResult>> SearchListItemsInternal(SP.ClientContext ctx, ListItemSearchCriteria searchCriteria)
        {
            var result = new ListItemSearchResult();

            var criteria = await PrepareSearchCriteria(ctx, searchCriteria);

            if (criteria.IsError)
            {
                return Result.Success(result);
            }

            var list = ctx.Web.Lists.GetById(new Guid(criteria.Id));
            var folderRelativeUrl = await GetRelativeUrl(ctx, criteria.FolderRelativeUrl);

            var query = new SP.CamlQuery
            {
                FolderServerRelativeUrl = folderRelativeUrl,
                AllowIncrementalResults = true
            };

            var conditions = ParseCriteria(criteria);
            var orderFields = ParseSort(criteria.Sort);
            result.Sort = criteria.Sort;

            var viewXml = new View(Condition.ToElement(conditions), null, OrderBy.ToElement(orderFields), criteria.PageSize);
            query.ViewXml = viewXml.ToString();

            if (!string.IsNullOrWhiteSpace(criteria.PageInfo))
            {
                query.ListItemCollectionPosition = new SP.ListItemCollectionPosition { PagingInfo = criteria.PageInfo };
            }

            var retry = false;
            var pageCount = 0;
            while (true)
            {
                try
                {
                    var items = list.GetItems(query);

                    ctx.Load(items, x => x.ListItemCollectionPosition,
                        collection => SP.ClientObjectQueryableExtension.Include(collection, ListItemRetrievals)
                    );
                    await ctx.ExecuteQueryAsync();

                    result.NextPageInfo = items.ListItemCollectionPosition?.PagingInfo;

                    foreach (var item in items)
                    {
                        var listItem = ToListItem(item);

                        result.Results.Add(listItem);
                    }

                    // The results may not be on the first page
                    if (items.Count != 0 || items.ListItemCollectionPosition == null || ++pageCount > 3)
                    {
                        break;
                    }
                    query.ListItemCollectionPosition = items.ListItemCollectionPosition;
                    retry = false;
                }
                // The attempted operation is prohibited because it exceeds the list view threshold.
                catch (SP.ServerException ex) when (ex.ServerErrorTypeName == ThrottledExceptionTypeName)
                {
                    // Just one retry
                    if (retry)
                    {
                        throw;
                    }
                    // Sorting does not work on too large lists
                    result.Sort = null;
                    viewXml = new View(viewXml.Where, null, null, viewXml.PageSize);
                    query.ViewXml = viewXml.ToString();
                    retry = true;
                }
            }

            if (!criteria.Child && result.TotalCount == 0)
            {
                result.TotalCount = result.Results.Count;
            }
             return Result.Success(result);
            
        }



        private static async Task<ListItemSearchCriteria> PrepareSearchCriteria(SP.ClientContext ctx, ListItemSearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                return null;
            }

            var criteria = searchCriteria;


            if (!string.IsNullOrEmpty(criteria.ItemRelativeUrl))
            {
                criteria.ItemRelativeUrl = await GetRelativeUrl(ctx, criteria.ItemRelativeUrl);
            }


            if (criteria.SearchPhrase?.Count > 0)
            {
                string model = null;

                if (!string.IsNullOrEmpty(model))
                {
                    criteria.IsError = true;
                }
            }

            return criteria;
        }

        private static async Task<string> GetRelativeUrl(SP.ClientContext ctx, string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                return null;
            }

            if (relativeUrl.StartsWith('/'))
            {
                return relativeUrl;
            }

            ctx.Load(ctx.Web, x => x.ServerRelativeUrl);
            await ctx.ExecuteQueryAsync();

            return $"{ctx.Web.ServerRelativeUrl}/{relativeUrl}";
        }
        protected virtual IList<Condition> ParseCriteria(ListItemSearchCriteria criteria)
        {
            var result = new List<Condition>();
            // Default condition for list
            if (criteria == null)
            {
                return result;
            }

            if (!string.IsNullOrEmpty(criteria.ItemRelativeUrl))
            {
                result.Add(new Eq("FileRef", ValueType.Text, criteria.ItemRelativeUrl));
            }

            if (criteria.SearchPhrase?.Count > 0)
            {
                foreach (var (key, value) in criteria.SearchPhrase)
                {
                    AddCondition(result, key, value);
                }
            }

            return result;
        }
        protected virtual IList<OrderField> ParseSort(string sort)
        {
            var result = new List<OrderField>();

            if (string.IsNullOrEmpty(sort))
            {
                return result;
            }


            return result;
        }
        protected virtual SpListItem ToListItem(SP.ListItem item)
        {
            var listItem = new SpListItem
            {
                Id = item.Id,
                DisplayName = item.DisplayName,
                ContentType = item.ContentType.Name,
                ContentTypeId = item.ContentType.StringId,
                Title = Convert.ToString(item["Title"]),
                FolderRelativeUrl = Convert.ToString(item["FileDirRef"]),
                Created = Convert.ToDateTime(item["Created"]),
                Modified = Convert.ToDateTime(item["Modified"]),
                CreatedBy = GetUserValue(item, "Author"),
                ModifiedBy = GetUserValue(item, "Editor"),
                Description = Convert.ToString(item["Description"])
            };

            if (item.File?.ServerObjectIsNull == false)
            {
                listItem.FileId = item.File.UniqueId;
                listItem.FileName = item.File.Name;
                listItem.FileContentType = listItem.ContentType;
                listItem.FileLength = item.File.Length;
                if (string.IsNullOrEmpty(listItem.Title))
                {
                    listItem.Title = listItem.DocumentSetTitle;
                }
            }
            else if (string.IsNullOrEmpty(listItem.DocumentSetTitle))
            {
                listItem.DocumentSetTitle = listItem.Title;
            }

            return listItem;
        }
        private static string GetUserValue(SP.ListItem item, string fieldName) =>
            (item[fieldName] as SP.FieldUserValue)?.LookupValue;

        private static string GetLookupValue(SP.ListItem item, string fieldName)
        {
            var value = item[fieldName];
            switch (value)
            {
                case SP.FieldLookupValue lookupValue when !string.IsNullOrEmpty(lookupValue.LookupValue):
                    return GetLookupValue(lookupValue.LookupValue, fieldName);
                case SP.FieldLookupValue[] lookupValues:
                    {
                        var values = lookupValues
                            .Select(x => x.LookupValue)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToArray();
                        return values.Length == 0 ? null : string.Join("; ", values.Select(name => GetLookupValue(name, fieldName)));
                    }
                case string textValue when !string.IsNullOrEmpty(textValue):
                    return GetLookupValue(textValue, fieldName);
            }

            return null;
        }

        private static string GetLookupValue(string name, string fieldName)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(fieldName))
            {
                return name;
            }

            switch (fieldName)
            {
                default:
                    return name;
            }
        }


        protected virtual void AddCondition(IList<Condition> conditions, string fieldName, string value)
        {
            if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(value))
            {
                return;
            }

            switch (fieldName)
            {
                case "ContentType":
                    conditions.Add(new Eq(20, fieldName, ValueType.Text, value));
                    break;
                case "MainATASNS":
                case "SubATASNS":
                case "ModelNumber":
                case "SubModel":
                    conditions.Add(new Includes(21, fieldName, ValueType.LookupMulti, value));
                    break;
                case "WorkingGroup":
                case "Supplier":
                    conditions.Add(new Eq(22, fieldName, ValueType.Lookup, value));
                    break;
                case "AvailableOnPortal":
                case "Status1":
                case "ETOPS":
                case "Criticality":
                    conditions.Add(new Eq(23, fieldName, ValueType.Choice, value));
                    break;
                case "DateFrom":
                    conditions.Add(new Geq(50, "ReleaseDate", ValueType.DateTime, value, false));
                    break;
                case "DateTo":
                    conditions.Add(new Leq(51, "ReleaseDate", ValueType.DateTime, value, false));
                    break;
                case "KeywordValues":
                    var orConditions = new List<Condition>
                    {
                        new Contains("KeywordValues", ValueType.Text, value),
                        new Contains("Title", ValueType.Text, value),
                    };
                    conditions.Add(new Or(11, orConditions));
                    break;
                default:
                    conditions.Add(new Contains(40, fieldName, ValueType.Text, value));
                    break;
            }
        }

        private static SP.ClientContext GetClientContextWithAccessToken(string siteUrl, string accessToken)
        {
            var clientContext = new SP.ClientContext(siteUrl);
            clientContext.ExecutingWebRequest +=
                (sender, e) => e.WebRequestExecutor.RequestHeaders["Authorization"] = $"Bearer {accessToken}";
            clientContext.ValidateOnClient = true;

            return clientContext;
        }
        private static Result<T> GetExceptionResult<T>(Exception ex, ILogger logger, string message = RequestErrorMessage)
        {
            if (ex is ResultException resultException)
            {
                return Result.Failed<T>(resultException.Errors);
            }

            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            logger.LogError(ex, message);
            return Result.Failed<T>("exception", $"{message}. {ex.ExpandExceptionMessage()}");
        }
    }
    
}
