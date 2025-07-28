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
//using Avalonia;
//using Loadout_Patcher.ViewModels;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.InteropServices;
using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;
using System.Xml;
using static Loadout_Patcher.Filesave;
//using static Loadout_Patcher.ViewModels.MultiplayerPageViewModel;

namespace Loadout_Patcher
{
    /// <summary>
    /// Filesave is for saving and loading data with the use of a save file being used for serialization and deserialization
    /// </summary>
    public class Filesave
    {
        // public const string saveSuccess = "--------------------------------> Saving done! <--------------------------------";
        // public const string saveError = "> Something went wrong when trying to save the ";

        // TODO: Delete the old paths
        //public static char directorySeparator = Path.DirectorySeparatorChar;
        // public static string path1 = $".{directorySeparator}patcherSave.txt";
        // public static string path2 = $".{directorySeparator}temp.txt";
        // public static string path3 = $".{directorySeparator}patcherSaveBackup.txt";
        //public static string? path1 = null;
        //public static string? path2 = null;
        //public static string? path3 = null;

        // TODO:
        // Have an internal save file, if that save file contains [use this as main save file: true], then proceed, else
        // try loading external backup save file from path that is written in intern save file and if there is nothing,
        // ask the user where to create/overwrite the external XML save file -> popup of a directory search
        // so: the internal xml will contain external path, default empty and boolean for being the main save file

        // TODO:
        // Put in the last saved data in case it needs to be restored (serves as backup)
        // Restoring save file contents can become a new option
        // a static struct with the saved content can be serialized at any time

        // TODO:
        // Add a ban list for maps blacklistedMaps that shouldn't appear, going into the save file
        // The ban list starts as an empty Hashtable, perfectly matching anything from any of the hashtables
        // It's a strong decision to go with the Hashtable because it can be ensured that nothing is listed twice
        // locomotiongym is the one exception where the Hashtable value makes any sense to separate none (game mode) from family
        // Add unbanning

        // TODO:
        // Add a whitelist for maps whitelistedMaps that should only appear, going into the save file
        // Having ban list and whitelist together is possible
        // There will be a radio button which one to use if both have entries

        // TODO:
        // If the user picks something that gives 0 results, a button can appear for blacklisting "Add criteria to blacklist"
        // Or better -> The blacklist may always contain by default:
        // {"thefreezer_kc_bots", "The Freezer\nGame mode: Special variant of Deathsnatch"},
        // {"cpr_bots", "Special variant of Blitz"},
        // {"kc_bots", "Special variant of Deathsnatch"},
        // {"three", "Unknown"},
        // "Your map blacklist isn't empty for good reasons. You can view and clear it at any time."
        // "Add a map to the blacklist"
        // "Add a criteria to the blacklist"
        // The whitelist may always contain by default:
        // {"shooting_gallery_solo", "Shooting Gallery\nGame mode: Solo"},

        // TODO:
        // Blacklists should work on custom maps while custom maps should still be available somewhere

        // Store a lot of information in a save file
        // Think in big dimensions
        // Once a save file is made and the patcher upgrade released, try to avoid enhancing for compatibility
        // The types: startingMap, customMap, favoriteMap, (currentMap), [mapQueue -> nextMap]
        // Endpoints, 1 or more, name of the parent process (ProcessHandling.LoadoutParentProcess.MainModule.ModuleName), 
        // save file creation date, last save date, save file and backup save file path including custom file names
        // Data Contract can also be: Save File [Windows] if Save File [Linux] must be different
        // - Alternatively, the XML file can be scanned for the data member and read the OS platform value out
        // Runtime Information struct in ProcessMemory.cs: .NET installation, OS description, runtime ID, process architecture, OS platform
        // username, custom GUI title, (long) Manual_Or_Auto-Generated_Player_ID (random or steam id or manual steam id), IP_Address
        // special user set options: preference blacklist or whitelist, exclude or show pve only,
        // if map queue is empty use favorite maps for new queue, looping queue active
        // TODO:
        // if the save file and backup save file are the default path, don't use the absolute path

        // The save file struct has get and set attributes, meaning there are no extra properties needed
        // In Map.cs the struct is something else because the fields on the top mustn't be static
        // Each DataMember needs to be static because the class is static and particular members may be changed

        // The SaveFileInfo properties can be outside and static in order to get and set values separately which means more flexibility
        public static byte Id { get; set; } // 1 to 255

        public static string? Type { get; set; } // "Main" or "Backup" is the string

        public static string? CreationDate { get; set; } // Save file's creation date

        public static string? LastTimeSaved { get; set; } // Most recent date the save file was saved

        public static string? SaveFilePath { get; set; } // Path and name of the save file, by default just the file name (no absolute path)

        public static string? SaveFilePathBackup { get; set; } // Path and name of the backup save file, by default just the file name (no absolute path)


        public Filesave()
        { }

        ///// <summary>
        ///// When creating an object, SaveFileInfo will be filled with what the parameters contain
        ///// This can to be used after loading a save file
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="id"></param>
        ///// <param name="creationDate"></param>
        ///// <param name="saveFilePath"></param>
        ///// <param name="saveFilePathBackup"></param>
        //public Filesave(byte id, string type = "Main", string creationDate = "", string lastTimeSaved = "", string saveFilePath = "", string saveFilePathBackup = "")
        //{
        //    Id = id;
        //    Type = type;
        //    CreationDate = creationDate;
        //    LastTimeSaved = lastTimeSaved;
        //    SaveFilePath = saveFilePath;
        //    SaveFilePathBackup = saveFilePathBackup;
        //}

        public static char DirectorySeparator { get; } = Path.DirectorySeparatorChar;
        public static string DefaultSavePath { get; set; } = $".{DirectorySeparator}patcherSave.xml";
        public static string DefaultSavePathBackup { get; set; } = $".{DirectorySeparator}patcherSaveBackup.xml";
        public static string DefaultSavePathBackupCopy { get; } = $".{DirectorySeparator}patcherSaveBackupCopy.xml";


        // Save file creation date, last save date, save file and backup save file path including custom file names
        public struct SaveFileInfo
        {
            public byte Id { get; set; } // 1 to 255

            public string Type { get; set; } // "Main" or "Backup" is the string

            public string CreationDate { get; set; } // Save file's creation date (.ToString())

            public string LastTimeSaved { get; set; } // Most recent date the save file was saved

            public string SaveFilePath { get; set; } // Path and name of the save file, by default just the file name (no absolute path)

            public string SaveFilePathBackup { get; set; } // Path and name of the backup save file, by default just the file name (no absolute path)
        }

        [Serializable]
        [DataContract(Name = "Save_File", Namespace = "Loadout_Patcher")]
        public struct SaveFile
        {
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

            [DataMember(Name = "SmartSteamEmu_Path")]
            public string SSEPath { get; set; } // The path to the SSELauncher.exe [ProcessHandling.cs]

            [DataMember(Name = "Loadout_Files_Path")]
            public string LoadoutPath { get; set; } // The path to the Loadout game files [ProcessHandling.cs]

            [DataMember(Name = "Web_API_Endpoints")]
            public List<string> WebApiEndpoints { get; set; } // 1st entry is the main endpoint, switching with alts will be allowed [ProcessMemory.cs]

            [DataMember(Name = "Runtime_Info")]
            public ProcessMemory.RuntimeInfo RuntimeInfo { get; set; } // which OS platform and more [ProcessMemory.cs]

            [DataMember(Name = "GUI_Title")]
            public string GuiTitle { get; set; } // The title of the GUI will be customizable [GUI.cs]

            // contains structs MapSearchPreferences and MapQueuePreferences as well as 
            // SkipStartPage [GUI.cs], StartLoadout [GUI.cs], CreateSSEShortcut [GUI.cs] and StartLoadoutViaSSE [GUI.cs]
            [DataMember(Name = "Map_Related_Preferences")]
            public GUI.UserSetOptions UserSetOptions { get; set; } 

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

            [DataMember(Name = "Primary_Custom_Map")]
            public string PrimaryCustomMap { get; set; } // string [Map.cs]

            [DataMember(Name = "Map_Deny_List")]
            public List<Map.LoadoutMap> MapBlacklist { get; set; } // list of struct [Map.cs]

            [DataMember(Name = "Map_Allow_List")]
            public List<Map.LoadoutMap> MapWhitelist { get; set; } // list of struct [Map.cs]

            [DataMember(Name = "No_Interactions")]
            public bool NoInteractions { get; set; } // boolean [Map.cs]

            [DataMember(Name = "Success_Sounds")]
            public bool SuccessSounds { get; set; } // boolean [Sound.cs]

            [DataMember(Name = "Minigame_Sounds")]
            public bool MinigameSounds { get; set; } // boolean [Sound.cs]

            [DataMember(Name = "Other_Sounds")]
            public bool OtherSounds { get; set; } // boolean [Sound.cs]

            [DataMember(Name = "Menu_Music")]
            public bool MenuMusic { get; set; } // boolean [Sound.cs]

            [DataMember(Name = "Blocked_Song")]
            public string BlockedSong { get; set; } // string [Sound.cs]

            [DataMember(Name = "Best_Minigame_Time")]
            public string BestTimeWithoutSolver { get; set; } // string [Solver.cs]

            [DataMember(Name = "Favorite_Sessions")]
            public List<MultiplayerSession.SessionInfoKeys> FavoriteSessions { get; set; } // list of struct [MultiplayerPageViewModel.cs]
        }

        /// <summary>
        /// Is save file found, return path to it. The main save file has priority over the backup
        /// </summary>
        /// <returns>string of the path to the save file without file extension</returns>
        public static string SaveFileLocator()
        {
            string path = "";

            /* System.AppContext.BaseDirectory is necessary for single file applications */
            if (File.Exists(Path.GetDirectoryName(System.AppContext.BaseDirectory) + DirectorySeparator + "patcherSave.xml"))
            {
                path = Path.GetDirectoryName(System.AppContext.BaseDirectory) + DirectorySeparator + "patcherSave.xml";
            }
            else if (File.Exists(Path.GetDirectoryName(System.AppContext.BaseDirectory) + DirectorySeparator + "patcherSaveBackup.xml"))
            {
                path = Path.GetDirectoryName(System.AppContext.BaseDirectory) + DirectorySeparator + "patcherSaveBackup.xml";
            }
            // path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\patcherSave";

            return path;
        }

        /// <summary>
        /// Static method that sends all save data to the relevant class attributes of several classes.
        /// </summary>
        /// <param name="saveFile"></param>
        public static void LoadSaveFileIntoEverything(Filesave.SaveFile saveFile)
        {
            Id = saveFile.SaveFileInfo.Id;
            Type = saveFile.SaveFileInfo.Type;
            CreationDate = saveFile.SaveFileInfo.CreationDate;
            LastTimeSaved = saveFile.SaveFileInfo.LastTimeSaved;
            SaveFilePath = saveFile.SaveFileInfo.SaveFilePath;
            SaveFilePathBackup = saveFile.SaveFileInfo.SaveFilePathBackup;
            Multiplayer.LoadSaveFileIntoMultiplayerProperties(saveFile);
            ProcessHandling.LoadSaveFileIntoProcessHandlingProperties(saveFile);
            ProcessMemory.LoadSaveFileIntoProcessMemoryProperties(saveFile);
            GUI.LoadSaveFileIntoGuiProperties(saveFile);
            Map.LoadSaveFileIntoMapProperties(saveFile);
            /* We also assign the save file favorite maps list to the FavoriteMaps list in MainProperties from the start */
            MainProperties.FavoriteMaps = saveFile.FavoriteMaps;
            /* We do the same with similar properties */
            MainProperties.MapBlacklist = saveFile.MapBlacklist;
            MainProperties.MapWhitelist = saveFile.MapWhitelist;
            MainProperties.MapQueueList = saveFile.MapQueue;
            if (saveFile.StartingMap.FullMapName != null)
            {
                MainProperties.StartingMap = saveFile.StartingMap;
            }
            Map.SetMapWithInteractions(saveFile.NoInteractions);
            Sound.LoadSaveFileIntoSoundProperties(saveFile);
            Solver.LoadSaveFileIntoSolverProperty(saveFile);
            MultiplayerSession.LoadSaveFileIntoMultiplayerSessionProperty(saveFile);
        }

        /// <summary>
        /// Gathers save data from different classes and saves it into the save file.
        /// It is recommend to not save very often, only synchronize, meaning this method doesn't affect the save files on the device.
        /// </summary>
        /// <param name="saveFile"></param>
        public static void SynchronizeEverythingIntoSaveFile(Filesave.SaveFile saveFile)
        {
            saveFile.SaveFileInfo = new SaveFileInfo
            {
                Id = Id,
                Type = Type!,
                CreationDate = CreationDate!,
                LastTimeSaved = LastTimeSaved!,
                SaveFilePath = SaveFilePath!,
                SaveFilePathBackup = SaveFilePathBackup!
            };
            Multiplayer.SynchronizeSaveFile(ref saveFile);
            ProcessHandling.SynchronizeSaveFile(ref saveFile);
            ProcessMemory.SynchronizeSaveFile(ref saveFile);
            GUI.SynchronizeSaveFile(ref saveFile);
            Map.SynchronizeSaveFile(ref saveFile);
            Sound.SynchronizeSaveFile(ref saveFile);
            Solver.SynchronizeSaveFile(ref saveFile);
            MultiplayerSession.SynchronizeSaveFile(ref saveFile);
        }

        // Similar to SynchronizeEverythingIntoSaveFile while it's used to get a save file.
        // Keep in mind such Method isn't supposed to be used everywhere, 1 time per class at most.

        public static SaveFile SaveFileBuilder()
        {
            SaveFile saveFile = new SaveFile
            {
                SaveFileInfo = new SaveFileInfo
                {
                    Id = Id,
                    Type = Type!,
                    CreationDate = CreationDate!,
                    LastTimeSaved = LastTimeSaved!,
                    SaveFilePath = SaveFilePath!,
                    SaveFilePathBackup = SaveFilePathBackup!
                }
            };
            Multiplayer.SynchronizeSaveFile(ref saveFile);
            ProcessHandling.SynchronizeSaveFile(ref saveFile);
            ProcessMemory.SynchronizeSaveFile(ref saveFile);
            GUI.SynchronizeSaveFile(ref saveFile);
            Map.SynchronizeSaveFile(ref saveFile);
            Sound.SynchronizeSaveFile(ref saveFile);
            Solver.SynchronizeSaveFile(ref saveFile);
            MultiplayerSession.SynchronizeSaveFile(ref saveFile);

            return saveFile;
        }


        /// <summary>
        /// Sets the static SaveFileInfo properties all at once
        /// They are static because the Filesave class itself isn't meant for making instances of it. SaveFile is meant to be used
        /// Keep in mind that actual saving includes LastTimeSaved = DateTime.Now.ToString("dd MMMM yyyy hh:mm:ss tt")
        /// </summary>
        /// <param name="saveFile"></param>
        public static void SetSaveFileInfoContent(SaveFile saveFile)
        {
            // The content in SaveFileInfo can be written outside of the struct
            // The content that is written in SaveFile doesn't belong in this class but making use of the struct would help
            // The best sense is to use the properties of other classes for loading
            Id = saveFile.SaveFileInfo.Id;
            Type = saveFile.SaveFileInfo.Type;
            CreationDate = saveFile.SaveFileInfo.CreationDate;
            LastTimeSaved = saveFile.SaveFileInfo.LastTimeSaved;
            SaveFilePath = saveFile.SaveFileInfo.SaveFilePath;
            SaveFilePathBackup = saveFile.SaveFileInfo.SaveFilePathBackup;
        }


        /// <summary>
        /// Assigns file paths, calls for file writing access and XML serialization
        /// </summary>
        /// <param name="makeBackup"></param>
        /// <param name="path"></param>
        /// <param name="pathBackup"></param>
        /// <returns>the number of saved files</returns>
        public static int SaveDataToFile(SaveFile saveFile, bool makeBackup = true, string path = "", string pathBackup = "")
        {
            int filesSaved = 0;

            if (path == "")
            {
                if (!String.IsNullOrEmpty(saveFile.SaveFileInfo.SaveFilePath))
                {
                    path = saveFile.SaveFileInfo.SaveFilePath;
                }
                else
                {
                    path = DefaultSavePath;
                }
            }

            if (makeBackup && pathBackup == "")
            {
                if (!String.IsNullOrEmpty(saveFile.SaveFileInfo.SaveFilePathBackup))
                {
                    pathBackup = saveFile.SaveFileInfo.SaveFilePathBackup;
                }
                else
                {
                    pathBackup = DefaultSavePathBackup;
                }
            }

            // It will be saved to 3 different paths. The paths will be treated separately
            // 1st path: Set SaveFileInfo.Type to "Main"
            // 2nd path: Set SaveFileInfo.Type to "Backup"
            // 3rd path: Keep "Backup" and once done, set it back to "Main"

            // A backup copy will only be made if the first both writing checks fail
            // It means that the backup copy is the last resource of getting a writing check and serialization done
            bool needBackupCopy = true;

            // If serialization doesn't work once, don't do it again. One error is enough
            // Yet the WritingCheck() method must be called for each file
            bool canSerialize = true;

            string fileName = path;
            int index = path.LastIndexOf(DirectorySeparator);
            if (index >= 0) { fileName = path.Substring(index + 1); }

            // If path works
            if (CheckFilePath(path, DirectorySeparator))
            {
                if (WritingCheck(path))
                {
                    needBackupCopy = false;
                    canSerialize = SerializeXML(saveFile, path, saveFile.SaveFileInfo, "Main");
                    if (canSerialize)
                    {

                        Console.WriteLine("> Success: A file was saved.");
                        Console.WriteLine("> Path: {0}", Path.GetFullPath(fileName));
                        filesSaved++;
                    }
                }
            }

            fileName = pathBackup;
            index = pathBackup.LastIndexOf(DirectorySeparator);
            if (index >= 0) { fileName = pathBackup.Substring(index + 1); }

            // If path backup works
            if (makeBackup && CheckFilePath(pathBackup, DirectorySeparator))
            {
                if (WritingCheck(pathBackup) && canSerialize)
                {
                    needBackupCopy = false;
                    canSerialize = SerializeXML(saveFile, pathBackup, saveFile.SaveFileInfo, "Backup");
                    if (canSerialize)
                    {
                        Console.WriteLine("> Success: A backup file was saved.\n");
                        Console.WriteLine("> Path: {0}", Path.GetFullPath(fileName));
                        filesSaved++;
                    }
                }
            }

            fileName = DefaultSavePathBackupCopy;
            index = DefaultSavePathBackupCopy.LastIndexOf(DirectorySeparator);
            if (index >= 0) { fileName = DefaultSavePathBackupCopy.Substring(index + 1); }

            // If path backup copy works and a backup copy is needed
            if (needBackupCopy && CheckFilePath(DefaultSavePathBackupCopy, DirectorySeparator))
            {
                if (WritingCheck(DefaultSavePathBackupCopy) && canSerialize)
                {
                    if (SerializeXML(saveFile, path, saveFile.SaveFileInfo, "Backup"))
                    {
                        Console.WriteLine("> Success: A special backup file was saved after saving didn't work.");
                        Console.WriteLine("> Path: {0}", Path.GetFullPath(fileName));
                        filesSaved++;
                    }
                    //saveFile.SaveFileInfo.Type = "Main";
                }
            }

            return filesSaved;
        }

        /// <summary>
        /// Uses save file content to generate XML content
        /// Saves the content to a file
        /// Only use this if the file allows writing
        /// </summary>
        /// <param name="path"></param>
        /// <param name="saveFileType"></param>
        /// <returns>true or false</returns>
        private static bool SerializeXML(SaveFile dataContract, string path, SaveFileInfo saveFileInfo, string saveFileType)
        {
            bool canSerialize = true;
            // We write the type of the save file into SaveFileInfo
            // We also use the current date and time for the LastTimeSaved attribute
            dataContract.SaveFileInfo = new SaveFileInfo
            {
                Id = saveFileInfo.Id,
                Type = saveFileType,
                CreationDate = ReadFileCreationDateDirectly(path), // saveFileInfo.CreationDate
                LastTimeSaved = DateTime.Now.ToString("dd MMMM yyyy hh:mm:ss tt"),
                SaveFilePath = saveFileInfo.SaveFilePath,
                SaveFilePathBackup = saveFileInfo.SaveFilePathBackup
            };
            //FileStream stream = new FileStream(path, FileMode.Create);
            try
            {
                //FileStream stream = new FileStream(path, FileMode.Create);
                DataContractSerializer objectWriter = new DataContractSerializer(dataContract.GetType());

                XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
                using (XmlWriter stream = XmlWriter.Create(path, settings))
                {
                    objectWriter.WriteObject(stream, dataContract);
                }

                //objectWriter.WriteObject(stream, dataContract);
            }
            catch (SerializationException serializationEx)
            {
                Console.WriteLine("\n> Error: Unable to generate XML content for the save file.");
                Console.WriteLine("> Press a key to receive the full information of the error.");
                Console.ReadKey(true);
                Console.WriteLine("> Stack trace: {0}", serializationEx.StackTrace);
                Console.WriteLine("> Source: {0}", serializationEx.Source);
                Console.WriteLine("> Data: {0}", serializationEx.Data);
                Console.WriteLine("> Message: {0}", serializationEx.Message);
                Console.WriteLine("> Press a key to continue.");
                Console.ReadKey(true);
                // Keep the patcher running
                canSerialize = false;
            }
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
            }
            //finally
            //{
            //    //stream.Dispose();
            //    //stream.Close();
            //}

            return canSerialize;
        }

        /// <summary>
        /// Checks if writing to a file can be done
        /// Only use this if the file path was checked with success
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true or false</returns>
        private static bool WritingCheck(string path)
        {
            bool writingWorks = true;

            FileStream streamCheck = File.Open(path, FileMode.OpenOrCreate);
            if (!streamCheck.CanWrite)
            {
                writingWorks = false;
            }
            streamCheck.Dispose();
            streamCheck.Close();

            return writingWorks;
        }

        /// <summary>
        /// Checks if reading from a file can be done
        /// Only use this if the file path was checked with success
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true or false</returns>
        private static bool ReadingCheck(string path)
        {
            bool readingWorks = true;

            FileStream streamCheck = File.Open(path, FileMode.OpenOrCreate);
            if (!streamCheck.CanRead)
            {
                readingWorks = false;
            }
            streamCheck.Dispose();
            streamCheck.Close();

            return readingWorks;
        }

        /// <summary>
        /// Loads save file content from an XML file
        /// Calls a method to use the loadout maps related part of the loaded save file immediately
        /// </summary>
        /// <param name="saveFile">passed by reference</param>
        /// <param name="path"></param>
        /// <param name="loadingFromBackup"></param>
        /// <returns>empty or loaded save file</returns>
        public static bool LoadSaveDataFromFile(ref SaveFile saveFile, string path, bool loadingFromBackup = false)
        {
            bool noFileDeserialized = true;

            string fileName = path;
            int index = path.LastIndexOf(DirectorySeparator);
            if (index >= 0) { fileName = path.Substring(index + 1); }

            SaveFile? receivedOutput = new SaveFile();

            // TODO: CheckFilePath fails on Linux
            // If path works and allows reading, deserialize the file
            if (CheckFilePath(path, DirectorySeparator) && ReadingCheck(path) && !loadingFromBackup)
            {
                receivedOutput = DeserializeXML(saveFile.GetType(), path);
                if (receivedOutput != null)
                {
                    Console.WriteLine("> Success: The save file was loaded.");
                    Console.WriteLine("> Path: {0}\n", Path.GetFullPath(fileName));
                    noFileDeserialized = false;
                }
            }

            fileName = DefaultSavePathBackup;
            index = DefaultSavePathBackup.LastIndexOf(DirectorySeparator);
            if (index >= 0) { fileName = DefaultSavePathBackup.Substring(index + 1); }

            // Attempt reading from backup
            if (noFileDeserialized && CheckFilePath(DefaultSavePathBackup, DirectorySeparator) && ReadingCheck(DefaultSavePathBackup))
            {
                receivedOutput = DeserializeXML(saveFile.GetType(), path);
                if (receivedOutput != null)
                {
                    Console.WriteLine("> Success: The save file was loaded.");
                    Console.WriteLine("> Path: {0}\n", Path.GetFullPath(fileName));
                    noFileDeserialized = false;
                }
            }

            fileName = DefaultSavePathBackupCopy;
            index = DefaultSavePathBackupCopy.LastIndexOf(DirectorySeparator);
            if (index >= 0) { fileName = DefaultSavePathBackupCopy.Substring(index + 1); }

            // Attempt reading from backup copy
            if (noFileDeserialized && CheckFilePath(DefaultSavePathBackupCopy, DirectorySeparator) && ReadingCheck(DefaultSavePathBackupCopy))
            {
                receivedOutput = DeserializeXML(saveFile.GetType(), path);
                if (receivedOutput != null)
                {
                    Console.WriteLine("> Success: The save file was loaded.");
                    Console.WriteLine("> Path: {0}\n", Path.GetFullPath(fileName));
                    noFileDeserialized = false;
                }
            }

            if (receivedOutput == null)
            {
                Console.WriteLine("> Error: No save file content was loaded.\n");

                return false;
            }
            else
            {
                // This method is required in case of a save file change
                // Calling a method in map to immediately use the save file information
                // Resets the hashtable entries and the list of loadout maps
                // This must be called before assigning the received output to the save file
                // The success case and the initial save file are both required!
                // That means it mustn't be called earlier or later
                Map.ResetLoadoutMapsAndHashtableEntries(saveFile);

                saveFile = (SaveFile)receivedOutput;
                SetSaveFileInfoContent(saveFile);

                Map.LoadSaveFileIntoMapProperties(saveFile);



                return true;
            }

            //// The save file can still be empty which can easily be checked outside
            //return loadedSaveFile;
        }

        private static SaveFile? DeserializeXML(Type type, string path)
        {
            object? readObject = null;
            SaveFile? saveFile = new SaveFile();

            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    DataContractSerializer deserializer = new DataContractSerializer(type);

                    XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());

                    readObject = deserializer.ReadObject(reader, true);

                    if (readObject != null)
                    {
                        saveFile = (SaveFile)readObject;
                    }
                    else
                    {
                        saveFile = null;
                    }
                }
            }
            catch (SerializationException serializationEx)
            {
                Console.WriteLine("\n> Error: Unable to read the requested XML content from the save file.");
                Console.WriteLine("\n> Maybe the format of the save file doesn't match.");
                Console.WriteLine("> Press a key to receive the full information of the error.");
                Console.ReadKey(true);
                Console.WriteLine("> Stack trace: {0}", serializationEx.StackTrace);
                Console.WriteLine("> Source: {0}", serializationEx.Source);
                Console.WriteLine("> Data: {0}", serializationEx.Data);
                Console.WriteLine("> Message: {0}", serializationEx.Message);
                Console.WriteLine("> Press a key to continue.");
                Console.ReadKey(true);
                // Keep the patcher running
                saveFile = null;
            }
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
                saveFile = null;
            }

            return saveFile;
        }

        /// <summary>
        /// Saves and overwrites particular data located in the save file
        /// This is the legacy method without serialization
        /// </summary>
        /// <param name="replacementLine"></param>
        /// <param name="directorySeparator"></param>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="replacementIndex"></param>
        /// <returns>true or false</returns>
        public static bool SaveDataIntoFile(string replacementLine, char directorySeparator, string path1, string path2, int replacementIndex = 0)
        {
            bool temporaryFile = false;
            Console.WriteLine();
            if (!CheckFilePath(path1, directorySeparator))
            {
                return false;
            }
            if (!CheckFilePath(path2, directorySeparator))
            {
                Console.WriteLine("> Main save file -> no issue yet | Temporary file -> not found/created!\n");
            }
            else
            {
                temporaryFile = true;
            }

            /* The save file contains up to 6 lines and each line serves a specific role. This is the best approach as long as 10 lines won't be surpassed to keep it manageable */
            string?[] receivedLines = new string[6] { "", "", "", "", "", "" };
            if (!temporaryFile) { path2 = path1; }
            FileStream streamCheck = File.Open(path2, FileMode.OpenOrCreate);
            if (streamCheck.CanRead && streamCheck.CanWrite)
            {
                streamCheck.Dispose();
                streamCheck.Close();
                StreamReader sr = new (path2);
                for (int i = 0; i < 6; i++)
                {                
                    if (sr.Peek() >= 0) { receivedLines[i] = sr.ReadLine(); }
                    /* Reserved index: 0 = Api, 1 = Starting map, 2 = Custom map, 3 = Favorite map - save slot [1], 4 = Favorite map - save slot [2], 5 = Favorite map - save slot [3] */
                    if (i == replacementIndex) { receivedLines[i] = replacementLine; }
                    //if (i < 5) { receivedLines[i] += '\n'; }
                }
                sr.Dispose();
                sr.Close();
                /* We write into a temp file to keep the following lines to copy them */
                StreamWriter sw = new (path2);
                foreach (string? line in receivedLines)
                {
                    sw.WriteLine(line);
                }
                sw.Dispose();
                sw.Close();

                try
                {
                    File.Delete(path1); // Delete the save file
                    if (temporaryFile) { File.Move(path2, path1); } // Make a new save file
                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    FileAttributes attributes = new FileInfo(path1).Attributes;
                    Console.Write("Unable to remove and replace the save file!\n");
                    if ((attributes & FileAttributes.ReadOnly) > 0)
                    {
                        Console.Write("The save file is read-only.\n");
                    }
                    else if ((attributes & FileAttributes.Normal) == 0)
                    {
                        Console.Write("The save file is not normal! Check for its file attributes.\n");
                    }                        
                }
            }
            return false;
        }


        /// <summary>
        /// Checks for a file path and tries to create it if it doesn't exist
        /// </summary>
        /// <param name="replacementLine"></param>
        /// <param name="directorySeparator"></param>
        /// <returns>true or false</returns>
        public static bool CheckFilePath(string path, char directorySeparator)
        {
            string fileName = path;
            int index = path.LastIndexOf(directorySeparator);
            if (index >= 0) { fileName = path.Substring(index + 1); }
            bool streamCheck = File.Exists(path);
            FileStream stream;
            if (!streamCheck)
            {
                try
                {
                    Console.WriteLine("> The file {0} wasn't found. Creating it ...\n", fileName);
                    stream = File.Create(path);
                    Console.WriteLine("> File ({0}) has been created.\n", fileName);
                    stream.Dispose();
                    stream.Close();
                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("> File {0} could not be created!", fileName);
                    Console.WriteLine("> Missing access to create the file!\n");
                }
                catch (ArgumentException) // It also catches ArgumentNullException
                {
                    // This exception can easily be triggered on Linux due to the different directory separator
                    Console.WriteLine("> File {0} could not be created!", fileName);
                    Console.WriteLine("> The path to the file or file name might be incorrect or empty!\n");
                }
                catch (PathTooLongException)
                {
                    Console.WriteLine("> File {0} could not be created!", fileName);
                    Console.WriteLine("> The entire path or file name is too long!\n");
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine("> File {0} could not be created!", fileName);
                    Console.WriteLine("> The directory wasn't found!\n");
                }
                catch (IOException)
                {
                    Console.WriteLine("> File {0} could not be created!", fileName);
                    Console.WriteLine("> Perhaps the file is already in use by a different operation!\n");
                }
                catch (NotSupportedException)
                {
                    Console.WriteLine("> File {0} could not be created!", fileName);
                    Console.WriteLine("> Missing functionality/support to create a file!\n");
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        public static string ReadFileCreationDateDirectly(string path)
        {
            string fileCreationDate = "";

            try
            {
                fileCreationDate = File.GetCreationTime(path).ToString("dd MMMM yyyy hh:mm:ss tt");
            }
            catch (Exception ex)
            {
                string fileName = path;
                int index = path.LastIndexOf(DirectorySeparator);
                if (index >= 0 && path.Length > 0 && path.Length < 256) { fileName = path.Substring(index + 1); }
                Console.WriteLine("\n> Error: Could not read the creation date of file {0}!", fileName);
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
            }
            return fileCreationDate;
        }

        #region GetSaveFileMethods
        /// <summary>
        /// Generates a new save file
        /// </summary>
        /// <returns>the default save file</returns>
        public static SaveFile GetNewSaveFileTemplate()
        {
            SaveFile newSaveFile = new SaveFile
            {
                SaveFileInfo = new SaveFileInfo()
                {
                    Id = 1,
                    Type = "",
                    CreationDate = DateTime.Now.ToString("dd MMMM yyyy hh:mm:ss tt"),
                    LastTimeSaved = DateTime.Now.ToString("dd MMMM yyyy hh:mm:ss tt"),
                    SaveFilePath = DefaultSavePath,
                    SaveFilePathBackup = DefaultSavePathBackup
                }
            };
            Multiplayer.Username = "";
            newSaveFile.Username = Multiplayer.Username;
            newSaveFile.PlayerId = Multiplayer.PlayerId;
            newSaveFile.IpAddress = Multiplayer.IpAddress ?? "";
            newSaveFile.ParentProcess = ProcessHandling.LoadoutParentProcessName ?? "";
            newSaveFile.SSEPath = ProcessHandling.SSEPath ?? "";
            newSaveFile.LoadoutPath = ProcessHandling.LoadoutPath ?? "";
            newSaveFile.WebApiEndpoints = new List<string>()
            {
                "api.loadout.rip"
            };
            new ProcessMemory();
            newSaveFile.RuntimeInfo = new ProcessMemory.RuntimeInfo()
            {
                NetInstallation = ProcessMemory.GetNetInstallation(),
                OsDescription = ProcessMemory.GetOsDescription(),
                RuntimeId = ProcessMemory.GetRuntimeId(),
                ProcessorArchitecture = ProcessMemory.GetProcessorArchitecture(),
                OsPlatform = ProcessMemory.GetOsPlatform()
            };
            newSaveFile.GuiTitle = GUI.Title;
            newSaveFile.UserSetOptions = new GUI.UserSetOptions()
            {
                MapSearchPreferences = new GUI.MapSearchPreferences
                {
                    BlacklistPreference = true,
                    WhitelistPreference = GUI.WhitelistPreference,
                    ExcludePve = GUI.ExcludePve,
                    ShowPveOnly = GUI.ShowPveOnly
                },

                MapQueuePreferences = new GUI.MapQueuePreferences
                {
                    FillEmptyMapQueueWithFavoriteMaps = GUI.FillEmptyMapQueueWithFavoriteMaps,
                    LoopMapQueue = GUI.LoopMapQueue
                },
                SkipStartPage = false,
                InstantPatching = true,
                StartLoadout = true,
                StartLoadoutViaSSE = true,
                CreateSSEShortcut = true
            };
            newSaveFile.StartingMap = new Map.LoadoutMap()
            { };
            newSaveFile.MapQueue = new List<Map.LoadoutMap>()
            { };
            newSaveFile.FavoriteMaps = new List<Map.LoadoutMap>()
            { };
            newSaveFile.CustomMaps = new List<Map.LoadoutMap>()
            { };
            newSaveFile.AdditionalMapHashtableEntries = new List<(Map.HashtablesEnum, KeyValuePair<string, string>)>()
            { };
            newSaveFile.MapBlacklist = new List<Map.LoadoutMap>()
            {
                new Map.LoadoutMap
                {
                    Id = "1507501501",
                    FullMapName = "brewery_art_master",
                    FullMapNameAlt = "Brewery_Art_Master",
                    BaseMap = "level_three",
                    DayNight = "day",
                    GameMode = "art_master",
                    PicturePath = "/Assets/Maps/brewery_art_master.webp"
                },
            };
            newSaveFile.MapWhitelist = new List<Map.LoadoutMap>()
            { };
            newSaveFile.NoInteractions = false;
            newSaveFile.BlockedSong = "None";
            newSaveFile.SuccessSounds = true;
            newSaveFile.MinigameSounds = true;
            newSaveFile.FavoriteSessions = new List<MultiplayerSession.SessionInfoKeys>()
            { };

            return newSaveFile;
        }

        /// <summary>
        /// Generates a save file filled with content
        /// Use this method for testing purposes
        /// A long DateTime string should always be used!
        /// </summary>
        /// <returns></returns>
        public static SaveFile GetSaveFileSample()
        {
            SaveFile saveFileSample = new SaveFile
            {
                SaveFileInfo = new SaveFileInfo()
                {
                    Id = 1,
                    Type = "",
                    CreationDate = ReadFileCreationDateDirectly(DefaultSavePath), // It can be DefaultSavePathBackup as well
                    LastTimeSaved = DateTime.Now.ToLongDateString(),
                    SaveFilePath = DefaultSavePath,
                    SaveFilePathBackup = DefaultSavePathBackup
                }
            };
            Multiplayer.Username = "Axl";
            saveFileSample.Username = Multiplayer.Username;
            Multiplayer.AutoGeneratePlayerId();
            saveFileSample.PlayerId = Multiplayer.PlayerId;
            Multiplayer.FindOutAndSetIpAddress();
            saveFileSample.IpAddress = Multiplayer.IpAddress ?? "";
            saveFileSample.ParentProcess = ProcessHandling.LoadoutParentProcessName ?? "";
            saveFileSample.SSEPath = ProcessHandling.SSEPath ?? "";
            saveFileSample.LoadoutPath = ProcessHandling.LoadoutPath ?? "";
            saveFileSample.WebApiEndpoints = new List<string>()
            {
                "api.loadout.rip"
            };
            new ProcessMemory();
            saveFileSample.RuntimeInfo = new ProcessMemory.RuntimeInfo()
            {
                NetInstallation = ProcessMemory.GetNetInstallation(),
                OsDescription = ProcessMemory.GetOsDescription(),
                RuntimeId = ProcessMemory.GetRuntimeId(),
                ProcessorArchitecture = ProcessMemory.GetProcessorArchitecture(),
                OsPlatform = ProcessMemory.GetOsPlatform()
            };
            saveFileSample.GuiTitle = GUI.Title;
            saveFileSample.UserSetOptions = new GUI.UserSetOptions()
            {
                MapSearchPreferences = new GUI.MapSearchPreferences
                {
                    BlacklistPreference = GUI.BlacklistPreference,
                    WhitelistPreference = GUI.WhitelistPreference,
                    ExcludePve = GUI.ExcludePve,
                    ShowPveOnly = GUI.ShowPveOnly
                },

                MapQueuePreferences = new GUI.MapQueuePreferences
                {
                    FillEmptyMapQueueWithFavoriteMaps = GUI.FillEmptyMapQueueWithFavoriteMaps,
                    LoopMapQueue = GUI.LoopMapQueue
                },
                SkipStartPage = GUI.SkipStartPage,
                InstantPatching = GUI.InstantPatching,
                StartLoadout = GUI.StartLoadout,
                StartLoadoutViaSSE = GUI.StartLoadoutViaSSE,
                CreateSSEShortcut = GUI.CreateSSEShortcut
            };
            saveFileSample.StartingMap = new Map.LoadoutMap()
            {
                Id = "1509501516",
                FullMapName = "projectx_tc",
                FullMapNameAlt = "ProjectX_TC",
                BaseMap = "projectx_tc",
                DayNight = "day",
                GameMode = "tc",
                PicturePath = "/Assets/Maps/projectx_tc.webp"
            };
            saveFileSample.MapQueue = new List<Map.LoadoutMap>()
            {
                new Map.LoadoutMap
                {
                    Id = "1505501511",
                    FullMapName = "gliese_581_mu",
                    FullMapNameAlt = "Gliese_581_MU",
                    BaseMap = "gliese_581",
                    DayNight = "day",
                    GameMode = "mu",
                    PicturePath = "/Assets/Maps/gliese_581_mu.webp"
                },
            };
            saveFileSample.FavoriteMaps = new List<Map.LoadoutMap>()
            {
                new Map.LoadoutMap
                {
                    Id = "1512501505",
                    FullMapName = "spires_ctf",
                    FullMapNameAlt = "Spires_CTF",
                    BaseMap = "spires",
                    DayNight = "day",
                    GameMode = "ctf",
                    PicturePath = "/Assets/Maps/spires_ctf.webp"
                }
            };
            saveFileSample.CustomMaps = new List<Map.LoadoutMap>()
            {
                new Map.LoadoutMap
                {
                    Id = "1520501515",
                    FullMapName = "two_port_solo",
                    FullMapNameAlt = "Two_Port_Solo",
                    BaseMap = "two_port",
                    DayNight = "day",
                    GameMode = "solo",
                    PicturePath = ""
                }
            };
            saveFileSample.AdditionalMapHashtableEntries = new List<(Map.HashtablesEnum, KeyValuePair<string, string>)>()
            {
                (Map.HashtablesEnum.FullMapsAndAliases, new KeyValuePair<string, string>(
                    "two_port_solo", "Two Ports (Alpha)\nGame mode: Solo"))
            };
            saveFileSample.MapBlacklist = new List<Map.LoadoutMap>()
            {
                new Map.LoadoutMap
                {
                    Id = "1512501505",
                    FullMapName = "spires_ctf",
                    FullMapNameAlt = "Spires_CTF",
                    BaseMap = "spires",
                    DayNight = "day",
                    GameMode = "ctf",
                    PicturePath = "/Assets/Maps/spires_ctf.webp"
                }
            };
            saveFileSample.MapWhitelist = new List<Map.LoadoutMap>()
            {
                new Map.LoadoutMap
                {
                    Id = "1512501505",
                    FullMapName = "spires_ctf",
                    FullMapNameAlt = "Spires_CTF",
                    BaseMap = "spires",
                    DayNight = "day",
                    GameMode = "ctf",
                    PicturePath = "/Assets/Maps/spires_ctf.webp"
                }
            };
            saveFileSample.NoInteractions = false;
            saveFileSample.BlockedSong = "0xf3614c6a.ogg";
            saveFileSample.SuccessSounds = true;
            saveFileSample.MinigameSounds = true;
            saveFileSample.FavoriteSessions = new List<MultiplayerSession.SessionInfoKeys>()
            {
                new MultiplayerSession.SessionInfoKeys
                {
                    ServerIPAddress = "123.123.123.124",
                    ServerName = "Hardcore Loadout Reloaded"
                }
            };
            // TODO: Update with picture paths
            /*
            saveFileSample.MapBlacklist = new Hashtable
            {
                // Bans that make the most sense
                {"thefreezer_kc_bots", "The Freezer\nGame mode: Special variant of Deathsnatch"},
                {"cpr_bots", "Special variant of Blitz"},
                {"kc_bots", "Special variant of Deathsnatch"},
                {"three", "Unknown"}
            };
            saveFileSample.MapWhitelist = new Hashtable
            {
                // PvE only
                {"botwave", "Hold Your Pole"},
                {"botwaves", "Hold Your Pole"},
                {"projectx_tc", "Project X"}
            };
            */
            return saveFileSample;
        }

        // For reference:
        /**
        ProcessMemory processMemory = new ProcessMemory();
        SaveFile newSafeFile = new SaveFile
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
                NetInstallation = processMemory.GetNetInstallation(),
                OsDescription = processMemory.GetOsDescription(),
                RuntimeId = processMemory.GetRuntimeId(),
                ProcessorArchitecture = processMemory.GetProcessorArchitecture(),
                OsPlatform = processMemory.GetOsPlatform()
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
            AdditionalMapHashtableEntries = new List<(Map.HashtablesEnum, KeyValuePair<string, string>)>()
            {
                (Map.HashtablesEnum.FullMapsAndAliases, new KeyValuePair<string, string>(
                    "two_port_solo", "Two Ports (Alpha)\nGame mode: Solo"))
            },
            MapBlacklist = new Hashtable
            { },
            MapWhitelist = new Hashtable
            { }
            **/
            #endregion
        }
    }


