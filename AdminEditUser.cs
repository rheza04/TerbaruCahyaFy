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
using TerbaruCahyaFy;

namespace TerbaruCahyaFy
{
    public partial class AdminEditUser : Form
    {
        private SQLiteConnection connection;
        private string originalUsername;

        public AdminEditUser(string username, SQLiteConnection conn)
        {
            InitializeComponent();
            InitializeCustomComponents();
            connection = conn;
            originalUsername = username;
            LoadUserDetails();
        }

        private void InitializeCustomComponents()
        {
            buttonEdit.Click += new EventHandler(buttonEdit_Click);
            buttonBatal.Click += new EventHandler(buttonBatal_Click);
        }

        private void LoadUserDetails()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM User WHERE Username = @username", connection))
            {
                command.Parameters.AddWithValue("@username", originalUsername);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        textBoxUsername.Text = reader["Username"].ToString();
                        textBoxPassword.Text = reader["Password"].ToString();
                        comboBoxRole.Text = reader["Role"].ToString();
                        textBoxNama.Text = reader["Nama"].ToString();
                        textBoxHP.Text = reader["No_HP_Telp"].ToString();
                        textBoxAlamat.Text = reader["Alamat"].ToString();
                    }
                }
            }
            connection.Close();
        }

        private bool IsDuplicate(string column, string value, string excludeUsername)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = $"SELECT COUNT(*) FROM User WHERE {column} = @value AND Username != @excludeUsername";
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@value", value);
                    command.Parameters.AddWithValue("@excludeUsername", excludeUsername);
                    conn.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsDuplicate("Username", textBoxUsername.Text, originalUsername))
                {
                    MessageBox.Show("Maaf, Username yang Anda mau ubah sudah ada.");
                    return;
                }

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "UPDATE User SET Username = @username, Password = @password, Role = @role, Nama = @nama, No_HP_Telp = @hp, Alamat = @alamat WHERE Username = @originalUsername";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@originalUsername", originalUsername);
                            command.Parameters.AddWithValue("@username", textBoxUsername.Text);
                            command.Parameters.AddWithValue("@password", textBoxPassword.Text);
                            command.Parameters.AddWithValue("@role", comboBoxRole.Text);
                            command.Parameters.AddWithValue("@nama", textBoxNama.Text);
                            command.Parameters.AddWithValue("@hp", textBoxHP.Text);
                            command.Parameters.AddWithValue("@alamat", textBoxAlamat.Text);
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
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void buttonBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
