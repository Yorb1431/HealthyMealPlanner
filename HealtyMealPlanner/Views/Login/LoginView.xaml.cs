using System.Windows;
using System.Windows.Controls;

namespace HealthyMealPlanner.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            try
            {
                InitializeComponent();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error initializing component: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var loggedOutView = new LoggedOutView();
            loggedOutView.Show();
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string usernameOrEmail = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            var data = new HealthyMealPlanner.Data();
            bool loggedIn = data.LoginUser(usernameOrEmail, password);

            if (loggedIn)
            {
                MessageBox.Show("Login successful!");
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username/email or password.");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registrationView = new Registration.RegistrationStepOneView();
            registrationView.Show();
            this.Close();
        }
    }
} 