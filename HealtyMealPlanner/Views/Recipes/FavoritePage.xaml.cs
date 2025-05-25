using System.Windows;
using System.Windows.Controls;
using HealthyMealPlanner.Models;

namespace HealthyMealPlanner.Views
{
    public partial class FavoritePage : UserControl
    {
        private readonly MainWindow _mainWindow;
        private readonly int _userId;

        public FavoritePage(MainWindow mainWindow, int userId)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _userId = userId;
            LoadFavorites();
        }

        private void LoadFavorites()
        {
            var data = new Data();
            var favorites = data.GetFavoriteRecipes(_userId);

            foreach (var recipe in favorites)
                recipe.Description ??= "No description provided.";

            RecipeItemsControl.ItemsSource = favorites;
        }

        private void RecipeCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is HealthyMealPlanner.Models.Recipe recipe)
            {
                var detailWindow = new RecipeView(recipe, _userId, () =>
                {
                    LoadFavorites(); // refresh after unfavorite
                }, closeOnUnfavorite: true)
                {
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                detailWindow.ShowDialog();
            }
        }
    }
}
