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
    public partial class Form6_bd : Form
    {
        public Form6_bd()
        {
            InitializeComponent();
        }


        #region 1、属性设置

        static string tt_conn;

        string tt_hostlable = "";
        string tt_shortmac = "";
        string tt_boxlable = "";
        string tt_shellable = "";
        string tt_id = "";
        int tt_yield = 0;


        private void Form6_bd_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=Adminsa@123";

            this.label4.Text = null;
            this.label5.Text = null;

            this.label11.Text = tt_yield.ToString();

            this.textBox2.Enabled = false;
            this.textBox7.Enabled = false;
            this.textBox17.Enabled = false;
            this.textBox10.Enabled = false;
            this.textBox13.Enabled = false;


            this.label10.Text = null;
            this.label12.Text = null;
            this.label13.Text = null;
            this.label18.Text = null;



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



        #region 2、清除方法

        //工单更新初始化
        private void ClearLabelInfo()
        {
            //扫描框
            this.textBox2.Text = null;
            this.textBox7.Text = null;
            this.textBox17.Text = null;
            this.textBox10.Text = null;
            this.textBox13.Text = null;

            //信息框
            this.textBox14.Text = null;
            this.textBox18.Text = null;
            this.textBox21.Text = null;
            this.textBox19.Text = null;
            this.textBox20.Text = null;

            this.textBox14.BackColor = Color.White;
            this.textBox18.BackColor = Color.White;
            this.textBox21.BackColor = Color.White;
            this.textBox19.BackColor = Color.White;
            this.textBox20.BackColor = Color.White;

            //工单信息
            this.label4.Text = null;
            this.label5.Text = null;


        }

         //主机条码扫描前数据清理
         private void ClearLabelInfo1()
        {
             //扫描框
            //this.textBox2.Text = null;
            //this.textBox7.Text = null;
            //this.textBox17.Text = null;
            //this.textBox10.Text = null;
            //this.textBox13.Text = null;

            //信息框
            this.textBox14.Text = null;
            this.textBox18.Text = null;
            this.textBox21.Text = null;
            this.textBox19.Text = null;
            this.textBox20.Text = null;

            this.textBox14.BackColor = Color.White;
            this.textBox18.BackColor = Color.White;
            this.textBox21.BackColor = Color.White;
            this.textBox19.BackColor = Color.White;
            this.textBox20.BackColor = Color.White;

             tt_hostlable = "";
             tt_shortmac ="";
             tt_boxlable = "";
             tt_id = "";

             label18.Text = null;
        }


        #endregion



        #region 3、锁定事件

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
                    GetProductInfo();
                    this.textBox2.Enabled = true;
                    this.textBox7.Enabled = true;
                    this.textBox17.Enabled = true;
                    this.textBox10.Enabled = true;
                    this.textBox13.Enabled = true;
                    

                }
                else
                {
                    MessageBox.Show("没有查询此工单，请确认！");


                }

            }
            else
            {
                this.label4.Text = null;
                this.label5.Text = null;
                this.textBox1.Enabled = true;

                this.textBox2.Enabled = false;
                this.textBox7.Enabled = false;
                this.textBox17.Enabled = false;
                this.textBox10.Enabled = false;
                this.textBox13.Enabled = false;

                ClearLabelInfo();
            }
        }

        //铭牌主机条码锁定
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

        


        //小串码是否扫描
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox7.Checked)
            {
                this.textBox17.Enabled = false;
            }
            else
            {
                this.textBox17.Enabled = true;
            }
        }



        //主机MAC是否扫描
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked)
            {
                this.textBox7.Enabled = false;
            }
            else
            {
                this.textBox7.Enabled = true;
            }
        }



        #endregion



        #region 4、条码扫描
        //铭牌主机条码扫描
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ClearLabelInfo1();


                tt_hostlable = this.textBox2.Text.ToUpper().Trim();

                //第一步位数效验
                Boolean tt_flag1 = false;
                if (CheckStrLengh(tt_hostlable, this.textBox3.Text))
                {
                    tt_flag1 = true;
                }
                else
                {
                    this.textBox14.Text = "位数不对";
                    this.textBox14.BackColor = Color.Red;
                }
                
                //第二步 包含效验
                Boolean tt_flag2 = false;
                if (tt_flag1 )
                {
                    if (CheckStrContain(tt_hostlable, this.textBox4.Text))
                    {
                        tt_flag2 = true;
                    }
                    else
                    {
                        this.textBox14.Text = "包含符不对";
                        this.textBox14.BackColor = Color.Red;
                    }
                }


                //第三步工单检验
                Boolean tt_flag3 = false;
                if ( tt_flag1 && tt_flag2 )
                {
                    string tt_sql3 = "select count(1),min(taskscode),0  from odc_alllable " +
                                     "where hostlable = '"+tt_hostlable+"' ";

                    string[] tt_array3 = new string[3];
                    tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                    if (tt_array3[0] == "1")
                    {
                        if (tt_array3[1] == this.textBox1.Text)
                        {
                            tt_flag3 = true;
                        }
                        else
                        {
                            this.textBox14.Text = "工单不对，" + tt_array3[1];
                            this.textBox14.BackColor = Color.Red;
                        }
                    }
                    else
                    {
                        this.textBox14.Text = "没找到该记录";
                        this.textBox14.BackColor = Color.Red;
                    }

                }



                //第四步查找信息
                Boolean tt_flag4 = false;
                if(tt_flag1 && tt_flag2 && tt_flag3)
                {
                    string tt_sql4 = "select bprintuser,boxlable,dystlable,id from odc_alllable " +
                                     "where taskscode =  '" + this.textBox1.Text + "' and hostlable = '"+tt_hostlable+"'";

                    DataSet ds4 = Dataset1.GetDataSet(tt_sql4, tt_conn);
                    if (ds4.Tables.Count > 0 && ds4.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        tt_shortmac = ds4.Tables[0].Rows[0].ItemArray[0].ToString().Substring(0,17);
                        tt_boxlable = ds4.Tables[0].Rows[0].ItemArray[1].ToString();
                        tt_shellable = ds4.Tables[0].Rows[0].ItemArray[2].ToString();
                        tt_id = ds4.Tables[0].Rows[0].ItemArray[3].ToString();
                    }
                    else
                    {
                        this.textBox14.Text = "该工单下，没找到该记录";
                        this.textBox14.BackColor = Color.Red;
                    }

                }






                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    this.textBox14.Text = "PASS";
                    this.textBox14.BackColor = Color.Chartreuse;


                    if (this.checkBox3.Checked)
                    {
                        if (this.checkBox7.Checked)
                        {
                            this.textBox10.Focus();
                            this.textBox10.SelectAll();
                        }
                        else
                        {
                            this.textBox17.Focus();
                            this.textBox17.SelectAll();
                        }
                    }
                    else
                    {
                        this.textBox7.Focus();
                        this.textBox7.SelectAll();
                    }


                }
                else
                {
                    this.textBox2.Focus();
                    this.textBox2.SelectAll();
                }



            }
        }


        //铭牌MAC扫描
        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                string tt_scanmac = this.textBox7.Text.ToUpper().Trim();

                if (tt_scanmac == tt_shortmac)
                {
                    this.textBox18.Text = "PASS";
                    this.textBox18.BackColor = Color.Chartreuse;

                    if (this.checkBox7.Checked)
                    {
                       this.textBox10.Focus();
                       this.textBox10.SelectAll();
                    }
                    else
                    {
                        this.textBox17.Focus();
                        this.textBox17.SelectAll();
                    }
                }
                else
                {
                    this.textBox18.Text = "FAIL:"+tt_shortmac;
                    this.textBox18.BackColor = Color.Red;
                    this.textBox7.Focus();
                    this.textBox7.SelectAll();
                }


            }
        }


        //小串码扫描
        private void textBox17_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                string tt_scanboxlable = this.textBox17.Text.ToUpper().Trim();

                if (tt_scanboxlable == tt_boxlable)
                {
                    this.textBox21.Text = "PASS";
                    this.textBox21.BackColor = Color.Chartreuse;

                
                    this.textBox10.Focus();
                    this.textBox10.SelectAll();
                   
                }
                else
                {
                    this.textBox21.Text = "FAIL:" + tt_boxlable;
                    this.textBox21.BackColor = Color.Red;
                    this.textBox17.Focus();
                    this.textBox17.SelectAll();
                }


            }
        }


        //彩盒串码扫描
        private void textBox10_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                string tt_scanboxlable2 = this.textBox10.Text.ToUpper().Trim();

                if (tt_scanboxlable2 == tt_boxlable)
                {
                    this.textBox19.Text = "PASS";
                    this.textBox19.BackColor = Color.Chartreuse;
                    this.textBox13.Focus();
                    this.textBox13.SelectAll();

                }
                else
                {
                    this.textBox19.Text = "FAIL:" + tt_boxlable;
                    this.textBox19.BackColor = Color.Red;
                    this.textBox10.Focus();
                    this.textBox10.SelectAll();
                }


            }
        }


        //彩盒电源扫描
        private void textBox13_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                string tt_scanshell = this.textBox13.Text.ToUpper().Trim();

                if (tt_scanshell == tt_shellable)
                {
                    this.textBox20.Text = "PASS";
                    this.textBox20.BackColor = Color.Chartreuse;


                    if ((this.textBox14.Text == "PASS") && (this.textBox18.Text == "PASS" || this.checkBox3.Checked) && (this.textBox21.Text == "PASS" || this.checkBox7.Checked) && (this.textBox19.Text == "PASS") && (this.textBox20.Text == "PASS"))
                    {
                        Boolean tt_update = CheckInfoIntoDataBase();
                        if (tt_update)
                        {
                            this.label18.Text = "OK，条码检验一致，信息记录成功";
                            tt_yield++;
                            label11.Text = tt_yield.ToString();
                            this.richTextBox1.Text = tt_hostlable + "\n" + this.richTextBox1.Text;
                            GetProductInfo();

                            //this.textBox2.Text = null;
                            //this.textBox7.Text = null;
                            //this.textBox17.Text = null;
                            //this.textBox10.Text = null;
                            //this.textBox13.Text = null;

                            this.textBox2.Focus();
                            this.textBox2.SelectAll();
                        }
                        else
                        {
                            this.label18.Text = "NG，条码检验一致，信息记录不成功，再扫描一次";
                        }
                    }

                }
                else
                {
                    this.textBox20.Text = "FAIL:" + tt_shellable;
                    this.textBox20.BackColor = Color.Red;
                    this.textBox10.Focus();
                    this.textBox10.SelectAll();
                }


            }
        }
        #endregion




        #region 5、辅助方法
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



        //记录信息
        private Boolean CheckInfoIntoDataBase()
        {
            Boolean tt_flag = false;
            string tt_update = "update odc_alllable set bosasn = '信息已比对' " +
                               "where taskscode = '" + this.textBox1.Text + "' and hostlable = '" + tt_hostlable + "'  and id = '" + tt_id + "'";

            int tt_exuct = Dataset1.ExecCommand(tt_update,tt_conn);

            if (tt_exuct > 0)
            {
                tt_flag = true;
            }


            return tt_flag;
        }



        //生产数据记录
        private void GetProductInfo()
        {
            string tt_sql = "select top 10 count(1),count(case when boxlable is not null then 1 end),count(case when bosasn is not null then 1 end) " +
                            "from odc_alllable where taskscode ='"+this.textBox1.Text+"' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label10.Text = tt_array[0];
            this.label12.Text = tt_array[1];
            this.label13.Text = tt_array[2];

        }




        #endregion


        #region 6、按钮事件
        //重置按钮
        private void button1_Click(object sender, EventArgs e)
        {

            ClearLabelInfo1();
            this.richTextBox1.Text = null;

            this.textBox2.Text = null;
            this.textBox7.Text = null;
            this.textBox17.Text = null;
            this.textBox10.Text = null;
            this.textBox13.Text = null;
            this.textBox2.Focus();
            this.textBox2.SelectAll();



        }
        #endregion

        


    }
}
