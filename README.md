# Json_Object
 .Net library for JSON parsing
 
After failing to find anything in .Net that was as simple to use as either
 PYTHON  - json.load(fs)   or
 Poweshell '$data = $text | ConvertFrom-Json'
 
I decided to make one.
I can't believe there isn't one but this seems pretty good.
(Caveat: It doesn't have exhaustive exception handling but if your JSON syntax is correct, it will work)

It conforms to RFC-8259
It returns a C# object of Type 'Json_Value'

The Type of the 'Value' element depends on the type of JSON value
    Numbers are returned as
        long - if they can be parsed as integers
        double - if they can be parsed as doubles
    Strings are returned as 'string' objects
    true and false are return as 'bool'
    null is returned as a 'null' object
    Arrays [...] are returned as 'List<object>' objects, allowing any mixture of Value types
    Objects {...} are returned as 'Dict<string,object>' objects, containing 'name:value' members
       Duplicate member names ARE allowed, in which case the Value will be a 'List<object>' 
       As with Arrays, the List items can be any Value type

To decode a JSON file use this syntax
-------------------
  static void Main(string[] args)
  {
      Json_Object.Json_Value data;..
      // read JSON file into an Object
      try
      {
           data = Json_Object.Json_Value.Read(args[0]);
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
      
      Json_Object.Json_Value data;
      // parse JSON string into an Object
      try
      {
          data = Json_Object.Json_Value.Parse(json);
      }
      catch (Exception e)
      {
          Console.WriteLine(e.Message);
      }
   }     
-------------------

A few 'txt' files are included, embodying the cases from the RFC

Comments welcome.
