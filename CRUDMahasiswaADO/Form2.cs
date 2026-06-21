using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace CRUDMahasiswaADO
{
    public partial class Form2 : Form
    {
        string connectionString = "Data Source=LAPTOP-MBD0B33T\\SHENDY;Initial Catalog=DBAkademikADO;User ID=sa;Password=Password123";
        SqlConnection conn;
        SqlDataAdapter da;
        DataTable dtMahasiswa;

        // Ganti dengan nama Report Anda (sesuaikan dengan nama file .rpt Anda)
        CrystalReport1 laporanSaya = new CrystalReport1();

        string prodi { get; set; }
        DateTime tglmasuk { get; set; }

        public Form2(string Prodi, DateTime TglMasuk)
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);

            prodi = Prodi;
            tglmasuk = TglMasuk;

            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd = new SqlCommand("sp_Report", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inProdi", prodi);
                cmd.Parameters.AddWithValue("@inTglMsuk", tglmasuk.Year.ToString());

                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);

                if (dtMahasiswa.Rows.Count > 0)
                {
                    laporanSaya.SetDataSource(dtMahasiswa);
                    crystalReportViewer1.ReportSource = laporanSaya;
                    crystalReportViewer1.Refresh();
                }
                else
                {
                    MessageBox.Show("Tidak ada data untuk dicetak pada Prodi dan Tahun tersebut.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat memuat laporan: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            // Kosong
        }
    }
}