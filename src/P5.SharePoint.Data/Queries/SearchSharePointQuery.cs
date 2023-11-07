using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;
using P5.SharePoint.Core.Models.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace P5.SharePoint.Data.Queries
{
    public class SearchSharePointQuery : IQuery<Result<ListItemSearchResult>>
    {
        public SearchSharePointQuery(string id)
        {
            Id = id;
        }
        public string Id { get; set; }

    }
}
