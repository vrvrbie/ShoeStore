using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShoeStoreApp.Models;
using ShoeStoreApp.Services;
using ShoeStoreApp.Helpers;

namespace ShoeStoreApp.Forms
{
    public partial class ProductForm : Form
    {
        private DatabaseService dbService;
        private Product editingProduct;
        private string selectedImagePath;

        private TextBox txtArticle, txtName, txtPrice, txtUnit, txtQuantity, txtDiscount, txtDescription;
        private ComboBox cbCategory, cbManufacturer, cbSupplier;
        private PictureBox pbImage;

        public ProductForm(Product product = null)
        {
            dbService = new DatabaseService();
            editingProduct = product;

            this.Text = editingProduct == null ? "➕ Добавление товара" : "✏️ Редактирование товара";
            this.Size = new Size(550, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Times New Roman", 10);
            this.BackColor = Color.White;

            CreateControls();
            LoadComboBoxes();

            if (editingProduct != null)
            {
                LoadProductData();
            }
        }

        private void CreateControls()
        {
            int y = 15;
            int left = 130;
            int labelWidth = 100;
            int controlWidth = 350;

            if (editingProduct != null)
            {
                Label lblId = new Label()
                {
                    Text = "ID товара:",
                    Location = new Point(15, y),
                    Size = new Size(labelWidth, 25),
                    Font = new Font("Times New Roman", 10, FontStyle.Bold)
                };
                Label lblIdValue = new Label()
                {
                    Text = editingProduct.ProductID.ToString(),
                    Location = new Point(left, y),
                    Size = new Size(100, 25),
                    Font = new Font("Times New Roman", 10),
                    BackColor = Color.LightGray
                };
                this.Controls.Add(lblId);
                this.Controls.Add(lblIdValue);
                y += 35;
            }

            Label lblImage = new Label() { Text = "Фото:", Location = new Point(15, y), Size = new Size(labelWidth, 25) };
            pbImage = new PictureBox() { Location = new Point(left, y), Size = new Size(100, 100), SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
            Button btnLoadImage = new Button() { Text = "Загрузить фото", Location = new Point(left + 110, y + 35), Size = new Size(120, 30) };
            btnLoadImage.Click += BtnLoadImage_Click;
            y += 115;

            AddLabelAndTextBox("Артикул:", ref y, left, controlWidth, out txtArticle);

            AddLabelAndTextBox("Наименование:*", ref y, left, controlWidth, out txtName);

            AddLabelAndComboBox("Категория:*", ref y, left, controlWidth, out cbCategory);

            AddLabelAndComboBox("Производитель:*", ref y, left, controlWidth, out cbManufacturer);

            AddLabelAndComboBox("Поставщик:*", ref y, left, controlWidth, out cbSupplier);

            AddLabelAndTextBox("Цена:*", ref y, left, controlWidth, out txtPrice);

            AddLabelAndTextBox("Ед. измерения:", ref y, left, controlWidth, out txtUnit);
            txtUnit.Text = "шт";

            AddLabelAndTextBox("Количество:*", ref y, left, controlWidth, out txtQuantity);

            AddLabelAndTextBox("Скидка (%):", ref y, left, controlWidth, out txtDiscount);
            txtDiscount.Text = "0";

            Label lblDescription = new Label() { Text = "Описание:", Location = new Point(15, y), Size = new Size(labelWidth, 25) };
            txtDescription = new TextBox() { Location = new Point(left, y), Size = new Size(controlWidth, 60), Multiline = true };
            y += 75;

            Button btnSave = new Button() { Text = "Сохранить", Location = new Point(150, y), Size = new Size(100, 35), BackColor = Color.FromArgb(0, 250, 154) };
            btnSave.Click += BtnSave_Click;
            Button btnCancel = new Button() { Text = "Отмена", Location = new Point(270, y), Size = new Size(100, 35), BackColor = Color.LightGray };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(lblImage);
            this.Controls.Add(pbImage);
            this.Controls.Add(btnLoadImage);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void AddLabelAndTextBox(string labelText, ref int y, int left, int controlWidth, out TextBox textBox)
        {
            Label lbl = new Label() { Text = labelText, Location = new Point(15, y), Size = new Size(100, 25) };
            textBox = new TextBox() { Location = new Point(130, y), Size = new Size(controlWidth, 25) };
            this.Controls.Add(lbl);
            this.Controls.Add(textBox);
            y += 35;
        }

        private void AddLabelAndComboBox(string labelText, ref int y, int left, int controlWidth, out ComboBox comboBox)
        {
            Label lbl = new Label() { Text = labelText, Location = new Point(15, y), Size = new Size(100, 25) };
            comboBox = new ComboBox() { Location = new Point(130, y), Size = new Size(controlWidth, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(lbl);
            this.Controls.Add(comboBox);
            y += 35;
        }

        private void LoadComboBoxes()
        {
            cbCategory.DisplayMember = "CategoryName";
            cbCategory.ValueMember = "CategoryID";
            cbCategory.DataSource = dbService.GetCategories();
            cbCategory.SelectedIndex = -1;

            cbManufacturer.DisplayMember = "ManufacturerName";
            cbManufacturer.ValueMember = "ManufacturerID";
            cbManufacturer.DataSource = dbService.GetManufacturers();
            cbManufacturer.SelectedIndex = -1;

            cbSupplier.DisplayMember = "SupplierName";
            cbSupplier.ValueMember = "SupplierID";
            cbSupplier.DataSource = dbService.GetSuppliers();
            cbSupplier.SelectedIndex = -1;
        }

        private void LoadProductData()
        {
            txtArticle.Text = editingProduct.Article;
            txtName.Text = editingProduct.Name;
            txtPrice.Text = editingProduct.Price.ToString("F2");
            txtUnit.Text = editingProduct.Unit;
            txtQuantity.Text = editingProduct.QuantityInStock.ToString();
            txtDiscount.Text = editingProduct.DiscountPercent.ToString();
            txtDescription.Text = editingProduct.Description;

            for (int i = 0; i < cbCategory.Items.Count; i++)
            {
                dynamic item = cbCategory.Items[i];
                if (item.CategoryName == editingProduct.CategoryName)
                {
                    cbCategory.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < cbManufacturer.Items.Count; i++)
            {
                dynamic item = cbManufacturer.Items[i];
                if (item.ManufacturerName == editingProduct.ManufacturerName)
                {
                    cbManufacturer.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < cbSupplier.Items.Count; i++)
            {
                dynamic item = cbSupplier.Items[i];
                if (item.SupplierName == editingProduct.SupplierName)
                {
                    cbSupplier.SelectedIndex = i;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(editingProduct.ImagePath) && File.Exists(editingProduct.ImagePath))
            {
                try
                {
                    pbImage.Image = Image.FromFile(editingProduct.ImagePath);
                    selectedImagePath = editingProduct.ImagePath;
                }
                catch { }
            }
        }

        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Image original = Image.FromFile(ofd.FileName);
                    Image resized = ResizeImage(original, 300, 200);
                    string fileName = Guid.NewGuid().ToString() + ".jpg";
                    string savePath = Path.Combine(Application.StartupPath, "ProductImages", fileName);
                    if (!Directory.Exists(Path.Combine(Application.StartupPath, "ProductImages")))
                        Directory.CreateDirectory(Path.Combine(Application.StartupPath, "ProductImages"));
                    resized.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    pbImage.Image = resized;
                    selectedImagePath = savePath;
                    original.Dispose();
                    resized.Dispose();
                    MessageHelper.ShowInfo("Изображение загружено");
                }
            }
        }

        private Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            double ratio = Math.Min((double)maxWidth / image.Width, (double)maxHeight / image.Height);
            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(newImage))
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageHelper.ShowWarning("Введите наименование товара!");
                return;
            }
            if (cbCategory.SelectedItem == null)
            {
                MessageHelper.ShowWarning("Выберите категорию!");
                return;
            }
            if (cbManufacturer.SelectedItem == null)
            {
                MessageHelper.ShowWarning("Выберите производителя!");
                return;
            }
            if (cbSupplier.SelectedItem == null)
            {
                MessageHelper.ShowWarning("Выберите поставщика!");
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageHelper.ShowWarning("Введите корректную цену!");
                return;
            }
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageHelper.ShowWarning("Введите корректное количество!");
                return;
            }

            decimal discount = 0;
            if (!string.IsNullOrWhiteSpace(txtDiscount.Text))
            {
                if (!decimal.TryParse(txtDiscount.Text, out discount) || discount < 0 || discount > 100)
                {
                    MessageHelper.ShowWarning("Скидка должна быть от 0 до 100!");
                    return;
                }
            }

            dynamic selectedCategory = cbCategory.SelectedItem;
            dynamic selectedManufacturer = cbManufacturer.SelectedItem;
            dynamic selectedSupplier = cbSupplier.SelectedItem;

            int categoryId = selectedCategory.CategoryID;
            int manufacturerId = selectedManufacturer.ManufacturerID;
            int supplierId = selectedSupplier.SupplierID;

            bool success;
            if (editingProduct == null)
            {
                success = dbService.AddProduct(
                    txtArticle.Text.Trim(),
                    txtName.Text.Trim(),
                    txtUnit.Text.Trim(),
                    price,
                    supplierId,
                    manufacturerId,
                    categoryId,
                    discount,
                    quantity,
                    txtDescription.Text.Trim(),
                    selectedImagePath
                );
                if (success) MessageHelper.ShowInfo("Товар успешно добавлен!");
            }
            else
            {
                success = dbService.UpdateProduct(
                    editingProduct.ProductID,
                    txtArticle.Text.Trim(),
                    txtName.Text.Trim(),
                    txtUnit.Text.Trim(),
                    price,
                    supplierId,
                    manufacturerId,
                    categoryId,
                    discount,
                    quantity,
                    txtDescription.Text.Trim(),
                    selectedImagePath
                );
                if (success) MessageHelper.ShowInfo("Товар успешно обновлён!");
            }

            if (success)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageHelper.ShowError("Ошибка при сохранении товара!");
            }
        }
    }
}
