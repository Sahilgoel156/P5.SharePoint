using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Data.Query;

namespace P5.SharePoint.Data.Schemas
{
    public class SpListType : ObjectGraphType<SpList>
    {
        public SpListType()
        {
            Field(x => x.Id);
            Field(x => x.Title);
        }
        
    }
}
