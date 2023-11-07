using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Data.Query
{
    public class Leq : Condition
    {
        public Leq(int sortOrder, string fieldName, ValueType type, string value, bool? includeTimeValue = default)
            : base(nameof(Leq), sortOrder, fieldName, type, value, includeTimeValue)
        {
        }

        public Leq(string fieldName, ValueType type, string value, bool? includeTimeValue = default)
            : this(0, fieldName, type, value, includeTimeValue)
        {
        }
    }
}
