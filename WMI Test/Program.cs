using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMI_Win32_Query.Queries;
using WMI_Win32_Query.Collections;

namespace WMI_Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Success Test
            List<string> drives = DriveQuery.GetDrives();
            List<Library> drivesLib = new List<Library>();
            foreach (string drive in drives)
            {
                drivesLib.Add(DriveQuery.GetSelectedDriveInformation(drive));
            }


            foreach(var drive in drivesLib)
            {
                foreach(var item in drive)
                {
                    Console.WriteLine(item.Key);
                    item.Value.Print();
                    Console.WriteLine();
                }
            }

            Console.ReadLine();
        }
    }
}
