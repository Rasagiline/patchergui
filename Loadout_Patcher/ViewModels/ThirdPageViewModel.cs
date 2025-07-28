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
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reflection.Emit;
using Loadout_Patcher.Models;
using System.Collections;

namespace Loadout_Patcher.ViewModels
{
    /// <summary>
    ///  This is our ViewModel for the last page
    /// </summary>
    public partial class ThirdPageViewModel : PageViewModelBase
    {
        [ObservableProperty]
        private bool _isPaneOpen = true;

        [ObservableProperty]
        private ViewModelBase _currentPage = new PatcherPageViewModel();

        [ObservableProperty]
        private ListItemTemplate? _selectedListItem;

        // This for changing the current page!
        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value is null) return;
            var instance = Activator.CreateInstance(value.ModelType);
            if (instance is null) return;
            CurrentPage = (ViewModelBase)instance;


            /** for testing
            string receive = MainProperties.NewEndpoint;
            receive = "TheNewEndpoint";
            MainProperties.NewEndpoint = receive;
            receive = "";
            **/
        }

        /**
        // This might be mandatory for the routing
        public ObservableCollection<ListItemTemplate> Items { get; } = new()
        {
            new ListItemTemplate(typeof(PatcherPageViewModel), "code_regular"),
            new ListItemTemplate(typeof(OptionsPageViewModel), "settings_regular"),
            new ListItemTemplate(typeof(MultiplayerPageViewModel), "people_team_regular"),
            new ListItemTemplate(typeof(SavePageViewModel), "preview_link_regular"),
            new ListItemTemplate(typeof(MapPageViewModel), "image_copy_regular"),
            new ListItemTemplate(typeof(MinigamePageViewModel), "games_regular"),
        };
        **/

        // Displaying the first page that is shown as soon as this page view is open. Here it is PatcherPageViewModel.
        public ThirdPageViewModel()
        {
            Items = new ObservableCollection<ListItemTemplate>(_templates);

            // This is for selecting the first selected page among several
            // Here we select - MainPage
            SelectedListItem = Items.First(vm => vm.ModelType == typeof(PatcherPageViewModel));
        }

        // This might be mandatory for the routing
        public ObservableCollection<ListItemTemplate> Items { get; }

        private readonly List<ListItemTemplate> _templates =
        [
            new ListItemTemplate(typeof(PatcherPageViewModel), "code_regular", "Patch for Game Access"), //Patch to Start
            new ListItemTemplate(typeof(OptionsPageViewModel), "settings_regular", "Options"),
            new ListItemTemplate(typeof(MultiplayerPageViewModel), "people_team_regular", "Multiplayer"),
            new ListItemTemplate(typeof(SavePageViewModel), "preview_link_regular", "Edit your Save"),
            new ListItemTemplate(typeof(MapPageViewModel), "image_copy_regular", "Map Patching"),
            new ListItemTemplate(typeof(MinigamePageViewModel), "games_regular", "Minigame"),
        ];

        [RelayCommand]
        private void TriggerPane()
        {
            IsPaneOpen = !IsPaneOpen;
        }

        // This is our last page, so we can't navigate to the next page in any case
        public override bool CanNavigateNext
        {
            get => false;
            protected set => throw new NotSupportedException();
        }

        // We allow to navigate back in any case
        public override bool CanNavigatePrevious
        {
            get => true;
            protected set => throw new NotSupportedException();
        }
    }

}



