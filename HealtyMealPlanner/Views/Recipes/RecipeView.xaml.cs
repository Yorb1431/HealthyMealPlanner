using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using HealthyMealPlanner.Models;
using System.Windows.Media;

namespace HealthyMealPlanner.Views
{
    public partial class RecipeView : Window
    {
        private readonly int _userid;
        private readonly HealthyMealPlanner.Models.Recipe _recipe;
        private readonly Action _onUnfavorited;
        private bool isFavorited;
        private readonly bool _closeOnUnfavorite;
        private readonly Action<Recipe> _onSelect;
        private readonly bool _closeOnSelect;
        public RecipeView(HealthyMealPlanner.Models.Recipe recipe, int userId, Action onUnfavorited = null, bool closeOnUnfavorite = false, Action<Recipe> onSelect = null, bool closeOnSelect = false)
        {
            InitializeComponent();

            _userid = userId;
            _recipe = recipe;
            _onUnfavorited = onUnfavorited;
            _closeOnUnfavorite = closeOnUnfavorite;
            _onSelect = onSelect;
            _closeOnSelect = closeOnSelect;

            DataContext = _recipe;

            // Show AddToMealPlan button if selection mode is active
            if (_onSelect != null)
            {
                AddToMealPlanButton.Visibility = Visibility.Visible;
            }

            // Check if this recipe is already favorited
            var data = new Data();
            isFavorited = data.IsRecipeFavorited(userId, recipe.RecipeID);
            UpdateFavoriteButton();

            TitleBlock.Text = _recipe.Title ?? "Untitled";
            SubtitleBlock.Text = _recipe.Description ?? "No description";

            PrepTimeBlock.Text = $"{_recipe.PrepTime} min";
            CookTimeBlock.Text = $"{_recipe.CookTime} min";
            ServingsBlock.Text = _recipe.Servings.ToString();
            CaloriesBlock.Text = $"{_recipe.Calories} kcal";

            IngredientsList.ItemsSource = _recipe.Ingredients;

            InstructionsList.Items.Clear();
            for (int i = 0; i < _recipe.Instructions.Count; i++)
            {
                InstructionsList.Items.Add(new TextBlock
                {
                    Text = $"{i + 1}. {_recipe.Instructions[i]}",
                    FontSize = 13,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 6)
                });
            }
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            isFavorited = !isFavorited;
            UpdateFavoriteButton();

            var data = new Data();
            int recipeId = _recipe.RecipeID;

            if (isFavorited)
            {
                data.ToggleFavorite(_userid, recipeId);
            }
            else
            {
                data.RemoveFavorite(_userid, recipeId);
                _onUnfavorited?.Invoke();
                if (_closeOnUnfavorite)
                    this.Close();
            }
        }

        private void UpdateFavoriteButton()
        {
            if (FavoriteButton != null)
            {
                FavoriteButton.Content = isFavorited ? "★" : "☆";
                FavoriteButton.Foreground = isFavorited ? new SolidColorBrush(Colors.Gold) : new SolidColorBrush(Colors.Black);
            }
        }

        private void AddToMealPlan_Click(object sender, RoutedEventArgs e)
        {
            _onSelect?.Invoke(_recipe);
            if (_closeOnSelect)
                this.Close();
        }




    }
}
