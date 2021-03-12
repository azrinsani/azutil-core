using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Azrin
{
    public static class AzExtensions
    {
        public static int? ToIntNull(this string s)
        {
            if (int.TryParse(s, out int res)) return res; else return null;
        }

        public static string ToFirstRegexMatch(this string s, string pattern)
        {
            var m = Regex.Match(s, pattern, RegexOptions.IgnoreCase);
            if (m.Success) return m.Value;
            else return null;

        }
        public static string WhiteSpaceToNull(this string str)
        {
            return (string.IsNullOrWhiteSpace(str) ? null : str); 
        }
        public static string ToFirstRegexMatch(this string s, string pattern, out string leftRemainder, out string rightRemainder)
        {
            leftRemainder = null; rightRemainder = null;
            var m = Regex.Match(s, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
            {
                leftRemainder = s.Substring(0, m.Index);
                rightRemainder = s[(m.Index + m.Value.Length)..];
                return m.Value;
            }
            else
            {
                leftRemainder = s;
                rightRemainder = s;
                return null;
            }
        }
        public static bool IsEqualToEither(this string s, params string[] comps)
        {
            foreach (string comp in comps) { if (string.Equals(comp, s)) return true; }
            return false;
        }
        public static string Last(this string S)
        {
            if (S.Length == 0) return "";
            return S[^1..];
        }
        public static void AddIfExist(this List<string> Strings, string Entry)
        {
            if (!Strings.Exists(S => string.Equals(S, Entry, StringComparison.InvariantCultureIgnoreCase))) Strings.Add(Entry);
        }
        public static void AddIfNotExist(this List<string> Strings, string Entry, out bool AlreadyExists)
        {
            AlreadyExists = false;
            string entry = Entry.Trim();
            if (!Strings.Exists(S => string.Equals(S, entry, StringComparison.InvariantCultureIgnoreCase))) Strings.Add(entry.ToLower()); else AlreadyExists = true;
        }
        public static bool IfExist(this List<string> Strings, string Entry)
        {
            string entry = Entry.Trim();
            return Strings.Exists(S => string.Equals(S, entry, StringComparison.InvariantCultureIgnoreCase));
        }
        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) stringBuilder.Append(c);
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static string ToSha256(this string input)
        {
            StringBuilder Sb = new StringBuilder();

            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                foreach (Byte b in hash) Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();

        }
        public static bool ToBool(this object value, bool ValueIfError = false, bool? ValueIfNotAString = null)
        {
            try
            {
                if (value is string S)
                {
                    if (string.IsNullOrWhiteSpace(S)) return ValueIfError;
                    string SL = S.ToLower();
                    if (S == "0" || SL == "false" || SL == "off" || SL == "close" || SL == "no") return false;
                    if (S == "1" || SL == "true" || SL == "on" || SL == "open" || SL == "yes") return true;
                    return ValueIfError;
                }
                else
                {
                    if (ValueIfNotAString == null) return Convert.ToBoolean(value);
                    else return ValueIfNotAString ?? false;
                }
            }
            catch { return ValueIfError; }
        }

        public static DateTime? ToDateTimeNull(this object value, bool IfTimeZoneNotSpecifiedAssumeUTC = true, DateTime? valueIfFail = null)
        {
            try
            {
                if (value == null || value == DBNull.Value) return null; 
                if (value is string TSStr) return StringToDT(TSStr, IfTimeZoneNotSpecifiedAssumeUTC); else return Convert.ToDateTime(value);
            }
            catch { return valueIfFail; }
        }
        public static DateTime ToDateTime(this object value, bool IfTimeZoneNotSpecifiedAssumeUTC = true, DateTime? ValueIfFail = null)
        {
            try { if (value is string TSStr) return StringToDT(TSStr, IfTimeZoneNotSpecifiedAssumeUTC); else return Convert.ToDateTime(value); }
            catch { return ValueIfFail ?? DateTime.MinValue; }
        }
        private static DateTime StringToDT(string TSStr, bool IfTimeZoneNotSpecifiedAssumeUTC = true)
        {
            if (DateTime.TryParseExact(TSStr, new string[] { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-M-d H:m:s", "yyyy-M-d HH:m:s.f" },
                            System.Globalization.CultureInfo.InvariantCulture, IfTimeZoneNotSpecifiedAssumeUTC ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal, out DateTime DT))
            { if (IfTimeZoneNotSpecifiedAssumeUTC) DT = DT.ToUniversalTime(); }
            else DateTime.TryParse(TSStr, null, IfTimeZoneNotSpecifiedAssumeUTC? DateTimeStyles.AssumeUniversal : DateTimeStyles.RoundtripKind, out DT); //This handles ECMA ISO8601 Date Format
            return DT;
        }
        public static Single ToSingle(this object value)
        {
            try { return Convert.ToSingle(value); }
            catch { return (Single)0;}
        }
        public static Double ToDouble(this object value)
        {
            try { return Convert.ToDouble(value); }
            catch { return (Double)0; }
        }
        public static long ToLong(this object value, Int64 DefaultValueIfError = 0)
        {
            try { return Convert.ToInt64(value); }
            catch { return DefaultValueIfError;  }
        }
        public static Byte ToByte(this object value)
        {
            try { return Convert.ToByte(value); }
            catch { return (Byte)0; }
        }
        public static ushort ToUShort(this object value)
        {
            try { return Convert.ToUInt16(value); }
            catch { return (ushort)0; }
        }
        public static Char ToChar(this object value)
        {
            try { return Convert.ToChar(value); }
            catch { return ' '; }
        }
        public static int? ToIntNull(this object value)
        {
            if (value == null || value == DBNull.Value) return null;
            try { return Convert.ToInt32(value); }
            catch { return null; }
        }
        public static Int16 ToInt16(this object value, Int16 ValueIfError = 0)
        {
            try { return Convert.ToInt16(value); }
            catch { return ValueIfError; }
        }
        public static Int64 ToInt64(this object value, Int64 ValueIfError = 0)
        {
            try { return Convert.ToInt64(value); }
            catch { return ValueIfError;}
        }
        public static uint ToUint(this object value, uint ValueIfError = 0)
        {
            try { return Convert.ToUInt32(value); }
            catch { return ValueIfError; }
        }
        public static UInt16 ToUint16(this object value, UInt16 ValueIfError = 0)
        {
            try { return Convert.ToUInt16(value); }
            catch { return ValueIfError; }
        }
        public static UInt64 ToUint64(this object value, UInt64 ValueIfError = 0)
        {
            try { return Convert.ToUInt64(value); }
            catch { return ValueIfError; }
        }
        public static int ToInt(this object value, int ValueIfFail = 0)
        {
            if (value == null || value == DBNull.Value) return ValueIfFail;
            try { return Convert.ToInt32(value); }
            catch { return ValueIfFail; }
        }
        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this string value) { return string.IsNullOrEmpty(value);}
        [DebuggerStepThrough]
        public static bool IsNullOrWhiteSpace(this string value) { return string.IsNullOrWhiteSpace(value); }
        [DebuggerStepThrough]
        public static bool IsEmptyOrWhiteSpace(this string value) => value.All(char.IsWhiteSpace);
        [DebuggerStepThrough]
        public static string Right(this string value, int length) { return value[^length..]; }
    }

}
