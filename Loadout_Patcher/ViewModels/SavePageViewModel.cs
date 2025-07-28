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
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using static Loadout_Patcher.Filesave;

namespace Loadout_Patcher.ViewModels;

public partial class SavePageViewModel : ViewModelBase
{
    public SavePageViewModel()
    {
        UsernameContent = "";
        NewSaveData = "";
        IpAddressContent = "";
        FavoriteMapContent = "";
        CustomMapContent = "";
        BlockedSongContent = "";

        //this.WhenAnyValue(p_vm => p_vm.FilterComboBoxIndex).Subscribe(_ => GoToPage());

        /*
        Save = new ObservableCollection<SaveObservableObject> { };
        SaveObservableObject SaveProperties = new SaveObservableObject();
        SaveProperties.ValueString = "Hello";
        SaveProperties.PropertyString = "Hello2";
        Save.Add(SaveProperties);
        */

        GuiTitleContent = GUI.Title;
        if (Multiplayer.IpAddress != null)
        {
            IpAddressContent = Multiplayer.IpAddress;
        }
        if (Multiplayer.Username != null)
        {
            UsernameContent = Multiplayer.Username;
        }
        if (Sound.BlockedSong != null)
        {
            BlockedSongContent = Sound.BlockedSong;
        }
        if (Map.PrimaryCustomMap != null)
        {
            CustomMapContent = Map.PrimaryCustomMap;
            if (Map.PrimaryCustomMap != "")
            {
                CustomMapExists = true;
            }
        }
        List<Map.LoadoutMap> favoriteMaps = Map.GetFavoriteMaps();
        if (favoriteMaps != null && favoriteMaps.Count > 0)
        {
            FavoriteMapContent = favoriteMaps[0].FullMapName;
        }
        /* Once we add PatchFavoriteMap as RelayCommand, we may have to change the MapPatchText on success */
        if (MainProperties.NewMap == Map.PrimaryCustomMap)
        {
            MapPatchText = "Custom map patched!";
        }
    }

    /*
    private ObservableCollection<SaveObservableObject>? _save;

    public ObservableCollection<SaveObservableObject> Save
    {
        get { return _save!; }
        set { SetProperty(ref _save, value); }
    }
    */

    [ObservableProperty]
    private string _mapPatchText = "Patch that custom map now";

    [ObservableProperty]
    private string _newSaveData;

    [ObservableProperty]
    private string _guiTitleContent;

    [ObservableProperty]
    private string _ipAddressContent;

    [ObservableProperty]
    private string _usernameContent;

    [ObservableProperty]
    private string _blockedSongContent;

    [ObservableProperty]
    private string _favoriteMapContent;

    [ObservableProperty]
    private string _customMapContent;

    [ObservableProperty]
    private bool _customMapExists = false;

    [Reactive]
    public int FilterComboBoxIndex { get; set; }


    [RelayCommand]
    private void LockIn()
    {
        if (NewSaveData != null && FilterComboBoxIndex != 0)
        {
            switch (FilterComboBoxIndex)
            {
                case 0:
                    /* We do nothing */
                    break;
                case 1:
                    /* We make changes for the GUI title */
                    GUI.Title = NewSaveData;
                    // Save file is being updated
                    Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
                    Filesave.SaveDataToFile(saveFile, true);

                    Console.WriteLine("> A new GUI title was set. Changes will be applied once you restart the patcher.\n");
                    Console.WriteLine("> Save file changed and saved.\n");
                    GuiTitleContent = GUI.Title;
                    break;
                case 2:
                    /* We make changes for the IP address */
                    Multiplayer.IpAddress = NewSaveData;
                    // Save file is being updated
                    saveFile = Filesave.SaveFileBuilder();
                    Filesave.SaveDataToFile(saveFile, true);

                    Console.WriteLine("> A new IP address was set. Restarting the patcher is recommended.\n");
                    Console.WriteLine("> Save file changed and saved.\n");
                    IpAddressContent = Multiplayer.IpAddress;
                    break;
                case 3:
                    /* We make changes for the username */
                    Multiplayer.Username = NewSaveData;
                    // Save file is being updated
                    saveFile = Filesave.SaveFileBuilder();
                    Filesave.SaveDataToFile(saveFile, true);

                    Console.WriteLine("> A new username was set. Restarting the patcher is recommended.\n");
                    Console.WriteLine("> Save file changed and saved.\n");
                    UsernameContent = Multiplayer.Username;
                    break;
                case 4:
                    /* We make changes for the primary custom map */
                    Map.PrimaryCustomMap = NewSaveData;
                    // Save file is being updated
                    saveFile = Filesave.SaveFileBuilder();
                    Filesave.SaveDataToFile(saveFile, true);

                    Console.WriteLine("> A new primary custom map was set.\n");
                    Console.WriteLine("> Save file changed and saved.\n");
                    CustomMapContent = Map.PrimaryCustomMap;
                    if (CustomMapContent != null && CustomMapContent != "")
                    {
                        CustomMapExists = true;
                    }
                    else
                    {
                        CustomMapExists = false;
                    }
                    /* We change the MapPatchText on success */
                    if (MainProperties.NewMap == Map.PrimaryCustomMap)
                    {
                        MapPatchText = "Custom map patched!";
                    }
                    else
                    {
                        MapPatchText = "Patch that custom map now";
                    }
                    break;
                case 5:
                    /* We make changes for the blocked song */
                    Sound.BlockedSong = NewSaveData;
                    // Save file is being updated
                    saveFile = Filesave.SaveFileBuilder();
                    Filesave.SaveDataToFile(saveFile, true);

                    Console.WriteLine("> A new blocked song was set. Restarting the patcher is recommended.\n");
                    Console.WriteLine("> Save file changed and saved.\n");
                    BlockedSongContent = Sound.BlockedSong;
                    break;
            }
            /* We play a random success sound, a melee hit sound, from Axl, Helga or T-Bone if enabled */
            Sound.PlaySuccessSoundsHitRandomly();
        }
    }
    [RelayCommand]
    private void PatchCustomMap()
    {
        if (CustomMapExists && ProcessHandling.LoadoutProcess != null)
        {
            if (ProcessHandling.LoadoutProcess.HasExited)
            {
                Console.WriteLine("> Loadout was closed. Please reopen the game!\n");
            }
            else
            {
                MainProperties.NewMap = Map.PrimaryCustomMap;
                Console.WriteLine("> Patching primary custom map: " + MainProperties.NewMap + "\n");

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
                Console.WriteLine();

                /* We change the MapPatchText on success */
                if (MainProperties.NewMap == Map.PrimaryCustomMap)
                {
                    MapPatchText = "Custom map patched!";
                }
            }
        }
    }
}
