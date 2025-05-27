using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HealthyMealPlanner;
using System.Linq;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class CompleteProfileStepTwoView : Window
    {
        private Border _selectedOption = null;
        private string _username;
        private string _email;
        private string _password;
        private string _fullName;
        private int _age;
        private string _gender;
        private string _role;
        private string _goal;
        private readonly Data _data;

        public CompleteProfileStepTwoView(string username, string email, string password, string fullName, int age, string gender, string role, string goal)
        {
            InitializeComponent();
            _username = username;
            _email = email;
            _password = password;
            _fullName = fullName;
            _age = age;
            _gender = gender;
            _role = role;
            _goal = goal;
            _data = new Data();
        }

        private void Option_Click(object sender, MouseButtonEventArgs e)
        {
            if (_selectedOption != null)
            {
                _selectedOption.Background = Brushes.White;
            }
            _selectedOption = sender as Border;
            _selectedOption.Background = new SolidColorBrush(Color.FromRgb(190, 242, 200)); // #BEF2C8
            NextButton.IsEnabled = true;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedOption == null)
                {
                    MessageBox.Show("Please select a diet type.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Option chosen by user
                string selectedDietType = _selectedOption.Name switch
                {
                    "OptionEverything" => "Omnivore",
                    "OptionVegetarian" => "Vegetarian",
                    "OptionVegan" => "Vegan",
                    "OptionKeto" => "Keto",
                    "OptionPaleo" => "Paleo",
                    _ => "Unknown"
                };

                var stepThree = new CompleteProfileStepThreeView(_username, _email, _password, _fullName, _age, _gender, _role, selectedDietType, _goal);
                stepThree.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var stepOne = new CompleteProfileStepOneView(_username, _email, _password, _fullName, _age, _gender, _role);
            stepOne.Show();
            this.Close();
        }
    }
} 