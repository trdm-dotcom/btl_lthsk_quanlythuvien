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

namespace btl_lthsk_quanlythuvien.Forms
{
    public partial class FormChangePass : Form
    {
        private User user;
        public FormChangePass(User user)
        {
            InitializeComponent();
            this.user = user;
        }

        private void btnConf_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtNew.Text) || string.IsNullOrEmpty(txtConf.Text))
            {
                MessageBox.Show("Nhập đầy đủ các trường thông tin");
                return;
            }
            if(!string.Equals(txtNew.Text, txtConf.Text))
            {
                MessageBox.Show("Mật khẩu xác thực không khớp");
                txtConf.Clear();
                return;
            }
            string connectionString = ConfigurationManager.ConnectionStrings["db_btlthsk"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("doChangePass", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@matt", SqlDbType.NVarChar).Value = this.user.Iduser;
                    cmd.Parameters.Add("@pass", SqlDbType.VarChar).Value = cryptPassword(txtPassword.Text);
                    cmd.Parameters.Add("@new", SqlDbType.VarChar).Value = cryptPassword(txtNew.Text);
                    con.Open();
                    int rowCount = cmd.ExecuteNonQuery();
                    if (rowCount > 0)
                    {
                        MessageBox.Show("Cập nhật thành công");
                        txtPassword.Clear();
                        txtNew.Clear();
                        txtConf.Clear();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Mật khẩu hiên tại không đúng");
                        return;
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
