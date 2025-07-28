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
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Loadout_Patcher;

public class GameServerObservableObject : ObservableObject
{
    private string? _serverIPAddress;
    private string? _id;
    private string? _serverName;
    private string? _ping;
    private string? _players;
    private string? _versus;
    private string? _mapName;
    private string? _mapString;
    private string? _gameMode;
    private string? _picturePath;
    private string? _availability;
    private string? _interactions;
    private string? _matchTimeRunning;
    private string? _serverTimeRunning;
    private string? _lastTimeUp;
    private string? _playerStatus;
    private string? _joinSessionText;
    private string? _pingColor;
    private string? _availabilityColor;
    private string? _playersColor;
    private string? _playerStatusColor;
    private string? _joinSessionEnabled;
    private string? _favoriteServerText;
    private string? _favoriteServerIcon;
    private string? _favoriteMapText;
    private string? _favoriteMapIcon;


    public string ServerIPAddress
    {
        get { return _serverIPAddress!; }
        set { SetProperty(ref _serverIPAddress, value); }
    }

    public string Id
    {
        get { return _id!; }
        set { SetProperty(ref _id, value); }
    }

    public string ServerName
    {
        get { return _serverName!; }
        set { SetProperty(ref _serverName, value); }
    }

    public string Ping
    {
        get { return _ping!; }
        set { SetProperty(ref _ping, value); }
    }

    public string Players
    {
        get { return _players!; }
        set { SetProperty(ref _players, value); }
    }

    public string Versus
    {
        get { return _versus!; }
        set { SetProperty(ref _versus, value); }
    }

    public string MapName
    {
        get { return _mapName!; }
        set { SetProperty(ref _mapName, value); }
    }

    public string MapString
    {
        get { return _mapString!; }
        set { SetProperty(ref _mapString, value); }
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

    public string Availability
    {
        get { return _availability!; }
        set { SetProperty(ref _availability, value); }
    }

    public string Interactions
    {
        get { return _interactions!; }
        set { SetProperty(ref _interactions, value); }
    }

    public string MatchTimeRunning
    {
        get { return _matchTimeRunning!; }
        set { SetProperty(ref _matchTimeRunning, value); }
    }

    public string ServerTimeRunning
    {
        get { return _serverTimeRunning!; }
        set { SetProperty(ref _serverTimeRunning, value); }
    }

    public string LastTimeUp
    {
        get { return _lastTimeUp!; }
        set { SetProperty(ref _lastTimeUp, value); }
    }

    public string PlayerStatus
    {
        get { return _playerStatus!; }
        set { SetProperty(ref _playerStatus, value); }
    }

    public string JoinSessionText
    {
        get { return _joinSessionText!; }
        set { SetProperty(ref _joinSessionText, value); }
    }

    public string PingColor
    {
        get { return _pingColor!; }
        set { SetProperty(ref _pingColor, value); }
    }

    public string AvailabilityColor
    {
        get { return _availabilityColor!; }
        set { SetProperty(ref _availabilityColor, value); }
    }

    public string PlayersColor
    {
        get { return _playersColor!; }
        set { SetProperty(ref _playersColor, value); }
    }

    public string PlayerStatusColor
    {
        get { return _playerStatusColor!; }
        set { SetProperty(ref _playerStatusColor, value); }
    }

    public string JoinSessionEnabled
    {
        get { return _joinSessionEnabled!; }
        set { SetProperty(ref _joinSessionEnabled, value); }
    }

    public string FavoriteServerText
    {
        get { return _favoriteServerText!; }
        set { SetProperty(ref _favoriteServerText, value); }
    }

    public string FavoriteServerIcon
    {
        get { return _favoriteServerIcon!; }
        set { SetProperty(ref _favoriteServerIcon, value); }
    }

    public string FavoriteMapText
    {
        get { return _favoriteMapText!; }
        set { SetProperty(ref _favoriteMapText, value); }
    }

    public string FavoriteMapIcon
    {
        get { return _favoriteMapIcon!; }
        set { SetProperty(ref _favoriteMapIcon, value); }
    }
}