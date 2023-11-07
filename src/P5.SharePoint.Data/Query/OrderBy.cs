using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace P5.SharePoint.Data.Query
{
    public static class OrderBy
    {
        public static XElement ToElement(IList<OrderField> fields)
        {
            fields ??= new List<OrderField>();
            if (fields.Count == 0 || !fields.Any(x => x.FieldName.Equals("ID", StringComparison.InvariantCultureIgnoreCase)))
            {
                fields.Add(new OrderField("ID", true));
            }

            return new XElement("OrderBy",
                new XAttribute("Override", "TRUE"),
                fields.Select(x => x.ToElement()));
        }
    }
}
