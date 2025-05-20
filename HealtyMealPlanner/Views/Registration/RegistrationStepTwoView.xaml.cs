using System.Windows;
using HealthyMealPlanner;

namespace HealthyMealPlanner.Views.Registration
{
    public partial class RegistrationStepTwoView : Window
    {
        private string _email;
        private string _password;

        public RegistrationStepTwoView(string email, string password)
        {
            InitializeComponent();
            _email = email;
            _password = password;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var stepOne = new RegistrationStepOneView();
            stepOne.Show();
            this.Close();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var data = new Data();
            bool success = data.RegisterUser(username, _email, _password);
            if (success)
            {
                MessageBox.Show("Registration successful!");
                var completeProfileStepOne = new Profile.CompleteProfileStepOneView();
                completeProfileStepOne.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Registration failed. Username or email may already exist.");
            }
        }
    }
} 