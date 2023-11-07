using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.Xml.Linq;

namespace P5.SharePoint.Data.Query
{
    public class OrderField : Field
    {
        public OrderField(string fieldName, bool? ascending = default)
            : base(fieldName)
        {
            Ascending = ascending;
        }

        public bool? Ascending { get; }

        public override XElement ToElement()
        {
            var element = base.ToElement();
            if (Ascending.HasValue)
            {
                element.SetAttributeValue("Ascending", Ascending.Value);
            }
            return element;
        }
    }
}
