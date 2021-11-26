using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace btl_lthsk_quanlythuvien
{
    public partial class FormLogin : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["db_btlthsk"].ConnectionString;
        DataTable dt;
        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtPassword.Text) || cbUser.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập mật khẩu và chọn thủ thư");
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("doLogin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@matt", SqlDbType.NVarChar).Value = cbUser.SelectedValue.ToString();
                    cmd.Parameters.Add("@pass", SqlDbType.VarChar).Value = cryptPassword(txtPassword.Text);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if(ds.Tables[0].Rows.Count != 0)
                    {
                        User user = new User(); 
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            user = new User(row["sTenTT"].ToString(), row["sMaTT"].ToString(), int.Parse(row["iQuyen"].ToString()));
                        }
                        this.Hide();
                        MainForm fm = new MainForm(user);
                        fm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Mật khẩu không đúng");
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

        private void FormLogin_Load(object sender, EventArgs e)
        {
            if (dt == null)
            {
                loadUser();
            }
            cbUser.DataSource = dt;
            cbUser.DisplayMember = "sTenTT";
            cbUser.ValueMember = "sMaTT";
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
        private string cryptPassword(string password)
        {
            string cryptPassword = null;
            password = EncodeToBase64(password);
            using (SHA256 mySHA256 = SHA256.Create())
            {
                try
                {
                    byte[] hashByte = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    cryptPassword = Convert.ToBase64String(hashByte);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in cryptPassword" + ex.Message);
                }
            }
            return cryptPassword;
        }
        private string EncodeToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = Encoding.UTF8.GetBytes(password);
                return Convert.ToBase64String(encData_byte);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
    }
}
