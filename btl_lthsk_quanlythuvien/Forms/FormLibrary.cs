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
        DataTable dtTypeSearch;
        public FormLibrary()
        {
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int index = dgvBook.CurrentRow.Index;
            DataGridViewRow dataRow = dgvBook.Rows[index];
            txtNameBook.Text = dataRow.Cells[1].Value.ToString();
            nmBook.Value = Decimal.Parse(dataRow.Cells[2].Value.ToString());
            cbTypeBook.SelectedValue = dataRow.Cells[3].Value.ToString();
            btnAddBook.Text = "Xác nhận";
            btnAddBook.Tag = dataRow.Cells[0].Value.ToString();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int index = dgvBook.CurrentRow.Index;
            DataGridViewRow dataRow = dgvBook.Rows[index];
            string name = dataRow.Cells[1].Value.ToString();
            string id = dataRow.Cells[0].Value.ToString();
            DialogResult res = MessageBox.Show(string.Format("Thực hiện xóa sách {0} khỏi hệ thống", name), "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (res == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("DELETE tblSach WHERE sMaS = @mas", con);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@mas", SqlDbType.NVarChar).Value = id;
                        con.Open();
                        int rowCount = cmd.ExecuteNonQuery();
                        if (rowCount > 0)
                        {
                            DataRow[] rows = dtBook.Select(String.Format("sMaS = '{0}'", id));
                            foreach (DataRow row in rows)
                            {
                                row.Delete();
                            }
                            dtBook.AcceptChanges();
                            DataView dvBook = dtBook.DefaultView;
                            dgvBook.DataSource = dvBook;
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
        private void btnPrint_Click(object sender, EventArgs e)
        {
            ViewReport.ViewReport view = new ViewReport.ViewReport("Sach", dtBook);
            view.Show();
        }

        private void btnAddType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNameType.Text))
            {
                MessageBox.Show("Nhập tên thể loại");
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    var count = dtType.Rows.Count + 1;
                    string id = string.Format("TL{0}", createId(count));
                    SqlCommand cmd = new SqlCommand("insertTheloai", con);
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
            if (string.IsNullOrEmpty(txtNameBook.Text) || nmBook.Value < 1 || cbTypeBook.SelectedValue == null)
            {
                MessageBox.Show("Nhập đầy đủ thông tin sách");
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = null;
                    string id = null;
                    if (btnAddBook.Tag == null)
                    {
                        var count = dtBook.Rows.Count + 1;
                        id = string.Format("S{0}", createId(count));
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
                            dtBook.Rows.Add(new Object[] { id, txtNameBook.Text, int.Parse(nmBook.Value.ToString()), cbTypeBook.SelectedValue.ToString(), cbTypeBook.GetItemText(cbTypeBook.SelectedItem), 0 });
                            dtBook.AcceptChanges();
                            DataView dvBook = dtBook.DefaultView;
                            dgvBook.DataSource = dvBook;
                            txtNameBook.Clear();
                            nmBook.Value = 0;
                        }
                    }
                    else
                    {
                        id = btnAddBook.Tag.ToString();
                        cmd = new SqlCommand("updateSach", con);
                        cmd.Parameters.Add("@mas", SqlDbType.NVarChar).Value = id;
                        cmd.Parameters.Add("@tens", SqlDbType.NVarChar).Value = txtNameBook.Text;
                        cmd.Parameters.Add("@mal", SqlDbType.NVarChar).Value = cbTypeBook.SelectedValue.ToString();
                        cmd.Parameters.Add("@sl", SqlDbType.Int).Value = nmBook.Value;
                        con.Open();
                        int rowCount = cmd.ExecuteNonQuery();
                        if (rowCount > 0)
                        {
                            DataRow row = dtBook.Select(String.Format("sMaS = '{0}'", id))[0];
                            row["sTenS"] = txtNameBook.Text;
                            row["sMaL"] = cbTypeBook.SelectedValue.ToString();
                            row["sTenL"] = cbTypeBook.GetItemText(cbTypeBook.SelectedItem);
                            row["iSoLuong"] = nmBook.Value;
                            dtBook.AcceptChanges();
                            DataView dvBook = dtBook.DefaultView;
                            dgvBook.DataSource = dvBook;
                            txtNameBook.Clear();
                            nmBook.Value = 0;
                            btnAddBook.Text = "Thêm";
                            btnAddBook.Tag = null;
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
        private void btnSearch_Click(object sender, EventArgs e)
        {
            DataView dvBook = dtBook.DefaultView;
            string select = "";
            if (!string.IsNullOrEmpty(txtSearch.Text.ToString()))
            {
                select += string.Format("sTenS like %{0}%", txtSearch.Text.ToString());
            }
            if (!string.IsNullOrEmpty(cbTypeSearch.SelectedValue.ToString()) && string.IsNullOrEmpty(txtSearch.Text.ToString()))
            {
                select += string.Format("sMaL = '{0}'", cbTypeSearch.SelectedValue.ToString());
            }
            else if (!string.IsNullOrEmpty(cbTypeSearch.SelectedValue.ToString()) && !string.IsNullOrEmpty(txtSearch.Text.ToString()))
            {
                select += string.Format(" and sMaL = '{0}'", cbTypeSearch.SelectedValue.ToString());
            }
            dvBook.RowFilter = select;
            dgvBook.DataSource = dvBook;
        }

        private void FormLibrary_Load(object sender, EventArgs e)
        {
            if(dtType == null && dtBook == null && dtTypeSearch == null)
            {
                loadData();
            }
            cbTypeBook.DataSource = dtType;
            cbTypeBook.DisplayMember = "sTenL";
            cbTypeBook.ValueMember = "sMaL";
            
            cbTypeSearch.DataSource = dtTypeSearch;
            cbTypeSearch.DisplayMember = "sTenL";
            cbTypeSearch.ValueMember = "sMaL";
            DataView dvBook = dtBook.DefaultView;
            dgvBook.DataSource = dvBook;
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

                    dtTypeSearch = new DataTable();
                    dtTypeSearch.Columns.Add("sTenL");
                    dtTypeSearch.Columns.Add("sMaL");
                    DataRow dr = dtTypeSearch.NewRow();
                    dr["sMaL"] = "";
                    dr["sTenL"] = "Tất cả";
                    dtTypeSearch.Rows.Add(dr);
                    foreach(DataRow row in dtType.Rows)
                    {
                        DataRow drow = dtTypeSearch.NewRow();
                        drow["sMaL"] = row["sMaL"];
                        drow["sTenL"] = row["sTenL"];
                        dtTypeSearch.Rows.Add(drow);
                    }

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
                id += string.Format("{0}", 0);
            }
            id += string.Format("{0}", count);
            return id;
        }

    }
}
