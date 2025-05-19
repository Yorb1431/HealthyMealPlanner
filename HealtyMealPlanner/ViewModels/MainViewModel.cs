using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HealthyMealPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _userName;
        private DateTime _selectedDate;
        private ObservableCollection<MealPlanItem> _todaysMeals;
        private ObservableCollection<MealPlanItem> _weeklyMeals;
        private bool _isLoading;

        public MainViewModel()
        {
            // Initialize with current date
            SelectedDate = DateTime.Today;
            
            // Initialize collections
            TodaysMeals = new ObservableCollection<MealPlanItem>();
            WeeklyMeals = new ObservableCollection<MealPlanItem>();

            // Initialize commands
            GenerateMealPlanCommand = new RelayCommand(GenerateMealPlan);
            SaveMealPlanCommand = new RelayCommand(SaveMealPlan);
            ExportMealPlanCommand = new RelayCommand(ExportMealPlan);
            LogoutCommand = new RelayCommand(Logout);
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                LoadMealPlanForDate(value);
            }
        }

        public ObservableCollection<MealPlanItem> TodaysMeals
        {
            get => _todaysMeals;
            set
            {
                _todaysMeals = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MealPlanItem> WeeklyMeals
        {
            get => _weeklyMeals;
            set
            {
                _weeklyMeals = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand GenerateMealPlanCommand { get; }
        public ICommand SaveMealPlanCommand { get; }
        public ICommand ExportMealPlanCommand { get; }
        public ICommand LogoutCommand { get; }

        private async void GenerateMealPlan()
        {
            IsLoading = true;
            try
            {
                // TODO: Implement meal plan generation logic
                // This will be implemented when we add the meal planning service
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void SaveMealPlan()
        {
            IsLoading = true;
            try
            {
                // TODO: Implement meal plan saving logic
                // This will be implemented when we add the data persistence service
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void ExportMealPlan()
        {
            IsLoading = true;
            try
            {
                // TODO: Implement meal plan export logic
                // This will be implemented when we add the export service
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Logout()
        {
            // TODO: Implement logout logic
            // This will be implemented when we add the authentication service
        }

        private async void LoadMealPlanForDate(DateTime date)
        {
            IsLoading = true;
            try
            {
                // TODO: Implement meal plan loading logic
                // This will be implemented when we add the data persistence service
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MealPlanItem
    {
        public string MealName { get; set; }
        public string Description { get; set; }
        public string Calories { get; set; }
        public string Protein { get; set; }
        public string Carbs { get; set; }
        public string Fat { get; set; }
        public string ImageUrl { get; set; }
        public DateTime MealTime { get; set; }
        public string MealType { get; set; } // Breakfast, Lunch, Dinner, Snack
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
} 