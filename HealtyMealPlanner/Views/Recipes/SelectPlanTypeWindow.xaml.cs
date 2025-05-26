using System.Windows;

namespace HealthyMealPlanner.Views
{
    public partial class SelectPlanTypeWindow : Window
    {
        public string SelectedType { get; private set; }

        public SelectPlanTypeWindow()
        {
            InitializeComponent(); 
        }

        //kies voor een weekplan 
        private void Weekly_Click(object sender, RoutedEventArgs e)
        {
            SelectedType = "Week";
            DialogResult = true;
            Close();
        }
        //kies voor een dagplan
        private void Day_Click(object sender, RoutedEventArgs e)
        {
            SelectedType = "Day";
            DialogResult = true;
            Close();
        }
    }
}
