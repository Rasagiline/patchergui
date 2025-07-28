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


namespace Loadout_Patcher;

public class MultiplayerSession
{
    /* TODO: Use this struct instead of the other one */
    public struct SessionInfoKeys
    {
        public string ServerIPAddress;
        public string ServerName;
    }





    #region Save file

    /* TODO: A lot of session info needs to be updated all the time and multiple keys will help finding the session */

    private static List<SessionInfoKeys> favoriteSessions = new List<SessionInfoKeys>();

    public static void SetFavoriteSessions(List<SessionInfoKeys> sessionList)
    {
        favoriteSessions = sessionList;
    }

    public static List<SessionInfoKeys> GetFavoriteSessions()
    {
        return favoriteSessions;
    }

    /// <summary>
    /// Loads save file information into the MultiplayerSession property
    /// </summary>
    /// <param name="saveFile"></param>
    public static void LoadSaveFileIntoMultiplayerSessionProperty(Filesave.SaveFile saveFile)
    {
        // A check if content is there. Nothing can be set if there is nothing
        if (saveFile.FavoriteSessions != null && saveFile.FavoriteSessions.Count > 0)
        {
            SetFavoriteSessions(saveFile.FavoriteSessions);
        }
    }

    /// <summary>
    /// Loads the favorite sessions into the save file
    /// This method makes it comfortable to set all things at once
    /// </summary>
    /// <param name="saveFile"></param>
    public static void SynchronizeSaveFile(ref Filesave.SaveFile saveFile)
    {
        // A check if content is there. Nothing can be set if there is nothing
        if (GetFavoriteSessions().Count > 0)
        {
            saveFile.FavoriteSessions = GetFavoriteSessions();
        }
    }

    #endregion





}
