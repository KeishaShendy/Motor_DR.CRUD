using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class frmRekapMahasiswa_Load : Form
    {
        // Connection string disesuaikan dengan laptopmu
        static string connectionString = "Data Source=LAPTOP-MBD0B33T\\SHENDY;Initial Catalog=DBAkademikADO;User ID=sa;Password=Password123";

        SqlConnection conn = new SqlConnection(connectionString);
        SqlDataAdapter da;
        DataTable dtMahasiswa;
        DataTable dtProdi;

        public frmRekapMahasiswa_Load()
        {
            InitializeComponent();
        }

        private void frmRekapMahasiswa_Load_Load(object sender, EventArgs e)
        {
            // Pengaturan Tahun Masuk (Hanya menampilkan Tahun)
            dtpTahunMasuk.Format = DateTimePickerFormat.Custom;
            dtpTahunMasuk.CustomFormat = "yyyy";
            dtpTahunMasuk.ShowUpDown = true;
            dtpTahunMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTahunMasuk.MaxDate = DateTime.Now;

            cmbProdi.DropDownStyle = ComboBoxStyle.DropDownList;
            btnCetak.Enabled = false; // Tombol cetak dimatikan sebelum data di-load 

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                // Load data Prodi ke ComboBox
                SqlCommand cmd = new SqlCommand("select namaprodi from programstudi", conn);
                cmd.CommandType = CommandType.Text;
                dtProdi = new DataTable();
                da = new SqlDataAdapter(cmd);
                da.Fill(dtProdi);

                cmbProdi.DataSource = dtProdi;
                cmbProdi.DisplayMember = "namaprodi";
                cmbProdi.ValueMember = "namaprodi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Load data prodi: " + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd = new SqlCommand("sp_Report", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Menambahkan parameter Stored Procedure
                cmd.Parameters.Add("@inProdi", SqlDbType.VarChar, 50).Value = cmbProdi.SelectedValue;
                cmd.Parameters.Add("@inTglMsuk", SqlDbType.VarChar, 4).Value = dtpTahunMasuk.Value.Year.ToString();

                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);

                dgvMahasiswa.DataSource = dtMahasiswa;

                // Validasi: tombol cetak aktif jika data ditemukan
                if (dtMahasiswa.Rows.Count > 0)
                {
                    btnCetak.Enabled = true;
                }
                else
                {
                    btnCetak.Enabled = false;
                    MessageBox.Show("Data tidak ditemukan untuk prodi dan tahun tersebut.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Load data: " + ex.Message);
            }
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            // BAGIAN PALING PENTING: Mengirim parameter Prodi & Tahun ke Form2
            Form2 frmReport = new Form2(cmbProdi.SelectedValue.ToString(), dtpTahunMasuk.Value);
            frmReport.Show();
        }
    }
}