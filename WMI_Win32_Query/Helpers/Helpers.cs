using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using WMI_Win32_Query.Collections;

namespace WMI_Win32_Query.Helpers
{
    public static class Helpers
    {
        //Print out objects to a specified filename
        //TODO: move file location to a specified one in an optional parameter
        public static void PrintToFile<T>(T value, string fileName)
        {
            string path = Directory.GetCurrentDirectory();

            using (StreamWriter sw = new StreamWriter(Path.Combine(path, $"{fileName}.txt")))
            {
                var json = JsonConvert.SerializeObject(value, Formatting.Indented);
                sw.WriteLine(json);
            }
        }

    }
}
