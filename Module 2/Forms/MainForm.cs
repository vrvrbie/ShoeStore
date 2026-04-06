using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ShoeStoreApp.Models;
using ShoeStoreApp.Services;

namespace ShoeStoreApp.Forms
{
    public partial class MainForm : Form
    {
        private FlowLayoutPanel flowProducts;
        private Label lblUserInfo;
        private Button btnLogout;
        private User currentUser;
        private DatabaseService dbService;

        public MainForm(User user)
        {
            currentUser = user;
            dbService = new DatabaseService();
            InitializeComponent();

            this.Font = new Font("Times New Roman", 10);

            string iconPath = @"C:\Users\варёк\source\repos\ShoeStoreApp\ShoeStoreApp\Resources\Icon.ico";
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }

            this.Text = "Магазин обуви";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            Panel topPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(127, 255, 0) 
            };

            string logoPath = Path.Combine(Application.StartupPath, "Resources", "logo.png");
            if (File.Exists(logoPath))
            {
                PictureBox logo = new PictureBox()
                {
                    Image = Image.FromFile(logoPath),
                    Size = new Size(100, 35),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Location = new Point(600, 3)
                };
                topPanel.Controls.Add(logo);
            }

            lblUserInfo = new Label()
            {
                Text = $"👤 {currentUser.FullName} | Роль: {GetRoleName()}",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Times New Roman", 10, FontStyle.Bold)
            };

            btnLogout = new Button()
            {
                Text = "Выход",
                Location = new Point(700, 8),
                Size = new Size(80, 25),
                BackColor = Color.FromArgb(0, 250, 154),
                Font = new Font("Times New Roman", 9)
            };
            btnLogout.Click += (s, e) =>
            {
                DialogResult result = MessageBox.Show(
                    "Вы уверены, что хотите выйти из аккаунта?",
                    "Подтверждение выхода",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    this.Close();
                }
            };

            topPanel.Controls.Add(lblUserInfo);
            topPanel.Controls.Add(btnLogout);

            Panel scrollPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White  
            };

            flowProducts = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White
            };

            scrollPanel.Controls.Add(flowProducts);

            this.Controls.Add(scrollPanel);
            this.Controls.Add(topPanel);

            LoadProducts();
        }

        private void LoadProducts()
        {
            flowProducts.Controls.Clear();
            List<Product> products = dbService.GetAllProducts();

            foreach (var product in products)
            {
                ProductCard card = new ProductCard(product);
                flowProducts.Controls.Add(card);
            }

            lblUserInfo.Text = $"👤 {currentUser.FullName} | Роль: {GetRoleName()}";
        }

        private string GetRoleName()
        {
            switch (currentUser.RoleID)
            {
                case 1: return "Гость";
                case 2: return "Клиент";
                case 3: return "Менеджер";
                case 4: return "Администратор";
                default: return currentUser.RoleName ?? "Неизвестно";
            }
        }
    }
}
