﻿using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace TerbaruCahyaFy
{
    public partial class AdminEdit : Form
    {
        private SQLiteConnection connection;
        private string originalId;

        public AdminEdit(string id, SQLiteConnection conn)
        {
            InitializeComponent();
            InitializeCustomComponents();
            connection = conn ?? InitializeDatabaseConnection();
            originalId = id;
            LoadItemDetails();
        }

        private void InitializeCustomComponents()
        {
            // Tambahkan event handler
            buttonEdit.Click += new EventHandler(buttonEdit_Click);
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

        private void LoadItemDetails()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT * FROM Items WHERE ID = @id";
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@id", originalId);
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
                    }
                }
            }
        }

        private bool IsDuplicate(string column, string value, string excludeId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = $"SELECT COUNT(*) FROM Items WHERE {column} = @value AND ID != @excludeId";
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@value", value);
                    command.Parameters.AddWithValue("@excludeId", excludeId);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            // Validasi duplikasi
            if (IsDuplicate("ID", textBoxID.Text, originalId))
            {
                MessageBox.Show("Maaf, ID yang Anda mau ubah sudah ada.");
                return;
            }
            if (IsDuplicate("Barcode", textBoxBarcode.Text, originalId))
            {
                MessageBox.Show("Maaf, Barcode yang Anda mau ubah sudah ada.");
                return;
            }
            if (IsDuplicate("NamaItem", textBoxNamaItem.Text, originalId))
            {
                MessageBox.Show("Maaf, Nama Item yang Anda mau ubah sudah ada.");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                conn.Open();
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = "UPDATE Items SET ID = @id, Barcode = @barcode, NamaItem = @namaItem, HJualItem = @hargaJual, Modal = @modal, Stok = @stok WHERE ID = @originalId";
                        using (SQLiteCommand command = new SQLiteCommand(query, conn, transaction))
                        {
                            command.Parameters.AddWithValue("@originalId", originalId);
                            command.Parameters.AddWithValue("@id", textBoxID.Text);
                            command.Parameters.AddWithValue("@barcode", textBoxBarcode.Text);
                            command.Parameters.AddWithValue("@namaItem", textBoxNamaItem.Text);
                            command.Parameters.AddWithValue("@hargaJual", textBoxHargaJual.Text);
                            command.Parameters.AddWithValue("@modal", textBoxModal.Text);
                            command.Parameters.AddWithValue("@stok", textBoxStok.Text);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        MessageBox.Show("Data berhasil diubah.");
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
