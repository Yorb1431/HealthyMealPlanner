using System.Collections.Generic;

namespace HealthyMealPlanner.Models
{
    public class Recipe
    {
        public int RecipeID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Servings { get; set; }
        public int Calories { get; set; }
        public string ImagePath { get; set; }
        public List<string> Ingredients { get; set; } = new();
        public List<string> Instructions { get; set; } = new();
        public string Category { get; set; }

        public string MetaTime => $"🕒 {PrepTime} + {CookTime} min";
        public string MetaServings => $"👥 {Servings} servings";
        public string MetaCalories => $"🔥 {Calories} kcal";

    }
}
