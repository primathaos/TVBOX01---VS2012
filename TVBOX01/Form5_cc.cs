using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVBOX01
{
    public partial class Form5_cc : Form
    {
        public Form5_cc()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_conn;
        private int tt_interval = 100;

        private void Form5_cc_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";

            this.radioButton1.Checked = true;
            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label12.Text = tt_interval.ToString();
            ClearLabelone();

        }

        private string str;//定义的私有变量
        public string STR//为窗体Form2定义的属性
        {
            get //读
            {
                return str;
            }
            set//写
            {
                str = value;
            }
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

        #endregion



        #region 2、按钮功能
        //重置
        private void button3_Click(object sender, EventArgs e)
        {
            ClearLabelone();
            this.textBox1.Text = null;
            this.textBox2.Text = null;
        }


        //执行
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.button1.Text == "停止")
            {
                MessageBox.Show("请先停止自动执行");



            }
            else
            {
                selectShowMain();

            }
        }


        //开始
        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                button1.Text = "开始";

                timer1.Stop();
            }
            else
            {

                try
                {

                    button1.Text = "停止";
                    timer1.Start();
                }
                catch (Exception)
                {

                }
            }
        }

        #endregion


        #region 3、锁定周期选择及清除事件
        //锁定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                this.textBox1.Enabled = false;
                this.textBox2.Enabled = false;
                this.comboBox1.Enabled = false;
            }
            else
            {
                this.textBox1.Enabled = true;
                this.textBox2.Enabled = true;
                this.comboBox1.Enabled = true;
              
            }
        }

        //周期选择
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label12.Text = tt_interval.ToString();
        }



        //数据统计标签数据
        private void ClearLabelone()
        {
            this.label2.Text = null;
            this.label3.Text = null;
            this.label5.Text = null;
            this.label7.Text = null;
            this.label14.Text = null;
            this.dataGridView1.DataSource = null;
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.Columns.Clear();
        }



        #endregion


        #region 4、时间控件及主要方法

        //时间控件
        private void timer1_Tick(object sender, EventArgs e)
        {
            tt_interval--;
            this.label12.Text = tt_interval.ToString();
            if (tt_interval == 0)
            {
                selectShowMain();
                tt_interval = Convert.ToInt32(this.comboBox1.Text);
                this.label12.Text = tt_interval.ToString();
            }
        }



        private void selectShowMain()
        {

            ClearLabelone();
            string tt_date = DateTime.Now.ToString("yyyy-MM-dd");
            string tt_date1 = tt_date.Replace("-","");
            this.label14.Text = tt_date1;
            

            //---工序
            string tt_code = "";
            if (this.textBox1.Text != "")
            {
                tt_code = " and STANCE =  '" + this.textBox1.Text + "' ";
            }

            //---线别
            string tt_line = "";
            if (this.textBox2.Text != "")
            {
                tt_line = " and LINE =  '" + this.textBox2.Text + "' ";
            }


            string tt_sql = "";


            //汇总一
            string tt_sql1 = "select RECORD_ID 日期,STANCE 站位,sum(cast(PASSNUM as int)) 通过数,sum(cast(NGNUM as int)) NG数  " +
                            "from ODC_STATISTICS  " +
                            "where  RECORD_ID  = '" + tt_date1 + "'" + tt_code + tt_line + 
                            " group by RECORD_ID,STANCE " +
                            " order by STANCE";
            //汇总三 
            string tt_sql2 = "select RECORD_ID 日期,STANCE 站位,COMPUTER_NAME 电脑名称,sum(cast(PASSNUM as int)) 通过数,sum(cast(NGNUM as int)) NG数  " +
                            "from ODC_STATISTICS  " +
                             "where  RECORD_ID  = '" + tt_date1 + "'" + tt_code + tt_line + 
                            " group by RECORD_ID,STANCE,COMPUTER_NAME " +
                            " order by STANCE";

            if (this.radioButton1.Checked == true) tt_sql = tt_sql1;
            if (this.radioButton2.Checked == true) tt_sql = tt_sql2;


            DataSet ds = Dataset1.GetDataSet(tt_sql, tt_conn);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                dataGridView1.DataSource = ds;
                dataGridView1.DataMember = "Table";

                DataGridViewTextBoxColumn acCode = new DataGridViewTextBoxColumn();
                acCode.Name = "acCode";
                acCode.DataPropertyName = "acCode";
                acCode.HeaderText = "良率";
                dataGridView1.Columns.Add(acCode);

            }


            if (ds.Tables.Count == 1 && ds.Tables[0].Rows.Count == 0)
            {
                //MessageBox.Show("sorry,没有查询到数据");
            }
            else
            {

                decimal tt_pass = 0; //通过数
                decimal tt_ngnum = 0;  //NG数

                decimal tt_pass1 = 0; //通过数
                decimal tt_ngnum1 = 0;  //NG数

                decimal tt_sum = 0;   //总数
                decimal tt_radio = 0;  //良率

                decimal tt_coderadio = 0; //站位良率


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tt_pass1 = 0;
                    tt_ngnum1 = 0;
                    tt_coderadio = 0;

                    if (ds.Tables[0].Rows[i]["通过数"].ToString() != "")
                    {
                        tt_pass = tt_pass + decimal.Parse(ds.Tables[0].Rows[i]["通过数"].ToString());
                        tt_pass1 = decimal.Parse(ds.Tables[0].Rows[i]["通过数"].ToString());
                    }

                    if (ds.Tables[0].Rows[i]["NG数"].ToString() != "")
                    {
                        tt_ngnum = tt_ngnum + decimal.Parse(ds.Tables[0].Rows[i]["NG数"].ToString());
                        tt_ngnum1 = decimal.Parse(ds.Tables[0].Rows[i]["NG数"].ToString());
                    }




                    if (tt_pass1 != 0 || tt_ngnum1 != 0)
                    {
                        tt_coderadio = Math.Round(tt_pass1 / (tt_pass1 + tt_ngnum1) * 100, 2);
                        dataGridView1.Rows[i].Cells["acCode"].Value = tt_coderadio.ToString() + "%";
                    }


                }

                tt_sum = tt_pass + tt_ngnum;

                if (tt_sum > 0 && tt_pass > 0)
                {
                    tt_radio = Math.Round(tt_pass / tt_sum * 100, 2);

                }

                this.label2.Text = tt_sum.ToString();
                this.label5.Text = tt_pass.ToString();
                this.label7.Text = tt_ngnum.ToString();
                this.label3.Text = tt_radio.ToString() + "%";

            }



        }



        #endregion

        






    }
}
