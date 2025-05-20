using System.Windows;
using HealthyMealPlanner.ViewModels;

namespace HealthyMealPlanner.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
                InitializeComponent();
            
            // Set the window's DataContext
            DataContext = new MainViewModel();
        }
    }
} 