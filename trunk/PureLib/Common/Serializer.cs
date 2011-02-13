using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PureLib.Common {
    /// <summary>
    /// Provides methods to serialize and deserialize objects to XML or binary.
    /// </summary>
    public static class Serializer {
        /// <summary>
        /// Serializes an object and write the XML string to disk.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="indent"></param>
        /// <param name="fileMode"></param>
        public static void WriteToXml(this string path, object obj, bool indent = true, FileMode fileMode = FileMode.Create) {
            FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None);
            ToXml(obj, stream, indent);
        }

        /// <summary>
        /// Serializes an object to XML string.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public static string ToXml(this object obj, bool indent = true) {
            MemoryStream stream = new MemoryStream();
            ToXml(obj, stream, indent);
            byte[] buffer = stream.GetBuffer();
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }

        private static void ToXml(object obj, Stream stream, bool indent) {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings() { Indent = indent });
            serializer.Serialize(writer, obj);
            writer.Flush();
            writer.Close();
            stream.Close();
        }

        /// <summary>
        /// Deserializes an object from the XML string read from disk.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="fileMode"></param>
        /// <returns></returns>
        public static T ReadFromXml<T>(this string path, FileMode fileMode = FileMode.Open) {
            FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read);
            return FromXml<T>(stream);
        }

        /// <summary>
        /// Deserializes an object from XML string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T FromXml<T>(this string xml) {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            return FromXml<T>(stream);
        }

        private static T FromXml<T>(Stream stream) {
            XmlReader reader = XmlReader.Create(stream);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T obj = (T)serializer.Deserialize(reader);
            reader.Close();
            stream.Close();
            return obj;
        }

        /// <summary>
        /// Serializes an object and write the binary to disk.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="fileMode"></param>
        public static void WriteToBinary(this string path, object obj, FileMode fileMode = FileMode.Create) {
            FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None);
            ToBinary(obj, stream);
        }

        /// <summary>
        /// Serializes an object to binary data.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToBinary(this object obj) {
            MemoryStream stream = new MemoryStream();
            ToBinary(obj, stream);
            byte[] buffer = stream.GetBuffer();
            return buffer;
        }

        private static void ToBinary(object obj, Stream stream) {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        /// <summary>
        /// Deserializes an object from the binary data read from disk.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="fileMode"></param>
        /// <returns></returns>
        public static T ReadFromBinary<T>(this string path, FileMode fileMode = FileMode.Open) {
            FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read);
            return FromBinary<T>(stream);
        }

        /// <summary>
        /// Deserializes an object from binary data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static T FromBinary<T>(this byte[] binary) {
            MemoryStream stream = new MemoryStream(binary);
            return FromBinary<T>(stream);
        }

        private static T FromBinary<T>(Stream stream) {
            BinaryFormatter formatter = new BinaryFormatter();
            T obj = (T)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }
    }
}