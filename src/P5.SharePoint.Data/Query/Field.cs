using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace P5.SharePoint.Data.Query
{
    public class Field
    {
        public Field(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; }

        public virtual XElement ToElement() =>
            new XElement("FieldRef",
                new XAttribute("Name", FieldName)
            );
    }
}
