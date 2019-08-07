using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Json_Decoder;

namespace Test_Json_Decoder
{
    //TODO: create a List with error reports as a 'property'

    class json_test
    {
        static void Main(string[] args)
        {
            object content;

            // read JSON file into an Object
            try
            {
                 content = Json_Decoder.Json.Load(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // parse JSON string into an Object
            try
            {
                string json = File.ReadAllText(args[0]);
                content = Json_Decoder.Json.Parse(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
