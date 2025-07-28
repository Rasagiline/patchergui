/*******************************************************************************
 * Loadout_Patcher
 * 
 * Copyright (c) 2025 Rasagiline
 * GitHub: https://github.com/Rasagiline
 *
 * This program and the accompanying materials are made available under the
 * terms of the Eclipse Public License v. 2.0 which is available at
 * https://www.eclipse.org/legal/epl-2.0/
 *
 * SPDX-License-Identifier: EPL-2.0
 *******************************************************************************/
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Loadout_Patcher.Filesave;
using Avalonia;
using Microsoft.CodeAnalysis;
using System.Reflection;

namespace Loadout_Patcher
{
    /// <summary>
    /// ProcessMemory is for reading and writing memory including system error management
    /// </summary>
    public class ProcessMemory
    {
        // uberentEndpoint, uesEndpoint and matchmakingEndpoint with their addresses
        public static readonly KeyValuePair<string, int>[] BasicEndpoints = new KeyValuePair<string, int>[3]
        {
            new KeyValuePair<string, int>( "uberent.com", 0x1015434 ),
            new KeyValuePair<string, int>( "ues.loadout.com", 0x0f438b8 ),
            new KeyValuePair<string, int>( "mm2.loadout.com", 0x1015540 )
        };

        // const until there are more patcher endpoints than api.loadout.rip
        public const string DefaultPatcherEndpoint = "api.loadout.rip";

        /* Shooting_Gallery_Solo is the exact output of the map reading the memory at the start */
        /* We want it to be shooting_gallery_solo */
        public const string DefaultMapReadMemory = "Shooting_Gallery_Solo";

        public const int MapAddress = 0x0cc94d0;


        private static List<string> webApiEndpoints = new List<string>();

        public static List<string> GetWebApiEndpoints()
        {
            return webApiEndpoints;
        }

        public static void SetWebApiEndpoints(List<string> endpoints)
        {
            webApiEndpoints = endpoints;
        }

        private static string? netInstallation;

        public static string GetNetInstallation()
        {
            return netInstallation!;
        }

        public static void SetNetInstallation(string value)
        {
            netInstallation = value;
        }

        private static string? osDescription;

        public static string GetOsDescription()
        {
            return osDescription!;
        }

        public static void SetOsDescription(string value)
        {
            osDescription = value;
        }


        private static string? runtimeId;

        public static string GetRuntimeId()
        {
            return runtimeId!;
        }

        public static void SetRuntimeId(string value)
        {
            runtimeId = value;
        }


        private static Architecture processorArchitecture;

        public static Architecture GetProcessorArchitecture()
        {
            return processorArchitecture;
        }

        public static void SetProcessorArchitecture(Architecture value)
        {
            processorArchitecture = value;
        }

        private static string? osPlatform;

        public static string GetOsPlatform()
        {
            return osPlatform!;
        }

        public static void SetOsPlatform(string value)
        {
            osPlatform = value;
        }


        public ProcessMemory()
        {
            string os = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                os = "Windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                os = "macOS";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                os = "FreeBSD";
            }

            netInstallation = RuntimeInformation.FrameworkDescription;
            osDescription = RuntimeInformation.OSDescription;
            runtimeId = RuntimeInformation.RuntimeIdentifier;
            processorArchitecture = RuntimeInformation.ProcessArchitecture;
            osPlatform = os;
        }

        // Runtime Information struct in ProcessMemory.cs: .NET installation, OS description, runtime ID, process architecture, OS platform
        // This class needs to store a struct for RuntimeInformation because it's using System.Runtime.InteropServices
        public struct RuntimeInfo
        {
            // A string that indicates the name of the .NET installation on which an app is running. (.NET 6.0.4)
            public string NetInstallation;
            // A string that describes the operating system on which the app is running. (Microsoft Windows 10.0.19044)
            public string OsDescription;
            // Short identifier of the OS and architecture (win10-x64)
            public string RuntimeId;
            // The platform architecture on which the current app is running. (X64)
            public Architecture ProcessorArchitecture;
            // OS platform
            public string OsPlatform;
        }

        public static void LoadSaveFileIntoProcessMemoryProperties(SaveFile saveFile)
        {
            SetWebApiEndpoints(saveFile.WebApiEndpoints);

            SetNetInstallation(saveFile.RuntimeInfo.NetInstallation);
            SetOsDescription(saveFile.RuntimeInfo.OsDescription);
            SetRuntimeId(saveFile.RuntimeInfo.RuntimeId);
            SetProcessorArchitecture(saveFile.RuntimeInfo.ProcessorArchitecture);
            SetOsPlatform(saveFile.RuntimeInfo.OsPlatform);
        }

        public static void SynchronizeSaveFile(ref SaveFile saveFile)
        {
            saveFile.WebApiEndpoints = GetWebApiEndpoints();

            saveFile.RuntimeInfo = new RuntimeInfo
            {
                NetInstallation = GetNetInstallation(),
                OsDescription = GetOsDescription(),
                RuntimeId = GetRuntimeId(),
                ProcessorArchitecture = GetProcessorArchitecture(),
                OsPlatform = GetOsPlatform()
            };
        }











        // TODO:
        // Move string readMemoryUberentString; and all similar strings and functionality into this class

        #region DllImports
        // Make sure to close this handle once done. The process won't be terminated.
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int numberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int numberOfBytesWritten);

        // Receive and be able to interpret errors that can occur when tying to open a process or read/write its memory
        // Returns an error code without message
        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        // Only GetErrorMessage(int? errorCode, char openReadWrite) should use this in order to get an error message in every case
        [DllImport("kernel32.dll")]
        private static extern int SetLastError(int lastError);

        // Gets the error message back from the system
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId,
            uint dwLanguageId, out StringBuilder msgOut, int nSize, IntPtr Arguments);
        #endregion

        // Using Marshal.FreeHGlobal(IntPtr) instead to free unmanaged memory
        // private static extern bool CloseHandle([In] IntPtr hObject);

        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;

        // They are used for the output from FormatMessage
        const int ALLOCATE_BUFFER = 0x00000100;
        const int IGNORE_INSERTS = 0x00000200;
        const int FROM_SYSTEM = 0x00001000;

        // Changing the data type to int? is not allowed
        private static int numberOfBytesRead = 0;
        private static int numberOfBytesWritten = 0;

        // The variables grant more control of recovering error codes
        // GetLastError() is not perfectly reliable
        // 1. Errors that have absolutely nothing to do with ProcessMemory don't influence these variables
        // 2. Some functions automatically reset the last error to 0 on success
        // At least 1 method is responsible to clean them up as needed
        private static int? errorCodeOpening = null;
        private static int? errorCodeReading = null;
        private static int? errorCodeWriting = null;

        /// <summary>
        /// Checks if the buffer of the address we want to patch is not empty and returns it as a string
        /// This method can be used to read memory and also check if writing was successful
        /// </summary>
        /// <param name="loadoutProcess"></param>
        /// <param name="offset"></param>
        /// <param name="replacementString"></param>
        /// <param name="checkForPerfectMatch"></param>
        /// <returns>readable string that was read from the memory</returns>
        public static string CheckStringAtOffset(Process loadoutProcess, int offset, string replacementString, bool checkForPerfectMatch = false)
        {
            // Reset the out parameter
            numberOfBytesRead = 0;

            // Checks if the process is running
            if (Process.GetProcessesByName(loadoutProcess.ProcessName).Length == 0 || loadoutProcess.HasExited)
            {
                Console.WriteLine("> Error: Couldn't find a running process called: {0}\n", loadoutProcess.ProcessName);
                Console.WriteLine("> The process must have been closed.\n");
                return "";
            }

            // else:
            // Process.GetProcessesByName(nameOfTheProcess)[0]; gets the first process using a given process name

            // If it's necessary to work with the second or third process with the same name, make an extra method
            // It can become questionable if this is useful, but if the process restarts, it must be refreshed
            // TODO: A better approach is to refresh it elsewhere and a lot more often using the Snapshot class
            // Resource expensive approach:
            // Call outside: ProcessMemory.CheckStringAtOffset(Snapshot.GetCurrentStandardAndParentProcess.Item1, uesAddress, uberentEndpoint)
            // Uncomment: ProcessHandling.LoadoutProcess = loadoutProcess;
            // Cheap approach:
            // EventHandler -> Process.HasExited -> spam Snapshot.RefreshStrandardAndParentProcess -> process found -> reassign
            // unlikely, but the parent process can also easily change! reassign

            try
            {
                if (OperatingSystem.IsFreeBSD())
                {
                    (nint, int) openProcessAndGetLastError = InterprocessCommunication.OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, loadoutProcess.Id);
                    ProcessHandling.LoadoutProcessHandle = openProcessAndGetLastError.Item1;
                    errorCodeOpening = openProcessAndGetLastError.Item2;                    
                }
                else
                {
                    // We are checking the process for access rights
                    // The assignment to processHandle is absolutely necessary for the reading and writing methods
                    ProcessHandling.LoadoutProcessHandle = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, loadoutProcess.Id);
                    errorCodeOpening = GetLastError();
                }

            }
            catch (Exception)
            {
                Console.WriteLine("> Error: Couldn't check if the process allows reading and writing memory.");
                Console.WriteLine("> Continuing with risk ...\n");
            }

            /* We obtain memory to check if it's empty. If it isn't, we convert it to a readable string */
        //byte[] memoryRead = ReadMemory(offset, replacementString.Length);
        //string memoryReadString = new (System.Text.Encoding.UTF8.GetString(memoryRead).Replace("\0", ""));

        byte[] memoryRead = ReadMemory(offset, replacementString.Length);
            /* For comparison and accurate displaying, we remove the binary null characters from the strings */
            string memoryReadString = new(System.Text.Encoding.UTF8.GetString(memoryRead).Replace("\0", ""));
            replacementString = replacementString.Replace("\0", "");

            /* We want an exact match if we want to read what we have just written. If we didn't write, we must skip this part */
            /* If we read a custom map string, it can be anything. Only reading and going in there can lead to infinite restarts of the patcher */
            if (checkForPerfectMatch)
            {
                // Consider adding null, CompareOptions.Ordinal to the comparison when working with UTF-16
                if (String.Compare(memoryReadString, replacementString) != 0)
                {
                    Console.WriteLine("> Error: Read string value {0} instead of {1}\n", memoryReadString, replacementString);
                    Console.WriteLine("> Restarting the patcher ...\n");
                    /**
                     * If the final check fails, we set our error code.
                     * This is needed for the GetErrorMessage() method and the patcherRestarted boolean.
                     * Error code 666 is not reserved, so it can be used.
                     * Consider removing the error code if a patcher restart becomes unnecessary.
                     * Alternatively, use SetErrorCode(uint errorCode) directly.
                     */
                    errorCodeReading = 666;
                }
            }

            //processHandle = null;
            //loadoutProcess.Dispose();


            // Read read memory buffer converted to a readable string (UTF-8)
            return memoryReadString;
        }

        /// <summary>
        /// Patches an endpoint and checks if the memory was successfully overwritten
        /// </summary>
        /// <param name="loadoutProcess"></param>
        /// <param name="offset"></param>
        /// <param name="stringToReplace"></param>
        /// <param name="replacementString"></param>
        /// <returns>overwritten memory as a string</returns>
        public static string OverwriteStringAtOffset(Process loadoutProcess, int offset, string stringToReplace, string replacementString, bool isMap = false)
        {
            // Reset the out parameter
            numberOfBytesWritten = 0;

            Console.WriteLine("> Beginning patching {0} ...", stringToReplace);
            Console.WriteLine("> Patching {0} at {1}", stringToReplace, offset);

            /* If we want to write the map into the memory, we want the replacementString to be exactly 29 characters long */
            /* If it's shorter, we must fill it with binary null characters */
            if (isMap)
            {
                int remainingLength = 29 - replacementString.Length;
                replacementString += new string('\u0000', remainingLength);
                replacementString = replacementString.Substring(0, 29);
            }

            /* We can then write our replacement */
            if (!WriteMemory(offset, replacementString.ToCharArray()))
            {
                Console.WriteLine("\n------------------------------> Patching failed! <------------------------------\n");
                return "";
            }
            Console.WriteLine("> Written value {0} at {1}\n", replacementString, offset);

            /* We check if the value was correctly written. An error code as output is an integer provided by the system. In case of an error, the patcher will automatically restart */
            return CheckStringAtOffset(loadoutProcess, offset, replacementString, true);
        }

        /// <summary>
        /// Patches an endpoint silently to not worry if the early or late attempt before was successful
        /// </summary>
        /// <param name="loadoutProcess"></param>
        /// <param name="offset"></param>
        /// <param name="stringToReplace"></param>
        /// <param name="replacementString"></param>
        /// <returns>overwritten memory as a string</returns>
        public static void OverwriteStringAtOffsetSilent(int offset, string replacementString, bool isMap = false)
        {
            // Reset the out parameter
            numberOfBytesWritten = 0;

            /* If we want to write the map into the memory, we want the replacementString to be exactly 29 characters long */
            /* If it's shorter, we must fill it with binary null characters */
            if (isMap)
            {
                int remainingLength = 29 - replacementString.Length;
                replacementString += new string('\u0000', remainingLength);
                replacementString = replacementString.Substring(0, 29);
            }

            /* We can then write our replacement */
            WriteMemory(offset, replacementString.ToCharArray());
        }

        /// <summary>
        /// Gets the module's address of a process
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns>memory address where the module was loaded as integer</returns>
        public static int GetModuleAddress(string moduleName)
        {
            try
            {
                if (ProcessHandling.LoadoutProcess is not null && !ProcessHandling.LoadoutProcess.HasExited)
                {
                    foreach (ProcessModule procMod in ProcessHandling.LoadoutProcess.Modules)
                    {
                        // Alternatively outcomment the whole if statement
                        // to not reassign the moduleName variable
                        // and use this instead: int MyDll = Memory.GetModuleAddress("MyDll.dll");
                        if (!moduleName.Contains(".dll"))
                        {
                            moduleName = moduleName.Insert(moduleName.Length, ".dll");
                        }

                        if (moduleName == procMod.ModuleName)
                        {
                            return (int)procMod.BaseAddress;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("> Type of exception: " + ex.InnerException + "\n");
                Console.WriteLine("> Error message: " + ex.Message + "\n");
            }
            
            return -1;
        }

        /// <summary>
        /// Reads memory with the size of a given type.
        /// Use the byte[] overload for reading strings with a specific size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset"></param>
        /// <returns>read memory buffer to be converted into a proper data type</returns>
        public static T? ReadMemory<T>(int offset) where T : struct
        {
            // Gets the byteSize of dataType
            int byteSize = Marshal.SizeOf(typeof(T));
            // Creates a buffer with the size of byteSize
            byte[] buffer = new byte[byteSize];
            // Reads a number of bytes from the memory and assigns it to a buffer
            if (!ReadProcessMemory((int)ProcessHandling.LoadoutProcessHandle!, offset, buffer, buffer.Length, out numberOfBytesRead))
            {
                errorCodeReading = GetLastError();
            }

            // Transforms the byteArray to the desired dataType
            return ByteArrayToStructure<T>(buffer);
        }

        /// <summary>
        /// Sets a buffer with the exact number of bytes (possibly representing characters) 
        /// the user wants to read and calls the method that reads memory.
        /// The buffer, a byte array, can probably become a readable string.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns>read memory buffer, a byte array</returns>
        public static byte[] ReadMemory(int offset, int size)
        {
            // To read unicode, multiply the byte size by 2 before calling this method
            // 'H e l l o   W o r l d ! '
            byte[] buffer = new byte[size];

            if (!ReadProcessMemory((int)ProcessHandling.LoadoutProcessHandle!, offset, buffer, size, out numberOfBytesRead))
            {
                errorCodeReading = GetLastError();
            }

            return buffer;
        }

        /// <summary>
        /// Reads memory with the size of a given type.
        /// The output then needs to be converted into a float array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="offset"></param>
        /// <param name="matrixSize"></param>
        /// <returns>read memory buffer to be converted into a float array</returns>
        public static float[] ReadMatrix<T>(int offset, int matrixSize) where T : struct
        {
            // Initialization of byteSize used to determine the size of the given data type for this method
            int byteSize = Marshal.SizeOf(typeof(T));
            // Creates a buffer with the size of byteSize * matrixSize
            byte[] buffer = new byte[byteSize * matrixSize];            
            if (!ReadProcessMemory((int)ProcessHandling.LoadoutProcessHandle!, offset, buffer, buffer.Length, out numberOfBytesRead))
            {
                errorCodeReading = GetLastError();
            }

            // Transforms the byte array to a float array
            return ByteArrayToFloatArray(buffer);
        }

        /// <summary>
        /// Sets a buffer with the exact size of the object the user wants to overwrite
        /// and calls the method that overwrites memory.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public static bool WriteMemory(int offset, object value)
        {
            // Transforms data to an array of bytes
            // The buffer will contain the data used to overwrite
            byte[] buffer = StructureToByteArray(value);

            if (!WriteProcessMemory((int)ProcessHandling.LoadoutProcessHandle!, offset, buffer, buffer.Length, out numberOfBytesWritten))
            {
                errorCodeWriting = GetLastError();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets a buffer with the exact size of characters the user wants to overwrite
        /// and calls the method that overwrites memory.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public static bool WriteMemory(int offset, char[] value)
        {
            // Transforms a character array, basically a string, to an array of bytes
            // The buffer will contain the data used to overwrite
            byte[] buffer = Encoding.UTF8.GetBytes(value);
                        
            if (!WriteProcessMemory((int)ProcessHandling.LoadoutProcessHandle!, offset, buffer, buffer.Length, out numberOfBytesWritten))
            {
                errorCodeWriting = GetLastError();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts a fresh read byte array into a float array.
        /// ReadMatrix<T>(int, int) makes use of this method.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>array of floats</returns>
        public static float[] ByteArrayToFloatArray(byte[] byteArray)
        {
            if (byteArray.Length % 4 != 0)
            {
                Console.WriteLine("> Error: Possible mismatch reading from memory!\n");
                Console.WriteLine("> The byte array that was read can't be expressed in floats!\n");
            }

            float[] floats = new float[byteArray.Length / 4];

            for (int i = 0; i < floats.Length; i++)
            {
                floats[i] = BitConverter.ToSingle(byteArray, i * 4);
            }

            return floats;
        }

        /// <summary>
        /// Converts a fresh read byte array into a set data type to understand the output.
        /// The structure needs to be a standard data type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns>read data of any set standard type or null</returns>
        private static T? ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T?)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            catch (AccessViolationException)
            {
                Console.WriteLine("> Error: The read memory can only be converted into a standard type!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("> Type of exception: " + ex.InnerException + "\n");
                Console.WriteLine("> Error message: " + ex.Message + "\n");
            }
            finally
            {
                handle.Free();
            }
            return null;
        }


        /// <summary>
        /// Converts the object into an array of bytes to overwrite memory.
        /// The method to actually write memory is not being called.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>array of bytes</returns>
        private static byte[] StructureToByteArray(object obj)
        {
            int objSize = Marshal.SizeOf(obj);

            byte[] byteArray = new byte[objSize];

            IntPtr pointer = Marshal.AllocHGlobal(objSize);

            Marshal.StructureToPtr(obj, pointer, true);
            Marshal.Copy(pointer, byteArray, 0, objSize);
            // Free memory after working with type IntPtr
            Marshal.FreeHGlobal(pointer);

            return byteArray;
        }

        public static int NumberOfBytesRead
        {
            get { return numberOfBytesRead; }
        }

        public static int NumberOfBytesWritten
        {
            get { return numberOfBytesWritten; }
        }

        /// <summary>
        /// Outputs error codes and messages.
        /// Only successful errors that were made using the ProcessMemory class lead to this method.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="openReadWrite"></param>
        private static void GetErrorMessage(int errorCode, char openReadWrite)
        {
            // Questionable if externErrorCode is needed
            int externErrorCode;
            externErrorCode = SetLastError(errorCode);
            StringBuilder errorMessage = new (512);
            externErrorCode = FormatMessage(ALLOCATE_BUFFER | FROM_SYSTEM | IGNORE_INSERTS, IntPtr.Zero, errorCode, 0, out errorMessage, errorMessage.Capacity, IntPtr.Zero);
            Console.WriteLine("> Error code: {0}\n", errorCode);
            if (openReadWrite == 'o')
            {
                Console.WriteLine("> The process could not be opened.\n");
                Console.WriteLine("> System error message about this error: \n");
                Console.WriteLine("> {0}", errorMessage);
            }
            else if (openReadWrite == 'r')
            {
                Console.WriteLine("> The memory could not be read.\n");
                Console.WriteLine("> System error message about this error: \n");
                Console.WriteLine("> {0}", errorMessage);
                if (errorCode == 299) { Console.WriteLine("> Known error: Loadout must have been closed!\n"); }
            }
            else if (openReadWrite == 'w')
            {
                Console.Write("> The memory could not be overwritten.\n");
                Console.WriteLine("> System error message about this error: \n");
                Console.WriteLine("> {0}", errorMessage);
                if (errorCode == 5) { Console.WriteLine("> Known error: Loadout must have been closed!\n"); }
            }
            Console.WriteLine("> Restarting the patcher ...\n");
            if (errorMessage is not null)
            {
                errorMessage.Clear();
            }
        }

        /// <summary>
        /// Checks for error codes.
        /// If an error was found, this method will call an error message method.
        /// It will also return true after the error message output.
        /// </summary>
        public static bool GetLastErrorOfProcessMemory()
        {
            if (errorCodeOpening != null && errorCodeOpening != 0 || errorCodeReading != null && errorCodeReading != 0 || errorCodeWriting != null && errorCodeWriting != 0)
            {
                // The system is only able to output 1 error code which gets overwritten frequently
                // With this approach, 3 separate error codes can be recovered
                if (errorCodeOpening != null && errorCodeOpening != 0 && errorCodeOpening != 666)
                {
                    GetErrorMessage((int)errorCodeOpening, 'o');
                }
                if (errorCodeReading != null && errorCodeReading != 0 && errorCodeReading != 666)
                {
                    GetErrorMessage((int)errorCodeReading, 'r');
                }
                if (errorCodeWriting != null && errorCodeWriting != 0 && errorCodeWriting != 666)
                {
                    GetErrorMessage((int)errorCodeWriting, 'w');
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// It must be used after handling errors, for example after resetting the patcher
        /// </summary>
        public static void CleanErrorsOfProcessMemory()
        {
            // Errors were treated, so the values must be cleaning
            // An separate check for null could find out if there is not enough cleaning in Main()
            errorCodeOpening = null;
            errorCodeReading = null;
            errorCodeWriting = null;
        }
    }
}