using MySqlConnector;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Globalization;
using HealthyMealPlanner.Models;
using System.Net.Mail;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Data;
namespace HealthyMealPlanner
{
    public class Data
    {
        private string connectionString = "server = 127.0.0.1;" +
            "port = 3307; " +
            "username = root; password =;" +
            "database = healthymealplanner;";

        // Hash password using SHA256
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Insert helper for non-query commands
        private int Insert(string query, params MySqlParameter[] parameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)command.LastInsertedId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return -1;
            }
        }

        // Check if user exists by email or username
        public bool UserExists(string email, string username)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @email OR Username = @username";
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@username", username);

                try
                {
                    connection.Open();
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }


        public string GetUserRole(string username)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand("SELECT Role FROM Users WHERE Username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    var result = command.ExecuteScalar();
                    return result != null ? result.ToString() : "User";
                }
            }
        }

        // Check if user exists by email or username without registering
        public bool CheckUserExists(string email, string username)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @email OR Username = @username";
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@username", username);

                try
                {
                    connection.Open();
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public int GetUserId(string username)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT UserID FROM users WHERE Username = @username LIMIT 1";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);

            object result = cmd.ExecuteScalar();

            if (result != null && int.TryParse(result.ToString(), out int userId))
            {
                return userId;
            }
            return -1;
        }

        public bool CreateUserAndProfile(string username, string email, string password, string fullName, int age, string gender, string role, string dietGoal)
        {
            string passwordHash = HashPassword(password);
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert into Users
                        string userQuery = "INSERT INTO Users (Username, Email, PasswordHash, Role) VALUES (@Username, @Email, @PasswordHash, @Role)";
                        var userCommand = new MySqlCommand(userQuery, connection, transaction);
                        userCommand.Parameters.AddWithValue("@Username", username);
                        userCommand.Parameters.AddWithValue("@Email", email);
                        userCommand.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        userCommand.Parameters.AddWithValue("@Role", role);
                        userCommand.ExecuteNonQuery();
                        int userId = (int)userCommand.LastInsertedId;

                        // Insert into UserProfiles
                        string profileQuery = @"INSERT INTO UserProfiles (UserID, FullName, Age, Gender, DietGoal) 
                                        VALUES (@UserID, @FullName, @Age, @Gender, @DietGoal)";
                        var profileCommand = new MySqlCommand(profileQuery, connection, transaction);
                        profileCommand.Parameters.AddWithValue("@UserID", userId);
                        profileCommand.Parameters.AddWithValue("@FullName", fullName);
                        profileCommand.Parameters.AddWithValue("@Age", age);
                        profileCommand.Parameters.AddWithValue("@Gender", gender);
                        profileCommand.Parameters.AddWithValue("@DietGoal", dietGoal);
                        profileCommand.ExecuteNonQuery();

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating user and profile: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        // Update user profile
        public bool UpdateUserProfile(int userId, string dietGoal, string dietType, string allergies)
        {
            string query = @"UPDATE UserProfiles 
                           SET DietGoal = @dietGoal, 
                               DietType = @dietType, 
                               Allergies = @allergies 
                           WHERE UserID = @userId";
            var parameters = new[]
            {
                new MySqlParameter("@userId", userId),
                new MySqlParameter("@dietGoal", dietGoal),
                new MySqlParameter("@dietType", dietType),
                new MySqlParameter("@allergies", allergies)
            };

            return Insert(query, parameters) != -1;
        }

        // Get user ID by email
        public int GetUserIdByEmail(string email)
        {
            string query = "SELECT UserID FROM Users WHERE Email = @email";
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email", email);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }
            }
        }

        // Login user by username or email and password
        public bool LoginUser(string usernameOrEmail, string password)
        {
            string passwordHash = HashPassword(password);
            string query = @"SELECT COUNT(*) FROM Users
                             WHERE (Username = @userOrEmail OR Email = @userOrEmail)
                             AND PasswordHash = @passwordHash";
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userOrEmail", usernameOrEmail);
                command.Parameters.AddWithValue("@passwordHash", passwordHash);

                try
                {
                    connection.Open();
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public string GetUsername(string emailOrUsername)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Username FROM Users WHERE Email = @Email OR Username = @Username";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", emailOrUsername);
                        command.Parameters.AddWithValue("@Username", emailOrUsername);
                        var result = command.ExecuteScalar();
                        return result?.ToString() ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving username: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        public bool SaveDietType(int userId, string dietType)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand("UPDATE UserProfiles SET DietType = @DietType WHERE UserID = @UserID", connection))
                    {
                        command.Parameters.AddWithValue("@DietType", dietType);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving diet type: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        public bool SaveWeightGoal(int userId, string goal)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand("UPDATE UserProfiles SET DietGoal = @DietGoal WHERE UserID = @UserID", connection))
                    {
                        command.Parameters.AddWithValue("@DietGoal", goal);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving diet goal: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool SaveAllergies(int userId, List<int> allergyIds)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                //delete existing allergies
                using (var command = new MySqlCommand("DELETE FROM UserAllergies WHERE UserID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.ExecuteNonQuery();
                }

                //insert new allergies by ID
                foreach (int allergyId in allergyIds)
                {
                    using var command = new MySqlCommand("INSERT INTO UserAllergies (UserID, AllergyID) VALUES (@UserID, @AllergyID)", connection);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@AllergyID", allergyId);
                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving allergies: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        public bool SaveActivityLevel(int userId, string activityLevel)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand("UPDATE UserProfiles SET ActivityLevel = @ActivityLevel WHERE UserID = @UserID", connection))
                    {
                        command.Parameters.AddWithValue("@ActivityLevel", activityLevel);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving activity level: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool SaveMeasurements(int userId, string height, string weight)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(
                        "UPDATE UserProfiles SET Height = @Height, Weight = @Weight WHERE UserID = @UserID", connection))
                    {
                        command.Parameters.AddWithValue("@Height", float.Parse(height, CultureInfo.InvariantCulture));
                        command.Parameters.AddWithValue("@Weight", float.Parse(weight, CultureInfo.InvariantCulture));
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving measurements: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public int GetUserIdByUsername(string username)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT UserID FROM Users WHERE Username = @Username";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting user ID: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }
        public (double Height, double Weight, string DietType, List<string> Allergies, bool IsMetric) GetUserProfile(string username)
        {
            double height = 0, weight = 0;
            string dietType = "";
            List<string> allergies = new List<string>();
            bool isMetric = true;

            int userId = GetUserIdByUsername(username);
            if (userId == -1) return (height, weight, dietType, allergies, isMetric);

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Profile data
                string profileQuery = "SELECT Height, Weight, DietType, IsMetric FROM UserProfiles WHERE UserID = @UserID";
                using (var cmd = new MySqlCommand(profileQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            height = reader.IsDBNull(0) ? 0 : reader.GetDouble(0);
                            weight = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
                            dietType = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            isMetric = reader.IsDBNull(3) ? true : reader.GetBoolean(3);
                        }
                    }
                }

                // Allergy names
                string allergyQuery = @"SELECT a.Name 
                                FROM UserAllergies ua 
                                JOIN Allergies a ON ua.AllergyID = a.AllergyID 
                                WHERE ua.UserID = @UserID";
                using (var cmd = new MySqlCommand(allergyQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allergies.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return (height, weight, dietType, allergies, isMetric);
        }


        public bool SaveUnitPreference(int userId, bool isMetric)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand("UPDATE UserProfiles SET IsMetric = @IsMetric WHERE UserID = @UserID", connection))
                    {
                        command.Parameters.AddWithValue("@IsMetric", isMetric);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving unit preference: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public string GetEmailByUsername(string username)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT Email FROM Users WHERE Username = @Username", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    return cmd.ExecuteScalar()?.ToString() ?? "";
                }
            }
        }
        public class FullUserProfile
        {
            public string Email { get; set; }
            public string FullName { get; set; }
            public int Age { get; set; }
            public string Gender { get; set; }
            public double Height { get; set; }
            public double Weight { get; set; }
            public string ActivityLevel { get; set; }
            public string DietType { get; set; }
            public List<string> Allergies { get; set; }
            public string ProfileImagePath { get; set; }
            public string DietGoal { get; set; }
            public bool IsMetric { get; set; }
            public string Role { get; set; }

            public bool IsVerified { get; set; }
        }

        public FullUserProfile GetFullUserProfile(string username)
        {
            var userId = GetUserIdByUsername(username);
            var profile = new FullUserProfile { Allergies = new List<string>() };

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Email
                using (var cmd = new MySqlCommand("SELECT Email FROM Users WHERE Username = @Username", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    profile.Email = cmd.ExecuteScalar()?.ToString() ?? "";
                }

                // Role
                using (var cmd = new MySqlCommand("SELECT Role FROM Users WHERE Username = @Username", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    profile.Role = cmd.ExecuteScalar()?.ToString() ?? "User";
                }

                // Profile fields
                using (var cmd = new MySqlCommand(@"SELECT FullName, Age, Gender, Height, Weight, ActivityLevel, DietType, IsMetric, ProfileImagePath, IsVerified, DietGoal 
                                            FROM UserProfiles WHERE UserID = @UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            profile.FullName = reader.IsDBNull(0) ? "" : reader.GetString(0);
                            profile.Age = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                            profile.Gender = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            profile.Height = reader.IsDBNull(3) ? 0 : reader.GetDouble(3);
                            profile.Weight = reader.IsDBNull(4) ? 0 : reader.GetDouble(4);
                            profile.ActivityLevel = reader.IsDBNull(5) ? "" : reader.GetString(5);
                            profile.DietType = reader.IsDBNull(6) ? "" : reader.GetString(6);
                            profile.IsMetric = reader.IsDBNull(7) ? true : reader.GetBoolean(7);
                            profile.ProfileImagePath = reader.IsDBNull(8) ? null : reader.GetString(8);
                            profile.IsVerified = !reader.IsDBNull(9) && reader.GetBoolean(9);
                            profile.DietGoal = reader.IsDBNull(10) ? "" : reader.GetString(10);
                        }
                    }
                }

                // Allergies
                using (var cmd = new MySqlCommand(@"SELECT a.Name 
                                            FROM UserAllergies ua 
                                            JOIN Allergies a ON ua.AllergyID = a.AllergyID 
                                            WHERE ua.UserID = @UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            profile.Allergies.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return profile;
        }


        public bool UpdateUnitPreference(int userId, bool isMetric)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE userprofiles SET IsMetric = @isMetric WHERE UserID = @userId";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@isMetric", isMetric);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ValidatePassword(string username, string password)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string storedHash = result.ToString();
                            string enteredHash = HashPassword(password);
                            return storedHash == enteredHash;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        public bool DeleteUserAccount(int userId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        //delete from UserAllergies
                        using (var cmd1 = new MySqlCommand("DELETE FROM UserAllergies WHERE UserID = @UserID", connection, transaction))
                        {
                            cmd1.Parameters.AddWithValue("@UserID", userId);
                            cmd1.ExecuteNonQuery();
                        }

                        //delete from Favorites
                        using (var cmd0 = new MySqlCommand("DELETE FROM Favorites WHERE UserID = @UserID", connection, transaction))
                        {
                            cmd0.Parameters.AddWithValue("@UserID", userId);
                            cmd0.ExecuteNonQuery();
                        }

                        //delete from UserProfiles
                        using (var cmd2 = new MySqlCommand("DELETE FROM UserProfiles WHERE UserID = @UserID", connection, transaction))
                        {
                            cmd2.Parameters.AddWithValue("@UserID", userId);
                            cmd2.ExecuteNonQuery();
                        }

                        //delete from Users
                        using (var cmd3 = new MySqlCommand("DELETE FROM Users WHERE UserID = @UserID", connection, transaction))
                        {
                            cmd3.Parameters.AddWithValue("@UserID", userId);
                            cmd3.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user account: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool SaveProfileImagePath(int userId, string path)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE UserProfiles SET ProfileImagePath = @path WHERE UserID = @userId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@path", path);
                        command.Parameters.AddWithValue("@userId", userId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving profile image path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdateProfileImagePath(int userId, string imagePath)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("UPDATE UserProfiles SET ProfileImagePath = @Path WHERE UserID = @UserID", conn))
                    {
                        cmd.Parameters.AddWithValue("@Path", imagePath);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile image path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }



        public int GetActiveUserCount()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM Users WHERE Role = 'User'", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int GetTotalMeals()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM MealPlanItems", conn); 
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int GetMealPlanCount()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM MealPlans", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }


        public List<HealthyMealPlanner.Models.Recipe> GetRecipesMatchingIngredients(List<string> ingredients)
{
    var recipes = new List<HealthyMealPlanner.Models.Recipe>();
    if (ingredients == null || ingredients.Count == 0)
        return recipes;

    using (var connection = new MySqlConnection(connectionString))
    {
        connection.Open();

        string inClause = string.Join(",", ingredients.Select((_, i) => $"@param{i}"));
        string query = $@"
            SELECT r.RecipeID, r.Title, r.Instructions, r.PrepTime, r.CookTime, r.Servings, r.Calories,
                   i.Name AS IngredientName
            FROM Recipes r
            JOIN RecipeIngredients ri ON r.RecipeID = ri.RecipeID
            JOIN Ingredients i ON ri.IngredientID = i.IngredientID
            WHERE i.Name IN ({inClause})";

        using var command = new MySqlCommand(query, connection);
        for (int i = 0; i < ingredients.Count; i++)
        {
            command.Parameters.AddWithValue($"@param{i}", ingredients[i]);
        }

        using var reader = command.ExecuteReader();
        var recipeMap = new Dictionary<int, HealthyMealPlanner.Models.Recipe>();

        while (reader.Read())
        {
            int id = reader.GetInt32("RecipeID");
            if (!recipeMap.TryGetValue(id, out var recipe))
            {
                recipe = new HealthyMealPlanner.Models.Recipe
                {
                    Title = reader.GetString("Title"),
                    Instructions = reader["Instructions"]?.ToString()?.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                    PrepTime = reader.GetInt32("PrepTime"),
                    CookTime = reader.GetInt32("CookTime"),
                    Servings = reader.GetInt32("Servings"),
                    Calories = reader.GetInt32("Calories"),
                    Ingredients = new List<string>()
                };
                recipeMap[id] = recipe;
            }

            if (!reader.IsDBNull(reader.GetOrdinal("IngredientName")))
                recipe.Ingredients.Add(reader.GetString("IngredientName"));
        }

        recipes = recipeMap.Values.ToList();
    }

    return recipes;
}


        public List<string> GetAllIngredients()
        {
            var ingredients = new List<string>();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT DISTINCT Name FROM Ingredients", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ingredients.Add(reader.GetString(0));
            }
            return ingredients;
        }


        public List<string> GetAllUsersWithUserRole()
        {
            var users = new List<string>();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT Username FROM Users WHERE Role = 'User'", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(reader.GetString(0));
            }
            return users;
        }


        public bool UpdateBasicProfileInfo(int userId, string fullName, int age, string gender, string activityLevel, string dietType)
        {
            try
            {
                using var conn = new MySqlConnection(connectionString);
                conn.Open();
                string query = @"UPDATE UserProfiles 
                         SET FullName = @FullName, 
                             Age = @Age, 
                             Gender = @Gender,
                             ActivityLevel = @ActivityLevel,
                             DietType = @DietType
                         WHERE UserID = @UserID";

                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FullName", fullName);
                cmd.Parameters.AddWithValue("@Age", age);
                cmd.Parameters.AddWithValue("@Gender", gender);
                cmd.Parameters.AddWithValue("@ActivityLevel", activityLevel);
                cmd.Parameters.AddWithValue("@DietType", dietType);
                cmd.Parameters.AddWithValue("@UserID", userId);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public List<Recipe> GetAllRecipesWithIngredients()
        {
            var recipes = new Dictionary<int, Recipe>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT 
                                r.RecipeID, r.Title, r.Description, r.Instructions, 
                                r.PrepTime, r.CookTime, r.Servings, r.Calories, r.ImagePath,
                                r.CategoryID, c.Name AS CategoryName,
                                i.Name AS IngredientName
                            FROM recipes r
                            LEFT JOIN recipeingredients ri ON r.RecipeID = ri.RecipeID
                            LEFT JOIN ingredients i ON ri.IngredientID = i.IngredientID
                            LEFT JOIN categories c ON r.CategoryID = c.CategoryID;";

                using var cmd = new MySqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32("RecipeID");

                    if (!recipes.ContainsKey(id))
                    {
                        var instructionText = reader["Instructions"]?.ToString() ?? "";
                        recipes[id] = new Recipe
                        {
                            RecipeID = id,
                            Title = reader.GetString("Title"),
                            Description = reader["Description"]?.ToString(),
                            Instructions = instructionText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                            PrepTime = reader.GetInt32("PrepTime"),
                            CookTime = reader.GetInt32("CookTime"),
                            Servings = reader.GetInt32("Servings"),
                            Calories = reader.GetInt32("Calories"),
                            ImagePath = reader["ImagePath"]?.ToString(),
                            Category = reader["CategoryName"]?.ToString(),
                            Ingredients = new List<string>()
                        };
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("IngredientName")))
                    {
                        recipes[id].Ingredients.Add(reader.GetString("IngredientName"));
                    }
                }
            }

            return recipes.Values.ToList();
        }

        public void ToggleFavorite(int userId, int recipeId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string checkQuery = "SELECT COUNT(*) FROM favorites WHERE UserID = @userId AND RecipeID = @recipeId";
            using var checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@userId", userId);
            checkCmd.Parameters.AddWithValue("@recipeId", recipeId);
            long exists = (long)checkCmd.ExecuteScalar();

            string query = exists > 0
                ? "DELETE FROM favorites WHERE UserID = @userId AND RecipeID = @recipeId"
                : "INSERT INTO favorites (UserID, RecipeID) VALUES (@userId, @recipeId)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@recipeId", recipeId);
            cmd.ExecuteNonQuery();
        }

        public List<Recipe> GetFavoriteRecipes(int userId)
        {
            var recipes = new Dictionary<int, Recipe>();

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = @"
            SELECT r.RecipeID, r.Title, r.Description, r.Instructions, 
                   r.PrepTime, r.CookTime, r.Servings, r.Calories, r.ImagePath,
                   i.Name AS IngredientName
            FROM recipes r
            JOIN favorites f ON f.RecipeID = r.RecipeID
            LEFT JOIN recipeingredients ri ON r.RecipeID = ri.RecipeID
            LEFT JOIN ingredients i ON ri.IngredientID = i.IngredientID
            WHERE f.UserID = @userId";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32("RecipeID");
                if (!recipes.ContainsKey(id))
                {
                    var instructionText = reader["Instructions"]?.ToString() ?? "";
                    recipes[id] = new Recipe
                    {
                        RecipeID = id,
                        Title = reader.GetString("Title"),
                        Description = reader["Description"]?.ToString(),
                        Instructions = instructionText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                        PrepTime = reader.GetInt32("PrepTime"),
                        CookTime = reader.GetInt32("CookTime"),
                        Servings = reader.GetInt32("Servings"),
                        Calories = reader.GetInt32("Calories"),
                        ImagePath = reader["ImagePath"]?.ToString(),
                        Ingredients = new List<string>()
                    };
                }

                if (!reader.IsDBNull(reader.GetOrdinal("IngredientName")))
                {
                    recipes[id].Ingredients.Add(reader.GetString("IngredientName"));
                }
            }

            return recipes.Values.ToList();
        }

        public void RemoveFavorite(int userId, int recipeId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "DELETE FROM favorites WHERE UserID = @userId AND RecipeID = @recipeId";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@recipeId", recipeId);
            cmd.ExecuteNonQuery();
        }


        public bool IsRecipeFavorited(int userId, int recipeId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT COUNT(*) FROM favorites WHERE UserID = @userId AND RecipeID = @recipeId";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@recipeId", recipeId);

            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }


        public (string FromEmail, string AppPassword) GetEmailSettings()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            using var cmd = new MySqlCommand("SELECT FromEmail, AppPassword FROM EmailSettings LIMIT 1", conn);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string email = reader.GetString("FromEmail");
                string password = reader.GetString("AppPassword");
                return (email, password);
            }

            return (null, null); 
        }


        public string GetEmail(string emailOrUsername)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Email FROM Users WHERE Email = @value OR Username = @value";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@value", emailOrUsername);
                    var result = command.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
        }

        public bool UpdateUserPassword(string email, string newPassword)
        {
            try
            {
                string hash = HashPassword(newPassword);
                using var conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "UPDATE Users SET PasswordHash = @hash WHERE Email = @Email";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@hash", hash);
                cmd.Parameters.AddWithValue("@Email", email);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void SendEmailCode(string recipientEmail, string code, string purpose)
        {
            var (fromEmail, appPassword) = GetEmailSettings();

            if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(appPassword))
            {
                MessageBox.Show("Email settings not configured in the database.", "Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string subject = "", body = "";

            if (purpose == "2FA")
            {
                subject = "Your 2FA Verification Code";
                body = $"Hello,\n\nYour verification code is: {code}\n\nIf you did not request this code, please ignore this email.\n\nKind regards,\nHealthy Meal Planner";
            }
            else if (purpose == "Reset")
            {
                subject = "Reset Your Password";
                body = $"Hello,\n\nYou requested to reset your password. Use the following code to continue: {code}\n\nIf you didn't request this, please ignore this message.\n\nHealthy Meal Planner";
            }

            else if (purpose == "Verification")
            {
                subject = "Verify Your Account";
                body = $"Hello,\n\nYour verification code is: {code}\n\nThank you for verifying your account!";
            }

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true
            };

            var message = new MailMessage(fromEmail, recipientEmail)
            {
                Subject = subject,
                Body = body
            };

            smtpClient.Send(message);
        }

        public bool MarkUserAsVerified(string username)
        {
            int userId = GetUserIdByUsername(username);
            if (userId == -1)
                return false;

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "UPDATE UserProfiles SET IsVerified = 1 WHERE UserID = @UserID";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public string GetDietTypeByUserId(int userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            using var cmd = new MySqlCommand("SELECT DietType FROM UserProfiles WHERE UserID = @UserId", conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? "Omnivore";
        }


        public Dictionary<string, int> GetAllAllergies()
        {
            var map = new Dictionary<string, int>();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT AllergyID, Name FROM Allergies";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                map[reader.GetString("Name")] = reader.GetInt32("AllergyID");
            }

            return map;
        }

        public List<int> GetUserAllergyIds(int userId)
        {
            var allergyIds = new List<int>();
            string query = "SELECT AllergyID FROM UserAllergies WHERE UserID = @UserID";

            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allergyIds.Add(reader.GetInt32("AllergyID"));
                    }
                }
            }
            return allergyIds;
        }

        public Dictionary<int, List<int>> GetAllRecipeAllergyLinks()
        {
            var dict = new Dictionary<int, List<int>>();
            string query = "SELECT RecipeID, AllergyID FROM RecipeAllergies";

            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int recipeId = reader.GetInt32("RecipeID");
                        int allergyId = reader.GetInt32("AllergyID");

                        if (!dict.ContainsKey(recipeId))
                            dict[recipeId] = new List<int>();

                        dict[recipeId].Add(allergyId);
                    }
                }
            }
            return dict;
        }

        public bool SaveMealPlan(string planName, int userId, Dictionary<string, List<MealEntry>> mealPlan)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        //insert into MealPlans table
                        var insertMealPlanCmd = new MySqlCommand(
                            "INSERT INTO MealPlans (UserID, Name, CreatedDate) VALUES (@UserID, @Name, @CreatedDate)", connection, transaction);
                        insertMealPlanCmd.Parameters.AddWithValue("@UserID", userId);
                        insertMealPlanCmd.Parameters.AddWithValue("@Name", planName);
                        insertMealPlanCmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        insertMealPlanCmd.ExecuteNonQuery();

                        long mealPlanId = insertMealPlanCmd.LastInsertedId;

                        //insert into MealPlanRecipes
                        foreach (var day in mealPlan)
                        {
                            string dayOfWeek = day.Key;

                            foreach (var entry in day.Value)
                            {
                                var insertItemCmd = new MySqlCommand(
                                    "INSERT INTO MealPlanItems (MealPlanID, RecipeID, MealType, DayOfWeek) VALUES (@MealPlanID, @RecipeID, @MealType, @DayOfWeek)",
                                    connection, transaction);

                                insertItemCmd.Parameters.AddWithValue("@MealPlanID", mealPlanId);
                                insertItemCmd.Parameters.AddWithValue("@RecipeID", entry.Recipe.RecipeID);
                                insertItemCmd.Parameters.AddWithValue("@MealType", entry.MealType);
                                insertItemCmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);

                                insertItemCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error saving meal plan: " + ex.Message);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }


        public bool DeleteMealPlansForUser(int userId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        //get all MealPlanIDs for the user
                        var getPlansCmd = new MySqlCommand("SELECT MealPlanID FROM MealPlans WHERE UserID = @UserID", connection, transaction);
                        getPlansCmd.Parameters.AddWithValue("@UserID", userId);

                        var mealPlanIds = new List<int>();
                        using (var reader = getPlansCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mealPlanIds.Add(reader.GetInt32(0));
                            }
                        }

                        //delete from MealPlanItems for those plans
                        foreach (int planId in mealPlanIds)
                        {
                            var deleteItemsCmd = new MySqlCommand("DELETE FROM MealPlanItems WHERE MealPlanID = @MealPlanID", connection, transaction);
                            deleteItemsCmd.Parameters.AddWithValue("@MealPlanID", planId);
                            deleteItemsCmd.ExecuteNonQuery();
                        }

                        //delete from MealPlans
                        var deletePlansCmd = new MySqlCommand("DELETE FROM MealPlans WHERE UserID = @UserID", connection, transaction);
                        deletePlansCmd.Parameters.AddWithValue("@UserID", userId);
                        deletePlansCmd.ExecuteNonQuery();

                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting meal plans for user: " + ex.Message);
                    return false;
                }
            }
        }

        public bool SaveDietGoal(int userId, string goal)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand("UPDATE UserProfiles SET DietGoal = @DietGoal WHERE UserID = @UserID", connection))
                    {
                        command.Parameters.AddWithValue("@DietGoal", goal);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving diet goal: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public class MealPlanSummary
        {
            public int MealPlanID { get; set; }
            public string Name { get; set; }

            public string CreatedDate { get; set; }
        }

        public List<MealPlanSummary> GetMealPlansByUser(int userId)
        {
            var list = new List<MealPlanSummary>();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT MealPlanID, Name, CreatedDate FROM MealPlans WHERE UserID = @UserID";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new MealPlanSummary
                {
                    MealPlanID = reader.GetInt32("MealPlanID"),
                    Name = reader.GetString("Name"),
                    CreatedDate = reader.GetDateTime("CreatedDate").ToString("dd MMM yyyy")
                });
            }
            return list;
        }


        public bool DeleteMealPlan(int planId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var trans = conn.BeginTransaction();

            try
            {
                var delItems = new MySqlCommand("DELETE FROM MealPlanItems WHERE MealPlanID = @id", conn, trans);
                delItems.Parameters.AddWithValue("@id", planId);
                delItems.ExecuteNonQuery();

                var delPlan = new MySqlCommand("DELETE FROM MealPlans WHERE MealPlanID = @id", conn, trans);
                delPlan.Parameters.AddWithValue("@id", planId);
                delPlan.ExecuteNonQuery();

                trans.Commit();
                return true;
            }
            catch
            {
                trans.Rollback();
                return false;
            }
        }

        public string GetUsernameByUserId(int userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT Username FROM Users WHERE UserID = @UserID";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);

            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? string.Empty;
        }
        public Dictionary<string, List<MealEntry>> GetMealPlanById(int planId)
        {
            var result = new Dictionary<string, List<MealEntry>>();
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                                        SELECT DayOfWeek, MealType, RecipeID
                                        FROM MealPlanItems
                                        WHERE MealPlanID = @id
                                        ORDER BY DayOfWeek, MealType", conn); 

            cmd.Parameters.AddWithValue("@id", planId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string day = reader.GetString("DayOfWeek");
                string mealType = reader.GetString("MealType");
                int recipeId = reader.GetInt32("RecipeID");

                var recipe = GetRecipeById(recipeId);

                if (!result.ContainsKey(day))
                    result[day] = new List<MealEntry>();

                result[day].Add(new MealEntry
                {
                    MealType = mealType,
                    Recipe = recipe
                });
            }

            return result;
        }

        public Recipe GetRecipeById(int recipeId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
                    SELECT r.RecipeID, r.Title, r.Description, r.Instructions, r.PrepTime, r.CookTime,
                           r.Servings, r.Calories, r.ImagePath, c.Name AS Category
                    FROM Recipes r
                    LEFT JOIN Categories c ON r.CategoryID = c.CategoryID
                    WHERE r.RecipeID = @id", conn); 

            cmd.Parameters.AddWithValue("@id", recipeId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string instructionText = reader["Instructions"]?.ToString() ?? "";

                return new Recipe
                {
                    RecipeID = reader.GetInt32("RecipeID"),
                    Title = reader.GetString("Title"),
                    Description = reader.IsDBNull("Description") ? "No description provided." : reader.GetString("Description"),
                    Instructions = instructionText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                    PrepTime = reader.IsDBNull("PrepTime") ? 0 : reader.GetInt32("PrepTime"),
                    CookTime = reader.IsDBNull("CookTime") ? 0 : reader.GetInt32("CookTime"),
                    Servings = reader.IsDBNull("Servings") ? 1 : reader.GetInt32("Servings"),
                    Calories = reader.IsDBNull("Calories") ? 0 : reader.GetInt32("Calories"),
                    ImagePath = reader.IsDBNull("ImagePath") ? null : reader.GetString("ImagePath"),
                    Category = reader.IsDBNull("Category") ? "Uncategorized" : reader.GetString("Category"),
                    Ingredients = GetIngredientsByRecipeId(recipeId)
                };
            }

            return null;
        }

        public List<string> GetIngredientsByRecipeId(int recipeId)
        {
            var ingredients = new List<string>();

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var cmd = new MySqlCommand(@"
        SELECT i.Name
        FROM RecipeIngredients ri
        JOIN Ingredients i ON ri.IngredientID = i.IngredientID
        WHERE ri.RecipeID = @RecipeID", conn);

            cmd.Parameters.AddWithValue("@RecipeID", recipeId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ingredients.Add(reader.GetString("Name"));
            }

            return ingredients;
        }

        public int GetMealPlanOwner(int planId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT UserID FROM MealPlans WHERE MealPlanID = @id", conn);
            cmd.Parameters.AddWithValue("@id", planId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool UpdateMealPlan(int userId, int planId, Dictionary<string, List<MealEntry>> updatedPlan)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        //delete old meals
                        var deleteCmd = new MySqlCommand("DELETE FROM MealPlanItems WHERE MealPlanID = @MealPlanID", connection, transaction);
                        deleteCmd.Parameters.AddWithValue("@MealPlanID", planId);
                        deleteCmd.ExecuteNonQuery();

                        //insert updated meals
                        foreach (var day in updatedPlan)
                        {
                            foreach (var entry in day.Value)
                            {
                                var insertCmd = new MySqlCommand(@"
                            INSERT INTO MealPlanItems (MealPlanID, DayOfWeek, MealType, RecipeID)
                            VALUES (@MealPlanID, @DayOfWeek, @MealType, @RecipeID)", connection, transaction);

                                insertCmd.Parameters.AddWithValue("@MealPlanID", planId);
                                insertCmd.Parameters.AddWithValue("@DayOfWeek", day.Key);
                                insertCmd.Parameters.AddWithValue("@MealType", entry.MealType);
                                insertCmd.Parameters.AddWithValue("@RecipeID", entry.Recipe.RecipeID);
                                insertCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }


    }
}
