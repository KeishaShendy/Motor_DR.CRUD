namespace CRUDMahasiswaADO
{
    partial class FrmDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.lblJudul = new System.Windows.Forms.Label();
            this.lblTahunMasuk = new System.Windows.Forms.Label();
            this.dtpTahunMasuk = new System.Windows.Forms.DateTimePicker();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.cboFilter = new System.Windows.Forms.ComboBox();
            this.chartMahasiswa = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnDataMahasiswa = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chartMahasiswa)).BeginInit();
            this.SuspendLayout();
            // 
            // lblJudul
            // 
            this.lblJudul.AutoSize = true;
            this.lblJudul.Location = new System.Drawing.Point(262, 9);
            this.lblJudul.Name = "lblJudul";
            this.lblJudul.Size = new System.Drawing.Size(177, 16);
            this.lblJudul.TabIndex = 0;
            this.lblJudul.Text = "REKAP DATA MAHASISWA";
            // 
            // lblTahunMasuk
            // 
            this.lblTahunMasuk.AutoSize = true;
            this.lblTahunMasuk.Location = new System.Drawing.Point(34, 64);
            this.lblTahunMasuk.Name = "lblTahunMasuk";
            this.lblTahunMasuk.Size = new System.Drawing.Size(88, 16);
            this.lblTahunMasuk.TabIndex = 1;
            this.lblTahunMasuk.Text = "Tahun Masuk";
            // 
            // dtpTahunMasuk
            // 
            this.dtpTahunMasuk.Location = new System.Drawing.Point(139, 64);
            this.dtpTahunMasuk.Name = "dtpTahunMasuk";
            this.dtpTahunMasuk.Size = new System.Drawing.Size(200, 22);
            this.dtpTahunMasuk.TabIndex = 2;
            this.dtpTahunMasuk.ValueChanged += new System.EventHandler(this.dtp_TahunMasuk_ValueChanged);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(345, 64);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btn_Load_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(426, 66);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // cboFilter
            // 
            this.cboFilter.FormattingEnabled = true;
            this.cboFilter.Location = new System.Drawing.Point(621, 61);
            this.cboFilter.Name = "cboFilter";
            this.cboFilter.Size = new System.Drawing.Size(121, 24);
            this.cboFilter.TabIndex = 5;
            this.cboFilter.SelectedIndexChanged += new System.EventHandler(this.cbo_Filter_SelectedIndexChanged);
            // 
            // chartMahasiswa
            // 
            chartArea5.Name = "ChartArea1";
            this.chartMahasiswa.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            this.chartMahasiswa.Legends.Add(legend5);
            this.chartMahasiswa.Location = new System.Drawing.Point(179, 95);
            this.chartMahasiswa.Name = "chartMahasiswa";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.chartMahasiswa.Series.Add(series5);
            this.chartMahasiswa.Size = new System.Drawing.Size(398, 304);
            this.chartMahasiswa.TabIndex = 6;
            this.chartMahasiswa.Text = "chart1";
            this.chartMahasiswa.Click += new System.EventHandler(this.chartMahasiswa_Click);
            // 
            // btnDataMahasiswa
            // 
            this.btnDataMahasiswa.Location = new System.Drawing.Point(580, 415);
            this.btnDataMahasiswa.Name = "btnDataMahasiswa";
            this.btnDataMahasiswa.Size = new System.Drawing.Size(75, 23);
            this.btnDataMahasiswa.TabIndex = 7;
            this.btnDataMahasiswa.Text = "Data Mahasiswa";
            this.btnDataMahasiswa.UseVisualStyleBackColor = true;
            this.btnDataMahasiswa.Click += new System.EventHandler(this.btn_DataMahasiswa_Click);
            // 
            // FrmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnDataMahasiswa);
            this.Controls.Add(this.chartMahasiswa);
            this.Controls.Add(this.cboFilter);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.dtpTahunMasuk);
            this.Controls.Add(this.lblTahunMasuk);
            this.Controls.Add(this.lblJudul);
            this.Name = "FrmDashboard";
            this.Text = "Dashboard";
            this.Load += new System.EventHandler(this.FrmDashboard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartMahasiswa)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblJudul;
        private System.Windows.Forms.Label lblTahunMasuk;
        private System.Windows.Forms.DateTimePicker dtpTahunMasuk;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ComboBox cboFilter;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartMahasiswa;
        private System.Windows.Forms.Button btnDataMahasiswa;
    }
}