using System.Windows;
using System.Windows.Controls;
using AsteroidTracker.Models;

namespace AsteroidTracker
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is AsteroidModel asteroid)
            {
                var detailWindow = new AsteroidDetailWindow(asteroid);
                detailWindow.Owner = this;
                detailWindow.ShowDialog();
            }
        }
    }
}
