
using MultivendorWebViewer.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MultivendorWebViewer.Components;


namespace MultivendorWebViewer.Configuration
{
    /// <summary>
    /// Defines how to format a value. E.g. to make a part number be display in groups like 1234 5678 90.
    /// </summary>
    public class Formatter 
    {
        /// <summary>
        /// Set to true to format to empty value. Default is false.
        /// </summary>
        /// <value>Attribute name hide</value>
        /// 
        [XmlAttribute("hide")]
        public bool Hide { get; set; }
        protected string id;
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set
            {
                id=value;
            }
        }
        public static Formatter GetNamedInstance(string id)
        {
            if (id == null) return null;
            Formatter baseInstance = new Formatter();
            return baseInstance;
        }
        [XmlAttribute("html")]
        public bool HtmlEncode { get; set; }

        [XmlElement("FormatProvider")]
        public XmlElementWrapper<IFormatProvider> FormatProviderWrapper { get; set; }

        [XmlIgnore]
        public IFormatProvider FormatProvider { get { return FormatProviderWrapper as IFormatProvider; } }

        //[XmlElement("Transform")]
        public FormatterTransformDictionary Transforms { get; set; }

        /// <summary>
        /// The actual format to use when formatting the value.
        /// </summary>
        /// <example>See https://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx </example>
        /// <value>Attribute name format</value>
        [XmlAttribute("format")]
        public string Format { get; set; }

        /// <summary>
        /// A regex pattern to use for extracting variables from the input value. Those values can then be used in the format.
        /// </summary>
        /// <example>\b(?'a'\d{4})(?'b'\d{4})(?'c'\d{2})\b
        /// will match a 10 digit number dividing it into three groups a, b, c
        /// used together with format '${a} ${b} ${c}' will give an output of #### #### ## for a 10 digit number. If the regex is not matched, the value will be unformatted.
        /// </example>
        /// <value>Attribute name pattern</value>
        [XmlAttribute("pattern")]
        public string RegexPattern { get; set; }

        private Regex regex;
        public Regex Regex { get { return regex ?? (regex = new Regex(RegexPattern)); } }

        public virtual string GetFormattedValue(object value, FormatterContext context = null, IFormatProvider formatProvider = null)
        {
            if (Hide == true) return null;

            if (formatProvider == null)
            {
                formatProvider = FormatProvider;
            }

            string formattedValue = null;

            if (value != null)
            {
                var values = value is string ? null : value as IEnumerable;
                if (values == null)
                {
                    if (Format != null)
                    {
                        string format;
                        if (context != null && context.FormatTransformer != null)
                        {
                            format = context.FormatTransformer(context, Format);
                        }
                        else
                        {
                            format = Format;
                        }

                        if (RegexPattern != null)
                        {
                            formattedValue = Formatter.ToFormattedString(value, formatProvider);
                            formattedValue = Regex.Replace(formattedValue, format);
                        }
                        else
                        {
                            formattedValue = Formatter.ToFormattedString(value, formatProvider, format);
                        }
                    }
                    else
                    {
                        formattedValue = Formatter.ToFormattedString(value, formatProvider);
                    }
                }
                else
                {
                    if (Format != null)
                    {
                        string format;
                        if (context != null && context.FormatTransformer != null)
                        {
                            format = context.FormatTransformer(context, Format);
                        }
                        else
                        {
                            format = Format;
                        }
                        formattedValue = string.Format(formatProvider, format, values.Cast<object>().ToArray());
                    }
                    else
                    {
                        formattedValue = Formatter.ToFormattedString(value, formatProvider);
                    }
                }
            }

            if (Transforms != null)
            {
                var entry = Transforms.Entries[formattedValue ?? string.Empty];
                if (entry != null)
                {
                    formattedValue = entry.GetValue(context);
                }
            }

            return formattedValue;
        }

    
        public static string ToFormattedString(object value, IFormatProvider formatProvider, string format = null)
        {
            var formattableValue = value as IFormattable;
            if (formattableValue != null)
            {
                return formattableValue.ToString(format, formatProvider);
            }
            var convertible = value as IConvertible;
            if (convertible != null)
            {
                return convertible.ToString(formatProvider);
            }

            return value != null ? value.ToString() : null;
        }

        public static Formatter Create(string format)
        {
            return format != null ? new Formatter { Format = format } : null;
        }
    }

    public interface IFormatterProvider
    {
        Formatter Formatter { get; }
    }


    public enum IdentifierOperator
    {
        NotSet,
        /// <summary>
        /// Equals
        /// </summary>
        [XmlEnum("==")]
        Equal,
        /// <summary>
        /// Not equals
        /// </summary>
        [XmlEnum("!=")]
        NotEqual,
        /// <summary>
        /// Less than
        /// </summary>
        [XmlEnum("<")]
        LessThan,
        /// <summary>
        /// Less or equal than
        /// </summary>
        [XmlEnum("<=")]
        LessOrEqualThan,
        /// <summary>
        /// Greater than
        /// </summary>
        [XmlEnum(">")]
        GreaterThan,
        /// <summary>
        /// Greater or equal than
        /// </summary>
        [XmlEnum(">=")]
        GreaterOrEqualThan
    };

    public class FormatterContext
    {
        public FormatterContext() { }

        public FormatterContext(ApplicationRequestContext applicationRequestContext)
        {
            ApplicationRequestContext = applicationRequestContext;
        }

        public FormatterContext(ApplicationRequestContext applicationRequestContext, object component)
        {
            ApplicationRequestContext = applicationRequestContext;
            Component = component;
        }

        public ApplicationRequestContext ApplicationRequestContext { get; set; }

        public object Component { get; set; }

        public Func<FormatterContext, string, string> FormatTransformer { get; set; }
    }

    public class FormatterContext<T> : FormatterContext
    {
        public FormatterContext() { }

        public FormatterContext(ApplicationRequestContext applicationRequestContext, T component)
        {
            ApplicationRequestContext = applicationRequestContext;
            Component = component;
        }

        public new T Component { get; set; }
    }

    public class FormatterTransformDictionary
    {
        [XmlAttribute("ignore-case")]
        public bool IgnoreCase { get; set; }

        [XmlElement("String", Type = typeof(FormatterStringTransform))]
        [XmlElement("Text", Type = typeof(FormatterTextTransform))]
        //[XmlElement("Icon", Type = typeof(FormatterIconTransform))] TODO WEB
        public FormatterTransfromBase[] EntriesCollection
        {
            get { return Entries.ToArray(); }
            set { Entries = new EntriesDictionary(value, IgnoreCase == true ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal); }
        }

        [XmlIgnore]
        public EntriesDictionary Entries { get; private set; }

        public class EntriesDictionary : DictionaryCollection<string, FormatterTransfromBase>
        {
            public EntriesDictionary(IEnumerable<FormatterTransfromBase> items, StringComparer comparer) : base(items, comparer) { }

            protected override string GetKeyForItem(FormatterTransfromBase item)
            {
                return item.Key;
            }
        }
    }

    public interface IFormatterTransform
    {
        string Key { get; }

        string GetValue(FormatterContext context);
    }

    public abstract class FormatterTransfromBase : IFormatterTransform
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        public abstract string GetValue(FormatterContext context);
    }

    public class FormatterStringTransform : FormatterTransfromBase
    {
        [XmlAttribute("value")]
        public string String { get; set; }

        public override string GetValue(FormatterContext context)
        {
            return String;
        }
    }

    public class FormatterTextTransform : FormatterTransfromBase
    {
        [XmlAttribute("value")]
        public string Text { get; set; }

        public override string GetValue(FormatterContext context)
        {
            return context != null ? context.ApplicationRequestContext.GetApplicationTextTranslation(Text) : Text;
        }
    }
}
