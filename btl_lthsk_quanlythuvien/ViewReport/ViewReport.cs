using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace btl_lthsk_quanlythuvien.ViewReport
{
    public partial class ViewReport : Form
    {
        private string type;
        private DataTable dt;
        public ViewReport(string type, DataTable dt)
        {
            InitializeComponent();
            this.type = type;
            this.dt = dt;
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            try
            {
                if (type.Equals("DanhSachPhieu"))
                {
                    List<LibraryCardInfo> _list = new List<LibraryCardInfo>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        _list.Add(new LibraryCardInfo
                        {
                            idCard = dr["sMaPhieu"].ToString(),
                            idUser = dr["sMaTT"].ToString(),
                            idStudent = dr["sMaTT"].ToString(),
                            nameStudent = dr["sTenSV"].ToString(),
                            nameUser = dr["sTenTT"].ToString(),
                            status = dr["trangthai"].ToString(),
                            date = Convert.ToDateTime(dr["dNgayMuon"].ToString())
                        });
                    }
                    CrytalReport.DanhSachPhieu danhSachPhieu = new CrytalReport.DanhSachPhieu();
                    danhSachPhieu.SetDataSource(_list);
                    crystalReportViewer1.ReportSource = danhSachPhieu;
                }
                if (type.Equals("Sach"))
                {
                    List<Book> _list = new List<Book>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        _list.Add(new Book
                        {
                            idBook = dr["sMaS"].ToString(),
                            nameBook = dr["sTenS"].ToString(),
                            idType = dr["sMaL"].ToString(),
                            nameType = dr["sTenL"].ToString(),
                            quantity = int.Parse(dr["iSoLuong"].ToString()),
                            count = int.Parse(dr["duocmuon"].ToString())
                        });
                        CrytalReport.Sach sach = new CrytalReport.Sach();
                        sach.SetDataSource(_list);
                        crystalReportViewer1.ReportSource = sach;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error" + ex.Message);
            }
        }
    }
}
