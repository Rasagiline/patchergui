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

namespace Loadout_Patcher;

/// <summary>
/// This class uses InterprocessFilesave.cs
/// </summary>
public class InterprocessCommunication
{
    public static (nint, int) OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId)
    {
        // TODO: Write the parameters into the interprocess save file
        // Request nint and int for GetLastError() both!
        nint i = 0;
        int j = 0;


        return (i, j);
    }











}
