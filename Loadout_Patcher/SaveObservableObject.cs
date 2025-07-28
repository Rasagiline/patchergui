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
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Loadout_Patcher
{
    /* This class is currently not in use in SavePageViewModel */
    public class SaveObservableObject : ObservableObject
    {
        private string? _valueString;
        private string? _propertyString;

        public string ValueString
        {
            get { return _valueString!; }
            set { SetProperty(ref _valueString, value); }
        }

        public string PropertyString
        {
            get { return _propertyString!; }
            set { SetProperty(ref _propertyString, value); }
        }
    }
}

