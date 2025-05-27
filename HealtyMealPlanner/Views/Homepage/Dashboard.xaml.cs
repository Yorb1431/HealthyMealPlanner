using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HealthyMealPlanner.Views
{
    public partial class Dashboard : UserControl
    {
        private readonly Data data = new Data();
        private readonly string currentUsername;
        private readonly MainWindow _mainWindow;

        public Dashboard(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            currentUsername = App.Current.Properties["CurrentUsername"]?.ToString();
            LoadDashboardData();
            UserList.SelectionChanged += UserList_SelectionChanged;
            EditButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
        }

        private void LoadDashboardData()
        {
            try
            {
                // Load users with role 'User' only
                var users = data.GetAllUsersWithUserRole();
                UserList.ItemsSource = users;

                // Load system statistics
                int activeUsers = data.GetActiveUserCount();
                int totalMeals = data.GetTotalMeals();
                int mealPlansGenerated = data.GetMealPlanCount();

                ActiveUsersText.Text = activeUsers.ToString();
                TotalMealsText.Text = totalMeals.ToString();
                MealPlansText.Text = mealPlansGenerated.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow?.LoadHomepage(null, null);
        }

        private void ViewUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserList.SelectedItem is string username)
            {
                int userId = data.GetUserIdByUsername(username);
                if (userId != -1)
                {
                    var detailsWindow = new UserDetailsWindow(userId);
                    detailsWindow.Owner = Window.GetWindow(this);
                    detailsWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Could not find user ID for selected user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a user first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void ClearMealPlans_Click(object sender, RoutedEventArgs e)
        {
            if (UserList.SelectedItem is string selectedUser)
            {
                var confirm = MessageBox.Show(
                    $"This will delete all meal plans for '{selectedUser}'.\n\nContinue?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    int userId = data.GetUserIdByUsername(selectedUser);
                    if (userId != -1)
                    {
                        bool success = data.DeleteMealPlansForUser(userId);
                        if (success)
                        {
                            MessageBox.Show("Meal plans deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDashboardData();
                        }
                        else
                        {
                            MessageBox.Show("Error deleting meal plans.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserList.SelectedItem != null)
            {
                EditButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
                ClearMealPlans.Visibility = Visibility.Visible;
                ViewButton.Visibility = Visibility.Visible;
            }
            else
            {
                EditButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;
                ClearMealPlans.Visibility = Visibility.Collapsed;
                ViewButton.Visibility = Visibility.Collapsed;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserList.SelectedItem is string selectedUser)
            {
                var profile = data.GetFullUserProfile(selectedUser);
                var editWindow = new EditUserWindow(selectedUser, profile)
                {
                    Owner = Window.GetWindow(this)
                };

                bool? result = editWindow.ShowDialog();
                if (result == true)
                {
                    int userId = data.GetUserIdByUsername(selectedUser);
                    if (userId != -1)
                    {
                        bool success = data.UpdateBasicProfileInfo(
                            userId,
                            editWindow.FullName,
                            editWindow.Age,
                            editWindow.Gender,
                            editWindow.ActivityLevel,
                            editWindow.DietType
                        );

                        if (success)
                        {
                            MessageBox.Show("User updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDashboardData();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserList.SelectedItem is string selectedUser)
            {
                var result = MessageBox.Show(
                    $"⚠️ You are about to permanently delete the account for '{selectedUser}'.\n\nThis action cannot be undone.\n\nAre you sure you want to continue?",
                    "Confirm Permanent Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    int userId = data.GetUserIdByUsername(selectedUser);
                    if (userId != -1 && data.DeleteUserAccount(userId))
                    {
                        MessageBox.Show("User has been permanently deleted.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDashboardData();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
