using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace PureLib.Common {
    public static class DiskHelper {
        public static void WriteBinary(this string path, byte[] bytes, FileMode fileMode = FileMode.Create) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None)) {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }

        public static async Task WriteBinaryAsync(this string path, byte[] bytes, FileMode fileMode = FileMode.Create) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None)) {
                await stream.WriteAsync(bytes, 0, bytes.Length);
                await stream.FlushAsync();
            }
        }

        public static byte[] ReadBinary(this string path, FileMode fileMode = FileMode.Open) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read)) {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public static async Task<byte[]> ReadBinaryAsync(this string path, FileMode fileMode = FileMode.Open) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read)) {
                byte[] bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public static void WriteText(this string path, string text, Encoding enc, FileMode fileMode = FileMode.Create) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None)) {
                StreamWriter writer = new StreamWriter(stream, enc);
                writer.Write(text);
                writer.Flush();
            }
        }

        public static async Task WriteTextAsync(this string path, string text, Encoding enc, FileMode fileMode = FileMode.Create) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None)) {
                StreamWriter writer = new StreamWriter(stream, enc);
                await writer.WriteAsync(text);
                await writer.FlushAsync();
            }
        }

        public static string ReadText(this string path, Encoding enc = null, FileMode fileMode = FileMode.Open) {
            if (enc == null)
                enc = Encoding.Default;

            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read)) {
                StreamReader reader = new StreamReader(stream, enc, true);
                return reader.ReadToEnd();
            }
        }

        public static async Task<string> ReadTextAsync(this string path, Encoding enc = null, FileMode fileMode = FileMode.Open) {
            if (enc == null)
                enc = Encoding.Default;

            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read)) {
                StreamReader reader = new StreamReader(stream, enc, true);
                return await reader.ReadToEndAsync();
            }
        }
    }
}