using System.Windows.Forms;

namespace TerbaruCahyaFy
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelHeader = new System.Windows.Forms.Label();
            this.tableLayoutPanelKiriKananPemisah = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelKiri = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelPenjualan = new System.Windows.Forms.TableLayoutPanel();
            this.labeltbPmbln = new System.Windows.Forms.Label();
            this.labelPenjualan = new System.Windows.Forms.Label();
            this.labelinputBrg = new System.Windows.Forms.Label();
            this.labeltbPnjln = new System.Windows.Forms.Label();
            this.labelinputSup = new System.Windows.Forms.Label();
            this.labellistPnjln = new System.Windows.Forms.Label();
            this.labelPembelian = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanelLogo = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelHarga = new System.Windows.Forms.TableLayoutPanel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelKiriKananPemisah.SuspendLayout();
            this.tableLayoutPanelKiri.SuspendLayout();
            this.tableLayoutPanelPenjualan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanelLogo.SuspendLayout();
            this.tableLayoutPanelHarga.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.labelHeader, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelKiriKananPemisah, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelLogo, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 71F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.02128F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 62.15805F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.74199F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1350, 729);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.BackColor = System.Drawing.Color.RoyalBlue;
            this.labelHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeader.Font = new System.Drawing.Font("MV Boli", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeader.ForeColor = System.Drawing.SystemColors.Control;
            this.labelHeader.Location = new System.Drawing.Point(3, 0);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(1344, 71);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "Dobit";
            this.labelHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanelKiriKananPemisah
            // 
            this.tableLayoutPanelKiriKananPemisah.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tableLayoutPanelKiriKananPemisah.ColumnCount = 2;
            this.tableLayoutPanelKiriKananPemisah.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.98569F));
            this.tableLayoutPanelKiriKananPemisah.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.01431F));
            this.tableLayoutPanelKiriKananPemisah.Controls.Add(this.tableLayoutPanelKiri, 0, 0);
            this.tableLayoutPanelKiriKananPemisah.Controls.Add(this.dataGridView1, 1, 0);
            this.tableLayoutPanelKiriKananPemisah.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelKiriKananPemisah.Location = new System.Drawing.Point(3, 186);
            this.tableLayoutPanelKiriKananPemisah.Name = "tableLayoutPanelKiriKananPemisah";
            this.tableLayoutPanelKiriKananPemisah.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanelKiriKananPemisah.RowCount = 1;
            this.tableLayoutPanelKiriKananPemisah.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelKiriKananPemisah.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 401F));
            this.tableLayoutPanelKiriKananPemisah.Size = new System.Drawing.Size(1344, 403);
            this.tableLayoutPanelKiriKananPemisah.TabIndex = 1;
            // 
            // tableLayoutPanelKiri
            // 
            this.tableLayoutPanelKiri.AutoScroll = true;
            this.tableLayoutPanelKiri.BackColor = System.Drawing.Color.Silver;
            this.tableLayoutPanelKiri.ColumnCount = 1;
            this.tableLayoutPanelKiri.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelKiri.Controls.Add(this.tableLayoutPanelPenjualan, 0, 0);
            this.tableLayoutPanelKiri.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanelKiri.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelKiri.Name = "tableLayoutPanelKiri";
            this.tableLayoutPanelKiri.RowCount = 2;
            this.tableLayoutPanelKiri.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelKiri.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelKiri.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelKiri.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelKiri.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelKiri.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelKiri.Size = new System.Drawing.Size(257, 393);
            this.tableLayoutPanelKiri.TabIndex = 0;
            // 
            // tableLayoutPanelPenjualan
            // 
            this.tableLayoutPanelPenjualan.ColumnCount = 1;
            this.tableLayoutPanelPenjualan.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelPenjualan.Controls.Add(this.labeltbPmbln, 0, 6);
            this.tableLayoutPanelPenjualan.Controls.Add(this.labelPenjualan, 0, 0);
            this.tableLayoutPanelPenjualan.Controls.Add(this.labelinputBrg, 0, 5);
            this.tableLayoutPanelPenjualan.Controls.Add(this.labeltbPnjln, 0, 1);
            this.tableLayoutPanelPenjualan.Controls.Add(this.labelinputSup, 0, 4);
            this.tableLayoutPanelPenjualan.Controls.Add(this.labellistPnjln, 0, 2);
            this.tableLayoutPanelPenjualan.Controls.Add(this.labelPembelian, 0, 3);
            this.tableLayoutPanelPenjualan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelPenjualan.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelPenjualan.Name = "tableLayoutPanelPenjualan";
            this.tableLayoutPanelPenjualan.RowCount = 7;
            this.tableLayoutPanelPenjualan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanelPenjualan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanelPenjualan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanelPenjualan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanelPenjualan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanelPenjualan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanelPenjualan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanelPenjualan.Size = new System.Drawing.Size(251, 319);
            this.tableLayoutPanelPenjualan.TabIndex = 2;
            // 
            // labeltbPmbln
            // 
            this.labeltbPmbln.BackColor = System.Drawing.Color.Transparent;
            this.labeltbPmbln.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labeltbPmbln.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labeltbPmbln.ForeColor = System.Drawing.Color.Black;
            this.labeltbPmbln.Location = new System.Drawing.Point(3, 270);
            this.labeltbPmbln.Name = "labeltbPmbln";
            this.labeltbPmbln.Size = new System.Drawing.Size(245, 49);
            this.labeltbPmbln.TabIndex = 37;
            this.labeltbPmbln.Text = "Tambah Pembelian";
            this.labeltbPmbln.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labeltbPmbln.UseCompatibleTextRendering = true;
            // 
            // labelPenjualan
            // 
            this.labelPenjualan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPenjualan.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPenjualan.Location = new System.Drawing.Point(3, 0);
            this.labelPenjualan.Name = "labelPenjualan";
            this.labelPenjualan.Size = new System.Drawing.Size(245, 45);
            this.labelPenjualan.TabIndex = 31;
            this.labelPenjualan.Text = "Penjualan";
            this.labelPenjualan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelPenjualan.UseCompatibleTextRendering = true;
            // 
            // labelinputBrg
            // 
            this.labelinputBrg.BackColor = System.Drawing.Color.Transparent;
            this.labelinputBrg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelinputBrg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelinputBrg.ForeColor = System.Drawing.Color.Black;
            this.labelinputBrg.Location = new System.Drawing.Point(3, 225);
            this.labelinputBrg.Name = "labelinputBrg";
            this.labelinputBrg.Size = new System.Drawing.Size(245, 45);
            this.labelinputBrg.TabIndex = 36;
            this.labelinputBrg.Text = "Input Barang";
            this.labelinputBrg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelinputBrg.UseCompatibleTextRendering = true;
            // 
            // labeltbPnjln
            // 
            this.labeltbPnjln.BackColor = System.Drawing.Color.Transparent;
            this.labeltbPnjln.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labeltbPnjln.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labeltbPnjln.ForeColor = System.Drawing.Color.Black;
            this.labeltbPnjln.Location = new System.Drawing.Point(3, 45);
            this.labeltbPnjln.Name = "labeltbPnjln";
            this.labeltbPnjln.Size = new System.Drawing.Size(245, 45);
            this.labeltbPnjln.TabIndex = 32;
            this.labeltbPnjln.Text = "Tambah Penjualan";
            this.labeltbPnjln.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labeltbPnjln.UseCompatibleTextRendering = true;
            // 
            // labelinputSup
            // 
            this.labelinputSup.BackColor = System.Drawing.Color.Transparent;
            this.labelinputSup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelinputSup.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelinputSup.ForeColor = System.Drawing.Color.Black;
            this.labelinputSup.Location = new System.Drawing.Point(3, 180);
            this.labelinputSup.Name = "labelinputSup";
            this.labelinputSup.Size = new System.Drawing.Size(245, 45);
            this.labelinputSup.TabIndex = 35;
            this.labelinputSup.Text = "Input Supplier";
            this.labelinputSup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelinputSup.UseCompatibleTextRendering = true;
            // 
            // labellistPnjln
            // 
            this.labellistPnjln.BackColor = System.Drawing.Color.Transparent;
            this.labellistPnjln.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labellistPnjln.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labellistPnjln.ForeColor = System.Drawing.Color.Black;
            this.labellistPnjln.Location = new System.Drawing.Point(3, 90);
            this.labellistPnjln.Name = "labellistPnjln";
            this.labellistPnjln.Size = new System.Drawing.Size(245, 45);
            this.labellistPnjln.TabIndex = 34;
            this.labellistPnjln.Text = "List Data Penjualan";
            this.labellistPnjln.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labellistPnjln.UseCompatibleTextRendering = true;
            // 
            // labelPembelian
            // 
            this.labelPembelian.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPembelian.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPembelian.Location = new System.Drawing.Point(3, 135);
            this.labelPembelian.Name = "labelPembelian";
            this.labelPembelian.Size = new System.Drawing.Size(245, 45);
            this.labelPembelian.TabIndex = 33;
            this.labelPembelian.Text = "Pembelian";
            this.labelPembelian.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelPembelian.UseCompatibleTextRendering = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Silver;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6});
            this.dataGridView1.Location = new System.Drawing.Point(287, 8);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1049, 387);
            this.dataGridView1.TabIndex = 1;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Kode";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Nama Item";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Barcode";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "H. Jual Unit";
            this.Column4.Name = "Column4";
            // 
            // Column5
            // 
            this.Column5.HeaderText = "H. Jual Pcs";
            this.Column5.Name = "Column5";
            // 
            // Column6
            // 
            this.Column6.HeaderText = "H. Jual Spc";
            this.Column6.Name = "Column6";
            // 
            // tableLayoutPanelLogo
            // 
            this.tableLayoutPanelLogo.ColumnCount = 2;
            this.tableLayoutPanelLogo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.24107F));
            this.tableLayoutPanelLogo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.75893F));
            this.tableLayoutPanelLogo.Controls.Add(this.tableLayoutPanelHarga, 1, 0);
            this.tableLayoutPanelLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelLogo.Location = new System.Drawing.Point(3, 74);
            this.tableLayoutPanelLogo.Name = "tableLayoutPanelLogo";
            this.tableLayoutPanelLogo.RowCount = 1;
            this.tableLayoutPanelLogo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLogo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelLogo.Size = new System.Drawing.Size(1344, 106);
            this.tableLayoutPanelLogo.TabIndex = 2;
            // 
            // tableLayoutPanelHarga
            // 
            this.tableLayoutPanelHarga.ColumnCount = 1;
            this.tableLayoutPanelHarga.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelHarga.Controls.Add(this.textBox2, 0, 1);
            this.tableLayoutPanelHarga.Controls.Add(this.textBox1, 0, 0);
            this.tableLayoutPanelHarga.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelHarga.Location = new System.Drawing.Point(1068, 3);
            this.tableLayoutPanelHarga.Name = "tableLayoutPanelHarga";
            this.tableLayoutPanelHarga.RowCount = 2;
            this.tableLayoutPanelHarga.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 37F));
            this.tableLayoutPanelHarga.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 63F));
            this.tableLayoutPanelHarga.Size = new System.Drawing.Size(273, 100);
            this.tableLayoutPanelHarga.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Microsoft New Tai Lue", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(267, 31);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "Cahaya Anugrah";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Font = new System.Drawing.Font("Impact", 27F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(3, 40);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.textBox2.Size = new System.Drawing.Size(267, 57);
            this.textBox2.TabIndex = 2;
            this.textBox2.Text = "6000";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(1350, 729);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelKiriKananPemisah.ResumeLayout(false);
            this.tableLayoutPanelKiri.ResumeLayout(false);
            this.tableLayoutPanelPenjualan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanelLogo.ResumeLayout(false);
            this.tableLayoutPanelHarga.ResumeLayout(false);
            this.tableLayoutPanelHarga.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label labelHeader;
        private TableLayoutPanel tableLayoutPanelKiriKananPemisah;
        private TableLayoutPanel tableLayoutPanelKiri;
        private TableLayoutPanel tableLayoutPanelPenjualan;
        private Label labeltbPmbln;
        private Label labelPenjualan;
        private Label labelinputBrg;
        private Label labeltbPnjln;
        private Label labelinputSup;
        private Label labellistPnjln;
        private Label labelPembelian;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
        private TableLayoutPanel tableLayoutPanelLogo;
        private TableLayoutPanel tableLayoutPanelHarga;
        private TextBox textBox2;
        private TextBox textBox1;
    }
}
