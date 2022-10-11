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
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //Create a list of strings to hold the drive letters
            List<string> drives = DriveQuery.GetDrives();

            ////List<string> partitions = DriveQuery.GetPartitions("C:");

            //drives.ForEach(drive =>
            //{
            //    Console.WriteLine($"{drive}");
            //    DriveQuery.GetPartitions(drive).ForEach(partition => Console.WriteLine(partition));
            //    Console.WriteLine();
            //});

            //Create a list of libraries to hold the drives information
            //Each library is a single drive (logical, disk, and partitions information)
            List<Library> drivesLib = new List<Library>();

            //Loop through the drive letters adding the selected drives information to the list of libraries
            foreach (string drive in drives)
            {
                //Each Call to GetSelected Drive Info adds the entire drives information to the library
                drivesLib.Add(DriveQuery.GetSelectedDriveInformation(drive));
            }

            //Just an example of how to print out the information
            foreach (var drive in drivesLib)
            {
                foreach (var item in drive)
                {
                    Console.WriteLine(item.Key);
                    item.Value.Print();
                    Console.WriteLine();
                }
            }

            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds} milliseconds");
            watch.Reset();

            Console.ReadLine();
        }
    }
}
