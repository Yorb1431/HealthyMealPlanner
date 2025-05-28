using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class UserProfile : UserControl
    {
        private List<int> _selectedAllergyIds = new List<int>();
        private Dictionary<string, int> _allergyNameToId = new();
        private Dictionary<int, string> _allergyIdToName = new();
        private List<string> allergies = new List<string>();

        private string _username;
        private bool _isMetric;
        private string _profileImagePath;
        private string _role;
        private MainWindow _mainWindow;
        private bool _isVerified;
        private string verificationCode;
        private string _goal;
        private bool _isEditing = false;


        public UserProfile(MainWindow mainWindow, string username, double height, double weight, string dietType, List<string> allergies, bool isMetric, string profileImagePath, string role, string goal)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _username = username;
            _isMetric = isMetric;
            _profileImagePath = profileImagePath;
            _goal = goal;

            var data = new Data();
            _role = data.GetUserRole(username);
            var fullProfile = data.GetFullUserProfile(_username);
            _isVerified = fullProfile.IsVerified == true;

            var allergyMap = data.GetAllAllergies();
            _allergyNameToId = allergyMap;
            _allergyIdToName = allergyMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);


            // Show verification button only if not verified
            UpdateVerificationBadge();

            // Load image from database
            string imagePath = string.IsNullOrEmpty(_profileImagePath) ? "UserImages/defaultpicture.png" : _profileImagePath;
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
            if (File.Exists(fullPath))
            {
                ProfileImage.ImageSource = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
            }

            // Load rest of profile
            LoadProfile(height, weight, dietType, allergies, goal);

            // Show edit image buttons
            ChangeProfilePicture.Visibility = Visibility.Collapsed;
            DeleteProfilePicture.Visibility = Visibility.Collapsed;
        }

        private void LoadProfile(double height, double weight, string dietType, List<string> allergies, string goal)
        {
            if (_isMetric)
            {
                HeightBox.Text = height.ToString("0.0");
                WeightBox.Text = weight.ToString("0.0");
                HeightUnitLabel.Text = "cm";
                WeightUnitLabel.Text = "kg";
            }
            else
            {
                double imperialHeight = height / 2.54;
                double imperialWeight = weight * 2.20462;

                int totalInches = (int)Math.Round(imperialHeight);
                int feet = totalInches / 12;
                int inches = totalInches % 12;
                HeightBox.Text = $"{feet}'{inches}\"";
                WeightBox.Text = imperialWeight.ToString("0.0");
                HeightUnitLabel.Text = "feet";
                WeightUnitLabel.Text = "lbs";
            }

            RecalculateValues();

            foreach (ComboBoxItem item in DietComboBox.Items)
            {
                if (item.Content.ToString() == dietType)
                {
                    DietComboBox.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in GoalComboBox.Items)
            {
                if (item.Content.ToString().Equals(goal, StringComparison.OrdinalIgnoreCase))
                {
                    GoalComboBox.SelectedItem = item;
                    break;
                }
            }

            foreach (var allergy in allergies)
                AddAllergy(allergy);

            ToggleUnitButton.Visibility = Visibility.Collapsed;

            if (_role == "Admin")
            {
                AdminLabel.Visibility = Visibility.Visible;
            }

            // Update username display with badge
            UpdateVerificationBadge();
        }


        private void RecalculateValues()
        {
            double bmi = 0;
            double calories = 0;

            if (!_isMetric)
            {
                var heightParts = HeightBox.Text.Split(new[] { '\'', '"' }, StringSplitOptions.RemoveEmptyEntries);
                if (heightParts.Length < 1 || !int.TryParse(heightParts[0], out int feet)) return;
                int inches = (heightParts.Length >= 2 && int.TryParse(heightParts[1], out int i)) ? i : 0;

                double totalInches = feet * 12 + inches;
                if (!double.TryParse(WeightBox.Text, out double weightLbs)) return;

                bmi = (703 * weightLbs) / (totalInches * totalInches);
                calories = 66 + (6.23 * weightLbs) + (12.7 * totalInches) - (6.8 * 25);
            }
            else
            {
                if (!double.TryParse(HeightBox.Text, out double heightCm) || !double.TryParse(WeightBox.Text, out double weightKg)) return;
                double heightM = heightCm / 100;
                bmi = weightKg / (heightM * heightM);
                calories = 10 * weightKg + 6.25 * heightCm - 5 * 25 + 5;
            }

            BMIValue.Text = bmi.ToString("0.0");
            CaloriesValue.Text = ((int)calories).ToString();
        }

        private void UnitComboBox_Changed(object sender, SelectionChangedEventArgs e)
        {
            RecalculateValues();
        }
        private void CalculateBMI(double heightCm, double weightKg)
        {
            if (heightCm > 0)
            {
                double heightM = heightCm / 100;
                double bmi = weightKg / (heightM * heightM);
                BMIValue.Text = bmi.ToString("0.0");
            }
        }

        private void CalculateCalories(double heightCm, double weightKg)
        {
            double bmr = 10 * weightKg + 6.25 * heightCm - 5 * 25 + 5;
            CaloriesValue.Text = ((int)bmr).ToString();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow?.LoadHomepage(null, null);
        }

        private void AddAllergy(string allergy)
        {
            if (_allergyNameToId.ContainsKey(allergy) && !allergies.Contains(allergy))
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

                stack.Children.Add(text);

                if (_isEditing)
                {
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
                    stack.Children.Add(removeBtn);
                }

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
                ShowAllergySelector(); 
            }
        }


        private void AddAllergy_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string allergyName)
            {
                if (!_allergyNameToId.TryGetValue(allergyName, out int allergyId))
                    return;

                if (!allergies.Contains(allergyName))
                {
                    allergies.Add(allergyName);
                    _selectedAllergyIds.Add(allergyId);
                    UpdateAllergyList();
                    ShowAllergySelector(); // Refresh button colors
                }
            }
        }


        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            ToggleUnitButton.Visibility = Visibility.Collapsed;
            try
            {
                var data = new Data();
                int userId = data.GetUserIdByUsername(_username);
                string heightUnit = HeightUnitLabel.Text;
                string weightUnit = WeightUnitLabel.Text;

                // Determine isMetric
                bool isMetric = heightUnit == "cm" && weightUnit == "kg";

                double height, weight;

                if (heightUnit == "feet")
                {
                    var heightParts = HeightBox.Text.Split(new[] { '\'', '"' }, StringSplitOptions.RemoveEmptyEntries);
                    if (heightParts.Length < 1 || !int.TryParse(heightParts[0], out int feet))
                    {
                        MessageBox.Show("Please enter height in format: 5'11\"", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    int inches = (heightParts.Length >= 2 && int.TryParse(heightParts[1], out int i)) ? i : 0;
                    height = (feet * 12 + inches) * 2.54; // convert to cm
                }
                else if (!double.TryParse(HeightBox.Text, out height))
                {
                    MessageBox.Show("Please enter a valid height.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (weightUnit == "lbs")
                {
                    if (!double.TryParse(WeightBox.Text, out double weightLbs))
                    {
                        MessageBox.Show("Please enter a valid weight.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    weight = weightLbs / 2.20462; // convert to kg
                }
                else if (!double.TryParse(WeightBox.Text, out weight))
                {
                    MessageBox.Show("Please enter a valid weight.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string dietType = (DietComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";
                string goal = (GoalComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";

                // Collect updated allergies
                var updatedAllergyIds = new List<int>();
                foreach (var child in AllergyList.Children)
                {
                    if (child is Border border && border.Child is StackPanel stack)
                    {
                        foreach (var element in stack.Children)
                        {
                            if (element is TextBlock text && _allergyNameToId.TryGetValue(text.Text, out int id))
                            {
                                updatedAllergyIds.Add(id);
                                break;
                            }
                        }
                    }
                }
                data.SaveAllergies(userId, updatedAllergyIds);


                // Save to database
                bool imageSaved = true;

                if (ProfileImage.ImageSource is BitmapImage image)
                {
                    string imageName = Path.GetFileName(image.UriSource?.LocalPath);
                    string imagesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserImages");
                    string targetPath = Path.Combine(imagesDir, imageName);

                    try
                    {
                        Directory.CreateDirectory(imagesDir);

                        if (image.UriSource != null && File.Exists(image.UriSource.LocalPath))
                        {
                            File.Copy(image.UriSource.LocalPath, targetPath, true);
                        }

                        _profileImagePath = $"UserImages/{imageName}";
                    }
                    catch (Exception ex)
                    {
                        imageSaved = false;
                    }
                }
                else
                {
                    _profileImagePath = "UserImages/defaultpicture.png";
                }
                if (userId == -1)
                {
                    MessageBox.Show("Could not find user ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool success =
                    data.SaveMeasurements(userId, height.ToString(CultureInfo.InvariantCulture), weight.ToString(CultureInfo.InvariantCulture)) && 
                    data.SaveDietType(userId, dietType) &&
                    data.SaveAllergies(userId, updatedAllergyIds) &&
                    data.SaveDietGoal(userId, goal) &&
                    data.UpdateUnitPreference(userId, isMetric); // Update the unit preference
                    data.UpdateProfileImagePath(userId, _profileImagePath);

                if (success)
                {
                    CalculateBMI(height, weight);
                    CalculateCalories(height, weight);

                    HeightBox.IsEnabled = false;
                    WeightBox.IsEnabled = false;
                    GoalComboBox.IsEnabled = false;
                    DietComboBox.IsEnabled = false;
                    _isEditing = false;
                    UpdateAllergyList();

                    foreach (var child in AllergyList.Children)
                    {
                        if (child is Border border && border.Child is StackPanel stack)
                        {
                            foreach (var item in stack.Children)
                            {
                                if (item is Button btn)
                                    btn.IsEnabled = false;
                            }
                        }
                    }

                    SaveChangesButton.Visibility = Visibility.Collapsed;
                    DeleteAccountButton.Visibility = Visibility.Collapsed;
                    ChangeProfilePicture.Visibility = Visibility.Collapsed;
                    DeleteProfilePicture.Visibility = Visibility.Collapsed;
                    AllergySelector.Visibility = Visibility.Collapsed;
                    EditProfileButton.IsEnabled = true;

                }
                else
                {
                    MessageBox.Show("Failed to update profile data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error while saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PasswordInputDialog("If you really want to delete your account, enter your password:");
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string enteredPassword = dialog.EnteredPassword;

                var data = new Data();
                bool valid = data.ValidatePassword(_username, enteredPassword);

                if (!valid)
                {
                    MessageBox.Show("Incorrect password.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var confirm = MessageBox.Show(
                    "⚠ This decision is final. Are you sure you want to delete your account?",
                    "Final Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    int userId = data.GetUserIdByUsername(_username);
                    if (userId != -1)
                    {
                        data.DeleteUserAccount(userId);
                    }
                    var logout = new LoggedOutView();
                    logout.Show();

                    // Close MainWindow
                    Window mainWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is MainWindow);
                    mainWindow?.Close();
                }
            }
        }
        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            HeightBox.IsEnabled = true;
            WeightBox.IsEnabled = true;
            GoalComboBox.IsEnabled = true;
            DietComboBox.IsEnabled = true;
            _isEditing = true;

            AllergySelector.Visibility = Visibility.Visible;
            UpdateAllergyList();
            ShowAllergySelector();
            ToggleUnitButton.Visibility = Visibility.Visible;

            foreach (var child in AllergyList.Children)
            {
                if (child is Border border && border.Child is StackPanel stack)
                {
                    foreach (var item in stack.Children)
                    {
                        if (item is Button btn)
                            btn.IsEnabled = true;
                    }
                }
            }

            SaveChangesButton.Visibility = Visibility.Visible;
            DeleteAccountButton.Visibility = Visibility.Visible;
            ChangeProfilePicture.Visibility = Visibility.Visible;
            DeleteProfilePicture.Visibility = Visibility.Visible;
            EditProfileButton.IsEnabled = false;
        }



        private void DeleteProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            string defaultRelativePath = "UserImages/defaultpicture.png";
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultRelativePath);

            if (File.Exists(fullPath))
            {
                ProfileImage.ImageSource = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
                _profileImagePath = defaultRelativePath;

                var data = new Data();
                int userId = data.GetUserIdByUsername(_username);
                data.UpdateProfileImagePath(userId, _profileImagePath);
            }
        }
        private void ToggleUnit_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(WeightBox.Text, out double weight)) return;

            if (_isMetric)
            {
                if (!double.TryParse(HeightBox.Text, out double heightCm)) return;
                int totalInches = (int)Math.Round(heightCm / 2.54);
                int feet = totalInches / 12;
                int inches = totalInches % 12;
                HeightBox.Text = $"{feet}'{inches}\"";
                WeightBox.Text = (weight * 2.20462).ToString("0.0");
                HeightUnitLabel.Text = "feet";
                WeightUnitLabel.Text = "lbs";
                _isMetric = false;
            }
            else
            {
                var heightParts = HeightBox.Text.Split(new[] { '\'', '"' }, StringSplitOptions.RemoveEmptyEntries);
                if (heightParts.Length < 1 || !int.TryParse(heightParts[0], out int feet)) return;
                int inches = (heightParts.Length >= 2 && int.TryParse(heightParts[1], out int i)) ? i : 0;
                double heightCm = (feet * 12 + inches) * 2.54;
                HeightBox.Text = heightCm.ToString("0.0");
                WeightBox.Text = (weight / 2.20462).ToString("0.0");
                HeightUnitLabel.Text = "cm";
                WeightUnitLabel.Text = "kg";
                _isMetric = true;
            }
        }

        private void ChangeProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Select Profile Picture",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    string selectedFilePath = dlg.FileName;
                    string fileName = System.IO.Path.GetFileName(selectedFilePath);
                    string imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserImages");
                    Directory.CreateDirectory(imagesDir);

                    string newFilePath = System.IO.Path.Combine(imagesDir, fileName);

                    if (!string.Equals(selectedFilePath, newFilePath, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(selectedFilePath, newFilePath, true);
                    }

                    ProfileImage.ImageSource = new BitmapImage(new Uri(newFilePath, UriKind.Absolute));
                    _profileImagePath = $"UserImages/{fileName}";

                    // Update DB
                    var data = new Data();
                    int userId = data.GetUserIdByUsername(_username);
                    data.UpdateProfileImagePath(userId, _profileImagePath);

                }
                catch (Exception ex)
                {
                    //message alwys shows error due to WPF quirk
                }
            }
        }


        private void GetVerifiedButton_Click(object sender, RoutedEventArgs e)
        {
            verificationCode = new Random().Next(100000, 999999).ToString();

            var data = new Data();
            string userEmail = data.GetEmail(_username);
            data.SendEmailCode(userEmail, verificationCode, "Verification");

            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the verification code sent to your email:",
                "Email Verification");

            if (input == verificationCode)
            {
                if (data.MarkUserAsVerified(_username))
                {
                    _isVerified = true;
                    UpdateVerificationBadge();
                    MessageBox.Show("You're now verified! ✅");
                }
                else
                {
                    MessageBox.Show("Verification failed to update in database.");
                }
            }
            else
            {
                MessageBox.Show("Incorrect code.");
            }
        }


        private void UpdateVerificationBadge()
        {
            GetVerifiedButton.Visibility = _isVerified ? Visibility.Collapsed : Visibility.Visible;

            UsernamePanel.Children.Clear();
            UsernamePanel.Children.Add(new TextBlock
            {
                Text = _username,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                VerticalAlignment = VerticalAlignment.Center
            });

            if (_isVerified)
            {
                UsernamePanel.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Icons/checkmark.png")),
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(6, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                });
            }
        }

        private void ShowAllergySelector()
        {
            AllergySelector.Children.Clear();

            foreach (var kvp in _allergyNameToId)
            {
                string allergy = kvp.Key;
                bool isSelected = allergies.Contains(allergy);

                var btn = new Button
                {
                    Content = allergy,
                    Margin = new Thickness(4),
                    Tag = allergy,
                    Background = isSelected ? Brushes.IndianRed : Brushes.LightGray,
                    Foreground = Brushes.White,
                    Padding = new Thickness(8, 4, 8, 4),
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand
                };

                btn.Click += ToggleAllergySelection;
                AllergySelector.Children.Add(btn);
            }

            AllergySelector.Visibility = Visibility.Visible;
        }

        private void ToggleAllergySelection(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string allergy)
            {
                if (allergies.Contains(allergy))
                {
                    allergies.Remove(allergy);
                    btn.Background = Brushes.LightGray;
                }
                else
                {
                    allergies.Add(allergy);
                    btn.Background = Brushes.IndianRed;
                }

                UpdateAllergyList();
            }
        }




    }
}