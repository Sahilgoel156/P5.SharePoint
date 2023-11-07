using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Options;
using StackExchange.Redis;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Settings;
using SP = Microsoft.SharePoint.Client;

namespace P5.SharePoint.Data.Services.Schemas.V2
{
    public partial class SharePointService : BaseSharePointService
    {
        private readonly SharePointOptions _sharePointOptions;

        private static readonly Expression<Func<SP.ListItem, object>>[] _listItemRetrievals =
        {
            item => item["ContainerDocumentSetName"],
            item => item["OriginalFileContentType"]
        };

        private static readonly IList<Library> _libraries = new[]
        {
            new Library("service", "Q Series Service"),
            new Library("operator-message", "Q Series Service", null, "All Operator Message (AOM) Document Set"),
            new Library("manual-status", "Q Series Technical Publication", null, "Publication or Manual Document Set"),
            new Library("service-bulletin", "Q Series Technical Publication", null, "Service Bulletin Document Set"),
            new Library("engineering", "Q Series Engineering"),
            new Library("engineering-aircraft-databases", "Q Series Engineering", null, "Engineering and Aircraft Databases Document Set"),
            new Library("action-register", "Q Series Engineering", null, "Action Register Document Set"),
            new Library("technical-newsletter", "Q Series Technical Newsletter"),
            new Library("training-video", "Q Series Generic", null, "Training Generic Document Document Set", true),
            new Library("event", "Q Series Generic", null, "Conference or Trade show Documentation Document Set", "Meeting Minutes and Presentations Document Set"),
            new Library("meeting-minutes-presentations", "Q Series Generic", null, "FlightOps Meeting Minutes Document Set", true),
            new Library("events", "Q Series Generic", null, "Events Document Set", true),
            new Library("sponsorship", "Q Series Generic", null, "Sponsorship Document Set", true),
            new Library("supplier", "Q Series Suppliers")
        };

        private static readonly Regex _errorKeywordRegex = new Regex(@"((?:Path:)|(?:ListID:)|(?:AvailableOnPortalOWSTEXT:)|(?:ModelNumber1OWSTEXT:)|(?:MSN:)|(?:^\s*OR\s+))+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly string[] _keywordSuggestionsProperties =
        {
            "SiteName",
            nameof(SpKeywordResult.Title),
            nameof(SpKeywordResult.Author),
            nameof(SpKeywordResult.LastModifiedTime),
            nameof(SpKeywordResult.Path),
            nameof(SpKeywordResult.ParentLink),
            "ContentTypeName", // alias "CTN"
            nameof(SpKeywordResult.UniqueId),
            nameof(SpKeywordResult.IsDocument),
            nameof(SpKeywordResult.IsContainer),
            nameof(SpKeywordResult.IsExternalContent)
        };

        private static readonly string[] _keywordSelectedProperties = _keywordSuggestionsProperties
            .Concat(new[]
            {
                nameof(SpKeywordResult.Description),
                nameof(SpKeywordResult.HitHighlightedSummary),
                nameof(SpKeywordResult.Size),
                "PictureThumbnailURL",
                nameof(SpKeywordResult.FileType),
                nameof(SpKeywordResult.FileExtension),
                nameof(SpKeywordResult.DocId),
                nameof(SpKeywordResult.ListId),
                "RefineListID",
                "ModelNumber",
            })
            .ToArray();


        public SharePointService(
            ConfidentialApplicationService confidentialService,
            IOptions<SharePointOptions> sharePointOptions,
            IPlatformMemoryCache memoryCache,
            ISettingsManager settingsManager,
            IEventPublisher eventPublisher,
            ILoggerFactory loggerFactory)
            : base(confidentialService, sharePointOptions, memoryCache, settingsManager, eventPublisher, loggerFactory)
        {
            _sharePointOptions = sharePointOptions.Value;
        }

    }
}
