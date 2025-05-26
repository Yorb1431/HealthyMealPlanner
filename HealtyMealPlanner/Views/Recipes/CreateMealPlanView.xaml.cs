using HealthyMealPlanner.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HealthyMealPlanner.Views
{
    public partial class CreateMealPlanView : Window
    {
        private readonly Dictionary<string, List<MealEntry>> _mealPlan;
        private readonly int _userId;
        private readonly MainWindow _mainWindow;

        public CreateMealPlanView(Dictionary<string, List<MealEntry>> mealPlan, int userId, MainWindow mainWindow)
        {
            InitializeComponent();
            _mealPlan = mealPlan;
            _userId = userId;
            _mainWindow = mainWindow;

            RenderEditor();
        }
        //tekent de volledige mealplan in de UI
        private void RenderEditor()
        {
            MealPlanEditor.Items.Clear();
            string[] mealOrder = { "Breakfast", "Lunch", "Dinner" };

            foreach (var day in _mealPlan)
            {
                var dayPanel = new StackPanel { Margin = new Thickness(0, 16, 0, 32) };
                //naam van de dag tonen
                dayPanel.Children.Add(new TextBlock
                {
                    Text = day.Key,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 8)
                });

                var wrap = new WrapPanel();

                foreach (string meal in mealOrder)
                {
                    // Zoek de juiste maaltijdtype 
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
                    //Maaltijdtype tonen
                    content.Children.Add(new TextBlock
                    {
                        Text = meal,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = Brushes.Black,
                        Margin = new Thickness(0, 0, 0, 4)
                    });

                    if (entry?.Recipe != null)
                    {
                        // Als er al een recept gekozen is -> toon de titel + verwijderknop
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
                        // Als er nog geen recept is -> toon een "+"
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
        }

        //verwijdert het recept uit de mealplan
        private void RemoveMeal_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<string, string> tag)
            {
                string day = tag.Item1;
                string meal = tag.Item2;

                //verwijder het recept uit de juiste maaltijd van de juiste dag
                var entry = _mealPlan[day].FirstOrDefault(m => m.MealType == meal);
                if (entry != null)
                    entry.Recipe = null;

                RenderEditor();
            }
        }

        //voeg een maaltijd toe aan de mealplan --> opent de recipepage om gerechten te selecteren
        private void AddMeal_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<string, string> tag)
            {
                string day = tag.Item1;
                string mealType = tag.Item2;

                string dietType = new Data().GetDietTypeByUserId(_userId);
                var browserWindow = new Window();

                var browser = new RecipePage(_mainWindow, _userId, dietType)
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
                            RenderEditor();
                        }

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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //open popup om plan een naam te geven
            var namePrompt = new SaveMealPlanWindow
            {
                Owner = this
            };

            bool? result = namePrompt.ShowDialog();
            if (result == true)
            {
                string name = namePrompt.MealPlanName;
                //probeer mealplan op te slaan met SaveMealPlan
                var success = new Data().SaveMealPlan(name, _userId, _mealPlan);

                if (success)
                {
                    MessageBox.Show("Meal plan saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to save meal plan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //sluit venster
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
