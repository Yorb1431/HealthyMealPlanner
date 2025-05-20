using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using HealthyMealPlanner;

namespace HealthyMealPlanner.Views.Registration
{
    public partial class RegistrationStepOneView : Window
    {
        private string _email;
        private string _password;

        public RegistrationStepOneView()
        {
            InitializeComponent();
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
            ValidateInput();
        }

        private void ValidateInput()
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            
            // Basic email validation
            bool isValidEmail = !string.IsNullOrEmpty(email) && email.Contains("@");
            
            // Password validation
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$");
            bool isValidPassword = regex.IsMatch(password);

            // Enable button only if both email and password are valid
            NextButton.IsEnabled = isValidEmail && isValidPassword;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var regex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$");
            if (!regex.IsMatch(password))
            {
                PasswordError.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                PasswordError.Visibility = Visibility.Collapsed;
                var stepTwo = new RegistrationStepTwoView(email, password);
                stepTwo.Show();
                this.Close();
            }
        }
    }
} 