using ShoeStoreApp.Helpers;
using ShoeStoreApp.Models;
using ShoeStoreApp.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShoeStoreApp.Forms
{
    public partial class OrderForm : Form
    {
        private DatabaseService dbService;
        private User currentUser;
        private int? editingOrderId;
        private TextBox txtOrderNumber;
        private ComboBox cbStatus, cbPickupPoint;
        private DateTimePicker dtOrderDate, dtDeliveryDate;

        public OrderForm(User user, int? orderId = null)
        {
            currentUser = user;
            dbService = new DatabaseService();
            editingOrderId = orderId;
            InitializeComponent();
            CreateControls();
            LoadComboBoxes();
            if (editingOrderId.HasValue) LoadOrderData();
            string iconPath = @"C:\Users\варёк\source\repos\ShoeStoreApp\ShoeStoreApp\Resources\Icon.ico";
            if (System.IO.File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }
        }

        private void CreateControls()
        {
            this.Text = editingOrderId.HasValue ? "✏️ Редактирование заказа" : "➕ Новый заказ";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Times New Roman", 10);
            this.BackColor = Color.White;

            int y = 20;
            int left = 130;

            Label lblNumber = new Label() { Text = "Номер заказа:", Location = new Point(15, y), Size = new Size(100, 25) };
            txtOrderNumber = new TextBox() { Location = new Point(left, y), Size = new Size(250, 25) };
            this.Controls.Add(lblNumber);
            this.Controls.Add(txtOrderNumber);
            y += 35;

            Label lblStatus = new Label() { Text = "Статус:", Location = new Point(15, y), Size = new Size(100, 25) };
            cbStatus = new ComboBox() { Location = new Point(left, y), Size = new Size(250, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(lblStatus);
            this.Controls.Add(cbStatus);
            y += 35;

            Label lblPoint = new Label() { Text = "Пункт выдачи:", Location = new Point(15, y), Size = new Size(100, 25) };
            cbPickupPoint = new ComboBox() { Location = new Point(left, y), Size = new Size(250, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(lblPoint);
            this.Controls.Add(cbPickupPoint);
            y += 35;

            Label lblOrderDate = new Label() { Text = "Дата заказа:", Location = new Point(15, y), Size = new Size(100, 25) };
            dtOrderDate = new DateTimePicker() { Location = new Point(left, y), Size = new Size(250, 25), Value = DateTime.Today };
            this.Controls.Add(lblOrderDate);
            this.Controls.Add(dtOrderDate);
            y += 35;

            Label lblDeliveryDate = new Label() { Text = "Дата выдачи:", Location = new Point(15, y), Size = new Size(100, 25) };
            dtDeliveryDate = new DateTimePicker() { Location = new Point(left, y), Size = new Size(250, 25), Value = DateTime.Today };
            this.Controls.Add(lblDeliveryDate);
            this.Controls.Add(dtDeliveryDate);
            y += 50;
            
            Button btnSave = new Button() { Text = "Сохранить", Location = new Point(110, y), Size = new Size(100, 35), BackColor = Color.FromArgb(0, 250, 154) };
            btnSave.Click += BtnSave_Click;
            Button btnCancel = new Button() { Text = "Отмена", Location = new Point(230, y), Size = new Size(100, 35), BackColor = Color.LightGray };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void LoadComboBoxes()
        {
            cbStatus.DataSource = dbService.GetOrderStatuses();
            cbStatus.DisplayMember = "StatusName";
            cbStatus.ValueMember = "StatusID";

            cbPickupPoint.DataSource = dbService.GetPickupPoints();
            cbPickupPoint.DisplayMember = "Address";
            cbPickupPoint.ValueMember = "PointID";
        }

        private void LoadOrderData()
        {
            var order = dbService.GetOrderById(editingOrderId.Value);
            if (order != null)
            {
                txtOrderNumber.Text = order.OrderNumber.ToString();
                dtOrderDate.Value = order.OrderDate;

                if (order.DeliveryDate != DateTime.MinValue)
                    dtDeliveryDate.Value = order.DeliveryDate;
                cbStatus.SelectedValue = order.StatusID;
                cbPickupPoint.SelectedValue = order.PointID;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtOrderNumber.Text, out int orderNumber))
            {
                MessageHelper.ShowWarning("Введите номер заказа!");
                return;
            }

            if (cbStatus.SelectedItem == null || cbPickupPoint.SelectedItem == null)
            {
                MessageHelper.ShowWarning("Заполните все поля!");
                return;
            }

            int statusId = (int)cbStatus.SelectedValue;
            int pointId = (int)cbPickupPoint.SelectedValue;
            DateTime orderDate = dtOrderDate.Value;
            DateTime? deliveryDate = dtDeliveryDate.Value;

            bool success;
            if (editingOrderId.HasValue)
                success = dbService.UpdateOrder(editingOrderId.Value, orderNumber, orderDate, deliveryDate, pointId, statusId);
            else
                success = dbService.AddOrder(orderNumber, orderDate, deliveryDate, pointId, statusId, currentUser.UserID);

            if (success)
            {
                MessageHelper.ShowInfo("Сохранено!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageHelper.ShowError("Ошибка сохранения!");
            }
        }
    }
}
