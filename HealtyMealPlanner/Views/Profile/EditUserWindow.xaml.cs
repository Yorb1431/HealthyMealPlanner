using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;

namespace HealthyMealPlanner.Views
{
    public partial class EditUserWindow : Window
    {
        public string FullName => FullNameBox.Text.Trim();
        public int Age => int.TryParse(AgeBox.Text.Trim(), out int age) ? age : 0;
        public string Gender => (GenderCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
        public string ActivityLevel => (ActivityCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
        public string DietType => (DietCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

        public EditUserWindow(string username, Data.FullUserProfile profile)
        {
            InitializeComponent();

            // Set window title immediately
            WindowTitle.Text = $"Edit {username}'s Profile";

            // Load profile picture using consistent fallback method
            string imagePath = string.IsNullOrEmpty(profile.ProfileImagePath) ? "UserImages/defaultpicture.png" : profile.ProfileImagePath;
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
            if (File.Exists(fullPath))
            {
                ProfileImage.Fill = new ImageBrush(new BitmapImage(new Uri(fullPath, UriKind.Absolute)))
                {
                    Stretch = Stretch.UniformToFill
                };
            }

            // Populate values immediately — works because XAML ComboBoxItems are available right after InitializeComponent
            FullNameBox.Text = profile.FullName;
            AgeBox.Text = profile.Age.ToString();

            // Assign values based on string content
            SetComboBoxByValue(GenderCombo, profile.Gender);
            SetComboBoxByValue(ActivityCombo, profile.ActivityLevel);
            SetComboBoxByValue(DietCombo, profile.DietType);
        }

        private void SetComboBoxByValue(ComboBox comboBox, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            foreach (var item in comboBox.Items)
            {
                if (item is ComboBoxItem cbItem &&
                    string.Equals(cbItem.Content?.ToString()?.Trim(), value.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedItem = cbItem;
                    return;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}