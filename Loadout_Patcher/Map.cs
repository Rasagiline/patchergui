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
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Loadout_Patcher
{
    /// <summary>
    /// Map is about everything regarding the maps of the game
    /// This is what makes this application interesting
    /// </summary>
    public class Map
    {
        private string id;

        private string fullMapName;

        private string fullMapNameAlt;

        private string baseMap;

        private string dayNight;

        private string gameMode;

        private string picturePath;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string FullMapName
        {
            get { return fullMapName; }
            set { fullMapName = value; }
        }

        public string FullMapNameAlt
        {
            get { return fullMapNameAlt; }
            set { fullMapNameAlt = value; }
        }

        public string BaseMap
        {
            get { return baseMap; }
            set { baseMap = value; }
        }

        public string DayNight
        {
            get { return dayNight; }
            set { dayNight = value; }
        }

        public string GameMode
        {
            get { return gameMode; }
            set { gameMode = value; }
        }

        public string PicturePath
        {
            get { return picturePath; }
            set { picturePath = value; }
        }

        // The case sensitive issue of fullMapName:
        // What if the user inserts a customMap that could match fullMapName but doesn't because a few letters are capitalized?
        // Solution: Let the user know that a fullMapName is written in lower case
        // TODO:
        // Format the fullMapName string to lower case but ignore fullMapNameAlt that is supposed to be it (defaultMap)

        // The default map or the starting map is causing this at the start
        // TODO:
        // The user needs to know if the currentMap is either fullMapName or fullMapNameAlt
        // "We recommend to switch to fullMapName, so you can switch between 2 weapons and pick up game mode relevant objectives"
        // GetMap() can't be GetCurrentMap() because a check with readMemoryMapString is missing
        // GetCurrentMap() would have to use fullMapName and fullMapNameAlt and compare with the readMemoryMapString

        private static string primaryCustomMap = "";

        public static string PrimaryCustomMap
        {
            get { return primaryCustomMap; }
            set { primaryCustomMap = value; }
        }

        /// <summary>
        /// Fills the new map object's properties with values
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fullMapName"></param>
        /// <param name="fullMapNameAlt"></param>
        /// <param name="baseMap"></param>
        /// <param name="dayNight"></param>
        /// <param name="gameMode"></param>
        public Map(string id, string fullMapName, string fullMapNameAlt, string baseMap, string dayNight, string gameMode, string picturePath = "")
        {
            this.id = id;
            this.fullMapName = fullMapName;
            this.fullMapNameAlt = fullMapNameAlt;
            this.baseMap = baseMap;
            this.dayNight = dayNight;
            this.gameMode = gameMode;
            this.picturePath = picturePath;
        }

        /// <summary>
        /// Loads save file information into map properties such as the list of favorite maps
        /// </summary>
        /// <param name="saveFile"></param>
        public static void LoadSaveFileIntoMapProperties(Filesave.SaveFile saveFile)
        {
            // TODO:
            // The user must be asked what they want to keep during a save file change.
            // Perhaps they want to keep the map queue
            // From here a function in GUI can be called, asking the user, and their response will be handled here
            // "You are about to load the map queue from a different save file. Do you want to change the current map queue?"

            // A check if content is there. Nothing can be set if there is nothing
            // Use the null check for other properties
            if (saveFile.FavoriteMaps != null && saveFile.FavoriteMaps.Count > 0)
            {
                SetFavoriteMaps(saveFile.FavoriteMaps);
            }
            if (saveFile.MapQueue != null && saveFile.MapQueue.Count > 0)
            {
                SetMapQueue(new Stack<LoadoutMap>(saveFile.MapQueue));
            }
            if (saveFile.StartingMap.Id != "")
            {
                SetStartingMap(saveFile.StartingMap); // TODO: Test that
            }
            if (saveFile.MapBlacklist != null && saveFile.MapBlacklist.Count > 0)
            {
                SetMapBlacklist(saveFile.MapBlacklist);
            }
            if (saveFile.MapWhitelist != null && saveFile.MapWhitelist.Count > 0)
            {
                SetMapWhitelist(saveFile.MapWhitelist);
            }
            // Before: Not to forget the case of a save file change (includes list of loadout map reset via id hashset) and also a single custom map removal
            // The method that must be called before (call happens in Filesave.cs) is ResetLoadoutMapsAndHashtableEntries();
            if (saveFile.CustomMaps != null && saveFile.CustomMaps.Count > 0)
            {
                foreach (LoadoutMap customMap in saveFile.CustomMaps)
                {
                    // Takes care of list of loadout maps as well as the hashtables of this class
                    SetNewCustomMap(customMap, saveFile.AdditionalMapHashtableEntries);
                }
            }
            SetMapWithInteractions(saveFile.NoInteractions);
            Map.PrimaryCustomMap = saveFile.PrimaryCustomMap;
        }

        /// <summary>
        /// Loads all static map data into the save file
        /// This method makes it comfortable to set all things at once
        /// </summary>
        /// <param name="saveFile"></param>
        public static void SynchronizeSaveFile(ref Filesave.SaveFile saveFile)
        {
            // A check if content is there. Nothing can be set if there is nothing
            if (GetFavoriteMaps().Count > 0)
            {
                saveFile.FavoriteMaps = GetFavoriteMaps();
            }
            if (GetMapQueueListCopy().Count > 0)
            {
                saveFile.MapQueue = GetMapQueueListCopy();
            }
            if (GetStartingMap().Id != "")
            {
                saveFile.StartingMap = GetStartingMap(); // TODO: Test that
            }
            if (GetMapBlacklist().Count > 0)
            {
                saveFile.MapBlacklist = GetMapBlacklist();
            }
            if (GetMapWhitelist().Count > 0)
            {
                saveFile.MapWhitelist = GetMapWhitelist();
            }
            saveFile.NoInteractions = GetMapWithoutInteractions();
            saveFile.PrimaryCustomMap = PrimaryCustomMap;

            // Custom maps and their hashtables are already being handled by corresponding methods
        }

        // TODO:
        // At the start, the list of favoriteMaps and the adding of customMaps need to be constructed at the start
        // A queue of maps can contain multiple objects
        // Solution: Make a static constructor for the lists. It will work only once
        // What if the user comes with a different save file with other lists and
        // a different starting map while they wish to keep the current map?
        // The types: startingMap, customMap, favoriteMap, currentMap, [mapQueue -> nextMap]
        // Add and Remove single entries, replace lists, first out last in (mapQueue) unless the user makes changes
        // TODO:
        // A method to quickly let the favoriteMaps be the mapQueue

        // This is the field for the starting map
        private static LoadoutMap startingMap;

        public static LoadoutMap GetStartingMap()
        {
            return startingMap;
        }

        public static void SetStartingMap(LoadoutMap map)
        {
            startingMap = map;
        }

        // This is the field for the list of favorite maps
        private static List<LoadoutMap> favoriteMaps = new List<LoadoutMap>();

        public static List<LoadoutMap> GetFavoriteMaps()
        {
            return favoriteMaps;
        }

        public static void SetFavoriteMaps(List<LoadoutMap> mapList)
        {
            favoriteMaps = mapList;
        }

        // This is the field for the map queue
        private static Stack<LoadoutMap> mapQueue = new Stack<LoadoutMap>();

        public static Stack<LoadoutMap> GetMapQueue()
        {
            return mapQueue;
        }

        public static void SetMapQueue(Stack<LoadoutMap> mapList)
        {
            mapQueue = mapList;
            mapQueueListCopy = new List<LoadoutMap>(mapList);
        }

        // It's recommended to use this for the save file. Stack<> won't work using XmlSerializer but works using DataContractSerializer
        // Stack<> does not contain a definition for Add, so this appears to be useful
        private static List<LoadoutMap> mapQueueListCopy = new List<LoadoutMap>();

        public static List<LoadoutMap> GetMapQueueListCopy()
        {
            return mapQueueListCopy;
        }

        /// <summary>
        /// Overwrites the map queue with the list of favorite maps
        /// </summary>
        public static void SetFavoriteMapsAsMapQueue()
        {
            SetMapQueue(new Stack<LoadoutMap>(GetFavoriteMaps()));
        }

        // This is the field for the blacklist of maps or search criteria
        private static List<LoadoutMap> mapBlacklist = new List<LoadoutMap>();

        public static List<LoadoutMap> GetMapBlacklist()
        {
            return mapBlacklist;
        }

        public static void SetMapBlacklist(List<LoadoutMap> blacklistMapList)
        {
            mapBlacklist = blacklistMapList;
        }

        // This is the field for the whitelist of maps or search criteria
        private static List<LoadoutMap> mapWhitelist = new List<LoadoutMap>();

        public static List<LoadoutMap> GetMapWhitelist()
        {
            return mapWhitelist;
        }

        public static void SetMapWhitelist(List<LoadoutMap> whitelistMapList)
        {
            mapWhitelist = whitelistMapList;
        }

        // TODO:
        // Store a list of favorite maps
        // This list can contain standard and custom maps
        // How to find out if a favorite map is a custom map?
        // When removing a custom map, it needs to be removed from starting map, favorite maps, map blacklist and whitelist, map queue

        public static HashSet<string> GetStandardMapIds()
        {
            return standardMapIds;
        }


        /* Game modes */
        public static readonly string[] suffixAliases = new string[18] { "Unknown","Extraction","Annihilation","Special variant of Deathsnatch","Deathsnatch","Jackhammer","Special variant of Blitz","Blitz",
                                                   "Domination","Unknown","Hold Your Pole","Hold Your Pole","Extraction (Alpha) - test stage","Annihilation (Alpha) - test stage",
                                                   "Deathsnatch (Alpha) - test stage","Solo","Unknown","None" };
        /* The empty string must be the last element and territorycontrol the first one. The order of the array elements should correspond with the order of their aliases */
        public static readonly string[] suffixes = new string[18] { "territorycontrol","rr","mu","kc_bots","kc","ctf","cpr_bots","cpr",
                                                   "domination","tc","botwave","botwaves","ctp","mashup",
                                                   "tdm","solo","pj","" };

        // Game modes such as cpr_bots will not be excluded by default.
        // Base maps such as three will not be excluded by default.
        // The whitelist or blacklist take care of that.

        // Game mode definitions
        private readonly static Hashtable mapSuffixesAndDefinitions = new Hashtable()
        {
            {"art_master", "No definition"},
            {"botwave", "Defend the control point from alien NPCs called kroads"},
            {"botwaves", "Defend the control point from alien NPCs called kroads"},
            {"cpr", "Capture control points that spawn randomly"},
            {"cpr_bots", "Capture control points that spawn randomly"},
            {"ctf", "Steal the enemy's hammer, smash around and capture it"},
            {"ctp", "This is a test stage for collecting a blutonium stone"},
            {"domination", "Capture 3 different control points and defend all of them"},
            {"geo", "No definition"},
            {"kc", "Kill opponents to collect their capsules being dropped"},
            {"kc_bots", "Kill opponents to collect their capsules being dropped"},
            {"mashup", "This is a test stage for a competitive game mode with many things to do"},
            {"mu", "Take care of as many objective as you can and buy upgrades. Originally ranked"},
            {"none", "No definition"},
            {"pj", "Shoot slowly flying balls that come in groups to train your aim"},
            {"rr", "As the collector, grab blutonium stones and collect them in machines"},
            {"solo", "Test your custom loadout on staying and running NPCs"},
            {"tc", "Shoot aggressive turrets that respawn"},
            {"tdm", "No definition"},
            {"territorycontrol", "No definition"}
        };

        public static Hashtable GetMapSuffixesAndDefinitions()
        {
            return mapSuffixesAndDefinitions;
        }

        // ---------------------------------------------------------

        private readonly static Hashtable mapSuffixesAndAliases = new Hashtable()
        {
            {"art_master", "None"},
            {"botwave", "Hold Your Pole"},
            {"botwaves", "Hold Your Pole"},
            {"cpr", "Blitz"},
            {"cpr_bots", "Special variant of Blitz"},
            {"ctf", "Jackhammer"},
            {"ctp", "Extraction (Alpha) - test stage"},
            {"domination", "Domination"},
            {"geo", "None"},
            {"kc", "Deathsnatch"},
            {"kc_bots", "Special variant of Deathsnatch"},
            {"mashup", "Annihilation (Alpha) - test stage"},
            {"mu", "Annihilation"},
            {"none", "None"},
            {"pj", "Unknown"},
            {"rr", "Extraction"},
            {"solo", "Solo"},
            {"tc", "Unknown"},
            {"tdm", "Deathsnatch (Alpha) - test stage"},
            {"territorycontrol", "Unknown"}
        };

        public static Hashtable GetMapSuffixesAndAliases()
        {
            return mapSuffixesAndAliases;
        }

        public static int GetNumberOfMapSuffixesAndAliases()
        {
            return mapSuffixesAndAliases.Count;
        }

        /* Base maps */
        public static readonly string[] baseMapAliases = new string[20] { "Drill Cavern","Drill Cavern (Beta)","Four Points","Fissure","Trailer Park","Dev - simple big green room","The Brewery",
                                                   "Dev - 3 different maps to test game modes","Dev - big map with many turrets","Shattered","Weaponcrafting - loadout test map","Spires",
                                                   "Sploded (Alpha) - colorless trees and almost beta","Dev - tiny map with 4 control points","Dev - small map with a meme",
                                                   "Dev - flying targets which explode","CommTower","Unconfirmed map",
                                                   "Shipping Yard (Alpha) - containers and grippers","Two Ports (Alpha) - one building on each side" };
        /* The order of the array elements should correspond with the order of their aliases */
        public static readonly string[] baseMapOptions = new string[20] { "drillcavern","drillcavern_beta","fath_705","fissure","gliese_581","greenroom","level_three",
                                                   "locomotiongym","projectx_tc","shattered","shooting_gallery_solo","spires",
                                                   "sploded","test_territorycontrol","thefreezer",
                                                   "thepit_pj","tower","three",
                                                   "truckstop2","two_port" };

        // Treatment of PvE ready and game modes in general:
        // The user wants to search for PvE maps or exclude them immediately. What now?
        // Simple checkboxes for: [x] Exclude PvE, [x] Show PvE only
        // Show PvE only: PvE translates to list all maps of game modes botwave + botwaves + tc, manageable size
        // That means it becomes a game mode search
        // A game mode search will always allow the user to click on 1 to n game modes
        // because it's very common for a user to want a few of all game modes
        // Exclude PvE: no projectx_tc since begin + final map (closer choice): loadoutMaps.gameMode != botwave + botwaves

        // TODO:
        // Add a map queue where the user presses a Next button and the next map in the queue becomes the new map

        // TODO:
        // Add a function where the user can click on 4 game modes and go for random map search
        // This function will use its own Hashtable of whatever the user put in and
        // it will pick anything from the big list of struct, outputting the final choice
        // TODO: Have a button to "Reroll" the random map in case the user doesn't like the first output

        private readonly static Hashtable baseMapsAndDefinitions = new Hashtable()
        {
            {"drillcavern", "The map has a house in the middle with a giant drill going through"},
            {"drillcavern_beta_kc", "The map has a house in the middle with a giant drill going through"},
            {"fath_705", "The map is small and has a bridge that crosses the middle"},
            {"fissure", "The map has 3 areas to fall deep as well as a drill rig on each side"},
            {"gliese_581", "The map has a rotating LIVE sign near the center and toilets"},
            {"greenroom", "The developer map is a simple big room where everything is green"},
            {"level_three", "The map has an iconic brewery that is inside of a gorge"},
            {"locomotiongym", "The developer map has different areas for testing game modes"},
            {"projectx", "The developer map is a big map with many aggressive turrets in game mode tc"},
            {"shattered", "The map is a rock land with a giant drill in the middle"},
            {"shooting_gallery_solo", "This is the standard map for testing guns and improving aim"},
            {"spires", "The map has a lot of giant rocks and several paths as well as danger signs"},
            {"sploded", "The map has untextured large trees while everything else is textured"},
            {"test_territorycontrol", "The developer map is a tiny map with 4 control points"},
            {"thefreezer", "The developer map is a small map with a meme on a wall"},
            {"thepit_pj", "The developer map has flying targets which explode"},
            {"tower", "The map has a tower in the middle with a harmless energy beam"},
            // {"three", "Unconfirmed map: Use level_three instead"},
            {"trailerpark_agt", "This is the ranked map with shuttles on each side"},
            {"truckstop2", "The map has containers and grippers and is missing textures"},
            {"two_port", "The map has one building on each side and is missing textures"}
        };

        public static Hashtable GetBaseMapsAndDefinitions()
        {
            return baseMapsAndDefinitions;
        }

        private readonly static Hashtable baseMapsAndDefinitionsNight = new Hashtable()
        {
            {"drillcavern_night", "The dark map has a house in the middle with a giant drill going through"},
            {"fissurenight", "The dark map has 3 areas to fall deep as well as a drill rig on each side"},
            {"gliese_581_night", "The dark map has a rotating LIVE sign near the center and toilets"}
        };

        public static Hashtable GetBaseMapsAndDefinitionsNight()
        {
            return baseMapsAndDefinitionsNight;
        }

        // ---------------------------------------------------------

        // Display-names
        private readonly static Hashtable baseMapsAndAliases = new Hashtable()
        {
            {"drillcavern", "Drill Cavern"},
            {"drillcavern_beta_kc", "Drill Cavern (Beta)"},
            {"fath_705", "Four Points"},
            {"fissure", "Fissure"},
            {"gliese_581", "Trailer Park"},
            {"greenroom", "Greenroom"},
            {"level_three", "The Brewery"},
            {"locomotiongym", "LocomotionGym"},
            {"projectx", "Project X"},
            {"shattered", "Shattered"},
            {"shooting_gallery_solo", "Shooting Gallery"},
            {"spires", "Spires"},
            {"sploded", "Sploded (Alpha)"},
            {"test_territorycontrol", "Test"},
            {"thefreezer", "The Freezer"},
            {"thepit_pj", "The Pit"},
            {"tower", "CommTower"},
            // {"three", "Unknown"},
            {"trailerpark_agt", "Trailer Park (Ranked)"},
            {"truckstop2", "Shipping Yard (Alpha)"},
            {"two_port", "Two Ports (Alpha)"}
        };

        public static Hashtable GetBaseMapsAndAliases()
        {
            return baseMapsAndAliases;
        }

        private readonly static Hashtable baseMapsAndAliasesNight = new Hashtable()
        {
            {"drillcavern_night", "Drill Cavern at Night"},
            {"fissurenight", "Fissure at Night"},
            {"gliese_581_night", "Trailer Park at Night"}
        };

        public static Hashtable GetBaseMapsAndAliasesNight()
        {
            return baseMapsAndAliasesNight;
        }

        public static int GetNumberOfAllBaseMapsAndAliases()
        {
            return baseMapsAndAliases.Count + baseMapsAndAliasesNight.Count;
        }

        // Having the value/alias and looking for a key: foreach Hashtable if value/alias matches then take key

        // The day/night and base map issue with solution:
        // Is Drillcavern at Night not a different base map?
        // Can't night maps have an own place in baseMapsAndAliases?
        // pro: will get treated as different maps, con: how to not list them?
        // >> solution: proceed and use a separate Hashtable for night maps. don't ever change that
        // reason for the solution: this may allow easy switching between day and night variants
        // >> solution addition: banning maps forces to have drillcavern_night
        // !!! so: drillcavern is simply drillcavern at day. if that's not all, even the hashtable itself says if it's day or night
        // it's not a decision for the fastest time listing everything. this is a structure and style decision!
        // handling of the solution addition: user picked drillcavern_night and wants the day variant -> substring checks can and must be used
        // !!! so: not a single key among all Hashtables can exist twice, except for definitions because their keys must match

        // User adds a custom map:
        // - The custom map is supposed to be part of everything
        // - The user writes his own struct LoadoutMap content
        // - The given information will attempt to match anything of the hashtables after reading the save file
        // -- Game mode: If there is no match, there is either a default (such as none as game mode) or simply a new entry
        // --- The user will be asked to explain the game mode and if they explain it, it will be added to the hashtable
        // -- Full map: If there is no key match, always insert at the bottom. If there is a match, abort with an error (duplicate)
        // -- Base map: If there is a match, ignore the hashtable. If there is no match, always insert. Ignore night hashtables
        // -- Struct id: The id will be calculated: "1" + "x" + "501" + "x"
        // --- Unique id issue: User adds 2 custom maps, both of drillcavern with none as game mode (first guess is it's a duplicate)
        // ---- Solution: The user MUST explain the game mode of the second custom map which must be unique
        // ----- If uniqueId found in list of struct then user gets told "We already found ..." and asked what unlisted game mode it is
        // ----- so: The user can add drillcavern_x and drillcavern_y and say the first one is none as game mode (allowed to not specify once)

        public static Hashtable GetFullMapsAndAliases()
        {
            return fullMapsAndAliases;
        }

        /* Maps starting with three as well as maps ending with bots (game mode "Blitz" or "Deathsnatch") are excluded because they don't work except for thefreezer_kc_bots */
        public static readonly string[] maps = new string[86] { "drillcavern_botwave","drillcavern_cpr","drillcavern_domination","drillcavern_kc","drillcavern_night_botwave","drillcavern_night_cpr",
                                                   "drillcavern_night_domination","drillcavern_night_kc","drillcavern_night_rr","drillcavern_rr","drillcavern_beta_kc","fath_705_botwave",
                                                   "fath_705_cpr","fath_705_domination","fath_705_kc","fath_705_rr","fissure_botwave","fissure_cpr","fissure_ctf","fissure_domination",
                                                   "fissure_kc","fissure_rr","fissurenight_botwave","fissurenight_cpr","fissurenight_ctf","fissurenight_domination","fissurenight_kc",
                                                   "fissurenight_rr","gliese_581_botwave","gliese_581_cpr","gliese_581_ctf","gliese_581_domination","gliese_581_kc","gliese_581_mu",
                                                   "gliese_581_night_botwave","gliese_581_night_cpr","gliese_581_night_ctf","gliese_581_night_domination","gliese_581_night_kc",
                                                   "gliese_581_night_rr","gliese_581_rr","greenroom_ctf","greenroom_tdm","level_three_botwaves","level_three_cpr","level_three_domination",
                                                   "level_three_kc","level_three_rr","locomotiongym","locomotiongym_ctf","locomotiongym_domination","locomotiongym_mashup","projectx_tc",
                                                   "shattered_botwave","shattered_cpr","shattered_ctf","shattered_domination","shattered_kc","shattered_rr","shooting_gallery_solo",
                                                   "spires_botwave","spires_cpr","spires_ctf","spires_domination","spires_kc","spires_rr","sploded_ctf","sploded_kc","test_territorycontrol",
                                                   "thefreezer_botwaves","thefreezer_ctp","thefreezer_kc","thefreezer_kc_bots","thefreezer_tdm","thepit_pj","tower_cpr","tower_ctf",
                                                   "tower_domination","tower_kc","tower_rr","truckstop2_cpr","truckstop2_kc","truckstop2_rr","two_port_ctf","two_port_kc","two_port_rr" };

        // ID idea and loadoutMap order:
        // All IDs start with 1
        // The base maps go in alphabetic order from 501 to n - 1st sort priority <-
        // Day and night go from 501 to 502 - 2nd sort priority <-
        // The game modes go in alphabetic order from 501 to n - 3rd sort priority <-
        // "drillcavern_beta_kc" = 1502501508
        // Remaining logic: user says he wants game mode rr, index position 14 in the array, 14 + 500 = id part 514
        // id must be a string to easily substring

        #region LoadoutMap

        public struct LoadoutMap
        {
            public string Id;
            public string FullMapName;
            public string FullMapNameAlt;
            public string BaseMap;
            public string DayNight;
            public string GameMode;
            public string PicturePath;
        }

        #endregion

        public static string MapOrMapAltDecider()
        {
            bool mapAltOrMap = GetMapWithoutInteractions();
            string chosenMap;

            if (mapAltOrMap)
            {
                chosenMap = GetStartingMap().FullMapNameAlt;                
            }
            else
            {
                chosenMap = GetStartingMap().FullMapName;
            }
            return chosenMap;
        }

        private static bool mapWithoutInteractions = false;

        public static bool GetMapWithoutInteractions()
        {
            return mapWithoutInteractions;
        }

        public static void SetMapWithInteractions(bool choice)
        {
            mapWithoutInteractions = choice;
        }




        // The one game mode issue and decision:
        // Main list: listing baseMaps hashtable: if foreach ... thepit == loadoutMaps.baseMap && oneGameMode true then hashtable fullMapNamesAndAliases
        // output full map and alias -> {"thepit_pj", "Dev - flying targets which explode"}
        // downside of that: a foreach through loadoutMaps for every single baseMap to list, looking for the oneGameMode boolean
        // Decision: This will NOT happen. base maps with only 1 game mode will contain the full string
        // reason having the base name drillcavern_beta_kc is wrong, but the ALIAS of it, "Drill Cavern (Beta)", will make everything correct
        // reason to NOT have a oneGameMode boolean: choosing any base map such as Drill Cavern (Beta) will cause a foreach through the list, 
        // finding drillcavern_beta_kc [only once] as result will immediately go for the full map alias hashtable, output
        // "Drill Cavern (Beta)\nGame mode: Deathsnatch". one button "Patch now!" will become clickable and the "Cancel" button (far) to the right clickable the whole time
        // reason to not output the game mode immediately in such case: too much text/information at once
        // so: shooting_gallery_solo will never be shooting_gallery anywhere -> alias does the job

        // the core idea is to work with the hashtables a lot
        // if the user wants a specific map which means it can be only 1 map, the list must be used
        // as well as the hashtables for fullMapNameAlias and Definition

        // id of gliese_581_night_botwave = 1503502501 only works thanks to the 502 part in the middle!
        // 502 will always say: ignore the day maps and look into night because this is a night map
        // so: ever reading the id means it will always look for day-night part first (501/502) before anything else

        public static List<LoadoutMap> GetLoadoutMaps()
        {
            return loadoutMaps;
        }

        // TODO: A new method to patch a random map PatchRandomMap()

        // TODO: A new method to patch a random night map PatchRandomNightMap()

        /// <summary>
        /// Counts unknown errors that can occur using this class
        /// </summary>
        private static int unknownErrorCount = 0;

        public static int GetUnknownErrorCount()
        {
            return unknownErrorCount;
        }

        private static bool aHashtablePairWasAdded = false;

        // Enum used to differentiate the hashtables
        // All hashtables:
        // baseMapsAndAliases
        // baseMapsAndDefinitions
        // baseMapsAndAliasesNight
        // baseMapsAndDefinitionsNight
        // mapSuffixesAndAliases
        // mapSuffixesAndDefinitions
        // fullMapsAndAliases
        [Flags]
        public enum HashtablesEnum : int
        {
            BaseMapsAndAliases = 0x00000001,
            BaseMapsAndDefinitions = 0x00000002,
            BaseMapsAndAliasesNight = 0x00000004,
            BaseMapsAndDefinitionsNight = 0x00000008,
            MapSuffixesAndAliases = 0x00000010,
            MapSuffixesAndDefinitions = 0x00000020,
            FullMapsAndAliases = 0x00000040,
            All = 0x00000080,
        }

        /// <summary>
        /// Resets the list of loadout maps as well as all the map hashtables
        /// Use this method with the initial save file immediately after a different save file was loaded
        /// </summary>
        /// <param name="saveFile"></param>
        public static void ResetLoadoutMapsAndHashtableEntries(Filesave.SaveFile saveFile)
        {
            // 1st: Reset the list of LoadoutMap (loadoutMaps)
            foreach (LoadoutMap loadoutMap in loadoutMaps)
            {
                if (!standardMapIds.Contains(loadoutMap.Id))
                {
                    loadoutMaps.Remove(loadoutMap);
                }
            }

            // 2nd: Foreach (HashtablesEnum, KeyValuePair<string, string>) hashtableEntry in entries, determine which hashtable, remove the entry
            // We use an if condition because there are many cases where entries don't exist. There may be cases where no custom maps are 
            // written but additional entries.
            // The inner sense of this is to have an already loaded save file where all values exist. Now the user wants to remove a custom map and 
            // therefore, additional entries also exist.
            // In the case of starting the program and wanting to remove entries, this if condition comes before the save file contents are loaded in.
            // That means this method is only triggered after the save file is loaded in, after the user removes a custom map using the GUI.
            // Major reason to have this is because the user usually wants to keep their custom maps, at least when first loading the save.
            if (saveFile.AdditionalMapHashtableEntries != null)
            {
                foreach ((HashtablesEnum, KeyValuePair<string, string>) hashtableEntry in saveFile.AdditionalMapHashtableEntries)
                {
                    try
                    {
                        if (hashtableEntry.Item1 == HashtablesEnum.BaseMapsAndAliases)
                        {
                            baseMapsAndAliases.Remove(hashtableEntry.Item2.Key);
                        }

                        if (hashtableEntry.Item1 == HashtablesEnum.BaseMapsAndDefinitions)
                        {
                            baseMapsAndDefinitions.Remove(hashtableEntry.Item2.Key);
                        }

                        if (hashtableEntry.Item1 == HashtablesEnum.BaseMapsAndAliasesNight)
                        {
                            baseMapsAndAliasesNight.Remove(hashtableEntry.Item2.Key);
                        }

                        if (hashtableEntry.Item1 == HashtablesEnum.BaseMapsAndDefinitionsNight)
                        {
                            baseMapsAndDefinitionsNight.Remove(hashtableEntry.Item2.Key);
                        }

                        if (hashtableEntry.Item1 == HashtablesEnum.MapSuffixesAndAliases)
                        {
                            mapSuffixesAndAliases.Remove(hashtableEntry.Item2.Key);
                        }

                        if (hashtableEntry.Item1 == HashtablesEnum.MapSuffixesAndDefinitions)
                        {
                            mapSuffixesAndDefinitions.Remove(hashtableEntry.Item2.Key);
                        }

                        if (hashtableEntry.Item1 == HashtablesEnum.FullMapsAndAliases)
                        {
                            fullMapsAndAliases.Remove(hashtableEntry.Item2.Key);
                        }
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
                        unknownErrorCount++;
                    }
                }
            }
        }

        // This is a shorthand to find out where entries were added for a single custom map to get their values
        private readonly static List<(HashtablesEnum, KeyValuePair<string, string>)> addedHashtableEntries = new List<(HashtablesEnum, KeyValuePair<string, string>)> ();

        /// <summary>
        /// Gives the log list of added hashtable entries
        /// Use this to quickly access the hashtable values of a custom map
        /// Use this as a shorthand to save data to the save file
        /// </summary>
        /// <returns>the log of added hashtable entries for a single custom map</returns>
        private static List<(HashtablesEnum, KeyValuePair<string, string>)> GetAddedHashtableEntries()
        {
            return addedHashtableEntries;
        }

        /// <summary>
        /// Adds an entry to the hashtable entries log
        /// </summary>
        /// <param name="hashtableEntry"></param>
        private static void AddEntryToAddedHashtableEntries((HashtablesEnum, KeyValuePair<string, string>) hashtableEntry)
        {
            addedHashtableEntries.Add(hashtableEntry);
        }

        /// <summary>
        /// Clears a log of hashtable entries
        /// </summary>
        private static void ClearAddedHashtableEntries()
        {
            addedHashtableEntries.Clear();
        }

        /// <summary>
        /// Uses the provided data from the save file to add an hashtable entry
        /// Saves a copy of the hashtable entry to the log
        /// </summary>
        /// <param name="mapFromSaveFile"></param>
        /// <param name="whichHashtables"></param>
        /// <returns>true or false</returns>
        private static bool AddNewHashtableEntry((HashtablesEnum, KeyValuePair<string, string>) hashtableEntry)
        {
            // TODO: Get List<(HashtablesEnum, KeyValuePair<string, string>)> from the save file

            // LoadoutMap[] treating outside
            // List<(HashtablesEnum, KeyValuePair<string, string>)> foreach elsewhere within the class

            // All hashtables:
            // baseMapsAndAliases
            // baseMapsAndDefinitions
            // baseMapsAndAliasesNight
            // baseMapsAndDefinitionsNight
            // mapSuffixesAndAliases
            // mapSuffixesAndDefinitions
            // fullMapsAndAliases

            try
            {
                if (hashtableEntry.Item1 == HashtablesEnum.BaseMapsAndAliases)
                {
                    baseMapsAndAliases.Add(hashtableEntry.Item2.Key, hashtableEntry.Item2.Value);
                }

                if (hashtableEntry.Item1 == HashtablesEnum.BaseMapsAndDefinitions)
                {
                    baseMapsAndDefinitions.Add(hashtableEntry.Item2.Key, hashtableEntry.Item2.Value);
                }

                if (hashtableEntry.Item1 == HashtablesEnum.BaseMapsAndAliasesNight)
                {
                    baseMapsAndAliasesNight.Add(hashtableEntry.Item2.Key, hashtableEntry.Item2.Value);
                }

                if (hashtableEntry.Item1 == HashtablesEnum.BaseMapsAndDefinitionsNight)
                {
                    baseMapsAndDefinitionsNight.Add(hashtableEntry.Item2.Key, hashtableEntry.Item2.Value);
                }

                if (hashtableEntry.Item1 == HashtablesEnum.MapSuffixesAndAliases)
                {
                    mapSuffixesAndAliases.Add(hashtableEntry.Item2.Key, hashtableEntry.Item2.Value);
                }

                if (hashtableEntry.Item1 == HashtablesEnum.MapSuffixesAndDefinitions)
                {
                    mapSuffixesAndDefinitions.Add(hashtableEntry.Item2.Key, hashtableEntry.Item2.Value);
                }

                if (hashtableEntry.Item1 == HashtablesEnum.FullMapsAndAliases)
                {
                    fullMapsAndAliases.Add(hashtableEntry.Item2.Key, hashtableEntry.Item2.Value);
                }

                // Saving a copy of the hashtable entry to the log
                AddEntryToAddedHashtableEntries(hashtableEntry);
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
                unknownErrorCount++;
                return false;
            }

            aHashtablePairWasAdded = true;
            return true;
        }


        /// <summary>
        /// Finds out where the given key belongs, asks for the value or even generates it and
        /// adds one or two key-value pairs to one or two hashtables each time it's called
        /// Additionally, it calls a method to save a copy of the key-value pair or pairs to a log
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashtable"></param>
        /// <returns>true or false</returns>
        private static bool AddNewHashtableEntry(string key, HashtablesEnum hashtable)
        {
            // Definitions as well
            // It must know which hashtable (day or night or game)
            // In every case (if this method succeeded once) it must also be added an entry to fullName
            // a full name add uses all information to build a .Value that makes sense
            // This method is not allowed to set aHashtablePairWasAdded to false because
            // aHashtablePairWasAdded counts during the whole process of adding a custom map

            // All hashtables:
            // baseMapsAndAliases
            // baseMapsAndDefinitions
            // baseMapsAndAliasesNight
            // baseMapsAndDefinitionsNight
            // mapSuffixesAndAliases
            // mapSuffixesAndDefinitions
            // fullMapsAndAliases
            // - build .Value for fullMapsAndAliases
            string? alias = null;
            string? definition = null;

            if (hashtable != HashtablesEnum.FullMapsAndAliases)
            {
                Console.WriteLine("Do you want to add an alias for " + key + "?");
                alias = Console.ReadLine();
                if (System.String.IsNullOrEmpty(alias))
                {
                    alias = key;
                }
                Console.WriteLine("Do you want to add a definition for " + alias + "?");
                definition = Console.ReadLine();
                if (System.String.IsNullOrEmpty(alias))
                {
                    definition = "No definition";
                }
            }

            KeyValuePair<string, string> entryAlias = new KeyValuePair<string, string>();
            KeyValuePair<string, string> entryDefinition = new KeyValuePair<string, string>();

            if (alias != null && definition != null)
            {
                entryAlias = new KeyValuePair<string, string>(key, alias);
                entryDefinition = new KeyValuePair<string, string>(key, definition);
            }

            try
            {
                if (hashtable == HashtablesEnum.BaseMapsAndAliases)
                {
                    baseMapsAndAliases.Add(key, alias);
                    AddEntryToAddedHashtableEntries((HashtablesEnum.BaseMapsAndAliases, entryAlias));
                    baseMapsAndDefinitions.Add(key, definition);
                    AddEntryToAddedHashtableEntries((HashtablesEnum.BaseMapsAndAliases, entryDefinition));
                }

                if (hashtable == HashtablesEnum.BaseMapsAndAliasesNight)
                {
                    baseMapsAndAliasesNight.Add(key, alias);
                    AddEntryToAddedHashtableEntries((HashtablesEnum.BaseMapsAndAliasesNight, entryAlias));
                    baseMapsAndDefinitionsNight.Add(key, definition);
                    AddEntryToAddedHashtableEntries((HashtablesEnum.BaseMapsAndAliasesNight, entryDefinition));
                }

                if (hashtable == HashtablesEnum.MapSuffixesAndAliases)
                {
                    mapSuffixesAndAliases.Add(key, alias);
                    AddEntryToAddedHashtableEntries((HashtablesEnum.MapSuffixesAndAliases, entryAlias));
                    mapSuffixesAndDefinitions.Add(key, definition);
                    AddEntryToAddedHashtableEntries((HashtablesEnum.MapSuffixesAndDefinitions, entryDefinition));
                }

                if (hashtable == HashtablesEnum.FullMapsAndAliases)
                {
                    // Most users would have trouble to write a good alias here. It needs to be built
                    // Here an example: "Locomotion Gym\nGame mode: Annihilation (Alpha) - test stage"

                    string baseMapAliasPart = "";

                    string gameModeAliasPart = "";
                                        
                    List<(HashtablesEnum, KeyValuePair<string, string>)> entriesOfCustomMap = GetAddedHashtableEntries();

                    // {"locomotiongym_mashup", "Locomotion Gym\nGame mode: Annihilation (Alpha) - test stage"},
                    foreach ((HashtablesEnum, KeyValuePair<string, string>) entry in entriesOfCustomMap)
                    {
                        if (entry.Item1 == HashtablesEnum.BaseMapsAndAliases || entry.Item1 == HashtablesEnum.BaseMapsAndAliasesNight)
                        {
                            if (entry.Item2.Value is not null)
                            {
                                baseMapAliasPart = (string)entry.Item2.Value;
                            }
                        }

                        if (entry.Item1 == HashtablesEnum.MapSuffixesAndAliases)
                        {
                            if (entry.Item2.Value is not null)
                            {
                                gameModeAliasPart = (string)entry.Item2.Value;
                            }
                        }
                    }

                    // Does it grant a good alias? Answer: The worst that can happen because of the user:
                    // instead of: {"locomotiongym_mashup", "Locomotion Gym\nGame mode: Annihilation (Alpha) - test stage"},
                    // user skipped providing details: {"locomotiongym_mashup", "locomotiongym_mashup\nGame mode: none"},
                    // The program isn't able to optimize the alias any more.

                    alias = baseMapAliasPart + '\n' + "Game mode: " + gameModeAliasPart;

                    fullMapsAndAliases.Add(key, alias);
                    AddEntryToAddedHashtableEntries((HashtablesEnum.FullMapsAndAliases, entryAlias));
                }
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
                unknownErrorCount++;
            }            

            aHashtablePairWasAdded = true;
            return true;
        }

        /// <summary>
        /// Removes a custom map and its entries and makes the same changes on the given save file
        /// Use this method after selecting a custom map to remove via GUI
        /// </summary>
        /// <param name="mapFromSaveFile"></param>
        /// <param name="saveFile"></param>
        /// <returns>true or false</returns>
        public static bool RemoveCustomMap(LoadoutMap mapFromSaveFile, Filesave.SaveFile saveFile)
        {
            // Via GUI the custom map to remove can be selected
            // Here we find out which hashtable entries belong to the map

            // All hashtables:
            // baseMapsAndAliases
            // baseMapsAndDefinitions
            // baseMapsAndAliasesNight
            // baseMapsAndDefinitionsNight
            // mapSuffixesAndAliases
            // mapSuffixesAndDefinitions
            // fullMapsAndAliases

            // 1st: Remove the hashtable entries related to this chosen custom map, mapFromSaveFile
            // A first check is required to make sure what is being removed is non-standard
            foreach ((Map.HashtablesEnum, KeyValuePair<string, string>) additionalMapHashtableEntry in saveFile.AdditionalMapHashtableEntries)
            {
                // If the entry that is stored in the save file has a relation with the chosen custom map for removal
                // If it's true, this already states that 4 hashtables most likely contain the key
                if (mapFromSaveFile.BaseMap == additionalMapHashtableEntry.Item2.Key)
                {
                    // If the base map is non-standard
                    // This is a double check that is only needed to verify that there is something that needs to be removed
                    // In this case, at the very least, it can either be day or night and not both, so the double check makes sense
                    if (baseMapsAndAliases.ContainsKey(mapFromSaveFile.BaseMap))
                    {
                        baseMapsAndAliases.Remove(mapFromSaveFile.BaseMap);
                    }
                    if (baseMapsAndDefinitions.ContainsKey(mapFromSaveFile.BaseMap))
                    {
                        baseMapsAndDefinitions.Remove(mapFromSaveFile.BaseMap);
                    }
                    if (baseMapsAndAliasesNight.ContainsKey(mapFromSaveFile.BaseMap))
                    {
                        baseMapsAndAliasesNight.Remove(mapFromSaveFile.BaseMap);
                    }
                    if (baseMapsAndDefinitionsNight.ContainsKey(mapFromSaveFile.BaseMap))
                    {
                        baseMapsAndDefinitionsNight.Remove(mapFromSaveFile.BaseMap);
                    }

                    // Save file synchronization
                    saveFile.AdditionalMapHashtableEntries.Remove(additionalMapHashtableEntry);
                }

                // If the entry that is stored in the save file has a relation with the chosen custom map for removal
                // If it's true, this already states that 2 hashtables most likely contain the key
                if (mapFromSaveFile.GameMode == additionalMapHashtableEntry.Item2.Key)
                {
                    // If the game mode is non-standard
                    // This is an almost unnecessary double check. There is a possibility the method is being more than it should
                    if (mapSuffixesAndAliases.ContainsKey(mapFromSaveFile.GameMode))
                    {
                        mapSuffixesAndAliases.Remove(mapFromSaveFile.GameMode);
                    }
                    if (mapSuffixesAndDefinitions.ContainsKey(mapFromSaveFile.GameMode))
                    {
                        mapSuffixesAndDefinitions.Remove(mapFromSaveFile.GameMode);
                    }

                    // Save file synchronization
                    saveFile.AdditionalMapHashtableEntries.Remove(additionalMapHashtableEntry);
                }

                // If the entry that is stored in the save file has a relation with the chosen custom map for removal
                // If it's true, this already states that 1 hashtable most likely contains the key
                // This check is required or else the first entry of the foreach, no matter what it is, would get removed
                if (mapFromSaveFile.FullMapName == additionalMapHashtableEntry.Item2.Key)
                {
                    // The full map name must be non-standard because it's a custom map
                    // This is an almost unnecessary double check. There is a possibility the method is being more than it should
                    if (fullMapsAndAliases.ContainsKey(mapFromSaveFile.FullMapName))
                    {
                        fullMapsAndAliases.Remove(mapFromSaveFile.FullMapName);
                    }

                    // Save file synchronization
                    saveFile.AdditionalMapHashtableEntries.Remove(additionalMapHashtableEntry);
                }
            }

            // 2nd: Remove the custom map from the list of LoadoutMap (loadoutMaps)
            if (loadoutMaps.Contains(mapFromSaveFile))
            {
                loadoutMaps.Remove(mapFromSaveFile);

                // Save file synchronization
                saveFile.CustomMaps.Remove(mapFromSaveFile);

                // ----------
                // 3rd: Remove the custom map from starting map, favorite maps, map queue, map blacklist and map whitelist
                if (startingMap.Id == mapFromSaveFile.Id)
                {
                    // We replace the custom starting map with shooting_gallery_solo
                    SetStartingMap(new LoadoutMap
                    {
                        Id = "1511501515",
                        FullMapName = "shooting_gallery_solo",
                        FullMapNameAlt = "Shooting_Gallery_Solo",
                        BaseMap = "shooting_gallery_solo",
                        DayNight = "day",
                        GameMode = "solo"
                    });
                }

                if (favoriteMaps.Count > 0)
                {
                    foreach (LoadoutMap favoriteMap in favoriteMaps)
                    {
                        if (favoriteMap.Id == mapFromSaveFile.Id)
                        {
                            favoriteMaps.Remove(favoriteMap);
                            break;
                        }
                    }
                }

                if (mapQueue.Count > 0)
                {
                    foreach (LoadoutMap map in mapQueue)
                    {
                        if (map.Id == mapFromSaveFile.Id)
                        {
                            // Trigger the creation of an updated map queue list copy
                            SetMapQueue(mapQueue);
                            // Delete the custom map from the map queue
                            mapQueueListCopy.Remove(map);
                            SetMapQueue(new Stack<LoadoutMap>(mapQueueListCopy));
                            break;
                        }
                    }
                }

                if (mapBlacklist.Count > 0)
                {
                    foreach (LoadoutMap map in mapBlacklist)
                    {
                        if (map.Id == mapFromSaveFile.Id)
                        {
                            mapBlacklist.Remove(map);
                            break;
                        }
                    }
                }

                if (mapWhitelist.Count > 0)
                {
                    foreach (LoadoutMap map in mapWhitelist)
                    {
                        if (map.Id == mapFromSaveFile.Id)
                        {
                            mapWhitelist.Remove(map);
                            break;
                        }
                    }
                }
                // ----------

                return true;
            }
            else
            {
                Console.WriteLine("\nError: The custom map to remove wasn't found in the list of loadout maps!");
                return false;
            }
        }

        /// <summary>
        /// Inserts a custom map from the save file
        /// This is the simpler method because the save file should contain all necessary information, especially the calculated ID
        /// This is a private method to make sure it's not trying to add the same custom map twice
        /// </summary>
        /// <param name="mapFromSaveFile"></param>
        /// <param name="hashtables"></param>
        /// <returns>true or false</returns>
        private static bool SetNewCustomMap(LoadoutMap mapFromSaveFile, List<(HashtablesEnum, KeyValuePair<string, string>)> hashtableEntries)
        {
            // If id of mapFromSaveFile wasn't found in standardloadoutmapids, meaning it must be a custom map
            if (!GetStandardMapIds().Contains(mapFromSaveFile.Id))
            {
                // Make hashtable inserts
                foreach ((HashtablesEnum, KeyValuePair<string, string>) hashtableEntry in hashtableEntries)
                {
                    if (!AddNewHashtableEntry(hashtableEntry))
                    {
                        // We output an error for the duplicate and cancel
                        if (hashtableEntry.Item1 == HashtablesEnum.FullMapsAndAliases)
                        {
                            Console.WriteLine("> Error: A custom map couldn't be added again because it was found in a list!");
                            Console.WriteLine("> This is a duplicate!");
                        }
                        return false;
                    }
                }

                // Add to list of struct
                loadoutMaps.Add(new LoadoutMap
                {
                    Id = mapFromSaveFile.Id,
                    FullMapName = mapFromSaveFile.FullMapName,
                    FullMapNameAlt = mapFromSaveFile.FullMapNameAlt,
                    BaseMap = mapFromSaveFile.BaseMap,
                    DayNight = mapFromSaveFile.DayNight,
                    GameMode = mapFromSaveFile.GameMode
                });
            }
            else
            {
                return false;
            }    

            // This only clears a log containing copies, not the actual entries
            ClearAddedHashtableEntries();

            return true;
        }

        // TODO:
        // In case of a custom map add, give the user the default option to add
        // {"unlisted", "Unlisted"} as mapSuffixesAndAliases entry
        // {"unlisted", "No definition"} as mapSuffixesAndDefinitions entry

        /// <summary>
        /// Goes through 6 necessary steps in order to add a new custom map
        /// Makes calls in order to save the log that is being created for the save file
        /// </summary>
        /// <param name="saveFile">passed by reference</param>
        /// <param name="newMap"></param>
        /// <param name="baseMap"></param>
        /// <param name="dayNight"></param>
        /// <param name="gameMode"></param>
        /// <param name="fullMapNameAlt"></param>
        /// <returns>true or false</returns>
        public static bool SetNewCustomMap(ref Filesave.SaveFile saveFile, string newMap, string baseMap, string dayNight, string gameMode, string fullMapNameAlt = "")
        {
            if (fullMapNameAlt == "")
            {
                fullMapNameAlt = GenerateFullMapNameAlt(newMap);
            }
            newMap = newMap.ToLower();

            // 1st: Scan full map hashtable for duplicate (don't continue if there is a match)
            // Going through fullMapsAndAliases
            foreach (KeyValuePair<string, string> fullMapInHashtable in fullMapsAndAliases)
            {
                // if .Value matches, then baseMap must no longer be .Value because it's supposed to be .Key
                if ((string)fullMapInHashtable.Key == newMap)
                {
                    // We output an error for the duplicate and cancel
                    Console.WriteLine("> Error: The custom map can't be added again because it was found in a list!");
                    Console.WriteLine("> This is a duplicate!");
                    Console.WriteLine("> Cancelling task ...");
                    return false;
                }
            }

            // 2nd: Calculate id, make hashtable inserts, assign the returned id
            string calculatedId = CalculateMapId(baseMap, dayNight, gameMode);

            // 3rd: Scan list of struct for duplicate id (don't continue if there is a match which should mean no insert ever happened)
            foreach (LoadoutMap map in loadoutMaps)
            {
                if (map.Id == calculatedId)
                {
                    // We output an error for the duplicate and cancel
                    Console.WriteLine("> Error: The custom map can't be added because its id was found in a list!");
                    Console.WriteLine("> This is a duplicate!");
                    Console.WriteLine("> Cancelling task ...");
                    return false;
                }
            }

            // 4th: Add to the full map hashtable
            if (aHashtablePairWasAdded)
            {
                if (!AddNewHashtableEntry(newMap, HashtablesEnum.FullMapsAndAliases))
                {
                    Console.WriteLine("> Error: The entry for a custom map could not be added in full map.");
                    Console.WriteLine("> Proceeding with the task ...");
                }
            }

            // 5th: Add to list of struct
            LoadoutMap toSaveFileLoadoutMapStruct = new LoadoutMap
            {
                Id = calculatedId,
                FullMapName = newMap,
                FullMapNameAlt = fullMapNameAlt,
                BaseMap = baseMap,
                DayNight = dayNight,
                GameMode = gameMode
            };
            loadoutMaps.Add(toSaveFileLoadoutMapStruct);
            List<(HashtablesEnum, KeyValuePair<string, string>)> toSaveFileHashtableEntries = GetAddedHashtableEntries();

            // 6th: Save toSaveFileLoadoutMapStruct as well as toSaveFileHashtableEntries into saveFile
            saveFile.CustomMaps.Add(toSaveFileLoadoutMapStruct);
            foreach ((HashtablesEnum, KeyValuePair<string, string>) toSaveFileHashtableEntry in toSaveFileHashtableEntries)
            {
                saveFile.AdditionalMapHashtableEntries.Add(toSaveFileHashtableEntry);
            }

            // This only clears a log containing copies, not the actual entries
            // Make sure that this log was saved to the save file before clearing it!
            ClearAddedHashtableEntries();

            return true;
        }


        /// <summary>
        /// Selects a random map within the range of the list of loadout maps
        /// Use this method if the user wants to play in any map of any game mode
        /// </summary>
        /// <param name="selectedMap"></param>
        public static void SelectRandomMap(out LoadoutMap selectedMap)
        {
            int numberOfElements = loadoutMaps.Count;

            // Initializes a variable with any index number within the range of the list of loadout maps
            int randomMapIndex = new Random().Next(numberOfElements);

            selectedMap = loadoutMaps[randomMapIndex];
        }

        /// <summary>
        /// Selects a random map within the range of the list of maps at night
        /// Use this method if the user wants to play in any night map of any game mode
        /// </summary>
        /// <param name="selectedMap"></param>
        public static void SelectRandomMapNight(out LoadoutMap selectedMap)
        {
            // To avoid errors, the default selected night map gliese_581_night_ctf gets assigned
            // If this method is reliable, this should never influence the randomness because it works as expected
            selectedMap = new LoadoutMap
            {
                Id = "1503502505",
                FullMapName = "gliese_581_night_ctf",
                FullMapNameAlt = "Gliese_581_Night_CTF",
                BaseMap = "gliese_581_night",
                DayNight = "night",
                GameMode = "ctf"
            };

            int numberOfElements = 0;

            // Find out the number of night maps including all variations
            foreach (LoadoutMap loadoutMap in loadoutMaps)
            {
                if (loadoutMap.DayNight == "night")
                {
                    numberOfElements++;
                }
            }

            // Initializes a variable with any index number within the range of night maps and their variations
            int randomMapIndex = new Random().Next(numberOfElements);
            bool complete = false;

            foreach (LoadoutMap loadoutMap in loadoutMaps)
            {
                // Uses the random number on night maps only
                if (loadoutMap.DayNight == "night")
                {
                    // If the random counter is at zero, assign the next found night map
                    if (randomMapIndex == 0)
                    {
                        selectedMap = loadoutMap;
                        complete = true;
                        break;
                    }
                    // Reduces the number by 1 for every found night map, sticking to night maps only
                    randomMapIndex--;
                }
            }

            if (!complete)
            {
                Console.WriteLine("\n> Error: Something went wrong trying to get a random night map!");
                Console.WriteLine("\n> The default night map was chosen instead.");
            }
        }



        /// <summary>
        /// Converts a given map name into an alternative map name
        /// This method can never cover all different cases for new custom maps
        /// The user will either accept the generated string or type in the correct one if it's wrong
        /// </summary>
        /// <param name="mapFullName"></param>
        /// <returns>alternative map name</returns>
        public static string GenerateFullMapNameAlt(string mapFullName)
        {
            char[] theMapCut = mapFullName.ToCharArray();
            string mapFullNameAlt = "";
            bool underscoreDetected = false;
            bool doneOnce = false;
            for (int i = 0; i < theMapCut.Length; i++)
            {
                // If it's the first character, it will always be capitalized
                if (i == 0)
                {
                    mapFullNameAlt += theMapCut[i].ToString().ToUpper();
                }
                else if (theMapCut[i] == '_')
                {
                    mapFullNameAlt += theMapCut[i];
                    // Always capitalize the next character after an underscore
                    theMapCut[i + 1] = Char.Parse(theMapCut[i + 1].ToString().ToUpper());
                    underscoreDetected = true;
                }
                else
                {
                    mapFullNameAlt += theMapCut[i];
                }

                // If there is an underscore and this wasn't done yet, go inside
                if (underscoreDetected && !doneOnce)
                {
                    // If the map ending is reached with 2 letters after the underscore like kc which stands for Deathsnatch
                    if (i + 3 == theMapCut.Length)
                    {
                        // Capitalize not only 1st letter but also the 2nd letter as well
                        theMapCut[i + 2] = Char.Parse(theMapCut[i + 2].ToString().ToUpper());
                        doneOnce = true;
                    }
                    // Else if the map ending is reached with 3 letters after the underscore like cpr which stands for Blitz
                    else if (i + 4 == theMapCut.Length)
                    {
                        // Capitalize not only 1st letter but also the 2nd and 3rd letter as well
                        theMapCut[i + 2] = Char.Parse(theMapCut[i + 2].ToString().ToUpper());
                        theMapCut[i + 3] = Char.Parse(theMapCut[i + 3].ToString().ToUpper());
                        doneOnce = true;
                    }
                }
            }
            return mapFullNameAlt;
        }

        /// <summary>
        /// This is a new function that uses the base map string to fetch the alias
        /// Example: Base map fath_705 is alias Four Points
        /// </summary>
        /// <returns>the matching alias from a hashtable of aliases</returns>
        public static string FetchMatchingAliasMap(string baseMap)
        {
            Hashtable mapNames = Map.GetBaseMapsAndAliases();
            Hashtable mapNamesNight = Map.GetBaseMapsAndAliasesNight();

            if (mapNamesNight.ContainsKey(baseMap) == true)
            {
                MainProperties.MatchingAliasMap = (string)mapNamesNight[baseMap]!;
            }
            else if (mapNames.ContainsKey(baseMap) == true)
            {
                MainProperties.MatchingAliasMap = (string)mapNames[baseMap]!;
            }
            else
            {
                MainProperties.MatchingAliasMap = "";
            }

            return MainProperties.MatchingAliasMap!;
        }

        /// <summary>
        /// This is a new function that uses the game mode string to fetch the alias
        /// Example: Game mode ctf is alias Jackhammer
        /// </summary>
        /// <returns>the matching alias from a hashtable of aliases</returns>
        public static string FetchMatchingAliasGameMode(string gameMode)
        {
            Hashtable gameModeTranslations = Map.GetMapSuffixesAndAliases();

            if (gameModeTranslations.ContainsKey(gameMode) == true)
            {
                MainProperties.MatchingAliasGameMode = (string)gameModeTranslations[gameMode]!;
            }
            else
            {
                MainProperties.MatchingAliasGameMode = "";
            }

            return MainProperties.MatchingAliasGameMode!;
        }

        /// <summary>
        /// Gets and returns the alias that matches the base map part of the new map. Base map aliases and game mode aliases are supported
        /// This is a legacy function!
        /// </summary>
        /// <param name="newMap"></param>
        /// <param name="options"></param>
        /// <param name="aliases"></param>
        /// <param name="totalOptions"></param>
        /// <returns>the matching alias from an array of aliases</returns>
        public static string FetchMatchingAlias(string newMap, string[] options, string[] aliases, int totalOptions)
        {
            string matchingAlias = "";
            for (int i = 0; i < totalOptions; i++)
            {
                // newMap.IndexOf(options[i]) > 0 // 1 if something was found
                // Zero stand for the same position of the strings, meaning identical strings
                if (newMap.Contains(options[i]))
                {
                    matchingAlias = aliases[i];
                    break;
                }
            }
            return matchingAlias;
        }

        #region MapIdCalculation
        /// <summary>
        /// Calculates the map id while calling for hashtable entry inserts that fit
        /// </summary>
        /// <param name="baseMap"></param>
        /// <param name="dayNight"></param>
        /// <param name="gameMode"></param>
        /// <returns>the calculated map id, supposed but not yet proven to be unique</returns>
        public static string CalculateMapId(string baseMap, string dayNight, string gameMode)
        {
            // baseMapsAndAliases required
            string idBaseMapPart = "";

            // if string day or night
            string idDayNightPart = "";

            // mapSuffixesAndAliases required
            string idGameModePart = "";

            // Going through baseMapsAndAliases
            int baseMapMatchAt = 1;
            foreach (KeyValuePair<string, string> baseMapInHashtable in baseMapsAndAliases)
            {
                if (baseMapInHashtable.Value is not null)
                {
                    // if .Value matches, then baseMap must no longer be .Value because it's supposed to be .Key
                    if ((string)baseMapInHashtable.Key == baseMap || (string)baseMapInHashtable.Value == baseMap)
                    {
                        idBaseMapPart = (500 + baseMapMatchAt).ToString();
                        idDayNightPart = "501";
                        break;
                    }
                }
                baseMapMatchAt++;
            }

            // Going through baseMapsAndAliasesNight if there is no match in baseMapsAndAliases
            if (idBaseMapPart == "")
            {
                baseMapMatchAt = 1;
                foreach (KeyValuePair<string, string> baseMapInHashtable in baseMapsAndAliasesNight)
                {
                    if (baseMapInHashtable.Value is not null)
                    {
                        // if .Value matches, then baseMap must no longer be .Value because it's supposed to be .Key
                        if ((string)baseMapInHashtable.Key == baseMap || (string)baseMapInHashtable.Value == baseMap)
                        {
                            idBaseMapPart = (500 + baseMapMatchAt).ToString();
                            idDayNightPart = "502";
                            break;
                        }
                    }
                    baseMapMatchAt++;
                }
            }

            if (idDayNightPart == "")
            {
                if (dayNight == "night")
                {
                    idDayNightPart = "502";
                }
                else
                {
                    idDayNightPart = "501";
                }
            }

            // If there was no match, check for idDayNightPart and add a new entry in baseMapsAndAliases and baseMapsAndDefinitions
            if (idBaseMapPart == "" && idDayNightPart == "501")
            {
                if (AddNewHashtableEntry(baseMap, HashtablesEnum.BaseMapsAndAliases))
                {
                    idBaseMapPart = (500 + baseMapsAndAliases.Count).ToString();
                }
                else
                {
                    Console.WriteLine("> Error: The entry for a custom map could not be added in base map.");
                    Console.WriteLine("> Proceeding with the task ...");
                }
            }
            // If there was no match, check for idDayNightPart and add a new entry in baseMapsAndAliasesNight and baseMapsAndDefinitionsNight
            else if (idBaseMapPart == "" && idDayNightPart == "502")
            {
                if (AddNewHashtableEntry(baseMap, HashtablesEnum.BaseMapsAndAliasesNight))
                {
                    idBaseMapPart = (500 + baseMapsAndAliasesNight.Count).ToString();
                }
                else
                {
                    Console.WriteLine("> Error: The entry for a custom map could not be added in base map night.");
                    Console.WriteLine("> Proceeding with the task ...");
                }
            }

            int gameModeMatchAt = 1;
            foreach (KeyValuePair<string, string> gameModeInHashtable in mapSuffixesAndAliases)
            {
                if (gameModeInHashtable.Value is not null)
                {
                    if ((string)gameModeInHashtable.Key == gameMode || (string)gameModeInHashtable.Value == gameMode)
                    {
                        idGameModePart = (500 + gameModeMatchAt).ToString();
                        break;
                    }
                }
                gameModeMatchAt++;
            }

            // If there was no match, add a new entry in mapSuffixesAndAliases and mapSuffixesAndDefinitions
            if (idGameModePart == "")
            {
                if (AddNewHashtableEntry(gameMode, HashtablesEnum.MapSuffixesAndAliases))
                {
                    idGameModePart = (500 + mapSuffixesAndAliases.Count).ToString();
                }
                else
                {
                    Console.WriteLine("> Error: The entry for a custom map could not be added in base map.");
                    Console.WriteLine("> Proceeding with the task ...");
                    unknownErrorCount++;
                }
            }

            string calculatedId = "1" + idBaseMapPart + idDayNightPart + idGameModePart;

            return calculatedId;

            // if not found then insert in hashtable as well as idPart = 500 + count of pairs in hashtable
        }
        #endregion

        #region StandardLoadoutMapIds
        /// <summary>
        /// Contains all the unique IDs of available maps that are hard coded and work
        /// This HashSet automatically removes duplicates without throwing an exception
        /// </summary>
        private readonly static HashSet<string> standardMapIds = new HashSet<string>()
        {
           "1507501501",
           "1517501501",
           "1501501501",
           "1502501510",
           "1501501502",
           "1501501504",
           "1501501508",
           "1501501510",
           "1501502501",
           "1501502502",
           "1501502504",
           "1501502508",
           "1501502510",
           "1501502516",
           "1501501516",
           "1503501501",
           "1503501502",
           "1503501504",
           "1503501508",
           "1503501510",
           "1503501516",
           "1504501501",
           "1504501502",
           "1504501504",
           "1504501506",
           "1504501508",
           "1504501509",
           "1504501510",
           "1504502501",
           "1504501516",
           "1504502502",
           "1504502504",
           "1504502506",
           "1504502508",
           "1504502510",
           "1504502516",
           "1505501502",
           "1505501504",
           "1505501506",
           "1505501508",
           "1505501510",
           "1518501513",
           "1505502502",
           "1505502504",
           "1505502506",
           "1505502508",
           "1505502510",
           "1505502516",
           "1505501516",
           "1506501514",
           "1506501506",
           "1506501519",
           "1507501503",
           "1507501504",
           "1507501508",
           "1507501510",
           "1507501516",
           "1508501514",
           "1508501506",
           "1508501508",
           "1508501512",
           "1509501514",
           "1509501518",
           "1510501501", // Duplicate ID, maps seem identical
           "1510501502",
           "1510501504",
           "1510501506",
           "1510501508",
           "1510501510",
           "1510501501", // Duplicate ID, maps seem identical
           "1510501516",
           "1511501517",
           "1512501501",
           "1512501502",
           "1512501504",
           "1512501506",
           "1512501508",
           "1512501510",
           "1512501516",
           "1513501514",
           "1513501506",
           "1513501510",
           "1514501520",
           "1515501514",
           "1515501503",
           "1515501507",
           "1515501510",
           "1515501511",
           "1515501519",
           "1516501515",
           "1517501504",
           "1517501506",
           "1517501508",
           "1517501510",
           "1517501516",
           "1518501501",
           "1505501501",
           "1505502501",
           "1519501514",
           "1519501504",
           "1519501510",
           "1519501516",
           "1520501514",
           "1520501506",
           "1520501510",
           "1520501516"
        };
        #endregion

        public static int GetNumberOfLoadoutMaps()
        {
            return loadoutMaps.Count;
        }

        public static int GetNumberOfBotMaps()
        {
            int count = GetVersusBotsMaps().Count;

            return count;
        }

        // Versus bots = Game modes are botwave, botwaves and tc
        public static List<LoadoutMap> GetVersusBotsMaps()
        {
            List<LoadoutMap> loadoutMaps = GetLoadoutMaps();
            List<LoadoutMap> versusBotsMaps = new List<LoadoutMap>();

            foreach (Map.LoadoutMap map in loadoutMaps)
            {
                if (map.GameMode == "botwave" || map.GameMode == "botwaves" || map.GameMode == "tc")
                {
                    versusBotsMaps.Add(map);
                }                
            }
                return versusBotsMaps;
        }

        #region FullMapsAndAliases
        /// <summary>
        /// In alphabetic order
        /// </summary>
        private readonly static Hashtable fullMapsAndAliases = new Hashtable()
        {
            {"brewery_art_master", "The Brewery\nGame mode: None"},
            {"com_tower_art_master", "CommTower\nGame mode: None"},
            {"drillcavern_art_master", "Drillcavern\nGame mode: None"},
            {"drillcavern_beta_kc", "Drillcavern (Beta)\nGame mode: Deathsnatch"},
            {"drillcavern_botwave", "Drillcavern\nGame mode: Hold Your Pole"},
            {"drillcavern_cpr", "Drillcavern\nGame mode: Blitz"},
            {"drillcavern_domination", "Drillcavern\nGame mode: Domination"},
            {"drillcavern_kc", "Drillcavern\nGame mode: Deathsnatch"},
            {"drillcavern_night_art_master", "Drillcavern at Night\nGame mode: None"},
            {"drillcavern_night_botwave", "Drillcavern at Night\nGame mode: Hold Your Pole"},
            {"drillcavern_night_cpr", "Drillcavern at Night\nGame mode: Blitz"},
            {"drillcavern_night_domination", "Drillcavern at Night\nGame mode: Domination"},
            {"drillcavern_night_kc", "Drillcavern at Night\nGame mode: Deathsnatch"},
            {"drillcavern_night_rr", "Drillcavern at Night\nGame mode: Extraction"},
            {"drillcavern_rr", "Drillcavern\nGame mode: Extraction"},
            {"fath_705_art_master", "Four Points\nGame mode: None"},
            {"fath_705_botwave", "Four Points\nGame mode: Hold Your Pole"},
            {"fath_705_cpr", "Four Points\nGame mode: Blitz"},
            {"fath_705_domination", "Four Points\nGame mode: Domination"},
            {"fath_705_kc", "Four Points\nGame mode: Deathsnatch"},
            {"fath_705_rr", "Four Points\nGame mode: Extraction"},
            {"fissure_art_master", "Fissure\nGame mode: None"},
            {"fissure_botwave", "Fissure\nGame mode: Hold Your Pole"},
            {"fissure_cpr", "Fissure\nGame mode: Blitz"},
            {"fissure_ctf", "Fissure\nGame mode: Jackhammer"},
            {"fissure_domination", "Fissure\nGame mode: Domination"},
            {"fissure_geo", "Fissure\nGame mode: None"},
            {"fissure_kc", "Fissure\nGame mode: Deathsnatch"},
            {"fissure_night_art_master", "Fissure at Night\nGame mode: None"},
            {"fissure_rr", "Fissure\nGame mode: Extraction"},
            {"fissurenight_botwave", "Fissure at Night\nGame mode: Hold Your Pole"},
            {"fissurenight_cpr", "Fissure at Night\nGame mode: Blitz"},
            {"fissurenight_ctf", "Fissure at Night\nGame mode: Jackhammer"},
            {"fissurenight_domination", "Fissure at Night\nGame mode: Domination"},
            {"fissurenight_kc", "Fissure at Night\nGame mode: Deathsnatch"},
            {"fissurenight_rr", "Fissure at Night\nGame mode: Extraction"},
            {"gliese_581_botwave", "Trailer Park\nGame mode: Hold Your Pole"},
            {"gliese_581_cpr", "Trailer Park\nGame mode: Blitz"},
            {"gliese_581_ctf", "Trailer Park\nGame mode: Jackhammer"},
            {"gliese_581_domination", "Trailer Park\nGame mode: Domination"},
            {"gliese_581_kc", "Trailer Park\nGame mode: Deathsnatch"},
            {"gliese_581_mu", "Trailer Park\nGame mode: Annihilation"},
            {"gliese_581_night_botwave", "Trailer Park at Night\nGame mode: Hold Your Pole"},
            {"gliese_581_night_cpr", "Trailer Park at Night\nGame mode: Blitz"},
            {"gliese_581_night_ctf", "Trailer Park at Night\nGame mode: Jackhammer"},
            {"gliese_581_night_domination", "Trailer Park at Night\nGame mode: Domination"},
            {"gliese_581_night_kc", "Trailer Park at Night\nGame mode: Deathsnatch"},
            {"gliese_581_night_rr", "Trailer Park at Night\nGame mode: Extraction"},
            {"gliese_581_rr", "Trailer Park\nGame mode: Extraction"},
            {"greenroom", "Greenroom\nGame mode: None"},
            {"greenroom_ctf", "Greenroom\nGame mode: Jackhammer"},
            {"greenroom_tdm", "Greenroom\nGame mode: Deathsnatch (Alpha) - test stage"},
            {"level_three_botwaves", "The Brewery\nGame mode: Hold Your Pole"},
            {"level_three_cpr", "The Brewery\nGame mode: Blitz"},
            {"level_three_domination", "The Brewery\nGame mode: Domination"},
            {"level_three_kc", "The Brewery\nGame mode: Deathsnatch"},
            {"level_three_rr", "The Brewery\nGame mode: Extraction"},
            {"locomotiongym", "Locomotion Gym\nGame mode: None"},
            {"locomotiongym_ctf", "Locomotion Gym\nGame mode: Jackhammer"},
            {"locomotiongym_domination", "Locomotion Gym\nGame mode: Domination"},
            {"locomotiongym_mashup", "Locomotion Gym\nGame mode: Annihilation (Alpha) - test stage"},
            {"projectx", "Project X\nGame mode: None"},
            {"projectx_tc", "Project X\nGame mode: Unknown"},
            {"shattered_art_master", "Shattered\nGame mode: None"},
            {"shattered_botwave", "Shattered\nGame mode: Hold Your Pole"},
            {"shattered_cpr", "Shattered\nGame mode: Blitz"},
            {"shattered_ctf", "Shattered\nGame mode: Jackhammer"},
            {"shattered_domination", "Shattered\nGame mode: Domination"},
            {"shattered_kc", "Shattered\nGame mode: Deathsnatch"},
            {"shattered_no_jack_art_master", "Shattered\nGame mode: None"},
            {"shattered_rr", "Shattered\nGame mode: Extraction"},
            {"shooting_gallery_solo", "Shooting Gallery\nGame mode: Solo"},
            {"spires_art_master", "Spires\nGame mode: None"},
            {"spires_botwave", "Spires\nGame mode: Hold Your Pole"},
            {"spires_cpr", "Spires\nGame mode: Blitz"},
            {"spires_ctf", "Spires\nGame mode: Jackhammer"},
            {"spires_domination", "Spires\nGame mode: Domination"},
            {"spires_kc", "Spires\nGame mode: Deathsnatch"},
            {"spires_rr", "Spires\nGame mode: Extraction"},
            {"sploded_blueroom", "Sploded (Alpha)\nGame mode: None"},
            {"sploded_ctf", "Sploded (Alpha)\nGame mode: Jackhammer"},
            {"sploded_kc", "Sploded (Alpha)\nGame mode: Deathsnatch"},
            {"test_territorycontrol", "Test\nGame mode: Unknown"},
            {"thefreezer", "The Freezer\nGame mode: None"},
            {"thefreezer_botwaves", "The Freezer\nGame mode: Hold Your Pole"}, // Teammates and kroads don't always spawn
            {"thefreezer_ctp", "The Freezer\nGame mode: Extraction (Alpha) - test stage"},
            {"thefreezer_kc", "The Freezer\nGame mode: Deathsnatch"},
            {"thefreezer_kc_bots", "The Freezer\nGame mode: Special variant of Deathsnatch"}, // Equals thefreezer_kc
            {"thefreezer_tdm", "The Freezer\nGame mode: Deathsnatch (Alpha) - test stage"},
            {"thepit_pj", "The Pit\nGame mode: Unknown"},
            {"tower_cpr", "CommTower\nGame mode: Blitz"},
            {"tower_ctf", "CommTower\nGame mode: Jackhammer"},
            {"tower_domination", "CommTower\nGame mode: Domination"},
            {"tower_kc", "CommTower\nGame mode: Deathsnatch"},
            {"tower_rr", "CommTower\nGame mode: Extraction"},
            {"trailerpark_agt_art_master", "Trailer Park\nGame mode: None"}, // Art master of annihilation map
            {"trailerpark_art_master", "Trailer Park\nGame mode: None"},
            {"trailerpark_night_art_master", "Trailer Park\nGame mode: None"},
            {"truckstop2", "Shipping Yard (Alpha)\nGame mode: None"}, // 100% falling into the pit
            {"truckstop2_cpr", "Shipping Yard (Alpha)\nGame mode: Blitz"},
            {"truckstop2_kc", "Shipping Yard (Alpha)\nGame mode: Deathsnatch"},
            {"truckstop2_rr", "Shipping Yard (Alpha)\nGame mode: Extraction"},
            {"two_port", "Two Ports (Alpha)\nGame mode: None"}, // 100% falling into the pit
            {"two_port_ctf", "Two Ports (Alpha)\nGame mode: Jackhammer"},
            {"two_port_kc", "Two Ports (Alpha)\nGame mode: Deathsnatch"},
            {"two_port_rr", "Two Ports (Alpha)\nGame mode: Extraction"}
        };
        #endregion

        #region ListOfLoadoutMap
        private readonly static List<LoadoutMap> loadoutMaps = new List<LoadoutMap>
        {
            new LoadoutMap
            {
                Id = "1507501501",
                FullMapName = "brewery_art_master",
                FullMapNameAlt = "Brewery_Art_Master",
                BaseMap = "level_three",
                DayNight = "day",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/brewery_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1517501501",
                FullMapName = "com_tower_art_master",
                FullMapNameAlt = "Com_Tower_Art_Master",
                BaseMap = "tower",
                DayNight = "day",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/com_tower_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1501501501",
                FullMapName = "drillcavern_art_master",
                FullMapNameAlt = "Drillcavern_Art_Master",
                BaseMap = "drillcavern",
                DayNight = "day",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/drillcavern_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1502501510",
                FullMapName = "drillcavern_beta_kc",
                FullMapNameAlt = "Drillcavern_Beta_KC",
                BaseMap = "drillcavern_beta_kc",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/drillcavern_beta_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1501501502",
                FullMapName = "drillcavern_botwave",
                FullMapNameAlt = "Drillcavern_Botwave",
                BaseMap = "drillcavern",
                DayNight = "day",
                GameMode = "botwave",
                PicturePath = "/Assets/Maps/drillcavern_botwave.webp"
            },

            new LoadoutMap
            {
                Id = "1501501504",
                FullMapName = "drillcavern_cpr",
                FullMapNameAlt = "Drillcavern_CPR",
                BaseMap = "drillcavern",
                DayNight = "day",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/drillcavern_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1501501508",
                FullMapName = "drillcavern_domination",
                FullMapNameAlt = "Drillcavern_Domination",
                BaseMap = "drillcavern",
                DayNight = "day",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/drillcavern_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1501501510",
                FullMapName = "drillcavern_kc",
                FullMapNameAlt = "Drillcavern_KC",
                BaseMap = "drillcavern",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/drillcavern_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1501502501",
                FullMapName = "drillcavern_night_art_master",
                FullMapNameAlt = "Drillcavern_Night_Art_Master",
                BaseMap = "drillcavern_night",
                DayNight = "night",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/drillcavern_night_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1501502502",
                FullMapName = "drillcavern_night_botwave",
                FullMapNameAlt = "Drillcavern_Night_BotWave",
                BaseMap = "drillcavern_night",
                DayNight = "night",
                GameMode = "botwave",
                PicturePath = "/Assets/Maps/drillcavern_night_botwave.webp"
            },

            new LoadoutMap
            {
                Id = "1501502504",
                FullMapName = "drillcavern_night_cpr",
                FullMapNameAlt = "Drillcavern_Night_CPR",
                BaseMap = "drillcavern_night",
                DayNight = "night",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/drillcavern_night_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1501502508",
                FullMapName = "drillcavern_night_domination",
                FullMapNameAlt = "Drillcavern_Night_Domination",
                BaseMap = "drillcavern_night",
                DayNight = "night",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/drillcavern_night_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1501502510",
                FullMapName = "drillcavern_night_kc",
                FullMapNameAlt = "Drillcavern_Night_KC",
                BaseMap = "drillcavern_night",
                DayNight = "night",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/drillcavern_night_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1501502516",
                FullMapName = "drillcavern_night_rr",
                FullMapNameAlt = "Drillcavern_Night_RR",
                BaseMap = "drillcavern_night",
                DayNight = "night",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/drillcavern_night_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1501501516",
                FullMapName = "drillcavern_rr",
                FullMapNameAlt = "Drillcavern_RR",
                BaseMap = "drillcavern",
                DayNight = "day",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/drillcavern_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1503501501",
                FullMapName = "fath_705_art_master",
                FullMapNameAlt = "Fath_705_Art_Master",
                BaseMap = "fath_705",
                DayNight = "day",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/fath_705_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1503501502",
                FullMapName = "fath_705_botwave",
                FullMapNameAlt = "Fath_705_BotWave",
                BaseMap = "fath_705",
                DayNight = "day",
                GameMode = "botwave",
                PicturePath = "/Assets/Maps/fath_705_botwave.webp"
            },

            new LoadoutMap
            {
                Id = "1503501504",
                FullMapName = "fath_705_cpr",
                FullMapNameAlt = "Fath_705_CPR",
                BaseMap = "fath_705",
                DayNight = "day",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/fath_705_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1503501508",
                FullMapName = "fath_705_domination",
                FullMapNameAlt = "Fath_705_Domination",
                BaseMap = "fath_705",
                DayNight = "day",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/fath_705_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1503501510",
                FullMapName = "fath_705_kc",
                FullMapNameAlt = "Fath_705_KC",
                BaseMap = "fath_705",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/fath_705_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1503501516",
                FullMapName = "fath_705_rr",
                FullMapNameAlt = "Fath_705_RR",
                BaseMap = "fath_705",
                DayNight = "day",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/fath_705_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1504501501",
                FullMapName = "fissure_art_master",
                FullMapNameAlt = "Fissure_Art_Master",
                BaseMap = "fissure",
                DayNight = "day",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/fissure_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1504501502",
                FullMapName = "fissure_botwave",
                FullMapNameAlt = "Fissure_Botwave",
                BaseMap = "fissure",
                DayNight = "day",
                GameMode = "botwave",
                PicturePath = "/Assets/Maps/fissure_botwave.webp"
            },

            new LoadoutMap
            {
                Id = "1504501504",
                FullMapName = "fissure_cpr",
                FullMapNameAlt = "Fissure_CPR",
                BaseMap = "fissure",
                DayNight = "day",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/fissure_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1504501506",
                FullMapName = "fissure_ctf",
                FullMapNameAlt = "Fissure_CTF",
                BaseMap = "fissure",
                DayNight = "day",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/fissure_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1504501508",
                FullMapName = "fissure_domination",
                FullMapNameAlt = "Fissure_Domination",
                BaseMap = "fissure",
                DayNight = "day",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/fissure_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1504501509",
                FullMapName = "fissure_geo",
                FullMapNameAlt = "Fissure_Geo",
                BaseMap = "fissure",
                DayNight = "day",
                GameMode = "geo",
                PicturePath = "/Assets/Maps/fissure_geo.webp"
            },

            new LoadoutMap
            {
                Id = "1504501510",
                FullMapName = "fissure_kc",
                FullMapNameAlt = "Fissure_KC",
                BaseMap = "fissure",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/fissure_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1504502501",
                FullMapName = "fissure_night_art_master",
                FullMapNameAlt = "Fissure_Night_Art_Master",
                BaseMap = "fissurenight",
                DayNight = "night",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/fissure_night_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1504501516",
                FullMapName = "fissure_rr",
                FullMapNameAlt = "Fissure_RR",
                BaseMap = "fissure",
                DayNight = "day",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/fissure_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1504502502",
                FullMapName = "fissurenight_botwave",
                FullMapNameAlt = "FissureNight_BotWave",
                BaseMap = "fissurenight",
                DayNight = "night",
                GameMode = "botwave",
                PicturePath = "/Assets/Maps/fissurenight_botwave.webp"
            },

            new LoadoutMap
            {
                Id = "1504502504",
                FullMapName = "fissurenight_cpr",
                FullMapNameAlt = "FissureNight_CPR",
                BaseMap = "fissurenight",
                DayNight = "night",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/fissurenight_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1504502506",
                FullMapName = "fissurenight_ctf",
                FullMapNameAlt = "FissureNight_CTF",
                BaseMap = "fissurenight",
                DayNight = "night",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/fissurenight_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1504502508",
                FullMapName = "fissurenight_domination",
                FullMapNameAlt = "FissureNight_Domination",
                BaseMap = "fissurenight",
                DayNight = "night",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/fissurenight_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1504502510",
                FullMapName = "fissurenight_kc",
                FullMapNameAlt = "FissureNight_KC",
                BaseMap = "fissurenight",
                DayNight = "night",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/fissurenight_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1504502516",
                FullMapName = "fissurenight_rr",
                FullMapNameAlt = "FissureNight_RR",
                BaseMap = "fissurenight",
                DayNight = "night",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/fissurenight_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1505501502",
                FullMapName = "gliese_581_botwave",
                FullMapNameAlt = "Gliese_581_BotWave",
                BaseMap = "gliese_581",
                DayNight = "day",
                GameMode = "botwave",
                PicturePath = "/Assets/Maps/gliese_581_botwave.webp"
            },

            new LoadoutMap
            {
                Id = "1505501504",
                FullMapName = "gliese_581_cpr",
                FullMapNameAlt = "Gliese_581_CPR",
                BaseMap = "gliese_581",
                DayNight = "day",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/gliese_581_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1505501506",
                FullMapName = "gliese_581_ctf",
                FullMapNameAlt = "Gliese_581_CTF",
                BaseMap = "gliese_581",
                DayNight = "day",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/gliese_581_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1505501508",
                FullMapName = "gliese_581_domination",
                FullMapNameAlt = "Gliese_581_Domination",
                BaseMap = "gliese_581",
                DayNight = "day",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/gliese_581_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1505501510",
                FullMapName = "gliese_581_kc",
                FullMapNameAlt = "Gliese_581_KC",
                BaseMap = "gliese_581",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/gliese_581_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1518501513",
                FullMapName = "gliese_581_mu",
                FullMapNameAlt = "Gliese_581_MU",
                BaseMap = "trailerpark_agt",
                DayNight = "day",
                GameMode = "mu",
                PicturePath = "/Assets/Maps/gliese_581_mu.webp"
            },

            new LoadoutMap
            {
                Id = "1505502502",
                FullMapName = "gliese_581_night_botwave",
                FullMapNameAlt = "Gliese_581_Night_Botwave",
                BaseMap = "gliese_581_night",
                DayNight = "night",
                GameMode = "botwave",
                PicturePath = "/Assets/Maps/gliese_581_night_botwave.webp"
            },

            new LoadoutMap
            {
                Id = "1505502504",
                FullMapName = "gliese_581_night_cpr",
                FullMapNameAlt = "Gliese_581_Night_CPR",
                BaseMap = "gliese_581_night",
                DayNight = "night",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/gliese_581_night_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1505502506",
                FullMapName = "gliese_581_night_ctf",
                FullMapNameAlt = "Gliese_581_Night_CTF",
                BaseMap = "gliese_581_night",
                DayNight = "night",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/gliese_581_night_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1505502508",
                FullMapName = "gliese_581_night_domination",
                FullMapNameAlt = "Gliese_581_Night_Domination",
                BaseMap = "gliese_581_night",
                DayNight = "night",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/gliese_581_night_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1505502510",
                FullMapName = "gliese_581_night_kc",
                FullMapNameAlt = "Gliese_581_Night_KC",
                BaseMap = "gliese_581_night",
                DayNight = "night",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/gliese_581_night_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1505502516",
                FullMapName = "gliese_581_night_rr",
                FullMapNameAlt = "Gliese_581_Night_RR",
                BaseMap = "gliese_581_night",
                DayNight = "night",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/gliese_581_night_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1505501516",
                FullMapName = "gliese_581_rr",
                FullMapNameAlt = "Gliese_581_RR",
                BaseMap = "gliese_581",
                DayNight = "day",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/gliese_581_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1506501514",
                FullMapName = "greenroom",
                FullMapNameAlt = "Greenroom",
                BaseMap = "greenroom",
                DayNight = "day",
                GameMode = "none",
                PicturePath = "/Assets/Maps/greenroom.webp"
            },

            new LoadoutMap
            {
                Id = "1506501506",
                FullMapName = "greenroom_ctf",
                FullMapNameAlt = "Greenroom_CTF",
                BaseMap = "greenroom",
                DayNight = "day",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/greenroom_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1506501519",
                FullMapName = "greenroom_tdm",
                FullMapNameAlt = "Greenroom_TDM",
                BaseMap = "greenroom",
                DayNight = "day",
                GameMode = "tdm",
                PicturePath = "/Assets/Maps/greenroom_tdm.webp"
            },

            new LoadoutMap
            {
                Id = "1507501503",
                FullMapName = "level_three_botwaves",
                FullMapNameAlt = "Level_Three_Botwaves",
                BaseMap = "level_three",
                DayNight = "day",
                GameMode = "botwaves",
                PicturePath = "/Assets/Maps/level_three_botwaves.webp"
            },

            new LoadoutMap
            {
                Id = "1507501504",
                FullMapName = "level_three_cpr",
                FullMapNameAlt = "Level_Three_CPR",
                BaseMap = "level_three",
                DayNight = "day",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/level_three_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1507501508",
                FullMapName = "level_three_domination",
                FullMapNameAlt = "Level_Three_Domination",
                BaseMap = "level_three",
                DayNight = "day",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/level_three_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1507501510",
                FullMapName = "level_three_kc",
                FullMapNameAlt = "Level_Three_KC",
                BaseMap = "level_three",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/level_three_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1507501516",
                FullMapName = "level_three_rr",
                FullMapNameAlt = "Level_Three_RR",
                BaseMap = "level_three",
                DayNight = "day",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/level_three_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1508501514",
                FullMapName = "locomotiongym",
                FullMapNameAlt = "LocomotionGym",
                BaseMap = "locomotiongym",
                DayNight = "day",
                GameMode = "none",
                PicturePath = "/Assets/Maps/locomotiongym.webp"
            },

            new LoadoutMap
            {
                Id = "1508501506",
                FullMapName = "locomotiongym_ctf",
                FullMapNameAlt = "LocomotionGym_CTF",
                BaseMap = "locomotiongym",
                DayNight = "day",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/locomotiongym_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1508501508",
                FullMapName = "locomotiongym_domination",
                FullMapNameAlt = "LocomotionGym_Domination",
                BaseMap = "locomotiongym",
                DayNight = "day",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/locomotiongym_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1508501512",
                FullMapName = "locomotiongym_mashup",
                FullMapNameAlt = "LocomotionGym_MashUp",
                BaseMap = "locomotiongym",
                DayNight = "day",
                GameMode = "mashup",
                PicturePath = "/Assets/Maps/locomotiongym_mashup.webp"
            },

            new LoadoutMap
            {
                Id = "1509501514",
                FullMapName = "projectx",
                FullMapNameAlt = "ProjectX",
                BaseMap = "projectx",
                DayNight = "day",
                GameMode = "none",
                PicturePath = "/Assets/Maps/projectx.webp"
            },

            new LoadoutMap
            {
                Id = "1509501518",
                FullMapName = "projectx_tc",
                FullMapNameAlt = "ProjectX_TC",
                BaseMap = "projectx",
                DayNight = "day",
                GameMode = "tc",
                PicturePath = "/Assets/Maps/projectx_tc.webp"
            },

            new LoadoutMap
            {
                Id = "1510501501", // Identical to shattered_no_jack_art_master, no difference found, double ID stays
                FullMapName = "shattered_art_master",
                FullMapNameAlt = "Shattered_Art_Master",
                BaseMap = "shattered",
                DayNight = "day",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/shattered_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1510501502",
                FullMapName = "shattered_botwave",
                FullMapNameAlt = "Shattered_BotWave",
                BaseMap = "shattered",
                DayNight = "day",
                GameMode = "botwave",
                PicturePath = "/Assets/Maps/shattered_botwave.webp"
            },

            new LoadoutMap
            {
                Id = "1510501504",
                FullMapName = "shattered_cpr",
                FullMapNameAlt = "Shattered_CPR",
                BaseMap = "shattered",
                DayNight = "day",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/shattered_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1510501506",
                FullMapName = "shattered_ctf",
                FullMapNameAlt = "Shattered_CTF",
                BaseMap = "shattered",
                DayNight = "day",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/shattered_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1510501508",
                FullMapName = "shattered_domination",
                FullMapNameAlt = "Shattered_Domination",
                BaseMap = "shattered",
                DayNight = "day",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/shattered_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1510501510",
                FullMapName = "shattered_kc",
                FullMapNameAlt = "Shattered_KC",
                BaseMap = "shattered",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/shattered_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1510501501", // Identical to shattered_art_master, no difference found, double ID stays
                FullMapName = "shattered_no_jack_art_master",
                FullMapNameAlt = "Shattered_No_Jack_Art_Master",
                BaseMap = "shattered",
                DayNight = "day",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/shattered_no_jack_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1510501516",
                FullMapName = "shattered_rr",
                FullMapNameAlt = "Shattered_RR",
                BaseMap = "shattered",
                DayNight = "day",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/shattered_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1511501517",
                FullMapName = "shooting_gallery_solo",
                FullMapNameAlt = "Shooting_Gallery_Solo",
                BaseMap = "shooting_gallery_solo",
                DayNight = "day",
                GameMode = "solo",
                PicturePath = "/Assets/Maps/shooting_gallery_solo.webp"
            },

            new LoadoutMap
            {
                Id = "1512501501",
                FullMapName = "spires_art_master",
                FullMapNameAlt = "Spires_Art_Master",
                BaseMap = "spires",
                DayNight = "day",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/spires_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1512501502",
                FullMapName = "spires_botwave",
                FullMapNameAlt = "Spires_BotWave",
                BaseMap = "spires",
                DayNight = "day",
                GameMode = "botwave",
                PicturePath = "/Assets/Maps/spires_botwave.webp"
            },

            new LoadoutMap
            {
                Id = "1512501504",
                FullMapName = "spires_cpr",
                FullMapNameAlt = "Spires_CPR",
                BaseMap = "spires",
                DayNight = "day",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/spires_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1512501506",
                FullMapName = "spires_ctf",
                FullMapNameAlt = "Spires_CTF",
                BaseMap = "spires",
                DayNight = "day",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/spires_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1512501508",
                FullMapName = "spires_domination",
                FullMapNameAlt = "Spires_Domination",
                BaseMap = "spires",
                DayNight = "day",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/spires_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1512501510",
                FullMapName = "spires_kc",
                FullMapNameAlt = "Spires_KC",
                BaseMap = "spires",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/spires_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1512501516",
                FullMapName = "spires_rr",
                FullMapNameAlt = "Spires_RR",
                BaseMap = "spires",
                DayNight = "day",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/spires_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1513501514",
                FullMapName = "sploded_blueroom",
                FullMapNameAlt = "Sploded_BlueRoom",
                BaseMap = "sploded",
                DayNight = "day",
                GameMode = "none",
                PicturePath = "/Assets/Maps/sploded_blueroom.webp"
            },

            new LoadoutMap
            {
                Id = "1513501506",
                FullMapName = "sploded_ctf",
                FullMapNameAlt = "Sploded_CTF",
                BaseMap = "sploded",
                DayNight = "day",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/sploded_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1513501510",
                FullMapName = "sploded_kc",
                FullMapNameAlt = "Sploded_KC",
                BaseMap = "sploded",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/sploded_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1514501520",
                FullMapName = "test_territorycontrol",
                FullMapNameAlt = "Test_TerritoryControl",
                BaseMap = "test_territorycontrol",
                DayNight = "day",
                GameMode = "territorycontrol",
                PicturePath = "/Assets/Maps/test_territorycontrol.webp"
            },

            new LoadoutMap
            {
                Id = "1515501514",
                FullMapName = "thefreezer",
                FullMapNameAlt = "TheFreezer",
                BaseMap = "thefreezer",
                DayNight = "day",
                GameMode = "none",
                PicturePath = "/Assets/Maps/thefreezer.webp"
            },

            new LoadoutMap
            {
                Id = "1515501503",
                FullMapName = "thefreezer_botwaves",
                FullMapNameAlt = "TheFreezer_BotWaves",
                BaseMap = "thefreezer",
                DayNight = "day",
                GameMode = "botwaves",
                PicturePath = "/Assets/Maps/thefreezer_botwaves.webp"
            },

            new LoadoutMap
            {
                Id = "1515501507",
                FullMapName = "thefreezer_ctp",
                FullMapNameAlt = "TheFreezer_CTP",
                BaseMap = "thefreezer",
                DayNight = "day",
                GameMode = "ctp",
                PicturePath = "/Assets/Maps/thefreezer_ctp.webp"
            },

            new LoadoutMap
            {
                Id = "1515501510",
                FullMapName = "thefreezer_kc",
                FullMapNameAlt = "TheFreezer_KC",
                BaseMap = "thefreezer",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/thefreezer_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1515501511",
                FullMapName = "thefreezer_kc_bots",
                FullMapNameAlt = "TheFreezer_KC_Bots",
                BaseMap = "thefreezer",
                DayNight = "day",
                GameMode = "kc_bots",
                PicturePath = "/Assets/Maps/thefreezer_kc_bots.webp"
            },

            new LoadoutMap
            {
                Id = "1515501519",
                FullMapName = "thefreezer_tdm",
                FullMapNameAlt = "TheFreezer_TDM",
                BaseMap = "thefreezer",
                DayNight = "day",
                GameMode = "tdm",
                PicturePath = "/Assets/Maps/thefreezer_tdm.webp"
            },

            new LoadoutMap
            {
                Id = "1516501515",
                FullMapName = "thepit_pj",
                FullMapNameAlt = "ThePit_PJ",
                BaseMap = "thepit_pj",
                DayNight = "day",
                GameMode = "pj",
                PicturePath = "/Assets/Maps/thepit_pj.webp"
            },

            new LoadoutMap
            {
                Id = "1517501504",
                FullMapName = "tower_cpr",
                FullMapNameAlt = "Tower_CPR",
                BaseMap = "tower",
                DayNight = "day",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/tower_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1517501506",
                FullMapName = "tower_ctf",
                FullMapNameAlt = "Tower_CTF",
                BaseMap = "tower",
                DayNight = "day",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/tower_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1517501508",
                FullMapName = "tower_domination",
                FullMapNameAlt = "Tower_Domination",
                BaseMap = "tower",
                DayNight = "day",
                GameMode = "domination",
                PicturePath = "/Assets/Maps/tower_domination.webp"
            },

            new LoadoutMap
            {
                Id = "1517501510",
                FullMapName = "tower_kc",
                FullMapNameAlt = "Tower_KC",
                BaseMap = "tower",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/tower_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1517501516",
                FullMapName = "tower_rr",
                FullMapNameAlt = "Tower_RR",
                BaseMap = "tower",
                DayNight = "day",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/tower_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1518501501",
                FullMapName = "trailerpark_agt_art_master",
                FullMapNameAlt = "Trailerpark_Agt_Art_Master",
                BaseMap = "trailerpark_agt",
                DayNight = "day",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/trailerpark_agt_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1505501501",
                FullMapName = "trailerpark_art_master",
                FullMapNameAlt = "Trailerpark_Art_Master",
                BaseMap = "gliese_581",
                DayNight = "day",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/trailerpark_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1505502501",
                FullMapName = "trailerpark_night_art_master",
                FullMapNameAlt = "Trailerpark_Night_Art_Master",
                BaseMap = "gliese_581_night",
                DayNight = "night",
                GameMode = "art_master",
                PicturePath = "/Assets/Maps/trailerpark_night_art_master.webp"
            },

            new LoadoutMap
            {
                Id = "1519501514",
                FullMapName = "truckstop2",
                FullMapNameAlt = "Truckstop2",
                BaseMap = "truckstop2",
                DayNight = "day",
                GameMode = "none",
                PicturePath = "/Assets/Maps/truckstop2.webp"
            },

            new LoadoutMap
            {
                Id = "1519501504",
                FullMapName = "truckstop2_cpr",
                FullMapNameAlt = "Truckstop2_CPR",
                BaseMap = "truckstop2",
                DayNight = "day",
                GameMode = "cpr",
                PicturePath = "/Assets/Maps/truckstop2_cpr.webp"
            },

            new LoadoutMap
            {
                Id = "1519501510",
                FullMapName = "truckstop2_kc",
                FullMapNameAlt = "Truckstop2_KC",
                BaseMap = "truckstop2",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/truckstop2_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1519501516",
                FullMapName = "truckstop2_rr",
                FullMapNameAlt = "Truckstop2_RR",
                BaseMap = "truckstop2",
                DayNight = "day",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/truckstop2_rr.webp"
            },

            new LoadoutMap
            {
                Id = "1520501514",
                FullMapName = "two_port",
                FullMapNameAlt = "Two_Port",
                BaseMap = "two_port",
                DayNight = "day",
                GameMode = "none",
                PicturePath = "/Assets/Maps/two_port.webp"
            },

            new LoadoutMap
            {
                Id = "1520501506",
                FullMapName = "two_port_ctf",
                FullMapNameAlt = "Two_Port_CTF",
                BaseMap = "two_port",
                DayNight = "day",
                GameMode = "ctf",
                PicturePath = "/Assets/Maps/two_port_ctf.webp"
            },

            new LoadoutMap
            {
                Id = "1520501510",
                FullMapName = "two_port_kc",
                FullMapNameAlt = "Two_Port_KC",
                BaseMap = "two_port",
                DayNight = "day",
                GameMode = "kc",
                PicturePath = "/Assets/Maps/two_port_kc.webp"
            },

            new LoadoutMap
            {
                Id = "1520501516",
                FullMapName = "two_port_rr",
                FullMapNameAlt = "Two_Port_RR",
                BaseMap = "two_port",
                DayNight = "day",
                GameMode = "rr",
                PicturePath = "/Assets/Maps/two_port_rr.webp"
            }
        };
        #endregion
    }
}
