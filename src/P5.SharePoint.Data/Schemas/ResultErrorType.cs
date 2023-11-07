using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;
using P5.SharePoint.Core.Models.Common;

namespace P5.SharePoint.Data.Schemas
{
    public class ResultErrorType : ObjectGraphType<ResultError>
    {
        public ResultErrorType()
        {
            Field(x => x.Code);
            Field(x => x.Description);
        }
    }
}
