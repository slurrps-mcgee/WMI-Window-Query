using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMI_Win32_Query.Queries;
using WMI_Win32_Query.Collections;
using WMI_Win32_Query.Helpers;

namespace WMI_Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //Get All Drives Information
            List<Win32_Library> drivesLib = Drive_Query.GetAllDrivesInformation();
            //Print each item to Console
            drivesLib.ForEach(drive => drive.PrintLibraryBook());

            Helpers.PrintToFile(drivesLib, "Drives");

            //Get Single Drive Information
            Win32_Library driveLib = Drive_Query.GetSelectedDriveInformation("C:");
            //Print library to console
            driveLib.PrintLibraryBook();

            Helpers.PrintToFile(driveLib, "Drive-C");

            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds} milliseconds");
            watch.Reset();

            Console.ReadLine();
        }
    }
}
