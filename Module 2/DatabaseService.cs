using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using ShoeStoreApp.Models;


namespace ShoeStoreApp.Services
{
    internal class DatabaseService
    {
        private const string ConnectionString = "Host=localhost;Port=5432;Database=ShoeStoreDB;Username=postgres;Password=";

        public User Authenticate(string login, string password)
        {
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT u.UserID, u.UserLogin, u.UserFullName, u.RoleID, r.RoleName
                        FROM Users u
                        JOIN Roles r ON u.RoleID = r.RoleID
                        WHERE u.UserLogin = @login AND u.UserPasswordHash = @password";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    UserID = reader.GetInt32(0),
                                    Login = reader.GetString(1),
                                    FullName = reader.GetString(2),
                                    RoleID = reader.GetInt32(3),
                                    RoleName = reader.GetString(4)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка подключения к БД: " + ex.Message);
            }
            return null;
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT p.ProductID, p.ProductArticle, p.ProductName, p.ProductUnit, p.ProductPrice,
                               s.SupplierName, m.ManufacturerName, c.CategoryName,
                               p.DiscountPercent, p.QuantityInStock, p.Description, p.ImagePath
                        FROM Products p
                        JOIN Suppliers s ON p.SupplierID = s.SupplierID
                        JOIN Manufacturers m ON p.ManufacturerID = m.ManufacturerID
                        JOIN Categories c ON p.CategoryID = c.CategoryID
                        ORDER BY p.ProductName";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductID = reader.GetInt32(0),
                                Article = reader.GetString(1),
                                Name = reader.GetString(2),
                                Unit = reader.GetString(3),
                                Price = reader.GetDecimal(4),
                                SupplierName = reader.GetString(5),
                                ManufacturerName = reader.GetString(6),
                                CategoryName = reader.GetString(7),
                                DiscountPercent = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8),
                                QuantityInStock = reader.GetInt32(9),
                                Description = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                ImagePath = reader.IsDBNull(11) ? null : reader.GetString(11)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка получения товаров: " + ex.Message);
            }
            return products;
        }
    }
}
