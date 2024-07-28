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
    public partial class LoginForm : Form
    {
        private SQLiteConnection connection;

        public LoginForm()
        {
            InitializeComponent();
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data Barang.db");
            connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                connection.Open();
                labelCheckConnection.Text = "Connection Successful";
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();
                string query = "SELECT * FROM User WHERE Username = @Username AND Password = @Password";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", textBoxUsername.Text);
                    command.Parameters.AddWithValue("@Password", textBoxPwd.Text);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string role = reader["Role"].ToString();
                            reader.Close();
                            connection.Close(); 

                            if (role == "Admin")
                            {
                                MessageBox.Show("Login sebagai Admin berhasil");
                                this.Hide();
                                using (Admin inputBarangForm = new Admin())
                                {
                                    inputBarangForm.ShowDialog();
                                }
                                this.Show();
                            }
                            else if (role == "User")
                            {
                                MessageBox.Show("Login sebagai User berhasil");
                                this.Hide();
                                using (Kasir kasirForm = new Kasir(textBoxUsername.Text))
                                {
                                    kasirForm.ShowDialog();
                                }
                                this.Show();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Username atau Password salah");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
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

        private void textBoxUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBoxPwd.Focus();
            }
        }

        private void textBoxPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonLogin.PerformClick();
            }
        }
    }
}
