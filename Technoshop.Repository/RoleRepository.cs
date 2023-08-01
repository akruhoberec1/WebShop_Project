using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Repository.Common;

namespace Technoshop.Repository
{
    public class RoleRepository : IRoleRepository
    {
        NpgsqlConnection connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

        public async Task<PagedList<Role>> GetRoleAsync(RoleFilter filter, Sorting sorting, Paginate pagination)
        {
            List<Role> roles = new List<Role>();
            try
            {
               

                using (connection)
                {
                    await connection.OpenAsync();
                    NpgsqlCommand command = new NpgsqlCommand();

                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append(" SELECT \"Id\", \"Title\", \"IsActive\" FROM \"Role\" ");


                    if (filter != null)
                    {
                        queryBuilder.Append(" WHERE 1=1 ");

                        if (!string.IsNullOrEmpty(filter.SearchQuery))
                        {
                            queryBuilder.Append(" AND \"Title\" LIKE '%' || @SearchQuery || '%' ");
                            command.Parameters.AddWithValue("@SearchQuery", filter.SearchQuery);
                        }

                    }


                    if (sorting != null)
                    {
                        queryBuilder.Append($" ORDER BY \"{sorting.SortBy}\" {(sorting.OrderBy)} ");
                    }


                    if (pagination != null)
                    {
                        queryBuilder.Append(" LIMIT @PageSize OFFSET @Offset ");
                        command.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        command.Parameters.AddWithValue("@Offset", ((pagination.PageNumber - 1) * (pagination.PageSize)));
                    }

                    command.Connection = connection;
                    command.CommandText = queryBuilder.ToString();

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                Role role = new Role
                                {
                                    Id = reader.GetGuid(0),
                                    Title = reader.GetString(1),
                                    IsActive = reader.GetBoolean(2)
                                };

                                roles.Add(role);
                            }
                        }
                    }

                    NpgsqlCommand cmdCount = new NpgsqlCommand("", connection);
                    StringBuilder countQuery = new StringBuilder();
                    countQuery.Append(" SELECT COUNT(*) FROM \"Role\" ");

                    // filter
                    if (filter != null)
                    {
                        countQuery.Append(" WHERE 1=1 ");

                        if (!string.IsNullOrEmpty(filter.SearchQuery))
                        {
                            countQuery.Append(" AND p.\"Title\" LIKE '%' || @SearchQuery || '%' ");
                            cmdCount.Parameters.AddWithValue("@SearchQuery", filter.SearchQuery);
                        }

                    }

                    cmdCount.Connection = connection;
                    cmdCount.CommandText = countQuery.ToString();
                    int roleCount = Convert.ToInt32(await cmdCount.ExecuteScalarAsync());

                    PagedList<Role> paged = new PagedList<Role>();
                    //roles, pagination.PageNumber, pagination.PageSize, roleCount
                    return paged;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving roles: {ex.Message}");
                return null;
            }
        }
        public async Task<Role> GetRoleByIdAsync(Guid id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                await connection.OpenAsync();
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT \"Id\", \"Title\", \"CreatedAt\",\"UpdatedAt\",\"IsActive\",\"CreatedBy\",\"UpdatedBy FROM \"Role\" WHERE \"Id\" =@Id ";
                command.Parameters.AddWithValue("@Id", id);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        Role role = new Role
                        {
                            Id = reader.GetGuid(0),
                            Title = reader.GetString(1),
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            UpdatedAt = (DateTime)reader["UpdatedAt"],

                            IsActive = reader.GetBoolean(4),
                            CreatedBy = reader.GetGuid(5),
                            UpdatedBy = reader.GetGuid(6)
                        };
                        return role;
                    }
                }
            }

            return null;
        }
        public async Task<bool> CreateRoleAsync(Role role)
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
                        command.CommandText = "INSERT INTO \"Role\" (\"Id\", \"Title\", \"CreatedAt\",\"UpdatedAt\",\"IsActive\",\"CreatedBy\",\"UpdatedBy\") VALUES (@Id, @Title, @CreatedAt, @UpdatedAt, @IsActive,@CreatedBy,@UpdatedBy)";
                        command.Parameters.AddWithValue("@Id", newGuid);
                        command.Parameters.AddWithValue("Title", role.Title);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                        command.Parameters.AddWithValue("@IsActive", true);

                        command.Parameters.AddWithValue("@UpdatedBy", role.UpdatedBy);
                        command.Parameters.AddWithValue("@CreatedBy", role.CreatedAt);
                        await command.ExecuteNonQueryAsync();
                    }

                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the role: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> UpdateRoleAsync(Guid id, Role role)
        {
            try
            {
                using (connection)
                {
                    await connection.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        StringBuilder queryBuilder = new StringBuilder();
                        queryBuilder.Append(" UPDATE \"Role\" SET ");

                        if (!string.IsNullOrEmpty(role.Title))
                        {
                            queryBuilder.Append(" \"Title\" = @Title, ");
                            command.Parameters.AddWithValue("@Title", role.Title);
                        }

                        queryBuilder.Append(" \"UpdatedAt\" = @UpdatedAt, ");
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                        if (role.IsActive)
                        {
                            queryBuilder.Append(" \"IsActive\" = @IsActive, ");
                            command.Parameters.AddWithValue("@IsActive", role.IsActive);
                        }

                        if (role.UpdatedBy != null)
                        {
                            queryBuilder.Append(" \"UpdatedBy\" = @UpdatedBy, ");
                            command.Parameters.AddWithValue("@UpdatedBy", role.UpdatedBy);
                        }


                        queryBuilder.Remove(queryBuilder.Length - 2, 1);

                        queryBuilder.Append(" WHERE \"Id\" = @Id ");
                        command.Parameters.AddWithValue("@Id", id);

                        command.Connection = connection;
                        command.CommandText = queryBuilder.ToString();

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating the role: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteRoleAsync(Guid id)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection())
                {
                    await connection.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "DELETE \"Role\" SET \"IsActive\"=false WHERE \"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting the role: " + ex.Message);
                return false;
            }
        }



    }
    
   
    
    
}
 


