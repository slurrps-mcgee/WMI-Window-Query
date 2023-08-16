# WMI-Window-Query
A C# library to query windows WMI Win32 Provider information and return it encased in dictionaries. Please refer to microsofts documentation for win32 providers https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-provider. Right now it only queries drive information on a system but I will be adding all of the Win32 Provider information over time. There is an example project with how to use the library inside the solution. Please reference that for how to use the library. I mostly made this for a project I was working on but found using dictionaries to hold this information to be super easy to query and useful to use.

## Usage
1. Use GetDrives method to get all the drive letters attached to the computer
2. Create a new Library for a specific drive or List<Win32_Library> if wanting to get all drives information
3. Set the library to the return method GetSelectedDriveInformation(using the drive letter from the list of drives) or GetAllDrivesInformation to retrieve all drives information without selecting a specific drive
4. Use the new filled dictionary of information from the query as needed

Please see the project WMI_TEST to see further examples of usage.

### Queries

#### DriveQuery - Used to Query the Drives in the system it is being used on.


### Custom Collections
#### Win32_Library - Holds a Dictionary of Win32_Book(s)

#### Win32_Book - Holds a Dictionary of Win32 Class Properties


### DriveQuery Methods
Public Methods
- List<string> GetDrives
- List<string> GetPartitions(string driveLetter)
- List<Win32_Library> GetAllDrivesInformation()
- Win32_Library GetSelectedDriveInformation(string driveLetter)
- float TotalSpace(Book dict)
- float UsedSpace(Book dict)
Helper Methods
- bool ISOSDrive(string driveLetter)
- string DriveType(string driveNumber)
- float ConversionToGig(float conversionNum)
- float ConversionToTer(float conversionNum)

Private Methods
- Win32_Library GetLogicalInformation(string driveLetter)
- Win32_Library GetDiskInformation(string driveLetter, out string diskNum)
- Win32_Library GetPartitionInformation(string driveNum, string partitionNum)
