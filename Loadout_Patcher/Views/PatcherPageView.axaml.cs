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
using System.Runtime.InteropServices;
using static Loadout_Patcher.Filesave;

namespace Loadout_Patcher.Views;

public partial class PatcherPageView : UserControl
{
    public PatcherPageView()
    {
        InitializeComponent();
    }

    private void Button_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        /* The user can always bring Loadout to the foreground by switching to the patcher tab */
        if (ProcessHandling.LoadoutProcess != null || ProcessHandling.LoadoutParentProcess != null)
        {
            /* We focus on the Loadout process since we have patched a map */
            /* Requires user32.dll which allows only Windows users to focus on the game */
            if (OperatingSystem.IsWindows())
            {
                if (ProcessHandling.LoadoutProcess != null)
                {
                    if (!ProcessHandling.LoadoutProcess.HasExited)
                    {
                        if (ProcessHandling.LoadoutProcess.Responding)
                        {
                            ProcessHandling.SetForeground();
                        }
                    }
                }
            }
        }
        /* This can provoke crashes in Program.cs */
        if (ProcessMemory.GetNetInstallation() != RuntimeInformation.FrameworkDescription ||
            ProcessMemory.GetOsDescription() != RuntimeInformation.OSDescription ||
            ProcessMemory.GetProcessorArchitecture() != RuntimeInformation.ProcessArchitecture ||
            ProcessMemory.GetRuntimeId() != RuntimeInformation.RuntimeIdentifier)
        {
            /* We need to update the OS information everytime if there was a change */
            new ProcessMemory();
            // Save file is being updated
            Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
            Filesave.SaveDataToFile(saveFile, true);

            Console.WriteLine("> Operating system information was updated!");
            Console.WriteLine("> Save file changed and saved.\n");
        }
    }
}