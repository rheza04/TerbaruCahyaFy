using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TerbaruCahyaFy
{
    public partial class AdminTambahItem : Form
    {
        private SQLiteConnection connection;

        public AdminTambahItem(SQLiteConnection conn)
        {
            InitializeComponent();
            InitializeCustomComponents();
            connection = conn ?? InitializeDatabaseConnection();
        }

        private void InitializeCustomComponents()
        {
            // Tambahkan event handler
            buttonTambah.Click += new EventHandler(buttonTambah_Click);
            buttonBatal.Click += new EventHandler(buttonBatal_Click);
        }

        private SQLiteConnection InitializeDatabaseConnection()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data Barang.db");
            SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            try
            {
                conn.Open();
                MessageBox.Show("Connection Successful");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return conn;
        }

        private bool IsDuplicate(string column, string value)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = $"SELECT COUNT(*) FROM Items WHERE {column} = @value";
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@value", value);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private void buttonTambah_Click(object sender, EventArgs e)
        {
            // Validasi duplikasi
            if (IsDuplicate("ID", textBoxID.Text))
            {
                MessageBox.Show("Maaf, ID yang Anda mau tambah sudah ada.");
                return;
            }
            if (IsDuplicate("Barcode", textBoxBarcode.Text))
            {
                MessageBox.Show("Maaf, Barcode yang Anda mau tambah sudah ada.");
                return;
            }
            if (IsDuplicate("NamaItem", textBoxNamaItem.Text))
            {
                MessageBox.Show("Maaf, Nama Item yang Anda mau tambah sudah ada.");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                conn.Open();
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO Items (ID, Barcode, NamaItem, HJualItem, Modal, Stok) VALUES (@id, @barcode, @namaItem, @hargaJual, @modal, @stok)";
                        using (SQLiteCommand command = new SQLiteCommand(query, conn, transaction))
                        {
                            command.Parameters.AddWithValue("@id", textBoxID.Text);
                            command.Parameters.AddWithValue("@barcode", textBoxBarcode.Text);
                            command.Parameters.AddWithValue("@namaItem", textBoxNamaItem.Text);
                            command.Parameters.AddWithValue("@hargaJual", textBoxHargaJual.Text);
                            command.Parameters.AddWithValue("@modal", textBoxModal.Text);
                            command.Parameters.AddWithValue("@stok", textBoxStok.Text);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        MessageBox.Show("Data berhasil ditambahkan.");
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}