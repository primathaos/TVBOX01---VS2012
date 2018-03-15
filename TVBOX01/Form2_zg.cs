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
    public partial class Form2_zg : Form
    {
        public Form2_zg()
        {
            InitializeComponent();
        }

        #region 1、属性设置

        private void Form2_zg_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";
            this.toolStripStatusLabel6.Text = tt_productstarttime.ToString();
            

            ClearItem1();
            this.textBox2.Visible = false;
            this.textBox7.Visible = false;
            this.label7.Text = tt_yield.ToString();

            //剩余MAC
            this.label15.Text = null;
            this.label16.Text = null;
            this.label17.Text = null;

            //生产节拍
            this.label24.Text = null;
            this.label25.Text = null;
            this.label26.Text = null;

            //listview设置
            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.LabelEdit = true; //是否可编辑,ListView只可编辑第一列。
            this.listView1.Scrollable = true;//有滚动条
            this.listView1.HeaderStyle = ColumnHeaderStyle.Clickable;//对表头进行设置
            this.listView1.FullRowSelect = true;//是否可以选择行

            //添加表头
            this.listView1.Columns.Add("NO", 30);
            this.listView1.Columns.Add("PCBA", 130);
            this.listView1.Columns.Add("BOSA", 120);
            this.listView1.Columns.Add("MAC", 130);
            this.listView1.Columns.Add("GPSN", 100);
            this.listView1.Columns.Add("BARCODE", 200);
            this.listView1.Columns.Add("Ith", 100);
            this.listView1.Columns.Add("VBR", 100);

        }

        //VBR相关
        string tt_ponname = "";
        string tt_bosatype = "";
        string tt_bosavbr = "";
        string tt_bosaith = "";

        //产品编译时间
        string tt_svers = "";

        //产品关联BOSA相关信息
        string tt_bosatype_explicit = "";

        static string tt_conn;
        static int tt_yield = 0;

        DateTime tt_productstarttime = DateTime.Now; //开始时间
        DateTime tt_productprimtime; //上一次时间

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


        #region 2、辅助功能
        //重置
        private void button1_Click(object sender, EventArgs e)
        {
            CleatListView();
            this.dataGridView1.DataSource = null;
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;
            this.label11.Text = null;

            this.textBox2.Text = null;
            this.textBox7.Text = null;
            this.textBox2.Enabled = true;
            this.textBox5.Enabled = true;

            textBox2.Focus();
            textBox2.SelectAll();
        }


        //清除信息
        private void ClearItem1()
        {
            //清除工单信息
            this.label4.Text = null;
            this.label5.Text = null;
            this.label29.Text = null;
            this.label30.Text = null;
            this.label32.Text = null;
            this.label38.Text = null;

            //物料密码设置信息
            this.label40.Text = null;
            this.label42.Text = null;
            this.label44.Text = null;
            this.label46.Text = null;


            //清除提示信息
            this.label11.Text = null;

            //MAC关联信息
            this.label15.Text = null;
            this.label16.Text = null;
            this.label17.Text = null;

            //下一站位信息
            this.label35.Text = null;
            this.label36.Text = null;
        }

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


        //包含字符判断
        private Boolean CheckStrContain(string tt_scansn, string tt_containstr)
        {
            Boolean tt_flag = false;

            if (tt_containstr.Length > 1)
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
                setRichtexBox("2、包含符为空，请输入至少2位包含符，over");
                PutLableInfor("包含符判断不正确，请输入至少2位包含符,请确认！");
            }

            return tt_flag;
        }



        //字符串遍历
        private bool getStrChar(string tt_longstr, string tt_chartype )
        {
            Boolean tt_flag = false;

            String tt_chars = "";

            for (int i = 0; i < tt_longstr.Length; i++ )
            {
                tt_chars = tt_longstr.Substring(i,1);
                tt_flag = getCharsCheck(tt_chars, tt_chartype);
                if (!tt_flag) break;


            }

            return tt_flag;
        }


        //字符大小判断
        private bool getCharsCheck(string tt_char, string chartype)
        {
            bool tt_flag = false;

            char c = Convert.ToChar(tt_char);

            //小写判定
            if (chartype == "1")
            {
                if (char.IsDigit(tt_char,0))
                {
                    tt_flag = true;
                    setRichtexBox(tt_char+":为数字不用大小写判断,Goon");
                }
                else
                {
                    if (c >= 'a' && c <= 'z')
                    {
                        tt_flag = true;
                        setRichtexBox(tt_char + ":为小写，判断正确,OK");
                    }
                    else
                    {
                        setRichtexBox(tt_char + ":为大写，判断不正确,Fail");
                        PutLableInfor("密码："+tt_char + ":为大写，判断不正确");
                    }
                }
            }

            //大写判定
            if (chartype == "2")
            {
                if (char.IsDigit(tt_char, 1))
                {
                    tt_flag = true;
                    setRichtexBox(tt_char + ":为数字,不用大小写判断,Goon");
                }
                else
                {
                    if (c >= 'A' && c <= 'Z')
                    {
                        tt_flag = true;
                        setRichtexBox(tt_char + ":为大写，判断正确,OK");
                    }
                    else
                    {
                        setRichtexBox(tt_char + ":为小写，判断不正确,Fail");
                        PutLableInfor("密码：" + tt_char + ":为小写，判断不正确");
                    }
                }
            }


            return tt_flag;
        }




        #endregion


        #region 3、锁定事件

        //工单选择
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if ( this.checkBox1.Checked  )
            {
                string tt_sql = " select tasksquantity,product_name,areacode,gyid,tasktype,pon_name,bosatype,svers " +
                          "from odc_tasks where  taskstate = 2 and taskscode = '" + this.textBox1.Text + "' ";


                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql,tt_conn);
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label4.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label5.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //产品名称
                    this.label30.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区
                    this.label32.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString();  //流程
                    this.label38.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString(); //物料编码
                    tt_ponname = ds1.Tables[0].Rows[0].ItemArray[5].ToString(); //PON 类型
                    tt_bosatype = ds1.Tables[0].Rows[0].ItemArray[6].ToString(); //BOSA 类型
                    tt_svers = ds1.Tables[0].Rows[0].ItemArray[7].ToString();//制造单编译时间

                    //第一步 流程代码检查
                    #region
                    Boolean tt_flag1 = false;

                    if (!this.label32.Text.Equals(""))
                    {
                        tt_flag1 = true;
                        
                    }
                    else
                    {
                        MessageBox.Show("该工单没有配置流程，请检查");
                    }
                    #endregion
                    
                    //第二步下一站位检查
                    #region
                    Boolean tt_flag2 = false;
                    if (tt_flag1)
                    {
                        tt_flag2 = GetNextCode(this.textBox1.Text, str);
                        if (tt_flag2)
                        {
                        }
                        else
                        {
                            MessageBox.Show("该工单配置流程，但是没有找到下一站位！");
                        }
                    }
                    #endregion
                    
                    //第三步运行商检查
                    #region
                    Boolean tt_flag3 = false;
                    string tt_telecustomer = "";
                    if (tt_flag2)
                    {
                        string tt_product = this.label5.Text;
                        tt_telecustomer = getTelecomOperator(tt_product);
                        if (tt_telecustomer == "0")
                        {
                            MessageBox.Show("运营商获取失败，无法确定是电信还是移动产品");
                        }
                        else
                        {
                            tt_flag3 = true;
                            this.label29.Text = tt_telecustomer;
                        }


                    }
                    #endregion
                    
                    //第四步 物料编码检查
                    #region
                    Boolean tt_flag4 = false;
                    if (tt_flag3)
                    {
                        this.label44.Text = setMetrialCheck(this.label30.Text,this.label5.Text,tt_telecustomer);
                        if (this.label44.Text == this.label38.Text )
                        {
                            if (this.label38.Text != "")
                            {
                               tt_flag4 = true;
                            }
                            else
                            {
                                MessageBox.Show("该工单物料编码为空，请检查工单设置！");
                            }
                        }
                        else
                        {
                            MessageBox.Show("该工单物料编码:" + this.label38.Text + ",与设定物料编码:" + this.label44.Text + ",不一致，请确认");
                        }
                    }
                    #endregion
                                        
                    //第五步 获取用户名密码设定
                    #region
                    Boolean tt_flag5 = false;
                    if( tt_flag4)
                    {
                        string tt_sql4 = "select username,digits,format from odc_fhuser " +
                                         "where aear = '" + this.label30.Text + "' and  operator = '"+tt_telecustomer+"' ";
                        DataSet ds4 = Dataset1.GetDataSetTwo(tt_sql4,tt_conn);
                        if (ds4.Tables.Count > 0 && ds4.Tables[0].Rows.Count > 0)
                        {
                            tt_flag5 = true;
                            this.label40.Text = ds4.Tables[0].Rows[0].ItemArray[0].ToString(); //用户名
                            this.label42.Text = ds4.Tables[0].Rows[0].ItemArray[1].ToString(); //密码位数
                            this.label46.Text = ds4.Tables[0].Rows[0].ItemArray[2].ToString();  //密码大小写
                        }
                        else
                        {
                            MessageBox.Show("没有找到地区:" + this.label30.Text +"，的用户名及密码设定，请确认！");
                        }


                    }
                    #endregion

                    //第六步 用户密码设定检查
                    #region
                    Boolean tt_flag6 = false;
                    if( tt_flag5)
                    {
                        if (this.label40.Text == "" || this.label42.Text == "" || this.label46.Text == "")
                        {
                            MessageBox.Show("用户名，或密码设定值为空，请检查数据");
                        }
                        else
                        {
                            tt_flag6 = true;
                        }
                    }
                    #endregion

                    //最后判断
                    #region
                    if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                    {
                        this.textBox2.Text = null;
                        this.textBox7.Text = null;
                        this.textBox2.Visible = true;
                        this.textBox7.Visible = true;
                        GetMacUseNumber();
                    }
                    #endregion
                }
                else
                {

                   MessageBox.Show("没有找到此工单！或者该工单还没有审批，请确认");
                }

                this.textBox1.Enabled = false;

            }
            else
            {
                this.textBox1.Enabled = true;
                this.textBox2.Visible = false;
                this.textBox7.Visible = false;

                tt_ponname = "";
                tt_bosatype = "";
                tt_bosavbr = "";
                tt_bosaith = "";
                tt_svers = "";

                ClearItem1();
                CleatListView();
                this.dataGridView1.DataSource = null;
                this.richTextBox1.Text = null;
                this.richTextBox1.BackColor = Color.White;
            }
        }


        //判断位数锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if ( this.checkBox2.Checked )
            {
                this.textBox3.Enabled = false;
                this.textBox4.Enabled = false;
                this.textBox5.Enabled = false;
                this.textBox6.Enabled = false;
            }
            else
            {
                this.textBox3.Enabled = true;
                this.textBox4.Enabled = true;
                this.textBox5.Enabled = true;
                this.textBox6.Enabled = true;
            }

        }

        #endregion


        #region 4、扫描事件


        //扫描单板       
        private DateTime dt_1 = DateTime.Now; //手动输入限制的私有变量
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            //限制手动输入
            DateTime tempDt = DateTime.Now;
            TimeSpan ts = tempDt.Subtract(dt_1);
            if (ts.Milliseconds > 100)
            {
                textBox2.Clear();
            }
            dt_1 = tempDt;
            
            //条码数据判断
            if (e.KeyCode == Keys.Enter)
            {

                //开始关联 数据清理
                string tt_scanpcba = this.textBox2.Text.Trim().ToUpper();
                string tt_task = this.textBox1.Text;
                
                this.label11.Text = null;
                this.richTextBox1.Text = null;
                this.richTextBox1.BackColor = Color.White;
                this.dataGridView1.DataSource = null;
                this.textBox7.Enabled = false;
                setRichtexBox("--开始单板:" + tt_scanpcba + ",扫描-------------");

                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanpcba, this.textBox3.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanpcba, this.textBox4.Text.Trim());
                }


                //第三步 单板号判断
                Boolean tt_flag3 = false;
                if ( tt_flag1 && tt_flag2 )
                {
                    string tt_sql3 = "select count(1),0,0  from odc_alllable where pcbasn = '" + tt_scanpcba + "' ";
                    string[] tt_array3 = new string[3];
                    tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                    if (tt_array3[0] == "0" )
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、该单板在关联表alllable中没有数据，可以关联,goon");
                    }
                    else
                    {
                        setRichtexBox("3、该单板在关联表alllable中已有数据，不能关联,ober");
                        PutLableInfor("此单板在关联表中已有数据,请确认是否已关联过！");

                    }
                }

                //第四步 是否有MAC检查
                Boolean tt_flag4 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 )
                {
                    if (this.label15.Text == "0" || this.label15.Text == "")
                   {
                       setRichtexBox("4、从统计信息上看，该工单已没有MAC，不能再做关联,over");
                       PutLableInfor("该工单已没有MAC，不能再做关联！");
                   }
                   else
                   {
                       tt_flag4 = true;
                       setRichtexBox("4、从统计信息上看，该工单已还有MAC，可以继续关联,goon");

                   }

                }

                //第五步检查工单的流程配置
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    if (this.label35.Text == "" || this.label36.Text == "")
                    {
                        setRichtexBox("5、该工单没有配置流程," + this.label35.Text + "," + this.label36.Text + ",over");
                        PutLableInfor("没有获取到当前待测站位，及下一站位，请检查");
                    }
                    else
                    {
                        tt_flag5 = true;
                        setRichtexBox("5、该工单已配置流程," + this.label35.Text + "," + this.label36.Text + ",goon");
                    }
                }

                //第六步包装表中是否有这个单板，就是单板双胞胎判断
                Boolean tt_flag6 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 )
                {
                    string tt_sql6 = "select count(1),min(taskcode),0 from odc_package " +
                                     "where pasn = '" + tt_scanpcba + "' ";
                    string[] tt_array6 = new string[3];
                    tt_array6 = Dataset1.GetDatasetArray(tt_sql6, tt_conn);
                    if (tt_array6[0] == "0")
                    {
                        tt_flag6 = true;
                        setRichtexBox("6、该单板在包装表package没有找到数据，可以关联,goon");
                    }
                    else
                    {
                        setRichtexBox("6、该单板在包装表package中已有数据，已用工单" + tt_array6[1] + ",不能关联,ober");
                        PutLableInfor("此单板在包装表package已有数据,请确认是否已使用过！");
                    }
                }                

                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {
                    setRichtexBox("6、单板扫描完成,请扫描BOSA号，over");
                    PutLableInfor("单板扫描OK,请扫描BOSA");
                    this.textBox2.Enabled = false;
                    this.textBox7.Enabled = true;
                    textBox7.Focus();
                    textBox7.SelectAll();                    
                }
                else
                {
                    this.textBox2.Enabled = true;
                    this.textBox7.Enabled = false;
                    this.richTextBox1.BackColor = Color.Red;
                    textBox2.Focus();
                    textBox2.SelectAll();
                }
            }
        }

        //BOSA扫描
        private DateTime dt_2 = DateTime.Now; //手动输入限制的私有变量
        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            //限制手动输入
            DateTime tempDt = DateTime.Now;
            TimeSpan ts = tempDt.Subtract(dt_2);
            if (ts.Milliseconds > 100)
            {
                textBox7.Clear();
            }
            dt_2 = tempDt;

            //条码数据判断
            if (e.KeyCode == Keys.Enter)
            {

                ////开始关联 数据清理
                string tt_scanpcba = this.textBox2.Text.Trim().ToUpper();
                string tt_scanbosa = this.textBox7.Text.Trim().ToUpper();
                string tt_task = this.textBox1.Text;
                string tt_username = STR;
                string tt_gyid = this.label32.Text;
                string tt_ccode = this.label35.Text;
                string tt_ncode = this.label36.Text;
                string tt_mac = ""; 

                setRichtexBox("----开始BOSA:" + tt_scanbosa + ",扫描-----");
                this.label11.Text = null;

                //第零步错板防呆
                Boolean tt_flag0 = false;

                int tt_bosasnlength = int.Parse(this.textBox3.Text);

                if (tt_scanbosa.Length == tt_bosasnlength && (tt_scanbosa.Substring(0, 2) == tt_scanpcba.Substring(0, 2) || tt_scanbosa.Substring(4, 5) == tt_scanpcba.Substring(4, 5)))
                {
                    setRichtexBox("0、编码格式与单板一致，可能扫描错误,over");
                    PutLableInfor("编码格式与单板一致，可能扫描错误，请重新从单板开始扫描");
                }
                else
                {
                    tt_flag0 = true;
                    setRichtexBox("0、单板号与BOSA号格式不一致,goon");
                }

                //第一步位数判断
                Boolean tt_flag1 = false;
                if (tt_flag0)
                {
                    tt_flag1 = CheckStrLengh(tt_scanbosa, this.textBox6.Text);
                }


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanbosa, this.textBox5.Text.Trim());
                }

                //第二附一步 单板与BOSA对应关联判断
                Boolean tt_flag2_1 = false;
                if (tt_flag2)
                {
                    string tt_pcba_num = tt_scanpcba.Substring(4, 4) + "-" + tt_scanpcba.Substring(8, 1);

                    tt_bosatype_explicit = "";

                    string tt_sql2 = "select startindex_1,length_1,feature_1,startindex_2,length_2,feature_2,bosa_type,key_0 from odc_bosatypelist " +
                                     "where pcba_num like '%" + tt_pcba_num + "%'";

                    DataSet ds2 = Dataset1.GetDataSetTwo(tt_sql2, tt_conn);
                    if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0) //单板条码在数据库BOSATYPELIST表查到的情况，即检查单板是Macom方案，BOSA不是Macom方案的情况
                    {
                        bool tt_flag2_2 = false;
                        for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                        {
                            int tt_startindex_1 = int.Parse(ds2.Tables[0].Rows[i].ItemArray[0].ToString());
                            int tt_length_1 = int.Parse(ds2.Tables[0].Rows[i].ItemArray[1].ToString());
                            string tt_feature_1 = ds2.Tables[0].Rows[i].ItemArray[2].ToString();
                            int tt_startindex_2 = int.Parse(ds2.Tables[0].Rows[i].ItemArray[3].ToString());
                            int tt_length_2 = int.Parse(ds2.Tables[0].Rows[i].ItemArray[4].ToString());
                            string tt_feature_2 = ds2.Tables[0].Rows[i].ItemArray[5].ToString();
                            int tt_key = int.Parse(ds2.Tables[0].Rows[i].ItemArray[7].ToString());

                            bool bool_bosatype_1 = false;
                            bool bool_bosatype_2 = false;
                            string tt_bosatype_1 = "";
                            string tt_bosatype_2 = "";

                            if (tt_length_1 == 0 && tt_length_2 == 0)
                            {
                                PutLableInfor("BOSA方案拦截，单一物料不能全设为0，请工程检查数据库");
                                i = ds2.Tables[0].Rows.Count;
                            }
                            else
                            {
                                if (tt_length_1 == 0 && tt_key == 2)
                                {
                                    bool_bosatype_1 = true;
                                }
                                else
                                {
                                    tt_bosatype_1 = tt_scanbosa.Substring(tt_startindex_1, tt_length_1);
                                }

                                if (tt_length_2 == 0 && tt_key == 1)
                                {
                                    bool_bosatype_2 = true;
                                }
                                else
                                {
                                    tt_bosatype_2 = tt_scanbosa.Substring(tt_startindex_2, tt_length_2);
                                }
                            }                            

                            if (tt_bosatype_1 != "" && tt_bosatype_2 != "" && tt_bosatype_1 == tt_feature_1 && tt_bosatype_2 == tt_feature_2)
                            {
                                tt_flag2_2 = true;
                                tt_bosatype_explicit = ds2.Tables[0].Rows[i].ItemArray[6].ToString();
                                i = ds2.Tables[0].Rows.Count;
                            }
                            else if (tt_bosatype_2 != "" && bool_bosatype_1 && tt_bosatype_2 == tt_feature_2)
                            {
                                tt_flag2_2 = true;
                                tt_bosatype_explicit = ds2.Tables[0].Rows[i].ItemArray[6].ToString();
                                i = ds2.Tables[0].Rows.Count;
                            }
                            else if (tt_bosatype_1 != "" && tt_bosatype_1 == tt_feature_1 && bool_bosatype_2)
                            {
                                tt_flag2_2 = true;
                                tt_bosatype_explicit = ds2.Tables[0].Rows[i].ItemArray[6].ToString();
                                i = ds2.Tables[0].Rows.Count;
                            }
                        }

                        if (tt_flag2_2)
                        {
                            tt_flag2_1 = true;
                            setRichtexBox("2.1、单板与BOSA方案匹配正确,goon");
                        }
                        else
                        {
                            setRichtexBox("2.1、BOSA与单板方案匹配不正确,over");
                            PutLableInfor("BOSA与单板方案不匹配，请检查");
                        }
                    }
                    else //单板条码在数据库BOSATYPELIST表查不到的情况，即检查单板不是Macom方案，BOSA用Macom方案的情况
                    {
                        string tt_sql2_1 = "select startindex_1,length_1,feature_1,startindex_2,length_2,feature_2,key_0 from odc_bosatypelist";

                        DataSet ds2_1 = Dataset1.GetDataSetTwo(tt_sql2_1, tt_conn);
                        if (ds2_1.Tables.Count > 0 && ds2_1.Tables[0].Rows.Count > 0)
                        {
                            bool tt_flag2_3 = false;
                            for (int i = 0; i < ds2_1.Tables[0].Rows.Count; i++)
                            {
                                int tt_startindex_1 = int.Parse(ds2_1.Tables[0].Rows[i].ItemArray[0].ToString());
                                int tt_length_1 = int.Parse(ds2_1.Tables[0].Rows[i].ItemArray[1].ToString());
                                string tt_feature_1 = ds2_1.Tables[0].Rows[i].ItemArray[2].ToString();
                                int tt_startindex_2 = int.Parse(ds2_1.Tables[0].Rows[i].ItemArray[3].ToString());
                                int tt_length_2 = int.Parse(ds2_1.Tables[0].Rows[i].ItemArray[4].ToString());
                                string tt_feature_2 = ds2_1.Tables[0].Rows[i].ItemArray[5].ToString();
                                int tt_key = int.Parse(ds2_1.Tables[0].Rows[i].ItemArray[6].ToString());

                                bool bool_bosatype_1 = false;
                                bool bool_bosatype_2 = false;
                                string tt_bosatype_1 = "";
                                string tt_bosatype_2 = "";

                                int tt_lengthcheck = int.Parse(this.textBox6.Text);

                                if (tt_length_1 == 0 && tt_length_2 == 0)
                                {
                                    PutLableInfor("BOSA方案拦截，单一物料不能全设为0，请工程检查数据库");
                                    i = ds2_1.Tables[0].Rows.Count;
                                }
                                else if (tt_startindex_2 >= tt_lengthcheck)//防止BOSA条码长度低于数据库截断长度
                                {
                                    tt_flag2_1 = true;
                                }
                                else
                                {
                                    if (tt_length_1 == 0 && tt_key == 2)
                                    {
                                        bool_bosatype_1 = true;
                                    }
                                    else
                                    {
                                        tt_bosatype_1 = tt_scanbosa.Substring(tt_startindex_1, tt_length_1);
                                    }

                                    if (tt_length_2 == 0 && tt_key == 1)
                                    {
                                        bool_bosatype_2 = true;
                                    }
                                    else
                                    {
                                        tt_bosatype_2 = tt_scanbosa.Substring(tt_startindex_2, tt_length_2);
                                    }
                                }

                                if (tt_bosatype_1 != "" && tt_bosatype_2 != "" && tt_bosatype_1 == tt_feature_1 && tt_bosatype_2 == tt_feature_2)
                                {
                                    tt_flag2_3 = true;
                                    i = ds2_1.Tables[0].Rows.Count;
                                }
                                else if (tt_bosatype_2 != "" && bool_bosatype_1 && tt_bosatype_2 == tt_feature_2)
                                {
                                    tt_flag2_3 = true;
                                    i = ds2_1.Tables[0].Rows.Count;
                                }
                                else if (tt_bosatype_1 != "" && tt_bosatype_1 == tt_feature_1 && bool_bosatype_2)
                                {
                                    tt_flag2_3 = true;
                                    i = ds2_1.Tables[0].Rows.Count;
                                }
                            }

                            if (tt_flag2_3)
                            {
                                setRichtexBox("2.1、单板与BOSA方案匹配不正确,over");
                                PutLableInfor("单板与BOSA方案不匹配，请检查");
                            }
                            else
                            {
                                tt_flag2_1 = true;
                                setRichtexBox("2.1、未找到单板与BOSA匹配信息,无需进行关联约束动作，goon");
                            }
                        }
                    }
                }

                //第三步 单板号与BOSA号判断
                Boolean tt_flag3 = false;
                if (tt_flag2_1)
                {

                    if (tt_scanpcba == tt_scanbosa)
                    {
                        setRichtexBox("3、单板号与BOSA号一致，可能重复扫描,over");
                        PutLableInfor("单板号与BOSA号一致，可能重复扫描，请检查");
                    }
                    else
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、单板号与BOSA号不一致,goon");
                    }

                }


                //第四步 是否有MAC检查
                Boolean tt_flag4 = false;
                string tt_langmac = "";
                string tt_shortmac = "";
                string tt_barcode = "";
                string tt_gpsn = "";
                string tt_user = "";
                string tt_password = "";
                if (tt_flag3)
                {

                    string tt_sql4 = "select top 10 mac,barcode,sn,username,password  from odc_macinfo " +
                                      " where taskscode = '" + tt_task + "'  and fusestate is null ";

                    DataSet ds1 = Dataset1.GetDataSet(tt_sql4, tt_conn);

                    if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        //取随机数
                        int tt_rowcount = ds1.Tables[0].Rows.Count;
                        Random ran = new Random();
                        int n = ran.Next(0, tt_rowcount-1);

                        //int n1 = ran.Next(0, 0); //测试就一个值是什么情况

                        tt_langmac = ds1.Tables[0].Rows[n].ItemArray[0].ToString();
                        tt_barcode = ds1.Tables[0].Rows[n].ItemArray[1].ToString();
                        tt_gpsn = ds1.Tables[0].Rows[n].ItemArray[2].ToString();
                        tt_user = ds1.Tables[0].Rows[n].ItemArray[3].ToString();
                        tt_password = ds1.Tables[0].Rows[n].ItemArray[4].ToString();
                        tt_shortmac = GetShortMac(tt_langmac);
                        tt_mac = tt_shortmac;

                        if (tt_gpsn != "")
                        {
                            tt_flag4 = true;
                            setRichtexBox("4.1、该工单还有剩余AMC：共获取MAC数:" + tt_rowcount.ToString() + "，随机数：" + n.ToString() + ",goon");
                            setRichtexBox("4.2、该工单还有剩余AMC：已获取一个长MAC为:" + tt_langmac + "，短MAC为：" + tt_shortmac +
                                ",32位移动编码:" + tt_barcode + ",GPSN号码：" + tt_gpsn + ",可以关联,goon");
                            setRichtexBox("4.3、用户名检验，已获取一个用户为:" + tt_user + "，密码为：" + tt_password);
                        }
                        else
                        {
                            setRichtexBox("4、ONUMAC或GPSN未导入数据库,over");
                            PutLableInfor("ONUMAC或GPSN未导入数据库，不能关联");
                        }

                    }
                    else
                    {
                        setRichtexBox("4、该工单已没有有剩余AMC，不能再关联,over");
                        PutLableInfor("该工单已没有有剩余AMC，不能再关联");
                    }

                }


                //第五步检查该MAC是否已用过
                Boolean tt_flag5 = false;
                if (tt_flag4)
                {
                    string tt_sql5 = "select count(1),0,0 from odc_alllable where maclable = '"+tt_mac+"' ";

                    string[] tt_array5 = new string[3];
                    tt_array5 = Dataset1.GetDatasetArray(tt_sql5, tt_conn);
                    if (tt_array5[0] == "0")
                    {
                        tt_flag5 = true;
                        setRichtexBox("5、该MAC:"+tt_mac+"在关联表alllable中没有找到，可以关联,goon");
                    }
                    else
                    {

                        //把MAC置为已用状态
                        string tt_sql51 = "update odc_macinfo set fusestate ='1'  where taskscode='"+tt_task+"' and MAC ='"+tt_langmac+"' ";

                        int tt_int51 = Dataset1.ExecCommand(tt_sql51, tt_conn);
                        if (tt_int51 > 0)
                        {

                            setRichtexBox("5、该MAC:" + tt_mac + "在关联表alllable中有一个重复，不可以关联,已把这个状态值改为1，over");
                            PutLableInfor("获取MAC已用过,请重新扫描BOSA，重新获取MAC");
                        }
                        else
                        {
                            setRichtexBox("5、该MAC:" + tt_mac + "在关联表alllable中有一个重复，不可以关联,状态值没有改为1，over");
                            PutLableInfor("获取MAC已用过,请重新扫描BOSA，重新获取MAC");
                        }
                    }
                }


                //第六步获取MAC站位检查
                Boolean tt_flag6 = false;
                if (tt_flag5)
                {
                    string tt_sql6 = "select count(1),min(Ncode),0  from odc_routingtasklist " +
                                     "where pcba_pn = '" + tt_shortmac + "'  and napplytype is null ";

                    string[] tt_array6 = new string[3];
                    tt_array6 = Dataset1.GetDatasetArray(tt_sql6, tt_conn);
                    if (tt_array6[0] == "0")
                    {
                        tt_flag6 = true;
                        setRichtexBox("6、该MAC：" + tt_shortmac + "没有待测站位，可以关联，goon");
                    }
                    else
                    {
                        setRichtexBox("6、该MAC:" + tt_shortmac + "已有待测站位：" + tt_array6[1] + "，请再次扫描");
                        PutLableInfor("该MAC:" + tt_shortmac + "已有待测站位：" + tt_array6[1] + "，请再次扫描!");
                    }

                }

                //第七步 查看BOSA是否已用过
                Boolean tt_flag7 = false;
                if (tt_flag6)
                {
                    string tt_sql7 = "select count(1),min(taskscode),min(pcbasn)  " +
                                     "from odc_alllable " +
                                     "where bosasn = '"+tt_scanbosa+"' ";

                    string[] tt_array7 = new string[3];
                    tt_array7 = Dataset1.GetDatasetArray(tt_sql7, tt_conn);
                    if (tt_array7[0] == "0")
                    {
                        tt_flag7 = true;
                        setRichtexBox("7、该BOSA还没有使用，可以关联，goon");
                    }
                    else
                    {
                        setRichtexBox("7、该BOSA:" + tt_scanbosa + "已使用：已用在工单" + tt_array7[1] + "，已关联单板：" + tt_array7[2] + ",请换BOSA,over");
                        PutLableInfor("该BOSA已使用，不能重复关联,请更换BOSA！");
                    }

                }

                //第七步附一 查找是否有VBR值
                Boolean tt_flag7_1 = false;
                if (tt_flag7)
                {
                    if (tt_ponname == "GPON" && tt_bosatype == "APD")
                    {
                        string tt_sql7_1 = "select count(1),min(Ith),min(VBR) " +
                                         "from BOSA_VBR_XLS " +
                                         "where bosa_sn = '" + tt_scanbosa + "' ";

                        string[] tt_array7_1 = new string[3];
                        tt_array7_1 = Dataset1.GetDatasetArray(tt_sql7_1, tt_conn);
                        if (tt_array7_1[0] == "1")
                        {
                            tt_flag7_1 = true;
                            tt_bosaith = tt_array7_1[1];
                            tt_bosavbr = tt_array7_1[2];
                            setRichtexBox("7、该BOSA有参数信息，允许关联，goon");
                        }
                        else if (tt_array7_1[0] == "0")
                        {
                            setRichtexBox("7、该BOSA没有参数信息（VBR），不允许关联,over");
                            PutLableInfor("该BOSA没有参数信息（VBR），不允许关联！");
                        }
                    }
                    else if (tt_ponname == "GPON" && tt_bosatype == "SUPER_TIA")
                    {
                        tt_flag7_1 = true;
                        setRichtexBox("7.1、制造单为Super_TIA方案，不检查VBR，可以关联，goon");
                    }
                    else if (tt_ponname == "EPON")
                    {
                        tt_flag7_1 = true;
                        setRichtexBox("7.1、制造单为EPON，不检查VBR，可以关联，goon");
                    }
                    else
                    {
                        tt_flag7_1 = true;
                        setRichtexBox("7.1、制造单不区分BOSA，不检查VBR，可以关联，goon");
                    }
                }


                //第八步 用户名检查
                Boolean tt_flag8 = false;
                if(tt_flag7_1)
                {
                    if (this.label40.Text == "0" )
                    {
                        tt_flag8 = true;
                        setRichtexBox("8、设定的用户名为0，不需要进行用户名检验，goon");
                    }
                    else
                    {
                        if (this.label40.Text == tt_user)
                        {
                            tt_flag8 = true;
                            setRichtexBox("8、获取MAC用户名与设定的用户一致，都是:" + tt_user + "，goon");
                        }
                        else
                        {
                            setRichtexBox("8、该MAC的用户名:" + tt_user + ",与设定的用户名不一致：" + this.label40.Text + "，请检查MAC导入信息,over");
                            PutLableInfor("获取MAC用户名" + tt_user + "与设定的用户不一致，请检查MAC导入信息！");
                        }
                    }
                }

                //第九步 密码位数检查
                Boolean tt_flag9 = false;
                if( tt_flag8)
                {
                    if (this.label42.Text == "0" )
                    {
                        tt_flag9 = true;
                        setRichtexBox("9、密码位数设置为0,不需要位数判断，goon");
                    }
                    else
                    {
                        string tt_passwordlen = tt_password.Length.ToString();
                        if (this.label42.Text == tt_passwordlen)
                        {
                            tt_flag9 = true;
                            setRichtexBox("9、获取MAC密码" + tt_password + "的位数与设定的密码位数一致，都是:" + tt_passwordlen + "位，goon");
                        }
                        else
                        {
                            setRichtexBox("9、获取MAC密码" + tt_password + "的位数与设定的密码位数不一致，不是:" + tt_passwordlen + "位，goon");
                            PutLableInfor("9、获取MAC密码" + tt_password + "的位数与设定的密码位数不一致");
                        }
                    }
                }

                //第十步 密码大小写检查
                Boolean tt_flag10 = false;
                if( tt_flag9)
                {
                    if( this.label46.Text == "0")
                    {
                        tt_flag10 = true;
                        setRichtexBox("10、密码大小写设置为0,不需要大小判断，goon");
                    }
                    else
                    {
                        bool tt_flag101 = getStrChar(tt_password, this.label46.Text);
                        if (tt_flag101)
                        {
                            tt_flag10 = true;
                            setRichtexBox("10、密码大小写判断正确，goon");
                        }
                        else
                        {
                            setRichtexBox("10、该MAC的密码:" + tt_password + "，大小写判定不正确，1为小写2为大写");
                            setRichtexBox("该MAC的密码:"+tt_password+"，大小写判定不正确");
                        }
                    }
                }


                //第十一步进站
                Boolean tt_flag11 = false;
                if (tt_flag10)
                {
                    tt_flag11 = Dataset1.FHStarinStation(tt_task, tt_username,tt_scanpcba,
                                                        tt_langmac, tt_shortmac, 
                                                        tt_gpsn,tt_barcode, tt_scanbosa,
                                                        tt_gyid, tt_ccode, tt_ncode, tt_svers,
                                                        tt_bosatype_explicit,tt_conn);
                    if (tt_flag11)
                    {
                        setRichtexBox("11、单板MAC关联成功，请继续扫描");
                        PutLableInfor("单板MAC关联成功，请继续扫描");
                    }
                    else
                    {
                        setRichtexBox("11、单板MAC关联不成功，事务已回滚");
                        PutLableInfor("单板MAC关联不成功，请检查或再次扫描");
                    }
                }


                //最后判断
                if (tt_flag0 && tt_flag1 && tt_flag2 && tt_flag2_1 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag7_1 && tt_flag8 && tt_flag9 && tt_flag10 && tt_flag11)
                {
                    setRichtexBox("9、单板BOSA扫描完成,MAC关联完毕，over");
                    PutLableInfor("MAC关联OK,请扫描下一单板号");
                    GetMacUseNumber();
                    CheckStation(tt_mac);
                    GetProductRhythm();
                    PutListViewData(tt_scanpcba, tt_scanbosa, tt_mac, tt_gpsn, tt_barcode, tt_bosaith, tt_bosavbr);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    this.textBox2.Enabled = true;
                    this.textBox7.Enabled = false;
                    textBox2.Focus();
                    textBox2.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    this.textBox7.Text = "";
                    this.textBox2.Enabled = true;
                    this.textBox7.Enabled = false;
                    textBox2.Focus();
                    textBox2.SelectAll();
                }

            }
        }

        #endregion


        #region 5、其他功能

        //剩余MAC显示
        public void GetMacUseNumber()
        {
            //获取MAC信息
            string tt_sql1 = "select 1,  count(case when fusestate is  null then 1 end ) as fcount1, count(case when fusestate is  not null then 1 end ) as fcount2 " +
                             " from odc_macinfo  where taskscode = '" + this.textBox1.Text + "' ";

            string[] tt_array1 = new string[3];
            tt_array1 = Dataset1.GetDatasetArray(tt_sql1, tt_conn);
            this.label15.Text = tt_array1[1];
            this.label16.Text = tt_array1[2];


            //获取工单信息
            string tt_sql2 = "select count(1),0,0 from odc_alllable where Hprintman = '" + this.textBox1.Text + "' ";
            string[] tt_array2 = new string[3];
            tt_array2 = Dataset1.GetDatasetArray(tt_sql2, tt_conn);
            this.label17.Text = tt_array2[0];


        }




        //刷新站位
        private void CheckStation(string tt_mac)
        {
            string tt_sql = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime,fremark " +
                            "from ODC_ROUTINGTASKLIST    where pcba_pn = '" + tt_mac + "' ";

            DataSet ds1 = Dataset1.GetDataSet(tt_sql, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView1.DataSource = ds1;
                dataGridView1.DataMember = "Table";
            }

        }


        //--生产节拍
        private void GetProductRhythm()
        {
            tt_yield++;
            DateTime tt_productendtime = DateTime.Now;  //当前时间

            //计算时间差
            TimeSpan tt_diff;
            if (tt_yield == 1)
            {
                tt_productprimtime = tt_productstarttime;
                tt_diff = tt_productendtime - tt_productprimtime;
                tt_productprimtime = tt_productendtime;

            }
            else
            {
                tt_diff = tt_productendtime - tt_productprimtime;
                tt_productprimtime = tt_productendtime;
            }

            decimal tt_difftime = tt_diff.Hours * 3600 + tt_diff.Minutes * 60 + tt_diff.Seconds;
            string tt_millsecnds = tt_diff.Milliseconds.ToString();
            string tt_differtime2 = tt_difftime.ToString() + "." + tt_millsecnds;



            TimeSpan tt_ts = tt_productendtime - tt_productstarttime;  //耗用时间
            int tt_second = tt_ts.Hours * 3600 + tt_ts.Minutes * 60 + tt_ts.Seconds;
            string tt_time = tt_ts.Hours.ToString() + "小时" + tt_ts.Minutes.ToString() + "分" + tt_ts.Seconds.ToString() + "秒";


            int tt_avgtime = 0;
            if (tt_yield > 0 && tt_second > 0)
            {
                tt_avgtime = Math.Abs(tt_second / tt_yield);
            }

            this.toolStripStatusLabel8.Text = tt_second.ToString();
            this.label7.Text = tt_yield.ToString();   //本班产量
            this.label24.Text = tt_time;               //生产时间
            this.label25.Text = tt_avgtime.ToString();  //平均节拍
            this.label26.Text = tt_differtime2;        //实时节拍

        }




        //流程检查，获取下一流程
        private bool GetNextCode(string tt_task, string tt_username)
        {
            Boolean tt_flag = false;

            //第一步获取当前站位
            Boolean tt_flag1 = false;
            string tt_testcode = "";
            string tt_sql1 = "select count(1),min(Fcode),0 " +
                            " from odc_fhpassword where Fname = '" + tt_username + "' ";

            string[] tt_array1 = new string[3];
            tt_array1 = Dataset1.GetDatasetArrayTwo(tt_sql1, tt_conn);

            if (tt_array1[0] == "1")
            {
                tt_testcode = tt_array1[1];
                tt_flag1 = true;
            }
            else
            {
                MessageBox.Show("当前用户号：" + tt_username + "没有找到设定的待测站位，请确认");
            }


            //第二步获取第一站位
            Boolean tt_flag2 = false;
            string tt_ccode = "";
            if (tt_flag1)
            {
                string tt_sql2 = "select count(1),min(b.PXID) ,min(a.GYID) " +
                                     "from odc_tasks a,odc_routing b " +
                                     "WHERE a.GYID=b.PID AND b.LCBZ=1 AND a.TASKSCODE='" + tt_task + "' ";
                string[] tt_array2 = new string[3];
                tt_array2 = Dataset1.GetDatasetArray(tt_sql2, tt_conn);
                if (tt_array2[0] == "1")
                {
                    tt_ccode = tt_array2[1];
                    tt_flag2 = true;
                }
                else
                {
                    MessageBox.Show("该工单没有配置流程,请检查流程位置工单表以及流程表！");
                }

            }


            //第三步检查第一站位与设定的站位是否一致
            Boolean tt_flag3 = false;
            if (tt_flag1 && tt_flag2)
            {
                if (tt_ccode == tt_testcode)
                {
                    tt_flag3 = true;
                }
                else
                {
                    MessageBox.Show("程序设定待测站位与流程的第一站位不匹配，请检查！");
                }
            }


            //第四步 获取下一站位
            Boolean tt_flag4 = false;
            string tt_ncode = "";
            if (tt_flag1 && tt_flag2 && tt_flag3)
            {

                string tt_sql4 = "select count(1),min(z.pxid),0 " +
                                           " from odc_tasks t,odc_routing z  " +
                                           " where t.gyid=z.pid  and t.taskscode='" + tt_task + "' " +
                                           " and z.lcbz in( select (lcbz+1) lcbz " +
                                                            "from odc_tasks a,odc_routing b " +
                                                            "where b.pid=a.gyid and b.pxid='" + tt_ccode + "' " +
                                                            " and a.taskscode='" + tt_task + "') ";

                string[] tt_array4 = new string[3];
                tt_array4 = Dataset1.GetDatasetArray(tt_sql4, tt_conn);
                if (tt_array4[0] == "1")
                {
                    tt_flag4 = true;
                    tt_ncode = tt_array4[1];
                }
                else
                {
                    MessageBox.Show("该工单流程配置异常,有前站位没有后站位，请检查流程位置工单表以及流程表!");
                }




            }


            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
            {
                tt_flag = true;
                this.label35.Text = tt_ccode;
                this.label36.Text = tt_ncode;
            }




            return tt_flag;
        }



        //工单检查设定物料编码检查
        private string setMetrialCheck(string tt_area, string tt_product, string tt_telecustomer )
        {
            string tt_setmetrial = "";
            string tt_sql = "select count(1),min(product_code),0 from odc_fhspec " +
                      "where aear = '" + tt_area + "' and product_name = '" + tt_product + "' and operator = '" + tt_telecustomer + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            if (tt_array[0] == "1")
            {
                tt_setmetrial = tt_array[1];
            }
            else
            {
                MessageBox.Show("没有找到设定物料编码");
            }

            return tt_setmetrial;
        }


       
        //获取运营商
        private string getTelecomOperator(string tt_peoductname)
        {
            string tt_teleplan = "0";

            string tt_sql = "select count(1),min(Fdesc),0  from odc_dypowertype where Ftype = '" + tt_peoductname + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            if (tt_array[0] == "1")
            {
                tt_teleplan = tt_array[1];
            }
            else
            {
                MessageBox.Show("没有找打产品型号" + tt_peoductname + "，对应的供应商，请确认产品型号设置表");
            }


            return tt_teleplan;
        }


       #endregion
        

        #region 6、列表操作
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
        private void PutListViewData(string tt_pcba, string tt_bosa, string tt_mac, string tt_gpsn, string tt_barcode,string bosaith,string bosavbr)
        {
            int i = this.listView1.Items.Count + 1;
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { i.ToString(), tt_pcba, tt_bosa, tt_mac, tt_gpsn, tt_barcode, bosaith, bosavbr });
            this.listView1.Items.AddRange(p);
            this.listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
        }

        #endregion

        

       

    }
}
