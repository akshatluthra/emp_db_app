using System;
using System.Collections.Generic;
using EmployeeManagement.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement.Repositories
{

    public class UserRepository
    {
        private readonly IConfiguration configuration;

        public UserRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private string GetConnectionString()
        {
            return configuration.GetConnectionString("MyDbConnection");
        }

        private SqlConnection GetConnection()
        {
            string connectionString = GetConnectionString();
            return new SqlConnection(connectionString);
        }

        public User GetUserById(int id)
        {
            using (SqlConnection connection = GetConnection())
            {
               
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return GetUserFromSql(reader);
                    }

                    return null;
                }
                
            }
        
    

    public User GetUserByEmail(string email)
    {
            using (SqlConnection connection = GetConnection())
            {
               

                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE Email = @Email", connection);
                command.Parameters.AddWithValue("@Email", email);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return GetUserFromSql(reader);
                }

                return null;
               
            }
    }

    public void RegisterUser(User user)
    {
            using (SqlConnection connection = GetConnection())
            {
               

                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Users (Name, Email, Role, Password) VALUES (@Name, @Email, @Role, @Password)", connection);
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.ExecuteNonQuery();
               
                    
            }
    }

    public List<User> GetAllUsers()
    {
        List<User> Users = new List<User>();

        using (SqlConnection connection = GetConnection())
        {
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Users", connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                User user = GetUserFromSql(reader);
                Users.Add(user);
            }
        }

        return Users;
    }

    public void UpdateUser(User user)
    {
        using (SqlConnection connection =GetConnection())
        {
            connection.Open();
            SqlCommand command = new SqlCommand("UPDATE Users SET Name = @Name, Email = @Email, Role = @Role, Password = @Password WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Role", user.Role);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.ExecuteNonQuery();
        }
    }

    public void DeleteUser(int id)
    {
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();
            SqlCommand command = new SqlCommand("DELETE FROM Users WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
    }

    private User GetUserFromSql(SqlDataReader reader)
    {
        return new User
        {
            Id = (int)reader["Id"],
            Name = (string)reader["Name"],
            Email = (string)reader["Email"],
            Role = (string)reader["Role"],
            Password = (string)reader["Password"]
        };
    }
}
    }

