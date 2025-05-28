using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HealthyMealPlanner.Models;

namespace HealthyMealPlanner.Views
{
    public partial class MealPlannerView : Window
    {
        private readonly int _userid;
        public Dictionary<string, List<MealEntry>> MealPlan { get; }

        public MealPlannerView(Dictionary<string, List<MealEntry>> mealPlan, int userId, bool showSaveButton = true)
        {
            InitializeComponent();
            _userid = userId;
            MealPlan = mealPlan;

            DataContext = this;
            SaveButton.Visibility = showSaveButton ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Meal_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is MealEntry entry)
            {
                var recipeView = new RecipeView(entry.Recipe, _userid);
                recipeView.ShowDialog();
            }
        }


        private void SaveMealPlan_Click(object sender, RoutedEventArgs e)
        {
            var saveWindow = new SaveMealPlanWindow
            {
                Owner = this
            };

            bool? result = saveWindow.ShowDialog();

            if (result == true)
            {
                string name = saveWindow.MealPlanName;
                var data = new Data();
                bool success = data.SaveMealPlan(name, _userid, MealPlan);

                if (success)
                {
                    MessageBox.Show("Meal plan saved successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    SaveButton.IsEnabled = false; //disable save button
                    SaveButton.Content = "✔ Saved";
                }
                else
                {
                    MessageBox.Show("Something went wrong while saving.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }




    }
}
