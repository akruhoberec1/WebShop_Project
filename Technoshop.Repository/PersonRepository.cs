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
    public class PersonRepository:IPersonRepository
    {
        string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

         
        public async Task<PagedList<Person>> GetPersonAsync(PersonFilter filter, Sorting sorting, Paginate pagination)
          {
        List<Person> person = new List<Person>();
        try
        {


            NpgsqlConnection connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            using (connection)
            {
                await connection.OpenAsync();
                NpgsqlCommand command = new NpgsqlCommand();

                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append("SELECT \"Id\", \"FirstName\", \"LastName\",\"Email\",\"Phone\",\"IsActive\"  FROM \"Person\" ");

                // filter
                if (filter != null)
                {
                    queryBuilder.Append(" WHERE 1=1 ");

                    if (!string.IsNullOrEmpty(filter.SearchQuery))
                    {
                        queryBuilder.Append("\"FirstName\" AND \"LastName\" LIKE '%' || @SearchQuery || '%' ");
                        command.Parameters.AddWithValue("@SearchQuery", filter.SearchQuery);
                    }

                }

                //Sorting
                if (sorting != null)
                {
                    queryBuilder.Append($" ORDER BY \"{sorting.SortBy}\" {(sorting.OrderBy)} ");
                }

                // Pagination
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
                            Person per = new Person
                            {
                                Id = reader.GetGuid(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Phone=reader.GetString(3),
                                Email=reader.GetString(4),
                                IsActive=reader.GetBoolean(5),

                            };

                            person.Add(per);
                        }
                    }
                }

                NpgsqlCommand cmdCount = new NpgsqlCommand("", connection);
                StringBuilder countQuery = new StringBuilder();
                countQuery.Append(" SELECT COUNT(*) FROM \"Person\" ");

                // filter
                if (filter != null)
                {
                    countQuery.Append(" WHERE 1=1 ");

                    if (!string.IsNullOrEmpty(filter.SearchQuery))
                    {
                        countQuery.Append("\"FirstName\" AND \"LastName\" LIKE '%' || @SearchQuery || '%' ");
                        cmdCount.Parameters.AddWithValue("@SearchQuery", filter.SearchQuery);
                    }

                }

                cmdCount.Connection = connection;
                cmdCount.CommandText = countQuery.ToString();
                int personCount = Convert.ToInt32(await cmdCount.ExecuteScalarAsync());

                PagedList<Person> pagedPerson = new PagedList<Person>();
                    //person, pagination.PageNumber, pagination.PageSize, personCount
                    return pagedPerson;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while retrieving person: {ex.Message}");
            return null;
        }
    }

    public async Task<Person> GetPersonByIdAsync(Guid id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();
                NpgsqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT \"Id\", \"FirstName\", \"LastName\",\"Email\",\"Phone\",\"IsActive\" FROM \"Person\" WHERE \"Id\" =@Id ";
                command.Parameters.AddWithValue("@Id", id);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        Person person=new Person()
                        {
                            Id = reader.GetGuid(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Phone= reader.GetString(3),
                            Email= reader.GetString(4),
                            IsActive = reader.GetBoolean(5)
                        };
                        return person;
                    }
                }
            }

            return null;
        }

        public async Task<Person> GetPersonByUserId(Guid id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();
                NpgsqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT p.\"Id\", p.\"FirstName\", p.\"LastName\",p.\"Email\",p.\"Phone\",p.\"IsActive\",p.\"CreatedAt\" FROM \"Person\" p INNER JOIN \"User\" u ON p.\"Id\" = u.\"PersonId\" WHERE u.\"Id\" =@Id ";
                command.Parameters.AddWithValue("@Id", id);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        Person person = new Person()
                        {
                            Id = reader.GetGuid(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Phone = reader.GetString(4),
                            IsActive = reader.GetBoolean(5),
                            CreatedAt = reader.GetDateTime(6)
                        };
                        return person;
                    }
                }
            }
            return null;
        }


        public async Task<bool> CreatePersonAsync(Person person)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        Guid newGuid = Guid.NewGuid();
                        command.Connection = conn;
                        command.CommandText = "INSERT INTO \"Person\" (\"Id\", \"FirstName\", \"LastName\", \"Phone\", \"Email\",\"UpdatedBy\",\"IsActive\",\"Addresses\") VALUES (@Id, @FirstName, @LastName, @Phone, @Email,@UpdatedBy,@IsActive,@Addresses)";
                        command.Parameters.AddWithValue("@Id", newGuid);
                        command.Parameters.AddWithValue("@FirstName", person.FirstName);
                        command.Parameters.AddWithValue("@LastName", person.LastName);
                        command.Parameters.AddWithValue("@Email", person.Email);
                        command.Parameters.AddWithValue("@Phone", person.Phone);

                        command.Parameters.AddWithValue("@IsActive", true);
                        
                        command.Parameters.AddWithValue("@UpdatedBy", person.UpdatedBy);
                        await command.ExecuteNonQueryAsync();
                    }

                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the person: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> UpdatePersonAsync(Guid id, Person person)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        Guid newGuid = Guid.NewGuid();
                        command.Connection = conn;
                        command.CommandText = "INSERT INTO \"Person\" (\"Id\", \"FirstName\", \"LastName\", \"Phone\", \"Email\",\"UpdatedBy\",\"IsActive\",\"Addresses\") VALUES (@Id, @FirstName, @LastName, @Phone, @Email,@UpdatedBy,@IsActive,@Addresses\"WHERE \\\"Id\\\" = @Id \";)";

                        command.Parameters.AddWithValue("@Id", newGuid);
                        command.Parameters.AddWithValue("@FirstName", person.FirstName);
                        command.Parameters.AddWithValue("@LastName", person.LastName);
                        command.Parameters.AddWithValue("@Email", person.Email);
                        command.Parameters.AddWithValue("@Phone", person.Phone);

                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                        command.Parameters.AddWithValue("@IsActive", true);

                        command.Parameters.AddWithValue("@UpdatedBy", person.UpdatedBy);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating the person: " + ex.Message);
                return false;

            }
        }
        public async Task<bool> DeletePersonAsync(Guid id)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "UPDATE \"Person\" SET \"IsActive\"=false WHERE \"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting the person: " + ex.Message);
                return false;
            }
        }
    }
}
