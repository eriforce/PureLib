using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Microsoft.VisualBasic;

namespace PureLib.Common {
    public static class LanguageHelper {
        public static string ResolveUnreadableCodes(this string text, Encoding src, Encoding dst) {
            return dst.GetString(src.GetBytes(text));
        }

        public static string ToProperCase(this string text) {
            return Strings.StrConv(text, VbStrConv.ProperCase);
        }

        public static string ToNarrow(this string wide) {
            return Strings.StrConv(wide, VbStrConv.Narrow);
        }

        public static string ToWide(this string narrow) {
            return Strings.StrConv(narrow, VbStrConv.Wide);
        }

        public static string ToKatakana(this string hiragana) {
            return ConvertJapanese(hiragana, HiraganaToKatakanaMethodName);
        }

        public static string ToHiragana(this string katakana) {
            return ConvertJapanese(katakana, KatakanaToHiraganaMethodName);
        }

        public static string ParseRomaji(this string romaji) {
            return ConvertJapanese(romaji, RomajiToHiraganaMethodName);
        }

        private static string ConvertJapanese(string text, string methodName) {
            Assembly assembly = LoadAssembly(JapaneseConverterAssemblyPath);
            MethodInfo method = assembly.GetType(JapaneseConverterName).GetMethod(methodName);
            return method.Invoke(null, new object[] { text }) as string;
        }

        public static string ToSimplifiedChinese(this string traditional) {
            return ConvertChinese(traditional, Direction.TraditionalToSimplified);
        }

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
