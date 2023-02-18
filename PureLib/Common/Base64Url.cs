using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PureLib.Common {
    public static class Base64Url {
        [SkipLocalsInit]
        public static string Encode(ReadOnlySpan<byte> input) {
            if (input.IsEmpty)
                return string.Empty;

            int bufferSize = GetBase64StringSize(input.Length);
            char[] bufferToReturn = null;
            Span<char> buffer = bufferSize <= Constants.StackAllocThresholdOfChars
                ? stackalloc char[Constants.StackAllocThresholdOfChars]
                : bufferToReturn = ArrayPool<char>.Shared.Rent(bufferSize);

            int actualSize = Encode(input, buffer);
            string base64Url = new(buffer[..actualSize]);

            if (bufferToReturn != null)
                ArrayPool<char>.Shared.Return(bufferToReturn);

            return base64Url;
        }

        [SkipLocalsInit]
        public static byte[] Decode(string input) {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length == 0)
                return Array.Empty<byte>();

            int padSize = GetPadSize(input.Length);
            int bufferSize = GetBase64StringSize(input.Length, padSize);
            char[] bufferToReturn = null;
            Span<char> buffer = bufferSize <= Constants.StackAllocThresholdOfChars
                ? stackalloc char[Constants.StackAllocThresholdOfChars]
                : bufferToReturn = ArrayPool<char>.Shared.Rent(bufferSize);

            // Fix up '-' -> '+' and '_' -> '/'.
            int i = 0;
            for (; i < input.Length; i++) {
                char ch = input[i];
                if (ch == '-')
                    buffer[i] = '+';
                else if (ch == '_')
                    buffer[i] = '/';
                else
                    buffer[i] = ch;
            }
            for (; padSize > 0; i++, padSize--) {
                buffer[i] = '=';
            }

            Span<char> chars = buffer[..bufferSize];
            int resultLength = ComputeDecodeResultLength(chars);
            byte[] result = new byte[resultLength];
            Convert.TryFromBase64Chars(chars, result, out _);

            if (bufferToReturn != null)
                ArrayPool<char>.Shared.Return(bufferToReturn);

            return result;
        }

        private static int Encode(ReadOnlySpan<byte> input, Span<char> output) {
            Debug.Assert(output.Length >= GetBase64StringSize(input.Length));

            if (input.IsEmpty)
                return 0;

            Convert.TryToBase64Chars(input, output, out int charsWritten);

            // Fix up '+' -> '-' and '/' -> '_'. Drop padding characters.
            for (int i = 0; i < charsWritten; i++) {
                char ch = output[i];
                if (ch == '+')
                    output[i] = '-';
                else if (ch == '/')
                    output[i] = '_';
                else if (ch == '=')
                    return i; // We've reached a padding character; truncate the remainder.
            }

            return charsWritten;
        }

        private static int GetBase64StringSize(int byteCount) {
            int numWholeOrPartialInputBlocks = checked(byteCount + 2) / 3;
            return checked(numWholeOrPartialInputBlocks * 4);
        }

        private static int GetBase64StringSize(int base64UrlLength, int padSize) {
            if (base64UrlLength == 0)
                return 0;
            return checked(base64UrlLength + padSize);
        }

        private static int GetPadSize(int base64UrlLength) {
            return (base64UrlLength % 4) switch {
                2 => 2,
                3 => 1,
                0 => 0,
                _ => throw new FormatException("Base64Url string is invalid."),
            };
        }

        private static int ComputeDecodeResultLength(Span<char> chars) {
            int num = chars.Length;
            int num2 = 0;
            for (int i = 0; i < chars.Length; i++) {
                switch ((int)chars[i]) {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 32:
                        num--;
                        break;
                    case 61:
                        num--;
                        num2++;
                        break;
                }
            }
            switch (num2) {
                case 1:
                    num2 = 2;
                    break;
                case 2:
                    num2 = 1;
                    break;
                case 0:
                    break;
                default:
                    throw new FormatException("Base64 string is invalid.");
            }
            return num / 4 * 3 + num2;
        }
    }
}
