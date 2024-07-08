using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerbaruCahyaFy
{
    public partial class Kasir : Form
    {
        private string currentIPAddress;
        private string currentNetworkStatus;
        private Timer timer;

        public Kasir()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        public Kasir(string Username)
        {
            InitializeComponent();
            textBoxNama.Text = Username;
            textBoxNama2.Text = Username;
            InitializeCustomComponents();
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
    }
}
