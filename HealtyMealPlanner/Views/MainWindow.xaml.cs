using System.Windows;
using HealthyMealPlanner.ViewModels;

namespace HealthyMealPlanner.Views
{
    public partial class MainWindow : Window
    {
        private readonly int _userId;

        public MainWindow(string username)
        {
            try
            {
                InitializeComponent();

                DataContext = new MainViewModel
                {
                    UserName = username
                };

                var data = new Data();
                _userId = data.GetUserId(username); // Store the user ID

                if (data.GetUserRole(username) == "Admin")
                {
                    DashboardButton.Visibility = Visibility.Visible;
                }

                LoadHomepage(null, null);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error initializing MainWindow: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public MainWindow() : this("Guest") { }

        public void LoadHomepage(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = DataContext as MainViewModel;
                var username = vm?.UserName ?? "Guest";
                var data = new Data();
                var role = data.GetUserRole(username);

                var homepage = new Homepage(username, role);
                MainContent.Content = homepage;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading homepage: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProfile(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = DataContext as MainViewModel;
                var data = new Data();
                var fullProfile = data.GetFullUserProfile(vm?.UserName ?? "Guest");

                var profileView = new Profile.UserProfile(
                    this,
                    vm?.UserName ?? "Guest",
                    fullProfile.Height,
                    fullProfile.Weight,
                    fullProfile.DietType,
                    fullProfile.Allergies,
                    fullProfile.IsMetric,
                    fullProfile.ProfileImagePath,
                    fullProfile.Role,
                    fullProfile.DietGoal);

                MainContent.Content = profileView;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDashboard(object sender, RoutedEventArgs e)
        {
            try
            {
                MainContent.Content = new Dashboard(this);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMealPlans(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = new Data();
                string username = data.GetUsernameByUserId(_userId);
                MainContent.Content = new MealPlansPage(this, username);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading meal plans: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecipeBrowser(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = new Data();
                string dietType = data.GetDietTypeByUserId(_userId);
                MainContent.Content = new RecipePage(this, _userId, dietType);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading recipes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void LoadFavorites(object sender, RoutedEventArgs e)
        {
            try 
            {
                MainContent.Content = new FavoritePage(this, _userId);
            }

            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading favorites: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loggedOutView = new LoggedOutView();
            loggedOutView.Show();
            this.Close();
        }
    }
}
