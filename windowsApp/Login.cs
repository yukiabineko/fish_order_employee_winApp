using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace windowsApp
{
    public partial class Login : UserControl
    {
        public Form1 main;
        private string sessinUrl = "https://uematsu-backend.herokuapp.com/sessions";

        public Login()
        {
            InitializeComponent();
            
            groupBox1.Visible = false;
            textBox2.PasswordChar = '・';
            progressBar1.Minimum = 10;
            progressBar1.Maximum = 100;
            progressBar1.Value = progressBar1.Minimum;
           

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            groupBox1.Visible = true;
            for(var i = progressBar1.Minimum; i<progressBar1.Maximum; i += 10)
            {
                progressBar1.Value = i;
            }
            
            using(WebClient wc = new WebClient())
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("email", textBox1.Text);
                collection.Add("password", textBox2.Text);
                wc.UploadValuesAsync(new Uri(sessinUrl), collection);
                wc.UploadValuesCompleted += (s, o) =>
                {
                    try
                    {
                        var data = System.Text.Encoding.UTF8.GetString(o.Result);
                        JObject obj = JObject.Parse(data);
                        if (obj["name"] != null)
                        {
                            main.label1.Text = (string)obj["name"];
                            main.label2.Text = (string)obj["email"];
                            main.menu.Visible = true;
                            this.Visible = false;
                            main.setMail(textBox1.Text);
                            main.setPass(textBox2.Text);
                            main.menu.setMail(textBox1.Text);
                            main.menu.setPass(textBox2.Text);

                            MessageBox.Show("ログインしました。");
                        }
                        else
                        {
                            MessageBox.Show("認証失敗しました。");
                            button1.Enabled = true;
                        }
                       
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("ログイン失敗しました。");
                    }
                    groupBox1.Visible = false;
                    button1.Enabled = true;

                };
            };
           
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Login_Load(object sender, EventArgs e)
        {
           
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UserNew usernew = new UserNew();
            usernew.ShowDialog(this);
            usernew.Dispose();
        }
    }
}
