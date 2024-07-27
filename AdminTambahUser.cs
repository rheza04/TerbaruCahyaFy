using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace TerbaruCahyaFy
{
    public partial class AdminTambahUser : Form
    {
        private SQLiteConnection connection;

        public AdminTambahUser(SQLiteConnection conn)
        {
            InitializeComponent();
            InitializeCustomComponents();
            connection = conn ?? InitializeDatabaseConnection();
        }

        private void InitializeCustomComponents()
        {
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
                string query = $"SELECT COUNT(*) FROM ListUser WHERE {column} = @value";
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
            if (IsDuplicate("Username", textBoxUsername.Text))
            {
                MessageBox.Show("Maaf, Username yang Anda masukkan sudah ada.");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(connection.ConnectionString))
            {
                conn.Open();
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO ListUser (Username, Password, Role, Nama, No_HP_Telp, Alamat) VALUES (@username, @password, @role, @nama, @hp, @alamat)";
                        using (SQLiteCommand command = new SQLiteCommand(query, conn, transaction))
                        {
                            command.Parameters.AddWithValue("@username", textBoxUsername.Text);
                            command.Parameters.AddWithValue("@password", textBoxPassword.Text);
                            command.Parameters.AddWithValue("@role", comboBoxRole.Text);
                            command.Parameters.AddWithValue("@nama", textBoxNama.Text);
                            command.Parameters.AddWithValue("@hp", textBoxHP.Text);
                            command.Parameters.AddWithValue("@alamat", textBoxAlamat.Text);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        MessageBox.Show("User berhasil ditambahkan.");
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
