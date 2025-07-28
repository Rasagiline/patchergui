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
//using Avalonia.Animation; TODO: Outcomment not used usings
//using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using DynamicData;
//using DynamicData.Kernel;
//using FluentAvalonia.Core;
//using Loadout_Patcher.Views;
//using System;
using System.Collections;
//using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Data;
//using System.Reflection;
//using System.Reflection.Metadata.Ecma335;

namespace Loadout_Patcher.ViewModels;

/* 15 columns per data set is enough. The removal option of a favorite map, whitelisted map etc. appears after adding */
public partial class MapPageViewModel : ViewModelBase
{
    public MapPageViewModel()
    {
        if (Map.GetMapWithoutInteractions())
        {
            NoPickupsCheck = true;
        }
        if (GUI.ExcludePve)
        {
            NoBotsCheck = true;
        }
        if (GUI.BlacklistPreference)
        {
            MapBlacklistCheck = true;
        }
        if (GUI.WhitelistPreference)
        {
            MapWhitelistCheck = true;
        }
        if (Map.GetNumberOfLoadoutMaps() == 1)
        {
            NumberOfMapsText = "Select from 1 map"; // TODO: This needs to be updated for 0 maps!
        }
        else if (Map.GetNumberOfLoadoutMaps() == 0)
        {
            NumberOfMapsText = "There is no map to select! Go and remove a map filter.";
        }
        else
        {
            NumberOfMapsText = "Select from all " + Map.GetNumberOfLoadoutMaps().ToString() + " maps";
        }
        if (Map.GetNumberOfBotMaps() == 1)
        {
            NumberOfVersusBotsMapsText = "Select from 1 map that supports fighting versus Kroads or sentries"; // TODO: This needs to be updated for 0 maps!
        }
        else if (Map.GetNumberOfBotMaps() == 0)
        {
            NumberOfVersusBotsMapsText = "There is no bot map to select! Go and remove a map filter.";
        }
        else
        {
            NumberOfVersusBotsMapsText = "Select from " + Map.GetNumberOfBotMaps().ToString() + " maps that support fighting versus Kroads or sentries";
        }
        // Map queue: zero, singular, plural
        if (MainProperties.MapQueueList != null)
        {
            if (MainProperties.MapQueueList.Count == 1)
            {
                NumberOfMapQueueMapsText = "Your map queue currently has 1 entry";
            }
            else
            {
                NumberOfMapQueueMapsText = "Your map queue currently has " + MainProperties.MapQueueList.Count + " entries";
            }
        }
        else
        {
            NumberOfMapQueueMapsText = "Your map queue currently has 0 entries";
        }

        //NumberOfMapsText = Map.GetNumberOfLoadoutMaps().ToString();
        //NumberOfVersusBotsMapsText = Map.GetNumberOfBotMaps().ToString();
        Maps = new ObservableCollection<MapObservableObject>{};
        BotMaps = new ObservableCollection<MapObservableObject>{};
        MapQueueMaps = new ObservableCollection<MapObservableObject>{};

        // TODO: This whole section might be redundant since the Filter command is activated anyway
        foreach (Map.LoadoutMap map in Map.GetLoadoutMaps())
        {
            MapObservableObject convertedMap = new MapObservableObject
            {
                Id = map.Id,
                FullMapName = map.FullMapName,
                FullMapNameAlt = map.FullMapNameAlt,
                BaseMap = map.BaseMap,
                DayNight = map.DayNight,
                GameMode = map.GameMode,
                PicturePath = map.PicturePath,
                MapPatchText = "Patch now",
                FavoriteMapsText = "Add",
                MapBlacklistText = "Add",
                MapWhitelistText = "Add",
                MapQueueText = "Add",
                StartingMapText = "Add",
                MapQueuePosition = ""
            };
            Hashtable gameModeDefinitions = Map.GetMapSuffixesAndDefinitions();
            Hashtable gameModeTranslations = Map.GetMapSuffixesAndAliases();
            Hashtable mapNames = Map.GetBaseMapsAndAliases();
            Hashtable mapNamesNight = Map.GetBaseMapsAndAliasesNight();
            Hashtable mapDescriptions = Map.GetBaseMapsAndDefinitions();
            Hashtable mapDescriptionsNight = Map.GetBaseMapsAndDefinitionsNight();

            /* We handle hashtables to get more information of the maps */
            // "Map string: " + "Game mode description: " + "Game mode: " + "Map name: " + "Map description: "
            if (gameModeDefinitions.ContainsKey(map.GameMode) == true)
            {
                convertedMap.GameModeDefinition = (string)gameModeDefinitions[map.GameMode]!;
            }
            if (gameModeTranslations.ContainsKey(map.GameMode) == true)
            {
                convertedMap.GameModeTranslated = (string)gameModeTranslations[map.GameMode]!;
            }
            if (mapNamesNight.ContainsKey(map.BaseMap) == true)
            {
                convertedMap.MapName = (string)mapNamesNight[map.BaseMap]!;
            }
            else if (mapNames.ContainsKey(map.BaseMap) == true)
            {
                convertedMap.MapName = (string)mapNames[map.BaseMap]!;
            }
            if (mapNamesNight.ContainsKey(map.BaseMap) == true)
            {
                convertedMap.MapDescription = (string)mapDescriptionsNight[map.BaseMap]!;
            }
            else if (mapNames.ContainsKey(map.BaseMap) == true)
            {
                convertedMap.MapDescription = (string)mapDescriptions[map.BaseMap]!;
            }

            if (MainProperties.MapQueueList != null)
            {
                int i = 1;
                foreach (Map.LoadoutMap mapQueueMap in MainProperties.MapQueueList)
                {
                    if (mapQueueMap.FullMapName == convertedMap.FullMapName)
                    {                        
                        convertedMap.MapQueuePosition = i.ToString();
                    }
                    i++;
                }
            }
            // All hashtables:
            // baseMapsAndAliases
            // baseMapsAndDefinitions
            // baseMapsAndAliasesNight
            // baseMapsAndDefinitionsNight
            // mapSuffixesAndAliases
            // mapSuffixesAndDefinitions
            // fullMapsAndAliases

            /*
            if (!MapsAtNightCheck)
            {
                Maps.Add(convertedMap);
                if (convertedMap.GameMode == "botwave" || convertedMap.GameMode == "botwaves" || convertedMap.GameMode == "tc")
                {
                    BotMaps.Add(convertedMap);
                }
            }
            else
            {
                if (convertedMap.DayNight == "night")
                {
                    Maps.Add(convertedMap);
                    if (convertedMap.GameMode == "botwave" || convertedMap.GameMode == "botwaves" || convertedMap.GameMode == "tc")
                    {
                        BotMaps.Add(convertedMap);
                    }
                }
            }
            */
            Maps.Add(convertedMap);
            if (convertedMap.GameMode == "botwave" || convertedMap.GameMode == "botwaves" || convertedMap.GameMode == "tc")
            {
                BotMaps.Add(convertedMap);
            }
            if (MainProperties.MapQueueList != null)
            {
                foreach (Map.LoadoutMap mapQueueMap in MainProperties.MapQueueList)
                {
                    if (convertedMap.FullMapName == mapQueueMap.FullMapName)
                    {
                        MapQueueMaps.Add(convertedMap);
                    }
                }
            }
        }
        /* Some filters can be active from the beginning and, for example, some favorite maps can be in a list from the beginning */
        /* We run it after the foreach loop */
        Filter();

        //Maps = new ObservableCollection<Map.LoadoutMap>{ Map.GetLoadoutMaps() };
        //Maps.Add<Map.LoadoutMap>(Map.GetLoadoutMaps());

        //PicturePaths = new ObservableCollection<string>{};

        /*
        foreach (Map.LoadoutMap map in Map.GetLoadoutMaps())
        {
            new Map.LoadoutMap
            {
                Id = map.Id,
                FullMapName = map.FullMapName,
                FullMapNameAlt = map.FullMapNameAlt,
                BaseMap = map.BaseMap,
                DayNight = map.DayNight,
                GameMode = map.GameMode,
                PicturePath = map.PicturePath
            };
            //Maps.Add(map);
            //PicturePaths.Add(map.PicturePath);
        }
        */
    }

    [ObservableProperty]
    private bool _avoidDoubleFilter;

    [ObservableProperty]
    private string _numberOfMapsText;

    [ObservableProperty]
    private string _numberOfVersusBotsMapsText;

    [ObservableProperty]
    private string _numberOfMapQueueMapsText;

    private ObservableCollection<MapObservableObject>? _maps;

    public ObservableCollection<MapObservableObject> Maps
    {
        get { return _maps!; }
        set { SetProperty(ref _maps, value); }
    }

    private ObservableCollection<MapObservableObject>? _botMaps;

    public ObservableCollection<MapObservableObject> BotMaps
    {
        get { return _botMaps!; }
        set { SetProperty(ref _botMaps, value); }
    }

    private ObservableCollection<MapObservableObject>? _mapQueueMaps;

    public ObservableCollection<MapObservableObject> MapQueueMaps
    {
        get { return _mapQueueMaps!; }
        set { SetProperty(ref _mapQueueMaps, value); }
    }

    //[ObservableProperty]
    //private bool _mapPatchVisible = false;

    [ObservableProperty]
    private bool _resetActive = false;

    [ObservableProperty]
    private bool _fillQueueCheck = false;

    [ObservableProperty]
    private bool _loopQueueCheck = false;

    //

    [ObservableProperty]
    private bool _mapsAtNightCheck = false;

    [ObservableProperty]
    private bool _mapsAtDayCheck = false;

    [ObservableProperty]
    private bool _noPickupsCheck = false;

    [ObservableProperty]
    private bool _noBotsCheck = false;

    [ObservableProperty]
    private bool _randomMapCheck = false;

    [ObservableProperty]
    private bool _randomMapNightCheck = false;

    //

    [ObservableProperty]
    private bool _customMapsCheck = false;

    [ObservableProperty]
    private bool _favoriteMapsCheck = false;

    [ObservableProperty]
    private bool _mapBlacklistCheck = false;

    [ObservableProperty]
    private bool _mapWhitelistCheck = false;

    [ObservableProperty]
    private bool _mapQueueCheck = false;

    [ObservableProperty]
    private bool _startingMapCheck = false;

    //

    [ObservableProperty]
    private bool _commTowerCheck = false;

    [ObservableProperty]
    private bool _drillCavernBetaCheck = false;

    [ObservableProperty]
    private bool _drillCavernNightCheck = false;

    [ObservableProperty]
    private bool _drillCavernCheck = false;

    [ObservableProperty]
    private bool _fourPointsCheck = false;

    [ObservableProperty]
    private bool _fissureNightCheck = false;

    [ObservableProperty]
    private bool _fissureCheck = false;

    [ObservableProperty]
    private bool _trailerParkRankedCheck = false;

    [ObservableProperty]
    private bool _trailerParkNightCheck = false;

    [ObservableProperty]
    private bool _trailerParkCheck = false;

    [ObservableProperty]
    private bool _greenroomCheck = false;

    [ObservableProperty]
    private bool _theBreweryCheck = false;

    [ObservableProperty]
    private bool _locomotionGymCheck = false;

    [ObservableProperty]
    private bool _projectXCheck = false;

    [ObservableProperty]
    private bool _shatteredCheck = false;

    [ObservableProperty]
    private bool _shootingGalleryCheck = false;

    [ObservableProperty]
    private bool _spiresCheck = false;

    [ObservableProperty]
    private bool _splodedCheck = false;

    [ObservableProperty]
    private bool _testCheck = false;

    [ObservableProperty]
    private bool _theFreezerCheck = false;

    [ObservableProperty]
    private bool _thePitCheck = false;

    [ObservableProperty]
    private bool _truckStopCheck = false;

    [ObservableProperty]
    private bool _twoPortCheck = false;

    [ObservableProperty]
    private bool _devMapCheck = false;

    //

    [ObservableProperty]
    private bool _annihilationCheck = false;

    [ObservableProperty]
    private bool _blitzCheck = false;

    [ObservableProperty]
    private bool _deathsnatchCheck = false;

    [ObservableProperty]
    private bool _dominationCheck = false;

    [ObservableProperty]
    private bool _extractionCheck = false;

    [ObservableProperty]
    private bool _holdYourPoleCheck = false;

    [ObservableProperty]
    private bool _jackhammerCheck = false;

    [ObservableProperty]
    private bool _soloCheck = false;

    [ObservableProperty]
    private bool _artMasterCheck = false;

    [ObservableProperty]
    private bool _annihilationOtherCheck = false;

    [ObservableProperty]
    private bool _blitzOtherCheck = false;

    [ObservableProperty]
    private bool _deathsnatchOtherCheck = false;

    [ObservableProperty]
    private bool _extractionOtherCheck = false;

    [ObservableProperty]
    private bool _geoCheck = false;

    [ObservableProperty]
    private bool _noGameModeCheck = false;

    [ObservableProperty]
    private bool _unknownGameModeCheck = false;

    //

    // string mapPatchText = "Patch now", string favoriteMapsText = "Add"
    [RelayCommand]
    private void Filter()
    {
        /* We must use a task delay or else the program gets overused and suddenly crashes. */
        System.Threading.Thread.Sleep(10);

        /* Some filter options get saved */
        /* This operation must come before everything else to spot differences between checkbox and save file data */
        if (NoPickupsCheck == Map.GetMapWithoutInteractions() || MapBlacklistCheck != GUI.BlacklistPreference || MapWhitelistCheck != GUI.WhitelistPreference
            || NoBotsCheck != GUI.ExcludePve)
        {
            Map.SetMapWithInteractions(NoPickupsCheck);
            GUI.BlacklistPreference = MapBlacklistCheck;
            GUI.WhitelistPreference = MapWhitelistCheck;
            GUI.ExcludePve = NoBotsCheck;

            // Save file is being updated
            Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
            Filesave.SaveDataToFile(saveFile, true);
            Console.WriteLine("> One or more filters were saved.\n");
            Console.WriteLine("> Save file changed and saved.\n");
        }

        ResetActive = true; // If any checkbox is true, make resetActive true

        Maps = new ObservableCollection<MapObservableObject> { };
        BotMaps = new ObservableCollection<MapObservableObject> { };
        MapQueueMaps = new ObservableCollection<MapObservableObject> { };

        bool randomMapSet = false;
        Map.LoadoutMap randomMap = new Map.LoadoutMap();

        foreach (Map.LoadoutMap map in Map.GetLoadoutMaps())
        {
            MapObservableObject convertedMap = new MapObservableObject
            {
                Id = map.Id,
                FullMapName = map.FullMapName,
                FullMapNameAlt = map.FullMapNameAlt,
                BaseMap = map.BaseMap,
                DayNight = map.DayNight,
                GameMode = map.GameMode,
                PicturePath = map.PicturePath,
                MapPatchText = "Patch now",
                FavoriteMapsText = "Add",
                MapBlacklistText = "Add",
                MapWhitelistText = "Add",
                MapQueueText = "Add",
                StartingMapText = "Add",
                MapQueuePosition = ""
            };
            Hashtable gameModeDefinitions = Map.GetMapSuffixesAndDefinitions();
            Hashtable gameModeTranslations = Map.GetMapSuffixesAndAliases();
            Hashtable mapNames = Map.GetBaseMapsAndAliases();
            Hashtable mapNamesNight = Map.GetBaseMapsAndAliasesNight();
            Hashtable mapDescriptions = Map.GetBaseMapsAndDefinitions();
            Hashtable mapDescriptionsNight = Map.GetBaseMapsAndDefinitionsNight();

            /* We handle hashtables to get more information of the maps */
            /* "Map string: " + "Game mode description: " + "Game mode: " + "Map name: " + "Map description: " */
            if (gameModeDefinitions.ContainsKey(map.GameMode) == true)
            {
                convertedMap.GameModeDefinition = (string)gameModeDefinitions[map.GameMode]!;
            }
            if (gameModeDefinitions.ContainsKey(map.GameMode) == true)
            {
                convertedMap.GameModeTranslated = (string)gameModeTranslations[map.GameMode]!;
            }
            if (mapNamesNight.ContainsKey(map.BaseMap) == true)
            {
                convertedMap.MapName = (string)mapNamesNight[map.BaseMap]!;
            }
            else if (mapNames.ContainsKey(map.BaseMap) == true)
            {
                convertedMap.MapName = (string)mapNames[map.BaseMap]!;
            }
            if (mapNamesNight.ContainsKey(map.BaseMap) == true)
            {
                convertedMap.MapDescription = (string)mapDescriptionsNight[map.BaseMap]!;
            }
            else if (mapNames.ContainsKey(map.BaseMap) == true)
            {
                convertedMap.MapDescription = (string)mapDescriptions[map.BaseMap]!;
            }

            if (MainProperties.MapQueueList != null)
            {
                int i = 1;
                foreach (Map.LoadoutMap mapQueueMap in MainProperties.MapQueueList)
                {
                    if (mapQueueMap.FullMapName == convertedMap.FullMapName)
                    {
                        convertedMap.MapQueuePosition = i.ToString();
                    }
                    i++;
                }
            }

            // Multiple filter: Things don't get added, they can get removed, or simply use continue
            //bool filtered = false;

            if (MapsAtNightCheck)
            {
                if (convertedMap.DayNight != "night")
                {
                    continue;
                }
            }
            if (MapsAtDayCheck)
            {
                if (convertedMap.DayNight != "day")
                {
                    continue;
                }
            }
            // NoPickupsCheck lies above
            if (NoBotsCheck)
            {
                if (convertedMap.GameMode == "botwave" || convertedMap.GameMode == "botwaves" || convertedMap.GameMode == "tc")
                {
                    continue;
                }
            }
            if (RandomMapCheck)
            {
                if (!randomMapSet)
                {
                    // Only execute once during the loop
                    Map.SelectRandomMap(out randomMap);
                    randomMapSet = true;
                }
                if (convertedMap.FullMapName != randomMap.FullMapName)
                {
                    continue;
                    // Else: Add as below
                }
            }
            if (RandomMapNightCheck)
            {
                if (!randomMapSet)
                {
                    // Only execute once during the loop
                    Map.SelectRandomMapNight(out randomMap);
                    randomMapSet = true;
                }
                if (convertedMap.FullMapName != randomMap.FullMapName)
                {
                    continue;
                    // Else: Add as below
                }
            }
            // Save file checks get special treatment

            bool broke = false;

            if (MainProperties.FavoriteMaps != null)
            {
                if (FavoriteMapsCheck)
                {
                    foreach (Map.LoadoutMap favoriteMap in MainProperties.FavoriteMaps)
                    {
                        if (favoriteMap.FullMapName == convertedMap.FullMapName)
                        {
                            broke = true;
                            break;
                        }
                    }
                    if (!broke)
                    {
                        continue;
                    }
                }
            }

            if (MainProperties.MapBlacklist != null)
            {
                if (MapBlacklistCheck)
                {
                    foreach (Map.LoadoutMap blacklistedMap in MainProperties.MapBlacklist)
                    {
                        /* Here's the difference between blacklist and whitelist */
                        if (blacklistedMap.FullMapName == convertedMap.FullMapName)
                        {
                            broke = true;
                            break;
                        }
                    }
                    /* If we did break, we continue. That's how a blacklist gets treated! */
                    if (broke)
                    {
                        broke = false;
                        continue;
                    }
                }
            }
            if (MainProperties.MapWhitelist != null)
            {
                if (MapWhitelistCheck)
                {
                    foreach (Map.LoadoutMap whitelistedMap in MainProperties.MapWhitelist)
                    {
                        if (whitelistedMap.FullMapName == convertedMap.FullMapName)
                        {
                            broke = true;
                            break;
                        }
                    }
                    if (!broke)
                    {
                        continue;
                    }
                }
            }

            if (MainProperties.MapQueueList != null)
            {
                if (MapQueueCheck)
                {
                    foreach (Map.LoadoutMap mapQueueMap in MainProperties.MapQueueList)
                    {
                        if (mapQueueMap.FullMapName == convertedMap.FullMapName)
                        {
                            broke = true;
                            break;
                        }
                    }
                    if (!broke)
                    {
                        continue;
                    }
                }
            }

            if (StartingMapCheck)
            {
                if (MainProperties.StartingMap.FullMapName != convertedMap.FullMapName)
                {
                    continue;
                }
            }

            // CustomMaps is missing

            // End of save file checks
            if (CommTowerCheck)
            {
                if (convertedMap.BaseMap != "tower")
                {
                    continue;
                }
            }
            if (DrillCavernBetaCheck)
            {
                if (convertedMap.BaseMap != "drillcavern_beta_kc")
                {
                    continue;
                }
            }
            if (DrillCavernNightCheck)
            {
                if (convertedMap.BaseMap != "drillcavern_night")
                {
                    continue;
                }
            }
            if (DrillCavernCheck)
            {
                if (convertedMap.BaseMap != "drillcavern")
                {
                    continue;
                }
            }
            if (FourPointsCheck)
            {
                if (convertedMap.BaseMap != "fath_705")
                {
                    continue;
                }
            }
            if (FissureNightCheck)
            {
                if (convertedMap.BaseMap != "fissurenight")
                {
                    continue;
                }
            }
            if (FissureCheck)
            {
                if (convertedMap.BaseMap != "fissure")
                {
                    continue;
                }
            }
            if (TrailerParkRankedCheck)
            {
                if (convertedMap.BaseMap != "trailerpark_agt")
                {
                    continue;
                }
            }
            if (TrailerParkNightCheck)
            {
                if (convertedMap.BaseMap != "gliese_581_night")
                {
                    continue;
                }
            }
            if (TrailerParkCheck)
            {
                if (convertedMap.BaseMap != "gliese_581")
                {
                    continue;
                }
            }
            if (GreenroomCheck)
            {
                if (convertedMap.BaseMap != "greenroom")
                {
                    continue;
                }
            }
            if (TheBreweryCheck)
            {
                if (convertedMap.BaseMap != "level_three")
                {
                    continue;
                }
            }
            if (LocomotionGymCheck)
            {
                if (convertedMap.BaseMap != "locomotiongym")
                {
                    continue;
                }
            }
            if (ProjectXCheck)
            {
                if (convertedMap.BaseMap != "projectx")
                {
                    continue;
                }
            }
            if (ShatteredCheck)
            {
                if (convertedMap.BaseMap != "shattered")
                {
                    continue;
                }
            }
            if (ShootingGalleryCheck)
            {
                if (convertedMap.BaseMap != "shooting_gallery_solo")
                {
                    continue;
                }
            }
            if (SpiresCheck)
            {
                if (convertedMap.BaseMap != "spires")
                {
                    continue;
                }
            }
            if (SplodedCheck)
            {
                if (convertedMap.BaseMap != "sploded")
                {
                    continue;
                }
            }
            if (TestCheck)
            {
                if (convertedMap.BaseMap != "test_territorycontrol")
                {
                    continue;
                }
            }
            if (TheFreezerCheck)
            {
                if (convertedMap.BaseMap != "thefreezer")
                {
                    continue;
                }
            }
            if (ThePitCheck)
            {
                if (convertedMap.BaseMap != "thepit_pj")
                {
                    continue;
                }
            }
            if (TruckStopCheck)
            {
                if (convertedMap.BaseMap != "truckstop2")
                {
                    continue;
                }
            }
            if (TwoPortCheck)
            {
                if (convertedMap.BaseMap != "two_port")
                {
                    continue;
                }
            }
            if (DevMapCheck)
            {
                if (convertedMap.BaseMap != "greenroom" && convertedMap.BaseMap != "locomotiongym" && convertedMap.BaseMap != "projectx" &&
                    convertedMap.BaseMap != "test_territorycontrol" && convertedMap.BaseMap != "thefreezer" && convertedMap.BaseMap != "thepit_pj")
                {
                    continue;
                }
            }
            if (AnnihilationCheck)
            {
                /* Arguable if we let count the trailerpark agt art master */
                if (convertedMap.GameMode != "mu" && convertedMap.BaseMap != "trailerpark_agt")
                {
                    continue;
                }
            }
            if (BlitzCheck)
            {
                if (convertedMap.GameMode != "cpr")
                {
                    continue;
                }
            }
            if (DeathsnatchCheck)
            {
                if (convertedMap.GameMode != "kc")
                {
                    continue;
                }
            }
            if (DominationCheck)
            {
                if (convertedMap.GameMode != "domination")
                {
                    continue;
                }
            }
            if (ExtractionCheck)
            {
                if (convertedMap.GameMode != "rr")
                {
                    continue;
                }
            }
            if (HoldYourPoleCheck)
            {
                if (convertedMap.GameMode != "botwave" && convertedMap.GameMode != "botwaves")
                {
                    continue;
                }
            }
            if (JackhammerCheck)
            {
                if (convertedMap.GameMode != "ctf")
                {
                    continue;
                }
            }
            if (SoloCheck)
            {
                if (convertedMap.GameMode != "solo")
                {
                    continue;
                }
            }
            if (ArtMasterCheck)
            {
                if (convertedMap.GameMode != "art_master")
                {
                    continue;
                }
            }
            if (AnnihilationOtherCheck)
            {
                if (convertedMap.GameMode != "mashup")
                {
                    continue;
                }
            }
            if (BlitzOtherCheck)
            {
                if (convertedMap.GameMode != "cpr_bots")
                {
                    continue;
                }
            }
            if (DeathsnatchOtherCheck)
            {
                if (convertedMap.GameMode != "kc_bots" && convertedMap.GameMode != "tdm")
                {
                    continue;
                }
            }
            if (ExtractionOtherCheck)
            {
                if (convertedMap.GameMode != "ctp")
                {
                    continue;
                }
            }
            if (GeoCheck)
            {
                if (convertedMap.GameMode != "geo")
                {
                    continue;
                }
            }
            if (NoGameModeCheck)
            {
                if (convertedMap.GameMode != "none")
                {
                    continue;
                }
            }
            if (UnknownGameModeCheck)
            {
                if (convertedMap.GameMode != "pj" && convertedMap.GameMode != "tc" && convertedMap.GameMode != "territorycontrol")
                {
                    continue;
                }
            }


            /*
            Maps.Add(convertedMap);
            if (convertedMap.GameMode == "botwave" || convertedMap.GameMode == "botwaves" || convertedMap.GameMode == "tc")
            {
                BotMaps.Add(convertedMap);
            }
            */
            //if (!filtered)

            if (MainProperties.NewMap == convertedMap.FullMapName || MainProperties.NewMap == convertedMap.FullMapNameAlt)
            {
                convertedMap.MapPatchText = "Patched!";
            }

            /* We check if we have favorite maps in MainProperties, otherwise it will lead to an error */
            if (MainProperties.FavoriteMaps != null)
            {
                foreach (Map.LoadoutMap favoriteMap in MainProperties.FavoriteMaps)
                {
                    if (favoriteMap.FullMapName == convertedMap.FullMapName)
                    {
                        convertedMap.FavoriteMapsText = "Remove";
                    }
                }
            }

            /* We check if we have favorite maps in MainProperties, otherwise it will lead to an error */
            if (MainProperties.MapBlacklist != null)
            {
                foreach (Map.LoadoutMap blacklistedMap in MainProperties.MapBlacklist)
                {
                    if (blacklistedMap.FullMapName == convertedMap.FullMapName)
                    {
                        convertedMap.MapBlacklistText = "Remove";
                    }
                }
            }

            /* We check if we have favorite maps in MainProperties, otherwise it will lead to an error */
            if (MainProperties.MapWhitelist != null)
            {
                foreach (Map.LoadoutMap whitelistedMap in MainProperties.MapWhitelist)
                {
                    if (whitelistedMap.FullMapName == convertedMap.FullMapName)
                    {
                        convertedMap.MapWhitelistText = "Remove";
                    }
                }
            }

            /* We check if we have favorite maps in MainProperties, otherwise it will lead to an error */
            if (MainProperties.MapQueueList != null)
            {
                foreach (Map.LoadoutMap mapQueueMap in MainProperties.MapQueueList)
                {
                    if (mapQueueMap.FullMapName == convertedMap.FullMapName)
                    {
                        convertedMap.MapQueueText = "Remove";
                    }
                }
            }

            if (MainProperties.StartingMap.FullMapName == convertedMap.FullMapName || MainProperties.StartingMap.FullMapNameAlt == convertedMap.FullMapNameAlt)
            {
                convertedMap.StartingMapText = "Remove";
            }

            Maps.Add(convertedMap);
            if (convertedMap.GameMode == "botwave" || convertedMap.GameMode == "botwaves" || convertedMap.GameMode == "tc")
            {
                BotMaps.Add(convertedMap);
            }
            if (MainProperties.MapQueueList != null)
            {
                foreach (Map.LoadoutMap mapQueueMap in MainProperties.MapQueueList)
                {
                    if (convertedMap.FullMapName == mapQueueMap.FullMapName)
                    {
                        MapQueueMaps.Add(convertedMap);
                    }
                }
            }

            /*
            if (MapsAtNightCheck)
            {
                if (convertedMap.DayNight == "night")
                {
                    Maps.Add(convertedMap);
                    if (convertedMap.GameMode == "botwave" || convertedMap.GameMode == "botwaves" || convertedMap.GameMode == "tc")
                    {
                        BotMaps.Add(convertedMap);
                    }
                }
            }
            else
            {
                Maps.Add(convertedMap);
                if (convertedMap.GameMode == "botwave" || convertedMap.GameMode == "botwaves" || convertedMap.GameMode == "tc")
                {
                    BotMaps.Add(convertedMap);
                }
            }
            */
        }
        // Maps: singular, plural
        if (Maps.Count == 1)
        {
            NumberOfMapsText = "Select from 1 map"; // This needs to be updated!
        }
        else if (Maps.Count == 0)
        {
            NumberOfMapsText = "There is no map to select! Go and remove a map filter.";
        }
        else
        {
            NumberOfMapsText = "Select from all " + Maps.Count.ToString() + " maps";
        }
        // Versus bots maps: singular, plural
        if (BotMaps.Count == 1)
        {
            NumberOfVersusBotsMapsText = "Select from 1 map that supports fighting versus Kroads or sentries";
        }
        else if (BotMaps.Count == 0)
        {
            NumberOfVersusBotsMapsText = "There is no bot map to select! Go and remove a map filter.";
        }
        else
        {
            NumberOfVersusBotsMapsText = "Select from " + BotMaps.Count.ToString() + " maps that support fighting versus Kroads or sentries";
        }
        // Map queue: zero, singular, plural
        if (MainProperties.MapQueueList != null)
        {
            if (MainProperties.MapQueueList.Count == 1)
            {
                NumberOfMapQueueMapsText = "Your map queue currently has 1 entry"; // This needs to be updated!
            }
            else
            {
                NumberOfMapQueueMapsText = "Your map queue currently has " + MainProperties.MapQueueList.Count + " entries";
            }
        }
        else
        {
            NumberOfMapQueueMapsText = "Your map queue currently has 0 entries";
        }
    }

    [RelayCommand]
    private void ResetFilter()
    {
        //
        MapsAtNightCheck = false;
        MapsAtDayCheck = false;
        NoPickupsCheck = false;
        NoBotsCheck = false;
        RandomMapCheck = false;
        RandomMapNightCheck = false;
        //
        CustomMapsCheck = false;
        FavoriteMapsCheck = false;
        MapBlacklistCheck = false;
        MapWhitelistCheck = false;
        MapQueueCheck = false;
        StartingMapCheck = false;
        //
        CommTowerCheck = false;
        DrillCavernBetaCheck = false;
        DrillCavernNightCheck = false;
        DrillCavernCheck = false;
        FourPointsCheck = false;
        FissureNightCheck = false;
        FissureCheck = false;
        TrailerParkRankedCheck = false;
        TrailerParkNightCheck = false;
        TrailerParkCheck = false;
        GreenroomCheck = false;
        TheBreweryCheck = false;
        LocomotionGymCheck = false;
        ProjectXCheck = false;
        ShatteredCheck = false;
        ShootingGalleryCheck = false;
        SpiresCheck = false;
        SplodedCheck = false;
        TestCheck = false;
        TheFreezerCheck = false;
        ThePitCheck = false;
        TruckStopCheck = false;
        TwoPortCheck = false;
        DevMapCheck = false;
        //
        AnnihilationCheck = false;
        BlitzCheck = false;
        DeathsnatchCheck = false;
        DominationCheck = false;
        ExtractionCheck = false;
        HoldYourPoleCheck = false;
        JackhammerCheck = false;
        SoloCheck = false;
        ArtMasterCheck = false;
        AnnihilationOtherCheck = false;
        BlitzOtherCheck = false;
        DeathsnatchOtherCheck = false;
        ExtractionOtherCheck = false;
        GeoCheck = false;
        NoGameModeCheck = false;
        UnknownGameModeCheck = false;
        //
        Filter(); // This must come before ResetActive = false

        ResetActive = false;
    }

    [RelayCommand]
    private void PatchMap(string mapString)
    {
        try
        {
            /* We must use a task delay or else the program gets overused and suddenly crashes. */
            System.Threading.Thread.Sleep(30);

            /* We only patch the map if patching for game access/simple patching was completed */
            /* We can also repatch a patched map! This is important for changing from Name to AltName */
            if (ProcessHandling.LoadoutProcess != null)
            {
                if (ProcessHandling.LoadoutProcess.HasExited)
                {
                    Console.WriteLine("> Loadout was closed. Please reopen the game!\n");
                }
                else
                {
                    MainProperties.NewMap = mapString;
                    Map.LoadoutMap mapStruct = new Map.LoadoutMap();

                    foreach (Map.LoadoutMap map in Map.GetLoadoutMaps())
                    {
                        if (MainProperties.NewMap == map.FullMapNameAlt)
                        {
                            mapStruct = map;
                        }
                    }

                    if (!NoPickupsCheck)
                    {
                        /* If we want to have pickups in our map, we change the string to lower case */
                        if (mapStruct.FullMapName != null)
                        {
                            MainProperties.NewMap = mapStruct.FullMapName;
                        }
                        else
                        {
                            MainProperties.NewMap = mapString.ToLower();
                        }
                    }

                    /* We fetch aliases from existing hashtables. */
                    MainProperties.MatchingAliasMap = Map.FetchMatchingAliasMap(mapStruct.BaseMap!);
                    MainProperties.MatchingAliasGameMode = Map.FetchMatchingAliasGameMode(mapStruct.GameMode!);

                    /*
                    if (!NoPickupsCheck)
                    {
                        // If we want to have pickups in our map, we change the string to lower case
                        mapString = mapString.ToLower();
                    }
                    */

                    /* patching [readMemoryMapString] */
                    MainProperties.ReadMemoryMapString = ProcessMemory.OverwriteStringAtOffset(ProcessHandling.LoadoutProcess, ProcessMemory.MapAddress,
                    MainProperties.ReadMemoryMapString, MainProperties.NewMap, true);
                    if (ProcessMemory.GetLastErrorOfProcessMemory())
                    {
                        MainProperties.Patched = false;
                        /* We reset the simple patcher */
                        MainProperties.Reset();
                    }
                    Console.WriteLine("-----------------------------> Map patching done! <-----------------------------\n");
                    /* We play a random success sound, a melee hit sound, from Axl, Helga or T-Bone if enabled */
                    Sound.PlaySuccessSoundsHitRandomly();

                    Console.WriteLine("-> [= Complete] Map selected: {0}", MainProperties.NewMap);
                    if (MainProperties.MatchingAliasMap != "")
                    {
                        Console.WriteLine("-> [= Complete] Map known as: {0}", MainProperties.MatchingAliasMap);
                    }
                    if (MainProperties.MatchingAliasGameMode != "")
                    {
                        Console.WriteLine("-> [= Complete] Game mode is: {0}", MainProperties.MatchingAliasGameMode);
                    }
                    Console.WriteLine();

                    /* 1. If we press patch map in the map queue tab or generally, the map queue map will be removed as queue map */
                    /* 2. If loop queue is turned on, the map will find itself at the bottom */
                    /* Only issue: the order/sorting needs to be controlled */
                    /* This section must come before Filter() and after everything else */
                    if (MainProperties.MapQueueList != null)
                    {
                        foreach (Map.LoadoutMap mapQueueMap in MainProperties.MapQueueList)
                        {
                            if (mapString == mapQueueMap.FullMapName || mapString == mapQueueMap.FullMapNameAlt)
                            {
                                /* We abort the filter in this method because we get back here and filter */
                                AvoidDoubleFilter = true;
                                AddOrRemoveMapQueue(mapString);
                                break;
                            }
                        }
                    }
                    /* Change button, so instead of "Patch now" it will display "Patched!". This is done in Filter() */
                    Filter();

                    /* We focus on the Loadout process since we have patched a map */
                    /* Requires user32.dll which allows only Windows users to focus on the game */
                    if (OperatingSystem.IsWindows())
                    {
                        if (ProcessHandling.LoadoutProcess != null)
                        {
                            if (!ProcessHandling.LoadoutProcess.HasExited)
                            {
                                if (ProcessHandling.LoadoutProcess.Responding)
                                {
                                    ProcessHandling.SetForeground();
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (NullReferenceException nullEx)
        {
            Console.WriteLine($"> Exception thrown in the map patching tab!");
            Console.WriteLine($"> Object reference not set to an instance of an object: '{nullEx}'");
            Console.WriteLine($"> Try again!");
        }
    }

    [RelayCommand]
    private void AddOrRemoveFavoriteMaps(string mapString)
    {
        try
        {
            /* We must use a task delay or else the program gets overused and suddenly crashes. */
            System.Threading.Thread.Sleep(30);

            MainProperties.FavoriteMaps ??= new List<Map.LoadoutMap>();

            /* Unlike map patching, adding a map to favorite maps doesn't depend on simple patching. It can be done without it. */

            if (!NoPickupsCheck)
            {
                /* If we want to have pickups in our map, we change the string to lower case */
                mapString = mapString.ToLower();
            }

            /* The foreach loop needs to come after the query */
            foreach (Map.LoadoutMap map in Map.GetLoadoutMaps())
            {
                /* FullMapNameAlt is our command parameter but can be converted to lower case before. It will be dealt with later. */
                if (mapString == map.FullMapNameAlt || mapString == map.FullMapName)
                {
                    if (MainProperties.FavoriteMaps.Contains(map))
                    {
                        /* If we try to add a map to favorite maps but it's already in favorite maps, we realize we want to remove it! */
                        MainProperties.FavoriteMaps.Remove(map);
                        Console.WriteLine("> Favorite map removed.\n");
                        break;
                    }
                    else
                    {
                        MainProperties.FavoriteMaps.Add(map);
                        Console.WriteLine("> Favorite map added.\n");
                        break;
                    }
                }
            }

            /* We share our list of favorite maps with the Map class, so it can be saved */
            Map.SetFavoriteMaps(MainProperties.FavoriteMaps);

            // Add terminal text for adding a favorite map, copy code, edit

            // Save file is being updated
            Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
            Filesave.SaveDataToFile(saveFile, true);

            Console.WriteLine("> Save file changed and saved.\n");

            /*
            if (Filesave.SaveDataIntoFile(MainProperties.FavoriteMaps, Filesave.directorySeparator, Filesave.path1!, Filesave.path2!))
            {
                Console.WriteLine(Filesave.saveSuccess);
            }
            else
            {
                Console.WriteLine("{0}favorite map.", Filesave.saveError);
            }
            Console.WriteLine();
            */

            /* Change button, so instead of "Add" it will display "Remove". This is done in Filter(). The list is being refreshed. */
            Filter();
        }
        catch (NullReferenceException nullEx)
        {
            Console.WriteLine($"> Exception thrown in the map patching tab!");
            Console.WriteLine($"> Object reference not set to an instance of an object: '{nullEx}'");
            Console.WriteLine($"> Try again!");
        }
    }

    // Could rename to AddOrRemoveBlacklistMap
    [RelayCommand]
    private void AddOrRemoveMapBlacklist(string mapString)
    {
        try
        {
            /* We must use a task delay or else the program gets overused and suddenly crashes. */
            System.Threading.Thread.Sleep(30);

            // TODO: Make this whole section a function!
            MainProperties.MapBlacklist ??= new List<Map.LoadoutMap>();

            /* Unlike map patching, adding a map to favorite maps doesn't depend on simple patching. It can be done without it. */

            if (!NoPickupsCheck)
            {
                /* If we want to have pickups in our map, we change the string to lower case */
                mapString = mapString.ToLower();
            }

            /* The foreach loop needs to come after the query */
            foreach (Map.LoadoutMap map in Map.GetLoadoutMaps())
            {
                /* FullMapNameAlt is our command parameter but can be converted to lower case before. It will be dealt with later. */
                if (mapString == map.FullMapNameAlt || mapString == map.FullMapName)
                {
                    if (MainProperties.MapBlacklist.Contains(map))
                    {
                        /* If we try to add a map to favorite maps but it's already in favorite maps, we realize we want to remove it! */
                        MainProperties.MapBlacklist.Remove(map);
                        Console.WriteLine("> Blacklisted map removed.\n");
                        break;
                    }
                    else
                    {
                        MainProperties.MapBlacklist.Add(map);
                        Console.WriteLine("> Map added to blacklist.\n");
                        break;
                    }
                }
            }

            /* We share our list of favorite maps with the Map class, so it can be saved */
            Map.SetMapBlacklist(MainProperties.MapBlacklist);

            // Save file is being updated
            Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
            Filesave.SaveDataToFile(saveFile, true);

            Console.WriteLine("> Save file changed and saved.\n");

            /* Change button, so instead of "Add" it will display "Remove". This is done in Filter(). The list is being refreshed. */
            Filter();
        }
        catch (NullReferenceException nullEx)
        {
            Console.WriteLine($"> Exception thrown in the map patching tab!");
            Console.WriteLine($"> Object reference not set to an instance of an object: '{nullEx}'");
            Console.WriteLine($"> Try again!");
        }
    }

    // Could rename to AddOrRemoveWhitelistMap
    [RelayCommand]
    private void AddOrRemoveMapWhitelist(string mapString)
    {
        try
        {
            /* We must use a task delay or else the program gets overused and suddenly crashes. */
            System.Threading.Thread.Sleep(30);

            MainProperties.MapWhitelist ??= new List<Map.LoadoutMap>();

            /* Unlike map patching, adding a map to favorite maps doesn't depend on simple patching. It can be done without it. */

            if (!NoPickupsCheck)
            {
                /* If we want to have pickups in our map, we change the string to lower case */
                mapString = mapString.ToLower();
            }

            /* The foreach loop needs to come after the query */
            foreach (Map.LoadoutMap map in Map.GetLoadoutMaps())
            {
                /* FullMapNameAlt is our command parameter but can be converted to lower case before. It will be dealt with later. */
                if (mapString == map.FullMapNameAlt || mapString == map.FullMapName)
                {
                    if (MainProperties.MapWhitelist.Contains(map))
                    {
                        /* If we try to add a map to favorite maps but it's already in favorite maps, we realize we want to remove it! */
                        MainProperties.MapWhitelist.Remove(map);
                        Console.WriteLine("> Whitelisted map removed.\n");
                        break;
                    }
                    else
                    {
                        MainProperties.MapWhitelist.Add(map);
                        Console.WriteLine("> Map added to whitelist.\n");
                        break;
                    }
                }
            }

            /* We share our list of favorite maps with the Map class, so it can be saved */
            Map.SetMapWhitelist(MainProperties.MapWhitelist);

            // Save file is being updated
            Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
            Filesave.SaveDataToFile(saveFile, true);

            Console.WriteLine("> Save file changed and saved.\n");

            /* Change button, so instead of "Add" it will display "Remove". This is done in Filter(). The list is being refreshed. */
            Filter();
        }
        catch (NullReferenceException nullEx)
        {
            Console.WriteLine($"> Exception thrown in the map patching tab!");
            Console.WriteLine($"> Object reference not set to an instance of an object: '{nullEx}'");
            Console.WriteLine($"> Try again!");
        }
    }

    [RelayCommand]
    private void AddOrRemoveMapQueue(string mapString)
    {
        try
        {
            /* We must use a task delay or else the program gets overused and suddenly crashes. */
            System.Threading.Thread.Sleep(30);

            MainProperties.MapQueueList ??= new List<Map.LoadoutMap>();

            /* Unlike map patching, adding a map to favorite maps doesn't depend on simple patching. It can be done without it. */

            if (!NoPickupsCheck)
            {
                /* If we want to have pickups in our map, we change the string to lower case */
                mapString = mapString.ToLower();
            }

            /* The foreach loop needs to come after the query */
            foreach (Map.LoadoutMap map in Map.GetLoadoutMaps())
            {
                /* FullMapNameAlt is our command parameter but can be converted to lower case before. It will be dealt with later. */
                if (mapString == map.FullMapNameAlt || mapString == map.FullMapName)
                {
                    if (MainProperties.MapQueueList.Contains(map))
                    {
                        if (!LoopQueueCheck)
                        {
                            /* If we try to add a map to favorite maps but it's already in favorite maps, we realize we want to remove it! */
                            MainProperties.MapQueueList.Remove(map);
                            Console.WriteLine("> Map queue entry was removed from list.\n");
                            break;
                        }
                        else
                        {
                            MainProperties.MapQueueList.Remove(map);
                            MainProperties.MapQueueList.Add(map);
                            Console.WriteLine("> Recent map queue entry was brought to the end of the queue.\n");
                            break;
                        }
                    }
                    else
                    {
                        MainProperties.MapQueueList.Add(map);
                        Console.WriteLine("> Map queue entry was added to list.\n");
                        break;
                    }
                }
            }

            /* We share our list of favorite maps with the Map class, so it can be saved */
            // Map.SetMapQueue(new Stack<Map.LoadoutMap>(MainProperties.MapQueueList));
            Map.SetMapQueue(new Stack<Map.LoadoutMap>(MainProperties.MapQueueList));

            // Save file is being updated
            Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
            Filesave.SaveDataToFile(saveFile, true);

            Console.WriteLine("> Save file changed and saved.\n");

            /* Change button, so instead of "Add" it will display "Remove". This is done in Filter(). The list is being refreshed. */
            if (AvoidDoubleFilter)
            {
                AvoidDoubleFilter = false;
            }
            else
            {
                Filter();
            }
        }   
        catch (NullReferenceException nullEx)
        {
            Console.WriteLine($"> Exception thrown in the map patching tab!");
            Console.WriteLine($"> Object reference not set to an instance of an object: '{nullEx}'");
            Console.WriteLine($"> Try again!");
        }
    }

    [RelayCommand]
    private void AddOrRemoveStartingMap(string mapString)
    {
        try
        {
            /* We must use a task delay or else the program gets overused and suddenly crashes. */
            System.Threading.Thread.Sleep(30);

            /* Unlike map patching, adding a map to favorite maps doesn't depend on simple patching. It can be done without it. */

            if (!NoPickupsCheck)
            {
                /* If we want to have pickups in our map, we change the string to lower case */
                mapString = mapString.ToLower();
            }

            /* The foreach loop needs to come after the query */
            foreach (Map.LoadoutMap map in Map.GetLoadoutMaps())
            {
                /* FullMapNameAlt is our command parameter but can be converted to lower case before. It will be dealt with later. */
                if (mapString == map.FullMapNameAlt || mapString == map.FullMapName)
                {
                    if (MainProperties.StartingMap.FullMapName == map.FullMapName)
                    {
                        /* If we try to add a map to favorite maps but it's already in favorite maps, we realize we want to remove it! */
                        MainProperties.StartingMap = new Map.LoadoutMap();
                        Console.WriteLine("> Starting map was removed.\n");
                        break;
                    }
                    else
                    {
                        MainProperties.StartingMap = map;
                        Console.WriteLine("> Starting map was added or replaced.\n");
                        break;
                    }
                }
            }
            /* We share our list of favorite maps with the Map class, so it can be saved */
            Map.SetStartingMap(MainProperties.StartingMap);

            // Save file is being updated
            Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
            Filesave.SaveDataToFile(saveFile, true);

            Console.WriteLine("> Save file changed and saved.\n");

            /* Change button, so instead of "Add" it will display "Remove". This is done in Filter(). The list is being refreshed. */
            Filter();
        }
        catch (NullReferenceException nullEx)
        {
            Console.WriteLine($"> Exception thrown in the map patching tab!");
            Console.WriteLine($"> Object reference not set to an instance of an object: '{nullEx}'");
            Console.WriteLine($"> Try again!");
        }
    }

    [RelayCommand]
    private void FillQueue(string mapString)
    {
        /* We must use a task delay or else the program gets overused and suddenly crashes. */
        System.Threading.Thread.Sleep(30);

        if (FillQueueCheck)
        {
            if (MainProperties.FavoriteMaps != null)
            {
                MainProperties.MapQueueList = MainProperties.FavoriteMaps;
                Map.SetFavoriteMapsAsMapQueue();

                // Save file is being updated
                Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
                Filesave.SaveDataToFile(saveFile, true);

                Console.WriteLine("> Your map queue has been filled with your favorite maps.\n");
                Console.WriteLine("> Save file changed and saved.\n");

                /* The list is being refreshed since we filled the queue. */
                Filter();
            }
        }
        else
        {
            MainProperties.MapQueueList = new List<Map.LoadoutMap>();
            Map.SetMapQueue(new Stack<Map.LoadoutMap>());

            // Save file is being updated
            Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
            Filesave.SaveDataToFile(saveFile, true);

            Console.WriteLine("> Your map queue has been filled with your favorite maps.\n");
            Console.WriteLine("> Save file changed and saved.\n");

            /* The list is being refreshed since we emptied the queue. */
            Filter();
        }
    }

    [RelayCommand]
    private static void LoopQueue()
    {

    }

    /*
    private ObservableCollection<string> _picturePath;

    public ObservableCollection<string> PicturePaths
    {
        get { return _picturePath; }
        set { SetProperty(ref _picturePath, value); }
    }
    */

}