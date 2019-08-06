using System.Collections.Generic;

namespace Json_Object
{
    internal class Json_Array
    {
        internal List<object> Value = new List<object> { };

        internal Json_Array(string text, ref int index)
        {
            index++;

            while (text[index] != ']')
            {
                var next = new Json_Value(text, ref index);
                Value.Add(next.Value);
                if (text[index] == ',')
                    index++;
                while (Json_Object.IsWS(text[index])) index++;
            }
            index++;
        }
    }
}