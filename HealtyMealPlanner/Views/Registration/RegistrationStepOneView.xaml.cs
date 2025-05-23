using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using HealthyMealPlanner;

namespace HealthyMealPlanner.Views.Registration
{
    public partial class RegistrationStepOneView : Window
    {
        private string _email;
        private string _password;
        private bool _isPasswordVisible = false;

        public RegistrationStepOneView()
        {
            InitializeComponent();
            PasswordError.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var loggedOutView = new LoggedOutView();
            loggedOutView.Show();
            this.Close();
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isPasswordVisible)
                ValidateInput();
        }

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

            ValidateInput();
        }

        private void ValidateInput()
        {
            string email = EmailTextBox.Text.Trim();
            string password = _isPasswordVisible ? VisiblePasswordBox.Text : PasswordBox.Password;

            // Basic email validation
            bool isValidEmail = !string.IsNullOrEmpty(email) && email.Contains("@");

            // Password validation
            bool isValidPassword = !string.IsNullOrEmpty(password) &&
                                   password.Length >= 8 &&
                                   password.Any(char.IsUpper) &&
                                   password.Any(char.IsDigit) &&
                                   password.Any(c => !char.IsLetterOrDigit(c));

            // Show password error
            PasswordError.Visibility = (!string.IsNullOrEmpty(password) && !isValidPassword)
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Enable Next only if both are valid
            NextButton.IsEnabled = isValidEmail && isValidPassword;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = _isPasswordVisible ? VisiblePasswordBox.Text : PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isValidPassword = password.Length >= 8 &&
                                   password.Any(char.IsUpper) &&
                                   password.Any(char.IsDigit) &&
                                   password.Any(c => !char.IsLetterOrDigit(c));

            if (!isValidPassword)
            {
                PasswordError.Visibility = Visibility.Visible;
                return;
            }

            var data = new Data();
            if (data.CheckUserExists(email, ""))
            {
                MessageBox.Show("A user with this email already exists. Please try another one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            string role = AdminCheckBox.IsChecked == true ? "Admin" : "User";
            var stepTwo = new RegistrationStepTwoView(email, password, role);
            stepTwo.Show();
            this.Close();
        }
    }
}
