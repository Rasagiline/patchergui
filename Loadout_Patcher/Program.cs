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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Numerics;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using System.Net;

using System.Linq;
using System.Threading;

using Avalonia;
using Avalonia.ReactiveUI;
using static Loadout_Patcher.Filesave;
using static Loadout_Patcher.Map;
using Loadout_Patcher.ViewModels;
using System.Globalization;

namespace Loadout_Patcher
{
    /// <summary>
    /// Save file configuration happens here! (e. g. LoadSaveFileIntoEverything())
    /// Center uses what all classes can provide for a clear and coordinated process
    /// </summary>
    public class Center : Multiplayer
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static int Main(string[] args)
        {
            SaveFile saveFile = new SaveFile();

            /**
            // Eventual changes required
            ProcessMemory processMemory = new ProcessMemory();
            saveFile = new SaveFile
            {
                SaveFileInfo = new SaveFileInfo
                {
                    Id = 1,
                    Type = "Main",
                    CreationDate = ReadFileCreationDateDirectly(DefaultSavePath),
                    LastTimeSaved = DateTime.Now.ToString("dd MMMM yyyy hh:mm:ss tt"),
                    SaveFilePath = "patcherSave.xml",
                    SaveFilePathBackup = "patcherSaveBackup.xml"
                },
                Username = "No username",
                PlayerId = 6561198066760375,
                IpAddress = "",
                ParentProcess = "SmartSteamLoader.exe",
                WebApiEndpoints = new List<string>
            {
                "api.loadout.rip"
            },
                RuntimeInfo = new ProcessMemory.RuntimeInfo
                {
                    NetInstallation = ProcessMemory.GetNetInstallation(),
                    OsDescription = ProcessMemory.GetOsDescription(),
                    RuntimeId = ProcessMemory.GetRuntimeId(),
                    ProcessorArchitecture = ProcessMemory.GetProcessorArchitecture(),
                    OsPlatform = ProcessMemory.GetOsPlatform()
                },
                GuiTitle = "Loadout Patcher",
                UserSetOptions = new GUI.UserSetOptions
                {
                    MapSearchPreferences = new GUI.MapSearchPreferences
                    {
                        BlacklistPreference = true,
                        WhitelistPreference = false,
                        ExcludePve = false,
                        ShowPveOnly = false
                    },
                    MapQueuePreferences = new GUI.MapQueuePreferences
                    {
                        FillEmptyMapQueueWithFavoriteMaps = false,
                        LoopMapQueue = false
                    },
                    SkipStartPage = false
                },
                StartingMap = new Map.LoadoutMap
                {
                    id = "1511501515",
                    fullMapName = "shooting_gallery_solo",
                    fullMapNameAlt = "Shooting_Gallery_Solo",
                    baseMap = "shooting_gallery_solo",
                    dayNight = "day",
                    gameMode = "solo"
                },
                MapQueue = new List<LoadoutMap>
            {
                new LoadoutMap {}
            },
                FavoriteMaps = new List<LoadoutMap>
            {
                new LoadoutMap {}
            },
                CustomMaps = new List<LoadoutMap>
            {
                new LoadoutMap {}
            },
                AdditionalMapHashtableEntries = new List<(Map.HashtablesEnum, KeyValuePair<string, string>)>(),
                MapBlacklist = new Hashtable
                { },
                MapWhitelist = new Hashtable
                { }
            };

            int numberOfSavesFiles = SaveDataToFile(saveFile);
            **/

            // If saveFile isn't found, get a new template!
            MultiplayerPageViewModel.FirstTimeTableDataLoad = 0;
            string fileLocation = Filesave.SaveFileLocator();
            if (fileLocation == "")
            {
                // A template fills the save file if none exists.
                saveFile = GetNewSaveFileTemplate();
                // We save the file before we load it if none existed.
                SaveDataToFile(saveFile);
                fileLocation = Filesave.SaveFileLocator();
                // We load the save file template that hasn't existed before.
                LoadSaveDataFromFile(ref saveFile, fileLocation, false);
            }
            else
            {
                // The save file variable will be filled because of the ref keyword in the method.
                if (!LoadSaveDataFromFile(ref saveFile, fileLocation, false))
                {
                    /* We try to load from the backup save file if loading from the save file failed */
                    LoadSaveDataFromFile(ref saveFile, fileLocation, true);
                }
            }
            // If the current save file is invalid, causing errors, use method GetNewSaveFileTemplate()

            if (saveFile.IpAddress == null)
            {
                Multiplayer.FindOutAndSetIpAddress();
                if (Multiplayer.IpAddress != null)
                {
                    saveFile.IpAddress = Multiplayer.IpAddress;
                    SaveDataToFile(saveFile);
                }
            }

            // Save file content is being spread all over the project. Properties can then be used to build a save file any time.
            Filesave.LoadSaveFileIntoEverything(saveFile);
            if (Map.GetMapBlacklist().Count > 0 && GUI.BlacklistPreference)
            {
                Console.WriteLine("> Your blacklist is active.");
                Console.WriteLine("> You have at least 1 map in your blacklist.\n");
            }
            /* This might be redundant as soon as the checkboxes can be disabled via binding */
            if (OperatingSystem.IsLinux())
            {
                GUI.StartLoadout = false;
                GUI.StartLoadoutViaSSE = false;
                GUI.CreateSSEShortcut = false;
                Console.WriteLine("> Options about autostarting Loadout or creating a shortcut are not available on Linux!\n");
                Sound.AllSoundsMuted = true;
                Sound.SuccessSounds = false;
                Sound.MinigameSounds = false;
                Sound.OtherSounds = false;
                Sound.MenuMusic = false;
                Console.WriteLine("> Sound doesn't work on Linux!\n");
                Console.WriteLine("> Linux users currently experience crashes. Native Linux support is planned!\n");
            }

            /**
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
            **/

            /**          
            [DataMember(Name = "Save_File_Info")]
            public SaveFileInfo SaveFileInfo { get; set; } // Information about the save file and backup save file [Filesave.cs]

            [DataMember(Name = "Username")]
            public string Username { get; set; } // The username the user was asked for [Multiplayer.cs]

            [DataMember(Name = "Auto-Generated_Or_Manual_steamID64")]
            public long PlayerId { get; set; } // Auto-generated ID or manually inserted Steam ID (steamID64) [Multiplayer.cs]

            [DataMember(Name = "IP_Address")]
            public string IpAddress { get; set; } // IP address, either the one connected to the internet or localhost [Multiplayer.cs]

            [DataMember(Name = "Parent_Process")]
            public string ParentProcess { get; set; } // Example: SmartSteamLoader.exe (.MainModule.ModuleName) [ProcessHandling.cs]

            [DataMember(Name = "Web_API_Endpoints")]
            public List<string> WebApiEndpoints { get; set; } // 1st entry is the main endpoint, switching with alts will be allowed [ProcessMemory.cs]

            [DataMember(Name = "Runtime_Info")]
            public ProcessMemory.RuntimeInfo RuntimeInfo { get; set; } // which OS platform and more [ProcessMemory.cs]

            [DataMember(Name = "GUI_Title")]
            public string GuiTitle { get; set; } // The title of the GUI will be customizable [GUI.cs]

            [DataMember(Name = "Map_Related_Preferences")]
            public GUI.UserSetOptions UserSetOptions { get; set; } // contains structs MapSearchPreferences and MapQueuePreferences as well as SkipStartPage [GUI.cs]

            [DataMember(Name = "Starting_Map")]
            public Map.LoadoutMap StartingMap { get; set; } // struct [Map.cs]

            [DataMember(Name = "Map_Queue")]
            public List<Map.LoadoutMap> MapQueue { get; set; } // stack of struct OR alternatively list of struct (from Stack<>) [Map.cs]

            [DataMember(Name = "Favorite_Maps")]
            public List<Map.LoadoutMap> FavoriteMaps { get; set; } // list of struct [Map.cs]

            [DataMember(Name = "Custom_Maps")]
            public List<Map.LoadoutMap> CustomMaps { get; set; } // list of struct [Map.cs]

            [DataMember(Name = "Custom_Map_Details")]
            public List<(Map.HashtablesEnum, KeyValuePair<string, string>)> AdditionalMapHashtableEntries { get; set; }  // includes aliases and definitions [Map.cs]

            [DataMember(Name = "Map_Deny_List")]
            public Hashtable MapBlacklist { get; set; } // Hashtable [Map.cs]

            [DataMember(Name = "Map_Allow_List")]
            public Hashtable MapWhitelist { get; set; } // Hashtable [Map.cs]
            **/


            /**
                 We get a saved endpoint, default map, custom map and favorite map if we find them 

            if (Filesave.CheckFilePath(Filesave.path1, Filesave.directorySeparator))
            {
                FileStream streamCheck = File.Open(Filesave.path1, FileMode.OpenOrCreate);

                if (streamCheck.CanRead)
                {
                    streamCheck.Dispose();
                    streamCheck.Close();
                    StreamReader stream = new(Filesave.path1);
                                 Reserved index: 0 = Api, 1 = Starting map, 2 = Custom map, 3 = Favorite map - save slot [1], 4 = Favorite map - save slot [2], 5 = Favorite map - save slot [3]
                    string? readString = stream.ReadLine();
                    if (readString == null)
                    {
                        readString = "";
                    }

                    MainProperties.NewEndpoint = readString;
                    MainProperties.StartingMap = stream.ReadLine();
                    MainProperties.CustomMap = stream.ReadLine();
                    MainProperties.FavoriteMap1 = stream.ReadLine();
                    MainProperties.FavoriteMap2 = stream.ReadLine();
                    MainProperties.FavoriteMap3 = stream.ReadLine();
                    stream.Dispose();
                    stream.Close();
                }
            }
            **/

            var builder = BuildAvaloniaApp();
            double GetScaling()
            {
                var idx = Array.IndexOf(args, "--scaling");
                if (idx != 0 && args.Length > idx + 1 &&
                    double.TryParse(args[idx + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out var scaling))
                    return scaling;
                return 1;
            }
            if (args.Contains("--drm"))
            {
                SilenceConsole();
                // If Card0, Card1 and Card2 all don't work. You can also try:                 
                // return builder.StartLinuxFbDev(args);
                // return builder.StartLinuxDrm(args, "/dev/dri/card1");
                // return builder.StartLinuxDrm(args, "/dev/dri/card1", 1D);
                
                // No, this exactly the answer to the question. The default for card appears to be /dev/dri/card0,
                // but needs to be card1 on Raspberry Pi 4. Setting it explicitly makes the call succeed.
                // -
                return builder.StartLinuxDrm(args: args, card: "/dev/dri/card1", scaling: GetScaling());
            }

            try
            {
                return builder.StartWithClassicDesktopLifetime(args);
            }
            catch (NullReferenceException nullEx)
            {
                Console.WriteLine($"> Exception thrown!");
                Console.WriteLine($"> Object reference not set to an instance of an object: '{nullEx}'");
                Console.WriteLine($"> We noted this only happens when too many operations happen at once.");
                Console.WriteLine($"> The patcher is going to crash! Please make a screenshot and let us know!");
                Console.WriteLine($"> You may press a key in this window to stop the program.");
                /* We use a big task delay, so the user can screenshot the error */
                Console.ReadKey(false);
            }
            return builder.StartWithClassicDesktopLifetime(args);
        }


        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();

        private static void SilenceConsole()
        {
            new Thread(() =>
            {
                Console.CursorVisible = false;
                while (true)
                    Console.ReadKey(true);
            })
            { IsBackground = true }.Start();
        }

        /*
        private static void Functions() //Main()
        {
            ProcessHandling.ConsoleSettings();

            Extended options which will be listed below the standard map options
            string[] extendedMapOptions = new string[8] { "Set a new starting map (Type it in)","Set a custom map (Type it in)","Set your favorite map (Type it in)",
                "Load custom map: ","Load favorite map [1]: ","Load favorite map [2]: ","Load favorite map [3]: ","Save the current map as ..." };

            int totalSuffixes = Map.suffixes.Length;
            int totalMaps = Map.maps.Length;
            int totalBaseMaps = Map.baseMapOptions.Length;

            string readMemoryEndpointString = ""; // neutral naming (uberent, ues, or matchmaking)
            string readMemoryUberentString;
            string readMemoryUesString = "";
            string readMemoryMatchmakingString = "";
            string readMemoryMapString = "";

            string newEndpoint = "";
            string newMap = "";
            const string defaultMap = "shooting_gallery_solo";
            string? startingMap = null;
            string? customMap = null;
            string? favoriteMap;
            string? favoriteMap1 = null;
            string? favoriteMap2 = null;
            string? favoriteMap3 = null;
            string matchingAliasMap;
            string matchingAliasGameMode;

            bool breakOnFreshStart = false;
            bool patcherRestarted = false;
            





            Header
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>> Loadout Reloaded Patcher <<<<<<<<<<<<<<<<<<<<<<<<<<<");
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>> Made by: Reloaded Team <<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            Console.WriteLine("--------------------------------------------------------------------------------\n");



            




        The main work such as patching and interacting with the user starts here
        restartPoint:
            try
            {

                if (patcherRestarted)
                {
                    Console.WriteLine("-----------------------> The patcher has been restarted <-----------------------\n");
                    ProcessMemory.CleanErrorsOfProcessMemory();
                    // sseUser = false;
                }


                We get a saved endpoint, default map, custom map and favorite map if we find them
                if (Filesave.CheckFilePath(Filesave.path1!, Filesave.directorySeparator))
                {
                    FileStream streamCheck = File.Open(Filesave.path1!, FileMode.OpenOrCreate);

                    if (streamCheck.CanRead)
                    {
                        streamCheck.Dispose();
                        streamCheck.Close();
                        StreamReader stream = new(Filesave.path1!);
                        Reserved index: 0 = Api, 1 = Starting map, 2 = Custom map, 3 = Favorite map - save slot [1], 4 = Favorite map - save slot [2], 5 = Favorite map - save slot [3]                     
                        string? readString = stream.ReadLine();
                        readString ??= "";
                        newEndpoint = readString;
                        startingMap = stream.ReadLine();
                        customMap = stream.ReadLine();
                        favoriteMap1 = stream.ReadLine();
                        favoriteMap2 = stream.ReadLine();
                        favoriteMap3 = stream.ReadLine();
                        stream.Dispose();
                        stream.Close();
                    }
                }
                Otherwise, we let the user insert an endpoint or simply choose the default one
                if (newEndpoint == "")
                {
                    TypedUserInput.SetEndpoint(ProcessMemory.DefaultPatcherEndpoint, out newEndpoint);

                    Console.Write("\n> Keep this endpoint for further use? (y/n, y is default): ");
                    char saveEndpoint = 'n';

                    Getting user input
                    saveEndpoint = Console.ReadKey(false).KeyChar;

                    We save the endpoint if the user insists
                    if (saveEndpoint != 'n')
                    {
                        if (Filesave.SaveDataIntoFile(newEndpoint, Filesave.directorySeparator, Filesave.path1!, Filesave.path2!)) { Console.WriteLine(Filesave.saveSuccess); }
                        else { Console.WriteLine(Filesave.saveError + "endpoint.\n"); }
                    }
                }

                Wait for the Loadout.exe process and continue once we get access
                Console.WriteLine("> Waiting for Loadout.exe ... (Loadout Beta is not supported)\n");

                while (true)
                {
                    ProcessHandling.LoadoutProcess = Snapshot.GetCurrentStandardAndParentProcess.Item1;
                    ProcessHandling.LoadoutParentProcess = Snapshot.GetCurrentStandardAndParentProcess.Item2;
                    if (ProcessHandling.LoadoutProcess != null && ProcessHandling.LoadoutParentProcess != null)
                    {
                        if (ProcessHandling.LoadoutProcess.ProcessName == "Loadout")
                        {
                            // Console.WriteLine("> " + loadoutStandardProcess + " " + loadoutParentProcess);
                            if (ProcessHandling.LoadoutParentProcess.ProcessName == "SmartSteamLoader")
                            {
                                // sseUser = true;
                                // Console.WriteLine("> The SmartSteamEmu Launcher is being used on Loadout.");
                            }
                            // This can be used to find out if Loadout has just been started or has been started some time ago
                            // Subtraction minimizes the chance of time manipulation
                            //uptimeInSec = DateTime.Now.Subtract(loadoutStandardProcess.StartTime).TotalSeconds;
                            ProcessHandling.UptimeInSec = ProcessHandling.LoadoutProcess.TotalProcessorTime.TotalSeconds;
                            break;
                        }
                    }
                    else { breakOnFreshStart = true; }
                    Thread.Sleep(10);
                }

                Console.WriteLine("-------------------------------> Loadout found! <-------------------------------");

                Check and if necessary wait to make sure that we can enter the game and load common maps safely
                Console.WriteLine("\n> Waiting for endpoints to be initialized ...\n");

                for (int i = 30; i < 1600; i++)
                {
                    Speed ranking (may vary): 1. [mapAddress]: 1470 ms, 2. [uesAddress]: 1550 ms, 4. [matchmakingAddress]: 2660 ms, 4. [uberentAddress]: 2660 ms
                         first start (may vary): 1. [mapAddress]: 1560 ms, 2. [uesAddress]: 1760 ms, 4. [matchmakingAddress]: 3830 ms, 4. [uberentAddress]: 3830 ms
                         longest duration measured: 4880 ms
                    readMemoryUberentString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess, 
                        ProcessMemory.BasicEndpoints[0].Value, ProcessMemory.BasicEndpoints[0].Key);
                    This is a chain reaction to keep checks at a minimum and receive no more than 1 (identical) error message in case of an error
                    if (readMemoryUberentString.Length > 0)
                    {
                        readMemoryUesString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess, 
                            ProcessMemory.BasicEndpoints[1].Value, ProcessMemory.BasicEndpoints[1].Key);

                        if (readMemoryUesString.Length > 0)
                        {
                            readMemoryMatchmakingString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess, 
                                ProcessMemory.BasicEndpoints[2].Value, ProcessMemory.BasicEndpoints[2].Key);

                            if (readMemoryMatchmakingString.Length > 0)
                            {
                                readMemoryMapString = ProcessMemory.CheckStringAtOffset(ProcessHandling.LoadoutProcess, 
                                    ProcessMemory.MapAddress, ProcessMemory.DefaultMapReadMemory);
                            }
                        }
                    }

                    If all memory was successfully read, we move on
                    if (readMemoryUberentString.Length > 0 && readMemoryMatchmakingString.Length > 0 && readMemoryUesString.Length > 0 && readMemoryMapString.Length > 0)
                    {
                        Console.WriteLine("> [Try " + Math.Ceiling((double)i / 400) + "] Endpoints are now initialized.");
                        Output of each read memory string
                        Console.WriteLine("> [" + readMemoryUberentString + "] [" + readMemoryUesString + "] [" + readMemoryMatchmakingString + "] [" + readMemoryMapString + "]\n");
                        break;
                    }
                    else
                    {
                        If Loadout has been started between 1470 ms and 3830 ms before the patcher was opened, we wait
                        if (!String.IsNullOrEmpty(readMemoryUberentString) || !String.IsNullOrEmpty(readMemoryMatchmakingString) ||
                            !String.IsNullOrEmpty(readMemoryUesString) || !String.IsNullOrEmpty(readMemoryMapString))
                        {
                            Extra check for error codes
                            if (ProcessMemory.GetLastErrorOfProcessMemory())
                            {
                                patcherRestarted = true;
                                goto restartPoint;
                            }
                            else if (i < 35) { Thread.Sleep(2800); }
                            breakOnFreshStart = true;

                            If Loadout gets started after the patcher and the checks for initialized endpoints failed a few times, we wait
                        }
                        else if (breakOnFreshStart = true && i == 34) { Thread.Sleep(3200); }
                        Waiting is critical here. We accelerate if necessary
                        Thread.Sleep(1 + 19 / (i / 30));
                    }
                    if (i % 400 == 0) { Console.WriteLine("> [Try " + i / 400 + "] Endpoints are not initialized. This can happen on slow machines.\n"); }
                    After a good break, we secretly try it one more time and let the user continue in case the results were "false negative"
                    if (i == 1598)
                    {
                        Console.WriteLine("\n> Endpoints are still not initialized! Continuing with risk ...\n");
                        Thread.Sleep(2600);
                    }
                }

                Let's patch the endpoints

                patching the basic endpoints [uberentEndpoint], [uesEndpoint] and [matchmakingEndpoint] to be able to play the game
                foreach (KeyValuePair<string, int> endpointToOverwriteAndAddress in ProcessMemory.BasicEndpoints)
                {
                    readMemoryEndpointString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, endpointToOverwriteAndAddress.Value, 
                        endpointToOverwriteAndAddress.Key, newEndpoint);
                    if (ProcessMemory.GetLastErrorOfProcessMemory())
                    {
                        patcherRestarted = true;
                        goto restartPoint;
                    }
                }

                We patch the default map with the starting map that was found in the save file. If not, we attempt to patch defaultMapReadMemory at least
                if (!String.IsNullOrEmpty(startingMap) && startingMap != defaultMap)
                {
                    patching [defaultMap]
                    readMemoryMapString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, ProcessMemory.MapAddress, defaultMap, startingMap, true);
                    if (ProcessMemory.GetLastErrorOfProcessMemory())
                    {
                        patcherRestarted = true;
                        goto restartPoint;
                    }

                    Console.WriteLine("-----------------------------> Map patching done! <-----------------------------\n");

                    matchingAliasMap = Map.FetchMatchingAlias(startingMap, Map.baseMapOptions, Map.baseMapAliases, totalBaseMaps);
                    matchingAliasGameMode = Map.FetchMatchingAlias(startingMap, Map.suffixes, Map.suffixAliases, totalSuffixes);

                    Console.WriteLine("-> The following map has been set automatically.");
                    Console.WriteLine("-> Change the starting map if you experience errors.");
                    Console.WriteLine("-> The safest map is called: {0}\n'", defaultMap);
                    Console.WriteLine("-> [= Complete] Starting map: {0}", startingMap);
                    if (!String.IsNullOrEmpty(matchingAliasMap)) { Console.WriteLine("-> [= Complete] Map known as: {0}", matchingAliasMap); }
                    if (!String.IsNullOrEmpty(matchingAliasGameMode)) { Console.WriteLine("-> [= Complete] Game mode is: {0}", matchingAliasGameMode); }

                    We make sure the starting map will be treated as the new map
                    newMap = startingMap;
                    Console.WriteLine("");
                }
                else
                {
                    patching [defaultMapReadMemory]
                    This should be the minimum happening because shooting_gallery_solo gives more options than Shooting_Gallery_Solo
                    readMemoryMapString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, ProcessMemory.MapAddress, ProcessMemory.DefaultMapReadMemory, defaultMap, true);
                    if (ProcessMemory.GetLastErrorOfProcessMemory())
                    {
                        patcherRestarted = true;
                        goto restartPoint;
                    }
                }

                Console.WriteLine("---------------------------> Endpoint patching done <---------------------------");

                We must let the user know that
                Console.WriteLine("\n>> Follow these steps to safely apply a map change:");
                Console.WriteLine(">> 1. Click on a different ingame tab once.");
                Console.WriteLine(">> 2. Wait for 4 seconds.");
                Console.WriteLine(">> 3. Enter the map!\n");
                Console.WriteLine(">> Not following these steps can lead to a game crash.\n");
                If we have remaining time because we didn't need it, we use it here to give this message more attention
                if (breakOnFreshStart) { Thread.Sleep(1800); }

                Thread.Sleep(2600);
                There is no downside patching again at this point
                ProcessMemory.OverwriteStringAtOffsetSilent(ProcessMemory.BasicEndpoints[0].Value, newEndpoint);
                ProcessMemory.OverwriteStringAtOffsetSilent(ProcessMemory.BasicEndpoints[1].Value, newEndpoint);
                ProcessMemory.OverwriteStringAtOffsetSilent(ProcessMemory.BasicEndpoints[2].Value, newEndpoint);
                if (!String.IsNullOrEmpty(startingMap) && startingMap != defaultMap)
                {
                    ProcessMemory.OverwriteStringAtOffsetSilent(ProcessMemory.MapAddress, startingMap, true);
                }
                else
                {
                    ProcessMemory.OverwriteStringAtOffsetSilent(ProcessMemory.MapAddress, defaultMap, true);
                }

                Map selection
                while (true)
                {
                    We prepare to loop through the base maps we got
                    string[] baseNames = new string[totalBaseMaps];
                    string? mapName = "";
                    int mapOptionIndex = 0;
                    Console.WriteLine("--------------------------------------------------------------------------------");
                    Console.WriteLine("><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><");
                    Console.WriteLine("><><><><><><><><><><><><><><><><>< Map Select ><><><><><><><><><><><><><><><><><");
                    Console.WriteLine("><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><");
                    Console.WriteLine("--------------------------------------------------------------------------------\n");

                    Look for unique base names to sort things out
                    for (int i = 0; i < totalMaps - 1; i++)
                    {
                        If an underscore wasn't found in the array's element, keep the full string which gets assigned to base name
                        int substringEnd = Map.maps[i].Contains('_') ? Map.maps[i].IndexOf('_') : Map.maps[i].Length;
                        string baseName = Map.maps[i].Substring(0, substringEnd);
                        
                        We adapt the base names a little to deal with edge cases
                        if (Map.maps[i] == "drillcavern_beta") { baseName = "drillcavern_beta_kc"; }
                        else if (baseName == "fath") { baseName = "fath_705"; }
                        else if (baseName == "fissurenight") { baseName = "fissure"; }
                        else if (baseName == "gliese") { baseName = "gliese_581"; }
                        else if (baseName == "level") { baseName = "level_three"; }
                        else if (baseName == "projectx") { baseName = "projectx_tc"; }
                        else if (baseName == "shooting") { baseName = "shooting_gallery_solo"; }
                        else if (baseName == "test") { baseName = "test_territorycontrol"; }
                        else if (baseName == "thepit") { baseName = "thepit_pj"; }
                        else if (baseName == "two") { baseName = "two_port"; }

                        Check which adapted base name belongs to the mass of base names to assign it to the proper base name option
                        If the base name was not found in the array of base names that is being filled, give it a place
                        if (Array.IndexOf(baseNames, baseName) == -1)
                        {
                            baseNames[mapOptionIndex] = baseName;

                            Find the alias for the current suffix
                            string aliasOutput = "";
                            for (int j = 0; j < totalMaps; j++)
                            {
                                if (Map.baseMapOptions[j] == baseName)
                                {
                                    if (Map.baseMapAliases[j] != "")
                                    {
                                        aliasOutput = " (" + Map.baseMapAliases[j] + ")";
                                        Marking certain maps as [PvE ready]. This includes projectx_tc and all maps that support Hold Your Pole as game mode
                                        if (baseName == "drillcavern" || baseName == "fath_705" || baseName == "fissure" || baseName == "gliese_581" || baseName == "level_three" ||
                                            baseName == "shattered" || baseName == "spires" || baseName == "thefreezer" || baseName == "projectx_tc")
                                        {
                                            aliasOutput += " [PvE ready]";
                                        }
                                        break;
                                    }
                                }
                            }

                            Display the first set of options
                            string closingBracketAndIndention = "";
                            if (mapOptionIndex < 10)
                            {
                                closingBracketAndIndention = ")  ";
                            }
                            else
                            {
                                closingBracketAndIndention = ") ";
                            }
                            Console.WriteLine("({0}{1}{2}{3}", mapOptionIndex++, closingBracketAndIndention, baseName, aliasOutput);
                        }
                    }

                    Console.WriteLine("\n--------------------------------------------------------------------------------\n");
                    We offer the additional options "Set starting map, Set custom map, Set favorite map, 
                    Custom map, Favorite maps [1] to [3] and Save current map as ..." for custom needs.
                    int extendedOptionIndex = mapOptionIndex;
                    int extendedMapOptionsArrayIndex = -1;
                    do
                    {
                        string extendedMapOption = extendedMapOptions[++extendedMapOptionsArrayIndex];
                        Custom map
                        if (extendedMapOptionsArrayIndex == 3)
                        {
                            if (customMap == "") { extendedMapOption += "No custom map found."; }
                            else { extendedMapOption += customMap; }
                        }
                        Favorite map [1]
                        else if (extendedMapOptionsArrayIndex == 4)
                        {
                            if (favoriteMap1 == "") { extendedMapOption += "No favorite map found."; }
                            else { extendedMapOption += favoriteMap1; }
                        }
                        Favorite map [2]
                        else if (extendedMapOptionsArrayIndex == 5)
                        {
                            if (favoriteMap2 == "") { extendedMapOption += "No favorite map found."; }
                            else { extendedMapOption += favoriteMap2; }
                        }
                        Favorite map [3]
                        else if (extendedMapOptionsArrayIndex == 6)
                        {
                            if (favoriteMap3 == "") { extendedMapOption += "No favorite map found."; }
                            else { extendedMapOption += favoriteMap3; }
                        }
                        Console.WriteLine("({0}) {1}", extendedOptionIndex, extendedMapOption);


                    } while (++extendedOptionIndex < mapOptionIndex + extendedMapOptions.Length);
                    Console.WriteLine("\n--------------------------------------------------------------------------------");

                    int option = TypedUserInput.GetUserNumberSelection("> Select your map: ", --extendedOptionIndex);
                    if (option >= mapOptionIndex)
                    {
                        extendedMapOptionsArrayIndex = option - mapOptionIndex;
                    }
                    else
                    {
                        mapName = baseNames[option];
                    }
                    if (option != mapOptionIndex + 7)
                    {
                        newMap = mapName;
                    }

                    We lock in the extended map option as selected option
                    if (mapName == "")
                    {
                        The map string needs to be filled here. We name it "Not a map" for easy error handling
                        mapName = "Not a map";

                        switch (extendedMapOptionsArrayIndex)
                        {
                            case 3:
                                mapName = customMap;
                                break;
                            case 4:
                                mapName = favoriteMap1;
                                break;
                            case 5:
                                mapName = favoriteMap2;
                                break;
                            case 6:
                                mapName = favoriteMap3;
                                break;
                        }

                        if (String.IsNullOrEmpty(mapName))
                        {
                            Console.WriteLine("\n-> No map was found under that option!");
                            Console.WriteLine("-> Check out the and \"Set\" and \"Save\" options.\n");
                            Thread.Sleep(260);
                            continue;
                        }
                    };

                    matchingAliasMap = Map.FetchMatchingAlias(mapName, Map.baseMapOptions, Map.baseMapAliases, mapOptionIndex);
                    Console.WriteLine("\n-> [+ Map Option] Selected: {0}", mapName);
                    if (matchingAliasMap != "")
                    {
                        Console.WriteLine("-> [+ Map Option] Known as: {0}", matchingAliasMap);
                    }
                    Console.WriteLine();

                    Taking care of Day/Night and Suffixes
                    if (option < mapOptionIndex)
                    {
                        Day/Night variants
                        string dayNightMap = "";
                        char dayNightInput = 'd';
                        if (mapName == "drillcavern" || mapName == "fissure" || mapName == "gliese_581")
                        {
                            Console.WriteLine("><><><><><><><><><><><><><><>< Day / Night Select ><><><><><><><><><><><><><><><\n");
                            option = -1;
                            while (option == -1)
                            {
                                Console.Write("> Day or Night? (d/n): ");

                                Getting user input
                                dayNightInput = Console.ReadKey(false).KeyChar;

                                if (dayNightInput == 'd')
                                {
                                    option = 1;
                                    dayNightMap = " (map at day)";
                                }
                                else if (dayNightInput == 'n')
                                {
                                    option = 2;
                                    dayNightMap = " (map at night)";
                                }
                            }
                            if (option == 2)
                            {
                                We check for an edge case within one of the night cases
                                if (mapName == "fissure")
                                {
                                    newMap = mapName + "night";
                                }
                                else
                                {
                                    newMap = mapName + "_night";
                                }
                            }
                            Console.WriteLine("\n\n-> [+ Day/Night] Selected: {0}{1}\n", newMap, dayNightMap);
                        }

                        Suffixes
                        if (newMap != "test_territorycontrol" && newMap != "projectx_tc" && newMap != "thepit_pj" && newMap != "shooting_gallery_solo" && newMap != "drillcavern_beta_kc")
                        {
                            string[] mapSuffixes = new string[totalSuffixes];
                            int nMapSuffixes = 0;
                            bool[] validSuffixes = new bool[totalSuffixes];

                            for (int i = 0; i < totalMaps; i++)
                            {
                                Look only for maps that match the chosen one
                                if (Map.maps[i].Contains(newMap))
                                {
                                    if (dayNightInput == 'd')
                                    {
                                        Remove night maps from the results
                                        if (!Map.maps[i].Contains("night"))
                                            Handle the special case of "locomotiongym" that has no underscore
                                            if (Map.maps[i].Substring(Map.maps[i].LastIndexOf('_') + 1) != "locomotiongym")
                                            {
                                                mapSuffixes[nMapSuffixes] = Map.maps[i].Substring(Map.maps[i].LastIndexOf('_') + 1);
                                                nMapSuffixes++;
                                            }
                                            else
                                            {
                                                mapSuffixes[nMapSuffixes] = "";
                                                nMapSuffixes++;
                                            }
                                    }
                                    else
                                    {
                                        mapSuffixes[nMapSuffixes] = Map.maps[i].Substring(Map.maps[i].LastIndexOf('_') + 1);
                                        nMapSuffixes++;
                                    }
                                }
                            }

                            Console.WriteLine("><><><><><><><><><><><><><><><>< Game Mode Select ><><><><><><><><><><><><><><><\n");
                            Displaying available game modes of the chosen base map
                            for (int i = 0; i < nMapSuffixes; i++)
                            {
                                string alias = "";
                                string mapSuffixesOutput;

                                Find the alias for the current suffix
                                for (int j = 0; j < totalSuffixes; j++)
                                {
                                    if (Map.suffixes[j] == mapSuffixes[i])
                                    {
                                        alias = Map.suffixAliases[j];
                                    }
                                }

                                if (mapSuffixes[i] == "")
                                {
                                    mapSuffixesOutput = " ";
                                }
                                else
                                {
                                    mapSuffixesOutput = " " + mapSuffixes[i] + " ";
                                }
                                Console.WriteLine("({0}){1}({2})", i, mapSuffixesOutput, alias);
                            }
                            option = TypedUserInput.GetUserNumberSelection("> Select the game mode: ", --nMapSuffixes);

                            if (mapSuffixes[option] != "" && mapSuffixes[option] != "None")
                            {
                                newMap += "_" + mapSuffixes[option];
                            }

                            matchingAliasGameMode = Map.FetchMatchingAlias(newMap, Map.suffixes, Map.suffixAliases, totalSuffixes);
                            Console.WriteLine("\n-> [+ Game Mode] Selected: {0}", newMap);
                            if (matchingAliasGameMode != "")
                            {
                                Console.WriteLine("-> [+ Game Mode] Known as: {0}", matchingAliasGameMode);
                            }
                            Console.WriteLine();
                        }
                    }

                    Extended option: Save a new starting map
                    else if (option == mapOptionIndex)
                    {
                        Console.WriteLine("> The default starting map is: {0}", defaultMap);
                        option = -1;
                        We continue if the user accepts the risk to crash the game whenever the starting map gets loaded
                        while (option == -1)
                        {
                            if (!String.IsNullOrEmpty(startingMap) && defaultMap != startingMap)
                            {
                                Console.WriteLine("> The current starting map is: {0}", startingMap);
                                This option can be used to find out what the starting map is. No can be chosen to do nothing more than that
                                Console.Write("> This isn't the default starting map. Do you want to change it? (y/n): ");
                            }
                            else
                            {
                                Console.Write("> Caution: A different starting map can cause errors. Continue? (y/n): ");
                            }

                            char yesNoInput;

                            Getting user input
                            yesNoInput = Console.ReadKey(false).KeyChar;

                            if (yesNoInput == 'y')
                            {
                                option = 1;
                            }
                            else if (yesNoInput == 'n')
                            {
                                option = 2;
                            }
                            Console.WriteLine();
                        }

                        if (option == 1)
                        {
                            TypedUserInput.GetAndCheckUserString("> Enter your starting map: \n", out startingMap, out newMap);

                            We save the new starting map
                            if (Filesave.SaveDataIntoFile(newMap, Filesave.directorySeparator, Filesave.path1!, Filesave.path2!, 1))
                            {
                                Console.WriteLine(Filesave.saveSuccess);
                            }
                            else
                            {
                                Console.WriteLine("{0}new starting map.", Filesave.saveError);
                            }
                            Console.WriteLine();
                        }
                        else
                        {
                            continue;
                        }
                    }

                    Extended option: Insert a custom map
                    else if (option == mapOptionIndex + 1)
                    {
                        TypedUserInput.GetAndCheckUserString("> Enter your custom map: \n", out customMap, out newMap, true);

                        Console.Write("\n> Keep the entered map for further use? (y/n, y is default): ");
                        char saveCustomMap;

                        Getting user input
                        saveCustomMap = Console.ReadKey(false).KeyChar;

                        We save the custom map
                        if (saveCustomMap != 'n')
                        {
                            if (Filesave.SaveDataIntoFile(newMap, Filesave.directorySeparator, Filesave.path1!, Filesave.path2!, 2))
                            {
                                Console.WriteLine(Filesave.saveSuccess);
                            }
                            else
                            {
                                Console.WriteLine("{0}custom map.", Filesave.saveError);
                            }
                        }
                        Console.WriteLine();
                    }

                    Extended option: Insert the favorite map
                    else if (option == mapOptionIndex + 2)
                    {
                        We continue if the user picked a save slot for their favorite map
                        int saveMapOption = 0;
                        Console.WriteLine("> Choose a save slot for your favorite map:\n");
                        Console.WriteLine("({0}) Save slot [1]", saveMapOption++);
                        Console.WriteLine("({0}) Save slot [2]", saveMapOption++);
                        Console.WriteLine("({0}) Save slot [3]", saveMapOption);
                        Console.WriteLine("> The options are enclosed in round brackets as usual.\n");
                        saveMapOption = TypedUserInput.GetUserNumberSelection("> [0 / 1 / 2] Decision: ", saveMapOption);
                        Console.WriteLine();

                        TypedUserInput.GetAndCheckUserString("> Enter your favorite map: \n", out favoriteMap, out newMap);

                        We assign the favorite map to its matching save slot and the save slot to the new map. The option represents the reserved position in the save file
                        switch (saveMapOption)
                        {
                            case 0:
                                favoriteMap1 = favoriteMap;
                                option = 3;
                                break;
                            case 1:
                                favoriteMap2 = favoriteMap;
                                option = 4;
                                break;
                            case 2:
                                favoriteMap3 = favoriteMap;
                                option = 5;
                                break;
                        }

                        We save the favorite map
                        if (Filesave.SaveDataIntoFile(newMap, Filesave.directorySeparator, Filesave.path1!, Filesave.path2!, option))
                        {
                            Console.WriteLine(Filesave.saveSuccess);
                        }
                        else
                        {
                            Console.WriteLine("{0}favorite map.", Filesave.saveError);
                        }
                        Console.WriteLine();
                    }

                    Extended option: Load a custom map
                    else if (option == mapOptionIndex + 3)
                    {
                        if (customMap != null)
                        {
                            newMap = customMap;
                        }
                    }

                    Extended option: Load the favorite map [1], [2] or [3]
                    else if (option >= mapOptionIndex + 4 && option <= mapOptionIndex + 6)
                    {

                        switch (option - mapOptionIndex)
                        {
                            case 4:
                                if (favoriteMap1 != null)
                                {
                                    newMap = favoriteMap1;
                                }
                                break;

                            case 5:
                                if (favoriteMap2 != null)
                                {
                                    newMap = favoriteMap2;
                                }
                                break;

                            case 6:
                                if (favoriteMap3 != null)
                                {
                                    newMap = favoriteMap3;
                                }
                                break;
                        }
                    }

                    Extended option: Save the current map
                    else if (option == mapOptionIndex + 7)
                    {
                        if (newMap == "")
                        {
                            newMap = defaultMap;
                        }

                        We continue if the user decided what the current map should be saved as
                        int saveMapOption = 0;
                        Console.WriteLine("({0}) Cancel", saveMapOption++);
                        Console.WriteLine("({0}) * Starting map", saveMapOption++);
                        Console.WriteLine("({0}) Custom map", saveMapOption++);
                        Console.WriteLine("({0}) Favorite map [1]", saveMapOption++);
                        Console.WriteLine("({0}) Favorite map [2]", saveMapOption++);
                        Console.WriteLine("({0}) Favorite map [3]\n", saveMapOption);
                        Console.WriteLine("* Can cause unexpected results.");
                        Console.WriteLine("* Make sure the current map is always stable!\n");
                        Console.WriteLine("> The patcher will be restarted to apply the change.");
                        saveMapOption = TypedUserInput.GetUserNumberSelection("> [0 / 1 / 2 / 3 / 4 / 5] Decision: ", saveMapOption);

                        switch (saveMapOption)
                        {
                            case 1:
                                startingMap = newMap;
                                break;
                            case 2:
                                customMap = newMap;
                                break;
                            case 3:
                                favoriteMap1 = newMap;
                                break;
                            case 4:
                                favoriteMap2 = newMap;
                                break;
                            case 5:
                                favoriteMap3 = newMap;
                                break;
                        }

                        We save the new map if the user insisted
                        if (saveMapOption > 0)
                        {
                            Reserved index: 1 = Starting map, 2 = Custom map, 3 = Favorite map - save slot [1], 4 = Favorite map - save slot [2], 5 = Favorite map - save slot [3]
                            if (Filesave.SaveDataIntoFile(newMap, Filesave.directorySeparator, Filesave.path1!, Filesave.path2!, saveMapOption))
                            {
                                Console.WriteLine(Filesave.saveSuccess);
                                We apply the change
                                Console.WriteLine("> Restarting the patcher ...\n");
                                Thread.Sleep(2200);
                                breakOnFreshStart = false;
                                patcherRestarted = true;
                                goto restartPoint;
                            }
                            else
                            {
                                Console.WriteLine("{0}current map.\n", Filesave.saveError);
                            }
                        }
                        Console.WriteLine();
                        continue;
                    }

                    patching [readMemoryMapString]
                    readMemoryMapString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, ProcessMemory.MapAddress, readMemoryMapString, newMap, true);
                    if (ProcessMemory.GetLastErrorOfProcessMemory())
                    {
                        patcherRestarted = true;
                        goto restartPoint;
                    }
                    Console.WriteLine("-----------------------------> Map patching done! <-----------------------------\n");

                    matchingAliasMap = Map.FetchMatchingAlias(newMap, Map.baseMapOptions, Map.baseMapAliases, mapOptionIndex);
                    matchingAliasGameMode = Map.FetchMatchingAlias(newMap, Map.suffixes, Map.suffixAliases, totalSuffixes);

                    Console.WriteLine("-> [= Complete] Map selected: {0}", newMap);
                    if (matchingAliasMap != "")
                    {
                        Console.WriteLine("-> [= Complete] Map known as: {0}", matchingAliasMap);
                    }
                    if (matchingAliasGameMode != "")
                    {
                        Console.WriteLine("-> [= Complete] Game mode is: {0}", matchingAliasGameMode);
                    }
                    Console.WriteLine();

                }
                // while() of the entire map select just ended





            }
            In case of an error from the patcher, the error that could be related to external methods gets displayed
            catch (MarshalDirectiveException sourceCodeError)
            {
                Console.WriteLine("\n> Critical error: Check if the external method supports the data type.");
                Console.WriteLine("> Change the source code of the patcher in order to fix it.");
                Console.WriteLine("> Press a key to receive full information of the error.");
                Console.ReadKey(true);
                Console.WriteLine("> Stack trace: {0}", sourceCodeError.StackTrace);
                Console.WriteLine("> Source: {0}", sourceCodeError.Source);
                Console.WriteLine("> Data: {0}", sourceCodeError.Data);
                Console.WriteLine("> Message: {0}", sourceCodeError.Message);
                Console.WriteLine("> Press a key to continue.");
                Console.ReadKey(true);
                // Keep the patcher running
                patcherRestarted = true;
                goto restartPoint;
            }
            In case of any other unexpected error from the patcher, the error gets displayed
            catch (Exception ex)
            {
                Console.WriteLine("\n> An unknown error occured!");
                Console.WriteLine("> Press a key to receive the full information of the error.");
                Console.ReadKey(true);
                Console.WriteLine("> Inner exception: {0}", ex.InnerException);
                Console.WriteLine("> Stack trace: {0}", ex.StackTrace);
                Console.WriteLine("> Source: {0}", ex.Source);
                Console.WriteLine("> Data: {0}", ex.Data);
                Console.WriteLine("> Message: {0}", ex.Message);
                Console.WriteLine("> Press a key to continue.");
                Console.ReadKey(true);
                // Keep the patcher running
                patcherRestarted = true;
                goto restartPoint;
            }
        }
        */
    }
}