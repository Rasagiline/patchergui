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
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.DependencyInjection;
using Loadout_Patcher.ViewModels;
using Loadout_Patcher.Views;
using System;

namespace Loadout_Patcher
{
    public class ViewLocator : IDataTemplate
    {
        private readonly Dictionary<Type, Func<Control?>> _locator = new();

        public ViewLocator()
        {
            RegisterViewFactory<MainViewModel, MainWindow>();
            RegisterViewFactory<SecondPageViewModel, SecondPageView>();
            RegisterViewFactory<ThirdPageViewModel, ThirdPageView>();
            RegisterViewFactory<PatcherPageViewModel, PatcherPageView>();
            RegisterViewFactory<MultiplayerPageViewModel, MultiplayerPageView>();
            RegisterViewFactory<OptionsPageViewModel, OptionsPageView>();
            RegisterViewFactory<SavePageViewModel, SavePageView>();
            RegisterViewFactory<MapPageViewModel, MapPageView>();
            RegisterViewFactory<MinigamePageViewModel, MinigamePageView>();
        }


        public Control Build(object? data)
        {
            if (data is null)
            {
                return new TextBlock { Text = "data was null" };
            }

            _locator.TryGetValue(data.GetType(), out var factory);

            return factory?.Invoke() ?? new TextBlock { Text = $"VM Not registered: {data.GetType().Name}" };



            /**
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
            **/
        }


        private void RegisterViewFactory<TViewModel, TView>()
        where TViewModel : class
        where TView : Control
        => _locator.Add(
            typeof(TViewModel),
            Design.IsDesignMode
                ? Activator.CreateInstance<TView>
                : Ioc.Default.GetService<TView>);


        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
