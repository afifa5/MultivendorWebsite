using MultivendorWebViewer.Common;
using MultivendorWebViewer.Helpers;
using MultivendorWebViewer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MultivendorWebViewer.ComponentModel;

namespace MultivendorWebViewer.Configuration
{
    public class HtmlAttributeCollection : ObjectDictionary<HtmlAttribute>, IAddToAttributes
    {
        public HtmlAttributeCollection() { }

        public HtmlAttributeCollection(IEnumerable<HtmlAttribute> items) : base(items) { }

        protected override string GetKeyForItem(HtmlAttribute item)
        {
            return item.Name;
        }

        public void AddToHtmlAttributes(object obj, IDictionary<string, object> htmlAttributes, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)
        {
            foreach (var attr in this)
            {
                if (attr.Name != null)
                {
                    string value = attr.GetValue(obj, formatterContext, formatProvider);
                    if (value != null)
                    {
                        htmlAttributes[attr.Name.StartsWith("data-", StringComparison.OrdinalIgnoreCase) ? attr.Name : "data-" + attr.Name] = value;
                    }
                }
            }
        }

        public AttributeBuilder AddToAttributes(AttributeBuilder attributeBuilder, object obj = null, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)
        {
            if (attributeBuilder == null) throw new NotImplementedException("AddToAttributes");// attributeBuilder = new AttributeBuilder();
            foreach (var attr in this)
            {
                if (attr.Name != null)
                {
                    string value = attr.GetValue(obj, formatterContext, formatProvider);
                    if (value != null)
                    {
                        attributeBuilder.DataAttr(attr.Name, value);
                    }
                }
            }
            return attributeBuilder;
        }

        public AttributeBuilder AddToAttributes(AttributeBuilder attributeBuilder, object obj = null, IFormatProvider formatProvider = null)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Defines a html attribute to ba added to a html element.
    /// </summary>
    public class HtmlAttribute
    {
        /* <example>&ltName&gtxyz&lt/Name&gt gives </example> */

        /// <summary>
        /// Name of the attribute.
        /// </summary>
        /// <value>Attribute name name</value>
        /// <example>name="xyz" gives an attribute named data-xyz</example>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Name of the objects property to get the value from, converted to string. The property name can be a path.
        /// </summary>
        /// <value>Attribute name property</value>
        /// <example>Quantity
        /// Part.PartNumber
        /// </example>
        [XmlAttribute("property")]
        public string PropertyName { get; set; }

        // TODO USE VALUE PROVIDER INSTEAD!

        /// <summary>
        /// If true, serialize the value to json.
        /// </summary>
        [XmlAttribute("json")]
        public bool Json { get; set; }

        public string GetValue(object obj, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)
        {
            var propertyGetter = PropertyGetter.Create(PropertyName, obj.GetType());
            var objectPropertyValue = propertyGetter != null ? propertyGetter.GetValue(obj) : null;
            if (objectPropertyValue == null) return null;
            if (Json == false)
            {
                //var objectPropertyValues = objectPropertyValue as IEnumerable<object>;
                //if (objectPropertyValues != null)
                //{
                //    objectPropertyValue = objectPropertyValues.FirstOrDefault();
                //}

                //if (objectPropertyValue == null) return null;

                //string objectPropertyValueString = objectPropertyValue as string ?? objectPropertyValue.ToString();
                //return objectPropertyValueString;
                return ValueProvider.GetFormattedValue(objectPropertyValue, null, formatterContext, formatProvider);
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(objectPropertyValue);
            }
        }
    }
}
