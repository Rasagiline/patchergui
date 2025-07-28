using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
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
using Loadout_Patcher.ViewModels;

namespace Loadout_Patcher.Views
{
    public partial class Field : UserControl
    {
        public Field()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void FieldPressed(object? sender, PointerPressedEventArgs e)
        {
            /*

            FieldViewModel fieldViewModel = (FieldViewModel)DataContext!;
            if (e.GetCurrentPoint(null).Properties.IsRightButtonPressed) fieldViewModel.FieldRightClicked();
            if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed) fieldViewModel.FieldLeftClicked();

            */
        }
    }
}