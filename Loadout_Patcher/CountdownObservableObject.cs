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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Loadout_Patcher;

public class CountdownObservableObject : ObservableObject
{
    private string? _currentTimeOfTheDay;
    private string? _refreshServerListText;

    public string CurrentTimeOfTheDay
    {
        get { return _currentTimeOfTheDay!; }
        set { SetProperty(ref _currentTimeOfTheDay, value); }
    }

    public string RefreshServerListText
    {
        get { return _refreshServerListText!; }
        set { SetProperty(ref _refreshServerListText, value); }
    }
}
