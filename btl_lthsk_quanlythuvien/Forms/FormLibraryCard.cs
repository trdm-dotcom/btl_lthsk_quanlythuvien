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
    public partial class FormLibraryCard : Form
    {
        private User user;
        string connectionString = ConfigurationManager.ConnectionStrings["db_btlthsk"].ConnectionString;
        private DataTable dtCard = null;
        private DataTable dtBook = null;
        private DataTable dt = null;
        private DataTable dtStudent = null;
        private DataTable dtUser;
        public FormLibraryCard(User user)
        {
            InitializeComponent();
            this.user = user;
        }
        private void FormLibraryCard_Load(object sender, EventArgs e)
        {
            if (dtCard == null)
            {
                loadData();
                dtUser = new DataTable();
                dtUser.Clear();
                dtUser.Columns.Add("sMaTT");
                dtUser.Columns.Add("sTenTT");
                DataRow dr = dtUser.NewRow();
                dr["sMaTT"] = this.user.Iduser;
                dr["sTenTT"] = this.user.Name;
                dtUser.Rows.Add(dr);
                foreach (DataColumn col in dtCard.Columns) col.ReadOnly = false;
            }
            DataView dvCard = dtCard.DefaultView;
            dgvLibraryCard.DataSource = dvCard;
            cbUser.DataSource = dtUser;
            cbUser.DisplayMember = "sTenTT";
            cbUser.ValueMember = "sMaTT";

            dt = new DataTable();
            dt.Columns.Add("sMaS");
            dt.Columns.Add("sTenS");

            DataTable dtType = new DataTable();
            dtType.Columns.Add("value");
            dtType.Columns.Add("display");
            DataRow drType = dtType.NewRow();
            drType["value"] = "";
            drType["display"] = "Tất cả";
            dtType.Rows.Add(drType);
            DataRow drType1 = dtType.NewRow();
            drType1["value"] = "0";
            drType1["display"] = "Chưa trả";
            dtType.Rows.Add(drType1);
            DataRow drType2 = dtType.NewRow();
            drType2["value"] = "1";
            drType2["display"] = "Đã trả";
            dtType.Rows.Add(drType2);
            cbType.DataSource = dtType;
            cbType.DisplayMember = "display";
            cbType.ValueMember = "value";
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            DataView dvCard = dtCard.DefaultView;
            string select = "";
            if (!string.IsNullOrEmpty(txtIDSearch.Text.ToString()))
            {
                select += string.Format("sMaPhieu = '{0}'", txtIDSearch.Text.ToString());
            }
            if (!string.IsNullOrEmpty(cbType.SelectedValue.ToString()) && string.IsNullOrEmpty(txtIDSearch.Text.ToString()))
            {
                select += string.Format("iTrangthai = '{0}'", cbType.SelectedValue.ToString());
            }
            else if(!string.IsNullOrEmpty(cbType.SelectedValue.ToString()) && !string.IsNullOrEmpty(txtIDSearch.Text.ToString()))
            {
                select += string.Format(" and iTrangthai = '{0}'", cbType.SelectedValue.ToString());
            }
            dvCard.RowFilter = select;
            dgvLibraryCard.DataSource = dvCard;
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dgvLibraryCard.CurrentRow.Index;
                DataGridViewRow dataRow = dgvLibraryCard.Rows[index];
                string idCard = dataRow.Cells[0].Value.ToString();
                DataRow dr = dtCard.Select(string.Format("sMaPhieu = '{0}'", idCard))[0];
                string id = dr["sMaPhieu"].ToString();
                int trangthai =  int.Parse(dr["iTrangthai"].ToString());
                txtId.Text = id;
                txtIDSV.Text = dr["sMaSV"].ToString();
                dateTimePicker1.Value = Convert.ToDateTime(dr["dNgayMuon"].ToString());

                dtUser.Clear();
                DataRow drUser = dtUser.NewRow();
                drUser["sMaTT"] = dr["sMaTT"].ToString();
                drUser["sTenTT"] = dr["sTenTT"].ToString();
                dtUser.Rows.Add(drUser);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("getChitiet", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@mapm", SqlDbType.NVarChar).Value = id;
                    con.Open();
                    SqlDataReader data = cmd.ExecuteReader();
                    DataTable temp = new DataTable();
                    temp.Load(data);
                    dt.Clear();
                    foreach (DataRow row in temp.Rows)
                    {
                        string idS = row["sMaS"].ToString();
                        string sach = dtBook.Select(string.Format("sMaS = '{0}'", idS))[0]["sTenS"].ToString();
                        DataRow drow = dt.NewRow();
                        drow["sMaS"] = idS;
                        drow["sTenS"] = sach;
                        dt.Rows.Add(drow);
                    }
                    dgvBook.DataSource = dt;
                }
                if(trangthai != 1)
                {
                    btnCreate.Enabled = true;
                    btnCreate.Text = "Xác nhận";
                    btnCreate.Tag = "conf";
                }
                else
                {
                    btnCreate.Enabled = false;
                    btnCreate.Text = "Tạo";
                    btnCreate.Tag = null;
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
        }
        private void btnDo_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = true;
            btnCreate.Enabled = true;
            string id = createId();
            txtId.Text = id;
        }
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtIDSV.Text.ToString()) || cbUser.SelectedValue == null || dt.Rows.Count < 1)
            {
                MessageBox.Show("Nhập thông tin phiếu mượn");
                return;
            }
            DataRow[] drStudent = dtStudent.Select(string.Format("sMaSV = '{0}'", txtIDSV.Text.ToString()));
            if (drStudent.Length < 1)
            {
                MessageBox.Show("Chưa có thông tin sinh viên trong hệ thống");
                return;
            }
            if(int.Parse(drStudent[0]["solan"].ToString()) > 3)
            {
                MessageBox.Show("Sinh viên quá số lần được mượn sách");
                return;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    if (btnCreate.Tag == null)
                    {
                        SqlCommand cmd = new SqlCommand("insertPhieumuon", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@mapm", SqlDbType.NVarChar).Value = txtId.Text.ToString();
                        cmd.Parameters.Add("@matt", SqlDbType.NVarChar).Value = cbUser.SelectedValue.ToString();
                        cmd.Parameters.Add("@masv", SqlDbType.NVarChar).Value = txtIDSV.Text.ToString();
                        cmd.Parameters.Add("@dngaymuon", SqlDbType.Date).Value = dateTimePicker1.Value.Date;
                        int rowCount = cmd.ExecuteNonQuery();
                        if (rowCount > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                cmd = new SqlCommand("insertChitiet", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@mapm", SqlDbType.NVarChar).Value = txtId.Text.ToString();
                                cmd.Parameters.Add("@mas", SqlDbType.NVarChar).Value = row["sMaS"];
                                cmd.ExecuteNonQuery();
                            }
                            String tenSV = dtStudent.Select(string.Format("sMaSV = '{0}'", txtIDSV.Text.ToString()))[0]["sTenSV"].ToString();
                            String tenTT = cbUser.GetItemText(cbUser.SelectedItem);
                            dtCard.Rows.Add(new Object[] { txtId.Text.ToString(), cbUser.SelectedValue.ToString(), txtIDSV.Text.ToString(), dateTimePicker1.Value.Date, tenTT, tenSV, 0,"Chưa trả" });
                            dtCard.AcceptChanges();
                            DataView dvCard = dtCard.DefaultView;
                            dgvLibraryCard.DataSource = dvCard;
                            btnAdd.Enabled = false;
                            btnCreate.Enabled = false;
                            ViewReport.ViewPhieuMuon viewPhieuMuon = new ViewReport.ViewPhieuMuon(new LibraryCardInfo
                            {
                                idCard = txtId.Text.ToString(),
                                idUser = cbUser.SelectedValue.ToString(),
                                nameUser = tenTT,
                                nameStudent = tenSV,
                                idStudent = txtIDSV.Text.ToString(),
                                status = "Chưa trả",
                                date = Convert.ToDateTime(DateTime.Now.Date.ToString())
                            }, dt);
                            viewPhieuMuon.Show();
                            txtId.Clear();
                            txtIDSV.Clear();
                            dt.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Thực hiên lại");
                            return;
                        }
                    }
                    else
                    {
                        string idCard = txtId.Text.ToString();
                        SqlCommand cmd = new SqlCommand("traSach", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@mapm", SqlDbType.NVarChar).Value = idCard;
                        int rowCount = cmd.ExecuteNonQuery();
                        if (rowCount > 0)
                        {
                            DataRow row = dtCard.Select(string.Format("sMaPhieu = '{0}'", idCard))[0];
                            row["trangthai"] = "Đã trả";
                            dtCard.AcceptChanges();
                            DataView dvCard = dtCard.DefaultView;
                            dgvLibraryCard.DataSource = dvCard;
                            txtId.Clear();
                            txtIDSV.Clear();
                            dt.Clear();
                            btnCreate.Tag = null;
                            dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.Date.ToString());
                            dtUser.Clear();
                            DataRow drUser = dtUser.NewRow();
                            drUser["sMaTT"] = this.user.Iduser;
                            drUser["sTenTT"] = this.user.Name;
                            dtUser.Rows.Add(drUser);
                            btnCreate.Enabled = false;
                            btnCreate.Text = "Tạo";
                            MessageBox.Show("Hoàn thành");
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Thực hiên lại");
                            return;
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
        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataView dvCard = dtCard.DefaultView;
            string select = "";
            if (!string.IsNullOrEmpty(txtIDSearch.Text.ToString()))
            {
                select += string.Format("sMaPhieu = '{0}'", txtIDSearch.Text.ToString());
            }
            if (!string.IsNullOrEmpty(cbType.SelectedValue.ToString()) && string.IsNullOrEmpty(txtIDSearch.Text.ToString()))
            {
                select += string.Format("iTrangthai = '{0}'", cbType.SelectedValue.ToString());
            }
            else if (!string.IsNullOrEmpty(cbType.SelectedValue.ToString()) && !string.IsNullOrEmpty(txtIDSearch.Text.ToString()))
            {
                select += string.Format(" and iTrangthai = '{0}'", cbType.SelectedValue.ToString());
            }
            dvCard.RowFilter = select;
            DataTable dtTemp = dvCard.ToTable();
            ViewReport.ViewReport view = new ViewReport.ViewReport("DanhSachPhieu", dtTemp);
            view.Show();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtIDBook.Text.ToString()))
            {
                MessageBox.Show("Nhập mã sách");
                return;
            }
            try
            {

                DataRow[] dr = dtBook.Select(string.Format("sMaS = '{0}'", txtIDBook.Text.ToString().ToString()));
                if(dr.Length < 1)
                {
                    MessageBox.Show("Không tìm thấy sách");
                    return;
                }
                if(dt.Select(string.Format("sMaS = '{0}'", txtIDBook.Text.ToString().ToString())).Length > 0)
                {
                    MessageBox.Show("Sinh viên được mượn mỗi sách 1 quyển");
                    return;
                }
                int tong = int.Parse(dr[0]["iSoLuong"].ToString());
                int muon = int.Parse(dr[0]["duocmuon"].ToString());
                if(muon >= tong)
                {
                    MessageBox.Show("Kiểm tra lại mã sách");
                    return;
                }
                DataRow row = dt.NewRow();
                row["sMaS"] = dr[0]["sMaS"];
                row["sTenS"] = dr[0]["sTenS"];
                dt.Rows.Add(row);
                dgvBook.DataSource = dt;
                txtIDBook.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception("Error" + ex.Message);
            }
        }
        private string createId()
        {
            try
            {
                var now = DateTime.Now.Date;
                var count = dtCard.Select(string.Format("dNgayMuon = #{0}#", now.ToString())).Length;
                string id = string.Format("PM{0}{1}", dateTimePicker1.Value.ToString("ddMMyy"), count);
                return id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error" + ex.Message);
            }
        }
        private void loadData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("getPhieu", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader data = cmd.ExecuteReader();
                    dtCard = new DataTable();
                    dtCard.Load(data);
                
                    cmd = new SqlCommand("getSach", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    data = cmd.ExecuteReader();
                    dtBook = new DataTable();
                    dtBook.Load(data);

                    cmd = new SqlCommand("getStudent", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    data = cmd.ExecuteReader();
                    dtStudent = new DataTable();
                    dtStudent.Load(data);
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
