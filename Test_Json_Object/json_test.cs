using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Json_Object;

namespace Test_Json_Object
{
    class json_test
    {
        static void Main(string[] args)
        {
            Json_Object.Json_Value data;

            // read JSON file into an Object
            try
            {
                 data = Json_Object.Json_Value.Read(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // parse JSON string into an Object
            try
            {
                string json = File.ReadAllText(args[0]);
                data = Json_Object.Json_Value.Parse(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
