using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Common
{
    public interface IValueProvider
    {
        object GetFormattedValue(object obj, FormatterContext formatterContext = null, IFormatProvider formatProvider = null);

        string GetFormattedValue(object obj, string multipleValuesSeparator, FormatterContext formatterContext = null, IFormatProvider formatProvider = null);

        object GetValue(object obj = null, FormatterContext formatterContext = null);
    }

    public abstract class ValueProviderBase : IValueProvider
    {
        public abstract object GetFormattedValue(object obj, FormatterContext formatterContext = null, IFormatProvider formatProvider = null);

        public abstract string GetFormattedValue(object obj, string multipleValuesSeparator, FormatterContext formatterContext = null, IFormatProvider formatProvider = null);

        public abstract object GetValue(object obj = null, FormatterContext formatterContext = null);
    }

    public class ValueProvider : ValueProviderBase
    {
        //[XmlAttribute("id")]
        //public string Id { get; set; }

        //[XmlAttribute("name")]
        //public string Name { get; set; }

        [XmlAttribute("property")]
        public string PropertyName { get; set; }

        //[XmlElement("Specification")]
        //public SpecificationIdentifier Specification { get; set; }

        //[XmlAttribute("specification")]
        //public string SpecificationCodeShorthand { get { return Specification != null ? Specification.Code : null; } set { Specification = value != null ? new SpecificationIdentifier { Code = value } : null; } }

        [XmlElement("Value")]
        public StaticValue StaticValue { get; set; }

        [XmlAttribute("value-separator")]
        public string MultiValueSeparator { get; set; }

        [XmlAttribute("html-class")]
        public string HtmlClass { get; set; }

        [XmlAttribute("html-tag")]
        public string HtmlTag { get; set; }

        [XmlIgnore]
        public int? MaxValues { get; set; }

        [XmlAttribute("max-values")]
        public string MaxValuesSerializable { get { return MaxValues.ToString(); } set { MaxValues = value.ToNullableInt(); } }

        [XmlIgnore]
        public bool? KeepValues { get; set; }

        [XmlAttribute("keep-values")]
        public string KeepValuesSerializable { get { return KeepValues.ToString(); } set { KeepValues = value.ToNullableBool(); } }

        /// <summary>
        /// Formatting of the value.
        /// </summary>
        /// <value>Element name Format</value>
        public Formatter Format { get; set; }


        [XmlAttribute("format")]
        public string FormatShorthand { get { return Format != null ? Format.Format : null; } set { Format = Formatter.Create(value); } }

        [XmlElement("Fallback")]
        public ValueProvider FallbackValueProvider { get; set; }

        [XmlIgnore]
        public float? Order { get; set; }

        [XmlAttribute("order")]
        public string OrderSerializable { get { return Order.ToString(); } set { Order = value.ToNullableFloat(); } }

        /// <summary>
        /// Gets the formatted value(s) as string(s).
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public override object GetFormattedValue(object obj, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)
        {
            return GetFormattedValue(obj, formatterContext, formatProvider, null);
        }

        /// <summary>
        /// Gets the formatted value(s) as string.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public override string GetFormattedValue(object obj, string multipleValuesSeparator = null, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)
        {
            return GetFormattedValue(obj, formatterContext, formatProvider, multipleValuesSeparator ?? MultiValueSeparator ?? " ") as string;
        }

        protected virtual object GetFormattedValue(object obj, FormatterContext formatterContext, IFormatProvider formatProvider, string multipleValuesSeparator)
        {
            var value = GetValue(obj, formatterContext);
            if (value == null && FallbackValueProvider != null) return FallbackValueProvider.GetFormattedValue(obj, formatterContext, formatProvider, multipleValuesSeparator);
            return ValueProvider.GetFormattedValueCore(value, Format, formatterContext, formatProvider, multipleValuesSeparator);
        }

        public string FormatValue(object value, string multipleValuesSeparator = null, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)
        {
            return ValueProvider.GetFormattedValueCore(value, Format, formatterContext, formatProvider, multipleValuesSeparator ?? MultiValueSeparator ?? " ") as string;
        }

        protected static object GetFormattedValueCore(object value, Formatter formatter = null, FormatterContext formatterContext = null, IFormatProvider formatProvider = null, string multipleValuesSeparator = null)
        {
            var values = value is string ? null : value as IEnumerable;

            if (values == null)
            {
                if (value == null) return null;

                if (formatter != null)
                {
                    return formatter.GetFormattedValue(value, formatterContext, formatProvider);
                }
                else
                {
                    return Formatter.ToFormattedString(value, formatProvider);
                }
            }
            else
            {
                if (formatter != null)
                {
                    var formattedValues = values.Cast<object>().Select(v => formatter.GetFormattedValue(v, formatterContext, formatProvider));
                    if (multipleValuesSeparator == null) return formattedValues;
                    return string.Join(multipleValuesSeparator, formattedValues);
                }
                else
                {
                    var formattedValues = values.Cast<object>().Select(v => Formatter.ToFormattedString(v, formatProvider));
                    if (multipleValuesSeparator == null) return formattedValues;
                    return string.Join(multipleValuesSeparator, formattedValues);
                }
            }
        }

        public static string GetFormattedValue(object value, Formatter formatter = null, FormatterContext formatterContext = null, IFormatProvider formatProvider = null, string multipleValuesSeparator = ", ")
        {
            object formattedValue = ValueProvider.GetFormattedValueCore(value, formatter, formatterContext, formatProvider);
            string formattedStringValue = formattedValue as string;
            if (formattedStringValue != null) return formattedStringValue;
            var formattedValues = formattedValue as IEnumerable<string>;
            if (formattedValues != null)
            {
                return string.Join(multipleValuesSeparator, formattedValues);
            }
            return null;
        }

        public static IEnumerable<string> GetFormattedValues(object value, Formatter formatter = null, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)
        {
            object formattedValue = ValueProvider.GetFormattedValueCore(value, formatter, formatterContext, formatProvider);
            var formattedValues = formattedValue as IEnumerable<string>;
            if (formattedValues != null) return formattedValues;
            string formattedStringValue = formattedValue as string;
            return formattedStringValue != null ? new string[] { formattedStringValue } : Enumerable.Empty<string>();
        }

        public override object GetValue(object obj = null, FormatterContext formatterContext = null)
        {
            //object value = null;
            //if (Specification != null)
            //{
            //    IEnumerable<object> specValues = null;
            //    var specificationsContainer = obj as ISpecificationIdentifiables;
            //    if (specificationsContainer != null)
            //    {
            //        var matchContext = formatterContext != null ? new MatchContext(formatterContext.ApplicationRequestContext) : null;
            //        specValues = specificationsContainer.Specifications.Where(s => Specification.Match(s, matchContext));
            //    }
            //    else
            //    {
            //        var specifications = obj as IEnumerable<ISpecificationIdentifiable>;
            //        if (specifications != null)
            //        {
            //            var matchContext = formatterContext != null ? new MatchContext(formatterContext.ApplicationRequestContext) : null;
            //            specValues = specifications.Where(s => Specification.Match(s, matchContext));
            //        }
            //        else
            //        {
            //            var specificationModels = obj as IEnumerable<Specification>;
            //            if (specificationModels != null)
            //            {
            //                var matchContext = formatterContext != null ? new MatchContext(formatterContext.ApplicationRequestContext) : null;
            //                specValues = specificationModels.Where(s => Specification.Match(s, matchContext));
            //            }
            //            else
            //            {
            //                var modelSpecificationsContainer = obj as ISpecifications;
            //                if (modelSpecificationsContainer != null)
            //                {
            //                    var matchContext = formatterContext != null ? new MatchContext(formatterContext.ApplicationRequestContext) : null;
            //                    specValues = modelSpecificationsContainer.Specifications.Where(s => Specification.Match(s, matchContext));
            //                }
            //                else
            //                {
            //                    return null;
            //                }
            //            }
            //        }
            //    }

            //    if (PropertyName != null && PropertyName.Length > 0)
            //    {
            //        specValues = specValues.Select(s =>
            //        {
            //            var propertyGetter = PropertyGetter.Create(PropertyName, s.GetType());
            //            return propertyGetter != null ? propertyGetter.GetValue(s) : null;
            //        }).Where(s => s != null);
            //    }

            //    if (MaxValues.HasValue == true)
            //    {
            //        specValues = specValues.Take(MaxValues.Value);
            //    }

            //    if (KeepValues != false)
            //    {
            //        return specValues.FirstIfSingleOrAll();
            //    }

            //    return specValues;
            //}
            //else if (PropertyName != null)
            //{
            //    if (obj == null || PropertyName.Length == 0) return null;

            //    if (PropertyName.StartsWith(".") == false)
            //    {
            //        var propertyGetter = PropertyGetter.Create(PropertyName, obj.GetType());
            //        if (propertyGetter != null)
            //        {
            //            object value = propertyGetter.GetValue(obj);

            //            var valueCollection = value as IEnumerable;
            //            if (valueCollection != null && !(value is string))
            //            {
            //                if (MaxValues.HasValue == true)
            //                {
            //                    if (KeepValues != false)
            //                    {
            //                        if (MaxValues.Value == 1)
            //                        {
            //                            return valueCollection.FirstOrDefaultNonGeneric();
            //                        }
            //                        else
            //                        {
            //                            return valueCollection;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        return valueCollection.TakeNonGeneric(MaxValues.Value);
            //                    }
            //                }
            //                else if (KeepValues != false)
            //                {
            //                    return valueCollection.FirstIfSingleOrAll();
            //                }

            //                return valueCollection;
            //            }

            //            return value;
            //            //object objectPropertyValue = propertyGetter.GetValue(obj);
            //            //return objectPropertyValue as string ?? (objectPropertyValue != null ? objectPropertyValue.ToString() : string.Empty);
            //        }
            //    }
            //    else
            //    {
            //        var propertyGetter = PropertyGetter.Create(PropertyName.Substring(1), typeof(IApplicationRequestContext));
            //        if (propertyGetter != null)
            //        {
            //            object value = propertyGetter.GetValue(formatterContext.ApplicationRequestContext);
            //            var valueCollection = value as IEnumerable;
            //            if (valueCollection != null && !(value is string))
            //            {
            //                if (MaxValues.HasValue == true)
            //                {
            //                    valueCollection = valueCollection.TakeNonGeneric(MaxValues.Value);
            //                }
            //                if (KeepValues != false)
            //                {
            //                    return valueCollection.FirstIfSingleOrAll();
            //                }
            //                return valueCollection;
            //            }
            //            return value;
            //        }
            //        else
            //        {
            //            int staticObjectIndex = PropertyName.IndexOf(".");
            //            if (staticObjectIndex >= 0)
            //            {
            //                string staticObjectTypeName = PropertyName.Substring(0, staticObjectIndex);
            //                Type staticObjectType = Type.GetType(staticObjectTypeName, false);
            //                if (staticObjectType != null)
            //                {
            //                    string staticObjectPropertyPath = PropertyName.Substring(staticObjectIndex + 1);
            //                    int staticObjectPropertyPathIndex = PropertyName.IndexOf(".");
            //                    if (staticObjectPropertyPathIndex >= 0)
            //                    {

            //                    }
            //                    else
            //                    {

            //                    }
            //                }
            //            }

            //        }
            //    }


            //}


            return StaticValue != null ? StaticValue.Value : null;
        }

        public static ValueProvider CreateProperty(string propertyName)
        {
            if (propertyName == null) return null;
            //if (propertyName.Length > 14 && propertyName.StartsWith("Specification", StringComparison.OrdinalIgnoreCase) == true && propertyName[13] == '=')
            //{
            //    return ValueProvider.CreateSpecification(propertyName.Substring(14));
            //}
            if (propertyName.Length > 5 && propertyName.StartsWith("Text", StringComparison.OrdinalIgnoreCase) == true && propertyName[4] == '=')
            {
                return ValueProvider.CreateText(propertyName.Substring(5));
            }
            // Handle fallback?! And path!
            return new ValueProvider { PropertyName = propertyName };
        }

        public static ValueProvider CreateStaticValue(StaticValue value)
        {
            return value != null ? new ValueProvider { StaticValue = value } : null;
        }

        //public static ValueProvider CreateSpecification(string specificationTypeCode)
        //{
        //    return specificationTypeCode != null ? new ValueProvider { SpecificationCodeShorthand = specificationTypeCode } : null;
        //}

        //public static IEnumerable<ValueProvider> CreateSpecifications(string specificationTypeCodes)
        //{
        //    if (specificationTypeCodes == null) return Enumerable.Empty<ValueProvider>();
        //    return specificationTypeCodes.Split(new char[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(c => new ValueProvider { SpecificationCodeShorthand = c });
        //}

        public static ValueProvider CreateText(string text)
        {
            return text != null ? new TextValueProvider { Text = text } : null;
        }

        public static ParsedValueProviderFormat ParseValueProviderFormat(string format, string debugContext = null)
        {
            try
            {
                int paramIndex = format.IndexOf('{');

                if (paramIndex == -1) return null;
                var valueProviders = new List<ValueProvider>();
                int handledIndex = 0;
                var transformedFormatBuilder = new StringBuilder();

                int index = 0;
                do
                {
                    if (paramIndex == format.Length) break; // Invalid format
                    int paramEndIndex = format.IndexOf('}', paramIndex);
                    if (paramEndIndex == -1) break; // Invalid format
                    int paramNameLength = paramEndIndex - paramIndex - 1;
                    int alignFormatTokenIndex = format.IndexOfAny(new char[] { ':', ',' }, paramIndex + 1, paramNameLength);
                    if (alignFormatTokenIndex >= 0)
                    {
                        paramNameLength = alignFormatTokenIndex - paramIndex - 1;
                    }
                    string paramName = format.Substring(paramIndex + 1, paramNameLength);
                    transformedFormatBuilder.Append(format.Substring(handledIndex, paramIndex + 1 - handledIndex));
                    transformedFormatBuilder.Append(index);
                    transformedFormatBuilder.Append(format.Substring(paramIndex + paramNameLength + 1, paramEndIndex - (paramIndex + paramNameLength)));
                    handledIndex = paramEndIndex + 1;

                    var valueProvider = ValueProvider.CreateProperty(paramName);
                    valueProvider.Order = index++;

                    // Fallback
                    if (format.Length > handledIndex + 1 && format[handledIndex] == '|')
                    {
                        if (format[handledIndex + 1] == '{')
                        {
                            int fallbackParamEndIndex = format.IndexOf('}', handledIndex + 2);
                            if (fallbackParamEndIndex == -1) break; // Invalid format
                            string fallbackParamName = format.Substring(handledIndex + 2, fallbackParamEndIndex - handledIndex - 2);
                            paramIndex = fallbackParamEndIndex + 1;
                            handledIndex = fallbackParamEndIndex + 1;
                            valueProvider.FallbackValueProvider = ValueProvider.CreateProperty(fallbackParamName);
                        }
                        else
                        {
                            int fallbackEndIndex = format.IndexOf(' ', handledIndex + 2);
                            string fallbackValue;
                            if (fallbackEndIndex == -1)
                            {
                                fallbackValue = format.Substring(handledIndex + 1);
                                valueProvider.FallbackValueProvider = ValueProvider.CreateStaticValue(new StaticValue(fallbackValue));
                                valueProviders.Add(valueProvider);
                                break;
                            }
                            else
                            {
                                fallbackValue = format.Substring(handledIndex + 1, fallbackEndIndex - handledIndex - 1);
                                paramIndex = fallbackEndIndex;
                                handledIndex = fallbackEndIndex;
                                valueProvider.FallbackValueProvider = ValueProvider.CreateStaticValue(new StaticValue(fallbackValue));
                            }

                        }

                    }

                    valueProviders.Add(valueProvider);

                    paramIndex = format.IndexOf('{', paramIndex + 1);
                } while (paramIndex >= 0);

                transformedFormatBuilder.Append(format.Substring(handledIndex));

                string transformedFormat = transformedFormatBuilder.ToString();

                return new ParsedValueProviderFormat { Format = transformedFormat, ValueProviders = valueProviders };
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public class ParsedValueProviderFormat
        {
            public string Format { get; set; }

            public List<ValueProvider> ValueProviders { get; set; }
        }
    }

    public class TextValueProvider : ValueProvider
    {
        [XmlAttribute("text")]
        public string Text { get; set; }

        public override object GetValue(object obj = null, FormatterContext formatterContext = null)
        {
            if (Text != null)
            {
                return formatterContext != null ? formatterContext.ApplicationRequestContext.GetApplicationTextTranslation(Text) : Text;
            }
            return null;
        }
    }

    public class DelegateValueProvider<T> : ValueProviderBase
    {
        public DelegateValueProvider() { }

        public DelegateValueProvider(Func<T, object> value)
        {
            Value = value;
        }

        public Func<T, object> Value { get; set; }

        public override object GetFormattedValue(object obj, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)
        {
            return MultivendorWebViewer.Common.ValueProvider.GetFormattedValue(Value((T)obj), formatter: null, formatterContext: formatterContext, formatProvider: formatProvider);
        }

        public override string GetFormattedValue(object obj, string multipleValuesSeparator, FormatterContext formatterContext = null, IFormatProvider formatProvider = null)
        {
            return MultivendorWebViewer.Common.ValueProvider.GetFormattedValue(Value((T)obj), formatter: null, formatterContext: formatterContext, formatProvider: formatProvider);
        }

        public override object GetValue(object obj = null, FormatterContext formatterContext = null)
        {
            return Value((T)obj);
        }
    }

    public class StaticValue : IXmlSerializable
    {
        public StaticValue() { }

        public StaticValue(object value)
        {
            if (value != null)
            {
                Value = value;
                Type = value.GetType();
            }
        }

        public StaticValue(object value, Type type)
        {
            Value = value;
            Type = type;
        }

        private static Dictionary<string, Type> typeIndex = new Dictionary<string, Type>();

        private static object typeIndexLock = new object();

        public object Value { get; set; }

        public Type Type { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            string typeName = reader.GetAttribute("type");
            bool emptyElement = reader.IsEmptyElement;

            reader.ReadStartElement();

            if (emptyElement == false)
            {
                reader.MoveToContent();

                var type = GetType(typeName ?? reader.LocalName) ?? typeof(string);

                var typeCode = Type.GetTypeCode(type);
                if (typeCode != TypeCode.Object)
                {
                    string content = reader.Value;
                    if (content != null)
                    {
                        if (typeCode == TypeCode.String)
                        {
                            Value = content;
                        }
                        else
                        {
                            Value = Convert.ChangeType(content, type);
                        }
                    }
                }
                else
                {
                    Value = Instance.Deserialize(type, reader, useDataContractSerializer: false);
                }

                reader.ReadEndElement();
            }

        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        protected virtual Type GetType(string name)
        {
            Type type;
            if (typeIndex.TryGetValue(name, out type) == false)
            {
                lock (typeIndexLock)
                {
                    if (typeIndex.TryGetValue(name, out type) == false)
                    {
                        typeIndex[name] = type = Instance.FindTypes(name, () => Assembly.GetAssembly(typeof(string)), () => Assembly.GetCallingAssembly()).FirstOrDefault();
                    }
                }
            }
            return type;
        }

        public static implicit operator StaticValue(string str)
        {
            return str != null ? new StaticValue(str, typeof(string)) : null;
        }
    }
}
