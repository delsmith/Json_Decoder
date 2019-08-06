using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json_Object
{
    internal class Json_String
    {
        internal string Value { get; set; }
        private static char c;

        internal Json_String(string text, ref int index)
        {
            Value = "";
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
        }

        private static Dictionary<char, string> lookup =
            new Dictionary<char, string>() {
                { '\\', "\\" }, { '\"', "\"" }, { '/', "/" },
                { 'b', "\x08" }, { '\f', "\x0c" }, 
                { 'n', "\x0a" }, { '\r', "\x0d" }, 
                { 't', "\x09" } 
            };

        private static string GetToken( string text, ref int index)
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
    }
}
