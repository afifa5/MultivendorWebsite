
using MultivendorWebViewer.Collections;
using MultivendorWebViewer.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MultivendorWebViewer.ViewModels;
using MultivendorWebViewer;
using MultivendorWebViewer.Configuration;
using MultivendorWebViewer.Components;
using MultivendorWebViewer.Models;

namespace MultivendorWebViewer.Configuration
{
    /// <summary>
    /// Identifier to indentify objects.
    /// </summary>
    public class Identifier
    {
        public Identifier()
        {
            MaxMatches = int.MaxValue;
        }

        /// <summary>
        /// The name of the named object to match against.
        /// </summary>
        /// <value>Attribute name object</value>
        [XmlAttribute("object")]
        public string Object { get; set; }

        /// <summary>
        /// The name of the objects property to get the value to match against.
        /// </summary>
        /// <value>Attribute name property</value>
        [XmlAttribute("property")]
        public string PropertyName { get; set; }

        /// <summary>
        /// The value to match against the value of the objects property specified by 'property'. 
        /// </summary>
        /// <value>Attribute name value</value>
        [XmlAttribute("value")]
        public string PropertyValue { get; set; }

        [XmlAttribute("has-value")]
        public string HasPropertyValueSerializable { get { return HasPropertyValue.ToString(); } set { HasPropertyValue = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? HasPropertyValue { get; set; }

        /// <summary>
        /// The name of the value property to match the value of PropertyName property against.
        /// </summary>
        /// <value>Attribute name value-property</value>
        [XmlAttribute("value-property")]
        public string ValuePropertyName { get; set; }


        /// <summary>
        /// Regex pattern to match against the value of the objects property specified by 'value'. 
        /// </summary>
        /// <value>Attribute name regex</value>
        [XmlAttribute("regex")]
        public string RegexPattern { get; set; }

        public bool UseRegex { get { return RegexPattern != null; } }

        /// <summary>
        /// An operator to determine if the 'value' is equal (==), not equal (!=), less than (&lt) or greater than (&gt) the value of objects property specified by 'property'.
        /// </summary>
        /// <value>Attribute name operator</value>
        /// <see cref="IdentifierOperator"/>
        [XmlAttribute("operator")]
        public IdentifierOperator Operator { get; set; }

        [XmlAttribute("max-matches")]
        public int MaxMatches { get; set; }

        //[XmlAttribute("context")]
        //public string AllContextString { get { return Utility.CodeSetToString(AllContext); } set { AllContext = Utility.CreateOptimizedCodeSet(value, comparer: StringComparer.OrdinalIgnoreCase); } }

        [XmlIgnore]
        public ISet<string> AllContext { get; set; }

        private Regex regex;
        [XmlIgnore]
        public Regex Regex
        {
            get { return regex ?? (regex = string.IsNullOrEmpty(RegexPattern) == false ? new Regex(RegexPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled) : null); }
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj) == true) return true;
            var other = obj as Identifier;
            if (other == null) return false;
            var comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Equals(PropertyName, other.PropertyName) == true && HasPropertyValue == other.HasPropertyValue && comparer.Equals(PropertyValue, other.PropertyValue) && comparer.Equals(Object, other.Object)
                && comparer.Equals(ValuePropertyName, other.ValuePropertyName) == true && UseRegex == other.UseRegex && comparer.Equals(RegexPattern, other.RegexPattern) &&
                Operator == other.Operator && MaxMatches == other.MaxMatches && new EnumerableEqualityComparer<string>(comparer).Equals(AllContext, other.AllContext);
        }
        public override int GetHashCode()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            HashHelper.CombineHashCode(comparer.GetHashCode(PropertyName), comparer.GetHashCode(PropertyValue), comparer.GetHashCode(Object), comparer.GetHashCode(ValuePropertyName), comparer.GetHashCode(RegexPattern),
                Operator.GetHashCode(), MaxMatches.GetHashCode(), HasPropertyValue.GetHashCode(), UseRegex.GetHashCode(), new EnumerableEqualityComparer<string>(comparer).GetHashCode(AllContext));

            return base.GetHashCode();
        }

        protected bool? MatchRegex(object obj)
        {
            if (RegexPattern != null)
            {
                string objectString = obj as string ?? (obj != null ? obj.ToString() : null);
                return objectString != null && Regex.IsMatch(objectString);
            }
            return null;
        }

        public virtual bool Match(object obj, MatchContext context = null)
        {
            try
            {
                if (PropertyValue != null || RegexPattern != null)
                {
                    string objectPropertyValueString = null;
                    if (string.IsNullOrEmpty(PropertyName) == false)
                    {
                        object valueObj = obj;
                        if (PropertyName.StartsWith(".", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            string globalName = PropertyName.Substring(1);
                            if (context == null) return false;
                            valueObj = context; // TODO
                        }

                        var propertyGetter = PropertyGetter.Create(PropertyName, valueObj.GetType());
                        var objectPropertyValue = propertyGetter != null ? propertyGetter.GetValue(valueObj) : null;
                        if (objectPropertyValue == null) return false;

                        var objectPropertyValues = objectPropertyValue as IEnumerable<object>;
                        if (objectPropertyValues != null && (objectPropertyValue is string) == false)
                        {
                            objectPropertyValue = objectPropertyValues.FirstOrDefault();
                        }

                        // TODO This only handles when propertyname refers a property that is IComparable, implement for valuepropertyname
                        if (Operator != IdentifierOperator.NotSet)
                        {
                            var comparableObjectPropertyValue = objectPropertyValue as IComparable;
                            if (comparableObjectPropertyValue != null)
                            {
                                var typeCode = Convert.GetTypeCode(comparableObjectPropertyValue);
                                if (typeCode != TypeCode.Empty && typeCode != TypeCode.Object)
                                {
                                    object convertedPropertyValue = null;
                                    if (ValuePropertyName != null)
                                    {
                                        var valuePropertyGetter = PropertyGetter.Create(ValuePropertyName, obj.GetType());
                                        convertedPropertyValue = valuePropertyGetter != null ? valuePropertyGetter.GetValue(obj) : null;
                                    }
                                    else
                                    {
                                        convertedPropertyValue = Convert.ChangeType(PropertyValue, typeCode, CultureInfo.InvariantCulture);
                                    }

                                    int result = comparableObjectPropertyValue.CompareTo(convertedPropertyValue);
                                    switch (Operator)
                                    {
                                        case IdentifierOperator.Equal: return result == 0;
                                        case IdentifierOperator.NotEqual: return result != 0;
                                        case IdentifierOperator.LessThan: return result < 0;
                                        case IdentifierOperator.LessOrEqualThan: return result <= 0;
                                        case IdentifierOperator.GreaterThan: return result > 0;
                                        case IdentifierOperator.GreaterOrEqualThan: return result >= 0;
                                    }
                                }
                            }

                            return false;
                        }

                        objectPropertyValueString = objectPropertyValue as string ?? objectPropertyValue.ToString();
                    }
                    else
                    {
                        objectPropertyValueString = obj as string ?? (obj != null ? obj.ToString() : null);
                    }

                    if (RegexPattern != null)
                    {
                        if (Regex.IsMatch(objectPropertyValueString) == false)
                        {
                            return false;
                        }
                    }

                    if (PropertyValue != null)
                    {
                        if (string.Equals(objectPropertyValueString, PropertyValue, StringComparison.OrdinalIgnoreCase) == false)
                        {
                            return false;
                        }
                    }

                    if (ValuePropertyName != null)
                    {
                        object valueObj = obj;
                        if (ValuePropertyName.StartsWith(".", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            string globalName = ValuePropertyName.Substring(1);
                            if (context == null) return false;
                            valueObj = context.RequestContext; // TODO
                        }

                        var valuePropertyGetter = PropertyGetter.Create(ValuePropertyName, valueObj.GetType());
                        var valueObjectPropertyValue = valuePropertyGetter != null ? valuePropertyGetter.GetValue(valueObj) : null;
                        string valueObjectPropertyValueString = valueObjectPropertyValue as string ?? (valueObjectPropertyValue != null ? valueObjectPropertyValue.ToString() : null);
                        if (string.Equals(objectPropertyValueString, valueObjectPropertyValueString, StringComparison.OrdinalIgnoreCase) == false)
                        {
                            return false;
                        }
                    }
                }

                // Check is the given property is null or not (or in case of an enumerable, is contains items or not)
                if (HasPropertyValue.HasValue == true)
                {
                    if (string.IsNullOrEmpty(PropertyName) == false)
                    {
                        object valueObj = obj;
                        if (PropertyName.StartsWith(".", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            string globalName = PropertyName.Substring(1);
                            valueObj = context.RequestContext; // TODO
                        }

                        var propertyGetter = PropertyGetter.Create(PropertyName, valueObj.GetType());
                        var objectPropertyValue = propertyGetter != null ? propertyGetter.GetValue(valueObj) : null;

                        if (objectPropertyValue == null) // Does not have value
                        {
                            if (HasPropertyValue.Value == true) return false;
                        }
                        else
                        {
                            var enumerable = objectPropertyValue as IEnumerable;
                            if (enumerable != null) // Is an enumerable
                            {
                                if (enumerable.GetEnumerator().MoveNext() == false) // No items
                                {
                                    if (HasPropertyValue.Value == true) return false;
                                }
                            }

                            // We have item(s)
                            if (HasPropertyValue.Value == false) return false;
                        }
                    }
                }

                if (AllContext != null)
                {
                    if (context != null && context.RequestContext != null)
                    {
                        var requestContext = context.RequestContext;

                        if (HasPropertyValue.HasValue == false || HasPropertyValue.Value == true)
                        {
                            foreach (string ctx in AllContext)
                            {
                                if (requestContext.ViewContext.Contains(ctx) == false && requestContext.HasCustomContext(ctx) == false)
                                {
                                    return false;
                                }
                            }
                            //if (context.RequestContext.ViewContext.IsSupersetOf(AllContext) == false)
                            //{
                            //    return false;
                            //}
                        }
                        else
                        {
                            foreach (string ctx in AllContext)
                            {
                                if (requestContext.ViewContext.Contains(ctx) == true || requestContext.HasCustomContext(ctx) == true)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return true;
        }
    }

    public class CompoundIdentifier<T> : Identifier
    where T : Identifier
    {
        public CompoundIdentifier()
        {
            Identifiers = new List<T>();
            Logic = CompoundLogic.Any;
        }


        [XmlAttribute("logic")]
        public CompoundLogic Logic { get; set; }

        //[XmlElement("CompoundTrigger", Type = typeof(CompoundIdentifier<T>))]
        [XmlElement("Trigger")] // TODO Custom serializing to handle child compounds
        public List<T> Identifiers { get; private set; }

        //public override bool Match(object obj, MatchContext context = null)
        //{
        //    switch (Logic)
        //    {
        //        case CompoundLogic.All: return Identifiers.All(i => i.Match(obj, context) == true);
        //        case CompoundLogic.None: return Identifiers.All(i => i.Match(obj, context) == false);
        //        case CompoundLogic.Any: return Identifiers.Any(i => i.Match(obj, context) == true);
        //        case CompoundLogic.One: return Identifiers.Where(i => i.Match(obj, context) == true).Count() == 1;
        //    }
        //    return true;
        //}
    }

    public enum CompoundLogic {[XmlEnum("any")] Any, [XmlEnum("any")] All, [XmlEnum("one")] One, [XmlEnum("none")] None };

    public class AssetIdentifier : Identifier
    {
        /// <summary>
        /// </summary>
        /// <value>Attribute name persistent-identity</value>
        [XmlAttribute("persistent-identity")]
        public string PersistentIdentiy { get; set; }

        //public override bool Match(object obj, MatchContext context = null)
        //{
        //    var asset = obj as AssetModel.Asset;
        //    if (asset != null)
        //    {
        //        if (PersistentIdentiy != null && StringComparer.Ordinal.Equals(PersistentIdentiy, asset.PersistentIdentity) == false)
        //        {
        //            return false;
        //        }
        //    }

        //    return base.Match(obj, context);
        //}
    }

    /// <summary>
    /// An identifier used to identitfy specifications.
    /// </summary>
    public class SpecificationIdentifier : Identifier
    {
        /// <summary>
        /// Match againts the specification id 
        /// </summary>
        /// <value>Attribute name id</value>
        public int Id { get; set; }

        /// <summary>
        /// Match againts the specification code 
        /// </summary>
        /// <value>Attribute name code</value>
        [XmlAttribute("code")]
        public string Code { get; set; }

        /// <summary>
        /// Match againts the specification value 
        /// </summary>
        /// <value>Attribute name value</value>
        [XmlAttribute("specification-value")]
        public string Value { get; set; }

        /// <summary>
        /// Match againts the specification formatted value
        /// </summary>
        /// <value>Attribute name formatted value</value>
        [XmlAttribute("specification-formatted-value")]
        public string FormattedValue { get; set; }

        //public override bool Match(object obj, MatchContext context = null)
        //{
           
        //    var specificationViewModel = obj as ISpecificationIdentifiable;
        //    if (specificationViewModel != null)
        //    {
        //        return Match(specificationViewModel);
        //    }
        //    else
        //    {
        //        var specification = obj as Specification;
        //        if (specification != null)
        //        {
        //            return Match(specification);
        //        }
        //        else
        //        {
        //            var specificationType = obj as SpecificationType;
        //            if (specificationType != null)
        //            {
        //                return Match(specificationType);
        //            }
        //        }
        //    }
        //    return base.Match(obj, context);
        //}

        //public virtual bool Match(ISpecificationIdentifiable specification, MatchContext context = null)
        //{
        //    if (Id != 0 && Id != specification.Id) return false;

        //    if (Code != null && Code.Equals(specification.Code, StringComparison.OrdinalIgnoreCase) == false) return false;

        //    if (FormattedValue != null && FormattedValue.Equals(specification.FormattedValue) == false) return false;

        //    if (Value != null)
        //    {
        //        object value = specification.Value;
        //        //var value = context != null && context.RequestContext != null ? context.RequestContext.SpecificationFormatter.ValueProvider.GetValue(specification.Model) : SpecificationValueProvider.Default.GetValue(specification.Model);
        //        string valueStr = value != null ? value.ToString() : null;
        //        if (Value.Equals(valueStr) == false) return false;
        //    }

        //    return base.Match(specification, context);
        //}

        //public virtual bool Match(Specification specification, MatchContext context = null)
        //{
        //    if (Code != null)
        //    {
        //        if (specification.SpecificationType != null)
        //        {
        //            if (Code.Equals(specification.SpecificationType.Identity, StringComparison.OrdinalIgnoreCase) == false) return false;
        //        }
        //    }

            

        //    return base.Match(specification, context);
        //}

        //public virtual bool Match(SpecificationType type, MatchContext context = null)
        //{
        //    if (Code != null && Code.Equals(type.Identity, StringComparison.OrdinalIgnoreCase) == false) return false;

        //    return base.Match(type, context);
        //}

        //public bool MatchAny(IEnumerable<ISpecificationIdentifiable> specifications, MatchContext context = null)
        //{
        //    foreach (var specification in specifications)
        //    {
        //        if (Match(specification, context) == true)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //public bool MatchAny(IEnumerable<Specification> specifications, MatchContext context = null)
        //{
        //    foreach (var specification in specifications)
        //    {
        //        if (Match(specification, context) == true)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }

    /// <summary>
    /// An identifier used to identitfy part assembly rows.
    /// </summary>
    public class PartAssemblyRowIdentifier : Identifier
    {

    }

    public class ImageIdentifier : Identifier
    {
        public ImageIdentifier() { }

        /// <summary>
        /// Match against the image persistent identity
        /// </summary>
        /// <value>Attribute name persistent-identity</value>
        [XmlIgnore]
        public int? Id { get; set; }

        /// <summary>
        /// Match against the image persistent identity
        /// </summary>
        /// <value>Attribute name persistent-identity</value>
        [XmlAttribute("persistent-identity")]
        public string PersistentIdentiy { get; set; }

        /// <summary>
        /// Match against the images file name
        /// </summary>
        /// <value>Attribute name filename</value>
        [XmlAttribute("filename")]
        public string FileName { get; set; }

        /// <summary>
        /// Match against the images file name
        /// </summary>
        /// <value>Attribute name filename</value>
        [XmlAttribute("original-filename")]
        public string OriginalFileName { get; set; }

        [XmlAttribute("category")]
        public string Category { get; set; }

        [XmlAttribute("image-for-document-extension")]
        public string ImageForDocumentType { get; set; }

        //[XmlAttribute("excluded-src")]
        //public string ExcludedSourceSerializable { get { return Utility.CodeSetToString(ExcludedSource); } set { ExcludedSource = Utility.CreateOptimizedCodeSet(value); } }

        [XmlIgnore]
        public ISet<string> ExcludedSource { get; set; }

        //public override bool Match(object obj, MatchContext context = null)
        //{
        //    var image = obj as Image;
        //    if (image != null)
        //    {
        //        if (Id.HasValue == true && Id.Value != image.Id)
        //        {
        //            return false;
        //        }

        //        if (PersistentIdentiy != null && StringComparer.Ordinal.Equals(PersistentIdentiy, image.PersistentIdentity) == false)
        //        {
        //            return false;
        //        }

        //        if (FileName != null && StringComparer.OrdinalIgnoreCase.Equals(FileName, image.FileUri(KnownImageFileCodes.Web)) == false)
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        var imageIdentifiable = obj as IImageIdentifiable;
        //        if (imageIdentifiable != null)
        //        {
        //            if (Id.HasValue == true && Id != image.Id)
        //            {
        //                return false;
        //            }

        //            if (Category != null && imageIdentifiable.Categories != null && imageIdentifiable.Categories.Contains(Category) == false)
        //            {
        //                return false;
        //            }

        //            if (ExcludedSource != null && imageIdentifiable.Owner != null && imageIdentifiable.Owner.Any(o => ExcludedSource.Contains(o) == true) == true)
        //            {
        //                return false;
        //            }

        //            var imageViewModelModel = imageIdentifiable as IImageIdentifiable;
        //            if (imageViewModelModel != null)
        //            {
        //                if (PersistentIdentiy != null && StringComparer.Ordinal.Equals(PersistentIdentiy, imageViewModelModel.PersistentIdentity) == false)
        //                {
        //                    return false;
        //                }

        //                if (FileName != null && StringComparer.OrdinalIgnoreCase.Equals(FileName, imageViewModelModel.FileName) == false)
        //                {
        //                    return false;
        //                }
        //            }
        //        }
        //    }

        //    return base.Match(obj, context);
        //}
    }

    /// <summary>
    /// An identifier used to identitfy presentations.
    /// </summary>
    public class PresentationIdentifier : Identifier
    {
        /// <summary>
        /// Match againts the presentations id 
        /// </summary>
        /// <value>Attribute name id</value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Match againts the presentations identity 
        /// </summary>
        /// <value>Attribute name identity</value>
        [XmlAttribute("identity")]
        public string Identity { get; set; }

        [XmlAttribute("identity-regex")]
        public string IdentiyRegexPattern { get; set; }

        private Regex identiyRegex;
        [XmlIgnore]
        public Regex IdentiyRegex
        {
            get { return identiyRegex ?? (identiyRegex = string.IsNullOrEmpty(IdentiyRegexPattern) == false ? new Regex(IdentiyRegexPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled) : null); }
        }


        /// <summary>
        /// Match against the presentations logical identity (label) 
        /// </summary>
        /// <value>Attribute name logical-identity</value>
        [XmlAttribute("label")]
        public string LogicalIdentity { get; set; }

        [XmlAttribute("label-regex")]
        public string LogicalIdentityRegexPattern { get; set; }

        private Regex logicalIdentityRegex;
        [XmlIgnore]
        public Regex LogicalIdentityRegex
        {
            get { return logicalIdentityRegex ?? (logicalIdentityRegex = string.IsNullOrEmpty(LogicalIdentityRegexPattern) == false ? new Regex(LogicalIdentityRegexPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled) : null); }
        }

        /// <summary>
        /// Match against the presentations persistent identity
        /// </summary>
        /// <value>Attribute name persistent-identity</value>
        [XmlAttribute("persistent-identity")]
        public string PersistentIdentity { get; set; }

        [XmlAttribute("persistent-identity-regex")]
        public string PersistentIdentiyRegexPattern { get; set; }

        private Regex persistentIdentiyRegex;
        [XmlIgnore]
        public Regex PersistentIdentiyRegex
        {
            get { return persistentIdentiyRegex ?? (persistentIdentiyRegex = string.IsNullOrEmpty(PersistentIdentiyRegexPattern) == false ? new Regex(PersistentIdentiyRegexPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled) : null); }
        }

        /// <summary>
        /// Match against the presentation type by the types code. 
        /// </summary>
        /// <value>Attribute name type</value>
        [XmlAttribute("type")]
        public string TypeCode { get; set; }

        [XmlIgnore]
        public int? TypeId { get; set; }

        /// <summary>
        /// Match against the presentations base type by the base types name.
        /// </summary>
        /// <value>Attribute name base-type</value>
        [XmlAttribute("base-type")]
        public string BaseType
        {
            get { return null; }
            set
            {

                BaseTypeId = null;
            }
        }

        /// <summary>
        /// Match against the presentations mode.
        /// </summary>
        /// <value>Attribute name mode</value>
        [XmlAttribute("mode")]
        public string ModeName
        {
            get { return  null; }
            set { Mode = (long?)null; }
        }

        [XmlIgnore]
        public long? Mode { get; set; }

        [XmlIgnore]
        public int? BaseTypeId { get; set; }

        [XmlIgnore]
        public int? AssociatedPartId { get; set; } // Not used now

        [XmlIgnore]
        public int? AssociatedPresentationId { get; set; } // Not used now

        //[XmlAttribute("associated-part-number")]
        //public string AssociatedPartNumber { get; set; }

        /// <summary>
        /// Match against the presentations part number.
        /// </summary>
        /// <value>Attribute name part-number</value>
        [XmlAttribute("part-number")]
        public string PartNumber { get; set; }

        [XmlAttribute("part-number-regex")]
        public string PartNumberRegexPattern { get; set; }

        private Regex partNumberRegex;
        [XmlIgnore]
        public Regex PartNumberRegex
        {
            get { return partNumberRegex ?? (partNumberRegex = string.IsNullOrEmpty(PartNumberRegexPattern) == false ? new Regex(PartNumberRegexPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled) : null); }
        }

        /// <summary>
        /// Match against that the presentation has a part number.
        /// </summary>
        /// <value>Attribute name has-part-number</value>
        [XmlAttribute("has-part-number")]
        public string HasPartNumberSerializable { get { return HasPartNumber.HasValue == true ? HasPartNumber.Value.ToString() : null; } set { HasPartNumber = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? HasPartNumber { get; set; }


        /// <summary>
        /// Match against that the presentation has a part number.
        /// </summary>
        /// <value>Attribute name has-part-number</value>
        [XmlAttribute("identity-equals-part-number")]
        public string IdentityEqualsPartNumberSerializable { get { return IdentityEqualsPartNumber.HasValue == true ? IdentityEqualsPartNumber.Value.ToString() : null; } set { IdentityEqualsPartNumber = value.ToNullableBool(); } }

        [XmlIgnore]
        public bool? IdentityEqualsPartNumber { get; set; }

        /// <summary>
        /// Match against the presentations specifications.
        /// </summary>
        /// <value>Element name Specification</value>
        [XmlElement("Specification")]
        public SpecificationIdentifier[] Specifications { get; set; }

        /// <summary>
        /// Match against if the presentation is a kit.
        /// </summary>
        /// <value>Element name IsKit</value>
        [XmlAttribute("kit")]
        public bool IsKitSerializable { get { return IsKit ?? false; } set { IsKit = value; } }
        [XmlIgnore]
        public bool? IsKit { get; set; }

        /// <summary>
        /// Match against if the presentation is specified as sellable or not sellable.
        /// </summary>
        /// <value>Element name IsSellable</value>
        [XmlAttribute("sellable")]
        public bool IsSellableSerializable { get { return IsSellable ?? false; } set { IsSellable = value; } }
        [XmlIgnore]
        public bool? IsSellable { get; set; }


        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj) == true) return true;
            var other = obj as PresentationIdentifier;
            if (other == null) return false;
            var stringComparer = StringComparer.OrdinalIgnoreCase;
            var caseStringComparer = StringComparer.Ordinal;
            return Id == other.Id && BaseTypeId == other.BaseTypeId && stringComparer.Equals(TypeCode, other.TypeCode) == true
                && stringComparer.Equals(Identity, other.Identity) == true && caseStringComparer.Equals(IdentiyRegexPattern, other.IdentiyRegexPattern) == true
                && stringComparer.Equals(LogicalIdentity, other.LogicalIdentity) == true && caseStringComparer.Equals(LogicalIdentityRegex, other.LogicalIdentityRegex) == true
                && stringComparer.Equals(PersistentIdentity, other.PersistentIdentity) == true && caseStringComparer.Equals(PersistentIdentiyRegexPattern, other.PersistentIdentiyRegexPattern) == true
                && stringComparer.Equals(PartNumber, other.PartNumber) == true && caseStringComparer.Equals(PartNumberRegexPattern, other.PartNumberRegexPattern) == true
                && Mode != other.Mode && HasPartNumber == other.HasPartNumber && IsKit == other.IsKit && IsSellable == other.IsSellable && IdentityEqualsPartNumber == other.IdentityEqualsPartNumber
                && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var stringComparer = StringComparer.OrdinalIgnoreCase;
            var caseStringComparer = StringComparer.Ordinal;
            return HashHelper.CombineHashCode(
                HashHelper.CombineHashCode(Id.GetHashCode(), BaseTypeId.GetHashCode(), stringComparer.GetHashCode(TypeCode), stringComparer.GetHashCode(Identity), caseStringComparer.GetHashCode(IdentiyRegexPattern), stringComparer.GetHashCode(LogicalIdentity), caseStringComparer.GetHashCode(LogicalIdentityRegex), stringComparer.GetHashCode(PersistentIdentity), caseStringComparer.GetHashCode(PersistentIdentiyRegexPattern))
                , HashHelper.CombineHashCode(stringComparer.GetHashCode(PartNumber), caseStringComparer.GetHashCode(PartNumberRegexPattern), Mode.GetHashCode(), HasPartNumber.GetHashCode(), IsKit.GetHashCode(), IsSellable.GetHashCode(), IdentityEqualsPartNumber.GetHashCode())
                , base.GetHashCode()
            );
        }

        protected object predicateLock = new object();




        protected Func<IPresentationIdentifiable, MatchContext, bool> presentationViewModelPredicate;

        //public virtual bool Match(IPresentationIdentifiable presentationViewModel, MatchContext context = null)
        //{
        //    // Init special predicates
        //    if (presentationViewModelPredicate == null)
        //    {
        //        lock (predicateLock)
        //        {
        //            if (presentationViewModelPredicate == null)
        //            {
        //                var predicates = new List<Func<IPresentationIdentifiable, MatchContext, bool>>();
        //                if (Id != 0) predicates.Add((p, c) => p.Id == Id);
        //                if (Identity != null) predicates.Add((p, c) => p.Identity == Identity);
        //                if (LogicalIdentity != null) predicates.Add((p, c) => p.LogicalIdentity == LogicalIdentity);
        //                if (PersistentIdentity != null) predicates.Add((p, c) => p.PersistentIdentity == PersistentIdentity);
        //                if (IdentiyRegexPattern != null) predicates.Add((p, c) => p.Identity != null && IdentiyRegex.IsMatch(p.Identity));
        //                if (LogicalIdentityRegexPattern != null) predicates.Add((p, c) => p.LogicalIdentity != null && LogicalIdentityRegex.IsMatch(p.LogicalIdentity));
        //                if (PersistentIdentiyRegexPattern != null) predicates.Add((p, c) => p.PersistentIdentity != null && PersistentIdentiyRegex.IsMatch(p.PersistentIdentity));
        //                if (PartNumber != null) predicates.Add((p, c) => PartNumber.Equals(p.PartNumber, StringComparison.OrdinalIgnoreCase));
        //                if (PartNumberRegexPattern != null) predicates.Add((p, c) => p.PartNumber != null && PartNumberRegex.IsMatch(p.PartNumber));
        //                if (HasPartNumber.HasValue == true) predicates.Add((p, c) => string.IsNullOrWhiteSpace(p.PartNumber) != HasPartNumber.Value);
        //                if (IdentityEqualsPartNumber.HasValue == true) predicates.Add((p, c) => StringComparer.OrdinalIgnoreCase.Equals(p.PartNumber, p.Identity) == IdentityEqualsPartNumber.Value);
        //                //if (TypeCode != null) predicates.Add((p, c) => TypeCode.Equals(p.Type.Code, StringComparison.OrdinalIgnoreCase));
        //                //if (TypeId.HasValue == true) predicates.Add((p, c) => p.Type != null && TypeId.Value == p.Type.Id);
        //                //if (BaseTypeId.HasValue == true)
        //                //{
        //                //    int id = BaseTypeId.Value;
        //                //    predicates.Add((p, c) => p.Type.BaseTypeId == id);
        //                //}
        //                if (Mode.HasValue == true)
        //                {
        //                    long mode = Mode.Value;
        //                    predicates.Add((p, c) => mode == p.Mode);
        //                }
        //                if (IsKit.HasValue == true)
        //                {
        //                    bool isKitValue = IsKit.Value;
        //                    predicates.Add((p, c) =>
        //                    {
        //                        return p.IsKit == isKitValue;
        //                        //var partModule = p as PartModuleViewModel;
        //                        //if (partModule != null)
        //                        //{
        //                        //    return partModule.IsKit == IsKit.Value;
        //                        //}
        //                        //return false;
        //                    });
        //                }
        //                if (IsSellable.HasValue == true) predicates.Add((p, c) => p.IsPartSellable == IsSellable);

        //                if (Specifications != null && Specifications.Length > 0)
        //                {
        //                    predicates.Add((p, c) =>
        //                    {
        //                        for (int index = 0; index < Specifications.Length; index++)
        //                        {
        //                            var specificationIdentifier = Specifications[index];
        //                            if (specificationIdentifier.MatchAny(p.Specifications, c) == false)
        //                            {
        //                                return false;
        //                            }
        //                        }
        //                        return true;
        //                    });
        //                }

        //                if (predicates.Count == 0)
        //                {
        //                    presentationViewModelPredicate = (p, c) => true;
        //                }
        //                else if (predicates.Count == 1)
        //                {
        //                    presentationViewModelPredicate = predicates[0];
        //                }
        //                else
        //                {
        //                    var presentationPredicates = predicates.ToArray();
        //                    presentationViewModelPredicate = (p, c) =>
        //                    {
        //                        for (int index = 0; index < presentationPredicates.Length; index++)
        //                        {
        //                            if (predicates[index](p, c) == false)
        //                            {
        //                                return false;
        //                            }
        //                        }
        //                        return true;
        //                    };
        //                }
        //            }
        //        }
        //    }

        //    // Match

        //    if (presentationViewModelPredicate(presentationViewModel, context) == false)
        //    {
        //        return false;
        //    }

        //    return base.Match(presentationViewModel, context);
        //}

       
    }

    /// <summary>
    /// An identifier used to identitfy reference identities.
    /// </summary>
    public class ReferenceIdentityIdentifier : Identifier
    {
        [XmlElement("Presentation")]
        public PresentationIdentifier Presentation { get; set; }

        //[XmlElement("Bulletin")]
        //public Identifier Bulletin { get; set; }

    }

    /// <summary>
    /// An identifier used to identitfy references.
    /// </summary>
    public class ReferenceIdentifier : ReferenceIdentityIdentifier
    {
        public ReferenceIdentifier()
        {
            Mode = -1;
        }

        [XmlAttribute("mode")]
        public long Mode { get; set; }

        //[XmlAttribute("is-document")]
        public bool? IsDocument { get; set; }



        [XmlElement("Identity")]
        public List<ReferenceIdentityIdentifier> Identities { get; set; }

    }

    public class NamedObject
    {
        public NamedObject() { }

        public NamedObject(object obj)
        {
            Object = obj;
        }

        public NamedObject(string name, object obj)
        {
            Name = name;
            Object = obj;
        }

        public string Name { get; set; }

        public object Object { get; set; }
    }

    public interface IImageIdentifiable
    {
        int? Id { get; }

        int SequenceNumber { get; }

        string Caption { get; }

        string ImageCode { get; set; }

        string FileName { get; }

        string PersistentIdentity { get; }

        bool IsVector { get; }

        double Width { get; }

        double Height { get; }

        ISet<string> Categories { get; }

        string[] Owner { get; set; }
    }

    public interface IPresentationIdentifiable
    {
        int Id { get; }

        bool HasIdentity { get; }

        string Identity { get; }

        bool HasPartNumber { get; }

        string PartNumber { get; }

        string LogicalIdentity { get; }

        string PersistentIdentity { get; }

        bool HasName { get; }

        string Name { get; }

        bool HasDescription { get; }

        string Description { get; }

        long Mode { get; }

        string TypeName { get; }


        bool HasFilters { get; }


        string FormattedValidFilters { get; }

        bool HasOrderInformation { get; }

        bool CanOrder { get; }

        bool IsPermitted { get; }


        string FormattedPartNumber { get; }

        bool HidePartNumber { get; }

        bool IsPartSellable { get; }

        string QuantityUnit { get; }


        bool HasAssociatedPart { get; }


        string AdministratedNote { get; set; }


        bool IsKit { get; }

        bool HasKits { get; }

        //IList<IPresentationIdentifiable> Kits { get; }

        bool HasSpecifications { get; }

        IEnumerable<ISpecificationIdentifiable> Specifications { get; }

        bool HasReplacements { get; }

        bool HasPartOptions { get; }

        bool HasUsedIn { get; }

        bool HasFitsIn { get; }

        //IEnumerable<IPresentationIdentifiable> FitsIn { get; }

        bool HasWizardSearch { get; }

    }
}
