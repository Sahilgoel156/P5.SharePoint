using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Core.Models
{
    public class Library : ICloneable
    {
        public Library(string name, string listTitle, string folderUrl = null, string contentType = null, bool hideContentTypes = false)
        {
            Name = name;
            ListTitle = listTitle;
            FolderUrl = folderUrl;
            ContentTypes = !string.IsNullOrEmpty(contentType) ? new[] { contentType } : null;
            HideContentTypes = hideContentTypes;
        }

        public Library(string name, string listTitle, string folderUrl = null, params string[] contentTypes)
        {
            Name = name;
            ListTitle = listTitle;
            FolderUrl = folderUrl;
            ContentTypes = contentTypes;
            HideContentTypes = false;
        }

        public string Name { get; }
        public string ListTitle { get; }
        public string ListId { get; set; }
        public string FolderUrl { get; }
        public IList<string> ContentTypes { get; }
        public bool HideContentTypes { get; }

        public virtual object Clone() => MemberwiseClone();
    }
}
