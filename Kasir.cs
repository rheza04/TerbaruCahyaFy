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
        private bool isScanning = false;

        public Kasir()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            InitializeCustomComponents();
            StartTcpServer();
            InitializeDataGridView();
            SetupDataGridView();
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

            Timer dateTimeTimer = new Timer();
            dateTimeTimer.Interval = 1000; // 1 detik
            dateTimeTimer.Tick += new EventHandler(UpdateDateTime);
            dateTimeTimer.Start();

            SetNetworkStatus();
            SetIPAddress();

            // Menampilkan tanggal hari ini
            textBox1.Text = DateTime.Now.ToString("dd MMMM yyyy");

            textBox3.TextChanged += new EventHandler(textBox3_TextChanged);
            textBox23.KeyDown += new KeyEventHandler(textBox23_KeyDown);
            dataGridViewQR.CellDoubleClick += new DataGridViewCellEventHandler(dataGridViewQR_CellDoubleClick);

            textBox23.TextChanged += new EventHandler(textBox23_TextChanged);
            textBox13.TextChanged += new EventHandler(textBox13_TextChanged);
            textBoxNama.TextChanged += new EventHandler(textBoxNama_TextChanged);
            textBoxNama2.TextChanged += new EventHandler(textBoxNama2_TextChanged);
            textBox23.KeyPress += new KeyPressEventHandler(textBox23_KeyPress);

            // Tambahkan event handler untuk dataGridViewJualan
            dataGridViewJualan.CellValueChanged += new DataGridViewCellEventHandler(dataGridViewJualan_CellValueChanged);
            dataGridViewJualan.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridViewJualan_EditingControlShowing);

            //autocomplete textbox3
            textBox3.AutoCompleteMode = AutoCompleteMode.Append;
            textBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;

            dataGridViewQR.KeyDown += new KeyEventHandler(dataGridViewQR_KeyDown);
        }

        private void UpdateAutoCompleteSource()
        {
            AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
            string query = "SELECT NamaItem FROM Items";
            if (connection == null)
            {
                InitializeDatabaseConnection();
            }
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            autoComplete.Add(reader["NamaItem"].ToString());
                        }
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
            textBox3.AutoCompleteCustomSource = autoComplete;
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

                // Update autocomplete suggestions
                AutoCompleteStringCollection suggestions = new AutoCompleteStringCollection();
                foreach (DataRow row in dt.Rows)
                {
                    suggestions.Add(row["NamaItem"].ToString());
                }
                textBox3.AutoCompleteCustomSource = suggestions;

                // Show dataGridViewQR if there are any results
                if (dt.Rows.Count > 0)
                {
                    dataGridViewQR.Visible = true;
                    dataGridViewJualan.Visible = false;
                }
                else
                {
                    dataGridViewQR.Visible = false;
                    dataGridViewJualan.Visible = true;
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

        private void dataGridViewQR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridViewQR.Rows[e.RowIndex];
                string barcode = row.Cells["Barcode"].Value.ToString();
                AddItemToDataGridViewJualan(barcode);
                dataGridViewQR.SendToBack(); // Pindahkan dataGridViewQR ke belakang
                dataGridViewJualan.BringToFront(); // Pastikan dataGridViewJualan berada di depan
                textBox3.Clear();
            }
        }

        private void dataGridViewQR_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dataGridViewQR.CurrentRow != null)
                {
                    var row = dataGridViewQR.CurrentRow;
                    string barcode = row.Cells["Barcode"].Value.ToString();
                    AddItemToDataGridViewJualan(barcode);
                    dataGridViewQR.SendToBack(); // Pindahkan dataGridViewQR ke belakang
                    dataGridViewJualan.BringToFront(); // Pastikan dataGridViewJualan berada di depan
                    textBox3.Clear();
                    e.Handled = true; // Prevents the 'ding' sound when Enter is pressed
                }
            }
        }

        private void AddItemToDataGridViewJualan(string barcode)
        {
            try
            {
                connection.Open();
                string query = "SELECT * FROM Items WHERE Barcode = @Barcode";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Barcode", barcode);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string namaItem = reader["NamaItem"].ToString();
                            int qty = 1;
                            decimal harga = Convert.ToDecimal(reader["HJualPcs"]);
                            int sisaQty = Convert.ToInt32(reader["Stok"]);

                            // Cek apakah barang sudah ada di dataGridViewJualan
                            bool itemFound = false;
                            foreach (DataGridViewRow dataRow in dataGridViewJualan.Rows)
                            {
                                if (dataRow.Cells["Kode"].Value != null && dataRow.Cells["Kode"].Value.ToString() == barcode)
                                {
                                    dataRow.Cells["Qty"].Value = Convert.ToInt32(dataRow.Cells["Qty"].Value) + 1;
                                    dataRow.Cells["SubTotal"].Value = Convert.ToDecimal(dataRow.Cells["SubTotal"].Value) + harga;
                                    itemFound = true;
                                    break;
                                }
                            }

                            // Jika barang tidak ditemukan, tambahkan sebagai baris baru
                            if (!itemFound)
                            {
                                dataGridViewJualan.Rows.Add(new object[] { barcode, namaItem, qty, harga, harga * qty });
                            }

                            // Update total setelah menambahkan barang
                            UpdateTotal();
                            dataGridViewJualan.BringToFront(); // Pastikan dataGridViewJualan berada di depan
                            dataGridViewJualan.Focus(); // Fokuskan ke dataGridViewJualan
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
                connection.Close();
            }
        }


        private void InitializeDataGridView()
        {
            dataGridViewJualan.Columns.Clear();
            dataGridViewJualan.Columns.Add("Kode", "Kode");
            dataGridViewJualan.Columns.Add("ItemKeterangan", "Item / Keterangan");
            dataGridViewJualan.Columns.Add("Qty", "Qty");
            dataGridViewJualan.Columns.Add("Harga", "Harga");
            dataGridViewJualan.Columns.Add("SubTotal", "Sub Total");

            dataGridViewJualan.Columns["ItemKeterangan"].Width = 200;
            dataGridViewJualan.Columns["Qty"].Width = 60;
            dataGridViewJualan.Columns["Harga"].Width = 140;
            dataGridViewJualan.Columns["SubTotal"].Width = 140;

            dataGridViewJualan.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewJualan.Columns["Harga"].DefaultCellStyle.Format = "N0";
            dataGridViewJualan.Columns["SubTotal"].DefaultCellStyle.Format = "N0";

            dataGridViewQR.Visible = false; // Hide dataGridViewQR by default
        }





        private void SetupDataGridView()
        {
            dataGridViewJualan.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewJualan.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewJualan.Columns["Harga"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewJualan.Columns["SubTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void dataGridViewJualan_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridViewJualan.Columns["Qty"].Index)
            {
                UpdateSubTotal(e.RowIndex);
                UpdateTotal();
            }
        }
        private void Qty_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Hanya izinkan angka dan karakter kontrol (seperti backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void UpdateSubTotal(int rowIndex)
        {
            var row = dataGridViewJualan.Rows[rowIndex];
            if (row.Cells["Qty"].Value != null && row.Cells["Harga"].Value != null)
            {
                int qty = Convert.ToInt32(row.Cells["Qty"].Value);
                decimal harga = Convert.ToDecimal(row.Cells["Harga"].Value);
                row.Cells["SubTotal"].Value = qty * harga;
            }
        }

        private void dataGridViewJualan_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridViewJualan.CurrentCell.ColumnIndex == dataGridViewJualan.Columns["Qty"].Index)
            {
                TextBox txtQty = e.Control as TextBox;
                if (txtQty != null)
                {
                    txtQty.KeyPress -= new KeyPressEventHandler(Qty_KeyPress);
                    txtQty.KeyPress += new KeyPressEventHandler(Qty_KeyPress);
                }
            }
        }

        private void UpdateDateTime(object sender, EventArgs e)
        {
            textBox9.Text = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");
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
                        this.Invoke((MethodInvoker)delegate
                        {
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
            bool itemExists = false;
            try
            {
                connection.Open(); // Membuka koneksi sebelum melakukan operasi database
                string query = "SELECT * FROM Items WHERE Barcode = @Barcode";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Barcode", barcode);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            itemExists = true; // Set itemExists to true if item is found
                            string namaItem = reader["NamaItem"].ToString();
                            int qty = 1;
                            decimal harga = Convert.ToDecimal(reader["HJualPcs"]);
                            int sisaQty = Convert.ToInt32(reader["Stok"]);

                            textBox16.Text = namaItem;
                            textBox15.Text = qty.ToString();
                            textBox12.Text = sisaQty.ToString();
                            textBox13.Text = harga.ToString("N0");

                            // Cek apakah barang sudah ada di dataGridViewJualan
                            bool itemFound = false;
                            foreach (DataGridViewRow row in dataGridViewJualan.Rows)
                            {
                                if (row.Cells["Kode"].Value != null && row.Cells["Kode"].Value.ToString() == barcode)
                                {
                                    row.Cells["Qty"].Value = Convert.ToInt32(row.Cells["Qty"].Value) + 1;
                                    row.Cells["SubTotal"].Value = Convert.ToDecimal(row.Cells["SubTotal"].Value) + harga;
                                    itemFound = true;
                                    break;
                                }
                            }

                            // Jika barang tidak ditemukan, tambahkan sebagai baris baru
                            if (!itemFound)
                            {
                                dataGridViewJualan.Rows.Add(new object[] { barcode, namaItem, qty, harga, harga * qty });
                            }

                            // Update total setelah menambahkan barang
                            UpdateTotal();
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
                connection.Close(); // Menutup koneksi setelah operasi selesai
            }

            // Tampilkan pesan jika barang tidak ditemukan
            if (!itemExists)
            {
                MessageBox.Show("Barang tidak ditemukan!"); // Tampilkan pesan ini jika itemExists tetap false
            }
        }






        private void UpdateTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dataGridViewJualan.Rows)
            {
                if (row.Cells["SubTotal"].Value != null)
                {
                    total += Convert.ToDecimal(row.Cells["SubTotal"].Value);
                }
            }
            textBox2.Text = total.ToString("N0");
            textBox4.Text = Terbilang((int)total) + " RUPIAH";
        }


        private string Terbilang(int angka)
        {
            if (angka == 0) return "";
            if (angka == 1) return " SATU";
            if (angka == 2) return " DUA";
            if (angka == 3) return " TIGA";
            if (angka == 4) return " EMPAT";
            if (angka == 5) return " LIMA";
            if (angka == 6) return " ENAM";
            if (angka == 7) return " TUJUH";
            if (angka == 8) return " DELAPAN";
            if (angka == 9) return " SEMBILAN";
            if (angka == 10) return " SEPULUH";
            if (angka == 11) return " SEBELAS";

            if (angka < 20) return Terbilang(angka - 10) + " BELAS";
            if (angka < 100) return Terbilang(angka / 10) + " PULUH" + Terbilang(angka % 10);
            if (angka < 200) return " SERATUS" + Terbilang(angka - 100);
            if (angka < 1000) return Terbilang(angka / 100) + " RATUS" + Terbilang(angka % 100);
            if (angka < 2000) return " SERIBU" + Terbilang(angka - 1000);
            if (angka < 1000000) return Terbilang(angka / 1000) + " RIBU" + Terbilang(angka % 1000);
            if (angka < 1000000000) return Terbilang(angka / 1000000) + " JUTA" + Terbilang(angka % 1000000);

            return angka.ToString();
        }

        private void textBox23_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    // Nominal uang dari customer
                    decimal bayar = Convert.ToDecimal(textBox23.Text.Replace(".", ""));

                    // Total belanja
                    decimal totalBelanja = Convert.ToDecimal(textBox2.Text.Replace(",", ""));

                    // Kembalian
                    decimal kembalian = bayar - totalBelanja;

                    // Menampilkan kembalian
                    textBox2.Text = kembalian.ToString("N0");
                    label7.Text = "KEMBALIAN";
                    textBox4.Text = Terbilang((int)bayar) + " RUPIAH";
                    textBox3.Text = "DIBAYAR";

                    // Pesan konfirmasi pembayaran
                    DialogResult result = MessageBox.Show("Apakah Anda ingin membayar dengan tunai?", "Konfirmasi Pembayaran", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        MessageBox.Show("Pembayaran berhasil!");
                        ResetForm(); // Reset form untuk nota baru
                    }
                }
                catch (FormatException)
                {
                    // MessageBox.Show("Format jumlah pembayaran tidak valid.");
                }
            }
        }


        //titik pada textbox23 = belum bisa
        private void textBox23_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox23.Text, out decimal amount))
            {
                textBox23.TextChanged -= textBox23_TextChanged;
                textBox23.Text = string.Format("{0:N0}", amount);
                textBox23.SelectionStart = textBox23.Text.Length;
                textBox23.TextChanged += textBox23_TextChanged;
            }
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox13.Text, out decimal amount))
            {
                textBox13.TextChanged -= textBox13_TextChanged;
                textBox13.Text = string.Format("{0:N0}", amount);
                textBox13.SelectionStart = textBox13.Text.Length;
                textBox13.TextChanged += textBox13_TextChanged;
            }
        }


        private void textBox23_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Hanya izinkan angka, titik, dan karakter kontrol (seperti backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            // Hanya satu titik yang diperbolehkan
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }


     




        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ScanBarang(textBox3.Text);
                textBox3.Clear();
            }
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

        private void ResetForm()
        {
            //reset semua + nota baru/database baru
            textBox16.Text = "";
            textBox15.Text = "1";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox4.Text = "";
            textBox23.Text = "";
            dataGridViewJualan.Rows.Clear();
            label7.Text = "CAHAYA ANUGRAH";
            textBox3.Text = "";
        }

        private void textBoxNama_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                int selectionStart = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;
                textBox.TextChanged -= textBoxNama_TextChanged;
                textBox.Text = textBox.Text.ToUpper();
                textBox.SelectionStart = selectionStart;
                textBox.SelectionLength = selectionLength;
                textBox.TextChanged += textBoxNama_TextChanged;
            }
        }

        private void textBoxNama2_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                int selectionStart = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;
                textBox.TextChanged -= textBoxNama_TextChanged;
                textBox.Text = textBox.Text.ToUpper();
                textBox.SelectionStart = selectionStart;
                textBox.SelectionLength = selectionLength;
                textBox.TextChanged += textBoxNama_TextChanged;
            }
        }
    }
}
