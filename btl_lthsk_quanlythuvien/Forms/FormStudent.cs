using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace btl_lthsk_quanlythuvien.Forms
{
    public partial class FormStudent : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["db_btlthsk"].ConnectionString;
        DataTable dt;
        public FormStudent()
        {
            InitializeComponent();
        }

        private void FormStudent_Load(object sender, EventArgs e)
        {
            if(dt == null)
            {
                loadStudents();
            }
            DataView dv = dt.DefaultView;
            dgvStudent.DataSource = dv;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    if (!hastext(txtName.Text) || !hastext(txtClass.Text) || !hastext(txtId.Text))
                    {
                        MessageBox.Show("Nhập lại thông tin");
                        return;
                    }
                    if (btnAdd.Tag == null)
                    {
                        SqlCommand cmd = new SqlCommand("insertSV", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@masv", SqlDbType.NVarChar).Value = txtId.Text;
                        cmd.Parameters.Add("@tensv", SqlDbType.NVarChar).Value = txtName.Text;
                        cmd.Parameters.Add("@lop", SqlDbType.VarChar).Value = txtClass.Text;
                        cmd.Parameters.Add("@trangthai", SqlDbType.Int).Value = 1;
                        con.Open();
                        int rowCount = cmd.ExecuteNonQuery();
                        if (rowCount > 0)
                        {
                            dt.Rows.Add(new Object[] { txtId.Text, txtName.Text, txtClass.Text, "" });
                            dt.AcceptChanges();
                            DataView dv = dt.DefaultView;
                            dgvStudent.DataSource = dv;
                            clear();
                        }
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("updateSV", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@masv", SqlDbType.NVarChar).Value = txtId.Text;
                        cmd.Parameters.Add("@tensv", SqlDbType.NVarChar).Value = txtName.Text;
                        cmd.Parameters.Add("@lop", SqlDbType.VarChar).Value = txtClass.Text;
                        con.Open();
                        int rowCount = cmd.ExecuteNonQuery();
                        if (rowCount > 0)
                        {
                            DataRow row = dt.Select(string.Format("sMaSV = '{0}'", txtId.Text))[0];
                            row["sMaSV"] = txtId.Text;
                            row["sTenSV"] = txtName.Text;
                            row["sLop"] = txtClass.Text;
                            dt.AcceptChanges();
                            DataView dv = dt.DefaultView;
                            dgvStudent.DataSource = dv;
                            btnAdd.Text = "Thêm";
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
            int index = dgvStudent.CurrentRow.Index;
            DataGridViewRow dataRow = dgvStudent.Rows[index];
            txtId.Text = dataRow.Cells[0].Value.ToString();
            txtName.Text = dataRow.Cells[1].Value.ToString();
            txtClass.Text = dataRow.Cells[2].Value.ToString();
            btnAdd.Text = "Cập nhật";
            btnAdd.Tag = dataRow.Cells[0].Value.ToString();
        }

        private void btnBand_Click(object sender, EventArgs e)
        {
            int index = dgvStudent.CurrentRow.Index;
            DataGridViewRow dataRow = dgvStudent.Rows[index];
            string id = dataRow.Cells[0].Value.ToString();
            string name = dataRow.Cells[1].Value.ToString();
            DialogResult res = MessageBox.Show(string.Format("Thực hiện khóa sinh viên {0} mượn sách", name), "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if(res == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("bandStudent", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@masv", SqlDbType.NVarChar).Value = id;
                        con.Open();
                        int rowCount = cmd.ExecuteNonQuery();
                        if (rowCount > 0)
                        {
                            DataRow row = dt.Select(string.Format("sMaSV = '{0}'", txtId.Text))[0];
                            row["trangthai"] = "Khóa";
                            dt.AcceptChanges();
                            DataView dv = dt.DefaultView;
                            dgvStudent.DataSource = dv;
                            clear();
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
            if (res == DialogResult.Cancel)
            {
                return;
            }
        }

        private bool hastext(string text)
        {
            return !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
        }

        private void clear()
        {
            txtId.Clear();
            txtClass.Clear();
            txtName.Clear();
            btnAdd.Tag = null;
        }

        private void loadStudents()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("getStudent", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader data = cmd.ExecuteReader();
                    dt = new DataTable();
                    dt.Load(data);
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
    }
}
