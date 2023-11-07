using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using P5.SharePoint.Data.Extensions;

namespace P5.SharePoint.Data.Query
{
    public class Or : Condition
    {
        public Or(int sortOrder, ICollection<Condition> conditions)
            : base(nameof(Or), sortOrder, GetElement(conditions))
        {
        }

        public Or(ICollection<Condition> conditions)
            : this(0, conditions)
        {
        }

        private static XElement GetElement(ICollection<Condition> conditions)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return null;
            }

            return conditions.Select(x => x.ToElement()).Split(2)
                .Aggregate(default(XElement), (current, elements) =>
                {
                    var element = elements.Length == 2 ? new XElement(nameof(Or), elements[0], elements[1]) : elements[0];
                    return current == null ? element : new XElement(nameof(Or), current, element);
                });
        }
    }
}
