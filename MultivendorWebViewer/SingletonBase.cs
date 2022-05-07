using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace MultivendorWebViewer
{
    public abstract class SingletonBase<T>
    {
        static SingletonBase()
        {
            Type type = Instance.GetExtension(typeof(T));

            if (type.IsAbstract == false)
            {
                SingletonBehaviourAttribute attribute = type.GetCustomAttribute<SingletonBehaviourAttribute>(true);

                if (attribute == null || attribute.CreateDefaultInstance == true)
                {
                    try
                    {
                        SingletonBase<T>.current = Instance.Create<T>();
                    }
                    catch
                    {
                        // Could not create default instance
                        throw;
                    }
                }
            }
        }

        public static event EventHandler CurrentChanged;

        private static T current;
        public static T Default
        {
            get
            {
                return current;
            }
            set
            {
                if (EqualityComparer<T>.Default.Equals(current, value) == false)
                {
                    current = value;

                    OnCurrentChanged();
                }
            }
        }

        protected static void OnCurrentChanged()
        {
            if (CurrentChanged != null)
            {
                CurrentChanged(null, EventArgs.Empty);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SingletonBehaviourAttribute : Attribute
    {
        public SingletonBehaviourAttribute(bool createDefaultInstance = true)
        {
            CreateDefaultInstance = createDefaultInstance;
        }

        public bool CreateDefaultInstance { get; set; }
    }

    public abstract class ConfigurationProviderBase<T>
    {
        public virtual T Configuration { get; set; }
    }

    public abstract class ConfigurationProviderBase<TProvider, TConfiguration> : SingletonBase<TProvider>
        where TProvider : new()
        where TConfiguration : class
    {
        private TConfiguration configuration;
        public virtual TConfiguration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    configuration = GetConfiguration();
                }
                return configuration;
            }
            set
            {
                configuration = value;
            }
        }

        public virtual void Reset()
        {
            configuration = null;
        }

        protected abstract TConfiguration GetConfiguration();
    }

    public class ConfigurationProvider<TProvider, TConfiguration> : ConfigurationProviderBase<TProvider, TConfiguration>
        where TProvider : new()
        where TConfiguration : class
    {
        protected override TConfiguration GetConfiguration()
        {
            return Instance.Create<TConfiguration>();
        }
    }

    public class SiteConfigurationFileProviderBase<TProvider, TConfiguration> : ConfigurationFileProviderBase<TProvider, TConfiguration>
        where TProvider : new()
        where TConfiguration : class
    {
        private Dictionary<string, TConfiguration> ConfigurationMap = new Dictionary<string, TConfiguration>();

        public override TConfiguration Configuration { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public virtual TConfiguration SiteConfiguration(string siteId)
        {
            if (String.IsNullOrEmpty(siteId) == false && ConfigurationMap.ContainsKey(siteId))
            {
                return ConfigurationMap[siteId];
            }

            if (String.IsNullOrEmpty(siteId) == false)
            {
                ConfigurationMap[siteId] = GetConfiguration(siteId);
                return ConfigurationMap[siteId];
            }

            return base.GetConfiguration();
        }

        public virtual TConfiguration GetConfiguration(string siteId)
        {
            if (String.IsNullOrEmpty(siteId) == false && ConfigurationMap.ContainsKey(siteId))
            {
                // Return cache entry
                return ConfigurationMap[siteId];
            }

            // Load site definition from file
            string filePath = GetFilePath(siteId);
            if (String.IsNullOrEmpty(filePath) == false)
            {
                TConfiguration configuration = Load(filePath);
                ConfigurationMap[siteId] = configuration;
                return configuration;
            }

            TConfiguration baseConfiguration = base.Configuration;
            if (baseConfiguration != null && CreateFile == true && File.Exists(FilePath) == false)
            {
                // Create a new default file when none existed
                Save(baseConfiguration);
                baseConfiguration = LoadCore(FilePath);
            }

            return baseConfiguration;
        }

        public virtual string GetFilePath(string siteId)
        {
            string filePath = Path.GetDirectoryName(DefaultFilePath);
            string fileName = Path.GetFileName(DefaultFilePath);

            if (String.IsNullOrEmpty(siteId) == false)
            {
                string path1 = Path.Combine(filePath, siteId, fileName);
                if (File.Exists(path1)) return path1;

                string name = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);

                string path2 = Path.Combine(filePath, name + "." + siteId + ext);
                if (File.Exists(path2)) return path2;
            }

            return null;
        }

        protected override string DefaultFilePath { get { throw new NotImplementedException(); } }

        public override void Reset()
        {
            ConfigurationMap = new Dictionary<string, TConfiguration>();
            base.Reset();
        }
    }

    public abstract class ConfigurationFileProviderBase<TProvider, TConfiguration> : ConfigurationProviderBase<TProvider, TConfiguration>
        where TProvider : new()
        where TConfiguration : class
    {
        public event EventHandler Changed;

        public ConfigurationFileProviderBase()
        {
            FilePath = DefaultFilePath;
        }

        public virtual bool DoWatch { get { return false; } }

        private FileSystemWatcher Watcher { get; set; }

        public virtual string FilePath { get; set; }

        protected abstract string DefaultFilePath { get; }

        protected virtual bool CanSave { get { return true; } }

        public override void Reset()
        {
            base.Reset();
            FilePath = null;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", typeof(TConfiguration).Name, FilePath);
        }

        /// <summary>
        /// Gets if a default configuration should be created if no configuration was loaded
        /// </summary>
        protected virtual bool CreateConfiguration { get { return false; } }

        /// <summary>
        /// Gets if a file should automatically be created if file is missing
        /// </summary>
        protected virtual bool CreateFile { get { return false; } }

        protected virtual Encoding Encoding { get { return null; } }

        protected virtual TConfiguration CreateDefaultConfiguration()
        {
            return Instance.Create<TConfiguration>();
        }

        protected virtual XmlReaderSettings GetXmlReaderSettings()
        {
            return null;
        }

        public virtual TConfiguration Load(string filePath)
        {
            TConfiguration configuration = LoadCore(filePath);

            if (configuration == null && CreateConfiguration)
            {
                configuration = CreateDefaultConfiguration();
            }

            return configuration;
        }

        Semaphore LockSemaphore = new Semaphore(1, 1);

        protected virtual TConfiguration LoadCore(string filePath)
        {
            if (File.Exists(filePath) == true)
            {
                try
                {
                    LockSemaphore.WaitOne();

                    if (Watcher != null)
                    {
                        Watcher.EnableRaisingEvents = false;
                        Watcher.Changed -= Watcher_Changed;
                        Watcher = null;
                    }

                    string xml = null;
                    try
                    {
                        xml = File.ReadAllText(filePath);
                    }
                    catch (IOException)
                    {
                        xml = File.ReadAllText(filePath);
                    }
					
                    string snippetDirectory = Path.GetDirectoryName(filePath);

                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml);
                        //Snippet setting SM
                        if (xmlDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            xmlDoc.RemoveChild(xmlDoc.FirstChild);
                        }
                        XmlNodeList xnList = xmlDoc.GetElementsByTagName("ProfileConfigSnippet");
                        List<string> snippetFileName = new List<string>();
                        foreach (XmlNode xn in xnList)
                        {
                            snippetFileName.Add(xn.InnerText);
                        }
                        foreach (string snipetFileName in snippetFileName)
                        {
                            XmlDocument xmlSnipetDoc = new XmlDocument();
                            string xmlSnipetString = File.ReadAllText(Path.Combine(snippetDirectory, snipetFileName));
                            xmlSnipetDoc.LoadXml(xmlSnipetString);
                            if (xmlSnipetDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                            {
                                xmlSnipetDoc.RemoveChild(xmlSnipetDoc.FirstChild);
                            }
                            XmlNode root = xmlSnipetDoc.FirstChild;

                            if (root.HasChildNodes)
                            {
                                foreach (XmlNode childNode in root.ChildNodes)
                                {
                                    XmlNode newChild = xmlDoc.ImportNode(childNode, true);
                                    xmlDoc.DocumentElement.AppendChild(newChild);
                                }
                            }
                        }
                        using (StringWriter sw = new StringWriter())
                        using (XmlTextWriter xw = new XmlTextWriter(sw))
                        {
                            //Snippet
                            xmlDoc.WriteTo(xw);
                            xml = sw.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        var pop = filePath;
                    }

                    //doc
                    var xmlReaderSettings = GetXmlReaderSettings();

                    TConfiguration configuration = null;

                    try
                    {
                        configuration = Deserialize(xml, xmlReaderSettings);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to deserialize file " + filePath, ex);
                    }

                    FilePath = filePath;

                    if (DoWatch == true)
                    {
                        string path = Path.GetDirectoryName(filePath);
                        string name = Path.GetFileName(filePath);

                        Watcher = new FileSystemWatcher(path, name);
                        Watcher.Changed += Watcher_Changed;
                        Watcher.EnableRaisingEvents = true;
                    }

                    return configuration;
                }
                finally
                {
                    LockSemaphore.Release();
                }
            }

            return default(TConfiguration);
        }

        protected virtual TConfiguration Deserialize(string xml, XmlReaderSettings xmlReaderSettings)
        {
            return xmlReaderSettings != null ? Instance.Deserialize<TConfiguration>(xml, xmlReaderSettings: xmlReaderSettings) : Instance.Deserialize<TConfiguration>(xml);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            this.Configuration = null;
            Thread.Sleep(100);      // Wait some when the file is being written

            OnChanged();
        }

        public virtual void Save()
        {
            if (CanSave == true && Configuration != null)
            {
                Save(Configuration);
            }
        }

        public virtual void Save(TConfiguration configuration)
        {
            try
            {
                LockSemaphore.WaitOne();

                CreateDirectory();

                string xml = Instance.Serialize<TConfiguration>(configuration);
                File.WriteAllText(FilePath, xml);
            }
            finally
            {
                LockSemaphore.Release();
            }
        }

        protected override TConfiguration GetConfiguration()
        {
            if (FilePath == null) FilePath = DefaultFilePath;

            TConfiguration configuration = Load(FilePath);

            if (configuration != null && CreateFile == true && File.Exists(FilePath) == false)
            {
                Save(configuration);
                configuration = LoadCore(FilePath);
            }

            return configuration;
        }

        protected virtual void CreateDirectory()
        {
            string directoryPath = Path.GetDirectoryName(FilePath);
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        protected virtual void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }
    }
    public sealed class EmptyLookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        public EmptyLookup()
        {

        }

        public bool Contains(TKey key)
        {
            return false;
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return 0; }
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get { return Enumerable.Empty<TElement>(); }
        }


        static EmptyLookup()
        {
            Default = new EmptyLookup<TKey, TElement>();
        }

        public static EmptyLookup<TKey, TElement> Default { get; private set; }
    }

}
