using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Types;
using Microsoft.SharePoint.Client.UserProfiles;
using P5.SharePoint.Core.Models;

namespace P5.SharePoint.Data.Schemas
{
    public class LibraryType : ObjectGraphType<Library>
    {
        public LibraryType()
        {
            Field(x => x.Name);
            Field(x => x.FolderUrl);
            Field(x => x.ListTitle);
            Field(x => x.HideContentTypes);
        }
    }
}
