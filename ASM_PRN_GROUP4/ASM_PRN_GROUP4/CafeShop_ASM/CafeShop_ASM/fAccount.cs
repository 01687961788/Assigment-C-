using CafeShop_ASM.DAO;
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
    public partial class fAccount : Form
    {
        public fAccount()
        {
            InitializeComponent();
        }
        public fAccount(AccountDTO account)
        {
            InitializeComponent();
            loadUserAccount(account);
        }
        private void loadUserAccount(AccountDTO account)
        {            
            txtUsername.Text = account.Username;
            txtFullName.Text = account.Fullname;
            txtPassword.Text = account.Password;
        }
        private void updateAccount(string username, string fullname, string password)
        {
            if (password.Trim().Length == 0)
            {
                MessageBox.Show("Chưa nhập mật khẩu mới");
            }else {
                if (AccountDAO.Instance.updateAccount(username, fullname, password))
            {
                MessageBox.Show("Cập nhật tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại");
            } }
            
        }
        
        #region event
        private void txtUpdate_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string fullname = txtFullName.Text;
            string newpass = txtNewPassword.Text;
            updateAccount(username,fullname,newpass);
        }
        private void txtExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cbShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowPass.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void cbShowNewPass_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowNewPass.Checked)
            {
                txtNewPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtNewPassword.UseSystemPasswordChar = true;
            }
        }
        #endregion
    }
}
