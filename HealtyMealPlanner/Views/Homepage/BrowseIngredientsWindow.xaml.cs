using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HealthyMealPlanner.Views
{
    public partial class BrowseIngredientsWindow : Window
    {
        public List<string> SelectedIngredients { get; private set; } = new();
        private List<string> allIngredients;

        public BrowseIngredientsWindow(List<string> preselectedIngredients)
        {
            InitializeComponent();
            var data = new Data();
            allIngredients = data.GetAllIngredients();

            foreach (var ingredient in allIngredients)
            {
                var checkbox = new CheckBox
                {
                    Content = ingredient,
                    Margin = new Thickness(5),
                    IsChecked = preselectedIngredients.Contains(ingredient)
                };
                IngredientWrapPanel.Children.Add(checkbox);
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedIngredients = IngredientWrapPanel.Children
                .OfType<CheckBox>()
                .Where(cb => cb.IsChecked == true)
                .Select(cb => cb.Content.ToString())
                .ToList();

            DialogResult = true;
        }
    }
}