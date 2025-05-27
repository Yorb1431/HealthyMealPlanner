using System.Collections.Generic;
using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using HealthyMealPlanner;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class CompleteProfileStepThreeView : Window
    {
        private readonly List<string> _selectedAllergyNames = new List<string>(); // Track selected allergy names
        private string _username;
        private string _email;
        private string _password;
        private string _fullName;
        private int _age;
        private string _gender;
        private string _dietType;
        private string _role;
        private string _goal;
        private readonly List<int> _selectedAllergieIds = new List<int>();
        private readonly Data _data;

        private readonly List<(int ID, string Name, string Icon)> _allergies = new()
        {
            (1, "Egg", "pack://application:,,,/Icons/egg.png"),
            (2, "Milk", "pack://application:,,,/Icons/milk.png"),
            (3, "Nuts", "pack://application:,,,/Icons/nut.png"),
            (4, "Soybean", "pack://application:,,,/Icons/soybean.png"),
            (5, "Fish", "pack://application:,,,/Icons/fish.png"),
            (6, "Wheat", "pack://application:,,,/Icons/wheat.png"),
            (7, "Celery", "pack://application:,,,/Icons/celery.png"),
            (8, "Shellfish", "pack://application:,,,/Icons/crustacean.png"),
            (9, "Sesame", "pack://application:,,,/Icons/sesame.png"),
        }; 


        public CompleteProfileStepThreeView(string username, string email, string password, string fullName, int age, string gender, string role , string dietType, string goal)
        {
            InitializeComponent();
            _username = username;
            _email = email;
            _password = password;
            _fullName = fullName;
            _age = age;
            _gender = gender;
            _role = role;
            _dietType = dietType;
            _goal = goal;
            _selectedAllergieIds = new List<int>();
            _data = new Data();
            PopulateAllergyGrid();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var stepTwo = new CompleteProfileStepTwoView(_username, _email, _password, _fullName, _age, _gender, _role, _goal);
            stepTwo.Show();
            this.Close();
        }
        private void PopulateAllergyGrid()
        {
            AllergyGrid.Children.Clear();
            foreach (var (id, name, icon) in _allergies)
            {
                var border = new Border
                {
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(40),
                    Margin = new Thickness(8),
                    Padding = new Thickness(8),
                    Cursor = Cursors.Hand,
                    Effect = (Effect)FindResource("ShadowEffect"),
                    Tag = id // Store AllergyID
                };

                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var image = new Image
                {
                    Source = new ImageSourceConverter().ConvertFromString(icon) as ImageSource,
                    Width = 32,
                    Height = 32,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var text = new TextBlock
                {
                    Text = name,
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromRgb(55, 65, 81)),
                    Margin = new Thickness(0, 8, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                stack.Children.Add(image);
                stack.Children.Add(text);
                border.Child = stack;
                border.MouseLeftButtonUp += (s, e) => ToggleAllergy(border);
                AllergyGrid.Children.Add(border);
            }
        }


        private void ToggleAllergy(Border border)
        {
            int allergyId = (int)border.Tag;
            if (_selectedAllergieIds.Contains(allergyId))
            {
                border.Background = Brushes.White;
                _selectedAllergieIds.Remove(allergyId);
            }
            else
            {
                border.Background = new SolidColorBrush(Color.FromRgb(190, 242, 200)); // green
                _selectedAllergieIds.Add(allergyId);
            }


            NextButton.IsEnabled = true;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stepFour = new CompleteProfileStepFourView(_username, _email, _password, _fullName, _age, _gender,_role, _dietType,_selectedAllergieIds, _goal);
                stepFour.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}