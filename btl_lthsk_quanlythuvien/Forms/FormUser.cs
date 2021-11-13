using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Data;

namespace btl_lthsk_quanlythuvien.Forms
{
    public partial class FormUser : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["db_btlthsk"].ConnectionString;
        SqlCommand cmd;
        DataTable dt;

        public FormUser()
        {
            InitializeComponent();
        }

        private void FormUser_Load(object sender, EventArgs e)
        {
            if (dt == null)
            {
                loadUser();
            }
            dgvUser.DataSource = dt;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                try
                {
                    
                    if (btnAdd.Tag==null)
                    {
                        insert(con);
                    }
                    else
                    {
                        update(con, btnAdd.Tag.ToString());
                    }
                    
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error in sql" + ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error" + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int index               = dgvUser.CurrentRow.Index;
            DataGridViewRow dataRow = dgvUser.Rows[index];
            txtName.Text            = dataRow.Cells[1].Value.ToString();
            btnAdd.Text             = "Cập nhật";
            btnAdd.Tag              = dataRow.Cells[0].Value.ToString();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int index = dgvUser.CurrentRow.Index;
            DataGridViewRow dataRow = dgvUser.Rows[index];
            string name = dataRow.Cells[1].Value.ToString();
            string id = dataRow.Cells[0].Value.ToString();
            DialogResult res = MessageBox.Show(String.Format("Thực hiện xóa {0} khỏi hệ thống", name), "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if(res == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    try
                    {
                        cmd = new SqlCommand("DELETE tblThuthu WHERE sMaTT = @matt", con);
                        cmd.CommandType = CommandType.Text;
                        SqlParameter idParam = new SqlParameter("@matt", SqlDbType.NVarChar);
                        idParam.Value = id;
                        cmd.Parameters.Add(idParam);
                        int rowCount = cmd.ExecuteNonQuery();
                        if (rowCount > 0)
                        {
                            DataRow[] rows = dt.Select(String.Format("sMaTT = '{0}'", id));
                            foreach (DataRow row in rows)
                            {
                                row.Delete();
                            }
                            dt.AcceptChanges();
                            dgvUser.DataSource = dt;
                        }
                        clear();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Error in sql" + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error" + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            if(res == DialogResult.Cancel)
            {
                return;
            }
        }

        private void insert(SqlConnection con)
        {
            if (!hastext(txtName.Text) || !hastext(txtPassword.Text))
            {
                MessageBox.Show("Nhập lại thông tin");
                return;
            }
            string name = txtName.Text.Trim();
            string password = cryptPassword(txtPassword.Text.Trim(), GenerateSalt());
            cmd = new SqlCommand("SELECT COUNT(*) FROM tblThuthu", con);
            cmd.CommandType = CommandType.Text;
            var count = int.Parse(cmd.ExecuteScalar().ToString()) + 1;
            string id = createId(count);
            cmd = new SqlCommand("insertTT", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter idParam = new SqlParameter("@matt", SqlDbType.NVarChar);
            idParam.Value = id;
            cmd.Parameters.Add(idParam);
            SqlParameter nameParam = new SqlParameter("@hoten", SqlDbType.NVarChar);
            nameParam.Value = name;
            cmd.Parameters.Add(nameParam);
            SqlParameter passwordParam = new SqlParameter("@matkhau", SqlDbType.VarChar);
            passwordParam.Value = password;
            cmd.Parameters.Add(passwordParam);
            SqlParameter permissionParam = new SqlParameter("@maquyen", SqlDbType.Int);
            permissionParam.Value = 2;
            cmd.Parameters.Add(permissionParam);
            int rowCount = cmd.ExecuteNonQuery();
            if (rowCount > 0)
            {
                dt.Rows.Add(new Object[] { id, name });
                dt.AcceptChanges();
                dgvUser.DataSource = dt;
                clear();
            }
        }

        private void update(SqlConnection con, string id)
        {
            if (!hastext(txtName.Text)){
                MessageBox.Show("Nhập lại thông tin");
                return;
            }
            string name = txtName.Text.Trim();
            string password = "";
            if (hastext(txtPassword.Text))
            {
                password = cryptPassword(txtPassword.Text.Trim(), GenerateSalt());
            }
            cmd = new SqlCommand("updateTT", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter idParam = new SqlParameter("@matt", SqlDbType.NVarChar);
            idParam.Value = id;
            cmd.Parameters.Add(idParam);
            SqlParameter nameParam = new SqlParameter("@hoten", SqlDbType.NVarChar);
            nameParam.Value = name;
            cmd.Parameters.Add(nameParam);
            SqlParameter passwordParam = new SqlParameter("@matkhau", SqlDbType.VarChar);
            passwordParam.Value = password;
            cmd.Parameters.Add(passwordParam);
            int rowCount = cmd.ExecuteNonQuery();
            if (rowCount > 0)
            {
                DataRow[] rows = dt.Select(String.Format("sMaTT = '{0}'", id));
                foreach (DataRow row in rows)
                {
                    row["sTenTT"] = name;
                }
                dt.AcceptChanges();
                dgvUser.DataSource = dt;
                clear();
            }
        }

        private void clear()
        {
            txtName.Clear();
            txtPassword.Clear(); 
        }

        private bool hastext(string text)
        {
            return !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
        }

        private string createId(int count)
        {
            string id = "TT";
            for(int i = 0; i < 8 - count.ToString().Length; i++)
            {
                id += String.Format("{0}", 0);
            }
            id += String.Format("{0}", count);
            return id;
        }

        private string cryptPassword(string password,string salt)
        {
            string cryptPassword    = null;
            password                = EncodeToBase64(password);
            using (SHA256 mySHA256 = SHA256.Create())
            {
                try
                {
                    byte[] hashByte = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(salt, password)));
                    cryptPassword = String.Format("{0}${1}${2}"
                        ,salt.Length,salt,Convert.ToBase64String(hashByte));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in cryptPassword" + ex.Message);
                }
            }
            return cryptPassword;
        }

        public string GenerateSalt()
        {
            var bytes   = new byte[16];
            var rng     = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private string EncodeToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte        = Encoding.UTF8.GetBytes(password);
                return Convert.ToBase64String(encData_byte);
            }
            catch(Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        private void loadUser()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                try
                {
                    cmd = new SqlCommand("SELECT sMaTT,sTenTT FROM tblThuthu ORDER BY sTenTT ASC", con);
                    cmd.CommandType = System.Data.CommandType.Text;
                    SqlDataReader data = cmd.ExecuteReader();
                    dt = new DataTable();
                    dt.Load(data);
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error in sql loadUser" + ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in loadUser" + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
