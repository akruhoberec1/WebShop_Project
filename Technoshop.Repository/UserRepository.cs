using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Repository.Common;
using System.Data;
using System.Security.Cryptography;
using BCrypt.Net;
using System.Web.Security;
using Microsoft.Extensions.Logging;

namespace Technoshop.Repository
{
    public class UserRepository : IUserRepository
    {
        NpgsqlConnection connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));


        public async Task<PagedList<User>> GetUserAsync(UserFilter filter, Sorting sorting, Paginate pagination)
        {
            List<User> users = new List<User>();
            try
            {
                using (connection)
                {

                    await connection.OpenAsync();
                    StringBuilder builder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    builder.Append(" SELECT \"Id\", \"UserName\", \"Password\",\"RoleId\",\"IsActive\",\" FROM \"User\" ");

                    if (filter != null)
                    {
                        builder.Append(" WHERE 1=1 ");

                        if (!string.IsNullOrEmpty(filter.SearchQuery))
                        {
                            builder.Append(" AND \"UserName\" LIKE '%' || @SearchQuery || '%' ");
                            command.Parameters.AddWithValue("@SearchQuery", filter.SearchQuery);
                        }
                    }
                    if (sorting != null)
                    {
                        builder.Append($" ORDER BY \"{sorting.SortBy}\" {(sorting.OrderBy)} ");
                    }
                    if (pagination != null)
                    {
                        builder.Append(" LIMIT @PageSize OFFSET @Offset ");
                        command.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        command.Parameters.AddWithValue("@Offset", ((pagination.PageNumber - 1) * (pagination.PageSize)));
                    }

                    command.Connection = connection;
                    command.CommandText = builder.ToString();
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                User user = new User

                                {
                                    Id = reader.GetGuid(0),
                                    UserName = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    RoleId = reader.GetGuid(3),
                                    PersonId = reader.GetGuid(4),
                                    CreatedAt = reader.GetDateTime(5),
                                    UpdatedAt = reader.GetDateTime(6),
                                    IsActive = reader.GetBoolean(7),
                                    UpdatedBy = reader.GetGuid(8)
                                };

                                users.Add(user);
                            }
                        }
                    }

                    NpgsqlCommand cmdCount = new NpgsqlCommand("", connection);
                    StringBuilder countQuery = new StringBuilder();
                    countQuery.Append(" SELECT COUNT(*) FROM \"User\" ");



                    cmdCount.Connection = connection;
                    cmdCount.CommandText = countQuery.ToString();
                    int userCount = Convert.ToInt32(await cmdCount.ExecuteScalarAsync());

                    PagedList<User> paged = new PagedList<User>();
                    //users, pagination.PageNumber, pagination.PageSize, userCount
                    return paged;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving users: {ex.Message}");
                return null;
            }

        }
        public async Task<User> GetUserByIdAsync(Guid id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                await connection.OpenAsync();
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT \"Id\", \"UserName\", \"Password\",\"RoleId\",\"PersonId\",\"CreatedAt\",\"UpdatedBy,\"IsActive FROM \"User\" WHERE \"Id\" =@Id ";
                command.Parameters.AddWithValue("@Id", id);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        User user = new User
                        {
                            Id = reader.GetGuid(0),
                            Password = reader.GetString(1),
                            RoleId = reader.GetGuid(2),
                            PersonId = reader.GetGuid(3),
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            IsActive = reader.GetBoolean(5),
                            UpdatedAt = (DateTime)reader["UpdatedAt"],
                            UpdatedBy = reader.GetGuid(7)
                        };
                        return user;
                    }
                }
            }

            return null;
        }
        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    await connection.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        Guid newGuid = Guid.NewGuid();
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO \"User\" (\"Id\", \"UserName\", \"Password\",\"RoleId\",\"PersonId\",\"CreatedAt\",\"UpdatedAt\",\"IsActive\",\"Updatedby\") +" +
                          " VALUES (@Id, @UserName, @Password, @RoleId, @PersonId,@CreatedAt,@UpdatedAt,@IsActive,@UpdatedBy)";
                        command.Parameters.AddWithValue("@Id", newGuid);
                        command.Parameters.AddWithValue("@UserName", user.UserName);
                        command.Parameters.AddWithValue("@Password", user.Password);

                        command.Parameters.AddWithValue("RoleId", DateTime.Now);

                        command.Parameters.AddWithValue("PersonId", user.PersonId);

                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@UpdatedBy", user.UpdatedBy);
                        await command.ExecuteNonQueryAsync();
                    }

                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the : " + ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(Guid id, User user)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("UPDATE User");

                    NpgsqlCommand command = new NpgsqlCommand("", connection);

                    await connection.OpenAsync();
                    NpgsqlTransaction transaction = connection.BeginTransaction();
                    stringBuilder.Append("Id=@id,");
                    command.Parameters.AddWithValue("@Id", user.Id);

                    stringBuilder.Append("UserName=@userName,");
                    command.Parameters.AddWithValue("UserName", user.UserName);

                    stringBuilder.Append("Password=@password,");
                    command.Parameters.AddWithValue("Password", user.Password);





                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the : " + ex.Message);
                return false;
            }

        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection())
                {
                    await connection.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "DELETE \"User\" SET \"IsActive\"=false WHERE \"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting the user: " + ex.Message);
                return false;
            }
        }



        public async Task<User> ValidateUserAsync(User request)
        {
            User requestedUser = new User();

            var conn = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            using (conn)
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM \"User\" WHERE \"Username\" = @username";
                    cmd.Parameters.AddWithValue("@username", request.UserName);

                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                       
                        string password = reader["Password"].ToString();
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                        bool passwordIsValid = BCrypt.Net.BCrypt.Verify(request.Password, hashedPassword);

                        if (passwordIsValid)
                        {
                            requestedUser = new User()
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                UserName = reader["Username"].ToString(),
                                RoleId = Guid.Parse(reader["RoleId"].ToString()),
                            };
                        }
                        else
                        {
                            requestedUser = null;
                        }
                    }
                    return requestedUser;
                }
            }
        }

        public async Task<Role> GetUserRoleAsync(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            using (connection)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT r.\"Title\" FROM \"Role\" r INNER JOIN \"User\" u ON r.\"Id\" = u.\"RoleId\" WHERE u.\"Id\" = @Id", connection);

                connection.Open();
                cmd.Parameters.AddWithValue("@Id", id);

                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();   

                if(reader.Read()) 
                { 
                    Role role = new Role(); 

                    role.Title = reader["Title"].ToString();
                    return role;
                }
                return null;
            }
        }

        public async Task<User> RegisterUser(Registration data)
        {
            if (data == null) 
            {
                return null;
            }

            NpgsqlConnection connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            using (connection)
            {
                NpgsqlCommand cmdPerson = new NpgsqlCommand("INSERT INTO \"Person\" (\"Id\", \"FirstName\", \"LastName\", \"Phone\", \"Email\",\"CreatedAt\",\"UpdatedAt\",\"IsActive\") VALUES " +
                    "(@Id, @FirstName, @LastName, @Phone, @Email, @CreatedAt, @UpdatedAt, @IsActive)", connection);


                NpgsqlCommand cmdUser = new NpgsqlCommand("INSERT INTO \"User\" (\"Id\", \"Username\", \"Password\",\"RoleId\",\"PersonId\",\"CreatedAt\",\"UpdatedAt\",\"IsActive\",\"UpdatedBy\") VALUES " +
                    "(@Id, @Username, @Password, @RoleId, @PersonId, @CreatedAt, @UpdatedAt, @IsActive, @UpdatedBy)", connection);

                connection.Open();
                NpgsqlTransaction transaction = connection.BeginTransaction(); 
                
                try
                {
                    if (data.FirstName != null && data.LastName != null && data.Email != null && data.Phone != null
                        && data.Username != null && data.Password != null && data.ConfirmPassword != null && data.Password == data.ConfirmPassword)
                    {
                        Guid personId = Guid.NewGuid();

                        cmdPerson.Parameters.AddWithValue("@Firstname", data.FirstName);
                        cmdPerson.Parameters.AddWithValue("@LastName", data.LastName);
                        cmdPerson.Parameters.AddWithValue("@Email", data.Email);
                        cmdPerson.Parameters.AddWithValue("@Phone", data.Phone);
                        cmdPerson.Parameters.AddWithValue("@Id", personId);
                        cmdPerson.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        cmdPerson.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                        cmdPerson.Parameters.AddWithValue("@IsActive", true);

                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);
                        Guid userId = Guid.NewGuid();
                        cmdUser.Parameters.AddWithValue("@Id", userId);
                        cmdUser.Parameters.AddWithValue("@Username", data.Username);
                        cmdUser.Parameters.AddWithValue("@Password", hashedPassword);
                        cmdUser.Parameters.AddWithValue("@Username", data.Username);
                        cmdUser.Parameters.AddWithValue("@PersonId", personId);
                        cmdUser.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        cmdUser.Parameters.AddWithValue("UpdatedAt", DateTime.UtcNow);
                        cmdUser.Parameters.AddWithValue("@UpdatedBy", userId);
                        cmdUser.Parameters.AddWithValue("@IsActive", true);

                        // cmdPerson.Parameters.AddWithValue("@UpdatedBy", userId);

                        //hardcoded customer
                        if (data.RoleId == null || data.RoleId == Guid.Empty)
                        {
                            Guid defaultRole = new Guid("6f74a160-0b44-43a7-a16f-1c032aa7d6bb");
                            cmdUser.Parameters.AddWithValue("@RoleId", defaultRole);
                        }
                        else
                        {
                            cmdUser.Parameters.AddWithValue("@RoleId", data.RoleId);
                        }

                        await cmdPerson.ExecuteNonQueryAsync();
                        await cmdUser.ExecuteNonQueryAsync();

                        User user = new User()
                        {
                            Id = userId,
                            PersonId = personId,
                            UserName = data.Username,
                            FirstName = data.FirstName,
                            LastName = data.LastName,
                            Email = data.Email, 
                            Phone = data.Phone,
                            Password = data.Password

                        };

                        transaction.Commit();
                        connection.Close();

                        return user;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public async Task<User> RegisterAdmin(Registration data)
        {
            if (data == null)
            {
                return null;
            }

            NpgsqlConnection connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            using (connection)
            {
                NpgsqlCommand cmdPerson = new NpgsqlCommand("INSERT INTO \"Person\" (\"Id\", \"FirstName\", \"LastName\", \"Phone\", \"Email\",\"CreatedAt\",\"UpdatedAt\",\"IsActive\") VALUES " +
                    "(@Id, @FirstName, @LastName, @Phone, @Email, @CreatedAt, @UpdatedAt, @IsActive)", connection);


                NpgsqlCommand cmdUser = new NpgsqlCommand("INSERT INTO \"User\" (\"Id\", \"Username\", \"Password\",\"RoleId\",\"PersonId\",\"CreatedAt\",\"UpdatedAt\",\"IsActive\",\"UpdatedBy\") VALUES " +
                    "(@Id, @Username, @Password, @RoleId, @PersonId, @CreatedAt, @UpdatedAt, @IsActive, @UpdatedBy)", connection);

                connection.Open();
                NpgsqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    if (data.FirstName != null && data.LastName != null && data.Email != null && data.Phone != null
                        && data.Username != null && data.Password != null && data.ConfirmPassword != null && data.Password == data.ConfirmPassword)
                    {
                        Guid personId = Guid.NewGuid();

                        cmdPerson.Parameters.AddWithValue("@Firstname", data.FirstName);
                        cmdPerson.Parameters.AddWithValue("@LastName", data.LastName);
                        cmdPerson.Parameters.AddWithValue("@Email", data.Email);
                        cmdPerson.Parameters.AddWithValue("@Phone", data.Phone);
                        cmdPerson.Parameters.AddWithValue("@Id", personId);
                        cmdPerson.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        cmdPerson.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                        cmdPerson.Parameters.AddWithValue("@IsActive", true);

                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);
                        Guid userId = Guid.NewGuid();
                        cmdUser.Parameters.AddWithValue("@Id", userId);
                        cmdUser.Parameters.AddWithValue("@Username", data.Username);
                        cmdUser.Parameters.AddWithValue("@Password", hashedPassword);
                        cmdUser.Parameters.AddWithValue("@Username", data.Username);
                        cmdUser.Parameters.AddWithValue("@PersonId", personId);
                        cmdUser.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        cmdUser.Parameters.AddWithValue("UpdatedAt", DateTime.UtcNow);
                        cmdUser.Parameters.AddWithValue("@UpdatedBy", userId);
                        cmdUser.Parameters.AddWithValue("@IsActive", true);

                        // cmdPerson.Parameters.AddWithValue("@UpdatedBy", userId);

                        //hardcoded customer
                        if (data.RoleId == null || data.RoleId == Guid.Empty)
                        {
                            Guid defaultRole = new Guid("4b1e7207-97b1-4a8b-a20e-96b073a58167");
                            cmdUser.Parameters.AddWithValue("@RoleId", defaultRole);
                        }
                        else
                        {
                            cmdUser.Parameters.AddWithValue("@RoleId", data.RoleId);
                        }

                        await cmdPerson.ExecuteNonQueryAsync();
                        await cmdUser.ExecuteNonQueryAsync();

                        User user = new User()
                        {
                            Id = userId,
                            PersonId = personId,
                            UserName = data.Username,
                            FirstName = data.FirstName,
                            LastName = data.LastName,
                            Email = data.Email,
                            Phone = data.Phone,
                            Password = data.Password

                        };

                        transaction.Commit();
                        connection.Close();

                        return user;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public async Task<User> LoginUserAsync(LoginData data)
        {
            NpgsqlConnection connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));


            using (connection)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("Select u.*, r.\"Title\" as \"RoleName\" FROM \"User\" u left join \"Role\" r on u.\"RoleId\" = r.\"Id\" WHERE \"Username\" =  @Username", connection);
                try
                {
                    cmd.Parameters.AddWithValue("@Username", data.Username);
                    connection.Open();
                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                    User user = new User();
                    reader.Read();

                    if (reader.HasRows)
                    {
                        user.Id = (Guid)reader["Id"];
                        user.UserName = (string)reader["Username"];
                        user.Password = (string)reader["Password"];
                        user.RoleId = (Guid)reader["RoleId"];
                        user.Role = new Role { Title = (string)reader["RoleName"] };


                        reader.Close();
                        connection.Close();
                    
                    }

                    if(BCrypt.Net.BCrypt.Verify(data.Password, user.Password))
                    {
                        return user;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }








    }
}


