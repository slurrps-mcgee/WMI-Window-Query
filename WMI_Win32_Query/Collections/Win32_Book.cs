using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMI_Win32_Query.Collections
{
    /// <summary>
    /// A Win32Book holds a Dictionary of all properties from a Win32 Class
    /// </summary>
    public class Win32_Book : IEnumerable<KeyValuePair<string, object>>
    {
        private IDictionary<string, object> _dictionary;

        #region Constructors
        public Win32_Book()
        {
            _dictionary = new Dictionary<string, object>();
        }

        public Win32_Book(IDictionary<string, object> dictionary)
        {
            _dictionary = dictionary;
        }
        #endregion

        #region Add Remove
        public void Add(string key, object value)
        {
            _dictionary.Add(key, value);
        }

        public void Remove(string key)
        {
            _dictionary.Remove(key);
        }
        #endregion

        #region Get Information
        public object GetValueByKey(string key)
        {
            object value;

            if (_dictionary.TryGetValue(key, out value))
                return value;
            return null;
        }

        public ICollection<string> Keys()
        {
            return _dictionary.Keys;
        }

        public ICollection<Object> Values()
        {
            return _dictionary.Values;
        }
        #endregion

        public void Clear()
        {
            _dictionary.Clear();
        }

        public int Count()
        {
            return _dictionary.Count;
        }

        #region Console
        //Printing
        //Print Book
        public void PrintBook()
        {
            foreach(var key in _dictionary)
            {
                Console.WriteLine($"{key.Key}: {key.Value}");
            }
        }
        #endregion

        #region Enumeration
        //Enumeration Implementation
        private IEnumerable<KeyValuePair<string, object>> Events()
        {
            foreach (var item in _dictionary)
            {
                yield return item;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Events().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
