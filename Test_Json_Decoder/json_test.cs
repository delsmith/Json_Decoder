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
            string tag;
            dynamic content;
            // read JSON file into an Object
            try
            {
                content = Json.Load(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            while(! string.IsNullOrEmpty( (tag = Console.ReadLine())))
            {
                var v1 = content.Item(tag);
                Console.WriteLine($"'tag' {tag} contains {v1}");
            }
        }
    }
}
