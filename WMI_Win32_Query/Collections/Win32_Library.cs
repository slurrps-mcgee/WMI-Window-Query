using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMI_Win32_Query.Collections
{
    /// <summary>
    /// Win32Library holds a Dictionary of Win32_Books
    /// </summary>
    public class Win32_Library : IEnumerable<KeyValuePair<string, Win32_Book>>
    {
        private IDictionary<string, Win32_Book> _dictionary;

        #region Constructors
        public Win32_Library()
        {
            _dictionary = new Dictionary<string, Win32_Book>();
        }

        public Win32_Library(IDictionary<string, Win32_Book> dictionary)
        {
            _dictionary = dictionary;
        }
        #endregion

        #region Add Remove
        public void Add(string key, Win32_Book value)
        {
            _dictionary.Add(key, value);
        }

        public void Remove(string key)
        {
            _dictionary.Remove(key);
        }
        #endregion

        #region Get Information
        public Win32_Book GetValueByKey(string key)
        {
            Win32_Book value;

            if (_dictionary.TryGetValue(key, out value))
                return value;
            return null;
        }

        public ICollection<string> Keys()
        {
            return _dictionary.Keys;
        }

        public ICollection<Win32_Book> Values()
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
        //Print Library
        public void PrintLibrary()
        {
            foreach (var item in _dictionary)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }

        public void PrintLibraryBook()
        {
            foreach(Win32_Book lib in _dictionary.Values)
            {
                foreach(var item in lib)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
                Console.WriteLine();
            }
        }
        #endregion

        #region Enumeration
        //Enumeration Implementation
        private IEnumerable<KeyValuePair<string, Win32_Book>> Events()
        {
            foreach (var item in _dictionary)
            {
                yield return item;
            }
        }

        public IEnumerator<KeyValuePair<string, Win32_Book>> GetEnumerator()
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
