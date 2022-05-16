using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Components
{
    public class RangeEnumerable<T> : IEnumerable<T>
    {
        public RangeEnumerable(IList<T> list, int startIndex, int length)
        {
            this.list = list;
            this.startIndex = startIndex;
            this.length = length;
        }

        public RangeEnumerable(IList<T> list, int startIndex)
        {
            this.list = list;
            this.startIndex = startIndex;
            this.length = list.Count - startIndex;
        }

        private IList<T> list;
        
        private int startIndex;

        private int length;

        public IEnumerator<T> GetEnumerator()
        {
            return new RangeEnumerator<T>(list, startIndex, length);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RangeEnumerator<T>(list, startIndex, length);
        }
    }

    public class RangeEnumerator<T> : IEnumerator<T>
    {
        public RangeEnumerator(IList<T> list, int startIndex, int length)
        {
            this.list = list;
            this.startIndex = startIndex;
            this.currentIndex = startIndex - 1;
            this.endIndex = Math.Min(list.Count, startIndex + length);
        }

        private IList<T> list;

        private int startIndex;

        private int currentIndex;

        private int endIndex;

        public T Current
        {
            get { return list[currentIndex]; }
        }

        public void Dispose() { }

        object IEnumerator.Current
        {
            get { return list[currentIndex]; }
        }

        public bool MoveNext()
        {
            return ++currentIndex < endIndex;
        }

        public void Reset()
        {
            currentIndex = startIndex - 1;
        }
    }
}
