using System;
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

        private  void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
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
                    webClient.UploadValuesCompleted += async (s, o) =>
                    {
                        string result = System.Text.Encoding.UTF8.GetString(o.Result);
                        JToken token = JToken.Parse(result);
                        DialogResult box = MessageBox.Show((string)token["message"],"",MessageBoxButtons.OK);
                        if(box == DialogResult.OK)
                        {
                            try
                            {
                                string processUrl = "https://uematsu-backend.herokuapp.com/processings/" + this.id;
                                dataGridView1.Rows.Clear();
                                array.Clear();
                                WebRequest request = WebRequest.Create(processUrl);
                                var stream = await request.GetResponseAsync();
                                var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                                array = JArray.Parse(reader);
                                foreach (var arr in array)
                                {
                                    dataGridView1.Rows.Add(arr["processing_name"]);
                                }
                                button1.Enabled = true;
                            }
                            catch (Exception) {
                                button1.Enabled = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("danger");
                        }
                    };
                }
                catch (Exception) {
                    button1.Enabled = true;
                }
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

            dataGridView1.Visible = false;

            string processUrl = "https://uematsu-backend.herokuapp.com/processings/" + this.id;
            try
            {
                WebRequest webRequest = WebRequest.Create(processUrl);
                var stream = await webRequest.GetResponseAsync();
                var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                array = JArray.Parse(reader);
                if(array.Count > 0)
                {
                    dataGridView1.Visible = true;
                    panel1.Visible = false;
                }
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

        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if(dgv.Columns[e.ColumnIndex].Name == "削除")
            {
                try
                {
                    string delid = (string)array[e.RowIndex]["id"];
                    Console.WriteLine(delid);
                    string delUrl = "https://uematsu-backend.herokuapp.com/processings/" + delid;
                    WebRequest webRequest = WebRequest.Create(delUrl);
                    webRequest.Method = "DELETE";
                    var stream = await webRequest.GetResponseAsync();
                    var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                    JObject obj = JObject.Parse(reader);
                    stream.Close();
                    MessageBox.Show((string)obj["message"]);
                    dgv.Rows[e.RowIndex].Visible = false;
                    foreach (DataGridViewRow r in dgv.SelectedRows)
                    {
                        dataGridView1.Rows.Remove(r);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("失敗しました。");
                }
               
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
        /*既に登録済みのアイテムあるか確認*/
        public bool CheckIncludeItem(string item)
        {
            bool res = true;
            foreach (var arr in array)
            {
                if(item == (string)arr["processing_name"])
                {
                    res = false;
                }
            }
            return res;
        }
    }
}
