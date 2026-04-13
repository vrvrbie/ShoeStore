using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShoeStoreApp.Models;
using ShoeStoreApp.Services;
using ShoeStoreApp.Helpers;

namespace ShoeStoreApp.Forms
{
    public partial class ProductCard : UserControl
    {
        private Product product;
        private User currentUser;
        private DatabaseService dbService;

        public event EventHandler ProductDeleted;

        public ProductCard(Product product, User user)
        {
            this.product = product;
            this.currentUser = user;
            dbService = new DatabaseService();
            InitializeComponent();

            this.Size = new Size(900, 120);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Margin = new Padding(5);

            PictureBox pbImage = new PictureBox() { Location = new Point(5, 5), Size = new Size(100, 100), SizeMode = PictureBoxSizeMode.Zoom };
            string placeholderPath = Path.Combine(Application.StartupPath, "Resources", "picture.png");
            if (!string.IsNullOrEmpty(product.ImagePath) && File.Exists(product.ImagePath))
            {
                try { pbImage.Image = Image.FromFile(product.ImagePath); }
                catch { if (File.Exists(placeholderPath)) pbImage.Image = Image.FromFile(placeholderPath); }
            }
            else if (File.Exists(placeholderPath)) pbImage.Image = Image.FromFile(placeholderPath);

            Label lblName = new Label() { Text = product.Name, Location = new Point(115, 5), Size = new Size(280, 25), Font = new Font("Times New Roman", 10, FontStyle.Bold) };
            Label lblCategory = new Label() { Text = $"📁 {product.CategoryName}", Location = new Point(115, 30), Size = new Size(280, 20), Font = new Font("Times New Roman", 8) };
            Label lblManufacturer = new Label() { Text = $"🏭 {product.ManufacturerName}", Location = new Point(115, 50), Size = new Size(280, 20), Font = new Font("Times New Roman", 8) };
            Label lblSupplier = new Label() { Text = $"🚚 {product.SupplierName}", Location = new Point(115, 70), Size = new Size(280, 20), Font = new Font("Times New Roman", 8) };

            string desc = product.Description ?? "";
            if (desc.Length > 55) desc = desc.Substring(0, 52) + "...";
            Label lblDescription = new Label() { Text = desc, Location = new Point(115, 90), Size = new Size(280, 25), Font = new Font("Times New Roman", 7, FontStyle.Italic) };

            Label lblStock = new Label() { Text = $"📊 На складе: {product.QuantityInStock} {product.Unit}", Location = new Point(420, 50), Size = new Size(180, 25), Font = new Font("Times New Roman", 9) };
            Label lblDiscount = new Label() { Text = product.DiscountPercent > 0 ? $"🎯 Скидка: {product.DiscountPercent}%" : "", Location = new Point(420, 75), Size = new Size(180, 25), Font = new Font("Times New Roman", 9), ForeColor = Color.DarkGreen };

            if (product.DiscountPercent > 0)
            {
                Label lblOldPrice = new Label() { Text = $"{product.Price:F2} руб.", Location = new Point(420, 5), Size = new Size(120, 20), Font = new Font("Times New Roman", 9, FontStyle.Strikeout), ForeColor = Color.Red };
                Label lblNewPrice = new Label() { Text = $"{product.FinalPrice:F2} руб.", Location = new Point(420, 25), Size = new Size(120, 20), Font = new Font("Times New Roman", 11, FontStyle.Bold), ForeColor = Color.Black };
                this.Controls.Add(lblOldPrice);
                this.Controls.Add(lblNewPrice);
            }
            else
            {
                Label lblPrice = new Label() { Text = $"{product.Price:F2} руб.", Location = new Point(420, 15), Size = new Size(150, 30), Font = new Font("Times New Roman", 11, FontStyle.Bold), ForeColor = Color.Black };
                this.Controls.Add(lblPrice);
            }

            this.Controls.Add(pbImage);
            this.Controls.Add(lblName);
            this.Controls.Add(lblCategory);
            this.Controls.Add(lblManufacturer);
            this.Controls.Add(lblSupplier);
            this.Controls.Add(lblDescription);
            this.Controls.Add(lblStock);
            this.Controls.Add(lblDiscount);

            if (currentUser.RoleID == 4)
            {
                Button btnEdit = new Button()
                {
                    Text = "✏️",
                    Location = new Point(620, 10),
                    Size = new Size(50, 40),
                    BackColor = Color.LightYellow,
                    Font = new Font("Segoe UI", 12)
                };
                btnEdit.Click += (s, e) =>
                {
                    var productForm = new ProductForm(product);
                    if (productForm.ShowDialog() == DialogResult.OK)
                    {
                        ProductDeleted?.Invoke(this, EventArgs.Empty);
                    }
                };

                Button btnDelete = new Button()
                {
                    Text = "🗑️",
                    Location = new Point(680, 10),
                    Size = new Size(50, 40),
                    BackColor = Color.LightCoral,
                    Font = new Font("Segoe UI", 12)
                };
                btnDelete.Click += (s, e) =>
                {
                    if (MessageHelper.ShowQuestion($"Удалить товар \"{product.Name}\"?") == DialogResult.Yes)
                    {
                        if (dbService.DeleteProduct(product.ProductID))
                        {
                            MessageHelper.ShowInfo("Товар удалён");
                            ProductDeleted?.Invoke(this, EventArgs.Empty);
                        }
                    }
                };

                this.Controls.Add(btnEdit);
                this.Controls.Add(btnDelete);
            }

            if (product.IsHighDiscount)
            {
                this.BackColor = Color.FromArgb(46, 139, 87);
                foreach (Control c in this.Controls)
                    if (c is Label) c.ForeColor = Color.White;
                lblDiscount.ForeColor = Color.Yellow;
            }
            else if (!product.IsInStock) this.BackColor = Color.LightBlue;
            else this.BackColor = Color.White;
        }
        
    }
}
