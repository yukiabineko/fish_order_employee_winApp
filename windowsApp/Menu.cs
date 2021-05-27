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
        public Menu()
        {
            InitializeComponent();
            tabPage1.Text = "商品管理";
            tabPage2.Text = "会員管理";
            tabPage1.Controls.Add(new ItemControl());
            tabPage2.Controls.Add(new UserControl1());
            
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
