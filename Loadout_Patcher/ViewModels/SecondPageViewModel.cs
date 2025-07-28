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
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Loadout_Patcher.ViewModels
{
    /// <summary>
    ///  This is a splash view window that reroutes automatically
    /// </summary>
    public partial class SecondPageViewModel : PageViewModelBase
    {

        /**
        public SecondPageViewModel()
        {
            Task.Run(async () =>
            {
                await Task.Delay(10000);
                ThirdPageView tpv = new ThirdPageView();
                tpv.InitializeComponent();
                MainViewModel mvm = new MainViewModel();
                mvm.NavigateNext();
                tpv.InitializeComponent();
                ViewLocator vl = new ViewLocator();
                ThirdPageViewModel tpvm = new ThirdPageViewModel();
                vl.Match(tpvm);
                tpv.InitializeComponent();
                vl.Build(tpvm);
                tpv.InitializeComponent();

            });
        }
        **/

        public SecondPageViewModel()
        {
            // Work with INotify
        }



        /**
        [ObservableProperty]
        private string _welcomeText = "Welcome to Loadout Patcher!";

        public string SaveLoaded => "Save file loaded";  (We want to load the save file before displaying anything)      
        public string Starting => "Starting...";
        public string Version => "Patcher v0.1";
        public string MadeBy => "Made by Reloaded Team";

        [RelayCommand]
        private void ButtonOnClick()
        {
            WelcomeText = "Starting ...";

        }

        **/




        /**
        public string WelcomeText
        {
            get => _welcomeText;
            set
            {
                _welcomeText = value;
                OnPropertyChanged();
            }
        }
        **/



        // Actively display whether or not Loadout is ready.


        // This is our first page, so we can navigate to the next page in any case
        public override bool CanNavigateNext
        {
            get => true;
            protected set => throw new NotSupportedException();
        }

        // You cannot go back from this page
        public override bool CanNavigatePrevious
        {
            get => false;
            protected set => throw new NotSupportedException();
        }


    }
}
