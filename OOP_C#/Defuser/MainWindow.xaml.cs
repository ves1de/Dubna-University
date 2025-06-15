using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Defuser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MinesweeperViewModel(); //Создает класс нашего сапёра и передаёт всю логику
        }
        
        private void MouseRightButtonUpHandler(object sender, MouseButtonEventArgs e) 
        {
            MinesweeperViewModel viewModel = DataContext as MinesweeperViewModel;
            if(viewModel != null)
            {
                var button = e.Source as Button;
                if(button != null)
                {
                    var cell = button.Tag as Cell;
                    viewModel.CellRightClickCommand.Execute(cell);
                }
            }
        }
    }
}
