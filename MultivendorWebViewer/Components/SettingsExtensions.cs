using MultivendorWebViewer.Common;
using MultivendorWebViewer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Configuration
{
    public static class SettingsExtensions
    {
        public static bool AllMatches<T>(this IEnumerable<T> identifiers, MatchContext context, object obj)
            where T : Identifier
        {
            if (identifiers == null) return true;

            foreach (var identifier in identifiers)
            {
                if (identifier.Object == null)
                {
                    if (identifier.Match(obj, context) == false)
                    {
                        return false;
                    }
                }
                else
                {
                    string objectName = identifier.Object;
                    var namedObject = obj as NamedObject;
                    if (namedObject != null && namedObject.Name == objectName)
                    {
                        if (identifier.Match(namedObject.Object, context) == false)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool AllMatches<T>(this IEnumerable<T> identifiers, IEnumerable<object> objects)
            where T : Identifier
        {
            return AllMatches(identifiers, null, objects);
        }

        /// <summary>
        /// Gets if all indentifiers have one or more match
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifiers"></param>
        /// <param name="objects"></param>
        /// <param name="allObjectsMustMatch">True if all objects must match each identifier, false if at only one object must match each identifier</param>
        /// <returns></returns>
        public static bool AllMatches<T>(this IEnumerable<T> identifiers, MatchContext context, IEnumerable<object> objects)
            where T : Identifier
        {
            if (identifiers == null) return true;

            foreach (var identifier in identifiers)
            {
                bool match = true;

                if (identifier.Object == null)
                {
                    foreach (var obj in objects)
                    {
                        if (identifier.Match(obj, context) == false)
                        {
                            match = false;
                            break;
                        }
                    }
                }
                else
                {
                    string objectName = identifier.Object;
                    foreach (var obj in objects)
                    {
                        var namedObject = obj as NamedObject;
                        if (namedObject != null && namedObject.Name == objectName)
                        {
                            if (identifier.Match(namedObject.Object, context) == false)
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                }

                if (match == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static IEnumerable<T> GetAllMatches<T>(this IEnumerable<T> identifiers, IEnumerable<object> objects)
            where T : Identifier
        {
            return GetAllMatches(identifiers, null, objects);
        }

        public static IEnumerable<T> GetAllMatches<T>(this IEnumerable<T> identifiers, MatchContext context, IEnumerable<object> objects)
            where T : Identifier
        {
            if (identifiers == null) yield break;

            foreach (var identifier in identifiers)
            {
                bool match = true;

                if (identifier.Object == null)
                {
                    foreach (var obj in objects)
                    {
                        if (identifier.Match(obj, context) == false)
                        {
                            match = false;
                            break;
                        }
                    }
                }
                else
                {
                    string objectName = identifier.Object;
                    bool namedExisted = false;
                    foreach (var obj in objects)
                    {
                        var namedObject = obj as NamedObject;
                        if (namedObject != null && namedObject.Name == objectName)
                        {
                            namedExisted = true;
                            if (identifier.Match(namedObject.Object, context) == false)
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (namedExisted == false)
                    {
                        match = false;
                    }
                }

                if (match == true)
                {
                    yield return identifier;
                }
            }
        }

        public static IEnumerable<TItem> GetAllMatches<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, MatchContext context, params object[] objects)
            where TItem : class
            where TIdentifier : Identifier
        {
            return GetAllMatches(triggers, context, (IEnumerable<object>)objects);
        }

        public static IEnumerable<TItem> GetAllMatches<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, params object[] objects)
            where TItem : class
            where TIdentifier : Identifier
        {
            return GetAllMatches(triggers, null, (IEnumerable<object>)objects);
        }

        public static IEnumerable<TItem> GetAllMatches<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, IEnumerable<object> objects)
            where TItem : class
            where TIdentifier : Identifier
        {
            return GetAllMatches(triggers, null, objects);
        }

        public static IEnumerable<TItem> GetAllMatches<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, MatchContext context, IEnumerable<object> objects)
            where TItem : class
            where TIdentifier : Identifier
        {
            if (triggers == null) yield break;
            bool empty = true;
            foreach (var trigger in triggers)
            {
                if (trigger.Triggers == null)
                {
                    yield return trigger.Item;
                }
                else
                {
                    bool match = true;

                    //int matchCount = 0;
                    foreach (var identifier in trigger.Triggers)
                    {
                        if (identifier.Object == null)
                        {
                            foreach (var obj in objects)
                            {
                                if (identifier.Match(obj, context) == false)
                                {
                                    match = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            string objectName = identifier.Object;
                            bool namedExisted = false;
                            foreach (var obj in objects)
                            {
                                var namedObject = obj as NamedObject;
                                if (namedObject != null && namedObject.Name == objectName)
                                {
                                    namedExisted = true;
                                    if (identifier.Match(namedObject.Object, context) == false)
                                    {
                                        match = false;
                                        break;
                                    }
                                }
                            }
                            if (namedExisted == false)
                            {
                                match = false;
                            }
                        }
                    }

                    if (match == true)
                    {
                        yield return trigger.Item;
                        //matchCount++;
                    }
                }
                empty = false;
            }
            if (empty == true)
            {
                var t = triggers as TriggerableObjectList<TItem, TIdentifier>;
                if (t != null && t.DefaultItem != null) yield return t.DefaultItem;
            }
        }

        public static IEnumerable<TItem> GetAllMatches<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, MatchContext context, object obj)
            where TItem : class
            where TIdentifier : Identifier
        {
            if (triggers == null) yield break;

            foreach (var trigger in triggers)
            {
                if (trigger.Triggers == null)
                {
                    yield return trigger.Item;
                }
                else
                {
                    bool match = true;

                    foreach (var identifier in trigger.Triggers)
                    {
                        if (identifier.Object == null)
                        {
                            if (identifier.Match(obj, context) == false)
                            {
                                match = false;
                                break;
                            }
                        }
                        else
                        {
                            string objectName = identifier.Object;
                            var namedObject = obj as NamedObject;
                            if (namedObject != null && namedObject.Name == objectName)
                            {
                                if (identifier.Match(namedObject.Object, context) == false)
                                {
                                    match = false;
                                    break;
                                }
                            }
                            else
                            {
                                match = false;
                                break;
                            }
                        }
                    }

                    if (match == true)
                    {
                        yield return trigger.Item;
                    }
                }
            }
        }

        public static TItem GetMergedMatch<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, MatchContext context, object obj)
            where TItem : class, new()
            where TIdentifier : Identifier
        {
            return SettingsExtensions.GetMergedMatch(triggers, context, null, obj);
        }

        //public static TItem GetMergedMatch<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, MatchContext context, MergedItemProvider<MergeableSettingsWrapper<TItem>> mergeProvider, object obj)
        //     where TItem : class, IMerge<TItem>, new()
        //     where TIdentifier : Identifier
        //{
        //    var allMatches = SettingsExtensions.GetAllMatches(triggers, context, obj).ToArray();
        //    if (allMatches.Length > 1)
        //    {
        //        if (mergeProvider != null)
        //        {
        //            return mergeProvider.GetMergedItem(allMatches);
        //        }
        //        else
        //        {
        //            var merged = new TItem() as IMerge<TItem>;
        //            if (merged != null)
        //            {
        //                for (int i = 0; i < allMatches.Length; i++)
        //                {
        //                    merged.MergeWith(allMatches[i]);
        //                }
        //            }
        //        }
        //    }
        //    return allMatches.FirstOrDefault();
        //}

        public static TItem GetMergedMatch<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, MatchContext context, MergedItemProvider<TItem> mergeProvider, object obj)
            where TItem : class, new()
            where TIdentifier : Identifier
        {
            var allMatches = SettingsExtensions.GetAllMatches(triggers, context, obj).ToArray();
            if (allMatches.Length > 1)
            {
                if (mergeProvider != null)
                {
                    return mergeProvider.GetMergedItem(allMatches);
                }
                else
                {
                    var merged = new TItem() as IMerge<TItem>;
                    if (merged != null)
                    {
                        for (int i = 0; i < allMatches.Length; i++)
                        {
                            merged.MergeWith(allMatches[i]);
                        }
                    }
                    return (TItem)merged;
                }
            }
            return allMatches.FirstOrDefault();
        }

        public static TItem GetFirstMatch<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, params object[] objects)
            where TItem : class
            where TIdentifier : Identifier
        {
            return GetAllMatches(triggers, null, (IEnumerable<object>)objects).FirstOrDefault();
        }

        public static TItem GetFirstMatch<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, MatchContext context, params object[] objects)
            where TItem : class
            where TIdentifier : Identifier
        {
            return GetAllMatches(triggers, context, (IEnumerable<object>)objects).FirstOrDefault();
        }

        public static TItem GetFirstMatch<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, IEnumerable<object> objects)
            where TItem : class
            where TIdentifier : Identifier
        {
            return GetAllMatches(triggers, null, objects).FirstOrDefault();
        }

        public static TItem GetFirstMatch<TItem, TIdentifier>(this IEnumerable<Trigger<TItem, TIdentifier>> triggers, MatchContext context, IEnumerable<object> objects)
            where TItem : class
            where TIdentifier : Identifier
        {
            return GetAllMatches(triggers, context, objects).FirstOrDefault();
        }
    }
}
