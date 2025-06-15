using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Defuser
{
    //Класс который содержит логику и данные для взаимодействия с пользовательским интерфейсом, так же наследует интерфейс который сообщает об изменении значений свойства
    public class MinesweeperViewModel : INotifyPropertyChanged 
    {
        private DifficultyLevel selectedDifficultyLevel;
        private ObservableCollection<Cell> gameBoard;

        public MinesweeperViewModel()
        {
            DifficultyLevels = new List<DifficultyLevel>
            {
                new DifficultyLevel {Name = "Новичёк: 10 бомб", BoardSize = 9},
                new DifficultyLevel {Name = "Мастер: 40 бомб", BoardSize = 16},
                new DifficultyLevel {Name = "Эксперт: 90 бомб", BoardSize = 25}
            };
            SelectedDifficultyLevel = DifficultyLevels[0];
            GenerateGameBoard();
        }

        public List<DifficultyLevel> DifficultyLevels { get; }
        
        public DifficultyLevel SelectedDifficultyLevel
        {
            get { return selectedDifficultyLevel; }
            set 
            {
                if(selectedDifficultyLevel != value)
                {
                    selectedDifficultyLevel = value;
                    GenerateGameBoard();
                    OnPropertyChanged(nameof(SelectedDifficultyLevel));
                }
            }
        }
        
        public ObservableCollection<Cell> GameBoard
        {
            get { return gameBoard; }
            set
            {
                if(gameBoard != value)
                {
                    gameBoard = value;
                    OnPropertyChanged(nameof(GameBoard));
                }
            }
        }
        public void GenerateGameBoard()
        {
            int boardsize = SelectedDifficultyLevel.BoardSize;
            Cell[,] gameboard = new Cell[boardsize, boardsize];
            int mineCount = 0;
            switch(SelectedDifficultyLevel.Name)
            {
                case "Новичёк: 10 бомб":
                    mineCount = 1;
                    break;
                case "Мастер: 40 бомб":
                    mineCount = 40;
                    break;
                case "Эксперт: 90 бомб":
                    mineCount = 90;
                    break;
            }
            Random random = new Random();
            int count = 0;
            for (int row = 0; row < boardsize; row++)
            {
                for (int col = 0; col < boardsize; col++)
                {
                    Cell cell = new Cell
                    {
                        Row = row,
                        Column = col,
                        IsMine = false,
                        IsRevealed = false,
                        Value = 0,
                        DisplayValue = string.Empty
                    };

                    gameboard[row, col] = cell;
                }
            }
            while (count < mineCount)
            {
                int randomRow = random.Next(0, boardsize);
                int randomCol = random.Next(0, boardsize);

                Cell cell = gameboard[randomRow, randomCol];

                if (!cell.IsMine)
                {
                    cell.IsMine = true;
                    count++;
                }
            }
            ObservableCollection<Cell> gameBoardCollection = new ObservableCollection<Cell>();
            for (int row = 0;row < boardsize; row++)
            {
                for (int col = 0;col < boardsize; col++)
                {
                    Cell cell = gameboard[row, col];
                    gameBoardCollection.Add(cell);
                }
            }
            GameBoard = gameBoardCollection;

            foreach (Cell cell in gameBoard)
            {
                cell.CellClickCommand = CellClickCommand;
            }
        }
        private ICommand restartCommand;
        public ICommand RestartCommand
        {
            get
            {
                if (restartCommand == null)
                {
                    restartCommand = new RelayCommand<object>(RestartGame, CanRestartGame);
                }
                return restartCommand;
            }
        }
        private bool CanRestartGame(object paramert)
        {
            return true;
        }

        private void RestartGame(object paramert)
        {
            GenerateGameBoard();
        }
        private ICommand cellClickCommand;

        public ICommand CellClickCommand
        {
            get
            {
                if(cellClickCommand == null)
                {
                    cellClickCommand = new RelayCommand<Cell>(HandleCellClick, CanHandleCellClick);
                }
                return cellClickCommand;
            }
        }
        private bool CanHandleCellClick(Cell cell)
        {
            return cell != null && !cell.IsRevealed;
        }
        public ICommand cellRightClickCommand;
        public ICommand CellRightClickCommand
        {
            get
            {
                if(cellRightClickCommand == null)
                {
                    cellRightClickCommand = new RelayCommand<Cell>(HandleCellRightClick);
                }
                return cellRightClickCommand;
            }
        }
        
        private void HandleCellRightClick(Cell cell)
        {
            if(cell != null && !cell.IsRevealed)
            {
                cell.IsFlagged = !cell.IsFlagged;
                cell.DisplayValue = cell.IsFlagged ? "🏴" : string.Empty;

                if (GameBoard.Where(c => c.IsMine).All(c => c.IsFlagged) || GameBoard.Where(c => !c.IsRevealed).All(c => c.IsMine))
                {
                    ShowMine();
                    switch (MessageBox.Show("Вы Выиграли!\n" + "Начать новую игру? ", "Поздравляем!!!", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes))
                    {
                        case MessageBoxResult.Yes:
                            GenerateGameBoard();
                            break;
                        case MessageBoxResult.No:
                            Application.Current.Shutdown();
                            break;
                        case MessageBoxResult.Cancel: break;
                    }
                    GenerateGameBoard();
                }
            }
        }
        private void ShowMine()
        {
            foreach (var cell in GameBoard)
            {
                if (cell.IsMine)
                {
                    cell.DisplayValue = "💣";
                }
            }
        }
        private void HandleCellClick(Cell cell)
        {
            if (!CanHandleCellClick(cell)) 
                return;
            if (!cell.IsRevealed)
            {
                cell.IsRevealed = true;
                if (cell.IsMine)
                {
                    ShowMine();
                    switch (MessageBox.Show("Вы Проиграли!\n" + "Начать новую игру? ", "Ква-ква-кваааааа", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes))
                    {
                        case MessageBoxResult.Yes:
                            GenerateGameBoard();
                            break;
                        case MessageBoxResult.No:
                            Application.Current.Shutdown();
                            break;
                        case MessageBoxResult.Cancel: break;
                    }
                }
                else
                {
                    cell.DisplayValue = CalculateCellValue(cell);
                    OnPropertyChanged(nameof(GameBoard));
                    if(cell.Value == 0)
                    {
                        OpenAdjacentCells(cell);
                    }
                }
            }
        }
        private void OpenAdjacentCells(Cell cell)
        {
            int[] offset = { -1, 0, 1 };
            foreach (int rowOffset in offset)
            {
                foreach (int colOffset in offset)
                {
                    if(rowOffset == 0 &&  colOffset == 0) continue;

                    int neighborRow = cell.Row + rowOffset;
                    int neighborCol = cell.Column + colOffset;
                    if(neighborRow >= 0 &&  neighborRow < SelectedDifficultyLevel.BoardSize &&
                       neighborCol >= 0 && neighborCol < SelectedDifficultyLevel.BoardSize) 
                    {
                        Cell neighborCell = GameBoard[neighborRow * SelectedDifficultyLevel.BoardSize + neighborCol];

                        if(!neighborCell.IsRevealed && !neighborCell.IsMine)
                        {
                            neighborCell.IsRevealed=true;
                            neighborCell.DisplayValue = CalculateCellValue(neighborCell);
                            OnPropertyChanged(nameof(GameBoard));
                            if(neighborCell.Value == 0)
                            {
                                OpenAdjacentCells(neighborCell);
                            }
                        } 
                    }
                }
            }
        }

        private string CalculateCellValue(Cell cell)
        {
            int minesCount = 0;
            int[] offsets = { -1, 0, 1 };
            foreach (int rowOffset in offsets)
            {
                foreach (int colOffset in offsets)
                {
                    if (rowOffset == 0 && colOffset == 0) continue;

                    int neighborRow = cell.Row + rowOffset;
                    int neighborCol = cell.Column + colOffset;
                    if (neighborRow >= 0 && neighborRow < SelectedDifficultyLevel.BoardSize &&
                       neighborCol >= 0 && neighborCol < SelectedDifficultyLevel.BoardSize)
                    {
                        Cell neighborCell = GameBoard[neighborRow * SelectedDifficultyLevel.BoardSize + neighborCol];
                        if (neighborCell.IsMine)
                        {
                            minesCount++;
                        }
                    }
                }
            }

            if(minesCount > 0)
            {
                cell.Value = minesCount;
                cell.DisplayValue = cell.Value.ToString();
                return minesCount.ToString();
            }
            return string.Empty;

        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
