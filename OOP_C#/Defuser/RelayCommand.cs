using System;
using System.Windows.Input;

namespace Defuser
{
    public class RelayCommand<T> : ICommand //Класс который запоминает действия команд наследует интерфейс который определяет действия команд
    {
        //Класс принимает 2 делегата 
        private readonly Action<T> execute;  //Предикат который определяет действие
        private readonly Predicate<T> canExecute; //Предикат который определяет может ли команда выполняться

        public RelayCommand(Action<T> execute) : this(execute,null)  //Конструктор для определения параметров canExecute устанавливает в null execute
        {
        }
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }
        
        public event EventHandler CanExecuteChanged         //Вызывается когда состояние команды изменяется и обновляет возможность выполнения команды 
        {
            add { CommandManager.RequerySuggested += value; }
            remove {  CommandManager.RequerySuggested -= value; }
        }
        
        public bool CanExecute(object parameter)            //Проверяет может ли быть команда выполнена
        {
            return canExecute?.Invoke((T)parameter) ?? false; //если null, то false
        }
        public void Execute(object parameter) //Вызывание метода для выполнения команды
        {
            execute((T) parameter);
        }
    }
}
