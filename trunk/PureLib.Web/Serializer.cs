using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace PureLib.Common {
    /// <summary>
    /// Provides methods to serialize and deserialize objects to JSON.
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