# Json_Object
 .Net library for JSON parsing
 
    After failing to find anything in .Net that was as simple to use as either
     PYTHON  - json.load(fs)   or
     Powershell '$data = $text | ConvertFrom-Json'
    I decided to make one.

    It conforms to RFC-8259
    It returns a C# object which depends on the JSON type of the root 'value'

    (Caveat: It doesn't have exhaustive exception handling but if your JSON syntax is correct, it will work)


    The Type of each element depends on the type of JSON value. 
     * Numbers are returned as
      long - if they can be parsed as integers
      double - if they can be parsed as doubles
     * Strings are returned as 'string' objects
     * 'true' and 'false' are return as 'bool's
     * 'null' is returned as a 'null' object
     * Arrays [...] are returned as 'List<object>' objects, allowing any mixture of Value types
     * Objects {...} are returned as 'Dictionary<string,object>' objects, containing 'name:value' members
      Duplicate member names ARE allowed, in which case the Value object is replaced with a 'List<object>' 
       List items can be any Value type

To decode a JSON file use this syntax

-------------------

     static void Main(string[] args)
     {
         object data;
         // read JSON file into an Object
         try
         {
              data = Json_Decoder.Json.Load(args[0]);
         }
         catch (Exception e)
         {
             Console.WriteLine(e.Message);
         }
      }     

-------------------

To decode a JSON string use this syntax

-------------------
      
    static void Main(string[] args)
    {
       string json = File.ReadAllText(args[0]);
       object data;
       // parse JSON string into an Object
       try
       {
           data = Json_Decoder.Json.Parse(json);
       }
       catch (Exception e)
       {
           Console.WriteLine(e.Message);
       }
    }     
-------------------

A few 'txt' files are included, embodying the cases from the RFC

Comments welcome.
