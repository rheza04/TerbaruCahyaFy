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

        //deteksi database konek apa kagak
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

        private void InitializeCustomComponents()
        {
            // fungsi cek interval konek jaringan apa kagak
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

            textBox3.TextChanged += new EventHandler(textBox3_TextChanged);
            textBox23.KeyDown += new KeyEventHandler(textBox23_KeyDown);
            dataGridViewQR.CellDoubleClick += new DataGridViewCellEventHandler(dataGridViewQR_CellDoubleClick);
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

        //fungsi scanbarang textbox3
        private void ScanBarang(string barcode)
        {
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

                    textBox16.Text = namaItem; // Menampilkan nama barang
                    textBox15.Text = qty.ToString(); // Menampilkan jumlah barang
                    textBox12.Text = sisaQty.ToString(); // Menampilkan sisa stok
                    textBox13.Text = harga.ToString(); // Menampilkan harga barang
                    textBox2.Text = (harga * qty).ToString("N0"); // Menampilkan total harga
                    textBox4.Text = Terbilang(Convert.ToInt32(harga * qty)) + " RUPIAH"; // Menampilkan total harga dalam teks

                    // Cek apakah barang sudah ada di dataGridViewJualan
                    bool itemFound = false;
                    foreach (DataGridViewRow row in dataGridViewJualan.Rows)
                    {
                        // Pastikan baris tidak kosong
                        if (row.Cells["Kode"].Value != null && row.Cells["Kode"].Value.ToString() == barcode)
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

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            string search = textBox3.Text;
            string query = "SELECT * FROM Items WHERE NamaItem LIKE @Search";
            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Search", "%" + search + "%");

            try
            {
                connection.Open();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridViewQR.DataSource = dt;
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

        private void dataGridViewQR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewQR.Rows[e.RowIndex];
                string barcode = row.Cells["Barcode"].Value.ToString();
                ScanBarang(barcode);
            }
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

            //lebar gridviewpenjualan
            dataGridViewJualan.Columns["Kode"].Width = 90;
            dataGridViewJualan.Columns["ItemKeterangan"].Width = 200;
            dataGridViewJualan.Columns["Qty"].Width = 60;
            dataGridViewJualan.Columns["Harga"].Width = 140;
            dataGridViewJualan.Columns["SubTotal"].Width = 140;
        }

        private string ConvertToWords(decimal number)
        {
            // Fungsi untuk mengubah angka menjadi teks dalam bahasa Indonesia
            // Implementasi fungsi ini sesuai kebutuhan
            return "SEPULUH RIBU RUPIAH"; // Contoh
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

        private void textBox23_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Pastikan nilai dalam textBox23 adalah angka yang valid
                if (decimal.TryParse(textBox23.Text, out decimal bayar))
                {
                    // Total belanja
                    if (decimal.TryParse(textBox2.Text, out decimal totalBelanja))
                    {
                        // Kembalian
                        decimal kembalian = bayar - totalBelanja;

                        // Menampilkan kembalian
                        textBox2.Text = kembalian.ToString("N0");
                        label7.Text = "KEMBALIAN";
                        textBox4.Text = Terbilang(Convert.ToInt32(kembalian)) + " RUPIAH";
                        textBox3.Text = "DIBAYAR";

                        // Pesan konfirmasi pembayaran
                        DialogResult result = MessageBox.Show("Apakah Anda ingin membayar dengan tunai?", "Konfirmasi Pembayaran", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            // Reset form untuk transaksi baru
                            ResetForm();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Format total belanja tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Format jumlah pembayaran tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResetForm()
        {
            // Reset semua textbox dan datagridview
            textBox16.Text = "";
            textBox15.Text = "1";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox2.Text = "";
            textBox4.Text = "";
            textBox23.Text = "";
            dataGridViewJualan.Rows.Clear();
            label7.Text = "TOTAL";
        }

        private void textBox23_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Hanya izinkan angka dan karakter kontrol (seperti backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private string ConvertToIndonesianWords(decimal number)
        {
            // Implementasi konversi angka ke teks bahasa Indonesia
            if (number == 0)
                return "Nol Rupiah";

            string[] angka = { "", "Satu", "Dua", "Tiga", "Empat", "Lima", "Enam", "Tujuh", "Delapan", "Sembilan" };
            string[] posisi = { "", "Puluh", "Ratus", "Ribu", "Juta", "Miliar", "Triliun" };

            string words = "";
            int unitPos = 0;

            while (number > 0)
            {
                int digit = (int)(number % 10);
                if (digit > 0)
                {
                    words = angka[digit] + " " + posisi[unitPos] + " " + words;
                }
                number = number / 10;
                unitPos++;
            }

            words = words.Trim() + " Rupiah";
            return words;
        }
    }
}
