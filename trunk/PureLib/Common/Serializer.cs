using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PureLib.Common {
    public static class Serializer {
        public static void WriteToXml(this string path, object obj, bool indent = true, FileMode fileMode = FileMode.Create) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None)) {
                ToXml(obj, stream, indent);
            }
        }

        public static string ToXml(this object obj, bool indent = true) {
            byte[] buffer = null;
            using (MemoryStream stream = new MemoryStream()) {
                ToXml(obj, stream, indent);
                buffer = stream.GetBuffer();
            }
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }

        private static void ToXml(object obj, Stream stream, bool indent) {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings() { Indent = indent })) {
                serializer.Serialize(writer, obj);
            }
        }

        public static T ReadFromXml<T>(this string path, FileMode fileMode = FileMode.Open) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read)) {
                return FromXml<T>(stream);
            }
        }

        public static T FromXml<T>(this string xml) {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml))) {
                return FromXml<T>(stream);
            }
        }

        private static T FromXml<T>(Stream stream) {
            T obj;
            using (XmlReader reader = XmlReader.Create(stream)) {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                obj = (T)serializer.Deserialize(reader);
            }
            return obj;
        }

        public static void WriteToBinary(this string path, object obj, FileMode fileMode = FileMode.Create) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None)) {
                ToBinary(obj, stream);
            }
        }

        public static byte[] ToBinary(this object obj) {
            using (MemoryStream stream = new MemoryStream()) {
                ToBinary(obj, stream);
                return stream.GetBuffer();
            }
        }

        private static void ToBinary(object obj, Stream stream) {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }

        public static T ReadFromBinary<T>(this string path, FileMode fileMode = FileMode.Open) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read)) {
                return FromBinary<T>(stream);
            }
        }

        public static T FromBinary<T>(this byte[] binary) {
            using (MemoryStream stream = new MemoryStream(binary)) {
                return FromBinary<T>(stream);
            }
        }

        private static T FromBinary<T>(Stream stream) {
            BinaryFormatter formatter = new BinaryFormatter();
            T obj = (T)formatter.Deserialize(stream);
            return obj;
        }
    }
}