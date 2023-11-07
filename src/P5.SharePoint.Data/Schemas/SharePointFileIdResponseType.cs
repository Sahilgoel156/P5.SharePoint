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
    public class SharePointFileIdResponseType : ObjectGraphType<Result<SpFileResult>>
    {
        public SharePointFileIdResponseType()
        {
            Field(x => x.Value.Name);
            Field(x => x.Value.ContentType);
            Field(x => x.Value.ETag);
        }
    }
}
