using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Xml.Serialization;

public class XmlHelper
{
    public static string XmlSerialize<T>(T t)
    {
        XmlSerializer xs = new XmlSerializer(typeof(T));
        MemoryStream ms = new MemoryStream();
        xs.Serialize(ms, t);
        string XmlString = Encoding.UTF8.GetString(ms.ToArray());
        ms.Close();
        return XmlString;
    }

    public static T XmlDeserialize<T>(string xmlString)
    {
        throw new NotImplementedException();
    }
}