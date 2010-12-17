using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace PureLib.Common {
    /// <summary>
    /// Provides methods to serialize and deserialize objects to XML, JSON or binary.
    /// </summary>
    public static class Serializer {
        /// <summary>
        /// Serializes an object to JSON string.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, bool indent = true) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(obj);
            if (indent) {
                JsonFormatter formatter = new JsonFormatter();
                return formatter.FormatJson(json);
            }
            else
                return json;
        }

        /// <summary>
        /// Deserializes an object from JSON string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }

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

    enum JsonContextType {
        Object, Array
    }
    class JsonFormatter {
        private const int DefaultIndent = 0;
        private const string Space = " ";
        private const string Indent = Space + Space + Space + Space;
        private readonly string NewLine = Environment.NewLine;

        private bool inDoubleString = false;
        private bool inSingleString = false;
        private bool inVariableAssignment = false;
        private char prevChar = '\0';
        private Stack<JsonContextType> context = new Stack<JsonContextType>();

        private void BuildIndents(int indents, StringBuilder output) {
            indents += DefaultIndent;
            for (; indents > 0; indents--)
                output.Append(Indent);
        }

        private bool InString() {
            return inDoubleString || inSingleString;
        }

        public string FormatJson(string jsonString) {
            StringBuilder input = new StringBuilder(jsonString);
            StringBuilder output = new StringBuilder();
            int inputLength = input.Length;
            char c;

            for (int i = 0; i < inputLength; i++) {
                c = input[i];
                switch (c) {
                    case '{':
                        if (!InString()) {
                            if (inVariableAssignment || (context.Count > 0 && context.Peek() != JsonContextType.Array)) {
                                output.Append(NewLine);
                                BuildIndents(context.Count, output);
                            }
                            output.Append(c);
                            context.Push(JsonContextType.Object);
                            output.Append(NewLine);
                            BuildIndents(context.Count, output);
                        }
                        else
                            output.Append(c);
                        break;
                    case '}':
                        if (!InString()) {
                            output.Append(NewLine);
                            context.Pop();
                            BuildIndents(context.Count, output);
                            output.Append(c);
                        }
                        else
                            output.Append(c);
                        break;
                    case '[':
                        output.Append(c);
                        if (!InString())
                            context.Push(JsonContextType.Array);
                        break;
                    case ']':
                        if (!InString()) {
                            output.Append(c);
                            context.Pop();
                        }
                        else
                            output.Append(c);
                        break;
                    case '=':
                        output.Append(c);
                        break;
                    case ',':
                        output.Append(c);
                        if (!InString() && context.Peek() != JsonContextType.Array) {
                            output.Append(NewLine);
                            BuildIndents(context.Count, output);
                            inVariableAssignment = false;
                        }
                        break;
                    case '\'':
                        if (!inDoubleString && prevChar != '\\')
                            inSingleString = !inSingleString;
                        output.Append(c);
                        break;
                    case ':':
                        if (!InString()) {
                            inVariableAssignment = true;
                            output.Append(Space);
                            output.Append(c);
                            output.Append(Space);
                        }
                        else
                            output.Append(c);
                        break;
                    case '"':
                        if (!inSingleString && prevChar != '\\')
                            inDoubleString = !inDoubleString;
                        output.Append(c);
                        break;
                    default:
                        output.Append(c);
                        break;
                }
                prevChar = c;
            }
            return output.ToString();
        }
    }
}