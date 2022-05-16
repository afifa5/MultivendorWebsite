
using MultivendorWebViewer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Common
{
    public interface IMerge<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="merger">Depracated</param>
        void MergeWith(T item, IMerger<T> merger = null);
    }

    public interface IMerger<T>
    {
        T Merge(T a, T b);
    }

    public enum MergeMode : byte { Source, SourceBeforeTarget, TargetBeforeSource, Target };

    public class MergedItemProvider<T>
        where T : class, new()
    {
        //private IMerger<T> merger;

        private bool createNewInstance = false;

        private System.Collections.Concurrent.ConcurrentDictionary<IEnumerable<T>, T> mergedItems;

        private bool isMergeItem;

        public MergedItemProvider(bool createNewInstance = false)
            : this(new EnumerableEqualityComparer<T>(EqualityComparer<T>.Default), createNewInstance)
        {
        }

        public MergedItemProvider(IEqualityComparer<T> itemComparer, bool createNewInstance = false)
            : this(new EnumerableEqualityComparer<T>(itemComparer), createNewInstance)
        {
        }

        public MergedItemProvider(IEqualityComparer<IEnumerable<T>> itemsComparer, bool createNewInstance = false)
        {
            this.mergedItems = new System.Collections.Concurrent.ConcurrentDictionary<IEnumerable<T>, T>(itemsComparer);
            isMergeItem = typeof(IMerge<T>).IsAssignableFrom(typeof(T));
        }

        public T GetMergedItem(IEnumerable<T> items)
        {
            if (items == null) return null;
            return mergedItems.GetOrAdd(items.ToArray(), key => CreateMergedItem(key));
        }

        public T GetMergedItem(params T[] items)
        {
            if (items == null) return null;
            return mergedItems.GetOrAdd(items, key => CreateMergedItem(key));
        }

        protected virtual T CreateMergedItem(IEnumerable<T> items)
        {
            if (isMergeItem == true)
            {
                var e = items.GetEnumerator();
                if (e.MoveNext() == true)
                {
                    T item = e.Current;
                    if (e.MoveNext() == false)
                    {
                        if (createNewInstance == false)
                        {
                            return item;
                        }
                        else
                        {
                            var clonable = item as ICloneable;
                            if (clonable != null)
                            {
                                return (T)clonable.Clone();
                            }
                            
                            T newItem = new T();
                            ((IMerge<T>)newItem).MergeWith(item);
                            return newItem;
                        }
                    }

                    IMerge<T> mergedItem;
                    var firstClonable = item as ICloneable;
                    if (firstClonable != null)
                    {
                        mergedItem = (IMerge<T>)firstClonable.Clone();
                    }
                    else
                    {
                        mergedItem = (IMerge<T>)new T();
                        mergedItem.MergeWith(item);
                    }

                    mergedItem.MergeWith(e.Current);

                    while (e.MoveNext() == true)
                    {
                        mergedItem.MergeWith(e.Current);
                    }

                    return (T)mergedItem;
                }
            }

            return default(T);
        }

        //protected virtual void MergeWith(T item, T otherItem)
        //{
        //    var merge = item as IMerge<T>;
        //    if (merge != null)
        //    {
        //        merge.MergeWith(otherItem, merger);
        //    }
        //    else
        //    {
        //        if (merger != null)
        //        {
        //            item = merger.Merge(item, otherItem);
        //        }
        //    }
        //}
    }
}
