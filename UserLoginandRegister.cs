using System;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace UserLoginandRegister
{
    internal class Program
    {
        private static SqlConnection connection;
        static void Main(string[] args)
        {
            Console.WriteLine("1- Register | 2- Login"); // Display menu options for registration and login
            int choice = Convert.ToInt32(Console.ReadLine()); // Read the user's choice

            // Define the database connection string
            string connectionString = "Your Database ConnectionString";

            connection = new SqlConnection(connectionString); // Initialize a new SqlConnection using the provided connection string
            connection.Open(); // Open the database connection

            switch (choice)
            {
                case 1:
                    SignUp(); // Call the SignUp function for user registration
                    break;
                case 2:
                    Login(); // Call the Login function for user login
                    break;
            }

            connection.Close(); // Close the database connection after user interaction
        }
        #region User Register and Login Procedures
        static void SignUp()
        {
            string Password1, Password2;
            Console.WriteLine("Register");
            Console.Write("Username: ");
            string userName = Console.ReadLine();
            Console.WriteLine("Name: ");
            string Name = Console.ReadLine();
            Console.WriteLine("Surname: ");
            string Surname = Console.ReadLine();
            Console.WriteLine("E-mail: ");
            string Email = Console.ReadLine();
            do
            {
                Console.Write("Password: ");
                Password1 = ReadPassword();
                Console.Write("Confirm Password: "); // Prompt the user to confirm the password
                Password2 = ReadPassword();

                if (Password1 != Password2)
                {
                    Console.WriteLine("Password did not match! Please try again.");
                }
            } while (Password1 != Password2);

            string hashedPassword = HashPassword(Password2); // Hash the confirmed password
            Console.WriteLine("Passwords matched. Registration Successful");

            string query = "INSERT INTO Users (userName, Name, Surname, E_mail, Password) VALUES (@UserName, @Name, @Surname, @E_mail, @Password)";

            // Create a SqlCommand with parameters to insert user data into the database
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userName", userName);
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Surname", Surname);
                command.Parameters.AddWithValue("@E_mail", Email);
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.ExecuteNonQuery(); // Execute the query to insert data into the database
            }
        }
        static void Login()
        {
            Console.Write("Enter username: ");
            string UserName = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = ReadPassword();
            string hashedPassword = HashPassword(password); // Hash the provided password

            // Prepare an SQL command to retrieve the hashed password from the database for the given username
            SqlCommand selectCommand = new SqlCommand("SELECT Password FROM Users WHERE UserName = @UserName", connection);
            selectCommand.Parameters.AddWithValue("@Username", UserName);
            string storedHashedPassword = selectCommand.ExecuteScalar() as string; // Execute the query and retrieve the stored hashed password

            if (storedHashedPassword != null && storedHashedPassword.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Login successful.");
            }
            else
            {
                Console.WriteLine("Login failed.");
            }
        }
        #endregion
        #region Password Hiding Procedures
        static string HashPassword(string Password)
        {
            using (SHA256 sha256Hash = SHA256.Create()) // Create a SHA-256 hashing algorithm instance
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Password)); // Convert the password to bytes and compute the hash

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Convert the hash bytes to a hexadecimal string representation
                }
                return builder.ToString(); // Return the hashed password as a string
            }
        }

        static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true); // Read a key from the console without displaying it
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(); // Print a newline when Enter is pressed to maintain console formatting
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1); // Remove the last character when Backspace is pressed
                    Console.Write("\b \b"); // Clear the character from the console
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar); // Append the typed character to the password
                    Console.Write("*"); // Display an asterisk to mask the typed character
                }
            }
            return password.ToString(); // Return the securely read password as a string
        }
        #endregion
    }
}

