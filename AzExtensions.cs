using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Buffers;
using System.Collections.ObjectModel;

namespace AzUtil.Core
{
    public static class AzExtensions
    {
       
        public static bool HasDateOnly(this DateTime dateTime) => dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0 && dateTime.Millisecond == 0;
        public static string TrimEnd(this string src, string StringToTrim)
        {
            if (src != null && src.EndsWith(StringToTrim))
            {
                src = src.Substring(0, src.Length - StringToTrim.Length);
            }
            return src;
        }
        public static List<int> FindIndexes<T>(this IList<T> source, Func<T, bool> predicate)
        {
            List<int> res = new List<int>();
            for (int n=0;n<source.Count;n++)
            {
                if (predicate.Invoke(source[n])) res.Add(n);
            }
            return res;

        }
        //public static ICollection<T> ReplaceCollection<T>(this ICollection<T> src, IEnumerable<T> toCopy, Func<T,T> initializationAction = null)
        public static T1 ReplaceCollection<T1,T2>(this T1 src, IEnumerable<T2> toCopy, Func<T2,T2> initializationAction = null) where T1 : ICollection<T2>
        {
            src.Clear();
            if (initializationAction != null)
            {
                foreach (var item in toCopy)
                {
                    var newItem = initializationAction.Invoke(item);
                    src.Add(newItem);
                }
            }
            else   
            {
                foreach (var item in toCopy) src.Add(item);
            }
            return src;
        }
        public static TVal GetValueOrDefault<TKey,TVal>(this Dictionary<TKey,TVal> dict, TKey key)
        {
            if (dict.TryGetValue(key, out TVal val))
            {
                return val;
            }
            else return default;

        }
        public static Color ToColor(this string colorHex, Color colorIfEmpty)
        {
            if (colorHex.IsNullOrEmpty()) return colorIfEmpty;
            else return Color.FromHex(colorHex);
        }
        public static T FirstOrDefaultWithIndex<T>(this IList<T> source, Func<T, bool> predicate, out int index)
        {
            index = -1;
            var res = source.FirstOrDefault(predicate);
            if (res != null) index = source.IndexOf(res);
            return res;
        }
        public static string ToMySqlTimeStamp(this DateTime Value)
        {
            //return Value.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return Value.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //return Value.Year + "-" + Value.Month + '-' + Value.Day + " " + Value.Hour + ":" + Value.Minute + ":" + Value.Second + "." + Value.Millisecond;
        }

        public static void AddRepeat<T>(this IList<T> list, Func<T> contructor, int numberOfTimes)
        {
            for (int n = 0; n < numberOfTimes; n++)
            {
                T newItem = contructor.Invoke();
                list.Add(newItem);
            }
        }
        
        public static bool IsEither(this IComparable source, params IComparable[] items)
        {
            foreach (var item in items)
            {
                if (source == item) return true;
            }
            return false;
        }
        public static string RegexRemoveFirst(this string source, Regex regex, out string removed)
        {            
            var match = regex.Match(source);
            removed = null;
            if (match.Success)
            {
                source = source.SubStringRemove(match.Index, match.Length);
                removed = match.Value;
            }
            return source;
        }
        //public static string DeleteSubstring(this string source, int start, int length)
        //{
        //    string front = source.Substring(0, start + 1);
        //    string end = source[(start + length)..];
        //    return front + end;
        //}

        public static string SubStringReplace(this string source, int startIndex, int length, string replaceWith)
        {
            if (source == null) return null;
            if (startIndex >= source.Length) return source;

            string strOut = source.Substring(0, startIndex);
            strOut += replaceWith;
            strOut += source[(startIndex + length)..];
            return strOut;
        }
        public static string SubStringRemove(this string source, int startIndex, int length)
        {
            if (source == null) return null;
            if (startIndex >= source.Length) return source;

            string strOut = source.Substring(0, startIndex);
            strOut += source[(startIndex + length)..];
            return strOut;
        }
        public static T[] AppendRange<T>(this T[] source, IEnumerable<T> additions)
        {
            var listArr = source.ToList();
            foreach (var addition in additions)
            {
                listArr.Add(addition);
            }
            source = listArr.ToArray();
            return source;
        }
        public static string RemoveLastCharacter(this string source)
        {
            if (source == null) return source;
            return source.Remove(source.Length - 1);
        }
        public static string RemoveLastCRLNIfExist(this string source)
        {
            if (source.Length <= 2) return source;
            if (source[^2..] == "\r\n") return source.Remove(source.Length - 2);
            else return source;
        }
        public static string RegexRemove(this string source, Regex regex, out Collection<string> removed)
        {
            MatchCollection matches = regex.Matches(source);
            removed = new Collection<string>();
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    removed.Add(match.Value);
                }
            }
            return regex.Replace(source, "");
        }
      
        public static string ToShorterString(this string source, int length)
        {
            if (source.Length > length) return source.Substring(0, length) + "..";
            else return source;
        }
        public static Collection<T> ToCollection<T>(this IEnumerable<T> source)
        {
            var collection = new Collection<T>();
            foreach (var e in source)
            {
                collection.Add(e);
            }
            return collection;
        }
        public static byte[] ToByteArray(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }


        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action.Invoke(item);
            }
        }
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T,int> action)
        {
            int n = 0;
            foreach (var item in collection)
            {
                action.Invoke(item,n);
                n++;
            }
        }
      
        public static string ConcatString(this IEnumerable<string> strs, string separator = null)
        {
            string combinedStr = "";
            int strCount = strs.Count();
            if (strCount == 0) return "";
            
            strs.ForEach((e, n) => 
            {
                if (n == strCount - 1) combinedStr += e; else combinedStr += e + separator; 
            });
            return combinedStr;
        }
        public static string ToUpperEveryWord(this string s)
        {
            if (s.IsNullOrWhiteSpace()) return s;
            var str = s.Split(" ");
            var res = str.Select(e => e.ToUpperFirstLetter()).ConcatString(" ");
            return res;
        }
        public static string ToUpperFirstLetter(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return char.ToUpper(s[0]) + s[1..];
        }
        public static V ValueOrDefault<K,V>(this Dictionary<K,V> dict,K key)
        {
            if (dict.TryGetValue(key, out V value)) return value;
            else return default;
        }


        public static byte[] ExtractResource(this System.Reflection.Assembly a, String filename)
        {
            using Stream resFilestream = a.GetManifestResourceStream(filename);
            if (resFilestream == null) return null;
            byte[] ba = new byte[resFilestream.Length];
            resFilestream.Read(ba, 0, ba.Length);
            return ba;
        }

        public static bool IsValidHexColor(this string hexStr)
        {
            return Regex.IsMatch(hexStr, "^#[0-9A-Fa-f]{6}$");
        }
        public static string ToSentence(this IEnumerable<string> strs)
        {
            string res = "";
            bool firstEntry = true;
            foreach(string s in strs)
            {
                if (firstEntry) { res = s; firstEntry = false; }
                else res = res + " " + s;
            }
            return res;
        }

     
        public static bool StartsWithNumber(this string s)
        {
            return (s.Length > 0 && char.IsDigit(s[0]));
        }
        public static bool StartsWithLetter(this string s)
        {
            return (s.Length > 0 && char.IsLetter(s[0]));
        }
        public static int? ToIntNull(this string s)
        {
            if (int.TryParse(s, out int res)) return res; else return null;
        }

        public static string ToFirstRegexMatch(this string s, string pattern)
        {
            if (s == null) return null;
            var m = Regex.Match(s, pattern, RegexOptions.IgnoreCase);
            if (m.Success) return m.Value;
            else return null;

        }
        public static string WhiteSpaceToNull(this string str)
        {
            return (string.IsNullOrWhiteSpace(str) ? null : str); 
        }


        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                static string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
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
        public static T ToEnum<T>(this string s, T valueIfError = default) where T: struct
        {
            if (Enum.TryParse(s, out T myEnum)) return myEnum; else return valueIfError;
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

        public static bool? ToBoolNull<T>(this T value)
        {
            try
            {
                if (value is string S)
                {
                    if (string.IsNullOrWhiteSpace(S)) return null;
                    string SL = S.ToLower();
                    if (S == "0" || SL == "false" || SL == "off" || SL == "close" || SL == "no" || SL=="n" || SL=="f" || SL == "✓") return false;
                    if (S == "1" || SL == "true" || SL == "on" || SL == "open" || SL == "yes" || SL=="y" || SL=="t" || SL == "ok" || SL == "✕") return true;
                    return null;
                }
                else if (value is bool b) return b;
                else if (value is int v1) return (v1 > 0);
                else if (value is double v2) return (v2 > 0);
                else if (value is uint v3) return (v3 > 0);
                else if (value is float v4) return (v4 > 0);
                else if (value is decimal v5) return (v5 > 0);
                else return null;
            }
            catch { return null; }
        }
        public static string ToFirst2Words(this string sentence)
        {
            return sentence.Trim().ToFirstRegexMatch(@"^[^\s]*(\s[^\s]*)?").Trim();   
        }
        public static string ToFirstWord(this string sentence)
        {
            return sentence.Trim().ToFirstRegexMatch(@"^[^\s]*");
        }
        public static string ToFriendlyName(this string fullName)
        {
            string firstWord = fullName.ToFirstWord();
            if (firstWord.Count() <= 3) return fullName.ToFirst2Words(); else return firstWord;
        }
        public static bool ToBool(this object value, bool ValueIfError = false, bool? ValueIfNotAString = null)
        {
            try
            {
                if (value is string S)
                {
                    if (string.IsNullOrWhiteSpace(S)) return ValueIfError;
                    string SL = S.ToLower();
                    if (S == "0" || SL == "false" || SL == "off" || SL == "close" || SL == "no" || SL == "n" || SL == "f" || SL == "✕") return false;
                    if (S == "1" || SL == "true" || SL == "on" || SL == "open" || SL == "yes" || SL == "y" || SL == "t" || SL == "ok" || SL == "✓") return true;
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
        [DebuggerStepThrough]
        public static bool CompareWithOrderedDiff<T1,T2>(this IList<T1> aList, IList<T2> bList, Func<T1, T2, bool> comparingFunction)
        {
            var tempAList = aList.ToList();
            var tempBList = bList.ToList();

            if (tempAList.Count != tempBList.Count) return false;
            for (int n = 0; n < tempAList.Count; n++)
            {
                if (!comparingFunction(tempAList[n], tempBList[n])) return false;
            }
            return true;

        }
        [DebuggerStepThrough]
        public static bool CompareWithOrdered<T>(this IList<T> aList, IList<T> bList, Func<T, T, bool> comparingFunction)
        {
            var tempAList = aList.ToList();
            var tempBList = bList.ToList();

            if (tempAList.Count != tempBList.Count) return false;
            for (int n=0; n< tempAList.Count; n++)
            {
                if (!comparingFunction(tempAList[n], tempBList[n])) return false;
            }
            return true;

        }
        [DebuggerStepThrough]
        public static bool CompareWithUnordered<T1,T2>(this IList<T1> aList, IList<T2> bList, Func<T1, T2, bool> comparingFunction)
        {
            var tempAList = aList.ToList();
            var tempBList = bList.ToList();

            if (tempAList.Count != tempBList.Count) return false;
            while (tempAList.Count > 0 && tempBList.Count > 0)
            {
                bool found = false;
                foreach (var a in tempAList)
                {
                    foreach (var b in tempBList)
                    {
                        if (comparingFunction(a, b))
                        {
                            tempAList.Remove(a);
                            tempBList.Remove(b);
                            found = true;
                            break;
                        }
                    }
                    if (found) break; else return false;
                }
            }
            return true;
        }

        public static void UpdateWithUnordered<T1, T2>(this IList<T1> aList, IList<T2> bList, Func<T1, T2, bool> comparingFunction, Action<T1,T2> updateAction)
        {
            var toUpdates = new List<Tuple<T1,T2>>();
            foreach (T2 bItem in bList)
            {
                foreach (T1 aItem in aList)
                {
                    if (comparingFunction(aItem,bItem))
                    {
                        toUpdates.Add(new Tuple<T1,T2>(aItem, bItem));
                    }
                }
            }
            toUpdates.ForEach(e => updateAction?.Invoke(e.Item1,e.Item2));
        }
        [DebuggerStepThrough]
        public static bool CompareWithUnordered<T1,T2>(this IList<T1> aList, IList<T2> bList, Func<T1, T2, bool> comparingFunction, out List<T2> toAdd, out List<T1> toRemove)
        {
            var tempAList = aList.ToList();
            var tempBList = bList.ToList();
            toRemove = new List<T1>();
            toAdd = new List<T2>();
            bool result = true;
            while (tempAList.Count > 0 && tempBList.Count > 0)
            {
                bool found = false;
                foreach (var a in tempAList)
                {
                    foreach (var b in tempBList)
                    {
                        if (comparingFunction(a, b))
                        {
                            tempAList.Remove(a);
                            tempBList.Remove(b);
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                    else
                    {
                        toRemove.Add(a);
                        tempAList.Remove(a);
                        result = false;
                        break;
                    }
                }
            }
            if (tempBList.Count > 0)
            {
                toAdd.AddRange(tempBList);
                result = false;
            }
            if (tempAList.Count > 0)
            {
                toRemove.AddRange(tempAList);
                result = false;
            }
            return result;
        }
        public static string ToRowSeparatedStringForCode(this IList<string> src)
        {
            string output = null;
            int srcCount = src.Count();
            for (int n = 0; n < srcCount; n++)
            {
                //case "vnd.android.cursor.item/vnd.com.whatsapp.profile":
                if (n == srcCount - 1) output += "case \"" + src[n] + "\":";
                else output += "case \""+src[n] + "\":\r\n";
            }
            return output;
        }

        public static List<List<T>> SplitIntoGroups<T>(this IEnumerable<T> source, int groupSizeLimit)
        {
            if (groupSizeLimit < 0) throw new Exception("Group Size cannot be zero or less");
            List<List<T>> list = new List<List<T>>();
            List<T> subList = new List<T>();
            foreach (T item in source)
            {
                if (subList.Count < groupSizeLimit)
                {
                    subList.Add(item);
                }
                else
                {
                    list.Add(subList);
                    subList = new List<T>();
                }
            }
            if (subList.Count > 0) list.Add(subList);
            return list;
        }
       
        public static IGrouping<TKey, TElement> ToGroup<TKey, TElement>(this IEnumerable<TElement> elements, TKey keyValue)
        {
            return elements.GroupBy(_ => keyValue).FirstOrDefault();
        }

        public static string ToSeparatedString(this IEnumerable<string> src, string Separator = ",")
        {
            string output = null;
            int srcCount = src.Count();
            int i = 1;
            foreach (var e in src)
            {
                if (i == srcCount) output += e; else output += e + Separator;
                i++;
            }
            return output;
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
        public static double Round(this double value)
        {
            return Math.Round(value);
        }
        public static decimal Round(this decimal value)
        {
            return Math.Round(value);
        }
        public static int ToInt(this object value, int ValueIfFail = 0)
        {
            if (value == null || value == DBNull.Value) return ValueIfFail;
            try { return Convert.ToInt32(value); }
            catch { return ValueIfFail; }
        }

        //public static FindInStringResult FindFirstWordThatStartsWith(this string str, string startsWith, int maxExtractClippedLength)
        //{
        //    var res = (" " + str.Trim()).FindInString(" " + startsWith, maxExtractClippedLength);
        //    if (res.Found)
        //    {
        //        string extractedString = res.ExtractedString;
        //        int extractStartIndex = res.ExtractStartIndex;
        //        int startPos = res.StartPos +1;
        //        int endPos = res.EndPos;
        //        string leftOfFoundString = res.LeftOfFoundString;
        //        string foundString = res.FoundString.Substring(1); //Remove the space
        //        string rightOfFoundString = res.RightOfFoundString;
        //        if (res.ExtractStartIndex==0) //If string was taken form the start
        //        {
        //            extractedString = res.ExtractedString[1..];
        //            startPos--;
        //            endPos--;
        //            if (leftOfFoundString.Length > 0) leftOfFoundString = leftOfFoundString[1..];
        //            foundString = foundString[1..];
        //        }
        //        return new FindInStringResult(true, extractedString,extractStartIndex, startPos, endPos, leftOfFoundString, foundString, rightOfFoundString);
        //    }
        //    else return new FindInStringResult(false, null, -1, -1, -1, null, null, null);
        //}

        public static bool HasChar(this char c, char[] toMatchChars, out int matchingCharIndex)
        {
            for (int n=0; n< toMatchChars.Count(); n++)
            {
                if (c == toMatchChars[n])
                {
                    matchingCharIndex = n;
                    return true;
                }
            }
            matchingCharIndex = -1;
            return false;
        }
        public static string ReplaceRegex(this string input, string pattern, string replaceWith, RegexOptions regexOptions = RegexOptions.IgnoreCase)
        {
            return Regex.Replace(input, pattern, replaceWith, regexOptions);
        }

        static readonly char[] wordSeparators = new char[] { ' ', ',',':','=','\"','\'','-','?',';','=' };
        public static FindStringsResult FindStrings(this string str, string stringToFind, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            List<FindStringMatch> matches = new List<FindStringMatch>();
            string[] stringsToFind = stringToFind.Split(new char[] { ' ', ',' },StringSplitOptions.RemoveEmptyEntries);

            int n = -1;
            for (int n2=0; n2< stringsToFind.Count(); n2++) // Go through every word to find in String
            {
                if (stringsToFind[n2].IsNullOrEmpty()) continue;
                string toFind = stringsToFind[n2];
                n++;
                bool noMoreResults = false;
                FindStringMatch currentMatch = null;
                int strToProcessStartPos = 0;
                var strToProcess = str;
                while (!noMoreResults) //for every word keep testing the entire string to find which word gives the highest score. A word that starts at the beginning will is scored higher
                {
                    var index = strToProcess.IndexOf(toFind, stringComparison);
                    if (index == -1) noMoreResults = true;
                    else
                    {
                        var startPos = index + strToProcessStartPos;
                        int endPos = startPos + toFind.Length;
                        bool isStartOfSentence = false;
                        bool isStartOfWord = false;
                        char startOfWordSeparator = ' ';
                        int matchScore = 1; //a match anywhere in the string
                        if (startPos == 0)
                        {
                            isStartOfSentence = true;
                            matchScore = 10000;
                        }
                        else
                        {
                            int wSC = wordSeparators.Count();
                            if (str[startPos - 1].HasChar(wordSeparators, out int matchingCharIndex)) //if occur at start of word
                            {
                                isStartOfWord = true;
                                startOfWordSeparator = wordSeparators[matchingCharIndex];
                                matchScore = 100 + (wSC - matchingCharIndex);
                            }
                            else //if occur at centre of word
                            {
                                if (toFind.Length == 1) //If searching for a single letter, only consider results when it occurs at the start of a word
                                {
                                    strToProcess = str[endPos..];
                                    strToProcessStartPos = endPos;
                                    continue;
                                }
                            }
                        }
                        bool isFullWordMatch = endPos>=str.Length || str[endPos].HasChar(wordSeparators, out _);
                        FindStringMatch newMatch = new FindStringMatch(startPos, endPos, toFind, isStartOfSentence, 
                            isStartOfWord, isFullWordMatch, startOfWordSeparator, matchScore);
                        strToProcess = str[newMatch.EndPos..];
                        strToProcessStartPos = newMatch.EndPos;
                        if (strToProcess.IsNullOrEmpty()) noMoreResults = true;
                        if (newMatch.IsStartOfSentence) { currentMatch = newMatch; break; }
                        if (currentMatch == null)
                        {
                            currentMatch = newMatch;
                        }
                        else
                        {
                            if (newMatch.MatchScore > currentMatch.MatchScore) currentMatch = newMatch;
                        }
                    }                    
                }

                //Detect for word continuation (flush), as this must be a higher score.
                //For example "Cik Ahmad Ismail", should be higher then "Ahmad" alone                
                if (currentMatch != null)
                {
                    if (matches.Count > 0 && currentMatch.IsStartOfWord)
                    {
                        var lastMatch = matches.Last();
                        if (lastMatch.IsStartOfWord)
                        {
                            currentMatch.MatchScore += 10000;
                        }
                    }
                    matches.Add(currentMatch);
                }
            }


            List<FindStringResultPart> resParts = new List<FindStringResultPart>();
            if (matches.Count > 0)
            {
                if (matches.Count == 1)
                {
                    if (0 < matches[0].StartPos) resParts.Add(new FindStringResultPart(str[0..matches[0].StartPos]));
                    if (matches[0].StartPos < matches[0].EndPos) resParts.Add(new FindStringResultPart(str[matches[0].StartPos..matches[0].EndPos], true));
                    if (matches[0].EndPos < str.Length) resParts.Add(new FindStringResultPart(str[matches[0].EndPos..]));
                }
                else
                {                    
                    matches.Sort((a, b) => { var compareRes = a.StartPos.CompareTo(b.StartPos); if (compareRes == 0) return (a.EndPos.CompareTo(b.EndPos)); else return compareRes; } );
                    int cursor = 0;
                    for (int n3 = 0; n3 < matches.Count; n3++)
                    {

                        if (n3 == matches.Count - 1)
                        {
                            if (cursor < matches[n3].StartPos) resParts.Add(new FindStringResultPart(str[cursor..matches[n3].StartPos]));
                            if (matches[n3].StartPos < matches[n3].EndPos) resParts.Add(new FindStringResultPart(str[matches[n3].StartPos..matches[n3].EndPos], true));
                            if (matches[n3].EndPos < str.Length) resParts.Add(new FindStringResultPart(str[matches[n3].EndPos..]));
                        }
                        else
                        {
                            //Give higher score for matches that continue upon each other, with a comma or any separator in between
                            if (matches[n3].EndPos == (matches[n3+1].StartPos-1))
                            {
                                var charAtPos = str[matches[n3 + 1].StartPos - 1];
                                if (charAtPos.HasChar(new Char[] {' ',',','-' }, out _))
                                {
                                    matches[n3].MatchScore = matches[n3].MatchScore * 2;
                                }
                            }    
                            

                            if (matches[n3].EndPos >= matches[n3 + 1].StartPos)
                            {
                                if (cursor < matches[n3].StartPos) resParts.Add(new FindStringResultPart(str[cursor..matches[n3].StartPos]));
                                if (matches[n3].StartPos < matches[n3 + 1].StartPos) resParts.Add(new FindStringResultPart(str[matches[n3].StartPos..matches[n3 + 1].StartPos], true));
                                cursor = matches[n3 + 1].StartPos;
                            }
                            else
                            {
                                if (cursor < matches[n3].StartPos) resParts.Add(new FindStringResultPart(str[cursor..matches[n3].StartPos]));
                                if (matches[n3].StartPos < matches[n3].EndPos) resParts.Add(new FindStringResultPart(str[matches[n3].StartPos..matches[n3].EndPos], true));
                                cursor = matches[n3].EndPos;
                            }
                        }
                        
                    }
                }
            }
            return new FindStringsResult(matches.Count > 0, matches.ToArray(),resParts.ToArray());
        }
        public static FindInStringResult FindInString(this string str, string toFind, int maxExtractClippedLength = -1)
        {
            var index = str.IndexOf(toFind, StringComparison.CurrentCultureIgnoreCase);
            if (index != -1)
            {
                if (maxExtractClippedLength == -1)
                {
                    int endPos = index + toFind.Length;
                    return new FindInStringResult(true, str, 0, index, endPos,
                        str[0..index],
                        str[index..endPos],
                        str[endPos..], str);
                }
                else
                {
                    int strLastIndex = str.Length - 1;
                    int halfPortion = (maxExtractClippedLength - toFind.Length) / 2;
                    if (halfPortion < 0) halfPortion = 0;
                    int clipStartIndex = index - halfPortion;
                    int clipLength;
                    if (clipStartIndex < 0) clipStartIndex = 0;
                    if (clipStartIndex + maxExtractClippedLength > str.Length)
                    {
                        clipStartIndex -= (maxExtractClippedLength + clipStartIndex - str.Length);
                        if (clipStartIndex < 0) clipStartIndex = 0;
                        clipLength = strLastIndex - clipStartIndex + 1;
                    }
                    else
                    {
                        clipLength = maxExtractClippedLength; // halfPortion + halfPortion + toFind.Length + (maxExtractClippedLength % 2);
                        if (clipStartIndex + clipLength > str.Length) clipLength = str.Length - clipStartIndex;
                    }
                    if (clipLength > maxExtractClippedLength)
                    {
                        clipLength = maxExtractClippedLength;
                    }
                    string extract = str.Substring(clipStartIndex, clipLength);
                    if (toFind.Length > extract.Length) toFind = toFind.Substring(0, extract.Length);
                    int extractStartIndex = extract.IndexOf(toFind, StringComparison.CurrentCultureIgnoreCase);
                    int endPos = extractStartIndex + toFind.Length;
                    return new FindInStringResult(true, extract, clipStartIndex, extractStartIndex, endPos, extract[0..extractStartIndex], extract[extractStartIndex..endPos], extract[endPos..], str);
                }
            }
            else return new FindInStringResult(false, null, -1, -1, -1, null, null, null, str);
        }

        public static double Clamp(this double self, double min, double max)
        {
            if (max < min)
            {
                return max;
            }
            else if (self < min)
            {
                return min;
            }
            else if (self > max)
            {
                return max;
            }

            return self;
        }

        public static int Clamp(this int self, int min, int max)
        {
            if (max < min)
            {
                return max;
            }
            else if (self < min)
            {
                return min;
            }
            else if (self > max)
            {
                return max;
            }

            return self;
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
    public struct FindStringResultPart
    {
        public FindStringResultPart(string text, bool highlight = false)
        {
            this.Highlight = highlight;
            this.Text = text;
        }

        public bool Highlight { get; set; } 
        public string Text { get; set; }
    }
    public class FindStringsResult
    {
        public FindStringsResult(bool success, FindStringMatch[] matches, FindStringResultPart[] parts)
        {
            Success = success;
            Matches = matches;
            Parts = parts;
            foreach (var match in Matches)
            {
                MatchScore += match.MatchScore;
            }
        }
        public bool Success { get; set; }
        public int MatchScore { get; set; } = 0;
        public FindStringResultPart[] Parts { get; set; }
        public FindStringMatch[] Matches { get; set; }        
    }
    public class FindStringMatch
    {
        public FindStringMatch(int startPos, int endPos, string match, bool isStartOfSentence = false, bool isStartOfWord = false, bool isFullWordMatch = false, char startOfWordSeparator=' ', int matchScore = 0)
        {
            StartPos = startPos;
            EndPos = endPos;
            Match = match;
            IsStartOfWord = isStartOfWord;
            IsFullWordMatch = isFullWordMatch;
            StartOfWordSeparator = startOfWordSeparator;
            MatchScore = matchScore;
            IsStartOfSentence = isStartOfSentence;
        }
        public int StartPos { get; set; }
        public int EndPos { get; set; }
        public string Match { get; set; }
        public bool IsStartOfSentence { get; set; }
        public bool IsStartOfWord { get; set; }
        public bool IsFullWordMatch { get; set; }
        public char StartOfWordSeparator { get; set; }
        public int MatchScore { get; set; }
    }

    public class FindInStringResult
    {
        public FindInStringResult(bool found, string extractedString, int extractStartIndex, int startPos, int endPos, string leftOfFoundString, string foundString, string rightOfFoundString, string searchedStr)
        {
            SearchedString = searchedStr;
            Found = found;
            ExtractStartIndex = extractStartIndex;
            StartPos = startPos;
            EndPos = endPos;
            LeftOfFoundString = leftOfFoundString;
            FoundString = foundString;
            RightOfFoundString = rightOfFoundString;
            ExtractedString = extractedString;
        }

        public bool Found { get; private set; } = true;
        public int ExtractStartIndex { get; private set; } = -1;
        public int StartPos { get; private set; } = -1;
        public int EndPos { get; private set; } = -1;
        public string SearchedString { get; private set; } = null;
        public string ExtractedString { get; private set; } = null;
        public string LeftOfFoundString { get; set; } = null;
        public string FoundString { get; private set; } = null;
        public string RightOfFoundString { get; set; } = null;
    }
}
