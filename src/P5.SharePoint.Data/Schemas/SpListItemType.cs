using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;
using P5.SharePoint.Core.Models;
using Polly;

namespace P5.SharePoint.Data.Schemas
{
    public class SpListItemType : ObjectGraphType<SpListItem>
    {
        public SpListItemType()
        {
            Field<NonNullGraphType<IntGraphType>>("id",resolve: Context => Context.Source.Id);
            Field(x => x.Supplier);
            Field(x => x.Effectivity);
            Field(x => x.WorkingGroup);
            Field(x => x.Title);
            Field(x => x.FileName);
            Field(x => x.DisplayName);
            Field(x => x.Description);
            Field(x => x.ContentType);
            Field(x => x.ModifiedBy);
            Field<NonNullGraphType<DateTimeGraphType>>("Modified", resolve: Context => Context.Source.Modified);
            Field<NonNullGraphType<DateTimeGraphType>>("Created", resolve: Context => Context.Source.Created);
            Field(x => x.CreatedBy);
            Field < NonNullGraphType<StringGraphType>>("FileId", resolve: Context => Context.Source.FileId.ToString());

            

                /* Field<NonNullGraphType<DateTimeGraphType>>("ModifiedBy", resolve: Context => Context.Source.ModifiedBy);
                 Field<NonNullGraphType<DateTimeGraphType>>("Modified", resolve: Context => Context.Source.Modified);
                 Field<NonNullGraphType<DateTimeGraphType>>("Created", resolve: Context => Context.Source.Created);
                 Field<NonNullGraphType<DateTimeGraphType>>("CreatedBy", resolve: Context => Context.Source.CreatedBy);
                 Field(x => x.Description);*/
        }
    }
}
