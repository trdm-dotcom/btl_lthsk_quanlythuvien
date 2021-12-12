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
    public partial class ViewPhieuMuon : Form
    {
        private LibraryCardInfo libraryCardInfo;
        private DataTable dtBook;
        public ViewPhieuMuon(LibraryCardInfo libraryCardInfo, DataTable dtBook)
        {
            InitializeComponent();
            this.libraryCardInfo = libraryCardInfo;
            this.dtBook = dtBook;
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            List<Book> _list = new List<Book>();
            foreach(DataRow dr in dtBook.Rows)
            {
                _list.Add(new Book { 
                    idBook = dr["sMaS"].ToString(), 
                    nameBook = dr["sTenS"].ToString(),
                    idType = null,
                    nameType = null,
                    quantity = 0,
                    count = 0
                });
            }
            phieuMuon1.SetDataSource(_list);
            phieuMuon1.SetParameterValue("pNameStudent",libraryCardInfo.nameStudent);
            phieuMuon1.SetParameterValue("pNameUser",libraryCardInfo.nameUser);
            phieuMuon1.SetParameterValue("pDate",libraryCardInfo.date);
            phieuMuon1.SetParameterValue("pTotal", dtBook.Rows.Count);
            crystalReportViewer1.ReportSource = phieuMuon1;
        }
    }
}
