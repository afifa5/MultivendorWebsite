#if NET5
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Caching.Memory;
using System.Runtime.Caching;
#endif
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
#if NET452
using System.Web.Caching;
using System.Web.Mvc;
#endif
using System.Xml.Serialization;

namespace MultivendorWebViewer.Common
{

    public enum CacheLocation { None, Application, Session }

    public   class SessionCacheProvider:SingletonBase<SessionCacheProvider>
    {
        public void Clear()
        {
            //var sessionData = ApplicationRequestContext.Current.SessionData;
            //if (sessionData != null)
            //{
            //    sessionData.Cache = null;
            //}
        }

        public  virtual  System.Runtime.Caching.ObjectCache Cache
        {
            get
            {
                return null;
                //var sessionData = ApplicationRequestContext.Current.SessionData;
                //return sessionData != null ? sessionData.Cache : null;
            }
        }
    }

    public class SiteDataCacheManager 
    {
        [XmlAttribute("cache-folder")]
        public string CacheFolderPath { get; set; }

        public string ThumbnailCacheFolderPath { get; set; }

        private string cacheKey;
        public string CacheKey { get { return cacheKey ?? (cacheKey = GetCacheKey() ?? string.Empty); } }

        protected virtual string GetCacheKey()
        {
            return "";
        }

        protected virtual string GetCachedFolderPath()
        {
            if (string.IsNullOrEmpty(CacheKey) == true) return null;

            string path = CacheFolderPath;
            if (path == null)
            {
                path = Path.Combine("c:/Temp", "Cache");
            }

            path = Path.Combine(path, CacheKey);

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        protected virtual string GetThumbnailCacheFolderPath()
        {
            if (ThumbnailCacheFolderPath != null)
            {
                string path = Path.Combine(ThumbnailCacheFolderPath, CacheKey);

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }

            var thumbPath = Path.Combine(GetCachedFolderPath(), "Thumbnails");
            if (Directory.Exists(thumbPath) == false)
            {
                Directory.CreateDirectory(thumbPath);
            }
            return thumbPath;
        }

        protected class CacheFileEntry
        {
            public string Key { get; set; }

            public string FileName { get; set; }

            public string ContentType { get; set; }
        }

        private object cachedThumbnailEntriesLock = new object();
        private ConcurrentDictionary<string, CacheFileEntry> cachedThumbnailEntries;
        protected CacheFileEntry GetThumbnailFileEntry(string fileKey)
        {
            if (cachedThumbnailEntries == null)
            {
                LoadGetThumbnailFileEntries();
            }

            return cachedThumbnailEntries.TryGetValue(fileKey);
        }

        protected void LoadGetThumbnailFileEntries()
        {
            lock (cachedThumbnailEntriesLock)
            {
                if (cachedThumbnailEntries == null)
                {
                    lock (cachedThumbnailEntriesLock)
                    {
                        cachedThumbnailEntries = new ConcurrentDictionary<string, CacheFileEntry>(StringComparer.OrdinalIgnoreCase);
                        string cacheFolder = GetThumbnailCacheFolderPath();
                        if (cacheFolder == null)
                        {
                            var directory = new DirectoryInfo(cacheFolder);
                            if (directory.Exists == true)
                            {
                                var files = directory.EnumerateFiles();
                                foreach (var file in files)
                                {
                                    var fileName = Path.GetFileNameWithoutExtension(file.Name);
                                    fileName.LastIndexOf('_');

                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual Task<FileResult> GetCachedThumbnailFileAsync(string fileName, int width, int height, int sampling, string key = null, Func<Tuple<byte[], string>> contentProvider = null)
        {
            return Task<FileResult>.Run(() =>
            {
                string cacheFolder = GetThumbnailCacheFolderPath();

                if (cacheFolder == null) return null;

                string fileKey = GetCachedThumbnailKey(fileName, width, height, sampling, key);

                var fileEntry = GetThumbnailFileEntry(fileKey);

                if (fileEntry != null)
                {
                    string filePath = Path.Combine(cacheFolder, fileEntry.FileName);
                    if (File.Exists(filePath) == true)
                    {
#if NET5
                        return (FileResult)new PhysicalFileResult(filePath, fileEntry.ContentType);
#else
                        return (FileResult)new FilePathResult(filePath, fileEntry.ContentType);
#endif
                        //return Task.FromResult<FileResult>(new FilePathResult(fileEntry.FileName, fileEntry.ContentType));
                    }
                }
                else
                {
                    // TODO
                    if (contentProvider != null)
                    {
                        var content = contentProvider();
                        CacheThumbnailFile(fileName, content.Item1, content.Item2, width, height, sampling, key);
                    }
                }

                return null;
            });
        }

        public virtual bool CacheThumbnailFile(string fileName, byte[] content, string contentType, int width, int height, int sampling, string key = null)
        {
            string cacheFolder = GetThumbnailCacheFolderPath();

            if (cacheFolder == null) return false;

            if (cachedThumbnailEntries == null)
            {
                LoadGetThumbnailFileEntries();
            }

            string fileKey = GetCachedThumbnailKey(fileName, width, height, sampling, key);

            var fileEntry = new CacheFileEntry { Key = fileKey, FileName = string.Concat(fileKey, ".", GetFileExtensionFromContentType(contentType)), ContentType = contentType };

            cachedThumbnailEntries.TryAdd(fileKey, fileEntry);

            string filePath = Path.Combine(cacheFolder, fileEntry.FileName);

            File.WriteAllBytes(filePath, content);

            return true;
        }

        private Dictionary<string, string> contentTypeToFileExtensionMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "image/svg+xml", "svg" },
            { "image/png", "png" },
            { "image/jpeg", "jpg" },
            { "image/gif", "gif" },
            { "image/bmp", "bmp" },
            { "image/tiff", "tiff" }
        };
        protected string GetFileExtensionFromContentType(string contentType, string originalFileName = null)
        {
            var extension = contentTypeToFileExtensionMapping.TryGetValue(contentType);
            if (extension != null)
            {
                if (originalFileName != null && extension.Equals(originalFileName, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return originalFileName.EndsWith("svgz", StringComparison.OrdinalIgnoreCase) ? "svgz" : "svg";
                }
                return extension;
            }

            return originalFileName != null ? Path.GetExtension(originalFileName) : "png";
        }

        protected virtual string GetCachedThumbnailKey(string originalFilename, int width, int height, int sampling, string key = null)
        {
            string fileName = string.Concat(Path.GetFileNameWithoutExtension(originalFilename), "_", Path.GetExtension(originalFilename).TrimStart('.'));
            string filePath = string.Format("{0}_{1}_{2}_{3}", fileName, width, height, sampling);
            return key == null ? filePath : string.Concat(filePath, "_", key);
        }
    }


#if NET5
    public static class Cache
    {
        public static DateTime NoAbsoluteExpiration = DateTime.MaxValue;
        public static TimeSpan NoSlidingExpiration = TimeSpan.Zero;
    }
#endif

    public  class CacheManager : SingletonBase<CacheManager>
    {
        private ConcurrentDictionary<string, object> applicationCacheLocks = new ConcurrentDictionary<string, object>();

        private object applicationCacheLocksLock = new object();

        private Dictionary<string, object> sessionCacheLocks = new Dictionary<string, object>();

        private object sessionCacheLocksLock = new object();

        //private ConcurrentDictionary<string, CustomViewCacheDependency> cacheDependencies = new ConcurrentDictionary<string, CustomViewCacheDependency>();

        public virtual int ApplicationCacheCount { get { return applicationCacheLocks.Count; } }

        public virtual int SesssionCacheCount { get { return sessionCacheLocks.Count; } }

        public virtual IEnumerable<string> ApplicationCacheKeys { get { return applicationCacheLocks.Keys; } }

        public virtual IEnumerable<string> SessionCacheKeys { get { return sessionCacheLocks.Keys; } }

        public virtual bool ContainsKey(string key, CacheLocation location)
        {
            if (location == CacheLocation.Session)
            {
                var sessionCache =SessionCacheProvider.Default.Cache;
                if (sessionCache != null)
                {
                    if (sessionCache.Contains(key) == true) return true;
                }
            }
            else if (location == CacheLocation.Application)
            {
#if NET5
                return MemoryCache.Default[key] != null;
#else
                var applicationCache = HttpRuntime.Cache;
                if (applicationCache != null)
                {
                    if (applicationCache[key] != null) return true;
                }
#endif
            }

            return false;
        }

        #region Public Add Methods

        public void Add(string key, object value, CacheLocation location = CacheLocation.Application, CacheItemPriority priority = CacheItemPriority.Default/*, CacheDependency dependencies = null, CacheItemRemovedCallback onRemoveCallback = null*/)
        {
            Add(key, value, location, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, priority/*, dependencies, onRemoveCallback*/);
        }

        public void Add(string key, object value, DateTime absoluteExpiration, CacheLocation location = CacheLocation.Application, CacheItemPriority priority = CacheItemPriority.Default/*, CacheDependency dependencies = null, CacheItemRemovedCallback onRemoveCallback = null*/)
        {
            Add(key, value, location, absoluteExpiration, Cache.NoSlidingExpiration, priority/*, dependencies, onRemoveCallback*/);
        }

        public void Add(string key, object value, TimeSpan slidingExpiration, CacheLocation location = CacheLocation.Application, CacheItemPriority priority = CacheItemPriority.Default/*, CacheDependency dependencies = null, CacheItemRemovedCallback onRemoveCallback = null*/)
        {
            Add(key, value, location, Cache.NoAbsoluteExpiration, slidingExpiration, priority/*, dependencies, onRemoveCallback*/);
        }


        private void Add(string key, object value, CacheLocation location, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority/*, CacheDependency dependencies, CacheItemRemovedCallback onRemoveCallback*/)
        {
            SetCore(key, () => value, absoluteExpiration, slidingExpiration, priority, location);
        }

        #endregion

        #region Public Get Methods

        public T Get<T>(string key)
        {
            return GetOrAdd<T>(key, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default/*, null, null*/, CacheLocation.Application);
        }

        /*public T Get<T>(object key)
        {
            return GetOrAdd<T>(GetKey(key), null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null, null, CacheLocation.Application);
        }*/

        public T Get<T>(string key, CacheLocation location, Func<T> setValue = null/*, CacheDependency dependencies = null, CacheItemRemovedCallback onRemoveCallback = null*/)
        {
            return GetOrAdd<T>(key, setValue, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default/*, dependencies, onRemoveCallback*/, location);
        }

        /*public T Get<T>(object key, CacheLocation location, Func<T> setValue = null, CacheDependency dependencies = null, CacheItemRemovedCallback onRemoveCallback = null)
        {
            return GetOrAdd<T>(GetKey(key), setValue, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, dependencies, onRemoveCallback, location);
        }*/

        public T Get<T>(string key, CacheLocation location, DateTime absoluteExpiration, Func<T> setValue = null/*, CacheDependency dependencies = null, CacheItemRemovedCallback onRemoveCallback = null*/)
        {
            return GetOrAdd<T>(key, setValue, absoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default/*, dependencies, onRemoveCallback*/, location);
        }

        /*public T Get<T>(object key, CacheLocation location, DateTime absoluteExpiration, Func<T> setValue = null, CacheDependency dependencies = null, CacheItemRemovedCallback onRemoveCallback = null)
        {
            return GetOrAdd<T>(GetKey(key), setValue, absoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, dependencies, onRemoveCallback, location);
        }*/

        public T Get<T>(string key, CacheLocation location, TimeSpan slidingExpiration, Func<T> setValue = null/*, CacheDependency dependencies = null, CacheItemRemovedCallback onRemoveCallback = null*/)
        {
            return GetOrAdd<T>(key, setValue, Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.Default/*, dependencies, onRemoveCallback*/, location);
        }

        /*public T Get<T>(object key, CacheLocation location, TimeSpan slidingExpiration, Func<T> setValue = null, CacheDependency dependencies = null, CacheItemRemovedCallback onRemoveCallback = null)
        {
            return GetOrAdd<T>(GetKey(key), setValue, Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.Default, dependencies, onRemoveCallback, location);
        }*/

        /*public T GetOrLock<T>(string key, CacheLocation location, TimeSpan slidingExpiration, out object entryLock)
        {
            return GetOrLock<T>(key, Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.Default, null, null, location, out entryLock);
        }*/

        /*public void ReleaseEntryLock(string key, CacheLocation location)
        {
            RemoveEntryLock(key, location);
        }*/

        /*public void ReleaseEntryLock(object entryLock, CacheLocation location)
        {
            RemoveEntryLock(entryLock, location);
        }*/

        #endregion

        #region Public Remove Methods

        public void Remove(string key)
        {
            Remove(key, CacheLocation.Session);
            Remove(key, CacheLocation.Application);
        }

        public virtual bool Remove(string key, CacheLocation location)
        {
            object entryLock = GetEntryLock(key, location);
            lock (entryLock)
            {
                //if (ContainsKey(key, location) == true)
                {
                    if (location == CacheLocation.Session)
                    {
                        var sessionCache =new SessionCacheProvider().Cache;
                        if (sessionCache != null)
                        {
                            var item = sessionCache.Remove(key);
                            return item != null;
                        }

                    }
                    else if (location == CacheLocation.Application)
                    {
#if NET5
                        MemoryCache.Default.Remove(key);
#else
                        var applicationCache = HttpRuntime.Cache;
                        if (applicationCache != null)
                        {
                            var item = HttpRuntime.Cache.Remove(key);
                            return item != null;
                        }
#endif
                    }
                }
            }

            return false;
        }

        #endregion

        #region Public Clear Methods

        public virtual void Clear()
        {
            Clear(CacheLocation.Application);

            Clear(CacheLocation.Session);
        }

        /// <summary>
        /// Cleares the specified cache. Note: It is not guaranteed that a key/value that are added while the cache is being cleared, is removed. Actually its maybe not guaranteed that any entry is removed =)
        /// </summary>
        /// <param name="location"></param>
        public virtual void Clear(CacheLocation location)
        {
            if (location == CacheLocation.Session)
            {
                var sessionCache =new  SessionCacheProvider().Cache;
                if (sessionCache != null)
                {
                    foreach (var entry in sessionCacheLocks.ToList())
                    {
                        lock (entry.Value)
                        {
                            sessionCache.Remove(entry.Key);
                        }
                    }
                }
            }
            else if (location == CacheLocation.Application)
            {
#if NET5
                MemoryCache.Default.Trim(100); // Somehow I do not fullr trust this
                foreach (var entry in MemoryCache.Default) // Should not be any
                {
                    MemoryCache.Default.Remove(entry.Key);
                }
#else
                var applicationCache = HttpRuntime.Cache;
                if (applicationCache != null)
                {
                    foreach (var entry in applicationCacheLocks.ToList())
                    {
                        lock (entry.Value)
                        {
                            applicationCache.Remove(entry.Key);
                        }
                    }
                }
#endif
            }
        }

        #endregion

        protected T GetOrAdd<T>(string key, Func<T> setValue, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority/*, CacheDependency dependencies, CacheItemRemovedCallback onRemoveCallback*/, CacheLocation location)
        {
            object value = GetCore(key, setValue != null ? () => setValue() : (Func<object>)null, absoluteExpiration, slidingExpiration, priority/*, dependencies, onRemoveCallback*/, location);
            return value is T ? (T)value : default(T);
        }

        protected virtual object GetCore(string key, Func<object> valueFactory, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority/*, CacheDependency dependencies, CacheItemRemovedCallback onRemoveCallback*/, CacheLocation location)
        {
            object value = GetValueCore(key, location);

            if (value == null && valueFactory != null)
            {
                return SetCore(key, valueFactory, absoluteExpiration, slidingExpiration, priority/*, dependencies, onRemoveCallback*/, location);
            }

            if (object.ReferenceEquals(nullObject, value) == true)
            {
                return null;
            }

            return value;
        }

        /*protected virtual T GetOrLock<T>(string key, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheDependency dependencies, CacheItemRemovedCallback onRemoveCallback, CacheLocation location, out object entryLock)
        {
            object value = GetValueCore(key, location);

            if (value == null)
            {
                entryLock = GetEntryLock(key, location);
                Monitor.Enter(entryLock);
                return default(T);
            }
            else
            {
                entryLock = null;
            }

            return (T)value;
        }*/

        protected virtual object GetValueCore(string key, CacheLocation location)
        {
            if (location == CacheLocation.Session)
            {

                var sessionCache =new  SessionCacheProvider().Cache;
                if (sessionCache != null)
                {
                    return sessionCache[key];
                }
            }
            else if (location == CacheLocation.Application)
            {
#if NET5
                return System.Runtime.Caching.MemoryCache.Default[key];
#else
                var applicationCache = HttpRuntime.Cache;
                if (applicationCache != null)
                {
                    return HttpRuntime.Cache.Get(key);
                }
#endif
            }

            return null;
        }

        private object nullObject = new object();

        protected virtual object SetCore(string key, Func<object> valueFactory, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority/*, CacheDependency dependencies, CacheItemRemovedCallback onRemoveCallback*/, CacheLocation location)
        {
            if (valueFactory == null) throw new ArgumentNullException("valueFactory");

            var entryLock = GetEntryLock(key, location);

            lock (entryLock)
            {
                // Check if somebody accuired the lock right before and added a value for the entry. In this rare case, return the existing value
                object exisitingObject = GetValueCore(key, location);
                if (exisitingObject != null)
                {
                    // If it is a null placeholder, return null
                    if (object.ReferenceEquals(nullObject, exisitingObject) == true)
                    {
                        return null;
                    }
                    return exisitingObject;
                }

                // Value is factorized inside lock to avoid several thread factorize not used values
                object value = valueFactory();

                if (value == null) // If value is null, store a null placeholder instead (as web cache throws when storing null and we wants null to be a proper outcome of GetOrAdd)
                {
                    value = nullObject;
                }

                if (location == CacheLocation.Session)
                {
                    var sessionCache =new  SessionCacheProvider().Cache;
                    if (sessionCache != null)
                    {

#if NET5
                        // Not implement, but session state should not be used anyhow
#else
                        var policy = new System.Runtime.Caching.CacheItemPolicy
                        {
                            Priority = priority == CacheItemPriority.NotRemovable ? System.Runtime.Caching.CacheItemPriority.NotRemovable : System.Runtime.Caching.CacheItemPriority.Default
                        };

                        if (absoluteExpiration != Cache.NoAbsoluteExpiration)
                        {
                            policy.AbsoluteExpiration = absoluteExpiration;
                        }

                        if (slidingExpiration != Cache.NoSlidingExpiration)
                        {
                            policy.SlidingExpiration = slidingExpiration;
                        }

                        policy.RemovedCallback = args => OnRemovedCallback(CacheLocation.Session, args.CacheItem.Key, args.CacheItem.Value,
                            args.RemovedReason == System.Runtime.Caching.CacheEntryRemovedReason.Expired ? CacheItemRemovedReason.Expired :
                            args.RemovedReason == System.Runtime.Caching.CacheEntryRemovedReason.Removed ? CacheItemRemovedReason.Removed :
                            args.RemovedReason == System.Runtime.Caching.CacheEntryRemovedReason.ChangeMonitorChanged ? CacheItemRemovedReason.DependencyChanged : CacheItemRemovedReason.Underused, null/*, onRemoveCallback*/);

                        sessionCache.Add(key, value, policy);
#endif
                    }
                }
                else if (location == CacheLocation.Application)
                {
#if NET5
                    var policy = new System.Runtime.Caching.CacheItemPolicy
                    {
                        Priority = priority == CacheItemPriority.NotRemovable ? System.Runtime.Caching.CacheItemPriority.NotRemovable : System.Runtime.Caching.CacheItemPriority.Default
                    };

                    if (absoluteExpiration != Cache.NoAbsoluteExpiration)
                    {
                        policy.AbsoluteExpiration = absoluteExpiration;
                    }

                    if (slidingExpiration != Cache.NoSlidingExpiration)
                    {
                        policy.SlidingExpiration = slidingExpiration;
                    }

                    return MemoryCache.Default.Add(key, value, policy);
#else
                    var applicationCache = HttpRuntime.Cache;
                    if (applicationCache != null)
                    {
                        // TODO Null place holder on remove callback
                        applicationCache.Add(key, value/*, dependencies*/, null, absoluteExpiration, slidingExpiration, priority, (_key, _value, _reason) => OnRemovedCallback(CacheLocation.Application, _key, _value, _reason/*, onRemoveCallback*/, null));
                    }
#endif
                }

                return value;
            }
        }


        protected virtual void OnRemovedCallback(CacheLocation location, string key, object value, CacheItemRemovedReason reason, CacheItemRemovedCallback callback)
        {
            RemoveEntryLock(key, location);

            if (callback != null)
            {
                callback(key, value, reason);
            }
        }

        /*protected virtual string GetKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, key);
                return Convert.ToBase64String(stream.ToArray());
            }
        }*/

        public class SourceValueResult<TSource, TValue>
        {
            public string Key { get; set; }

            public TSource Source { get; set; }

            public TValue Value { get; set; }
        }

        //public IDictionary<TSource, TResult> GetOrAdd<TSource, TValue, TResult>(IEnumerable<TSource> source, CacheLocation location, Func<TSource, string> keySelector, Func<TSource, TResult> resultFactory)
        //{
        //    var cacheResults = source.Select(s => { string key = keySelector(s); return new SourceValueResult<TSource, TResult> { Key = key, Source = s, Value = Get<TResult>(key, location) }; }).ToArray();

        //    var missingResults = cacheResults.Where(r => r.Value == null).ToArray();

        //    if (missingResults.Any() == true)
        //    {
        //        var missingKeys = missingResults.Select(r => r.Key);

        //        var entryLocks = LockAllEntries(missingKeys, location);

        //        foreach (var missingResult in missingResults)
        //        {
        //            missingResult.Value = resultFactory(missingResult.Source);                  
        //        }

        //        UnlockAllEntries(entryLocks, location);
        //    }

        //    return cacheResults.ToDictionaryOptimized(r => r.Source, r => r.Value);
        //}

        //IDictionary<TSource, TResult> 
        public TResult GetOrAdd<TData, TSource, TValue, TResult>(IEnumerable<TSource> source, CacheLocation location, TimeSpan slidingExpiration, Func<TSource, string> keySelector, Func<IEnumerable<TSource>, TData> dataProvider, Func<TSource, TData, TValue> valueSelector, Func<IEnumerable<SourceValueResult<TSource, TValue>>, TResult> resultSelector)
        {
            var cacheResults = source.Select(s => { string key = keySelector(s); return new SourceValueResult<TSource, TValue> { Key = key, Source = s, Value = Get<TValue>(key, location) }; }).ToArray();

            var missingResults = cacheResults.Where(r => r.Value == null).ToArray();

            if (missingResults.Any() == true)
            {
                var missingKeys = missingResults.Select(r => r.Key);

                var data = dataProvider(missingResults.Select(r => r.Source));

                var entryLocks = LockAllEntries(missingKeys, location);

                foreach (var missingResult in missingResults)
                {
                    missingResult.Value = valueSelector(missingResult.Source, data);

                    SetCore(missingResult.Key, () => missingResult.Value, Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.Default/*, null, null*/, location);
                }

                UnlockAllEntries(entryLocks, location);
            }

            return resultSelector(cacheResults);//.ToDictionaryOptimized(r => r.Source, r => r.Result);
        }

        //public IEnumerable<KeyValuePair<TSource, TResult>> GetOrAdd<TSource, TResult>(IEnumerable<TSource> source, CacheLocation location, Func<TSource, string> keySelector, Func<IEnumerable<TSource>, IEnumerable<KeyValuePair<TSource, TResult>>> resultsFactory)
        //{
        //    var sourceList = source.ToArray();
        //    var itemKeyPair = sourceList.Select(i => new KeyValuePair<string, TSource>(keySelector(i), i)).ToArray();

        //    var cacheResults = itemKeyPair.Select(itemKey => new { ItemKey = itemKey, Result = Get<TResult>(itemKey.Key, location) }).ToArray();

        //    var missing = cacheResults.Where(r => r.Result == null).ToArray();

        //    if (missing.Any() == true)
        //    {
        //        var missingKeys = missing.Select(r => r.ItemKey.Key);

        //        var entryLocks = LockAllEntries(missingKeys, location);

        //        var results = resultsFactory(missing.Select(r => r.ItemKey.Value));

        //        UnlockAllEntries(entryLocks, location);

        //        return results.Concat(cacheResults.Where(r => r.Result != null).Select(r => new KeyValuePair<TSource, TResult>(r.ItemKey.Value, r.Result)));
        //    }

        //    // We had them all
        //    return cacheResults.Select(r => new KeyValuePair<TSource, TResult>(r.ItemKey.Value, r.Result));
        //}

        public IEnumerable<object> LockAllEntries(IEnumerable<string> keys, CacheLocation location)
        {
            var entryLocks = new List<object>();
            if (location == CacheLocation.Session)
            {
                lock (sessionCacheLocksLock)
                {
                    foreach (string key in keys)
                    {
                        object entryLock;
                        if (sessionCacheLocks.TryGetValue(key, out entryLock) == false)
                        {
                            entryLock = new object();
                            sessionCacheLocks[key] = entryLock;
                        }

                        Monitor.Enter(entryLock);
                        entryLocks.Add(entryLock);
                    }
                }
            }
            else
            {
                lock (applicationCacheLocksLock)
                {
                    foreach (string key in keys)
                    {
                        object entryLock;
                        if (applicationCacheLocks.TryGetValue(key, out entryLock) == false)
                        {
                            entryLock = new object();
                            applicationCacheLocks[key] = entryLock;
                        }

                        Monitor.Enter(entryLock);
                        entryLocks.Add(entryLock);
                    }
                }
            }
            return entryLocks;
        }

        public void UnlockAllEntries(IEnumerable<object> entryLocks, CacheLocation location)
        {
            if (location == CacheLocation.Session)
            {
                foreach (object entryLock in entryLocks)
                {
                    Monitor.Exit(entryLock);
                }
            }
            else
            {
                foreach (object entryLock in entryLocks)
                {
                    Monitor.Exit(entryLock);
                }
            }
        }

        /*public void UnlockAllEntries(IEnumerable<string> keys, CacheLocation location)
        {
            if (location == CacheLocation.Session)
            {
                foreach (string key in keys)
                {
                    object entryLock = sessionCacheLocks[key];
                    Monitor.Exit(entryLock);
                }
            }
            else
            {
                foreach (string key in keys)
                {
                    object entryLock = applicationCacheLocks[key];
                    Monitor.Exit(entryLock);
                }
            }
        }*/

        protected virtual object GetEntryLock(string key, CacheLocation location)
        {
            object entryLock;
            if (location == CacheLocation.Session)
            {
                if (sessionCacheLocks.TryGetValue(key, out entryLock) == false)
                {
                    lock (sessionCacheLocksLock)
                    {
                        if (sessionCacheLocks.TryGetValue(key, out entryLock) == false)
                        {
                            entryLock = new object();
                            sessionCacheLocks[key] = entryLock;
                        }
                    }
                }
            }
            else
            {
                if (applicationCacheLocks.TryGetValue(key, out entryLock) == false)
                {
                    lock (applicationCacheLocksLock)
                    {
                        if (applicationCacheLocks.TryGetValue(key, out entryLock) == false)
                        {
                            entryLock = new object();
                            applicationCacheLocks[key] = entryLock;
                        }
                    }
                }
            }
            return entryLock;
        }

        protected virtual void RemoveEntryLock(string key, CacheLocation location)
        {
            if (location == CacheLocation.Session)
            {
                lock (sessionCacheLocksLock)
                {
                    sessionCacheLocks.Remove(key);
                }
            }
            else
            {
                lock (applicationCacheLocksLock)
                {
                    applicationCacheLocks.TryRemove(key);
                }
            }
        }

    }
}