using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using VirtoCommerce.Platform.Core.Common;

namespace P5.SharePoint.Core.Models.Search
{
    public class ListItemSearchResult : GenericSearchResult<SpListItem>
    {
        public int ItemCount { get; set; }
        public string NextPageInfo { get; set; }
        public string Sort { get; set; }
    }
}
