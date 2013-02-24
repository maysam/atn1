using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ATN.Data
{
    public static class XmlHelper
    {
        /// <summary>
        /// Serializes the passed object into an XML string
        /// </summary>
        /// <typeparam name="T">The type of object being passed</typeparam>
        /// <param name="t">The object which to serailize</param>
        /// <returns>A serialized XML representation of the passed object</returns>
        public static string XmlSerialize<T>(T t)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            xs.Serialize(ms, t);
            string XmlString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return XmlString;
        }

        /// <summary>
        /// Deserializes the XML representation of an object
        /// </summary>
        /// <typeparam name="T">The type of object the xmlString represents</typeparam>
        /// <param name="xmlString">The serialized representation of an object</param>
        /// <returns>An object constructed from the passed XML string</returns>
        public static T XmlDeserialize<T>(string xmlString)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            StringReader sr = new StringReader(xmlString);
            T Object = (T)xs.Deserialize(sr);
            sr.Close();
            return Object;
        }
    }
}