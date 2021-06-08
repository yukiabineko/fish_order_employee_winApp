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


            DataGridViewTextBoxColumn name = new DataGridViewTextBoxColumn();
            name.HeaderText = "商品名";
            name.Name = "show";
            name.Width = 140;


            DataGridViewTextBoxColumn price = new DataGridViewTextBoxColumn();
            price.HeaderText = "価格";
            price.Width = 140;


            DataGridViewTextBoxColumn stock = new DataGridViewTextBoxColumn();
            stock.HeaderText = "在庫";
            stock.Width = 120;

            DataGridViewTextBoxColumn total = new DataGridViewTextBoxColumn();
            total.HeaderText = "合計";
            total.Width = 140;

            DataGridViewButtonColumn edit = new DataGridViewButtonColumn();
            edit.Name = "edit";
            edit.Text = "編集";
            edit.Name = "edit";
            edit.UseColumnTextForButtonValue = true;
            edit.Width = 140;


            DataGridViewButtonColumn delete = new DataGridViewButtonColumn();
            delete.Name = "delete";
            delete.Text = "削除";
            delete.Name = "delete";
            delete.UseColumnTextForButtonValue = true;
            delete.Width = 140;


            
            dataGridView1.Columns.Add(name);
            dataGridView1.Columns.Add(price);
            dataGridView1.Columns.Add(stock);
            dataGridView1.Columns.Add(total);
            dataGridView1.Columns.Add(edit);
            dataGridView1.Columns.Add(delete);

            dataGridView1.Columns[4].DefaultCellStyle.BackColor = Color.Blue;
            dataGridView1.Columns[4].DefaultCellStyle.ForeColor = Color.White;
            dataGridView1.Columns[5].DefaultCellStyle.BackColor = Color.Red;
            dataGridView1.Columns[5].DefaultCellStyle.ForeColor = Color.White;

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if(dgv.Columns[e.ColumnIndex].Name == "edit")
            {
                MessageBox.Show("edit");
            }
            else {
                MessageBox.Show("delete");
            }
        }
    }
}
