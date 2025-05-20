using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class CompleteProfileStepOneView : Window
    {
        private Border _selectedOption = null;

        public CompleteProfileStepOneView()
        {
            InitializeComponent();
        }

        private void Option_Click(object sender, MouseButtonEventArgs e)
        {
            if (_selectedOption != null)
            {
                _selectedOption.Background = Brushes.White;
                // Set text color to gray for all TextBlocks in the unselected option
                if (_selectedOption.Child is StackPanel stack)
                {
                    foreach (var child in stack.Children)
                    {
                        if (child is TextBlock tb)
                            tb.Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175)); // #9CA3AF (gray-400)
                    }
                }
            }
            _selectedOption = sender as Border;
            _selectedOption.Background = new SolidColorBrush(Color.FromRgb(190, 242, 200)); // #BEF2C8
            // Set text color to dark for all TextBlocks in the selected option
            if (_selectedOption.Child is StackPanel selectedStack)
            {
                foreach (var child in selectedStack.Children)
                {
                    if (child is TextBlock tb)
                        tb.Foreground = new SolidColorBrush(Color.FromRgb(17, 24, 39)); // #111827 (gray-900)
                }
            }
            NextButton.IsEnabled = true;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOption == null)
            {
                MessageBox.Show("Please select a goal before continuing.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the selected goal text
            string selectedGoal = "";
            if (_selectedOption.Child is StackPanel stack)
            {
                foreach (var child in stack.Children)
                {
                    if (child is TextBlock tb && tb.FontWeight == FontWeights.Bold)
                    {
                        selectedGoal = tb.Text;
                        break;
                    }
                }
            }

            // Store the selected goal (you might want to store this in a user profile or settings)
            // For now, we'll just proceed to the next view
            var stepTwo = new CompleteProfileStepTwoView();
            stepTwo.Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var registrationStepTwoView = new Registration.RegistrationStepTwoView();
            registrationStepTwoView.Show();
            Close();
        }
    }
} 