using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace btl_lthsk_quanlythuvien
{
    public partial class MainForm : Form
    {
        private User user;
        private Button currentButton;
        private Form activateForm;
        public MainForm()
        {
            InitializeComponent();
        }
        public MainForm(User user)
        {
            InitializeComponent();
            this.user = user;
        }
        private void ActivateButton(object btnSender)
        {
            if(btnSender != null)
            {
                if(currentButton != (Button)btnSender)
                {
                    DisableButton();
                    currentButton = (Button)btnSender;
                    currentButton.BackColor = Color.FromArgb(30, 136, 229);
                    currentButton.ForeColor = Color.White;
                    currentButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }
            }
        }
        
        private void DisableButton()
        {
            foreach(Control previousBtn in panelMenu.Controls)
            {
                if(previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(66, 165, 245);
                    previousBtn.ForeColor = Color.White;
                    previousBtn.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }
            }
        }

        private void OpenChildForm(Form childForm, object btnSender)
        {
            if(activateForm != null)
            {
                activateForm.Close();
            }
            ActivateButton(btnSender);
            activateForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            childForm.AutoSize = true;
            this.panelDesktop.Controls.Add(childForm);
            this.panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void btnStudent_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormStudent(), sender);
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormLibrary(), sender);
        }

        private void btnCard_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormLibraryCard(this.user), sender);
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormUser(), sender);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormChangePass(this.user), sender);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lbWelcome.Text = string.Format("Xin chào {0} đăng nhập vào hệ thống", this.user.Name);
            if(user.Type != 1)
            {
                btnUser.Hide();
            }
        }
    }
}
