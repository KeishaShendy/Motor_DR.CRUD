using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class frmRekapMahasiswa_Load : Form
    {
        private string connectionString =
            "Data Source=LAPTOP-MBD0B33T\\SHENDY;Initial Catalog=DBAkademikADO;User ID=sa;Password=Password123";
        private SqlConnection conn;
        private SqlDataAdapter da;
        private DataTable dtMahasiswa;
        private DataTable dtProdi;

        public frmRekapMahasiswa_Load()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }

        private void frmRekapMahasiswa_Load_Load(object sender, EventArgs e)
        {
            // SETUP TAHUN MASUK
            dtpTahunMasuk.Format = DateTimePickerFormat.Custom;
            dtpTahunMasuk.CustomFormat = "yyyy";
            dtpTahunMasuk.ShowUpDown = true;
            dtpTahunMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTahunMasuk.MaxDate = DateTime.Now;
            dtpTahunMasuk.Value = DateTime.Now;

            // SETUP COMBOBOX
            cmbProdi.DropDownStyle = ComboBoxStyle.DropDownList;
            btnCetak.Enabled = false;

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT NamaProdi FROM ProgramStudi", conn);
                dtProdi = new DataTable();
                da = new SqlDataAdapter(cmd);
                da.Fill(dtProdi);

                cmbProdi.DataSource = dtProdi;
                cmbProdi.DisplayMember = "NamaProdi";
                cmbProdi.ValueMember = "NamaProdi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Load data prodi: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Report", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inProdi", cmbProdi.SelectedValue.ToString());
                cmd.Parameters.AddWithValue("@inTglMsuk", dtpTahunMasuk.Value.Year.ToString());

                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);

                dgvMahasiswa.DataSource = dtMahasiswa;

                if (dtMahasiswa.Rows.Count > 0)
                {
                    btnCetak.Enabled = true;
                }
                else
                {
                    btnCetak.Enabled = false;
                    MessageBox.Show("Data tidak ditemukan untuk prodi dan tahun tersebut.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Load data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            Form2 frmReport = new Form2(cmbProdi.SelectedValue.ToString(), dtpTahunMasuk.Value);
            frmReport.Show();
        }
    }
}