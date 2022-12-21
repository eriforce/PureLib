﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PureLib.Common {
    public sealed class JsonFormatter {
        private const int defaultIndent = 0;
        private const char space = ' ';
        private readonly string _indent;

        private Stack<JsonContextType> _context = new Stack<JsonContextType>();
        private bool _isInDoubleString = false;
        private bool _isInSingleString = false;
        private bool _isInVariableAssignment = false;
        private bool _isInString {
            get {
                return _isInDoubleString || _isInSingleString;
            }
        }

        public JsonFormatter(int indentSize = 4) {
            _indent = new string(space, indentSize);
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
                        if (!_isInString) {
                            if (_isInVariableAssignment || (_context.Count > 0 && _context.Peek() != JsonContextType.Array)) {
                                output.Append(Environment.NewLine);
                                BuildIndents(_context.Count, output);
                            }
                            output.Append(c);
                            _context.Push(JsonContextType.Object);
                            output.Append(Environment.NewLine);
                            BuildIndents(_context.Count, output);
                        }
                        else
                            output.Append(c);
                        break;
                    case '}':
                        if (!_isInString) {
                            output.Append(Environment.NewLine);
                            _context.Pop();
                            BuildIndents(_context.Count, output);
                            output.Append(c);
                        }
                        else
                            output.Append(c);
                        break;
                    case '[':
                        output.Append(c);
                        if (!_isInString)
                            _context.Push(JsonContextType.Array);
                        break;
                    case ']':
                        if (!_isInString) {
                            output.Append(c);
                            _context.Pop();
                        }
                        else
                            output.Append(c);
                        break;
                    case '=':
                        output.Append(c);
                        break;
                    case ',':
                        output.Append(c);
                        if (!_isInString && (_context.Peek() != JsonContextType.Array)) {
                            output.Append(Environment.NewLine);
                            BuildIndents(_context.Count, output);
                            _isInVariableAssignment = false;
                        }
                        break;
                    case '\'':
                        if (!_isInDoubleString && !IsQuoteInString(input, i))
                            _isInSingleString = !_isInSingleString;
                        output.Append(c);
                        break;
                    case ':':
                        if (!_isInString) {
                            _isInVariableAssignment = true;
                            output.Append(space);
                            output.Append(c);
                            output.Append(space);
                        }
                        else
                            output.Append(c);
                        break;
                    case '"':
                        if (!_isInSingleString && !IsQuoteInString(input, i))
                            _isInDoubleString = !_isInDoubleString;
                        output.Append(c);
                        break;
                    default:
                        output.Append(c);
                        break;
                }
            }
            return output.ToString();
        }

        private void BuildIndents(int indents, StringBuilder output) {
            indents += defaultIndent;
            for (; indents > 0; indents--)
                output.Append(_indent);
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
