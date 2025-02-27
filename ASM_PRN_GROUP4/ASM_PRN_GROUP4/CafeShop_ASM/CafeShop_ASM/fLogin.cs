﻿using CafeShop_ASM.DAO;
using CafeShop_ASM.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeShop_ASM
{
    public partial class fLogin : Form
    {
        public fLogin()
        {
            InitializeComponent();
        }
        private AccountDTO checkLogin(string userName, string passWord)
        {           
            return AccountDAO.Instance.checkLogin(userName, passWord);
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            AccountDTO dto = checkLogin(username, password);
            if (dto!=null)
            {
                fManager f = new fManager(dto);
                this.Hide();
                f.ShowDialog();
                this.Show();
                txtUsername.Clear();
                txtUsername.Focus();
                txtPassword.Clear();
            }
            else
            {
                MessageBox.Show("Sai tên tài khoản hoặc mật khẩu!");
                txtUsername.Clear();
                txtUsername.Focus();
                txtPassword.Clear();
                
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void fLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát ứng dụng không?","Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}
