using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Core.Models
{
    public class SpFileResult
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public DateTime? LastModified { get; set; }
        public string ETag { get; set; }
        public Stream Stream { get; set; }
    }
}
