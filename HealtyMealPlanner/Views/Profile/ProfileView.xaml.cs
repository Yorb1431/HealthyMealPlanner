using System.Windows;
using System.Windows.Controls;
using HealthyMealPlanner;
using HealthyMealPlanner.Views; 
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace HealthyMealPlanner
{
    public partial class ProfileView : Window
    {
        private readonly Data data;

        public ProfileView()
        {
            InitializeComponent();
            data = new Data();
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            try
            {
                string username = App.Current.Properties["CurrentUsername"]?.ToString();
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("No user is currently logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var profile = data.GetFullUserProfile(username);

                UsernameText.Text = username;
                EmailText.Text = profile.Email;
                FullNameText.Text = profile.FullName;
                AgeText.Text = profile.Age.ToString();
                GenderText.Text = profile.Gender;
                ActivityLevelText.Text = profile.ActivityLevel;
                DietTypeText.Text = profile.DietType;
                AllergiesText.Text = profile.Allergies.Count > 0 ? string.Join(", ", profile.Allergies) : "None";
                WeightGoalText.Text = profile.DietGoal;

                if (profile.Role == "Admin")
                {
                    RoleText.Visibility = Visibility.Visible;
                }

                if (profile.IsMetric)
                {
                    HeightText.Text = $"{profile.Height.ToString("0.0", CultureInfo.InvariantCulture)} cm";
                    WeightText.Text = $"{profile.Weight.ToString("0.0", CultureInfo.InvariantCulture)} kg";
                }
                else
                {
                    // Convert from cm (DB stores metric) to inches before display
                    double inchesTotal = profile.Height / 2.54;
                    int feet = (int)(inchesTotal / 12);
                    int inches = (int)Math.Round(inchesTotal % 12);
                    HeightText.Text = $"{feet}'{inches}\"";

                    double pounds = profile.Weight * 2.20462;
                    WeightText.Text = $"{pounds.ToString("0.0", CultureInfo.InvariantCulture)} lbs";
                }

                string imagePath = profile.ProfileImagePath ?? "UserImages/defaultpicture.png";
                string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);

                if (System.IO.File.Exists(fullPath))
                {
                    ProfileImageBrush.ImageSource = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show("Profile image file not found. Displaying default.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Edit profile functionality coming soon!", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LetsBeginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = App.Current.Properties["CurrentUsername"]?.ToString();
            if (!string.IsNullOrWhiteSpace(username))
            {
                var mainWindow = new MainWindow(username);
                mainWindow.Show();                        
                this.Close();                              
            }
        }
    }
}