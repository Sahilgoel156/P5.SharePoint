using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace P5.SharePoint.Data.Queries
{
    public class GetSharepointListsQuery : IQuery<Result<IList<SpList>>>
    {
        public GetSharepointListsQuery()
        {
        }
    }
}
