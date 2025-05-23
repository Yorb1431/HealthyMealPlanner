﻿using MySqlConnector;
using System;
using System.Security.Cryptography;
using System.Text;

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

        // Register a new user
        public bool RegisterUser(string username, string email, string password)
        {
            // First check if user already exists
            if (UserExists(email, username))
            {
                return false;
            }

            string passwordHash = HashPassword(password);
            string query = "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@username, @email, @passwordHash)";
            var parameters = new[]
            {
                new MySqlParameter("@username", username),
                new MySqlParameter("@email", email),
                new MySqlParameter("@passwordHash", passwordHash)
            };

            int userId = Insert(query, parameters);
            if (userId != -1)
            {
                // Create an empty user profile
                CreateUserProfile(userId);
                return true;
            }
            return false;
        }

        // Create a new user profile
        private bool CreateUserProfile(int userId)
        {
            string query = "INSERT INTO UserProfiles (UserID) VALUES (@userId)";
            var parameters = new[]
            {
                new MySqlParameter("@userId", userId)
            };

            return Insert(query, parameters) != -1;
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
        //get the user their email
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
//function that sends the 2FA code/reset password code and verification
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
//get username from email
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
//updates user their password(called when the verifiy code is correct in the reset password screen
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

         // Allergies (joined from ID to name)
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

        
    }
}
