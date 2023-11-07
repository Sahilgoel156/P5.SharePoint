using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using P5.SharePoint.Data.Extensions;

namespace P5.SharePoint.Data.Query
{
    public abstract class Condition
    {
        protected Condition(string name, int sortOrder, XElement element)
        {
            Name = name;
            SortOrder = sortOrder;
            Element = element;
        }

        protected Condition(string name, int sortOrder, string fieldName, ValueType? type, string value, bool? includeTimeValue = default)
        {
            Name = name;
            SortOrder = sortOrder;
            FieldName = fieldName;
            Type = type;
            Value = value;
            IncludeTimeValue = includeTimeValue;
        }

        public string Name { get; }
        public int SortOrder { get; }
        public XElement Element { get; }
        public string FieldName { get; }
        public ValueType? Type { get; }
        public string Value { get; }
        public bool? IncludeTimeValue { get; }

        public virtual XElement ToElement() =>
            Element ??
            new XElement(Name,
                new XElement("FieldRef",
                    new XAttribute("Name", FieldName)
                ),
                string.IsNullOrEmpty(Value) ? null : new XElement("Value",
                    IncludeTimeValue.HasValue ? new XAttribute("IncludeTimeValue", IncludeTimeValue.Value) : null,
                    Type.HasValue ? new XAttribute("Type", Type.Value.ToString("G")) : null,
                    Value)
            );

        public static XElement ToElement(IList<Condition> conditions)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return null;
            }

            var result = conditions.OrderBy(x => x.SortOrder)
                .Select(x => x.ToElement()).Split(2)
                .Aggregate(default(XElement), (current, elements) =>
                {
                    var element = elements.Length == 2 ? new XElement("And", elements[0], elements[1]) : elements[0];
                    return current == null ? element : new XElement("And", current, element);
                });

            return new XElement("Where", result);
        }
    }
}
