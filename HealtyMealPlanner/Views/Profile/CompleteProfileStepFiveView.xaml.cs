using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using HealthyMealPlanner;
using System.Collections.Generic;
using System.Globalization;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class CompleteProfileStepFiveView : Window
    {
        private string _username;
        private string _email;
        private string _password;
        private string _fullName;
        private int _age;
        private string _gender;
        private string _role;
        private string _dietType;
        private string _profileImagePath;
        private string _goal;
        private readonly List<int> _selectedAllergieIds;
        private bool isMetric = true;

        // Conversion helpers
        private double ConvertCmToInches(double cm) => cm / 2.54;
        private double ConvertInchesToCm(double inches) => inches * 2.54;
        private double ConvertKgToLbs(double kg) => kg * 2.20462;
        private double ConvertLbsToKg(double lbs) => lbs / 2.20462;

        public CompleteProfileStepFiveView(string username, string email, string password, string fullName, int age, string gender,string role, string dietType, List<int> selectedAllergieIds, string profileImagePath, string goal)
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
            _selectedAllergieIds = selectedAllergieIds;
            _profileImagePath = profileImagePath;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var stepFour = new CompleteProfileStepFourView(_username, _email, _password, _fullName, _age, _gender, _role, _dietType, _selectedAllergieIds, _goal);
            stepFour.Show();
            this.Close();
        }

        private void UnitToggleButton_Click(object sender, RoutedEventArgs e)
        {
            bool heightParsed = double.TryParse(HeightBox.Text, out double heightVal);
            bool weightParsed = double.TryParse(WeightBox.Text, out double weightVal);

            if (isMetric)
            {
                HeightLabel.Text = "Height (ft/in):";
                WeightLabel.Text = "Weight (lbs):";
                UnitToggleButton.Content = "Switch to Metric";

                if (heightParsed)
                {
                    double totalInches = ConvertCmToInches(heightVal);
                    int feet = (int)(totalInches / 12);
                    int inches = (int)(totalInches % 12);
                    HeightBox.Text = $"{feet}'{inches}";
                }

                if (weightParsed)
                {
                    double lbs = Math.Round(ConvertKgToLbs(weightVal), 1);
                    WeightBox.Text = lbs.ToString();
                }
            }
            else
            {
                HeightLabel.Text = "Height (cm):";
                WeightLabel.Text = "Weight (kg):";
                UnitToggleButton.Content = "Switch to Imperial";

                if (!string.IsNullOrWhiteSpace(HeightBox.Text))
                {
                    var heightInput = HeightBox.Text.Trim();
                    var heightParts = heightInput
                        .Replace("′", "'")
                        .Replace("″", "\"")
                        .Split(new[] { '\'', '"', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    int feet = 0, inches = 0;
                    bool valid = false;

                    if (heightParts.Length >= 1 && int.TryParse(heightParts[0], out feet))
                    {
                        valid = true;
                        if (heightParts.Length >= 2)
                            int.TryParse(heightParts[1], out inches);
                    }

                    if (valid)
                    {
                        double cm = Math.Round(ConvertInchesToCm((feet * 12) + inches), 1);
                        HeightBox.Text = cm.ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(heightInput))
                    {
                        MessageBox.Show("Height format invalid. Use format like 6'2 or just 6'", "Format Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                if (weightParsed)
                {
                    double kg = Math.Round(ConvertLbsToKg(weightVal), 1);
                    WeightBox.Text = kg.ToString();
                }
            }

            isMetric = !isMetric;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ActivityLevelBox.Text))
                {
                    MessageBox.Show("Please select your activity level.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(HeightBox.Text) || string.IsNullOrWhiteSpace(WeightBox.Text))
                {
                    MessageBox.Show("Please enter both height and weight.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                double parsedHeight;
                double parsedWeight;

                if (isMetric)
                {
                    if (!double.TryParse(HeightBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedHeight) ||
                        !double.TryParse(WeightBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedWeight))
                    {
                        MessageBox.Show("Height and weight must be numeric values.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    var heightInput = HeightBox.Text.Trim();
                    var heightParts = heightInput
                        .Replace("′", "'")
                        .Replace("″", "\"")
                        .Split(new[] { '\'', '"', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (heightParts.Length >= 1 && int.TryParse(heightParts[0], out int feet))
                    {
                        int inches = 0;
                        if (heightParts.Length >= 2)
                            int.TryParse(heightParts[1], out inches);

                        parsedHeight = ConvertInchesToCm((feet * 12) + inches);
                    }
                    else
                    {
                        MessageBox.Show("Height format invalid. Use format like 6'2 or 5′ 7″", "Format Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!double.TryParse(WeightBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double lbs))
                    {
                        MessageBox.Show("Weight must be a numeric value.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    parsedWeight = ConvertLbsToKg(lbs);
                }

                var data = new Data();
                bool success = data.CreateUserAndProfile(_username, _email, _password, _fullName, _age, _gender, _role, _goal);
                if (!success)
                {
                    MessageBox.Show("Failed to create user account and profile.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int userId = data.GetUserIdByUsername(_username);
                if (userId == -1)
                {
                    MessageBox.Show("User ID not found after creation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string errorMessage = "";

                // Reuse the `success` variable instead of redeclaring
                success = true;

                if (!data.SaveActivityLevel(userId, ActivityLevelBox.Text))
                {
                    success = false;
                    errorMessage = "Failed to save activity level.";
                }

                if (success && !data.SaveMeasurements(userId, parsedHeight.ToString(CultureInfo.InvariantCulture), parsedWeight.ToString(CultureInfo.InvariantCulture)))
                {
                    success = false;
                    errorMessage = "Failed to save measurements.";
                }

                if (success && !data.SaveDietType(userId, _dietType))
                {
                    success = false;
                    errorMessage = "Failed to save diet type.";
                }

                if (success && !data.SaveWeightGoal(userId, _goal))
                {
                    success = false;
                    errorMessage = "Failed to save weight goal.";
                }

                if (success && !data.SaveAllergies(userId, _selectedAllergieIds))
                {
                    success = false;
                    errorMessage = "Failed to save allergies.";
                }

                if (success && !data.SaveUnitPreference(userId, isMetric))
                {
                    success = false;
                    errorMessage = "Failed to save unit preference.";
                }

                if (success && !data.SaveProfileImagePath(userId, _profileImagePath))
                {
                    success = false;
                    errorMessage = "Failed to save profile image path.";
                }

                if (success)
                {
                    App.Current.Properties["CurrentUsername"] = _username;
                    new ProfileView().Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Failed to create profile: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}