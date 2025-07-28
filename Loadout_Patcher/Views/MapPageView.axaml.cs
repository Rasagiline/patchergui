using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Media.Imaging;
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
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Kernel;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace Loadout_Patcher.Views;

public partial class MapPageView : UserControl
{
    public MapPageView()
    {
        InitializeComponent();
        string assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!;
        var asset = AssetLoader.Open(new Uri($"avares://{assemblyName}/Assets/cursorJuice.png"));
        var bitmap = new Bitmap(asset);
        this.MapPatchingTable.Cursor = new Cursor(bitmap, new PixelPoint(18, 0));
        this.MapPatchingTable2.Cursor = new Cursor(bitmap, new PixelPoint(18, 0));
        this.MapPatchingTable3.Cursor = new Cursor(bitmap, new PixelPoint(18, 0));
        this.MapPatchingTable4.Cursor = new Cursor(bitmap, new PixelPoint(18, 0));
        this.MapPatchingTable5.Cursor = new Cursor(bitmap, new PixelPoint(18, 0));
        //DataGrid dataGrid1 = new DataGrid();
        //dataGrid1 = this.MapPatchingTable5;
        //Dispatcher.UIThread.InvokeAsync(() =>
        //    this.MapPatchingTable5.Columns.First().Sort(System.ComponentModel.ListSortDirection.Ascending));
        //Dispatcher.UIThread.InvokeAsync(() =>
        //    this.MapPatchingTable5.Columns.Order());
        //this.MapPatchingTable5.CurrentColumn.Sort(System.ComponentModel.ListSortDirection.Ascending);
        //this.MapPatchingTable5.Columns.OrderBy()
        /*
        MapPicture.GestureRecognizers.Add(new ScrollGestureRecognizer()
        {
            CanVerticallyScroll = true,
            CanHorizontallyScroll = true,
        });
        */
    }

    private void OnDataGridTemplateApplied(object? sender, TemplateAppliedEventArgs args)
    {
        var dataGrid = (DataGrid)sender!;
        dataGrid.Columns.Last().Sort(System.ComponentModel.ListSortDirection.Ascending);
        dataGrid.TemplateApplied -= OnDataGridTemplateApplied;
    }

    private void Button_ResourcesChanged(object? sender, ResourcesChangedEventArgs args)
    {
        /* We sort when any of the map queue buttons change, gets triggered unusually often */
        Dispatcher.UIThread.InvokeAsync(() =>
            this.FindControl<DataGrid>("MapPatchingTable5")!.Columns.Last().Sort(System.ComponentModel.ListSortDirection.Ascending));
    }

    /*
    private void OnDataGridCellPointerPressed(object? sender, Avalonia.Controls.DataGridCellPointerPressedEventArgs args)
    {

    }
    */

    /*
    private void OnDataGridCurrentCellChanged(object? sender, TemplateAppliedEventArgs args)
    {
        var dataGrid = (DataGrid)sender!;
        dataGrid.Columns.Last().Sort();
        dataGrid.TemplateApplied -= OnDataGridCurrentCellChanged;
    }
    private void DataGrid_CellEditEnded(object? sender, DataGridCellEditEndedEventArgs args)
    {
        var dataGrid = (DataGrid)sender!;
        dataGrid.Columns.Last().Sort();
        dataGrid.TemplateApplied -= OnDataGridCurrentCellChanged;
    }
    */
    // public object MapPicture { get; }
    /*
    private static void SortColumn(DataGrid dataGrid, int columnIndex)
    {
        
        var performSortMethod = typeof(DataGrid)
                                .GetMethod("PerformSort",
                                           BindingFlags.Instance | BindingFlags.NonPublic);

        performSortMethod?.Invoke(dataGrid, new[] { dataGrid.Columns[columnIndex] });
        
    }
    */
}
