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

            //当日、翌日用
            dataGridView2.AllowUserToAddRows = false;
            DataGridViewTextBoxColumn dayColumn2 = new DataGridViewTextBoxColumn();
            dayColumn2.HeaderText = "日付け";
            dayColumn2.Width = dataGridView2.Width / 6;


            DataGridViewTextBoxColumn nameColumn2 = new DataGridViewTextBoxColumn();
            nameColumn2.HeaderText = "商品名";
            nameColumn2.Width = dataGridView2.Width / 6;


            DataGridViewTextBoxColumn priceColumn2 = new DataGridViewTextBoxColumn();
            priceColumn2.HeaderText = "価格";
            priceColumn2.Width = dataGridView2.Width / 6;

            DataGridViewTextBoxColumn numColumn2 = new DataGridViewTextBoxColumn();
            numColumn2.HeaderText = "数量";
            numColumn2.Width = dataGridView2.Width / 6;

            DataGridViewTextBoxColumn processColumn2 = new DataGridViewTextBoxColumn();
            processColumn2.HeaderText = "依頼加工";
            processColumn2.Width = dataGridView2.Width / 6;

            DataGridViewTextBoxColumn confirm = new DataGridViewTextBoxColumn();
            confirm.HeaderText = "依頼状況";
            confirm.Width = dataGridView2.Width / 6;

            dataGridView2.Columns.Add(dayColumn2);
            dataGridView2.Columns.Add(nameColumn2);
            dataGridView2.Columns.Add(priceColumn2);
            dataGridView2.Columns.Add(numColumn2);
            dataGridView2.Columns.Add(processColumn2);
            dataGridView2.Columns.Add(confirm);

           
        }

        private void UserOrder_Load(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now;
            string td = today.ToString("yyyy/MM/dd");

            DateTime tomorrow = DateTime.Now;
            tomorrow = tomorrow.AddDays(1);
            string tw = tomorrow.ToString("yyyy/MM/dd");

            if (todayArr != null)
            {
                foreach (var arr in todayArr)
                {
                    if((string)arr["shopping_date"] == td)
                    {
                        dataGridView2.Rows.Add(
                          arr["shopping_date"],
                          arr["name"],
                          arr["price"],
                          arr["num"],
                          arr["process"],
                          (string)arr["status"] == "0" ? "申請中" : (string)arr["status"] == "1" ? "加工済み" : "受け渡し済み"
                       );
                    }
                    else
                    {
                        dataGridView2.Rows.Add(
                          arr["shopping_date"],
                          arr["name"],
                          arr["price"],
                          arr["num"],
                          arr["process"],
                          ""
                       );
                    }
                }
               
            }
            if (historyArray != null)
            {
                foreach (var arr in historyArray)
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
            //分岐で変更
          for(var i = 0; i<todayArr.Count; i++)
            {
                if(dataGridView2.Rows[i].Cells[0].Value.ToString() == tw)
                {
                    dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.Beige;
                }
                else if(dataGridView2.Rows[i].Cells[5].Value.ToString() == "申請中")
                {
                    dataGridView2.Rows[i].Cells[5].Style.ForeColor = Color.White;
                    dataGridView2.Rows[i].Cells[5].Style.BackColor = Color.Blue;
                    dataGridView2.Rows[i].Cells[5].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                    dataGridView2.Rows[i].Cells[5].Style.ForeColor = Color.White;
                    dataGridView2.Rows[i].Cells[5].Style.BackColor = Color.Red;
                    dataGridView2.Rows[i].Cells[5].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
