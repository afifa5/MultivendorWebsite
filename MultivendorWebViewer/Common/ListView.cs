using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Common
{
    public enum DataGridViewColumnSortHint { None, Numeric, String };

    public interface IListView : IBindingListView
    {
        IListViewFilter FilterExpression { get; set; }

        int OriginalItemsCount { get; }
    }

    public interface IListView<T> : IList<T>, IListView
    {
        new T this[int index] { get; set; }

        new int Count { get; }

        new void Add(T item);

        new void RemoveAt(int index);

        new void Clear();

        void AddRange(IEnumerable<T> items);

        void Move(int fromIndex, int toIndex);

        IList<T> GetOriginalItems();
    }

    public interface IListViewFilter
    {
        bool Match(object item);

        ListViewFilterEquality GetFilterEquality(IListViewFilter otherFilter);
    }

    public class PredicateListViewFilter<T> : IListViewFilter
    {
        public PredicateListViewFilter(Func<T, bool> predicate)
        {
            Predicate = predicate;
        }

        public Func<T, bool> Predicate { get; private set; }

        public virtual ListViewFilterEquality GetFilterEquality(IListViewFilter otherFilter)
        {
            return ListViewFilterEquality.Unkown;
        }

        public bool Match(object item)
        {
            return Predicate((T)item);
        }

        public static implicit operator PredicateListViewFilter<T>(Func<T, bool> predicate)
        {
            return new PredicateListViewFilter<T>(predicate);
        }
    }

    public enum ListViewFilterEquality { Unkown, Equal, LessRestrictive, MoreRestrictive }

}
