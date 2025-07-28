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
using Avalonia.Media;
using FluentAvalonia.UI.Windowing;
using Loadout_Patcher.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadout_Patcher
{
    public class ComplexSplashScreen : IApplicationSplashScreen
    {
        /*
        public ComplexSplashScreen()
        {
            //SplashScreenContent = new SomeSplashScreenView();
        }
        */

        public string AppName => "";

        public IImage? AppIcon => null;

        public object ?SplashScreenContent { get; }

        public int MinimumShowTime => 0;

        public async Task RunTasks(CancellationToken token)
        {
            await Task.Delay(0);
            //await ((SomeSplashScreenView)SplashScreenContent).InitApp();
        }
        

    }
}
