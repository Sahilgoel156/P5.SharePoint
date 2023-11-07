using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Data.Query
{
    public class IsNull : Condition
    {
        public IsNull(int sortOrder, string fieldName)
            : base(nameof(IsNull), sortOrder, fieldName, null, null)
        {
        }

        public IsNull(string fieldName)
            : this(0, fieldName)
        {
        }
    }
}
