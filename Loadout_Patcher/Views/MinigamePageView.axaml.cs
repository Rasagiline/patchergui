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
using Avalonia.Input;
using Loadout_Patcher.ViewModels;

namespace Loadout_Patcher.Views;

public partial class MinigamePageView : UserControl
{
    public MinigamePageView()
    {
        /* We play a sentry sound as soon as we click on the minigame tab. */
        Sound.PlayMinigameSoundSentry();
        InitializeComponent();
    }

    private void FieldPressed(object? sender, PointerPressedEventArgs e)
    {
        MinigamePageViewModel fieldViewModel = (MinigamePageViewModel)DataContext!;
        //if (e.GetCurrentPoint(null).Properties.IsRightButtonPressed)
        //{
        //    //fieldViewModel.Position = new Models.Point(0, 0);
        //    fieldViewModel.FieldRightClicked();
        //}
        if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
        { 
            fieldViewModel.FieldLeftClicked(); 
        }
    }
}
