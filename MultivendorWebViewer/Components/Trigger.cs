using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Configuration
{
    public class Trigger<TItem, TIdentifier> : IXmlSerializable
        where TItem : class
        where TIdentifier : Identifier
    {
        public TItem Item { get; set; }

        public Identifier[] Triggers { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        private static Dictionary<string, XmlSerializer> typedIdentitySerializers = new Dictionary<string, XmlSerializer>();
        private static object typedIdentitySerializersLock = new object();

        public void ReadXml(XmlReader reader)
        {
            string itemName = reader.LocalName;

            string outerXml = reader.ReadOuterXml();

            using (var triggerStringReader = new StringReader(outerXml))
            using (var triggerReader = XmlReader.Create(triggerStringReader))
            {
                var identitySerializer = Instance.CreateXmlSerializer(typeof(TIdentifier), new XmlRootAttribute { ElementName = "Trigger" });
                var triggers = new List<Identifier>();

                triggerReader.MoveToContent();
                triggerReader.Read();
                triggerReader.MoveToContent();
                //triggerReader.Read();

                while (triggerReader.LocalName.EndsWith("Trigger") == true)
                {
                    string localName = triggerReader.LocalName;
                    if (localName == "CompoundTrigger")
                    {
                        var compoundType = typeof(CompoundIdentifier<TIdentifier>);
                        var compoundSerializer = Instance.CreateXmlSerializer(compoundType, new XmlRootAttribute { ElementName = "CompoundTrigger" });
                        var compoundIdentifier = compoundSerializer.Deserialize(triggerReader) as CompoundIdentifier<TIdentifier>;
                        if (compoundIdentifier != null)
                        {
                            triggers.Add(compoundIdentifier);
                        }
                        continue;
                    }
                    else if (localName.Length > 7)
                    {
                        string triggerTypeName = localName.Substring(0, localName.Length - 7) + "Identifier";
                        XmlSerializer typedIdentitySerializer;
                        if (typedIdentitySerializers.TryGetValue(triggerTypeName, out typedIdentitySerializer) == false)
                        {
                            lock (typedIdentitySerializersLock)
                            {
                                if (typedIdentitySerializers.TryGetValue(triggerTypeName, out typedIdentitySerializer) == false)
                                {
                                    var triggerBaseType = typeof(Identifier);
                                    var type = Instance.GetTypes().Union(triggerBaseType.Assembly.GetTypes()).Where(t => t.Name == triggerTypeName && triggerBaseType.IsAssignableFrom(t) == true).FirstOrDefault();
                                    if (type != null)
                                    {
                                        typedIdentitySerializer = Instance.CreateXmlSerializer(type, new XmlRootAttribute { ElementName = triggerTypeName });
                                        typedIdentitySerializers.Add(triggerTypeName, typedIdentitySerializer);
                                    }
                                }
                            }
                        }

                        if (typedIdentitySerializer != null)
                        {
                            var typedIdentifier = typedIdentitySerializer.Deserialize(triggerReader) as Identifier;
                            if (typedIdentifier != null)
                            {
                                triggers.Add(typedIdentifier);
                            }
                            continue;
                        }

                    }

                    var identifier = identitySerializer.Deserialize(triggerReader) as Identifier;
                    if (identifier != null)
                    {
                        triggers.Add(identifier);
                    }

                    // Multi trigger
                }

                if (triggers.Count > 0)
                {
                    Triggers = triggers.ToArray();
                }
            }

            // Deserialize item
            using (var itemStringReader = new StringReader(outerXml))
            using (var itemReader = XmlReader.Create(itemStringReader))
            {
                Type type = GetType(itemReader.LocalName) ?? typeof(TItem);

                var itemSerializer = Instance.CreateXmlSerializer(type, new XmlRootAttribute { ElementName = itemName });
                var item = itemSerializer.Deserialize(itemReader) as TItem;
                var mergeableSettingsItem = item as IInheritable;
                if (mergeableSettingsItem != null)
                {
                    if (mergeableSettingsItem.BaseInstance != null)
                    {
                        item = mergeableSettingsItem.CreatedMerged(mergeableSettingsItem.BaseInstance) as TItem ?? item;
                    }
                }

                Item = item;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            // TODO
        }

        private static Dictionary<string, Type> cachedObjectTypes = new Dictionary<string, Type>();

        private static object cachedObjectTypesLock = new object();

        protected virtual Type GetType(string name)
        {
            Type objectType;
            if (cachedObjectTypes.TryGetValue(name, out objectType) == false)
            {
                lock (cachedObjectTypesLock)
                {
                    if (cachedObjectTypes.TryGetValue(name, out objectType) == false)
                    {
                        cachedObjectTypes[name] = objectType = Instance.FindTypes(name, () => Assembly.GetAssembly(typeof(TItem)), () => Assembly.GetCallingAssembly()).FirstOrDefault();
                    }
                }
            }

            return objectType;
        }
    }
    public interface IInheritable
    {
        string Id { get; set; }

        string Base { get; set; }

        IInheritable BaseInstance { get; }

        IInheritable CreatedMerged(IInheritable baseInstance);

        void MergeWith(IInheritable instance);
    }
}
