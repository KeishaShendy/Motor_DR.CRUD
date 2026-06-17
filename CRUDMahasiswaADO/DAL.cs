using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    class DAL
    {
        private SqlConnection conn;
        private SqlCommand cmd;
        private SqlDataAdapter da;
        private DataSet ds;

        // Constructor: Tempat menaruh Connection String Anda
        public DAL()
        {
            conn = new SqlConnection("Data Source=LAPTOP-MBD0B33T\\SHENDY;Initial Catalog=DBAkademikADO;User ID=sa;Password=Password123");
        }

        // Fungsi Read (Gambar 1)
        public DataSet GetMhs()
        {
            ds = new DataSet();
            try
            {
                conn.Open();
                cmd = new SqlCommand("sp_GetMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                da = new SqlDataAdapter(cmd);
                da.Fill(ds, "Mahasiswa");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        // Fungsi Insert (Gambar 2 & 3)
        public void InsertMhs(string NIM, string Nama, string Alamat, string JenisKelamin, DateTime TanggalLahir, string KodeProdi, byte[] Foto)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("sp_InsertMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pNIM", NIM);
                cmd.Parameters.AddWithValue("@pNama", Nama);
                cmd.Parameters.AddWithValue("@pAlamat", Alamat);
                cmd.Parameters.AddWithValue("@pJenisKelamin", JenisKelamin);
                cmd.Parameters.AddWithValue("@pTanggalLahir", TanggalLahir);
                cmd.Parameters.AddWithValue("@pKodeProdi", KodeProdi);

                if (Foto != null)
                    cmd.Parameters.AddWithValue("@pFoto", Foto);
                else
                    cmd.Parameters.AddWithValue("@pFoto", DBNull.Value);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        // Fungsi Update (Gambar 4)
        public void UpdateMhs(string NIM, string Nama, string Alamat, string JenisKelamin, DateTime TanggalLahir, string KodeProdi, byte[] Foto)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("sp_UpdateMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pNIM", NIM);
                cmd.Parameters.AddWithValue("@pNama", Nama);
                cmd.Parameters.AddWithValue("@pAlamat", Alamat);
                cmd.Parameters.AddWithValue("@pJenisKelamin", JenisKelamin);
                cmd.Parameters.AddWithValue("@pTanggalLahir", TanggalLahir);
                cmd.Parameters.AddWithValue("@pKodeProdi", KodeProdi);

                if (Foto != null)
                    cmd.Parameters.AddWithValue("@pFoto", Foto);
                else
                    cmd.Parameters.AddWithValue("@pFoto", DBNull.Value);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        // Fungsi Delete (Sesuai alur, Anda akan butuh ini)
        public void DeleteMhs(string NIM)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("sp_DeleteMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pNIM", NIM);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        // Fungsi Dashboard (Gambar 5)
        public DataSet Dashboard()
        {
            ds = new DataSet();
            try
            {
                conn.Open();
                cmd = new SqlCommand("sp_Dashboard", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                da = new SqlDataAdapter(cmd);
                da.Fill(ds, "Dashboard");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        // Fungsi DashboardByTahun (Gambar 6)
        public DataSet DashboardByTahun(string Tahun)
        {
            ds = new DataSet();
            try
            {
                conn.Open();
                cmd = new SqlCommand("sp_DashboardByTahun", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inTglMsuk", Tahun);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds, "Dashboard");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }
    }
}