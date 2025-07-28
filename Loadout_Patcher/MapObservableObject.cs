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
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Loadout_Patcher;

public class MapObservableObject : ObservableObject
{
    private string? _id;
    private string? _fullMapName;
    private string? _fullMapNameAlt;
    private string? _baseMap;
    private string? _dayNight;
    private string? _gameMode;
    private string? _picturePath;
    private string? _gameModeDefinition;
    private string? _gameModeTranslated;
    private string? _mapName;
    private string? _mapDescription;
    private string? _mapPatchText;
    private string? _favoriteMapsText;
    private string? _mapBlacklistText;
    private string? _mapWhitelistText;
    private string? _mapQueueText;
    private string? _startingMapText;
    private string? _mapQueuePosition;
    //private bool _mapPatchVisible;

    public string Id
    {
        get { return _id!; }
        set { SetProperty(ref _id, value); }
    }

    public string FullMapName
    {
        get { return _fullMapName!; }
        set { SetProperty(ref _fullMapName, value); }
    }

    public string FullMapNameAlt
    {
        get { return _fullMapNameAlt!; }
        set { SetProperty(ref _fullMapNameAlt, value); }
    }

    public string BaseMap
    {
        get { return _baseMap!; }
        set { SetProperty(ref _baseMap, value); }
    }

    public string DayNight
    {
        get { return _dayNight!; }
        set { SetProperty(ref _dayNight, value); }
    }

    public string GameMode
    {
        get { return _gameMode!; }
        set { SetProperty(ref _gameMode, value); }
    }

    public string PicturePath
    {
        get { return _picturePath!; }
        set { SetProperty(ref _picturePath, value); }
    }

    public string GameModeDefinition
    {
        get { return _gameModeDefinition!; }
        set { SetProperty(ref _gameModeDefinition, value); }
    }

    public string GameModeTranslated
    {
        get { return _gameModeTranslated!; }
        set { SetProperty(ref _gameModeTranslated, value); }
    }

    public string MapName
    {
        get { return _mapName!; }
        set { SetProperty(ref _mapName, value); }
    }

    public string MapDescription
    {
        get { return _mapDescription!; }
        set { SetProperty(ref _mapDescription, value); }
    }

    public string MapPatchText
    {
        get { return _mapPatchText!; }
        set { SetProperty(ref _mapPatchText, value); }
    }

    public string FavoriteMapsText
    {
        get { return _favoriteMapsText!; }
        set { SetProperty(ref _favoriteMapsText, value); }
    }

    public string MapBlacklistText
    {
        get { return _mapBlacklistText!; }
        set { SetProperty(ref _mapBlacklistText, value); }
    }

    public string MapWhitelistText
    {
        get { return _mapWhitelistText!; }
        set { SetProperty(ref _mapWhitelistText, value); }
    }

    public string MapQueueText
    {
        get { return _mapQueueText!; }
        set { SetProperty(ref _mapQueueText, value); }
    }

    public string StartingMapText
    {
        get { return _startingMapText!; }
        set { SetProperty(ref _startingMapText, value); }
    }

    public string MapQueuePosition
    {
        get { return _mapQueuePosition!; }
        set { SetProperty(ref _mapQueuePosition, value); }
    }

    //public bool MapPatchVisible
    //{
    //    get { return _mapPatchVisible!; }
    //    set { SetProperty(ref _mapPatchVisible, value); }
    //}
}