using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;
using P5.SharePoint.Core.Options;

namespace P5.SharePoint.Data.Schemas
{
    public class SharePointTitleResponseType : ObjectGraphType<Result<SpWeb>>
    {
        public SharePointTitleResponseType() {
            Field<NonNullGraphType<StringGraphType>>("id", resolve: Context => Context.Source.Value.Id);
            Field(x => x.Value.Title);
            Field(x => x.Value.Description);
            Field(x => x.Value.Url);
            Field(x => x.Value.ServerRelativeUrl);
            Field(x => x.Value.CheckNotificationSent);
            Field(x => x.Value.CheckSerialNumbers);
        }
    }
}
