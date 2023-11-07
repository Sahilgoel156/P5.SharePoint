using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace P5.SharePoint.Data.Query
{
    public class View
    {
        public const int MaxPageSize = 4900;

        public View(XElement where, XElement groupBy, XElement orderBy, int pageSize = MaxPageSize, bool recursiveAll = false)
        {
            Where = where;
            GroupBy = groupBy;
            OrderBy = orderBy;
            PageSize = pageSize;
            RecursiveAll = recursiveAll;
        }

        public XElement Where { get; }
        public XElement GroupBy { get; }
        public XElement OrderBy { get; }
        public int PageSize { get; }
        public bool RecursiveAll { get; }

        public XElement ToElement()
        {
            return new XElement("View",
                RecursiveAll ? new XAttribute("Scope", "RecursiveAll") : null,
                new XElement("Query",
                    Where,
                    GroupBy,
                    OrderBy
                ),
                new XElement("RowLimit",
                    new XAttribute("Paged", "TRUE"),
                    PageSize)
            );
        }

        public override string ToString()
        {
            return ToElement().ToString();
        }
    }
}
