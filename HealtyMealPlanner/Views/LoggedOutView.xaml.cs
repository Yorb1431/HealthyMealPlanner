using System.Windows;
using System.Diagnostics;

namespace HealthyMealPlanner.Views
{
    public partial class LoggedOutView : Window
    {
        public LoggedOutView()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginView = new LoginView();
            loginView.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registrationView = new Registration.RegistrationStepOneView();
            registrationView.Show();
            this.Close();
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Yorb1431/HealthyMealPlanner",
                UseShellExecute = true
            });
        }
    }
} 