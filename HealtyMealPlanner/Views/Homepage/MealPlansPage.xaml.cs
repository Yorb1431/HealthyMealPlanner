using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HealthyMealPlanner.Models;

namespace HealthyMealPlanner.Views
{
    public partial class MealPlansPage : UserControl
    {
        private readonly Data data = new Data();
        private readonly string _username;
        private readonly MainWindow _mainWindow;
        private readonly int _userId;

        public MealPlansPage(MainWindow mainWindow, string username)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _username = username;
            _userId = data.GetUserIdByUsername(username);
            LoadMealPlans();
        }

        private void LoadMealPlans()
        {
            var plans = data.GetMealPlansByUser(_userId);
            MealPlansList.ItemsSource = plans;
        }

        private void ViewPlan_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int planId)
            {
                var data = new Data();
                var mealPlan = data.GetMealPlanById(planId); 
                int userId = data.GetUserIdByUsername(_username);

                var viewer = new MealPlannerView(mealPlan, userId, false); // hide save
                viewer.ShowDialog();
            }
        }


        private void EditPlan_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int planId)
            {
                var data = new Data();
                var mealPlan = data.GetMealPlanById(planId); 
                int userId = data.GetMealPlanOwner(planId);
                var editor = new EditMealPlanView(mealPlan, userId, planId, _mainWindow);
                editor.ShowDialog();
            }
        }


        private void DeletePlan_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int planId)
            {
                var confirm = MessageBox.Show("Are you sure you want to delete this meal plan?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    if (data.DeleteMealPlan(planId))
                        LoadMealPlans();
                    else
                        MessageBox.Show("Failed to delete the meal plan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.LoadHomepage(null, null);
        }

        private void CreateNewPlan_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SelectPlanTypeWindow
            {
                Owner = Window.GetWindow(this)
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string selectedType = dialog.SelectedType;

                Dictionary<string, List<MealEntry>> emptyPlan = new();

                if (selectedType == "Week")
                {
                    foreach (string day in new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" })
                    {
                        emptyPlan[day] = new List<MealEntry>
                {
                    new() { MealType = "Breakfast" },
                    new() { MealType = "Lunch" },
                    new() { MealType = "Dinner" }
                };
                    }
                }
                else
                {
                    emptyPlan["Today"] = new List<MealEntry>
            {
                new() { MealType = "Breakfast" },
                new() { MealType = "Lunch" },
                new() { MealType = "Dinner" }
            };
                }

                var createWindow = new CreateMealPlanView(emptyPlan, _userId, _mainWindow);
                bool? created = createWindow.ShowDialog();

                if (created == true)
                {
                    LoadMealPlans();
                }


                LoadMealPlans();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMealPlans();
        }
    }
}
