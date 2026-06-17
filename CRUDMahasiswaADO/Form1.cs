using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ExcelDataReader;

namespace CRUDMahasiswaADO
{
    public partial class Form1 : Form
    {
        // Connection String sudah disesuaikan dengan laptop kamu
        private readonly string connectionString =
            "Data Source=LAPTOP-MBD0B33T\\SHENDY;Initial Catalog=DBAkademikADO;User ID=sa;Password=Password123";
        DAL dbLogic = new DAL();
        private DataTable dtMahasiswa = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }

        private void SimpanLog(string pesan)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO LogError VALUES(GETDATE(), @pesan)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@pesan", pesan);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan log: " + ex.Message);
            }
        }

        public void simpanLog(string message)
        {
            SimpanLog(message);
        }

        private void ConnectDatabase()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    MessageBox.Show("Koneksi Berhasil");
                }
            }
            catch (SqlException ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtNIM.Enabled = true;
            txtNIM.Clear();
            txtNama.Clear();
            cmbJK.SelectedIndex = -1;
            txtAlamat.Clear();
            txtKodeProdi.Clear();
            dtpTanggalLahir.Value = DateTime.Now;
            pictureBox1.Image = null;
            txtNIM.Focus();
        }

        private void HitungTotal()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
                        outputParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParam);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        lblTotal.Text = "Total Mahasiswa: " + outputParam.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dtMahasiswa = new DataTable();
                            da.Fill(dtMahasiswa);

                            mahasiswaBindingSource.DataSource = dtMahasiswa;
                            dataGridView1.DataSource = mahasiswaBindingSource;
                            bindingNavigator1.BindingSource = mahasiswaBindingSource;

                            // Set foto column jika ada
                            if (dataGridView1.Columns.Contains("Foto"))
                            {
                                DataGridViewImageColumn fotoColumn = (DataGridViewImageColumn)dataGridView1.Columns["Foto"];
                                fotoColumn.ImageLayout = DataGridViewImageCellLayout.Stretch;
                            }

                            BindControls();
                        }
                    }
                }

                HitungTotal();

                // Log kolom untuk debugging
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    Console.WriteLine("Name: " + col.Name + " | DataPropertyName: " + col.DataPropertyName);
                }

                dataGridView1.Enabled = true;
                btnImportDatabase.Enabled = false;
                btnInsert.Enabled = true;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                btnCari.Enabled = true;
                btnLoad.Enabled = true;
                btnResetData.Enabled = true;
                btnTestInjection.Enabled = true;
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void BindControls()
        {
            txtNIM.DataBindings.Clear();
            txtNama.DataBindings.Clear();
            cmbJK.DataBindings.Clear();
            dtpTanggalLahir.DataBindings.Clear();
            txtAlamat.DataBindings.Clear();
            txtKodeProdi.DataBindings.Clear();

            txtNIM.DataBindings.Add("Text", mahasiswaBindingSource, "NIM");
            txtNama.DataBindings.Add("Text", mahasiswaBindingSource, "Nama");
            cmbJK.DataBindings.Add("Text", mahasiswaBindingSource, "JenisKelamin");
            dtpTanggalLahir.DataBindings.Add("Value", mahasiswaBindingSource, "TanggalLahir");
            txtAlamat.DataBindings.Add("Text", mahasiswaBindingSource, "Alamat");
            txtKodeProdi.DataBindings.Add("Text", mahasiswaBindingSource, "KodeProdi");
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            cmbJK.Items.Clear();
            cmbJK.Items.Add("L");
            cmbJK.Items.Add("P");

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            bindingNavigator1.BindingSource = mahasiswaBindingSource;

            LoadData();
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            try
            {
                // Implementasi pencarian sederhana
                string keyword = txtNIM.Text.Trim();
                if (string.IsNullOrEmpty(keyword))
                {
                    LoadData();
                    return;
                }

                DataView dv = dtMahasiswa.DefaultView;
                dv.RowFilter = $"NIM LIKE '%{keyword}%' OR Nama LIKE '%{keyword}%'";
                dataGridView1.DataSource = dv;
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Gagal mencari data: " + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] ConvertImageToBytes(PictureBox pb)
                {
                    if (pb.Image == null)
                        return null;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        pb.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return ms.ToArray();
                    }
                }

                byte[] imgBytes = ConvertImageToBytes(pictureBox1);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);
                        cmd.Parameters.AddWithValue("@Nama", txtNama.Text);
                        cmd.Parameters.AddWithValue("@Alamat", txtAlamat.Text);
                        cmd.Parameters.AddWithValue("@JenisKelamin", cmbJK.Text);
                        cmd.Parameters.AddWithValue("@TanggalLahir", dtpTanggalLahir.Value.Date);
                        cmd.Parameters.AddWithValue("@KodeProdi", txtKodeProdi.Text);

                        if (imgBytes != null)
                            cmd.Parameters.AddWithValue("@Foto", imgBytes);
                        else
                            cmd.Parameters.AddWithValue("@Foto", DBNull.Value);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data mahasiswa berhasil ditambahkan");
                ClearForm();
                LoadData();
            }
            catch (SqlException ex)
            {
                simpanLog("Rollback Insert : " + ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                simpanLog("General Error : " + ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] ConvertImageToBytes(PictureBox pb)
                {
                    if (pb.Image == null)
                        return null;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        pb.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return ms.ToArray();
                    }
                }

                byte[] imgBytes = ConvertImageToBytes(pictureBox1);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);
                        cmd.Parameters.AddWithValue("@Nama", txtNama.Text);
                        cmd.Parameters.AddWithValue("@Alamat", txtAlamat.Text);
                        cmd.Parameters.AddWithValue("@JenisKelamin", cmbJK.Text);
                        cmd.Parameters.AddWithValue("@TanggalLahir", dtpTanggalLahir.Value.Date);
                        cmd.Parameters.AddWithValue("@KodeProdi", txtKodeProdi.Text);

                        if (imgBytes != null)
                            cmd.Parameters.AddWithValue("@Foto", imgBytes);
                        else
                            cmd.Parameters.AddWithValue("@Foto", DBNull.Value);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data mahasiswa berhasil diubah");
                ClearForm();
                btnLoad.PerformClick();
            }
            catch (SqlException ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dg = MessageBox.Show(
                    "Yakin ingin menghapus data?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dg == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Data mahasiswa berhasil dihapus");
                    ClearForm();
                    btnLoad.PerformClick();
                }
            }
            catch (SqlException ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("SQL Error :" + ex.Message);
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("General Error :" + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataRow row = ((DataRowView)mahasiswaBindingSource[e.RowIndex]).Row;

                txtNIM.Text = row[0].ToString();
                txtNama.Text = row[1].ToString();
                cmbJK.Text = row[2].ToString();
                dtpTanggalLahir.Value = Convert.ToDateTime(row[3]);
                txtAlamat.Text = row[4].ToString();
                txtKodeProdi.Text = row[6].ToString();

                if (row[5] != DBNull.Value)
                {
                    byte[] imgBytes = (byte[])row[5];
                    using (MemoryStream ms = new MemoryStream(imgBytes))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
                else
                {
                    pictureBox1.Image = null;
                }

                txtNIM.Enabled = false;
            }
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult resultConfirm = MessageBox.Show(
                    "Yakin ingin reset data dari backup?",
                    "Konfirmasi Reset",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (resultConfirm == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"
                            IF OBJECT_ID('dbo.Mahasiswa_Backup') IS NOT NULL
                            BEGIN
                                DELETE FROM dbo.Mahasiswa;
                                INSERT INTO dbo.Mahasiswa
                                SELECT * FROM dbo.Mahasiswa_Backup;
                            END";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Data berhasil direset");
                    ClearForm();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Reset gagal: " + ex.Message);
            }
        }

        private void btnTestInjection_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "UPDATE Mahasiswa SET Nama='" + txtNama.Text + "' WHERE NIM='" + txtNIM.Text + "'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        int result = cmd.ExecuteNonQuery();
                        MessageBox.Show(result + " baris terupdate");
                    }
                }

                LoadData();
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("safe"))
                {
                    simpanLog(ex.Message);
                    MessageBox.Show("SQL Error : Unsafe UPDATE operation not allowed");
                }
                else
                {
                    simpanLog(ex.Message);
                    MessageBox.Show("SQL Error :" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("General Error :" + ex.Message);
            }
        }

        private void lbsTotal_Click(object sender, EventArgs e)
        {
            // Tetap dikosongkan jika tidak dipakai
        }

        private void btnRekapData_Click(object sender, EventArgs e)
        {
            // Memanggil Form 3 (Filter) milik kamu
            frmRekapMahasiswa_Load fm3 = new frmRekapMahasiswa_Load();
            fm3.Show();
            this.Hide();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(ofd.FileName);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Excel Workbook|*.xlsx" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });

                            DataTable dt = result.Tables[0];
                            dataGridView1.DataSource = dt;
                            dataGridView1.Enabled = false;
                            btnImportDatabase.Enabled = true;
                            btnInsert.Enabled = false;
                            btnUpdate.Enabled = false;
                            btnDelete.Enabled = false;
                            btnCari.Enabled = false;
                            btnLoad.Enabled = false;
                            btnResetData.Enabled = false;
                            btnTestInjection.Enabled = false;
                        }
                    }
                }
            }
        }

        private void btnImportDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = (DataTable)dataGridView1.DataSource;

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Tidak ada data untuk diimport.");
                    return;
                }

                int sukses = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string nim = row["NIM"].ToString().Trim();
                    string nama = row["Nama"].ToString().Trim();
                    string jk = row["JenisKelamin"].ToString().Trim();
                    string alamat = row["Alamat"].ToString().Trim();
                    string kodeProdi = row["NamaProdi"].ToString().Trim();
                    string fotoPath = row.Table.Columns.Contains("FotoPath")
                        ? row["FotoPath"].ToString().Trim()
                        : string.Empty;

                    if (string.IsNullOrEmpty(nim) || string.IsNullOrEmpty(nama))
                        continue;

                    DateTime tglLahir;
                    if (!DateTime.TryParse(row["TanggalLahir"].ToString(), out tglLahir))
                        continue;

                    byte[] ConvertImageFromPath(string path)
                    {
                        if (string.IsNullOrWhiteSpace(path))
                            return null;

                        if (!File.Exists(path))
                            return null;

                        return File.ReadAllBytes(path);
                    }

                    byte[] fotoBytes = ConvertImageFromPath(fotoPath);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@NIM", nim);
                            cmd.Parameters.AddWithValue("@Nama", nama);
                            cmd.Parameters.AddWithValue("@Alamat", alamat);
                            cmd.Parameters.AddWithValue("@JenisKelamin", jk);
                            cmd.Parameters.AddWithValue("@TanggalLahir", tglLahir);
                            cmd.Parameters.AddWithValue("@KodeProdi", kodeProdi);

                            if (fotoBytes != null)
                                cmd.Parameters.AddWithValue("@Foto", fotoBytes);
                            else
                                cmd.Parameters.AddWithValue("@Foto", DBNull.Value);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    sukses++;
                }

                MessageBox.Show(sukses + " data mahasiswa berhasil diimport");
                ClearForm();
                LoadData();
            }
            catch (SqlException ex)
            {
                simpanLog("Rollback Insert : " + ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                simpanLog("General Error : " + ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            btnUpload_Click(sender, e);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Kosong
        }
    }
}