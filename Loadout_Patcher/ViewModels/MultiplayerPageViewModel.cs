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
using CommunityToolkit.Mvvm.Input;
//using NVorbis;
//using NAudio.Vorbis;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
//using Avalonia.Media;
//using System.Diagnostics;
//using System.Runtime.InteropServices;
using Avalonia.Threading;
using System.ComponentModel;
//using FluentAvalonia.UI.Controls;

namespace Loadout_Patcher.ViewModels;

public partial class MultiplayerPageViewModel : ViewModelBase
{
   
    public MultiplayerPageViewModel()
    {
        _disTimer.Interval = TimeSpan.FromSeconds(1);
        _disTimer.Tick += DispatcherTimer_Tick;
        _disTimer.Start();

        /* TODO: We load the old data, if FirstTimeTableDataLoad is at least 2 */
        /* TODO: Something triggers this twice. Identify the issue */
        /* At 1, this fires a request for the API, at 2 minimum, old data will be loaded */
        if (FirstTimeTableDataLoad == 1)
        {
            if (sessionInfoList.Count == 1)
            {
                NumberOfGameServersText = "1 game session is currently running";
            }
            else
            {
                NumberOfGameServersText = sessionInfoList.Count + " game sessions are currently running";
            }

            GameServers = new ObservableCollection<GameServerObservableObject> { };
            FavoriteGameServers = new ObservableCollection<GameServerObservableObject> { };

            foreach (SessionInfo sessionInfo in sessionInfoList)
            {
                GameServerObservableObject gameSession = new GameServerObservableObject
                {
                    ServerIPAddress = sessionInfo.ServerIPAddress,
                    Id = sessionInfo.Id,
                    ServerName = sessionInfo.ServerName,
                    Ping = sessionInfo.Ping,
                    Players = sessionInfo.Players,
                    Versus = sessionInfo.Versus,
                    MapName = sessionInfo.MapName,
                    GameMode = sessionInfo.GameMode,
                    PicturePath = sessionInfo.PicturePath, // Jumps into BitmapAssetValueConverter.cs
                    Availability = sessionInfo.Availability,
                    Interactions = sessionInfo.Interactions,
                    MatchTimeRunning = sessionInfo.MatchTimeRunning,
                    ServerTimeRunning = sessionInfo.ServerTimeRunning,
                    LastTimeUp = sessionInfo.LastTimeUp,
                    PlayerStatus = sessionInfo.PlayerStatus,
                    MapString = sessionInfo.MapString,
                    JoinSessionText = "Join now",
                    FavoriteServerText = "Add",
                    FavoriteServerIcon = "star_add_regular", // Jumps into FindResourceFromStringConverter.cs - makes it act like a dynamic resource
                    FavoriteMapText = "Add",
                    FavoriteMapIcon = "star_add_regular"
                };
                /* We set the ping color */
                int intResult;
                Int32.TryParse(gameSession.Ping, out intResult);
                if (intResult >= 225)
                {
                    gameSession.PingColor = "#FF0000"; // Red
                }
                else if (intResult >= 175)
                {
                    gameSession.PingColor = "#FFA500"; // Orange
                }
                else if (intResult >= 100)
                {
                    gameSession.PingColor = "#FFFF00"; // Yellow
                }
                else if (intResult >= 70)
                {
                    gameSession.PingColor = "#ACEE00"; // Greenyellow
                }
                else
                {
                    gameSession.PingColor = "#00FF00"; // Green
                }
                /* We set the server availability color */
                if (gameSession.Availability == "online")
                {
                    gameSession.AvailabilityColor = "#00FF00";
                }
                else
                {
                    gameSession.AvailabilityColor = "#FF0000";
                }
                /* We set the players count color */
                if (gameSession.Players == "4/4" || gameSession.Players == "8/8")
                {
                    gameSession.PlayersColor = "#FF0000";
                }
                else
                {
                    gameSession.PlayersColor = "#00FF00";
                }
                /* We set the player status color */
                if (gameSession.PlayerStatus == "allowed")
                {
                    gameSession.PlayerStatusColor = "#00FF00";
                }
                else
                {
                    gameSession.PlayerStatusColor = "#FF0000";
                }
                /* We set the join button functionality */
                if (gameSession.Availability == "offline" || gameSession.PlayersColor == "#FF0000" || gameSession.PlayerStatus == "banned")
                {
                    gameSession.JoinSessionEnabled = "False";
                }
                else
                {
                    gameSession.JoinSessionEnabled = "True";
                }

                /* We check if we have favorite sessions in your static list favoriteSessions, otherwise it will lead to an error */
                List<MultiplayerSession.SessionInfoKeys> favoriteSessionList = MultiplayerSession.GetFavoriteSessions();
                if (favoriteSessionList != null)
                {
                    foreach (MultiplayerSession.SessionInfoKeys favoriteSession in favoriteSessionList)
                    {
                        /* The transferred map name can either be the name or the alt name, so don't take it. */
                        if (favoriteSession.ServerIPAddress == gameSession.ServerIPAddress && favoriteSession.ServerName == gameSession.ServerName)
                        {
                            gameSession.FavoriteServerText = "Remove";
                            gameSession.FavoriteServerIcon = "star_off_regular";

                            /* We add the selected session to our favorite servers list */
                            FavoriteGameServers.Add(gameSession);
                        }
                    }
                }

                /* We check if we have favorite maps in MainProperties, otherwise it will lead to an error */
                if (MainProperties.FavoriteMaps != null)
                {
                    foreach (Map.LoadoutMap favoriteMap in MainProperties.FavoriteMaps)
                    {
                        /* The transferred map name can either be the name or the alt name */
                        if (favoriteMap.FullMapName == gameSession.MapString || favoriteMap.FullMapNameAlt == gameSession.MapString)
                        {
                            gameSession.FavoriteMapText = "Remove";
                            gameSession.FavoriteMapIcon = "star_off_regular";
                        }
                    }
                }

                GameServers.Add(gameSession);
            }
            CopyGameServers = GameServers;
        }
        else if (FirstTimeTableDataLoad > 1)
        {
            /* We send a copy of the data to not fire a new request */
            GameServers = CopyGameServers!;
        }
        FirstTimeTableDataLoad++;
    }

    private static int firstTimeTableDataLoad;

    public static int FirstTimeTableDataLoad
    {
        get { return firstTimeTableDataLoad; }
        set { firstTimeTableDataLoad = value; }
    }

    private static int joinCounter = 0;

    private bool nextJoinCooldown = true;

    //private TimeSpan secondsThatHavePassed = TimeSpan.Zero;

    private ObservableCollection<GameServerObservableObject>? _gameServers;

    public ObservableCollection<GameServerObservableObject> GameServers
    {
        get { return _gameServers!; }
        set { SetProperty(ref _gameServers, value); }
    }

    private ObservableCollection<GameServerObservableObject>? _favoriteGameServers;

    public ObservableCollection<GameServerObservableObject> FavoriteGameServers
    {
        get { return _favoriteGameServers!; }
        set { SetProperty(ref _favoriteGameServers, value); }
    }

    public static ObservableCollection<GameServerObservableObject>? CopyGameServers;

    private (string, string) lastJoinedSessionIdAndText = ("","");

    [ObservableProperty]
    private string _numberOfGameServersText = "";

    [ObservableProperty]
    private bool _refreshButtonPressed = false;

    private readonly static List<SessionInfo> sessionInfoList = new List<SessionInfo>
    {
        new SessionInfo
        {
            ServerIPAddress = "123.123.123.123",
            Id = "0001",
            ServerName = "Wat 24/7 Loadout Reloaded Game Server",
            Ping = "40",
            Players = "8/8",
            Versus = "PvP",
            MapName = "Fissure at Night",
            GameMode = "Jackhammer",
            PicturePath = "/Assets/Maps/fissurenight_ctf.webp",
            Availability = "online", // The front-end field will be set to green if online and red if offline
            Interactions = "yes",
            MatchTimeRunning = "04:25.355",
            ServerTimeRunning = "8 hours 24 minutes",
            LastTimeUp = "today, right now",
            PlayerStatus = "allowed",
            MapString = "fissurenight_ctf"
        },

        new SessionInfo
        {
            ServerIPAddress = "123.123.123.124",
            Id = "0002",
            ServerName = "Hardcore Loadout Reloaded",
            Ping = "70",
            Players = "1/8",
            Versus = "PvP",
            MapName = "Spires",
            GameMode = "Domination",
            PicturePath = "/Assets/Maps/spires_domination.webp",
            Availability = "online", // The front-end field will be set to green if online and red if offline
            Interactions = "yes",
            MatchTimeRunning = "09:12.682",
            ServerTimeRunning = "8 hours 24 minutes",
            LastTimeUp = "today, right now",
            PlayerStatus = "allowed",
            MapString = "spires_domination"
        },

        new SessionInfo
        {
            ServerIPAddress = "123.123.123.125",
            Id = "0003",
            ServerName = "Bot Server Easy Mode Loadout Reloaded",
            Ping = "999",
            Players = "0/4",
            Versus = "PvE",
            MapName = "Four Points",
            GameMode = "Hold Your Pole",
            PicturePath = "/Assets/Maps/fath_705_botwave.webp",
            Availability = "offline", // The front-end field will be set to green if online and red if offline
            Interactions = "no",
            MatchTimeRunning = "-",
            ServerTimeRunning = "-",
            LastTimeUp = "yesterday",
            PlayerStatus = "banned",
            MapString = "fath_705_botwave"
        },
    };

    public struct SessionInfo
    {
        public string ServerIPAddress;
        public string Id;
        public string ServerName;
        public string Ping;
        public string Players;
        public string Versus;
        public string MapName;
        public string GameMode;
        public string PicturePath;
        public string Availability;
        public string Interactions;
        public string MatchTimeRunning;
        public string ServerTimeRunning;
        public string LastTimeUp;
        public string PlayerStatus;
        public string JoinSessionText;
        public string MapString;
    }





    [RelayCommand]
    private void RefreshServerList()
    {
        if (RefreshServerListText == "Refresh")
        {
            /* We set up an easter egg */
            if (joinCounter == 1)
            {
                RefreshButtonPressed = true;
            }
            /* We reset the join cooldown before the set the join button functionality */
            if (nextJoinCooldown)
            {
                joinCounter = 0;
                nextJoinCooldown = false;
            }

            GameServers = new ObservableCollection<GameServerObservableObject> { };
            FavoriteGameServers = new ObservableCollection<GameServerObservableObject> { };
            /* We always need a new GameServerObservableObject of that kind to reset the list */
            CopyGameServers = new ObservableCollection<GameServerObservableObject> { };

            foreach (SessionInfo sessionInfo in sessionInfoList)
            {
                GameServerObservableObject gameSession = new GameServerObservableObject
                {
                    ServerIPAddress = sessionInfo.ServerIPAddress,
                    Id = sessionInfo.Id,
                    ServerName = sessionInfo.ServerName,
                    Ping = sessionInfo.Ping,
                    Players = sessionInfo.Players,
                    Versus = sessionInfo.Versus,
                    MapName = sessionInfo.MapName,
                    GameMode = sessionInfo.GameMode,
                    PicturePath = sessionInfo.PicturePath,
                    Availability = sessionInfo.Availability,
                    Interactions = sessionInfo.Interactions,
                    MatchTimeRunning = sessionInfo.MatchTimeRunning,
                    ServerTimeRunning = sessionInfo.ServerTimeRunning,
                    LastTimeUp = sessionInfo.LastTimeUp,
                    PlayerStatus = sessionInfo.PlayerStatus,
                    MapString = sessionInfo.MapString,
                    FavoriteServerText = "Add", // Remove
                    FavoriteServerIcon = "star_add_regular", // star_off_regular
                    FavoriteMapText = "Add",
                    FavoriteMapIcon = "star_add_regular"
                };
                /* We mess up some values for the template */
                gameSession.PlayerStatus = "allowed";
                Random randomNumber = new Random();
                int online = randomNumber.Next(0, 8); // Creates a number between 0 and 7
                int playerCount = randomNumber.Next(0, 5); // Creates a number between 0 and 4
                int ping = randomNumber.Next(20, 251); // Creates a number between 20 and 250
                                                       // TODO: Check for the game session ID instead of the parameter ID because we get no parameter
                if (online >= 1)
                {
                    gameSession.Availability = "online";
                }
                else
                {
                    gameSession.Availability = "offline";
                }
                if (gameSession.Availability == "online")
                {
                    gameSession.Players = playerCount.ToString() + sessionInfo.Players.Substring(1);
                    gameSession.Ping = ping.ToString();
                }
                else
                {
                    gameSession.Players = "0/4";
                    gameSession.Ping = "999";
                }
                /* We set the ping color */
                int intResult;
                Int32.TryParse(gameSession.Ping, out intResult);
                if (intResult >= 225)
                {
                    gameSession.PingColor = "#FF0000"; // Red
                }
                else if (intResult >= 175)
                {
                    gameSession.PingColor = "#FFA500"; // Orange
                }
                else if (intResult >= 100)
                {
                    gameSession.PingColor = "#FFFF00"; // Yellow
                }
                else if (intResult >= 70)
                {
                    gameSession.PingColor = "#ACEE00"; // Greenyellow
                }
                else
                {
                    gameSession.PingColor = "#00FF00"; // Green
                }
                /* We set the server availability color */
                if (gameSession.Availability == "online")
                {
                    gameSession.AvailabilityColor = "#00FF00";
                }
                else
                {
                    gameSession.AvailabilityColor = "#FF0000";
                }
                /* We set the players count color */
                if (gameSession.Players == "4/4" || gameSession.Players == "8/8")
                {
                    gameSession.PlayersColor = "#FF0000";
                }
                else
                {
                    gameSession.PlayersColor = "#00FF00";
                }
                /* We set the player status color */
                if (gameSession.PlayerStatus == "allowed")
                {
                    gameSession.PlayerStatusColor = "#00FF00";
                }
                else
                {
                    gameSession.PlayerStatusColor = "#FF0000";
                }
                /* We set the join button functionality */
                if (gameSession.Availability == "offline" || gameSession.PlayersColor == "#FF0000" || gameSession.PlayerStatus == "banned")
                {
                    gameSession.JoinSessionEnabled = "False";
                }
                else
                {
                    gameSession.JoinSessionEnabled = "True";
                }
                if (lastJoinedSessionIdAndText.Item1 == sessionInfo.Id)
                {
                    Console.WriteLine("> This is the server's ID: " + sessionInfo.Id);
                    Console.WriteLine("> This is the server's IP address: " + sessionInfo.ServerIPAddress + "\n");
                    gameSession.JoinSessionText = lastJoinedSessionIdAndText.Item2;
                }
                else
                {
                    gameSession.JoinSessionText = "Join now";
                }

                /* We check if we have favorite sessions in your static list favoriteSessions, otherwise it will lead to an error */
                List<MultiplayerSession.SessionInfoKeys> favoriteSessionList = MultiplayerSession.GetFavoriteSessions();
                if (favoriteSessionList != null)
                {
                    foreach (MultiplayerSession.SessionInfoKeys favoriteSession in favoriteSessionList)
                    {
                        /* The transferred map name can either be the name or the alt name */
                        if (favoriteSession.ServerIPAddress == gameSession.ServerIPAddress && favoriteSession.ServerName == gameSession.ServerName)
                        {
                            gameSession.FavoriteServerText = "Remove";
                            gameSession.FavoriteServerIcon = "star_off_regular";

                            /* We add the selected session to our favorite servers list */
                            FavoriteGameServers.Add(gameSession);
                        }
                    }
                }

                /* We check if we have favorite maps in MainProperties, otherwise it will lead to an error */
                if (MainProperties.FavoriteMaps != null)
                {
                    foreach (Map.LoadoutMap favoriteMap in MainProperties.FavoriteMaps)
                    {
                        /* The transferred map name can either be the name or the alt name */
                        if (favoriteMap.FullMapName == gameSession.MapString || favoriteMap.FullMapNameAlt == gameSession.MapString)
                        {
                            gameSession.FavoriteMapText = "Remove";
                            gameSession.FavoriteMapIcon = "star_off_regular";
                        }
                    }
                }

                GameServers.Add(gameSession);
                /* This part is for saving the random values into a copy. Will probably deleted when real values come in */
                CopyGameServers.Add(gameSession);
            }
            /* We play a refreshing sound from Axl, Helga or T-Bone on click if enabled */
            Sound.PlayOtherSoundsRandomly();

            ts = DateTime.Now.AddSeconds(12) - DateTime.Now;
            RefreshServerListEnabled = false;
        }
        else
        {
            RefreshServerListEnabled = true;
        }
        System.Console.WriteLine("> Server list refreshed!\n");
    }

    /// <summary>
    /// This method counts as a refresh, but without request. Old data is being used.
    /// </summary>
    /// <param name="Id"></param>
    [RelayCommand]
    private void JoinSession(string Id)
    {
        GameServers = new ObservableCollection<GameServerObservableObject> { };
        FavoriteGameServers = new ObservableCollection<GameServerObservableObject> { };
        if (joinCounter == 0)
        {
            nextJoinCooldown = false;
        }
        int copyCounter = 0;
        foreach (SessionInfo sessionInfo in sessionInfoList)
        {
            GameServerObservableObject gameSession = new GameServerObservableObject
            {
                ServerIPAddress = sessionInfo.ServerIPAddress,
                Id = sessionInfo.Id,
                ServerName = sessionInfo.ServerName,
                Ping = sessionInfo.Ping,
                Players = sessionInfo.Players,
                Versus = sessionInfo.Versus,
                MapName = sessionInfo.MapName,
                GameMode = sessionInfo.GameMode,
                PicturePath = sessionInfo.PicturePath,
                Availability = sessionInfo.Availability,
                Interactions = sessionInfo.Interactions,
                MatchTimeRunning = sessionInfo.MatchTimeRunning,
                ServerTimeRunning = sessionInfo.ServerTimeRunning,
                LastTimeUp = sessionInfo.LastTimeUp,
                PlayerStatus = sessionInfo.PlayerStatus,
                MapString = sessionInfo.MapString,
                FavoriteServerText = "Add",
                FavoriteServerIcon = "star_add_regular",
                FavoriteMapText = "Add",
                FavoriteMapIcon = "star_add_regular"                
            };
            /* Possibly delete this part once the real values come in. Actually this part needs to stay. */
            if (CopyGameServers != null)
            {
                gameSession = CopyGameServers[copyCounter];
            }
            /* We set the ping color */
            int intResult;
            Int32.TryParse(gameSession.Ping, out intResult);
            if (intResult >= 225)
            {
                gameSession.PingColor = "#FF0000"; // Red
            }
            else if (intResult >= 175)
            {
                gameSession.PingColor = "#FFA500"; // Orange
            }
            else if (intResult >= 100)
            {
                gameSession.PingColor = "#FFFF00"; // Yellow
            }
            else if (intResult >= 70)
            {
                gameSession.PingColor = "#ACEE00"; // Greenyellow
            }
            else
            {
                gameSession.PingColor = "#00FF00"; // Green
            }
            /* We set the server availability color */
            if (gameSession.Availability == "online")
            {
                gameSession.AvailabilityColor = "#00FF00";
            }
            else
            {
                gameSession.AvailabilityColor = "#FF0000";
            }
            /* We set the players count color */
            if (gameSession.Players == "4/4" || gameSession.Players == "8/8")
            {
                gameSession.PlayersColor = "#FF0000";
            }
            else
            {
                gameSession.PlayersColor = "#00FF00";
            }
            /* We set the player status color */
            if (gameSession.PlayerStatus == "allowed")
            {
                gameSession.PlayerStatusColor = "#00FF00";
            }
            else
            {
                gameSession.PlayerStatusColor = "#FF0000";
            }
            /* We set the join button functionality */
            if (gameSession.Availability == "offline" || gameSession.PlayersColor == "#FF0000" || gameSession.PlayerStatus == "banned")
            {
                gameSession.JoinSessionEnabled = "False";
            }
            else
            {
                gameSession.JoinSessionEnabled = "True";
            }
            if (Id == sessionInfo.Id)
            {
                Console.WriteLine("> This is the server's ID: " + sessionInfo.Id);
                Console.WriteLine("> This is the server's IP address: " + sessionInfo.ServerIPAddress + "\n");
                if (lastJoinedSessionIdAndText.Item1 == Id && lastJoinedSessionIdAndText.Item2 == "Joined!")
                {
                    gameSession.JoinSessionText = "Left";
                }
                else if (lastJoinedSessionIdAndText.Item1 == Id && lastJoinedSessionIdAndText.Item2 == "Left")
                {
                    gameSession.JoinSessionText = "Joined!";
                }
                else
                {
                    gameSession.JoinSessionText = "Joined!";
                }
                lastJoinedSessionIdAndText.Item1 = Id;
                lastJoinedSessionIdAndText.Item2 = gameSession.JoinSessionText;
            }
            else
            {
                gameSession.JoinSessionText = "Join now";
            }
            // TODO: Option to leave server "JoinSessionText: Left"
            if (nextJoinCooldown || joinCounter >= 25)
            {
                gameSession.JoinSessionEnabled = "False";
                // lastJoinedSessionIdAndText.Item2 == "Joined!"

                if (copyCounter == sessionInfoList.Count - 1)
                {
                    /* We inform the user about the disabled join buttons */
                    System.Console.WriteLine("> All join buttons are currently disabled.");
                    System.Console.WriteLine("> To to able to join a server again, press the refresh button!\n");
                }
            }

            /* We check if we have favorite sessions in your static list favoriteSessions, otherwise it will lead to an error */
            List<MultiplayerSession.SessionInfoKeys> favoriteSessionList = MultiplayerSession.GetFavoriteSessions();
            if (favoriteSessionList != null)
            {
                foreach (MultiplayerSession.SessionInfoKeys favoriteSession in favoriteSessionList)
                {
                    /* The transferred map name can either be the name or the alt name */
                    if (favoriteSession.ServerIPAddress == gameSession.ServerIPAddress && favoriteSession.ServerName == gameSession.ServerName)
                    {
                        gameSession.FavoriteServerText = "Remove";
                        gameSession.FavoriteServerIcon = "star_off_regular";

                        /* We add the selected session to our favorite servers list */
                        FavoriteGameServers.Add(gameSession);
                    }
                }
            }

            /* We check if we have favorite maps in MainProperties, otherwise it will lead to an error */
            if (MainProperties.FavoriteMaps != null)
            {
                foreach (Map.LoadoutMap favoriteMap in MainProperties.FavoriteMaps)
                {
                    /* The transferred map name can either be the name or the alt name */
                    if (favoriteMap.FullMapName == gameSession.MapString || favoriteMap.FullMapNameAlt == gameSession.MapString)
                    {
                        gameSession.FavoriteMapText = "Remove";
                        gameSession.FavoriteMapIcon = "star_off_regular";
                    }
                }
            }

            GameServers.Add(gameSession);
            /* We do some join button cooldown management */
            if (copyCounter == sessionInfoList.Count - 1 && joinCounter == 0)
            {
                nextJoinCooldown = true;
            }
            else if (copyCounter == sessionInfoList.Count - 1 && lastJoinedSessionIdAndText.Item2 == "Joined!")
            {
                // We want a delayed cooldown, waiting for "Left".
                if (joinCounter % 3 == 0)
                {
                    nextJoinCooldown = true;
                }
                else
                {
                    nextJoinCooldown = false;
                }
            }
            else if (copyCounter == sessionInfoList.Count - 1 && lastJoinedSessionIdAndText.Item2 == "Left")
            {
                // Don't change this condition anymore.
                nextJoinCooldown = true;
            }
            copyCounter++;
        }

        /* We focus on the Loadout process since we have joined a session */
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
        // Alternatively joinCounter % 2 == 0 && joinCounter > 1 to allow 3 joins before a cooldown.
        /* We outcomment this to reduce the attempts */
        /*
        if (onJoinCooldown && joinCooldownFalsed)
        {
            System.Console.WriteLine("You have joined multiple times.");
            System.Console.WriteLine("Please wait and then refresh, so you can join again.");

            //onJoinCooldown = true;
            //joinCooldownFalsed = false;
        }
        */
        joinCounter++;
    }

    /* It's questionable if we want to look for the ID */
    [RelayCommand]
    private void AddOrRemoveFavoriteServers(string Id)
    {
        try
        {
            List<MultiplayerSession.SessionInfoKeys> favoriteSessionList = MultiplayerSession.GetFavoriteSessions();

            if (CopyGameServers != null)
            {
                string ipAddress = "";
                string serverName = "";
                favoriteSessionList ??= new List<MultiplayerSession.SessionInfoKeys>();

                foreach (GameServerObservableObject sessionInfo in CopyGameServers)
                {
                    if (sessionInfo.Id == Id)
                    {
                        ipAddress = sessionInfo.ServerIPAddress;
                        serverName = sessionInfo.ServerName;
                    }
                }

                // This assignment is probably unnecessary if GameServers is always updated
                GameServers = CopyGameServers;
                // Do not use this line as it would break everything
                //FavoriteGameServers = CopyGameServers;

                // TODO: Insert more such foreach breaks when finished!
                // FavoriteGameServers.Remove(gameServer);
                // break;

                /* This already finds the corresponding session */
                /* The foreach loop needs to come after the query */
                foreach (SessionInfo session in sessionInfoList)
                {
                    MultiplayerSession.SessionInfoKeys sessionInfoKeys = new MultiplayerSession.SessionInfoKeys()
                    {
                        ServerIPAddress = ipAddress,
                        ServerName = serverName
                    };

                    if (ipAddress == session.ServerIPAddress)
                    {
                        if (favoriteSessionList.Contains(sessionInfoKeys))
                        {
                            /* If we try to add a session to the session list but it's already in that list, we realize we want to remove it! */
                            favoriteSessionList.Remove(sessionInfoKeys);
                            Console.WriteLine("> Favorite session removed.\n");

                            /* We perform a refresh of the server list */
                            foreach (GameServerObservableObject gameServer in GameServers)
                            {
                                if (gameServer.Id == Id)
                                {
                                    gameServer.FavoriteServerText = "Add";
                                    gameServer.FavoriteServerIcon = "star_add_regular";

                                    FavoriteGameServers.Remove(gameServer);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            favoriteSessionList.Add(sessionInfoKeys);
                            Console.WriteLine("> Favorite session added.\n");

                            /* We perform a refresh of the server list */
                            foreach (GameServerObservableObject gameServer in GameServers)
                            {
                                if (gameServer.Id == Id)
                                {
                                    gameServer.FavoriteServerText = "Remove";
                                    gameServer.FavoriteServerIcon = "star_off_regular";

                                    FavoriteGameServers.Add(gameServer);
                                    break;
                                }
                            }
                        }
                    }
                }

                /* We share our list of favorite sessions, so it can be saved */
                MultiplayerSession.SetFavoriteSessions(favoriteSessionList);

                // Add terminal text for adding a favorite session, copy code, edit

                // Save file is being updated
                Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
                Filesave.SaveDataToFile(saveFile, true);

                Console.WriteLine("> Save file changed and saved.");
                Console.WriteLine("> It is recommended to press on Refresh to refresh the server list!\n");
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
    private void AddOrRemoveFavoriteMaps(string Id)
    {
        try
        {
            if (CopyGameServers != null)
            {
                string mapString = "";
                MainProperties.FavoriteMaps ??= new List<Map.LoadoutMap>();

                foreach (GameServerObservableObject sessionInfo in CopyGameServers)
                {
                    if (sessionInfo.Id == Id)
                    {
                        mapString = sessionInfo.MapString;
                    }
                }

                /* This already finds the corresponding map, no matter if alt name or not */
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

                            /* We perform a refresh of the server list */
                            foreach (GameServerObservableObject gameServer in GameServers)
                            {
                                if (gameServer.Id == Id)
                                {
                                    gameServer.FavoriteMapText = "Add";
                                    gameServer.FavoriteMapIcon = "star_add_regular";
                                    break;
                                }
                            }
                        }
                        else
                        {
                            MainProperties.FavoriteMaps.Add(map);
                            Console.WriteLine("> Favorite map added.\n");

                            /* We perform a refresh of the server list */
                            foreach (GameServerObservableObject gameServer in GameServers)
                            {
                                if (gameServer.Id == Id)
                                {
                                    gameServer.FavoriteMapText = "Remove";
                                    gameServer.FavoriteMapIcon = "star_off_regular";
                                    break;
                                }
                            }
                        }
                    }
                }

                /* We share our list of favorite maps with the Map class, so it can be saved */
                Map.SetFavoriteMaps(MainProperties.FavoriteMaps);

                // Add terminal text for adding a favorite map, copy code, edit

                // Save file is being updated
                Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
                Filesave.SaveDataToFile(saveFile, true);

                Console.WriteLine("> Save file changed and saved.");
                Console.WriteLine("> It is recommended to press on Refresh to refresh the server list!\n");
            }
        }
        catch (NullReferenceException nullEx)
        {
            Console.WriteLine($"> Exception thrown in the map patching tab!");
            Console.WriteLine($"> Object reference not set to an instance of an object: '{nullEx}'");
            Console.WriteLine($"> Try again!");
        }
    }


    #region Timer

    /* Public bool because it's being used inside MultiplayerPageView.axaml.cs */
    [ObservableProperty]
    public bool refreshServerListEnabled;






    private TimeSpan ts;

    private DispatcherTimer _disTimer = new DispatcherTimer();

    public new event PropertyChangedEventHandler? PropertyChanged;
    public EventHandler<DateTime> OnEveryHour = (s, e) => { };

    [ObservableProperty]
    private string currentTimeOfTheDay = "";

    private string RefreshServerListText = "";

    private DateTime _nextCycle = DateTime.Now.AddSeconds(12);

    private ObservableCollection<CountdownObservableObject>? _countdownTimer;

    public ObservableCollection<CountdownObservableObject> CountdownTimer
    {
        get { return _countdownTimer!; }
        set { SetProperty(ref _countdownTimer, value); }
    }

    private void DispatcherTimer_Tick(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        if (now >= _nextCycle)
        {
            //_nextCycle = DateTime.Now.AddSeconds(1);
            OnEveryHour(this, now);
        }
        if (_nextCycle > now)
        {
            ts = _nextCycle - now;
        }
        else
        {
            ts = ts - TimeSpan.FromSeconds(1);
        }
        if (ts.Seconds > 0)
        {
            RefreshServerListText = String.Format("Refresh ({0})", ts.Seconds);
            RefreshServerListEnabled = false;
        }
        else
        {
            RefreshServerListText = String.Format("Refresh");
            RefreshServerListEnabled = true;
        }
        /* We don't touch the time of the day */
        CurrentTimeOfTheDay = now.ToString("HH:mm:ss"); // HH:mm:ss

        // Test
        CountdownTimer = new ObservableCollection<CountdownObservableObject>
        {
            new CountdownObservableObject
            {
                CurrentTimeOfTheDay = CurrentTimeOfTheDay,
                RefreshServerListText = RefreshServerListText
            },
        };

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTimeOfTheDay)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RefreshServerListText)));
    }

    #endregion














}