using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PureLib.Common {
    public class JsonFormatter {
        private const int defaultIndent = 0;
        private const string space = " ";
        private const string indent = space + space + space + space;

        private bool isInDoubleString = false;
        private bool isInSingleString = false;
        private bool isInVariableAssignment = false;
        private Stack<JsonContextType> context = new Stack<JsonContextType>();

        private bool isInString {
            get {
                return isInDoubleString || isInSingleString;
            }
        }

        private void BuildIndents(int indents, StringBuilder output) {
            indents += defaultIndent;
            for (; indents > 0; indents--)
                output.Append(indent);
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
                        if (!isInString) {
                            if (isInVariableAssignment || (context.Count > 0 && context.Peek() != JsonContextType.Array)) {
                                output.Append(Environment.NewLine);
                                BuildIndents(context.Count, output);
                            }
                            output.Append(c);
                            context.Push(JsonContextType.Object);
                            output.Append(Environment.NewLine);
                            BuildIndents(context.Count, output);
                        }
                        else
                            output.Append(c);
                        break;
                    case '}':
                        if (!isInString) {
                            output.Append(Environment.NewLine);
                            context.Pop();
                            BuildIndents(context.Count, output);
                            output.Append(c);
                        }
                        else
                            output.Append(c);
                        break;
                    case '[':
                        output.Append(c);
                        if (!isInString)
                            context.Push(JsonContextType.Array);
                        break;
                    case ']':
                        if (!isInString) {
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
                        if (!isInString && context.Peek() != JsonContextType.Array) {
                            output.Append(Environment.NewLine);
                            BuildIndents(context.Count, output);
                            isInVariableAssignment = false;
                        }
                        break;
                    case '\'':
                        if (!isInDoubleString && !IsQuoteInString(input, i))
                            isInSingleString = !isInSingleString;
                        output.Append(c);
                        break;
                    case ':':
                        if (!isInString) {
                            isInVariableAssignment = true;
                            output.Append(space);
                            output.Append(c);
                            output.Append(space);
                        }
                        else
                            output.Append(c);
                        break;
                    case '"':
                        if (!isInSingleString && !IsQuoteInString(input, i))
                            isInDoubleString = !isInDoubleString;
                        output.Append(c);
                        break;
                    default:
                        output.Append(c);
                        break;
                }
            }
            return output.ToString();
        }

        private bool IsQuoteInString(StringBuilder input, int index) {
            if (index <= 0)
                return false;

            bool isInString = false;
            while (index > 0) {
                index--;
                if (input[index] == '\\')
                    isInString = !isInString;
                else
                    return isInString;
            }
            return isInString;
        }
    }

    internal enum JsonContextType {
        Object, Array
    }
}
