namespace json_parse
{
    partial class Json_Value
    {
        public enum TokenType {
            j_number,
            j_string,
            begin_object,
            end_object,
            begin_array,
            end_array,
            member_tag,
            value_tag,
            j_ws,
            j_true,
            j_false,
            j_null,
            j_quote,
            j_escape,
            j_slash,
            j_ff,
            j_lf,
            j_cr,
            j_tab,
            j_4hex,
            j_invalid
        };
    }
}
