using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HealthyMealPlanner
{
    public partial class HomePage : Window
    {
        private readonly List<string> _defaultIngredients = new() { "Tomatoes", "Chicken", "Rice", "Avocado" };
        private readonly List<(string Name, List<string> Ingredients)> _mealDatabase = new()
        {
            ("Grilled Chicken Salad", new List<string>{ "Chicken", "Lettuce", "Tomatoes" }),
            ("Avocado Toast", new List<string>{ "Bread", "Avocado" }),
            ("Tomato Rice Bowl", new List<string>{ "Rice", "Tomatoes", "Corn" }),
            ("Veggie Stir Fry", new List<string>{ "Broccoli", "Carrots", "Soy Sauce", "Rice" }),
            ("Chicken Curry", new List<string>{ "Chicken", "Curry Paste", "Coconut Milk", "Rice" }),
            ("Quinoa Bowl", new List<string>{ "Quinoa", "Avocado", "Black Beans" })
        };

        public HomePage()
        {
            InitializeComponent();
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            foreach (var ingredient in _defaultIngredients)
            {
                AddIngredientTag(ingredient);
            }

            var quotes = new[]
            {
                "Healthy eating starts with a single ingredient. 🥗",
                "Fuel your day with the right meal. 💪",
                "A good plan today means a good meal tonight!",
                "Stay strong. Eat clean. Live long. 🍎"
            };
            var rnd = new Random();
            dailyQuote.Text = quotes[rnd.Next(quotes.Length)];
        }

        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            var ingredient = IngredientInput.Text.Trim();
            if (!string.IsNullOrEmpty(ingredient))
            {
                AddIngredientTag(ingredient);
                IngredientInput.Clear();
            }
        }

        private void AddIngredientTag(string name)
        {
            var tag = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(15),
                Margin = new Thickness(4),
                Padding = new Thickness(10, 5, 10, 5)
            };

            var panel = new StackPanel { Orientation = Orientation.Horizontal };
            panel.Children.Add(new TextBlock { Text = $"+ {name}", Margin = new Thickness(0, 0, 8, 0) });
            var close = new Button
            {
                Content = "×",
                Background = null,
                BorderThickness = new Thickness(0),
                Foreground = System.Windows.Media.Brushes.Red,
                Padding = new Thickness(0),
                FontWeight = FontWeights.Bold
            };
            close.Click += (s, e) => IngredientList.Children.Remove(tag);
            panel.Children.Add(close);

            tag.Child = panel;
            IngredientList.Children.Add(tag);
        }

        private List<string> GetCurrentIngredients()
        {
            return IngredientList.Children.OfType<Border>()
                .Select(b => ((b.Child as StackPanel)?.Children[0] as TextBlock)?.Text.Replace("+ ", ""))
                .Where(i => !string.IsNullOrEmpty(i))
                .ToList();
        }

        private void WeeklyPlanButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You selected the Weekly Plan");
        }

        private void QuickPlanButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You selected the Quick Plan");
        }

        private void SurprisePlan_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            var meal = _mealDatabase[random.Next(_mealDatabase.Count)].Name;
            HistoryList.Items.Insert(0, $"🎲 Surprise Meal: {meal}");
            MessageBox.Show($"Your surprise meal is: {meal}");
        }

        private void GeneratePlan_Click(object sender, RoutedEventArgs e)
        {
            SuggestedMeals.Items.Clear();
            var selectedIngredients = GetCurrentIngredients();
            var suggestions = _mealDatabase.Where(meal =>
                meal.Ingredients.Any(i => selectedIngredients.Contains(i)))
                .Select(m => $"📦 {m.Name} ({string.Join(", ", m.Ingredients)})");

            foreach (var meal in suggestions)
            {
                SuggestedMeals.Items.Add(meal);
            }

            HistoryList.Items.Insert(0, $"✅ Plan for {ServingsBox.Text} with {selectedIngredients.Count} ingredients");
        }
    }
}
