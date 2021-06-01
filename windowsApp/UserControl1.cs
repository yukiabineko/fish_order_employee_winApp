﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace windowsApp
{
    public partial class UserControl1 : UserControl
    {
        public Menu menu;
        JArray array;

        public UserControl1()
        {
            InitializeComponent();
            groupBox1.Visible = false;
            progressBar1.Minimum = 10;
            progressBar1.Maximum = 100;
            progressBar1.Value = progressBar1.Minimum;

            dataGridView1.AllowUserToAddRows = false;
            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.HeaderText = "会員名";
            nameColumn.Width = 160;


            DataGridViewTextBoxColumn mailColumn = new DataGridViewTextBoxColumn();
            mailColumn.HeaderText = "メールアドレス";
            mailColumn.Width = 240;


            DataGridViewTextBoxColumn telColumn = new DataGridViewTextBoxColumn();
            telColumn.HeaderText = "電話番号";
            telColumn.Width = 155;


            DataGridViewButtonColumn deleteBtn = new DataGridViewButtonColumn();
            deleteBtn.UseColumnTextForButtonValue = true;
            deleteBtn.Name = "delete";
            deleteBtn.Text = "削除";
            deleteBtn.Width = 160;



            dataGridView1.Columns.Add(nameColumn);
            dataGridView1.Columns.Add(mailColumn);
            dataGridView1.Columns.Add(telColumn);
            dataGridView1.Columns.Add(deleteBtn);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            string usersUrl = "https://uematsu-backend.herokuapp.com/users/index";
            groupBox1.Visible = true;
            for(var i = progressBar1.Minimum; i<progressBar1.Maximum; i += 10)
            {
                progressBar1.Value = i;
            }

            using (WebClient webClient = new WebClient())
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("email", menu.getMail());
                collection.Add("password", menu.getPass());
                webClient.UploadValuesAsync(new Uri(usersUrl), collection);
                webClient.UploadValuesCompleted += (s, o) =>
                {
                    string data = System.Text.Encoding.UTF8.GetString(o.Result);
                    array = JArray.Parse(data);
                    foreach(var obj in array)
                    {
                        dataGridView1.Rows.Add(
                            obj["name"],
                            obj["email"],
                            obj["tel"]
                        );
                    }
                    groupBox1.Visible = false;
                };

            };
           
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(menu.getMail());
            UserNew usernew = new UserNew();
            usernew.ShowDialog(this);
            usernew.Dispose();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if(dgv.Columns[e.ColumnIndex].Name == "delete")
            {

                DialogResult result = MessageBox.Show("削除しますか？","",MessageBoxButtons.YesNo);
                if(result == DialogResult.Yes)
                {
                    JToken obj = array[e.RowIndex];
                    MessageBox.Show((string)obj["id"]);
                    string deleteUrl = "https://uematsu-backend.herokuapp.com/users/" + (string)obj["id"];
                    using (WebClient webClient = new WebClient())
                    {
                        NameValueCollection collection = new NameValueCollection();
                        collection.Add("id", (string)obj["id"]);
                        collection.Add("email", menu.getMail());
                        collection.Add("password", menu.getPass());
                        webClient.UploadValuesAsync(new Uri(deleteUrl), "delete", collection);
                        webClient.UploadValuesCompleted += (s, o) =>
                        {
                            dgv.Rows[e.RowIndex].Visible = false;
                            foreach(DataGridViewRow r in dgv.SelectedRows)
                            {
                                dgv.Rows.Remove(r);
                            }
                            MessageBox.Show("削除しました。");

                        };
                    };

                }
            }
            else
            {
                UserOrder orderWin = new UserOrder();
                orderWin.ShowDialog(this);
                orderWin.Dispose();
            }
        }
    }
}
