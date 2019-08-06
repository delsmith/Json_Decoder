using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json_Object
{
    class Json_parse
    {
        static void Main(string[] args)
        {
            try
            {
                //string json = File.ReadAllText(args[0]);
                //data = Json_Object.Parse(json);
                var data = Json_Object.Read(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
