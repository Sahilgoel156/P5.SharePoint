using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Data.Query
{
    public class Contains : Condition
    {
        public Contains(int sortOrder, string fieldName, ValueType type, string value)
            : base(nameof(Contains), sortOrder, fieldName, type, value)
        {
        }

        public Contains(string fieldName, ValueType type, string value)
            : this(0, fieldName, type, value)
        {
        }
    }
}
