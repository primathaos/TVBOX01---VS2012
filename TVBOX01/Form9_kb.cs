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
    public partial class Form9_kb : Form
    {
        public Form9_kb()
        {
            InitializeComponent();
        }


        #region 1、设置信息
        static string tt_conn;
        int tt_shouldoldnumber = 0;  //应关联数量
        int tt_alreadyoldnumber = 0;

        private void Form9_kb_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=Adminsa@123";

            ClearTaskInfo();
            ClearProductInfo();

            this.textBox2.Visible = false;
            this.label13.Text = null;
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
        //重置按钮
        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = null;
            this.richTextBox2.Text = null;
            this.richTextBox1.BackColor = Color.White;
            this.label13.Text = null;
            this.textBox2.Text = null;
            textBox2.Focus();
            textBox2.SelectAll();
        }
        #endregion

        #region 3、清除事件
        //清除工单信息
        private void ClearTaskInfo()
        {
            //工单显示信息
            this.label6.Text = null;
            this.label7.Text = null;
            this.label8.Text = null;
            this.label9.Text = null;
            this.label22.Text = null;

            //生产信息
            this.label17.Text = null;
            this.label18.Text = null;
            this.label19.Text = null;
        }

        //清除生产信息
        private void ClearProductInfo()
        {
            this.label17.Text = null;
            this.label18.Text = null;
            this.label19.Text = null;
            
        }
        #endregion

        #region 4、锁定事件
        //工单锁定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                string tt_sql1 = "select  tasksquantity,product_name, customer, flhratio, " +
                                        "convert(int,abs(tasksquantity*flhratio/100)) as Fshouldoldnum " +
                                 "from odc_tasks where taskscode = '" + this.textBox1.Text + "' and  taskstate = 2 ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label6.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label7.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                    this.label8.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();
                    this.label9.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString();
                    this.label22.Text = GetOldStation(this.label7.Text);  //老化站位
                    tt_shouldoldnumber = Convert.ToInt32(ds1.Tables[0].Rows[0].ItemArray[4].ToString());
                    this.label18.Text = tt_shouldoldnumber.ToString();
                    GetProductInfor();
                    this.textBox1.Enabled = false;
                    this.textBox2.Visible = true;
                    textBox2.Focus();
                    textBox2.SelectAll();
                }
                else
                {

                    MessageBox.Show("没有查询此工单，或此工单没有审批，请确认！");
                }


            }
            else
            {
                this.textBox1.Enabled = true;
                this.textBox2.Visible = false;
                ClearTaskInfo();
            }
        }

        //字符判断锁定
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
        #endregion


        #region 5、功能事件
        //获取生产信息
        private void GetProductInfor()
        {
            string tt_sql = "select COUNT(1),sum(case when ageing = '1' then 1 else 0 end) N01,0  "+
                           "from ODC_ALLLABLE  where TASKSCODE = '"+this.textBox1.Text+"' ";

            string[] tt_array1 = new string[3];
            tt_array1 = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label17.Text = tt_array1[0];
            this.label19.Text = tt_array1[1];
            tt_shouldoldnumber = Convert.ToInt32(tt_array1[2]);
        }


        //单板扫描
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //开始单板扫描
                this.label13.Text = null;
                string tt_scanmac = this.textBox2.Text.Trim();
                string tt_scanpcbasn = tt_scanmac.Replace(":", ""); ;
                string tt_task = this.textBox1.Text.Trim();
                string tt_mac = tt_scanpcbasn;
                this.richTextBox1.Text = null;
                this.richTextBox1.BackColor = Color.White;
                setRichtexBox("------开始扫描单板号-------");

                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(this.textBox2.Text.Trim(), this.textBox3.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(this.textBox2.Text.Trim(), this.textBox4.Text.Trim());
                }


                //第三步 是否超计划老化判断
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    tt_alreadyoldnumber = Convert.ToInt32(this.label19.Text);
                    tt_shouldoldnumber = Convert.ToInt32(this.label18.Text);

                    if (tt_alreadyoldnumber <= tt_shouldoldnumber)
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、已老化数量还没有到应老化数量，goon");
                    }
                    else
                    {
                        setRichtexBox("3、已老化数量已超过应老化数量，不能再扫描了，over");
                        PutLableInfor("已老化数量已超过应老化数量！");
                    }

                }



                //第四步 判断是否已老化
                Boolean tt_flag4 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    string tt_sql4 = "select COUNT(1), sum(case when ageing = '1' then 1 else 0 end) N01,min(maclable) N02 " +
                           "from ODC_ALLLABLE where TASKSCODE = '" + tt_task + "' and (PCBASN = '" + tt_scanpcbasn + "' or maclable = '" + tt_scanpcbasn + "') ";

                    string[] tt_array4 = new string[3];
                    tt_array4 = Dataset1.GetDatasetArray(tt_sql4, tt_conn);
                    if (tt_array4[0] == "1")
                    {
                        if (tt_array4[1] == "0")
                        {
                            tt_flag4 = true;
                            tt_mac = tt_array4[2];
                            setRichtexBox("4、该单板还没进老化，可以老化，MAC:"+tt_mac+",goon");
                        }
                        else
                        {
                            setRichtexBox("4、该单板已经进老化，不能再扫描了，over");
                            PutLableInfor("该单板已经进老化！不用再扫描");
                        }
                    }
                    else
                    {
                        setRichtexBox("4、该工单下没有发现该单板，确认是否工单选错了，或单板拿错了，over");
                        PutLableInfor("该工单下没有发现该单板，请检查！");
                    }


                }



                //第五步站位判断
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    string tt_code = this.label22.Text;

                    string tt_sql5 = "select count(1),min(ccode),min(ncode) from odc_routingtasklist " +
                                     "where  pcba_pn = '" + tt_mac + "' and napplytype is null ";


                    string[] tt_array5 = new string[3];
                    tt_array5 = Dataset1.GetDatasetArray(tt_sql5, tt_conn);
                    if (tt_array5[0] == "1")
                    {
                        if (tt_array5[2] == tt_code)
                        {
                            tt_flag5 = true;
                            setRichtexBox("5、该单板有待测站位，站位：" + tt_array5[1] + "--->" + tt_array5[2] + ",可以老化扫描 goon");
                        }
                        else
                        {
                            setRichtexBox("5、该单板待测站位不在" + tt_code + "，站位：" + tt_array5[1] + "--->" + tt_array5[2] + ",不可以老化扫描 over");
                            PutLableInfor("该单板当前站位：" + tt_array5[2] + "不在" + tt_code + "站位！不能老化");
                        }

                    }
                    else
                    {
                        setRichtexBox("5、没有找到待测站位，或有多条待测站位，流程异常，over");
                        PutLableInfor("没有找到待测站位，或有多条待测站位，流程异常！");
                    }

                }









                //第六步 老化表做记录
                Boolean tt_flag6 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 )
                {
                    string tt_sql6 = "insert into ODC_BARCODELH(BARCODE,TASKSCODE,Fdate) "+
                                     "values('" + tt_mac + "','" + tt_task + "',getdate() ) ";

                    int tt_exec6 = Dataset1.ExecCommand(tt_sql6,tt_conn);

                    if (tt_exec6 >0 )
                    {
                        tt_flag6 = true;
                        setRichtexBox("6、数据已成功记录到老化表ODC_BARCODELH，goon");
                    }
                    else
                    {
                        setRichtexBox("6、数据没有记录到老化表ODC_BARCODELH，over");
                        PutLableInfor("数据没有记录到老化表，请再次扫描！");
                    }

                }



                //第七步 修改关联表状态
                Boolean tt_flag7 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 )
                {
                    string tt_sql7 = "update ODC_ALLLABLE set AGEING = '1' "+
                          "where TASKSCODE = '"+tt_task+"' and (PCBASN = '"+tt_scanpcbasn+"' or maclable = '"+tt_mac+"') ";

                    int tt_exec7 = Dataset1.ExecCommand(tt_sql7, tt_conn);

                    if (tt_exec7 > 0)
                    {
                        tt_flag7 = true;
                        setRichtexBox("7、关联表数据已更新成功，goon");
                    }
                    else
                    {
                        setRichtexBox("7、关联表数据没有更新成功，over");
                        PutLableInfor("关联表数据没有更新成功，请再次扫描！");
                    }

                }




                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7)
                {
                    GetProductInfor();
                    PutLableInfor("老化记录成功，请继续扫描！");
                    this.richTextBox2.Text = tt_scanpcbasn + "\n" + this.richTextBox2.Text;
                    this.richTextBox1.BackColor = Color.Chartreuse;
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                }



                textBox2.Focus();
                textBox2.SelectAll();

            }

        }


        #endregion



        #region 6、辅助功能
        //richtext加记录
        private void setRichtexBox(string tt_textinfor)
        {
            this.richTextBox1.Text = this.richTextBox1.Text + tt_textinfor + "\n";
        }


        //lable提示信息
        private void PutLableInfor(string tt_lableinfo)
        {
            this.label13.Text = tt_lableinfo;
        }

        //位数判断方法
        private Boolean CheckStrLengh(string tt_checkstr, string tt_lengthtext)
        {
            Boolean tt_flag = false;

            int tt_snlength = int.Parse(tt_lengthtext);
            if (tt_checkstr.Length == tt_snlength)
            {
                tt_flag = true;
                setRichtexBox("1、位数判断正确，" + tt_snlength.ToString() + "位，goon");
            }
            else
            {
                setRichtexBox("1、位数判断不正确，不是" + tt_snlength.ToString() + "位");
                PutLableInfor("位数判断不正确，不是" + tt_snlength.ToString() + "位,请确认！");
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
                    setRichtexBox("2、包含符判断正确，goon");
                }
                else
                {
                    setRichtexBox("2、包含符判断不正确，不包含字符" + tt_containstr + ",over");
                    PutLableInfor("包含符判断不正确，不包含字符" + tt_containstr + ",请确认！");
                }

            }
            else
            {
                tt_flag = true;
                setRichtexBox("2、包含符为空，不需判断，goon");
            }

            return tt_flag;
        }


        //获取老化站位
        private string GetOldStation(string tt_class)
        {
            string tt_code = "I DONT KNOW";

            if (tt_class == "8Q40N")
            {
                tt_code = "3010";
            }


            if (tt_class == "K144J")
            {
                tt_code = "3000";
            }


            return tt_code;
        }

        #endregion

    }
}
