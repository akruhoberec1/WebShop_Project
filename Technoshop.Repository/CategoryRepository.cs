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
    public class CategoryRepository : ICategoryRepository
    {
        NpgsqlConnection conn = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

        public async Task<PagedList<Category>> GetCategoriesAsync(CategoryFilter filter, Sorting sorting, Paginate pagination)
        {
            List<Category> categories = new List<Category>();
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

                using (conn)
                {
                    await conn.OpenAsync();
                    NpgsqlCommand command = new NpgsqlCommand();

                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append(" SELECT c.\"Id\", c.\"Title\", c.\"IsActive\", c.\"CreatedAt\", c.\"UpdatedAt\", c.\"CreatedBy\", c.\"UpdatedBy\", ");
                    queryBuilder.Append(" perCreated.\"FirstName\" AS CreatedByFirstName, perCreated.\"LastName\" AS CreatedByLastName ,");
                    queryBuilder.Append(" perUpdated.\"FirstName\" AS UpdatedByFirstName, perUpdated.\"LastName\" AS UpdatedByLastName ");
                    queryBuilder.Append(" FROM \"Category\" c");
                    queryBuilder.Append(" JOIN \"User\" uc ON uc.\"Id\" = c.\"CreatedBy\" ");
                    queryBuilder.Append(" JOIN \"User\" uu ON uu.\"Id\" = c.\"UpdatedBy\" ");
                    queryBuilder.Append(" JOIN \"Person\" perCreated ON perCreated.\"Id\" = uc.\"PersonId\" ");
                    queryBuilder.Append(" JOIN \"Person\" perUpdated ON perUpdated.\"Id\" = uu.\"PersonId\" ");

                    // filter
                    if (filter != null)
                    {
                        queryBuilder.Append(" WHERE 1=1 ");

                        if (!string.IsNullOrEmpty(filter.SearchQuery))
                        {
                            queryBuilder.Append(" AND \"Title\" LIKE '%' || @SearchQuery || '%' ");
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

                    command.Connection = conn;
                    command.CommandText = queryBuilder.ToString();

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                Category category = new Category
                                {
                                    Id = reader.GetGuid(0),
                                    Title = reader.GetString(1),
                                    IsActive = reader.GetBoolean(2),
                                    CreatedAt = reader.GetDateTime(3),
                                    UpdatedAt = reader.GetDateTime(4),
                                    CreatedBy = reader.GetGuid(5),
                                    UpdatedBy = reader.GetGuid(6),
                                    PersonCreated = new Person
                                    {
                                        FirstName = reader.GetString(7),
                                        LastName = reader.GetString(8)
                                    },
                                    PersonUpdated = new Person
                                    {
                                        FirstName = reader.GetString(9),
                                        LastName = reader.GetString(10)
                                    }
                                };
                                categories.Add(category);
                            }
                        }
                    }

                    NpgsqlCommand cmdCount = new NpgsqlCommand("", conn);
                    StringBuilder countQuery = new StringBuilder();
                    countQuery.Append(" SELECT COUNT(*) FROM \"Category\" ");

                    // filter
                    if (filter != null)
                    {
                        countQuery.Append(" WHERE 1=1 ");

                        if (!string.IsNullOrEmpty(filter.SearchQuery))
                        {
                            countQuery.Append(" AND \"Title\" LIKE '%' || @SearchQuery || '%' ");
                            cmdCount.Parameters.AddWithValue("@SearchQuery", filter.SearchQuery);
                        }

                    }

                    cmdCount.Connection = conn;
                    cmdCount.CommandText = countQuery.ToString();
                    int totalCountCategory = Convert.ToInt32(await cmdCount.ExecuteScalarAsync());

                    PagedList<Category> pagedCategories = new PagedList<Category>()
                    {
                        Results = categories,
                        CurrentPage = pagination.PageNumber,
                        PageSize = pagination.PageSize,
                        TotalCount = totalCountCategory



                    };
                    return pagedCategories;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving categories: {ex.Message}");
                throw;
            }
        }
        /*End GetCategoryiesAsync ------------------------------------- */

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                using (conn)
                {
                    await conn.OpenAsync();
                    NpgsqlCommand command = conn.CreateCommand();
                    command.CommandText = "SELECT \"Id\", \"Title\", \"IsActive\" FROM \"Category\" WHERE \"Id\" = @Id";
                    command.Parameters.AddWithValue("@Id", id);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            Category category = new Category
                            {
                                Id = reader.GetGuid(0),
                                Title = reader.GetString(1),
                                IsActive = reader.GetBoolean(2)
                            };
                            return category;
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
        /*End GetCategoryById-----------------------------------*/

        public async Task<bool> CreateCategoryAsync(Category category)
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
                        command.CommandText = "INSERT INTO \"Category\" (\"Id\", \"Title\", \"CreatedAt\", \"UpdatedAt\", \"IsActive\", \"CreatedBy\", \"UpdatedBy\") VALUES (@Id, @Title, @CreatedAt, @UpdatedAt, @IsActive, @CreatedBy, @UpdatedBy)";
                        command.Parameters.AddWithValue("@Id", newGuid);
                        command.Parameters.AddWithValue("@Title", category.Title);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@IsActive", true);
                        command.Parameters.AddWithValue("@CreatedBy", category.CreatedBy);
                        command.Parameters.AddWithValue("@UpdatedBy", category.UpdatedBy);
                        await command.ExecuteNonQueryAsync();
                    }

                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the category: " + ex.Message);
                return false;
            }
        }/*End CreateCategoryAsync ----------------------------------*/


        public async Task<bool> UpdateCategoryAsync(Guid id, Category category)
        {
            try
            {
                using (conn)
                {
                    await conn.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        StringBuilder queryBuilder = new StringBuilder();
                        queryBuilder.Append(" UPDATE \"Category\" SET ");

                        if (!string.IsNullOrEmpty(category.Title))
                        {
                            queryBuilder.Append(" \"Title\" = @Title, ");
                            command.Parameters.AddWithValue("@Title", category.Title);
                        }

                        queryBuilder.Append(" \"UpdatedAt\" = @UpdatedAt, ");
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                        if (category.IsActive != null)
                        {
                            queryBuilder.Append(" \"IsActive\" = @IsActive, ");
                            command.Parameters.AddWithValue("@IsActive", category.IsActive);
                        }

                        if (category.UpdatedBy != null)
                        {
                            queryBuilder.Append(" \"UpdatedBy\" = @UpdatedBy, ");
                            command.Parameters.AddWithValue("@UpdatedBy", category.UpdatedBy);
                        }


                        queryBuilder.Remove(queryBuilder.Length - 2, 1);

                        queryBuilder.Append(" WHERE \"Id\" = @Id ");
                        command.Parameters.AddWithValue("@Id", id);

                        command.Connection = conn;
                        command.CommandText = queryBuilder.ToString();

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating the category: " + ex.Message);
                return false;
            }
        }



        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            try
            {
                using (conn)
                {
                    await conn.OpenAsync();
                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "UPDATE \"Category\" SET \"IsActive\" = NOT \"IsActive\" WHERE \"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating the category: " + ex.Message);
                return false;
            }
        }
        /*End DeleteCategoryAsync ----------------------------------*/

    }
}
