using System;
namespace Dictionary
{
    class Program
    {
        static void Main(string[] args)
        {

            _Dictionary dictionary1 = new _Dictionary();
            dictionary1.Add("key1", "val1");
            dictionary1.Add("key2", "val2");
            dictionary1.Add("key3", "val3");
            dictionary1.Add("key4", "val4");
            dictionary1.Add("key5", "val5");
            dictionary1.Add("key6", "val6");
            dictionary1.Add("key7", "val7");
            dictionary1.Add("key8", "val8");
            Console.WriteLine(dictionary1);
            string key0 = "key1";
            Console.WriteLine(dictionary1[key0]);
            Console.WriteLine(dictionary1.Count);
            foreach (var key in dictionary1.Keys)
            {
                Console.WriteLine(key);
            }
            foreach (var value in dictionary1.Values)
            {
                Console.WriteLine(value);
            }
            var key2 = "key1";
            Console.WriteLine(dictionary1.ContainsKey(key2));
            key2 = "key4";
            Console.WriteLine(dictionary1.ContainsKey(key2));
            dictionary1.Clear();
            Console.WriteLine(dictionary1.Count);
            string val1 = "val1";
            Console.WriteLine(dictionary1.ContainsValue(val1));
            KeyValuePair[] keyValuePair =
            {  new KeyValuePair() {Key = "key01", Value = "val01"},
                new KeyValuePair() {Key = "key02", Value = "val02"},

            };
            dictionary1._CopyTo(keyValuePair, 3);
            foreach (var element in keyValuePair)
            {
                Console.WriteLine(element.ToString());
            }
            string key3 = "key3";
            dictionary1.Remove(key3);
            foreach (var key in dictionary1.Keys)
            {
                Console.WriteLine(key);
            }
            foreach (var value in dictionary1.Values)
            {
                Console.WriteLine(value);
            }
        }
    }
}
