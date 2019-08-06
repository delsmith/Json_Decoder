using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Json_Object
{
    public class Json_Value
    {
        #region Public Interface
        public static Json_Value Parse(string text)
        {
            int index = 0;
            return new Json_Value(text, ref index);
        }

        public static Json_Value Read(string filename)
        {
            return Parse(File.ReadAllText(filename));
        }
        #endregion

        private static char c;
        internal object Value;

        internal Json_Value(string text, ref int index)
        {
            while (Json_Object.IsWS(c=text[index])) index++;
            switch (c)
            {
                case '{': Value = new Json_Object(text, ref index).Value;
                    break;
                case '[': Value = new Json_Array(text, ref index).Value;
                    break;
                case '"': Value = new Json_String(text, ref index).Value;
                    break;
                default: // number, literal or invalid
                    try
                    {
                        Value = new Json_Number(text, ref index).Value;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    break;
            }
        }
    }
}
