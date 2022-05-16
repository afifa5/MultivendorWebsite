using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Collections
{
    public sealed class SingleItemEnumerable<T> : IEnumerable<T>
    {
        public SingleItemEnumerable(T item)
        {
            this.item = item;
        }

        private T item;

        public IEnumerator<T> GetEnumerator()
        {
            return new SingleItemEnumerator<T>(item);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SingleItemEnumerator<T>(item);
        }
    }

    public static class SingleItemEnumerable
    {
        public static SingleItemEnumerable<T> Create<T>(T item)
        {
            return new SingleItemEnumerable<T>(item);
        }
    }

    public sealed class SingleItemEnumerator<T> : IEnumerator<T>
    {
        public SingleItemEnumerator(T item)
        {
            this.item = item;
        }

        private T item;

        private bool started;

        public T Current
        {
            get { return item; }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return item; }
        }

        public bool MoveNext()
        {
            if (started == false)
            {
                started = true;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            started = false;
        }
    }
}
