using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace windowsApp
{
    public partial class Product : UserControl
    {
        public Product()
        {
            InitializeComponent();
            groupBox1.Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            progressBar1.Minimum = 10;
            progressBar1.Maximum = 100;
            progressBar1.Value = progressBar1.Minimum;


            DataGridViewButtonColumn name = new DataGridViewButtonColumn();
            name.HeaderText = "商品名";
            name.Width = 140;


            DataGridViewButtonColumn price = new DataGridViewButtonColumn();
            price.HeaderText = "価格";
            price.Width = 140;


            DataGridViewButtonColumn stock = new DataGridViewButtonColumn();
            stock.HeaderText = "在庫";
            stock.Width = 120;

            DataGridViewButtonColumn total = new DataGridViewButtonColumn();
            total.HeaderText = "加工法";
            total.Width = 140;

            DataGridViewButtonColumn edit = new DataGridViewButtonColumn();
            edit.Name = "edit";
            edit.Text = "編集";
            edit.DefaultCellStyle.BackColor = Color.Blue;
            edit.DefaultCellStyle.ForeColor = Color.White;
            edit.UseColumnTextForButtonValue = true;
            edit.Width = 140;


            DataGridViewButtonColumn delete = new DataGridViewButtonColumn();
            delete.Name = "delete";
            delete.Text = "削除";
            delete.DefaultCellStyle.BackColor = Color.Red;
            delete.DefaultCellStyle.ForeColor = Color.White;
            delete.UseColumnTextForButtonValue = true;
            delete.Width = 140;


            dataGridView1.Columns.Add(name);
            dataGridView1.Columns.Add(price);
            dataGridView1.Columns.Add(stock);
            dataGridView1.Columns.Add(total);
            dataGridView1.Columns.Add(edit);
            dataGridView1.Columns.Add(delete);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            for(var i = progressBar1.Minimum; i<progressBar1.Maximum; i++)
            {
                progressBar1.Value = i;
            }
            string url = "https://uematsu-backend.herokuapp.com/orders";
            using(WebClient webClient = new WebClient())
            {
                webClient.DownloadStringAsync(new Uri(url));
                webClient.DownloadStringCompleted += (s, o) =>
                {
                    string data = o.Result;
                    JArray array = JArray.Parse(data);
                    Console.WriteLine(data);
                    foreach(var arr in array)
                    {
                        dataGridView1.Rows.Add(
                           arr["name"],
                           arr["price"],
                           arr["stock"],
                           arr["process"]
                       );
                    };
                    groupBox1.Visible = false;
                };
               
           
            };
        }
    }
}
