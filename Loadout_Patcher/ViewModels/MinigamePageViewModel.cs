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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Loadout_Patcher.Models;
using Loadout_Patcher.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net.Security;
using System.Reactive;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Loadout_Patcher.ViewModels;

public partial class MinigamePageViewModel : ViewModelBase, IEquatable<MinigamePageViewModel>
{
    // ---------------------------------------------------------------------------------------------------------
    // From Board

    private readonly List<MinigamePageViewModel> _bombs = new(); // Performance
    private readonly Random _rnd = new();

    public MinigamePageViewModel()
    {
        BombSizeWidth = BombSizeHeight = FlagSizeWidth = FlagSizeHeight = 50;



        string minigameTime = Solver.GetBestTimeWithoutSolver();
        int result;
        if (minigameTime != "" && minigameTime.Length > 1 && Int32.TryParse(minigameTime.Substring(0, 2), out result))
        {
            if (Int32.Parse(minigameTime.Substring(0, 2)) < 10)
            {
                FastestMinigameTime = minigameTime;
            }
            else
            {
                FastestMinigameTime = "09:59:59.9999999";
            }
        }
        else
        {
            FastestMinigameTime = "09:59:59.9999999";
        }
        FastestMinigameTime = FastestMinigameTime.Substring(1);
        CoverColor = Brushes.MidnightBlue;
        _background = SolidColorBrush.Parse("#F5A460");
        CreateBoard();
        OnLoadBoardClicked = ReactiveCommand.Create(LoadBoard);
        OnStartClicked = ReactiveCommand.Create(StartOrReset);
        _timer.Elapsed += TimerTick;
    }

    public MinigamePageViewModel(int value, Models.Point position, MinigamePageViewModel global)
    {
        string minigameTime = Solver.GetBestTimeWithoutSolver();
        int result;
        if (minigameTime != "" && minigameTime.Length > 1 && Int32.TryParse(minigameTime.Substring(0, 2), out result))
        {
            if (Int32.Parse(minigameTime.Substring(0, 2)) < 10)
            {
                FastestMinigameTime = minigameTime;
            }
            else
            {
                FastestMinigameTime = "09:59:59.9999999";
            }
        }
        else
        {
            FastestMinigameTime = "09:59:59.9999999";
        }
        FastestMinigameTime = FastestMinigameTime.Substring(1);
        CoverColor = Brushes.MidnightBlue;
        _background = SolidColorBrush.Parse("#F5A460");
        _global = global;
        OnFieldLeftClicked = ReactiveCommand.Create(FieldLeftClicked);
        Position = position;
        Value = value;
    }

    public ObservableCollectionExtended<MinigamePageViewModel> Fields { get; set; } = new();

    private MinigamePageViewModel? GetField(int x, int y)
    {
        if (x < 0 || y < 0 || x >= CurrentRowsAndColumns || y >= CurrentRowsAndColumns)
        {
            return null;
        }

        int index = CurrentRowsAndColumns * y + x;

        if (index >= Fields.Count || index < 0)
        {
            return null;
        }
        MinigamePageViewModel field = Fields[index];
        return field;
    }

    private MinigamePageViewModel? GetField(Models.Point position)
    {
        return GetField(position.X, position.Y);
    }


    private void RemoveRedBackground()
    {
        bool dark = false;
        for (int i = 0; i < CurrentRowsAndColumns; i++)
        {
            if (CurrentRowsAndColumns % 2 == 0) dark = !dark;

            for (int j = 0; j < CurrentRowsAndColumns; j++)
            {
                MinigamePageViewModel field = GetField(j, i)!;
                dark = !dark;
                /* The 2 colors must be the same as the 2 background colors as below */
                if (field.Background.Equals(Brushes.Red))
                {
                    field.Background = dark ? SolidColorBrush.Parse("#F5A460") : SolidColorBrush.Parse("#FFD700");
                }
            }
        }
    }

    public void CreateBoard()
    {
        if (CurrentAmountBombs > Math.Pow(CurrentRowsAndColumns, 2) / 2)
        {
            CurrentAmountBombs = (int)(Math.Pow(CurrentRowsAndColumns, 2) / 4);
        }
        using (Fields.SuspendNotifications())
        {
            Fields.Clear();

            bool dark = false;
            for (int i = 0; i < CurrentRowsAndColumns; i++)
            {
                if (CurrentRowsAndColumns % 2 == 0)
                {
                    dark = !dark;
                }
                for (int j = 0; j < CurrentRowsAndColumns; j++)
                {
                    MinigamePageViewModel field = new(0, new Models.Point(i, j), this);
                    dark = !dark;
                    /* We set the board colors */
                    field.Background = dark ? SolidColorBrush.Parse("#F5A460") : SolidColorBrush.Parse("#FFD700"); // Original Brushes.SandyBrown and Brushes.Gold
                    field.CoverColor = dark ? Brushes.MidnightBlue : Brushes.CornflowerBlue;
                    /* ----------------------------------------------------------------------------- */
                    /* Here we can change the fields such as the width and height of bombs and flags */
                    /* ----------------------------------------------------------------------------- */
                    if (CurrentRowsAndColumns > 20)
                    {
                        field.BombSizeWidth = field.BombSizeHeight = field.FlagSizeWidth = field.FlagSizeHeight = 50;
                    }
                    else
                    {
                        field.BombSizeWidth = field.BombSizeHeight = field.FlagSizeWidth = field.FlagSizeHeight = 76;
                    }
                    Fields.Add(field);
                }
            }
        }
    }

    public void FillInBombs(Models.Point firstFieldPosition)
    {
        NeedsReset = true;
        StartTime = DateTime.Now;
        BombsArePlaced = true;
        AddBombs(CurrentAmountBombs, firstFieldPosition);
        GenerateValue();
    }

    private void GenerateValue()
    {
        foreach (MinigamePageViewModel field in Fields)
        {
            IEnumerable<MinigamePageViewModel> surroundingFields = GetSurroundingFields(field.Position!);
            int value = surroundingFields.Count(f => f.HasBomb);
            field.Value = field.HasBomb ? 0 : value;
            field.HasNumber = field.Value > 0;
        }
    }

    private void AddBombs(int amount, Models.Point firstFieldPosition)
    {
        List<Models.Point> availablePoints = new();
        for (int i = 0; i < CurrentRowsAndColumns; i++)
            for (int j = 0; j < CurrentRowsAndColumns; j++)
            {
                /* It is impossible to hit a bomb on the first click */
                if (Math.Abs(j - firstFieldPosition.X) <= 1 &&
                    Math.Abs(i - firstFieldPosition.Y) <= 1)
                    continue;

                availablePoints.Add(new Models.Point(i, j));
            }

        for (int i = 0; i < amount; i++)
        {
            int pointIndex = _rnd.Next(availablePoints.Count);
            Models.Point point = availablePoints[pointIndex];
            MinigamePageViewModel bomb = GetField(point)!;
            bomb.HasBomb = true;
            _bombs.Add(bomb);
            availablePoints.Remove(point);
        }
    }

    public void UncoverSurroundingZeros(MinigamePageViewModel field)
    {
        if (field.Value != 0) return;
        foreach (MinigamePageViewModel surroundingUncoveredField in GetSurroundingFields(field.Position!))
        {
            if (!surroundingUncoveredField.IsCovered) continue;
            Uncover(surroundingUncoveredField); // if you dont uncover it, then it will return -1 as value
            if (surroundingUncoveredField.Value == 0) UncoverSurroundingZeros(surroundingUncoveredField);
        }
    }

    public static void Uncover(MinigamePageViewModel field)
    {
        field.IsCovered = false;
    }

    public void UncoverEveryFieldSurroundingIfValueMatchesFlags(MinigamePageViewModel field)
    {
        IEnumerable<MinigamePageViewModel> surroundingFields = GetSurroundingFields(field.Position!);
        if (field.Value != surroundingFields.Count(f => f.IsFlagged))
            return;
        bool isGameOver = false;
        List<MinigamePageViewModel> uncoveredBombs = new();
        foreach (MinigamePageViewModel fieldViewModel in surroundingFields.Where(f => !f.IsFlagged))
        {
            if (fieldViewModel.IsCovered) // should improve performance
                Uncover(fieldViewModel);

            if (fieldViewModel.Value == 0) UncoverSurroundingZeros(fieldViewModel);

            if (fieldViewModel.HasBomb)
            {
                isGameOver = true;
                uncoveredBombs.Add(fieldViewModel);
            }
        }

        if (isGameOver) GameOver(uncoveredBombs);
    }

    private IEnumerable<MinigamePageViewModel> GetSurroundingFields(Models.Point position)
    {
        MinigamePageViewModel? topLeft = GetField(position.X - 1, position.Y - 1);
        if (topLeft != null)
            yield return topLeft;

        MinigamePageViewModel? top = GetField(position.X, position.Y - 1);
        if (top != null)
            yield return top;

        MinigamePageViewModel? topRight = GetField(position.X + 1, position.Y - 1);
        if (topRight != null)
            yield return topRight;

        MinigamePageViewModel? right = GetField(position.X + 1, position.Y);
        if (right != null)
            yield return right;

        MinigamePageViewModel? bottomRight = GetField(position.X + 1, position.Y + 1);
        if (bottomRight != null)
            yield return bottomRight;

        MinigamePageViewModel? bottom = GetField(position.X, position.Y + 1);
        if (bottom != null)
            yield return bottom;

        MinigamePageViewModel? bottomLeft = GetField(position.X - 1, position.Y + 1);
        if (bottomLeft != null)
            yield return bottomLeft;

        MinigamePageViewModel? left = GetField(position.X - 1, position.Y);
        if (left != null)
            yield return left;
    }

    private void ResetBoard()
    {
        foreach (MinigamePageViewModel field in Fields)
        {
            field.HasBomb = false;
            field.IsCovered = true;
            field.Value = 0;
            field.IsFlagged = false;
            field.HasNumber = false;
            _bombs.Clear();
        }

        GlobalReset();
    }

    public bool HasWon()
    {
        return !Fields.Any(f => !f.HasBomb && f.IsCovered);
    }

    public void GameOver(List<MinigamePageViewModel> uncoveredBombs)
    {
        foreach (MinigamePageViewModel uncoveredBomb in uncoveredBombs) uncoveredBomb.Background = Brushes.Red;
        foreach (MinigamePageViewModel bomb in _bombs) bomb.IsCovered = false;
        GlobalStop();
        GameInfoText = "You Lost!";
        /* We play a terminal sound as we lose the minigame */
        Sound.PlayMinigameSoundExplosion();
    }

    public void WinBoard()
    {
        GlobalStop();
        TimeSpan timeTaken = DateTime.Now.Subtract(StartTime);
        GameInfoText =
            $"You Won! \n You needed \n {Math.Floor(timeTaken.TotalMinutes)}:{timeTaken:ss\\.fff}";

        TimeSpan interval;
        CultureInfo culture = CultureInfo.CurrentCulture;
        string currentBestTime = Solver.GetBestTimeWithoutSolver();
        TimeSpan.TryParseExact(currentBestTime, "c", culture, out interval);

        TimeSpan result;
        if (!TimeSpan.TryParse(currentBestTime, culture, out result))
        {
            currentBestTime = "09:59:59.9999999";
            TimeSpan.TryParseExact(currentBestTime, "c", culture, out interval);
        }

        /* We check for a new best time */
        if (timeTaken < interval || currentBestTime == "")
        {
            /* We check if this is classic mode with 40 bombs and 20 rows and columns */
            if (CurrentAmountBombs == 40 && CurrentRowsAndColumns == 20)
            {
                Solver.SetBestTimeWithoutSolver(timeTaken);

                /* We save the time */
                // Save file is being updated
                Filesave.SaveFile saveFile = Filesave.SaveFileBuilder();
                Filesave.SaveDataToFile(saveFile, true);

                Console.WriteLine("> Your new best time was saved.\n");
            }
        }
        /* We play a terminal sound as we win the minigame */
        Sound.PlayMinigameSoundJoy();
    }

    public void StartBoard()
    {
        RemoveRedBackground();
        GlobalStart();
        GameInfoText = "Game running";
    }

    public void StopBoard()
    {
        ResetBoard(); // GameRunning false is included
        GameInfoText = "Game stopped";
    }

    public void ExecuteMove(Move move)
    {
        if (GameFlagsSet != move.FlagsSet) GameFlagsSet = move.FlagsSet;
        for (int column = 0; column < CurrentRowsAndColumns; column++)
            for (int row = 0; row < CurrentRowsAndColumns; row++)
            {
                MinigamePageViewModel fieldViewModel = GetField(column, row)!;
                CreationField field = move.Fields[column, row];
                if (fieldViewModel.IsCovered != field.IsCovered) fieldViewModel.IsCovered = field.IsCovered;

                if (fieldViewModel.IsFlagged != field.IsFlagged) fieldViewModel.IsFlagged = field.IsFlagged;

                if (fieldViewModel.Value != field.Value) fieldViewModel.Value = field.Value;

                if (fieldViewModel.HasBomb != field.HasBomb) fieldViewModel.HasBomb = field.HasBomb;

                if (fieldViewModel.Background.Equals(Brushes.Red) && !field.CausedLose) RemoveRedBackground();

                if (!fieldViewModel.Background.Equals(Brushes.Red) && field.CausedLose)
                    fieldViewModel.Background = Brushes.Red;
            }
    }

    // ---------------------------------------------------------------------------------------------------------
    // From Controls
    private readonly Timer _timer = new();
    private int _amountMoves;

    private bool _automatic;

    private int _currentMove;
    private int _delay = 300;

    private bool _forward = true;
    private Solver? _solver;

    private bool _solverActive = false;

    public List<Move> Moves { get; } = new();

    public bool SolverActive
    {
        get => _solverActive;
        set => _solverActive = value;
    }

    public int Delay
    {
        get => _delay;
        set
        {
            this.SetProperty(ref _delay, value);
            DelayHasChanged();
        }
    }

    public int CurrentMove
    {
        get => _currentMove;
        set
        {
            this.SetProperty(ref _currentMove, value);
            CurrentMoveHasChanged();
        }
    }

    public bool Automatic
    {
        get => _automatic;
        set
        {
            this.SetProperty(ref _automatic, value);
            AutomaticHasChanged();
        }
    }

    public bool Forward
    {
        get => _forward;
        set => this.SetProperty(ref _forward, value);
    }

    public int AmountMoves
    {
        get => _amountMoves;
        set => this.SetProperty(ref _amountMoves, value);
    }

    //--------------------------//

    private int _bombSizeWidth = 76;

    public int BombSizeWidth
    {
        get => _bombSizeWidth;
        set => this.SetProperty(ref _bombSizeWidth, value);
    }

    private int _bombSizeHeight = 76;

    public int BombSizeHeight
    {
        get => _bombSizeHeight;
        set => this.SetProperty(ref _bombSizeHeight, value);
    }

    private int _flagSizeWidth = 76;

    public int FlagSizeWidth
    {
        get => _flagSizeWidth;
        set => this.SetProperty(ref _flagSizeWidth, value);
    }

    private int _flagSizeHeight = 76;

    public int FlagSizeHeight
    {
        get => _flagSizeHeight;
        set => this.SetProperty(ref _flagSizeHeight, value);
    }


    //--------------------------//

    public ReactiveCommand<Unit, Unit>? OnLoadBoardClicked { get; }
    public ReactiveCommand<Unit, Unit>? OnStartClicked { get; }

    private void CurrentMoveHasChanged()
    {
        if (GameRunning)
        {
            ExecuteMove(Moves[CurrentMove]);
        }
    }

    private void AutomaticHasChanged()
    {
        if (_automatic)
            _timer.Start();
        else
            _timer.Stop();
    }

    private void TimerTick(object? sender, ElapsedEventArgs e)
    {
        ExecuteMove(Moves[_currentMove]);

        if (Forward)
        {
            if (CurrentMove < AmountMoves) CurrentMove++;
        }
        else
        {
            if (CurrentMove > 0) CurrentMove--;
        }
    }

    private void DelayHasChanged()
    {
        _timer.Interval = Delay;
    }

    private void Reset()
    {
        Automatic = false;
        Forward = true;
        Moves.Clear();
        AmountMoves = 0;
        CurrentMove = 0;
    }

    private bool AiHasWon()
    {
        int uncoveredFields = 0;
        Move lastMove = Moves.Last();
        for (int y = 0; y < CurrentRowsAndColumns; y++)
            for (int x = 0; x < CurrentRowsAndColumns; x++)
                if (!lastMove.Fields[y, x].IsCovered)
                    uncoveredFields++;

        int amountFields = CurrentRowsAndColumns * CurrentRowsAndColumns;
        return uncoveredFields - (amountFields - CurrentAmountBombs) == 0;
    }

    private void LoadBoard()
    {
        if (GameRunning || NeedsReset) return;
        CurrentAmountBombs = AmountBombs;
        CurrentRowsAndColumns = RowsAndColumns;
        CreateBoard(); // Changed from Board.CreateBoard();
    }

    private void StartOrReset()
    {
        SolverActive = _solverActive;

        if (MainButtonStatus == MainButtonStatuses.Start)
        {
            Start();
            /* We play a terminal sound as the minigame starts */
            Sound.PlayMinigameSoundTerminalGameStart();
        }
        else if (MainButtonStatus == MainButtonStatuses.Reset)
        {
            Stop();
        }
        else if (MainButtonStatus == MainButtonStatuses.Cancel)
        {
            CancelSolver();
        }
    }

    private void Start()
    {
        StartBoard();
        if (_solverActive)
        { 
            StartSolver();
        }
        else
        {
            MainButtonStatus = MainButtonStatuses.Reset;
        }
        AmountMoves--;
    }

    private void Stop()
    {
        _timer.Stop();
        StopBoard();
        Reset();
        MainButtonStatus = MainButtonStatuses.Start;
    }

    private void CancelSolver()
    {
        GameInfoText = "Canceling Solver...";
        _solver!.IsCanceled = true;
    }

    private void StartSolver()
    {
        GameInfoText = "Solver Started...";
        _solver = new Solver(CurrentRowsAndColumns, CurrentAmountBombs, this);
        TimeSpan solverTimeSpan;
        StartTime = DateTime.Now;
        SolverIsSolving = true;
        MainButtonStatus = MainButtonStatuses.Cancel;
        Thread solverThread = new(() =>
        {
            try
            {
                _solver.Begin();
                solverTimeSpan = DateTime.Now.Subtract(StartTime);
                GameInfoText = AiHasWon()
                    ? $"Solver Won! \nin {Math.Round(solverTimeSpan.TotalMilliseconds)}ms"
                    : $"Solver Finished \nin {Math.Round(solverTimeSpan.TotalMilliseconds)}ms";
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(BombUncoveredException))
                {
                    BombUncoveredException bombUncoveredException = (BombUncoveredException)e;
                    solverTimeSpan = DateTime.Now.Subtract(StartTime);
                    CreationField[,] fields = CloneCreationFields(Moves[AmountMoves - 1].Fields);
                    for (int y = 0; y < CurrentRowsAndColumns; y++)
                        for (int x = 0; x < CurrentRowsAndColumns; x++)
                            if (fields[y, x].HasBomb)
                                fields[y, x].IsCovered = false;

                    fields[bombUncoveredException.Field.Position!.Y, bombUncoveredException.Field.Position.X]
                        .CausedLose = true;

                    Moves.Add(new Move(fields, Moves[_amountMoves - 1].FlagsSet));
                    AmountMoves++;
                    GameInfoText = $"Solver Lost! \nin {Math.Round(solverTimeSpan.TotalMilliseconds)}ms";
                }
                else
                {
                    GameInfoText = $"Error was thrown in Solver: {e.Message}";
                }
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            MainButtonStatus = MainButtonStatuses.Reset;
            SolverIsSolving = false;
        });

        solverThread.Start();
    }

    private CreationField[,] CloneCreationFields(CreationField[,] creationFields)
    {
        CreationField[,] fields = new CreationField[CurrentRowsAndColumns, CurrentRowsAndColumns];
        for (int y = 0; y < CurrentRowsAndColumns; y++)
            for (int x = 0; x < CurrentRowsAndColumns; x++)
            {
                CreationField creationField = new()
                {
                    Position = new Models.Point(y, x),
                    Value = creationFields[y, x].Value,
                    HasBomb = creationFields[y, x].HasBomb,
                    IsCovered = creationFields[y, x].IsCovered,
                    IsFlagged = creationFields[y, x].IsFlagged
                };
                fields[y, x] = creationField;
            }

        return fields;
    }

    // ---------------------------------------------------------------------------------------------------------
    // From Global

    [ObservableProperty]
    public string _fastestMinigameTime = "";

    [ObservableProperty]
    public int _amountBombs = 40;

    [ObservableProperty]
    private int _currentAmountBombs = 40; // Only changes when the board changes

    [ObservableProperty]
    private int _currentRowsAndColumns = 20; // Only changes when the board changes

    [ObservableProperty]
    private int _gameFlagsSet = 0;

    private static bool _gameRunning;

    [ObservableProperty]
    private string _gameInfoText = "Game hasn't started";

    [ObservableProperty]
    private string _mainButtonText = "Start";

    private int _maxBombs = 112;

    private static int _rowsAndColumns = 20;
    private bool _solverIsSolving;

    private static bool BombsArePlaced;

    private static DateTime StartTime;

    public bool SolverIsSolving
    {
        get => _solverIsSolving;
        set => _solverIsSolving = value;
    }

    [ObservableProperty]
    public bool _needsReset;

    public static bool GameRunning
    {
        get => _gameRunning;
        set => _gameRunning = value;
    }

    public int RowsAndColumns
    {
        get => _rowsAndColumns;
        set
        {
            _rowsAndColumns = value;
            MaxBombs = (int)(Math.Pow(value, 2) / 2);
        }
    }

    public int MaxBombs
    {
        get => _maxBombs;
        set => _maxBombs = value;
    }


    private MainButtonStatuses _mainButtonStatus = MainButtonStatuses.Start;

    public MainButtonStatuses MainButtonStatus
    {
        get => _mainButtonStatus;
        set
        {
            _mainButtonStatus = value;
            switch (value)
            {
                case MainButtonStatuses.Start:
                    MainButtonText = "Start";
                    return;
                case MainButtonStatuses.Reset:
                    MainButtonText = "Reset";
                    return;
                case MainButtonStatuses.Cancel:
                    MainButtonText = "Cancel";
                    return;
            }
        }
    }

    public void GlobalReset()
    {
        GlobalStop();
        GameInfoText = "";
        GameFlagsSet = 0;
        BombsArePlaced = false;
        NeedsReset = false;
    }

    public void GlobalStart()
    {
        GameRunning = true;
        NeedsReset = true;
    }

    public static void GlobalStop()
    {
        GameRunning = false;
    }

    public enum MainButtonStatuses
    {
        Start,
        Reset,
        Cancel
    }

    // ---------------------------------------------------------------------------------------------------------
    // From FieldViewModel

    private IBrush _background;

    private readonly MinigamePageViewModel? _global;

    private bool _hasBomb;
    private bool _hasNumber;

    private bool _isCovered = true;

    private int _value;

    private IBrush _valueColor = Brushes.Black;

    public int Value
    {
        get => _value;
        set
        {
            this.SetProperty(ref _value, value);
            UpdateFontColor();
        }
    }

    public bool IsCovered
    {
        get => _isCovered;
        set => this.SetProperty(ref _isCovered, value);
    }

    public IBrush ValueColor
    {
        get => _valueColor;
        set => this.SetProperty(ref _valueColor, value);
    }

    public bool HasBomb
    {
        get => _hasBomb;
        set => this.SetProperty(ref _hasBomb, value);
    }

    public ReactiveCommand<Unit, Unit>? OnFieldLeftClicked { get; set; }

    //public ReactiveCommand<Unit, Unit> OnFieldRightClicked { get; set; }

    [ObservableProperty]
    public Models.Point? _position;

    public IBrush Background
    {
        get => _background;
        set => this.SetProperty(ref _background, value);
    }

    public IBrush CoverColor { get; set; }

    public bool HasNumber
    {
        get => _hasNumber;
        set => this.SetProperty(ref _hasNumber, value);
    }

    [ObservableProperty]
    public bool _isFlagged = false;

    public bool Equals(MinigamePageViewModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _global!.Equals(other._global) && Position!.Equals(other.Position);
    }


    public void FieldLeftClicked()
    {
        //IsFlagged || 
        if (!MinigamePageViewModel.GameRunning || SolverActive) return;
        if (!IsCovered) return;
        Console.WriteLine($"Field Left clicked X: {Position!.X} Y: {Position.Y}");

        if (!MinigamePageViewModel.BombsArePlaced) _global!.FillInBombs(Position);

        //Console.WriteLine($"Field Right clicked X: {Position.X} Y: {Position.Y}");
        if (_global!.FlagRemoverMode && IsFlagged)
        {
            _global.GameFlagsSet--;
            IsFlagged = false;
            /* We play a terminal sound as we remove a flag during the minigame */
            Sound.PlayMinigameSoundTerminalFlagRemove();
        }
        else if (_global.DirectMode)
        {
            /* We play a terminal sound as we make a direct move during the minigame */
            Sound.PlayMinigameSoundTerminalDirectMove();

            if (IsFlagged)
            {
                _global.GameFlagsSet--;
                IsFlagged = false;
            }

            if (!IsCovered)
            {
                if (HasNumber) _global.UncoverEveryFieldSurroundingIfValueMatchesFlags(this);
                if (_global.HasWon()) _global.WinBoard();
            }
            else
            {
                if (IsFlagged)
                {
                    _global.GameFlagsSet--;
                    IsFlagged = false;
                }
                Uncover(this);
                if (HasBomb)
                {
                    List<MinigamePageViewModel> uncoveredBombs = new() { this };
                    _global.GameOver(uncoveredBombs);
                }
                else
                {
                    _global.UncoverSurroundingZeros(this);
                    if (_global.HasWon()) _global.WinBoard();
                }
            }
        }
        else
        {
            if (!IsFlagged)
            {
                IsFlagged = true;
                _global.GameFlagsSet++;
                /* We play a terminal sound as we set a flag during the minigame */
                Sound.PlayMinigameSoundTerminalFlagSet();
            }
            else
            {
                _global.GameFlagsSet--;
                IsFlagged = false;
                /* We play a terminal sound as we remove a flag during the minigame */
                Sound.PlayMinigameSoundTerminalFlagRemove();

                if (!IsCovered)
                {
                    if (HasNumber) _global.UncoverEveryFieldSurroundingIfValueMatchesFlags(this);
                    if (_global.HasWon()) _global.WinBoard();
                }
                else
                {
                    Uncover(this);
                    if (HasBomb)
                    {
                        List<MinigamePageViewModel> uncoveredBombs = new() { this };
                        _global.GameOver(uncoveredBombs);
                    }
                    else
                    {
                        _global.UncoverSurroundingZeros(this);
                        if (_global.HasWon()) _global.WinBoard();
                    }
                }
            }
        }
    }

    /*
    [RelayCommand]
    public void FieldRightClicked()
    {
        if (!IsCovered || !MinigamePageViewModel.GameRunning || MinigamePageViewModel.SolverActive) return;
        Console.WriteLine($"Field Right clicked X: {Position.X} Y: {Position.Y}");
        if (!IsFlagged)
        {
            IsFlagged = true;
            FlagsSet++;
        }
        else
        {
            FlagsSet--;
            IsFlagged = false;
        }

        //_global = GetField(Position.X, Position.Y);
        // We don't win the game by clicking right at the moment
        //if (_global.HasWon())
        //{
        //    _global.WinBoard();
        //}
    }
    */
    private void UpdateFontColor()
    {
        HasNumber = true;

        switch (Value)
        {
            case 0:
                HasNumber = false;
                break;
            case 1:
                ValueColor = Brushes.Blue;
                HasNumber = true;
                break;
            case 2:
                ValueColor = Brushes.Green;
                HasNumber = true;
                break;
            case 3:
                ValueColor = Brushes.Red;
                HasNumber = true;
                break;
            case 4:
                ValueColor = Brushes.Purple;
                HasNumber = true;
                break;
            case 5:
                ValueColor = Brushes.Orange;
                HasNumber = true;
                break;
            case 6:
                ValueColor = Brushes.Yellow;
                HasNumber = true;
                break;
            case 7:
                ValueColor = Brushes.Pink;
                HasNumber = true;
                break;
            case 8:
                ValueColor = Brushes.Black;
                HasNumber = true;
                break;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((MinigamePageViewModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_global, Position);
    }

    public static bool operator ==(MinigamePageViewModel? left, MinigamePageViewModel? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(MinigamePageViewModel? left, MinigamePageViewModel? right)
    {
        return !Equals(left, right);
    }

    // ---------------------------------------------------------------------------------------------------------
    // New
    [ObservableProperty]
    public bool _flagRemoverMode;

    [ObservableProperty]
    public bool _directMode;
}