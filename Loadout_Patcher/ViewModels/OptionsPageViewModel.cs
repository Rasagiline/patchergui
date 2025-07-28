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
using Avalonia.Threading;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Loadout_Patcher.Views;
using Avalonia.Controls.Notifications;
using Avalonia.Controls;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;


namespace Loadout_Patcher.ViewModels;

public partial class OptionsPageViewModel : ViewModelBase
{

    public OptionsPageViewModel()
    {
        // TODO: If checkbox StartLoadout true and checkbox StartViaSSE false, it will still be started via SSE if Steam isn't ready
        // This needs a good amount of console messages
        StartSkipSaveFileData = GUI.SkipStartPage;
        InstantPatchingSaveFileData = GUI.InstantPatching;
        StartLoadoutSaveFileData = GUI.StartLoadout;
        StartLoadoutViaSSESaveFileData = GUI.StartLoadoutViaSSE;
        CreateSSEShortcutSaveFileData = GUI.CreateSSEShortcut;
        SuccessSoundsSaveFileData = Sound.SuccessSounds;
        MinigameSoundsSaveFileData = Sound.MinigameSounds;
        OtherSoundsSaveFileData = Sound.OtherSounds;
        MenuMusicSaveFileData = Sound.MenuMusic;
        AreAllSoundsMuted = MuteAllSoundsFalse;
        ResetStaticFalse = false;
        ApiStaticFalse = false;
        CurrentPrimaryApi = ProcessMemory.GetWebApiEndpoints()[0];
        CurrentApiList = ProcessMemory.GetWebApiEndpoints();
        WebApiCheckboxText = "Change primary API (" + CurrentPrimaryApi + " found)";

        ShowWebApiEndpoints();
    }

    [ObservableProperty]
    private bool _startLoadoutSaveFileData = GUI.StartLoadout;

    [ObservableProperty]
    private bool _startLoadoutViaSSESaveFileData = GUI.StartLoadoutViaSSE;

    [ObservableProperty]
    private bool _createSSEShortcutSaveFileData = GUI.CreateSSEShortcut;

    [ObservableProperty]
    private bool _startLoadoutEnabled = OperatingSystem.IsWindows();

    [ObservableProperty]
    private bool _startLoadoutViaSSEEnabled = OperatingSystem.IsWindows();

    [ObservableProperty]
    private bool _createSSEShortcutEnabled = OperatingSystem.IsWindows();

    // -- //

    [ObservableProperty]
    private bool _areAllSoundsMuted;

    // TODO: Checkbox booleans may be redundant
    [ObservableProperty]
    private bool _successSoundsCheckbox = true;

    [ObservableProperty]
    private bool _minigameSoundsCheckbox = true;

    [ObservableProperty]
    private bool _otherSoundsCheckbox = false;

    [ObservableProperty]
    private bool _menuMusicCheckbox = false;

    [ObservableProperty]
    private bool _muteAllSoundsCheckbox = false;

    [ObservableProperty]
    private bool _successSoundsSaveFileData = true;

    [ObservableProperty]
    private bool _minigameSoundsSaveFileData = true;

    [ObservableProperty]
    private bool _otherSoundsSaveFileData = false;

    [ObservableProperty]
    private bool _menuMusicSaveFileData = false;

    [ObservableProperty]
    private bool _muteAllSoundsFalse = false;

    // -- //

    [ObservableProperty]
    private bool _startSkipCheckbox = GUI.SkipStartPage;

    [ObservableProperty]
    private bool _resetPatcherCheckbox = false;

    [ObservableProperty]
    private bool _primaryApiCheckbox = false;

    [ObservableProperty]
    private bool _startSkipSaveFileData = false;

    [ObservableProperty]
    private bool _instantPatchingSaveFileData = GUI.InstantPatching;

    [ObservableProperty]
    private bool _resetStaticFalse = false;

    [ObservableProperty]
    private bool _apiStaticFalse = false;

    [ObservableProperty]
    private bool _webApiCheckboxIsChecked = false;

    [ObservableProperty]
    private string _applyChangesName = "";

    [ObservableProperty]
    private string _apiChangesName = "";

    [ObservableProperty]
    private bool _resetPatcherFixed = true;

    [ObservableProperty]
    private bool _startSkipFixed = true;

    [ObservableProperty]
    private bool _primaryApiFixed = true;

    [ObservableProperty]
    private string _currentPrimaryApi;

    [ObservableProperty]
    private List<string> _currentApiList;

    [ObservableProperty]
    private bool _webApiCheckButton;

    // Displays Change primary API (" + CurrentPrimaryApi + " found)
    [ObservableProperty]
    private string _webApiCheckboxText;


    // to ObservableCollection
    [ObservableProperty]
    public List<string> _webApiEndpoints = ProcessMemory.GetWebApiEndpoints();

    public ObservableCollection<string>? WebApiList { get; set; }

    [ObservableProperty]
    public bool _listBoxWebApiCheckBox;

    [ObservableProperty]
    private string? _newApi;

    [ObservableProperty]
    private string? _newApiTextInput;





    [ObservableProperty]
    private string? _selectedItem; // The selected items needs to be displayed instantly




    // Do not bind to this property if your Items collection contains duplicates as it is impossible to distinguish between duplicate values.
    /**
    public string SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    private void RaiseAndSetIfChanged(ref string selectedItem, string value)
    {
        throw new NotImplementedException();
    }
    **/






    [RelayCommand]
    private void ShowWebApiEndpoints()
    {
        WebApiList = new ObservableCollection<string>();
        for (var i = 0; i < WebApiEndpoints.Count; i++)
        {
            WebApiList.Add(WebApiEndpoints[i]);
        }
        WebApiList.CollectionChanged += (sender, args) =>
        {
            if (args.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove)
            {
                RemoveWebApiCommand.NotifyCanExecuteChanged();
            }
        };

        // This might be redundant. Issue here: WebApiEndpoints comes from save file data that isn't the same as WebApiList!
        WebApiEndpoints = WebApiList.ToList<string>();
    }


    /**
    [ObservableProperty]
    public List<string> _webApiEndpoints = new List<string>()
    {
        "api.loadout.rip",
        "loady.loadout.z",
        "rat.loadout.rat"
    };
    **/



    [RelayCommand]
    private void AddNewWebApi()
    {
        // The new api text is being inserted at position 0 while all other api names dive 1 index deeper.
        WebApiEndpoints.Insert(0, NewApiTextInput!); // conflict with WebApiList

        ProcessMemory.SetWebApiEndpoints(WebApiEndpoints);

        ShowWebApiEndpoints();

        // WebApiList.Add($"{Random.Shared.Next(0, 100)}");
    }

    [RelayCommand(CanExecute = nameof(CanDeleteWebApi))]
    private void RemoveWebApi()
    {
        WebApiEndpoints.RemoveAt(WebApiList!.IndexOf(WebApiList.Last()));

        ShowWebApiEndpoints();
    }

    private bool CanDeleteWebApi()
    {
        return WebApiList!.Count > 0;
    }


    [RelayCommand]
    private void PrimaryWebApi()
    {
        // NewApiTextInput != ""
        if (SelectedItem != null)
        {
            // WebApiCheckboxIsChecked = true;
            WebApiEndpoints.Remove(SelectedItem);

            WebApiEndpoints.Insert(0, SelectedItem);

            // The list needs a refresh afterwards!
            ShowWebApiEndpoints();

            Console.WriteLine("> List of web API endpoints changed. Awaiting click on Confirm to save changes.\n");

            // See: https://docs.avaloniaui.net/docs/0.10.x/controls/listbox
            // See: https://github.com/AvaloniaUI/Avalonia/discussions/11865
            // See: https://docs.avaloniaui.net/docs/basics/data/data-binding/compiled-bindings#type-casting

            // The new api text is being inserted at position 0 while all other api names dive 1 index deeper.
            //WebApiEndpoints.Insert(0, NewApiTextInput);
        }


        // Console.WriteLine(NewApi + "\n");
    }


    [RelayCommand]
    private void WebApiButtonPressed()
    {
        Console.WriteLine("ButtonPressed");
    }


    /**
    [RelayCommand]
    private void ButtonOnClick()
    {
        ApplyChangesName = "Proceed with changed options?";

    }
    **/

    [ObservableProperty]
    private bool _applyButtonPressed = false;

    [RelayCommand]
    private void ButtonApplyPressed()
    {
        if (StartSkipSaveFileData != GUI.SkipStartPage || InstantPatchingSaveFileData != GUI.InstantPatching || StartLoadoutSaveFileData != GUI.StartLoadout || StartLoadoutViaSSESaveFileData != GUI.StartLoadoutViaSSE ||
            CreateSSEShortcutSaveFileData != GUI.CreateSSEShortcut || ResetStaticFalse == true || ApiStaticFalse == true || SuccessSoundsSaveFileData != Sound.SuccessSounds || 
            MinigameSoundsSaveFileData != Sound.MinigameSounds || OtherSoundsSaveFileData != Sound.OtherSounds || MenuMusicSaveFileData != Sound.MenuMusic || 
            MuteAllSoundsFalse == true)
        {
            ResetPatcherFixed ^= true;
            StartSkipFixed ^= true;
            PrimaryApiFixed ^= true;
            ApplyButtonPressed ^= true; // Way to invert the boolean

            // If you change this to GUI.SkipStartPage, you can see that GUI.SkipStartPage isn't updated

            if (ApplyChangesName == "")
            {
                // If one of the options actually changed
                ApplyChangesName = "Proceed with changed options?";
            }
            else
            {
                ApplyChangesName = "";
            }
            ButtonApplyPressedApi();
        }
    }

    [ObservableProperty]
    private bool _applyButtonPressedApi = false;


    [RelayCommand]
    private void ButtonApplyPressedApi()
    {
        if (ApiStaticFalse)
        {
            // This time, ApplyButtonPressedApi gets either true or false in the else case. This is mandatory.
            ApplyButtonPressedApi = true;
            
            ApiChangesName = "Your current API is: " + CurrentPrimaryApi + Environment.NewLine +
                "Select from other APIs or type in your new API." + Environment.NewLine + 
                "Press Confirm when you're done!";
            // Make textblock and text input field visible


            /**
            numbers = new ObservableCollection<string>();
            for (var i = 0; i < 10; i++)
            {
                numbers.Add($"{Random.Shared.Next(0, 100)}");
            }
            numbers.CollectionChanged += (sender, args) =>
            {
            };
            **/


        }
        else
        {
            ApplyButtonPressedApi = false;
            ApiChangesName = "";
        }
    }


    [RelayCommand]
    private void ButtonResetPressed()
    {
        ApplyButtonPressed = false; // Way to invert the boolean
        ApplyChangesName = "";
        StartSkipSaveFileData = GUI.SkipStartPage;
        InstantPatchingSaveFileData = GUI.InstantPatching;
        StartLoadoutSaveFileData = GUI.StartLoadout;
        StartLoadoutViaSSESaveFileData = GUI.StartLoadoutViaSSE;
        CreateSSEShortcutSaveFileData = GUI.CreateSSEShortcut;
        SuccessSoundsSaveFileData = Sound.SuccessSounds;
        MinigameSoundsSaveFileData = Sound.MinigameSounds;
        OtherSoundsSaveFileData = Sound.OtherSounds;
        MenuMusicSaveFileData = Sound.MenuMusic;
        if (SuccessSoundsSaveFileData || MinigameSoundsSaveFileData || OtherSoundsSaveFileData || MenuMusicSaveFileData)
        {
            MuteAllSoundsFalse = false;
        }
        else
        {
            MuteAllSoundsFalse = true;
        }
        // The reset button will not be reset before the reset method. It will be handled separately.
        // ResetStaticFalse = false;
        ApiStaticFalse = false;

        ResetPatcherFixed = true;
        StartSkipFixed = true;
        PrimaryApiFixed = true;
    }


    [RelayCommand]
    private void ConfirmExecute()
    {
        // We make an edit to our save file, comparing with GUI.SkipStartPage because it can get from true to false and false to true
        if (StartSkipSaveFileData != GUI.SkipStartPage || InstantPatchingSaveFileData != GUI.InstantPatching || StartLoadoutSaveFileData != GUI.StartLoadout || 
            StartLoadoutViaSSESaveFileData != GUI.StartLoadoutViaSSE || CreateSSEShortcutSaveFileData != GUI.CreateSSEShortcut || SuccessSoundsSaveFileData != Sound.SuccessSounds ||
            MinigameSoundsSaveFileData != Sound.MinigameSounds || OtherSoundsSaveFileData != Sound.OtherSounds || MenuMusicSaveFileData != Sound.MenuMusic
            || AreAllSoundsMuted != MuteAllSoundsFalse)
        {
            // GUI properties are being updated
            GUI.SkipStartPage = StartSkipSaveFileData;
            GUI.InstantPatching = InstantPatchingSaveFileData;
            GUI.StartLoadout = StartLoadoutSaveFileData;
            GUI.StartLoadoutViaSSE = StartLoadoutViaSSESaveFileData;
            GUI.CreateSSEShortcut = CreateSSEShortcutSaveFileData;
            if (MuteAllSoundsFalse)
            {
                SuccessSoundsSaveFileData = false;
                MinigameSoundsSaveFileData = false;
                OtherSoundsSaveFileData = false;
                MenuMusicSaveFileData = false;
            }
            // Sound properties are being updated
            Sound.SuccessSounds = SuccessSoundsSaveFileData;
            Sound.MinigameSounds = MinigameSoundsSaveFileData;
            Sound.OtherSounds = OtherSoundsSaveFileData;
            /* We play menu music immediately as soon as we find out the user checked the menu music checkbox */
            if (!Sound.MenuMusic && Sound.MenuMusic != MenuMusicSaveFileData)
            {
                /* We assign the value twice but it's the best way */
                Sound.MenuMusic = MenuMusicSaveFileData;
                Sound.PlayMenuMusicRandomly();
            }
            Sound.MenuMusic = MenuMusicSaveFileData;

            /* This might be redundant as soon as the checkboxes can be disabled via binding */
            if (OperatingSystem.IsLinux() && GUI.StartLoadout || OperatingSystem.IsLinux() && GUI.StartLoadoutViaSSE || OperatingSystem.IsLinux() && GUI.CreateSSEShortcut)
            {
                GUI.StartLoadout = false;
                GUI.StartLoadoutViaSSE = false;
                GUI.CreateSSEShortcut = false;
                Console.WriteLine("> Options about autostarting Loadout or creating a shortcut are not available on Linux!\n");
            }

            // Save file is being updated
            Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
            Filesave.SaveDataToFile(saveFile, true);

            Console.WriteLine("> Save file changed and saved.\n");
        }


        // We change the primary web API
        if (ApiStaticFalse)
        {
            ApiChangesName = "";

            // This might be used too often in this class
            ProcessMemory.SetWebApiEndpoints(WebApiEndpoints);

            // Save file is being updated
            Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
            Filesave.SaveDataToFile(saveFile, true);

            Console.WriteLine("> Save file changed and saved.\n");

            // The list of api endpoints is being refreshed
            ShowWebApiEndpoints();
        }
        else
        {
            ApiChangesName = "Your current API is: " + CurrentPrimaryApi + Environment.NewLine +
            "Select from other APIs or type in your new API.";
            // TODO: Make textblock and text input field visible
        }

        ButtonResetPressed();

        // We reset the patcher at last!
        if (ResetStaticFalse)
        {
            ResetStaticFalse = false;

            Console.WriteLine("> Resetting ...\n");

            /* Reset inserted. This can be used in multiple places */
            MainProperties.Patched = false;
            MainProperties.Reset();

            Console.WriteLine("> Reset finished. Go and patch again.\n");
        }        

    }
}
