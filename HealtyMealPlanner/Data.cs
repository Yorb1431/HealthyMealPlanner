using MySqlConnector;
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
    }
}