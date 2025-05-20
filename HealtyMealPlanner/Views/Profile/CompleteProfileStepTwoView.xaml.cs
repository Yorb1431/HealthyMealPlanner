using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class CompleteProfileStepTwoView : Window
    {
        private Border _selectedOption = null;

        public CompleteProfileStepTwoView()
        {
            InitializeComponent();
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
            if (_selectedOption == null)
            {
                MessageBox.Show("Please select a diet type before continuing.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var stepThree = new CompleteProfileStepThreeView();
            stepThree.Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var stepOneView = new CompleteProfileStepOneView();
            stepOneView.Show();
            Close();
        }
    }
} 