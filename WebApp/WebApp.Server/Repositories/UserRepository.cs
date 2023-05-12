using WebApp.Shared.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace WebApp.Shared.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;

        public UserRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DbConnection");
        }
  
        public User? GetUserByEmail(string Email)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM [User] WHERE Email = @email", connection);
                command.Parameters.AddWithValue("@email", Email);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    return new User 
                    { 
                        ID = (int)reader["ID"], 
                        FirstName = $"{reader["FirstName"]}",
                        LastName = $"{reader["LastName"]}", 
                        Email = $"{reader["Email"]}", 
                        UserName = $"{reader["UserName"]}", 
                        Password = $"{reader["Password"]}", 
                        Role = $"{reader["Role"]}" 
                    };
                }
                return null;
            }
        }

        public void CreateUser(User user)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO [User] (FirstName, LastName, Email, UserName, [Password], [Role]) VALUES (@firstName, @lastName, @email, @userName, @password, @role)", connection);
                command.Parameters.AddWithValue("@firstName", $"{user.FirstName}");
                command.Parameters.AddWithValue("@lastName", $"{user.LastName}");
                command.Parameters.AddWithValue("@email", $"{user.Email}");
                command.Parameters.AddWithValue("@userName", $"{user.UserName}");
                command.Parameters.AddWithValue("@password", $"{user.Password}");
                command.Parameters.AddWithValue("@role", $"{user.Role}");
                command.ExecuteNonQuery();
            }
        }
    }
}
