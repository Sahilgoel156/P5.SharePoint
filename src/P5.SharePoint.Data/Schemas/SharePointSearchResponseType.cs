using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;
using P5.SharePoint.Core.Models.Search;
using VirtoCommerce.Platform.Core.Extensions;

namespace P5.SharePoint.Data.Schemas
{
    public class SharePointSearchResponseType : ObjectGraphType<Result<ListItemSearchResult>>
    {
        public SharePointSearchResponseType()
        {
            Field<ListGraphType<SpListItemType>>("result", resolve: context => context.Source.Value.Results);
            Field<ListGraphType<ResultErrorType>>("spList", resolve: context => context.Source.Errors);
            Field(x => x.Value.NextPageInfo);
            Field(x => x.Value.ItemCount);
            Field(x => x.Value.TotalCount);
            Field(x => x.Value.Sort);
        }
    }
}
