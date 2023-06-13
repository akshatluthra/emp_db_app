using System;
using System.Collections.Generic;
using EmployeeManagement.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement.Repositories
{

    public class EmpRepository
    {
        private readonly IConfiguration configuration;

        public EmpRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private string GetConnectionString()
        {
            return configuration.GetConnectionString("SqlDbConnection");
        }

        private SqlConnection GetConnection()
        {
            string connectionString = GetConnectionString();
            return new SqlConnection(connectionString);
        }

        public Employee GetById(int id)
        {
            using (SqlConnection connection = GetConnection())
            {
               
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM Employee WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return GetEmpFromSql(reader);
                    }

                    return null;
                }
                
            }
        
    

    public Employee GetEmployee (string email)
    {
            using (SqlConnection connection = GetConnection())
            {
               

                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Employee WHERE Email = @Email", connection);
                command.Parameters.AddWithValue("@Email", email);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return GetEmpFromSql(reader);
                }

                return null;
               
            }
    }

    public void AddEmployee(Employee employee)
    {
            using (SqlConnection connection = GetConnection())
            {
               

                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Employee (Name, Email, Role, Password) VALUES (@Name, @Email, @Role, @Password)", connection);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Email", employee.Email);
                command.Parameters.AddWithValue("@Role", employee.Role);
                command.Parameters.AddWithValue("@Password", employee.Password);
                command.ExecuteNonQuery();
               
                    
            }
    }

    public List<Employee> GetEmployees()
    {
        List<Employee> Employee = new List<Employee>();

        using (SqlConnection connection = GetConnection())
        {
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Employee", connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Employee employee = GetEmpFromSql(reader);
                Employee.Add(employee);
            }
        }

        return Employee;
    }

    public void UpdateEmp(Employee employee)
    {
        using (SqlConnection connection =GetConnection())
        {
            connection.Open();
            SqlCommand command = new SqlCommand("UPDATE Employee SET Name = @Name, Email = @Email, Role = @Role, Password = @Password WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", employee.Id);
            command.Parameters.AddWithValue("@Name", employee.Name);
            command.Parameters.AddWithValue("@Email", employee.Email);
            command.Parameters.AddWithValue("@Role", employee.Role);
            command.Parameters.AddWithValue("@Password", employee.Password);
            command.ExecuteNonQuery();
        }
    }

    public void DeleteEmp(int id)
    {
        using (SqlConnection connection = GetConnection())
        {
            connection.Open();
            SqlCommand command = new SqlCommand("DELETE FROM Employee WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
    }

    private Employee GetEmpFromSql(SqlDataReader reader)
    {
        return new Employee
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

