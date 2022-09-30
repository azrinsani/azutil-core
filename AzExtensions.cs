using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzUtil.Core;
using System.Text.Json;
using System.Threading;

//UPDATE 21/7/2022
namespace AzUtil.Core
{
    //Global Extensions
    public static class AzExtensions
    {
        public static async Task<string> GetMyIp(this HttpClient httpClient)
        {
            var services = new List<string>
            {
                    "https://ipv4.icanhazip.com",
                    "https://api.ipify.org",
                    "https://ipinfo.io/ip",
                    "https://checkip.amazonaws.com",
                    "https://wtfismyip.com/text"
                };
            using var webclient = new WebClient();
            foreach (var service in services)
            {
                try
                {
                    var response = await httpClient.GetAsync(service);
                    if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }

        public static ICollection<T> RemoveWhere<T>(this ICollection<T> col, Func<T, bool> func)
        {
            var toRemoves = col.Where(func).ToList();
            foreach (var toRemove in toRemoves) col.Remove(toRemove);
            return col;
        }

        public static ICollection<T> RemoveWhere<T>(this ICollection<T> col, Func<T, bool> func, out List<T> removedItems)
        {
            var toRemoves = col.Where(func).ToList();
            foreach (var toRemove in toRemoves) col.Remove(toRemove);
            removedItems = toRemoves;
            return col;
        }

        public static async Task<string> GetCountyCodeByIp(this HttpClient httpClient, string ip = null)
        {
            ip ??= await httpClient.GetMyIp();
            var url = "https://api.iplocation.net/?ip=" + ip;
            try
            {
                Uri uri = new Uri(string.Format(url, string.Empty));
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode) return null;
                string content = await response.Content.ReadAsStringAsync();
                string country = content.ToFirstRegexMatch(@"(?<=\""country_code2\""\:"")[^\""]*");
                return country.ToLower();
            }
            catch
            {
                return null;
            }
        }
        
        public static T ToDeserializedObject<T>(this string jsonStr) 
            => JsonSerializer.Deserialize<T>(jsonStr);

        public static T ToDeserializedObject<T>(this byte[] jsonBytes) 
            => JsonSerializer.Deserialize<T>(jsonBytes);

        public static string ToSerializedString<T>(this T obj)
        {
            var ba = JsonSerializer.SerializeToUtf8Bytes(obj);
            return Encoding.UTF8.GetString(ba, 0, ba.Length);
        }
        
        public static byte[] ToSerializedByteArray<T>(this T obj) 
            => JsonSerializer.SerializeToUtf8Bytes(obj);
        
        public static bool HasDateOnly(this DateTime dateTime) 
            => dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0 && dateTime.Millisecond == 0;
        
        public static List<int> FindIndexes<T>(this IList<T> source, Func<T, bool> predicate)
        {
            var res = new List<int>();
            for (int n=0;n<source.Count;n++)
            {
                if (predicate.Invoke(source[n])) res.Add(n);
            }
            return res;
        }
        
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
            => dict.TryGetValue(key, out var val) ? val : default;

        public static Color ToColor(this string colorHex, Color colorIfEmpty) 
            => colorHex.IsNullOrEmpty() ? colorIfEmpty : Color.FromHex(colorHex);

        public static bool CompareWithTreatNullWhiteSpaceAsEqual(this string a, string b) 
            => string.IsNullOrWhiteSpace(a) ? string.IsNullOrWhiteSpace(b) : string.Equals(a, b);
        
        public static bool CompareWithTreatNullEmptyAsEqual(this string a, string b) 
            => string.IsNullOrEmpty(a) ? string.IsNullOrEmpty(b) : string.Equals(a, b);

        public static T FirstOrDefaultWithIndex<T>(this IList<T> source, Func<T, bool> predicate, out int index)
        {
            index = -1;
            var res = source.FirstOrDefault(predicate);
            if (res != null) index = source.IndexOf(res);
            return res;
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
            return items.Any(item => Equals(source, item));
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
            listArr.AddRange(additions);
            source = listArr.ToArray();
            return source;
        }
        
        public static string RemoveLastCharacter(this string source) 
            => source?.Remove(source.Length - 1);

        public static string RemoveLastCrlnIfExist(this string source)
        {
            if (source.Length <= 2) return source;
            return source[^2..] == "\r\n" ? source.Remove(source.Length - 2) : source;
        }
        
        public static string RegexRemove(this string source, Regex regex, out Collection<string> removed)
        {
            MatchCollection matches = regex.Matches(source);
            removed = new Collection<string>();
            foreach (Match match in matches)
            {
                if (match.Success) removed.Add(match.Value);
            }
            return regex.Replace(source, "");
        }
      
        public static string ToShorterString(this string source, int length)
        {
            if (source.Length > length) return source.Substring(0, length) + "..";
            else return source;
        }

        public static ICollection<T> RemoveRange<T>(this ICollection<T> source, IEnumerable<T> toRemoves)
        {
            foreach (var toRemove in toRemoves) source.Remove(toRemove);
            return source;
        }
        public static ICollection<T> AddRange<T>(this ICollection<T> source, IEnumerable<T> toAdds)
        {
            foreach (var toAdd in toAdds)
            {
                source.Add(toAdd);
            }
            return source;
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
        public static List<T2> ToClonedList<T, T2>(this IEnumerable<T> source, Func<T, T2> transformItem) => source.Select(transformItem).ToList();
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
            foreach (var item in collection) action.Invoke(item);
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
            var enumerable = strs as string[] ?? strs.ToArray();
            int strCount = enumerable.Count();
            if (strCount == 0) return "";
            enumerable.ForEach((e, n) => 
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
        
        public static TV ValueOrDefault<TK,TV>(this Dictionary<TK,TV> dict,TK key) 
            => dict.TryGetValue(key, out TV value) ? value : default;

        public static byte[] ExtractResource(this System.Reflection.Assembly a, String filename)
        {
            using Stream resFilestream = a.GetManifestResourceStream(filename);
            if (resFilestream == null) return null;
            byte[] ba = new byte[resFilestream.Length];
            resFilestream.Read(ba, 0, ba.Length);
            return ba;
        }

        public static bool IsValidHexColor(this string hexStr) 
            => Regex.IsMatch(hexStr, "^#[0-9A-Fa-f]{6}$");

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
            => s.Length > 0 && char.IsDigit(s[0]);

        public static bool StartsWithLetter(this string s) 
            => s.Length > 0 && char.IsLetter(s[0]);

        public static int? ToIntNull(this string s)
        {
            if (int.TryParse(s, out int res)) return res; else return null;
        }

        public static string ToFirstRegexMatch(this string s, string pattern)
        {
            if (s == null) return null;
            var m = Regex.Match(s, pattern, RegexOptions.IgnoreCase);
            return m.Success ? m.Value : null;
        }
        
        public static string WhiteSpaceToNull(this string str) 
            => string.IsNullOrWhiteSpace(str) ? null : str;

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

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
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
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

        public static string RepeatString(this string s, int numberOfTimes)
        {
            StringBuilder sb = new StringBuilder();
            for (int n = 0; n < numberOfTimes; n++) sb.Append(s);
            return sb.ToString();
        }
        
        public static bool IsEqualToEither(this string s, params string[] comps) 
            => comps.Any(comp => string.Equals(comp, s));
        
        public static T ToEnum<T>(this string s, T valueIfError = default) where T: struct 
            => Enum.TryParse(s, out T myEnum) ? myEnum : valueIfError;

        public static T ToEnumFromInt<T>(this string s) where T: struct
        {
            T t = (T) Enum.ToObject(typeof(T), s.ToInt());
            return t;
        }
        
        public static string Last(this string s)
        {
            if (s.Length == 0) return "";
            return s[^1..];
        }
        
        public static void AddIfExist(this List<string> strings, string entry)
        {
            if (!strings.Exists(s => string.Equals(s, entry, StringComparison.InvariantCultureIgnoreCase))) strings.Add(entry);
        }
        
        public static void AddIfNotExist(this List<string> strings, string entry, out bool alreadyExists)
        {
            alreadyExists = false;
            string entryTrimmed = entry.Trim();
            if (!strings.Exists(s => string.Equals(s, entryTrimmed, StringComparison.InvariantCultureIgnoreCase))) strings.Add(entryTrimmed.ToLower()); else alreadyExists = true;
        }
        
        public static bool IfExist(this List<string> strings, string entry)
        {
            string entryTrim = entry.Trim();
            return strings.Exists(s => string.Equals(s, entryTrim, StringComparison.InvariantCultureIgnoreCase));
        }

        public static string RemoveEmptyLines(this string text)
        {
            var regex = new Regex(@"\s*\r");
            return text.RegexRemove(regex, out _);
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
            var sb = new StringBuilder();
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                foreach (Byte b in hash) sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static bool? ToBoolNull<T>(this T value)
        {
            try
            {
                switch (value)
                {
                    case string s when string.IsNullOrWhiteSpace(s):
                        return null;
                    case string s:
                    {
                        string sl = s.ToLower();
                        if (s == "0" || sl == "false" || sl == "off" || sl == "close" || sl == "no" || sl == "n" || sl == "f" || sl == "✓") return false;
                        if (s == "1" || sl == "true" || sl == "on" || sl == "open" || sl == "yes" || sl == "y" || sl == "t" || sl == "ok" || sl == "✕") return true;
                        return null;
                    }
                    case bool b:
                        return b;
                    case int v1:
                        return v1 > 0;
                    case double v2:
                        return v2 > 0;
                    case uint v3:
                        return v3 > 0;
                    case float v4:
                        return v4 > 0;
                    case decimal v5:
                        return v5 > 0;
                    default:
                        return null;
                }
            }
            catch { return null; }
        }
        
        public static string ToFirst2Words(this string sentence) 
            => sentence.Trim().ToFirstRegexMatch(@"^[^\s]*(\s[^\s]*)?").Trim();

        public static string ToFirstWord(this string sentence) 
            => sentence.Trim().ToFirstRegexMatch(@"^[^\s]*");

        public static string ToFriendlyName(this string fullName)
        {
            var firstWord = fullName.ToFirstWord();
            return firstWord.Count() <= 3 ? fullName.ToFirst2Words() : firstWord;
        }
        
        public static bool? ToBoolNull(this object value)
        {
            try
            {
                if (value is string s && !s.IsNullOrWhiteSpace())
                {
                    var sl = s.ToLower();
                    if (s == "0" || sl == "false" || sl == "off" || sl == "close" || sl == "no" || sl == "n" || sl == "f" || sl == "✕") return false;
                    if (s == "1" || sl == "true" || sl == "on" || sl == "open" || sl == "yes" || sl == "y" || sl == "t" || sl == "ok" || sl == "✓") return true;
                }
            }
            catch
            {
                // ignored
            }
            return null;
        }
        
        public static bool ToBool(this object value, bool valueIfError = false, bool? valueIfNotAString = null)
        {
            try
            {
                if (value is string s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return valueIfError;
                    string sl = s.ToLower();
                    if (s == "0" || sl == "false" || sl == "off" || sl == "close" || sl == "no" || sl == "n" || sl == "f" || sl == "✕") return false;
                    if (s == "1" || sl == "true" || sl == "on" || sl == "open" || sl == "yes" || sl == "y" || sl == "t" || sl == "ok" || sl == "✓") return true;
                    return valueIfError;
                }
                else
                {
                    if (valueIfNotAString == null) return Convert.ToBoolean(value);
                    else return (bool) valueIfNotAString;
                }
            }
            catch { return valueIfError; }
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
            var list = new List<List<T>>();
            var subList = new List<T>();
            foreach (T item in source)
            {
                subList.Add(item);
                if (subList.Count >= groupSizeLimit)
                {
                    list.Add(subList);
                    subList = new List<T>();
                }
            }
            if (subList.Count > 0) list.Add(subList);
            return list;
        }

        public static IGrouping<TKey, TElement> ToGroup<TKey, TElement>(this IEnumerable<TElement> elements, TKey keyValue) 
            => elements.GroupBy(_ => keyValue).FirstOrDefault();

        public static string ToSeparatedString(this IEnumerable<string> src, string separator = ",")
        {
            string output = null;
            var enumerable = src as string[] ?? src.ToArray();
            int srcCount = enumerable.Count();
            int i = 1;
            foreach (var e in enumerable)
            {
                if (i == srcCount) output += e; else output += e + separator;
                i++;
            }
            return output;
        }
        
        public static DateTime? ToDateTimeNull(this object value, bool ifTimeZoneNotSpecifiedAssumeUtc = true, DateTime? valueIfFail = null)
        {
            try
            {
                if (value == null || value == DBNull.Value) return null; 
                if (value is string tsStr) return StringToDt(tsStr, ifTimeZoneNotSpecifiedAssumeUtc); else return Convert.ToDateTime(value);
            }
            catch { return valueIfFail; }
        }
        
        public static DateTime ToDateTime(this object value, bool ifTimeZoneNotSpecifiedAssumeUtc = true, DateTime? valueIfFail = null)
        {
            try { if (value is string tsStr) return StringToDt(tsStr, ifTimeZoneNotSpecifiedAssumeUtc); else return Convert.ToDateTime(value); }
            catch { return valueIfFail ?? DateTime.MinValue; }
        }
        
        private static DateTime StringToDt(string tsStr, bool ifTimeZoneNotSpecifiedAssumeUtc = true)
        {
            if (DateTime.TryParseExact(tsStr, new[] { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-M-d H:m:s", "yyyy-M-d HH:m:s.f" }, 
                    CultureInfo.InvariantCulture, ifTimeZoneNotSpecifiedAssumeUtc ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal, out DateTime dt))
            { if (ifTimeZoneNotSpecifiedAssumeUtc) dt = dt.ToUniversalTime(); }
            else DateTime.TryParse(tsStr, null, ifTimeZoneNotSpecifiedAssumeUtc? DateTimeStyles.AssumeUniversal : DateTimeStyles.RoundtripKind, out dt); //This handles ECMA ISO8601 Date Format
            return dt;
        }
        
        public static float ToSingle(this object value)
        {
            try { return Convert.ToSingle(value); }
            catch { return 0;}
        }
        
        public static double ToDouble(this object value)
        {
            try { return Convert.ToDouble(value); }
            catch { return 0; }
        }
        
        public static double ToDouble(this object value, double defaultValueIfError)
        {
            try { return Convert.ToDouble(value); }
            catch { return defaultValueIfError; }
        }
        
        public static long ToLong(this object value, Int64 defaultValueIfError = 0)
        {
            try { return Convert.ToInt64(value); }
            catch { return defaultValueIfError;  }
        }
        
        public static Byte ToByte(this object value)
        {
            try { return Convert.ToByte(value); }
            catch { return 0; }
        }
        
        public static ushort ToUShort(this object value)
        {
            try { return Convert.ToUInt16(value); }
            catch { return 0; }
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
        
        public static Int16 ToInt16(this object value, Int16 valueIfError = 0)
        {
            try { return Convert.ToInt16(value); }
            catch { return valueIfError; }
        }
        
        public static Int64 ToInt64(this object value, Int64 valueIfError = 0)
        {
            try { return Convert.ToInt64(value); }
            catch { return valueIfError;}
        }
        
        public static uint ToUint(this object value, uint valueIfError = 0)
        {
            try { return Convert.ToUInt32(value); }
            catch { return valueIfError; }
        }
        
        public static UInt16 ToUint16(this object value, UInt16 valueIfError = 0)
        {
            try { return Convert.ToUInt16(value); }
            catch { return valueIfError; }
        }
        
        public static UInt64 ToUint64(this object value, UInt64 valueIfError = 0)
        {
            try { return Convert.ToUInt64(value); }
            catch { return valueIfError; }
        }
        
        public static double Round(this double value) => Math.Round(value);

        public static decimal Round(this decimal value) => Math.Round(value);

        public static int ToInt(this object value, int valueIfFail = 0)
        {
            if (value == null || value == DBNull.Value) return valueIfFail;
            try { return Convert.ToInt32(value); }
            catch { return valueIfFail; }
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
        //        if (res.ExtractStartIndex == 0) //If string was taken form the start
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
            for (int n=0; n< toMatchChars.Length; n++)
            {
                if (c != toMatchChars[n]) continue;
                matchingCharIndex = n;
                return true;
            }
            matchingCharIndex = -1;
            return false;
        }
        
        public static string ReplaceRegex(this string input, string pattern, string replaceWith, RegexOptions options = RegexOptions.IgnoreCase) 
            => Regex.Replace(input, pattern, replaceWith, options);

        private static readonly char[] wordSeparators = { ' ', ',', ':', '=', '-', '[', ']', '(', ')', '?', ';', '{', '}', '|', '&', '_', '+', '<', '>', '@', '.', ';', '!', '#', '$', '*', '^', '~', '\"', '\''};
        
        public static FindStringsResult FindStrings(this string fullString, string stringToFind, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            var matches = new List<FindStringMatch>();
            string[] stringsToFind = stringToFind.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            int n = 0;
            foreach (var toFind in stringsToFind)
            {
                if (toFind.IsNullOrEmpty()) continue;
                bool noMoreResults = false;
                FindStringMatch currentMatch = null;
                int strToProcessStartPos = 0;
                var strToProcess = fullString;
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
                        int matchScore = 10; //A match anywhere in the string
                        if (startPos == 0 && n == 0)
                        {
                            isStartOfSentence = true;
                            isStartOfWord = true;
                        }
                        else
                        {
                            int wSc = wordSeparators.Length;
                            int matchingCharIndex = 0;
                            if (startPos == 0 || fullString[startPos - 1].HasChar(wordSeparators, out matchingCharIndex)) //if occur at start of word
                            {
                                isStartOfWord = true;
                                matchScore = 100 + (wSc - matchingCharIndex) 
                                                 + (30 - startPos); //Take in consideration the start position of the match 
                            }
                            else //if occur at centre of word
                            {
                                if (toFind.Length == 1) //If searching for a single letter, only consider results when it occurs at the start of a word
                                {
                                    strToProcess = fullString[endPos..];
                                    strToProcessStartPos = endPos;
                                    continue;
                                }
                            }
                        }
                        
                        bool isCompleteWordMatch = false;
                        switch (toFind.Length)
                        {
                            // If searching for a single Alphabet
                            case 1 when stringsToFind.Length == 1:
                            {
                                if (isStartOfSentence) matchScore += 10000;
                                else if (isStartOfWord) matchScore *= 4;
                                break;
                            }
                            // If match is Start of word/sentence but not end of Sentence
                            case > 1 when endPos < fullString.Length && isStartOfWord:
                            {
                                if (fullString[endPos].HasChar(wordSeparators, out _)) //Complete word match
                                {
                                    matchScore = isStartOfSentence ? matchScore + 10001 : matchScore * 4; 
                                    isCompleteWordMatch = true;
                                }
                                else matchScore = isStartOfSentence ? matchScore + 10000 : matchScore * 2;
                                break;
                            }
                            // If match is Start of word/sentence but end of sentence (making it a complete word match)
                            case > 1 when isStartOfWord:
                                matchScore = isStartOfSentence ? matchScore + 10000 : matchScore * 4; 
                                isCompleteWordMatch = true;
                                break;
                        }
                        
                        FindStringMatch newMatch = new FindStringMatch(startPos, endPos, isStartOfSentence, isStartOfWord, matchScore, isCompleteWordMatch);
                        strToProcess = fullString[newMatch.EndPos..];
                        strToProcessStartPos = newMatch.EndPos;
                        if (strToProcess.IsNullOrEmpty()) noMoreResults = true;
                        if (newMatch.IsStartOfSentence)
                        {
                            currentMatch = newMatch; 
                            break;
                        }
                        if (currentMatch == null)
                            currentMatch = newMatch;
                        else if (newMatch.MatchScore > currentMatch.MatchScore) 
                            currentMatch = newMatch;
                    }                    
                }

                if (currentMatch != null)
                {
                    //Detect for word continuation (flush), as this must be a higher score.
                    //For example "Cik Ahmad Ismail", should be higher then "Ahmad" alone                
                    if (matches.Count > 0)
                    {
                        var lastMatch = matches.Last();
                        if (lastMatch.IsStartOfWord && currentMatch.StartPos > lastMatch.StartPos && currentMatch.IsStartOfWord)
                        {
                            currentMatch.MatchScore = currentMatch.IsCompleteWordMatch
                                ? currentMatch.MatchScore + 20000
                                : currentMatch.MatchScore + 10000; // If the Exact word is found
                        }
                    }
                    matches.Add(currentMatch);
                }
                n++;
            }
            
            //Calculate the Result Parts
            var resParts = new List<FindStringResultPart>();
            if (matches.Count > 0)
            {
                if (matches.Count == 1)
                {
                    if (0 < matches[0].StartPos) resParts.Add(new FindStringResultPart(fullString[..matches[0].StartPos]));
                    if (matches[0].StartPos < matches[0].EndPos) resParts.Add(new FindStringResultPart(fullString[matches[0].StartPos..matches[0].EndPos], true));
                    if (matches[0].EndPos < fullString.Length) resParts.Add(new FindStringResultPart(fullString[matches[0].EndPos..]));
                }
                else
                {                    
                    matches.Sort((a, b) =>
                    {
                        var compareRes = a.StartPos.CompareTo(b.StartPos);
                        return compareRes == 0 ? a.EndPos.CompareTo(b.EndPos) : compareRes;
                    } );
                    int cursor = 0;
                    for (int n3 = 0; n3 < matches.Count; n3++)
                    {
                        if (n3 == matches.Count - 1)
                        {
                            if (cursor < matches[n3].StartPos) resParts.Add(new FindStringResultPart(fullString[cursor..matches[n3].StartPos]));
                            if (matches[n3].StartPos < matches[n3].EndPos) resParts.Add(new FindStringResultPart(fullString[matches[n3].StartPos..matches[n3].EndPos], true));
                            if (matches[n3].EndPos < fullString.Length) resParts.Add(new FindStringResultPart(fullString[matches[n3].EndPos..]));
                        }
                        else
                        {
                            //Give higher score for matches that continue upon each other, with a comma or any separator in between
                            // if (matches[n3].EndPos == matches[n3+1].StartPos - 1)
                            // {
                            //     var charAtPos = str[matches[n3 + 1].StartPos - 1];
                            //     if (charAtPos.HasChar(new[] {' ',',','-' }, out _)) matches[n3].MatchScore *= 2;
                            // }
                            if (matches[n3].EndPos >= matches[n3 + 1].StartPos)
                            {
                                if (cursor < matches[n3].StartPos) resParts.Add(new FindStringResultPart(fullString[cursor..matches[n3].StartPos]));
                                if (matches[n3].StartPos < matches[n3 + 1].StartPos) resParts.Add(new FindStringResultPart(fullString[matches[n3].StartPos..matches[n3 + 1].StartPos], true));
                                cursor = matches[n3 + 1].StartPos;
                            }
                            else
                            {
                                if (cursor < matches[n3].StartPos) resParts.Add(new FindStringResultPart(fullString[cursor..matches[n3].StartPos]));
                                if (matches[n3].StartPos < matches[n3].EndPos) resParts.Add(new FindStringResultPart(fullString[matches[n3].StartPos..matches[n3].EndPos], true));
                                cursor = matches[n3].EndPos;
                            }
                        }
                    }
                }
            }
            return new FindStringsResult(matches.Count > 0, matches.ToArray(),resParts.ToArray());
        }

        public static double Clamp(this double self, double min, double max)
        {
            if (max < min)
                return max;
            else if (self < min)
                return min;
            else if (self > max)
                return max;
            return self;
        }

        public static int Clamp(this int self, int min, int max)
        {
            if (max < min)
                return max;
            else if (self < min)
                return min;
            else if (self > max)
                return max;
            return self;
        }

        public static bool IsSuccess(this HttpStatusCode code)
        {
            int codeInt = (int) code;
            return codeInt >= 200 & codeInt < 400;
        }
        
        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this string value) { return string.IsNullOrEmpty(value);}

        [DebuggerStepThrough]
        public static string ToNullIfEmpty(this string value) => string.IsNullOrEmpty(value) ? null : value;

        [DebuggerStepThrough]
        public static string ToNullIfWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value) ? null : value;

        [DebuggerStepThrough]
        public static bool IsNullOrWhiteSpace(this string value) { return string.IsNullOrWhiteSpace(value); }
        
        [DebuggerStepThrough]
        public static bool IsEmptyOrWhiteSpace(this string value) => value.All(char.IsWhiteSpace);
        
        [DebuggerStepThrough]
        public static string Right(this string value, int length) { return value[^length..]; }
        
        /// <summary>
        /// By default, cancellation token source cancels asyncronously. This ensures the cancellation is done before continuing.
        /// </summary>
        /// <param name="cancellationTokenSource"></param>
        public static void CancelWithBackgroundContinuations(this CancellationTokenSource cancellationTokenSource)
        {
            Task.Run(cancellationTokenSource.Cancel);
            cancellationTokenSource.Token.WaitHandle.WaitOne(); // make sure to only continue when the cancellation completed (without waiting for all the callbacks)
        }
    }
}
