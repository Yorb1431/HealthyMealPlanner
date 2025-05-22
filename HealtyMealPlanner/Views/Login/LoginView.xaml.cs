using System.Net.Mail;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using HealthyMealPlanner.ViewModels;
using System.Windows.Input;

namespace HealthyMealPlanner.Views
{
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
                MessageBox.Show($"Error initializing component: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var loggedOutView = new LoggedOutView();
            loggedOutView.Show();
            this.Close();
        }

        private bool _isPasswordVisible = false;

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                VisiblePasswordBox.Text = PasswordBox.Password;
                VisiblePasswordBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
                TogglePasswordButton.Content = "🙈";
            }
            else
            {
                PasswordBox.Password = VisiblePasswordBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                VisiblePasswordBox.Visibility = Visibility.Collapsed;
                TogglePasswordButton.Content = "👁";
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string usernameOrEmail = EmailTextBox.Text.Trim();
                string password = _isPasswordVisible ? VisiblePasswordBox.Text : PasswordBox.Password;

                if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please enter both username/email and password.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var data = new HealthyMealPlanner.Data();
                bool loggedIn = data.LoginUser(usernameOrEmail, password);

                if (loggedIn)
                {
                    // Get the full email address (important for 2FA)
                    string userEmail = data.GetEmail(usernameOrEmail); // You need to implement this if you don’t have it
                    string username = data.GetUsername(usernameOrEmail);

                    // 1. Generate 2FA Code
                    string code = GenerateVerificationCode();

                    // 2. Send Email
                    data.SendEmailCode(userEmail, code, "2FA");

                    // 3. Show 2FA Window
                    var twoFAWindow = new TwoFactorAuthWindow(code);
                    bool? result = twoFAWindow.ShowDialog();

                    if (result == true && twoFAWindow.IsVerified)
                    {
                        // 4. Open Main App
                        var mainWindow = new Views.MainWindow(username);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("2FA verification failed or was canceled.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username/email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error during login: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registrationView = new Registration.RegistrationStepOneView();
            registrationView.Show();
            this.Close();
        }


        private void ResetPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var resetWindow = new Views.ResetPasswordRequestWindow();
            resetWindow.Show();
            this.Close();
        }

    }
} 
