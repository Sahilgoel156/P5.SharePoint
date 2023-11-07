using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client.Publishing;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;

namespace P5.SharePoint.Core.Models.Search
{
    public class ListItemSearchCriteria : ValueObject
    {
        public int PageSize { get; set; } = 30;
        public string PageInfo { get; set; }
        public string ListTitle { get; set; }
        public string FolderRelativeUrl { get; set; }
        public string ItemRelativeUrl { get; set; }
        public bool Child { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Sort { get; set; }
        public IDictionary<string, string> SearchPhrase { get; set; }
        public string siteId { get; set; }
        public string Id { get; set; }
        [JsonIgnore]
        public string OperatorCode { get; set; }
        [JsonIgnore]
        public IList<string> SerialNumbers { get; set; }
        [JsonIgnore]
        public bool IsError { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PageSize;
            yield return PageInfo;
            yield return ListTitle;
            yield return FolderRelativeUrl;
            yield return ItemRelativeUrl;
            yield return Child;
            yield return DateFrom;
            yield return DateTo;
            yield return Sort;
            yield return OperatorCode;
            yield return siteId;
            yield return Id;



            if (SearchPhrase?.Count > 0)
            {
                yield return '[';
                foreach (var (key, value) in SearchPhrase.OrderBy(x => x.Key))
                {
                    yield return key;
                    yield return value;
                }
                yield return ']';
            }
        }

        protected virtual IEnumerable<object> GetCountEqualityComponents()
        {
            yield return ListTitle;
            yield return FolderRelativeUrl;
            yield return ItemRelativeUrl;
            yield return DateFrom;
            yield return DateTo;
            yield return OperatorCode;


            if (SearchPhrase?.Count > 0)
            {
                yield return '[';
                foreach (var (key, value) in SearchPhrase.OrderBy(x => x.Key))
                {
                    yield return key;
                    yield return value;
                }
                yield return ']';
            }
        }

        public virtual string GetCountCacheKey()
        {
            var keyValues = GetCountEqualityComponents()
                .Select(x => x is string ? $"'{x}'" : x)
                .Select(x => x is ICacheKey cacheKey ? cacheKey.GetCacheKey() : x?.ToString());

            return string.Join("|", keyValues);
        }
    }
}
