using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace windowsApp
{
    public partial class Product : UserControl
    {
        public JArray items;
        public JArray products;
        public Menu main;
        private string email = "";
        private string pass = "";
        public ProdoctEdit prodoctEdit;

        public Product()
        {
            InitializeComponent();
            groupBox1.Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            progressBar1.Minimum = 10;
            progressBar1.Maximum = 100;
            progressBar1.Value = progressBar1.Minimum;

            button1.Visible = false;


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
            edit.Text = "編集";
            edit.Name = "edit";
            edit.FlatStyle = FlatStyle.Flat;
            edit.UseColumnTextForButtonValue = true;
            edit.Width = 140;


            DataGridViewButtonColumn delete = new DataGridViewButtonColumn();
            delete.Text = "削除";
            delete.Name = "delete";
            delete.FlatStyle = FlatStyle.Flat;
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

            dataGridView1.Visible = false;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            groupBox1.Visible = true;
            for(var i = progressBar1.Minimum; i<progressBar1.Maximum; i++)
            {
                progressBar1.Value = i;
            }
            string url = "https://uematsu-backend.herokuapp.com/orders";
            using(WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadStringAsync(new Uri(url));
                    webClient.DownloadStringCompleted += (s, o) =>
                    {
                        string data = o.Result;
                        products = JArray.Parse(data);
                        if(products.Count > 0)
                        {
                            dataGridView1.Visible = true;
                            panel1.Visible = false;
                        }
                        Console.WriteLine(data);
                        foreach (var arr in products)
                        {
                            dataGridView1.Rows.Add(
                               arr["name"],
                               arr["price"],
                               arr["stock"],
                               (int)arr["price"] * (int)arr["stock"]

                           );
                        };
                        groupBox1.Visible = false;
                    };
                }
                catch (Exception) {
                    MessageBox.Show("データ取得に失敗しました。ネットワークをご確認ください。");
                }
                
               
           
            };
        }

        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if(dgv.Columns[e.ColumnIndex].Name == "edit")
            {
                Console.WriteLine(items);
                JToken item = products[e.RowIndex];
                prodoctEdit = new ProdoctEdit();
                prodoctEdit.main = this;
                prodoctEdit.setParameter(
                    (string)item["id"],
                    (string)item["name"], 
                    (string)item["stock"],
                    (string)item["price"]
                );
                prodoctEdit.ShowDialog(this);
            }
            /*削除処理*/
            else {
                DataGridView dg = (DataGridView)sender;
                JToken jToken = items[e.RowIndex];
                string url = "https://uematsu-backend.herokuapp.com/orders/" + (string)jToken["id"];
                DialogResult result = MessageBox.Show("削除しますか？", "", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        WebRequest delrequest = WebRequest.Create(url);
                        delrequest.Method = "DELETE";
                        var stream = await delrequest.GetResponseAsync();
                        var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                        JObject ob = JObject.Parse(reader);
                        stream.Close();
                        MessageBox.Show((string)ob["message"]);
                        dg.Rows[e.RowIndex].Visible = false;
                        foreach (DataGridViewRow r in dgv.SelectedRows)
                        {
                            dataGridView1.Rows.Remove(r);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("削除失敗");
                    }
                }

            }
        }

        private async void Product_Load(object sender, EventArgs e)
        {
            this.setEmail(main.getMail());
            this.setPass(main.getPass());

            try
            {
                WebRequest request = WebRequest.Create("https://uematsu-backend.herokuapp.com/items");
                var stream = await request.GetResponseAsync();
                var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                items = JArray.Parse(reader);
                button1.Visible = true;
            }
            catch (Exception) { }

            /*using(WebClient webClient = new WebClient())
            {
                string itemUrl = "https://uematsu-backend.herokuapp.com/items";
                webClient.DownloadDataAsync(new Uri(itemUrl));
                webClient.DownloadDataCompleted += (s, o) =>
                {
                    string data = System.Text.Encoding.UTF8.GetString(o.Result);
                    items = JArray.Parse(data);
                    button1.Visible = true;
                };


            };
            */
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine(items);
            ProductNew productNew = new ProductNew();
            productNew.main = this;
            productNew.setArray(items);
            productNew.setEmail(email);
            productNew.setPass(pass);
            productNew.ShowDialog(this);
            productNew.Dispose();
        }
        public void setEmail(string email)
        {
            this.email = email;

        }
        public void setPass(string pass)
        {
            this.pass = pass;

        }
        public string getEmail()
        {
            return this.email;
        }
        public string getPass()
        {
            return this.pass;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string url = "https://uematsu-backend.herokuapp.com/orders/deleteAll";
            DialogResult result = MessageBox.Show("全ての店頭商品を削除します。よろしいですか？", "", MessageBoxButtons.YesNo);
            if(result == DialogResult.Yes)
            {
                try
                {
                    WebRequest delrequest = WebRequest.Create(url);
                    var stream = await delrequest.GetResponseAsync();
                    var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                    JObject ob = JObject.Parse(reader);
                    stream.Close();
                    MessageBox.Show((string)ob["message"]);
                    dataGridView1.Rows.Clear();
                    
                }
                catch (Exception)
                {
                    MessageBox.Show("削除失敗");
                }
            }
        }

    }
}
