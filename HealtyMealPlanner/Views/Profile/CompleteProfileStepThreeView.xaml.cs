using System.Collections.Generic;
using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class CompleteProfileStepThreeView : Window
    {
        private readonly List<Border> _selectedAllergies = new List<Border>();
        private readonly List<string> _selectedAllergyNames = new List<string>(); // Track selected allergy names

        private readonly List<(string Name, string Icon)> _allergies = new List<(string, string)>
        {
            ("Egg", "M12 2C7.03 2 2.73 7.11 2.73 12.5c0 5.39 4.3 9.5 9.27 9.5s9.27-4.11 9.27-9.5C21.27 7.11 16.97 2 12 2zm0 17c-3.31 0-6-2.69-6-6 0-3.31 2.69-6 6-6s6 2.69 6 6c0 3.31-2.69 6-6 6z"),
            ("Milk", "M12 2C8.13 2 5 5.13 5 9c0 3.87 3.13 7 7 7s7-3.13 7-7c0-3.87-3.13-7-7-7zm0 12c-2.76 0-5-2.24-5-5 0-2.76 2.24-5 5-5s5 2.24 5 5c0 2.76-2.24 5-5 5z"),
            ("Nut", "M12 2C6.48 2 2 6.48 2 12c0 5.52 4.48 10 10 10s10-4.48 10-10C22 6.48 17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8 0-4.41 3.59-8 8-8s8 3.59 8 8c0 4.41-3.59 8-8 8zm-1-13h2v6h-2zm0 8h2v2h-2z"),
            ("Soybean", "M12 2C6.48 2 2 6.48 2 12c0 5.52 4.48 10 10 10s10-4.48 10-10C22 6.48 17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8 0-4.41 3.59-8 8-8s8 3.59 8 8c0 4.41-3.59 8-8 8zm-1-13h2v6h-2zm0 8h2v2h-2z"),
            ("Fish", "M2 12c0-5.52 4.48-10 10-10s10 4.48 10 10-4.48 10-10 10S2 17.52 2 12zm10 8c4.41 0 8-3.59 8-8s-3.59-8-8-8-8 3.59-8 8 3.59 8 8 8zm-1-13h2v6h-2zm0 8h2v2h-2z"),
            ("Wheat", "M2 12c0-5.52 4.48-10 10-10s10 4.48 10 10-4.48 10-10 10S2 17.52 2 12zm10 8c4.41 0 8-3.59 8-8s-3.59-8-8-8-8 3.59-8 8 3.59 8 8 8zm-1-13h2v6h-2zm0 8h2v2h-2z"),
            ("Celery", "M2 12c0-5.52 4.48-10 10-10s10 4.48 10 10-4.48 10-10 10S2 17.52 2 12zm10 8c4.41 0 8-3.59 8-8s-3.59-8-8-8-8 3.59-8 8 3.59 8 8 8zm-1-13h2v6h-2zm0 8h2v2h-2z"),
            ("Crustacean", "M2 12c0-5.52 4.48-10 10-10s10 4.48 10 10-4.48 10-10 10S2 17.52 2 12zm10 8c4.41 0 8-3.59 8-8s-3.59-8-8-8-8 3.59-8 8 3.59 8 8 8zm-1-13h2v6h-2zm0 8h2v2h-2z"),
            ("Mustard", "M2 12c0-5.52 4.48-10 10-10s10 4.48 10 10-4.48 10-10 10S2 17.52 2 12zm10 8c4.41 0 8-3.59 8-8s-3.59-8-8-8-8 3.59-8 8 3.59 8 8 8zm-1-13h2v6h-2zm0 8h2v2h-2z"),
        };

        public CompleteProfileStepThreeView()
        {
            InitializeComponent();
            PopulateAllergyGrid();
        }

        private void PopulateAllergyGrid()
        {
            AllergyGrid.Children.Clear();
            foreach (var (name, icon) in _allergies)
            {
                var border = new Border
                {
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(40),
                    Margin = new Thickness(8),
                    Padding = new Thickness(8),
                    Cursor = Cursors.Hand,
                    Effect = (Effect)FindResource("ShadowEffect"),
                    Tag = name // Store the allergy name for easy access
                };
                var stack = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
                var path = new Path
                {
                    Data = Geometry.Parse(icon),
                    Fill = new SolidColorBrush(Color.FromRgb(21, 128, 61)),
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
                stack.Children.Add(path);
                stack.Children.Add(text);
                border.Child = stack;
                border.MouseLeftButtonUp += (s, e) => ToggleAllergy(border);
                AllergyGrid.Children.Add(border);
            }
        }

        private void ToggleAllergy(Border border)
        {
            string allergyName = border.Tag as string;
            if (_selectedAllergies.Contains(border))
            {
                border.Background = Brushes.White;
                _selectedAllergies.Remove(border);
                _selectedAllergyNames.Remove(allergyName);
            }
            else
            {
                border.Background = new SolidColorBrush(Color.FromRgb(190, 242, 200)); // #BEF2C8
                _selectedAllergies.Add(border);
                if (!_selectedAllergyNames.Contains(allergyName))
                    _selectedAllergyNames.Add(allergyName);
            }

            // Zet knop aan of uit
            NextButton.IsEnabled = _selectedAllergyNames.Count > 0;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // _selectedAllergyNames now contains all selected allergies as strings
            var stepFour = new CompleteProfileStepFourView();
            stepFour.Show();
            this.Close();
        }
    }
}