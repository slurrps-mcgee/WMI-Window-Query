using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMI_Win32_Query.Collections
{
    public class Book
    {
        private IDictionary<string, object> _dictionary;

        public Book()
        {
            _dictionary = new Dictionary<string, object>();
        }

        public Book(IDictionary<string, object> dictionary)
        {
            _dictionary = dictionary;
        }

        public void Add(string key, object value)
        {
            _dictionary.Add(key, value);
        }

        public object GetValueByKey(string key)
        {
            object value;

            if (_dictionary.TryGetValue(key, out value))
                return value;
            return null;
        }

        public void Print()
        {
            foreach(var key in _dictionary)
            {
                Console.WriteLine($"{key.Key}: {key.Value}");
            }
        }

        public IDictionary<string, object> GetDictionary()
        {
            return _dictionary;
        }
    }
}
