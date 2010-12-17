using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Microsoft.VisualBasic;

namespace PureLib.Common {
    /// <summary>
    /// Providers methods about string conversion.
    /// </summary>
    public static class LanguageHelper {
        /// <summary>
        /// Resolves the unreadable codes cause by wrong decoding charset applied.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static string ResolveUnreadableCodes(this string text, Encoding src, Encoding dst) {
            return dst.GetString(src.GetBytes(text));
        }

        /// <summary>
        /// Converts the first letter of every word in the string to uppercase.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToProperCase(this string text) {
            return Strings.StrConv(text, VbStrConv.ProperCase);
        }

        /// <summary>
        /// Converts wide (double-byte) characters in the string to narrow (single-byte) characters. Applies to Asian locales.
        /// </summary>
        /// <param name="wide"></param>
        /// <returns></returns>
        public static string ToNarrow(this string wide) {
            return Strings.StrConv(wide, VbStrConv.Narrow);
        }

        /// <summary>
        /// Converts narrow (single-byte) characters in the string to wide (double-byte) characters. Applies to Asian locales.
        /// </summary>
        /// <param name="narrow"></param>
        /// <returns></returns>
        public static string ToWide(this string narrow) {
            return Strings.StrConv(narrow, VbStrConv.Wide);
        }

        /// <summary>
        /// Converts Hiragana characters in the string to Katakana characters.
        /// </summary>
        /// <param name="hiragana"></param>
        /// <returns></returns>
        public static string ToKatakana(this string hiragana) {
            return ConvertJapanese(hiragana, HiraganaToKatakanaMethodName);
        }

        /// <summary>
        /// Converts Katakana characters in the string to Hiragana characters.
        /// </summary>
        /// <param name="katakana"></param>
        /// <returns></returns>
        public static string ToHiragana(this string katakana) {
            return ConvertJapanese(katakana, KatakanaToHiraganaMethodName);
        }

        /// <summary>
        /// Parse Romaji to Hiragana characters in the string.
        /// </summary>
        /// <param name="romaji"></param>
        /// <returns></returns>
        public static string ParseRomaji(this string romaji) {
            return ConvertJapanese(romaji, RomajiToHiraganaMethodName);
        }

        private static string ConvertJapanese(string text, string methodName) {
            Assembly assembly = LoadAssembly(JapaneseConverterAssemblyPath);
            MethodInfo method = assembly.GetType(JapaneseConverterName).GetMethod(methodName);
            return method.Invoke(null, new object[] { text }) as string;
        }

        /// <summary>
        /// Converts the string to Simplified Chinese characters.
        /// </summary>
        /// <param name="traditional"></param>
        /// <returns></returns>
        public static string ToSimplifiedChinese(this string traditional) {
            return ConvertChinese(traditional, Direction.TraditionalToSimplified);
        }

        /// <summary>
        /// Converts the string to Traditional Chinese characters.
        /// </summary>
        /// <param name="simplified"></param>
        /// <returns></returns>
        public static string ToTraditionalChinese(this string simplified) {
            return ConvertChinese(simplified, Direction.SimplifiedToTraditional);
        }

        private static string ConvertChinese(string text, Direction direction) {
            Assembly assembly = LoadAssembly(ChineseConverterAssemblyPath);
            MethodInfo convert = assembly.GetType(ChineseConverterName).GetMethod(ChineseConvertMethodName);
            var d = Enum.GetValues(assembly.GetType(ChineseConversionDirection)).GetValue((int)direction);
            return convert.Invoke(null, new object[] { text, d }) as string;
        }

        private static Assembly LoadAssembly(string path) {
            Assembly assembly = null;
            try {
                assembly = Assembly.LoadFrom(path);
            } catch (FileNotFoundException) {
                throw new ApplicationException("{0} cannot be found.".FormatWith(path));
            }
            return assembly;
        }

        private const string ChineseConverterAssemblyPath = @"Lib\ChineseConverter.dll";
        private const string ChineseConverterName = "Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter.ChineseConverter";
        private const string ChineseConversionDirection = "Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter.ChineseConversionDirection";
        private const string ChineseConvertMethodName = "Convert";

        private const string JapaneseConverterAssemblyPath = @"Lib\JpnKanaConvHelper.dll";
        private const string JapaneseConverterName = "Microsoft.International.Converters.KanaConverter";
        private const string HiraganaToKatakanaMethodName = "HiraganaToKatakana";
        private const string KatakanaToHiraganaMethodName = "KatakanaToHiragana";
        private const string RomajiToHiraganaMethodName = "RomajiToHiragana";
    }

    enum Direction {
        SimplifiedToTraditional,
        TraditionalToSimplified
    }
}
