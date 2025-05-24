using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HealthyMealPlanner.Models;

namespace HealthyMealPlanner.Views
{
    public partial class RecipePage : UserControl
    {
        private readonly MainWindow _mainWindow;
        private readonly int _userId;
        private readonly string _dietType;

        public bool IsSelectionMode { get; set; } = false;
        public Action<Recipe> OnRecipeSelected { get; set; }

        public RecipePage(MainWindow mainWindow, int userId, string dietType)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _userId = userId;
            _dietType = dietType;

            DietTypeLabel.Text = $"Diet: {_dietType}";
            LoadRecipes();
        }

        private void LoadRecipes()
        {
            var data = new Data();

            var recipes = data.GetAllRecipesWithIngredients();
            var userAllergyIds = data.GetUserAllergyIds(_userId);
            var recipeAllergyMap = data.GetAllRecipeAllergyLinks();

            var userProfile = data.GetFullUserProfile(data.GetUsernameByUserId(_userId));
            var dietGoal = userProfile.DietGoal;

            // Apply diet type filter if not omnivore
            if (_dietType != "Omnivore")
            {
                recipes = recipes.Where(r => r.Category == _dietType).ToList();
            }

            // Apply allergy filtering
            recipes = recipes.Where(recipe =>
                !recipeAllergyMap.TryGetValue(recipe.RecipeID, out var recipeAllergies) ||
                !recipeAllergies.Intersect(userAllergyIds).Any()
            ).ToList();

            // Apply diet goal calorie filter
            int minCalories = 0;
            int maxCalories = int.MaxValue;

            if (dietGoal.Equals("Lose Weight", StringComparison.OrdinalIgnoreCase))
                maxCalories = 400;
            else if (dietGoal.Equals("Gain Weight", StringComparison.OrdinalIgnoreCase))
                minCalories = 700;
            // Maintain → no filtering needed

            recipes = recipes
                .Where(r => r.Calories >= minCalories && r.Calories <= maxCalories)
                .ToList();

            // Add fallback descriptions and randomize
            foreach (var r in recipes)
                r.Description ??= "No description provided.";

            recipes = recipes.OrderBy(r => Guid.NewGuid()).ToList();

            RecipeItemsControl.ItemsSource = recipes;
        }


        private void RecipeCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Recipe recipe)
            {
                if (IsSelectionMode)
                {
                    var detailWindow = new RecipeView(
                        recipe,
                        _userId,
                        onSelect: OnRecipeSelected,
                        closeOnSelect: true)
                    {
                        Owner = Window.GetWindow(this),
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    detailWindow.ShowDialog();
                }
                else
                {
                    var detailWindow = new RecipeView(recipe, _userId)
                    {
                        Owner = Window.GetWindow(this),
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    detailWindow.ShowDialog();
                }
            }
        }
    }
}
