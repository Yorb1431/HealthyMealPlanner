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
