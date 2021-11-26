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
    public partial class FormLibrary : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["db_btlthsk"].ConnectionString;
        DataTable dtBook;
        DataTable dtType;
        SqlCommand cmd;
        String select = "";
        public FormLibrary()
        {
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int index = dgvBook.CurrentRow.Index;
            DataGridViewRow dataRow = dgvBook.Rows[index];
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void btnAddType_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNameType.Text))
            {
                MessageBox.Show("Nhập tên thể loại");
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    var count = dtType.Rows.Count + 1;
                    string id = String.Format("TL{0}", createId(count));
                    cmd = new SqlCommand("insertTheloai", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@matl", SqlDbType.NVarChar).Value = id;
                    cmd.Parameters.Add("@tentl", SqlDbType.NVarChar).Value = txtNameType.Text;
                    con.Open();
                    int rowCount = cmd.ExecuteNonQuery();
                    if (rowCount > 0)
                    {
                        dtType.Rows.Add(new Object[] { id, txtNameType.Text });
                        dtType.AcceptChanges();
                        cbTypeBook.DataSource = dtType;
                        cbTypeBook.DisplayMember = "sTenL";
                        cbTypeBook.ValueMember = "sMaL";
                        cbTypeSearch.DataSource = dtType;
                        cbTypeSearch.DisplayMember = "sTenL";
                        cbTypeSearch.ValueMember = "sMaL";
                        txtNameType.Clear();
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

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNameBook.Text) || nmBook.Value < 1 || cbTypeBook.SelectedValue == null)
            {
                MessageBox.Show("Nhập đầy đủ thông tin sách");
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    var count = dtBook.Rows.Count + 1;
                    string id = String.Format("S{0}", createId(count));
                    cmd = new SqlCommand("insertSach", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@mas", SqlDbType.NVarChar).Value = id;
                    cmd.Parameters.Add("@tens", SqlDbType.NVarChar).Value = txtNameBook.Text;
                    cmd.Parameters.Add("@mal", SqlDbType.NVarChar).Value = cbTypeBook.SelectedValue.ToString();
                    cmd.Parameters.Add("@sl", SqlDbType.Int).Value = nmBook.Value;
                    con.Open();
                    int rowCount = cmd.ExecuteNonQuery();
                    if (rowCount > 0)
                    {
                        dtBook.Rows.Add(new Object[] { id, txtNameBook.Text, int.Parse(nmBook.Value.ToString()), cbTypeBook.SelectedValue.ToString(), cbTypeBook.GetItemText(cbTypeBook.SelectedItem),0});
                        dtBook.AcceptChanges();
                        dgvBook.DataSource = dtBook;
                        txtNameBook.Clear();
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void cbTypeSearch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FormLibrary_Load(object sender, EventArgs e)
        {
            if(dtType == null && dtBook == null)
            {
                loadData();
            }
            cbTypeBook.DataSource = dtType;
            cbTypeBook.DisplayMember = "sTenL";
            cbTypeBook.ValueMember = "sMaL";
            cbTypeSearch.DataSource = dtType;
            cbTypeSearch.DisplayMember = "sTenL";
            cbTypeSearch.ValueMember = "sMaL";
            dgvBook.DataSource = dtBook;
        }

        private void loadData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("getTheloai", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader data = cmd.ExecuteReader();
                    dtType = new DataTable();
                    dtType.Load(data);

                    cmd = new SqlCommand("getSach", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    data = cmd.ExecuteReader();
                    dtBook = new DataTable();
                    dtBook.Load(data);
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

        private string createId(int count)
        {
            string id = "";
            for (int i = 0; i < 8 - count.ToString().Length; i++)
            {
                id += String.Format("{0}", 0);
            }
            id += String.Format("{0}", count);
            return id;
        }
    }
}
