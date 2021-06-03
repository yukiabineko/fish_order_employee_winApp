using System;
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
    public partial class UserOrder : Form
    {
        private string id = "";
        private string mail = "";
        private string pass = "";
        private JArray array;
        private JArray todayArr;
        private JArray historyArray;
       

        public UserOrder()
        {
            InitializeComponent();
            dataGridView1.AllowUserToAddRows = false;
            DataGridViewTextBoxColumn dayColumn = new DataGridViewTextBoxColumn();
            dayColumn.HeaderText = "日付け";
            dayColumn.Width = dataGridView1.Width/4;


            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.HeaderText = "商品名";
            nameColumn.Width = dataGridView1.Width/5;


            DataGridViewTextBoxColumn priceColumn = new DataGridViewTextBoxColumn();
            priceColumn.HeaderText = "価格";
            priceColumn.Width = dataGridView1.Width / 5;

            DataGridViewTextBoxColumn numColumn = new DataGridViewTextBoxColumn();
            numColumn.HeaderText = "数量";
            numColumn.Width = dataGridView1.Width/5;

            DataGridViewTextBoxColumn processColumn = new DataGridViewTextBoxColumn();
            processColumn.HeaderText = "依頼加工";
            processColumn.Width = 160;

            dataGridView1.Columns.Add(dayColumn);
            dataGridView1.Columns.Add(nameColumn);
            dataGridView1.Columns.Add(priceColumn);
            dataGridView1.Columns.Add(numColumn);
            dataGridView1.Columns.Add(processColumn);

            

        }

        private void UserOrder_Load(object sender, EventArgs e)
        {
            if (array != null)
            {
                foreach (var arr in array[0])
                {
                    dataGridView1.Rows.Add(
                       arr["shopping_date"],
                       arr["name"],
                       arr["price"],
                       arr["num"],
                       arr["process"]
                    );
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            string orderUrl = "https://uematsu-backend.herokuapp.com/users/show";
            using(WebClient webClient = new WebClient())
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("id", this.getId());
                collection.Add("email", this.getMail());
                collection.Add("password", this.getPass());
                webClient.UploadValuesAsync(new Uri(orderUrl), collection);
                webClient.UploadValuesCompleted += (s, o) =>
                {
                    string data = System.Text.Encoding.UTF8.GetString(o.Result);
                    JObject obj = JObject.Parse(data);
                    
                    array = (JArray)obj["orders"];
                    Console.WriteLine(array[0]);
                    foreach(var arr in array[0])
                    {
                        dataGridView1.Rows.Add(
                           arr["shopping_date"],
                           arr["name"],
                           arr["price"],
                           arr["num"],
                           arr["process"]
                        );
                    }

                };
            };
        }
        public void setId(string str)
        {
            this.id = str;
        }
        public string getId()
        {
            return this.id;
        }
        public void setMail(string str)
        {
            this.mail = str;
        }
        public string getMail()
        {
            return this.mail;
        }
        public void setPass(string str)
        {
            this.pass = str;
        }
        public string getPass()
        {
            return this.pass;
        }
        public void setJArray(JArray arr)
        {
            this.array = arr;
        }
        public JArray GetArray()
        {
            return this.array;
        }
        public void setTodayArray(JArray arr)
        {
            this.todayArr = arr;
        }
        public JArray GetTodayArray()
        {
            return this.todayArr;
        }
        public void setHistoryArray(JArray arr)
        {
            this.historyArray = arr;
        }
        public JArray GetHistoryArray()
        {
            return this.historyArray;
        }
    }
}
