using System;
using System.Windows;

namespace HealthyMealPlanner.Views
{
    public partial class ResetPasswordRequestWindow : Window
    {
        private string resetCode;
        private string userEmail;

        public ResetPasswordRequestWindow()
        {
            InitializeComponent();
        }

        private void SendCode_Click(object sender, RoutedEventArgs e)
        {
            string input = EmailTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Enter your email or username.");
                return;
            }

            var data = new Data();
            userEmail = data.GetEmail(input);
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                MessageBox.Show("No account found for this user.");
                return;
            }

            resetCode = GenerateVerificationCode();

            data.SendEmailCode(userEmail, resetCode, "Reset");

            var verifyWindow = new Login.VerifyResetCodeWindow(resetCode, userEmail);
            verifyWindow.Show();
            this.Close();
        }

        private string GenerateVerificationCode()
        {
            Random r = new Random();
            return r.Next(100000, 999999).ToString();
        }
    }
}
