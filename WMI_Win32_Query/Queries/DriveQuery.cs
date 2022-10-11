﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using WMI_Win32_Query.Collections;

namespace WMI_Win32_Query.Queries
{
    public sealed class DriveQuery
    {
        #region Get Drive and Partition List
        /// <summary>
        /// Returns a list of drive letters from drives attached to the system
        /// </summary>
        /// <returns>List<string></returns>
        public static List<string> GetDrives()
        {
            //Create a list to hold the drives
            List<string> driveList = new List<string>();

            //Go through each DriveInfo from the GetDrives method
            foreach (DriveInfo di in DriveInfo.GetDrives())
            {
                //Add the di.Name to the driveList
                driveList.Add(di.Name.Substring(0, 2));

            }

            //Return the list
            return driveList;
        }

        /// <summary>
        /// Returns a list of partitions for a provided drive letter string EX Format: C:
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <returns>List<string></returns>
        public static List<string> GetPartitions(string driveLetter)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher();

            List<string> partitions = new List<string>();

            try
            {
                searcher.Query.QueryString = $"select * from Win32_LogicalDisk where DeviceID = '{driveLetter}'";

                ////////////////////////////////////////////////////LOGICAL DISK////////////////////////////////////////////////////////
                foreach (ManagementObject d in searcher.Get())
                {
                    searcher.Query.QueryString = $"associators of {{{d.Path.RelativePath}}} where AssocClass = Win32_LogicalDiskToPartition";

                    ////////////////////////////////////////////////////PARTITION////////////////////////////////////////////////////////
                    foreach (ManagementObject p in searcher.Get())
                    {
                        searcher.Query.QueryString = $"Select * from Win32_DiskDrive where Name like'%PHYSICALDRIVE{p.Properties["Name"].Value.ToString().Substring(6, 1)}'";

                        ////////////////////////////////////////////////////DISK DRIVES////////////////////////////////////////////////////////
                        foreach (ManagementObject dd in searcher.Get())
                        {
                            searcher.Query.QueryString = $"associators of {{{dd.Path.RelativePath}}} where AssocClass = Win32_DiskDriveToDiskPartition";

                            ////////////////////////////////////////////////////DISK DRIVE PARTITIONS////////////////////////////////////////////////////////
                            foreach (ManagementObject pp in searcher.Get())
                            {
                                partitions.Add(pp.Properties["DeviceID"].Value.ToString().Substring(pp.Properties["DeviceID"].Value.ToString().Length - 1, 1));
                            }//Disk Partitions
                        }//Disk Drive
                    }//Logical Partition
                }//Logical Disk
            }
            catch
            {
                throw;
            }
            finally
            {
                searcher.Dispose();
            }

            return partitions;
        }
        #endregion

        #region Drive Space Calculations
        /// <summary>
        /// Calculates a given Book drive Dictionary Total Space
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>float</returns>
        public static float TotalSpace(Book driveDict)
        {
            return ConversionToGig(Convert.ToInt64(driveDict.GetValueByKey("Size")));
        }

        /// <summary>
        /// Calculates a given Book drive Dictionary Used Space
        /// </summary>
        /// <param name="driveDict"></param>
        /// <returns>float</returns>
        public static float UsedSpace(Book driveDict)
        {
            return (ConversionToGig(Convert.ToInt64(driveDict.GetValueByKey("Size"))) - ConversionToGig(Convert.ToInt64(driveDict.GetValueByKey("FreeSpace"))));
        }
        #endregion

        #region Get Drive or Partition Details
        /// <summary>
        /// Returns a Library of Books each book contains a drives Logical, Disk, and Partition information
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <returns></returns>
        public static Library GetSelectedDriveInformation(string driveLetter)
        {
            //Create driveNum variable and initialize it to empty
            string driveNum = string.Empty;
            //Create a New Library called driveDetails
            Library driveDetails = new Library();

            try
            {
                driveDetails.Add("Logical", GetLogicalInformation(driveLetter));
                driveDetails.Add("Disk", GetDiskInformation(driveLetter, out driveNum));

                #region Partitions
                //Create a List of strings and fill them with the found partitions from the provided driveLetter
                List<string> partitions = GetPartitions(driveLetter);

                //Loop through the list
                partitions.ForEach(x =>
                {
                    //Add each GetPartitionInformation Book to the Library
                    driveDetails.Add($"Partition {partitions.IndexOf(x)}", GetPartitionInformation(driveNum, partitions.IndexOf(x).ToString()));
                });
                #endregion
            }
            catch
            {
                throw;
            }
            
            return driveDetails;
        }

        //Gets only the selected partition info on top of the other details
        public static Library GetSelectedDrivePartitionDetails(string driveLetter, string partitionNum)
        {
            Library details = new Library();
            string diskNum = "";
            details.Add("Logical", GetLogicalInformation(driveLetter));
            details.Add("Disk", GetDiskInformation(driveLetter, out diskNum));
            details.Add("Partition", GetPartitionInformation(diskNum, partitionNum));

            return details;
        }
        #endregion

        #region Private Methods
        //Gets Data Table of Logical Drive information
        private static Book GetLogicalInformation(string driveLetter)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(driveLetter);

            Book logical = new Book();

            try
            {
                searcher.Query.QueryString = $"select * from Win32_LogicalDisk where DeviceID = '{driveLetter}'";

                ////////////////////////////////////////////////////LOGICAL DISK////////////////////////////////////////////////////////
                foreach (ManagementObject d in searcher.Get())
                {
                    logical.Add("Logical Disk", d.Properties["name"].Value);
                    logical.Add("OS Drive", ISOSDrive(driveLetter));
                    foreach (var property in d.Properties)
                    {
                        if (property.Value == null || property.Value.ToString() == string.Empty)
                        {
                            logical.Add(property.Name, "N/A");
                        }
                        else
                        {
                            //Check for arrays and print out
                            if (property.IsArray && property.Value != null)
                            {
                                string temp = "";
                                //Explicit cast value to array
                                Array array = (Array)property.Value;
                                //Loop through the array
                                foreach (var item in array)
                                {
                                    //Check if first item
                                    if (temp != "")
                                    {
                                        temp += $", {item.ToString()}";
                                    }
                                    else
                                    {
                                        temp += $"{item.ToString()}";
                                    }

                                }
                                logical.Add(property.Name, temp);
                            }
                            else
                            {
                                logical.Add(property.Name, property.Value);
                            }
                            
                            //Maybe calc size?
                        }
                    }
                }//Logical Disk
            }
            catch
            {
                throw;
            }
            finally
            {
                searcher.Dispose();
            }

            return logical;
        }

        //Gets specific Drive information
        private static Book GetDiskInformation(string driveLetter, out string diskNum)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(driveLetter);

            Book disk = new Book();
            diskNum = string.Empty;

            try
            {
                searcher.Query.QueryString = $"select * from Win32_LogicalDisk where DeviceID = '{driveLetter}'";

                ////////////////////////////////////////////////////LOGICAL DISK////////////////////////////////////////////////////////
                foreach (ManagementObject d in searcher.Get())
                {
                    searcher.Query.QueryString = $"associators of {{{d.Path.RelativePath}}} where AssocClass = Win32_LogicalDiskToPartition";

                    ////////////////////////////////////////////////////PARTITION////////////////////////////////////////////////////////
                    foreach (ManagementObject p in searcher.Get())
                    {
                        //Set Disk Number
                        diskNum = p.Properties["Name"].Value.ToString().Substring(6, 1);

                        searcher.Query.QueryString = $"Select * from Win32_DiskDrive where Name like'%PHYSICALDRIVE{diskNum}'";

                        ////////////////////////////////////////////////////DISK DRIVES////////////////////////////////////////////////////////
                        foreach (ManagementObject dd in searcher.Get())
                        {
                            disk.Add("Disk Drive", dd.Properties["name"].Value);
                            disk.Add("Drive Type", DriveType(Convert.ToInt32(diskNum)));
                            foreach (var property in dd.Properties)
                            {
                                if (property.Value == null || property.Value.ToString() == string.Empty)
                                {
                                    disk.Add(property.Name, "N/A");
                                }
                                else
                                {
                                    if (property.Name == "Size" || property.Name == "FreeSpace")
                                    {
                                        disk.Add(property.Name, $"{ConversionToGig(Convert.ToInt64(property.Value)).ToString("n2")} GB");
                                    }
                                    else
                                    {
                                        //Check for arrays and print out
                                        if (property.IsArray && property.Value != null)
                                        {
                                            string temp = "";
                                            //Explicit cast value to array
                                            Array array = (Array)property.Value;
                                            //Loop through the array
                                            foreach (var item in array)
                                            {
                                                //Check if first item
                                                if (temp != "")
                                                {
                                                    temp += $", {item.ToString()}";
                                                }
                                                else
                                                {
                                                    temp += $"{item.ToString()}";
                                                }

                                            }
                                            disk.Add(property.Name, temp);
                                        }
                                        else
                                        {
                                            disk.Add(property.Name, property.Value);
                                        }
                                    }
                                }
                            }
                        }//Disk Drive
                    }//Logical Partition
                }//Logical Disk

            }
            catch
            {
                throw;
            }
            finally
            {
                searcher.Dispose();
            }

            return disk;
        }

        private static Book GetPartitionInformation(string driveNum, string partitionNum)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(driveNum);

            Book partition = new Book();

            try
            {
                searcher.Query.QueryString = $"Select * from Win32_DiskPartition where DeviceId = 'Disk #{driveNum}, Partition #{partitionNum}'";

                ////////////////////////////////////////////////////DISK DRIVE PARTITIONS////////////////////////////////////////////////////////
                //Disk Partition Details
                foreach (ManagementObject pp in searcher.Get())
                {
                    partition.Add("Partition", pp.Properties["name"].Value);
                    foreach (var property in pp.Properties)
                    {
                        if (property.Value == null || property.Value.ToString() == string.Empty)
                        {
                            partition.Add(property.Name, "N/A");
                        }
                        else
                        {
                            if (property.Name == "Size" || property.Name == "FreeSpace")
                            {
                                partition.Add(property.Name, $"{ConversionToGig(Convert.ToInt64(property.Value)).ToString("n2")} GB");
                            }
                            else
                            {
                                //Check for arrays and print out
                                if (property.IsArray && property.Value != null)
                                {
                                    string temp = "";
                                    //Explicit cast value to array
                                    Array array = (Array)property.Value;
                                    //Loop through the array
                                    foreach (var item in array)
                                    {
                                        //Check if first item
                                        if (temp != "")
                                        {
                                            temp += $", {item.ToString()}";
                                        }
                                        else
                                        {
                                            temp += $"{item.ToString()}";
                                        }

                                    }
                                    partition.Add(property.Name, temp);
                                }
                                else
                                {
                                    partition.Add(property.Name, property.Value);
                                }
                            }
                        }
                    }
                }//Disk Partitions
            }
            catch
            {
                throw;
            }
            finally
            {
                searcher.Dispose();
            }

            return partition;
        }
        #endregion

        #region Public Methods
        #region Detect OS Drive
        public static bool ISOSDrive(string driveLetter)
        {
            //Pre: requires driveLetter to be initialized
            //Post: returns true or false
            //Purpose: To see if the drive selected is contains an OS

            //Create string driveLetter/Windows
            driveLetter += @"\Windows";
            //Check if the drive has a folder called Windows 
            if (Directory.Exists(driveLetter))
            {
                //Return true if it exists
                return true;
            }
            else
            {
                //Else returns false
                return false;
            }
        }//End Detect OS Drive
        #endregion

        #region Drive Type
        /// <summary>
        /// Returns the drive type based on the driveNumber provided
        /// </summary>
        /// <param name="driveNumber"></param>
        /// <returns>string</returns>
        public static string DriveType(int driveNumber)
        {
            ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM MSFT_PhysicalDisk where DeviceId = " + driveNumber.ToString());

            string type = "";
            scope.Connect();
            searcher.Scope = scope;

            try
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    switch (Convert.ToInt16(queryObj["MediaType"]))
                    {
                        case 1:
                            type = "Unspecified";
                            return type;

                        case 3:
                            type = "HDD";
                            return type;

                        case 4:
                            type = "SSD";
                            return type;

                        case 5:
                            type = "SCM";
                            return type;

                        default:
                            type = "Unspecified";
                            return type;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                searcher.Dispose();
            }

            return type;
        }
        #endregion

        #region Conversion Functions
        #region Conversion Variables
        //Constants for conveersions of different byt sizes
        private const float FLOAT_GIG_CONVERSION = 1073741824f; //Holds the float conversion number of GB per bit
        private const float FLOAT_TERA_CONVERSION = 0.0009765625F;//Holds the float conversion number for TB per bit
        #endregion

        /// <summary>
        /// Converts a provided Megabyte float to Gigabytes
        /// </summary>
        /// <param name="conversionNum"></param>
        /// <returns>float</returns>
        public static float ConversionToGig(float conversionNum)
        {
            //Pre: Needs conversionNum to be initialized
            //Post: Returns gigConversion number to the program
            //Purpose: To convert the bytes number that is incoming to gigabytes

            //Set the gigConversion to 0
            float gigConversion;
            //Grabs the conversionNum from the one passed into the function then 
            //divides by the Float_GIG_CONVERSION Constant
            gigConversion = conversionNum / FLOAT_GIG_CONVERSION;

            return gigConversion; //Returns the variable gigConversion
        }//End ConversionToGig

        /// <summary>
        /// Converts a provided Gigabyte float to Terabytes
        /// </summary>
        /// <param name="ConversionNum"></param>
        /// <returns>float</returns>
        public static float ConversionToTer(float ConversionNum)
        {
            //Pre: Needs conversionNum to be initialized
            //Post: Returns teraConversion number to the program
            //Purpose: To convert the bytes number that is incoming to terabytes

            //Set the teraConversion to 0
            float teraConversion;
            //Grabs the conversionNum from the one passed into the function then 
            //divides by the Float_TERA_CONVERSION Constant
            teraConversion = ConversionNum / FLOAT_TERA_CONVERSION;

            return teraConversion;
        }//End ConversionToTer
        #endregion
        #endregion
    }
}
