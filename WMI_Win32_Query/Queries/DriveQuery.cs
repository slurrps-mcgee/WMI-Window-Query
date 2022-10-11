using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using WMI_Win32_Query.Collections;

namespace WMI_Win32_Query.Queries
{
    public static class DriveQuery
    {

        //Gets the drives attached to the computer
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

        //Maybe private??
        //Gets a list of partitions for a drive providing a drive letter
        public static List<string> GetPartitions(string driveLetter)
        {
            List<string> partitions = new List<string>();


            //Foreach loop to go through the managementObjects in driveQuery
            try
            {
                //New Query text to search LogicalDisk via drive letter
                var logicalQueryText = string.Format($"select * from Win32_LogicalDisk where DeviceID = '{driveLetter}'");
                //Create a new managementObjectSearcher giving it the query
                var logicalQuery = new ManagementObjectSearcher(logicalQueryText);

                ////////////////////////////////////////////////////LOGICAL DISK////////////////////////////////////////////////////////
                //Foreach loop to go through the managementObjects in logicalQuery
                foreach (ManagementObject d in logicalQuery.Get())
                {
                    //New Query text to associate Win32_LogicalDisk to DiskPartition
                    var partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", d.Path.RelativePath);
                    //Create a new managementObjectSearcher giving it the query
                    var partitionQuery = new ManagementObjectSearcher(partitionQueryText);

                    ////////////////////////////////////////////////////PARTITION////////////////////////////////////////////////////////
                    //Loop to Search the ManagementObjects in the partitionQuery
                    foreach (ManagementObject p in partitionQuery.Get())
                    {
                        //Get disk number
                        string diskNum = p.Properties["Name"].Value.ToString().Substring(6, 1); //Disk Number of logical disk partition

                        ////New Query text to search DiskDrive searching the Physical Drive number
                        var diskDriveQueryText = string.Format($"Select * from Win32_DiskDrive where Name like'%PHYSICALDRIVE{diskNum}'");
                        //Create a new managementObjectSearcher giving it the query
                        var diskDriveQuery = new ManagementObjectSearcher(diskDriveQueryText);

                        ////////////////////////////////////////////////////DISK DRIVES////////////////////////////////////////////////////////
                        //Loop to Search the ManagementObjects in the query
                        foreach (ManagementObject dd in diskDriveQuery.Get())
                        {
                            //New query text to associate Win32_DiskDrive to DiskPartition
                            partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", dd.Path.RelativePath);
                            //Create a new managementObjectSearcher giving it the query
                            partitionQuery = new ManagementObjectSearcher(partitionQueryText);

                            ////////////////////////////////////////////////////DISK DRIVE PARTITIONS////////////////////////////////////////////////////////
                            //Disk Partition Details
                            foreach (ManagementObject pp in partitionQuery.Get())
                            {
                                partitions.Add(pp.Properties["DeviceID"].Value.ToString().Substring(pp.Properties["DeviceID"].Value.ToString().Length - 1, 1));
                            }//Disk Partitions
                        }//Disk Drive
                    }//Logical Partition
                }//Logical Disk
            }
            catch (Exception ex)
            {
                //MessageBox.Show
            }
            return partitions;
        }

        #region Drive Methods
        //Used to get space calculations
        public static float TotalSpace(Book dict)
        {
            return ConversionToGig(Convert.ToInt64(dict.GetValueByKey("Size")));
        }

        public static float UsedSpace(Book dict)
        {
            return (ConversionToGig(Convert.ToInt64(dict.GetValueByKey("Size"))) - ConversionToGig(Convert.ToInt64(dict.GetValueByKey("FreeSpace"))));
        }
        #endregion

        //Gets all information and all partition info
        public static Library GetSelectedDriveInformation(string driveLetter)
        {
            string driveNum = "";
            Library details = new Library();
            details.Add("Logical", GetLogicalInformation(driveLetter));
            details.Add("Disk", GetDiskInformation(driveLetter, out driveNum));

            List<string> list = GetPartitions(driveLetter);
            list.ForEach(x =>
            {
                details.Add($"Partition {list.IndexOf(x)}", GetPartitionInformation(driveNum, list.IndexOf(x).ToString()));
            });

            return details;
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

        #region Private
        //Gets Data Table of Logical Drive information
        private static Book GetLogicalInformation(string driveLetter)
        {
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Property");
            //dt.Columns.Add("Value");
            Book logical = new Book();

            //Foreach loop to go through the managementObjects in driveQuery
            try
            {
                //New Query text to search LogicalDisk via drive letter
                var logicalQueryText = string.Format($"select * from Win32_LogicalDisk where DeviceID = '{driveLetter}'");
                //Create a new managementObjectSearcher giving it the query
                var logicalQuery = new ManagementObjectSearcher(logicalQueryText);


                ////////////////////////////////////////////////////LOGICAL DISK////////////////////////////////////////////////////////
                //Foreach loop to go through the managementObjects in logicalQuery
                foreach (ManagementObject d in logicalQuery.Get())
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
                            //if (property.Name == "Size" || property.Name == "FreeSpace")
                            //{
                            //    logical.Add(property.Name, $"{DriveInfoClass.ConversionToGig(Convert.ToInt64(property.Value)).ToString("n2")} GB");
                            //}
                            //else
                            //{

                            //}
                        }
                    }
                    //size = DriveInfoClass.ConversionToGig(Convert.ToInt64(d.Properties["Size"].Value));
                    //freeSpace = DriveInfoClass.ConversionToGig(Convert.ToInt64(d.Properties["FreeSpace"].Value));
                }//Logical Disk
            }
            catch (Exception ex)
            {
                //MessageBox.Show
            }

            return logical;
        }

        //Gets specific Drive information
        private static Book GetDiskInformation(string driveLetter, out string diskNum)
        {
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Property");
            //dt.Columns.Add("Value");
            Book disk = new Book();
            diskNum = "";

            //Foreach loop to go through the managementObjects in driveQuery
            try
            {
                #region Query
                //New Query text to search LogicalDisk via drive letter
                var logicalQueryText = string.Format($"select * from Win32_LogicalDisk where DeviceID = '{driveLetter}'");
                //Create a new managementObjectSearcher giving it the query
                var logicalQuery = new ManagementObjectSearcher(logicalQueryText);
                #endregion
                ////////////////////////////////////////////////////LOGICAL DISK////////////////////////////////////////////////////////
                //Foreach loop to go through the managementObjects in logicalQuery
                foreach (ManagementObject d in logicalQuery.Get())
                {
                    #region Query
                    //New Query text to associate Win32_LogicalDisk to DiskPartition
                    var partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", d.Path.RelativePath);
                    //Create a new managementObjectSearcher giving it the query
                    var partitionQuery = new ManagementObjectSearcher(partitionQueryText);
                    #endregion
                    ////////////////////////////////////////////////////PARTITION////////////////////////////////////////////////////////
                    //Loop to Search the ManagementObjects in the partitionQuery
                    foreach (ManagementObject p in partitionQuery.Get())
                    {
                        //Get disk number
                        diskNum = p.Properties["Name"].Value.ToString().Substring(6, 1); //Disk Number of logical disk partition

                        #region Query
                        ////New Query text to search DiskDrive searching the Physical Drive number
                        var diskDriveQueryText = string.Format($"Select * from Win32_DiskDrive where Name like'%PHYSICALDRIVE{diskNum}'");
                        //Create a new managementObjectSearcher giving it the query
                        var diskDriveQuery = new ManagementObjectSearcher(diskDriveQueryText);
                        #endregion
                        ////////////////////////////////////////////////////DISK DRIVES////////////////////////////////////////////////////////
                        //Loop to Search the ManagementObjects in the query
                        foreach (ManagementObject dd in diskDriveQuery.Get())
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
            catch (Exception ex)
            {
                //MessageBox.Show
            }

            return disk;
        }

        private static Book GetPartitionInformation(string driveNum, string partitionNum)
        {
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Property");
            //dt.Columns.Add("Value");
            Book partition = new Book();

            //New query text to associate Win32_DiskDrive to DiskPartition
            var partitionQueryText = string.Format($"Select * from Win32_DiskPartition where DeviceId = 'Disk #{driveNum}, Partition #{partitionNum}'");
            //Create a new managementObjectSearcher giving it the query
            var partitionQuery = new ManagementObjectSearcher(partitionQueryText);

            ////////////////////////////////////////////////////DISK DRIVE PARTITIONS////////////////////////////////////////////////////////
            //Disk Partition Details
            foreach (ManagementObject pp in partitionQuery.Get())
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
            ////Sorting the Table
            //DataView dv = dt.DefaultView;
            //dv.Sort = "Property";
            //DataTable sortedDT = dv.ToTable();


            return partition;
        }
        #endregion

        //Helper Functions

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
        //Some test code to get the drive type SSD HDD ect...
        private static string DriveType(int driveNumber)
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

            }

            searcher.Dispose();

            return type;
        }
        #endregion

        #region Conversion Functions
        #region Conversion
        //Constants for conveersions of different byt sizes
        const float FLOAT_GIG_CONVERSION = 1073741824f; //Holds the float conversion number of GB per bit
        const float FLOAT_TERA_CONVERSION = 0.0009765625F;//Holds the float conversion number for TB per bit
        #endregion

        //Convert bytes to Gigabytes used to display correct drive information
        public static float ConversionToGig(float conversionNum)
        {
            //Pre: Needs conversionNum to be initialized
            //Pose: Returns gigConversion number to the program
            //Purpose: To convert the bytes number that is incoming to gigabytes

            //Set the gigConversion to 0
            float gigConversion;
            //Grabs the conversionNum from the one passed into the function then 
            //divides by the Float_GIG_CONVERSION Constant
            gigConversion = conversionNum / FLOAT_GIG_CONVERSION;

            return gigConversion; //Returns the variable gigConversion
        }//End ConversionToGig

        //Convert bytes to TeraBytes used to display correct drive information
        public static float ConversionToTer(float ConversionNum)
        {
            //Pre: Needs conversionNum to be initialized
            //Pose: Returns teraConversion number to the program
            //Purpose: To convert the bytes number that is incoming to terabytes

            //Set the teraConversion to 0
            float teraConversion;
            //Grabs the conversionNum from the one passed into the function then 
            //divides by the Float_TERA_CONVERSION Constant
            teraConversion = ConversionNum / FLOAT_TERA_CONVERSION;

            return teraConversion;
        }//End ConversionToTer
        #endregion
    }
}
