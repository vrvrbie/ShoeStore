using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShoeStoreApp.Services;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Forms
{
    public partial class LoginForm : Form
    {
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnGuest;
        private DatabaseService dbService;

        public LoginForm()
        {
            dbService = new DatabaseService();
            InitializeComponent();

            this.Font = new Font("Times New Roman", 10);

            string iconPath = @"C:\Users\варёк\source\repos\ShoeStoreApp\ShoeStoreApp\Resources\Icon.ico";
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }

            this.Text = "Авторизация - Магазин обуви";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White; 

            Label lblTitle = new Label()
            {
                Text = "Добро пожаловать!",
                Font = new Font("Times New Roman", 16, FontStyle.Bold),
                Location = new Point(120, 20),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblLoginLabel = new Label()
            {
                Text = "Логин:",
                Location = new Point(50, 80),
                Size = new Size(80, 25),
                Font = new Font("Times New Roman", 10)
            };

            txtLogin = new TextBox()
            {
                Location = new Point(140, 80),
                Size = new Size(180, 25),
                Font = new Font("Times New Roman", 10)
            };

            Label lblPasswordLabel = new Label()
            {
                Text = "Пароль:",
                Location = new Point(50, 120),
                Size = new Size(80, 25),
                Font = new Font("Times New Roman", 10)
            };

            txtPassword = new TextBox()
            {
                Location = new Point(140, 120),
                Size = new Size(180, 25),
                PasswordChar = '*',
                Font = new Font("Times New Roman", 10)
            };

            btnLogin = new Button()
            {
                Text = "Войти",
                Location = new Point(100, 170),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(0, 250, 154),  
                Font = new Font("Times New Roman", 10)
            };
            btnLogin.Click += BtnLogin_Click;

            btnGuest = new Button()
            {
                Text = "Войти как гость",
                Location = new Point(200, 170),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(127, 255, 0), 
                Font = new Font("Times New Roman", 10)
            };
            btnGuest.Click += BtnGuest_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblLoginLabel);
            this.Controls.Add(txtLogin);
            this.Controls.Add(lblPasswordLabel);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnGuest);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            User user = dbService.Authenticate(login, password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Hide();
            MainForm mainForm = new MainForm(user);
            mainForm.ShowDialog();
            this.Show();
            txtLogin.Clear();
            txtPassword.Clear();
        }

        private void BtnGuest_Click(object sender, EventArgs e)
        {
            User guest = new User
            {
                UserID = 0,
                Login = "guest",
                FullName = "Гость",
                RoleID = 1,
                RoleName = "Guest"
            };

            this.Hide();
            MainForm mainForm = new MainForm(guest);
            mainForm.ShowDialog();
            this.Show();
        }
    }
}
