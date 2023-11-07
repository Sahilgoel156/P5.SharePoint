using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;
using P5.SharePoint.Core.Models;

namespace P5.SharePoint.Data.Schemas
{
    public class ListItemSearchResultType : ObjectGraphType<SpListItem>
    {
        public ListItemSearchResultType()
        {
            Field<ListGraphType<SpListItemType>>("results", resolve: Context => Context.Source);
            Field(x => x.Supplier);
            Field(x => x.WorkingGroup);
            Field(x => x.Effectivity);
            Field(x => x.ModifiedBy);
            Field(x => x.Modified);
            Field(x => x.Created);
            Field(x => x.CreatedBy);
            Field(x => x.Description);
        }
    }
}
