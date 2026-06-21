using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public class DAL
    {
        // ============================================
        // GET LOCAL IP ADDRESS - SESUAI MODUL
        // ============================================
        public static string GetLocalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting local IP address: " + ex.Message);
            }
            return localIP;
        }

        // ============================================
        // GET CONNECTION STRING - SESUAI MODUL
        // ============================================
        public static string GetConnectionString()
        {
            string connectionString = $"Data Source={GetLocalIPAddress()};Initial Catalog=DBAkademikADO;User ID=sa;Password=PasswordSA;";
            return connectionString;
        }

        // ============================================
        // CONNECTION STRING - HARDCODE (UNTUK DEMO)
        // ============================================
        // ✅ PAKAI INI UNTUK DEMO (PASTI BERHASIL)
        private readonly string connectionString = "Data Source=LAPTOP-MBD0B33T\\SHENDY;Initial Catalog=DBAkademikADO;User ID=sa;Password=Password123";

        // ❌ COMMENT INI DULU (PAKAI IP OTOMATIS)
        // private readonly string connectionString = GetConnectionString();

        private SqlConnection conn;
        private SqlDataAdapter da;
        private DataTable dtMahasiswa;

        public DAL()
        {
            conn = new SqlConnection(connectionString);
        }

        // ============================================
        // METHOD UNTUK DASHBOARD
        // ============================================
        public DataSet Dashboard()
        {
            DataSet ds = new DataSet();
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_DashBoard", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                da = new SqlDataAdapter(cmd);
                da.Fill(ds, "Dashboard");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return ds;
        }

        public DataSet DashboardByTahun(string tahun)
        {
            DataSet ds = new DataSet();
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_DashBoardByTahun", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@intgLmsuk", tahun);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds, "Dashboard");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return ds;
        }

        // ============================================
        // METHOD CRUD MAHASISWA
        // ============================================
        public DataTable GetMhsByNIM(string nim)
        {
            DataTable dt = new DataTable();
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_GetMahasiswaByNIM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pNIM", nim);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }

        public void DeleteMhs(string nim)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = "DELETE FROM Mahasiswa WHERE NIM = @NIM";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NIM", nim);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public void resetData()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = @"
                    IF OBJECT_ID('dbo.Mahasiswa_Backup') IS NOT NULL
                    BEGIN
                        DELETE FROM dbo.Mahasiswa;
                        INSERT INTO dbo.Mahasiswa
                        SELECT * FROM dbo.Mahasiswa_Backup;
                    END";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public void InsertLog(string message)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = "INSERT INTO LogError (waktu, pesan_error) VALUES (GETDATE(), @pesan)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@pesan", message);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public void simpanLog(string message)
        {
            InsertLog(message);
        }

        public void testInject(string nim)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = "UPDATE Mahasiswa SET Nama = 'HACKED' WHERE NIM = '" + nim + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public int CountMahasiswa()
        {
            int total = 0;
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = "SELECT COUNT(*) FROM Mahasiswa";
                SqlCommand cmd = new SqlCommand(query, conn);
                total = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return total;
        }
    }
}