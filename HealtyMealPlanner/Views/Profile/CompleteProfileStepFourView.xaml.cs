using System.Windows;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class CompleteProfileStepFourView : Window
    {
        public CompleteProfileStepFourView()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var previousView = new CompleteProfileStepThreeView();
            previousView.Show();
            this.Close();
        }

        private void BeginButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
} 