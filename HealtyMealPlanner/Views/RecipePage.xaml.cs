using System.Windows;
using System.Windows.Controls;

namespace HealthyMealPlanner.Views
{
    public partial class RecipePage : Window
    {
        public RecipePage()
        {
            InitializeComponent();
        }

        private void ShowShoppingList_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("🛒 Shopping List:\n- Chicken breast\n- Tortilla wraps\n- Eggs\n- Kousenband\n(etc...)", "Shopping List");
        }

        private void DownloadShoppingList_Click(object sender, RoutedEventArgs e)
        {
            var shoppingList = "Shopping List:\n- Chicken breast\n- Tortilla wraps\n- Eggs\n- Kousenband\n(etc...)";
            var filePath = "shopping_list.txt";
            System.IO.File.WriteAllText(filePath, shoppingList);
            MessageBox.Show("Shopping list saved to " + filePath, "Download Complete");
        }
    }
}
