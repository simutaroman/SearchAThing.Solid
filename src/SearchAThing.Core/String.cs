#region SearchAThing.Core, Copyright(C) 2015-2016 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Math;
using SearchAThing;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Globalization;
using static System.FormattableString;
using Newtonsoft.Json;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// Returns the given string stripped from the given part if exists at beginning.
        /// </summary>        
        public static string StripBegin(this string str, char c, bool ignoreCase = false)
        {
            if (str.Length > 0 && (ignoreCase ? (char.ToUpper(str[0]) == char.ToUpper(c)) : (str[0] == c)))
                return str.Substring(1, str.Length - 1);
            else
                return str;
        }

        /// <summary>
        /// Returns the given string stripped from the given part if exists at beginning.
        /// </summary>        
        public static string StripBegin(this string str, string partToStrip, bool ignoreCase = false)
        {
            if (str.StartsWith(partToStrip, ignoreCase, CultureInfo.CurrentCulture))
            {
                var ptsl = partToStrip.Length;
                return str.Substring(ptsl, str.Length - ptsl);
            }
            else
                return str;
        }

        /// <summary>
        /// Returns the given string stripped from the given part if exists at end.
        /// </summary>        
        public static string StripEnd(this string str, char c, bool ignoreCase = false)
        {
            if (str.Length > 0 && (ignoreCase ? (char.ToUpper(str[str.Length - 1]) == char.ToUpper(c)) : (str[str.Length - 1] == c)))
                return str.Substring(0, str.Length - 1);
            else
                return str;
        }

        /// <summary>
        /// Returns the given string stripped from the given part if exists at end.
        /// </summary>        
        public static string StripEnd(this string str, string partToStrip, bool ignoreCase = false)
        {
            if (str.EndsWith(partToStrip, ignoreCase, CultureInfo.CurrentCulture))
                return str.Substring(0, str.Length - partToStrip.Length);
            else
                return str;
        }

        /// <summary>
        /// extract line from string buffer
        /// </summary>        
        public static string NextLine(this string txt, ref int off)
        {
            return null;
        }

        /// <summary>
        /// Smart line splitter that split a text into lines whatever unix or windows line ending style.
        /// By default its remove empty lines.
        /// </summary>        
        /// <param name="removeEmptyLines">If true remove empty lines.</param>        
        public static IEnumerable<string> Lines(this string txt, bool removeEmptyLines = true)
        {
            var q = txt.Replace("\r\n", "\n").Split('\n');

            if (removeEmptyLines)
                return q.Where(r => r.Trim().Length > 0);
            else
                return q;
        }

        /// <summary>
        /// Returns a human readable bytes length. (eg. 1000, 1K, 1M, 1G, 1T)
        /// if onlyBytesUnit is set to false it will enable representation through K, M, G, T suffixes
        /// </summary>        
        public static string HumanReadable(this long bytes, bool onlyBytesUnit = true, long bytesMultiple = 1L, int decimals = 1)
        {
            var k = 1024L;
            var m = k * 1024;
            var g = m * 1024;
            var t = g * 1024;

            if (bytesMultiple != 1L) bytes = (long)((double)bytes).MRound(bytesMultiple);

            if (bytes < k) { if (onlyBytesUnit) return $"{bytes}"; else return Invariant($"{bytes} b"); }
            else if (bytes >= k && bytes < m) return string.Format("{0,6:0." + "0".Repeat(decimals) + "} Kb", ((double)bytes) / k);
            else if (bytes >= m && bytes < g) return string.Format("{0,6:0." + "0".Repeat(decimals) + "} Mb", ((double)bytes) / m);
            else if (bytes >= g && bytes < t) return string.Format("{0,6:0." + "0".Repeat(decimals) + "} Gb", ((double)bytes) / g);
            else return string.Format("{0:0." + "0".Repeat(decimals) + "} Tb", ((double)bytes) / t);
        }

        /// <summary>
        /// Repeat given string for cnt by concatenate itself
        /// </summary>        
        public static string Repeat(this string s, int cnt)
        {
            var sb = new StringBuilder();

            while (cnt > 0)
            {
                sb.Append(s);
                --cnt;
            }

            return sb.ToString();
        }

        /// <summary>
        /// convert wildcard pattern to regex
        /// the asterisk '*' character replace any group of chars
        /// the question '?' character replace any single character
        /// </summary>        
        public static string WildcardToRegex(this string pattern)
        {
            return $"^{Regex.Escape(pattern).Replace("\\*", ".*").Replace('?', '.')}$";
        }

        /// <summary>
        /// return true if given string matches the given pattern
        /// the asterisk '*' character replace any group of chars
        /// the question '?' character replace any single character
        /// </summary>                
        public static bool WildcardMatch(this string str, string pattern, bool caseSentitive = true)
        {
            var regexStr = pattern.WildcardToRegex();

            Regex regex = null;
            if (caseSentitive)
                regex = new Regex(regexStr);
            else
                regex = new Regex(regexStr, RegexOptions.IgnoreCase);

            return regex.IsMatch(str);
        }

        /// <summary>
        /// split string with given separator string
        /// </summary>        
        public static string[] Split(this string str, string sepStr)
        {
            return str.Split(sepStr.ToArray(), StringSplitOptions.None);
        }

        /// <summary>
        /// return dynamic array from given [[xx],[yy],...] json array
        /// </summary>        
        public static dynamic GetJsonArray(this string jsonDumps)
        {
            return ((dynamic)JObject.Parse($"{{a:{jsonDumps}}}")).a;
        }

        /// <summary>
        /// parse given array of doubles ( invariant ) comma separated
        /// </summary>        
        public static double[] Import(this string ary)
        {
            if (string.IsNullOrEmpty(ary.Trim())) return new double[] { };

            return ary.Split(',').Select(w => double.Parse(w, CultureInfo.InvariantCulture)).ToArray();
        }

        /// <summary>
        /// export to a string ( invariant ) comma separated
        /// </summary>        
        public static string Export(this double[] ary)
        {
            var sb = new StringBuilder();

            foreach (var x in ary)
            {
                if (sb.Length > 0) sb.Append(",");
                sb.Append(string.Format(CultureInfo.InvariantCulture, "{0}", x));
            }

            return sb.ToString();
        }

        /// <summary>
        /// convert a string that exceed N given characters length to {prefix}{latest N chars}
        /// </summary>        
        public static string Latest(this string str, int last_n_chars, string prefix_if_exceed = "...")
        {
            if (str.Length <= last_n_chars)
                return str;
            else
                return $"{prefix_if_exceed}{str.Substring(str.Length - last_n_chars, last_n_chars)}";
        }

        /// <summary>
        /// check if given string contains the part ( ignoring case )
        /// </summary>        
        public static bool ContainsIgnoreCase(this string str, string part)
        {
            return str.ToUpper().Contains(part.ToUpper());
        }

        /// <summary>
        /// convert invalid worksheet characters :\/?*[]' into underscore
        /// </summary>        
        public static string NormalizeWorksheetName(this string s)
        {
            return Regex.Replace(s, $"[{Regex.Escape(@":\/?*[]'").Replace("]", "\\]")}]", "_");
        }

        public static string NormalizeFilename(this string filename, char subst = '_')
        {
            string res = "";
            var invalid_chars = System.IO.Path.GetInvalidFileNameChars();
            for (int i = 0; i < filename.Length; ++i)
            {
                if (invalid_chars.Any(t => t == filename[i]))
                    res += subst;
                else
                    res += filename[i];
            }

            return res;
        }

        public static StringWrapper ToStringWrapper(this StringBuilder sb)
        {
            return new StringWrapper() { str = sb.ToString() };
        }

        public static string ToJson(this object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        /// <summary>
        /// removes all characters that aren't 0-9 dot or comma
        /// </summary>        
        public static string TrimNonNumericCharacters(this string s)
        {
            string res = "";

            for (int i = 0; i < s.Length; ++i)
            {
                if (char.IsNumber(s[i]) || s[i] == '.' || s[i] == ',') res += $"{s[i]}";
            }

            return res;
        }

        public static int ParseInt(this string s) { return int.Parse(s); }

        /// <summary>
        /// return yyyy-MM-dd HH:mm.ss representation
        /// </summary>        
        public static string InvarianteDateTime(this DateTime dt, string datesep = "-", string timesep = ":")
        {
            return $"{dt.InvariantDate(datesep)} {dt.InvariantTime(timesep)}";
        }

        /// <summary>
        /// return yyyy-MM-dd representation
        /// </summary>        
        public static string InvariantDate(this DateTime dt, string sep = "-")
        {
            return string.Format("{0:0000}{1}{2:00}{3}{4:00}", dt.Year, sep, dt.Month, sep, dt.Day);
        }

        /// <summary>
        /// return HH:mm.ss representation
        /// </summary>        
        public static string InvariantTime(this DateTime dt, string sep = "-")
        {
            return string.Format("{0:00}{1}{2:00}{3}{4:00}", dt.Hour, sep, dt.Minute, sep, dt.Second);
        }

        /// <summary>
        /// retrieve nr. of occurrence of given pattern through regex
        /// </summary>        
        public static int RegexMatch(this string s, string pattern, RegexOptions opt = RegexOptions.None)
        {
            return Regex.Matches(s, pattern, opt).Count;
        }

    }

    public class StringWrapperLineReader
    {

        public StringWrapper Strw { get; private set; }

        IEnumerator<string> en = null;

        bool has_next = false;

        public StringWrapperLineReader(StringWrapper strw)
        {
            Strw = strw;

            en = strw.Lines().GetEnumerator();

            has_next = en.MoveNext();
        }

        public bool HasNext() { return has_next; }

        public StringWrapper GetNext()
        {
            var res = new StringWrapper() { str = en.Current };
            has_next = en.MoveNext();
            return res;
        }

    }

    /// <summary>
    /// wrapper for memory optimized string argument passing
    /// </summary>
    public class StringWrapper
    {
        public string str;

        public StringWrapperLineReader LineReader()
        {
            return new StringWrapperLineReader(this);
        }

        public IEnumerable<string> Lines()
        {
            int back_off = 0;
            int off = 0;
            int maxlen = str.Length;

            while (off < maxlen)
            {
                while (off < maxlen && str[off] != '\r' && str[off] != '\n') ++off;
                if (off == maxlen)
                {
                    if (back_off != off)
                    {
                        var trimend_cnt = 0;
                        {
                            var j = off - 1;
                            while (j >= 0 && str[j] == '\r' || str[j] == '\n') { --j; ++trimend_cnt; }
                        }

                        yield return str.Substring(back_off, off - back_off + ((off == maxlen) ? 0 : 1));
                    }
                }
                ++off;

                if (off > 0 && str[off - 1] == '\r' && str[off] == '\n') ++off;

                if (back_off != off)
                {
                    var trimend_cnt = 0;
                    {
                        var j = off - 1;
                        while (j >= 0 && str[j] == '\r' || str[j] == '\n') { --j; ++trimend_cnt; }
                    }
                    yield return str.Substring(back_off, off - back_off - trimend_cnt);
                }

                back_off = off;
            }

        }

        public override string ToString()
        {
            return str;
        }

    }

}
