﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections.Specialized;


namespace windowsApp
{
    public partial class UserNew : Form
    {
        public UserNew()
        {
            InitializeComponent();
            textBox4.PasswordChar = '*';
            textBox5.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using(WebClient webClient = new WebClient())
            {
                NameValueCollection collection = new NameValueCollection();
                
                if(textBox1.Text == "" ||  textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "")
                {
                    MessageBox.Show("未入力な項目があります。");
                }
                else if(textBox4.Text != textBox5.Text)
                {
                    MessageBox.Show("パスワードをご確認ください。");
                }
                else if (IsValidMailAddress(textBox2.Text) == false)
                {
                    MessageBox.Show("メールアドレスが不正です。");
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(textBox3.Text, @"\A0\d{1,4}\d{1,4}\d{4}\z") == false)
                {
                    MessageBox.Show("電話番号が不正です。");
                }
                else
                {
                    string userAddUrl = "https://uematsu-backend.herokuapp.com/users";

                    collection.Add("name", textBox1.Text);
                    collection.Add("email", textBox2.Text);
                    collection.Add("tel", textBox3.Text);
                    collection.Add("password", textBox4.Text);
                    collection.Add("password_confirmation", textBox5.Text);
                    webClient.UploadValuesAsync(new Uri(userAddUrl), collection);
                    webClient.UploadValuesCompleted += (s, o) =>
                    {
                        MessageBox.Show("登録しました。");
                        this.Close();
                    };

                }
                

            };
        }


        private void UserNew_Load(object sender, EventArgs e)
        {

        }

        private void textBox3_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if ('0' <= e.KeyChar && e.KeyChar <= '9')
            {

            }
            else if (e.KeyChar == '\b')
            {
            }
            else
            {
                e.Handled = true;
            }
        }
        public static bool IsValidMailAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return false;
            }

            try
            {
                System.Net.Mail.MailAddress a =
                    new System.Net.Mail.MailAddress(address);
            }
            catch (FormatException)
            {
                //FormatExceptionがスローされた時は、正しくない
                return false;
            }

            return true;
        }
    }
}
