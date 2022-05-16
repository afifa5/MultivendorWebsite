using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Components
{
    public enum CompareOperator
    {
        NotSet,
        /// <summary>
        /// Equals
        /// </summary>
        [XmlEnum("==")]
        [JsonProperty("==")]
        Equal,
        /// <summary>
        /// Not equals
        /// </summary>
        [XmlEnum("!=")]
        [JsonProperty("!=")]
        NotEqual,
        /// <summary>
        /// Less than
        /// </summary>
        [XmlEnum("<")]
        [JsonProperty("<")]
        LessThan,
        /// <summary>
        /// Less or equal than
        /// </summary>
        [XmlEnum("<=")]
        [JsonProperty("<=")]
        LessOrEqualThan,
        /// <summary>
        /// Greater than
        /// </summary>
        [XmlEnum(">")]
        [JsonProperty(">")]
        GreaterThan,
        /// <summary>
        /// Greater or equal than
        /// </summary>
        [XmlEnum(">=")]
        [JsonProperty(">=")]
        GreaterOrEqualThan,
    };

    public enum LogicalOperator
    {
        [XmlEnum("not")]
        [JsonProperty("not")]
        NOT,
        [XmlEnum("and")]
        [JsonProperty("and")]
        AND,
        [XmlEnum("xor")]
        [JsonProperty("xor")]
        XOR,
        [XmlEnum("or")]
        [JsonProperty("or")]
        OR
    }

    public enum StringCompareMode 
    {
        [XmlEnum("equals")]
        [JsonProperty("equals")]
        Equals, 
        [XmlEnum("contains")]
        [JsonProperty("contains")]
        Contains, 
        [XmlEnum("starts-with")]
        [JsonProperty("startsWith")]
        StartsWith, 
        [XmlEnum("ends-with")]
        [JsonProperty("endsWith")]
        EndsWith,
        [XmlEnum("not-equals")]
        [JsonProperty("notEquals")]
        NotEquals,
    }
}
