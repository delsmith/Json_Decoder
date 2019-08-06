using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json_Object
{
    internal class Json_Number
    {
        // return numeric string or literal: null string or bool true/false
        internal object Value { get; set; }
        private static char c;

        internal Json_Number(string text, ref int index)
        {
            while (Json_Object.IsWS(c = text[index])) index++;
            int start = index;

            if (text.Length >= start + 4 && text.IndexOf("true",start,4) == start)
            {
                Value = true;
                index += 4;
            }
            else if (text.Length >= start + 5 && text.IndexOf("false",start,5) == start)
            {
                Value = false;
                index += 5;
            }
            else if (text.Length >= start + 4 && text.IndexOf("null",start,4) == start)
            {
                Value = null;
                index += 4;
            }
            else
            {
                while (text.Length > index && ",}]".IndexOf((c=text[index])) < 0 && !Json_Object.IsWS(c)) index++;
                string sValue = text.Substring(start, index - start);
                if (Int64.TryParse(sValue, out long iValue)) Value = iValue;
                else if (double.TryParse(sValue, out double dValue)) Value = dValue;
                else throw new Exception($"Invalid numeric value '{sValue}' and char #{index}");
            }
            while (text.Length > index && Json_Object.IsWS(c = text[index])) index++;
        }
    }
}
