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
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Loadout_Patcher.ViewModels;

public class MainViewModel : ViewModelBase, IReactiveObject
{
    public MainViewModel()
    {
        // Set current page to first on start up
        _CurrentPage = Pages[0];

        // Create Observables which will activate to deactivate our commands based on CurrentPage state
        var canNavNext = this.WhenAnyValue(x => x.CurrentPage.CanNavigateNext);
        var canNavPrev = this.WhenAnyValue(x => x.CurrentPage.CanNavigatePrevious);

        NavigateNextCommand = ReactiveCommand.Create(NavigateNext, canNavNext);
        NavigatePreviousCommand = ReactiveCommand.Create(NavigatePrevious, canNavPrev);
    }

    // A read-only array of possible pages
    private readonly PageViewModelBase[] Pages =
    {
            //new SecondPageViewModel(),
            new ThirdPageViewModel()
     };

    // The default is the first page
    private PageViewModelBase _CurrentPage;

    /// <summary>
    /// Gets the current page. The property is read-only
    /// </summary>
    public PageViewModelBase CurrentPage
    {
        get { return _CurrentPage; }
        private set { this.RaiseAndSetIfChanged(ref _CurrentPage, value); }
        // this.RaiseAndSetIfChanged(ref _CurrentPage, value);
    }

    /// <summary>
    /// Gets a command that navigates to the next page
    /// </summary>
    public ICommand NavigateNextCommand { get; }

    private void NavigateNext()
    {
        // get the current index and add 1
        var index = Pages.IndexOf(CurrentPage) + 1;

        //  /!\ Be aware that we have no check if the index is valid. You may want to add it on your own. /!\
        CurrentPage = Pages[index];
    }

    /// <summary>
    /// Gets a command that navigates to the previous page
    /// </summary>
    public ICommand NavigatePreviousCommand { get; }

    private void NavigatePrevious()
    {
        // get the current index and subtract 1
        var index = Pages.IndexOf(CurrentPage) - 1;

        //  /!\ Be aware that we have no check if the index is valid. You may want to add it on your own. /!\
        CurrentPage = Pages[index];
    }

    public void RaisePropertyChanging(PropertyChangingEventArgs args)
    {
        throw new NotImplementedException();
    }

    public void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        throw new NotImplementedException();
    }
}
