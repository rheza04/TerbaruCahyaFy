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

        private async Task<string> GetIPAddress()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // Mendapatkan IP Publik
                using (var client = new HttpClient())
                {
                    return await client.GetStringAsync("https://api.ipify.org");
                }
            }
            else
            {
                // Mendapatkan IP Lokal
                return GetLocalIPAddress();
            }
        }
        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Tidak ada jaringan yang tersambung!");
        }

        private async void CheckIPAddress(object sender, EventArgs e)
        {
            string newIPAddress = await GetIPAddress();
            if (newIPAddress != currentIPAddress)
            {
                currentIPAddress = newIPAddress;
                textBox22.Text = "PC : " + currentIPAddress;
            }
        }

        private async void SetIPAddress()
        {
            currentIPAddress = await GetIPAddress();
            textBox22.Text = "PC : " + currentIPAddress;
        }

        private void InitializeCustomComponents()
        {
            // Timer cek Ip
            timer = new Timer();
            timer.Interval = 10000; // 5 detik
            timer.Tick += new EventHandler(CheckIPAddress);
            timer.Start();

            // Ip pertama
            SetIPAddress();

            // tgl
            textBox1.Text = DateTime.Now.ToString("dd MMMM yyyy");

            // status jaringan
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                textBox21.Text = "Lokal Online";
            }
            else
            {
                textBox21.Text = "Lokal Offline";
            }

            // ip address
            string localIP = GetLocalIPAddress();
            textBox22.Text = "PC : " + localIP;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
