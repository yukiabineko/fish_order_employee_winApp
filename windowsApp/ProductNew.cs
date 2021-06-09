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
    public partial class ProductNew : Form
    {
        private JArray items;
        public Product main;

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
           
            string[] str = new string[items.Count];
            for(var i = 0; i<items.Count; i++)
            {
                str[i] = (string)items[i]["name"];
            }
            Console.WriteLine(str);
            comboBox1.Items.AddRange(str);
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //コンボボックスのアイテムをオートコンプリートの選択候補とする
            comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
        }
    }
}
