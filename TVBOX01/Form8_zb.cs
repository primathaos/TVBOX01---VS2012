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
    public partial class Form8_zb : Form
    {
        public Form8_zb()
        {
            InitializeComponent();
        }



        #region 1、属性设置
        string tt_package = "";
        static string tt_conn;
        private void Form8_zb_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=Adminsa@123";


            this.label4.Text = null;
            this.label5.Text = null;

            this.label12.Text = null;
            this.label13.Text = null;
            this.label14.Text = null;

            this.label18.Text = null;
            this.label19.Text = null;
            this.label20.Text = null;

            this.textBox2.Text = null;
            this.textBox7.Text = null;

            this.textBox2.Visible = false;
            this.textBox7.Visible = false;


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



        #region 2、锁定事件
        //工单锁定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                string tt_sql1 = "select  tasksquantity,product_name " +
                                "from odc_tasks where taskscode = '" + this.textBox1.Text + "' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);

                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label4.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label5.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();

                    this.textBox1.Enabled = false;

                    this.textBox2.Visible = true;
                    this.textBox7.Visible = true;


                    ClearLabelInfo();

                }
                else
                {
                    MessageBox.Show("没有查询此工单，请确认！");

                }
            }
            else
            {
                this.textBox1.Enabled = true;
                this.label4.Text = null;
                this.label5.Text = null;
                this.textBox2.Visible = false;
                this.textBox7.Visible = false;
                ClearLabelInfo();
            }
        }


        //中箱号码锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.textBox3.Enabled = false;
                this.textBox4.Enabled = false;

            }
            else
            {
                this.textBox3.Enabled = true;
                this.textBox4.Enabled = true;
            }
        }

        //彩盒号码锁定
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked)
            {
                this.textBox5.Enabled = false;
                this.textBox6.Enabled = false;

            }
            else
            {
                this.textBox5.Enabled = true;
                this.textBox6.Enabled = true;
            }

        }
        #endregion



        #region 3、辅助方法

        //位数判断方法
        private Boolean CheckStrLengh(string tt_checkstr, string tt_lengthtext)
        {
            Boolean tt_flag = false;
            if (tt_lengthtext.Length > 0)
            {
                int tt_snlength = int.Parse(tt_lengthtext);
                if (tt_checkstr.Length == tt_snlength)
                {
                    tt_flag = true;

                }

            }
            else
            {
                tt_flag = true;
            }
            return tt_flag;
        }



        //包含字符判断
        private Boolean CheckStrContain(string tt_scansn, string tt_containstr)
        {
            Boolean tt_flag = false;
            if (tt_containstr.Length > 0)
            {
                if (tt_scansn.Contains(tt_containstr))
                {
                    tt_flag = true;

                }
                else
                {

                }

            }
            else
            {
                tt_flag = true;

            }

            return tt_flag;
        }
        
        //工单清除lable
        private void ClearLabelInfo()
        {
            this.label12.Text = null;
            this.label13.Text = null;
            this.label14.Text = null;

        }


        //重置清除画面
        private void ClearLabelInfo2()
        {
            this.label12.Text = null;
            this.label13.Text = null;
            this.label14.Text = null;

            this.label18.Text = null;
            this.label19.Text = null;
            this.label20.Text = null;

            this.textBox2.Text = null;
            this.textBox7.Text = null;

            this.richTextBox1.Text = null;


        }


        //重置按钮
        private void button1_Click(object sender, EventArgs e)
        {
            ClearLabelInfo2();

            this.textBox2.Enabled = true;
            this.textBox2.Focus();
            this.textBox2.SelectAll();
           
        }



        //获取未比对信息
        private string getCheckNumber(string tt_task, string tt_package)
        {
            string tt_checknum = "0";

            string tt_sql = "select  count(1),0,0 from odc_package " +
                            "where taskcode = '" + tt_task + "' and pagesn = '" + tt_package + "' and pageperson = '中箱已比对' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            tt_checknum = tt_array[0];
            return tt_checknum;

        }


        //获取未比对信息列表
        private void getCheckNumber2(string tt_task, string tt_package)
        {

            this.richTextBox2.Text = null;
            string tt_sql = "select  T2.boxlable "+
                            "from odc_package T1 "+
                            "left outer join odc_alllable T2 "+
                            "on T1.pasn = T2.pcbasn "+
                           "where T1.taskcode = '" + tt_task + "' and T1.pagesn = '" + tt_package + "' and T1.pageperson <> '中箱已比对' ";

             DataSet ds3 = Dataset1.GetDataSet(tt_sql, tt_conn);
             if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
             {
                 //遍历一个表多行一列
                 foreach (DataRow row in ds3.Tables[0].Rows)
                 {
                     this.richTextBox2.Text = row[0].ToString() + "\n" + this.richTextBox2.Text;

                 }



             }

            
          

        }




        #endregion



        #region 4、扫描事件
        //中箱箱号扫描
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                ClearLabelInfo();

                tt_package = this.textBox2.Text.ToUpper().Trim();

                //第一步位数效验
                Boolean tt_flag1 = false;
                if (CheckStrLengh(tt_package, this.textBox3.Text))
                {
                    tt_flag1 = true;
                }
                else
                {
                    this.label12.Text = "位数不对";
                    
                }

                //第二步 包含效验
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    if (CheckStrContain(tt_package, this.textBox4.Text))
                    {
                        tt_flag2 = true;
                    }
                    else
                    {
                        this.label12.Text = "包含符不对";
                        
                    }
                }



                //第三步工单检验
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    string tt_sql3 = "select  count(1),min(taskcode),0  from odc_package " +
                                     "where  taskcode = '" + this.textBox1.Text + "' and  PAGESN = '" + tt_package + "' ";

                    string[] tt_array3 = new string[3];
                    tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                    if (tt_array3[0] == "0")
                    {

                        this.label12.Text = "该工单没有找到该箱号记录，请确认工单号是否正确或箱号是否正确";
                        
                    }
                    else
                    {
                        if (tt_array3[1] == this.textBox1.Text)
                        {
                            tt_flag3 = true;
                            this.label18.Text = tt_package;
                            this.label19.Text = tt_array3[0];
                        }
                        else
                        {
                            this.label12.Text = "工单不对," + tt_array3[1];
                           
                        }

                    }

                }


                 //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {

                    //获取已扫描信息
                    this.label20.Text = getCheckNumber(this.textBox1.Text,this.textBox2.Text);

                    //获取未扫描信息
                    getCheckNumber2(this.textBox1.Text, this.textBox2.Text);

                    this.textBox2.Enabled = false;

                    this.textBox7.Focus();
                    this.textBox7.SelectAll();
                }
                else
                {
                    this.textBox2.Focus();
                    this.textBox2.SelectAll();
                }

                this.label14.Text = null;

            }
        }


        //彩盒条码扫描
        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.label13.Text = null;
                this.label14.Text = null;

                string tt_boxlable = this.textBox7.Text.ToUpper().Trim();
                string tt_pcbasn = "";

                //第一步位数效验
                Boolean tt_flag1 = false;
                if (CheckStrLengh(tt_boxlable, this.textBox6.Text))
                {
                    tt_flag1 = true;
                }
                else
                {
                    this.label13.Text = "位数不对";

                }

                //第二步 包含效验
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    if (CheckStrContain(tt_boxlable, this.textBox5.Text))
                    {
                        tt_flag2 = true;
                    }
                    else
                    {
                        this.label13.Text = "包含符不对";

                    }
                }


                //第三步 数据判断
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    string tt_sql4 = "select count(1),min(T2.pcbasn),min(pageperson) " +
                                    "from odc_package T1 " +
                                    "left outer join odc_alllable T2 " +
                                    "on T1.pasn = T2.pcbasn " +

                                    "where T1.taskcode = '"+this.textBox1.Text+"' and   T1.PAGESN = '"+tt_package+"' " +
                                    "and T2.boxlable = '"+tt_boxlable+"' ";

                    string[] tt_array4 = new string[3];
                    tt_array4 = Dataset1.GetDatasetArray(tt_sql4, tt_conn);
                    if (tt_array4[0] == "1")
                    {

                        if (tt_array4[2] == "中箱已比对")
                        {
                            this.label13.Text = "亲，该条码已比对，不用再比对了";
                        }
                        else
                        {

                            tt_pcbasn = tt_array4[1];
                            tt_flag3 = true;
                        }

                    }
                    else
                    {
                          this.label13.Text = "该箱号，没有找到对应的彩盒号" ;

                        

                    }

                }

                
                //第四步 记录数据
                Boolean tt_flag4 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 )
                {
                    string tt_update = "update odc_package set pageperson = '中箱已比对' " +
                    "where taskcode = '" + this.textBox1.Text + "' and pagesn = '" + this.textBox2.Text + "' and pasn = '" + tt_pcbasn + "' ";

                    int tt_int = Dataset1.ExecCommand(tt_update, tt_conn);
                    if (tt_int > 0)
                    {
                        tt_flag4 = true;

                    }
                    else
                    {
                        this.label13.Text = "数据没有更新成功请重试";
                    }
                }



                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {


                    this.label14.Text = "PASS";
                    this.label14.ForeColor = Color.Yellow;
                    this.label14.BackColor = Color.Chartreuse;

                    this.label20.Text = getCheckNumber(this.textBox1.Text, this.textBox2.Text);
                    this.richTextBox1.Text =   this.textBox7.Text + "\n" + this.richTextBox1.Text ;

                    //获取未扫描信息
                    getCheckNumber2(this.textBox1.Text, this.textBox2.Text);

                    if (this.label19.Text == this.label20.Text)
                    {
                        this.textBox2.Enabled = true;
                        this.textBox2.Focus();
                        this.textBox2.SelectAll();
                    }
                    else
                    {
                        this.textBox7.Focus();
                        this.textBox7.SelectAll();
                    }

                }
                else
                {
                    this.label14.Text = "FAIL";
                    this.label14.ForeColor = Color.Black;
                    this.label14.BackColor = Color.Red;

                    this.textBox7.Focus();
                    this.textBox7.SelectAll();
                }

            }
        }

        #endregion



       







    }
}
