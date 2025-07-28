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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Loadout_Patcher
{
    /// <summary>
    /// Snapshot is for getting the handle of the targeted process among the list of active processes
    /// </summary>
    public static class Snapshot
    {
        // CharSet.Auto is supposed to deal with different character sets
        // It may be different on unix based operating systems
        #region DllImports
        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr CreateToolhelp32Snapshot([In] UInt32 dwFlags, [In] UInt32 th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32First([In] IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32Next([In] IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle([In] IntPtr hObject);
        #endregion

        // Enum used only internally
        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }

        // Struct used only internally
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            internal UInt32 dwSize;
            internal UInt32 cntUsage;
            internal UInt32 th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal UInt32 th32ModuleID;
            internal UInt32 cntThreads;
            internal UInt32 th32ParentProcessID;
            internal Int32 pcPriClassBase;
            internal UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }

        /// <summary>
        /// Gets a specific process and its parent process as tuple if found
        /// </summary>
        /// <param name="exeFileToLookFor"></param>
        /// <param name="processId"></param>
        /// <returns>a tuple of a process and its parent process</returns>
        /// <exception cref="ApplicationException"></exception>
        private static (Process?, Process?) FetchStandardAndParentProcess(string exeFileToLookFor, int processId = 0)
        {
            (Process?, Process?) standardAndParentProcess = (null, null);
            IntPtr handleToSnapshot = IntPtr.Zero;
            try
            {
                PROCESSENTRY32 procEntry = new ();
                procEntry.dwSize = (UInt32)Marshal.SizeOf(typeof(PROCESSENTRY32));
                handleToSnapshot = CreateToolhelp32Snapshot((uint)SnapshotFlags.Process, 0);
                if (Process32First(handleToSnapshot, ref procEntry))
                {                    
                    while (Process32Next(handleToSnapshot, ref procEntry))
                    {
                        // Careful of Linux, probably must distinguish
                        // byte[] theProcess = UTF8Encoding.UTF8.GetBytes(procEntry.szExeFile);
                        // remove the comments if .CharSet.Auto does the job on Linux
                        //Console.WriteLine("Found: " + procEntry.szExeFile);
                        if (procEntry.szExeFile == exeFileToLookFor)
                        {
                            standardAndParentProcess = (Process.GetProcessById((int)procEntry.th32ProcessID), Process.GetProcessById((int)procEntry.th32ParentProcessID));
                            break;
                        }
                    }
                }
                else
                {
                    throw new ApplicationException(string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error()));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Can't get processes at all.", ex);
            }
            finally
            {
                // Cleaning up the snapshot object!
                CloseHandle(handleToSnapshot);
            }
            return standardAndParentProcess;
        }

        /// <summary>
        /// Gets a specific process and its parent process as tuple if found
        /// </summary>
        public static (Process?, Process?) GetCurrentStandardAndParentProcess
        {
            // TODO: Check for Loadout Beta and output a corresponding if found
            get
            {
                (Process?, Process?) theProcessesOrNothing = FetchStandardAndParentProcess("Loadout.exe");
                // CheckForLoadoutBeta(theProcessesOrNothing)
                return theProcessesOrNothing;
            }
        }
    }
}