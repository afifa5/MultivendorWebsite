using System;
using System.Collections;
using System.Collections.Generic;

namespace MultivendorWebViewer.Components
{
    public sealed class EmptyEnumerator<T> : IEnumerator<T>
    {
        public T Current
        {
            get { throw new InvalidOperationException("Enum not started"); }
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset() { }

        static EmptyEnumerator()
        {
            EmptyEnumerator<T>.Default = new EmptyEnumerator<T>();
        }

        public static EmptyEnumerator<T> Default { get; private set; }
    }

    public sealed class EmptyReadOnlyCollection<T> : ICollection<T>
    {
        public int Count { get { return 0; } }

        public bool IsReadOnly { get { return true; } }

        public void Add(T item)
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
            // Nothing to do
        }

        public IEnumerator<T> GetEnumerator()
        {
            return System.Linq.Enumerable.Empty<T>().GetEnumerator();
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return System.Linq.Enumerable.Empty<T>().GetEnumerator();
        }

        static EmptyReadOnlyCollection()
        {
            Default = new EmptyReadOnlyCollection<T>();
        }

        public static EmptyReadOnlyCollection<T> Default { get; private set; }
    }

    //public sealed class ConcatEnumerator<T> : IEnumerator<T>
    //{
    //    private IEnumerable<IEnumerable<T>> sequences;

    //    private IEnumerator<T> currentEnumerator

    //    public T Current
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public void Dispose()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    object IEnumerator.Current
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public bool MoveNext()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Reset()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
