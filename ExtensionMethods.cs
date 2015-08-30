using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace guiWords.Utilities
{
    public static class ExtensionMethods
    {
        #region Serialization Methods
        /// <summary>
        /// Serialize a serializable object to an bin file specified.
        /// </summary>
        /// <param name="fileLoc">Absolute location of destination</param>
        public static void serialize<T>(this List<T> c, string fileLoc)
        {

            using (Stream FileStream = File.Create(fileLoc))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(FileStream, c);
            }
        }
        /// <summary>
        /// Deserialize a serializable object from a string defining a url.
        /// </summary>
        /// <returns></returns>
        public static List<T> deserialize<T>(this string fileLoc) where T : class
        {

            using (Stream FileStream = File.OpenRead(fileLoc))
            {
                BinaryFormatter deserializer = new BinaryFormatter();
                try
                {
                    return (List<T>)deserializer.Deserialize(FileStream);
                }
                catch (Exception e)
                {
                    MainWindow.AppLog.WriteLog("Unable to load the file due to an error." + "/r/n" + e.Message);
                    throw;
                }
            }

        }
        #endregion
        #region Enum Methods
        /// <summary>
        /// Returns the Description attribute of an enum value if that attribute exists. Otherwise, it returns the name.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns>String</returns>
        public static string GetDescription(this Enum enumValue)
        {
            object[] attr = enumValue.GetType().GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attr.Length > 0
               ? ((DescriptionAttribute)attr[0]).Description
               : enumValue.ToString();
        }
        /// <summary>
        /// Returns an Enum with the description of the string. Otherwise it returns an Enum with the name of the string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stringVal"></param>
        /// <param name="defaultValue"></param>
        /// <returns>Enum</returns>
        public static T ParseEnum<T>(this string stringVal)
        {
            Type type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            System.Reflection.MemberInfo[] fields = type.GetFields();
            foreach (var field in fields)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0 && attributes[0].Description == stringVal)
                {
                    return (T)Enum.Parse(typeof(T), field.Name);
                }
            }
            return (T)Enum.Parse(typeof(T), stringVal);
        }
        /// <summary>
        /// Method to get the description or names of an enum.
        /// </summary>
        /// <param name="t">typeof(T)</param>
        /// <returns>Array of Descriptions or Names of the Enums</returns>
        public static string[] GetEnumNames(Type t)
        {
            Array EnumValues = Enum.GetValues(t);
            string[] items = new string[EnumValues.Length];
            int i = 0;
            foreach (Enum e in EnumValues)
            {
                items[i] = e.GetDescription();
                i++;
            }
            return items;
        }
        #endregion
    }
}
