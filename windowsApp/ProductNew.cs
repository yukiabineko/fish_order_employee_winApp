using System;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Collections.Specialized;

namespace windowsApp
{
    public partial class ProductNew : Form
    {
        private JArray items;
        private string[] str;
        private string email = "";
        private string pass = "";

        public ProductNew()
        {
            InitializeComponent();
            

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        public void setArray(JArray array)
        {
            this.items = array;
        }
        public JArray getArray()
        {
            return items;
        }

        private void ProductNew_Load(object sender, EventArgs e)
        {
            Console.WriteLine(email);
            Console.WriteLine(pass);
           
            str = new string[items.Count];
            for(var i = 0; i<items.Count; i++)
            {
                str[i] = (string)items[i]["name"];
            }
            Console.WriteLine(str);
            comboBox1.Items.AddRange(str);
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //コンボボックスのアイテムをオートコンプリートの選択候補とする
            comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
            numericUpDown1.Minimum = 0;
            numericUpDown1.Value = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!str.Contains(comboBox1.SelectedItem))
            {
                MessageBox.Show("リストにない商品です。");
            }
            else if(textBox1.Text == "")
            {
                MessageBox.Show("金額が入力されてません。");
            }
            else if(numericUpDown1.Value == 0)
            {
                MessageBox.Show("在庫の設定が不正です。");
            }
            else
            {
                MessageBox.Show("ok。");
                string url = "https://uematsu-backend.herokuapp.com/orders";
                using(WebClient webClient = new WebClient())
                {
                    NameValueCollection collection = new NameValueCollection();
                    collection.Add("name", comboBox1.SelectedItem.ToString());
                    collection.Add("price", textBox1.Text);
                    collection.Add("stock", numericUpDown1.Value.ToString());
                    Console.WriteLine(numericUpDown1.Value.ToString());
                    webClient.UploadValuesAsync(new Uri(url), collection);
                    webClient.UploadValuesCompleted += (o, s) =>
                    {
                        MessageBox.Show("登録しました。");
                    };
                };
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < '0' || '9' < e.KeyChar)
            {
                //押されたキーが 0～9でない場合は、イベントをキャンセルする
                button2.Visible = true;
                e.Handled = true;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                button2.Visible = false;
            }
            else
            {
                button2.Visible = true;
            }
        }
        public void setEmail(string email)
        {
            this.email = email;
        }
        public void setPass(string pass)
        {
            this.pass = pass;
        }
    }
}
