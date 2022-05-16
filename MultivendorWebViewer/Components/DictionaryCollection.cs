using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MultivendorWebViewer.Components
{
    public interface IListDictionary<TKey, TItem> : IDictionary<TKey, TItem>, IList<TItem> { }

    public abstract class DictionaryCollection<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
        public DictionaryCollection() { }

        public DictionaryCollection(IEqualityComparer<TKey> comparer) : base(comparer) { }

        public DictionaryCollection(int dictionaryCreationThreshold) : base(null, dictionaryCreationThreshold) { }

        public DictionaryCollection(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold) : base(comparer, dictionaryCreationThreshold) { }

        public DictionaryCollection(IEnumerable<TItem> items)
        {
            AddRange(items);
        }

        public DictionaryCollection(IEnumerable<TItem> items, IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
            AddRange(items);
        }

        public DictionaryCollection(IEnumerable<TItem> items, IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
            : base(comparer, dictionaryCreationThreshold)
        {
            AddRange(items);
        }

        public virtual new TItem this[TKey key]
        {
            get { return GetItem(key); }
        }

        public void AddOrReplace(TItem item)
        {
            TKey key = GetKeyForItem(item);

            int index = IndexOf(key);
            if (index != -1)
            {
                SetItem(index, item);
            }
            else
            {
                Add(item);
            }
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            foreach (TItem item in items)
            {
                Add(item);
            }
        }

        public void MergeRange(IEnumerable<TItem> items, bool replaceConflictingItems = true)
        {
            foreach (TItem item in items)
            {
                TKey itemKey = GetKeyForItem(item);
                if (Contains(itemKey) == true)
                {
                    if (replaceConflictingItems == true)
                    {
                        Remove(itemKey);
                        Add(item);
                    }
                    else
                    {

                    }

                }
                else
                {
                    Add(item);
                }
            }
        }

        public void InsertRange(int index, IEnumerable<TItem> items)
        {
            foreach (TItem item in items)
            {
                Insert(index++, item);
            }
        }

        public void RemoveRange(IEnumerable<TItem> items)
        {
            foreach (TItem item in items)
            {
                Remove(item);
            }
        }

        public int IndexOf(TKey key)
        {
            TItem item = GetItem(key);
            return item != null ? IndexOf(item) : -1;
        }

        public TItem GetItem(TKey key)
        {
            if (Dictionary != null && key != null)
            {
                TItem item;
                if (Dictionary.TryGetValue(key, out item) == true)
                {
                    return item;
                }
                return default(TItem);
            }
            else
            {
                foreach (TItem item in Items)
                {
                    if (key != null && Comparer.Equals(GetKeyForItem(item), key) == true)
                    {
                        return item;
                    }
                }
            }
            return default(TItem);
        }

        public bool TryGetValue(TKey key, out TItem item)
        {
            if (Dictionary != null)
            {
                return Dictionary.TryGetValue(key, out item);
            }
            else
            {
                foreach (TItem existingItem in Items)
                {
                    if (Comparer.Equals(GetKeyForItem(existingItem), key) == true)
                    {
                        item = existingItem;

                        return true;
                    }
                }

                item = default(TItem);

                return false;
            }
        }

        public void Move(TKey key, int toIndex)
        {
            int fromIndex = IndexOf(key);
            if (fromIndex != -1)
            {
                Items.Move(fromIndex, toIndex);
            }
        }

        public void InsertBefore(TKey key, TItem item)
        {
            int index = IndexOf(key);
            if (index != -1)
            {
                Insert(index, item);
            }
        }

        public void InsertAfter(TKey key, TItem item)
        {
            int index = IndexOf(key);
            if (index != -1)
            {
                Insert(index + 1, item);
            }
        }

        public IDictionary<TKey, TItem> AsDictionary()
        {
            return new DictionaryCollectionProxy(this);
        }

        internal class DictionaryCollectionProxy : IDictionary<TKey, TItem>
        {
            public DictionaryCollectionProxy(DictionaryCollection<TKey, TItem> collection)
            {
                Collection = collection;
            }

            public DictionaryCollection<TKey, TItem> Collection { get; private set; }

            public void Add(TKey key, TItem value)
            {
                ValidateKeyArgument(key, value);

                Collection.Add(value);
            }

            public bool ContainsKey(TKey key)
            {
                return Collection.Contains(key);
            }

            public ICollection<TKey> Keys
            {
                get { return Collection.Dictionary != null ? Collection.Dictionary.Keys : Collection.Items.Select(item => Collection.GetKeyForItem(item)).ToArray(); }
            }

            public bool Remove(TKey key)
            {
                return Collection.Remove(key);
            }

            public bool TryGetValue(TKey key, out TItem value)
            {
                return Collection.TryGetValue(key, out value);
            }

            public ICollection<TItem> Values
            {
                get { return Collection.Items; }
            }

            public TItem this[TKey key]
            {
                get { return Collection[key]; }
                set { Collection.Add(value); }
            }

            public void Add(KeyValuePair<TKey, TItem> item)
            {
                Add(item.Key, item.Value);
            }

            public void Clear()
            {
                Collection.Clear();
            }

            public bool Contains(KeyValuePair<TKey, TItem> item)
            {
                ValidateKeyArgument(item.Key, item.Value);

                return Collection.Contains(item.Value);
            }

            public void CopyTo(KeyValuePair<TKey, TItem>[] array, int arrayIndex)
            {
                if (Collection.Dictionary != null)
                {
                    Collection.Dictionary.CopyTo(array, arrayIndex);
                }
                else
                {
                    this.ToArray().CopyTo(array, arrayIndex);
                }
            }

            public int Count
            {
                get { return Collection.Count; }
            }

            public bool IsReadOnly
            {
                get { return Collection.Dictionary != null ? Collection.Dictionary.IsReadOnly : Collection.Items.IsReadOnly; }
            }

            public bool Remove(KeyValuePair<TKey, TItem> item)
            {
                ValidateKeyArgument(item.Key, item.Value);

                return Collection.Remove(item.Value);
            }

            public IEnumerator<KeyValuePair<TKey, TItem>> GetEnumerator()
            {
                return Collection.Dictionary != null ? Collection.Dictionary.GetEnumerator() : GetItemEnumerator();
            }

            private IEnumerator<KeyValuePair<TKey, TItem>> GetItemEnumerator()
            {
                foreach (TItem item in Collection)
                {
                    yield return new KeyValuePair<TKey, TItem>(Collection.GetKeyForItem(item), item);
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return Collection.GetEnumerator();
            }

            private void ValidateKeyArgument(TKey key, TItem item)
            {
                if (key != null && Collection.Comparer.Equals(Collection.GetKeyForItem(item), key) == false)
                {
                    throw new ArgumentException("The supplied key must be null or the same as the calculated key", "key");
                }
            }
        }

        //public static implicit operator IDictionary<TKey, TItem>(DictionaryCollection<TKey, TItem> collection)
        //{
        //    return new DictionaryCollectionProxy(collection);
        //}
    }

    public class DelegateDictionaryCollection<TKey, TItem> : DictionaryCollection<TKey, TItem>
    {
        public DelegateDictionaryCollection(Func<TItem, TKey> keySelector)
        {
            Init(keySelector);
        }

        public DelegateDictionaryCollection(Func<TItem, TKey> keySelector, IEnumerable<TItem> items)
        {
            Init(keySelector);

            AddRange(items);
        }

        public DelegateDictionaryCollection(Func<TItem, TKey> keySelector, IEqualityComparer<TKey> comparer) 
            : base(comparer) 
        {
            Init(keySelector);
        }

        public DelegateDictionaryCollection(Func<TItem, TKey> keySelector, int dictionaryCreationThreshold) 
            : base(null, dictionaryCreationThreshold) 
        {
            Init(keySelector);
        }

        public DelegateDictionaryCollection(Func<TItem, TKey> keySelector, IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold) 
            : base(comparer, dictionaryCreationThreshold) 
        {
            Init(keySelector);
        }

        public DelegateDictionaryCollection(Func<TItem, TKey> keySelector, IEnumerable<TItem> items, IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
            : base(comparer, dictionaryCreationThreshold)
        {
            AddRange(items);
        }

        private void Init(Func<TItem, TKey> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            KeySelector = keySelector;
        }

        protected Func<TItem, TKey> KeySelector { get; set; }

        protected override TKey GetKeyForItem(TItem item)
        {
            return KeySelector(item);
        }
    }

    //public static class DictionaryCollection
    //{
    //    public static DelegateDictionaryCollection<TKey, TItem> Create<TKey, TItem>(Func<TItem, TKey> keySelector)
    //    {
    //        return new DelegateDictionaryCollection<TKey, TItem>(keySelector);
    //    }

    //    public static DelegateDictionaryCollection<TKey, TItem> Create<TKey, TItem>(Func<TItem, TKey> keySelector, IEnumerable<TItem> items)
    //    {
    //        return new DelegateDictionaryCollection<TKey, TItem>(keySelector, items);
    //    }

    //    public static DelegateDictionaryCollection<TKey, TItem> Create<TKey, TItem>(Func<TItem, TKey> keySelector, IEqualityComparer<TKey> comparer)
    //    {
    //        return new DelegateDictionaryCollection<TKey, TItem>(keySelector, comparer);
    //    }

    //    public static DictionaryCollection<TKey, TItem> Create<TKey, TItem>(Func<TItem, TKey> keySelector, int dictionaryCreationThreshold)
    //    {
    //        return new DelegateDictionaryCollection<TKey, TItem>(keySelector, dictionaryCreationThreshold);
    //    }

    //    public static DelegateDictionaryCollection<TKey, TItem> Create<TKey, TItem>(Func<TItem, TKey> keySelector, IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
    //    {
    //        return new DelegateDictionaryCollection<TKey, TItem>(keySelector, comparer, dictionaryCreationThreshold);
    //    }

    //    public static DelegateDictionaryCollection<TKey, TItem> Create<TKey, TItem>(Func<TItem, TKey> keySelector, IEnumerable<TItem> items, IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
    //    {
    //        return new DelegateDictionaryCollection<TKey, TItem>(keySelector, items, comparer, dictionaryCreationThreshold);
    //    }
    //}


    //public class ListDictionary<TKey, TItem> : IListDictionary<TKey, TItem>
    //{

    //}
}
