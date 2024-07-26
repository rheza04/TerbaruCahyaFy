using System;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;
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
                    string query = "SELECT * FROM ListSupplier WHERE Username = @Username AND Password = @Password";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", textBoxUsername.Text);
                    command.Parameters.AddWithValue("@Password", textBoxPwd.Text);

                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string role = reader["Role"].ToString();
                        if (role == "admin")
                        {
                            MessageBox.Show("Login sebagai Admin berhasil");
                            this.Hide();
                            Admin inputBarangForm = new Admin();
                            inputBarangForm.ShowDialog();
                        }
                        else if (role == "user")
                        {
                            MessageBox.Show("Login sebagai User berhasil");
                            this.Hide();
                            Kasir kasirForm = new Kasir(textBoxUsername.Text);
                            kasirForm.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Username atau Password salah");
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
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
