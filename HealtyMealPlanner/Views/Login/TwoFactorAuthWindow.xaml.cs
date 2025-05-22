using System.Windows;

namespace HealthyMealPlanner.Views
{
    public partial class TwoFactorAuthWindow : Window
    {
        private string expectedCode;

        public bool IsVerified { get; private set; } = false;

        public TwoFactorAuthWindow(string codeSentToUser)
        {
            InitializeComponent();
            expectedCode = codeSentToUser;
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            if (CodeInput.Text.Trim() == expectedCode)
            {
                IsVerified = true;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect code. Please try again.", "Verification Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
