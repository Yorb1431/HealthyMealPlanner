using System.Windows;

namespace HealthyMealPlanner.Views
{
    public partial class SaveMealPlanWindow : Window
    {
        public string MealPlanName { get; private set; }

        public SaveMealPlanWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PlanNameBox.Text))
            {
                MealPlanName = PlanNameBox.Text.Trim();
                DialogResult = true; 
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid name for your meal plan.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
