using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMI_Win32_Query.Collections
{
    public class Library : IEnumerable<KeyValuePair<string, Book>>
    {
        private IDictionary<string, Book> _dictionary;

        public Library()
        {
            _dictionary = new Dictionary<string, Book>();
        }

        public Library(IDictionary<string, Book> dictionary)
        {
            _dictionary = dictionary;
        }



        public void Add(string key, Book value)
        {
            _dictionary.Add(key, value);
        }

        public Book GetValueByKey(string key)
        {
            Book value;

            if (_dictionary.TryGetValue(key, out value))
                return value;
            return null;
        }

        //Enumeration Implementation
        private IEnumerable<KeyValuePair<string, Book>> Events()
        {
            foreach(var item in _dictionary)
            {
                yield return item;
            }
        }

        public IEnumerator<KeyValuePair<string, Book>> GetEnumerator()
        {
            return Events().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Get dictionary
        public IDictionary<string, Book> GetDictionary()
        {
            return _dictionary;
        }

        
    }
}
