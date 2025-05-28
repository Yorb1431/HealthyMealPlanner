using System.Windows;
using HealthyMealPlanner.Models;

namespace HealthyMealPlanner.Views
{
    public partial class UserDetailsWindow : Window
    {
        public UserDetailsWindow(int userId)
        {
            InitializeComponent();

            var data = new Data();
            string username = data.GetUsernameByUserId(userId);
            var profile = data.GetFullUserProfile(username);

            FullNameText.Text = profile.FullName;
            EmailText.Text = profile.Email;
            AgeText.Text = profile.Age.ToString();
            GenderText.Text = profile.Gender;
            ActivityText.Text = profile.ActivityLevel;
            DietTypeText.Text = profile.DietType;
            GoalText.Text = profile.DietGoal;
            AllergiesText.Text = string.Join(", ", profile.Allergies);
            UnitText.Text = profile.IsMetric ? "Metric" : "Imperial";
            VerifiedText.Text = profile.IsVerified ? "Yes" : "No";
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
