using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P5.SharePoint.Core.Options;

namespace P5.SharePoint.Core.Models
{
    public class SpWeb
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ServerRelativeUrl { get; set; }
        public SchemeVersion SchemeVersion { get; set; }
        public bool CheckNotificationSent { get; set; }
        public bool CheckSerialNumbers { get; set; }
    }
}
