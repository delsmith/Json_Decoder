using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Json_Decoder;

namespace Test_Json_Decoder
{
    //TODO: create a List with error reports as a 'property'

    class json_test
    {
        static void Main(string[] args)
        {
            // read JSON file into an Object
            try
            {
                dynamic content = Json.Load(args[0]);
                var v1 = content.Item("data");
                v1 = content.Item("dxta");
                var v2 = content.Item("data.1");
                v2 = content.Item("data.2");
                var v3 = content.Item("data.1[1]");
                v3 = content.Item("data.1[500]");
                var v4 = content.Item("data.1[1].frequency");
                v4 = content.Item("data.1[1].foo");
                v4 = content.Item("data.1[500].frequency");

                foreach (string key in content.Keys)
                {
                    Console.WriteLine($"{key}: {content[key]}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
