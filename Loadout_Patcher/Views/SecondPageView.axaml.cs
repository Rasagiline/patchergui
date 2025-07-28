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
using System.Collections;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Windowing;

namespace Loadout_Patcher.Views;

// It has become a window, so it must inherit from Window
public partial class SecondPageView : Window // Changing Window to AppWindow causes a significant difference
{
    private readonly Action? _mainAction;

    public SecondPageView() { }

    public SecondPageView(Action mainAction)
    {
        InitializeComponent();
        _mainAction = mainAction;
        //A complex splash screen mustn't inherit from Window/AppWindow
        //SplashScreen = new ComplexSplashScreen();
        SplashScreenTime();
    }

    /**
    // Add this and remove DummyLoad(); above!
    protected override void OnLoaded()
    {
        DummyLoad();
    }
    **/

    // Close the splash screen after a ~3 seconds break
    private async void SplashScreenTime()
    {
        /* From Avalonia UI - 09 - Making a splash screen */

        long start = DateTime.Now.Ticks;
        long time = start;
        int progressValue = 0;

        /* This is our start sound, 25% chance to use Loadout's iconic splash sound */
        Sound.PlaySuccessSoundsStartRandomly();
        /* Do some background stuff here. Part 1 */
        await Task.Delay(450);

        while ((time - start) < TimeSpan.TicksPerSecond)
        {
            progressValue++;
            Dispatcher.UIThread.Post(() => LoadingBar.Value = progressValue);
            await Task.Delay(36);
            time = DateTime.Now.Ticks;
        }

        start = time;
        var limit = TimeSpan.TicksPerSecond * 1.1;
        while ((time - start) < limit)
        {
            progressValue += 1;
            Dispatcher.UIThread.Post(() => LoadingBar.Value = progressValue);
            await Task.Delay(26);
            time = DateTime.Now.Ticks;
        }

        while (progressValue < 100)
        {
            progressValue += 1;
            Dispatcher.UIThread.Post(() => LoadingBar.Value = progressValue);
            await Task.Delay(18);
        }

        /* Do some background stuff here. Part 2 */
        await Task.Delay(50);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _mainAction?.Invoke();
            /* It closes the window. Apparently this fits for a splash screen. */
            Close();
        });
    }

    /**
    public async Task InitApp()
    {
        var start = DateTime.Now.Ticks;
        var time = start;
        var progressValue = 0;

        while ((time - start) < TimeSpan.TicksPerSecond)
        {
            progressValue++;
            await Task.Delay(100);
            time = DateTime.Now.Ticks;
        }

        start = time;
        var limit = TimeSpan.TicksPerSecond * 1.1;
        while ((time - start) < limit)
        {
            progressValue += 1;
            await Task.Delay(150);
            time = DateTime.Now.Ticks;
        }

        while (progressValue < 100)
        {
            progressValue += 1;
            await Task.Delay(10);
        }
    }
    **/

}
