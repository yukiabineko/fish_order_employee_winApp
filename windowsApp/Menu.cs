using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace windowsApp
{
    public partial class Menu : UserControl
    {
        private string mail = "";
        private string pass = "";
        public UserControl1 users;
        public ItemControl items;
        public UserOrder userOrder;
        public Product product;
        public OrdersControl orders;

        public Menu()
        {
            InitializeComponent();
            items = new ItemControl();
            users = new UserControl1();
            product = new Product();
            product.main = this;
            orders = new OrdersControl();
            orders.main = this;


            tabPage1.Text = "商品管理";
            tabPage2.Text = "会員管理";
            tabPage3.Text = "店頭商品管理";
            tabPage4.Text = "本日注文状況";
            users.menu = this;
            
            tabPage1.Controls.Add(items);
            tabPage2.Controls.Add(users);
            tabPage3.Controls.Add(product);
            tabPage4.Controls.Add(orders);
            
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            product.setEmail(mail);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        public void setMail(string s)
        {
            mail = s;
        }
        public string getMail()
        {
            return mail;
        }
        public void setPass(string s)
        {
            pass = s;
        }
        public string getPass()
        {
            return pass;
        }

        private void Menu_Load(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
           
        }
    }

}

