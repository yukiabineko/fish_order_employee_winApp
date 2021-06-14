using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections.Specialized;


namespace windowsApp
{
    public partial class NewItem : Form
    {
        string phpUrl = "http://yukiabineko.sakura.ne.jp/windowsFomUpdate.php";
        string railsUrl = "https://uematsu-backend.herokuapp.com/items";

        public NewItem()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using(WebClient webClient = new WebClient())
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("name", textBox2.Text);
                webClient.QueryString = collection;
                webClient.UploadFileAsync(new Uri(phpUrl), textBox1.Text);

            };
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("name", textBox2.Text);
                collection.Add("price", textBox3.Text);
                collection.Add("category", comboBox1.SelectedItem.ToString());
                webClient.UploadValuesAsync(new Uri(railsUrl), collection);
                webClient.UploadValuesCompleted += (s, o) =>
                {
                    MessageBox.Show("登録しました。");
                };
            };

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
            textBox1.Text = openFileDialog1.FileName;
        }

        private void NewItem_Load(object sender, EventArgs e)
        {

        }
    }
}
