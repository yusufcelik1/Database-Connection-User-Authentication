using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace ConsoleApp4
{
    internal class Program
    {
         private static SqlConnection connection;
        static void Main(string[] args)
        {
            string Password1, Password2;
            Console.WriteLine("Üye Ol");
            Console.Write("Kullanıcı Adı: ");
            string userName = Console.ReadLine();
            Console.WriteLine("Adınızı giriniz: ");
            string Name= Console.ReadLine();
            Console.WriteLine("Soyadınızı giriniz: ");
            string Surname = Console.ReadLine();
            Console.WriteLine("E-mailinizi giriniz: ");
            string Email = Console.ReadLine();
            do
            {
                Console.Write("Parola: ");
                Password1 = ReadPassword();
                Console.Write("Parola: ");
                Password2 = ReadPassword();
                if (Password1 != Password2)
                {
                    Console.WriteLine("Parola eşleşmedi! Lütfen tekrar deneyin.");
                }
            } while (Password1 != Password2);
            string hashedParola = HashParola(Password2);
            Console.WriteLine("Şifreler eşleşti. Kayıt Başarılı");
            Console.WriteLine(hashedParola);

            string connectionString = @"Server=DESKTOP-FLOJOJC\SQLEXPRESS; Database=Eticaret; Trusted_Connection=SSPI; MultipleActiveResultSets=true; TrustServerCertificate=true;";
            // Burada veritabanına üye bilgilerini kaydedebilirsiniz.
            connection = new SqlConnection(connectionString);
            connection.Open();
            VeriEkle(userName,Name,Surname,Email, hashedParola);
            Console.ReadLine();
        }
        static string HashParola(string Password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            return password.ToString();
        }
        public static void VeriEkle(string userName, string Name, string Surname, string Email, string hashedParola)
        {
            string query = "INSERT INTO Users (userName, Name, Surname, E_mail, Password) VALUES (@UserName, @Name, @Surname, @E_mail, @Password)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userName", userName);
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Surname", Surname);
                command.Parameters.AddWithValue("@E_mail", Email);
                command.Parameters.AddWithValue("@Password", hashedParola);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Veri eklendi.");
        }

    }
}

