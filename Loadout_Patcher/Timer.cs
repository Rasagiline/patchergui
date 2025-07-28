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
using Avalonia.Threading;
using System;
using System.ComponentModel;

namespace Loadout_Patcher;

public class Countdown : INotifyPropertyChanged
{
    private DispatcherTimer _disTimer = new DispatcherTimer();

    public event PropertyChangedEventHandler? PropertyChanged;
    public EventHandler<DateTime> OnEveryHour = (s, e) => { };

    public string CurrentTime { get; set; } = "Current Time";

    public string NextDownloadCycle { get; set; } = "Downloading in: ";

    private DateTime _nextCycle = DateTime.Now.AddHours(1);

    public Countdown()
    {
        _disTimer.Interval = TimeSpan.FromSeconds(1);
        _disTimer.Tick += DispatcherTimer_Tick;
        _disTimer.Start();
    }

    private void DispatcherTimer_Tick(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        if (now >= _nextCycle)
        {
            _nextCycle = DateTime.Now.AddHours(1);
            OnEveryHour(this, now);
        }
        TimeSpan ts = _nextCycle - now;
        NextDownloadCycle = String.Format(
            "Next crawl session at {0}(in {1}m{2}s)",
            _nextCycle.ToString("HH:mm:ss"),
            Math.Round(ts.TotalMinutes) - 1,
            ts.Seconds
        );
        CurrentTime = now.ToString("HH:mm:ss");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextDownloadCycle)));
    }
}