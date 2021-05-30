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
        private string mail ="";
        private string pass ="";

        public Form1()
        {
            InitializeComponent();
            login = new Login();
            login.main = this;

           
            menu = new Menu();
            menu.setMail(mail);
            menu.setPass(pass);
            login.Visible = true;
            menu.Visible = false;
            panel1.Controls.Add(login);
            panel1.Controls.Add(menu);
        }

        private void Form1_Load(object sender, EventArgs e)
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
    }
}
