using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace PureLib.Common {
    public static class DiskHelper {
        public static void WriteBinary(this string path, byte[] bytes, FileMode fileMode = FileMode.Create) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None)) {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(bytes);
                writer.Flush();
            }
        }

        public static byte[] ReadBinary(this string path, FileMode fileMode = FileMode.Open) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read)) {
                BinaryReader reader = new BinaryReader(stream);
                byte[] bytes = new byte[stream.Length];
                reader.Read(bytes, 0, bytes.Length);
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

        public static string ReadText(this string path, Encoding enc = null, FileMode fileMode = FileMode.Open) {
            if (enc == null)
                enc = Encoding.Default;

            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read)) {
                StreamReader reader = new StreamReader(stream, enc, true);
                return reader.ReadToEnd();
            }
        }
    }
}