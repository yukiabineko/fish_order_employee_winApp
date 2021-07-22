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
        [System.Runtime.InteropServices.DllImport("kernel32.dll")] // この行を追加
        private static extern bool AllocConsole();

        public Form1()
        {
            InitializeComponent();
            AllocConsole();
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
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            // 最大化・最小化の無効
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            menuStrip1.Visible = false;
           
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


        private void menuStrip1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {
            login.Visible = true;
            menu.Visible = false;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            MessageBox.Show("ログアウトしました。");
            menuStrip1.Visible = false;
            login.Visible = true;
            menu.Visible = false;
            
            label1.Text = "";
            label2.Text = "";
            login.textBox1.Text = "";
            login.textBox2.Text = "";
            
        }
    }
}