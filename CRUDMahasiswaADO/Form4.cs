using System;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CRUDMahasiswaADO
{
    public partial class FrmDashboard : Form
    {
        // Memanggil class DAL yang Anda kirimkan
        DAL dbLogic = new DAL();
        int button = 0;

        public FrmDashboard()
        {
            InitializeComponent();
        }

        private void FrmDashboard_Load(object sender, EventArgs e)
        {
            dtpTahunMasuk.Format = DateTimePickerFormat.Custom;
            dtpTahunMasuk.CustomFormat = "yyyy";
            dtpTahunMasuk.ShowUpDown = true;

            cboFilter.Items.Clear();
            cboFilter.Items.Add("Keseluruhan");
            cboFilter.Items.Add("Berdasarkan Tahun");
            cboFilter.SelectedIndex = 0;

            loadDataChart();
        }

        public void loadDataChart()
        {
            chartMahasiswa.Series.Clear();
            chartMahasiswa.Titles.Clear();
            chartMahasiswa.Legends.Clear();
            chartMahasiswa.ChartAreas.Clear();

            ChartArea ca = new ChartArea("MainArea");
            ca.AxisX.Title = "Program Studi";
            ca.AxisY.Title = "Jumlah Mahasiswa";
            ca.AxisX.LabelStyle.Angle = -45;
            chartMahasiswa.ChartAreas.Add(ca);

            try
            {
                // Mengambil data dari DAL (mengembalikan DataSet)
                DataSet ds;
                if (button == 1)
                {
                    ds = dbLogic.DashboardByTahun(dtpTahunMasuk.Value.Year.ToString());
                }
                else
                {
                    ds = dbLogic.Dashboard();
                }

                // Mengambil DataTable dari DataSet dengan nama tabel "Dashboard"
                if (ds != null && ds.Tables["Dashboard"].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables["Dashboard"];

                    Series s = new Series("Mahasiswa");
                    s.ChartType = SeriesChartType.Column;

                    foreach (DataRow row in dt.Rows)
                    {
                        string prodi = row["NamaProdi"].ToString();
                        // Pastikan nama kolom di SQL sesuai dengan yang di bawah (JmlMhs)
                        int jumlah = Convert.ToInt32(row["JmlMhs"]);
                        s.Points.AddXY(prodi, jumlah);
                    }
                    chartMahasiswa.Series.Add(s);
                }
                else
                {
                    MessageBox.Show("Tidak ada data untuk ditampilkan.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            button = 1;
            loadDataChart();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            button = 0;
            loadDataChart();
        }

        private void btn_DataMahasiswa_Click(object sender, EventArgs e)
        {
            Form1 frm1 = new Form1();
            frm1.Show();
            this.Hide();
        }

        private void cbo_Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFilter.SelectedIndex == 0) // Keseluruhan
            {
                dtpTahunMasuk.Enabled = false;
                btnLoad.Enabled = false;
                button = 0;
                loadDataChart();
            }
            else if (cboFilter.SelectedIndex == 1) // Berdasarkan Tahun
            {
                dtpTahunMasuk.Enabled = true;
                btnLoad.Enabled = true;
                button = 1;
                loadDataChart();
            }
        }

        private void dtp_TahunMasuk_ValueChanged(object sender, EventArgs e)
        {
            // Kosong - event handler untuk DateTimePicker
        }

        private void chartMahasiswa_Click(object sender, EventArgs e)
        {
            // Kosong - event handler untuk Chart
        }
    }
}