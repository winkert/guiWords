using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Core.Utils
{
	[Serializable] 
	public class SystemSetting 
	{ 
		public SystemSetting() {	} 
		public SystemSetting(string n, string v) 
		{ 
			Name = n;
			Value = v;
		} 
		public string Name {get { return sName;} set {sName = value;}}
		public string Value{get { return sValue;} set {sValue = value;}}
		private string sName;
		private string sValue;
	}

	public static class ExtensionMethods
	{
		/// <summary>
		/// Serialize a list of serializable objects into a bin file.
		/// </summary>
		/// <param name="fileLoc">Absolute location of destination</param>
		public static void serialize<T>(this List<T> c, string fileLoc) where T : class
		{
		    using (Stream FileStream = File.Create(fileLoc))
		    {
			    BinaryFormatter serializer = new BinaryFormatter();
			    try
			    {
				    serializer.Serialize(FileStream, c);
			    }
			    catch (Exception e)
			    {
				    throw e;
			    }
		    }
		}
		/// <summary>
		/// Deserialize a list of serializable objects from a string defining a url.
		/// </summary>
		/// <returns>new List()</returns>
		public static List<T> deserialize<T>(this string fileLoc) where T : class
		{
		    using (Stream FileStream = File.OpenRead(fileLoc))
		    {
			    BinaryFormatter deserializer = new BinaryFormatter();
			    try
			    {

                    deserializer.Binder = new typeconvertor();
    				return (List<T>)deserializer.Deserialize(FileStream);
			    }
			    catch (Exception e)
			    {
                    throw e;
			    }
		    }
		}
	}
    sealed class typeconvertor : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type returntype = null;
            if (assemblyName == "SettingSerializer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")
            {
                assemblyName = Assembly.GetExecutingAssembly().FullName;
                returntype = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
                return returntype;
            }
            if (typeName == "System.Collections.Generic.List`1[[Core.Utils.SystemSetting, SettingSerializer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]")
            {
                typeName =typeName.Replace("SettingSerializer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Assembly.GetExecutingAssembly().FullName);
                returntype = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
                return returntype;
            }
            return returntype;
        }
    }
}