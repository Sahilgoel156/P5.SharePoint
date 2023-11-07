using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Core.Models
{
    public class SpKeywordResult
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string HitHighlightedSummary { get; set; }
        public string Author { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public int? Size { get; set; }
        public string Path { get; set; }
        public string ParentLink { get; set; }
        public string PictureThumbnailUrl { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public long? DocId { get; set; }
        public string UniqueId { get; set; }
        public string ListId { get; set; }
        public string ContentType { get; set; }
        public bool? IsDocument { get; set; }
        public bool? IsContainer { get; set; }
        public bool? IsExternalContent { get; set; }
    }
}
