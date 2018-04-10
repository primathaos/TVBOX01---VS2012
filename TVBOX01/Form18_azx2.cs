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
    public partial class Form18_azx2 : Form
    {
        public Form18_azx2()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_conn;
        static string tt_ccode = "0000";
        static string tt_gyid = "";
        int tt_yield = 0;  //产量

        private void Form18_azx2_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";


            tt_ccode = getTestCode(str);
            this.label7.Text = tt_ccode;
            this.label16.Text = null;
            this.radioButton1.Checked = true;

            ClearItemLable();
            this.textBox7.Visible = false;
            //生产节拍
            this.label8.Text = tt_yield.ToString();

            //listview设置
            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.LabelEdit = true; //是否可编辑,ListView只可编辑第一列。
            this.listView1.Scrollable = true;//有滚动条
            this.listView1.HeaderStyle = ColumnHeaderStyle.Clickable;//对表头进行设置
            this.listView1.FullRowSelect = true;//是否可以选择行

            //添加表头
            this.listView1.Columns.Add("NO", 60);
            this.listView1.Columns.Add("MAC", 200);
            this.listView1.Columns.Add("PCBA", 200);

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


        #region 2、数据清理

        private void ClearItemLable()
        {
            //清除工单
            this.label55.Text = null;
            this.label56.Text = null;
            this.label57.Text = null;

            //站位
            this.label58.Text = null;
            this.label59.Text = null;

            //信息提示
            this.label11.Text = null;

        }

        //扫描前数据初始化
        private void ScanDataInitial()
        {
            this.label11.Text = null;

            //richtext
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;


        }


        #endregion


        #region 3、辅助功能

        //richtext加记录
        private void setRichtexBox(string tt_textinfor)
        {
            this.richTextBox1.Text = this.richTextBox1.Text + tt_textinfor + "\n";
        }


        //lable提示信息
        private void PutLableInfor(string tt_lableinfo)
        {
            this.label11.Text = tt_lableinfo;
        }

        //长MAC转换为短MAC
        private string GetShortMac(string tt_longmac)
        {
            string tt_shortmac = "";
            if (tt_longmac.Contains("-"))
            {
                tt_shortmac = tt_longmac.Replace("-", "");
            }
            else if (tt_longmac.Contains(":"))
            {
                tt_shortmac = tt_longmac.Replace(":", "");
            }
            else if (tt_longmac.Contains(" "))
            {
                tt_shortmac = tt_longmac.Replace(" ", "");
            }
            else
            {
                tt_shortmac = tt_longmac;
            }


            return tt_shortmac;
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
                setRichtexBox("1、位数判断不正确，不是" + tt_snlength.ToString() + "位,实际为：" + tt_checkstr.Length.ToString());
                PutLableInfor("位数判断不正确，不是" + tt_snlength.ToString() + "位,实际为：" + tt_checkstr.Length.ToString());
            }


            return tt_flag;
        }


        //字符串转换为int
        private int getTransmitStrToInt(string tt_str)
        {
            int tt_int = 0;
            if (tt_str == "")
            {
            }
            else
            {
                try
                {
                    tt_int = int.Parse(tt_str);
                }
                catch
                {
                    MessageBox.Show(tt_str + ",转换为数字失败，请检查！");
                }
            }


            return tt_int;
        }


        //字符串转换为int
        private double getTransmitStrToDoub(string tt_str)
        {
            double tt_doub = 0;
            if (tt_str == "")
            {
            }
            else
            {
                try
                {
                    tt_doub = double.Parse(tt_str);
                }
                catch
                {
                    MessageBox.Show(tt_str + ",转换为数字失败，请检查！");
                }
            }


            return tt_doub;
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
                setRichtexBox("2、不包含符为空，不需判断，goon");
            }

            return tt_flag;
        }


        //不包含字符判断
        private Boolean CheckStrContain2(string tt_scansn, string tt_containstr)
        {
            Boolean tt_flag = false;

            if (tt_containstr.Length > 0)
            {

                if (!tt_scansn.Contains(tt_containstr))
                {
                    tt_flag = true;
                    setRichtexBox("3、不包含符判断正确，goon");
                }
                else
                {
                    setRichtexBox("3、不包含符判断不正确，不包含字符" + tt_containstr + ",over");
                    PutLableInfor("不包含符判断不正确，包含字符" + tt_containstr + ",请确认！");
                }

            }
            else
            {
                tt_flag = true;
                setRichtexBox("3、包含符为空，不需判断，goon");
            }

            return tt_flag;
        }


        #endregion


        #region 4、数据功能
        //获取生产信息
        private void GetProductNumInfo()
        {
            string tt_sql = "select  count(1),0,0 " +
                            "from odc_insertcode  where taskcode = '" + this.textBox1.Text + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label59.Text = tt_array[0];

        }

        //获取应扫描数量
        private string getShouldScan(string tt_quantity)
        {
            string tt_shouldscan = "";
            double tt_doub1 = getTransmitStrToDoub(tt_quantity);
            double tt_doub2 = tt_doub1 * 0.9;
            int tt_shoudint = Convert.ToInt32(tt_doub2);
            tt_shouldscan = tt_shoudint.ToString();
            return tt_shouldscan;
        }


        //获取当前账号的测试站位
        private string getTestCode(string tt_username)
        {
            string tt_testcode = "";
            string tt_sql1 = "select count(1),min(Fcode),0 " +
                            " from odc_fhpassword where Fname = '" + tt_username + "' ";
            string[] tt_array1 = new string[3];
            tt_array1 = Dataset1.GetDatasetArrayTwo(tt_sql1, tt_conn);

            if (tt_array1[0] == "1")
            {
                tt_testcode = tt_array1[1];
            }
            else
            {
                MessageBox.Show("当前用户号：" + tt_username + "没有找到设定的待测站位，请确认");
            }
            return tt_testcode;
        }

        #endregion


        #region 5、列表操作
        //清理listview
        private void CleatListView()
        {
            int tt_count = this.listView1.Items.Count;

            for (int i = 0; i < tt_count; i++)
            {
                listView1.Items[0].Remove();
            }
        }

        //添加listview数据
        private void PutListViewData(string tt_mac, string tt_pcba)
        {
            int i = this.listView1.Items.Count + 1;
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { i.ToString(),  tt_mac, tt_pcba });
            this.listView1.Items.AddRange(p);
            this.listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
        }

        #endregion


        #region 6、锁定功能
        //工单选择
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                string tt_sql = " select tasksquantity,product_name,areacode,gyid  " +
                          "from odc_tasks where  taskstate = 2 and taskscode = '" + this.textBox1.Text + "' ";

                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql, tt_conn);

                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label55.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label56.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //产品名称
                    this.label57.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区
                    tt_gyid = ds1.Tables[0].Rows[0].ItemArray[3].ToString();  //生产流程
                    this.label58.Text = getShouldScan(this.label55.Text);   //获取应扫描数量

                    this.textBox1.Enabled = false;
                    this.textBox7.Text = null;
                    this.textBox7.Visible = true;
                    GetProductNumInfo();
                }
                else
                {
                    MessageBox.Show("没有找到此工单！或者该工单还没有审批，请确认");
                }

               
            }
            else
            {
                this.textBox7.Visible = false;
                ClearItemLable();
                this.textBox1.Enabled = true;
            }
        }
        
        //锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.textBox2.Enabled = false;
                this.textBox3.Enabled = false;
                this.textBox4.Enabled = false;
            }
            else
            {
                this.textBox2.Enabled = true;
                this.textBox3.Enabled = true;
                this.textBox4.Enabled = true;
            }
        }

        #endregion


        #region 7、按钮功能
        //重置
        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox7.Text = null;
            this.label11.Text = null;

            //richtext
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;
        }
        #endregion

        
        #region 8、扫描功能
        //扫描
        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描--------");
                string tt_task = this.textBox1.Text.ToUpper().Trim();
                string tt_scanmac = this.textBox7.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace("-", "");


                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox3.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox4.Text.Trim());
                }

                //第三步 不包含符判断
                Boolean tt_flag3 = false;
                if (tt_flag2)
                {
                    tt_flag3 = CheckStrContain2(tt_scanmac, this.textBox2.Text.Trim());
                }




                //第四步 扣数判断
                Boolean tt_flag4 = false;
                if (tt_flag3)
                {
                    int tt_shouldscan = getTransmitStrToInt(this.label58.Text);
                    int tt_scanint = getTransmitStrToInt(this.label59.Text);
                    if (tt_scanint <= tt_shouldscan)
                    {
                        tt_flag4 = true;
                        setRichtexBox("4、还没有达到上限，应扫描数量：" + tt_shouldscan.ToString() + ",实际扫描数量：" + tt_scanint.ToString() + ",goon");

                    }
                    else
                    {
                    }

                }




                //第五步 获取单板信息
                string tt_pcba = "";
                string tt_mac = "";
                Boolean tt_flag5 = false;
                if (tt_flag4)
                {
                    string tt_sql5 = "select pcbasn,maclable  from odc_alllable " +
                                      "where hprintman = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";

                    DataSet ds5 = Dataset1.GetDataSet(tt_sql5, tt_conn);
                    if (ds5.Tables.Count > 0 && ds5.Tables[0].Rows.Count > 0)
                    {
                        tt_flag5 = true;
                        tt_pcba = ds5.Tables[0].Rows[0].ItemArray[0].ToString();   //单板号
                        tt_mac = ds5.Tables[0].Rows[0].ItemArray[1].ToString();   //单板号
                        setRichtexBox("5、关联表查询到一条数据，pcba=" + tt_pcba + ",mac=" + tt_mac +  ",goon");

                    }
                    else
                    {
                        setRichtexBox("5、关联表没有查询到数据，over");
                        PutLableInfor("关联表没有查询到数据，请检查！");
                    }
                }



                //第六步 获取站位
                Boolean tt_flag6 = false;
                if (tt_flag5)
                {
                    string tt_sql6 = "select count(1),min(ccode),min(ncode) from odc_routingtasklist " +
                                     "where  pcba_pn = '" + tt_shortmac + "' and napplytype is null ";


                    string[] tt_array6 = new string[3];
                    tt_array6 = Dataset1.GetDatasetArray(tt_sql6, tt_conn);
                    if (tt_array6[0] == "1")
                    {
                        if (tt_array6[2] == tt_ccode)
                        {
                            tt_flag6 = true;
                            setRichtexBox("6、该单板有待测站位，站位：" + tt_array6[1] + "，" + tt_array6[2] + ",可以过站 goon");
                        }
                        else
                        {
                            setRichtexBox("6、该单板待测站位不在" + tt_ccode + "，站位：" + tt_array6[1] + "，" + tt_array6[2] + ",不可以过站 goon");
                            PutLableInfor("该单板当前站位：" + tt_array6[2] + "不在" + tt_ccode + "站位！");
                        }

                    }
                    else
                    {
                        setRichtexBox("6、没有找到待测站位，或有多条待测站位，流程异常，over");
                        PutLableInfor("没有找到待测站位，或有多条待测站位，流程异常！");
                    }
                }

                //第六步附 检查2115 站位状态
                Boolean tt_flag6_1 = false;
                if (tt_flag6)
                {
                    string tt_sql6_1 = "select ncode,napplytype from dbo.odc_routingtasklist where pcba_pn = '" + tt_shortmac + "' and ncode = '2115'";

                    string tt_allprocesses = Dataset2.getGyidAllProcess(tt_gyid, tt_conn);                    
                    DataSet ds6_1 = Dataset1.GetDataSetTwo(tt_sql6_1, tt_conn);
                    bool tt_processcheck = true;
                    if (ds6_1.Tables.Count > 0 && ds6_1.Tables[0].Rows.Count > 0)
                    {
                        string tt_napplytype = "1";
                        for (int i = 0; i < ds6_1.Tables[0].Rows.Count; i++)
                        {
                            tt_napplytype = ds6_1.Tables[0].Rows[i].ItemArray[1].ToString();
                            if (tt_napplytype == "0")
                            {
                                tt_processcheck = false;
                                break;
                            } 
                        }

                        if ((tt_allprocesses.Contains("2111") && tt_napplytype == "1") || tt_processcheck)
                        {
                            tt_flag6_1 = true;
                            setRichtexBox("6.1、该产品产品耦合测试没有出现过不良，或者有2111站位，且当前已通过耦合测试，可以过站，goon");
                        }
                        else if (!tt_allprocesses.Contains("2111") && !tt_processcheck)
                        {
                            setRichtexBox("6.1、该产品流程没有2111站位，且耦合测试曾经出现过测试失败，不允许过站，over");
                            PutLableInfor("该产品2111不是必测站位，且耦合测试曾经出现过测试失败，不建议测试吞吐量！");
                        }
                        else if (tt_allprocesses.Contains("2111") && tt_napplytype == "0")
                        {
                            setRichtexBox("6.1、该产品流程有2111站位，但耦合测试没有通过测试，不允许过站，over");
                            PutLableInfor("该产品流程有2111站位，但耦合测试没有通过测试，不允许过站！");
                        }
                    }
                    else
                    {
                        setRichtexBox("6.1、没有找到2115站位状态，流程异常，over");
                        PutLableInfor("没有找到2115站位状态，流程异常！");
                    }
                }

                //第七步 开始过站
                Boolean tt_flag7 = false;
                if (tt_flag6_1)
                {
                    string tt_sql7 = "update odc_routingtasklist set ncode = '2111',Fremark ='10%扫描耦合' " +
                                     "where pcba_pn = '" + tt_mac + "' and napplytype is null ";

                    int tt_int7 = Dataset1.ExecCommand(tt_sql7,tt_conn);
                    if (tt_int7 > 0)
                    {
                        tt_flag7 = true;
                        setRichtexBox("7、OK过站成功，跳站到2111");
                    }
                    else
                    {
                        setRichtexBox("7、Fail过站失败，over");
                        PutLableInfor("跳站2111失败，请重新扫描！");
                    }
                }




                 //第八步 记录数据
                Boolean tt_flag8 = false;
                if (tt_flag7)
                {
                    string tt_sql8 = "insert into odc_insertcode(Taskcode,MAC,PCBA,Ncode,Fdate) " +
                                     "values('"+tt_task+"','"+tt_mac+"','"+tt_pcba+"','"+tt_ccode+"',getdate()) ";
                    int tt_int8 = Dataset1.ExecCommand(tt_sql8, tt_conn);
                    if (tt_int8 > 0)
                    {
                        tt_flag8 = true;
                        setRichtexBox("8、过站记录成功，goon");
                    }
                    else
                    {
                        setRichtexBox("8、过站记录成功，over");
                        PutLableInfor("过站记录失败，扫描其他单板！");
                    }

                }



                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8)
                {
                    GetProductNumInfo();
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    setRichtexBox("9、over");
                    tt_yield++;
                    this.label8.Text = tt_yield.ToString();
                    PutListViewData(tt_mac,tt_pcba);
                    PutLableInfor("请继续扫描");
                    textBox7.Focus();
                    textBox7.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox7.Focus();
                    textBox7.SelectAll();
                }

            }
        }
        #endregion


        #region 9、扫描数据查询
        //确定
        private void button2_Click(object sender, EventArgs e)
        {
            this.label16.Text = null;
            this.dataGridView1.DataSource = null;
            bool tt_flag = false;
            if(this.textBox5.Text != "")
            {
                tt_flag = true;
            }
            else
            {
                MessageBox.Show("工单号不能为空！");
            }

            if (tt_flag)
            {
                string tt_task = this.textBox5.Text.ToUpper().Trim();
                string tt_sql = "select 1 ";

                string tt_sql1 = "select taskcode 工单号,count(1) 数量 from odc_insertcode  " +
                                 "where taskcode = '" + tt_task + "'  group by taskcode ";

                string tt_sql2 = "select Taskcode 工单号,MAC,PCBA 单板号,Ncode 跳转站位,Fdate 日期  from odc_insertcode "+
                                 "where Taskcode = '" + tt_task + "' ";


                if (this.radioButton1.Checked) tt_sql = tt_sql1;
                if (this.radioButton2.Checked) tt_sql = tt_sql2;


                DataSet ds1 = Dataset1.GetDataSet(tt_sql, tt_conn);

                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    dataGridView1.DataSource = ds1;
                    dataGridView1.DataMember = "Table";

                    if (this.radioButton2.Checked) this.label16.Text = ds1.Tables[0].Rows.Count.ToString();

                }
                else
                {
                    MessageBox.Show("sorry,没有查询到数据");
                }

            }

        }

        //重置
        private void button3_Click(object sender, EventArgs e)
        {
            this.label16.Text = null;
            this.textBox5.Text = null;
            this.dataGridView1.DataSource = null;

        }
       
        //显示行号
        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);  
        }

        #endregion
    }
}
