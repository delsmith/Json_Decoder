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
            // read JSON file into an Object
            try
            {
                var content = Json.Load(args[0]);
                foreach(string key in content.Keys)
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
