using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using Microsoft.VisualBasic;

namespace TerbaruCahyaFy
{
    public partial class Admin : Form
    {
        private SQLiteConnection connection;
        private bool isTextChanging = false;

        public Admin()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            LoadData();
            LoadData2();

            textBoxID.TextChanged += textBoxID_TextChanged;
            textBoxBarcode.TextChanged += textBoxBarcode_TextChanged;
            textBoxNamaItem.TextChanged += textBoxNamaItem_TextChanged;

            buttonRestockItem.Click += buttonRestockItem_Click;
            buttonTambahItem.Click += buttonTambahItem_Click;
            buttonEditItem.Click += buttonEditItem_Click;
            buttonHapusItem.Click += buttonHapusItem_Click;

            SetupAutoCompleteForID();
            SetupAutoCompleteForBarcode();
            SetupAutoCompleteForNamaItem();

            textBoxUsername.TextChanged += textBoxUsername_TextChanged;

            buttonTambahUser.Click += buttonTambahUser_Click;
            buttonEditUser.Click += buttonEditUser_Click;
            buttonHapusUser.Click += buttonHapusUser_Click;

            SetupAutoCompleteForUsername();

            toolStripButtonUserItem1.Click += new EventHandler(toolStripButtonUserItem1_Click);
            toolStripButtonUserUser2.Click += new EventHandler(toolStripButtonUserUser2_Click);
            toolStripButtonItemItem1.Click += new EventHandler(toolStripButtonItemItem1_Click);
            toolStripButtonItemUser2.Click += new EventHandler(toolStripButtonItemUser2_Click);
            toolStripButtonUserKeluar3.Click += new EventHandler(toolStripButtonUserKeluar3_Click);
            toolStripButtonItemKeluar3.Click += new EventHandler(toolStripButtonItemKeluar3_Click);
        }

        private void InitializeDatabaseConnection()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data Barang.db");
            connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            try
            {
                connection.Open();
                MessageBox.Show("Connection Successful");
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void LoadData()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT * FROM Items";
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn))
                {
                    DataTable dataTable = new DataTable();
                    try
                    {
                        conn.Open();
                        adapter.Fill(dataTable);
                        dataGridViewItem.DataSource = dataTable;
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void LoadData2()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT * FROM User";
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn))
                {
                    DataTable dataTable = new DataTable();
                    try
                    {
                        conn.Open();
                        adapter.Fill(dataTable);
                        dataGridViewUser.DataSource = dataTable;
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void UpdateDataGridViewWithSearchResults2()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT * FROM User WHERE 1=1";

                if (!string.IsNullOrEmpty(textBoxUsername.Text))
                {
                    query += " AND Username LIKE @Username";
                }

                SQLiteCommand command = new SQLiteCommand(query, conn);
                if (!string.IsNullOrEmpty(textBoxUsername.Text))
                {
                    command.Parameters.AddWithValue("@Username", "%" + textBoxUsername.Text + "%");
                }

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridViewUser.DataSource = dataTable;
            }
        }

        private void UpdateDataGridViewWithSearchResults()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT * FROM Items WHERE 1=1";

                if (!string.IsNullOrEmpty(textBoxID.Text))
                {
                    query += " AND ID LIKE @ID";
                }
                if (!string.IsNullOrEmpty(textBoxBarcode.Text))
                {
                    query += " AND Barcode LIKE @Barcode";
                }
                if (!string.IsNullOrEmpty(textBoxNamaItem.Text))
                {
                    query += " AND NamaItem LIKE @NamaItem";
                }

                SQLiteCommand command = new SQLiteCommand(query, conn);
                if (!string.IsNullOrEmpty(textBoxID.Text))
                {
                    command.Parameters.AddWithValue("@ID", "%" + textBoxID.Text + "%");
                }
                if (!string.IsNullOrEmpty(textBoxBarcode.Text))
                {
                    command.Parameters.AddWithValue("@Barcode", "%" + textBoxBarcode.Text + "%");
                }
                if (!string.IsNullOrEmpty(textBoxNamaItem.Text))
                {
                    command.Parameters.AddWithValue("@NamaItem", "%" + textBoxNamaItem.Text + "%");
                }

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridViewItem.DataSource = dataTable;
            }
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            if (isTextChanging) return;
            isTextChanging = true;

            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT * FROM User WHERE Username = @value COLLATE NOCASE";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                command.Parameters.AddWithValue("@value", textBoxUsername.Text);
                conn.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        textBoxUsername.TextChanged -= textBoxUsername_TextChanged;

                        textBoxPassword.Text = reader["Password"].ToString();
                        comboBoxRole.Text = reader["Role"].ToString();
                        textBoxNama.Text = reader["Nama"].ToString();
                        textBoxHP.Text = reader["No_HP_Telp"].ToString();
                        textBoxAlamat.Text = reader["Alamat"].ToString();

                        textBoxUsername.TextChanged += textBoxUsername_TextChanged;
                    }
                    else
                    {
                        textBoxUsername.TextChanged -= textBoxUsername_TextChanged;

                        textBoxPassword.Text = string.Empty;
                        comboBoxRole.Text = string.Empty;
                        textBoxNama.Text = string.Empty;
                        textBoxHP.Text = string.Empty;
                        textBoxAlamat.Text = string.Empty;

                        textBoxUsername.TextChanged += textBoxUsername_TextChanged;
                    }
                }
            }
            isTextChanging = false;
            UpdateDataGridViewWithSearchResults2();
        }

        private void UpdateAutoCompleteForUsername()
        {
            textBoxUsername.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxUsername.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection autoCompleteData = new AutoCompleteStringCollection();
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT Username FROM User";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                conn.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    autoCompleteData.Add(reader["Username"].ToString());
                }
            }
            textBoxUsername.AutoCompleteCustomSource = autoCompleteData;
        }

        private void SetupAutoCompleteForUsername()
        {
            textBoxUsername.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxUsername.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection autoCompleteData = new AutoCompleteStringCollection();
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT Username FROM User";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                conn.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    autoCompleteData.Add(reader["Username"].ToString());
                }
            }
            textBoxUsername.AutoCompleteCustomSource = autoCompleteData;
        }

        private void buttonTambahUser_Click(object sender, EventArgs e)
        {
            using (AdminTambahUser formTambah = new AdminTambahUser(connection))
            {
                if (formTambah.ShowDialog() == DialogResult.OK)
                {
                    LoadData2();
                    UpdateAutoCompleteForUsername();
                }
            }
        }

        private void buttonEditUser_Click(object sender, EventArgs e)
        {
            if (textBoxUsername.Text == string.Empty)
            {
                MessageBox.Show("Pilih user terlebih dahulu.");
                return;
            }

            using (AdminEditUser editForm = new AdminEditUser(textBoxUsername.Text, connection))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadData2();
                    FillUserTextBoxesFromDatabase("Username", textBoxUsername.Text);
                    UpdateAutoCompleteForUsername();
                }
            }
        }

        private void buttonHapusUser_Click(object sender, EventArgs e)
        {
            if (textBoxUsername.Text == string.Empty)
            {
                MessageBox.Show("Pilih user terlebih dahulu.");
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus user ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
                {
                    conn.Open();
                    using (SQLiteCommand command = new SQLiteCommand("DELETE FROM User WHERE Username = @username", conn))
                    {
                        command.Parameters.AddWithValue("@username", textBoxUsername.Text);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil dihapus.");
                LoadData2();
            }
        }


        private void textBoxID_TextChanged(object sender, EventArgs e)
        {
            if (isTextChanging) return;
            isTextChanging = true;

            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT * FROM Items WHERE ID = @value COLLATE NOCASE";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                command.Parameters.AddWithValue("@value", textBoxID.Text);
                conn.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        textBoxID.TextChanged -= textBoxID_TextChanged;
                        textBoxBarcode.TextChanged -= textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged -= textBoxNamaItem_TextChanged;

                        textBoxBarcode.Text = reader["Barcode"].ToString();
                        textBoxNamaItem.Text = reader["NamaItem"].ToString();
                        textBoxHargaJual.Text = reader["HJualItem"].ToString();
                        textBoxModal.Text = reader["Modal"].ToString();
                        textBoxStok.Text = reader["Stok"].ToString();

                        textBoxID.TextChanged += textBoxID_TextChanged;
                        textBoxBarcode.TextChanged += textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged += textBoxNamaItem_TextChanged;
                    }
                    else
                    {
                        textBoxID.TextChanged -= textBoxID_TextChanged;
                        textBoxBarcode.TextChanged -= textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged -= textBoxNamaItem_TextChanged;

                        textBoxBarcode.Text = string.Empty;
                        textBoxNamaItem.Text = string.Empty;
                        textBoxHargaJual.Text = string.Empty;
                        textBoxModal.Text = string.Empty;
                        textBoxStok.Text = string.Empty;

                        textBoxID.TextChanged += textBoxID_TextChanged;
                        textBoxBarcode.TextChanged += textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged += textBoxNamaItem_TextChanged;
                    }
                }
            }
            isTextChanging = false;
            UpdateDataGridViewWithSearchResults();
        }


        private void textBoxBarcode_TextChanged(object sender, EventArgs e)
        {
            if (isTextChanging) return;
            isTextChanging = true;

            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT * FROM Items WHERE Barcode = @value COLLATE NOCASE";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                command.Parameters.AddWithValue("@value", textBoxBarcode.Text);
                conn.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        textBoxID.TextChanged -= textBoxID_TextChanged;
                        textBoxBarcode.TextChanged -= textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged -= textBoxNamaItem_TextChanged;

                        textBoxID.Text = reader["ID"].ToString();
                        textBoxNamaItem.Text = reader["NamaItem"].ToString();
                        textBoxHargaJual.Text = reader["HJualItem"].ToString();
                        textBoxModal.Text = reader["Modal"].ToString();
                        textBoxStok.Text = reader["Stok"].ToString();

                        textBoxID.TextChanged += textBoxID_TextChanged;
                        textBoxBarcode.TextChanged += textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged += textBoxNamaItem_TextChanged;
                    }
                    else
                    {
                        textBoxID.TextChanged -= textBoxID_TextChanged;
                        textBoxBarcode.TextChanged -= textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged -= textBoxNamaItem_TextChanged;

                       
                        textBoxID.Text = string.Empty;
                        textBoxNamaItem.Text = string.Empty;
                        textBoxHargaJual.Text = string.Empty;
                        textBoxModal.Text = string.Empty;
                        textBoxStok.Text = string.Empty;

                        textBoxID.TextChanged += textBoxID_TextChanged;
                        textBoxBarcode.TextChanged += textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged += textBoxNamaItem_TextChanged;
                    }
                }
            }
            isTextChanging = false;
            UpdateDataGridViewWithSearchResults();
        }


        private void textBoxNamaItem_TextChanged(object sender, EventArgs e)
        {
            if (isTextChanging) return;
            isTextChanging = true;

            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT * FROM Items WHERE NamaItem = @value COLLATE NOCASE";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                command.Parameters.AddWithValue("@value", textBoxNamaItem.Text);
                conn.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        textBoxID.TextChanged -= textBoxID_TextChanged;
                        textBoxBarcode.TextChanged -= textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged -= textBoxNamaItem_TextChanged;

                        textBoxID.Text = reader["ID"].ToString();
                        textBoxBarcode.Text = reader["Barcode"].ToString();
                        textBoxHargaJual.Text = reader["HJualItem"].ToString();
                        textBoxModal.Text = reader["Modal"].ToString();
                        textBoxStok.Text = reader["Stok"].ToString();

                        textBoxID.TextChanged += textBoxID_TextChanged;
                        textBoxBarcode.TextChanged += textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged += textBoxNamaItem_TextChanged;
                    }
                    else
                    {
                        textBoxID.TextChanged -= textBoxID_TextChanged;
                        textBoxBarcode.TextChanged -= textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged -= textBoxNamaItem_TextChanged;

                        textBoxID.Text = string.Empty;
                        textBoxBarcode.Text = string.Empty;
                        textBoxHargaJual.Text = string.Empty;
                        textBoxModal.Text = string.Empty;
                        textBoxStok.Text = string.Empty;

                        textBoxID.TextChanged += textBoxID_TextChanged;
                        textBoxBarcode.TextChanged += textBoxBarcode_TextChanged;
                        textBoxNamaItem.TextChanged += textBoxNamaItem_TextChanged;
                    }
                }
            }
            isTextChanging = false;
            UpdateDataGridViewWithSearchResults();
        }

        private void SetupAutoCompleteForID()
        {
            textBoxID.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxID.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection autoCompleteData = new AutoCompleteStringCollection();
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT ID FROM Items";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                conn.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    autoCompleteData.Add(reader["ID"].ToString());
                }
            }
            textBoxID.AutoCompleteCustomSource = autoCompleteData;
        }

        private void SetupAutoCompleteForBarcode()
        {
            textBoxBarcode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxBarcode.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection autoCompleteData = new AutoCompleteStringCollection();
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT Barcode FROM Items";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                conn.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    autoCompleteData.Add(reader["Barcode"].ToString());
                }
            }
            textBoxBarcode.AutoCompleteCustomSource = autoCompleteData;
        }

        private void SetupAutoCompleteForNamaItem()
        {
            textBoxNamaItem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxNamaItem.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection autoCompleteData = new AutoCompleteStringCollection();
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT NamaItem FROM Items";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                conn.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    autoCompleteData.Add(reader["NamaItem"].ToString());
                }
            }
            textBoxNamaItem.AutoCompleteCustomSource = autoCompleteData;
        }

        private void FillTextBoxesFromDatabase(string column, string value)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = $"SELECT * FROM Items WHERE {column} = @value COLLATE NOCASE";
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@value", value);
                    conn.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBoxID.TextChanged -= textBoxID_TextChanged;
                            textBoxBarcode.TextChanged -= textBoxBarcode_TextChanged;
                            textBoxNamaItem.TextChanged -= textBoxNamaItem_TextChanged;

                            textBoxID.Text = reader["ID"].ToString();
                            textBoxBarcode.Text = reader["Barcode"].ToString();
                            textBoxNamaItem.Text = reader["NamaItem"].ToString();
                            textBoxHargaJual.Text = reader["HJualItem"].ToString();
                            textBoxModal.Text = reader["Modal"].ToString();
                            textBoxStok.Text = reader["Stok"].ToString();

                            textBoxID.TextChanged += textBoxID_TextChanged;
                            textBoxBarcode.TextChanged += textBoxBarcode_TextChanged;
                            textBoxNamaItem.TextChanged += textBoxNamaItem_TextChanged;
                        }
                        else
                        {
                            textBoxID.Clear();
                            textBoxBarcode.Clear();
                            textBoxNamaItem.Clear();
                            textBoxHargaJual.Clear();
                            textBoxModal.Clear();
                            textBoxStok.Clear();
                        }
                    }
                }
            }
        }

        private void FillUserTextBoxesFromDatabase(string column, string value)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = $"SELECT * FROM User WHERE {column} = @value COLLATE NOCASE";
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@value", value);
                    conn.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBoxUsername.TextChanged -= textBoxUsername_TextChanged;

                            textBoxUsername.Text = reader["Username"].ToString();
                            textBoxPassword.Text = reader["Password"].ToString();
                            comboBoxRole.Text = reader["Role"].ToString();
                            textBoxNama.Text = reader["Nama"].ToString();
                            textBoxHP.Text = reader["No_Hp_Telp"].ToString();
                            textBoxAlamat.Text = reader["Alamat"].ToString();

                            textBoxUsername.TextChanged += textBoxUsername_TextChanged;
                        }
                        else
                        {
                            textBoxUsername.Clear();
                            textBoxPassword.Clear();
                            comboBoxRole.Text = string.Empty;
                            textBoxNama.Clear();
                            textBoxHP.Clear();
                            textBoxAlamat.Clear();
                        }
                    }
                }
            }
        }




        private void buttonRestockItem_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text == string.Empty)
            {
                MessageBox.Show("Pilih item terlebih dahulu.");
                return;
            }

            int stokTambahan = 0;
            string input = Microsoft.VisualBasic.Interaction.InputBox("Masukkan jumlah stok yang mau ditambah:", "Restock", "0");

            if (int.TryParse(input, out stokTambahan))
            {
                using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
                {
                    conn.Open();
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string query = "UPDATE Items SET Stok = Stok + @stokTambahan WHERE ID = @id";
                            using (SQLiteCommand command = new SQLiteCommand(query, conn, transaction))
                            {
                                command.Parameters.AddWithValue("@stokTambahan", stokTambahan);
                                command.Parameters.AddWithValue("@id", textBoxID.Text);
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            MessageBox.Show("Stok berhasil ditambahkan.");
                            LoadData();
                            UpdateTextBoxes(textBoxID.Text, "ID");
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Jumlah stok tidak valid.");
            }
        }

        private void UpdateTextBoxes(string value, string column)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = $"SELECT * FROM Items WHERE {column} = @value COLLATE NOCASE";
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@value", value);
                    try
                    {
                        conn.Open();
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                textBoxID.Text = reader["ID"].ToString();
                                textBoxBarcode.Text = reader["Barcode"].ToString();
                                textBoxNamaItem.Text = reader["NamaItem"].ToString();
                                textBoxHargaJual.Text = reader["HJualItem"].ToString();
                                textBoxModal.Text = reader["Modal"].ToString();
                                textBoxStok.Text = reader["Stok"].ToString();
                            }
                            else
                            {
                                textBoxID.Clear();
                                textBoxBarcode.Clear();
                                textBoxNamaItem.Clear();
                                textBoxHargaJual.Clear();
                                textBoxModal.Clear();
                                textBoxStok.Clear();
                            }
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void buttonTambahItem_Click(object sender, EventArgs e)
        {
            using (AdminTambahItem formTambah = new AdminTambahItem(connection))
            {
                if (formTambah.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }


        private void buttonEditItem_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text == string.Empty)
            {
                MessageBox.Show("Pilih item terlebih dahulu.");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                conn.Open();
                using (AdminEditItem editForm = new AdminEditItem(textBoxID.Text, conn))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                        FillTextBoxesFromDatabase("ID", textBoxID.Text);
                    }
                }
            }
        }

        private void toolStripButtonUserUser2_Click(object sender, EventArgs e)
        {
            panel1User.BringToFront();
        }

        private void toolStripButtonItemUser2_Click(object sender, EventArgs e)
        {
            panel1User.BringToFront(); 
        }

        private void toolStripButtonUserItem1_Click(object sender, EventArgs e)
        {
            panel1Item.BringToFront();
        }

        private void toolStripButtonItemItem1_Click(object sender, EventArgs e)
        {
            panel1Item.BringToFront();
        }

        private void toolStripButtonUserKeluar3_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }

        private void toolStripButtonItemKeluar3_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }

        private void buttonHapusItem_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text == string.Empty)
            {
                MessageBox.Show("Pilih item terlebih dahulu.");
                return;
            }

            
            DialogResult dialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus item ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
                {
                    string query = "DELETE FROM Items WHERE ID = @id";
                    SQLiteCommand command = new SQLiteCommand(query, conn);
                    command.Parameters.AddWithValue("@id", textBoxID.Text);
                    conn.Open();
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Data berhasil dihapus.");
                LoadData();
            }
            else
            {

            }
        }

    }
}
