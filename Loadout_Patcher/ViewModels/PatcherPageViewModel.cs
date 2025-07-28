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
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using IWshRuntimeLibrary;

namespace Loadout_Patcher.ViewModels;

public partial class PatcherPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private static int _progressValue;

    [ObservableProperty]
    private static bool _patchingNotification;

    [ObservableProperty]
    private static bool _whileLoopNotification;

    [ObservableProperty]
    private static bool _patchedText;

    [ObservableProperty]
    private static bool _patchingEnabled = true;

    [ObservableProperty]
    private static bool _isPatching;

    [ObservableProperty]
    private static int _countUp = 0;

    private static bool autoStarted;

    public PatcherPageViewModel()
    {
        if (GUI.StartLoadout && CountUp == 0)
        {
            autoStarted = false;
            // TODO: Create SSEPath LoadoutPath saves everywhere it patches!

            /* Creating the link might not be supported on Linux and macOS */

            Console.WriteLine("> Please be aware that you shouldn't run Loadout on Steam and SSE at the same time.");
            Console.WriteLine("> Restart Steam if that happens.\n");

            if (GUI.StartLoadoutViaSSE && ProcessHandling.SSEPath == "")
            {
                Console.WriteLine("> SSE users can autostart Loadout the 2nd time they run the patcher.\n");
            }

            // We check if Loadout is already open
            // We use the endpoint in string[0]
            MainProperties.NewEndpoint = ProcessMemory.GetWebApiEndpoints()[0];

            ProcessHandling.LoadoutProcess = Snapshot.GetCurrentStandardAndParentProcess.Item1;
            ProcessHandling.LoadoutParentProcess = Snapshot.GetCurrentStandardAndParentProcess.Item2;
            if (ProcessHandling.LoadoutProcess == null || ProcessHandling.LoadoutParentProcess == null)
            {
                try
                {
                    /* --------------------------- */
                    /* This is the section for SSE */
                    /* --------------------------- */
                    // sselauncher
                    // "C:\Users\dev\Desktop\Data Storage\Free Time\Project Loadout\ssm HERE\SSELauncher.exe" -appid 208090
                    // -
                    // -
                    if (GUI.StartLoadoutViaSSE && ProcessHandling.SSEPath != "")
                    {
                        /* Part 1: Creating Shortcut */
                        /* We also have to check if there is a shortcut, it has to belong to SSE */
                        /* One step to be safe is to name the link Loadout Reloaded */
                        string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                        object shDesktop = "Desktop";
                        WshShell shell = new WshShell();
                        string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + Filesave.DirectorySeparator + @"Loadout Reloaded.lnk";
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
                        shortcut.Description = "Loadout Reloaded run by SmartSteamEmu";
                        shortcut.Hotkey = "Ctrl+Shift+L";
                        shortcut.IconLocation = ProcessHandling.LoadoutPath!;
                        shortcut.TargetPath = ProcessHandling.SSEPath!.Remove(ProcessHandling.SSEPath!.Length - 34) + "SSELauncher.exe";
                        shortcut.WorkingDirectory = ProcessHandling.SSEPath.Remove(ProcessHandling.SSEPath!.Length - 34);
                        shortcut.Arguments = "-appid 208090";
                        shortcut.Save();

                        /* Part 2: Starting Loadout */
                        ProcessStartInfo ps = new ProcessStartInfo((string)shell.SpecialFolders.Item(ref shDesktop) + Filesave.DirectorySeparator + @"Loadout Reloaded.lnk")
                        {
                            UseShellExecute = true,
                            Verb = "open",
                        };
                        /* We start SSE and try to run Loadout */
                        Process? sseProcess = Process.Start(ps);
                        /* We need to try-catch and throw it because this exception can't be completely excluded. */
                        if (sseProcess == null)
                        {
                            throw new InvalidOperationException();
                        }
                        else
                        {
                            Console.WriteLine("> Opening SSE on an attempt to run Loadout.\n");
                            autoStarted = true;
                            sseProcess.Close();
                            sseProcess.Dispose();
                        }

                        /* Part 3: Eventually deleting shortcut */
                        /* Those who don't want an SSE Shortcut still get it for 1 ms. */
                        if (!GUI.CreateSSEShortcut)
                        {
                            if (System.IO.File.Exists((string)shell.SpecialFolders.Item(ref shDesktop) + Filesave.DirectorySeparator + @"Loadout Reloaded.lnk"))
                            {
                                /* This appears to not work reliably, so we need to refresh the desktop */
                                System.IO.File.Delete((string)shell.SpecialFolders.Item(ref shDesktop) + Filesave.DirectorySeparator + @"Loadout Reloaded.lnk");
                                /* This flashes the desktop to update it */
                                RefreshDesktop();
                            }
                        }
                    }
                }
                catch (InvalidOperationException invOpEx)
                {
                    Console.WriteLine($"> Invalid operation error: '{invOpEx}'\n");
                    Console.WriteLine("> Steam wasn't found!");
                    Console.WriteLine("> Continuing ...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"> Error: '{ex}'");
                    Console.WriteLine("> Autostarting Loadout failed.");
                    Console.WriteLine("> Using an operation system that isn't Windows could be the issue.\n");
                }
                
                /* ----------------------------- */
                /* This is the section for Steam */
                /* ----------------------------- */
                if (!GUI.StartLoadoutViaSSE)
                {
                    /* This may cause an Exception if it doesn't work! We use a try-catch in that case */
                    /* No critical error if Steam wasn't found. The user would only see weird behavior */
                    /* Debug mode gives an exception */
                    try
                    {
                        ProcessStartInfo ps = new ProcessStartInfo("steam://rungameid/208090")
                        {
                            UseShellExecute = true,
                            Verb = "open",
                        };
                        /* We start Steam and try to run Loadout */
                        Process? steamProcess = Process.Start(ps);
                        /* We need to try-catch and throw it because this exception can't be completely excluded. */
                        if (steamProcess == null)
                        {
                            throw new InvalidOperationException();
                        }
                        else
                        {
                            Console.WriteLine("> Opening Steam on an attempt to run Loadout.\n");
                            autoStarted = true;
                            steamProcess.Close();
                            steamProcess.Dispose();
                        }
                    }
                    catch (InvalidOperationException invOpEx)
                    {
                        Console.WriteLine($"> Invalid operation error: '{invOpEx}'\n");
                        Console.WriteLine("> Steam wasn't found!");
                        Console.WriteLine("> Continuing ...");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"> An unknown error occured: '{ex}'\n");
                        Console.WriteLine("> Autostarting Loadout failed.");
                        Console.WriteLine("> Using an operation system that isn't Windows could be the issue.\n");
                    }
                    /* We don't start Loadout if Loadout was found */
                }
            }
        }

        CountUp++;
        if (CountUp == 6 || CountUp == 2 && GUI.SkipStartPage)
        {
            /* We don't let a count 6 happen if we skip the start page (splash screen) */
            if (GUI.SkipStartPage)
            {
                CountUp = 7;
            }

            /* We play Loadout's menu music and taunts continuously if enabled */
            Sound.PlayMenuMusicRandomly();
            /* We ultimately want to skip this part if it takes too long, so we need this variable */
            int timePassed = 0;

            /* If we have a starting map and the game already open, the patcher will patch immediately */
            /* This is a quick patch operation, allowing only 1 attempt */
            while (MainProperties.StartingMap.FullMapName != "" && MainProperties.StartingMap.FullMapName != null || GUI.InstantPatching)
            {
                if (MainProperties.Patched)
                {
                    break;
                }

                // We use the endpoint in string[0]
                MainProperties.NewEndpoint = ProcessMemory.GetWebApiEndpoints()[0];

                ProcessHandling.LoadoutProcess = Snapshot.GetCurrentStandardAndParentProcess.Item1;
                ProcessHandling.LoadoutParentProcess = Snapshot.GetCurrentStandardAndParentProcess.Item2;
                if (ProcessHandling.LoadoutProcess != null && ProcessHandling.LoadoutParentProcess != null)
                {
                    if (ProcessHandling.LoadoutProcess.ProcessName == "Loadout")
                    {
                        /* Once we find Loadout, we immediately save the path */
                        if (ProcessHandling.LoadoutProcess.MainModule != null)
                        {
                            if (ProcessHandling.LoadoutPath != ProcessHandling.LoadoutProcess.MainModule.FileName)
                            {
                                ProcessHandling.LoadoutPath = ProcessHandling.LoadoutProcess.MainModule.FileName;

                                Console.WriteLine("> Since Loadout was found, the path to the game files was saved.\n");

                                // Save file is being updated
                                Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
                                Filesave.SaveDataToFile(saveFile, true);

                                Console.WriteLine("> Save file changed and saved.\n");
                            }
                        }

                        //uptimeInSec = DateTime.Now.Subtract(loadoutStandardProcess.StartTime).TotalSeconds;
                        ProcessHandling.UptimeInSec = ProcessHandling.LoadoutProcess.TotalProcessorTime.TotalSeconds;
                        /* This is vague since the total seconds seem somewhat unreliable */
                        /* Once Loadout with an uptime of at least 8 seconds was found, the loop acts as one-shot */
                        /* It's purely intended to make the user wait if Loadout is below 5.2, 7.5 or 5 Seconds uptime */
                        /* This fits amazingly, because the user still sees the progress bar */
                        /* TODO: Adjust the times for the case of Loadout the first time after the PC booted up */
                        while (ProcessHandling.UptimeInSec >= 5.2 && autoStarted || ProcessHandling.UptimeInSec >= 7.5 && GUI.InstantPatching && !autoStarted ||
                            ProcessHandling.UptimeInSec >= 5 && !autoStarted)
                        {
                            Console.WriteLine("-------------------------------> Loadout found! <-------------------------------\n");

                            if (ProcessHandling.LoadoutParentProcess.ProcessName == "SmartSteamLoader")
                            {
                                Console.WriteLine("> The SmartSteamEmu Launcher is being used on Loadout.\n");

                                /* We save the path to SmartSteamEmu as SSEPath */
                                if (ProcessHandling.LoadoutParentProcess.MainModule != null)
                                {
                                    if (ProcessHandling.SSEPath != ProcessHandling.LoadoutParentProcess.MainModule.FileName)
                                    {
                                        ProcessHandling.SSEPath = ProcessHandling.LoadoutParentProcess.MainModule.FileName;

                                        Console.WriteLine("> Since you used SmartSteamEmu, the file path is being saved.\n");

                                        // Save file is being updated
                                        Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
                                        Filesave.SaveDataToFile(saveFile, true);

                                        Console.WriteLine("> Save file changed and saved.\n");
                                    }
                                }
                            }
                            /* We fetch aliases from existing hashtables. */
                            MainProperties.MatchingAliasMap = Map.FetchMatchingAliasMap(MainProperties.StartingMap.BaseMap);
                            MainProperties.MatchingAliasGameMode = Map.FetchMatchingAliasGameMode(MainProperties.StartingMap.GameMode);

                            MainProperties.ReadMemoryUberentString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess,
                                ProcessMemory.BasicEndpoints[0].Value, ProcessMemory.BasicEndpoints[0].Key);
                            /* This is a chain reaction to keep checks at a minimum and receive no more than 1 (identical) error message in case of an error */
                            if (MainProperties.ReadMemoryUberentString.Length > 0)
                            {
                                MainProperties.ReadMemoryUesString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess,
                                    ProcessMemory.BasicEndpoints[1].Value, ProcessMemory.BasicEndpoints[1].Key);

                                if (MainProperties.ReadMemoryUesString.Length > 0)
                                {
                                    MainProperties.ReadMemoryMatchmakingString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess,
                                        ProcessMemory.BasicEndpoints[2].Value, ProcessMemory.BasicEndpoints[2].Key);

                                    if (MainProperties.ReadMemoryMatchmakingString.Length > 0)
                                    {
                                        MainProperties.ReadMemoryMapString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess,
                                            ProcessMemory.MapAddress, MainProperties.DefaultMap);
                                    }
                                }
                            }

                            /* patching the basic endpoints [uberentEndpoint], [uesEndpoint] and [matchmakingEndpoint] to be able to play the game */
                            foreach (KeyValuePair<string, int> endpointToOverwriteAndAddress in ProcessMemory.BasicEndpoints)
                            {
                                MainProperties.ReadMemoryEndpointString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, endpointToOverwriteAndAddress.Value,
                                    endpointToOverwriteAndAddress.Key, MainProperties.NewEndpoint);
                                if (ProcessMemory.GetLastErrorOfProcessMemory())
                                {
                                    MainProperties.PatcherReset = true;
                                    MainProperties.Patched = false;
                                    // MainProperties.Reset();
                                }
                            }

                            /* patching [readMemoryMapString] */
                            // TODO: It can be MainProperties.StartingMap.FullMapNameAlt as well. NoHealthPickups (checkbox) boolean belongs in the save file
                            MainProperties.ReadMemoryMapString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, ProcessMemory.MapAddress,
                                MainProperties.ReadMemoryMapString, MainProperties.StartingMap.FullMapName!, true);
                            if (ProcessMemory.GetLastErrorOfProcessMemory())
                            {
                                MainProperties.Patched = false;
                                MainProperties.PatcherReset = true;
                                /* We reset the simple patcher */
                                MainProperties.Reset();
                            }
                            Console.WriteLine("-----------------------------> Map patching done! <-----------------------------\n");
                            MainProperties.PatcherReset = false;
                            /* We play a random success sound, a melee hit sound, from Axl, Helga or T-Bone if enabled */
                            Sound.PlaySuccessSoundsHitRandomly();

                            Console.WriteLine("-> [= Complete] Map selected: {0}", MainProperties.StartingMap.FullMapName);
                            if (MainProperties.MatchingAliasMap != "")
                            {
                                Console.WriteLine("-> [= Complete] Map known as: {0}", MainProperties.MatchingAliasMap);
                            }
                            if (MainProperties.MatchingAliasGameMode != "")
                            {
                                Console.WriteLine("-> [= Complete] Game mode is: {0}", MainProperties.MatchingAliasGameMode);
                            }
                            Console.WriteLine();

                            ProgressValue = 100;
                            PatchedText = true;
                            MainProperties.Patched = true;
                            break;
                        }
                        /* We continue if Loadout was found and wait the 6 seconds until we can jump into the patching */
                        /* During that time frame, we also detect if Loadout is still running, making the loop meaningful */
                        continue;
                    }
                }
                /* SSE and Steam take their time to start the game, so we have to continue this loop for at least ~4 seconds */
                else if (GUI.SkipStartPage && autoStarted && timePassed <= 3999)
                {
                    if (GUI.SkipStartPage)
                    {
                        if (timePassed % 1000 == 0 && timePassed < 3000)
                        {
                            if (!GUI.StartLoadoutViaSSE && timePassed > 1000)
                            {
                                Console.WriteLine("***********************************************");
                                Console.WriteLine("> You want to use Steam to get Loadout started.");
                                Console.WriteLine("> Please make sure to be logged in!");
                                Console.WriteLine("***********************************************\n");
                            }
                            Console.WriteLine($"> The GUI will open in at most {4 - (timePassed / 1000)} seconds.");
                            Console.WriteLine("> Waiting for Loadout to start ...\n");
                        }
                        else if (timePassed % 1000 == 0 && timePassed > 2999)
                        {
                            Console.WriteLine($"> The GUI will open in at most 1 second.\n");
                        }
                    }
                    /* It takes the suspension time + 51 to 57 ms to reach this point again */
                    System.Threading.Thread.Sleep(45);
                    timePassed += 100;
                    continue;
                }
                /* We inform every other user that Loadout wasn't found */
                if (!GUI.SkipStartPage && autoStarted && timePassed == 0)
                {
                    if (!GUI.StartLoadoutViaSSE)
                    {
                        Console.WriteLine("***********************************************");
                        Console.WriteLine("> You want to use Steam to get Loadout started.");
                        Console.WriteLine("> Please make sure to be logged in!");
                        Console.WriteLine("***********************************************\n");
                    }
                    Console.WriteLine($"> The GUI will open now.\n");
                }
                /* If Loadout wasn't found, especially after an attempt to start it, we quit the loop */
                break;
            }
        }

        /* If it's trying to patch and the user changed tabs while it's on it, the user will get a special notification to start Loadout */
        if (IsPatching && !MainProperties.PatcherReset)
        {
            /* Notifications can be made visible here in the constructor instantly. No guarantee anywhere else */
            /* The user will be told "It's trying to patch" meaning a while loop is consistently running */
            WhileLoopNotification = true;
        }

        /* If no patching happened, we reset values in case the MainProperties.Reset() function was triggered */
        /* This is also part of the patcher restarting */
        if (!MainProperties.Patched) 
        {
            ProgressValue = 0;
            PatchingNotification = false;
            PatchedText = false; // This is not like MainProperties.Patched
        }
    }

    /// <summary>
    /// Function that increases the progress bar step by step when patching progress was made
    /// This function should only lead to 99% of the progress bar since there is extra loading
    /// </summary>
    /// <param name="progressValue"></param>
    /// <returns></returns>
    private int MakingProgress(ref int progressValue)
    {
        // This operation is bug sensitive, so we force the maximum of 99 here.
        if (progressValue < 91) { progressValue += 13; }
        else { progressValue += 8; }
        if (progressValue > 99) { progressValue = 99; }
        ProgressValue = progressValue;

        return progressValue;
    }

    [RelayCommand]
    private async Task Patching()
    {
        IsPatching = true;

        if (MainProperties.Patched)
        {
            /* If the game was patched and we still want to patch, it will be checked if Loadout is running */
            ProcessHandling.LoadoutProcess = Snapshot.GetCurrentStandardAndParentProcess.Item1;
            if (ProcessHandling.LoadoutProcess == null)
            {
                /* Patching will be set to false if Loadout wasn't found */
                MainProperties.Patched = false;
            }
        }

        int progressValue = 0;
        if (!MainProperties.Patched)
        {
            ProgressValue = 0;
            PatchingNotification = true;
            PatchedText = false;
        }

        // ADD: Function Map.GetStartingMap().FullMapName or Map.GetStartingMap().FullMapNameAlt decider "MapOrMapAltDecider"
        // it returns either FullMapName or FullMapNameAlt
        //
        // Check ProcessHandling.UptimeInSec < 5 by testing


        // Patching happens immediately without a question and it always happens when Loadout wasn't found
        if (!MainProperties.Patched)
        {
            // We use the endpoint in string[0]
            MainProperties.NewEndpoint = ProcessMemory.GetWebApiEndpoints()[0];

            // TODO: Add a check within SetEndpoint() to OptionsPageView
            // TypedUserInput.SetEndpoint(ProcessMemory.defaultPatcherEndpoint, out endpoint);
            // TODO: We want ProcessHandling.UptimeInSec to find out since when Loadout runs!

            /* Wait for the Loadout.exe process and continue once we get access */
            Console.WriteLine("> Waiting for Loadout.exe ... (Loadout Beta is not supported)\n");

            /* We disable patching since this while loop will continue running if the tab was switched. */
            /* If we let the user patch again before patching was completed, the program would freeze. */
            PatchingEnabled = false;
            while (true)
            {
                ProcessHandling.LoadoutProcess = Snapshot.GetCurrentStandardAndParentProcess.Item1;
                ProcessHandling.LoadoutParentProcess = Snapshot.GetCurrentStandardAndParentProcess.Item2;
                if (ProcessHandling.LoadoutProcess != null && ProcessHandling.LoadoutParentProcess != null)
                {
                    if (ProcessHandling.LoadoutProcess.ProcessName == "Loadout")
                    {
                        /* Once we find Loadout, we immediately save the path */
                        if (ProcessHandling.LoadoutProcess.MainModule != null)
                        {
                            if (ProcessHandling.LoadoutPath != ProcessHandling.LoadoutProcess.MainModule.FileName)
                            {
                                ProcessHandling.LoadoutPath = ProcessHandling.LoadoutProcess.MainModule.FileName;

                                Console.WriteLine("> Since Loadout was found, the path to the game files was saved.\n");

                                // Save file is being updated
                                Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
                                Filesave.SaveDataToFile(saveFile, true);

                                Console.WriteLine("> Save file changed and saved.\n");
                            }
                        }

                        // Console.WriteLine("> " + loadoutStandardProcess + " " + loadoutParentProcess);
                        if (ProcessHandling.LoadoutParentProcess.ProcessName == "SmartSteamLoader")
                        {
                            // sseUser = true;
                            Console.WriteLine("> The SmartSteamEmu Launcher is being used on Loadout.");

                            /* We save the path to SmartSteamEmu as SSEPath */
                            if (ProcessHandling.LoadoutParentProcess.MainModule != null)
                            {
                                if (ProcessHandling.SSEPath != ProcessHandling.LoadoutParentProcess.MainModule.FileName)
                                {
                                    ProcessHandling.SSEPath = ProcessHandling.LoadoutParentProcess.MainModule.FileName;

                                    Console.WriteLine("> Since you used SmartSteamEmu, the file path is being saved.\n");

                                    // Save file is being updated
                                    Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
                                    Filesave.SaveDataToFile(saveFile, true);

                                    Console.WriteLine("> Save file changed and saved.\n");
                                }
                            }
                        }
                        // This can be used to find out if Loadout has just been started or has been started some time ago
                        // Subtraction minimizes the chance of time manipulation
                        //uptimeInSec = DateTime.Now.Subtract(loadoutStandardProcess.StartTime).TotalSeconds;
                        ProcessHandling.UptimeInSec = ProcessHandling.LoadoutProcess.TotalProcessorTime.TotalSeconds;
                        break;
                    }
                }
                else { MainProperties.BreakOnFreshStart = true; }
                await Task.Delay(10);
            }
            WhileLoopNotification = false;

            this.MakingProgress(ref progressValue); // 13
            Console.WriteLine("-------------------------------> Loadout found! <-------------------------------");

            /* Check and if necessary wait to make sure that we can enter the game and load common maps safely */
            Console.WriteLine("\n> Waiting for endpoints to be initialized ...\n");

            for (int i = 30; i < 1600; i++)
            {
                /* Speed ranking (may vary): 1. [mapAddress]: 1470 ms, 2. [uesAddress]: 1550 ms, 4. [matchmakingAddress]: 2660 ms, 4. [uberentAddress]: 2660 ms
                     first start (may vary): 1. [mapAddress]: 1560 ms, 2. [uesAddress]: 1760 ms, 4. [matchmakingAddress]: 3830 ms, 4. [uberentAddress]: 3830 ms
                     longest duration measured: 4880 ms */
                MainProperties.ReadMemoryUberentString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess,
                    ProcessMemory.BasicEndpoints[0].Value, ProcessMemory.BasicEndpoints[0].Key);
                /* This is a chain reaction to keep checks at a minimum and receive no more than 1 (identical) error message in case of an error */
                if (MainProperties.ReadMemoryUberentString.Length > 0)
                {
                    MainProperties.ReadMemoryUesString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess,
                        ProcessMemory.BasicEndpoints[1].Value, ProcessMemory.BasicEndpoints[1].Key);

                    if (MainProperties.ReadMemoryUesString.Length > 0)
                    {
                        MainProperties.ReadMemoryMatchmakingString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess,
                            ProcessMemory.BasicEndpoints[2].Value, ProcessMemory.BasicEndpoints[2].Key);

                        if (MainProperties.ReadMemoryMatchmakingString.Length > 0)
                        {
                            MainProperties.ReadMemoryMapString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess,
                                ProcessMemory.MapAddress, MainProperties.DefaultMap);
                        }
                    }
                }

                this.MakingProgress(ref progressValue); // 26

                /* If all memory was successfully read, we move on */
                if (MainProperties.ReadMemoryUberentString.Length > 0 && MainProperties.ReadMemoryMatchmakingString.Length > 0 && MainProperties.ReadMemoryUesString.Length > 0 && MainProperties.ReadMemoryMapString.Length > 0)
                {
                    Console.WriteLine("> [Try " + Math.Ceiling((double)i / 400) + "] Endpoints are now initialized.");
                    /* Output of each read memory string */
                    Console.WriteLine("> [" + MainProperties.ReadMemoryUberentString + "] [" + MainProperties.ReadMemoryUesString + "] [" + MainProperties.ReadMemoryMatchmakingString + "] [" + MainProperties.ReadMemoryMapString + "]\n");
                    break;
                }
                else
                {
                    /* If Loadout has been started between 1470 ms and 3830 ms before the patcher was opened, we wait */
                    if (!String.IsNullOrEmpty(MainProperties.ReadMemoryUberentString) || !String.IsNullOrEmpty(MainProperties.ReadMemoryMatchmakingString) ||
                        !String.IsNullOrEmpty(MainProperties.ReadMemoryUesString) || !String.IsNullOrEmpty(MainProperties.ReadMemoryMapString))
                    {
                        /* Extra check for error codes */
                        if (ProcessMemory.GetLastErrorOfProcessMemory())
                        {
                            MainProperties.PatcherReset = true;
                            MainProperties.Patched = false;
                        }
                        else if (i < 35 && ProcessHandling.UptimeInSec < 5) { await Task.Delay(2800); }
                        MainProperties.BreakOnFreshStart = true;

                        /* If Loadout gets started after the patcher and the checks for initialized endpoints failed a few times, we wait */
                    }
                    else if (MainProperties.BreakOnFreshStart = true && i == 34 && ProcessHandling.UptimeInSec < 5) { await Task.Delay(3200); }
                    /* Waiting is critical here. We accelerate if necessary */
                    await Task.Delay(1 + 19 / (i / 30));
                }
                if (i % 400 == 0) { Console.WriteLine("> [Try " + i / 400 + "] Endpoints are not initialized. This can happen on slow machines.\n"); }
                /* After a good break, we secretly try it one more time and let the user continue in case the results were "false negative" */
                if (i == 1598)
                {
                    Console.WriteLine("\n> Endpoints are still not initialized! Continuing with risk ...\n");
                    await Task.Delay(2600);
                }
            }

            this.MakingProgress(ref progressValue); // 39

            /* Let's patch the endpoints */

            /* patching the basic endpoints [uberentEndpoint], [uesEndpoint] and [matchmakingEndpoint] to be able to play the game */
            foreach (KeyValuePair<string, int> endpointToOverwriteAndAddress in ProcessMemory.BasicEndpoints)
            {
                MainProperties.ReadMemoryEndpointString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, endpointToOverwriteAndAddress.Value,
                    endpointToOverwriteAndAddress.Key, MainProperties.NewEndpoint);
                if (ProcessMemory.GetLastErrorOfProcessMemory())
                {
                    MainProperties.PatcherReset = true;
                    MainProperties.Patched = false;
                }
            }

            this.MakingProgress(ref progressValue); // 52

            //Map.SetMapWithInteractions(true); // NoPickupsCheck can't be used here
            // Needed for the Map.MapOrMapAltDecider() method
            Map.SetStartingMap(MainProperties.StartingMap);
            // && Map.MapOrMapAltDecider() != MainProperties.StartingMap.FullMapName
            /* We patch the default map with the starting map that was found in the save file. If not, we attempt to patch defaultMapReadMemory at least */
            if (!String.IsNullOrEmpty(Map.MapOrMapAltDecider()))
            {
                /* patching [DefaultMap] */
                /*
                MainProperties.ReadMemoryMapString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, ProcessMemory.MapAddress, MainProperties.DefaultMap, Map.MapOrMapAltDecider(), true);
                if (ProcessMemory.GetLastErrorOfProcessMemory())
                {
                    MainProperties.PatcherReset = true;
                    MainProperties.Patched = false;
                }
                */

                // TODO: Influence mapWithHealthPickups in Map.cs

                /* patching [StartingMap] */
                MainProperties.ReadMemoryMapString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, ProcessMemory.MapAddress, MainProperties.DefaultMap, Map.MapOrMapAltDecider(), true);
                if (ProcessMemory.GetLastErrorOfProcessMemory())
                {
                    MainProperties.PatcherReset = true;
                    MainProperties.Patched = false;
                }

                Console.WriteLine("-----------------------------> Map patching done! <-----------------------------\n");
                /* We play a random success sound, a melee hit sound, from Axl, Helga or T-Bone if enabled */
                Sound.PlaySuccessSoundsHitRandomly();

                /* We fetch aliases from existing hashtables. */
                MainProperties.MatchingAliasMap = Map.FetchMatchingAliasMap(MainProperties.StartingMap.BaseMap);
                MainProperties.MatchingAliasGameMode = Map.FetchMatchingAliasGameMode(MainProperties.StartingMap.GameMode);

                this.MakingProgress(ref progressValue); // 65

                Console.WriteLine("-> The following map has been set automatically.");
                Console.WriteLine("-> Change the starting map if you experience errors.");
                // This is the only place where MainProperties.DefaultMap must still be used!
                Console.WriteLine("-> The safest map is called: {0}\n", MainProperties.DefaultMap);
                Console.WriteLine("-> [= Complete] Starting map: {0}", Map.MapOrMapAltDecider());
                if (!String.IsNullOrEmpty(MainProperties.MatchingAliasMap)) { Console.WriteLine("-> [= Complete] Map known as: {0}", MainProperties.MatchingAliasMap); }
                if (!String.IsNullOrEmpty(MainProperties.MatchingAliasGameMode)) { Console.WriteLine("-> [= Complete] Game mode is: {0}", MainProperties.MatchingAliasGameMode); }

                /* We make sure the starting map will be treated as the new map */
                MainProperties.NewMap = Map.MapOrMapAltDecider();
                Console.WriteLine("");
            }
            else
            {
                this.MakingProgress(ref progressValue); // 65
                /* patching [defaultMapReadMemory] */
                /* shooting_gallery_solo could give more options than Shooting_Gallery_Solo, allowing interactions */
                /* but shooting_gallery_solo and Shooting_Gallery_Solo are equal because it generally offers no interactions */
                MainProperties.ReadMemoryMapString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, ProcessMemory.MapAddress, ProcessMemory.DefaultMapReadMemory, MainProperties.DefaultMap, true);
                if (ProcessMemory.GetLastErrorOfProcessMemory())
                {
                    MainProperties.PatcherReset = true;
                    MainProperties.Patched = false;
                }
            }

            this.MakingProgress(ref progressValue); // 78

            Console.WriteLine("---------------------------> Endpoint patching done <---------------------------");
            MainProperties.PatcherReset = false;

            /* We must let the user know that */
            Console.WriteLine("\n>> Follow these steps to safely apply a map change:");
            Console.WriteLine(">> 1. Click on a different ingame tab once.");
            Console.WriteLine(">> 2. Wait for 4 seconds.");
            Console.WriteLine(">> 3. Enter the map!\n");
            Console.WriteLine(">> Not following these steps can lead to a game crash.\n");
            /* If we have remaining time because we didn't need it, we use it here to give this message more attention */
            if (MainProperties.BreakOnFreshStart && ProcessHandling.UptimeInSec < 8) { await Task.Delay(1800); }

            this.MakingProgress(ref progressValue); // 91

            if (ProcessHandling.UptimeInSec < 8) { await Task.Delay(2600); }
            /* There is no downside patching again at this point */
            ProcessMemory.OverwriteStringAtOffsetSilent(ProcessMemory.BasicEndpoints[0].Value, MainProperties.NewEndpoint);
            ProcessMemory.OverwriteStringAtOffsetSilent(ProcessMemory.BasicEndpoints[1].Value, MainProperties.NewEndpoint);
            ProcessMemory.OverwriteStringAtOffsetSilent(ProcessMemory.BasicEndpoints[2].Value, MainProperties.NewEndpoint);
            if (!String.IsNullOrEmpty(Map.MapOrMapAltDecider()) && Map.MapOrMapAltDecider() != MainProperties.DefaultMap)
            {
                ProcessMemory.OverwriteStringAtOffsetSilent(ProcessMemory.MapAddress, Map.MapOrMapAltDecider(), true);
            }
            else
            {
                ProcessMemory.OverwriteStringAtOffsetSilent(ProcessMemory.MapAddress, MainProperties.DefaultMap, true);
            }

            // We finish by setting patched true, so we can continue in the Map Patching tab that checks for it.
            MainProperties.Patched = true;

            this.MakingProgress(ref progressValue); // 99
            PatchingNotification = false;
            PatchedText = true;
            // The progress value must be 99 because there's some extra loading. Once done, we move it to 100
            ProgressValue = 100;
            PatchingEnabled = true;
            IsPatching = false;
        }



        /**
        if (newEndpoint == "")
        {
            TypedUserInput.SetEndpoint(ProcessMemory.defaultPatcherEndpoint, out newEndpoint);

            Console.Write("\n> Keep this endpoint for further use? (y/n, y is default): ");
            char saveEndpoint = 'n';


            saveEndpoint = Console.ReadKey(false).KeyChar;


            if (saveEndpoint != 'n')
            {
                if (Filesave.SaveDataIntoFile(newEndpoint, Filesave.directorySeparator, Filesave.path1, Filesave.path2)) { Console.WriteLine(Filesave.saveSuccess); }
                else { Console.WriteLine(Filesave.saveError + "endpoint.\n"); }
            }
        }
        **/
    }

    public const int SHCNE_ASSOCCHANGED = 0x8000000;
    public const int SHCNF_IDLIST = 0;

    [System.Runtime.InteropServices.DllImport("Shell32.dll")]
    private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

    /// <summary>
    /// Refreshes the entire desktop
    /// </summary>
    public static void RefreshDesktop()
    {
        SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
    }
}
