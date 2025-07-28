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
using Avalonia.Interactivity;
using Loadout_Patcher.ViewModels;
using FluentAvalonia.UI.Windowing;
using Avalonia.Controls.Notifications;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using System.Reflection;

namespace Loadout_Patcher.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow(ViewModels.MainViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
        /* This must come after the InitializeComponent() */
        Title = GUI.Title;
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }

    // It's not clear for what this is used.
    public MainWindow() : this(new MainViewModel()) { }

    /**
    public static void Test()
    {
        MainWindow asta = new MainWindow();

        var not = new Notification("Test", "this is a test notification message", NotificationType.Success);
        var nm = new WindowNotificationManager(asta)
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 1
        };
        nm.TemplateApplied += (sender, args) =>
        {
            nm.Show(not);
        };

    }
    **/

        // Makes minimum width and height of the main window count for every view except of the splash screen.
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        MinWidth = Width;
        MinHeight = Height;
        SizeToContent = SizeToContent.Manual;

        string assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!;
        var asset = AssetLoader.Open(new Uri($"avares://{assemblyName}/Assets/cursor.png"));
        var bitmap = new Bitmap(asset);
        this.Cursor = new Cursor(bitmap, new PixelPoint(0, 0));
    }


    private static string? _guiTitle;

    public static string GuiTitle
    {
        get { return _guiTitle!; }
        set { _guiTitle = value; }
    }

}
