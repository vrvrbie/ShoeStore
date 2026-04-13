using ShoeStoreApp.Helpers;
using ShoeStoreApp.Models;
using ShoeStoreApp.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ShoeStoreApp.Forms
{
    public partial class MainForm : Form
    {
        private FlowLayoutPanel flowProducts;
        private Label lblUserInfo;
        private Button btnLogout;
        private User currentUser;
        private DatabaseService dbService;

        private TextBox txtSearch;
        private ComboBox cbSupplier;
        private ComboBox cbSort;
        private Button btnAddProduct;

        public MainForm(User user)
        {
            currentUser = user;
            dbService = new DatabaseService();

            this.Text = "Магазин обуви";
            this.Size = new Size(950, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Times New Roman", 10);

            string iconPath = @"C:\Users\варёк\source\repos\ShoeStoreApp\ShoeStoreApp\Resources\Icon.ico";
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }

            // ========== ВЕРХНЯЯ ПАНЕЛЬ ==========
            Panel topPanel = new Panel() { Dock = DockStyle.Top, Height = 45, BackColor = Color.FromArgb(127, 255, 0) };

            lblUserInfo = new Label()
            {
                Text = $"👤 {currentUser.FullName} | Роль: {GetRoleName()}",
                Location = new Point(10, 12),
                Size = new Size(500, 25),
                Font = new Font("Times New Roman", 10, FontStyle.Bold)
            };

            btnLogout = new Button()
            {
                Text = "Выход",
                Location = new Point(850, 10),
                Size = new Size(80, 27),
                BackColor = Color.FromArgb(0, 250, 154)
            };
            btnLogout.Click += (s, e) =>
            {
                if (MessageHelper.ShowQuestion("Вы уверены, что хотите выйти?") == DialogResult.Yes)
                    this.Close();
            };

            topPanel.Controls.Add(lblUserInfo);
            topPanel.Controls.Add(btnLogout);

            // Кнопка "Добавить товар" (только админ)
            if (currentUser.RoleID == 4)
            {
                btnAddProduct = new Button()
                {
                    Text = "➕ Добавить товар",
                    Location = new Point(700, 10),
                    Size = new Size(130, 27),
                    BackColor = Color.FromArgb(0, 250, 154)
                };
                btnAddProduct.Click += (s, e) =>
                {
                    var productForm = new ProductForm();
                    if (productForm.ShowDialog() == DialogResult.OK)
                        LoadProducts();
                };
                topPanel.Controls.Add(btnAddProduct);
            }
            // Кнопка "Заказы" (для менеджера и администратора)
            if (currentUser.RoleID == 3 || currentUser.RoleID == 4)
            {
                Button btnOrders = new Button()
                {
                    Text = "📋 Заказы",
                    Location = new Point(550, 10),   // левее кнопки "Добавить товар"
                    Size = new Size(100, 27),
                    BackColor = Color.FromArgb(0, 250, 154),
                    Font = new Font("Times New Roman", 9)
                };
                btnOrders.Click += (s, e) =>
                {
                    var ordersForm = new OrdersForm(currentUser);
                    ordersForm.ShowDialog();
                };
                topPanel.Controls.Add(btnOrders);
            }

            // ========== ПАНЕЛЬ ПОИСКА (только менеджер и админ) ==========
            Panel searchPanel = null;
            if (currentUser.RoleID == 3 || currentUser.RoleID == 4)
            {
                searchPanel = new Panel() { Dock = DockStyle.Top, Height = 45, BackColor = Color.FromArgb(240, 240, 240) };

                Label lblSearch = new Label() { Text = "🔍 Поиск:", Location = new Point(10, 12), Size = new Size(60, 25) };
                txtSearch = new TextBox() { Location = new Point(75, 10), Size = new Size(200, 25) };
                txtSearch.TextChanged += (s, e) => LoadProducts();

                Label lblFilter = new Label() { Text = "Поставщик:", Location = new Point(290, 12), Size = new Size(70, 25) };
                cbSupplier = new ComboBox() { Location = new Point(365, 10), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
                cbSupplier.SelectedIndexChanged += (s, e) => LoadProducts();

                Label lblSort = new Label() { Text = "Сортировка:", Location = new Point(530, 12), Size = new Size(70, 25) };
                cbSort = new ComboBox() { Location = new Point(605, 10), Size = new Size(160, 25), DropDownStyle = ComboBoxStyle.DropDownList };
                cbSort.Items.AddRange(new string[] { "Без сортировки", "По возрастанию (склад)", "По убыванию (склад)" });
                cbSort.SelectedIndex = 0;
                cbSort.SelectedIndexChanged += (s, e) => LoadProducts();

                searchPanel.Controls.Add(lblSearch);
                searchPanel.Controls.Add(txtSearch);
                searchPanel.Controls.Add(lblFilter);
                searchPanel.Controls.Add(cbSupplier);
                searchPanel.Controls.Add(lblSort);
                searchPanel.Controls.Add(cbSort);
            }

            // ========== ПАНЕЛЬ С ТОВАРАМИ ==========
            Panel scrollPanel = new Panel() { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White };
            flowProducts = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, BackColor = Color.White };
            scrollPanel.Controls.Add(flowProducts);

            // ========== ДОБАВЛЯЕМ НА ФОРМУ (СНИЗУ ВВЕРХ) ==========
            this.Controls.Add(scrollPanel);
            if (searchPanel != null) this.Controls.Add(searchPanel);
            this.Controls.Add(topPanel);

            if (cbSupplier != null) LoadSuppliers();
            LoadProducts();
        }

        private void LoadSuppliers()
        {
            if (cbSupplier == null) return;
            cbSupplier.Items.Clear();
            cbSupplier.Items.Add("Все поставщики");
            var products = dbService.GetAllProducts();
            var suppliers = products.Select(p => p.SupplierName).Distinct().OrderBy(s => s);
            foreach (var supplier in suppliers) cbSupplier.Items.Add(supplier);
            cbSupplier.SelectedIndex = 0;
        }

        public void LoadProducts()
        {
            if (flowProducts == null) return;
            flowProducts.Controls.Clear();
            var products = dbService.GetAllProducts();

            if (txtSearch != null && !string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                products = products.Where(p =>
                    p.Name.ToLower().Contains(searchText) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchText)) ||
                    p.CategoryName.ToLower().Contains(searchText) ||
                    p.ManufacturerName.ToLower().Contains(searchText) ||
                    p.SupplierName.ToLower().Contains(searchText)).ToList();
            }

            if (cbSupplier != null && cbSupplier.SelectedItem != null && cbSupplier.SelectedItem.ToString() != "Все поставщики")
            {
                string selectedSupplier = cbSupplier.SelectedItem.ToString();
                products = products.Where(p => p.SupplierName == selectedSupplier).ToList();
            }

            if (cbSort != null)
            {
                if (cbSort.SelectedIndex == 1) products = products.OrderBy(p => p.QuantityInStock).ToList();
                else if (cbSort.SelectedIndex == 2) products = products.OrderByDescending(p => p.QuantityInStock).ToList();
            }

            foreach (var product in products)
            {
                var card = new ProductCard(product, currentUser);
                card.ProductDeleted += (s, e) => LoadProducts();
                flowProducts.Controls.Add(card);
            }

            lblUserInfo.Text = $"👤 {currentUser.FullName} | Роль: {GetRoleName()} | Товаров: {products.Count}";
        }

        private string GetRoleName()
        {
            switch (currentUser.RoleID)
            {
                case 1: return "Гость";
                case 2: return "Клиент";
                case 3: return "Менеджер";
                case 4: return "Администратор";
                default: return "Неизвестно";
            }
        }
    }
}