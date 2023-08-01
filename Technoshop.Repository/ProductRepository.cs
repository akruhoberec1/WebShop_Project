using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Npgsql;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Model.Common;
using Technoshop.Repository.Common;

namespace Technoshop.Repository
{
    public class ProductRepository : IProductRepository
    {

        private readonly IConnectionStringProvider _connectionProvider;

        public ProductRepository(IConnectionStringProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }



        NpgsqlConnection conn = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));



        public async Task<PagedList<Product>> GetProductsAsync(ProductFilter filter, Sorting sorting, Paginate pagination)
        {
            List<Product> products = new List<Product>();
            try
            {
                using (conn)
                {
                    await conn.OpenAsync();
                    NpgsqlCommand command = new NpgsqlCommand();

                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append("SELECT p.\"Id\", p.\"Name\", c.\"Title\", p.\"Price\",");
                    queryBuilder.Append(" p.\"Quantity\", p.\"CategoryId\", p.\"CreatedAt\", p.\"UpdatedAt\", p.\"IsActive\", p.\"CreatedBy\", p.\"UpdatedBy\",");
                    queryBuilder.Append(" perCreated.\"FirstName\" AS CreatedByFirstName, perCreated.\"LastName\" AS CreatedByLastName ,");
                    queryBuilder.Append(" perUpdated.\"FirstName\" AS UpdatedByFirstName, perUpdated.\"LastName\" AS UpdatedByLastName");
                    queryBuilder.Append(" FROM \"Product\" p");
                    queryBuilder.Append(" JOIN \"Category\" c ON p.\"CategoryId\" = c.\"Id\"");
                    queryBuilder.Append(" JOIN \"User\" uc ON uc.\"Id\" = p.\"CreatedBy\"");
                    queryBuilder.Append(" JOIN \"User\" uu ON uu.\"Id\" = p.\"UpdatedBy\"");
                    queryBuilder.Append(" JOIN \"Person\" perCreated ON perCreated.\"Id\" = uc.\"PersonId\"");
                    queryBuilder.Append(" JOIN \"Person\" perUpdated ON perUpdated.\"Id\" = uu.\"PersonId\"");

                    // filter
                    if (filter != null)
                    {
                        queryBuilder.Append(" WHERE 1 = 1  ");

                        if (!string.IsNullOrEmpty(filter.SearchQuery))
                        {
                            queryBuilder.Append(" AND p.\"Name\" LIKE '%' || @SearchQuery || '%' ");
                            command.Parameters.AddWithValue("@SearchQuery", filter.SearchQuery);
                        }

                        if (!string.IsNullOrEmpty(filter.Title))
                        {
                            queryBuilder.Append(" AND c.\"Title\" = @Title ");
                            command.Parameters.AddWithValue("@Title", filter.Title);
                        }

                    
                        if (filter.MinPrice != null)
                        {
                            queryBuilder.Append(" AND p.\"Price\" > @MinPrice ");
                            command.Parameters.AddWithValue("@MinPrice", filter.MinPrice);
                        }

                     
                        if (filter.MaxPrice != null)
                        {
                            queryBuilder.Append(" AND p.\"Price\" < @MaxPrice ");
                            command.Parameters.AddWithValue("@MaxPrice", filter.MaxPrice);
                        }

                    if (filter.Quantity != null)
                        {
                            queryBuilder.Append(" AND p.\"Quantity\" = @Quantity ");
                            command.Parameters.AddWithValue("@Quantity", filter.Quantity);
                        }

                        queryBuilder.Remove(queryBuilder.Length - 1, 1);
                    }


                    //Sorting
                    if ((sorting != null && !string.IsNullOrEmpty(sorting.SortBy)))
                    {
                        queryBuilder.Append($" ORDER BY p.\"{sorting.SortBy}\" {(sorting.OrderBy)}");
                    }

                    // Pagination
                    if (pagination != null)
                    {
                        queryBuilder.Append(" LIMIT @PageSize OFFSET @Offset ");
                        command.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                       command.Parameters.AddWithValue("@Offset", ((pagination.PageNumber - 1) * (pagination.PageSize)));
                      

                    }

                    command.Connection = conn;
                    command.CommandText = queryBuilder.ToString();

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                Product product = new Product
                                {
                                    Id = reader.GetGuid(0),
                                    Name = reader.GetString(1),
                                    CategoryTitle = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    Quantity = reader.GetInt32(4),
                                    CategoryId = reader.GetGuid(5),
                                    CreatedAt = reader.GetDateTime(6),
                                    UpdatedAt = reader.GetDateTime(7),
                                    IsActive = reader.GetBoolean(8),
                                    CreatedBy = reader.GetGuid(9),
                                    UpdatedBy = reader.GetGuid(10),
                                    PersonCreated = new Person
                                    {
                                        FirstName = reader.GetString(11),
                                        LastName = reader.GetString(12),
                                    },
                                    PersonUpdated = new Person
                                    {
                                        FirstName = reader.GetString(13),
                                        LastName = reader.GetString(14)
                                    }

                                };

                                products.Add(product);
                            }
                        }
                    }

                    NpgsqlCommand cmdCount = new NpgsqlCommand("", conn);
                    StringBuilder countQuery = new StringBuilder();
                    countQuery.Append("SELECT COUNT(*) FROM \"Product\" p JOIN \"Category\" c ON p.\"CategoryId\" = c.\"Id\"");

                    // filter
                    if (filter != null)
                    {
                        countQuery.Append(" WHERE 1 = 1  ");

                        if (!string.IsNullOrEmpty(filter.SearchQuery))
                        {
                            countQuery.Append(" AND p.\"Name\" LIKE '%' || @SearchQuery || '%' ");
                            cmdCount.Parameters.AddWithValue("@SearchQuery", filter.SearchQuery);
                        }

                        if (filter.MinPrice != null)
                        {
                            countQuery.Append(" AND p.\"Price\" > @MinPrice ");
                            cmdCount.Parameters.AddWithValue("@MinPrice", filter.MinPrice);
                        }


                        if (filter.MinPrice != null)
                        {
                            countQuery.Append(" AND p.\"Price\" < @MaxPrice ");
                            cmdCount.Parameters.AddWithValue("@MaxPrice", filter.MaxPrice);
                        }
                        if (filter.Quantity != null)
                        {
                            countQuery.Append(" AND p.\"Quantity\" = @Quantity ");
                            cmdCount.Parameters.AddWithValue("@Quantity", filter.Quantity);
                        }

                        if (!string.IsNullOrEmpty(filter.Title))
                        {
                            countQuery.Append(" AND c.\"Title\" = @Title ");
                            cmdCount.Parameters.AddWithValue("@Title", filter.Title);
                        }
                        countQuery.Remove(countQuery.Length -1, 1);

                    }

                    cmdCount.Connection = conn;
                    cmdCount.CommandText = countQuery.ToString();
                    int totalCountProduct = Convert.ToInt32(await cmdCount.ExecuteScalarAsync());

                    PagedList<Product> pagedProducts = new PagedList<Product>() {

                        Results = products, 
                        CurrentPage=pagination.PageNumber, 
                        PageSize = pagination.PageSize, 
                        TotalCount = totalCountProduct
                    };
                    return pagedProducts;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving products: {ex.Message}");
                return null;
            }
        }
        /*End GetProductsAsync ------------------------------------- */

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            try
            {
                using (conn)
                {
                    await conn.OpenAsync();
                    using (NpgsqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT \"Name\", \"Price\", \"Quantity\", \"Title\" FROM \"Product\" p JOIN \"Category\" c ON p.\"CategoryId\" = c.\"Id\" WHERE p.\"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();
                                Product product = new Product
                                {
                                    Id = reader.GetGuid(0),
                                    Name = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    Quantity = reader.GetInt32(3),
                                    Category = new Category { Title = reader.GetString(4) }
                                };
                                return product;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the product: {ex.Message}");
                throw;

            }

            return null;
        }
        /*End GetProductById-----------------------------------*/

        public async Task<bool> CreateProductAsync(Product product)
        {
            try
            {
                using (conn)
                {
                    await conn.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        Guid newGuid = Guid.NewGuid();
                        command.Connection = conn;
                        command.CommandText = "INSERT INTO \"Product\" (\"Id\", \"Name\", \"Price\", \"Quantity\", \"CategoryId\", \"CreatedAt\", \"UpdatedAt\", \"IsActive\", \"CreatedBy\", \"UpdatedBy\") VALUES (@Id, @Name, @Price, @Quantity, @CategoryId, @CreatedAt, @UpdatedAt, @IsActive, @CreatedBy, @UpdatedBy)";
                        command.Parameters.AddWithValue("@Id", newGuid);
                        command.Parameters.AddWithValue("@Name", product.Name);
                        command.Parameters.AddWithValue("@Price", product.Price);
                        command.Parameters.AddWithValue("@Quantity", product.Quantity);
                        command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@IsActive", true);
                        command.Parameters.AddWithValue("@CreatedBy", product.CreatedBy);
                        command.Parameters.AddWithValue("@UpdatedBy", product.UpdatedBy);
                        await command.ExecuteNonQueryAsync();
                    }

                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the product: " + ex.Message);
                return false;
            }
        }/*End CreateProductAsync ----------------------------------*/


        public async Task<bool> UpdateProductAsync(Guid id, Product product)
        {
            try
            {
                using (conn)
                {
                    await conn.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = conn;
                        StringBuilder queryBuilder = new StringBuilder();
                        queryBuilder.Append("UPDATE \"Product\" SET");

                        if (!string.IsNullOrEmpty(product.Name))
                        {
                            queryBuilder.Append(" \"Name\" = @Name,");
                            command.Parameters.AddWithValue("@Name", product.Name);
                        }

                        if (product.Price != null)
                        {
                            queryBuilder.Append(" \"Price\" = @Price,");
                            command.Parameters.AddWithValue("@Price", product.Price);
                        }

                        if (product.Quantity != null)
                        {
                            queryBuilder.Append(" \"Quantity\" = @Quantity,");
                            command.Parameters.AddWithValue("@Quantity", product.Quantity);
                        }

                        if (product.CategoryId != null)
                        {
                            queryBuilder.Append(" \"CategoryId\" = @CategoryId,");
                            command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                        }

                        queryBuilder.Append(" \"UpdatedAt\" = @UpdatedAt,");
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                        if (product.IsActive != null)
                        {
                            queryBuilder.Append(" \"IsActive\" = @IsActive,");
                            command.Parameters.AddWithValue("@IsActive", product.IsActive);
                        }

                        if (product.UpdatedBy != null)
                        {
                            queryBuilder.Append(" \"UpdatedBy\" = @UpdatedBy,");
                            command.Parameters.AddWithValue("@UpdatedBy", product.UpdatedBy);
                        }

                        queryBuilder.Remove(queryBuilder.Length - 1, 1);

                        queryBuilder.Append(" WHERE \"Id\" = @Id");
                        command.Parameters.AddWithValue("@Id", id);

                        command.CommandText = queryBuilder.ToString();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating the product: " + ex.Message);
                return false;

            }
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                using (conn)
                {
                    await conn.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "UPDATE \"Product\" SET \"IsActive\" = CASE WHEN \"IsActive\" = true THEN false ELSE true END WHERE \"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting the product: " + ex.Message);
                return false;
            }
        } /*End DeleteProductAsync ----------------------------------*/


        public async Task<PagedList<Order>> GetProductsByOrderIdAsync(PagedList<Order> orders)
        {
            //mapiram guide za dictionary kako bi bili key, value æe biti liste proizvoda pa æu te liste dodijeliti narudzbi gdje je taj orderId
            //kako ne bih imao mnogo selectova na bazu ili iterirao kroz proizvode kako bih dodijeljivao prema orderIDu
            NpgsqlConnection connection = new NpgsqlConnection(_connectionProvider.GetConnectionString());
            connection.Open();
            using (connection)
            {
                List<Guid> orderIds = orders.Results.Select(order => order.Id).Distinct().ToList();
                Dictionary<Guid, List<Product>> productsByOrderDictionary = new Dictionary<Guid, List<Product>>();

                NpgsqlCommand cmd = new NpgsqlCommand("SELECT p.\"Id\", p.\"Name\", p.\"Price\", po_group.\"ProductQuantity\", po_group.\"OrderId\" " +
                                                      "FROM \"Product\" p " +
                                                      "INNER JOIN( " +
                                                      "SELECT \"OrderId\", \"ProductId\", \"ProductQty\" AS \"ProductQuantity\" " +
                                                      "FROM \"ProductOrder\" " +
                                                      "WHERE \"OrderId\" = ANY(@orderIds) " +
                                                      "GROUP BY \"OrderId\", \"ProductId\", \"ProductQty\" " +
                                                      ") po_group ON p.\"Id\" = po_group.\"ProductId\" WHERE p.\"IsActive\" = true", connection);
                cmd.Parameters.AddWithValue("orderIds", orderIds);
                cmd.Connection = connection;

                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Guid orderId = (Guid)reader["OrderId"];

                        if (!productsByOrderDictionary.ContainsKey(orderId))
                        {
                            productsByOrderDictionary[orderId] = new List<Product>();
                        }

                        Product product = new Product();
                        product.Id = (Guid)reader["Id"];
                        product.Name = (string)reader["Name"];
                        product.Price = (decimal)reader["Price"];
                        product.Quantity = (int)reader["ProductQuantity"];
                        productsByOrderDictionary[orderId].Add(product);
                    }
                }
                reader.Close();

                foreach (var order in orders.Results)
                {
                    if (productsByOrderDictionary.TryGetValue(order.Id, out List<Product> products))
                    {
                        order.Products = products;
                    }
                }

                return orders;
            }
        }



        public async Task<List<Product>> GetSimpleProductsAsync()
        {
            List<Product> products = new List<Product>();
            NpgsqlConnection connection = new NpgsqlConnection(_connectionProvider.GetConnectionString());
            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("select \"Id\", \"Name\", \"Price\", \"Quantity\" from \"Product\" order by \"Name\" asc", connection);
                    connection.Open();
                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.Id = (Guid)reader["Id"];
                            product.Name = (string)reader["Name"];
                            product.Price = (decimal)reader["Price"];
                            product.Quantity = (int)reader["Quantity"];

                            products.Add(product);
                        }
                    }
                    reader.Close();
                    return products;

                }
            }
            catch (Exception ex)
            {
                connection.Close();
                throw ex;
            }

        }


    }
}
