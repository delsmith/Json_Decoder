using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json_Decoder
{
    public class Json
    {
        private static char c;
        internal static bool IsWS(char c) { return ("\x20\x09\x0a\x0d".IndexOf(c) >= 0); }
        internal class Json_List : List<object> { }

        #region Public Interface
        public static object Parse(string text)
        {
            object Value;
            int index = 0;
            Value = Json_Value(text, ref index);
            while (text.Length > index && IsWS(c = text[index])) index++;
            if (text.Length > index)
                throw new Exception($"Parsing terminated before end-of-text at char #{index}");
            return Value;
        }

        public static object Load(string filename)
        {
            return Parse(File.ReadAllText(filename));
        }
        #endregion

        internal static object Json_Value(string text, ref int index)
        {
            while (IsWS(c = text[index])) index++;
            switch (c)
            {
                case '[':
                    return Json_Array(text, ref index);
                case '{':
                    return Json_Object(text, ref index);
                case '"':
                    return Json_String(text, ref index);
                default: // number, literal or invalid
                    try
                    {
                        return Json_Number(text, ref index);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
            }
        }

        internal static object Json_Array(string text, ref int index)
        {
            List<object> Value = new List<object> { };

            index++;

            while (text[index] != ']')
            {
                var next = Json_Value(text, ref index);
                Value.Add(next);
                if (text[index] == ',')
                    index++;
                while (IsWS(text[index])) index++;
            }
            index++;
            return Value;
        }

        #region Json_Object
        internal static object Json_Object(string text, ref int index)
        {
            Dictionary<string, object> JObject = new Dictionary<string, object> { };
            index++;
            while (IsWS(c = text[index])) index++;
            while (text[index] != '}')
            {
                var next = Json_Member(text, ref index);
                string key = next.Name;

                if (!JObject.ContainsKey(key))
                    JObject[key] = next.Value;
                // When member name is repeated, create a List<object> to contain
                else if (JObject[key].GetType().Name != "Json_List")
                {
                    object first = JObject[key];
                    JObject.Remove(key);
                    JObject[key] = new Json_List() { first, next.Value };
                }
                else
                {
                    ((Json_List)JObject[key]).Add(next.Value);
                }

                if (text[index] == ',')
                    index++;
                while (IsWS(c = text[index])) index++;
            }
            c = text[index];
            index++;
            return JObject;
        }

        internal class JMember
        {
            internal string Name { get; set; } = null;
            internal object Value { get; set; } = null;
        }

        internal static JMember Json_Member(string text, ref int index)
        {
            JMember member = new JMember();
            int start = index;
            while (IsWS(c = text[index])) index++;
            if (c == '"')
            {
                try
                {
                    member.Name = Json_String(text, ref index);
                    while (IsWS(c = text[index])) index++;
                    if (c == ':')
                    {
                        index++;
                        member.Value = Json_Value(text, ref index);
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
            index++;
            // scan to the next un-escaped double-quote
            while (again)
            {
                c = text[index++];
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

        internal static object Json_Number(string text, ref int index)
        {
            object Value;

            while (IsWS(c = text[index])) index++;
            int start = index;

            if (text.Length >= start + 4 && text.IndexOf("true", start, 4) == start)
            {
                Value = true;
                index += 4;
            }
            else if (text.Length >= start + 5 && text.IndexOf("false", start, 5) == start)
            {
                Value = false;
                index += 5;
            }
            else if (text.Length >= start + 4 && text.IndexOf("null", start, 4) == start)
            {
                Value = null;
                index += 4;
            }
            else
            {
                while (text.Length > index && ",}]".IndexOf((c = text[index])) < 0 && !IsWS(c)) index++;
                string sValue = text.Substring(start, index - start);
                if (Int64.TryParse(sValue, out long iValue)) Value = iValue;
                else if (double.TryParse(sValue, out double dValue)) Value = dValue;
                else throw new Exception($"Invalid numeric value '{sValue}' and char #{index}");
            }
            while (text.Length > index && IsWS(c = text[index])) index++;

            return Value;
        }
    }
}
