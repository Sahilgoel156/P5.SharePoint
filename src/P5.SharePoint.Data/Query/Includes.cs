using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Data.Query
{
    public class Includes : Condition
    {
        public Includes(int sortOrder, string fieldName, ValueType type, string value)
            : base(nameof(Includes), sortOrder, fieldName, type, value)
        {
        }

        public Includes(string fieldName, ValueType type, string value)
            : this(0, fieldName, type, value)
        {
        }
    }
}
