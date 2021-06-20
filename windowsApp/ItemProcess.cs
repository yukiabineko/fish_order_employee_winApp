﻿using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;


namespace windowsApp
{
    public partial class Process : Form
    {
        private string id = "";
        private string strings;
        private JArray array;
       
        
        public Process()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            string posturl = "https://uematsu-backend.herokuapp.com/processings";

            using(WebClient webClient = new WebClient())
            {
                try
                {
                    NameValueCollection collection = new NameValueCollection();
                    NameValueCollection getCollection = new NameValueCollection();
                    getCollection.Add("id", this.id);

                    collection.Add("win_processes", strings);
                    webClient.QueryString = getCollection;
                    webClient.UploadValuesAsync(new Uri(posturl), collection);
                    webClient.UploadValuesCompleted += (s, o) =>
                    {
                        string result = System.Text.Encoding.UTF8.GetString(o.Result);
                        JToken token = JToken.Parse(result);
                        MessageBox.Show((string)token["message"]);
                        this.Close();
                    };
                }
                catch (Exception) { }
            }
        }

        private async void Process_Load(object sender, EventArgs e)
        {
            Console.WriteLine(this.id);
            DataGridViewTextBoxColumn processName = new DataGridViewTextBoxColumn();
            processName.Width = dataGridView1.Width / 2;
            dataGridView1.AllowUserToAddRows = false;

            DataGridViewButtonColumn deleteBtn = new DataGridViewButtonColumn();
            deleteBtn.Text = "削除";
            deleteBtn.Name = "削除";
            deleteBtn.Width = dataGridView1.Width / 2 - 50;
            deleteBtn.UseColumnTextForButtonValue = true;
            deleteBtn.FlatStyle = FlatStyle.Flat;
            deleteBtn.DefaultCellStyle.BackColor = Color.Red;

            dataGridView1.Columns.Add(processName);
            dataGridView1.Columns.Add(deleteBtn);

            string processUrl = "https://uematsu-backend.herokuapp.com/processings/" + id;
            try
            {
                WebRequest webRequest = WebRequest.Create(processUrl);
                var stream = await webRequest.GetResponseAsync();
                var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                array = JArray.Parse(reader);
                Console.WriteLine(array);
                foreach(var process in array)
                {
                    dataGridView1.Rows.Add(process["processing_name"]);
                }
            }
            catch (Exception)
            {

            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if(dgv.Columns[e.ColumnIndex].Name == "削除")
            {
                string delid = (string)array[e.RowIndex]["id"];
                Console.WriteLine(delid);
                MessageBox.Show("削除");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            ListBox listBox = (ListBox)sender;
            ListBox.SelectedObjectCollection items = listBox.SelectedItems;
            foreach(var item in items)
            {
                strings += item + ",";
            }
            Console.WriteLine(strings);

        }
        public void setId(string id)
        {
            this.id = id;
        }
    }
}
