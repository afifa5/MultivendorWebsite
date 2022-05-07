
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MultivendorWebViewer
{
    [Flags]
    public enum CryptographicMode { None = 0x0, Encrypted = 0x1, Signed = 0x2, EncryptedAndSigned = 0x3 }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public class ExtensionAttribute : Attribute
    {
        public ExtensionAttribute()
        {
        }

        public ExtensionAttribute(Type implementType)
        {
            TargetType = implementType;
        }

        public Type TargetType { get; private set; }
    }

    public static class Instance
    {
        public static bool Enable = true;
        public static bool UseMemoryStreamSerializer = true;
        public static List<string> RegisteredAssemblies { get { return registeredAssemblies; } }

        #region Register
#if DEBUG
        private static bool directoryRegistered = false;
#endif

        // Save loaded assemblies file names
        private static List<string> registeredAssemblies = new List<string>();

        private static Dictionary<Type, List<Type>> registeredExtensionTypes = new Dictionary<Type, List<Type>>();

        private static List<Type> registeredUnspecifiedExtensionTypes = new List<Type>();

        private static Dictionary<Type, Type[]> usedRegisteredUnspecifiedExtensionTypes = new Dictionary<Type, Type[]>();

        private delegate void InitDelegate();

        private static List<InitDelegate> registeredInitDelegates = new List<InitDelegate>();

        private static bool updateActivatorSuspended = false;

        private static bool pendingUpdateActivators = false;

        /// <summary>
        /// Are any types loaded from customize module?
        /// </summary>
        /// <returns></returns>
        public static bool IsEmpty()
        {
            return registeredAssemblies.Count == 0;
        }

        public static void Reset()
        {
            SuspendUpdateActivators();

            registeredExtensionTypes.Clear();
            registeredUnspecifiedExtensionTypes.Clear();
            usedRegisteredUnspecifiedExtensionTypes.Clear();
            registeredInitDelegates.Clear();
            registeredAssemblies.Clear();

            ResumeUpdateActivators();
            UpdateActivators();
#if DEBUG
            directoryRegistered = false;
#endif
            Enable = true;
        }

        public static void Register(Assembly assembly)
        {
            if (Enable == false) return;

            Instance.Register(new[] { assembly });
        }

        public static void Register(IEnumerable<Assembly> assemblies)
        {
            if (Enable == false) return;

            bool resume = Instance.updateActivatorSuspended == false;

            Instance.SuspendUpdateActivators();

            foreach (Assembly asm in assemblies)
            {
                Instance.RegisterAssembly(asm);
            }

            if (resume == true)
            {
                Instance.ResumeUpdateActivators();
            }
        }

        public static void Register(params string[] filenames)
        {
            if (Enable == false) return;

            bool resume = Instance.updateActivatorSuspended == false;

            Instance.SuspendUpdateActivators();

            // Get available image file forms
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            List<string> extensions = new List<string>();
            foreach (string encoderExtension in encoders.Select(c => c.FilenameExtension).ToList())
            {
                string[] encoderExtensionArr = encoderExtension.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                encoderExtensionArr.ForEach(i => extensions.Add(i.Replace("*", "").ToLower()));
            }

            // Extensions of common customize files that will not be loaded as modules
            extensions.AddRange(new string[] { ".ico", ".xsl", ".xslx", ".mrt", ".config", ".css", ".ttf", ".sql", ".csv", ".png", ".jpg" });

            foreach (string filename in filenames)
            {
                try
                {
                    // Skip a file that is known not to be a customize dll file, from its extension
                    string extension = Path.GetExtension(filename);
                    if (extensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase) == true) continue;

                    Assembly assembly = Assembly.UnsafeLoadFrom(filename);

                    Instance.RegisterAssembly(assembly);
                    Instance.registeredAssemblies.Add(filename);
                }
                catch (BadImageFormatException ex)
                {
                    Trace.WriteLine(String.Format("Failed to load file: {0}", ex.Message));
                    // Swallow
                }
            }

            if (resume == true)
            {
                Instance.ResumeUpdateActivators();
            }
        }

        public static void RegisterDirectory(string path, string searchPattern = null, SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (Enable == false) return;

            bool resume = Instance.updateActivatorSuspended == false;

            Instance.SuspendUpdateActivators();

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Exists)
                {
                    FileInfo[] files = searchPattern == null ? directoryInfo.GetFiles() : directoryInfo.GetFiles(searchPattern, options);
                    Instance.Register(files.Select(f => f.FullName).ToArray());
                }
            }
            finally
            {
                if (resume == true)
                {
                    Instance.ResumeUpdateActivators();
                }
            }

#if DEBUG
            directoryRegistered = true;
#endif
        }

        public static void Register<T>(Type type)
        {
            if (Enable == false) return;

            Instance.Register(type, typeof(T));
        }

        private static Type[] GetAllUnspecifiedExtension(Type type)
        {
            Type[] usedTypes;
            if (Instance.usedRegisteredUnspecifiedExtensionTypes.TryGetValue(type, out usedTypes) == false)
            {
                List<Type> types = new List<Type>();
                if (type.IsInterface == false)
                {
                    foreach (Type unspecifiedType in Instance.registeredUnspecifiedExtensionTypes)
                    {
                        if (unspecifiedType.IsClass == true && unspecifiedType.IsSubclassOf(type) == true)
                        {
                            types.Add(unspecifiedType);
                        }
                    }
                }
                else
                {
                    foreach (Type unspecifiedType in Instance.registeredUnspecifiedExtensionTypes)
                    {
                        if (unspecifiedType.IsClass == true && unspecifiedType.GetInterface(type.Name) != null)
                        {
                            types.Add(unspecifiedType);
                        }
                    }
                }

                usedTypes = types.ToArray();

                Instance.usedRegisteredUnspecifiedExtensionTypes[type] = usedTypes; // TODO MAKE THREAD SAFE
            }

            return usedTypes;
        }

        public static bool ExistsExtension(Type type)
        {
            return GetExtension(type, false) != null;
        }

        /// <summary>
        /// Gets all registered extension types.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes()
        {
            return Instance.registeredExtensionTypes.Values.SelectMany(t => t);
        }

        public static Type GetExtension(Type type, bool fallback = true)
        {
            List<Type> implementList;
            if (Instance.registeredExtensionTypes.TryGetValue(type, out implementList) == true)
            {
                return implementList[0];
            }

            Type[] implementedTypes = Instance.GetAllUnspecifiedExtension(type);
            if (implementedTypes.Length > 0)
            {
                return implementedTypes[0];
            }

            return fallback == true ? type : null;
        }

        public static IEnumerable<Type> FindTypes(string name, bool ignoreCase = false, bool searchExtensions = true)
        {
            return Instance.FindTypes(name, AppDomain.CurrentDomain.GetAssemblies().Select(a => { Func<Assembly> func = () => a; return func; }), ignoreCase, searchExtensions);
        }

        public static IEnumerable<Type> FindTypes(string name, Assembly[] assemblies)
        {
            return Instance.FindTypes(name, assemblies.Select(a => { Func<Assembly> func = () => a; return func; }), false, true);
        }

        public static IEnumerable<Type> FindTypes(string name, params Func<Assembly>[] assemblies)
        {
            return Instance.FindTypes(name, assemblies, false, true);
        }

        public static IEnumerable<Type> FindTypes(string name, IEnumerable<Func<Assembly>> assemblies, bool ignoreCase = false, bool searchExtensions = true)
        {
            bool fullName = name.Contains(".");

            var stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            if (searchExtensions == true)
            {
                foreach (var type in Instance.GetTypes().Where(t => name.Equals((fullName == true ? t.FullName : t.Name), stringComparison) == true))
                {
                    yield return type;
                }
            }

            var searchedAssemblies = new HashSet<Assembly>();

            foreach (var assembly in assemblies)
            {
                var asm = assembly();
                if (asm != null && searchedAssemblies.Contains(asm) == false)
                {
                    searchedAssemblies.Add(asm);

                    ILookup<string, Type> assemblyTypes = null;
                    if (fullName == false)
                    {
                        assemblyTypes = exportedTypesByName.GetOrAdd(asm.FullName, a =>
                        {
                            try
                            {
                                return asm.GetExportedTypes().ToLookup(t => t.Name, StringComparer.OrdinalIgnoreCase);
                            }
                            catch (Exception)
                            {
                                return EmptyLookup<string, Type>.Default;
                            }
                        });
                    }
                    else
                    {
                        assemblyTypes = exportedTypesByName.GetOrAdd(asm.FullName, a =>
                        {
                            try
                            {
                                return asm.GetExportedTypes().ToLookup(t => t.FullName, StringComparer.OrdinalIgnoreCase);
                            }
                            catch (Exception)
                            {
                                return EmptyLookup<string, Type>.Default;
                            }
                        });
                    }

                    var types = ignoreCase == true ? assemblyTypes[name] : assemblyTypes[name].Where(t => name.Equals((fullName == true ? t.FullName : t.Name), stringComparison) == true);

                    foreach (var type in types)
                    {
                        if (searchExtensions == true)
                        {
                            yield return Instance.GetExtension(type);
                        }
                        else
                        {
                            yield return type;
                        }
                    }

                    //foreach (var type in asm.GetExportedTypes().Where(t => name.Equals((fullName == true ? t.FullName : t.Name), stringComparison) == true))
                    //{
                    //    if (searchExtensions == true)
                    //    {
                    //        yield return Instance.GetExtension(type);
                    //    }
                    //    else
                    //    {
                    //        yield return type;
                    //    }
                    //}
                }
            }
        }

        private static ConcurrentDictionary<string, ILookup<string, Type>> exportedTypesByName = new ConcurrentDictionary<string, ILookup<string, Type>>();

        private static ConcurrentDictionary<string, ILookup<string, Type>> exportedTypesByFullName = new ConcurrentDictionary<string, ILookup<string, Type>>();

        public static Type[] GetAllExtensions(Type type, bool fallback = true)
        {
#if DEBUG
            Debug.Assert(directoryRegistered, "Customize folder is not loaded, no Extension classes are registred");
#endif
            List<Type> implementList;
            if (Instance.registeredExtensionTypes.TryGetValue(type, out implementList) == true)
            {
                return implementList.ToArray();
            }

            Type[] implementedTypes = Instance.GetAllUnspecifiedExtension(type);
            if (implementedTypes.Length > 0)
            {
                return implementedTypes;
            }

            return fallback == true ? new[] { type } : Type.EmptyTypes;
        }

        public static IEnumerable<Type> GetAllExtensions()
        {
            return Instance.registeredExtensionTypes.SelectMany(t => t.Value).Union(Instance.registeredUnspecifiedExtensionTypes);
        }

        private static void RegisterAssembly(Assembly assembly)
        {
            if (Enable == false) return;

            try
            {
                Type[] exportedTypes = assembly.GetExportedTypes();

                foreach (Type exportedType in exportedTypes)
                {
                    object[] implementationAttributes = exportedType.GetCustomAttributes(typeof(ExtensionAttribute), false);
                    for (int index = 0; index < implementationAttributes.Length; index++)
                    {
                        ExtensionAttribute implementation = (ExtensionAttribute)implementationAttributes[index];
                        Instance.Register(exportedType, implementation.TargetType);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(String.Format("Failed to load types: {0} : {1}", assembly.FullName, ex.Message));
                // Swallow
            }
        }

        private static void Register(Type type, Type targetType)
        {
            if (Enable == false) return;

            if (targetType == null)
            {
                Instance.registeredUnspecifiedExtensionTypes.Add(type);
                //Type baseType = type.BaseType;
                //while (baseType != null)
                //{
                //    Instance.Register(type, baseType);
                //    baseType = baseType.BaseType;
                //}
            }
            else
            {
                List<Type> implementList;

                if (Instance.registeredExtensionTypes.TryGetValue(targetType, out implementList) == false)
                {
                    implementList = new List<Type>();
                    Instance.registeredExtensionTypes.Add(targetType, implementList);
                }
                implementList.Add(type);
            }

            Instance.UpdateActivators(); // Make it possible to suspend when several is about to be added
        }

        public static void SuspendUpdateActivators()
        {
            Instance.updateActivatorSuspended = false;
        }

        public static void ResumeUpdateActivators()
        {
            Instance.updateActivatorSuspended = false;

            if (Instance.pendingUpdateActivators == true)
            {
                Instance.UpdateActivators();
            }
        }

        public static void UpdateActivators()
        {
            if (Instance.updateActivatorSuspended == true)
            {
                Instance.pendingUpdateActivators = true;
                return;
            }

            foreach (InitDelegate initDelegate in registeredInitDelegates)
            {
                initDelegate();
            }

            Instance.pendingUpdateActivators = false;
        }

        private static void Attach(InitDelegate initDelegate)
        {
            if (Instance.pendingUpdateActivators == true)
            {
                Instance.updateActivatorSuspended = false;
                Instance.UpdateActivators();
            }

            Instance.registeredInitDelegates.Add(initDelegate);

            initDelegate();
        }

        #endregion

        #region Create Instance

        #region Dynamic Result, Dynamic Parameters

        public static TResult Create<TResult>(Type type, params object[] parameters)
        {
            return Instance.GetActivator<Func<object[], TResult>>(Instance.GetExtension(type), Type.GetTypeArray(parameters))(parameters);
        }

        public static object Create(Type type, params object[] parameters)
        {
            return Instance.GetActivator<Func<object[], object>>(Instance.GetExtension(type), Type.GetTypeArray(parameters))(parameters);
        }

        #endregion

        #region Dynamic Result, Static Parameters

        #region Parameters 0

        // TODO Is this really the final solution??
        static object createWith0ParameterLock = new object();
        public static TResult Create<TResult>(Type type)
        {
            Func<TResult> func;
            if (Instance.ActivatorDynamicResultDictionary<TResult>.table.TryGetValue(type, out func) == false)
            {
                lock (createWith0ParameterLock)
                {
                    if (Instance.ActivatorDynamicResultDictionary<TResult>.table.TryGetValue(type, out func) == false)
                    {
                        func = Instance.GetActivator<Func<TResult>>(Instance.GetExtension(type));
                        Instance.ActivatorDynamicResultDictionary<TResult>.table.Add(type, func);
                    }
                }
            }
            return func();
        }

        private static class ActivatorDynamicResultDictionary<TResult>
        {
            public static Dictionary<Type, Func<TResult>> table;

            static ActivatorDynamicResultDictionary()
            {
                Instance.Attach(() => table = new Dictionary<Type, Func<TResult>>());
            }
        }

        #endregion

        #region Parameters 1

        // TODO Is this really the final solution??
        static object createWith1ParameterLock = new object();
        public static TResult Create<T1, TResult>(Type type, T1 arg1)
        {
            if (type == null) throw new ArgumentNullException("type");

            Func<T1, TResult> func;
            if (Instance.ActivatorDynamicResultDictionary<T1, TResult>.table.TryGetValue(type, out func) == false)
            {
                lock (createWith1ParameterLock)
                {
                    if (Instance.ActivatorDynamicResultDictionary<T1, TResult>.table.TryGetValue(type, out func) == false)
                    {
                        func = Instance.GetActivator<Func<T1, TResult>>(Instance.GetExtension(type), typeof(T1));
                        if (func == null) throw new NullReferenceException("Type not created: " + type.ToString());
                        if (Instance.ActivatorDynamicResultDictionary<T1, TResult>.table == null) throw new NullReferenceException("Instance.ActivatorDynamicResultDictionary<T1, TResult>.table is NULL!");
                        Instance.ActivatorDynamicResultDictionary<T1, TResult>.table[type] = func;
                    }
                }
            }

            return func(arg1);
        }

        private static class ActivatorDynamicResultDictionary<T1, TResult>
        {
            public static Dictionary<Type, Func<T1, TResult>> table;

            static ActivatorDynamicResultDictionary()
            {
                Instance.Attach(() => table = new Dictionary<Type, Func<T1, TResult>>());
            }
        }

        #endregion

        #region Parameters 2

        //public static object Create<T1, T2>(Type type, T1 arg1, T2 arg2)
        //{
        //    return Instance.Create<T1, T2, object>(type, arg1, arg2);
        //}

        // TODO Is this really the final solution??
        static object createWith2ParameterLock = new object();
        public static TResult Create<T1, T2, TResult>(Type type, T1 arg1, T2 arg2)
        {
            Func<T1, T2, TResult> func;
            if (Instance.ActivatorDynamicResultDictionary<T1, T2, TResult>.table.TryGetValue(type, out func) == false)
            {
                lock (createWith2ParameterLock)
                {
                    if (Instance.ActivatorDynamicResultDictionary<T1, T2, TResult>.table.TryGetValue(type, out func) == false)
                    {
                        func = Instance.GetActivator<Func<T1, T2, TResult>>(Instance.GetExtension(type), typeof(T1), typeof(T2));
                        Instance.ActivatorDynamicResultDictionary<T1, T2, TResult>.table.Add(type, func);
                    }
                }
            }
            return func(arg1, arg2);
        }

        private static class ActivatorDynamicResultDictionary<T1, T2, TResult>
        {
            public static Dictionary<Type, Func<T1, T2, TResult>> table;

            static ActivatorDynamicResultDictionary()
            {
                Instance.Attach(() => table = new Dictionary<Type, Func<T1, T2, TResult>>());
            }
        }

        #endregion

        #region Parameters 3

        // TODO Is this really the final solution??
        static object createWith3ParameterLock = new object();
        public static TResult Create<T1, T2, T3, TResult>(Type type, T1 arg1, T2 arg2, T3 arg3)
        {
            Func<T1, T2, T3, TResult> func;
            if (Instance.ActivatorDynamicResultDictionary<T1, T2, T3, TResult>.table.TryGetValue(type, out func) == false)
            {
                lock (createWith3ParameterLock)
                {
                    if (Instance.ActivatorDynamicResultDictionary<T1, T2, T3, TResult>.table.TryGetValue(type, out func) == false)
                    {
                        func = Instance.GetActivator<Func<T1, T2, T3, TResult>>(Instance.GetExtension(type), typeof(T1), typeof(T2), typeof(T3));
                        Instance.ActivatorDynamicResultDictionary<T1, T2, T3, TResult>.table.Add(type, func);
                    }
                }
            }
            return func(arg1, arg2, arg3);
        }

        private static class ActivatorDynamicResultDictionary<T1, T2, T3, TResult>
        {
            public static Dictionary<Type, Func<T1, T2, T3, TResult>> table;

            static ActivatorDynamicResultDictionary()
            {
                Instance.Attach(() => table = new Dictionary<Type, Func<T1, T2, T3, TResult>>());
            }
        }

        #endregion

        #region Parameters 4

        // TODO Is this really the final solution??
        static object createWith4ParameterLock = new object();
        public static TResult Create<T1, T2, T3, T4, TResult>(Type type, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Func<T1, T2, T3, T4, TResult> func;
            if (Instance.ActivatorDynamicResultDictionary<T1, T2, T3, T4, TResult>.table.TryGetValue(type, out func) == false)
            {
                lock (createWith4ParameterLock)
                {
                    if (Instance.ActivatorDynamicResultDictionary<T1, T2, T3, T4, TResult>.table.TryGetValue(type, out func) == false)
                    {
                        func = Instance.GetActivator<Func<T1, T2, T3, T4, TResult>>(Instance.GetExtension(type), typeof(T1), typeof(T2), typeof(T3), typeof(T4));
                        Instance.ActivatorDynamicResultDictionary<T1, T2, T3, T4, TResult>.table.Add(type, func);
                    }
                }
            }
            return func(arg1, arg2, arg3, arg4);
        }

        private static class ActivatorDynamicResultDictionary<T1, T2, T3, T4, TResult>
        {
            public static Dictionary<Type, Func<T1, T2, T3, T4, TResult>> table;

            static ActivatorDynamicResultDictionary()
            {
                Instance.Attach(() => table = new Dictionary<Type, Func<T1, T2, T3, T4, TResult>>());
            }
        }

        #endregion

        #endregion

        #region Static Result, Static Parameters

        #region Get a list of implementating classes

        public static Type[] CreateBaseTypeList<TResult>()
        {
            Type baseType = typeof(TResult);
            List<Type> result = new List<Type>();

            Assembly assembly = Assembly.GetAssembly(baseType);

            if (baseType.IsInterface)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetInterface(baseType.Name) != null)
                    {
                        result.Add(type);
                    }
                }
            }
            else
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(baseType))
                    {
                        result.Add(type);
                    }
                }
            }

            return result.ToArray<Type>();

        }

        public static TResult[] CreateBaseList<TResult>(Assembly lookupAssembly = null)
        {
            string location = String.Empty;

            try
            {
                Type baseType = typeof(TResult);

                List<TResult> result = new List<TResult>();

                Assembly assembly = lookupAssembly ?? Assembly.GetAssembly(baseType);
                location = assembly.Location;

                if (baseType.IsInterface)
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsAbstract == false && type.GetInterface(baseType.Name) != null)
                        {
                            ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
                            Debug.Assert(ci != null, "No constructor defined for: " + type.Name);
                            TResult obj = (TResult)ci.Invoke(null);
                            result.Add(obj);
                        }
                    }
                }
                else
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(baseType))
                        {
                            ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
                            TResult obj = (TResult)ci.Invoke(null);
                            result.Add(obj);
                        }
                    }
                }
                return result.ToArray<TResult>();
            }
            catch (ReflectionTypeLoadException rlex)
            {
                throw new Exception(String.Format("Not loaded {0} {1}", location, String.Join("|", rlex.LoaderExceptions.Select(i => i.Message))));
            }
        }

        public static TResult[] CreateList<TResult>(bool fallback = false)
        {
            var typeList = GetAllExtensions(typeof(TResult), fallback);

            int i = 0;
            TResult[] resultList = new TResult[typeList.Length];

            foreach (Type type in typeList)
            {
                resultList[i++] = Create<TResult>(type);
            }

            return resultList;
        }
        #endregion

        #region Parameters 0

        public static TResult Create<TResult>()
        {
            return Instance.InstanceActivator<TResult, TResult>.activator();
        }

        public static TResult CreateSubstitute<TResult, TIn>()
        {
            return Instance.InstanceActivator<TResult, TIn>.activator();
        }

        private static class InstanceActivator<TResult, TIn>
        {
            public static Func<TResult> activator;

            static InstanceActivator()
            {
                Instance.Attach(() => activator = Instance.GetActivator<Func<TResult>>(Instance.GetExtension(typeof(TIn))));
            }
        }

        #endregion

        #region Parameters 1

        public static TResult Create<T1, TResult>(T1 arg1)
        {
            return Instance.InstanceActivator<T1, TResult, TResult>.activator(arg1);
        }

        public static TResult CreateSubstitute<T1, TResult, TIn>(T1 arg1)
        {
            return Instance.InstanceActivator<T1, TResult, TIn>.activator(arg1);
        }

        private static class InstanceActivator<T1, TResult, TIn>
        {
            public static Func<T1, TResult> activator;

            static InstanceActivator()
            {
                Instance.Attach(() => activator = Instance.GetActivator<Func<T1, TResult>>(Instance.GetExtension(typeof(TIn)), typeof(T1)));
            }
        }

        #endregion

        #region Parameters 2

        public static TResult Create<T1, T2, TResult>(T1 arg1, T2 arg2)
        {
            return Instance.InstanceActivator<T1, T2, TResult, TResult>.activator(arg1, arg2);
        }

        public static TResult CreateSubstitute<T1, T2, TResult, TIn>(T1 arg1, T2 arg2)
        {
            return Instance.InstanceActivator<T1, T2, TResult, TIn>.activator(arg1, arg2);
        }

        private static class InstanceActivator<T1, T2, TResult, TIn>
        {
            public static Func<T1, T2, TResult> activator;

            static InstanceActivator()
            {
                Instance.Attach(() => { activator = Instance.GetActivator<Func<T1, T2, TResult>>(Instance.GetExtension(typeof(TIn)), typeof(T1), typeof(T2)); });
            }
        }

        #endregion

        #region Parameters 3

        public static TResult Create<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3)
        {
            return Instance.InstanceActivator<T1, T2, T3, TResult, TResult>.activator(arg1, arg2, arg3);
        }

        public static TResult CreateSubstitute<T1, T2, T3, TResult, TIn>(T1 arg1, T2 arg2, T3 arg3)
        {
            return Instance.InstanceActivator<T1, T2, T3, TResult, TIn>.activator(arg1, arg2, arg3);
        }

        private static class InstanceActivator<T1, T2, T3, TResult, TIn>
        {
            public static Func<T1, T2, T3, TResult> activator;

            static InstanceActivator()
            {
                Instance.Attach(() => activator = Instance.GetActivator<Func<T1, T2, T3, TResult>>(Instance.GetExtension(typeof(TIn)), typeof(T1), typeof(T2), typeof(T3)));
            }
        }

        #endregion

        #region Parameters 4

        public static TResult Create<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return Instance.InstanceActivator<T1, T2, T3, T4, TResult, TResult>.activator(arg1, arg2, arg3, arg4);
        }

        public static TResult CreateSubstitute<T1, T2, T3, T4, TResult, TIn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return Instance.InstanceActivator<T1, T2, T3, T4, TResult, TIn>.activator(arg1, arg2, arg3, arg4);
        }

        private static class InstanceActivator<T1, T2, T3, T4, TResult, TIn>
        {
            public static Func<T1, T2, T3, T4, TResult> activator;

            static InstanceActivator()
            {
                Instance.Attach(() => activator = Instance.GetActivator<Func<T1, T2, T3, T4, TResult>>(Instance.GetExtension(typeof(TIn)), typeof(T1), typeof(T2), typeof(T3), typeof(T4)));
            }
        }

        #endregion

        #region Parameters 5

        public static TResult Create<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return Instance.InstanceActivator<T1, T2, T3, T4, T5, TResult, TResult>.activator(arg1, arg2, arg3, arg4, arg5);
        }

        public static TResult CreateSubstitute<T1, T2, T3, T4, T5, TResult, TIn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return Instance.InstanceActivator<T1, T2, T3, T4, T5, TResult, TIn>.activator(arg1, arg2, arg3, arg4, arg5);
        }

        private static class InstanceActivator<T1, T2, T3, T4, T5, TResult, TIn>
        {
            public static Func<T1, T2, T3, T4, T5, TResult> activator;

            static InstanceActivator()
            {
                Instance.Attach(() => { activator = Instance.GetActivator<Func<T1, T2, T3, T4, T5, TResult>>(Instance.GetExtension(typeof(TIn)), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)); });
            }
        }

        #endregion

        #region Parameters 6

        public static TResult Create<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return Instance.InstanceActivator<T1, T2, T3, T4, T5, T6, TResult, TResult>.activator(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static TResult CreateSubstitute<T1, T2, T3, T4, T5, T6, TResult, TIn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return Instance.InstanceActivator<T1, T2, T3, T4, T5, T6, TResult, TIn>.activator(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        private static class InstanceActivator<T1, T2, T3, T4, T5, T6, TResult, TIn>
        {
            public static Func<T1, T2, T3, T4, T5, T6, TResult> activator;

            static InstanceActivator()
            {
                Instance.Attach(() => activator = Instance.GetActivator<Func<T1, T2, T3, T4, T5, T6, TResult>>(Instance.GetExtension(typeof(TIn)), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)));
            }
        }

        #endregion

        #region Parameters 7

        public static TResult Create<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return Instance.InstanceActivator<T1, T2, T3, T4, T5, T6, T7, TResult, TResult>.activator(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static TResult CreateSubstitute<T1, T2, T3, T4, T5, T6, T7, TResult, TIn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return Instance.InstanceActivator<T1, T2, T3, T4, T5, T6, T7, TResult, TIn>.activator(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        private static class InstanceActivator<T1, T2, T3, T4, T5, T6, T7, TResult, TIn>
        {
            public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> activator;

            static InstanceActivator()
            {
                Instance.Attach(() => activator = Instance.GetActivator<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>(Instance.GetExtension(typeof(TIn)), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)));
            }
        }

        #endregion

        #region Parameters 8

        public static TResult Create<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return Instance.InstanceActivator<T1, T2, T3, T4, T5, T6, T7, T8, TResult, TResult>.activator(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static TResult CreateSubstitute<T1, T2, T3, T4, T5, T6, T7, T8, TResult, TIn>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return Instance.InstanceActivator<T1, T2, T3, T4, T5, T6, T7, T8, TResult, TIn>.activator(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        private static class InstanceActivator<T1, T2, T3, T4, T5, T6, T7, T8, TResult, TIn>
        {
            public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> activator;

            static InstanceActivator()
            {
                Instance.Attach(() => activator = Instance.GetActivator<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>(Instance.GetExtension(typeof(TIn)), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)));
            }
        }

        #endregion

        /// <summary>
        /// Gets the activator for a specific type. Note, extension is not automatically applied.
        /// </summary>
        /// <typeparam name="T">The activator function definition</typeparam>
        /// <param name="instanceType">The specific type of the instance that the activator creates</param>
        /// <param name="parametersType">The types of the parameters to the activator</param>
        /// <returns>An activator function that creates instances of the specified instance</returns>
        public static T GetActivator<T>(Type instanceType, params Type[] parametersType)
        {
            Debug.Assert(instanceType != null, "Expected a type when computing the constructor");
            Debug.Assert(parametersType != null, "Constructor parameter type vector can not be null");

            var parameterExpression = new ParameterExpression[parametersType.Length];
            for (int index = 0; index < parametersType.Length; index++)
            {
                parameterExpression[index] = System.Linq.Expressions.Expression.Parameter(parametersType[index]);
            }

            var constructor = instanceType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parametersType, null);
            var expression = System.Linq.Expressions.Expression.New(constructor, parameterExpression);
            return System.Linq.Expressions.Expression.Lambda<T>(expression, parameterExpression).Compile();
        }

        #endregion

        #endregion

        #region Serializers

        static Encoding utf8 = new UTF8Encoding(false);
        static XmlWriterSettings defaultXmlWriterSettings = new XmlWriterSettings { Encoding = utf8, Indent = true };

        public static string Serialize<T>(T instance, XmlWriterSettings xmlWriterSettings = null, bool? useDataContractSerializer = null, DataContractSerializerSettings dataContractSerializerSettings = null)
        {
            Type type = Instance.GetExtension(typeof(T));

            bool dataContractSerialize = useDataContractSerializer.HasValue == true ? useDataContractSerializer.Value : SupportDataContractSerializer(type);

            if (dataContractSerialize == true)
            {
                var serializer = Instance.CreateDataContractSerializer(type, dataContractSerializerSettings);
                var stringBuilder = new StringBuilder();
                using (var writer = XmlWriter.Create(stringBuilder, xmlWriterSettings))
                {
                    serializer.WriteObject(writer, instance);
                }
                string result = stringBuilder.ToString();
                return result;
            }
            else
            {
                if (UseMemoryStreamSerializer == false)
                {
                    using (var stringWriter = new UTF8StringWriter())
                    {
                        using (var writer = XmlWriter.Create(stringWriter, xmlWriterSettings ?? defaultXmlWriterSettings))
                        {
                            var serializer = Instance.CreateXmlSerializer(type);
                            serializer.Serialize(writer, instance);
                        }

                        return stringWriter.ToString();
                    }
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var writer = XmlWriter.Create(memoryStream, xmlWriterSettings ?? defaultXmlWriterSettings))
                        {
                            var serializer = Instance.CreateXmlSerializer(type);
                            serializer.Serialize(writer, instance);
                        }
                        return utf8.GetString(memoryStream.ToArray());
                    }
                }
            }
        }

        public static void Serialize<T>(Stream stream, T instance, XmlWriterSettings xmlWriterSettings = null, bool? useDataContractSerializer = null, DataContractSerializerSettings dataContractSerializerSettings = null)
        {
            Type type = Instance.GetExtension(typeof(T));

            bool dataContractSerialize = useDataContractSerializer.HasValue == true ? useDataContractSerializer.Value : SupportDataContractSerializer(type);

            if (dataContractSerialize == true)
            {
                var serializer = Instance.CreateDataContractSerializer(type, dataContractSerializerSettings);
                if (xmlWriterSettings == null)
                {
                    serializer.WriteObject(stream, instance);
                }
                else
                {
                    using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.WriteObject(xmlWriter, instance);
                    }
                }
            }
            else
            {
                var serializer = Instance.CreateXmlSerializer<T>();
                if (xmlWriterSettings == null)
                {
                    serializer.Serialize(stream, instance);
                }
                else
                {
                    using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, instance);
                    }
                }
            }
        }

        //[Obsolete] // TODO how to handle this?
        public static string Serialize<T>(T instance, Type[] extraTypes)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(stringBuilder))
            {
                XmlSerializer serializer = Instance.CreateXmlSerializer<T>(extraTypes);
                serializer.Serialize(writer, instance);
            }
            string result = stringBuilder.ToString();
            return result;

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    using (XmlWriter writer = XmlWriter.Create(ms))
            //    {
            //        XmlSerializer serializer = Instance.CreateXmlSerializer<T>(extraTypes);
            //        serializer.Serialize(writer, instance);
            //    }
            //    return Encoding.UTF8.GetString(ms.ToArray());
            //}
        }

        public static void Serialize<T>(Stream stream, T instance, CryptographicMode mode, Encoding encoding = null, byte[] dataEncryptionKey = null, byte[] dataEncryptionIV = null, byte[] signatureKey = null)
        {
            XmlWriterSettings xmlWriterSettings = null;
            if (encoding != null)
            {
                xmlWriterSettings = new XmlWriterSettings()
                {
                    Encoding = encoding
                };
            }

            switch (mode)
            {
                case CryptographicMode.EncryptedAndSigned:
                    using (var memoryStream = new MemoryStream())
                    {
                        Instance.Serialize<T>(memoryStream, instance, xmlWriterSettings);
                    }
                    break;
                case CryptographicMode.Encrypted:

                    break;
                case CryptographicMode.Signed:
                    using (var memoryStream = new MemoryStream())
                    {
                        Instance.Serialize<T>(memoryStream, instance, xmlWriterSettings);
                    }
                    break;
                default:
                    Instance.Serialize<T>(stream, instance, xmlWriterSettings);
                    break;
            }
        }

        #region Data contract serializer

        public static bool SupportDataContractSerializer(Type type)
        {
            return type.GetCustomAttributes(true)
                                             .Where(x =>
                                                 x is System.Runtime.Serialization.DataContractAttribute/* |
                                                 x is System.SerializableAttribute |
                                                 x is System.ServiceModel.MessageContractAttribute*/)
                                             .Any();
        }

        public static XmlObjectSerializer CreateDataContractSerializer(Type type, DataContractSerializerSettings settings = null)
        {
            if (settings == null)
            {
                settings = new DataContractSerializerSettings()
                {
                    PreserveObjectReferences = true,
                };
            }

            return new DataContractSerializer(Instance.GetExtension(type), settings);
        }

        #endregion

        #region XML serializer

        private static ConcurrentDictionary<string, XmlSerializer> xmlSerializerMap = new ConcurrentDictionary<string, XmlSerializer>();
        private static Dictionary<string, int> cacheLookupMap = new Dictionary<string, int>();
        private static object LockObject = new object();

        static void Log(string key)
        {
#if DEBUG
            lock (LockObject)
            {
                if (cacheLookupMap.ContainsKey(key) == false) cacheLookupMap.Add(key, 0);
                cacheLookupMap[key]++;
                int hit = cacheLookupMap.Values.Sum();
                if (hit % 1000 == 0)
                {
                    Trace.WriteLine("XmlSerializer create hit count: " + hit.ToString());
                    cacheLookupMap.Keys.ForEach(i =>
                    {
                        Trace.WriteLine("XmlSerializer create hit: " + i + " : " + cacheLookupMap[i].ToString());
                    });
                }
            }
#endif
        }

        public static XmlSerializer CreateXmlSerializer(Type t, string rootElementName)
        {
            string key = rootElementName == null ? t.Name : string.Format("{0}N:{1}", t.Name, rootElementName);

            XmlSerializer serializer;
            if (xmlSerializerMap.TryGetValue(key, out serializer) == false)
            {
                lock (LockObject)
                {
                    if (xmlSerializerMap.TryGetValue(key, out serializer) == false)
                    {
                        serializer = rootElementName == null ? new XmlSerializer(t) : new XmlSerializer(t, new XmlRootAttribute(rootElementName));
                        xmlSerializerMap.TryAdd(key, serializer);
                    }
                }
            }

            Log(key);

            return serializer;
        }

        public static XmlSerializer CreateXmlSerializer(Type t, XmlRootAttribute rootAttribute = null)
        {
            string key = rootAttribute == null ? t.Name : string.Format("{0}N:{1}.{2}D:{3}", t.Name, rootAttribute.Namespace, rootAttribute.ElementName, rootAttribute.DataType);

            XmlSerializer serializer;
            if (xmlSerializerMap.TryGetValue(key, out serializer) == false)
            {
                lock (LockObject)
                {
                    if (xmlSerializerMap.TryGetValue(key, out serializer) == false)
                    {
                        serializer = rootAttribute == null ? new XmlSerializer(t) : new XmlSerializer(t, rootAttribute);
                        xmlSerializerMap.TryAdd(key, serializer);
                    }
                }
            }

            Log(key);

            return serializer;
        }

        public static XmlSerializer CreateXmlSerializer(Type t, Type[] extraTypes)
        {
            string extraTypesNames = extraTypes != null && extraTypes.Length > 0 ? String.Join(".", extraTypes.Select(i => i.Name)) : String.Empty;
            string key = string.Format("{0}:{1}", t.Name, extraTypesNames);
            
            if (xmlSerializerMap.ContainsKey(key) == false)
            {
                lock (LockObject)
                {
                    if (xmlSerializerMap.ContainsKey(key) == false)
                    {
                        xmlSerializerMap.TryAdd(key, new XmlSerializer(t, extraTypes));
                    }
                }
            }

            Log(key);
            return xmlSerializerMap[key];
        }

        public static XmlSerializer CreateXmlSerializer(Type t, XmlAttributeOverrides overrides)
        {
            string key = string.Format("{0}:{1:X}", t.Name, overrides.GetHashCode());

            if (xmlSerializerMap.ContainsKey(key) == false)
            {
                lock (LockObject)
                {
                    if (xmlSerializerMap.ContainsKey(key) == false)
                    {
                        xmlSerializerMap.TryAdd(key, new XmlSerializer(t, overrides));
                    }
                }
            }

            Log(key);
            return xmlSerializerMap[key];
        }

        public static XmlSerializer CreateXmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
        {
            string extraTypesNames = extraTypes != null && extraTypes.Length > 0 ? String.Join(".", extraTypes.Select(i => i.Name)) : String.Empty;
            string key = string.Format("{0}:{1:X}:{2}:{3:X}:{4}", type.Name, overrides.GetHashCode(), extraTypesNames, root.GetHashCode(), defaultNamespace);

            if (xmlSerializerMap.ContainsKey(key) == false)
            {
                lock (LockObject)
                {
                    if (xmlSerializerMap.ContainsKey(key) == false)
                    {
                        xmlSerializerMap.TryAdd(key, new XmlSerializer(type, overrides, extraTypes, root, defaultNamespace));
                    }
                }
            }

            Log(key);
            return xmlSerializerMap[key];
        }

        public static XmlSerializer CreateXmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location)
        {
            string extraTypesNames = extraTypes != null && extraTypes.Length > 0 ? String.Join(".", extraTypes.Select(i => i.Name)) : String.Empty;
            string key = string.Format("{0}:{1:X}:{2}:{3:X}:{4}:{5}", type.Name, overrides.GetHashCode(), extraTypesNames , root.GetHashCode(), defaultNamespace, location);

            if (xmlSerializerMap.ContainsKey(key) == false)
            {
                lock (LockObject)
                {
                    if (xmlSerializerMap.ContainsKey(key) == false)
                    {
                        xmlSerializerMap.TryAdd(key, new XmlSerializer(type, overrides, extraTypes, root, defaultNamespace, location));
                    }
                }
            }

            Log(key);
            return xmlSerializerMap[key];
        }

        public static XmlSerializer CreateXmlSerializer<T>(string rootElementName = null)
        {
            return CreateXmlSerializer(typeof(T), rootElementName);
        }

        public static XmlSerializer CreateXmlSerializer<T>(Type[] extraTypes)
        {
            Type t = typeof(T);
            string extraTypesNames = extraTypes != null && extraTypes.Length > 0 ? String.Join(".", extraTypes.Select(i => i.Name)) : String.Empty;
            string key = string.Format("{0}:{1}", t.Name, extraTypesNames);

            if (xmlSerializerMap.ContainsKey(key) == false)
            {
                lock (LockObject)
                {
                    if (xmlSerializerMap.ContainsKey(key) == false)
                    {
                        xmlSerializerMap.TryAdd(key, new XmlSerializer(t, extraTypes));
                    }
                }
            }

            Log(key);
            return xmlSerializerMap[key];
        }

        #endregion

        public static object Deserialize(Type type, XmlReader xmlReader, bool? useDataContractSerializer = null, DataContractSerializerSettings dataContractSerializerSettings = null)
        {
            bool dataContractSerialize = useDataContractSerializer.HasValue == true ? useDataContractSerializer.Value : SupportDataContractSerializer(type);

            if (dataContractSerialize == true)
            {
                var serializer = Instance.CreateDataContractSerializer(type, dataContractSerializerSettings);
                var obj = serializer.ReadObject(xmlReader);
                return obj;
            }
            else
            {
                var serializer = Instance.CreateXmlSerializer(type);
                var obj = serializer.Deserialize(xmlReader);
                return obj;
            }
        }

        public static object Deserialize(Type type, TextReader textReader, XmlReaderSettings xmlReaderSettings = null, bool? useDataContractSerializer = null, DataContractSerializerSettings dataContractSerializerSettings = null)
        {
            bool dataContractSerialize = useDataContractSerializer.HasValue == true ? useDataContractSerializer.Value : SupportDataContractSerializer(type);
            if (dataContractSerialize == true)
            {
                using (var xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
                {
                    return Instance.Deserialize(type, xmlReader, true, dataContractSerializerSettings);
                }
            }
            else
            {
                if (xmlReaderSettings == null)
                {
                    var serializer = Instance.CreateXmlSerializer(type);
                    var obj = serializer.Deserialize(textReader);
                    return obj;
                }
                else
                {
                    using (var xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
                    {
                        return Instance.Deserialize(type, xmlReader, false);
                    }
                }
            }
        }

        public static object Deserialize(Type type, string xml, XmlReaderSettings xmlReaderSettings = null, bool? useDataContractSerializer = null, DataContractSerializerSettings dataContractSerializerSettings = null)
        {
            using (var stringReader = new StringReader(xml))
            {
                return Instance.Deserialize(type, stringReader, xmlReaderSettings, useDataContractSerializer, dataContractSerializerSettings);
            }
        }

        public static object Deserialize(Type type, Stream stream, XmlReaderSettings xmlReaderSettings = null, bool? useDataContractSerializer = null, DataContractSerializerSettings dataContractSerializerSettings = null)
        {
            bool dataContractSerialize = useDataContractSerializer.HasValue == true ? useDataContractSerializer.Value : SupportDataContractSerializer(type);

            if (dataContractSerialize == true)
            {
                if (xmlReaderSettings == null)
                {
                    var serializer = Instance.CreateDataContractSerializer(type, dataContractSerializerSettings);
                    var obj = serializer.ReadObject(stream);
                    return obj;
                }
                else
                {
                    using (var xmlReader = XmlReader.Create(stream, xmlReaderSettings))
                    {
                        return Instance.Deserialize(type, xmlReader, true, dataContractSerializerSettings);
                    }
                }
            }
            else
            {
                if (xmlReaderSettings == null)
                {
                    var serializer = Instance.CreateXmlSerializer(type);
                    var obj = serializer.Deserialize(stream);
                    return obj;
                }
                else
                {
                    using (var xmlReader = XmlReader.Create(stream, xmlReaderSettings))
                    {
                        return Instance.Deserialize(type, xmlReader, false);
                    }
                }
            }
        }

        public static T Deserialize<T>(XmlReader xmlReader, bool? useDataContractSerializer = null, DataContractSerializerSettings dataContractSerializerSettings = null)
        {
            return (T)Instance.Deserialize(Instance.GetExtension(typeof(T)), xmlReader, useDataContractSerializer, dataContractSerializerSettings);
        }

        public static T Deserialize<T>(TextReader textReader, XmlReaderSettings xmlReaderSettings = null, bool? useDataContractSerializer = null, DataContractSerializerSettings dataContractSerializerSettings = null)
        {
            return (T)Instance.Deserialize(Instance.GetExtension(typeof(T)), textReader, xmlReaderSettings, useDataContractSerializer, dataContractSerializerSettings);
        }

        public static T Deserialize<T>(string xml, XmlReaderSettings xmlReaderSettings = null, bool? useDataContractSerializer = null, DataContractSerializerSettings dataContractSerializerSettings = null)
        {
            return (T)Instance.Deserialize(Instance.GetExtension(typeof(T)), xml, xmlReaderSettings, useDataContractSerializer, dataContractSerializerSettings);
        }

        public static T Deserialize<T>(Stream stream, XmlReaderSettings xmlReaderSettings = null, bool? useDataContractSerializer = null, DataContractSerializerSettings dataContractSerializerSettings = null)
        {
            return (T)Instance.Deserialize(Instance.GetExtension(typeof(T)), stream, xmlReaderSettings, useDataContractSerializer, dataContractSerializerSettings);
        }

        public static T Deserialize<T>(Stream stream, CryptographicMode mode, Encoding encoding = null, byte[] dataEncryptionKey = null, byte[] dataEncryptionIV = null, byte[] signatureVerifyKey = null)
        {
            Stream contentStream = null;

            switch (mode)
            {
                case CryptographicMode.EncryptedAndSigned:

                    return Instance.Deserialize<T>(contentStream);
                case CryptographicMode.Encrypted:

                case CryptographicMode.Signed:

                    return Instance.Deserialize<T>(contentStream);
                default:
                    return Instance.Deserialize<T>(stream);
            }
        }

        #endregion
    }

    #region Interface Implementation

    public static class InterfaceImplementor
    {
        private static class ImplementedInterface<TInterface, TImplementator>
            where TImplementator : MethodInvokerBase
        {
            public static Type type = null; //InterfaceImplementor.ImplementInternal<TInterface, TImplementator>();

            public static Func<TInterface> activator = null; //Instance.GetActivator<Func<TInterface>>(type);

            static ImplementedInterface()
            {
                Type templateType = typeof(ImplementedInterface<,>);
                Type genericType = templateType.MakeGenericType(new Type[] { typeof(TInterface), typeof(TImplementator) });

                type = genericType; // InterfaceImplementor.ImplementInternal<TInterface, TImplementator>();
                activator = Instance.GetActivator<Func<TInterface>>(typeof(TImplementator));
            }
        }

        public static TInterface CreateInstance<TInterface, TImplementator>()
            where TImplementator : MethodInvokerBase
        {
            return ImplementedInterface<TInterface, TImplementator>.activator();
        }

        public static Type Implement<TInterface, TImplementator>()
            where TImplementator : MethodInvokerBase
        {
            return ImplementedInterface<TInterface, TImplementator>.type;
        }

        private static Dictionary<string, Tuple<Type, Delegate>> cachedRunTimeImplementations = new Dictionary<string, Tuple<Type, Delegate>>();

        public static Type Implement(Type interfaceType, Type implementorType)
        {
            BindingFlags flags = BindingFlags.GetField | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.Public;

            string key = string.Concat(interfaceType.Name, ".", implementorType.Name);

            Tuple<Type, Delegate> implementation;
            if (cachedRunTimeImplementations.TryGetValue(key, out implementation) == false)
            {
                Type templateType = typeof(ImplementedInterface<,>);
                Type genericType = templateType.MakeGenericType(new Type[] { interfaceType, implementorType });

                Type implementType = (Type)genericType.InvokeMember("type", flags, null, null, null);

                Delegate implementDelegate = (Delegate)genericType.InvokeMember("activator", flags, null, null, null);

                implementation = new Tuple<Type, Delegate>(implementType, implementDelegate);
                cachedRunTimeImplementations.Add(key, implementation);
            }

            return implementation.Item1;
        }

        private static Type ImplementInternal<TInterface, TImplementator>()
            where TImplementator : MethodInvokerBase
        {
            return Implement(typeof(TInterface), typeof(TImplementator));
        }

        [Obsolete]
        private static Type ImplementInternal(Type interfaceType, Type implementorType)
        {
            string name = "InterfaceImplementation";
           var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.Run); 

#if NET5
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.Run);
#endif
            ModuleBuilder module = assembly.DefineDynamicModule(name);

            string typeName = string.Concat(implementorType.Name, interfaceType.Name);

            // create the type that is used to wrap the object into the interface.
            var typeBuilder = module.DefineType(typeName, TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed);

            //var invokeMethod = baseType.GetMethod("Invoke", BindingFlags.NonPublic | BindingFlags.Instance);

            typeBuilder.SetParent(implementorType);

            // Add the interface implementation to the type.
            typeBuilder.AddInterfaceImplementation(interfaceType);

            foreach (var methodInfo in interfaceType.GetMethods(BindingFlags.Default))
            {
                var parameters = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

                // Check to see if there already is a method there for the interface
                if (implementorType.GetMethod(methodInfo.Name, parameters) != null)
                {
                    continue;
                }

                var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, parameters);
                var generator = methodBuilder.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0);

                generator.Emit(OpCodes.Ldstr, methodInfo.Name);

                var invokeMethod = implementorType.GetMethod("Invoke" + parameters.Length.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);
                // push the parameters for the method call onto the stack.
                if (parameters.Length != 0)
                //    generator.Emit(OpCodes.Ldnull);
                //else
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        generator.Emit(OpCodes.Ldarg, i + 1);
                        if (parameters[i].IsClass == false)
                            generator.Emit(OpCodes.Box, parameters[i]);
                    }
                }

                generator.Emit(OpCodes.Call, invokeMethod);
                //generator.EmitCall(OpCodes.Callvirt, invokeMethod, new Type [] { typeof(string) }.Union(parameters).ToArray());

                if (methodInfo.ReturnType.IsClass)
                    generator.Emit(OpCodes.Castclass, methodInfo.ReturnType);
                else
                    generator.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
                generator.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
            }

            return typeBuilder.CreateType();
        }
    }

    public abstract class MethodInvokerBase
    {
        protected internal object Invoke0(string member) { return Invoke(member, new object[] { }); }

        protected internal object Invoke1(string member, object arg1) { return Invoke(member, new object[] { arg1 }); }

        protected internal object Invoke2(string member, object arg1, object arg2) { return Invoke(member, new object[] { arg1, arg2 }); }

        protected internal object Invoke3(string member, object arg1, object arg2, object arg3) { return Invoke(member, new object[] { arg1, arg2, arg3 }); }

        protected internal object Invoke4(string member, object arg1, object arg2, object arg3, object arg4) { return Invoke(member, new object[] { arg1, arg2, arg3, arg4 }); }

        protected internal object Invoke5(string member, object arg1, object arg2, object arg3, object arg4, object arg5) { return Invoke(member, new object[] { arg1, arg2, arg3, arg4, arg5 }); }

        protected internal object Invoke6(string member, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) { return Invoke(member, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 }); }

        protected internal object Invoke7(string member, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7) { return Invoke(member, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 }); }

        protected internal object Invoke8(string member, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8) { return Invoke(member, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }); }

        public abstract object Invoke(string member, params object[] args);
        public abstract object InvokeAsynch(string member, params object[] args);
    }

    #endregion

    public class InstanceBase<T>
        where T : class
    {
        public static T Create()
        {
            return Instance.Create<T>();
        }

        public static T LoadXml(string xml)
        {
            return Instance.Deserialize<T>(xml);
        }

        virtual public string ToXml()
        {
            return Instance.Serialize<T>(this as T);
        }
    }

    public class Implementable<T>
    {
        protected Implementable()
        {
        }

        public static T Create()
        {
            return Instance.Create<T>();
        }

        public static T Create(params object[] parameters)
        {
            return Instance.Create<T>(typeof(T), parameters);
        }
    }

    public class Serializable<T> : Implementable<T>
    {
        public static T ReadXml(string xml)
        {
            return (T)Instance.Deserialize<T>(xml);
        }

        public static string WriteXml(T instance)
        {
            return Instance.Serialize<T>(instance);
        }
    }

    public interface IFileSerializable
    {
        string FilePath { get; set; }

        void Save();
    }

    public class UTF8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }

    public class EncodedStringWriter : StringWriter
    {
        public EncodedStringWriter(Encoding encoding) 
        {
            this.encoding = encoding;
        }

        public EncodedStringWriter(Encoding encoding, StringBuilder stringBuilder)
            : base(stringBuilder)
        {
            this.encoding = encoding;
        }

        public EncodedStringWriter(Encoding encoding, IFormatProvider formatProvider)
            : base(formatProvider)
        {
            this.encoding = encoding;
        }

        public EncodedStringWriter(Encoding encoding, StringBuilder stringBuilder, IFormatProvider formatProvider)
            : base(stringBuilder, formatProvider)
        {
            this.encoding = encoding;
        }

        private Encoding encoding;
        public override Encoding Encoding { get { return encoding; } }
    }

    public class FileSerializable<T> : Serializable<T>, IFileSerializable where T : IFileSerializable
    {
        #region IFileSerializable Members

        public string FilePath { get; set; }

        public void Save()
        {
            FileSerializable<T>.Save((T)(object)this, this.FilePath);
        }

        #endregion

        public static T Load(string path)
        {
            T instance = default(T);

            if (File.Exists(path) == false)
            {
                // Create a default instance
                instance = Implementable<T>.Create();
                instance.FilePath = path;
                return instance;
            }

            try
            {
                instance = Serializable<T>.ReadXml(File.ReadAllText(path));
                instance.FilePath = path;
                return instance;
            }
            catch (Exception exception)
            {
                throw new Exception("Could not load file ", exception);
            }
        }

        public static void Save(T instance, string path)
        {
            if (string.IsNullOrEmpty(path) == false)
            {
                try
                {
                    if (Directory.Exists(Path.GetDirectoryName(path)) == false)
                    {
                        Directory.CreateDirectory(path);
                    }

                    File.WriteAllText(path, Serializable<T>.WriteXml(instance));
                }
                catch (Exception exception)
                {
                    throw new Exception("Could not save file", exception);
                }
            }
        }
    }
}
