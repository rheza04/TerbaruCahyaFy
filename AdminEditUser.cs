using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

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
            connection = conn ?? InitializeDatabaseConnection();
            originalUsername = username;
            LoadUserDetails();
        }

        private void InitializeCustomComponents()
        {
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

        private void LoadUserDetails()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = "SELECT * FROM ListUser WHERE Username = @username";
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@username", originalUsername);
                    conn.Open();
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
            }
        }

        private bool IsDuplicate(string column, string value, string excludeUsername)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                string query = $"SELECT COUNT(*) FROM ListUser WHERE {column} = @value AND Username != @excludeUsername";
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
            if (IsDuplicate("Username", textBoxUsername.Text, originalUsername))
            {
                MessageBox.Show("Maaf, Username yang Anda mau ubah sudah ada.");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                conn.Open();
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = "UPDATE ListUser SET Username = @username, Password = @password, Role = @role, Nama = @nama, No_HP_Telp = @hp, Alamat = @alamat WHERE Username = @originalUsername";
                        using (SQLiteCommand command = new SQLiteCommand(query, conn, transaction))
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
