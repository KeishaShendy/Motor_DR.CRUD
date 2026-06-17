using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace CRUDMahasiswaADO
{
    public partial class Form2 : Form
    {
        // 1. Connection string milikmu
        static string connectionString = "Data Source=LAPTOP-MBD0B33T\\SHENDY;Initial Catalog=DBAkademikADO;User ID=sa;Password=Password123";

        SqlConnection conn = new SqlConnection(connectionString);
        SqlDataAdapter da;
        DataTable dtMahasiswa;

        // 2. Memanggil file report milikmu
        CrystalReport1 laporanSaya = new CrystalReport1();

        // 3. Variabel penampung parameter dari Form Filter (Form3)
        string prodi { get; set; }
        DateTime tglmasuk { get; set; }

        // Constructor dirubah untuk menerima parameter, bukan DataTable
        public Form2(string Prodi, DateTime TglMasuk)
        {
            InitializeComponent();

            prodi = Prodi;
            tglmasuk = TglMasuk;

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                // 4. Eksekusi Stored Procedure langsung di Form Report
                SqlCommand cmd = new SqlCommand("sp_Report", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inProdi", prodi);
                cmd.Parameters.AddWithValue("@inTglMsuk", tglmasuk.Year);

                da = new SqlDataAdapter(cmd);

                dtMahasiswa = new DataTable();
  