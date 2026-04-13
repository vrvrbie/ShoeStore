using Npgsql;
using ShoeStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


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
        // Добавьте эти методы в конец класса DatabaseService

        public List<dynamic> GetCategories()
        {
            var list = new List<dynamic>();
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new { CategoryID = reader.GetInt32(0), CategoryName = reader.GetString(1) });
                    }
                }
            }
            return list;
        }

        public List<dynamic> GetManufacturers()
        {
            var list = new List<dynamic>();
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "SELECT ManufacturerID, ManufacturerName FROM Manufacturers ORDER BY ManufacturerName";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new { ManufacturerID = reader.GetInt32(0), ManufacturerName = reader.GetString(1) });
                    }
                }
            }
            return list;
        }

        public List<dynamic> GetSuppliers()
        {
            var list = new List<dynamic>();
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "SELECT SupplierID, SupplierName FROM Suppliers ORDER BY SupplierName";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new { SupplierID = reader.GetInt32(0), SupplierName = reader.GetString(1) });
                    }
                }
            }
            return list;
        }

        public bool AddProduct(string article, string name, string unit, decimal price, int supplierId, int manufacturerId, int categoryId, decimal discount, int quantity, string description, string imagePath)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Products (ProductArticle, ProductName, ProductUnit, ProductPrice, SupplierID, ManufacturerID, CategoryID, DiscountPercent, QuantityInStock, Description, ImagePath)
                       VALUES (@article, @name, @unit, @price, @supplierId, @manufacturerId, @categoryId, @discount, @quantity, @description, @imagePath)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@article", article);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@unit", unit);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@supplierId", supplierId);
                    cmd.Parameters.AddWithValue("@manufacturerId", manufacturerId);
                    cmd.Parameters.AddWithValue("@categoryId", categoryId);
                    cmd.Parameters.AddWithValue("@discount", discount);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@description", description ?? "");
                    cmd.Parameters.AddWithValue("@imagePath", imagePath ?? "");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateProduct(int productId, string article, string name, string unit, decimal price, int supplierId, int manufacturerId, int categoryId, decimal discount, int quantity, string description, string imagePath)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"UPDATE Products SET 
                       ProductArticle=@article, ProductName=@name, ProductUnit=@unit, ProductPrice=@price,
                       SupplierID=@supplierId, ManufacturerID=@manufacturerId, CategoryID=@categoryId,
                       DiscountPercent=@discount, QuantityInStock=@quantity, Description=@description, ImagePath=@imagePath
                       WHERE ProductID=@productId";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@article", article);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@unit", unit);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@supplierId", supplierId);
                    cmd.Parameters.AddWithValue("@manufacturerId", manufacturerId);
                    cmd.Parameters.AddWithValue("@categoryId", categoryId);
                    cmd.Parameters.AddWithValue("@discount", discount);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@description", description ?? "");
                    cmd.Parameters.AddWithValue("@imagePath", imagePath ?? "");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteProduct(int productId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                // Проверка наличия в заказах
                string checkSql = "SELECT COUNT(*) FROM OrderDetails WHERE ProductID = @productId";
                using (var checkCmd = new NpgsqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@productId", productId);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("Нельзя удалить товар, который присутствует в заказах!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                string deleteSql = "DELETE FROM Products WHERE ProductID = @productId";
                using (var deleteCmd = new NpgsqlCommand(deleteSql, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@productId", productId);
                    return deleteCmd.ExecuteNonQuery() > 0;
                }
            }
        }
        // ========== МОДУЛЬ 4: РАБОТА С ЗАКАЗАМИ ==========

        public List<dynamic> GetAllOrders()
        {
            var list = new List<dynamic>();
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"
            SELECT o.OrderID, o.OrderNumber, o.OrderDate, o.DeliveryDate, 
                   p.Address as PickupPointAddress, u.UserFullName, s.StatusName
            FROM Orders o
            JOIN PickupPoints p ON o.PickupPointID = p.PointID
            JOIN Users u ON o.UserID = u.UserID
            JOIN OrderStatuses s ON o.OrderStatusID = s.StatusID
            ORDER BY o.OrderDate DESC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            OrderID = reader.GetInt32(0),
                            OrderNumber = reader.GetInt32(1),
                            OrderDate = reader.GetDateTime(2),
                            DeliveryDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            PickupPointAddress = reader.GetString(4),
                            UserFullName = reader.GetString(5),
                            StatusName = reader.GetString(6)
                        });
                    }
                }
            }
            return list;
        }

        public List<dynamic> GetOrderStatuses()
        {
            var list = new List<dynamic>();
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT StatusID, StatusName FROM OrderStatuses", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(new { StatusID = reader.GetInt32(0), StatusName = reader.GetString(1) });
                }
            }
            return list;
        }

        public List<dynamic> GetPickupPoints()
        {
            var list = new List<dynamic>();
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT PointID, Address FROM PickupPoints", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(new { PointID = reader.GetInt32(0), Address = reader.GetString(1) });
                }
            }
            return list;
        }

        public dynamic GetOrderById(int orderId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "SELECT OrderID, OrderNumber, OrderDate, DeliveryDate, PickupPointID, OrderStatusID FROM Orders WHERE OrderID = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return new
                            {
                                OrderID = reader.GetInt32(0),
                                OrderNumber = reader.GetInt32(1),
                                OrderDate = reader.GetDateTime(2),
                                DeliveryDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                PointID = reader.GetInt32(4),
                                StatusID = reader.GetInt32(5)
                            };
                    }
                }
            }
            return null;
        }

        public bool AddOrder(int orderNumber, DateTime orderDate, DateTime? deliveryDate, int pointId, int statusId, int userId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointID, UserID, OrderStatusID, OrderPickupCode)
                       VALUES (@num, @orderDate, @deliveryDate, @pointId, @userId, @statusId, @code)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@num", orderNumber);
                    cmd.Parameters.AddWithValue("@orderDate", orderDate);
                    cmd.Parameters.AddWithValue("@deliveryDate", deliveryDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@pointId", pointId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@statusId", statusId);
                    cmd.Parameters.AddWithValue("@code", new Random().Next(100, 999));
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateOrder(int orderId, int orderNumber, DateTime orderDate, DateTime? deliveryDate, int pointId, int statusId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"UPDATE Orders SET OrderNumber=@num, OrderDate=@orderDate, DeliveryDate=@deliveryDate, PickupPointID=@pointId, OrderStatusID=@statusId WHERE OrderID=@id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    cmd.Parameters.AddWithValue("@num", orderNumber);
                    cmd.Parameters.AddWithValue("@orderDate", orderDate);
                    cmd.Parameters.AddWithValue("@deliveryDate", deliveryDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@pointId", pointId);
                    cmd.Parameters.AddWithValue("@statusId", statusId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteOrder(int orderId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM Orders WHERE OrderID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}