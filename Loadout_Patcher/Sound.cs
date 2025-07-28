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
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Loadout_Patcher
{
    public class Sound
    {
        private readonly static string[] soundListSuccessSoundsStart = new string[]
        {
            "0x10ae43bd.ogg"
        };

        private readonly static string[] soundListSuccessSoundsStartMechanical = new string[]
        {
            "0x159777ca.ogg",
            "0x194721b4.ogg",
            "0x1e6ece31.ogg",
            "0x28d6968.ogg",
            "0x2e27e4b.ogg",
            "0x66f3f662.ogg",
            "0x7e663c44.ogg",
            "0x8c9e2670.ogg",
            "0x9935166a.ogg",
            "0xe76f6dfe.ogg"
        };

        private readonly static string[] soundListSuccessSoundsAxl = new string[]
        {
            "0x1d82cc7f.ogg",
            "0x3bb59174.ogg",
            "0x58496e3a.ogg",
            "0x5f24aa23.ogg",
            "0x6333b0c2.ogg",
            "0x7a288183.ogg",
            "0xb6470f16.ogg"
        };

        private readonly static string[] soundListSuccessSoundsHelga = new string[]
        {
            "0x320268f4.ogg",
            "0x45055862.ogg",
            "0x5175dfeb.ogg",
            "0x7856fe81.ogg",
            "0xb6cdf2ec.ogg",
            "0xdb61cdc1.ogg",
            "0xdc0c09d8.ogg"
        };

        private readonly static string[] soundListSuccessSoundsTBone = new string[]
        {
            "0x25ac9295.ogg",
            "0x52aba203.ogg",
            "0x55c6661a.ogg",
            "0xae36077b.ogg",
            "0xb27e4b1d.ogg",
            "0xbbc80736.ogg",
            "0xd7eabfdf.ogg"
        };

        private readonly static string[] soundListMinigameSoundsSentry = new string[]
        {
            "0x83dda7ae.ogg" // Enter
        };

        private readonly static string[] soundListMinigameSoundsTerminal = new string[]
        {
            "0x21aed949.ogg", // Flag set
            "0x6166a70d.ogg", // Game start
            "0xada5f42b.ogg", // Flag remove
            "0xbfae91fc.ogg" // Direct move
        };

        private readonly static string[] soundListMinigameSoundsExplosion = new string[]
        {
            "0x1d359341.ogg" // Lose
        };

        private readonly static string[] soundListMinigameSoundsJoy = new string[]
        {
            "0xd28af703.ogg" // Win
        };

        private readonly static string[] soundListOtherSoundsAxl = new string[]
        {
            "0x11c8b95e.ogg",
            "0x16a57d47.ogg",
            "0x27157ff8.ogg",
            "0x3c7bd08e.ogg",
            "0x50124f6e.ogg",
            "0x577f8b77.ogg",
            "0x64482fc.ogg",
            "0x762e7673.ogg",
            "0x9f4dd346.ogg",
            "0xa5728134.ogg",
            "0xb971ea5b.ogg",
            "0xce76dacd.ogg",
            "0xe84ae3d0.ogg"
        };

        private readonly static string[] soundListOtherSoundsHelga = new string[]
        {
            "0x2154f305.ogg",
            "0x33b24ecd.ogg",
            "0x36944a76.ogg",
            "0x3d69c6ff.ogg",
            "0x41937ae0.ogg",
            "0x44b57e5b.ogg",
            "0x5653c393.ogg",
            "0xa4609745.ogg",
            "0xaf9d1bcc.ogg",
            "0xb6862a8d.ogg",
            "0xd367a7d3.ogg"
        };

        private readonly static string[] soundListOtherSoundsTBone = new string[]
        {
            "0x2058aa0.ogg",
            "0x61a24dd1.ogg",
            "0x726f7e2f.ogg",
            "0x7502ba36.ogg",
            "0x861a60d6.ogg",
            "0x8fac2cfd.ogg",
            "0x9c611f03.ogg",
            "0xeb662f95.ogg",
            "0xec0beb8c.ogg",
            "0xf8ab1c6b.ogg",
            "0xffc6d872.ogg"
        };

        private readonly static string[] soundListMenuMusicMenuMusic = new string[]
        {
            "0x3403b17a.ogg",
            "0xba8cb699.ogg",
            "0xe517acbe.ogg",
            "0xed23a3d6.ogg",
            "0xf3614c6a.ogg"
        };

        private readonly static string[] soundListMenuMusicShortMusic = new string[]
        {
            "0x44c13f8f.ogg",
            "0x47e69393.ogg",
            "0x7626b607.ogg",
            "0x7f625c47.ogg",
            "0xee6ab6a.ogg"
        };

        private readonly static string[] soundListMenuMusicTaunts = new string[]
        {
            "0x16f76711.ogg",
            "0x1f6eb4b1.ogg",
            "0x277ee713.ogg",
            "0x292b81ba.ogg",
            "0x2d8b103a.ogg",
            "0x350ea5b.ogg",
            "0x3696bd8d.ogg",
            "0x3750e2df.ogg",
            "0x3ea051fc.ogg",
            "0x3eb4cc6c.ogg",
            "0x4e984b50.ogg",
            "0x52d8ce36.ogg",
            "0x5b31206e.ogg",
            "0x6e8bfb6a.ogg",
            "0x7608830b.ogg",
            "0x7ab3d84b.ogg",
            "0x805a64a9.ogg",
            "0xc3aeb681.ogg",
            "0xc80c39fe.ogg"
        };

        private static int axlOrHelgaOrTBoneRefreshRecent;

        private static int menuMusicMenuMusicRecent;

        private static int menuMusicShortMusicRecent;

        private static int menuMusicTauntsRecent;

        private static int axlOrHelgaOrTBoneHitRecent;

        private static bool soundPlayingSuccessSounds;

        public async static void PlayMinigameSoundExplosion()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (MinigameSounds)
                {
                    // This keeps the trigger enabled!
                    await PlaySound(@"\Assets\Sounds\Minigame Sounds\Explosion\", soundListMinigameSoundsExplosion[0], token);
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public async static void PlayMinigameSoundJoy()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (MinigameSounds)
                {
                    // This keeps the trigger enabled!
                    await PlaySound(@"\Assets\Sounds\Minigame Sounds\Joy\", soundListMinigameSoundsJoy[0], token);
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public async static void PlayMinigameSoundTerminalDirectMove()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (MinigameSounds)
                {
                    // This keeps the trigger enabled!
                    await PlaySound(@"\Assets\Sounds\Minigame Sounds\Terminal\", soundListMinigameSoundsTerminal[3], token);
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public async static void PlayMinigameSoundTerminalFlagRemove()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (MinigameSounds)
                {
                    // This keeps the trigger enabled!
                    await PlaySound(@"\Assets\Sounds\Minigame Sounds\Terminal\", soundListMinigameSoundsTerminal[2], token);
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public async static void PlayMinigameSoundTerminalGameStart()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (MinigameSounds)
                {
                    // This keeps the trigger enabled!
                    await PlaySound(@"\Assets\Sounds\Minigame Sounds\Terminal\", soundListMinigameSoundsTerminal[1], token);
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public async static void PlayMinigameSoundTerminalFlagSet()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (MinigameSounds)
                {
                    // This keeps the trigger enabled!
                    await PlaySound(@"\Assets\Sounds\Minigame Sounds\Terminal\", soundListMinigameSoundsTerminal[0], token);
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public async static void PlayMinigameSoundSentry()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (MinigameSounds)
                {
                    // This keeps the trigger enabled!
                    await PlaySound(@"\Assets\Sounds\Minigame Sounds\Sentry\", soundListMinigameSoundsSentry[0], token);
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public async static void PlaySuccessSoundsHitRandomly()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (SuccessSounds && !soundPlayingSuccessSounds)
                {
                    Random randomSound = new Random();
                    int indexSuccessSoundsAxl = randomSound.Next(0, 7); // Creates a number between 0 and 6
                    int indexSuccessSoundsHelga = randomSound.Next(0, 7); // Creates a number between 0 and 6
                    int indexSuccessSoundsTBone = randomSound.Next(0, 7); // Creates a number between 0 and 6
                    int axlOrHelgaOrTBone = randomSound.Next(0, 3); // Creates a number between 0 and 2
                    /* This makes playing the same sound in a row less common. */
                    /* We use a while loop to make it impossible. */
                    while (axlOrHelgaOrTBoneHitRecent == axlOrHelgaOrTBone)
                    {
                        axlOrHelgaOrTBone = randomSound.Next(0, 3);
                    }
                    axlOrHelgaOrTBoneHitRecent = axlOrHelgaOrTBone;
                    soundPlayingSuccessSounds = true;

                    switch (axlOrHelgaOrTBone)
                    {
                        case 0:
                            // This keeps the trigger enabled!
                            await PlaySound(@"\Assets\Sounds\Success Sounds\Axl\", soundListSuccessSoundsAxl[indexSuccessSoundsAxl], token);
                            break;
                        case 1:
                            // This keeps the trigger enabled!
                            await PlaySound(@"\Assets\Sounds\Success Sounds\Helga\", soundListSuccessSoundsHelga[indexSuccessSoundsHelga], token);
                            break;
                        case 2:
                            // This keeps the trigger enabled!
                            await PlaySound(@"\Assets\Sounds\Success Sounds\T-Bone\", soundListSuccessSoundsTBone[indexSuccessSoundsTBone], token);
                            break;
                    }
                    soundPlayingSuccessSounds = false;
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public async static void PlaySuccessSoundsStartRandomly()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (SuccessSounds)
                {
                    Random randomSound = new Random();

                    int indexSuccessSoundsStartMechanical = randomSound.Next(0, 10); // Creates a number between 0 and 9
                    int chance = randomSound.Next(0, 4); // Creates a number between 0 and 3
                    soundPlayingSuccessSounds = true;
                    /* The classic start sound gets a 25% chance to be played. */
                    if (chance == 3)
                    {
                        // This keeps the trigger enabled!
                        await PlaySound(@"\Assets\Sounds\Success Sounds\Start\", soundListSuccessSoundsStart[0], token);
                    }
                    /* Each other sound gets a 7.5% chance to be played */
                    else
                    {
                        // This keeps the trigger enabled!
                        await PlaySound(@"\Assets\Sounds\Success Sounds\Start Mechanical\", soundListSuccessSoundsStartMechanical[indexSuccessSoundsStartMechanical], token);
                    }
                    soundPlayingSuccessSounds = false;
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        // TODO: Turn off instantly on confirm button click (options page view model)
        public async static void PlayMenuMusicRandomly()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                Random randomSound = new Random();
                while (MenuMusic)
                {
                    int indexMenuMusicMenuMusic = randomSound.Next(0, 5); // Creates a number between 0 and 4
                    while (menuMusicMenuMusicRecent == indexMenuMusicMenuMusic || BlockedSong == soundListMenuMusicMenuMusic[indexMenuMusicMenuMusic])
                    {
                        indexMenuMusicMenuMusic = randomSound.Next(0, 5); // Creates a number between 0 and 4
                    }
                    menuMusicMenuMusicRecent = indexMenuMusicMenuMusic;
                    // This keeps the trigger enabled!
                    await PlaySound(@"\Assets\Sounds\Menu Music\Menu Music\", soundListMenuMusicMenuMusic[indexMenuMusicMenuMusic], token);

                    int indexMenuMusicTaunts = randomSound.Next(0, 19); // Creates a number between 0 and 18
                    while (menuMusicTauntsRecent == indexMenuMusicTaunts)
                    {
                        indexMenuMusicTaunts = randomSound.Next(0, 19); // Creates a number between 0 and 18
                    }
                    menuMusicTauntsRecent = indexMenuMusicTaunts;
                    // This keeps the trigger enabled!
                    await PlaySound(@"\Assets\Sounds\Menu Music\Taunts\", soundListMenuMusicTaunts[indexMenuMusicTaunts], token);

                    if (!MenuMusic)
                    {
                        break;
                    }

                    int indexMenuMusicShortMusic = randomSound.Next(0, 5);  // Creates a number between 0 and 4
                    while (menuMusicShortMusicRecent == indexMenuMusicShortMusic || BlockedSong == soundListMenuMusicShortMusic[indexMenuMusicShortMusic])
                    {
                        indexMenuMusicShortMusic = randomSound.Next(0, 5);  // Creates a number between 0 and 4
                    }
                    menuMusicShortMusicRecent = indexMenuMusicShortMusic;
                    // This keeps the trigger enabled!
                    await PlaySound(@"\Assets\Sounds\Menu Music\Short Music\", soundListMenuMusicShortMusic[indexMenuMusicShortMusic], token);

                    indexMenuMusicTaunts = randomSound.Next(0, 19); // Creates a number between 0 and 18
                    while (menuMusicTauntsRecent == indexMenuMusicTaunts)
                    {
                        indexMenuMusicTaunts = randomSound.Next(0, 19); // Creates a number between 0 and 18
                    }
                    // This keeps the trigger enabled!
                    menuMusicTauntsRecent = indexMenuMusicTaunts;
                    await PlaySound(@"\Assets\Sounds\Menu Music\Taunts\", soundListMenuMusicTaunts[indexMenuMusicTaunts], token);
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public async static void PlayOtherSoundsRandomly()
        {
            /* CancellationTokenSource provides the token and have authority to cancel the token */
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                if (OtherSounds)
                {
                    Random randomSound = new Random();
                    int indexOtherSoundsAxl = randomSound.Next(0, 13); // Creates a number between 0 and 12
                    int indexOtherSoundsHelga = randomSound.Next(0, 11); // Creates a number between 0 and 10
                    int indexOtherSoundsTBone = randomSound.Next(0, 11); // Creates a number between 0 and 10
                    int axlOrHelgaOrTBone = randomSound.Next(0, 3); // Creates a number between 0 and 2
                    /* This makes playing the same sound in a row less common. */
                    /* We use a while loop to make it impossible. */
                    while (axlOrHelgaOrTBoneRefreshRecent == axlOrHelgaOrTBone)
                    {
                        axlOrHelgaOrTBone = randomSound.Next(0, 3);
                    }
                    axlOrHelgaOrTBoneRefreshRecent = axlOrHelgaOrTBone;

                    switch (axlOrHelgaOrTBone)
                    {
                        case 0:
                            // This keeps the trigger enabled!
                            await PlaySound(@"\Assets\Sounds\Other Sounds\Axl\", soundListOtherSoundsAxl[indexOtherSoundsAxl], token);
                            break;
                        case 1:
                            // This keeps the trigger enabled!
                            await PlaySound(@"\Assets\Sounds\Other Sounds\Helga\", soundListOtherSoundsHelga[indexOtherSoundsHelga], token);
                            break;
                        case 2:
                            // This keeps the trigger enabled!
                            await PlaySound(@"\Assets\Sounds\Other Sounds\T-Bone\", soundListOtherSoundsTBone[indexOtherSoundsTBone], token);
                            break;
                    }
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        public async static Task PlaySound(string assetPath, string fileName, CancellationToken token)
        {
            try
            {
                string assetPathNew = assetPath.Replace(@"\", ".");
                assetPathNew = assetPathNew.Replace(@" ", "_");
                assetPathNew = assetPathNew.Replace(@"-", "_");

                string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                /* We use the namespace Loadout_Patcher */
                Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "Loadout_Patcher" + assetPathNew + fileName)!;

                // Loadout_Patcher.Assets.Sounds.Success_Sounds.Start_Mechanical.0x28d6968.ogg
                // Assembly.GetExecutingAssembly().GetManifestResourceNames().
                // string resourceString = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath!) + assetPathNew + fileName;
                
                /*
                string baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                //int index = check.IndexOf("Loadout_Patcher");
                int index = baseDirectory.IndexOf(System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName));
                if (index >= 0)
                {
                    baseDirectory = baseDirectory.Substring(0, index) + System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName);
                }
                */

                using (var vorbisStream = new NAudio.Vorbis.VorbisWaveReader(resourceStream, true))
                //using (var vorbisStream = new NAudio.Vorbis.VorbisWaveReader(baseDirectory + assetPath + fileName))
                using (var waveOut = new NAudio.Wave.WaveOutEvent())
                {
                    waveOut.Init(vorbisStream);
                    waveOut.Play();

                    /* We add a delay to actually hear the playback until it stops. */
                    /* We can do everything else in the meanwhile */
                    await Task.Delay(vorbisStream.TotalTime);
                    resourceStream.Dispose();
                }
            }
            catch (TaskCanceledException taskCancelEx)
            {
                Console.WriteLine("> Exception: " + taskCancelEx.GetType().Name);
            }
        }

        private static string? recentSoundPlayed;

        public static string? RecentSoundPlayed
        {
            get { return recentSoundPlayed; }
            set { recentSoundPlayed = value; }
        }

        private static bool successSounds = true;

        public static bool SuccessSounds
        {
            get { return successSounds; }
            set { successSounds = value; }
        }

        private static bool minigameSounds = true;

        public static bool MinigameSounds
        {
            get { return minigameSounds; }
            set { minigameSounds = value; }
        }

        private static bool otherSounds = false;

        public static bool OtherSounds
        {
            get { return otherSounds; }
            set { otherSounds = value; }
        }

        private static bool menuMusic = false;

        public static bool MenuMusic
        {
            get { return menuMusic; }
            set { menuMusic = value; }
        }

        /* All sounds muted won't be part of the save file */
        private static bool allSoundsMuted = false;

        public static bool AllSoundsMuted
        {
            get { return allSoundsMuted; }
            set { allSoundsMuted = value; }
        }

        private static string blockedSong = "None";

        public static string BlockedSong
        {
            get { return blockedSong; }
            set { blockedSong = value; }
        }

        public static void SynchronizeSaveFile(ref Filesave.SaveFile saveFile)
        {
            saveFile.SuccessSounds = SuccessSounds;
            saveFile.MinigameSounds = MinigameSounds;
            saveFile.OtherSounds = OtherSounds;
            saveFile.MenuMusic = MenuMusic;
            saveFile.BlockedSong = BlockedSong;
        }

        public static void LoadSaveFileIntoSoundProperties(Filesave.SaveFile saveFile)
        {
            SuccessSounds = saveFile.SuccessSounds;
            MinigameSounds = saveFile.MinigameSounds;
            OtherSounds = saveFile.OtherSounds;
            MenuMusic = saveFile.MenuMusic;
            BlockedSong = saveFile.BlockedSong;
        }
    }
}
