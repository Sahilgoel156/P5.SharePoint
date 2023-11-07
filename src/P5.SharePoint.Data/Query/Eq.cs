using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Data.Query
{
    public class Eq : Condition
    {
        public Eq(int sortOrder, string fieldName, ValueType type, string value, bool? includeTimeValue = default)
            : base(nameof(Eq), sortOrder, fieldName, type, value, includeTimeValue)
        {
        }

        public Eq(string fieldName, ValueType type, string value, bool? includeTimeValue = default)
            : this(0, fieldName, type, value, includeTimeValue)
        {
        }
    }
}
