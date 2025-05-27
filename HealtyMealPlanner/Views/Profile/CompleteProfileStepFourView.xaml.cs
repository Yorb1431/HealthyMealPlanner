using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HealthyMealPlanner.Views.Profile
{
    public partial class CompleteProfileStepFourView : Window
    {
        private string _username;
        private string _email;
        private string _password;
        private string _fullName;
        private int _age;
        private string _gender;
        private string _dietType;
        private string _role;
        private string _goal;
        private string _profileImagePath;
        private readonly List<int> _selectedAllergieIds;
        public CompleteProfileStepFourView(string username, string email, string password, string fullName, int age, string gender,string role,  string dietType, List<int> selectedAllergieIds, string goal)
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

            // default picture
            string defaultPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserImages", "defaultpicture.png");
            if (File.Exists(defaultPath))
            {
                _profileImagePath = defaultPath; // assign this first
                ProfileImage.ImageSource = new BitmapImage(new Uri(_profileImagePath, UriKind.Absolute));
            }
            else
            {
                MessageBox.Show($"Default profile picture not found at: {defaultPath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var stepThree = new CompleteProfileStepThreeView(_username, _email, _password, _fullName, _age, _gender, _role, _dietType, _goal);
            stepThree.Show();
            this.Close();
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Select Profile Picture",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dlg.ShowDialog() == true)
            {
                _profileImagePath = dlg.FileName;
                ProfileImage.ImageSource = new BitmapImage(new Uri(_profileImagePath, UriKind.Absolute));
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserImages");
                Directory.CreateDirectory(imagesDir);

                string relativePath;

                if (!string.IsNullOrEmpty(_profileImagePath) && !string.Equals(System.IO.Path.GetFileName(_profileImagePath), "defaultpicture.png", StringComparison.OrdinalIgnoreCase))
                {
                    string originalFileName = System.IO.Path.GetFileName(_profileImagePath);
                    string targetPath = System.IO.Path.Combine(imagesDir, originalFileName);

                    File.Copy(_profileImagePath, targetPath, true);
                    relativePath = $"UserImages/{originalFileName}";
                }
                else
                {
                    relativePath = "UserImages/defaultpicture.png";
                }

                // Pass the relative path to the next step
                var nextStep = new CompleteProfileStepFiveView(_username, _email, _password, _fullName, _age, _gender,_role, _dietType, _selectedAllergieIds, relativePath, _goal);
                nextStep.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while preparing profile picture: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
