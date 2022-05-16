using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Components
{
    /// <summary>
	/// Compares two arrays to see if the values inside of the array are the same. This is
	/// dependent on the type contained in the array having a valid Equals() override.
	/// </summary>
	/// <typeparam name="T">The type of data stored in the array</typeparam>
	public class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        private IEqualityComparer<T> itemComparer;

        public EnumerableEqualityComparer(IEqualityComparer<T> itemComparer = null)
        {
            this.itemComparer = itemComparer ?? EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Gets the hash code for the contents of the array since the default hash code
        /// for an array is unique even if the contents are the same.
        /// </summary>
        /// <remarks>
        /// See Jon Skeet (C# MVP) response in the StackOverflow thread 
        /// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        /// </remarks>
        /// <param name="array">The array to generate a hash code for.</param>
        /// <returns>The hash code for the values in the array.</returns>
        public int GetHashCode(IEnumerable<T> items)
        {
            // if non-null array then go into unchecked block to avoid overflow
            if (items != null)
            {
                unchecked
                {
                    int hash = 17;

                    // get hash code for all items in array
                    foreach (var item in items)
                    {
                        hash = hash * 23 + ((item != null) ? itemComparer.GetHashCode(item) : 0);
                    }

                    return hash;
                }
            }

            // if null, hash code is zero
            return 0;
        }

        /// <summary>
        /// Compares the contents of both enumerables to see if they are equal. This depends on 
        /// typeparameter T having a valid override for Equals().
        /// </summary>
        /// <param name="first">The first enumerable to compare.</param>
        /// <param name="second">The second enumerable to compare.</param>
        /// <returns>True if enumerable and enumerable have equal contents.</returns>
        public bool Equals(IEnumerable<T> first, IEnumerable<T> second)
        {
            // if same reference or both null, then equality is true
            if (object.ReferenceEquals(first, second))
            {
                return true;
            }

            // If one is null, return false
            if (first == null || second == null) return false;

            // If both are collections of different size, return false
            var firstCollection = first as ICollection<T>;
            if (firstCollection != null)
            {
                var secondCollection = second as ICollection<T>;
                if (secondCollection != null && firstCollection.Count != secondCollection.Count) return false;
            }

            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (!(e2.MoveNext() && itemComparer.Equals(e1.Current, e2.Current))) return false;
                }
                if (e2.MoveNext()) return false;
            }
            return true;

            //var firstCollection = first as IList<T> ?? first.ToArray();
            //var secondCollection = second as IList<T> ?? second.ToArray();

            //// otherwise, if both arrays have same length, compare all elements
            //if (firstCollection.Count == secondCollection.Count)
            //{
            //    for (int i = 0, c = firstCollection.Count; i < c; i++)
            //    {
            //        // if any mismatch, not equal
            //        if (itemComparer.Equals(firstCollection[i], secondCollection[i]) == false)
            //        {
            //            return false;
            //        }
            //    }

            //    // if no mismatches, equal
            //    return true;
            //}

            //// if we get here, they are not equal
            //return false;
        }
    }
}
