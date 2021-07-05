using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;

namespace windowsApp
{
    public partial class ProdoctEdit : Form
    {
        private string id = "";
        private string name = "";
        private string stock = "";
        private string price = "";
        public Product main;


        public ProdoctEdit()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://uematsu-backend.herokuapp.com/orders/" + this.id;
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("name", this.name);
                collection.Add("price", textBox1.Text);
                collection.Add("stock", numericUpDown1.Value.ToString());
                webClient.UploadValuesAsync(new Uri(url), "PATCH", collection);
                webClient.UploadValuesCompleted += (s, o) =>
                {
                    try
                    {
                        string responseStr = System.Text.Encoding.UTF8.GetString(o.Result);
                        JToken token = JToken.Parse(responseStr);
                        MessageBox.Show((string)token["message"]);
                        try
                        {
                            /*一覧の再取得*/
                            main.groupBox1.Visible = true;
                            for (var i = main.progressBar1.Minimum; i < main.progressBar1.Maximum; i++)
                            {
                                main.progressBar1.Value = i;
                            }
                            string url = "https://uematsu-backend.herokuapp.com/orders";
                            using (WebClient webClient = new WebClient())
                            {
                                try
                                {
                                    webClient.DownloadStringAsync(new Uri(url));
                                    webClient.DownloadStringCompleted += (s, o) =>
                                    {
                                        string data = o.Result;
                                        if (data != null)
                                        {
                                            main.items.Clear();
                                            main.dataGridView1.Rows.Clear();
                                        }
                                        main.items = JArray.Parse(data);
                                        Console.WriteLine(data);
                                        foreach (var arr in main.items)
                                        {
                                            main.dataGridView1.Rows.Add(
                                               arr["name"],
                                               arr["price"],
                                               arr["stock"],
                                               (int)arr["price"] * (int)arr["stock"]

                                           );
                                        };
                                        main.groupBox1.Visible = false;
                                    };
                                }
                                catch (Exception) { }
                            };
                        }
                        catch (Exception) { }

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("更新失敗");
                    }
                    this.Close();
                };

            };
            
        }

        private void ProdoctEdit_Load(object sender, EventArgs e)
        {
            label1.Text = this.name;
            numericUpDown1.Value = Decimal.Parse(this.stock);
            textBox1.Text = this.price;

            
        }
        public void setParameter(string id, string name, string stock, string price)
        {
            this.id = id;
            this.name = name;
            this.price = price;
            this.stock = stock;
        }

    }
}
