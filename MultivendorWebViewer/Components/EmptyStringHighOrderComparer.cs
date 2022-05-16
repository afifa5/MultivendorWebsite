using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Components
{
    /// <summary>
    /// Returns -1 instead of 1 if y is IsNullOrEmpty when x is Not.
    /// </summary>
    public class EmptyStringHighOrderComparer : IComparer<string>
    {
        public EmptyStringHighOrderComparer()
        {
            StringComparer = System.StringComparer.CurrentCulture;
        }

        public EmptyStringHighOrderComparer(IComparer<string> stringComparer)
        {
            StringComparer = stringComparer ?? System.StringComparer.CurrentCulture;
        }

        public IComparer<string> StringComparer { get; set; }

        public int Compare(string x, string y)
        {
            if (String.IsNullOrEmpty(y) && !String.IsNullOrEmpty(x))
            {
                return -1;
            }
            else if (!String.IsNullOrEmpty(y) && String.IsNullOrEmpty(x))
            {
                return 1;
            }
            else
            {
                return StringComparer.Compare(x, y);
            }
        }
    }

    public class GenericObjectComparer : IComparer<object>, IComparer
    {
        public GenericObjectComparer()
        {
            StringComparer = System.StringComparer.CurrentCulture;
        }

        public GenericObjectComparer(IComparer<string> stringComparer, IFormatProvider formatProvider = null, string format = null)
        {
            StringComparer = stringComparer ?? System.StringComparer.CurrentCulture;
            FormatProvider = formatProvider;
            Format = format;
        }

        public IComparer<string> StringComparer { get; set; }

        public IFormatProvider FormatProvider { get; set; }

        public string Format { get; set; }

        public int Compare(object x, object y)
        {
            if (x == null)
            {
                return y == null ? 0 : 1;
            }
            else if (y == null) return -1;

            // If one of the object is a string, use string compare. Otherwise we might end up in converting to string that can throw exception
            string xString = x as string;
            if (xString != null)
            {
                string yString = y as string;
                if (yString != null)
                {
                    return StringComparer.Compare(xString, yString);
                }

                var yFormattable = y as IFormattable;
                yString = yFormattable != null ? yFormattable.ToString(Format, FormatProvider) : y.ToString();
                return StringComparer.Compare(xString, yString);
            }
            else
            {
                string yString = y as string;
                if (yString != null)
                {
                    var xFormattable = x as IFormattable;
                    xString = xFormattable != null ? xFormattable.ToString(Format, FormatProvider) : x.ToString();
                    return StringComparer.Compare(xString, yString);
                }
            }

            // No object was a string

            var xComparable = x as IComparable;
            if (xComparable != null)
            {
                var xType = x.GetType();
                // Is the object of same type? Just compare
                if (xType == y.GetType())
                {
                    return xComparable.CompareTo(y);
                }

                // Can we convert the other object? Only number conversions is intresting
                var yConvertible = y as IConvertible;
                if (yConvertible != null)
                {
                    var xConvertible = x as IConvertible;
                    if (xConvertible != null && xType.IsEnum == false)
                    {
                        var xTypeCode = xConvertible.GetTypeCode();
                        if (IsNumberTypeCode(xTypeCode) == true)
                        {
                            object convertedToNumber = TryChangeNumberType(yConvertible, xTypeCode, FormatProvider);
                            if (convertedToNumber != null)
                            {
                                return xComparable.CompareTo(convertedToNumber);
                            }
                        }
                    }
                }

                // If a number compare was not possible
                //return xComparable.CompareTo(y); // Just compare, maybe the implementor of IComparable can handle it. Or fall back to string compare

            }
            else
            {
                var yComparable = y as IComparable;
                if (yComparable != null)
                {
                    // Can we convert the other object? Only number conversions is intresting
                    var xConvertible = x as IConvertible;
                    if (xConvertible != null)
                    {
                        var yConvertible = y as IConvertible;
                        if (yConvertible != null && (y is Enum) == false)
                        {
                            var yTypeCode = yConvertible.GetTypeCode();
                            if (IsNumberTypeCode(yTypeCode) == true)
                            {
                                object convertedToNumber = TryChangeNumberType(xConvertible, yTypeCode, FormatProvider);
                                if (convertedToNumber != null)
                                {
                                    return -yComparable.CompareTo(convertedToNumber);
                                }
                            }
                        }
                    }


                    // If a number compare was not possible
                    //return -yComparable.CompareTo(x); // Just compare, maybe the implementor of IComparable can handle it. Or fall back to string compare
                }
            }

            // If we are here, no object is comparable and of same type or number convertible.

            var xFallbackFormattable = x as IFormattable;
            string xFallbackString = xFallbackFormattable != null ? xFallbackFormattable.ToString(Format, FormatProvider) : x.ToString();

            var yFallbackFormattable = y as IFormattable;
            string yFallbackString = yFallbackFormattable != null ? yFallbackFormattable.ToString(Format, FormatProvider) : y.ToString();

            return StringComparer.Compare(xFallbackString, yFallbackString);
        }

        public static bool IsNumberTypeCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return false;
                case TypeCode.Char:
                    return false;
                case TypeCode.SByte:
                    return true;
                case TypeCode.Byte:
                    return true;
                case TypeCode.Int16:
                    return true;
                case TypeCode.UInt16:
                    return true;
                case TypeCode.Int32:
                    return true;
                case TypeCode.UInt32:
                    return true;
                case TypeCode.Int64:
                    return true;
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Single:
                    return true;
                case TypeCode.Double:
                    return true;
                case TypeCode.Decimal:
                    return true;
                case TypeCode.DateTime:
                    return false;
                case TypeCode.String:
                    return false;
                case TypeCode.Object:
                    return false;
                case TypeCode.DBNull:
                    return false;
                case TypeCode.Empty:
                    return false;
                default:
                    return false;
            }
        }

        public static object TryChangeNumberType(IConvertible v, TypeCode typeCode, IFormatProvider provider)
        {
            switch (typeCode)
            {
                //case TypeCode.Boolean:
                //    return v.ToBoolean(provider);
                //case TypeCode.Char:
                //    return v.ToChar(provider);
                case TypeCode.SByte:
                    return v.ToSByte(provider);
                case TypeCode.Byte:
                    return v.ToByte(provider);
                case TypeCode.Int16:
                    return v.ToInt16(provider);
                case TypeCode.UInt16:
                    return v.ToUInt16(provider);
                case TypeCode.Int32:
                    return v.ToInt32(provider);
                case TypeCode.UInt32:
                    return v.ToUInt32(provider);
                case TypeCode.Int64:
                    return v.ToInt64(provider);
                case TypeCode.UInt64:
                    return v.ToUInt64(provider);
                case TypeCode.Single:
                    return v.ToSingle(provider);
                case TypeCode.Double:
                    return v.ToDouble(provider);
                case TypeCode.Decimal:
                    return v.ToDecimal(provider);
                //case TypeCode.DateTime:
                //    return v.ToDateTime(provider);
                //case TypeCode.String:
                //    return v.ToString(provider);
                //case TypeCode.Object:
                //    return value;
                //case TypeCode.DBNull:
                //    throw new InvalidCastException(Environment.GetResourceString("InvalidCast_DBNull"));
                //case TypeCode.Empty:
                //    throw new InvalidCastException(Environment.GetResourceString("InvalidCast_Empty"));
                default:
                    return null;
                    //throw new ArgumentException(Environment.GetResourceString("Arg_UnknownTypeCode"));
            }
        }
    }

    public class StringGroupComparer : IComparer<string>
    {
        public StringGroupComparer()
        {
            StringComparer = System.StringComparer.CurrentCulture;
        }

        public StringGroupComparer(IComparer<string> stringComparer)
        {
            StringComparer = stringComparer ?? System.StringComparer.CurrentCulture;
        }

        public IComparer<string> StringComparer { get; set; }

        public int Compare(string x, string y)
        {
            int xGroupIndex = 0;
            int yGroupIndex = 0;
            Group xGroup = null;
            Group yGroup = null;
            do
            {
                xGroup = GetGroup(x, 0);
                yGroup = GetGroup(y, 0);

                if (xGroup == null)
                {
                    if (yGroup == null) return 0;
                    return -1;
                }
                else if (yGroup == null)
                {
                    return 1;
                }

                if (xGroup.Number.HasValue == true)
                {
                    if (yGroup.Number.HasValue == false) return -1;
                    if (xGroup.Number.Value == yGroup.Number.Value)
                    {
                        if (xGroup.LeadingZeros == yGroup.LeadingZeros)
                        {
                            continue;
                        }
                        return xGroup.LeadingZeros < yGroup.LeadingZeros ? -1 : 1;
                    }
                    return xGroup.Number.Value < yGroup.Number.Value ? -1 : 1;
                }
                else if (yGroup.Number.HasValue == true)
                {
                    return 1;
                }

                int stringResult = StringComparer.Compare(xGroup.String, yGroup.String);
                if (stringResult != 0)
                {
                    return stringResult;
                }

                xGroupIndex = xGroup.EndIndex;
                yGroupIndex = yGroup.EndIndex;
            } while (1 == 0);
            return 0;
        }

        private Group GetGroup(string str, int startIndex)
        {
            int i = startIndex;
            if (i < str.Length)
            {
                char c = str[i];
                if (char.IsNumber(c) == true) // Number
                {
                    int leadingZero = 0;
                    if (c == '0') leadingZero = 1;             
                    while (++i < str.Length)
                    {
                        c = str[i];
                        if (char.IsNumber(c) == false) break;
                        if (c == '0') leadingZero++;
                    }
                    long number;
                    int length = i - startIndex;
                    string numberString = str.Substring(startIndex, length);
                    if (long.TryParse(numberString, out number) == true)
                    {
                        return new Group { EndIndex = i, Number = number, LeadingZeros = leadingZero };
                    }
                    return new Group { EndIndex = i, String = numberString };
                }
                else // Non number
                {
                    do
                    {
                        i++;
                    } while (i < str.Length && char.IsNumber(str, i) == false);
                    int length = i - startIndex;
                    return new Group { EndIndex = i, String = str.Substring(startIndex, length) };
                }
            }
            return null;
        }

        private class Group
        {
            public long? Number { get; set; }

            public int LeadingZeros { get; set; }

            public string String { get; set; }

            public int EndIndex { get; set; }
        }
    }

    public class CollectionSafeComparerWrapper : IComparer<object>, IComparer
    {
        public CollectionSafeComparerWrapper(IComparer comparer = null)
        {
            Comparer = comparer ?? System.Collections.Comparer.Default;
        }

        public IComparer Comparer { get; private set; }

        public int Compare(object x, object y)
        {
            var xEnumerable = x as IEnumerable;
            if (xEnumerable != null && (x is IComparable) == false)
            {
                x = xEnumerable.FirstOrDefaultNonGeneric();
            }

            var yEnumerable = y as IEnumerable;
            if (yEnumerable != null && (y is IComparable) == false)
            {
                y = yEnumerable.FirstOrDefaultNonGeneric();
            }

            return Comparer.Compare(x, y);
        }
    }

    public class CollectionSafeComparerWrapper<T> : IComparer<T>
    {
        public CollectionSafeComparerWrapper(IComparer<T> comparer)
        {
            Comparer = comparer ?? Comparer<T>.Default;
        }

        public IComparer<T> Comparer { get; private set; }

        public int Compare(T x, T y)
        {
            var xEnumerable = x as IEnumerable;
            if (xEnumerable != null && (x is IComparable<T>) == false)
            {
                var TEnumerable = x as IEnumerable<T>;
                if (TEnumerable != null)
                {
                    x = TEnumerable.FirstOrDefault();
                }
                else
                {
                    object xobj = xEnumerable.FirstOrDefaultNonGeneric();
                    x = xobj is T ? (T)xobj : default(T);
                }
            }

            var yEnumerable = y as IEnumerable<T>;
            if (yEnumerable != null && (y is IComparable<T>) == false)
            {
                var TEnumerable = x as IEnumerable<T>;
                if (TEnumerable != null)
                {
                    y = TEnumerable.FirstOrDefault();
                }
                else
                {
                    object yobj = yEnumerable.FirstOrDefaultNonGeneric();
                    y = yobj is T ? (T)yobj : default(T);
                }
            }

            return Comparer.Compare(x, y);
        }
    }
}
