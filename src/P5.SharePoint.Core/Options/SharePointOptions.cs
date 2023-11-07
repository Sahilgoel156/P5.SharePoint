using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Core.Options
{
    public class SharePointOptions
    {
        public const string SectionName = "Integration:SharePoint";
        public string SiteUrl { get; set; }
        public SchemeVersion SchemeVersion { get; set; } = SchemeVersion.V1;
        public bool CheckNotificationSent { get; set; } = false;
        public bool CheckSerialNumbers { get; set; } = false;
        public string StoreId { get; set; }
        public TimeSpan CacheAbsoluteExpiration { get; set; } = TimeSpan.FromMinutes(10);
        public TimeSpan LongCacheAbsoluteExpiration { get; set; } = TimeSpan.FromHours(1);
    }
}
