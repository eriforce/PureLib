using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace PureLib.Common {
    /// <summary>
    /// Provides methods to IO disk.
    /// </summary>
    public static class DiskHelper {
        /// <summary>
        /// Writes binary data to disk.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        /// <param name="fileMode"></param>
        public static void WriteBinary(this string path, byte[] bytes, FileMode fileMode = FileMode.Create) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None)) {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(bytes);
            }
        }

        /// <summary>
        /// Reads binary data from disk.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileMode"></param>
        /// <returns></returns>
        public static byte[] ReadBinary(this string path, FileMode fileMode = FileMode.Open) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Read, FileShare.Read)) {
                BinaryReader reader = new BinaryReader(stream);
                byte[] bytes = new byte[stream.Length];
                reader.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        /// <summary>
        /// Writes text to disk.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        /// <param name="enc"></param>
        /// <param name="fileMode"></param>
        public static void WriteText(this string path, string text, Encoding enc, FileMode fileMode = FileMode.Create) {
            using (FileStream stream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None)) {
                StreamWriter writer = new StreamWriter(stream, enc);
                writer.Write(text);
            }
        }

        /// <summary>
        /// Reads text from disk. Detects the encoding by looking at the first three bytes of the stream. It automatically recognizes UTF-8, little-endian Unicode, and big-endian Unicode text if the file starts with the appropriate byte order marks. Otherwise, the user-provided encoding is used.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="enc">If enc is null, then Encoding.Default will be used.</param>
        /// <param name="fileMode"></param>
        /// <returns></returns>
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