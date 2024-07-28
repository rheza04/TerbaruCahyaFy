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
            InitializeDataGridView(); 
            InitializeCustomComponents();
            StartTcpServer();
            SetupDataGridView();

           
            this.KeyDown += new KeyEventHandler(Kasir_KeyDown);
            this.KeyPreview = true; 
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

        private void InitializeCustomComponents()
        {
          
            timer = new Timer();
            timer.Interval = 5000;
            timer.Tick += new EventHandler(CheckStatus);
            timer.Start();

            Timer dateTimeTimer = new Timer();
            dateTimeTimer.Interval = 1000; 
            dateTimeTimer.Tick += new EventHandler(UpdateDateTime);
            dateTimeTimer.Start();

            SetNetworkStatus();
            SetIPAddress();

           
            textBox1.Text = DateTime.Now.ToString("dd MMMM yyyy");

        
            textBox3.AutoCompleteMode = AutoCompleteMode.Append;
            textBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;
            UpdateAutoCompleteSource();

          
            textBox3.TextChanged += new EventHandler(textBox3_TextChanged);
            textBox3.KeyDown += new KeyEventHandler(textBox3_KeyDown);

            textBox23.KeyDown += new KeyEventHandler(textBox23_KeyDown);
            textBox23.TextChanged += new EventHandler(textBox23_TextChanged);
            textBox13.TextChanged += new EventHandler(textBox13_TextChanged);
            textBoxNama.TextChanged += new EventHandler(textBoxNama_TextChanged);
            textBoxNama2.TextChanged += new EventHandler(textBoxNama2_TextChanged);
            textBox23.KeyPress += new KeyPressEventHandler(textBox23_KeyPress);

            dataGridViewJualan.CellValueChanged += new DataGridViewCellEventHandler(dataGridViewJualan_CellValueChanged);
            dataGridViewJualan.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridViewJualan_EditingControlShowing);

            dataGridViewQR.CellDoubleClick += new DataGridViewCellEventHandler(dataGridViewQR_CellDoubleClick);
            dataGridViewQR.KeyDown += new KeyEventHandler(dataGridViewQR_KeyDown);
        }

        private void Kasir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 || e.KeyCode == Keys.Space)
            {
                if (dataGridViewQR.Visible)
                {
                    dataGridViewQR.Visible = false;
                    dataGridViewQR.SendToBack();
                    dataGridViewJualan.Visible = true;
                    dataGridViewJualan.BringToFront();
                }
                textBox3.Focus();
                e.Handled = true; 
                Console.WriteLine("dataGridViewQR hidden (if visible) and focus set to textBox3 by F2 or Space key press.");
            }
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


        private void dataGridViewQR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridViewQR.Rows[e.RowIndex];
                string barcode = row.Cells["Barcode"].Value.ToString();
                AddItemToDataGridViewJualan(barcode);
                dataGridViewQR.Visible = false;
                dataGridViewQR.SendToBack();
                dataGridViewJualan.Visible = true;
                dataGridViewJualan.BringToFront();
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
                    Console.WriteLine("Hiding dataGridViewQR after enter key.");
                    dataGridViewQR.Visible = false;
                    dataGridViewQR.SendToBack();
                    dataGridViewJualan.Visible = true;
                    dataGridViewJualan.BringToFront();
                    textBox3.Clear();
                    e.Handled = true;
                    Console.WriteLine($"dataGridViewQR visibility after hiding: {dataGridViewQR.Visible}");
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
                            string id = reader["ID"].ToString(); 
                            string namaItem = reader["NamaItem"].ToString();
                            int qty = 1;
                            decimal harga = Convert.ToDecimal(reader["HJualItem"]);
                            int sisaQty = Convert.ToInt32(reader["Stok"]);

                            bool itemFound = false;
                            foreach (DataGridViewRow dataRow in dataGridViewJualan.Rows)
                            {
                                if (dataRow.Cells["ID"].Value != null && dataRow.Cells["ID"].Value.ToString() == id)
                                {
                                    dataRow.Cells["Qty"].Value = Convert.ToInt32(dataRow.Cells["Qty"].Value) + 1;
                                    dataRow.Cells["SubTotal"].Value = Convert.ToDecimal(dataRow.Cells["SubTotal"].Value) + harga;
                                    itemFound = true;
                                    break;
                                }
                            }

                            if (!itemFound)
                            {
                                dataGridViewJualan.Rows.Add(new object[] { id, barcode, namaItem, qty, harga, harga * qty });
                            }

                            UpdateTotal();
                        }
                        else
                        {
                            MessageBox.Show("Item tidak ditemukan");
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

                AutoCompleteStringCollection suggestions = new AutoCompleteStringCollection();
                foreach (DataRow row in dt.Rows)
                {
                    suggestions.Add(row["NamaItem"].ToString());
                }
                textBox3.AutoCompleteCustomSource = suggestions;

                if (dt.Rows.Count > 0)
                {
                    dataGridViewQR.Visible = true;
                    dataGridViewQR.BringToFront();
                    dataGridViewJualan.Visible = false;
                }
                else
                {
                    dataGridViewQR.Visible = false;
                    dataGridViewQR.SendToBack();
                    dataGridViewJualan.Visible = true;
                    dataGridViewJualan.BringToFront();
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
            dataGridViewJualan.Columns.Add("ID", "ID");
            dataGridViewJualan.Columns.Add("Barcode", "Barcode");
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
           
            Console.WriteLine("CellValueChanged triggered. ColumnIndex: " + e.ColumnIndex);
            if (dataGridViewJualan.Columns["Qty"] != null)
            {
                Console.WriteLine("Qty column index: " + dataGridViewJualan.Columns["Qty"].Index);
            }
            else
            {
                Console.WriteLine("Qty column not found!");
            }

            if (e.ColumnIndex == dataGridViewJualan.Columns["Qty"].Index)
            {
                UpdateSubTotal(e.RowIndex);
                UpdateTotal();
            }
        }

        private void Qty_KeyPress(object sender, KeyPressEventArgs e)
        {
          
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
                    int port = 5789; 
                    tcpListener = new TcpListener(IPAddress.Any, port);
                    tcpListener.Start();

                    while (true)
                    {
                        var client = tcpListener.AcceptTcpClient();
                        var stream = client.GetStream();
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string barcode = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                       
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


        private void ScanBarang(string barcode)
        {
            bool itemExists = false;
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
                            itemExists = true;
                            string id = reader["ID"].ToString();
                            string namaItem = reader["NamaItem"].ToString();
                            int qty = 1;
                            decimal harga = Convert.ToDecimal(reader["HJualItem"]);
                            int sisaQty = Convert.ToInt32(reader["Stok"]);

                            textBox16.Text = namaItem;
                            textBox15.Text = qty.ToString();
                            textBox12.Text = sisaQty.ToString();
                            textBox13.Text = harga.ToString("N0");

                          
                            bool itemFound = false;
                            foreach (DataGridViewRow row in dataGridViewJualan.Rows)
                            {
                                if (row.Cells["ID"].Value != null && row.Cells["ID"].Value.ToString() == id)
                                {
                                    row.Cells["Qty"].Value = Convert.ToInt32(row.Cells["Qty"].Value) + 1;
                                    row.Cells["SubTotal"].Value = Convert.ToDecimal(row.Cells["SubTotal"].Value) + harga;
                                    itemFound = true;
                                    break;
                                }
                            }

                         
                            if (!itemFound)
                            {
                                dataGridViewJualan.Rows.Add(new object[] { id, barcode, namaItem, qty, harga, harga * qty });
                            }

                         
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
                connection.Close();
            }

            if (!itemExists)
            {
                MessageBox.Show("Barang tidak ditemukan!");
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
                 
                    decimal bayar = Convert.ToDecimal(textBox23.Text.Replace(".", ""));

                 
                    decimal totalBelanja = Convert.ToDecimal(textBox2.Text.Replace(",", ""));

                 
                    decimal kembalian = bayar - totalBelanja;

                 
                    textBox2.Text = kembalian.ToString("N0");
                    label7.Text = "KEMBALIAN";
                    textBox4.Text = Terbilang((int)bayar) + " RUPIAH";
                    textBox3.Text = "DIBAYAR";

                
                    DialogResult result = MessageBox.Show("Apakah Anda ingin membayar dengan tunai?", "Konfirmasi Pembayaran", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        MessageBox.Show("Pembayaran berhasil!");
                        ResetForm(); 
                    }
                }
                catch (FormatException)
                {
                   
                }
            }
        }
      
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
           
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
          
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
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
               
                int selectionStart = textBox3.SelectionStart;
                if (textBox3.Text.Length > 0 && selectionStart > 0)
                {
                    textBox3.Text = textBox3.Text.Remove(selectionStart - 1, 1);
                    textBox3.SelectionStart = selectionStart - 1;
                }
                e.Handled = true; 
            }
        }

        private void CheckStatus(object sender, EventArgs e)
        {
           
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
          
            textBox16.Text = "";
            textBox15.Text = "1";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox4.Text = "";
            textBox23.Text = "";
            dataGridViewJualan.Rows.Clear();
            label7.Text = "CAHAYA ANUGRAH";
            textBox3.Text = "";
            textBox2.Text = "";
            dataGridViewQR.Visible = false;
            dataGridViewQR.SendToBack();
            dataGridViewJualan.Visible = true;
            dataGridViewJualan.BringToFront();
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
                textBox.TextChanged -= textBoxNama2_TextChanged;
                textBox.Text = textBox.Text.ToUpper();
                textBox.SelectionStart = selectionStart;
                textBox.SelectionLength = selectionLength;
                textBox.TextChanged += textBoxNama2_TextChanged;
            }
        }
    }
}
