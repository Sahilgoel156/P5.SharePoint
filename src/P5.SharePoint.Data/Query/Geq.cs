using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Data.Query
{
    public class Geq : Condition
    {
        public Geq(int sortOrder, string fieldName, ValueType type, string value, bool? includeTimeValue = default)
            : base(nameof(Geq), sortOrder, fieldName, type, value, includeTimeValue)
        {
        }

        public Geq(string fieldName, ValueType type, string value, bool? includeTimeValue = default)
            : this(0, fieldName, type, value, includeTimeValue)
        {
        }
    }
}
