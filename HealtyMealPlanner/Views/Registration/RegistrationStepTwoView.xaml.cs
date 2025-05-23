using System.Windows;
using System.Windows.Controls;
using HealthyMealPlanner;
using HealthyMealPlanner.Views.Profile;

namespace HealthyMealPlanner.Views.Registration
{
    public partial class RegistrationStepTwoView : Window
    {
        private string _email;
        private string _password;
        private string _username;
        private string _role;

        public RegistrationStepTwoView(string email, string password, string role)
        {
            InitializeComponent();
            _email = email;
            _password = password;
            _role = role;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var stepOne = new RegistrationStepOneView();
            stepOne.Show();
            this.Close();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _username = UsernameTextBox.Text.Trim();
                string fullName = FullNameTextBox.Text.Trim();
                string ageInput = AgeTextBox.Text.Trim();
                string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_email) ||
                    string.IsNullOrWhiteSpace(_password) || string.IsNullOrWhiteSpace(fullName) ||
                    string.IsNullOrWhiteSpace(ageInput) || string.IsNullOrWhiteSpace(gender))
                {
                    MessageBox.Show("Please fill in all fields.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(ageInput, out int age) || age < 0 || age > 120)
                {
                    MessageBox.Show("Please enter a valid age.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var data = new Data();
                if (data.CheckUserExists(_email, _username))
                {
                    MessageBox.Show("A user with this username already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Proceed to profile step one with full information, but do NOT save to DB yet
                var completeProfileStepOne = new Profile.CompleteProfileStepOneView(_username, _email, _password, fullName, age, gender, _role);

                completeProfileStepOne.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during registration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 
