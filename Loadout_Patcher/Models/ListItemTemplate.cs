using System;
using Avalonia;
using Avalonia.Controls;
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

namespace Loadout_Patcher.Models;

public class ListItemTemplate
{
    public ListItemTemplate(Type type, string iconKey, string label)
    {
        ModelType = type;
        Label = label;

        // Label = Type.GetType(type.Name.Replace("PageViewModel", ""))

        Application.Current!.TryFindResource(iconKey, out var res);
        ListItemIcon = (StreamGeometry)res!;

        // Eventually useful
        // var streamGeometry = res as StreamGeometry ?? StreamGeometry.Parse(StreamGeometryNotFound);
        // ListItemIcon = streamGeometry;
    }

    public Type ModelType { get; }
    public string Label { get; }
    public StreamGeometry ListItemIcon { get; }
}
