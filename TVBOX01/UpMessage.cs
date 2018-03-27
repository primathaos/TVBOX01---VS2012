using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVBOX01
{
    public partial class UpMessage : Form
    {
        public UpMessage()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private string sip;//定义的私有变量
        public string SIP//为窗体Form2定义的属性
        {
            get //读
            {
                return sip;
            }
            set//写
            {
                sip = value;
            }
        }

        static string tt_conn;

        private void UpMessage_Load(object sender, EventArgs e)
        {
            tt_conn = @"server=" + sip + @";database=oracle;uid=sa;pwd=adminsa";
            string tt_sql = "select top 1 up_message from printsoft_up_messages order by id desc";
            DataSet ds = Dataset1.GetDataSet(tt_sql, tt_conn);
            string Messageshow_Text = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            this.Messageshow.Text = "打印软件有更新，原因是：\n" + Messageshow_Text + "，\n立刻更新打印软件吗？";
        }

        private void UPNOW_Click(object sender, EventArgs e)
        {
            Dataset1.Uptext.UptextData.Clear();
            Dataset1.Uptext.UptextData.Add("Key1", "YES");
            this.Close();
        }

        private void UPLATE_Click(object sender, EventArgs e)
        {
            Dataset1.Uptext.UptextData.Clear();
            Dataset1.Uptext.UptextData.Add("Key1", "NO");
            this.Close();
        }
    }
}
