using MultivendorWebViewer;
using MultivendorWebViewer.Collections;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace MultivendorWebViewer.ComponentModel
{
    public interface IPropertyGetter
    {
        object GetValue(object obj);

        PropertyDescriptor Descriptor { get; }

        Type ComponentType { get; }

        Type PropertyType { get; }

        string PropertyName { get; }

        string PropertyNamePath { get; }
    }

    public interface IPropertyGetterDelegate
    {
        Func<object, object> Getter { get; }
    }

    public interface IPropertyGetter<T> : IPropertyGetter
    {
        new T GetValue(object obj);
    }

    public interface IPropertyGetterDelegate<T>
    {
        Func<object, T> Getter { get; }
    }

    public interface IPropertySetter
    {
        void SetValue(object obj, object value);
    }

    public interface IPropertySetterDelegate
    {
        Action<object, object> Setter { get; }
    }

    public interface IPropertySetter<T> : IPropertySetter
    {
        void SetValue(object obj, T value);
    }

    public interface IPropertySetterDelegate<T>
    {
        Action<object, T> Setter { get; }
    }

    public static class PropertyGetter
    {
        private static Dictionary<Type, IDictionary<string, IPropertyGetter>> cachedTypeProperties = new Dictionary<Type, IDictionary<string, IPropertyGetter>>();

        private static object cachedTypePropertiesLock = new object();

        private static Dictionary<Tuple<string, Type>, IPropertyGetter> cachedProperties = new Dictionary<Tuple<string, Type>, IPropertyGetter>();

        private static object cachedPropertiesLock = new object();

        public static PropertyDescriptor[] GetPublicProperties(this Type type)
        {
            //if (type.IsInterface)
            {
                //var propertyInfos = new List<PropertyInfo>();
                var propertyInfos = new List<PropertyDescriptor>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = TypeDescriptor.GetProperties(subType);

                    var newPropertyInfos = typeProperties.OfType<PropertyDescriptor>().Where(x => !propertyInfos.Contains(x));
                    //var typeProperties = subType.GetProperties(
                    //    BindingFlags.FlattenHierarchy
                    //    | BindingFlags.Public
                    //    | BindingFlags.Instance);

                    //var newPropertyInfos = typeProperties
                    //    .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            //return type.GetProperties(BindingFlags.FlattenHierarchy
            //    | BindingFlags.Public | BindingFlags.Instance);
        }

        public static IDictionary<string, IPropertyGetter> GetPropertyGetters(Type type)
        {
            IDictionary<string, IPropertyGetter> propertyGetter;
            if (PropertyGetter.cachedTypeProperties.TryGetValue(type, out propertyGetter) == false)
            {
                lock (PropertyGetter.cachedTypePropertiesLock)
                {
                    if (PropertyGetter.cachedTypeProperties.TryGetValue(type, out propertyGetter) == false)
                    {
                        if (type.IsInterface == false)
                        {
                            propertyGetter = TypeDescriptor.GetProperties(type).OfType<PropertyDescriptor>().Select(d => PropertyGetter.Create(d)).ToDictionary(d => d.PropertyName);
                        }
                        else
                        {
                            propertyGetter = GetPublicProperties(type).GroupBy(p => p.Name).Select(dg => PropertyGetter.Create(dg.FirstOrDefault(g => g.ComponentType == type) ?? dg.First())).ToDictionary(d => d.PropertyName);
                        }
                        PropertyGetter.cachedTypeProperties[type] = propertyGetter;
                    }
                }
            }
            return propertyGetter;
        }

        public class PropertyGetterPath : IPropertyGetter
        {
            public PropertyGetterPath(IPropertyGetter[] getters)
            {
                PathGetters = getters;
            }

            public PropertyGetterPath(IEnumerable<IPropertyGetter> getters)
            {
                PathGetters = getters.ToArray();
            }

            public IPropertyGetter[] PathGetters { get; private set; }

            public object GetValue(object obj)
            {
                object value = PathGetters[0].GetValue(obj);

                for (int index = 1; index < PathGetters.Length; index++)
                {
                    if (value == null) return null; // throw new NullReferenceException();

                    var values = value as IEnumerable<object>;
                    if (values == null)
                    {
                        value = PathGetters[index].GetValue(value);
                    }
                    else
                    {
                        var pathGetter = PathGetters[index];
                        value = values.Select(v => pathGetter.GetValue(v));
                    }
                }

                return value;
            }

            public PropertyDescriptor Descriptor
            {
                get { return PathGetters.Last().Descriptor; }
            }

            public Type ComponentType
            {
                get { return PathGetters.Last().ComponentType; }
            }

            public Type PropertyType
            {
                get { return PathGetters.Last().PropertyType; }
            }

            public string PropertyName
            {
                get { return PathGetters.Last().PropertyName; }
            }

            public string PropertyNamePath
            {
                get { return string.Join(".", PathGetters.Select(p => p.PropertyName)); }
            }
        }

        public static class PropertyGetterHelper
        {
            public static Type[] GetGenericInterfaceTypes(Type genericInterface, Type componentType)
            {
                if (componentType.IsInterface == true && componentType.IsGenericType == true && componentType.GetGenericTypeDefinition() == genericInterface)
                {
                    return componentType.GetGenericArguments();
                }

                foreach (var interfaceType in componentType.GetInterfaces())
                {
                    if (interfaceType.IsGenericType == true && interfaceType.GetGenericTypeDefinition() == genericInterface)
                    {
                        return interfaceType.GetGenericArguments();
                    }
                }

                return Type.EmptyTypes;
            }
        }

        public class IndexedPropertyGetter : IPropertyGetter
        {
            public IndexedPropertyGetter(IPropertyGetter collectionPropertyGetter)
            {
                CollectionPropertyGetter = collectionPropertyGetter;
                IndexResolver = AllIndexer;
            }

            public IndexedPropertyGetter(IPropertyGetter collectionPropertyGetter, Func<object, object> indexResolver)
            {
                CollectionPropertyGetter = collectionPropertyGetter;
                IndexResolver = indexResolver;
            }     

            private Type[] GetGenericInterfaceTypes(Type genericInterface, Type componentType)
            {
                if (componentType.IsInterface == true && componentType.IsGenericType == true && componentType.GetGenericTypeDefinition() == genericInterface)
                {
                    return componentType.GetGenericArguments();
                }

                foreach (var interfaceType in CollectionPropertyGetter.PropertyType.GetInterfaces())
                {
                    if (interfaceType.IsGenericType == true && interfaceType.GetGenericTypeDefinition() == genericInterface)
                    {
                        return interfaceType.GetGenericArguments();
                    }
                }

                return Type.EmptyTypes;
            }

            public IndexedPropertyGetter(IPropertyGetter collectionPropertyGetter, string indexValue)
            {
                CollectionPropertyGetter = collectionPropertyGetter;
                var indexCollectionPropertyType = CollectionPropertyGetter.PropertyType;
                CollectionItemType = GetGenericInterfaceTypes(typeof(IEnumerable<>), indexCollectionPropertyType).FirstOrDefault();

                if (indexValue[0] == '[') indexValue = indexValue.Substring(1, indexValue.Length - 2);

                if (indexValue.Length == 0)
                {
                    IndexResolver = AllIndexer;
                    return;
                }

                if (indexValue[0] != '"' && indexValue[0] != '\'')
                {
                    // Where enumerator
                    int indexOfWhere = indexValue.IndexOf('=');
                    if (indexOfWhere >= 0)
                    {
                        string indexProperty = indexValue.Substring(0, indexOfWhere).Trim();
                        string indexPropertyValue = indexValue.Substring(indexOfWhere + 1);
                        if (indexPropertyValue[0] == '"' || indexPropertyValue[0] == '\'') indexPropertyValue = indexPropertyValue.Substring(1, indexPropertyValue.Length - 2);

                        if (CollectionItemType != null)
                        {
                            CollectionItemPropertyGetter = PropertyGetter.Create(indexProperty, CollectionItemType);
                            CollectionIndexerValue = indexPropertyValue;
                            if (CollectionItemPropertyGetter != null)
                            {
                                IndexResolver = WhereIndexer;
                            }
                        }
                    } // Dictionary lookup
                    else
                    {
                        var indexIndexerType = typeof(IIndexer);
                        if (indexIndexerType.IsAssignableFrom(indexCollectionPropertyType) == true)
                        {
                            var indexer = new IndexIndexer { Index = indexValue };
                            IndexResolver = indexer.GetValue;
                        }
                        else
                        {
                            var istringsetType = typeof(ISet<string>);
                            if (istringsetType.IsAssignableFrom(indexCollectionPropertyType) == true)
                            {
                                CollectionIndexerValue = indexValue;
                                IndexResolver = StringSetIndexer;
                            }
                            else
                            {
                                int intIndex;
                                if (int.TryParse(indexValue, out intIndex) == false)
                                {
                                    CollectionIndexerValue = indexValue;
                                    IndexResolver = DictionaryIndexer;
                                }
                                else
                                {
                                    CollectionIntIndexerValue = intIndex;
                                    IndexResolver = CollectionIndexer;
                                }
                            }
                        }
                        //IndexResolver = o =>
                        //    {
                        //        var dictType = typeof(IDictionary<,>);

                        //        var otype = o.GetType();
                        //        var idictType = otype.GetInterfaces().Where(t => t.IsGenericType == true && t.GetGenericTypeDefinition() == dictType).First();

                        //        var keyType = idictType.GetGenericArguments()[0];
                        //        var valueType = idictType.GetGenericArguments()[1];

                        //        var tryGetValueMethod = idictType.GetMethod("TryGetValue");
                                
                        //        object v = null;
                        //        object result = tryGetValueMethod.Invoke(o, new object[] { CollectionIndexerValue, v });

                        //        DelegateTypeFactory dtf = new DelegateTypeFactory();
                        //        var dt = dtf.CreateDelegateType(tryGetValueMethod);
                        //        Delegate.CreateDelegate(dt, instance, method);
                        //        //var d = tryGetValueMethod.CreateDelegate(null);
                        //        //var d = (TryDelegate)Delegate.CreateDelegate(typeof(TryDelegate), tryGetValueMethod);


                        //        return DictionaryIndexer(o);
                        //        return DictionaryIndexer2(o);
                        //    };
                    }
                }

                //if (IndexResolver == null)
                //{
                    //var properties = collectionPropertyGetter.ComponentType.GetProperties();
                    //foreach (var property in properties)
                    //{
                    //    var indexParameters = property.GetIndexParameters();
                    //    if (indexParameters.Length == indexValuesParts.Length) // Match
                    //    {
                    //        property.GetGetMethod()
                    //    }
                    //}
                //}

                if (IndexResolver == null)
                {
                    IndexResolver = collection => null;
                }
            }

            class DelegateTypeFactory
            {
                private readonly ModuleBuilder m_module;

                public DelegateTypeFactory()
                {
#if NET452
                    var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                        new AssemblyName("DelegateTypeFactory"), AssemblyBuilderAccess.RunAndCollect);
#endif
#if NET5
                    var assembly = AssemblyBuilder.DefineDynamicAssembly(
                        new AssemblyName("DelegateTypeFactory"), AssemblyBuilderAccess.RunAndCollect);
#endif
                    //m_module = assembly.DefineDynamicModule("DelegateTypeFactory");
                }

                public Type CreateDelegateType(MethodInfo method)
                {
                    string nameBase = string.Format("{0}{1}", method.DeclaringType.Name, method.Name);
                    string name = GetUniqueName(nameBase);

                    var typeBuilder = m_module.DefineType(
                        name, TypeAttributes.Sealed | TypeAttributes.Public, typeof(MulticastDelegate));

                    var constructor = typeBuilder.DefineConstructor(
                        MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                        CallingConventions.Standard, new[] { typeof(object), typeof(IntPtr) });
                    constructor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

                    var parameters = method.GetParameters();

                    var invokeMethod = typeBuilder.DefineMethod(
                        "Invoke", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
                        method.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
                    invokeMethod.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        invokeMethod.DefineParameter(i + 1, ParameterAttributes.None, parameter.Name);
                    }

                    return typeBuilder.CreateType();
                }

                private string GetUniqueName(string nameBase)
                {
                    int number = 2;
                    string name = nameBase;
                    while (m_module.GetType(name) != null)
                        name = nameBase + number++;
                    return name;
                }
            }

            //private delegate bool TryDelegate(object dict, object key, ref object value);

            public IPropertyGetter CollectionPropertyGetter { get; private set; }

            public IPropertyGetter CollectionItemPropertyGetter { get; private set; }

            public Type CollectionItemType { get; private set; }

            public string CollectionIndexerValue { get; private set; }

            public int CollectionIntIndexerValue { get; private set; }

            public Func<object, object> IndexResolver { get; private set; }

            public object GetValue(object obj)
            {
                var collection = CollectionPropertyGetter.GetValue(obj);           
                return IndexResolver(collection);
            }

            public PropertyDescriptor Descriptor
            {
                get { return CollectionPropertyGetter.Descriptor; }
            }

            public Type ComponentType
            {
                get { return CollectionPropertyGetter.ComponentType; }
            }

            public Type PropertyType
            {
                get { return CollectionPropertyGetter.PropertyType; }
            }

            public string PropertyName
            {
                get { return CollectionPropertyGetter.PropertyName; }
            }

            public string PropertyNamePath
            {
                get { return PropertyName; }
            }

            protected IEnumerable<object> CollectionIndexer(object obj)
            {
                int index = CollectionIntIndexerValue;
                //if (int.TryParse(CollectionIndexerValue, out index) == true)
                {
                    var list = obj as IList;
                    if (list != null)
                    {
                        if (index < 0)
                        {
                            index = list.Count - index;
                        }
                        if (index < list.Count)
                        {
                            yield return list[index];
                            yield break;
                        }
                    }
                    else
                    {
                        var array = obj as Array;
                        if (array != null)
                        {
                            if (index < 0)
                            {
                                index = array.Length - index;
                            }
                            if (index < array.Length)
                            {
                                yield return array.GetValue(index);
                                yield break;
                            }
                        }
                        else
                        {
                            var collection = obj as IEnumerable;
                            if (collection != null)
                            {
                                if (index >= 0)
                                {
                                    int i = 0;
                                    foreach (var item in collection)
                                    {
                                        if (index == i++)
                                        {
                                            yield return item;
                                            yield break;
                                        }
                                    }
                                }
                                else
                                {
                                    index = -index;
                                    int i = 0;
                                    foreach (var item in collection.OfType<object>().Reverse())
                                    {
                                        if (index == i++)
                                        {
                                            yield return item;
                                            yield break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            protected IEnumerable<object> WhereIndexer(object obj)
            {
                var collection = obj as IEnumerable<object>;
                if (collection != null)
                {
                    foreach (object item in collection)
                    {
                        var itemValue = CollectionItemPropertyGetter.GetValue(item);
                        string itemValueString = itemValue != null ? itemValue.ToString() : null;
                        if (StringComparer.Ordinal.Equals(itemValueString, CollectionIndexerValue) == true)
                        {
                            yield return item;
                        }
                    }
                }
            }

            protected IEnumerable<object> AllIndexer(object obj)
            {
                return obj as IEnumerable<object> ?? Enumerable.Empty<object>();
            }

            protected IEnumerable<object> DictionaryIndexer(object obj)
            {
                var dictionary = obj as System.Collections.IDictionary;
                if (dictionary != null)
                {
                    if (dictionary.Contains(CollectionIndexerValue) == true)
                    {
                        yield return dictionary[CollectionIndexerValue];
                    }
                }
                else
                {
                    //var ienumerbale = obj as IEnumerable<object>;
                    //if (ienumerbale != null)
                    //{
                    //    foreach (var o in ienumerbale)
                    //    {
                    //        if (obj.Equals(CollectionIndexerValue) == true)
                    //        {
                    //            yield return o;
                    //        }
                    //    }
                    //}
                    //var dictType = typeof(IDictionary<,>);

                    //var otype = obj.GetType();
                    //var idictType = otype.GetInterfaces().Where(t => t.IsGenericType == true && t.GetGenericTypeDefinition() == dictType).First();

                    //var keyType = idictType.GetGenericArguments()[0];
                    //var valueType = idictType.GetGenericArguments()[1];

                    //var tryGetValueMethod = idictType.GetMethod("TryGetValue");

                    //object result = tryGetValueMethod.Invoke(obj, new object[] { CollectionIndexerValue });
                }
                //var collection = obj as IDictionary<, >;
                //if (collection != null)
                //{

                //}
            }

            protected IEnumerable<object> StringSetIndexer(object obj)
            {
                var stringSet = obj as ISet<string>;
                return stringSet != null ? new SingleItemEnumerable<object>(stringSet.Contains(CollectionIndexerValue)) : Enumerable.Empty<object>();
            }

            protected IEnumerable<object> IntSetIndexer(object obj)
            {
                var stringSet = obj as ISet<int>;
                if (stringSet != null)
                {
                    int value;
                    if (int.TryParse(CollectionIndexerValue, out value) == true)
                    {
                        return new SingleItemEnumerable<object>(stringSet.Contains(value));
                    }
                }
                return Enumerable.Empty<object>();
            }

            protected IEnumerable<object> SetIndexer(object obj)
            {
                var setType = typeof(ISet<>);
                var otype = obj.GetType();
                var idsetType = otype.GetInterfaces().Where(t => t.IsGenericType == true && t.GetGenericTypeDefinition() == setType).First();
                var itemType = idsetType.GetGenericArguments()[0];
                var containsMethod = idsetType.GetMethod("Contains");
                if (containsMethod != null)
                {
                    object item = Convert.ChangeType(CollectionIndexerValue, itemType);
                    return new SingleItemEnumerable<object>(containsMethod.Invoke(obj, new[] { item }));

                }
                return Enumerable.Empty<object>();
            }

            public class IndexIndexer
            {
                public object Index { get; set; }

                public IEnumerable<object> GetValue(object obj)
                {
                    var indexer = obj as IIndexer;
                    if (indexer != null)
                    {
                        return new SingleItemEnumerable<object>(indexer[Index]);
                    }
                    return null;
                }
            }

            //protected IEnumerable<object> ThisIndexer(object obj)
            //{
            //    var indexerProperty = obj.GetType().GetProperty("Item", BindingFlags.Instance | BindingFlags.Public);
            //    if (indexerProperty != null)
            //    {
            //        var indexers = indexerProperty.GetIndexParameters();
            //        if (indexers.Length > 0)
            //        {

            //        }
            //    }
            //}

            //public class ThisIndexer
            //{
            //    public object[] Values { get; set; }

            //    public ParameterInfo[] IndexParameters { get; set; }

            //    public PropertyInfo Property { get; set; }

            //    public protected IEnumerable<object> GetValues(object obj)
            //    {
            //        var values = Property.GetValue(obj, Values);
            //        return valöue
            //    }
            //}

            //protected IEnumerable<object> LookupIndexer(object obj)
            //{
            //    ILookup<object>
            //}

            protected IEnumerable<object> ObjectSetIndexer(object obj)
            {
                var objectSet = obj as ISet<object>;
                if (objectSet != null)
                {
                    bool contains = objectSet.Contains(obj);
                    return contains == true ? new SingleItemEnumerable<object>(true) : new SingleItemEnumerable<object>(false);
                }
                return new SingleItemEnumerable<object>(false);
            }
        }

        public static IList<string> SplitPath(string path)
        {
            List<string> paths = new List<string>();
            int pathStartIndex = 0;
            int index = 0;
            while (index < path.Length)
            {
                char c = path[index];
                if (c == '.')
                {
                    paths.Add(path.Substring(pathStartIndex, index - pathStartIndex));
                    pathStartIndex = ++index;
                }
                else if (c == '[')
                {
                    int indexerLevel = 0;
                    do
                    {
                        char ic = path[++index];
                        if (ic == ']')
                        {
                            if (indexerLevel == 0)
                            {
                                break;
                            }
                            indexerLevel--;
                        }
                        else if (ic == '[')
                        {
                            indexerLevel++;
                        }
                    } while (index < path.Length);
                }
                else
                {
                    index++;
                }
            }

            if (pathStartIndex < path.Length)
            {
                paths.Add(path.Substring(pathStartIndex));
            }
            return paths;
        }

        private static Type StringPropertyType = typeof(string);

        public static IPropertyGetter Create(string propertyPath, Type componentType, Func<IPropertyGetter, Func<System.Collections.IEnumerable, object>> indexResolverProvider = null, Func<string, Type> typeResolver = null)
        {
            var key = Tuple.Create(propertyPath, componentType);
            IPropertyGetter getter = null;
            if (PropertyGetter.cachedProperties.TryGetValue(key, out getter) == false)
            {
                lock (PropertyGetter.cachedPropertiesLock)
                {
                    if (PropertyGetter.cachedProperties.TryGetValue(key, out getter) == false)
                    {
                        try
                        {
                            var paths = SplitPath(key.Item1);

                            // Path of getters
                            if (paths.Count > 1)
                            {
                                int count = paths.Count;
                                var getters = new IPropertyGetter[count];
                                Type parentType = null;
                                string path = paths[0];

                                Type castType = null;
                                if (path[0] == '(')
                                {
                                    int i = path.IndexOf(')', 1);
                                    string castTypeName = path.Substring(1, i - 1);                                
                                    castType = typeResolver != null ? typeResolver(castTypeName) : Instance.FindTypes(castTypeName, new Func<Assembly>[] { () => componentType.Assembly, () => Assembly.GetCallingAssembly() }, ignoreCase: true).FirstOrDefault();
                                    path = path.Substring(i + 1);
                                }

                                int indexIndex = path.IndexOf("[");
                                // Indexed getter
                                if (indexIndex >= 0)
                                {
                                    string collectionPropertyPath = path.Substring(0, indexIndex);
                                    var collectionGetter = PropertyGetter.GetPropertyGetters(key.Item2).TryGetValue(collectionPropertyPath); // TODO CAST TYPE
                                    if (collectionGetter != null)
                                    {
                                        var indexedPropertyGetter = new IndexedPropertyGetter(collectionGetter, path.Substring(indexIndex));
                                        getters[0] = indexedPropertyGetter;
                                        parentType = indexedPropertyGetter.CollectionItemType;
                                    }
                                }
                                else
                                {
                                    var propertyGetter = PropertyGetter.GetPropertyGetters(key.Item2).TryGetValue(path);
                                    if (propertyGetter != null)
                                    {
                                        var collectionItemType = propertyGetter.PropertyType != StringPropertyType ? PropertyGetterHelper.GetGenericInterfaceTypes(typeof(IEnumerable<>), castType ?? propertyGetter.PropertyType).FirstOrDefault() : null;
                                        if (collectionItemType != null) // If a enumerable, create a collection getter
                                        {
                                            var collectionGetterPropertyGetter = new IndexedPropertyGetter(propertyGetter, path);
                                            getters[0] = collectionGetterPropertyGetter;
                                            parentType = collectionItemType;
                                        }
                                        else
                                        {
                                            getters[0] = propertyGetter;
                                            parentType = castType ?? getters[0].PropertyType;
                                        }
                                    }
                                }

                                if (parentType == null)
                                {
                                    var nullGetter = new NullPropertyGetter(key.Item1, key.Item2);
                                    PropertyGetter.cachedProperties[key] = nullGetter;
                                    return nullGetter;
                                }

                                IPropertyGetter subGetter = null;
                                for (int index = 1; index < count; index++)
                                {
                                    path = paths[index];

                                    castType = null;
                                    if (path[0] == '(')
                                    {
                                        int i = path.IndexOf(')', 1);
                                        string castTypeName = path.Substring(1, i - 1);
                                        castType = typeResolver != null ? typeResolver(castTypeName) : Instance.FindTypes(castTypeName, new Func<Assembly>[] { () => componentType.Assembly, () => Assembly.GetCallingAssembly() }, ignoreCase: true).FirstOrDefault();
                                        path = path.Substring(i + 1);
                                    }

                                    indexIndex = path.IndexOf("[");

                                    // Indexed getter
                                    if (indexIndex >= 0)
                                    {
                                        string collectionPropertyPath = path.Substring(0, indexIndex); // TODO CAST TYPE
                                        //var collectionGetter = PropertyGetter.GetPropertyGetters(key.Item2).TryGetValue(collectionPropertyPath);
                                        var collectionGetter = PropertyGetter.GetPropertyGetters(castType ?? parentType).TryGetValue(collectionPropertyPath);
                                        var indexedPropertyGetter = new IndexedPropertyGetter(collectionGetter, path.Substring(indexIndex));
                                        subGetter = indexedPropertyGetter;
                                        parentType = indexedPropertyGetter.CollectionItemType;
                                    }
                                    else
                                    {
                                        subGetter = PropertyGetter.GetPropertyGetters(parentType).TryGetValue(path);
                                        var collectionItemType = subGetter.PropertyType != StringPropertyType ? PropertyGetterHelper.GetGenericInterfaceTypes(typeof(IEnumerable<>), castType ?? subGetter.PropertyType).FirstOrDefault() : null;
                                        if (collectionItemType != null) // If a enumerable, create a collection getter
                                        {
                                            var collectionGetterPropertyGetter = new IndexedPropertyGetter(subGetter);
                                            subGetter = collectionGetterPropertyGetter;
                                            parentType = collectionItemType;
                                        }
                                        else
                                        {
                                            parentType = castType ?? subGetter.PropertyType;
                                        }
                                    }

                                    getters[index] = subGetter;

                                }

                                getter = new PropertyGetterPath(getters);
                            }
                            else
                            {
                                string path = paths[0];

                                int index = path.IndexOf("[");
                                if (index >= 0)
                                {
                                    string collectionPropertyPath = path.Substring(0, index);
                                    var collectionGetter = PropertyGetter.GetPropertyGetters(key.Item2).TryGetValue(collectionPropertyPath);

                                    //var indexResolver = indexResolverProvider != null ? indexResolverProvider(collectionGetter) : null;

                                    getter = new IndexedPropertyGetter(collectionGetter, path.Substring(index));
                                }
                                else
                                {
                                    getter = PropertyGetter.GetPropertyGetters(key.Item2).TryGetValue(path);
                                }
                            }

                            PropertyGetter.cachedProperties[key] = getter;
                        }
                        catch (Exception)
                        {
                            PropertyGetter.cachedProperties[key] = new NullPropertyGetter(key.Item1, key.Item2);
                        }
                    }
                }
            }
            return getter;
        }

        public static IPropertyGetter Create(PropertyDescriptor descriptor)
        {
            var key = Tuple.Create(descriptor.Name, descriptor.ComponentType);
            IPropertyGetter getter;
            if (PropertyGetter.cachedProperties.TryGetValue(key, out getter) == false)
            {
                lock (PropertyGetter.cachedPropertiesLock)
                {
                    if (PropertyGetter.cachedProperties.TryGetValue(key, out getter) == false)
                    {
                        try
                        {
                            getter = new PropertyGetter<object>(descriptor);
                            PropertyGetter.cachedProperties[key] = getter;
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }
            return getter;
        }

        public static IEnumerable<KeyValuePair<string, object>> GetPropertyValues(object obj)
        {
            return PropertyGetter.GetPropertyGetters(obj.GetType()).Select(kvp => new KeyValuePair<string, object>(kvp.Key, kvp.Value.GetValue(obj)));
        }

        public static IEnumerable<KeyValuePair<string, object>> GetPropertyValues(object obj, Predicate<IPropertyGetter> predicate)
        {
            return PropertyGetter.GetPropertyGetters(obj.GetType()).Where(pg => predicate(pg.Value)).Select(kvp => new KeyValuePair<string, object>(kvp.Key, kvp.Value.GetValue(obj)));
        }

        //private class PropertyGetterEqualityComparer : IEqualityComparer<IPropertyGetter>
        //{

        //    public bool Equals(IPropertyGetter x, IPropertyGetter y)
        //    {
        //        return object.Equals(x.ComponentType, y.ComponentType) && object.Equals(x.PropertyName, y.PropertyName);
        //    }

        //    public int GetHashCode(IPropertyGetter obj)
        //    {
        //        return HashHelper.GetHashCode(obj.ComponentType, obj.PropertyName);
        //    }
        //}
    }

    public class NullPropertyGetter : IPropertyGetter
    {
        public NullPropertyGetter(PropertyDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public NullPropertyGetter(string propertyName, Type componentType)
        {
            Descriptor = TypeDescriptor.GetProperties(componentType)[propertyName];
        }

        public object GetValue(object obj)
        {
            return null;
        }

        public PropertyDescriptor Descriptor { get; private set; }

        public Type ComponentType { get { return Descriptor.ComponentType; } }

        public Type PropertyType { get { return Descriptor.PropertyType; } }

        public string PropertyName { get { return Descriptor.Name; } }

        public string PropertyNamePath { get { return PropertyName; } }
    }

    public class PropertyGetter<T> : IPropertyGetter<T>, IPropertyGetterDelegate<T>
    {
        public PropertyGetter(PropertyDescriptor descriptor)
        {
            Descriptor = descriptor;

            Getter = CreateDelegate(Descriptor);
        }

        public PropertyGetter(string propertyName, Type componentType)
        {
            Descriptor = TypeDescriptor.GetProperties(componentType)[propertyName];

            Getter = CreateDelegate(Descriptor);
        }

        public Func<object, T> Getter { get; private set; }

        public PropertyDescriptor Descriptor { get; private set; }

        public Type ComponentType { get { return Descriptor.ComponentType; } }

        public Type PropertyType { get { return Descriptor.PropertyType; } }

        public string PropertyName { get { return Descriptor.Name; } }

        public string PropertyNamePath { get { return PropertyName; } }

        public T GetValue(object obj)
        {
            return Getter(obj);
        }

        object IPropertyGetter.GetValue(object obj)
        {
            return Getter(obj);
        }

        public override string ToString()
        {
            return string.Format("PropertyGetter - Component: {0} Name: {1} Type : {2}", ComponentType, PropertyName, PropertyType.Name);
        }

        public static Func<object, T> CreateDelegate(PropertyDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            var propertyType = descriptor.PropertyType;

            TypeConverter converter = null;
            // Do we have a converter that can convert the value, if needed, to correct type?
            if (typeof(T).IsAssignableFrom(descriptor.PropertyType) == false)
            {
                if (descriptor.Converter != null && descriptor.Converter.CanConvertTo(typeof(T)) == true)
                {
                    converter = descriptor.Converter;
                }
                else
                {
                    throw new Exception("Can not create delegate");
                }
            }

            //bool nullable = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>); (does not support nullable)

            // If the propertydescriptor is a the standard ReflectPropertyDescriptor, use our run-time compiled property getter instead 
            if (descriptor.GetType().Name == "ReflectPropertyDescriptor" && descriptor.ComponentType.IsInterface == false)
            {
                PropertyInfo propertyInfo = null;
                try
                {
                    propertyInfo = descriptor.ComponentType.GetProperty(descriptor.Name);
                }
                catch (AmbiguousMatchException)
                {
                    propertyInfo = descriptor.ComponentType.GetProperty(descriptor.Name, BindingFlags.DeclaredOnly);
                    if (propertyInfo == null)
                    {
                        propertyInfo = descriptor.ComponentType.GetProperty(descriptor.Name, descriptor.PropertyType);
                    }
                }

                if (converter != null)
                {
                    var getter = PropertyGetter<object>.CreatePropertyGetter(propertyInfo);
                    if (getter != null)
                    {
                        if (propertyType.IsValueType == true)
                        {
                            return item => (T)converter.ConvertTo(getter(item), typeof(T));
                        }
                        else
                        {
                            return item =>
                            {
                                object value = converter.ConvertTo(getter(item), typeof(T));
                                if (value != null)
                                {
                                    return (T)value;
                                }
                                return default(T);
                            };
                        }
                    }
                }
                else
                {
                    var getter = CreatePropertyGetter(propertyInfo);
                    if (getter != null)
                    {
                        return getter;
                    }
                }
            }

            if (converter != null)
            {
                return item =>
                {
                    object value = descriptor.GetValue(item);
                    if (value is T)
                    {
                        return (T)value;
                    }
                    else
                    {
                        value = converter.ConvertTo(value, typeof(T));
                        if (value != null)
                        {
                            return (T)value;
                        }
                        return default(T);
                    }
                };
            }
            else
            {
                return item =>
                {
                    object value = descriptor.GetValue(item);
                    if (value != null)
                    {
                        return (T)value;
                    }
                    return default(T);
                };
            }

        }

        private static Dictionary<Tuple<Type, string>, Func<object, T>> cachedPropertyGetters = new Dictionary<Tuple<Type, string>, Func<object, T>>();

        private static object cachedPropertyGettersLock = new object();
        
        public static Func<object, T> CreatePropertyGetter(PropertyInfo info)
        {
            var key = Tuple.Create(info.DeclaringType, info.Name);
            Func<object, T> getter;
            if (PropertyGetter<T>.cachedPropertyGetters.TryGetValue(key, out getter) == false)
            {
                lock (PropertyGetter<T>.cachedPropertyGettersLock)
                {
                    if (PropertyGetter<T>.cachedPropertyGetters.TryGetValue(key, out getter) == false)
                    {
                        getter = PropertyGetter<T>.CreatePropertyGetterInternal(info);
                        PropertyGetter<T>.cachedPropertyGetters[key] = getter;
                    }
                }
            }
            return getter;
        }

        private static Func<object, T> CreatePropertyGetterInternal(PropertyInfo info)
        {
            if (info.CanRead == true)
            {
                MethodInfo getMethod = info.GetGetMethod(true);

                DynamicMethod dynamicMethod = new DynamicMethod("GetValue", typeof(T), new Type[] { typeof(object) }, info.DeclaringType);

                ILGenerator generator = dynamicMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                //generator.Emit(OpCodes.Castclass, info.DeclaringType); // ???

                if (getMethod.IsVirtual == true)
                {
                    generator.Emit(OpCodes.Callvirt, getMethod);
                }
                else
                {
                    generator.Emit(OpCodes.Call, getMethod);
                }

                if (typeof(T) != getMethod.ReturnType)
                {
                    if (getMethod.ReturnType.IsValueType == true)
                    {
                        generator.Emit(OpCodes.Box, getMethod.ReturnType);
                        generator.Emit(OpCodes.Ret);
                    }
                    else
                    {
                        if (getMethod.ReturnType.IsSubclassOf(typeof(T)) == false)
                        {
                            Label notNull = generator.DefineLabel();
                            generator.DeclareLocal(getMethod.ReturnType);
                            generator.Emit(OpCodes.Stloc_0);
                            generator.Emit(OpCodes.Ldloc_0);
                            generator.Emit(OpCodes.Brtrue_S, notNull); // Check if Property return null (if so return null)
                            generator.Emit(OpCodes.Ldnull);
                            generator.Emit(OpCodes.Ret);
                            generator.MarkLabel(notNull);
                            generator.Emit(OpCodes.Ldloc_0);
                            generator.Emit(OpCodes.Castclass, typeof(T));
                            generator.Emit(OpCodes.Ret);
                        }
                        else
                        {
                            generator.Emit(OpCodes.Ret);
                        }
                    }
                }
                else
                {
                    generator.Emit(OpCodes.Ret);
                }
                

                return (Func<object, T>)dynamicMethod.CreateDelegate(typeof(Func<object, T>));
            }

            return null;
        }
    }

    public class PropertySetter<T> : IPropertySetter<T>, IPropertySetterDelegate<T>
    {
        public PropertySetter(PropertyDescriptor descriptor)
        {
            Setter = CreateDelegate(descriptor);

            ComponentType = descriptor.ComponentType;

            PropertyType = descriptor.PropertyType;

            PropertyName = descriptor.Name;
        }

        public PropertySetter(string propertyName, Type componentType)
            : this(TypeDescriptor.GetProperties(componentType)[propertyName])
        {
        }

        public Action<object, T> Setter { get; private set; }

        public Type ComponentType { get; private set; }

        public Type PropertyType { get; private set; }

        public string PropertyName { get; private set; }

        public void SetValue(object obj, T value)
        {
            Setter(obj, value);
        }

        void IPropertySetter.SetValue(object obj, object value)
        {
            if (value is T)
            {
                Setter(obj, (T)value);
            }
        }

        public override string ToString()
        {
            return string.Format("PropertySetter - Component: {0} Name: {1} Type : {2}", ComponentType, PropertyName, PropertyType.Name);
        }

        public static Action<object, T> CreateDelegate(PropertyDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            if (descriptor.IsReadOnly == true)
            {
                return null;
            }

            var propertyType = descriptor.PropertyType;

            TypeConverter converter = null;
            // Do we have a converter that can convert the value, if needed, to correct type?
            if (typeof(T).IsAssignableFrom(descriptor.PropertyType) == false)
            {
                if (descriptor.Converter != null && descriptor.Converter.CanConvertFrom(typeof(T)) == true)
                {
                    converter = descriptor.Converter;
                }
                else
                {
                    throw new Exception("Can not create delegate");
                }
            }

            if (descriptor.GetType().Name == "ReflectPropertyDescriptor")
            {
                var propertySetter = CreatePropertySetter(descriptor);
                if (propertySetter != null)
                {
                    if (converter != null)
                    {
                        if (propertyType.IsValueType == true)
                        {
                            return (item, value) => propertySetter(item, (T)converter.ConvertFrom(value));
                        }
                        else
                        {
                            return (item, value) =>
                            {
                                object convertedValue = converter.ConvertFrom(value);
                                if (convertedValue is T)
                                {
                                    propertySetter(item, (T)convertedValue);
                                }
                                else
                                {
                                    propertySetter(item, default(T));
                                }
                            };
                        }
                    }
                    else
                    {
                        return propertySetter;
                    }
                }
            }

            if (converter != null)
            {
                return (item, value) => descriptor.SetValue(item, converter.ConvertFrom(value));
            }
            else
            {
                return (item, value) => descriptor.SetValue(item, value);
            }
        }

        // Obsolete
        public static Action<object, T> CreatePropertySetter(PropertyDescriptor descriptor)
        {
            return CreatePropertySetter(descriptor.ComponentType.GetProperty(descriptor.Name));
        }

        public static Action<object, T> CreatePropertySetter(PropertyInfo info)
        {
            //MethodInfo mi = info.GetSetMethod();
            //if (mi != null)
            //{
            //    // NOTE:  As reader pointed out...
            //    //  Calling a property's get accessor is faster/cleaner using
            //    //  Delegate.CreateDelegate rather than Reflection.Emit 

            //    return (Action<TObj, T>)Delegate.CreateDelegate(typeof(Action<TObj, T>), mi);
            //}
            //return null;
            if (info.CanWrite == true)
            {
                MethodInfo setMethod = info.GetSetMethod(true);

                DynamicMethod dynamicMethod = new DynamicMethod("SetValue", null, new Type[] { typeof(object), typeof(T) }, info.DeclaringType);

                ILGenerator generator = dynamicMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Castclass, info.DeclaringType);
                generator.Emit(OpCodes.Ldarg_1);
                if (info.PropertyType.IsClass == true)
                {
                    generator.Emit(OpCodes.Castclass, info.PropertyType);
                }
                else
                {
                    generator.Emit(OpCodes.Unbox_Any, info.PropertyType);
                }

                if (setMethod.IsVirtual == true)
                {
                    generator.EmitCall(OpCodes.Callvirt, setMethod, null);
                }
                else
                {
                    generator.EmitCall(OpCodes.Call, setMethod, null);
                }
                generator.Emit(OpCodes.Ret);

                return (Action<object, T>)dynamicMethod.CreateDelegate(typeof(Action<object, T>));
            }

            return null;
        }
    }

    public class PropertyAccessor<T> : IPropertyGetter<T>, IPropertySetter<T>
    {
        public PropertyAccessor(PropertyDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }

            Descriptor = descriptor;

            var getter = PropertyGetter<T>.CreateDelegate(descriptor);
            CanRead = getter != null;
            Getter = getter;

            var setter = PropertySetter<T>.CreateDelegate(descriptor);
            CanWrite = setter != null;
            Setter = setter;
        }

        public PropertyAccessor(PropertyInfo propertyInfo) 
            : this(propertyInfo.DeclaringType, propertyInfo.Name) 
        { }

        public PropertyAccessor(Type propertyType, string propertyName)
            : this(TypeDescriptor.GetProperties(propertyType).OfType<PropertyDescriptor>().FirstOrDefault(p => p.Name == propertyName))
        { }

        public PropertyDescriptor Descriptor { get; private set; }

        public Type ComponentType { get { return Descriptor.ComponentType; } }

        public Type PropertyType { get { return Descriptor.PropertyType; } }

        public string PropertyName { get { return Descriptor.Name; } }

        public string PropertyNamePath { get { return PropertyName; } }

        public bool CanWrite { get; private set; }

        public bool CanRead { get; private set; }

        public Func<object, T> Getter { get; protected set; }

        public Action<object, T> Setter { get; private set; }

        public T GetValue(object obj)
        {
            return Getter(obj);
        }

        object IPropertyGetter.GetValue(object obj)
        {
            return Getter(obj);
        }

        public void SetValue(object obj, T value)
        {
            try
            {
                Setter(obj, value);
            }
            catch (Exception e) {
                
                Console.WriteLine(e.Message);
            }
        }

        void IPropertySetter.SetValue(object obj, object value)
        {
            if (value is T)
            {
                Setter(obj, (T)value);
            }
        }

        public override string ToString()
        {
            return string.Format("PropertyAccessor - Component: {0} Name: {1} Type : {2}", ComponentType, PropertyName, PropertyType.Name);
        }
    }

    public interface IIndexer
    {
        object this[object index] { get; }
    }

    public interface IIndexer<in TIndex, out TValue>
    {
        TValue this[TIndex index] { get; }
    }

    public interface IIndexer<in TIndex1, in TIndex2, out TValue>
    {
        TValue this[TIndex1 index1, TIndex2 index2] { get; }
    }

    public interface IIndexer<in TIndex1, in TIndex2, in TIndex3, out TValue>
    {
        TValue this[TIndex1 index1, TIndex2 index2, TIndex3 index3] { get; }
    }

    #region Old code - verify new code first before removal

    //public class PropertyGetter : IPropertyGetter
    //{
    //    public PropertyGetter(PropertyDescriptor propertyDescriptor)
    //    {
    //        Init(propertyDescriptor);
    //    }

    //    public PropertyGetter(Type type, string propertyName)
    //    {
    //        var descriptor = TypeDescriptor.GetProperties(type).OfType<PropertyDescriptor>().FirstOrDefault(p => p.Name == propertyName);
    //        if (descriptor == null)
    //        {
    //            throw new ArgumentException(string.Format("Could not find public property named {0} on type {1}", propertyName, type));
    //        }

    //        Init(descriptor);
    //    }

    //    public string PropertyName { get; protected set; }

    //    public Func<object, object> Getter { get; protected set; }

    //    public object GetValue(object obj)
    //    {
    //        return Getter(obj);
    //    }

    //    public T GetValue<T>(object obj)
    //    {
    //        return (T)Getter(obj);
    //    }

    //    public override string ToString()
    //    {
    //        return PropertyName;
    //    }

    //    protected void Init(PropertyDescriptor descriptor)
    //    {
    //        if (descriptor == null)
    //        {
    //            throw new ArgumentNullException("descriptor");
    //        }

    //        TypeConverter converter = null;

    //        if (descriptor.Converter != null && (descriptor.Converter is StringConverter) == false && descriptor.Converter.CanConvertTo(descriptor.PropertyType) == true)
    //        {
    //            converter = descriptor.Converter;
    //        }

    //        var propertyType = descriptor.PropertyType;
    //        PropertyName = descriptor.Name;

    //        var delegateDescriptor = descriptor as IDelegatingPropertyDescriptor;
    //        if (delegateDescriptor != null)
    //        {
    //            if (converter != null)
    //            {
    //                var getter = delegateDescriptor.Getter;
    //                Getter = item => converter.ConvertTo(getter(item), propertyType);
    //            }
    //            else
    //            {
    //                Getter = delegateDescriptor.Getter;
    //            }
    //        }
    //        else
    //        {
    //            if (descriptor == null)
    //            {
    //                throw new ArgumentNullException("propertyDescriptor");
    //            }

    //            var getter = PropertyGetter.Create(descriptor.ComponentType.GetProperty(descriptor.Name));
    //            if (getter == null)
    //            {
    //                Getter = PropertyGetter.IllegalGetter;
    //            }
    //            else if (converter != null)
    //            {
    //                Getter = item => converter.ConvertTo(getter(item), propertyType);
    //            }
    //            else
    //            {
    //                Getter = getter;
    //            }

    //        }
    //    }

    //    public static Func<object, object> Create(PropertyInfo info)
    //    {
    //        //Type ComponentType = info.DeclaringType;
    //        //Type PropertyType = info.PropertyType;

    //        if (info.CanRead == true)
    //        {
    //            MethodInfo getMethod = info.GetGetMethod(true);

    //            DynamicMethod dynamicMethod = new DynamicMethod("GetValue", typeof(object), new Type[] { typeof(object) }, info.DeclaringType);

    //            ILGenerator generator = dynamicMethod.GetILGenerator();
    //            generator.Emit(OpCodes.Ldarg_0);
    //            generator.Emit(OpCodes.Castclass, info.DeclaringType);
    //            if (getMethod.IsVirtual == true)
    //            {
    //                //if (method.ReturnType.IsValueType == true)
    //                //{
    //                //    generator.Emit(OpCodes.Constrained);
    //                //}
    //                generator.EmitCall(OpCodes.Callvirt, getMethod, null);
    //            }
    //            else
    //            {
    //                generator.EmitCall(OpCodes.Call, getMethod, null);
    //            }
    //            if (getMethod.ReturnType.IsValueType == true)
    //            {
    //                generator.Emit(OpCodes.Box, getMethod.ReturnType);
    //            }
    //            generator.Emit(OpCodes.Ret);

    //            return (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));

    //            //CanRead = true;
    //        }

    //        return null;
    //    }

    //    private static object IllegalGetter(object obj)
    //    {
    //        return null;
    //    }

    //    public static PropertyGetter Create<T>(string propertyName)
    //    {
    //        var descriptor = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>().FirstOrDefault(p => p.Name == propertyName);
    //        return descriptor != null ? new PropertyGetter(descriptor) : null;
    //    }

    //    public static Func<TIn, TOut> CreateDelegate<TIn, TOut>(string propertyName)
    //    {
    //        var instance = Expression.Parameter(typeof(TIn));
    //        var propertyInfo = typeof(TIn).GetProperty(propertyName);
    //        var property = Expression.Property(instance, propertyInfo.Name);
    //        Expression body = null;
    //        if (propertyInfo.PropertyType != typeof(TOut))
    //        {
    //            if (propertyInfo.PropertyType.IsValueType == true)
    //            {
    //                body = Expression.Convert(property, typeof(TOut));
    //            }
    //            else
    //            {
    //                body = Expression.TypeAs(property, typeof(TOut));
    //            }
    //        }
    //        else
    //        {
    //            body = property;
    //        }

    //        var del = (Func<TIn, TOut>)Expression.Lambda(body, instance).Compile();

    //        return del;
    //    }
    //}

    //public class StringPropertyGetter : IPropertyGetter
    //{
    //    public StringPropertyGetter(PropertyDescriptor descriptor)
    //    {
    //        Init(descriptor);
    //    }

    //    public StringPropertyGetter(Type type, string propertyName)
    //    {
    //        var descriptor = TypeDescriptor.GetProperties(type).OfType<PropertyDescriptor>().FirstOrDefault(p => p.Name == propertyName);
    //        if (descriptor == null)
    //        {
    //            throw new ArgumentException(string.Format("Could not find public property named {0} on type {1}", propertyName, type));
    //        }

    //        Init(descriptor);
    //    }

    //    protected void Init(PropertyDescriptor descriptor)
    //    {
    //        if (descriptor == null)
    //        {
    //            throw new ArgumentNullException("descriptor");
    //        }

    //        TypeConverter converter = null;
    //        if (descriptor.Converter != null && (descriptor.Converter is StringConverter) == false && descriptor.Converter.CanConvertTo(typeof(string)) == true)
    //        {
    //            converter = descriptor.Converter;
    //            //Getter = item => descriptor.Converter.ConvertToString(item);
    //        }

    //        var delegateDescriptor = descriptor as IDelegatingPropertyDescriptor;
    //        if (delegateDescriptor != null)
    //        {
    //            var delegateGetter = delegateDescriptor.Getter;
    //            if (converter != null)
    //            {
    //                Getter = item =>
    //                {
    //                    object value = delegateGetter(item);
    //                    return value != null ? converter.ConvertToString(value) : null;
    //                };
    //            }
    //            else
    //            {
    //                Getter = item =>
    //                {
    //                    object value = delegateGetter(item);
    //                    return value != null ? value.ToString() : null;
    //                };
    //            }
    //        }
    //        else if (descriptor.PropertyType.IsEnum == true || converter != null)
    //        {
    //            var getter = PropertyGetter.Create(descriptor.ComponentType.GetProperty(descriptor.Name));
    //            if (getter == null)
    //            {
    //                getter = StringPropertyGetter.IllegalGetter;
    //            }
    //            else if (converter != null)
    //            {
    //                Getter = item => converter.ConvertToString(getter(item));
    //            }
    //            else
    //            {
    //                Getter = item => getter(item).ToString();
    //            }
    //        }
    //        else
    //        {
    //            Getter = StringPropertyGetter.Create(descriptor.ComponentType, descriptor.Name);
    //        }
    //    }

    //    public Func<object, string> Getter { get; protected set; }

    //    public string GetValue(object obj)
    //    {
    //        return Getter(obj);
    //    }

    //    object IPropertyGetter.GetValue(object obj)
    //    {
    //        return Getter(obj);
    //    }

    //    public static string IllegalGetter(object obj)
    //    {
    //        return null;
    //    }

    //    public static Func<object, string> Create<T>(string propertyName)
    //    {
    //        PropertyInfo info = typeof(T).GetProperty(propertyName);
    //        return StringPropertyGetter.Create(info);
    //    }

    //    public static Func<object, string> Create(Type objectType, string propertyName)
    //    {
    //        PropertyInfo info = objectType.GetProperty(propertyName);
    //        return StringPropertyGetter.Create(info);
    //    }

    //    public static Func<object, string> Create(PropertyInfo info)
    //    {
    //        if (info == null)
    //        {
    //            throw new ArgumentNullException("info");
    //        }

    //        if (info.CanRead == true)
    //        {
    //            //var instance = Expression.Parameter(typeof(object)/*info.DeclaringType*/);
    //            //var cast = Expression.Convert(instance, info.DeclaringType);
    //            //var property = Expression.Property(cast, info.Name);
    //            //Expression body = null;
    //            //if (info.PropertyType != typeof(string))
    //            //{
    //            //    if (info.PropertyType.IsValueType == true)
    //            //    {
    //            //        body = Expression.Call(property, "ToString", Type.EmptyTypes);
    //            //    }
    //            //    else
    //            //    {
    //            //        var returnTarget = Expression.Label(typeof(string));
    //            //        body = Expression.Block(
    //            //            Expression.IfThenElse(Expression.Equal(property, Expression.Constant(null)), 
    //            //                Expression.Return(returnTarget, Expression.Constant(null, typeof(string))),
    //            //                Expression.Return(returnTarget, Expression.Call(property, "ToString", Type.EmptyTypes))),
    //            //            Expression.Label(returnTarget, Expression.Constant(null, typeof(string))) 
    //            //        );
    //            //    }
    //            //}
    //            //else
    //            //{
    //            //    body = property;
    //            //}

    //            //var del = (Func<object, string>)Expression.Lambda(body, instance).Compile();

    //            //return del;

    //            MethodInfo getMethod = info.GetGetMethod(true);

    //            DynamicMethod dynamicMethod = new DynamicMethod("GetValue", typeof(string), new Type[] { typeof(object) }, info.DeclaringType);

    //            MethodInfo toStringMethod = info.PropertyType.GetMethod("ToString", Type.EmptyTypes);

    //            ILGenerator generator = dynamicMethod.GetILGenerator();

    //            if (getMethod.ReturnType.IsValueType == true) // Property is a value type, it can not be null and needs no null checking before call to ToString()
    //            {
    //                LocalBuilder local = generator.DeclareLocal(getMethod.ReturnType);
    //                generator.Emit(OpCodes.Ldarg_0);
    //                generator.Emit(OpCodes.Callvirt, getMethod); // To speed things up, use Call virt if object is guarantied to be non-null (and Property of object is non-virtual)
    //                generator.Emit(OpCodes.Stloc_0);
    //                generator.Emit(OpCodes.Ldloca_S, local);
    //                generator.Emit(OpCodes.Call, toStringMethod);
    //                generator.Emit(OpCodes.Ret);
    //            }
    //            else if (getMethod.ReturnType == typeof(string)) // Property is string, return as is
    //            {
    //                generator.Emit(OpCodes.Ldarg_0);
    //                generator.Emit(OpCodes.Callvirt, getMethod);
    //                generator.Emit(OpCodes.Ret);
    //            }
    //            else // Property is a reference type, it can be null and needs null checking before call to ToString()
    //            {
    //                Label labelToString = generator.DefineLabel();
    //                generator.DeclareLocal(getMethod.ReturnType);
    //                generator.Emit(OpCodes.Ldarg_0);
    //                generator.Emit(OpCodes.Callvirt, getMethod);
    //                generator.Emit(OpCodes.Stloc_0);
    //                generator.Emit(OpCodes.Ldloc_0);
    //                generator.Emit(OpCodes.Brtrue_S, labelToString); // Check if Property return null (if so return null)
    //                generator.Emit(OpCodes.Ldnull);
    //                generator.Emit(OpCodes.Ret);
    //                generator.MarkLabel(labelToString);
    //                generator.Emit(OpCodes.Ldloc_0);
    //                generator.Emit(OpCodes.Callvirt, toStringMethod);
    //                generator.Emit(OpCodes.Ret);
    //            }

    //            return (Func<object, string>)dynamicMethod.CreateDelegate(typeof(Func<object, string>));
    //        }

    //        return null;
    //    }
    //}

    //public class PropertyGetter<TObject>
    //{
    //    public static Func<TObject, TProperty> Create<TProperty>(Expression<Func<TObject, TProperty>> selector)
    //    {
    //        MemberExpression expression = (MemberExpression)selector.Body;
    //        string propertyName = expression.Member.Name;
    //        return Create<TProperty>(propertyName);
    //    }

    //    public static Func<TObject, TProperty> Create<TProperty>(string propertyName)
    //    {
    //        PropertyInfo info = typeof(TObject).GetProperty(propertyName);
    //        return Create<TProperty>(info);
    //    }

    //    public static Func<TObject, TProperty> Create<TProperty>(PropertyInfo info)
    //    {
    //        if (info.CanRead == true)
    //        {
    //            MethodInfo getMethod = info.GetGetMethod(true);
    //            DynamicMethod dynamicMethod = new DynamicMethod("GetValue", info.PropertyType, new Type[] { typeof(TObject) }, info.DeclaringType);

    //            ILGenerator generator = dynamicMethod.GetILGenerator();
    //            generator.Emit(OpCodes.Ldarg_0);
    //            if (getMethod.IsVirtual == true)
    //            {
    //                generator.EmitCall(OpCodes.Callvirt, getMethod, null);
    //            }
    //            else
    //            {
    //                generator.EmitCall(OpCodes.Call, getMethod, null);
    //            }
    //            generator.Emit(OpCodes.Ret);

    //            return (Func<TObject, TProperty>)dynamicMethod.CreateDelegate(typeof(Func<TObject, TProperty>));
    //        }
    //        return null;
    //    }

    //    //public static Func<TObject, object> Create2(string propertyName)
    //    //{
    //    //    PropertyInfo info = typeof(TObject).GetProperty(propertyName);

    //    //    if (info.CanRead == true)
    //    //    {
    //    //        MethodInfo getMethod = info.GetGetMethod(true);
    //    //        DynamicMethod dynamicMethod = new DynamicMethod("GetValue", typeof(object), new Type[] { typeof(TObject) }, info.DeclaringType);

    //    //        ILGenerator generator = dynamicMethod.GetILGenerator();
    //    //        if (getMethod.IsVirtual == true)
    //    //        {
    //    //            generator.EmitCall(OpCodes.Callvirt, getMethod, null);
    //    //        }
    //    //        else
    //    //        {
    //    //            generator.EmitCall(OpCodes.Call, getMethod, null);
    //    //        }
    //    //        if (getMethod.ReturnType.IsValueType == true)
    //    //        {
    //    //            generator.Emit(OpCodes.Box, getMethod.ReturnType);
    //    //        }
    //    //        generator.Emit(OpCodes.Ret);

    //    //        return (Func<TObject, object>)dynamicMethod.CreateDelegate(typeof(Func<TObject, object>));
    //    //    }
    //    //    return null;
    //    //}
    //}

    #endregion
}
