using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace windowsApp
{
    public partial class Form1 : Form
    {
        public Login login;
        public Menu menu;

        public Form1()
        {
            InitializeComponent();
            login = new Login();
            login.main = this;
            menu = new Menu();
            login.Visible = true;
            menu.Visible = false;
            panel1.Controls.Add(login);
            panel1.Controls.Add(menu);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
