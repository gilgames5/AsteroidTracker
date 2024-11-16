using System.Windows;

namespace AsteroidTracker
{
    public partial class AsteroidDetailWindow : Window
    {
        public AsteroidDetailWindow(AsteroidModel asteroid)
        {
            InitializeComponent();
            DataContext = asteroid;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
