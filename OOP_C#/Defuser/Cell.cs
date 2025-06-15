using System.ComponentModel;
using System.Windows.Input;

namespace Defuser
{
    //Класс который описывает ячейку так же наследует интерфейс который сообщает об изменении значений свойства класса
    public class Cell : INotifyPropertyChanged
    {
        private bool isMine; 
        private bool isRevealed;
        private int value;
        private string displayValue;
        private ICommand cellClickCommand;
        private int row;
        private int column;
        private bool isFlagged;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) //Обновляет состояние объекта и отображает новое значение
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool IsMine
        {
            get { return isMine; }
            set
            {
                if (isMine != value)
                {
                    isMine = value;
                    OnPropertyChanged(nameof(isMine));
                }
            }
        }

        public bool IsFlagged
        {
            get { return isFlagged; }
            set
            {
                if (isFlagged != value)
                {
                    isFlagged = value;
                    OnPropertyChanged(nameof(IsFlagged));
                }
            }
        }

        public bool IsRevealed
        {
            get { return isRevealed; }
            set
            {
                if (isRevealed != value)
                {
                    isRevealed = value;
                    OnPropertyChanged(nameof(IsRevealed));
                }
            }
        }
        public int Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public string DisplayValue
        {
            get { return displayValue; }
            set
            {
                if (displayValue != value)
                {
                    displayValue = value;
                    OnPropertyChanged(nameof(DisplayValue));
                }
            }
        }

        public ICommand CellClickCommand
        {
            get { return cellClickCommand; }
            set
            {
                if (cellClickCommand != value)
                {
                    cellClickCommand = value;
                    OnPropertyChanged(nameof(CellClickCommand));
                }
            }
        }

        public int Row
        {
            get { return row; }
            set 
            {
                if (row != value)
                {
                    row = value;
                    OnPropertyChanged(nameof(Row));
                }
            }
        }
        public int Column
        {
            get { return column; }
            set
            {
                if(column != value)
                {
                    column = value;
                    OnPropertyChanged(nameof(Column));
                }
            }
        }


    }
}
