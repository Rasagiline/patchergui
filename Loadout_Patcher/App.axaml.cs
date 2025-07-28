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
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Loadout_Patcher.ViewModels;
using Loadout_Patcher.Views;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Extensions.DependencyInjection;
using System.Xml.Linq;
using Avalonia.Styling;

namespace Loadout_Patcher;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /*
    public void Restart()
    {

    }
    */

    public override void OnFrameworkInitializationCompleted()
    {
        var locator = new ViewLocator();
        DataTemplates.Add(locator);

        var services = new ServiceCollection();
        ConfigureViewModels(services);
        ConfigureViews(services);
        var provider = services.BuildServiceProvider();

        Ioc.Default.ConfigureServices(provider);

        var vm = Ioc.Default.GetRequiredService<MainViewModel>();

        /**
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }
        **/

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // If user enabled options to skip splash screen, don't even load it.
            if (!GUI.SkipStartPage)
            {
                // It's questionable if this helps, but should only be removed if it causes trouble.
                // desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnExplicitShutdown;
                // desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose;
                desktop.MainWindow = new SecondPageView(() =>
                {
                    var mainWindow = new MainWindow()
                    {
                        DataContext = new MainViewModel()
                    };

                    mainWindow.Show();
                    mainWindow.Focus();

                    desktop.MainWindow = new MainWindow(vm);
                });
            }
            else
            {
                desktop.MainWindow = new MainWindow(vm);
            }

        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            // If user enabled options to skip splash screen, don't even load it.
            if (!GUI.SkipStartPage)
            {
                singleViewPlatform.MainView = new SecondPageView(() =>
                {

                    var mainWindow = new MainWindow()
                    {
                        DataContext = new MainViewModel()
                    };

                    mainWindow.Show();
                    mainWindow.Focus();

                    singleViewPlatform.MainView = new MainView { DataContext = vm };

                });
            }
            else
            {
                singleViewPlatform.MainView = new MainView { DataContext = vm };
            }
        }
        RequestedThemeVariant = ThemeVariant.Dark;
        base.OnFrameworkInitializationCompleted();

    }

    [Singleton(typeof(MainViewModel))]
    [Transient(typeof(SecondPageViewModel))]
    [Transient(typeof(ThirdPageViewModel))]
    [Transient(typeof(PatcherPageViewModel))]
    [Transient(typeof(MultiplayerPageViewModel))]
    [Transient(typeof(OptionsPageViewModel))]
    [Transient(typeof(SavePageViewModel))]
    [Transient(typeof(MapPageViewModel))]
    [Transient(typeof(MinigamePageViewModel))]
    internal static partial void ConfigureViewModels(IServiceCollection services);
          



    [Singleton(typeof(MainWindow))]
    [Singleton(typeof(Field))]
    [Transient(typeof(SecondPageView))]
    [Transient(typeof(ThirdPageView))]
    [Transient(typeof(PatcherPageView))]
    [Transient(typeof(MultiplayerPageView))]
    [Transient(typeof(OptionsPageView))]
    [Transient(typeof(SavePageView))]
    [Transient(typeof(MapPageView))]
    [Transient(typeof(MinigamePageView))]
    internal static partial void ConfigureViews(IServiceCollection services);
        

}
