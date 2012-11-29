using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.International.Converters;
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

        public static string ToSimplifiedChinese(this string traditional) {
            return Strings.StrConv(traditional, VbStrConv.SimplifiedChinese);
        }

        public static string ToTraditionalChinese(this string simplified) {
            return Strings.StrConv(simplified, VbStrConv.TraditionalChinese);
        }

        public static string ToKatakana(this string hiragana) {
            return KanaConverter.HiraganaToKatakana(hiragana);
        }

        public static string ToHiragana(this string katakana) {
            return KanaConverter.KatakanaToHiragana(katakana);
        }

        public static string ParseRomaji(this string romaji) {
            return KanaConverter.RomajiToHiragana(romaji);
        }
    }
}
