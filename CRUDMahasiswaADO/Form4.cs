using System;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CRUDMahasiswaADO
{
    public partial class FrmDashboard : Form
    {
        private DAL dbLogic = new DAL();
        private int button = 0;

        public FrmDashboard()
        {
            InitializeComponent();
        }

        private void FrmDashboard_Load(object sender, EventArgs e)
        {
            // SETUP TAHUN MASUK
            dtpTahunMasuk.Format = DateTimePickerFormat.Custom;
            dtpTahunMasuk.CustomFormat = "yyyy";
            dtpTahunMasuk.ShowUpDown = true;
            dtpTahunMasuk.Enabled = false;

            // SETUP FILTER
            cboFilter.Items.Clear();
            cboFilter.Items.Add("Keseluruhan");
            cboFilter.Items.Add("Berdasarkan Tahun");
            cboFilter.SelectedIndex = 0;

            LoadDataChart();
        }

        public void LoadDataChart()
        {
            chartMahasiswa.Series.Clear();
            chartMahasiswa.Titles.Clear();
            chartMahasiswa.Legends.Clear();
            chartMahasiswa.ChartAreas.Clear();

            // SETUP CHART AREA
            ChartArea ca = new ChartArea("MainArea");
            ca.AxisX.Title = "Program Studi";
            ca.AxisY.Title = "Jumlah Mahasiswa";
            ca.AxisX.LabelStyle.Angle = -45;
            chartMahasiswa.ChartAreas.Add(ca);

            try
            {
                DataSet ds;
                if (button == 1)
                {
                    ds = dbLogic.DashboardByTahun(dtpTahunMasuk.Value.Year.ToString());
                }
                else
                {
                    ds = dbLogic.Dashboard();
                }

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    Series s = new Series("Mahasiswa");
                    s.ChartType = SeriesChartType.Column;

                    foreach (DataRow row in dt.Rows)
                    {
                        string prodi = row["NamaProdi"].ToString();
                        int jumlah = Convert.ToInt32(row["JmlMhs"]);
                        s.Points.AddXY(prodi, jumlah);
                    }

                    chartMahasiswa.Series.Add(s);
                }
                else
                {
                    Series s = new Series("Mahasiswa");
                    s.ChartType = SeriesChartType.Column;
                    s.Points.AddXY("Tidak Ada Data", 0);
                    chartMahasiswa.Series.Add(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            button = 1;
            LoadDataChart();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            button = 0;
            LoadDataChart();
        }

        private void btn_DataMahasiswa_Click(object sender, EventArgs e)
        {
            Form1 frm1 = new Form1();
            frm1.Show();
            this.Hide();
        }

        private void cbo_Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFilter.SelectedIndex == 0)
            {
                dtpTahunMasuk.Enabled = false;
                btnLoad.Enabled = false;
                button = 0;
                LoadDataChart();
            }
            else if (cboFilter.SelectedIndex == 1)
            {
                dtpTahunMasuk.Enabled = true;
                btnLoad.Enabled = true;
                button = 1;
                LoadDataChart();
            }
        }

        private void dtp_TahunMasuk_ValueChanged(object sender, EventArgs e) { }

        private void chartMahasiswa_Click(object sender, EventArgs e) { }
    }
}