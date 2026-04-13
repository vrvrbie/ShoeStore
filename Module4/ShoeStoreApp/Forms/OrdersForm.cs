using ShoeStoreApp.Helpers;
using ShoeStoreApp.Models;
using ShoeStoreApp.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShoeStoreApp.Forms
{
    public partial class OrdersForm : Form
    {
        private User currentUser;
        private DatabaseService dbService;
        private DataGridView dgvOrders;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;

        public OrdersForm(User user)
        {
            currentUser = user;
            dbService = new DatabaseService();
            InitializeComponent();
            CreateControls();   
            LoadOrders();
            SetPermissions();
            string iconPath = @"C:\Users\варёк\source\repos\ShoeStoreApp\ShoeStoreApp\Resources\Icon.ico";
            if (System.IO.File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }
        }

        private void CreateControls()
        {
            this.Text = "Управление заказами";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Times New Roman", 10);

            dgvOrders = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false
            };
            dgvOrders.CellDoubleClick += (s, e) => { if (btnEdit.Enabled) btnEdit.PerformClick(); };

            Panel panel = new Panel() { Dock = DockStyle.Bottom, Height = 50 };
            btnAdd = new Button() { Text = "➕ Добавить", Location = new Point(10, 10), Size = new Size(100, 30) };
            btnEdit = new Button() { Text = "✏️ Редактировать", Location = new Point(120, 10), Size = new Size(120, 30) };
            btnDelete = new Button() { Text = "🗑️ Удалить", Location = new Point(250, 10), Size = new Size(100, 30) };
            btnRefresh = new Button() { Text = "🔄 Обновить", Location = new Point(360, 10), Size = new Size(100, 30) };

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => LoadOrders();

            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnEdit);
            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnRefresh);

            this.Controls.Add(dgvOrders);
            this.Controls.Add(panel);
        }

        private void LoadOrders()
        {
            dgvOrders.DataSource = dbService.GetAllOrders();
            if (dgvOrders.Columns.Contains("OrderID")) dgvOrders.Columns["OrderID"].HeaderText = "ID";
            if (dgvOrders.Columns.Contains("OrderNumber")) dgvOrders.Columns["OrderNumber"].HeaderText = "Номер";
            if (dgvOrders.Columns.Contains("OrderDate")) dgvOrders.Columns["OrderDate"].HeaderText = "Дата заказа";
            if (dgvOrders.Columns.Contains("DeliveryDate")) dgvOrders.Columns["DeliveryDate"].HeaderText = "Дата выдачи";
            if (dgvOrders.Columns.Contains("PickupPointAddress")) dgvOrders.Columns["PickupPointAddress"].HeaderText = "Пункт выдачи";
            if (dgvOrders.Columns.Contains("UserFullName")) dgvOrders.Columns["UserFullName"].HeaderText = "Клиент";
            if (dgvOrders.Columns.Contains("StatusName")) dgvOrders.Columns["StatusName"].HeaderText = "Статус";
        }

        private void SetPermissions()
        {
            bool canEdit = (currentUser.RoleID == 4);
            btnAdd.Enabled = canEdit;
            btnEdit.Enabled = canEdit;
            btnDelete.Enabled = canEdit;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new OrderForm(currentUser);
            if (form.ShowDialog() == DialogResult.OK) LoadOrders();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow == null) return;
            int orderId = (int)dgvOrders.CurrentRow.Cells["OrderID"].Value;
            var form = new OrderForm(currentUser, orderId);
            if (form.ShowDialog() == DialogResult.OK) LoadOrders();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow == null) return;
            int orderId = (int)dgvOrders.CurrentRow.Cells["OrderID"].Value;
            if (MessageHelper.ShowQuestion($"Удалить заказ №{orderId}?") == DialogResult.Yes)
            {
                if (dbService.DeleteOrder(orderId))
                {
                    MessageHelper.ShowInfo("Заказ удалён");
                    LoadOrders();
                }
            }
        }
    }
}
