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

namespace TerbaruCahyaFy
{
    public partial class Kasir : Form
    {
        private SQLiteConnection connection;
        private TcpListener tcpListener;
        private string currentIPAddress;
        private string currentNetworkStatus;
        private Timer timer;

        public Kasir()
        {
            InitializeComponent();
            InitializeCustomComponents();
            StartTcpServer();
            InitializeDatabaseConnection();
            InitializeDataGridView();
        }

        public Kasir(string Username)
        {
            InitializeComponent();
            textBoxNama.Text = Username;
            textBoxNama2.Text = Username;
            InitializeCustomComponents();
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

        private void StartTcpServer()
        {
            Task.Run(() =>
            {
                try
                {
                    int port = 5789; // Port default yang sering digunakan oleh aplikasi barcode scanner
                    tcpListener = new TcpListener(IPAddress.Any, port);
                    tcpListener.Start();

                    while (true)
                    {
                        var client = tcpListener.AcceptTcpClient();
                        var stream = client.GetStream();
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string barcode = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // Panggil metode ScanBarang di thread UI
                        this.Invoke((MethodInvoker)delegate {
                            ScanBarang(barcode.Trim());
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            });
        }

        private void ScanBarang(string barcode)
        {
            // Fungsi untuk menangani scan barcode
            string query = "SELECT * FROM Items WHERE Barcode = @Barcode";
            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Barcode", barcode);

            try
            {
                connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string namaItem = reader["NamaItem"].ToString();
                    int qty = 1;
                    decimal harga = Convert.ToDecimal(reader["HJualPcs"]);
                    int sisaQty = Convert.ToInt32(reader["Stok"]);

                    textBox16.Text = namaItem;
                    textBox15.Text = qty.ToString();
                    textBox12.Text = sisaQty.ToString();
                    textBox13.Text = harga.ToString();
                    textBox2.Text = (harga * qty).ToString("N0");
                    textBox4.Text = Terbilang(Convert.ToInt32(harga * qty)) + " RUPIAH";

                    // Cek apakah barang sudah ada di dataGridViewJualan
                    bool itemFound = false;
                    foreach (DataGridViewRow row in dataGridViewJualan.Rows)
                    {
                        if (row.Cells["Kode"].Value.ToString() == barcode)
                        {
                            row.Cells["Qty"].Value = Convert.ToInt32(row.Cells["Qty"].Value) + 1;
                            row.Cells["SubTotal"].Value = Convert.ToDecimal(row.Cells["Qty"].Value) * Convert.ToDecimal(row.Cells["Harga"].Value);
                            itemFound = true;
                            break;
                        }
                    }

                    // Jika barang tidak ditemukan, tambahkan sebagai baris baru
                    if (!itemFound)
                    {
                        dataGridViewJualan.Rows.Add(new object[] { barcode, namaItem, qty, harga, harga * qty });
                    }
                }
                else
                {
                    MessageBox.Show("Barang tidak ditemukan!");
                }
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

        private string Terbilang(int angka)
        {
            // Implementasi fungsi terbilang untuk mengkonversi angka ke teks dalam bahasa Indonesia
            // Contoh sederhana
            if (angka == 10000) return "SEPULUH RIBU";
            // Implementasi lebih lengkap untuk berbagai angka diperlukan
            return angka.ToString();
        }

        private void UpdateTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dataGridViewJualan.Rows)
            {
                total += Convert.ToDecimal(row.Cells["SubTotal"].Value);
            }
            textBox2.Text = total.ToString();
            textBox4.Text = ConvertToWords(total);
        }

        private void InitializeDataGridView()
        {
            dataGridViewJualan.Columns.Clear();
            dataGridViewJualan.Columns.Add("Kode", "Kode");
            dataGridViewJualan.Columns.Add("ItemKeterangan", "Item / Keterangan");
            dataGridViewJualan.Columns.Add("Qty", "Qty");
            dataGridViewJualan.Columns.Add("Harga", "Harga");
            dataGridViewJualan.Columns.Add("SubTotal", "Sub Total");

            // Atur lebar kolom sesuai kebutuhan
            dataGridViewJualan.Columns["Kode"].Width = 100;
            dataGridViewJualan.Columns["ItemKeterangan"].Width = 200;
            dataGridViewJualan.Columns["Qty"].Width = 50;
            dataGridViewJualan.Columns["Harga"].Width = 100;
            dataGridViewJualan.Columns["SubTotal"].Width = 100;
        }

        private string ConvertToWords(decimal number)
        {
            // Fungsi untuk mengubah angka menjadi teks dalam bahasa Indonesia
            // Implementasi fungsi ini sesuai kebutuhan
            return "SEPULUH RIBU RUPIAH"; // Contoh
        }

        private void InitializeCustomComponents()
        {
            // Menginisialisasi timer untuk cek jaringan dan IP
            timer = new Timer();
            timer.Interval = 5000; // 5 detik
            timer.Tick += new EventHandler(CheckStatus);
            timer.Start();

            // Set status jaringan pertama kali
            SetNetworkStatus();

            // Set IP pertama kali
            SetIPAddress();

            // Menampilkan tanggal hari ini
            textBox1.Text = DateTime.Now.ToString("dd MMMM yyyy");
        }

        private void CheckStatus(object sender, EventArgs e)
        {
            // Periksa status jaringan
            string newNetworkStatus = NetworkInterface.GetIsNetworkAvailable() ? "Lokal Online" : "Lokal Offline";
            if (newNetworkStatus != currentNetworkStatus)
            {
                currentNetworkStatus = newNetworkStatus;
                textBox21.Text = currentNetworkStatus;
                if (newNetworkStatus == "Lokal Offline")
                {
                    textBox21.BackColor = Color.Red;
                    textBox21.ForeColor = Color.LightYellow;
                }
                else
                {
                    textBox21.BackColor = SystemColors.Window;
                    textBox21.ForeColor = SystemColors.ControlText;
                }
            }

            // Periksa alamat IP
            string newIPAddress = GetLocalIPAddress();
            if (newIPAddress != currentIPAddress)
            {
                currentIPAddress = newIPAddress;
                textBox22.Text = "PC : " + currentIPAddress;
            }
        }

        private void SetNetworkStatus()
        {
            currentNetworkStatus = NetworkInterface.GetIsNetworkAvailable() ? "Lokal Online" : "Lokal Offline";
            textBox21.Text = currentNetworkStatus;
            if (currentNetworkStatus == "Lokal Offline")
            {
                textBox21.BackColor = Color.Red;
                textBox21.ForeColor = Color.LightYellow;
            }
            else
            {
                textBox21.BackColor = SystemColors.Window;
                textBox21.ForeColor = SystemColors.ControlText;
            }
        }

        private void SetIPAddress()
        {
            currentIPAddress = GetLocalIPAddress();
            textBox22.Text = "PC : " + currentIPAddress;
        }

        private string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "Tidak ada jaringan yang tersambung!";
            }
            catch (Exception)
            {
                return "Error mendapatkan IP";
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Event handler untuk Paint event pada tableLayoutPanel1
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            // Event handler untuk Paint event pada panel5
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ScanBarang(textBox3.Text);
                textBox3.Clear();
            }
        }
    }
}
