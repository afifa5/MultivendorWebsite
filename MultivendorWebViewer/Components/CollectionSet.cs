using MultivendorWebViewer.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Components
{
    public sealed class CollectionSet<T> : ISet<T>
    {
        private ICollection<T> items;

        private IEqualityComparer<T> comparer;

        public CollectionSet(IEnumerable<T> items, IEqualityComparer<T> comparer = null)
        {
            if (items == null) throw new ArgumentNullException("items");

            this.items = items as ICollection<T> ?? items.ToArray();
            this.comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public bool Add(T item)
        {
            int count = items.Count;
            items.Add(item);
            return count != items.Count;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            var expect = items.Except(other, comparer).ToArray();
            items.Clear();
            items.AddRange(expect);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            var intersection = items.Intersect(other, comparer).ToArray();
            items.Clear();
            items.AddRange(intersection);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (items.Count == 0) return false;

            var otherSet = other as HashSet<T>;

            if (otherSet != null)
            {
                return otherSet.Overlaps(items);
            }

            foreach (var item in items)
            {
                foreach (var otherItem in other)
                {
                    if (comparer.Equals(item, otherItem) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return other.SequenceEqual(items, comparer);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            items.AddRange(other.Except(items, comparer).ToArray());
        }

        void ICollection<T>.Add(T item)
        {
            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(T item)
        {
            return items.Contains(item, comparer);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return items.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }

    public sealed class SingleSet<T> : ISet<T>
    {
        private T item;

        private IEqualityComparer<T> comparer;

        public SingleSet(T item, IEqualityComparer<T> comparer = null)
        {            
            if (item == null) throw new ArgumentNullException("item");

            this.item = item;
            this.comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public bool Add(T item)
        {
            throw new NotSupportedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return other.Contains(item, comparer);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return other.HasCount(1) == true && comparer.Equals(item, other.First()) == true; 
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return comparer.Equals(this.item, item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            array[arrayIndex] = item;
        }

        public int Count
        {
            get { return 1; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SingleItemEnumerator<T>(item);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SingleItemEnumerator<T>(item);
        }
    }

    public sealed class EmptySet<T> : ISet<T>
    {
        public bool Add(T item)
        {
            throw new NotSupportedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return false;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return other.Any() == false;
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            // Nothing to copy
        }

        public int Count
        {
            get { return 0; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return EmptyEnumerator<T>.Default;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return EmptyEnumerator<T>.Default;
        }

        static EmptySet()
        {
            Default = new EmptySet<T>();
        }

        public static EmptySet<T> Default { get; private set; }
    }

    public sealed class CollectionReadOnlySet<T> : ISet<T>
    {
        private T[] items;

        public CollectionReadOnlySet(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            this.items = items as T[] ?? items.ToArray();
        }

        public bool Add(T item)
        {
            throw new NotSupportedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (items.Length == 0) return false;

            var otherSet = other as ISet<T>;

            if (otherSet != null)
            {
                return otherSet.Overlaps(items);
            }

            foreach (var item in items)
            {
                foreach (var otherItem in other)
                {
                    if (item.Equals(otherItem) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return other.SequenceEqual(items);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Equals(item) == true)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return items.Length; }
        }

        public bool IsReadOnly
        {
            get { return items.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((ICollection<T>)items).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }

    public sealed class SingleReadOnlyIntSet : ISet<int>
    {
        private int item;

        public SingleReadOnlyIntSet(int item)
        {
            this.item = item;
        }

        public bool Add(int item)
        {
            throw new NotSupportedException();
        }

        public void ExceptWith(IEnumerable<int> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<int> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<int> other)
        {
            return other.Contains(item) == false;
        }

        public bool IsSupersetOf(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<int> other)
        {
            return other.Contains(item);
        }

        public bool SetEquals(IEnumerable<int> other)
        {
            return other.HasCount(1) == true && item == other.First() == true;
        }

        public void SymmetricExceptWith(IEnumerable<int> other)
        {
            throw new NotSupportedException();
        }

        public void UnionWith(IEnumerable<int> other)
        {
            throw new NotSupportedException();
        }

        void ICollection<int>.Add(int item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(int item)
        {
            return this.item == item;
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            array[arrayIndex] = item;
        }

        public int Count
        {
            get { return 1; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(int item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            return new SingleItemEnumerator<int>(item);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SingleItemEnumerator<int>(item);
        }
    }

    public sealed class CollectionReadOnlyIntSet : ISet<int>
    {
        private int[] items;

        public CollectionReadOnlyIntSet(IEnumerable<int> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            this.items = items as int[] ?? items.ToArray();
        } 

        public bool Add(int item)
        {
            throw new NotSupportedException();
        }

        public void ExceptWith(IEnumerable<int> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<int> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<int> other)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (other.Contains(items[i]) == false) return false;
            }
            return true;
        }

        public bool IsSupersetOf(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<int> other)
        {
            if (items.Length == 0) return false;

            var otherSet = other as ISet<int>;

            if (otherSet != null)
            {
                return otherSet.Overlaps(items);
            }

            for (int i = 0; i < items.Length; i++)
            {
                int item = items[i];
                foreach (var otherItem in other)
                {
                    if (item == otherItem)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SetEquals(IEnumerable<int> other)
        {
            return other.SequenceEqual(items);
        }

        public void SymmetricExceptWith(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<int> other)
        {
            throw new NotSupportedException();
        }

        void ICollection<int>.Add(int item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(int item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == item)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return items.Length; }
        }

        public bool IsReadOnly
        {
            get { return items.IsReadOnly; }
        }

        public bool Remove(int item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ((ICollection<int>)items).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
