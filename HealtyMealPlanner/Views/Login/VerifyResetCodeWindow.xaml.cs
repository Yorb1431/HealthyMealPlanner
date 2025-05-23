using System;
using System.Windows;

namespace HealthyMealPlanner.Views.Login
{
    public partial class VerifyResetCodeWindow : Window
    {
        private string expectedCode;
        private string userEmail;

        public VerifyResetCodeWindow(string code, string email)
        {
            InitializeComponent();
            expectedCode = code;
            userEmail = email;
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            if (ResetCodeInput.Text.Trim() == expectedCode)
            {
                var setNewPasswordWindow = new SetNewPasswordWindow(userEmail);

                //return to the login screen
                setNewPasswordWindow.Closed += (s, args) =>
                {
                    var loginView = new LoginView();
                    loginView.Show();
                };

                setNewPasswordWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect code. Try again.");
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var loginView = new LoginView();
            loginView.Show();

            // Close this window
            this.Close();
        }
    }
}
