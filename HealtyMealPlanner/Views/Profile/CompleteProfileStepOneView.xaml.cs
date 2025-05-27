using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class CompleteProfileStepOneView : Window
    {
        private Border _selectedOption = null;
        private string _username;
        private string _email;
        private string _password;
        private string _fullName;
        private int _age;
        private string _gender;
        private string _role;

        public CompleteProfileStepOneView(string username, string email, string password, string fullName, int age, string gender, string role)
        {
            InitializeComponent();
            _username = username;
            _email = email;
            _password = password;
            _fullName = fullName;
            _age = age;
            _gender = gender;
            _role = role;
        }
        private void Option_Click(object sender, MouseButtonEventArgs e)
        {
            Border clickedBorder = GetParentBorder(e.OriginalSource as DependencyObject);
            if (clickedBorder == null)
            {
                MessageBox.Show("Could not determine selected option.");
                return;
            }

            // Reset previously
            if (_selectedOption != null)
            {
                _selectedOption.Background = Brushes.White;
                if (_selectedOption.Child is StackPanel stack)
                {
                    foreach (var child in stack.Children)
                    {
                        if (child is TextBlock tb)
                            tb.Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175)); // gray
                    }
                }
            }

            //  new selected option
            _selectedOption = clickedBorder;
            _selectedOption.Background = new SolidColorBrush(Color.FromRgb(190, 242, 200)); // green

            if (_selectedOption.Child is StackPanel selectedStack)
            {
                foreach (var child in selectedStack.Children)
                {
                    if (child is TextBlock tb)
                        tb.Foreground = new SolidColorBrush(Color.FromRgb(17, 24, 39)); // black
                }
            }

            NextButton.IsEnabled = true;
        }


        private Border GetParentBorder(DependencyObject element)
        {
            while (element != null && element is not Border)
            {
                element = VisualTreeHelper.GetParent(element);
            }
            return element as Border;
        }


        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedOption == null)
                {
                    MessageBox.Show("Please select a goal before continuing.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Get the selected goal
                string selectedGoal = _selectedOption?.Tag?.ToString() ?? "";

                
                var stepTwo = new CompleteProfileStepTwoView(_username, _email, _password, _fullName, _age, _gender, _role, selectedGoal);
                stepTwo.Show();
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error transitioning to next step: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 