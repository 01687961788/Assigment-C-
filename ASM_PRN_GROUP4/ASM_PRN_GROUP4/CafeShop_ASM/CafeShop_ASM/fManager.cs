using CafeShop_ASM.DAO;
using CafeShop_ASM.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace CafeShop_ASM
{
    public partial class fManager : Form
    {
        private AccountDTO account = null;
        BindingSource listBill = new BindingSource();
        private bool checkPay = false;
        private bool isNewBill = false;
        public fManager()
        {
            InitializeComponent();
        }
        public fManager(AccountDTO dto)
        {
            account = dto;
            InitializeComponent();           
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            loadData();
        }
        private void loadData()
        {
            loadCategory();
            loadBill();
            txtUsername.Text = "User: "+account.Fullname;
            txtUsername.ReadOnly = true;
        }
        #region method product & category
        private void loadCategory()
        {
            List<CategoryDTO> listCategory = CategoryDAO.Instance.getListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "cateName";
        }
        private void loadProductByCategoryID(string cateID)
        {
            List<ProductDTO> listProduct = ProductDAO.Instance.getListProductByCategoryID(cateID);
            cbProductByCategoryID.DataSource = listProduct;
            cbProductByCategoryID.DisplayMember = "proName";
        }
        #endregion
        #region event common
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccount f = new fAccount(this.account);
            f.ShowDialog();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.account.IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền truy cập", "Thông báo");
                return;
            }
            fAdmin f = new fAdmin();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }
        #endregion
        
        #region event of product panel
        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cateID = "";
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
            {
                return;
            }
            CategoryDTO selectedCateID = cb.SelectedItem as CategoryDTO;
            cateID = selectedCateID.CateID;
            loadProductByCategoryID(cateID);
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            if (txtCurrentBillID.Text.Equals(""))
            {
                MessageBox.Show("Hãy nhấn tạo mới hóa đơn trước", "Thông báo");
                return;
            }
            CultureInfo culture = new CultureInfo("vi-VN");

            int billID = BillDAO.Instance.getLastBillID();
            string proID = (cbProductByCategoryID.SelectedItem as ProductDTO).ProID;
            string proName = (cbProductByCategoryID.SelectedItem as ProductDTO).ProName;//cot 0
            int quantity =(int)nricProductQuantity.Value;//cot 1
            double unitPrice = (double)(cbProductByCategoryID.SelectedItem as ProductDTO).UnitPrice;//cot 2
            double totalPrice = quantity * unitPrice;//cot 3
            double totalBill = 0;

            bool checkDuplicate = false;
            
            if(lsvBill.Items.Count == 0)
            {
                if (quantity <= 0)
                {
                    return;
                }
                ListViewItem lsvItem = new ListViewItem(proName);
                lsvItem.SubItems.Add(quantity.ToString());
                lsvItem.SubItems.Add(unitPrice.ToString());
                lsvItem.SubItems.Add(totalPrice.ToString());
                lsvBill.Items.Add(lsvItem);
            }
            else
            {
                foreach (ListViewItem item in lsvBill.Items)
                {
                    if (item.SubItems[0].Text.Equals(proName))
                    {
                        int editQuanity = Convert.ToInt32(item.SubItems[1].Text) + quantity;
                        if (editQuanity <= 0)
                        {
                            lsvBill.Items.Remove(item);
                        }
                        item.SubItems[1].Text = editQuanity.ToString();
                        item.SubItems[3].Text = (editQuanity * Convert.ToDouble(item.SubItems[2].Text)).ToString();
                        checkDuplicate = true;
                        break;
                    }
                    
                }
                if (!checkDuplicate && quantity > 0)
                {
                    ListViewItem lsvItem = new ListViewItem(proName);
                    lsvItem.SubItems.Add(quantity.ToString());
                    lsvItem.SubItems.Add(unitPrice.ToString());
                    lsvItem.SubItems.Add(totalPrice.ToString());
                    lsvBill.Items.Add(lsvItem);
                }
            }
            foreach (ListViewItem item in lsvBill.Items)
            {
                totalBill += Convert.ToDouble(item.SubItems[3].Text);
            }
            txtTotalPrice.Text = totalBill.ToString("c",culture);
            nricProductQuantity.Value = 0;
            checkPay = true;
        }
        #endregion
        #region method bill


        #endregion
        #region event bill
        private void btnNewBill_Click(object sender, EventArgs e)
        {
            if (checkPay)
            {
                MessageBox.Show("Bạn chưa thanh toán hóa đơn!!!","Thông báo");
                return;
            }
            if(isNewBill)
            {
                MessageBox.Show("Hóa đơn trống!!!", "Thông báo");
                return;
            }
            BillDAO.Instance.insertBill(this.account.Username);
            txtCurrentBillID.Text = BillDAO.Instance.getLastBillID().ToString();
            isNewBill = true;
            lsvBill.Items.Clear();
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            string billID = txtCurrentBillID.Text;
            if (billID.Equals(""))
            {
                MessageBox.Show("Hãy tạo hóa hơn mới trước", "Thông báo");
                return;
            }
            if (!billID.Equals("") && lsvBill.Items.Count == 0)
            {
                MessageBox.Show("Hóa đơn trống!!!", "Thông báo");
                return;
            }
            foreach (ListViewItem item in lsvBill.Items)
            {
                string proID = ProductDAO.Instance.getProductIDbyProName(item.SubItems[0].Text);
                string quantity = item.SubItems[1].Text;
                string unitPrice = item.SubItems[2].Text;
                BillInfoDAO.Instance.insertBillInfo(billID, proID, quantity, unitPrice);
            }
            BillDAO.Instance.updateBillStatus(billID);
            MessageBox.Show("Thanh toán hoàn tất", "Thông báo");
            lsvBill.Clear();
            txtCurrentBillID.Text = "";
            txtTotalPrice.Text = "";
            checkPay = false;
            isNewBill = false;
            loadBill();
        }
        private void loadBill()
        {
            listBill.DataSource = BillDAO.Instance.getListBill();
        }
        
        private void dtgvBill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }


        #endregion
    }
}
