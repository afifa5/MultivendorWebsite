using MultivendorWebViewer.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Components
{
    public sealed class EmptyReadOnlyDictionary<TKey, TElement> : IDictionary<TKey, TElement>
    {
        public int Count
        {
            get { return 0; }
        }

        public void Add(TKey key, TElement value)
        {
            throw new NotSupportedException("The dictionary is read only");
        }

        public bool ContainsKey(TKey key)
        {
            return false;
        }

        public ICollection<TKey> Keys
        {
            get { return EmptyReadOnlyDictionary<TKey, TElement>.EmptyKeys; }
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException("The dictionary is read only");
        }

        public bool TryGetValue(TKey key, out TElement value)
        {
            value = default(TElement);
            return false;
        }

        public ICollection<TElement> Values
        {
            get { return EmptyReadOnlyDictionary<TKey, TElement>.EmptyValues; }
        }

        TElement IDictionary<TKey, TElement>.this[TKey key]
        {
            get
            {
                throw new KeyNotFoundException();
            }
            set
            {
                throw new NotSupportedException("The dictionary is read only");
            }
        }

        public void Add(KeyValuePair<TKey, TElement> item)
        {
            throw new NotSupportedException("The dictionary is read only");
        }

        public void Clear() 
        {
            throw new NotSupportedException("The dictionary is read only");
        }

        public bool Contains(KeyValuePair<TKey, TElement> item)
        {
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TElement>[] array, int arrayIndex)
        {
            // Nothing to copy
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(KeyValuePair<TKey, TElement> item)
        {
            throw new NotSupportedException("The dictionary is read only");
        }

        IEnumerator<KeyValuePair<TKey, TElement>> IEnumerable<KeyValuePair<TKey, TElement>>.GetEnumerator()
        {
            return EmptyEnumerator<KeyValuePair<TKey, TElement>>.Default;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return EmptyEnumerator<KeyValuePair<TKey, TElement>>.Default;
        }

        static EmptyReadOnlyDictionary()
        {
            EmptyKeys = new TKey[0];
            EmptyValues = new TElement[0];
            Default = new EmptyReadOnlyDictionary<TKey, TElement>();
        }

        public static EmptyReadOnlyDictionary<TKey, TElement> Default { get; private set; }

        private static TKey[] EmptyKeys;

        private static TElement[] EmptyValues;
    }

    public sealed class SingleReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private TKey key;
        
        private TValue value;

        private IEqualityComparer<TKey> comparer;

        public SingleReadOnlyDictionary(TKey key, TValue value, IEqualityComparer<TKey> comparer = null)
        {
            this.key = key;
            this.value = value;
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public SingleReadOnlyDictionary(KeyValuePair<TKey, TValue> kvp, IEqualityComparer<TKey> comparer = null)
        {
            this.key = kvp.Key;
            this.value = kvp.Value;
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public int Count
        {
            get { return 1; }
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException("The dictionary is read only");
        }
        
        public bool ContainsKey(TKey key)
        {
            return comparer.Equals(this.key, key);
        }

        public ICollection<TKey> Keys
        {
            get { return new [] { key }; }
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException("The dictionary is read only");
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (comparer.Equals(this.key, key) == true)
            {
                value = this.value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        public ICollection<TValue> Values
        {
            get { return new [] { value }; }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                if (comparer.Equals(this.key, key) == true) return this.value;
                throw new KeyNotFoundException();
            }
            set
            {
                throw new NotSupportedException("The dictionary is read only");
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("The dictionary is read only");
        }

        public void Clear() 
        {
            throw new NotSupportedException("The dictionary is read only");
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return comparer.Equals(this.key, key) == true && object.Equals(item, this.value) == true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            array[arrayIndex] = new KeyValuePair<TKey,TValue>(key, value);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("The dictionary is read only");
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new SingleItemEnumerator<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SingleItemEnumerator<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
        }
    }

    //public sealed class ArrayReadOnlytDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    //{
    //    private KeyValuePair<TKey, TValue>[] entries;

    //    private IEqualityComparer<TKey> comparer;

    //    public SingleReadOnlyDictionary(TKey key, TValue value, IEqualityComparer<TKey> comparer = null)
    //    {
    //        this.key = key;
    //        this.value = value;
    //        this.comparer = comparer ?? EqualityComparer<TKey>.Default;
    //    }

    //    public SingleReadOnlyDictionary(KeyValuePair<TKey, TValue> kvp, IEqualityComparer<TKey> comparer = null)
    //    {
    //        this.key = kvp.Key;
    //        this.value = kvp.Value;
    //        this.comparer = comparer ?? EqualityComparer<TKey>.Default;
    //    }

    //    public int Count
    //    {
    //        get { return 1; }
    //    }

    //    public void Add(TKey key, TValue value)
    //    {
    //        throw new NotSupportedException("The dictionary is read only");
    //    }

    //    public bool ContainsKey(TKey key)
    //    {
    //        return comparer.Equals(this.key, key);
    //    }

    //    public ICollection<TKey> Keys
    //    {
    //        get { return new[] { key }; }
    //    }

    //    public bool Remove(TKey key)
    //    {
    //        throw new NotSupportedException("The dictionary is read only");
    //    }

    //    public bool TryGetValue(TKey key, out TValue value)
    //    {
    //        if (comparer.Equals(this.key, key) == true)
    //        {
    //            value = this.value;
    //            return true;
    //        }
    //        value = default(TValue);
    //        return false;
    //    }

    //    public ICollection<TValue> Values
    //    {
    //        get { return new[] { value }; }
    //    }

    //    TValue IDictionary<TKey, TValue>.this[TKey key]
    //    {
    //        get
    //        {
    //            if (comparer.Equals(this.key, key) == true) return this.value;
    //            throw new KeyNotFoundException();
    //        }
    //        set
    //        {
    //            throw new NotSupportedException("The dictionary is read only");
    //        }
    //    }

    //    public void Add(KeyValuePair<TKey, TValue> item)
    //    {
    //        throw new NotSupportedException("The dictionary is read only");
    //    }

    //    public void Clear()
    //    {
    //        throw new NotSupportedException("The dictionary is read only");
    //    }

    //    public bool Contains(KeyValuePair<TKey, TValue> item)
    //    {
    //        return comparer.Equals(this.key, key) == true && object.Equals(item, this.value) == true;
    //    }

    //    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    //    {
    //        array[arrayIndex] = new KeyValuePair<TKey, TValue>(key, value);
    //    }

    //    public bool IsReadOnly
    //    {
    //        get { return true; }
    //    }

    //    public bool Remove(KeyValuePair<TKey, TValue> item)
    //    {
    //        throw new NotSupportedException("The dictionary is read only");
    //    }

    //    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    //    {
    //        return new SingleItemEnumerator<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
    //    }

    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return new SingleItemEnumerator<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
    //    }
    //}

}
