using System;
using System.Buffers;
using System.Diagnostics;

namespace PureLib.Common {
    public static class Base64Url {
        public static string Encode(ReadOnlySpan<byte> input) {
            const int StackAllocThreshold = 128;

            if (input.IsEmpty)
                return string.Empty;

            int bufferSize = GetArraySizeRequiredToEncode(input.Length);

            char[] bufferToReturnToPool = null;
            Span<char> buffer = bufferSize <= StackAllocThreshold
                ? stackalloc char[StackAllocThreshold]
                : bufferToReturnToPool = ArrayPool<char>.Shared.Rent(bufferSize);

            int numBase64Chars = Encode(input, buffer);
            string base64Url = new(buffer[..numBase64Chars]);

            if (bufferToReturnToPool != null)
                ArrayPool<char>.Shared.Return(bufferToReturnToPool);

            return base64Url;
        }

        public static byte[] Decode(string input) {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length == 0)
                return Array.Empty<byte>();

            // Copy input into buffer, fixing up '-' -> '+' and '_' -> '/'.
            int padSize = GetPadSize(input.Length);
            char[] buffer = new char[GetArraySizeRequiredToDecode(input.Length, padSize)];
            int i = 0;
            for (int j = 0; i < input.Length; i++, j++) {
                char ch = input[j];
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

            return Convert.FromBase64CharArray(buffer, 0, buffer.Length);
        }

        private static int Encode(ReadOnlySpan<byte> input, Span<char> output) {
            Debug.Assert(output.Length >= GetArraySizeRequiredToEncode(input.Length));

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

        private static int GetArraySizeRequiredToEncode(int count) {
            int numWholeOrPartialInputBlocks = checked(count + 2) / 3;
            return checked(numWholeOrPartialInputBlocks * 4);
        }

        private static int GetArraySizeRequiredToDecode(int count, int padSize) {
            if (count == 0)
                return 0;
            return checked(count + padSize);
        }

        private static int GetPadSize(int count) {
            return (count % 4) switch {
                2 => 2,
                3 => 1,
                0 => 0,
                _ => throw new ArgumentException("Invalid string to decode.", nameof(count)),
            };
        }
    }
}
