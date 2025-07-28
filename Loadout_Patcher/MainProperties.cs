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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Loadout_Patcher.ViewModels;
using static Loadout_Patcher.Map;

namespace Loadout_Patcher
{
    public class MainProperties
    {

        ///* Extended options which will be listed below the standard map options */
        //public static string[] ExtendedMapOptions = new string[8] { "Set a new starting map (Type it in)","Set a custom map (Type it in)","Set your favorite map (Type it in)",
        //        "Load custom map: ","Load favorite map [1]: ","Load favorite map [2]: ","Load favorite map [3]: ","Save the current map as ..." };

        public static int TotalSuffixes = Map.GetNumberOfMapSuffixesAndAliases();
        public static int TotalMaps = Map.GetNumberOfLoadoutMaps();
        public static int TotalBaseMaps = Map.GetNumberOfAllBaseMapsAndAliases();

        public static string ReadMemoryEndpointString = ""; // neutral naming (uberent, ues, or matchmaking)
        public static string? ReadMemoryUberentString;
        public static string ReadMemoryUesString = "";
        public static string ReadMemoryMatchmakingString = "";
        public static string ReadMemoryMapString = "";

        public static string NewEndpoint = "";
        public static string NewMap = "";
        public static string DefaultMap = "shooting_gallery_solo";
        //public static string? StartingMap = null;
        public static string? CustomMap = null;

        public static List<LoadoutMap>? FavoriteMaps;
        public static List<LoadoutMap>? MapBlacklist;
        public static List<LoadoutMap>? MapWhitelist;
        public static List<LoadoutMap>? MapQueueList;
        public static LoadoutMap StartingMap = new Map.LoadoutMap
        {
            Id = "1511501517",
            FullMapName = "shooting_gallery_solo",
            FullMapNameAlt = "Shooting_Gallery_Solo",
            BaseMap = "shooting_gallery_solo",
            DayNight = "day",
            GameMode = "solo",
            PicturePath = "/Assets/Maps/shooting_gallery_solo.webp"
        };

        public static string? MatchingAliasMap;
        public static string? MatchingAliasGameMode;

        public static bool BreakOnFreshStart = false;
        public static bool Patched = false;

        public static bool PatcherReset = false;
        
        public static void Reset()
        {
            ReadMemoryEndpointString = "";
            ReadMemoryUesString = "";
            ReadMemoryMatchmakingString = "";
            ReadMemoryMapString = "";
            NewEndpoint = "";
            NewMap = "";
            DefaultMap = "shooting_gallery_solo";
            CustomMap = null;
            BreakOnFreshStart = false;
            Patched = false;
            PatcherReset = true; // Not false!
            PatcherPageViewModel patcherPageViewModel = new PatcherPageViewModel(); // Triggers class constructor
        }
    }
}
