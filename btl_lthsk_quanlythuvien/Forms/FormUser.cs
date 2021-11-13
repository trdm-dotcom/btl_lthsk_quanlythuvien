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
                try
                {
                    if (btnAdd.Tag==null)
                    {
                        if (!hastext(txtName.Text) || !hastext(txtPassword.Text))
                        {
                            MessageBox.Show("Nhập lại thông tin");
                            return;
                        }
                        string name = txtName.Text.Trim();
                        string password = cryptPassword(txtPassword.Text.Trim(), GenerateSalt());
                        string id = createId();
                        SqlCommand cmd = new SqlCommand("insertTT", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@matt", SqlDbType.NVarChar).Value = id;
                        cmd.Parameters.Add("@hoten", SqlDbType.NVarChar).Value = name;
                        cmd.Parameters.Add("@matkhau", SqlDbType.VarChar).Value = password;
                        cmd.Parameters.Add("@maquyen", SqlDbType.Int).Value = 2;
                        con.Open();
                        int rowCount = cmd.ExecuteNonQuery();
                        if (rowCount > 0)
                        {
                            dt.Rows.Add(new Object[] { id, name });
                            dt.AcceptChanges();
                            dgvUser.DataSource = dt;
                            clear();
                        }
                    }
                    else
                    {
                        if (!hastext(txtName.Text))
                        {
                            MessageBox.Show("Nhập lại thông tin");
                            return;
                        }
                        string id = btnAdd.Tag.ToString();
                        string name = txtName.Text.Trim();
                        string password = "";
                        if (hastext(txtPassword.Text))
                        {
                            password = cryptPassword(txtPassword.Text.Trim(), GenerateSalt());
                        }
                        SqlCommand cmd = new SqlCommand("updateTT", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@matt", SqlDbType.NVarChar).Value = id;
                        cmd.Parameters.Add("@hoten", SqlDbType.NVarChar).Value = name;
                        cmd.Parameters.Add("@matkhau", SqlDbType.VarChar).Value = password;
                        con.Open();
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
                    try
                    {
                        SqlCommand cmd = new SqlCommand("DELETE tblThuthu WHERE sMaTT = @matt", con);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@matt", SqlDbType.NVarChar).Value = id;
                        con.Open();
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

        private void clear()
        {
            txtName.Clear();
            txtPassword.Clear(); 
        }

        private bool hastext(string text)
        {
            return !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
        }

        private string createId()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM tblThuthu", con);
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    var count = int.Parse(cmd.ExecuteScalar().ToString()) + 1;
                    string id = "TT";
                    for (int i = 0; i < 8 - count.ToString().Length; i++)
                    {
                        id += String.Format("{0}", 0);
                    }
                    id += String.Format("{0}", count);
                    return id;
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
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT sMaTT,sTenTT FROM tblThuthu ORDER BY sTenTT ASC", con);
                    cmd.CommandType = System.Data.CommandType.Text;
                    con.Open();
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
