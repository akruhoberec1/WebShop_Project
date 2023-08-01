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
    public class AddressRepository : IAddressRepository
    {
        private readonly IConnectionStringProvider _connectionProvider;

        public AddressRepository(IConnectionStringProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        NpgsqlConnection connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
        public async Task<PagedList<Address>> GetAddressAsync(AddressFilter filter, Sorting sorting, Paginate pagination)
        {
            List<Address> addresses = new List<Address>();
            try
            {
                using (connection)
                {

                    await connection.OpenAsync();
                    StringBuilder builder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    builder.Append(" SELECT \"Id\", \"StreetName\", \"StreetNumber\",\"City\",\"ZipCode\",\"IsActive\"\" FROM \"Address\" ");

                    if (filter != null)
                    {
                        builder.Append(" WHERE 1=1 ");

                        if (!string.IsNullOrEmpty(filter.SearchQuery))
                        {
                            builder.Append(" AND \"StreetName\" LIKE '%' || @SearchQuery || '%' ");
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
                                Address address = new Address

                                {
                                    Id = reader.GetGuid(0),
                                    StreetName = reader.GetString(1),
                                    StreetNumber = reader.GetString(2),
                                    City=reader.GetString(3),
                                    ZipCode=reader.GetInt16(4),
                                  
                                    PersonId = reader.GetGuid(5),
                                    CreatedAt = reader.GetDateTime(6),
                                    UpdatedAt = reader.GetDateTime(7),
                                    IsActive = reader.GetBoolean(8),
                                    CreatedBy=reader.GetGuid(9),
                                    UpdatedBy = reader.GetGuid(10)
                                };

                                addresses.Add(address);
                            }
                        }
                    }

                    NpgsqlCommand cmdCount = new NpgsqlCommand("", connection);
                    StringBuilder countQuery = new StringBuilder();
                    countQuery.Append(" SELECT COUNT(*) FROM \"Address\" ");



                    cmdCount.Connection = connection;
                    cmdCount.CommandText = countQuery.ToString();
                    int addressCount = Convert.ToInt32(await cmdCount.ExecuteScalarAsync());

                    PagedList<Address> paged = new PagedList<Address>()
                    {
                        Results=addresses, 
                        CurrentPage=pagination.PageNumber, 
                        PageSize=pagination.PageSize, 
                        TotalCount=addressCount
                    };
                    return paged;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving address: {ex.Message}");
                return null;
            }
        }
        public async Task<Address> GetAddressByIdAsync(Guid id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection())
            {
                await conn.OpenAsync();
                NpgsqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT \"Id\", \"StreetName\",\"StreetNumber\",\"City\",\"Zipcode\",\"PersonId\",\"CreatedAt\",\"UpdatedAt\",\"IsActive\",\"CreateBy\",\"UpdatedBy\" FROM \"Person\" WHERE \"Id\" =@Id ";
                command.Parameters.AddWithValue("@Id", id);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        Address address = new Address
                        {
                            Id = reader.GetGuid(0),
                            StreetName = reader.GetString(1),
                            StreetNumber = reader.GetString(2),
                            City = reader.GetString(3),
                            ZipCode = reader.GetInt16(4),
                            PersonId = reader.GetGuid(5),
                            CreatedAt = reader.GetDateTime(6),
                            UpdatedAt = reader.GetDateTime(7),
                            CreatedBy = reader.GetGuid(8),
                            UpdatedBy=reader.GetGuid(9)
                        };
                        return address;
                    }
                }
            }

            return null;
        }
        public async Task<bool> CreateAddressAsync(Guid id, Address address)
        {
            NpgsqlConnection conn = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            Guid addressId = Guid.NewGuid();

            try
              {
                using (conn)
                {
                    conn.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                   
                        Guid personId = Guid.Empty;

                        command.Connection = conn;

                        NpgsqlCommand cmdPersonId = new NpgsqlCommand("Select \"PersonId\" from \"User\" where \"Id\" = @Id ", conn);
                        cmdPersonId.Parameters.AddWithValue("@Id", id);
                        NpgsqlDataReader personReader = await cmdPersonId.ExecuteReaderAsync();
                        if (personReader.HasRows)
                        {
                            personReader.Read();    
                            personId = (Guid)personReader["PersonId"];
                        }
                        personReader.Close();

                        command.CommandText = "INSERT INTO \"Address\" (\"Id\", \"StreetName\",\"StreetNumber\",\"City\",\"Zipcode\",\"PersonId\",\"CreatedAt\",\"UpdatedAt\",\"IsActive\",\"IsShipping\",\"CreatedBy\",\"UpdatedBy\") VALUES (@Id, @StreetName, @StreetNumber, @City, @Zipcode, @PersonId, @CreatedAt, @UpdatedAt, @IsActive, @IsShipping, @CreatedBy, @UpdatedBy)";
                        command.Parameters.AddWithValue("@Id", addressId);
                        command.Parameters.AddWithValue("@StreetName", address.StreetName);
                        command.Parameters.AddWithValue("@StreetNumber", address.StreetNumber);
                        command.Parameters.AddWithValue("@City", address.City);
                        command.Parameters.AddWithValue("@Zipcode", address.ZipCode);
                        command.Parameters.AddWithValue("@PersonId", personId);
                        command.Parameters.AddWithValue("@CreatedAt",DateTime.Now);
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@IsActive", true);
                        command.Parameters.AddWithValue("@IsShipping", address.IsShipping);
                        command.Parameters.AddWithValue("@CreatedBy",id);
                        command.Parameters.AddWithValue("@UpdatedBy",id);
  
                        await command.ExecuteNonQueryAsync();
                    }

                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the address: " + ex.Message);
                return false;
            }
        }

        /*
         * GETTING BILLING ADDRESSES FOR USER PROFILE
         ******************/
        public async Task<List<Address>> GetBillingAddressesAsync(Guid id)
        {
            List<Address> addresses = new List<Address>();
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("SELECT a.\"Id\",\"StreetName\", a.\"StreetNumber\", a.\"City\", a.\"Zipcode\" from \"Address\" a inner join \"Person\" p on a.\"PersonId\" = p.\"Id\" inner join \"User\" u on p.\"Id\" = u.\"PersonId\" WHERE u.\"Id\" = @id and \"IsShipping\" = false;", connection);
                using (connection)
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Address address = new Address

                            {
                                Id = reader.GetGuid(0),
                                StreetName = reader.GetString(1),
                                StreetNumber = reader.GetString(2),
                                City = reader.GetString(3),
                                ZipCode = reader.GetInt32(4),
                            };

                            addresses.Add(address);
                        }
                        return addresses;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving address: {ex.Message}");
                return null;
            }
        }


        /*
         * GETTING BILLING ADDRESSES FOR ORDER EDIT 
         ******************/
        public async Task<List<Address>> GetOrderBillingAddressesAsync(Guid id)
        {
            List<Address> addresses = new List<Address>();
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("SELECT a.\"Id\",\"StreetName\", a.\"StreetNumber\", a.\"City\", a.\"Zipcode\" from \"Address\" a inner join \"Person\" p on a.\"PersonId\" = p.\"Id\" WHERE p.\"Id\" = @id and \"IsShipping\" = false;", connection);
                using (connection)
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Address address = new Address

                            {
                                Id = reader.GetGuid(0),
                                StreetName = reader.GetString(1),
                                StreetNumber = reader.GetString(2),
                                City = reader.GetString(3),
                                ZipCode = reader.GetInt32(4),
                            };

                            addresses.Add(address);
                        }
                        return addresses;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving address: {ex.Message}");
                return null;
            }
        }

        /*
         * GETTING SHIPPING ADDRESSES FOR USER PROFILE
         ******************/ 
        public async Task<List<Address>> GetShippingAddressesAsync(Guid id)
        {
            List<Address> addresses = new List<Address>();
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("SELECT a.\"Id\",\"StreetName\", \"StreetNumber\", \"City\", \"Zipcode\" from \"Address\" a inner join \"Person\" p on a.\"PersonId\" = p.\"Id\" inner join \"User\" u on p.\"Id\" = u.\"PersonId\" WHERE u.\"Id\" = @id and \"IsShipping\" = true;", connection);
                using (connection)
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            Address address = new Address

                            {
                                Id = reader.GetGuid(0),
                                StreetName = reader.GetString(1),
                                StreetNumber = reader.GetString(2),
                                City = reader.GetString(3),
                                ZipCode = reader.GetInt32(4),


                            };

                            addresses.Add(address);
                            
                        }
                        return addresses;
         
                    }
                    return null;
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving address: {ex.Message}");
                return null;
            }
        }



        /*
         * GETTING SHIPPING ADDRESSES FOR ORDER EDIT 
         ******************/

        public async Task<List<Address>> GetOrderShippingAddressesAsync(Guid id)
        {
            List<Address> addresses = new List<Address>();
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("SELECT a.\"Id\",\"StreetName\", \"StreetNumber\", \"City\", \"Zipcode\" from \"Address\" a inner join \"Person\" p on a.\"PersonId\" = p.\"Id\" WHERE p.\"Id\" = @id and \"IsShipping\" = true;", connection);
                using (connection)
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            Address address = new Address

                            {
                                Id = reader.GetGuid(0),
                                StreetName = reader.GetString(1),
                                StreetNumber = reader.GetString(2),
                                City = reader.GetString(3),
                                ZipCode = reader.GetInt32(4),


                            };

                            addresses.Add(address);

                        }
                        return addresses;

                    }
                    return null;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving address: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> UpdateAddressAsync(Guid id, Address address)
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
                        command.CommandText = "INSERT INTO \"Address\" (\"Id\", \"StreetName\",\"StreetNumber\",\"City\",\"Zipcode\",\"PersonId\",\"CreatedAt\",\"UpdatedAt\",\"IsActive\",\"CreateBy\",\"UpdatedBy\") VALUES (@Id, @StreetName, @StreetNumber, @City @ZipCode,@PersonId,@CreatedAt,@UpdatedAt,@IsActive,@CreatedBy,@UpdatedBy)";
                        command.Parameters.AddWithValue("@Id", newGuid);
                        command.Parameters.AddWithValue("@StreetName", address.StreetName);
                        command.Parameters.AddWithValue("@StreetNumber", address.StreetNumber);
                        command.Parameters.AddWithValue("@City", address.City);

                        command.Parameters.AddWithValue("@ZipCode", address.ZipCode);
                        command.Parameters.AddWithValue("@PersonId", address.PersonId);
                        command.Parameters.AddWithValue("@CreatedAt", address.CreatedAt);
                        command.Parameters.AddWithValue("@UpdatedAt", address.UpdatedAt);
                        command.Parameters.AddWithValue("@IsActive", address.IsActive);
                        command.Parameters.AddWithValue("@CreatedBy", address.CreatedBy);
                        command.Parameters.AddWithValue("@UpdatedBy", address.UpdatedBy);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating the address: " + ex.Message);
                return false;

            }
        }
        public async Task<bool> DeleteAddressAsync(Guid id)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    await connection.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "UPDATE \"Address\" SET \"IsActive\" = CASE WHEN \"IsActive\" = true THEN false ELSE true END WHERE \"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting the address: " + ex.Message);
                return false;
            }
        }


        public async Task<List<Address>> GetAddressesByPersonId(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionProvider.GetConnectionString());

            using (connection)
            {
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"Address\" WHERE \"PersonId\" = @Id", connection);

                List<Address> addresses = new List<Address>();

                connection.Open();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Address address = new Address();
                        address.Id = (Guid)reader["Id"];
                        address.StreetName = (string)reader["StreetName"];
                        address.StreetNumber = (string)reader["StreetNumber"];
                        address.City = (string)reader["City"];
                        address.ZipCode = (int)reader["Zipcode"];
                        addresses.Add(address);
                    }
                    return addresses;
                }

                return null;
            }
        }




    }
}
