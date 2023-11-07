using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;
using Microsoft.SharePoint.Client;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;
using StackExchange.Redis;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace P5.SharePoint.Data.Schemas
{
    public class SharePointListsResponseType : ObjectGraphType<Result<IList<SpList>>>
    {
        public SharePointListsResponseType()
        {
            Field(x => x.Succeeded);
            Field<ListGraphType<SpListType>>("result", resolve: context => context.Source.Value);
            Field<ListGraphType<ResultErrorType>>("spList", resolve: context => context.Source.Errors);
        }
    }       
}
