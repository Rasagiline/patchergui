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
using static Loadout_Patcher.ProcessMemory;

namespace Loadout_Patcher
{
    /// <summary>
    /// GUI is for managing information that is usually in use by the UI
    /// </summary>
    public class GUI
    {
        // TODO: if one ESC then if another ESC then
        // close/dispose process
        // closehandle / handle to zero
        // Goodbye! message
        // Environment.Exit(0);
        // must be GUI form eventargs
        //private void Form1_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Escape)
        //    {
        //        MessageBox.Show("Escape key pressed");

        //        // prevent child controls from handling this event as well
        //        e.SuppressKeyPress = true;
        //    }
        //}

        private static bool blacklistPreference;

        public static bool BlacklistPreference
        {
            get { return blacklistPreference; }
            set { blacklistPreference = value; }
        }

        private static bool whitelistPreference;

        public static bool WhitelistPreference
        {
            get { return whitelistPreference; }
            set { whitelistPreference = value; }
        }

        private static bool excludePve;

        public static bool ExcludePve
        {
            get { return excludePve; }
            set { excludePve = value; }
        }

        private static bool showPveOnly;

        public static bool ShowPveOnly
        {
            get { return showPveOnly; }
            set { showPveOnly = value; }
        }

        private static bool fillEmptyMapQueueWithFavoriteMaps;

        public static bool FillEmptyMapQueueWithFavoriteMaps
        {
            get { return fillEmptyMapQueueWithFavoriteMaps; }
            set { fillEmptyMapQueueWithFavoriteMaps = value; }
        }

        private static bool loopMapQueue;

        public static bool LoopMapQueue
        {
            get { return loopMapQueue; }
            set { loopMapQueue = value; }
        }

        private static bool skipStartPage;
        public static bool SkipStartPage
        {
            get { return skipStartPage; }
            set { skipStartPage = value; }
        }

        private static bool instantPatching;
        public static bool InstantPatching
        {
            get { return instantPatching; }
            set { instantPatching = value; }
        }

        private static bool startLoadout;
        public static bool StartLoadout
        {
            get { return startLoadout; }
            set { startLoadout = value; }
        }

        private static bool createSSEShortcut;
        public static bool CreateSSEShortcut
        {
            get { return createSSEShortcut; }
            set { createSSEShortcut = value; }
        }

        private static bool startLoadoutViaSSE;
        public static bool StartLoadoutViaSSE
        {
            get { return startLoadoutViaSSE; }
            set { startLoadoutViaSSE = value; }
        }

        private static string? title = "Loadout Patcher";

        public static string Title
        {
            get { return title!; }
            set { title = value; }
        }

        public GUI()
        {
            blacklistPreference = false;
            whitelistPreference = false;
            excludePve = false;
            showPveOnly = false;
            fillEmptyMapQueueWithFavoriteMaps = false;
            loopMapQueue = true;
            skipStartPage = false;
            instantPatching = true;
            startLoadout = true;
            createSSEShortcut = true;
            startLoadoutViaSSE = true;
            title = "Loadout Patcher";
        }

        // special user set options: preference blacklist or whitelist, exclude or show pve only,
        // if map queue is empty use favorite maps for new queue, looping queue active

        // TODO:
        // Add an option to set the preference to always load full name or always load full name alt

        // map search preferences
        public struct MapSearchPreferences
        {
            public bool BlacklistPreference { get; set; }
            public bool WhitelistPreference { get; set; }
            public bool ExcludePve { get; set; }
            // Show PvE only should focus the PvE tab.
            public bool ShowPveOnly { get; set; }
        }

        // map queue preferences
        public struct MapQueuePreferences
        {
            public bool FillEmptyMapQueueWithFavoriteMaps { get; set; }
            public bool LoopMapQueue { get; set; }
        }

        // structs inside of a struct
        public struct UserSetOptions
        {
            public MapSearchPreferences MapSearchPreferences;

            public MapQueuePreferences MapQueuePreferences;

            public bool SkipStartPage;

            public bool InstantPatching;

            public bool StartLoadout;

            public bool CreateSSEShortcut;

            public bool StartLoadoutViaSSE;
        }

        /// <summary>
        /// Loads the save file's GUI relevant information into a GUI object
        /// </summary>
        /// <param name="saveFile"></param>
        public static void LoadSaveFileIntoGuiProperties(Filesave.SaveFile saveFile)
        {
            BlacklistPreference = saveFile.UserSetOptions.MapSearchPreferences.BlacklistPreference;
            WhitelistPreference = saveFile.UserSetOptions.MapSearchPreferences.WhitelistPreference;
            ExcludePve = saveFile.UserSetOptions.MapSearchPreferences.ExcludePve;
            ShowPveOnly = saveFile.UserSetOptions.MapSearchPreferences.ShowPveOnly;
            FillEmptyMapQueueWithFavoriteMaps = saveFile.UserSetOptions.MapQueuePreferences.FillEmptyMapQueueWithFavoriteMaps;
            LoopMapQueue = saveFile.UserSetOptions.MapQueuePreferences.LoopMapQueue;
            SkipStartPage = saveFile.UserSetOptions.SkipStartPage;
            InstantPatching = saveFile.UserSetOptions.InstantPatching;
            StartLoadout = saveFile.UserSetOptions.StartLoadout;
            CreateSSEShortcut = saveFile.UserSetOptions.CreateSSEShortcut;
            StartLoadoutViaSSE = saveFile.UserSetOptions.StartLoadoutViaSSE;
            Title = saveFile.GuiTitle;
        }

        /// <summary>
        /// Loads all GUI settings into the save file
        /// This method makes it comfortable because the struct UserSetOptions needs to be set at once
        /// </summary>
        /// <param name="saveFile"></param>
        /// <returns>a GUI object with save file settings</returns>
        public static void SynchronizeSaveFile(ref Filesave.SaveFile saveFile)
        {
            saveFile.UserSetOptions = new UserSetOptions
            {
                MapSearchPreferences = new MapSearchPreferences
                {
                    BlacklistPreference = BlacklistPreference,
                    WhitelistPreference = WhitelistPreference,
                    ExcludePve = ExcludePve,
                    ShowPveOnly = ShowPveOnly
                },

                MapQueuePreferences = new MapQueuePreferences
                {
                    FillEmptyMapQueueWithFavoriteMaps = FillEmptyMapQueueWithFavoriteMaps,
                    LoopMapQueue = LoopMapQueue
                },
                SkipStartPage = SkipStartPage,
                InstantPatching = InstantPatching,
                StartLoadout = StartLoadout,
                CreateSSEShortcut = CreateSSEShortcut,
                StartLoadoutViaSSE = StartLoadoutViaSSE
            };
            saveFile.GuiTitle = Title;
        }
    }
}
