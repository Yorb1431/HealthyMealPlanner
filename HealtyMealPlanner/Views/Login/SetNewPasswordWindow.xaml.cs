using System;
using System.Linq;
using System.Windows;

namespace HealthyMealPlanner.Views
{
    public partial class SetNewPasswordWindow : Window
    {
        private string userEmail;

        public SetNewPasswordWindow(string email)
        {
            InitializeComponent();
            userEmail = email;
        }

        private void UpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            string newPass = NewPasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            if (newPass != confirm)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            bool isValidPassword = !string.IsNullOrEmpty(newPass) &&
                                   newPass.Length >= 8 &&
                                   newPass.Any(char.IsUpper) &&
                                   newPass.Any(char.IsDigit) &&
                                   newPass.Any(c => !char.IsLetterOrDigit(c));

            if (!isValidPassword)
            {
                MessageBox.Show("Password must be at least 8 characters, include an uppercase letter, a number, and a special character.", "Weak Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var data = new Data();
            bool updated = data.UpdateUserPassword(userEmail, newPass);

            if (updated)
            {
                MessageBox.Show("Password updated successfully.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to update password.");
            }
        }
    }
}
