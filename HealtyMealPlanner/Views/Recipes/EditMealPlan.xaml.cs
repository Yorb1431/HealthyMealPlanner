using HealthyMealPlanner.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HealthyMealPlanner.Views
{
    public partial class EditMealPlanView : Window
    {
        //huidige mealplan dat we bewerken
        private Dictionary<string, List<MealEntry>> _mealPlan;
        private readonly int _userId;
        private readonly MainWindow _mainWindow;
        private readonly int _planId;

        public EditMealPlanView(Dictionary<string, List<MealEntry>> mealPlan, int userId, int planId, MainWindow mainWindow)
        {
            InitializeComponent();
            _mealPlan = mealPlan;
            _userId = userId;
            _planId = planId;
            _mainWindow = mainWindow;

            RenderEditor();
        }

        private bool IsMealPlanFull()
        {
            string[] requiredMeals = { "Breakfast", "Lunch", "Dinner" };

            foreach (var day in _mealPlan)
            {
                var mealTypes = day.Value
                    .Where(e => e.Recipe != null)
                    .Select(e => e.MealType)
                    .ToList();

                foreach (var required in requiredMeals)
                {
                    if (!mealTypes.Contains(required))
                        return false;
                }
            }

            return true;
        }

        //bouwt de visuele strucutuur op van onze meaplan
        private void RenderEditor()
        {
            MealPlanEditor.Items.Clear();
            string[] mealOrder = { "Breakfast", "Lunch", "Dinner" };

            // Voor elke dag in het plan
            foreach (var day in _mealPlan)
            {
                var dayPanel = new StackPanel { Margin = new Thickness(0, 16, 0, 32) };
                dayPanel.Children.Add(new TextBlock
                {
                    Text = day.Key,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 8)
                });

                var wrap = new WrapPanel();

                // Voor elke maaltijd
                foreach (string meal in mealOrder)
                {
                    var entry = day.Value.FirstOrDefault(e => e.MealType == meal);
                    var container = new StackPanel { Width = 180, Margin = new Thickness(8) };
                    var border = new Border
                    {
                        Background = Brushes.White,
                        BorderBrush = Brushes.LightGray,
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(12),
                        Padding = new Thickness(10)
                    };

                    var content = new StackPanel();

                    // Toon maaltijdtype
                    content.Children.Add(new TextBlock
                    {
                        Text = meal,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = Brushes.Black,
                        Margin = new Thickness(0, 0, 0, 4)
                    });

                    if (entry?.Recipe != null)
                    {
                        //toon de titel en een verwijderknop
                        content.Children.Add(new TextBlock
                        {
                            Text = entry.Recipe.Title,
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = Brushes.Gray,
                            FontSize = 12
                        });

                        var removeButton = new Button
                        {
                            Content = "❌",
                            Width = 24,
                            Height = 24,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Background = Brushes.Transparent,
                            BorderThickness = new Thickness(0),
                            Cursor = Cursors.Hand,
                            Tag = (day.Key, meal)
                        };
                        removeButton.Click += RemoveMeal_Click;

                        content.Children.Add(removeButton);
                    }
                    else
                    {
                        //als er nog geen recept is -> toon een +
                        var addButton = new Button
                        {
                            Content = "+",
                            FontSize = 20,
                            Width = 40,
                            Height = 40,
                            Background = Brushes.Transparent,
                            BorderBrush = Brushes.Gray,
                            BorderThickness = new Thickness(1),
                            Cursor = Cursors.Hand,
                            Tag = (day.Key, meal)
                        };
                        addButton.Click += AddMeal_Click;

                        content.Children.Add(addButton);
                    }

                    border.Child = content;
                    container.Children.Add(border);
                    wrap.Children.Add(container);
                }

                dayPanel.Children.Add(wrap);
                MealPlanEditor.Items.Add(dayPanel);
            }
            SaveButton.IsEnabled = IsMealPlanFull();
        }


        //wordt uitgevoerd wanneer er op de "x" wordt gedrukt
        private void RemoveMeal_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<string, string> tag)
            {
                string day = tag.Item1;
                string meal = tag.Item2;

                //zoek de juiste maaltijd en verwijder het recept
                var entry = _mealPlan[day].FirstOrDefault(m => m.MealType == meal);
                if (entry != null)
                    entry.Recipe = null;

                RenderEditor();
            }
        }

        //wordt uitgevoerd wanneer de gebruiker op de + drukt 
        private void AddMeal_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<string, string> tag)
            {
                string day = tag.Item1;
                string mealType = tag.Item2;

                string dietType = new Data().GetDietTypeByUserId(_userId);
                //popupvenster voor receptselectie
                var recipePage = new RecipePage(_mainWindow, _userId, dietType)
                {
                    IsSelectionMode = true,
                    OnRecipeSelected = (selectedRecipe) =>
                    {
                        //wanneer een recept geselecteerd wordt, voeg het toe aan de juiste maaltijd
                        var mealEntries = _mealPlan[day];
                        var existingEntry = mealEntries.FirstOrDefault(m => m.MealType == mealType);

                        if (existingEntry != null)
                        {
                            existingEntry.Recipe = selectedRecipe;
                        }
                        else
                        {
                            mealEntries.Add(new MealEntry
                            {
                                MealType = mealType,
                                Recipe = selectedRecipe
                            });
                        }

                        RenderEditor();
                    }
                };

                var browserWindow = new Window();

                //open recipebrowser
                var browser = new RecipePage(_mainWindow, _userId, dietType: "Omnivore")
                {
                    IsSelectionMode = true,
                    OnRecipeSelected = (selectedRecipe) =>
                    {
                        var meals = _mealPlan[day];
                        var index = meals.FindIndex(m => m.MealType == mealType);
                        if (index >= 0)
                        {
                            meals[index] = new MealEntry
                            {
                                MealType = mealType,
                                Recipe = selectedRecipe
                            };
                            RenderEditor(); //herteken na toevoegen van recept
                        }

                        //sluit de recipebrowser
                        browserWindow.Close();
                    }
                };

                browserWindow.Title = "Choose Recipe";
                browserWindow.Width = 800;
                browserWindow.Height = 600;
                browserWindow.Content = browser;
                browserWindow.Owner = this;
                browserWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                browserWindow.ShowDialog();

            }
        }


        //sluit venster
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //slaat wijzingen op van het plan in de databse
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var data = new Data();
            bool success = data.UpdateMealPlan(_userId, _planId, _mealPlan);

            if (success)
            {
                MessageBox.Show("Meal plan updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Something went wrong while updating the meal plan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
