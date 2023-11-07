using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;

namespace P5.SharePoint.Data.Schemas
{
    public class SharePointLibraryResponseType : ObjectGraphType<Result<IList<Library>>>
    {
        public SharePointLibraryResponseType()
        {
            Field(x => x.Succeeded);
            Field<ListGraphType<LibraryType>>("result", resolve: context => context.Source.Value);
            Field<ListGraphType<ResultErrorType>>("spList", resolve: context => context.Source.Errors);
        }
    }
}
