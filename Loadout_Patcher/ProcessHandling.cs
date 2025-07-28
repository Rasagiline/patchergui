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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Loadout_Patcher
{
    /// <summary>
    /// ProcessHandling is for process related operations and using its handle
    /// </summary>
    public static class ProcessHandling
    {
        private static double uptimeInSec;
        // public static bool sseUser = false;

        // Once we have a process, we don't dispose it because a closed process provides a lot of information
        private static Process? loadoutProcess;

        private static Process? loadoutParentProcess;

        // The process handle is important for reading and writing memory
        private static IntPtr? loadoutProcessHandle;

        // Loadout can't be started directly and the process that launches it can be different
        private static string? loadoutParentProcessName;

        // Path to SmartSteamEmu
        private static string? sSEPath;

        // Path to the Loadout game files
        private static string? loadoutPath;

        public static double UptimeInSec
        {
            get { return uptimeInSec; }
            set { uptimeInSec = value; }
        }

        public static Process? LoadoutProcess
        {
            get { return loadoutProcess; }
            set { loadoutProcess = value; }
        }
        public static Process? LoadoutParentProcess
        {
            get { return loadoutParentProcess; }
            set { loadoutParentProcess = value; }
        }

        public static IntPtr? LoadoutProcessHandle
        {
            get { return loadoutProcessHandle; }
            set { loadoutProcessHandle = value; }
        }

        public static string? LoadoutParentProcessName
        {
            get { return loadoutParentProcessName; }
            set
            {
                if (LoadoutParentProcess != null && LoadoutParentProcess.MainModule != null)
                {
                    loadoutParentProcessName = LoadoutParentProcess.MainModule.ModuleName;
                }
            }
        }

        public static string? SSEPath
        {
            get { return sSEPath; }
            set
            {
                    sSEPath = value;
            }
        }

        public static string? LoadoutPath
        {
            get { return loadoutPath; }
            set
            {
                loadoutPath = value;
            }
        }

        public static void LoadSaveFileIntoProcessHandlingProperties(Filesave.SaveFile saveFile)
        {
            LoadoutParentProcessName = saveFile.ParentProcess;
            SSEPath = saveFile.SSEPath;
            LoadoutPath = saveFile.LoadoutPath;
        }

        public static void SynchronizeSaveFile(ref Filesave.SaveFile saveFile)
        {
            saveFile.ParentProcess = LoadoutParentProcessName!;
            saveFile.SSEPath = SSEPath!;
            saveFile.LoadoutPath = LoadoutPath!;
        }

        public static void SeeWhatIsPossible()
        {

        }

        /// <summary>
        /// Console settings that will be outcommented
        /// </summary>
        public static void ConsoleSettings()
        {
            // TODO: Remove the console settings
#pragma warning disable CA1416 // Validate platform compatibility
            Console.WindowWidth = 81;
            Console.WindowHeight = 60;
#pragma warning restore CA1416 // Validate platform compatibility
            Console.Title = "Loadout Reloaded Patcher";
        }

#region Focusing Loadout process
        private const int SW_SHOWNORMAL = 1;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        /// <summary>
        /// -
        /// </summary>
        public static void SetForeground()
        {
            ShowWindow(ProcessHandling.LoadoutProcess!.MainWindowHandle, SW_SHOWNORMAL);
            SetForegroundWindow(ProcessHandling.LoadoutProcess!.MainWindowHandle);
        }
#endregion

        //loadoutProcess.Responding; loadoutProcess.StartInfo.Environment; loadoutProcess.PriorityBoostEnabled = true; loadoutProcess.Kill();
        //loadoutProcess.ErrorDataReceived; loadoutProcess.ExitTime; loadoutProcess.Exited; loadoutProcess.Start();
        //loadoutProcess.CloseMainWindow(); then loadoutProcess.Close();
        //use loadoutProcess.Refresh to discard cached information of a closed process;
        //loadoutProcess.StartInfo.UseShellExecute = false;
        //loadoutProcess.EnableRaisingEvents = true; loadoutProcess.EnableRaisingEvents = false;

        //IntPtr processHandle2 = loadoutProcess.Handle;
        //ProcessModule? hm = loadoutProcess.MainModule;
        //    if (hm is not null)
        //    {
        //        IntPtr hmm = hm.EntryPointAddress;
        // The FileName of the process module (the .exe) gives the full path of the program
        // It can easily be used to restart the program if there is no other way to start it
        // Loadout cannot be started using its .exe directly
        // the parent is required in some way
        // perhaps there is a way to save all process and parents modules and restart them once exited
        //string? xx = hm.FileName;
        //string? xxx = hm.ModuleName;
    }
}
