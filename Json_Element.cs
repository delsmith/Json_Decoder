using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace json_parse
{
    public enum Element { Object, Array, Member, String, Number, Literal, Invalid };

    class Json_Element
    {
        public Element Type { get; set; }
        public object Value { get; set; }
    }
}
