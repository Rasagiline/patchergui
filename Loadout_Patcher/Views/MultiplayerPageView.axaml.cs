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
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using Loadout_Patcher.ViewModels;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Loadout_Patcher.Views;

public partial class MultiplayerPageView : UserControl
{
    public MultiplayerPageView()
    {
        InitializeComponent();
        string assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!;
        var asset = AssetLoader.Open(new Uri($"avares://{assemblyName}/Assets/cursorJuice.png"));
        var bitmap = new Bitmap(asset);
        this.ServerListTable.Cursor = new Cursor(bitmap, new PixelPoint(18, 0));
        this.FavoriteServerListTable.Cursor = new Cursor(bitmap, new PixelPoint(18, 0));
        this.DataContext = multiplayerPageVM;
        // -
        // -
        // this.Resources["FavoriteServerIcon"] = _starAdd;
    }
    //private Geometry _starAdd = Geometry.Parse("M17.5,12 C20.5375661,12 23,14.4624339 23,17.5 C23,20.5375661 20.5375661,23 17.5,23 C14.4624339,23 12,20.5375661 12,17.5 C12,14.4624339 14.4624339,12 17.5,12 Z M12.6728137,2.75963841 L15.3563695,8.20794048 L21.3672771,9.07653583 C21.9828509,9.16548822 22.2288847,9.92194118 21.7834166,10.355994 L20.4150744,11.6887401 C19.9202594,11.4400424 19.3893043,11.2526832 18.8321893,11.1366429 L19.6468361,10.3435066 L14.7508503,9.63602099 C14.5061256,9.60065749 14.2945567,9.44694368 14.1853,9.2251246 L12,4.78840895 L9.81470005,9.2251246 C9.70544328,9.44694368 9.49387437,9.60065749 9.24914969,9.63602099 L4.35428754,10.3433442 L7.89856004,13.7927085 C8.07576033,13.9651637 8.15657246,14.2138779 8.11458106,14.4575528 L7.27468983,19.3314183 L11.0013001,17.3686786 C11.0004347,17.4123481 11,17.4561233 11,17.5 C11,18.0076195 11.0581888,18.5016483 11.1682599,18.9757801 L6.6265261,21.3681981 C6.07605002,21.6581549 5.43223188,21.1903936 5.53789075,20.5772582 L6.56928006,14.5921347 L2.21690124,10.3563034 C1.77102944,9.92237116 2.01694609,9.16551755 2.63272295,9.07653583 L8.6436305,8.20794048 L11.3271863,2.75963841 C11.6020985,2.20149668 12.3979015,2.20149668 12.6728137,2.75963841 Z M17.5,13.9992349 L17.4101244,14.0072906 C17.2060313,14.0443345 17.0450996,14.2052662 17.0080557,14.4093593 L17,14.4992349 L16.9996498,16.9992349 L14.4976498,17 L14.4077742,17.0080557 C14.2036811,17.0450996 14.0427494,17.2060313 14.0057055,17.4101244 L13.9976498,17.5 L14.0057055,17.5898756 C14.0427494,17.7939687 14.2036811,17.9549004 14.4077742,17.9919443 L14.4976498,18 L17.0006498,17.9992349 L17.0011076,20.5034847 L17.0091633,20.5933603 C17.0462073,20.7974534 17.207139,20.9583851 17.411232,20.995429 L17.5011076,21.0034847 L17.5909833,20.995429 C17.7950763,20.9583851 17.956008,20.7974534 17.993052,20.5933603 L18.0011076,20.5034847 L18.0006498,17.9992349 L20.5045655,18 L20.5944411,17.9919443 C20.7985342,17.9549004 20.9594659,17.7939687 20.9965098,17.5898756 L21.0045655,17.5 L20.9965098,17.4101244 C20.9594659,17.2060313 20.7985342,17.0450996 20.5944411,17.0080557 L20.5045655,17 L17.9996498,16.9992349 L18,14.4992349 L17.9919443,14.4093593 C17.9549004,14.2052662 17.7939687,14.0443345 17.5898756,14.0072906 L17.5,13.9992349 Z");
    //private Geometry _starRemove = Geometry.Parse("M3.28033 2.21967C2.98744 1.92678 2.51257 1.92677 2.21967 2.21967C1.92678 2.51256 1.92677 2.98743 2.21967 3.28033L8.6437 9.70446L3.63623 10.4321C3.41859 10.4637 3.21745 10.5662 3.06394 10.7237C2.67844 11.1192 2.68653 11.7523 3.08202 12.1378L7.81294 16.7493L6.69612 23.2608C6.65894 23.4776 6.69426 23.7006 6.7966 23.8952C7.0536 24.3841 7.65822 24.572 8.14707 24.315L13.9948 21.2407L19.8425 24.315C20.0372 24.4174 20.2602 24.4527 20.4769 24.4155C21.0213 24.3221 21.3868 23.8052 21.2935 23.2608L21.1058 22.1668L24.7194 25.7805C25.0123 26.0734 25.4872 26.0734 25.7801 25.7805C26.073 25.4876 26.073 25.0127 25.7801 24.7198L3.28033 2.21967ZM19.2689 20.3298L19.6438 22.5159L13.9948 19.546L8.34579 22.5159L9.42466 16.2256L4.85451 11.7708L9.96712 11.0279L19.2689 20.3298Z M12.1169 8.93511L10.9969 7.81512L13.0981 3.55767C13.3425 3.06241 13.9421 2.85907 14.4374 3.1035C14.6346 3.20083 14.7942 3.36045 14.8915 3.55767L17.8154 9.48207L24.3534 10.4321C24.8999 10.5115 25.2786 11.019 25.1992 11.5655C25.1676 11.7831 25.0651 11.9843 24.9076 12.1378L20.2949 16.634L20.0457 16.8641L18.9915 15.8098L23.1351 11.7708L16.8193 10.8531L13.9948 5.13001L12.1169 8.93511Z");

    private MultiplayerPageViewModel multiplayerPageVM = new MultiplayerPageViewModel();

    private void Button_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {        
        // TODO: Insert DataContext: Inform the user if things are currently disabled
        /* This is an example output. It can be used for generating more useful output */
        if (!multiplayerPageVM.RefreshServerListEnabled)
        {
            System.Console.WriteLine("> " + multiplayerPageVM.CurrentTimeOfTheDay + " Refresh button currently on cooldown.\n");
        }
    }

    private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        await RunAnimation();
    }

    private async Task RunAnimation()
    {
        Animation animation = (Animation)this.Resources["ResourceAnimation"]!;
        // Running XAML animation on the Rect control. 
        if (animation.CheckAccess())
        {
            await animation.RunAsync(Rect);
        }
    }

    /*
    private void TextBlock_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        System.Console.WriteLine("Textblock loaded.");
        Countdown CountdownTimer = new Countdown();
    }

    
    public string CurrentTime = "Test";

    public string NextDownloadCycle = "Test";

    public Countdown CountdownTimer { get; set; } = new Countdown();
    */
}
