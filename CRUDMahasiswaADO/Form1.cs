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
