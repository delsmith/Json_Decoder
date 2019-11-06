using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json_Decoder
{
    public class JArray : List<dynamic> { }
    public class JDict: Dictionary<string, dynamic> { }

    public class Json
    {
        #region Public Interface
        public static dynamic Parse(string text)
        {
            char utf_BOM = '\ufeff';
            int index = (text[0] == utf_BOM) ? 1 : 0;
            dynamic Value = Json_Value(text, ref index);

            SkipWS(text, ref index);
            if (text.Length > index)
                throw new Exception($"Parsing terminated before end-of-text at char #{index}");

            return Value;
        }

        public static dynamic Load(string filename)
        {
            return Parse(File.ReadAllText(filename));
        }
        #endregion

        internal static dynamic Json_Value(string text, ref int index)
        {
            char c = SkipWS(text, ref index);
            switch (c)
            {
                case '[': // JSON Array
                    index++;
                    return Json_Array(text, ref index);
                case '{': // JSON Object
                    index++;
                    return Json_Object(text, ref index);
                case '"': // JSON String
                    index++;
                    return Json_String(text, ref index);
                default:  // number or literal (or invalid)
                    return Json_Number(text, ref index);
            }
        }

        internal static JArray Json_Array(string text, ref int index)
        {
            JArray Value = new JArray { };

            char c= SkipWS(text, ref index);
            while (c != ']')
            {
                Value.Add(Json_Value(text, ref index));

                c = SkipWS(text, ref index);
                if (c == ',')
                    c = text[++index];
            }
            index++;
            SkipWS(text, ref index);
            return Value;
        }

        #region Json_Object
        internal static JDict Json_Object(string text, ref int index)
        {
            JDict JObject = new JDict { };
            char c = SkipWS(text, ref index);
            while (c != '}')
            {
                var next = Json_Member(text, ref index);
                if (!JObject.ContainsKey(next.Name))
                    JObject[next.Name] = next.Value;
                // When member name is repeated, create a List<object> to contain
                else if (JObject[next.Name].GetType().Name != "JArray")
                {
                    object first = JObject[next.Name];
                    JObject.Remove(next.Name);
                    JObject[next.Name] = new JArray() { first, next.Value };
                }
                else
                {
                    ((JArray)JObject[next.Name]).Add(next.Value);
                }

                if (text[index] == ',')
                    index++;
                c = SkipWS(text, ref index);
            }
            index++;
            SkipWS(text, ref index);
            return JObject;
        }

        internal class JMember
        {
            internal string Name { get; set; } = null;
            internal dynamic Value { get; set; } = null;
        }

        internal static JMember Json_Member(string text, ref int index)
        {
            JMember member = new JMember();
            int start = index;
            char c = SkipWS(text, ref index);
            if (c == '"')
            {
                try
                {
                    index++;
                    member.Name = Json_String(text, ref index);
                    c = SkipWS(text, ref index);
                    if (c == ':')
                    {
                        index++;
                        dynamic Val = Json_Value(text, ref index);
                        member.Value = Val;
                        SkipWS(text, ref index);
                        return member;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            throw new Exception($"Error: Invalid Object Member ['\"name\":value] at char #{index}\n[{text.Substring(index, 80)}]");
        }
        #endregion

        #region Json_String
        internal static string Json_String(string text, ref int index)
        {
            string Value = "";
            bool again = true;
            // scan to the next un-escaped double-quote
            while (again)
            {
                char c = text[index++];
                switch (c)
                {
                    // one double-quote is end of string
                    case '"':
                        if (index < text.Length && text[index] == '"')
                        {
                            Value += c;
                            index++;
                        }
                        else
                            again = false;
                        break;
                    // back-slash indicates escape sequence
                    case '\\':
                        string token = GetToken(text, ref index);
                        Value += token;
                        break;
                    // anything else gets appended as-is
                    default:
                        Value += c;
                        break;
                }
            }
            SkipWS(text, ref index);
            return Value;
        }

        private static Dictionary<char, string> lookup =
            new Dictionary<char, string>() {
                { '\\', "\\" }, { '\"', "\"" }, { '/', "/" },
                { 'b', "\x08" }, { '\f', "\x0c" },
                { 'n', "\x0a" }, { '\r', "\x0d" },
                { 't', "\x09" }
            };

        private static string GetToken(string text, ref int index)
        {
            int start = index;

            if (lookup.ContainsKey(text[index]))
                return lookup[text[index++]];
            else if (text[index] != 'u')
                return $"{text[index++]}";
            else
            {
                // validate hex value and encode as two octets (big-endian or Network order)
                string hex1 = text.Substring(index + 1, 2);
                string hex2 = text.Substring(index + 3, 2);
                index += 5;
                try { int intValue = Convert.ToInt32($"0x{hex1}{hex2}", 16); }
                catch { throw new Exception($"Invalid hex quartet '{hex1}:{hex2}' at char #{start + 1}"); }

                return $"\\x{hex1}\\x{hex2}";
            }
        }
        #endregion

        internal static dynamic Json_Number(string text, ref int index)
        {
            char c = SkipWS(text, ref index);
            int start = index;

            if (text.Length >= start + 4 && text.IndexOf("true", start, 4) == start)
            {
                index += 4;
                return true;
            }
            else if (text.Length >= start + 5 && text.IndexOf("false", start, 5) == start)
            {
                index += 5;
                return false;
            }
            else if (text.Length >= start + 4 && text.IndexOf("null", start, 4) == start)
            {
                index += 4;
                return null;
            }
            else
            {
                while (text.Length > index && ",}]".IndexOf((c = text[index])) < 0 && !IsWS(c)) index++;
                string sValue = text.Substring(start, index - start);
                if (Int64.TryParse(sValue, out long iValue))return iValue;
                else if (double.TryParse(sValue, out double dValue)) return dValue;
                else throw new Exception($"Invalid numeric value '{sValue}' at char #{start}");
            }
        }

        private static bool IsWS(char c) { return ("\x20\x09\x0a\x0d".IndexOf(c) >= 0); }
        internal static char SkipWS(string text, ref int index)
        {
            while (text.Length > index)
            {
                if (IsWS(text[index])) index++;
                else return text[index];
            }
            return '\n';
        }
    }
}
