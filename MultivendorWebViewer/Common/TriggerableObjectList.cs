using MultivendorWebViewer.Common;
using MultivendorWebViewer.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Configuration
{
    //public class TriggerableObjectList<TItem, TIdentifier> : System.Collections.ObjectModel.Collection<Trigger<TItem, TIdentifier>>
    //    where TItem : class
    //    where TIdentifier : Identifier
    //{
    //    public TriggerableObjectList() { }

    //    public TriggerableObjectList(IEnumerable<Trigger<TItem, TIdentifier>> items)
    //    {
    //        AddRange(items);
    //    }

    //    public void AddRange(IEnumerable<Trigger<TItem, TIdentifier>> items)
    //    {
    //        foreach (var item in items.ToArray())
    //        {
    //            Add(item);
    //        }
    //    }

    //    public virtual void Add(TItem item)
    //    {
    //        Add(new Trigger<TItem, TIdentifier> { Item = item });
    //    }

    //    protected override void InsertItem(int index, Trigger<TItem, TIdentifier> item)
    //    {
    //        var comparer = new EnumerableEqualityComparer<Identifier>();
    //        var existingItemIndex = Items.IndexOf(i => comparer.Equals(i.Triggers, item.Triggers) == true);
    //        if (existingItemIndex == -1)
    //        {
    //            base.InsertItem(index, item);
    //        }
    //        else
    //        {
    //            var existingItemEntry = Items[existingItemIndex];
    //            var existingItem = existingItemEntry.Item as IInheritable;
    //            var inheritable = item.Item as IInheritable;
    //            if (inheritable != null && existingItem != null)
    //            {
    //                var copy = (IInheritable)Instance.Create<TItem>(item.Item.GetType());
    //                copy.MergeWith(existingItem);
    //                copy.MergeWith(inheritable);

    //                //if (ProfileSettingsProvider.CurrentDeserialized != null)
    //                //{
    //                //    // Alt 1. This updates the original item. Seems like we can't do it like this, will update items in other lists aswell
    //                //    inheritable.MergeWith(copy);
    //                //    Items[existingItemIndex] = item;
    //                //}
    //                //else
    //                {
    //                    // Alt 2. Create a new entry, copy trigger and use the copy as the item. This has shown to lead to inherit problems not yet solved.
    //                    var itemTrigger = new Trigger<TItem, TIdentifier>();
    //                    itemTrigger.Triggers = item.Triggers;
    //                    itemTrigger.Item = (TItem)copy;
    //                    Items[existingItemIndex] = itemTrigger;
    //                }

    //                //copy.MergeWith(inheritable);
    //                //copy.MergeWith((IInheritable)item.Item);
    //                //((IInheritable)item.Item).MergeWith(copy);

    //                //Items[existingItemIndex] = item;



    //                //var mergedItem = inheritable.CreatedMerged((IInheritable)existingItem.Item);


    //                //var itemTrigger = new Trigger<TItem, TIdentifier>();
    //                //itemTrigger.Triggers = item.Triggers;
    //                //itemTrigger.Item = (TItem)mergedItem;
    //                //Items[existingItemIndex] = itemTrigger;

    //                //inheritable.MergeWith((IInheritable)item.Item);
    //            }
    //            else
    //            {
    //                var mergeable = item.Item as IMerge<TItem>;
    //                if (mergeable != null)
    //                {
    //                    var mergedItem = (IMerge<TItem>)Instance.Create<TItem>(mergeable.GetType());
    //                    mergedItem.MergeWith(existingItemEntry.Item);
    //                    mergedItem.MergeWith(item.Item);

    //                    var itemTrigger = new Trigger<TItem, TIdentifier>();
    //                    itemTrigger.Triggers = item.Triggers;
    //                    itemTrigger.Item = (TItem)mergedItem;
    //                    Items[existingItemIndex] = itemTrigger;
    //                }
    //                else
    //                {
    //                    Items[existingItemIndex] = item;
    //                    //existingItem.Item = item.Item;
    //                }
    //            }
    //        }
    //    }



    //    //protected override void InsertItem(int index, Trigger<TItem, TIdentifier> item)
    //    //{
    //    //    var mergeableItem = item.Item as IMergeableSettings;
    //    //    if (mergeableItem != null && mergeableItem.BaseInstance != null)
    //    //    { 
    //    //        var mergedItem = mergeableItem.RecreateAsInherited();
    //    //        item.Item = mergeableItem as TItem;
    //    //    }
    //    //    base.InsertItem(index, item);
    //    //}

    //    public TItem DefaultItem { get; set; }

    //    public TItem GetNamedObject(string name)
    //    {
    //        foreach (var item in this)
    //        {
    //            var namedObject = item as NamedObject;
    //            if (namedObject != null && namedObject.Name == name)
    //            {
    //                return namedObject.Object as TItem;
    //            }
    //        }
    //        return null;
    //    }
    //}

    public abstract class ObjectDictionary<TKey, TItem> : ICollection<TItem>
        where TItem : class
    {
        public ObjectDictionary()
        {
            items = new InnerDictionaryCollection(this);
        }

        public ObjectDictionary(IEqualityComparer<TKey> comparer)
        {
            items = new InnerDictionaryCollection(this, comparer);
        }

        public ObjectDictionary(IEqualityComparer<TKey> comparer, TKey wildcardKey, Func<TKey, ICollection<TKey>> multiKeyProvider)
        {
            items = new InnerDictionaryCollection(this, comparer);
            WildcardKey = wildcardKey;
            MultiKeyProvider = multiKeyProvider;
        }

        private InnerDictionaryCollection items;

        public TKey WildcardKey { get; set; }

        public Func<TKey, ICollection<TKey>> MultiKeyProvider { get; set; }

        protected abstract TKey GetKeyForItem(TItem item);

        protected virtual TItem CreateItemCopy(TKey key, TItem item)
        {
            var copyableT = item as ICopyable<TItem>;
            if (copyableT != null)
            {
                return copyableT.Copy();
            }

            var copyable = item as ICopyable;
            if (copyable != null)
            {
                return (TItem)copyable.Copy();
            }

            var newItem = Instance.Create<TKey, TItem>(key);
            var mergeable = newItem as IMerge<TItem>;
            if (mergeable != null)
            {
                mergeable.MergeWith(item);
            }

            var clonable = item as ICloneable;
            if (clonable != null)
            {
                return (TItem)clonable.Clone();
            }

            return newItem;
        }

        public virtual TItem this[TKey key]
        {
            get
            {
                return items.GetItem(key);
            }
            set
            {
                if (WildcardKey != null && items.Comparer.Equals(key, WildcardKey) == true)
                {
                    foreach (var item in items)
                    {
                        var resolvedItem = ResolveConflictingItem(item, value);
                        if (object.ReferenceEquals(resolvedItem, item) == false)
                        {
                            items.Remove(key);
                            items.Add(resolvedItem);
                        }
                    }
                }
                else
                {
                    if (MultiKeyProvider != null)
                    {
                        var multiKeys = MultiKeyProvider(key);
                        if (multiKeys != null && multiKeys.Count > 1)
                        {
                            foreach (var multiKey in multiKeys)
                            {
                                TItem multiItem;
                                var newItem = CreateItemCopy(multiKey, value);

                                if (items.TryGetValue(multiKey, out multiItem) == true)
                                {
                                    var resolvedItem = ResolveConflictingItem(multiItem, newItem);
                                    // If the resolved item is not the already exisiting item, replace the exisiting item
                                    if (object.ReferenceEquals(resolvedItem, multiItem) == false)
                                    {
                                        items.Remove(multiKey);
                                        items.Add(resolvedItem);
                                    }
                                }
                                else if (AllowNew(newItem) == true)
                                {
                                    items.Add(newItem);
                                }
                            }
                        }
                    }

                    TItem item;
                    if (items.TryGetValue(key, out item) == true)
                    {
                        var resolvedItem = ResolveConflictingItem(item, value);
                        // If the resolved item is not the already exisiting item, replace the exisiting item
                        if (object.ReferenceEquals(resolvedItem, item) == false)
                        {
                            items.Remove(key);
                            items.Add(resolvedItem);
                        }
                    }
                    else if (AllowNew(value) == true)
                    {
                        items.Add(value);
                    }
                }

            }
        }

        protected virtual bool AllowNew(TItem item)
        {
            return true;
        }

        protected virtual TItem ResolveConflictingItem(TItem exisitingItem, TItem newItem)
        {
            IMerge<TItem> mergeableExistingItem = exisitingItem as IMerge<TItem>;
            if (mergeableExistingItem != null)
            {
                mergeableExistingItem.MergeWith(newItem);
                return exisitingItem;
            }
            return newItem;
        }

        public void Add(TItem item)
        {
            try
            {
                this[GetKeyForItem(item)] = item;
            }
            catch
            {

            }
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Remove(TKey key)
        {
            items.Remove(key);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(TItem item)
        {
            return items.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TItem item)
        {
            return items.Remove(item);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        protected class InnerDictionaryCollection : Components.DictionaryCollection<TKey, TItem>
        {
            public InnerDictionaryCollection(ObjectDictionary<TKey, TItem> dictionary)
            {
                this.dictionary = dictionary;
            }

            public InnerDictionaryCollection(ObjectDictionary<TKey, TItem> dictionary, IEqualityComparer<TKey> comparer)
                : base(comparer)
            {
                this.dictionary = dictionary;
            }

            public InnerDictionaryCollection(ObjectDictionary<TKey, TItem> dictionary, IEnumerable<TItem> items)
                : base(items)
            {
                this.dictionary = dictionary;
            }

            public InnerDictionaryCollection(ObjectDictionary<TKey, TItem> dictionary, IEnumerable<TItem> items, IEqualityComparer<TKey> comparer)
                : base(items, comparer)
            {
                this.dictionary = dictionary;
            }

            public InnerDictionaryCollection(ObjectDictionary<TKey, TItem> dictionary, IEnumerable<TItem> items, IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
                : base(items, comparer, dictionaryCreationThreshold)
            {
                this.dictionary = dictionary;
            }

            private ObjectDictionary<TKey, TItem> dictionary;

            protected override TKey GetKeyForItem(TItem item)
            {
                return dictionary.GetKeyForItem(item);
            }
        }
    }

    public abstract class ObjectDictionary<TItem> : ObjectDictionary<string, TItem>
        where TItem : class
    {
        private static char[] multiKeyDelimiters = new char[] { ' ', ',', ';' };

        public ObjectDictionary() : this(null, null, null) { }

        public ObjectDictionary(IEnumerable<TItem> items)
            : this(null, null, null)
        {
            AddRange(items);
        }

        public ObjectDictionary(IEqualityComparer<string> comparer, string wildcardKey = null, Func<string, ICollection<string>> multiKeyProvider = null)
            : base(comparer ?? StringComparer.OrdinalIgnoreCase)
        {
            WildcardKey = wildcardKey ?? "*";
            MultiKeyProvider = multiKeyProvider;
            if (MultiKeyProvider == null)
            {
                MultiKeyProvider = key => key != null ? key.Split(multiKeyDelimiters, StringSplitOptions.RemoveEmptyEntries) : new string[] { key };
            }
        }
    }

    //public abstract class TriggerableObjectDictionary<TKey, TItem, TIdentifier> : ICollection<Trigger<TItem, TIdentifier>>
    //    where TItem : class
    //    where TIdentifier : Identifier
    //{
    //    public TriggerableObjectDictionary()
    //    {
    //        items = new InnerDictionaryCollection(this);
    //    }

    //    public TriggerableObjectDictionary(IEqualityComparer<TKey> comparer)
    //    {
    //        items = new InnerDictionaryCollection(this, comparer);
    //    }

    //    public IEnumerable<TItem> Items { get { return items.Select(i => i.Item); } }

    //    private InnerDictionaryCollection items;

    //    protected abstract TKey GetKeyForItem(Trigger<TItem, TIdentifier> item);

    //    public Trigger<TItem, TIdentifier> GetByIndex(int index)
    //    {
    //        return items[index];
    //    }

    //    public virtual Trigger<TItem, TIdentifier> this[TKey key]
    //    {
    //        get
    //        {
    //            return items.GetItem(key);
    //        }
    //        set
    //        {
    //            Trigger<TItem, TIdentifier> item;
    //            if (items.TryGetValue(key, out item) == true)
    //            {
    //                var resolvedItem = ResolveConflictingItem(item, value);
    //                if (object.ReferenceEquals(resolvedItem, item) == false)
    //                {
    //                    items.Remove(key);
    //                    items.Add(resolvedItem);
    //                }
    //            }
    //            else
    //            {
    //                items.Add(value);
    //            }
    //        }
    //    }

    //    protected virtual Trigger<TItem, TIdentifier> ResolveConflictingItem(Trigger<TItem, TIdentifier> exisitingItem, Trigger<TItem, TIdentifier> newItem)
    //    {
    //        if (newItem.Triggers == null || newItem.Triggers.Length == 0)
    //        {
    //            newItem.Triggers = exisitingItem.Triggers;
    //        }

    //        return newItem;
    //    }

    //    public virtual IEnumerable<TItem> GetAllMatchingItems(MatchContext context, object obj)
    //    {
    //        foreach (var group in this)
    //        {
    //            if (group.Triggers.AllMatches(context, obj) == true)
    //            {
    //                yield return group.Item;
    //            }
    //        }
    //    }

    //    public virtual IEnumerable<TItem> GetAllMatchingItems(MatchContext context, params object[] obj)
    //    {
    //        foreach (var group in this)
    //        {
    //            if (group.Triggers.AllMatches(context, obj) == true)
    //            {
    //                yield return group.Item;
    //            }
    //        }
    //    }

    //    public virtual IEnumerable<TItem> GetAllMatchingItems(params object[] obj)
    //    {
    //        foreach (var group in this)
    //        {
    //            if (group.Triggers.AllMatches(obj) == true)
    //            {
    //                yield return group.Item;
    //            }
    //        }
    //    }

    //    public virtual void Add(Trigger<TItem, TIdentifier> item)
    //    {
    //        try
    //        {
    //            this[GetKeyForItem(item)] = item;
    //        }
    //        catch
    //        {

    //        }
    //    }

    //    public void Add(TItem item)
    //    {
    //        Add(new Trigger<TItem, TIdentifier>() { Item = item });
    //    }

    //    public virtual void Add(TItem item, params TIdentifier[] triggers)
    //    {
    //        Add(new Trigger<TItem, TIdentifier>() { Item = item, Triggers = triggers });
    //    }

    //    public void Remove(TKey key)
    //    {
    //        items.Remove(key);
    //    }

    //    public void Clear()
    //    {
    //        items.Clear();
    //    }

    //    public bool Contains(Trigger<TItem, TIdentifier> item)
    //    {
    //        return items.Contains(item);
    //    }

    //    public void CopyTo(Trigger<TItem, TIdentifier>[] array, int arrayIndex)
    //    {
    //        items.CopyTo(array, arrayIndex);
    //    }

    //    public int Count
    //    {
    //        get { return items.Count; }
    //    }

    //    public bool IsReadOnly
    //    {
    //        get { return false; }
    //    }

    //    public bool Remove(Trigger<TItem, TIdentifier> item)
    //    {
    //        return items.Remove(item);
    //    }

    //    public IEnumerator<Trigger<TItem, TIdentifier>> GetEnumerator()
    //    {
    //        return items.GetEnumerator();
    //    }

    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return items.GetEnumerator();
    //    }

    //    protected class InnerDictionaryCollection : DictionaryCollection<TKey, Trigger<TItem, TIdentifier>>
    //    {
    //        public InnerDictionaryCollection(TriggerableObjectDictionary<TKey, TItem, TIdentifier> dictionary)
    //        {
    //            this.dictionary = dictionary;
    //        }

    //        public InnerDictionaryCollection(TriggerableObjectDictionary<TKey, TItem, TIdentifier> dictionary, IEqualityComparer<TKey> comparer)
    //            : base(comparer)
    //        {
    //            this.dictionary = dictionary;
    //        }

    //        public InnerDictionaryCollection(TriggerableObjectDictionary<TKey, TItem, TIdentifier> dictionary, IEnumerable<Trigger<TItem, TIdentifier>> items)
    //            : base(items)
    //        {
    //            this.dictionary = dictionary;
    //        }

    //        public InnerDictionaryCollection(TriggerableObjectDictionary<TKey, TItem, TIdentifier> dictionary, IEnumerable<Trigger<TItem, TIdentifier>> items, IEqualityComparer<TKey> comparer)
    //            : base(items, comparer)
    //        {
    //            this.dictionary = dictionary;
    //        }

    //        public InnerDictionaryCollection(TriggerableObjectDictionary<TKey, TItem, TIdentifier> dictionary, IEnumerable<Trigger<TItem, TIdentifier>> items, IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
    //            : base(items, comparer, dictionaryCreationThreshold)
    //        {
    //            this.dictionary = dictionary;
    //        }

    //        private TriggerableObjectDictionary<TKey, TItem, TIdentifier> dictionary;

    //        protected override TKey GetKeyForItem(Trigger<TItem, TIdentifier> item)
    //        {
    //            return dictionary.GetKeyForItem(item);
    //        }
    //    }
    //}

    //public class TriggerableObjectList<TItem> : TriggerableObjectList<TItem, Identifier>
    //    where TItem : class
    //{
    //    public TriggerableObjectList() { }

    //    public TriggerableObjectList(IEnumerable<Trigger<TItem, Identifier>> items) : base(items) { }

    //}

    public class DataViewSelectorCollection<T> : ObjectDictionary<T>
        where T : DataViewSelector
    {
        public DataViewSelectorCollection() { }

        public DataViewSelectorCollection(IEnumerable<T> items) : base(items) { }

        protected override string GetKeyForItem(T item)
        {
            return item.Id;
        }

        protected override T CreateItemCopy(string key, T item)
        {
            var copy = item.Copy();
            copy.Id = key;
            return (T)copy;
        }

        protected override T ResolveConflictingItem(T exisitingItem, T newItem)
        {
            var mergedItem = (T)exisitingItem.Copy(clone: true);
            mergedItem.MergeWith(newItem);
            return mergedItem;
        }

        protected override bool AllowNew(T item)
        {
            return item.CreateNew != false;
        }
    }

    //public class GroupIdentifier
    //{
    //    public GroupIdentifier()
    //    {
    //        Enabled = true;
    //        LabelValueProvider = new ValueProvider();
    //        GroupByValueProvider = new ValueProvider();
    //        //GroupByIdentifier = new Identifier();
    //    }

    //    [XmlAttribute("id")]
    //    public string Id { get; set; }

    //    [XmlAttribute("enabled")]
    //    public bool Enabled { get; set; }

    //    [XmlAttribute("label")]
    //    public string Label { get { return LabelValueProvider.StaticValue != null ? LabelValueProvider.StaticValue.Value as string : null; } set { LabelValueProvider.StaticValue = value != null ? new StaticValue { Value = value } : null; } }

    //    [XmlAttribute("label-property")]
    //    public string LabelProperty { get { return LabelValueProvider.PropertyName; } set { LabelValueProvider.PropertyName = value; } }

    //    public Formatter LabelFormat { get { return LabelValueProvider.Format; } set { LabelValueProvider.Format = value; } }

    //    [XmlIgnore]
    //    public ValueProvider LabelValueProvider { get; private set; }


    //    //[XmlAttribute("label-pattern")]
    //    //public string LabelPattern { get; set; }

    //    //private Regex labelPatternRegex;
    //    //[XmlIgnore]
    //    //public Regex LabelPatternRegex
    //    //{
    //    //    get
    //    //    {
    //    //        if (labelPatternRegex == null && LabelPattern != null)
    //    //        {
    //    //            labelPatternRegex = new Regex(LabelPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
    //    //        }
    //    //        return labelPatternRegex;
    //    //    }
    //    //}

    //    //[XmlAttribute("label-format")]
    //    //public string LabelFormat { get; set; }

    //    [XmlIgnore]
    //    public ValueProvider GroupByValueProvider { get; private set; }


    //    public Formatter GroupByFormat { get { return GroupByValueProvider.Format; } set { GroupByValueProvider.Format = value; } }

    //    [XmlAttribute("group-key")]
    //    public string GroupKey { get; set; }

    //    [XmlAttribute("group-by-property")]
    //    public string GroupByProperty { get { return GroupByValueProvider.PropertyName; } set { GroupByValueProvider.PropertyName = value; } }

    //    [XmlAttribute("group-by-specification")]
    //    public string GroupBySpecification { get { return GroupByValueProvider.SpecificationCodeShorthand; } set { GroupByValueProvider = ValueProvider.CreateSpecification(value); } }

    //    [XmlAttribute("group-by-property-value")]
    //    public string GroupByPropertyValue { get; set; }

    //    private Identifier groupByIdentifier;
    //    public Identifier GroupByIdentifier
    //    {
    //        get
    //        {
    //            if (groupByIdentifier == null)
    //            {
    //                if (GroupBySpecification != null)
    //                {

    //                }
    //                else
    //                {
    //                    groupByIdentifier = new Identifier { PropertyName = GroupByProperty, RegexPattern = GroupByFormat != null ? GroupByFormat.RegexPattern : null, PropertyValue = GroupByPropertyValue };
    //                }

    //            }
    //            return groupByIdentifier;
    //        }
    //        set { groupByIdentifier = value; }
    //    }

    //    [XmlAttribute("order")]
    //    public float Order { get; set; }

    //    public Sort Sort { get; set; }

    //    public bool Match(object obj, MatchContext context = null)
    //    {
    //        return GroupByIdentifier.Match(obj, context);
    //    }

    //    private GroupDefinition staticGroupDefinition;

    //    public GroupDefinition GetGroupDefinition(object obj, FormatterContext formatterContext)
    //    {
    //        if (formatterContext == null) throw new ArgumentNullException("formatterContext");

    //        var requestContext = formatterContext.ApplicationRequestContext;

    //        if (GroupKey != null)
    //        {
    //            if (Match(obj, new MatchContext(requestContext)) == true)
    //            {
    //                return staticGroupDefinition ?? (staticGroupDefinition = new GroupDefinition
    //                {
    //                    Key = GroupKey,
    //                    KeyComparer = StringComparer.OrdinalIgnoreCase,
    //                    Label = requestContext.GetApplicationTextTranslation((string)LabelValueProvider.GetFormattedValue(obj: obj, formatProvider: requestContext.DisplayCulture, multipleValuesSeparator: ",")),
    //                    Order = Order
    //                });
    //            }
    //            return null;
    //        }

    //        var groupKey = (string)GroupByValueProvider.GetFormattedValue(obj: obj, formatProvider: requestContext.DisplayCulture, multipleValuesSeparator: ",");

    //        var group = new GroupDefinition
    //        {
    //            Key = string.IsNullOrEmpty(groupKey) == true ? null : groupKey, //(string)GroupByValueProvider.GetFormattedValue(obj: obj, formatProvider: requestContext.DisplayCulture, multipleValuesSeparator: ","),
    //            KeyComparer = StringComparer.Create(requestContext.DisplayCulture, true),
    //            Label = requestContext.GetApplicationTextTranslation((string)LabelValueProvider.GetFormattedValue(obj: obj, formatProvider: requestContext.DisplayCulture, multipleValuesSeparator: ",")),
    //            Order = Order
    //        };

    //        return group;
    //    }

    //    public GroupDefinition GetGroupDefinition(object obj, IApplicationRequestContext requestContext)
    //    {
    //        return GetGroupDefinition(obj, new FormatterContext { ApplicationRequestContext = requestContext });
    //    }
    //}

    //public class GroupRemoveIdentifier : GroupIdentifier { }

    public class GroupDefinition
    {
        public string Key { get; set; }

        public string Label { get; set; }

        public float Order { get; set; }

        public IEqualityComparer KeyComparer { get; set; }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj) == true) return true;

            var other = obj as GroupDefinition;
            if (other == null) return false;
            if ((KeyComparer ?? StringComparer.OrdinalIgnoreCase).Equals(Key, other.Key) == false) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return Key != null ? (KeyComparer ?? StringComparer.OrdinalIgnoreCase).GetHashCode(Key) : 0;
        }
    }

    //public class GroupIdentifierCollection : ObjectDictionary<GroupIdentifier>
    //{
    //    public GroupIdentifierCollection() : base(StringComparer.OrdinalIgnoreCase) { }

    //    public GroupIdentifierCollection(IEnumerable<GroupIdentifier> items)
    //        : base(StringComparer.OrdinalIgnoreCase)
    //    {
    //        AddRange(items);
    //    }

    //    //public override GroupIdentifier this[string key]
    //    //{
    //    //    get { return base[key]; }
    //    //    set
    //    //    {
    //    //        var remove = value as GroupRemoveIdentifier;
    //    //        if (remove == null)
    //    //        {
    //    //            base[key] = value;
    //    //        }
    //    //        else
    //    //        {
    //    //            if (remove.Id == "*")
    //    //            {
    //    //                Clear();
    //    //            }
    //    //            else
    //    //            {
    //    //                Remove(remove.Id);
    //    //            }
    //    //        }
    //    //    }
    //    //}

    //    protected override GroupIdentifier ResolveConflictingItem(GroupIdentifier exisitingItem, GroupIdentifier newItem)
    //    {
    //        return newItem;
    //    }
    //    protected override string GetKeyForItem(GroupIdentifier item)
    //    {
    //        return item.Id;
    //    }
    //}
}
