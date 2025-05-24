using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using HealthyMealPlanner.Models;

namespace HealthyMealPlanner.Views
{
   public partial class Homepage : UserControl
    {
        private readonly Data data = new Data();
        private string _username;
        private List<string> _allergies;
        private enum PlanMode { Weekly, Quick }
        private PlanMode _selectedMode = PlanMode.Weekly;


        public Homepage(string username, string role)
        {
            InitializeComponent();
            _username = username;
            UsernameTextBlock.Text = _username;
            _allergies = data.GetFullUserProfile(_username).Allergies;
        }

        private List<string> GetSelectedIngredients()
        {
            return IngredientList.Children.OfType<CheckBox>()
                .Where(cb => cb.IsChecked == true)
                .Select(cb => cb.Content.ToString())
                .ToList();
        }

        private void BrowseIngredients_Click(object sender, RoutedEventArgs e)
        {
            var currentIngredients = IngredientList.Children
                .OfType<CheckBox>()
                .Where(cb => cb.IsChecked == true)
                .Select(cb => cb.Content.ToString())
                .ToList();

            var browseWindow = new BrowseIngredientsWindow(currentIngredients);
            if (browseWindow.ShowDialog() == true)
            {
                IngredientList.Children.Clear();

                foreach (var ingredient in browseWindow.SelectedIngredients)
                {
                    var item = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5, 2, 5, 2) };

                    item.Children.Add(new TextBlock
                    {
                        Text = ingredient,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 6, 0)
                    });

                    var removeBtn = new Button
                    {
                        Content = "✕",
                        Tag = item,
                        Width = 20,
                        Height = 20,
                        FontSize = 12,
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Foreground = Brushes.Red,
                        Cursor = System.Windows.Input.Cursors.Hand,
                        Padding = new Thickness(0),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    removeBtn.Click += (s, e) =>
                    {
                        IngredientList.Children.Remove(item);
                        UpdateIngredientUIState();
                    };

                    item.Children.Add(removeBtn);
                    IngredientList.Children.Add(item);
                }

                UpdateIngredientUIState();
            }
        }


        private void DisplaySelectedIngredients(List<string> ingredients)
        {
            IngredientList.Children.Clear();

            foreach (var ingredient in ingredients)
            {
                var tag = new Border
                {
                    Background = Brushes.LightGreen,
                    CornerRadius = new CornerRadius(15),
                    Padding = new Thickness(8, 4, 8, 4),
                    Margin = new Thickness(5),
                    Tag = ingredient
                };

                var stack = new StackPanel { Orientation = Orientation.Horizontal };

                stack.Children.Add(new TextBlock
                {
                    Text = ingredient,
                    Foreground = Brushes.Black,
                    Margin = new Thickness(0, 0, 6, 0)
                });

                var removeBtn = new Button
                {
                    Content = "❌",
                    Padding = new Thickness(0),
                    Margin = new Thickness(0),
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.DarkRed,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = ingredient,
                    Width = 18,
                    Height = 18
                };
                removeBtn.Click += RemoveIngredient_Click;

                stack.Children.Add(removeBtn);
                tag.Child = stack;
                IngredientList.Children.Add(tag);
            }

            BrowseButton.Content = ingredients.Any() ? "Change Ingredients" : "Browse Ingredients";
        }


        private void RemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string ingredientToRemove)
            {
                var element = IngredientList.Children
                    .OfType<Border>()
                    .FirstOrDefault(b => (string)b.Tag == ingredientToRemove);

                if (element != null)
                {
                    IngredientList.Children.Remove(element);
                }

                BrowseButton.Content = IngredientList.Children.Count > 0 ? "Change Ingredients" : "Browse Ingredients";
            }
        }

        private void RemoveAllIngredients_Click(object sender, RoutedEventArgs e)
        {
            IngredientList.Children.Clear();
            UpdateIngredientUIState();
        }

        private void UpdateIngredientUIState()
        {
            bool hasIngredients = IngredientList.Children.Count > 0;
            BrowseButton.Content = hasIngredients ? "Change Ingredients" : "Browse Ingredients";
            RemoveAllButton.Visibility = hasIngredients ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GeneratePlan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIngredients = GetSelectedIngredients();
                var allRecipes = data.GetAllRecipesWithIngredients();

                if (selectedIngredients.Count > 0)
                {
                    allRecipes = allRecipes
                        .Where(r => r.Ingredients.Any(i => selectedIngredients.Contains(i, StringComparer.OrdinalIgnoreCase)))
                        .ToList();
                }

                var userProfile = data.GetFullUserProfile(_username);
                var dietType = userProfile.DietType;
                var dietGoal = userProfile.DietGoal;
                var userId = data.GetUserIdByUsername(_username);

                // Determine calorie bounds based on goal
                int minCalories = 0;
                int maxCalories = int.MaxValue;

                if (dietGoal.Equals("Lose Weight", StringComparison.OrdinalIgnoreCase))
                {
                    maxCalories = 400;
                }
                else if (dietGoal.Equals("Gain Weight", StringComparison.OrdinalIgnoreCase))
                {
                    minCalories = 700;
                }

                var safeRecipes = allRecipes
                    .Where(r =>
                        (dietType == "Omnivore" || r.Category == dietType) &&
                        !_allergies.Any(allergy => r.Ingredients.Contains(allergy, StringComparer.OrdinalIgnoreCase)) &&
                        r.Calories >= minCalories &&
                        r.Calories <= maxCalories)
                    .ToList();

                var fallbackRecipes = allRecipes
                    .Where(r => !_allergies.Any(allergy =>
                        r.Ingredients.Contains(allergy, StringComparer.OrdinalIgnoreCase)))
                    .ToList();

                if (safeRecipes.Count == 0 && fallbackRecipes.Count == 0)
                {
                    MessageBox.Show("No safe recipes available. Please adjust filters or ingredients.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var random = new Random();
                var weeklyPlan = new Dictionary<string, List<MealEntry>>();
                string[] days = _selectedMode == PlanMode.Quick ? new[] { "Today" } :
                    new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

                string[] meals = { "Breakfast", "Lunch", "Dinner" };

                var shuffledSafe = safeRecipes.OrderBy(_ => random.Next()).ToList();
                var shuffledFallback = fallbackRecipes.OrderBy(_ => random.Next()).ToList();

                int safeIndex = 0;

                foreach (string day in days)
                {
                    var dailyMeals = new List<MealEntry>();

                    foreach (string meal in meals)
                    {
                        Recipe chosen;
                        if (safeIndex < shuffledSafe.Count)
                        {
                            chosen = shuffledSafe[safeIndex++];
                        }
                        else
                        {
                            chosen = shuffledFallback[random.Next(shuffledFallback.Count)];
                        }

                        chosen.Description ??= "No description provided.";
                        dailyMeals.Add(new MealEntry { MealType = meal, Recipe = chosen });
                    }

                    weeklyPlan[day] = dailyMeals;
                }

                var plannerWindow = new MealPlannerView(weeklyPlan, userId, true);
                plannerWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating meal plan: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }






        private void MealItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Recipe recipe)
            {
                ShowRecipePopup(recipe);
            }
        }

        private void ShowRecipePopup(Recipe recipe)
        {
            var popup = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20),
                Effect = new DropShadowEffect { BlurRadius = 10, ShadowDepth = 3, Opacity = 0.3 },
                Width = 400,
                Height = 350,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Child = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { Text = recipe.Title, FontSize = 20, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) },
                        new TextBlock { Text = "Ingredients:", FontWeight = FontWeights.SemiBold },
                        new TextBlock { Text = string.Join(", ", recipe.Ingredients ?? new List<string>()), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 10) },
                        new TextBlock { Text = $"Prep Time: {recipe.PrepTime} min\nCook Time: {recipe.CookTime} min\nServings: {recipe.Servings}\nCalories: {recipe.Calories} kcal", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 10, 0, 10) },
                        new TextBlock { Text = "Instructions:", FontWeight = FontWeights.SemiBold },
                        new TextBlock { Text = recipe.Description, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 10) },
                        new Button
                        {
                            Content = "Close",
                            Background = Brushes.LightGray,
                            Margin = new Thickness(0, 10, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Padding = new Thickness(10, 4, 10, 4),
                        }
                    }
                }
            };

            if (popup.Child is StackPanel sp && sp.Children.Count > 0 && sp.Children[sp.Children.Count - 1] is Button closeBtn)
            {
                closeBtn.Click += (s, e) => MainGrid.Children.Remove(popup);
            }

            MainGrid.Children.Add(popup);
        }

        private void WeeklyPlanButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedMode = PlanMode.Weekly;
            HighlightSelectedMode();
        }

        private void QuickPlanButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedMode = PlanMode.Quick;
            HighlightSelectedMode();
         
        }

        private void HighlightSelectedMode()
        {
            if (_selectedMode == PlanMode.Weekly)
            {
                WeeklyPlanButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A34A"));
                WeeklyPlanButton.Foreground = Brushes.White;

                QuickPlanButton.Background = Brushes.White;
                QuickPlanButton.Foreground = Brushes.Black;
            }
            else
            {
                QuickPlanButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A34A"));
                QuickPlanButton.Foreground = Brushes.White;

                WeeklyPlanButton.Background = Brushes.White;
                WeeklyPlanButton.Foreground = Brushes.Black;
            }
        }


    }



}