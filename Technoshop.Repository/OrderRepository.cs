using System;
using System.Threading.Tasks;
using Technoshop.Common;
using Technoshop.Model;
using Technoshop.Repository.Common;
using Npgsql;
using System.Text;
using Npgsql.Replication.PgOutput.Messages;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using EllipticCurve.Utils;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Technoshop.Repository
{


    public class OrderRepository : IOrderRepository
    {

        private readonly IConnectionStringProvider _connectionProvider;
        private readonly ILogger _logger;

        public OrderRepository(IConnectionStringProvider connectionProvider, ILogger logger)
        {
            _connectionProvider = connectionProvider;
            _logger = logger;
        }
        /*
         * GET WITH FILTER
         */

        public async Task<PagedList<Order>> GetOrdersWithDataAsync(Paginate paginate, OrderFilter filtering, OrderSorting sorting)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionProvider.GetConnectionString());

            List<Order> orders = new List<Order>();
            connection.Open();

            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("", connection);
                    StringBuilder cmdBuilder = new StringBuilder("select o.\"Id\", o.\"CreatedAt\", o.\"ShippingAddressId\", o.\"BillingAddressId\", o.\"PersonId\", o.\"TotalPrice\", " +
                                    " pe.\"Id\" as \"PersonId\", pe.\"FirstName\", pe.\"LastName\", pe.\"Email\", sa.\"Id\" as \"ShippingAddressId\", sa.\"StreetName\" as \"ShippingStreetName\", " +
                                    "sa.\"StreetNumber\" as \"ShippingStreetNumber\",sa.\"City\" as \"ShippingCity\", sa.\"Zipcode\" as \"ShippingZipcode\", " +
                                    "ba.\"Id\" as \"BillingAddressId\", ba.\"StreetName\" as \"BillingStreetName\", ba.\"StreetNumber\" as \"BillingStreetNumber\", ba.\"City\" as \"BillingCity\", " +
                                    "ba.\"Zipcode\" as \"BillingZipcode\" from \"Order\" o " +
                                    "inner join \"Person\" pe on o.\"PersonId\" = pe.\"Id\" inner join \"Address\" sa on sa.\"Id\" = o.\"ShippingAddressId\" inner join \"Address\" ba on ba.\"Id\" = " +
                                    "o.\"BillingAddressId\"");
                    NpgsqlCommand countCmd = new NpgsqlCommand("", connection);
                    StringBuilder countBuilder = new StringBuilder("SELECT COUNT(*) FROM \"Order\" o " +
                       "INNER JOIN \"Person\" pe ON o.\"PersonId\" = pe.\"Id\" " +
                       "INNER JOIN \"Address\" sa ON sa.\"Id\" = o.\"ShippingAddressId\" " +
                       "INNER JOIN \"Address\" ba ON ba.\"Id\" = o.\"BillingAddressId\"");


                    cmdBuilder.Append(" WHERE 1 = 1 AND o.\"IsActive\" = true");
                    countBuilder.Append(" WHERE 1 = 1 AND o.\"IsActive\" = true");
                    if (!string.IsNullOrEmpty(filtering.SearchQuery))
                    {
                        cmdBuilder.Append(" AND (LOWER(pe.\"FirstName\") LIKE '%' || LOWER(@SearchQuery) || '%' OR LOWER(pe.\"LastName\") LIKE '%' || LOWER(@SearchQuery) || '%')");
                        cmd.Parameters.AddWithValue("@SearchQuery", filtering.SearchQuery);

                        countBuilder.Append(" AND (LOWER(pe.\"FirstName\") LIKE '%' || LOWER(@SearchQuery) || '%' OR LOWER(pe.\"LastName\") LIKE '%' || LOWER(@SearchQuery) || '%')");
                        countCmd.Parameters.AddWithValue("@SearchQuery", filtering.SearchQuery);
                    }
                    if (filtering.MinPrice.HasValue && filtering.MinPrice.Value != 0)
                    {
                        cmdBuilder.Append(" AND o.\"TotalPrice\" > @MinPrice");
                        countBuilder.Append(" AND o.\"TotalPrice\" > @MinPrice");

                        cmd.Parameters.AddWithValue("@MinPrice", filtering.MinPrice);
                        countCmd.Parameters.AddWithValue("@MinPrice", filtering.MinPrice);
                    }
                    if (filtering.MaxPrice.HasValue && filtering.MaxPrice.Value != 0)
                    {
                        cmdBuilder.Append(" AND o.\"TotalPrice\" < @MaxPrice");
                        countBuilder.Append(" AND o.\"TotalPrice\" < @MaxPrice");

                        cmd.Parameters.AddWithValue("@MaxPrice", filtering.MaxPrice);
                        countCmd.Parameters.AddWithValue("@MaxPrice", filtering.MaxPrice);
                    }

                    cmdBuilder.Append(" ORDER BY ");
                    if (!string.IsNullOrEmpty(sorting.SortBy))
                    {
                        cmdBuilder.Append(" @SortBy ");
                        cmd.Parameters.AddWithValue("@SortBy", sorting.SortBy);
                    }
                    else
                    {
                        cmdBuilder.Append(" o.\"CreatedAt\" ");
                    }

                    if (sorting.OrderBy)
                    {
                        cmdBuilder.Append(" ASC ");
                    }
                    else
                    {
                        cmdBuilder.Append(" DESC ");
                    }
                    cmdBuilder.Append(" OFFSET @OFFSET LIMIT @LIMIT");
                    cmd.Parameters.AddWithValue("@OFFSET", (paginate.PageNumber - 1) * paginate.PageSize);
                    cmd.Parameters.AddWithValue("@LIMIT", paginate.PageSize);

                    countCmd.CommandText = countBuilder.ToString();
                    int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

                    cmd.CommandText = cmdBuilder.ToString();
                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                    using (reader)
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Order order = new Order();
                                order.ShippingAddress = new Address();
                                order.BillingAddress = new Address();
                                order.Products = new List<Product>();

                                order.Id = (Guid)reader["Id"];
                                order.CreatedAt = (DateTime)reader["CreatedAt"];
                                order.TotalPrice = (decimal)reader["TotalPrice"];
                                order.Person = new Person();
                                order.Person.Id = (Guid)reader["PersonId"];
                                order.Person.FirstName = (string)reader["FirstName"];
                                order.Person.LastName = (string)reader["LastName"];
                                order.Person.Email = (string)reader["Email"];
                                order.PersonId = (Guid)reader["PersonId"];
                                order.ShippingAddress.Id = (Guid)reader["ShippingAddressId"];
                                order.ShippingAddress.StreetName = (string)reader["ShippingStreetName"];
                                order.ShippingAddress.StreetNumber = (string)reader["ShippingStreetNumber"];
                                order.ShippingAddress.City = (string)reader["ShippingCity"];
                                order.ShippingAddress.ZipCode = (int)reader["ShippingZipcode"];
                                order.BillingAddress.Id = (Guid)reader["BillingAddressId"];
                                order.BillingAddress.StreetName = (string)reader["BillingStreetName"];
                                order.BillingAddress.StreetNumber = (string)reader["BillingStreetNumber"];
                                order.BillingAddress.City = (string)reader["BillingCity"];
                                order.BillingAddress.ZipCode = (int)reader["BillingZipcode"];

                                orders.Add(order);
                            }
                        }
                    }

                    PagedList<Order> pagedOrders = new PagedList<Order>()
                    {
                        Results = orders,
                        CurrentPage = paginate.PageNumber,
                        PageSize = paginate.PageSize,
                        TotalCount = totalCount
                    };

                    return pagedOrders;
                }
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex.Message, $"Retrieving orders encountered an error at the data source: \n {ex}");
                return null;
            }
            catch (Exception exp) 
            {
                _logger.LogError(exp.Message, $"Couldn't retrieve the orders:\n {exp}");
                return null;
            }
        }


        /*
         * GET BY ID METHOD
         */
        public async Task<Order> GetOrderById(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionProvider.GetConnectionString());
            connection.Open();
            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("select a.\"Id\", a.\"ShippingAddressId\", a.\"BillingAddressId\", a.\"PersonId\"," +
                       " b.\"FirstName\", b.\"LastName\", b.\"Email\", sa.\"StreetName\" as \"ShippingStreetName\", " +
                       "sa.\"StreetNumber\" as \"ShippingStreetNumber\", sa.\"City\" as \"ShippingCity\", sa.\"Zipcode\" as \"ShippingZipcode\", " +
                       "ba.\"StreetName\" as \"BillingStreetName\", ba.\"StreetNumber\" as \"BillingStreetNumber\", ba.\"City\" as \"BillingCity\", " +
                       "ba.\"Zipcode\" as \"BillingZipcode\", p.\"Name\", p.\"Price\", po.\"ProductQty\" as \"ProductQuantity\" from \"Order\" a " +
                       "inner join \"Person\" b on a.\"PersonId\" = b.\"Id\" inner join \"Address\" sa on sa.\"Id\" = a.\"ShippingAddressId\" inner join \"Address\" ba on ba.\"Id\" = " +
                       "a.\"BillingAddressId\" inner join \"ProductOrder\" po on po.\"OrderId\" = a.\"Id\" inner join \"Product\" p ON po.\"ProductId\" = p.\"Id\" WHERE a.\"Id\" = @Id;", connection);

                    Order order = new Order();
                    order.ShippingAddress = new Address();
                    order.BillingAddress = new Address();
                    order.Products = new List<Product>();

                    cmd.Parameters.AddWithValue("@Id", id);

                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        order.TotalPrice = 0;
                        order.Person = new Person();
                        order.Person.FirstName = (string)reader["FirstName"];
                        order.Person.LastName = (string)reader["LastName"];
                        order.Person.Email = (string)reader["Email"];
                        order.Id = id;
                        order.PersonId = (Guid)reader["PersonId"];
                        order.ShippingAddress.StreetName = (string)reader["ShippingStreetName"];
                        order.ShippingAddress.StreetNumber = (string)reader["ShippingStreetNumber"];
                        order.ShippingAddress.City = (string)reader["ShippingCity"];
                        order.ShippingAddress.ZipCode = (int)reader["ShippingZipcode"];
                        order.BillingAddress.StreetName = (string)reader["BillingStreetName"];
                        order.BillingAddress.StreetNumber = (string)reader["BillingStreetNumber"];
                        order.BillingAddress.City = (string)reader["BillingCity"];
                        order.BillingAddress.ZipCode = (int)reader["BillingZipcode"];

                        do
                        {
                            Product product = new Product();
                            product.Id = (Guid)reader["Id"];
                            product.Name = (string)reader["Name"];
                            product.Price = (decimal)reader["Price"];
                            product.Quantity = (int)reader["ProductQuantity"];
                            order.TotalPrice += (product.Price ?? 1) * (product.Quantity ?? 1);

                            order.Products.Add(product);

                        } while (reader.Read());

                        reader.Close();

                        if (order != null)
                        {
                            return order;
                        }
                        return null;
                    }
                    return null;
                }
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex.Message, "Getting the order encountered an error at the source");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Couldn't fetch the order");
                return null;
            } 
        }


        /*
         * DELETE METHOD
         * */

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionProvider.GetConnectionString());
            connection.Open();
            using (connection)
            {
                NpgsqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    NpgsqlCommand cmdOrder = new NpgsqlCommand("Update \"Order\" SET \"IsActive\" = false WHERE \"Id\"=@Id", connection);
                    cmdOrder.Transaction = transaction;
                    cmdOrder.Parameters.AddWithValue("@Id", id);
                    await cmdOrder.ExecuteNonQueryAsync();

                    NpgsqlCommand cmdProductOrder = new NpgsqlCommand("Update \"ProductOrder\" SET \"IsActive\" = false WHERE \"OrderId\"=@Id", connection);
                    cmdProductOrder.Transaction = transaction;
                    cmdProductOrder.Parameters.AddWithValue("@Id", id);
                    await cmdProductOrder.ExecuteNonQueryAsync();
                }
                catch(NpgsqlException ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex.Message, "Deleting encountered an error at the source.");
                    return false;
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex.Message, "Could not delete the order");
                    return false;
                }

                transaction.Commit();
                return true;
            }
        }

        /*
         * UPDATE METHOD
         */

        public async Task<bool> UpdateOrderAsync(Guid id, Order updatedOrder)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionProvider.GetConnectionString());
            connection.Open();
            NpgsqlTransaction transaction = connection.BeginTransaction();


            try
            {
                Order existingOrder = await GetOrderById(id);



                using (transaction)
                {
                    if (existingOrder != null)
                    {
                        NpgsqlCommand command = new NpgsqlCommand("", connection);
                        StringBuilder queryBuilder = new StringBuilder();
                        queryBuilder.Append("UPDATE \"Order\" SET");

                        if (updatedOrder.PersonId != Guid.Empty && existingOrder.PersonId != updatedOrder.PersonId)
                        {
                            queryBuilder.Append(" \"PersonId\" = @PersonId,");
                            command.Parameters.AddWithValue("@PersonId", updatedOrder.PersonId);
                        }

                        if (updatedOrder.ShippingAddress != null && updatedOrder.ShippingAddress.Id != existingOrder.ShippingAddress.Id)
                        {
                            queryBuilder.Append(" \"ShippingAddressId\" = @ShippingAddressId,");
                            command.Parameters.AddWithValue("@ShippingAddressId", updatedOrder.ShippingAddress.Id);
                        }

                        if (updatedOrder.BillingAddress != null && updatedOrder.BillingAddress.Id != existingOrder.BillingAddress.Id)
                        {
                            queryBuilder.Append(" \"BillingAddressId\" = @BillingAddressId,");
                            command.Parameters.AddWithValue("@BillingAddressId", updatedOrder.BillingAddress.Id);
                        }

                        queryBuilder.Append(" \"UpdatedAt\" = @UpdatedAt,");
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                        queryBuilder.Remove(queryBuilder.Length - 1, 1);

                        queryBuilder.Append(" WHERE \"Id\" = @Id");
                        command.Parameters.AddWithValue("@Id", id);

                        command.CommandText = queryBuilder.ToString();

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            bool productsUpdated = await UpdateOrderProductsAsync(id, updatedOrder.Products, connection);

                            if (productsUpdated && rowsAffected > 0)
                            {
                                transaction.Commit();
                            }
                            return productsUpdated ? true : false;

                        }
                        transaction.Rollback();
                        return false;

                    }
                }
                return false;
            }
            catch(NpgsqlException ex)
            {
                transaction.Rollback(); 
                _logger.LogError(ex.Message, "DbSource encountered an error");
                return false;
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex.Message, "Couldn't update an order");
                return false;
            }
        }


        private async Task<bool> UpdateOrderProductsAsync(Guid orderId, List<Product> updatedProducts, NpgsqlConnection connection)
        {

            bool softDeleted = await DeleteProductOrdersAsync(orderId, connection);
            bool allProductsUpdated = true;

            foreach (Product product in updatedProducts)
            {
                bool productUpdated = await AddProductOrderAsync(orderId, product, connection);
                allProductsUpdated = allProductsUpdated && productUpdated;
            }

            return softDeleted && allProductsUpdated;
            
            
        }

        private async Task<bool> DeleteProductOrdersAsync(Guid orderId, NpgsqlConnection connection)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("", connection);
                command.CommandText = "Update \"ProductOrder\" SET \"IsActive\" = false WHERE \"OrderId\" = @OrderId";
                command.Parameters.AddWithValue("@OrderId", orderId);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0 ? true : false;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, "Failed to remove products from the order.");
                return false;
            }

        }

        private async Task<bool> AddProductOrderAsync(Guid orderId, Product product, NpgsqlConnection connection)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("", connection);
                Guid id = Guid.NewGuid();
                command.CommandText = "INSERT INTO \"ProductOrder\" (\"Id\",\"OrderId\", \"ProductId\", \"ProductQty\") VALUES (@Id,@OrderId, @ProductId, @ProductQuantity)";
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@ProductId", product.Id);
                command.Parameters.AddWithValue("@ProductQuantity", product.Quantity);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0 ? true : false;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, "Failed to add products to order");
                return false;
            }



        }


        public async Task<bool> CreateOrderAsync(Order order)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionProvider.GetConnectionString());
            
            using (connection) 
            {
                connection.Open();
                Guid userId = Guid.Empty;
                foreach (var product in order.Products)
                {
                    if (product.Price != null && product.Quantity != null)
                    {
                        order.TotalPrice += product.Price.Value * product.Quantity.Value;
                    }
                }

                NpgsqlTransaction transaction = connection.BeginTransaction();
                using (transaction)
                {
                    try
                    {
                        if (order.Person != null)
                        {
                            NpgsqlCommand cmdUserSelect = new NpgsqlCommand("Select u.\"Id\" from \"User\" u inner join \"Person\" p on u.\"PersonId\" = p.\"Id\" where p.\"Id\" = @Id", connection);
                            cmdUserSelect.Parameters.AddWithValue("@Id", order.Person.Id);

                            var userReader = await cmdUserSelect.ExecuteReaderAsync();
                            userReader.Read();
                            userId = (Guid)userReader["Id"];
                            userReader.Close();
                        }

                        NpgsqlCommand cmdOrder = new NpgsqlCommand("Insert into \"Order\" (\"Id\",\"PersonId\",\"ShippingAddressId\",\"BillingAddressId\",\"TotalPrice\",\"CreatedAt\",\"UpdatedAt\",\"IsActive\",\"CreatedBy\",\"UpdatedBy\")" +
                            "VALUES (@Id,@PersonId,@ShippingAddressId,@BillingAddressId,@TotalPrice,@CreatedAt,@UpdatedAt,@IsActive,@CreatedBy,@UpdatedBy)", connection);

                        cmdOrder.Parameters.AddWithValue("Id", order.Id);

                        if (order.Person != null && order.ShippingAddress != null && order.BillingAddress != null)
                        {
                            cmdOrder.Parameters.AddWithValue("PersonId", order.Person.Id);
                            cmdOrder.Parameters.AddWithValue("ShippingAddressId", order.ShippingAddress.Id);
                            cmdOrder.Parameters.AddWithValue("BillingAddressId", order.BillingAddress.Id);
                        }

                        cmdOrder.Parameters.AddWithValue("CreatedAt", DateTime.UtcNow);
                        cmdOrder.Parameters.AddWithValue("UpdatedAt", DateTime.UtcNow);
                        cmdOrder.Parameters.AddWithValue("IsActive", true);
                        cmdOrder.Parameters.AddWithValue("TotalPrice", order.TotalPrice);
                        cmdOrder.Parameters.AddWithValue("CreatedBy", userId);
                        cmdOrder.Parameters.AddWithValue("UpdatedBy", userId);

                        await cmdOrder.ExecuteNonQueryAsync();

                        if (order.Products != null)
                        {
                            NpgsqlCommand cmdProductOrder = new NpgsqlCommand("Insert into \"ProductOrder\" (\"Id\", \"ProductId\", \"OrderId\", \"ProductQty\") VALUES (@Id, @ProductId, @OrderId, @ProductQuantity)", connection);

                            foreach (var product in order.Products)
                            {
                                Guid productOrderId = Guid.NewGuid();

                                cmdProductOrder.Parameters.Clear();
                                cmdProductOrder.Parameters.AddWithValue("@Id", productOrderId);
                                cmdProductOrder.Parameters.AddWithValue("@ProductId", product.Id);
                                cmdProductOrder.Parameters.AddWithValue("@OrderId", order.Id);
                                cmdProductOrder.Parameters.AddWithValue("@ProductQuantity", product.Quantity);

                                await cmdProductOrder.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (NpgsqlException ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex,"Failed creating an order. Please try again!");
                    }
                    catch(Exception exp) 
                    {
                        transaction.Rollback();
                        _logger.LogError(exp.Message, "Something went wrong, please try again.");
                    }
                    return false;
                }
            }
        }
            







    }
}
















//PAGINATION USING KEYSET VALUES NEEDS FINISHING
//public async Task<List<Order>> GetOrdersWithDataAsync(Paginate paginate, OrderFilter filtering, OrderSorting sorting, NpgsqlConnection connection)
//{
//    List<Order> orders = new List<Order>();

//    StringBuilder cmdBuilder = new StringBuilder("select o.\"Id\", o.\"CreatedAt\", o.\"ShippingAddressId\", o.\"BillingAddressId\", o.\"PersonId\", o.\"TotalPrice\", " +
//                    " pe.\"Id\" as \"PersonId\", pe.\"FirstName\", pe.\"LastName\", pe.\"Email\", sa.\"Id\" as \"ShippingAddressId\", sa.\"StreetName\" as \"ShippingStreetName\", " +
//                    "sa.\"StreetNumber\" as \"ShippingStreetNumber\",sa.\"City\" as \"ShippingCity\", sa.\"Zipcode\" as \"ShippingZipcode\", " +
//                    "ba.\"Id\" as \"BillingAddressId\", ba.\"StreetName\" as \"BillingStreetName\", ba.\"StreetNumber\" as \"BillingStreetNumber\", ba.\"City\" as \"BillingCity\", " +
//                    "ba.\"Zipcode\" as \"BillingZipcode\", Count(*) Over() as \"TotalCount\" from \"Order\" o " +
//                    "inner join \"Person\" pe on o.\"PersonId\" = pe.\"Id\" inner join \"Address\" sa on sa.\"Id\" = o.\"ShippingAddressId\" inner join \"Address\" ba on ba.\"Id\" = " +
//                    "o.\"BillingAddressId\"");

//    NpgsqlCommand cmd = new NpgsqlCommand("", connection);

//    cmdBuilder.Append(" WHERE 1 = 1 AND o.\"IsActive\" = true");
//    if (!string.IsNullOrEmpty(filtering.SearchQuery))
//    {
//        cmdBuilder.Append(" AND pe.\"FirstName\" LIKE \'%\' || @SearchQuery || \'%\' OR pe.\"LastName\" LIKE \'%\' || @SearchQuery || \'%\'");
//        cmd.Parameters.AddWithValue("@SearchQuery", filtering.SearchQuery);
//    }
//    if (filtering.MinPrice.HasValue && filtering.MinPrice.Value != 0)
//    {
//        cmdBuilder.Append(" AND o.\"TotalPrice\" > @MinPrice");
//        cmd.Parameters.AddWithValue("@MinPrice", filtering.MinPrice);
//    }
//    if (filtering.MaxPrice.HasValue && filtering.MaxPrice.Value != 0)
//    {
//        cmdBuilder.Append(" AND o.\"TotalPrice\" < @MaxPrice");
//        cmd.Parameters.AddWithValue("@MaxPrice", filtering.MaxPrice);
//    }
//    cmdBuilder.Append(" AND");
//    if (!string.IsNullOrEmpty(sorting.SortBy) && sorting.LastRowKeyset != null)
//    {
//        cmdBuilder.Append(" @SortBy");
//        if (sorting.OrderBy)
//        {
//            cmdBuilder.Append(" >");
//        }
//        cmdBuilder.Append(" <");
//        Type type = sorting.LastRowKeyset.GetType();
//        if (type == typeof(string))
//        {
//            cmdBuilder.Append(" \"@LastRowKeyset\"");
//        }
//        else if (type == typeof(int) || type == typeof(decimal) || type == typeof(double) || type == typeof(float))
//        {
//            cmdBuilder.Append(" @LastRowKeyset");
//        }
//        else
//        {
//            cmdBuilder.Append(" \'@LastRowKeyset\'");
//        }
//        cmd.Parameters.AddWithValue("@LastRowKeyset", sorting.LastRowKeyset);
//        cmd.Parameters.AddWithValue("@SortBy", sorting.SortBy);

//    }
//    else
//    {
//        cmdBuilder.Append(" o.\"CreatedAt\" > TIMESTAMP 'epoch'");
//    }

//    cmdBuilder.Append(" ORDER BY ");
//    if (!string.IsNullOrEmpty(sorting.SortBy))
//    {
//        cmdBuilder.Append(" @SortBy ");
//        cmd.Parameters.AddWithValue("@SortBy", sorting.SortBy);
//    }
//    else
//    {
//        cmdBuilder.Append(" o.\"CreatedAt\" ");
//    }
//    if (sorting.OrderBy)
//    {
//        cmdBuilder.Append(" ASC ");
//    }
//    else
//    {
//        cmdBuilder.Append(" DESC ");
//    }
//    cmdBuilder.Append(" LIMIT @pageSize");
//    cmd.Parameters.AddWithValue("@pageSize", paginate.PageSize);

//    cmd.CommandText = cmdBuilder.ToString();
//    cmd.Connection = connection;

//    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
//    int i = 1;
//    sorting.LastRowKeyset = new Dictionary<int, object>();

//    using (reader)
//    {
//        if (reader.HasRows)
//        {
//            while (reader.Read())
//            {
//                Guid orderId = (Guid)reader["Id"];

//                Order order = new Order();
//                order.ShippingAddress = new Address();
//                order.BillingAddress = new Address();
//                order.Products = new List<Product>();

//                order.Id = (Guid)reader["Id"];
//                order.CreatedAt = (DateTime)reader["CreatedAt"];
//                order.TotalPrice = (decimal)reader["TotalPrice"];
//                order.Person = new Person();
//                order.Person.Id = (Guid)reader["PersonId"];
//                order.Person.FirstName = (string)reader["FirstName"];
//                order.Person.LastName = (string)reader["LastName"];
//                order.Person.Email = (string)reader["Email"];
//                order.PersonId = (Guid)reader["PersonId"];
//                order.ShippingAddress.Id = (Guid)reader["ShippingAddressId"];
//                order.ShippingAddress.StreetName = (string)reader["ShippingStreetName"];
//                order.ShippingAddress.StreetNumber = (string)reader["ShippingStreetNumber"];
//                order.ShippingAddress.City = (string)reader["ShippingCity"];
//                order.ShippingAddress.ZipCode = (int)reader["ShippingZipcode"];
//                order.BillingAddress.Id = (Guid)reader["BillingAddressId"];
//                order.BillingAddress.StreetName = (string)reader["BillingStreetName"];
//                order.BillingAddress.StreetNumber = (string)reader["BillingStreetNumber"];
//                order.BillingAddress.City = (string)reader["BillingCity"];
//                order.BillingAddress.ZipCode = (int)reader["BillingZipcode"];

//                orders.Add(order);

//                if (orders.Count % paginate.PageSize == 0)
//                {
//                    if (!string.IsNullOrEmpty(sorting.SortBy))
//                    {
//                        object setRowValue = reader[$"{sorting.SortBy}"];
//                        sorting.LastRowKeyset[i] = setRowValue;
//                    }
//                    object defaultConditionRowValue = (DateTime)reader["CreatedAt"];
//                    sorting.LastRowKeyset[i] = defaultConditionRowValue;

//                    i++;
//                }
//            }
//        }
//        return orders;
//    }
//}


