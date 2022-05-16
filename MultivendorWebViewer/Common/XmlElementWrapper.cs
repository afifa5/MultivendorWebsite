using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MultivendorWebViewer.Common
{
    internal static class XmlElementWrapperData
    {
        private static Dictionary<Tuple<Type, string>, Type> cachedObjectTypes = new Dictionary<Tuple<Type, string>, Type>();

        private static object cachedObjectTypesLock = new object();

        public static Type GetType(string name, Type parentInstanceType)
        {
            var key = new Tuple<Type, string>(parentInstanceType, name);

            Type objectType;
            if (cachedObjectTypes.TryGetValue(key, out objectType) == false)
            {
                lock (cachedObjectTypesLock)
                {
                    if (cachedObjectTypes.TryGetValue(key, out objectType) == false)
                    {
                        //var tType = typeof(T);
                        //var tAssembly = Assembly.GetAssembly(tType);
                        //var callingAssembly = Assembly.GetCallingAssembly();


                        //// T assembly (high chance that some implementors are defined)
                        //var type = tAssembly.GetExportedTypes().Where(t => name == (fullName == true ? t.FullName : t.Name) && tType.IsAssignableFrom(t) == true).FirstOrDefault();

                        //// Calling assembly
                        //if (type == null && callingAssembly != tAssembly)
                        //{
                        //    type = callingAssembly.GetExportedTypes().Where(t => name == (fullName == true ? t.FullName : t.Name) && tType.IsAssignableFrom(t) == true).FirstOrDefault();
                        //}

                        //// We have a type, check for customized extensions
                        //if (type != null)
                        //{
                        //    objectType = Instance.GetExtension(type);
                        //}
                        //else
                        //{
                        //    // Check customized type
                        //    objectType = Instance.GetTypes().Where(t => name == (fullName == true ? t.FullName : t.Name) && tType.IsAssignableFrom(t) == true).FirstOrDefault();
                        //    if (objectType == null)
                        //    {
                        //        // Check for any extension the in the customize module
                        //        objectType = Instance.GetExtension(tType);
                        //    }
                        //}
                        var types = Instance.FindTypes(name, () => Assembly.GetAssembly(parentInstanceType), () => Assembly.GetCallingAssembly());

                        cachedObjectTypes[key] = objectType = types.Where(t => parentInstanceType.IsAssignableFrom(t)).FirstOrDefault();
                    }
                }
            }

            return objectType;
        }
    }

    public class XmlElementWrapper<T> : IXmlSerializable
        //where T : class
    {


        public T Object { get; set; }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            #region old
            //string nameOfElement = reader.GetAttribute("type") ?? reader.LocalName;

            //Type baseType = GetObjectType(nameOfElement);

            //if (baseType != null)
            //{
            //    Object = Instance.Deserialize(baseType, reader, useDataContractSerializer: false) as T;
            //}
            //else
            //{
            //    reader.Skip();
            //}
            #endregion

            try
            {
                reader.MoveToContent();

                string typeName = reader.GetAttribute("type");

                bool emptyElement = reader.IsEmptyElement;

                reader.ReadStartElement();

                if (emptyElement == false)
                {
                    reader.MoveToContent();

                    Type baseType = GetType(typeName ?? reader.LocalName);

                    if (baseType != null)
                    {
                        DeserializeObject(baseType, reader);
                    }
                    else
                    {
                        reader.Skip();
                    }

                    reader.ReadEndElement();
                }
                else
                {

                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        protected virtual void DeserializeObject(Type baseType, XmlReader reader)
        {
            Object = (T)Instance.Deserialize(baseType, reader, useDataContractSerializer: false);
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(typeof(T).Name);

            var type = Object.GetType();
            var serializer = new XmlSerializer(type, new XmlRootAttribute(type.Name));
            serializer.Serialize(writer, Object);

            writer.WriteEndElement();
            //Type baseType = Object.GetType();
            //Instance.Serialize(Object, writer, useDataContractSerializer: false);
        }

        protected virtual Type GetType(string name)
        {
            return XmlElementWrapperData.GetType(name, typeof(T));
        }

        public static implicit operator T(XmlElementWrapper<T> wrapper)
        {
            return wrapper != null ? wrapper.Object : default(T);
        }
    }

    public class CachedXmlElementWrapper<T> : XmlElementWrapper<T>
    {
        public string Xml { get; set; }

        public Type BaseType { get; set; }

        public T CreateObject()
        {
            if (Xml != null)
            {
                T obj = (T)Instance.Deserialize(BaseType, Xml, useDataContractSerializer: false);
                return obj;
            }
            return default(T);
        }

        protected override void DeserializeObject(Type baseType, XmlReader reader)
        {
            if (Xml == null)
            {
                Xml = reader.ReadOuterXml();
                BaseType = baseType;
            }
        }
    }

    public class XmlElementWrapperCollection<T> : List<T>, IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement || reader.Read() == false)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                string typeName = reader.LocalName;
                var elementType = ResolveType(typeName);
                if (elementType != null)
                {
                    var elementSerializer = Instance.CreateXmlSerializer(elementType, typeName);
                    T element = (T)elementSerializer.Deserialize(reader);
                    AddInstance(element);
                }
                else
                {
                    reader.Skip();
                }
            }
            reader.ReadEndElement();
        }

        protected virtual void AddInstance(T item)
        {
            Add(item);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        protected virtual Type ResolveType(string name)
        {
            return XmlElementWrapperData.GetType(name, typeof(T));
        }
    }

    public static class XmlElementWrapperExtensions
    {
        /// <summary>
        /// Gets the object of the XmlElementWrapper. This extensions method differs from the instance method in that this method return null instead of throwing NullReferenceException if the XmlElementWrapper itself if null. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static T GetObjectSafe<T>(this XmlElementWrapper<T> wrapper)
            where T : class
        {
            return wrapper != null ? wrapper.Object : null;
        }

        public static T CreateObjectSafe<T>(this CachedXmlElementWrapper<T> wrapper)
            where T : class
        {
            return wrapper != null ? wrapper.CreateObject() : null;
        }
    }
}
