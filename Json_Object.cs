using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json_Object
{
    public class Json_Object
    {

    #region Public Interface
        public static Json_Object Parse(string text)
        {
            if(text[0] != '{')
                throw new Exception("Invalid JSON object: must start with '{'");
            int index = 1;
            return new Json_Object(text, ref index);
        }

        public static Json_Object Read(string filename)
        {
            return Parse(File.ReadAllText(filename));
        }
    #endregion

    #region Properties, helpers
        internal Dictionary<string, object> Value = new Dictionary<string, object> { };
        private static char c;
        internal static bool IsWS(char c) { return ("\x20\x09\x0a\x0d".IndexOf(c) >= 0); }
        internal class Json_List : List<object> { }
    #endregion

    #region Object builder
        internal Json_Object(string text, ref int index)
        {
            index++;
            while (Json_Object.IsWS(c = text[index])) index++;
            while (text[index] != '}')
            {
                var next = new Json_Member(text, ref index);
                string key = next.Name;

                if(! Value.ContainsKey(key))
                    Value[key] = next.Value;
                // When member name is repeated, create a List<object> to contain
                else if( Value[key].GetType().Name != "Json_List")
                {
                    object first = Value[key];
                    Value.Remove(key);
                    Value[key] = new Json_List() { first, next.Value };
                }
                else
                {
                   ((Json_List)Value[key]).Add(next.Value);
                }

                if (text[index] == ',')
                    index++;
                while (Json_Object.IsWS(c = text[index])) index++;
            }
            c = text[index];
            index++;
        }
    #endregion

    #region build a Name:Value pair
        internal class Json_Member
        {
            internal string Name { get; set; }
            internal object Value { get; set; }

            internal Json_Member(string text, ref int index)
            {
                int start = index;
                while (Json_Object.IsWS(c = text[index])) index++;
                if (c == '"')
                {
                    try
                    {
                        Name = new Json_String(text, ref index).Value;
                        while (Json_Object.IsWS(c = text[index])) index++;
                        if (c == ':')
                        {
                            index++;
                            Value = new Json_Value(text, ref index).Value;
                            return;
                        }
                    }
                    catch(Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                throw new Exception($"Error: Invalid Object Member ['\"name\":value] at char #{index}\n[{text.Substring(index, 80)}]");
            }
        }
    #endregion

    }
}
