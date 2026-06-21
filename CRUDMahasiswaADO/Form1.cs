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
        private readonly string connectionString =
            "Data Source=LAPTOP-MBD0B33T\\SHENDY;Initial Catalog=DBAkademikADO;User ID=sa;Password=Password123";
        private DAL dbLogic = new DAL();
        private DataTable dtMahasiswa = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }

        private void SimpanLog(string pesan)
        {
            try
            {
                dbLogic.simpanLog(pesan);
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
                int total = dbLogic.CountMahasiswa();
                lblTotal.Text = "Total Mahasiswa: " + total.ToString();
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                lblTotal.Text = "Total Mahasiswa: 0";
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

                dataGridView1.Enabled = true;
                btnImportDatabase.Enabled = false;
                btnInsert.Enabled = true;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                btnCari.Enabled = true;
                btnLoad.Enabled = true;
                btnResetData.Enabled = true;
                btnTestInjection.Enabled = true;
                btnImportExcel.Enabled = true;
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
            btnImportDatabase.Enabled = false;

            LoadData();
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            try
            {
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
            if (string.IsNullOrWhiteSpace(txtNIM.Text))
            {
                MessageBox.Show("NIM harus diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNIM.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNama.Text))
            {
                MessageBox.Show("Nama harus diisi!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNama.Focus();
                return;
            }

            try
            {
                byte[] imgBytes = null;
                if (pictureBox1.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        imgBytes = ms.ToArray();
                    }
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@pNIM", txtNIM.Text);
                        cmd.Parameters.AddWithValue("@pNama", txtNama.Text);
                        cmd.Parameters.AddWithValue("@pAlamat", txtAlamat.Text);
                        cmd.Parameters.AddWithValue("@pJenisKelamin", cmbJK.Text);
                        cmd.Parameters.AddWithValue("@pTanggalLahir", dtpTanggalLahir.Value.Date);
                        cmd.Parameters.AddWithValue("@pKodeProdi", txtKodeProdi.Text);
                        cmd.Parameters.AddWithValue("@pFoto", imgBytes ?? (object)DBNull.Value);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data mahasiswa berhasil ditambahkan!", "Sukses",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadData();
            }
            catch (SqlException ex)
            {
                simpanLog("Insert Error: " + ex.Message);
                if (ex.Number == 2627)
                    MessageBox.Show("NIM sudah terdaftar! Gunakan NIM yang berbeda.", "Duplicate Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("SQL Error: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                simpanLog("General Error: " + ex.Message);
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNIM.Text))
            {
                MessageBox.Show("Pilih data yang akan diupdate!", "Validasi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                byte[] imgBytes = null;
                if (pictureBox1.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        imgBytes = ms.ToArray();
                    }
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@pNIM", txtNIM.Text);
                        cmd.Parameters.AddWithValue("@pNama", txtNama.Text);
                        cmd.Parameters.AddWithValue("@pAlamat", txtAlamat.Text);
                        cmd.Parameters.AddWithValue("@pJenisKelamin", cmbJK.Text);
                        cmd.Parameters.AddWithValue("@pTanggalLahir", dtpTanggalLahir.Value.Date);
                        cmd.Parameters.AddWithValue("@pKodeProdi", txtKodeProdi.Text);
                        cmd.Parameters.AddWithValue("@pFoto", imgBytes ?? (object)DBNull.Value);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data mahasiswa berhasil diubah!", "Sukses",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                btnLoad.PerformClick();
            }
            catch (SqlException ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("SQL Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNIM.Text))
            {
                MessageBox.Show("Pilih data yang akan dihapus!", "Validasi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DialogResult dg = MessageBox.Show("Yakin ingin menghapus data?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dg == DialogResult.Yes)
                {
                    dbLogic.DeleteMhs(txtNIM.Text);
                    MessageBox.Show("Data mahasiswa berhasil dihapus!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    btnLoad.PerformClick();
                }
            }
            catch (SqlException ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("SQL Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataRow row = ((DataRowView)bindingNavigator1.BindingSource[e.RowIndex]).Row;

                    txtNIM.Text = row["NIM"].ToString();
                    txtNama.Text = row["Nama"].ToString();
                    cmbJK.Text = row["JenisKelamin"].ToString();
                    dtpTanggalLahir.Value = Convert.ToDateTime(row["TanggalLahir"]);
                    txtAlamat.Text = row["Alamat"].ToString();
                    txtKodeProdi.Text = row["KodeProdi"].ToString();

                    if (row["Foto"] != DBNull.Value && row["Foto"] != null)
                    {
                        try
                        {
                            byte[] imgBytes = (byte[])row["Foto"];
                            using (MemoryStream ms = new MemoryStream(imgBytes))
                            {
                                pictureBox1.Image = Image.FromStream(ms);
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                        }
                        catch
                        {
                            pictureBox1.Image = null;
                        }
                    }
                    else
                    {
                        pictureBox1.Image = null;
                    }

                    txtNIM.Enabled = false;
                }
                catch (Exception ex)
                {
                    simpanLog("CellClick Error: " + ex.Message);
                }
            }
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult resultConfirm = MessageBox.Show("Yakin ingin reset data dari backup?",
                    "Konfirmasi Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultConfirm == DialogResult.Yes)
                {
                    dbLogic.resetData();
                    MessageBox.Show("Data berhasil direset!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Reset gagal: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTestInjection_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNIM.Text))
            {
                MessageBox.Show("Masukkan NIM untuk test injection!", "Validasi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dbLogic.testInject(txtNIM.Text);
                LoadData();
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("safe"))
                {
                    simpanLog(ex.Message);
                    MessageBox.Show("SQL Error: Unsafe UPDATE operation not allowed", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    simpanLog(ex.Message);
                    MessageBox.Show("SQL Error: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRekapData_Click(object sender, EventArgs e)
        {
            frmRekapMahasiswa_Load fm3 = new frmRekapMahasiswa_Load();
            fm3.Show();
            this.Hide();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            ofd.Title = "Pilih Gambar";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = Image.FromFile(ofd.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat gambar: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            btnUpload_Click(sender, e);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadData();
        }

        // ============================================
        // IMPORT EXCEL
        // ============================================
        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Pilih File Excel"
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = openFileDialog.FileName;
                        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

                                if (dt == null || dt.Rows.Count == 0)
                                {
                                    MessageBox.Show("File Excel kosong! Tidak ada data.",
                                        "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                // KONVERSI TEMPLATE DOSEN
                                if (dt.Columns.Contains("NamaProdi"))
                                {
                                    dt = ConvertTemplateToStandard(dt);
                                }

                                if (!dt.Columns.Contains("KodeProdi"))
                                {
                                    MessageBox.Show("Format Excel salah!\n\n" +
                                        "Format yang benar:\n" +
                                        "NIM | Nama | JenisKelamin | TanggalLahir | Alamat | KodeProdi",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                dataGridView1.DataSource = dt;

                                btnImportDatabase.Enabled = true;

                                dataGridView1.Enabled = false;
                                btnInsert.Enabled = false;
                                btnUpdate.Enabled = false;
                                btnDelete.Enabled = false;
                                btnCari.Enabled = false;
                                btnLoad.Enabled = false;
                                btnResetData.Enabled = false;
                                btnTestInjection.Enabled = false;

                                MessageBox.Show($"Data berhasil di-load dari Excel!\nTotal: {dt.Rows.Count} data\n\n" +
                                    "Klik 'Import to Database' untuk menyimpan ke database.",
                                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("File sedang digunakan! Tutup file Excel terlebih dahulu.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ============================================
        // KONVERSI TEMPLATE DOSEN
        // ============================================
        private DataTable ConvertTemplateToStandard(DataTable sourceDt)
        {
            DataTable newDt = new DataTable();
            newDt.Columns.Add("NIM", typeof(string));
            newDt.Columns.Add("Nama", typeof(string));
            newDt.Columns.Add("JenisKelamin", typeof(string));
            newDt.Columns.Add("TanggalLahir", typeof(DateTime));
            newDt.Columns.Add("Alamat", typeof(string));
            newDt.Columns.Add("KodeProdi", typeof(string));

            foreach (DataRow row in sourceDt.Rows)
            {
                bool isEmpty = true;
                foreach (var item in row.ItemArray)
                {
                    if (item != null && !string.IsNullOrWhiteSpace(item.ToString()))
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (isEmpty) continue;

                try
                {
                    DataRow newRow = newDt.NewRow();
                    newRow["NIM"] = row["NIM"].ToString();
                    newRow["Nama"] = row["Nama"].ToString();

                    if (sourceDt.Columns.Contains("JenisKelamin"))
                        newRow["JenisKelamin"] = row["JenisKelamin"].ToString();
                    else
                        newRow["JenisKelamin"] = "L";

                    if (sourceDt.Columns.Contains("TanggalLahir"))
                    {
                        DateTime tgl;
                        if (DateTime.TryParse(row["TanggalLahir"].ToString(), out tgl))
                            newRow["TanggalLahir"] = tgl;
                        else
                            newRow["TanggalLahir"] = DateTime.Now;
                    }
                    else
                    {
                        newRow["TanggalLahir"] = DateTime.Now;
                    }

                    if (sourceDt.Columns.Contains("Alamat"))
                        newRow["Alamat"] = row["Alamat"].ToString();
                    else
                        newRow["Alamat"] = "";

                    string namaProdi = row["NamaProdi"].ToString().ToLower();
                    if (namaProdi.Contains("informatika"))
                        newRow["KodeProdi"] = "TI01";
                    else if (namaProdi.Contains("sistem informasi") || namaProdi.Contains("sistem"))
                        newRow["KodeProdi"] = "SI01";
                    else
                        newRow["KodeProdi"] = "TI01";

                    newDt.Rows.Add(newRow);
                }
                catch (Exception ex)
                {
                    simpanLog("Convert Error: " + ex.Message);
                }
            }

            return newDt;
        }

        // ============================================
        // IMPORT TO DATABASE - PAKAI SP (SESUAI DATABASE)
        // ============================================
        private void btnImportDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = (DataTable)dataGridView1.DataSource;

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Tidak ada data untuk diimport. Silakan import Excel terlebih dahulu.",
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogResult confirm = MessageBox.Show($"Yakin ingin mengimport {dt.Rows.Count} data ke database?",
                    "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                int sukses = 0;
                int gagal = 0;
                string errorMsg = "";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    foreach (DataRow row in dt.Rows)
                    {
                        try
                        {
                            string nim = row["NIM"].ToString().Trim();
                            string nama = row["Nama"].ToString().Trim();
                            string jk = row["JenisKelamin"].ToString().Trim();
                            string alamat = row["Alamat"].ToString().Trim();
                            string kodeProdi = row["KodeProdi"].ToString().Trim();

                            if (string.IsNullOrEmpty(nim) || string.IsNullOrEmpty(nama))
                            {
                                gagal++;
                                errorMsg += $"NIM/Nama kosong: {nim}\n";
                                continue;
                            }

                            if (string.IsNullOrEmpty(jk) || (jk != "L" && jk != "P"))
                                jk = "L";

                            DateTime tglLahir;
                            if (!DateTime.TryParse(row["TanggalLahir"].ToString(), out tglLahir))
                            {
                                tglLahir = DateTime.Now;
                            }

                            // CEK NIM SUDAH ADA
                            string cekQuery = "SELECT COUNT(*) FROM Mahasiswa WHERE NIM = @NIM";
                            using (SqlCommand cekCmd = new SqlCommand(cekQuery, conn))
                            {
                                cekCmd.Parameters.AddWithValue("@NIM", nim);
                                int exists = Convert.ToInt32(cekCmd.ExecuteScalar());
                                if (exists > 0)
                                {
                                    gagal++;
                                    errorMsg += $"NIM {nim} sudah ada!\n";
                                    continue;
                                }
                            }

                            // ============================================
                            // PAKAI SP - PERHATIKAN NAMA PARAMETER!
                            // @pTanggalLahir (bukan @pTanggallahir)
                            // ============================================
                            using (SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@pNIM", nim);
                                cmd.Parameters.AddWithValue("@pNama", nama);
                                cmd.Parameters.AddWithValue("@pAlamat", alamat);
                                cmd.Parameters.AddWithValue("@pJenisKelamin", jk);
                                cmd.Parameters.AddWithValue("@pTanggalLahir", tglLahir);
                                cmd.Parameters.AddWithValue("@pKodeProdi", kodeProdi);

                                // ✅ PERBAIKAN: Pakai SqlDbType.VarBinary untuk @pFoto
                                cmd.Parameters.Add("@pFoto", SqlDbType.VarBinary).Value = DBNull.Value;

                                cmd.ExecuteNonQuery();
                                sukses++;
                            }
                        }
                        catch (SqlException ex)
                        {
                            gagal++;
                            errorMsg += $"SQL Error: {ex.Message}\n";
                            simpanLog("Import Error: " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            gagal++;
                            errorMsg += $"Error: {ex.Message}\n";
                            simpanLog("Import Error: " + ex.Message);
                        }
                    }
                }

                MessageBox.Show($"Import selesai!\n\n✅ Sukses: {sukses} data\n❌ Gagal: {gagal} data" +
                    (string.IsNullOrEmpty(errorMsg) ? "" : $"\n\nDetail Error:\n{errorMsg}"),
                    "Hasil Import", MessageBoxButtons.OK,
                    gagal > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                btnImportDatabase.Enabled = false;
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                simpanLog("Import Error: " + ex.Message);
                MessageBox.Show("ERROR: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lbsTotal_Click(object sender, EventArgs e) { }

        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}