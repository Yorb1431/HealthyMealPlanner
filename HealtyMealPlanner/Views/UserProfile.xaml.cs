using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class UserProfile : Window
    {
        private List<string> allergies = new List<string>();

        public UserProfile()
        {
            InitializeComponent();
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            HeightBox.Text = "170";
            WeightBox.Text = "70";
            ServingsBox.Text = "2";
            BMIValue.Text = "--.--";
            CaloriesValue.Text = "--";
            AddAllergy("Dairy");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddAllergy(string allergy)
        {
            if (!allergies.Contains(allergy))
            {
                allergies.Add(allergy);
                UpdateAllergyList();
            }
        }

        private void UpdateAllergyList()
        {
            AllergyList.Children.Clear();

            foreach (var allergy in allergies)
            {
                var border = new Border
                {
                    Background = System.Windows.Media.Brushes.MistyRose,
                    CornerRadius = new CornerRadius(15),
                    Padding = new Thickness(6, 2, 6, 2),
                    Margin = new Thickness(4)
                };

                var stack = new StackPanel { Orientation = Orientation.Horizontal };

                var text = new TextBlock
                {
                    Text = allergy,
                    Foreground = System.Windows.Media.Brushes.DarkRed,
                    Margin = new Thickness(0, 0, 4, 0)
                };

                var removeBtn = new Button
                {
                    Content = "✕",
                    Background = System.Windows.Media.Brushes.Transparent,
                    Foreground = System.Windows.Media.Brushes.DarkRed,
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(0),
                    Tag = allergy
                };
                removeBtn.Click += RemoveAllergy_Click;

                stack.Children.Add(text);
                stack.Children.Add(removeBtn);
                border.Child = stack;

                AllergyList.Children.Add(border);
            }
        }

        private void RemoveAllergy_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string allergy)
            {
                allergies.Remove(allergy);
                UpdateAllergyList();
            }
        }

        private void AddAllergy_Click(object sender, RoutedEventArgs e)
        {
            string newAllergy = NewAllergy.Text.Trim();
            if (!string.IsNullOrWhiteSpace(newAllergy))
            {
                AddAllergy(newAllergy);
                NewAllergy.Text = string.Empty;
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Your changes have been saved successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete your account?", "Delete Account", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Your account has been deleted.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }

        private void ChangeProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg"
            };
            if (dlg.ShowDialog() == true)
            {
                ProfileImage.Source = new BitmapImage(new Uri(dlg.FileName));
            }
        }

        private void DeleteProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            ProfileImage.Source = null;
        }
    }
}