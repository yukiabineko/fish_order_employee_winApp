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
    public partial class ItemControl : UserControl
    {
        JArray array;

        public ItemControl()
        {
            InitializeComponent();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowTemplate.Height = 60;
           
            groupBox1.Visible = false;
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 10;
            progressBar1.Value = progressBar1.Minimum;

            DataGridViewImageColumn imgCell = new DataGridViewImageColumn();
            imgCell.HeaderText = "画像";
            imgCell.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgCell.Width = 130;


            DataGridViewTextBoxColumn nameCell = new DataGridViewTextBoxColumn();
            nameCell.HeaderText = "商品名";
            nameCell.Width = 130;

            DataGridViewTextBoxColumn priceCell = new DataGridViewTextBoxColumn();
            priceCell.HeaderText = "価格";
            priceCell.Width = 130;

            DataGridViewTextBoxColumn cateCell = new DataGridViewTextBoxColumn();
            cateCell.HeaderText = "カテゴリー";
            cateCell.Width = 130;

            DataGridViewButtonColumn editCell = new DataGridViewButtonColumn();
            editCell.HeaderText = "編集";
            editCell.Name = "edit";
            editCell.Text = "編集";
            editCell.UseColumnTextForButtonValue = true;
            editCell.Width = 130;

            DataGridViewButtonColumn deleteCell = new DataGridViewButtonColumn();
            deleteCell.HeaderText = "削除";
            deleteCell.Name = "delete";
            deleteCell.Text = "削除";
            deleteCell.UseColumnTextForButtonValue = true;
            deleteCell.Width = 130;

            dataGridView1.Columns.Add(imgCell);
            dataGridView1.Columns.Add(nameCell);
            dataGridView1.Columns.Add(priceCell);
            dataGridView1.Columns.Add(cateCell);
            dataGridView1.Columns.Add(editCell);
            dataGridView1.Columns.Add(deleteCell);

        }

        private void ItemControl_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();
            groupBox1.Visible = true;
            for(var i = progressBar1.Minimum; i<progressBar1.Maximum; i += 10)
            {
                progressBar1.Value = i;
            }
            WebRequest request = WebRequest.Create("http://192.168.1.7:3000/items");
            var stream = await request.GetResponseAsync();
            var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
            array = JArray.Parse(reader);
            using(WebClient wc = new WebClient())
            {
                foreach (var data in array)
                {
                    try
                    {
                        string imgUrl = "http://192.168.1.7:8888/upload/" + (string)data["name"] + ".jpg";
                        

                        var st = wc.OpenRead(imgUrl);
                        Bitmap bitmap = new Bitmap(st);
                        st.Close();


                        dataGridView1.Rows.Add(
                            bitmap,
                            data["name"],
                            data["price"],
                            data["category"]
                        );
                    }
                    catch (Exception) { }
                    groupBox1.Visible = false;
                }
            };
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewItem newItem = new NewItem();
            newItem.ShowDialog(this);
            newItem.Dispose();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button_action(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            JToken obj = array[e.RowIndex];

            if(dgv.Columns[e.ColumnIndex].Name == "edit")
            {
                ItemEdit itemEdit = new ItemEdit();
                itemEdit.pictureBox1.ImageLocation = "http://192.168.1.7:8888/upload/" + (string)obj["name"] + ".jpg";
                itemEdit.label1.Text = (string)obj["name"] + "編集";
                itemEdit.textBox2.Text = (string)obj["name"];
                itemEdit.textBox3.Text = (string)obj["price"];
                itemEdit.comboBox1.SelectedItem = (string)obj["category"];


                itemEdit.ShowDialog(this);
                itemEdit.Dispose();

            }
            else if(dgv.Columns[e.ColumnIndex].Name == "delete")
            {
                DialogResult result = MessageBox.Show("削除しますか？", "", MessageBoxButtons.YesNo);
                if(result == DialogResult.Yes)
                {
                    string deleteUrl = "http://192.168.1.7:8888/delete.php/";
                    string deleteItem = "http://192.168.1.7:3000/items/" + (string)obj["id"];

                    DataGridView dg = (DataGridView)sender;


                    using (WebClient webClient =new WebClient())
                    {
                        NameValueCollection collection = new NameValueCollection();
                        collection.Add("name", (string)obj["name"]);
                        webClient.UploadValuesAsync(new Uri(deleteUrl), collection);
                    };
                    using(WebClient wc = new WebClient())
                    {
                        NameValueCollection collection = new NameValueCollection();
                        collection.Add("id", (string)obj["id"]);
                        wc.QueryString = collection;
                        wc.UploadValuesAsync(new Uri(deleteItem),"DELETE", collection);
                        wc.UploadValuesCompleted += (s, o) =>
                        {
                            dg.Rows[e.RowIndex].Visible = false;
                            foreach(DataGridViewRow r in dgv.SelectedRows)
                            {
                                dataGridView1.Rows.Remove(r);
                            }

                            MessageBox.Show("削除しました。");
                        };
                    };
                }
            }
            else
            {
                MessageBox.Show("normal");
            }
        }
    }
}
