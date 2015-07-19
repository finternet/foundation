using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web.Script.Serialization;

namespace FI.Foundation.Extensions
{
    public static class StringExtensions
    {
        public static Color ToColor(this string hexcode)
        {
            if (string.IsNullOrWhiteSpace(hexcode) || hexcode.Equals("transparent", StringComparison.OrdinalIgnoreCase))
            {
                return Color.Transparent;
            }
            string c = hexcode;
            if (c.Length == 3)
            {
                c = string.Empty;
                foreach (var ch in hexcode.ToCharArray())
                {
                    c += ch;
                    c += ch;
                }
            }

            c = "FF" + c;

            int colorCode = -1;
            if (int.TryParse(c, System.Globalization.NumberStyles.HexNumber, new CultureInfo("en-US"), out colorCode))
            {
                return Color.FromArgb(colorCode);
            }
            return Color.Transparent;
        }

        public static string GenerateSlug(this string phrase, int maxLength = 0)
        {
            string str = phrase.ToLower();

            // extension: replace some characters such as &,%,$ with its equal like 'And', 'Percent', 'Dollar'
            str = str.Replace("&amp;", "and").Replace("&", "and");

            str = Regex.Replace(str, @"[^\w\s-]", ""); // remove invalid characters
            str = Regex.Replace(str, @"\s+", " ").Trim(); // remove double space
            str = Regex.Replace(str, @"\s", "-"); // replace space with hypen
            str = Regex.Replace(str, @"-+", "-"); // replace more than 1 hypen with a heypen

            if (maxLength > 1 && str.Length > maxLength)
            {
                str = str.Substring(0, maxLength);
                if (str.EndsWith("-"))
                    str = str.Substring(0, maxLength - 1);
            }
            return str;
        }

        public static string PrepareForLikeClause(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return null;
            string str = phrase.ToLower();

            str = Regex.Replace(str, @"[^\w\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim(); // remove double space
            str = Regex.Replace(str, @"\s", "%"); // replace space with hypen
            str = Regex.Replace(str, @"%+", "%"); // replace more than 1 hypen with a heypen
            if (str.Length > 1)
            {
                return string.Concat("%", str, "%");
            }
            return null;
        }

        public static int? ToInt32Nullable(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return null;
            int result = 0;
            if (!int.TryParse(phrase, out result)) return null;
            return result;
        }

        public static int ToInt32(this string phrase)
        {
            return ToInt32Nullable(phrase) ?? 0;
        }

        public static long? ToLongNullable(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return null;
            long result = 0;
            if (!long.TryParse(phrase, out result)) return null;
            return result;
        }

        public static double? ToDoubleNullable(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return null;
            double result = 0;
            if (!double.TryParse(phrase, out result)) return null;
            return result;
        }

        public static double ToDouble(this string phrase)
        {
            return ToDoubleNullable(phrase) ?? 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long")]
        public static long ToLong(this string phrase)
        {
            return ToLongNullable(phrase) ?? 0;
        }

        public static bool? ToBooleanNullable(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return null;
            bool result = false;
            if (!bool.TryParse(phrase, out result)) return null;
            return result;
        }

        public static bool ToBoolean(this string phrase)
        {
            return ToBooleanNullable(phrase) ?? false;
        }

        public static Guid? ToGuidNullable(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return null;
            Guid result = Guid.Empty;
            if (!Guid.TryParse(phrase, out result)) return null;
            return result;
        }

        public static Guid ToGuid(this string phrase)
        {
            return ToGuidNullable(phrase) ?? Guid.Empty;
        }
        public static DateTime ParseDate(this string phrase, string format)
        {
            DateTime result = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(phrase)) return result;
            DateTime.TryParseExact(phrase, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            return result;
        }
        public static DateTime? ToDateTimeNullable(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return null;

            // guess culture
            var ci = new CultureInfo("en-GB");
            if (phrase.EndsWith("pm", StringComparison.OrdinalIgnoreCase) || phrase.EndsWith("am", StringComparison.OrdinalIgnoreCase))
            {
                ci = new CultureInfo("en-US");
            }

            try
            {
                // added support for double value
                double result;
                if (double.TryParse(phrase, out result))
                {
                    return DateTime.FromOADate(result);
                }
                else
                {
                    return Convert.ToDateTime(phrase, ci);
                }
            }
            catch (FormatException)
            {
            }

            return null;
        }
        public static DateTime ToDateTime(this string phrase)
        {
            return ToDateTimeNullable(phrase) ?? DateTime.MinValue;
        }

        public static object ToType(this string phrase, Type type)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return null;

            if (type == typeof(int))
            {
                return phrase.ToInt32();
            }
            if (type == typeof(long))
            {
                return phrase.ToLong();
            }
            if (type == typeof(string))
            {
                return phrase;
            }
            if (type == typeof(bool))
            {
                return phrase.ToBoolean();
            }
            if (type == typeof(Guid))
            {
                return phrase.ToGuid();
            }
            if (type == typeof(DateTime))
            {
                return phrase.ToDateTime();
            }
            return phrase;
        }
        public static object ToType(this string phrase, string typeName)
        {
            typeName = typeName.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(phrase))
                return null;
            Type t = typeof(string);

            if (typeName == "int" || typeName == "int32" || typeName == "sqlint32")
            {
                t = typeof(int);
            }
            else if (typeName == "long" || typeName == "sqlint64" || typeName == "int64")
            {
                t = typeof(long);
            }
            else if (typeName == "string" || typeName == "sqlstring")
            {
                t = typeof(string);
            }
            else if (typeName == "bool" || typeName == "sqlbit")
            {
                t = typeof(bool);
            }
            else if (typeName == "guid" || typeName == "sqlguid")
            {
                t = typeof(Guid);
            }
            else if (typeName == "DateTime" || typeName == "sqldatetime")
            {
                t = typeof(DateTime);
            }
            return ToType(phrase, t);
        }

        public static char? ToCharNullable(this string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return null;
            if (phrase.Trim().Length == 0) return null;
            char result;
            if (!char.TryParse(phrase, out result)) return null;
            return result;
        }

        public static string ToHTMLParagraphs(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return null;
            return string.Concat("<p>", phrase.Replace("\n", "</p><p>").Replace("\r", string.Empty), "</p>");
        }

        public static string Limit(this string phrase, int length)
        {
            if (string.IsNullOrEmpty(phrase)) return string.Empty;
            string result = phrase.Trim();
            if (phrase.Length > length)
            {
                result = phrase.Substring(0, length);
                while (result.Substring(result.Length - 1, 1) != " " && result.Length > 0 && result.Contains(" "))
                {
                    result = result.Substring(0, result.Length - 1);
                }
                result = result.Trim() + "...";
            }
            return result;
        }

        public static string StripHtmlXmlTags(this string content)
        {
            if (string.IsNullOrEmpty(content)) return content;

            return Regex.Replace(Regex.Replace(content, @"((<[\s\/]*script\b[^>]*>)([^>]*)(<\/script>))", "", RegexOptions.IgnoreCase | RegexOptions.Compiled), "<[^>]+>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static string StripNonAlphaNumericTags(this string content)
        {
            if (string.IsNullOrEmpty(content)) return content;
            return Regex.Replace(content, "[^\\w]", " ", RegexOptions.Compiled | RegexOptions.Multiline);
        }

        public static string StripExtraSpaces(this string content)
        {
            if (string.IsNullOrEmpty(content)) return content;
            return Regex.Replace(content, " {1,}", " ", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        }

        /// <summary>
        /// Parses this string and returns equivalent dynamic object, always check for null values :)
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public static dynamic ParseJSON(this string phrase)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new FI.Foundation.Dynamic.DynamicJsonConverter() });
            dynamic obj = jss.Deserialize(phrase, typeof(object));
            return obj;
        }

        public static bool HasArabicGlyphs(this string text)
        {
            char[] glyphs = text.ToCharArray();
            foreach (char glyph in glyphs)
            {
                if (glyph >= 0x600 && glyph <= 0x6ff) return true;
                if (glyph >= 0x750 && glyph <= 0x77f) return true;
                if (glyph >= 0xfb50 && glyph <= 0xfc3f) return true;
                if (glyph >= 0xfe70 && glyph <= 0xfefc) return true;
            }
            return false;
    	}

        public static bool HasUnicodeCharacters(this string text)
        {
            return text.Any(c => c > 255);
        }

        public static string TransformMobile(this string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return string.Empty;

            mobile = mobile.StripNonAlphaNumericTags();
            if ((mobile.Length == 10 || mobile.Length == 9) && mobile.StartsWith("0"))
            {
                mobile = string.Concat("971", mobile.Substring(1));
            }
            else if (mobile.Length == 9)
            {
                mobile = string.Concat("971", mobile);
            }
            else if (mobile.Length == 14 && mobile.StartsWith("00"))
            {
                mobile = mobile.Substring(2);
            }
            
            return mobile;
        }
    }
}
