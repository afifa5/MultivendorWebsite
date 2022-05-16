using Newtonsoft.Json;
using MultivendorWebViewer;
using MultivendorWebViewer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Components
{
    /// <summary>
    /// Specifies the sort of lists and collections.
    /// </summary>
    public class Sort
    {
        public Sort() { }

        public Sort(string type, SortDirection direction = SortDirection.Ascending)
        {
            Type = type;
            Direction = direction;
        }

        /// <summary>
        /// The sort direction.
        /// </summary>
        /// <value>Attribute name direction</value>
        /// <example>Ascending</example>
        /// <remarks>Valid values: Ascending, Descending</remarks>
        [JsonProperty("direction")]
        [XmlAttribute("direction")]
        public SortDirection Direction { get; set; }

        /// <summary>
        /// What to sort on.
        /// </summary>
        /// <value>Attribute name type</value>
        /// <example>PartNumber</example>
        [JsonProperty("type")]
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("Value", Type = typeof(ValueProvider))]
        public ValueProviderBase ValueProvider { get; set; }

        [XmlIgnore]
        public IComparer<object> Comparer { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Sort;
            return other != null && other.Direction == Direction && StringComparer.OrdinalIgnoreCase.Equals(other.Type, Type);
        }

        public override int GetHashCode()
        {
            return HashHelper.CombineHashCode(StringComparer.OrdinalIgnoreCase.GetHashCode(Type), Direction.GetHashCode());
        }

        public override string ToString()
        {
            string str = string.Format("{0} {1}", Type, Direction.ToString());
            if (ValueProvider != null) str = string.Concat(str, " ", ValueProvider.ToString());
            return str;
        }

        public static Sort Parse(string str)
        {
            if (string.IsNullOrEmpty(str) == true) return null;
            var sort = new Sort();
            var parts = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                string type = parts[0];
                if (StringComparer.OrdinalIgnoreCase.Equals(type, "Ascending") == true)
                {
                    sort.Direction = SortDirection.Ascending;
                    if (parts.Length > 1) sort.Type = parts[1];
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(type, "Descending") == true)
                {
                    sort.Direction = SortDirection.Descending;
                    if (parts.Length > 1) sort.Type = parts[1];
                }
                else
                {
                    sort.Type = type;
                    if (parts.Length > 1)
                    {
                        if (StringComparer.OrdinalIgnoreCase.Equals(parts[1], "Ascending") == true) sort.Direction = SortDirection.Ascending;
                        else if (StringComparer.OrdinalIgnoreCase.Equals(parts[1], "Descending") == true) sort.Direction = SortDirection.Descending;
                    }
                }
            }
            return sort;
        }

        public virtual Sort Copy()
        {
            return new Sort
            {
                Type = this.Type,
                Comparer = this.Comparer,
                ValueProvider = this.ValueProvider,
                Direction = this.Direction
            };
        }
    }

    //
    // Summary:
    //     Specifies the direction in which to sort a list of items.
    public enum SortDirection
    {
        //
        // Summary:
        //     Sort from smallest to largest —for example, from 1 to 10.
        Ascending = 0,
        //
        // Summary:
        //     Sort from largest to smallest — for example, from 10 to 1.
        Descending = 1
    }
}
