using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace windowsApp
{
    public partial class OrderProcess : Form
    {
        private string user_name = "";
        private string item_name = "";
        private string price_data = "";
        private string num_data = "";
        private string total_data = "";
        private string order_status = "";



        public OrderProcess()
        {
            InitializeComponent();
        }

        private void OrderProcess_Load(object sender, EventArgs e)
        {
            item.Text = item_name;
            user.Text = user_name;
            pirce.Text = price_data;
            num.Text = num_data;
            total.Text = total_data;
            status.Text = order_status;
            
        }
        public void SetParameter(JToken token)
        {
            this.user_name = (string)token["user_name"];
            this.item_name = (string)token["name"];
            this.price_data = (string)token["price"];
            this.num_data = (string)token["num"];
            this.total_data = (int.Parse(this.price_data) * int.Parse(this.num_data)).ToString();
            if( (string)token["status"] == "0")
            {
                this.order_status = "申請中";
                status.ForeColor = Color.Blue;
                button1.Visible = false;
                button2.Location = new Point(65, 288);
                button3.Location = new Point(65, 353);
              
            }
            else if ((string)token["status"] == "1")
            {
                this.order_status = "加工済み";
                status.ForeColor = Color.Orange;
                button2.Visible = false;
                button3.Location = new Point(65, 353);
            }
            else
            {
                this.order_status = "受け渡し済み";
                status.ForeColor = Color.Red;
                button3.Visible = false;
                button2.Location = new Point(65, 353);
            }

        }
    }
}
