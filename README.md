# WMI-Window-Query
A C# library to query windows WMI information and return it encased in dictionaries. Right now it only queries drive information on a system but I will be adding all of the WMI information over time. There is an example project with how to use the library inside the solution. Please reference that for how to use the library. I mostly made this for a project I was working on but found using dictionaries to hold this information to be super easy to query and useful to use.

### Uses Two Collections to gather WMI information from the system (Working class names)

- Library - Holds Books information
- Books - Holds Details about the book or WMI Query in this instance

### Queries

- DriveQuery - Used to Query the Drives in the system it is being used on.

## Usage
1 - Use GetDrives method to get all the drive letters attached to the computer
2 - Create a new Library for a specific drive or List<Library> if wanting to get all drives information
3 - Set the library to the return method GetSelectedDriveInformation(using the drive letter from the list of drives)
4 - Use the new filled dictionary of information from the query as needed

### DriveQuery Methods

Public Methods
- List<string> GetDrives
- List<string> GetPartitions(string driveLetter)
- Library GetSelectedDriveInformation(string driveLetter)
- Library GetSelectedDrivePartitionDetails(string driveLetter, string partitionNum)
- float TotalSpace(Book dict)
- float UsedSpace(Book dict)
Helper Methods
- bool ISOSDrive(string driveLetter)
- string DriveType(string driveNumber)
- float ConversionToGig(float conversionNum)
- float ConversionToTer(float conversionNum)

Private Methods
- Library GetLogicalInformation(string driveLetter)
- Library GetDiskInformation(string driveLetter, out string diskNum)
- Library GetPartitionInformation(string driveNum, string partitionNum)
