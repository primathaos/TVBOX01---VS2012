using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text.RegularExpressions;  //正则表达式
using FastReport;
using FastReport.Barcode;

namespace TVBOX01
{
    public partial class Form24_ama : Form
    {
        public Form24_ama()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_conn;
        static int tt_yield = 0;
        static int tt_reprinttime = 0; //重打次数
        static string tt_code = "0000";
        static string tt_pcname = System.Net.Dns.GetHostName();
        static string tt_path = "";
        static string tt_md5 = "";
        static string tt_gyid2 = "";
        int tt_idnum1 = 0;
        //全流程检验
        static string tt_allprocesses = null;
        static string tt_partprocesses = null;
        static DataSet tt_routdataset = null;
        static DataSet tt_allroutdataset = null;
        //生产节拍
        DateTime tt_productstarttime = DateTime.Now; //开始时间
        DateTime tt_productprimtime; //上一次时间
        //标签微调
        static float tt_top = 0; //上下偏移量
        static float tt_left = 0; //左右偏移量

        //重打限制标识
        string tt_reprintmark = "1";
        //重打限数
        int tt_reprintchang = 0;
        //重打计时
        DateTime tt_reprintstattime;
        DateTime tt_reprintendtime;


        //本机MAC
        static string tt_computermac = "";
		
        //加载
        private void Form24_ama_Load(object sender, EventArgs e)
        {
            //FastReport环境变量设置（打印时不提示 "正在准备../正在打印..",一个程序只需设定一次，故一般写在程序入口）
            (new FastReport.EnvironmentSettings()).ReportSettings.ShowProgress = false;

            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";
            this.toolStripStatusLabel6.Text = tt_productstarttime.ToString();
            this.toolStripStatusLabel10.Text = tt_reprinttime.ToString();

            //初始不显示身份验证栏
            this.groupBox23.Visible = false;

            //初始不显示微调栏
            this.groupBox22.Visible = false;

            //隐藏线长调试按钮
            this.button20.Visible = false;

            //员工账号分离
            if (str.Contains("FH008"))
            {
                this.button2.Visible = false;
                this.button3.Visible = false;
                this.tabPage6.Parent = null;
                this.button20.Visible = true;
            }

            //页面信息清理
            ClearLabelInfo_Yield();
            ClearLabelInfo_Takscode();
            ClearLabelInfo_Code();
            ClearLabelInfo_Macuse();
            ClearLabelInfo_Barcode();
            ClearLabelInfo_Datacheck();
            ClearLabelInfo_Textbox();

            //扫描框隐藏
            this.textBox2.Visible = false;
            this.textBox3.Visible = false;

            //列表设置
            pudListviewInitial();

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



        #region 2、清除事件

        //清除工单信息
        private void ClearLabelInfo_Takscode()
        {
            
            this.label27.Text = null;
            this.label28.Text = null;
            this.label29.Text = null;
            this.label30.Text = null;
            this.label31.Text = null;
            this.label32.Text = null;
            this.label33.Text = null;
            this.label34.Text = null;
            this.label49.Text = null;
            this.label59.Text = null;
            this.label61.Text = null;
            this.label73.Text = null;
            this.label87.Text = null;
            this.label88.Text = null;
            this.label89.Text = null;
            this.label90.Text = null;
            this.label92.Text = null;
            this.label97.Text = null;
            this.label98.Text = null;
            this.label100.Text = null;
        }


        //流程信息
        private void ClearLabelInfo_Code()
        {
            this.label76.Text = null;
            this.label77.Text = null;
            this.label79.Text = null;
            this.label85.Text = null;
            this.label65.Text = null;
            this.label66.Text = null;
        }


        //条码信息清除
        private void ClearLabelInfo_Barcode()
        {
            //条码信息
            this.label51.Text = null;
            this.label53.Text = null;
            this.label54.Text = null;
            this.label55.Text = null;
            this.label56.Text = null;
            this.label80.Text = null;
            this.label102.Text = null;
            this.label103.Text = null;

        }

        //数据检验清除
        private void ClearLabelInfo_Datacheck()
        {
            this.label2.Text = null;
            this.label40.Text = null;
            this.label42.Text = null;
            this.label44.Text = null;
        }


        //剩余MAC信息清除
        private void ClearLabelInfo_Macuse()
        {
            this.label15.Text = null;
            this.label16.Text = null;
            this.label17.Text = null;
        }

        //生产节拍数据初始化
        private void ClearLabelInfo_Yield()
        {
            this.label7.Text = tt_yield.ToString();
            this.label10.Text = null;
            this.label8.Text = null;
            this.label9.Text = null;
        }

        //提示信息清除
        private void ClearLabelInfo_Textbox()
        {
            //提示信息
            this.label47.Text = null;
            this.textBox2.Text = null;
            this.textBox3.Text = null;

        }

        //扫描前数据初始化
        private void ScanDataInitial()
        {
            
            //提示信息
            this.label47.Text = null;

            //流程信息清除
            this.label65.Text = null;
            this.label85.Text = null;

            //表格
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

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
            this.label47.Text = tt_lableinfo;
        }

        //获取验证铭牌路径
        private Boolean getPathIstrue(string tt_file)
        {
            Boolean tt_flag = false;
            if (File.Exists(@tt_file))
            //if (Directory.Exists(@tt_file))
            {
                tt_flag = true;
            }
            else
            {
                tt_flag = false;
            }


            return tt_flag;
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


        //--生产节拍
        private void getProductRhythm(string tt_input)
        {
            if (tt_input =="1")  tt_yield = tt_yield + 1;  //输入为1就加1

            DateTime tt_productendtime = DateTime.Now;  //当前时间

            //计算时间差
            TimeSpan tt_diff;
            if (tt_yield == 1 || tt_yield == 0)
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
            this.label8.Text = tt_time;               //生产时间
            this.label9.Text = tt_avgtime.ToString();  //平均节拍
            this.label10.Text = tt_differtime2;        //实时节拍

        }


        //站位判断
        private bool CheckCodeStation(string tt_prientcode, string tt_checkcode)
        {
            Boolean tt_flag = false;

            int tt_pricode = 0;
            int tt_passcde = 0;
            Boolean tt_flag1 = false;

            if (tt_prientcode.Equals("") || tt_checkcode.Equals(""))
            {
                MessageBox.Show("当前站位与检测站位有空值情况，请检查！");
            }
            else
            {
                try
                {
                    tt_pricode = int.Parse(tt_prientcode);
                    tt_passcde = int.Parse(tt_checkcode);
                    tt_flag1 = true;
                }
                catch
                {
                    MessageBox.Show("字符串站位转换为数字失败，请检查站位情况！");
                }
            }


            if (tt_flag1)
            {
                if (tt_pricode > tt_passcde)
                {
                    tt_flag = true;
                }
            }

            return tt_flag;
        }


        /// <summary>  
        /// 获取文件的MD5码  
        /// </summary>  
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>  
        /// <returns></returns>  
        public string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }



        //字符串遍历
        private bool getStrChar(string tt_longstr, string tt_chartype)
        {
            Boolean tt_flag = false;

            String tt_chars = "";

            for (int i = 0; i < tt_longstr.Length; i++)
            {
                tt_chars = tt_longstr.Substring(i, 1);
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
                if (char.IsDigit(tt_char, 0))
                {
                    tt_flag = true;
                    setRichtexBox(tt_char + ":为数字不用大小写判断,Goon");
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
                        PutLableInfor("密码：" + tt_char + ":为大写，判断不正确");
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



        #region 4、列表操作
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
        private void PutListViewData(string tt_pcba, string tt_bosa, string tt_mac, string tt_gpsn, string tt_barcode)
        {
            int i = this.listView1.Items.Count + 1;
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { i.ToString(), tt_pcba, tt_bosa, tt_mac, tt_gpsn, tt_barcode });
            this.listView1.Items.AddRange(p);
            this.listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
        }


        //列表设置
        private void pudListviewInitial()
        {
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
            this.listView1.Columns.Add("OLD_MAC", 110);
            this.listView1.Columns.Add("NEW_MAC", 110);
            this.listView1.Columns.Add("GPSN", 100);
            this.listView1.Columns.Add("BARCODE", 200);
        }

        #endregion



        #region 5、锁定事件
        //工单锁定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                if (str.Contains("FH108"))
                {
                    this.button3.Visible = true;
                    this.tabPage6.Parent = tabControl2;
                    //获取调试开始时间
                    tt_reprintstattime = DateTime.Now;
                }

                Boolean tt_flag = getChoiceTaskcode();

                if (tt_flag)
                {
                    MessageBox.Show("---OK---,这是延迟制造模式，注意两个工单不要选错！左边工单1是真MAC工单，右边工单2是虚拟MAC工单");
                    this.textBox1.Enabled = false;
                    this.textBox9.Enabled = false;
                    GetMacUseNumber();
                    this.textBox2.Visible = true;
                    this.textBox3.Visible = true;

                }
                else
                {
                    MessageBox.Show("工单选择失败");

                    ClearLabelInfo_Takscode();
                    ClearLabelInfo_Code();
                    ClearLabelInfo_Macuse();
                    ClearLabelInfo_Barcode();
                    ClearLabelInfo_Datacheck();
                    ClearLabelInfo_Textbox();
                }

            }
            else
            {
                this.textBox1.Enabled = true;
                this.textBox9.Enabled = true;

                this.textBox2.Visible = false;
                this.textBox3.Visible = false;

                this.checkBox1.Checked = false;

                this.comboBox2.Text = "";
                this.textBox27.Text = "";
                this.textBox28.Text = "";
                this.comboBox2.Enabled = true;
                this.textBox27.Enabled = true;
                this.textBox28.Enabled = true;
                this.groupBox22.Visible = false;
                this.groupBox23.Visible = false;
                this.groupBox6.Visible = true;
                this.groupBox12.Visible = true;
                this.dataGridView1.Visible = true;
                this.button3.Visible = false;
                this.tabPage6.Parent = null;
                this.tabPage5.Parent = tabControl2;

                ClearLabelInfo_Takscode();
                ClearLabelInfo_Code();
                ClearLabelInfo_Macuse();
                ClearLabelInfo_Barcode();
                ClearLabelInfo_Datacheck();
                ClearLabelInfo_Textbox();
            }
        }

        //单板位数锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.textBox4.Enabled = false;
                this.textBox5.Enabled = false;
            }
            else
            {
                this.textBox4.Enabled = true;
                this.textBox5.Enabled = true;
            }
        }

        //MAC位数锁定
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked)
            {
                this.textBox6.Enabled = false;
                this.textBox7.Enabled = false;
            }
            else
            {
                this.textBox6.Enabled = true;
                this.textBox7.Enabled = true;
            }
        }

        #endregion



        #region 6、工单选择及站位检查

        //工单选择
        private bool getChoiceTaskcode()
        {
            Boolean tt_flag = false;
            string tt_task1 = this.textBox1.Text.Trim();
            string tt_task2 = this.textBox9.Text.Trim();

            string tt_productname = "";

            tt_computermac = Dataset1.GetHostIpName();


            //第一步 主工单检查
            #region
            bool tt_flag1 = false;

            string tt_sql1 = "select  tasksquantity,product_name,areacode,fec,convert(varchar, taskdate, 102) fdate,customer,flhratio,Gyid,Tasktype,Vendorid,Teamgroupid,pon_name,id " +
                                 "from odc_tasks where taskstate = 2 and taskscode = '" + tt_task1 + "' ";
            DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                tt_flag1 = true;
                this.label27.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                tt_productname = ds1.Tables[0].Rows[0].ItemArray[1].ToString(); //产品名称
                this.label30.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区
                this.label31.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString(); //EC编码
                this.label28.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //生产日期


                this.label79.Text = ds1.Tables[0].Rows[0].ItemArray[7].ToString();  //流程配置
                this.label49.Text = ds1.Tables[0].Rows[0].ItemArray[8].ToString();  //物料编码
                this.label61.Text = ds1.Tables[0].Rows[0].ItemArray[9].ToString();  //COMMID
                this.label88.Text = ds1.Tables[0].Rows[0].ItemArray[10].ToString();  //地区代码
                this.label89.Text = ds1.Tables[0].Rows[0].ItemArray[11].ToString();  //PON类型

                string tt_idnum = ds1.Tables[0].Rows[0].ItemArray[12].ToString();//制造单ID

                tt_idnum1 = Convert.ToInt32(tt_idnum);

                if (tt_productname == "HG6201G" || tt_productname == "HG6201GW" || tt_productname == "HG6201GS")
                {
                    this.label29.Text = "HG6201M";
                }
                else
                {
                    this.label29.Text = tt_productname;
                }

            }
            else
            {
                MessageBox.Show(this.textBox1.Text+"没有查询此工单，或此工单没有审核，请确认！");
            }
            #endregion
            

            //第二步 判断两个工单是否一致
            #region
            bool tt_flag2 = false;
            if( tt_flag1)
            {
                if(tt_task1 == tt_task2)
                {
                    MessageBox.Show("两个工单一致");
                }
                else
                {
                    tt_flag2 = true;
                }
            }
            #endregion


            //第三步判断工单2是否存在
            #region
            bool tt_flag3 = false;
            string tt_prodeucname2 = "";
            if( tt_flag2)
            {
                string tt_sql3 = "select count(1),min(product_name),min(gyid) from odc_tasks " +
                                  "where taskscode = '"+tt_task2+"' and onumodel = '延迟制造' ";

                string[] tt_array3 = new string[3];
                tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                if (tt_array3[0] == "1")
                {
                    tt_flag3 = true;
                    tt_prodeucname2 = tt_array3[1];
                    tt_gyid2 = tt_array3[2];
                }
                else
                {
                    MessageBox.Show("没有找到库存工单："+tt_task2+",请确认工单是否选错，或者这个工单不是延迟制造的库存工单");

                }

            }
            #endregion


            //第四步 判断产品型号是否一致
            #region
            bool tt_flag4 = false;
            if( tt_flag3)
            {
                if (this.label29.Text == tt_prodeucname2)
                {
                    tt_flag4 = true;
                }
                else
                {
                    MessageBox.Show("两个工单的产品型号不一致：分别为" + this.label29.Text + "," + tt_prodeucname2);
                }
            }
            #endregion
            

            //第五步 查找库存工单的最后站位
            #region
            bool tt_flag5 = false;
            string tt_lastcodeserials = "";
            if (tt_flag4)
            {
                string tt_sql5 = "select count(1),min(PXID),min(LCBZ) from odc_routing " +
                                 " where  spbzlx = 1  and pid = " + tt_gyid2;

                string[] tt_array5 = new string[3];
                tt_array5 = Dataset1.GetDatasetArray(tt_sql5, tt_conn);
                if (tt_array5[0] == "1")
                {
                    tt_flag5 = true;
                    this.label73.Text = tt_array5[1];
                    tt_lastcodeserials = tt_array5[2];
                }
                else
                {
                    MessageBox.Show("没有找到库存工单" + tt_task2+",的最后站位");

                }

            }
            #endregion


            //第六步 工单1流程检查
            #region
            bool tt_flag6 = false;
            string tt_gyid1 = this.label79.Text;
            if(tt_flag5)
            {
                if (!tt_gyid1.Equals(""))
                {
                    bool tt_flag61 = GetNextCode(this.textBox1.Text, str);
                    if (tt_flag61)
                    {
                        tt_flag6 = true;
                    }
                }
                else
                {
                    MessageBox.Show("该工单没有配置流程，请检查");
                }

            }
            #endregion


            //第六步 附1 文字变量查询 //10-9 杨浩
            #region
            bool tt_flag16 = false;
            if (tt_flag6)
            {
                if (this.label89.Text == "GPON") 
                {
                    if (tt_idnum1 >= 915 && this.label29.Text == "HG6201T")
                    {
                        this.label100.Text = "S/N";
                    }
                    else
                    {
                        this.label100.Text = "GPON SN";
                    }
                    this.label98.Text = "MAC";      //文字变量02
                    this.label97.Text = "吉比特";    //文字变量03
                    tt_flag16 = true;
                }
                else if (this.label89.Text == "EPON")
                {
                    this.label100.Text = "ONU MAC"; //文字变量01
                    this.label98.Text = "WAN MAC";  //文字变量02
                    this.label97.Text = "以太网";    //文字变量03
                    tt_flag16 = true;
                }
                else
                {
                    MessageBox.Show("文字变量无法匹配，请确认制造单下单信息，或产品是否为PON产品");
                }
            }
            #endregion


            //第六步 附2 电源规格查询 //10-9 杨浩
            #region
            bool tt_flag17 = false;
            if (tt_flag16)
            {
                string tt_sql17 = "select volt,ampere from odc_dypowertype where ftype = '" + this.label29.Text + "' ";
                DataSet ds17 = Dataset1.GetDataSetTwo(tt_sql17, tt_conn);

                if (ds17.Tables.Count > 0 && ds17.Tables[0].Rows.Count > 0)
                {
                    this.label90.Text = ds17.Tables[0].Rows[0].ItemArray[0].ToString(); //电压
                    this.label92.Text = ds17.Tables[0].Rows[0].ItemArray[1].ToString(); //电流
                    tt_flag17 = true;
                }
                else
                {
                    MessageBox.Show("没有电源适配器信息，请确认数据库电源表");
                }
            }
            #endregion


            //第七步 EC表检查
            #region
            bool tt_flag7 = false;
            if (tt_flag17)
            {
                string tt_sql7 = "select  docdesc,Fpath01,Fdata01,Fmd01  from odc_ec where zjbm = '" + this.label31.Text + "' ";

                DataSet ds2 = Dataset1.GetDataSet(tt_sql7, tt_conn);
                if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                {
                    this.label34.Text = ds2.Tables[0].Rows[0].ItemArray[0].ToString(); //EC描述
                    this.label33.Text = ds2.Tables[0].Rows[0].ItemArray[1].ToString(); //模板路径
                    this.label32.Text = ds2.Tables[0].Rows[0].ItemArray[2].ToString(); //数据类型
                    this.label59.Text = ds2.Tables[0].Rows[0].ItemArray[3].ToString(); //MD5码
                    tt_path = Application.StartupPath + ds2.Tables[0].Rows[0].ItemArray[1].ToString();
                    tt_md5 = ds2.Tables[0].Rows[0].ItemArray[3].ToString();
                    tt_flag7 = true;

                }
                else
                {
                    MessageBox.Show("没有找到工单表的EC表配置信息，请确认！");
                }
            }
            #endregion
            

            //第八步 检查模板文件是否存在
            #region
            bool tt_flag8 = false;
            if(tt_flag7)
            {
                tt_flag8 = getPathIstrue(tt_path);
                if (!tt_flag8)
                {
                    MessageBox.Show(" 找不到模板文件：" + tt_path + "，请确认！");
                }
            }
            #endregion


            //第九步 检验MD5码
            //#region
            //bool tt_flag9 = false;
            //if (tt_flag8)
            //{
            //    string tt_md6 = GetMD5HashFromFile(tt_path);

            //    if (tt_md5 == tt_md6)
            //    {
            //        tt_flag9 = true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("系统设定模板MD5码: '" + tt_md5 + "'与你使用模板的MD5码：'" + tt_md6 + "'不一致，请确认！");
            //    }
            //}
            //#endregion


            //第十步 获取运营商
            #region
            Boolean tt_flag10 = false;
            string tt_telecustomer = "";
            if (tt_flag8)
            {
                string tt_product = this.label29.Text;
                tt_telecustomer = getTelecomOperator(tt_product);
                if (tt_telecustomer == "0")
                {
                    MessageBox.Show("运营商获取失败，无法确定是电信还是移动产品");
                }
                else
                {
                    tt_flag10 = true;
                    this.label87.Text = tt_telecustomer;
                }


            }


            #endregion


            //第十一步 物料编码检查
            #region
            bool tt_flag11 = false;
            if(tt_flag10)
            {
                if (this.label49.Text != "")
                {
                    this.label44.Text = SetMetrialCheck(this.label30.Text, this.label29.Text, tt_telecustomer, this.label49.Text);
                    if (this.label44.Text == this.label49.Text)
                    {
                        tt_flag11 = true;
                    }
                    else
                    {
                        MessageBox.Show("该工单物料编码:" + this.label49.Text + ",与设定物料编码:" + this.label44.Text + ",不一致，请确认");
                    }
                }
                else
                {
                    MessageBox.Show("该工单物料编码为空，请检查工单设置！");
                }
            }
            #endregion
            

            //第十二步 获取用户名密码设定
            #region
            bool tt_flag12 = false;
            if(tt_flag11)
            {
                string tt_sql12 = "select username,digits,format from odc_fhuser " +
                                         "where aear = '" + this.label30.Text + "' and  operator = '" + tt_telecustomer + "' ";
                DataSet ds12 = Dataset1.GetDataSetTwo(tt_sql12, tt_conn);
                if (ds12.Tables.Count > 0 && ds12.Tables[0].Rows.Count > 0)
                {
                    tt_flag12 = true;
                    this.label40.Text = ds12.Tables[0].Rows[0].ItemArray[0].ToString(); //用户名
                    this.label42.Text = ds12.Tables[0].Rows[0].ItemArray[1].ToString(); //密码位数
                    this.label2.Text = ds12.Tables[0].Rows[0].ItemArray[2].ToString();  //密码大小写
                }
                else
                {
                    MessageBox.Show("没有找到地区:" + this.label30.Text + "，的用户名及密码设定，请确认！");
                }
            }
            #endregion


            //第十三步 用户密码设定检查
            #region
            bool tt_flag13 = false;
            if(tt_flag12)
            {
                if (this.label40.Text == "" || this.label42.Text == "" || this.label2.Text == "")
                {
                    MessageBox.Show("用户名，或密码设定值为空，请检查数据");
                }
                else
                {
                    tt_flag13 = true;
                }
            }
            #endregion
            

            //第十四步 工单2的最后流程检查
            #region
            bool tt_flag14 = false;
            if(tt_flag13)
            {
                if(this.label73.Text != "")
                {
                    tt_flag14 = true;
                }
                else
                {
                    MessageBox.Show("库存工单的最后流程没有，请检查！");
                }
            }
            #endregion
            

            //第十五步 获取站位流程数据集
            #region
            bool tt_flag15 = false;
            if(tt_flag14)
            {
                string tt_sql15 = "select pxid from odc_routing  where pid = " + tt_gyid2 + "  and LCBZ > 1 and LCBZ < "+tt_lastcodeserials ;
                tt_routdataset = Dataset1.GetDataSetTwo(tt_sql15, tt_conn);
                if (tt_routdataset.Tables.Count > 0 && tt_routdataset.Tables[0].Rows.Count > 0)
                {
                    tt_flag15 = true;
                    tt_allprocesses = Dataset2.getGyidAllProcess(tt_gyid2,tt_conn);
                    tt_partprocesses = Dataset2.getGyidPartProcess(tt_routdataset);
                    tt_allroutdataset = Dataset2.getGyidAllProcessDt(tt_gyid2, tt_conn);
                }
                else
                {
                    MessageBox.Show("没有找到流程:" + tt_gyid2 + "，的流程数据集Dataset，请流程设置！");
                }


            }
            #endregion
            

            //最后判断
            #region
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 &&
                tt_flag10 && tt_flag11 && tt_flag12 && tt_flag13 && tt_flag14 && tt_flag15 &&
                tt_flag16 && tt_flag17)
            {
                tt_flag = true;
            }
            #endregion

            return tt_flag;
        }


        //获取虚拟工单流程
        private string getGyidAllProcess(string tt_gyid)
        {
            string tt_gyidprocess = "虚拟单板工单流程没有找到";
            string tt_sql = "select count(1),min(process),0 from odc_process where id = " + tt_gyid;
            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            if (tt_array[0] == "1") tt_gyidprocess = tt_array[1];
            return tt_gyidprocess;
        }


        //工单待测站位检查
        private string getPcbaNowCode(DataSet tt_dt)
        {
            string tt_returnncode = "0";
           
            if (tt_dt.Tables.Count > 0 && tt_dt.Tables[0].Rows.Count > 0)
            {
                string tt_ncode = "";
                string tt_napplytype = "";
                string tt_nowcode = "";
                int tt_napplycount = 0;
                //以下数据遍历
                for (int i = 0;i< tt_dt.Tables[0].Rows.Count; i++ )
                {
                    tt_ncode = tt_dt.Tables[0].Rows[i][0].ToString();
                    tt_napplytype = tt_dt.Tables[0].Rows[i][1].ToString();

                    if (tt_napplytype.Equals(""))
                    {
                        tt_napplycount++;
                        tt_nowcode = tt_ncode;
                    }
                }
                //以下返回值判断
                if (tt_napplycount == 0) tt_returnncode = "0";
                if (tt_napplycount == 1) tt_returnncode = tt_nowcode;
                if (tt_napplycount >  1) tt_returnncode = "2";
            }


            return tt_returnncode;
        }


        //虚拟工单设定流程每个站位检查
        private string getPcbaAllCheck(DataSet tt_routdt, DataSet tt_pcbadt)
        {
            string tt_outmessage = "0";  //数据集有问题

            if (tt_pcbadt.Tables.Count > 0 && tt_pcbadt.Tables[0].Rows.Count > 0 && tt_routdt.Tables.Count > 0 && tt_routdt.Tables[0].Rows.Count > 0)
            {
                string tt_routingncode = "";
                string tt_checkinfo = "0";
                //以下数据遍历
                for (int i = 0; i < tt_routdt.Tables[0].Rows.Count; i++)
                {
                    tt_routingncode = tt_routdt.Tables[0].Rows[i][0].ToString();
                    //没有找到就返回站位，找到返回1
                    tt_checkinfo = getPcbaSinglCheck(tt_routingncode, tt_pcbadt);
                    if (tt_checkinfo == tt_routingncode)      break;
                }


                if (tt_checkinfo == "0")
                {
                    tt_outmessage = "1";
                }
                else
                {
                    tt_outmessage = tt_checkinfo;
                }


            }
            return tt_outmessage;
        }

        //虚拟MAC单板数据集的循环检查
        private string getPcbaSinglCheck(string tt_checkcode, DataSet tt_dt)
        {
            string tt_checkinfo = tt_checkcode;  //没有找到就返回站位，找到返回1

            if (tt_dt.Tables.Count > 0 && tt_dt.Tables[0].Rows.Count > 0)
            {
                string tt_ncode = "";
                string tt_napplytype = "";
                //以下数据遍历
                for (int i = 0; i < tt_dt.Tables[0].Rows.Count; i++)
                {
                    tt_ncode = tt_dt.Tables[0].Rows[i][0].ToString();
                    tt_napplytype = tt_dt.Tables[0].Rows[i][1].ToString();
                    if ((tt_napplytype.Equals("1") || tt_napplytype.Equals("")) && tt_ncode == tt_checkcode)
                    {
                        tt_checkinfo = "1";
                        break;
                    }
                }

            }

            return tt_checkinfo;
        }


        #endregion



        #region 7、数据功能

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

        //刷新站位
        private void CheckStation2(string tt_mac)
        {
            string tt_sql = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime,fremark " +
                            "from ODC_ROUTINGTASKLIST    where pcba_pn = '" + tt_mac + "' order by id desc";

            DataSet ds1 = Dataset1.GetDataSet(tt_sql, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView1.DataSource = ds1;
                dataGridView1.DataMember = "Table";
                this.label85.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //当前站位


                //获取流程的顺序值
                string tt_newcode = ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                string tt_process = this.label79.Text;
                this.label65.Text = GetCodeRoutNum(tt_newcode, tt_process);

            }

        }

        //获取站位routing顺序号
        private string GetCodeRoutNum(string tt_code, string tt_process)
        {
            string tt_routnum = "";

            string tt_sql = "select count(1),min(lcbz),0 from odc_routing " +
                            "where pid = " + tt_process + "  and pxid = " + tt_code;


            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);

            if (tt_array[0] == "1")
            {
                tt_routnum = tt_array[1];
            }
            else
            {
                MessageBox.Show("当前流程：" + tt_process + "，当前站位：" + tt_code + ",在routing表中没有找到序号，请确认");
            }


            return tt_routnum;
        }

        //流程检查，获取下一流程
        private bool GetNextCode_1(string tt_task, string tt_username)
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


            //第二步获取当前站位
            Boolean tt_flag2 = false;
            string tt_firstcode = "";
            string tt_ccode = "";
            string tt_ccodenumber = "";
            string tt_process = "";
            if (tt_flag1)
            {
                string tt_sql2 = "select count(1),min(b.PXID) ,min(a.GYID) " +
                                     "from odc_tasks a,odc_routing b " +
                                     "WHERE a.GYID=b.PID AND b.LCBZ=1 AND a.TASKSCODE='" + tt_task + "' ";
                string[] tt_array2 = new string[3];
                tt_array2 = Dataset1.GetDatasetArray(tt_sql2, tt_conn);
                if (tt_array2[0] == "1")
                {
                    tt_firstcode = tt_array2[1];
                    tt_ccode = tt_testcode;
                    tt_code = tt_ccode;

                    tt_process = tt_array2[2];
                    tt_ccodenumber = GetCodeRoutNum(tt_ccode, tt_process); //获取站位顺序

                    //this.label13.Text = "站位:" + tt_code;
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
                this.label76.Text = tt_ccode;
                this.label77.Text = tt_ncode;
                this.label66.Text = tt_ccodenumber;
            }




            return tt_flag;
        }

        //工单检查设定物料编码检查
        private string setMetrialCheck1(string tt_area, string tt_product)
        {
            string tt_setmetrial = "";
            string tt_sql = "select count(1),min(product_code),0 from odc_fhspec " +
                            "where aear = '" + tt_area + "' and product_name = '" + tt_product + "' ";

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

        //工单检查设定物料编码检查
        private string SetMetrialCheck(string tt_area, string tt_product, string tt_telecustomer, string tt_tasktype)
        {
            string tt_setmetrial = "";
            string tt_sql = "select count(1),min(product_code),0 from odc_fhspec " +
                      "where aear = '" + tt_area + "' and product_name = '" + tt_product + "' and operator = '" + tt_telecustomer + "' and product_code = '" + tt_tasktype + "'";

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

        //检查MAC或单板工单
        private string getSnRealTask(string tt_datatype, string tt_sn)
        {
            string tt_taskcode = "";
            string tt_sql = "Select 1,'不确定',1 ";
            string tt_sql1 = "select count(1),min(taskscode),0 from odc_alllable where pcbasn = '"+tt_sn+"' ";
            string tt_sql2 = "select count(1),min(taskscode),0 from odc_alllable where maclable = '" + tt_sn + "' ";
            if (tt_datatype == "1") tt_sql = tt_sql1;  //单板
            if (tt_datatype == "2") tt_sql = tt_sql2;  //MAC

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            if (tt_array[0] == "1")
            {
                tt_taskcode = tt_array[1];
            }
            else
            {
                tt_taskcode = "没找到";
            }

            return tt_taskcode;
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

        //列表显示工单
        private void setTaskcodeList()
        {
            string tt_sql = "select T1.taskscode+'    '+T1.areacode+T2.Fdesc+'   '+convert(varchar(20),tasksquantity)   N01 " +
                             "from odc_tasks T1 " +
                             "inner join odc_dypowertype T2 " +
                              "on T1.product_name = T2.Ftype " +
                              "order by T1.id desc  ";

            DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql, tt_conn);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                string tt_taskdesc = "";
                //以下数据遍历
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    tt_taskdesc = ds1.Tables[0].Rows[i][0].ToString();
                    setRichtexBox(tt_taskdesc);
                }
            }
        }

        //获取工号
        private string GetUserNumber(string tt_username)
        {
            string tt_UserNumber = "123456";

            string tt_sql = "select count(1),min(fusernum),min(fremark) " +
                            "from odc_fhpartitionpass where Fusername = '" + tt_username + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArrayTwo(tt_sql, tt_conn);

            if (tt_array[0] == "1")
            {
                tt_UserNumber = tt_array[1];
            }
            else
            {
                MessageBox.Show("网络连接失败，或没有" + tt_username + "此账号，请确认");
            }

            return tt_UserNumber;
        }

        //获取密码
        private string GetUserPassword(string tt_username)
        {
            string tt_password = "";

            string tt_sql = "select count(1),min(fpassword),min(fremark) " +
                            "from odc_fhpartitionpass where Fusername = '" + tt_username + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArrayTwo(tt_sql, tt_conn);

            if (tt_array[0] == "1")
            {
                tt_password = tt_array[1];
            }
            else
            {
                MessageBox.Show("网络连接失败，或没有" + tt_username + "此账号，请确认");
            }

            return tt_password;
        }

        //获取生产序列号
        private string Gethostlable(string tt_maclable)
        {
            string tt_hostlable = "";

            string tt_sql = "select count(1), min(hostlable), min(maclable) " +
                            "from odc_alllable where maclable = '" + tt_maclable + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArrayTwo(tt_sql, tt_conn);

            if (tt_array[0] == "1")
            {
                tt_hostlable = tt_array[1];
            }
            else
            {
                MessageBox.Show("网络连接失败，或此MAC" + tt_maclable + "未关联，请确认");
            }

            return tt_hostlable;
        }

        //打印数记录
        private void SetPrintRecord(string tt_task, string tt_mac, string tt_host, string tt_local, string tt_user ,string tt_computername, string tt_remark)
        {
            string tt_insertsql = "insert into odc_lablereprint (Ftaskcode,Fmaclable,Fhostlable,Flocal,Fname,Fdate,Fcomputername,Fremark) " +
                       "values('" + tt_task + "','" + tt_mac + "','" + tt_host + "','" + tt_local + "','" + tt_user + "',getdate(),'" + tt_computername + "','" + tt_remark + "') ";

            int tt_intcount = Dataset1.ExecCommand(tt_insertsql, tt_conn);

            if (tt_intcount > 0)
            {
                tt_reprinttime++;
            }
        }

        #endregion



        #region 8、SN条码查询

        //数据查询确定
        private void button4_Click(object sender, EventArgs e)
        {
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;


            string tt_task = "";
            string tt_pcba = "";
            string tt_mac = "";
            Boolean tt_flag = false;

            string tt_sn1 = this.textBox11.Text.Trim();
            string tt_sn = tt_sn1.Replace(":", "");

            string tt_sql1 = "select hprintman 总工单,taskscode 子工单, pcbasn 单板号,hostlable 主机条码,maclable MAC, " +
                             "boxlable 生产序列号,Bosasn BOSA, shelllable GPSN, Smtaskscode 串号, Dystlable 电源号, " +
                             "sprinttime 关联时间 " +

                            "from odc_alllable " +
                            "where pcbasn = '" + tt_sn + "' or hostlable = '" + tt_sn + "' or  maclable = '" + tt_sn + "' ";

            DataSet ds1 = Dataset1.GetDataSet(tt_sql1, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView3.DataSource = ds1;
                dataGridView3.DataMember = "Table";

                tt_task = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //工单号
                tt_pcba = ds1.Tables[0].Rows[0].ItemArray[2].ToString();     //单板条码
                tt_mac = ds1.Tables[0].Rows[0].ItemArray[4].ToString();      //MAC
                tt_flag = true;

            }
            else
            {
                MessageBox.Show("sorry,没有查询到数据");
            }

            //站位查询
            if (tt_flag)
            {
                string tt_sql2 = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime 进站时间, enddate 出站时间, fremark 备注  " +
                            "from ODC_ROUTINGTASKLIST    where pcba_pn = '" + tt_mac + "' order by id desc";

                DataSet ds2 = Dataset1.GetDataSet(tt_sql2, tt_conn);

                if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                {
                    dataGridView4.DataSource = ds2;
                    dataGridView4.DataMember = "Table";
                }


            }


            //箱号查询
            if (tt_flag)
            {
                string tt_sql3 = "select taskcode 工单号,pasn 单板号, pagesn 箱号, polletsn 栈板号,pagetime 装箱时间 " +
                                 "from odc_package " +
                                 "where pasn = '" + tt_pcba + "' and taskcode = '" + tt_task + "' ";

                DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);

                if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                {
                    dataGridView5.DataSource = ds3;
                    dataGridView5.DataMember = "Table";
                }

            }


        }

        //数据查询重置
        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox11.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;
        }
        #endregion



        #region 9、获取文件MD5码
        //选择文件
        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            fileDialog.InitialDirectory = "c:\\";//获取打开选择框的初始目录;
            fileDialog.ShowDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                // MessageBox.Show("已选择文件:" + file, "选择文件提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.textBox12.Text = file;
            }
        }

        //计算MD5码
        private void button7_Click(object sender, EventArgs e)
        {
            string tt_fliename = this.textBox12.Text;
            string tt_md5 = GetMD5HashFromFile(tt_fliename);
            this.textBox13.Text = tt_md5;
        }


        //重置
        private void button8_Click(object sender, EventArgs e)
        {
            this.textBox12.Text = null;
            this.textBox13.Text = null;
        }
        #endregion



        #region 10、工单查询
        //确定
        private void button10_Click(object sender, EventArgs e)
        {
            this.dataGridView6.DataSource = null;

            string tt_task = this.textBox8.Text.Trim();


            string tt_sql1 = "select hprintman 总工单,taskscode 子工单, pcbasn 单板号,hostlable 主机条码,maclable MAC, " +
                             "boxlable 生产序列号,Bosasn BOSA, shelllable GPSN, Smtaskscode 串号, Dystlable 电源号, " +
                             "sprinttime 关联时间 " +

                            "from odc_alllable " +
                            "where taskscode = '" + tt_task + "'";

            DataSet ds1 = Dataset1.GetDataSet(tt_sql1, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView6.DataSource = ds1;
                dataGridView6.DataMember = "Table";

            }
            else
            {
                MessageBox.Show("sorry,没有查询到数据");
            }
        }

        //重置
        private void button9_Click(object sender, EventArgs e)
        {
            this.textBox8.Text = null;
            this.dataGridView6.DataSource = null;
        }
       
        //显示行号
        private void dataGridView6_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SolidBrush brushOne = new SolidBrush(Color.Red);
            e.Graphics.DrawString(Convert.ToString(e.RowIndex + 1, System.Globalization.CultureInfo.CurrentUICulture), e.InheritedRowStyle.Font, brushOne, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
        }
        #endregion



        #region 11、功能按钮
        //重置
        private void button1_Click(object sender, EventArgs e)
        {
            ScanDataInitial();
            ClearLabelInfo_Barcode();
            CleatListView();
            this.textBox2.Text = null;
            this.textBox3.Text = null;
            textBox2.Focus();
            textBox2.SelectAll();
            setTaskcodeList();
        }

        //扫描款页签选择
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //单板扫描
            if (tabControl2.SelectedTab == tabPage5)
            {
                ScanDataInitial();
                ClearLabelInfo_Barcode();
                this.textBox3.Text = null;
                textBox3.Focus();
                textBox3.SelectAll();
            }

            //标签重打
            if (tabControl2.SelectedTab == tabPage6)
            {
                ScanDataInitial();
                ClearLabelInfo_Barcode();
                this.textBox2.Text = null;
                textBox2.Focus();
                textBox2.SelectAll();
            }
        }

        //预览
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {

                string tt_prientcode = this.label65.Text;
                string tt_checkcode = this.label66.Text;

                Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                if (tt_flag)
                {
                    GetParaDataPrint(2);  //预览
                }
                else
                {
                    MessageBox.Show("当前站位或序号：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",才能重打标签");
                }

            }
            else
            {
                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再预览模板");
            }

            textBox3.Focus();
            textBox3.SelectAll();
        }

        //打印
        private void button3_Click(object sender, EventArgs e)
        {
            tt_reprintendtime = DateTime.Now;

            TimeSpan tt_diffre;

            tt_diffre = tt_reprintendtime - tt_reprintstattime;

            if (tt_diffre.Minutes > 5)
            {
                this.checkBox1.Checked = false;
                MessageBox.Show("5分钟内未进行任何打印动作，退出打印模式");
                return;
            }

            if (this.dataGridView2.RowCount > 0)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                string tt_info = "";
                if (str.Contains("FH008"))
                {
                    tt_info = "，包装产品会被退回check站位";
                }
                DialogResult dr = MessageBox.Show("确定要重打铭牌吗，打印信息被记录" + tt_info, "铭牌重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label65.Text;
                    string tt_checkcode = this.label66.Text;
                    string tt_recordmac = this.textBox3.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                    DataSet tt_dataset1 = Dataset2.getMacAllCodeInfo(tt_recordmac, tt_conn);
                    string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset1);

                    if (tt_flag && tt_nowcode != "9990")
                    {
                        Reprint form1 = new Reprint();
                        form1.StartPosition = FormStartPosition.CenterScreen;
                        form1.ShowDialog();

                        string tt_remark = Dataset1.Context.ContextData["Key1"].ToString();

                        GetParaDataPrint(1);  //打印
                        string tt_taskscode = this.textBox1.Text.Trim().ToUpper();
                        string tt_host = Gethostlable(tt_recordmac);
                        string tt_local = "铭牌标签";
                        string tt_username = "";
                        if (str.Contains("FH008"))
                        {
                            tt_username = this.comboBox2.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }

                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);

                        if (str.Contains("FH008"))
                        {
                            if (int.Parse(this.label85.Text) >= 3000)
                            {
                                string tt_gyid = this.label79.Text;
                                string tt_ccode = this.label85.Text;
                                string tt_ncode = "2230";
                                bool tt_flag1 = Dataset1.FhUnPassStationI(tt_taskscode, tt_username, tt_recordmac, tt_gyid, tt_ccode, tt_ncode, tt_conn);
                                if (tt_flag1)
                                {
                                    setRichtexBox("重打完成，产品属于包装产品，已退回check站位,ok");
                                    PutLableInfor("重打完成，产品属于包装产品，已退回check站位");
                                }
                                else
                                {
                                    setRichtexBox("流程异常，产品未跳转也无法正常流线，请联系工程,NG");
                                    PutLableInfor("流程异常，产品未跳转也无法正常流线，请联系工程");
                                }
                            }
                        }

                        if (tt_reprintmark == "0")
                        {
                            tt_reprintchang++;

                            if (tt_reprintchang >= 5)
                            {
                                this.checkBox1.Checked = false;
                                MessageBox.Show("非认证打印电脑，已达到打印上限，退出打印模式");
                                tt_reprintchang = 0;
                            }
                            else
                            {
                                MessageBox.Show("非认证打印电脑，已打印" + tt_reprintchang + "次，本次打印次数剩余" + (5 - tt_reprintchang) + "次");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("当前站位或序号：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + "，或装箱已打散，才能重打标签");
                    }
                }
                else
                {

                }

            }
            else
            {
                PutLableInfor("参数表数据为空，不能打印！");
            }

            textBox3.Focus();
            textBox3.SelectAll();
            tt_reprintstattime = DateTime.Now;
        }

        //线长调试模式
        private void button20_Click(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
            {
                //打印限制标识状态刷新
                tt_reprintmark = Dataset1.GetComputerMAC(tt_conn);

                //获取线长名单
                string tt_sql1 = "select fusername from odc_fhpartitionpass where fdepart in ('生产','0') and fpermission in ('1','0') order by id";
                DataSet ds1 = Dataset1.GetDataSet(tt_sql1, tt_conn);
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.comboBox1.Text = "0.3";
                    this.comboBox2.Text = "下拉选择";
                    comboBox2.DataSource = ds1.Tables[0];
                    comboBox2.DisplayMember = "fusername";
                    this.groupBox22.Visible = true;
                    this.groupBox6.Visible = false;
                    this.groupBox12.Visible = false;
                    this.dataGridView1.Visible = false;
                    this.textBox27.Text = "";
                    this.textBox28.Text = "";
                    this.comboBox2.Enabled = true;
                    this.textBox27.Enabled = true;
                    this.textBox28.Enabled = true;
                    this.groupBox23.Visible = false;
                    this.button3.Visible = false;
                    this.tabPage6.Parent = null;
                    this.tabPage5.Parent = tabControl2;
                    this.textBox3.Enabled = true;
                    this.textBox3.Text = "";
                }
                else
                {
                    MessageBox.Show("获取不到线长名单，请检查网络！");
                }
            }
            else
            {
                MessageBox.Show("请先输入工单并锁定！");
            }
        }

        //输入限制
        private void textBox28_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (Char)8)
            {
                e.Handled = true;
            }
        }

        //线长身份验证
        private void button28_Click(object sender, EventArgs e)
        {
            if (this.comboBox2.Text != "" && this.comboBox2.Text != "下拉选择")
            {
                string tt_usernumber_MFG = GetUserNumber(this.comboBox2.Text);
                string tt_password_MFG = GetUserPassword(this.comboBox2.Text);

                if (this.textBox28.Text == tt_usernumber_MFG && this.textBox27.Text == tt_password_MFG)
                {
                    this.groupBox23.Visible = true;
                    this.comboBox2.Enabled = false;
                    this.textBox28.Enabled = false;
                    this.textBox27.Enabled = false;
                    this.button3.Visible = true;
                    this.tabPage5.Parent = null;
                    this.tabPage6.Parent = tabControl2;
                    ScanDataInitial();
                    //获取线长调试开始时间
                    tt_reprintstattime = DateTime.Now;
                }
                else
                {
                    MessageBox.Show("工号或密码不对，请确认");
                }
            }
        }

        //线长身份验证重置
        private void button27_Click(object sender, EventArgs e)
        {
            this.comboBox1.Text = "0.3";
            this.comboBox2.Text = "下拉选择";
            this.textBox27.Text = "";
            this.textBox28.Text = "";
            this.comboBox2.Enabled = true;
            this.textBox27.Enabled = true;
            this.textBox28.Enabled = true;
            this.groupBox23.Visible = false;
            this.button3.Visible = false;
            this.tabPage6.Parent = null;
            this.tabPage5.Parent = tabControl2;
        }

        //取消身份验证过程，并结束设置
        private void button26_Click(object sender, EventArgs e)
        {
            this.comboBox1.Text = "0.3";
            this.comboBox2.Text = "下拉选择";
            this.textBox27.Text = "";
            this.textBox28.Text = "";
            this.comboBox2.Enabled = true;
            this.textBox27.Enabled = true;
            this.textBox28.Enabled = true;
            this.groupBox22.Visible = false;
            this.groupBox23.Visible = false;
            this.groupBox6.Visible = true;
            this.groupBox12.Visible = true;
            this.dataGridView1.Visible = true;
            this.button3.Visible = false;
            this.tabPage6.Parent = null;
            this.tabPage5.Parent = tabControl2;
        }

        //上移按钮
        private void button25_Click(object sender, EventArgs e)
        {
            tt_top -= float.Parse(this.comboBox1.Text);
        }

        //下移按钮
        private void button24_Click(object sender, EventArgs e)
        {
            tt_top += float.Parse(this.comboBox1.Text);
        }

        //左移按钮
        private void button22_Click(object sender, EventArgs e)
        {
            tt_left -= float.Parse(this.comboBox1.Text);
        }

        //右移按钮
        private void button23_Click(object sender, EventArgs e)
        {
            tt_left += float.Parse(this.comboBox1.Text);
        }

        //结束设置
        private void button21_Click(object sender, EventArgs e)
        {
            this.comboBox1.Text = "0.3";
            this.comboBox2.Text = "下拉选择";
            this.textBox27.Text = "";
            this.textBox28.Text = "";
            this.comboBox2.Enabled = true;
            this.textBox27.Enabled = true;
            this.textBox28.Enabled = true;
            this.groupBox22.Visible = false;
            this.groupBox23.Visible = false;
            this.groupBox6.Visible = true;
            this.groupBox12.Visible = true;
            this.dataGridView1.Visible = true;
            this.button3.Visible = false;
            this.tabPage6.Parent = null;
            this.tabPage5.Parent = tabControl2;
        }


        #endregion



        #region 12、扫描事件
        //MAC扫描标签重打
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---------以下MAC扫描-------
                ScanDataInitial();
                ClearLabelInfo_Barcode();
                setRichtexBox("-----开始MAC扫描--------");
                string tt_task1 = this.textBox1.Text.Trim().ToUpper();
                string tt_scanmac = this.textBox3.Text.Trim().ToUpper();
                string tt_shortmac = GetShortMac(tt_scanmac);


                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox7.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox6.Text.Trim());
                }


                //第三步 判断路径
                Boolean tt_flag3 = false;
                if (tt_flag2)
                {
                    tt_flag3 = getPathIstrue(tt_path);
                    if (tt_flag3)
                    {
                        setRichtexBox("3、已找到一个铭牌模板,：" + tt_path + ",goon");
                    }
                    else
                    {
                        setRichtexBox("3、没有找到铭牌模板,：" + tt_path + ",over");
                        PutLableInfor("没有找到铭牌模板，请检查！");
                    }
                }



                //第四步查找信息
                Boolean tt_flag4 = false;
                string tt_longmac = "";
                string tt_gpsn0 = "";
                string tt_gpsn1 = this.label29.Text;
                string tt_ponname = this.label89.Text;
                if (tt_flag3)
                {
                    string tt_sql3 = "select pcbasn,hostlable,maclable,smtaskscode,bprintuser,shelllable  from odc_alllable " +
                                     "where taskscode = '" + tt_task1 + "' and maclable = '" + tt_shortmac + "' ";


                    DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
                    if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        this.label56.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString();  //单板号
                        this.label55.Text = ds3.Tables[0].Rows[0].ItemArray[1].ToString();  //主机条码
                        this.label54.Text = ds3.Tables[0].Rows[0].ItemArray[2].ToString();  //MAC
                        this.label53.Text = ds3.Tables[0].Rows[0].ItemArray[3].ToString();  //移动串码
                        this.label51.Text = ds3.Tables[0].Rows[0].ItemArray[4].ToString();  //长MAC
                        tt_gpsn0 = ds3.Tables[0].Rows[0].ItemArray[5].ToString();  //GPSN

                        if (tt_gpsn1 == "HG6201T")
                        {
                            if (tt_gpsn0.Substring(0, 8) == "46485454")
                            {
                                this.label80.Text = tt_gpsn0;
                                this.label102.Text = Regex.Replace(tt_gpsn0, "46485454", "FHTT");
                            }
                            else
                            {
                                this.label80.Text = Regex.Replace(tt_gpsn0, "FHTT", "46485454");
                                this.label102.Text = tt_gpsn0;
                            }
                        }
                        else
                        {
                            this.label80.Text = tt_gpsn0;
                            this.label102.Text = "";
                        }

                        if (tt_ponname == "EPON")
                        {
                            this.label103.Text = Regex.Replace(tt_gpsn0, "-", "");
                        }
                        else
                        {
                            this.label103.Text = this.label80.Text;
                        }

                        tt_longmac = this.label51.Text;
                        setRichtexBox("3、在工单1:"+tt_task1+"的关联表查询到一条数据，goon");

                    }
                    else
                    {

                        string tt_sql3_1 = "select pcbasn,hostlable,maclable,smtaskscode,bprintuser,shelllable  from odc_alllable " +
                                     "where taskscode like '" + tt_task1 + "%' and maclable = '" + tt_shortmac + "' ";


                        DataSet ds3_1 = Dataset1.GetDataSet(tt_sql3_1, tt_conn);
                        if (ds3_1.Tables.Count > 0 && ds3_1.Tables[0].Rows.Count > 0)
                        {
                            tt_flag4 = true;
                            this.label56.Text = ds3_1.Tables[0].Rows[0].ItemArray[0].ToString();  //单板号
                            this.label55.Text = ds3_1.Tables[0].Rows[0].ItemArray[1].ToString();  //主机条码
                            this.label54.Text = ds3_1.Tables[0].Rows[0].ItemArray[2].ToString();  //MAC
                            this.label53.Text = ds3_1.Tables[0].Rows[0].ItemArray[3].ToString();  //移动串码
                            this.label51.Text = ds3_1.Tables[0].Rows[0].ItemArray[4].ToString();  //长MAC
                            tt_gpsn0 = ds3_1.Tables[0].Rows[0].ItemArray[5].ToString();  //GPSN

                            if (tt_gpsn1 == "HG6201T")
                            {
                                if (tt_gpsn0.Substring(0, 8) == "46485454")
                                {
                                    this.label80.Text = tt_gpsn0;
                                    this.label95.Text = Regex.Replace(tt_gpsn0, "46485454", "FHTT");
                                }
                                else
                                {
                                    this.label80.Text = Regex.Replace(tt_gpsn0, "FHTT", "46485454");
                                    this.label95.Text = tt_gpsn0;
                                }
                            }
                            else
                            {
                                this.label80.Text = tt_gpsn0;
                                this.label102.Text = "";
                            }

                            if (tt_ponname == "EPON")
                            {
                                this.label103.Text = Regex.Replace(tt_gpsn0, "-", "");
                            }
                            else
                            {
                                this.label103.Text = this.label80.Text;
                            }

                            tt_longmac = this.label51.Text;
                            setRichtexBox("3、在工单1:" + tt_task1 + "的关联表查询到一条数据，goon");

                        }
                        else
                        {
                            string tt_querytask = getSnRealTask("2", tt_shortmac);
                            setRichtexBox("3、在工单1:" + tt_task1 + "的关联表中没有查询到数据，该MAC的工单是" + tt_querytask + ",over");
                            PutLableInfor("该MAC的工单为:" + tt_querytask + ",与工单1:" + tt_task1 + "不符");
                        }                        
                        
                    }

                }


                //第五步查询macinfo表信息
                Boolean tt_flag5 = false;
                if (tt_flag4)
                {
                    tt_flag5 = true;
                    setRichtexBox("5、现在需求不需要查找Macinfo表信息，以后再说了，goon");                  
                }


                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                {
                    GetParaDataPrint(0);
                    CheckStation2(tt_shortmac);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    setRichtexBox("6、查询完毕，可以重打标签或修改模板，over");
                    PutLableInfor("MAC查询完毕");
                    if (tt_reprintmark == "0")
                    {
                        this.textBox3.Enabled = false;
                    }
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                }

                //生产节拍
                getProductRhythm("0");
                textBox3.Focus();
                textBox3.SelectAll();
                //---------以上MAC扫描-------
            }

        }

        //单板扫描进站
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //-----以下PCBA扫描------
                #region
                ScanDataInitial();
                ClearLabelInfo_Barcode();
                setRichtexBox("-----开始单板扫描--------");
                string tt_scanpcba = this.textBox2.Text.Trim().ToUpper();
                string tt_task1 = this.textBox1.Text.Trim().ToUpper(); //主工单
                string tt_task2 = this.textBox9.Text.Trim().ToUpper(); //子工单
                string tt_username = STR;
                string tt_id = "0";
                string tt_getmac = "";
                #endregion


                //第一步位数判断
                #region
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanpcba, this.textBox4.Text);
                #endregion


                //第二步包含符判断
                #region
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanpcba, this.textBox5.Text.Trim());
                }
                #endregion


                //第三步 检查模板
                #region
                Boolean tt_flag3 = false;
                if (tt_flag2)
                {
                    tt_flag3 = getPathIstrue(tt_path);
                    if (tt_flag3)
                    {
                        setRichtexBox("3、已找到一个铭牌模板,：" + tt_path + ",goon");
                    }
                    else
                    {
                        setRichtexBox("3、没有找到铭牌模板,：" + tt_path + ",over");
                        PutLableInfor("没有找到铭牌模板，请检查！");
                    }

                }
                #endregion


                //第四步扣数检查
                #region
                Boolean tt_flag4 = false;
                if (tt_flag3)
                {
                    tt_flag4 = true;
                    setRichtexBox("4、扣数检查过,goon");
                }
                #endregion


                //第五步物料检查
                #region
                Boolean tt_flag5 = false;
                if (tt_flag4)
                {
                    tt_flag5 = true;
                    setRichtexBox("5、物料检查过,goon");
                }
                #endregion


                //第六步流程检查
                #region
                Boolean tt_flag6 = false;
                string tt_gyid = this.label79.Text;
                string tt_ccode = this.label76.Text;
                string tt_ncode = this.label77.Text;
                if (tt_flag5)
                {
                    if (tt_ccode == "" || tt_ncode == "")
                    {
                        setRichtexBox("6、该工单没有好配置流程,待测站位：" + tt_ccode + ",进站站位：" + tt_ncode + ",over");
                        PutLableInfor("没有获取到当前待测站位，及下一站位，请检查");
                    }
                    else
                    {
                        tt_flag6 = true;
                        setRichtexBox("6、该工单已配置流程,待测站位：" + tt_ccode + ",进站站位：" + tt_ncode + ",goon");
                    }

                }
                #endregion


                //第七步 是否有MAC初步判断
                #region
                Boolean tt_flag7 = false;
                if(tt_flag6)
                {
                    if (this.label15.Text == "0" || this.label15.Text == "")
                    {
                        setRichtexBox("7、从统计信息上看，该工单已没有MAC，不能再做关联,over");
                        PutLableInfor("该工单已没有MAC，不能再做关联！");
                    }
                    else
                    {
                        tt_flag7 = true;
                        setRichtexBox("7、从统计信息上看，该工单已还有MAC，可以继续关联,goon");

                    }
                }
                #endregion


                //第八步 库存工单检查站位判断 
                #region
                Boolean tt_flag8 = false;
                string tt_task2endcode = this.label73.Text;
                if(tt_flag7)
                {
                    if (!tt_task2endcode.Equals(""))
                    {
                        tt_flag8 = true;
                        setRichtexBox("8、库存工单的最后站位：" + tt_task2endcode+",goon");
                    }
                    else
                    {
                        setRichtexBox("8、没有找到库存工单的最后站位,请检查工单查询信息over");
                        PutLableInfor("没有找到库存工单的最后站位！请检查设置");
                    }
                }
                #endregion


                //第九步 单板双胞胎检查
                #region
                Boolean tt_flag9 = false;
                if(tt_flag8)
                {
                    string tt_sql9 = "select count(1),min(taskcode),0 from odc_package " +
                                     "where pasn = '" + tt_scanpcba + "' ";
                    string[] tt_array9 = new string[3];
                    tt_array9 = Dataset1.GetDatasetArray(tt_sql9, tt_conn);
                    if (tt_array9[0] == "0")
                    {
                        tt_flag9 = true;
                        setRichtexBox("9、该单板在包装表package没有找到数据，可以关联,goon");
                    }
                    else
                    {
                        setRichtexBox("9、该单板在包装表package中已有数据，已用工单" + tt_array9[1] + ",不能关联,ober");
                        PutLableInfor("此单板在包装表package已有数据,请确认是否已使用过！");

                    }
                }
                #endregion


                //第十步 预留
                #region
                Boolean tt_flag10 = false;
                if(tt_flag9)
                {
                    tt_flag10 = true;
                    setRichtexBox("10、该项检查预留,goon");
                }
                #endregion


                //第十一步 是否是维修板检查
                #region
                Boolean tt_flag11 = false;
                if( tt_flag10)
                {
                    string tt_sql11 = "select count(1),0,0  from repair  " +
                                      "where  Fpcba = '" + tt_scanpcba + "' and Type = 1 ";
                    string[] tt_array11 = new string[3];
                    tt_array11 = Dataset1.GetDatasetArray(tt_sql11, tt_conn);
                    if (tt_array11[0] == "0")
                    {
                        tt_flag11 = true;
                        setRichtexBox("11、该单板没有进维修库或已维修出库，可以使用,goon");
                    }
                    else
                    {
                        setRichtexBox("11、该单板已进维修库，并且没有出库，不能使用,over");
                        PutLableInfor("该单板已进维修库，并且没有修好，不能使用");

                    }

                }
                #endregion


                //第十二步 关联表检查
                #region
                Boolean tt_flag12 = false;
                string tt_oldhostlable = "";
                string tt_oldshortmac = "";
                string tt_oldsmtaskscode = "";
                string tt_oldlongmac = "";
                string tt_oldgpsn = "";
                if(tt_flag11)
                {
                    string tt_sql12 = "select hostlable,maclable,smtaskscode,bprintuser,id,shelllable from odc_alllable " +
                        "where taskscode = '"+tt_task2+"' and  hprintman = '" + tt_task2 + "' and  pcbasn = '" + tt_scanpcba+ "' ";

                    DataSet ds12 = Dataset1.GetDataSet(tt_sql12, tt_conn);
                    if (ds12.Tables.Count > 0 && ds12.Tables[0].Rows.Count > 0)
                    {
                        tt_flag12 = true;
                        tt_oldhostlable = ds12.Tables[0].Rows[0].ItemArray[0].ToString();  //主机条码
                        tt_oldshortmac = ds12.Tables[0].Rows[0].ItemArray[1].ToString();    //短MAC
                        tt_oldsmtaskscode = ds12.Tables[0].Rows[0].ItemArray[2].ToString();  //移动串号
                        tt_oldlongmac = ds12.Tables[0].Rows[0].ItemArray[3].ToString();     //长MAC
                        tt_id = ds12.Tables[0].Rows[0].ItemArray[4].ToString();      //行ID
                        tt_oldgpsn = ds12.Tables[0].Rows[0].ItemArray[5].ToString();   //GPSN

                        setRichtexBox("12、工单2中:"+tt_task2+"关联表查询到一条x虚拟MAC数据，hostlable=" + tt_oldhostlable + ",mac=" + tt_oldshortmac + ",smtaskscode=" + tt_oldsmtaskscode + ",id=" + tt_id + ",GPSN=" + tt_oldgpsn + ",goon");

                    }
                    else
                    {
                        string tt_querytask = getSnRealTask("1", tt_scanpcba);
                        setRichtexBox("12、在工单2:"+tt_task2+"的关联表中没有查询到该单板数据，该单板工单为："+tt_querytask+",over");
                        PutLableInfor("该单板工单为:"+tt_querytask+",与工单2:" + tt_task2 + "不符");
                    }

                }
                #endregion



                //第十三步 NG01 虚拟MAC 获取MAC站位信息
                #region
                Boolean tt_flag13 = false;
                DataSet tt_dataset1 = null;
                if(tt_flag12)
                {
                    tt_dataset1 = Dataset2.getMacAllCodeInfo(tt_oldshortmac, tt_conn);
                    if (tt_dataset1.Tables.Count > 0 && tt_dataset1.Tables[0].Rows.Count > 0)
                    {
                        tt_flag13 = true;
                        setRichtexBox("13、站位表找到虚拟MAC站位信息，记录数为:" + tt_dataset1.Tables[0].Rows.Count .ToString()+ ",goon");
                    }
                    else
                    {
                        setRichtexBox("13、NG01,站位表没有找虚拟MAC:"+tt_oldshortmac+"，站位信息，over");
                        PutLableInfor("NG01,站位表没有找虚拟MAC:" + tt_oldshortmac + "，站位信息");
                    }
                }
                #endregion



                //第十四步 NG02 虚拟MAC 的待测站位
                #region
                Boolean tt_flag14 = false;
                if(tt_flag13)
                {
                    string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset1);
                    if ( tt_nowcode == tt_task2endcode)
                    {
                        tt_flag14 = true;
                        setRichtexBox("14、该单板的最后站位与流程设置的最后站位一致，都是:"+tt_nowcode+",goon");
                    }
                    else
                    {
                        if (tt_nowcode == "0")
                        {
                            setRichtexBox("14、NG02,当前单板虚拟MAC:" + tt_oldshortmac + ",没有待测站位，请检查，over");
                            PutLableInfor("NG02,当前单板虚拟MAC:" + tt_oldshortmac + ",没有待测站位");
                        }
                        else
                        {
                            if (tt_nowcode == "2")
                            {
                                setRichtexBox("14、NG02,当前单板虚拟MAC:" + tt_oldshortmac + ",有多个待测待测站位，流程异常，over");
                                PutLableInfor("NG02,单板虚拟MAC:" + tt_oldshortmac + ",有多个待测站位,流程异常");
                            }
                            else
                            {
                                setRichtexBox("14、NG02,当前单板虚拟MAC:" + tt_oldshortmac + "，站位不对" + tt_nowcode + "，与设定站位" + tt_task2endcode + "不符，不过使用,over");
                                PutLableInfor("NG02,单板虚拟MAC:" + tt_oldshortmac + ",当前站位" + tt_nowcode + ",与"+tt_task2endcode+",不符");
                            }
                        }

                    }

                }
                #endregion


                //第十五步 NG03 虚拟MAC 1920站位检查
                #region
                Boolean tt_flag15 = false;
                int tt_int1920id = 0;
                if(tt_flag14)
                {
                    tt_int1920id = Dataset2.getFirstCodeId(tt_dataset1);
                    if (tt_int1920id > 0)
                    {
                        tt_flag15 = true;
                        setRichtexBox("15、前站位ccode找到一个最近的1920站位，id=" + tt_int1920id.ToString() + ",goon");
                    }
                    else
                    {
                        switch (tt_int1920id)
                        {
                            case 0:
                                setRichtexBox("15、NG03,查找起始站位1902数据集内容有问题，数据集内容为空值,id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor("NG03查找起始站位1902数据集有问题，为空值");
                                break;

                            case -1:
                                setRichtexBox("15、NG03,查找起始站位1902数据集排序有问题，不是从大到小的顺序排序，id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor("NG03查找起始站位1902数据集排序有问题，不是顺序排序");
                                break;

                            case -2:
                                setRichtexBox("15、NG03,查找起始站位1902数据集有问题，没有找到起始1920站位，id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor("NG03,过站没有找到1920站位");
                                break;

                            default:
                                setRichtexBox("15、NG03,查找起始站位1902数据集有问题，出现异常情况，id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor("NG03,查找起始站位1902数据集出现异常情况");
                                break;


                        }
                    }
                }
                #endregion


                //第十六步 NG04 虚拟MAC 3350跳出检验
                #region
                Boolean tt_flag16 = false;
                if(tt_flag15)
                {
                    tt_flag16 = true;
                    setRichtexBox("16、NG04 3350跳出检验跳过，over");

                    //string tt_maintaincheck = Dataset2.getMaintainJumpCheck(tt_dataset1, tt_int1920id);
                    //if (tt_maintaincheck.Equals("1"))
                    //{
                    //    tt_flag16 = true;
                    //    setRichtexBox("16、3350跳出检查OK没有问题，返回值：" + tt_maintaincheck + ",检查起始ID:" + tt_int1920id.ToString() + ",goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("16、NG04,3350跳出检查Fail有问题，返回值：" + tt_maintaincheck + ",over");
                    //    PutLableInfor("NG04," + tt_maintaincheck);
                    //}
                    
                }
                #endregion


                //第十七步  NG05 虚拟MAC 全部流程检查
                #region
                Boolean tt_flag17 = false;
                if (tt_flag16)
                {
                    int tt_productname_check = 0;

                    if (CheckStrContain("HG6201M,HG6201T,HG2201T", this.label29.Text.Trim()) == true)
                    {
                        tt_productname_check = 1;
                    }

                    string tt_codecheck = Dataset2.getPcbaAllCheck2(tt_routdataset, tt_dataset1, tt_int1920id, tt_productname_check);
                    if (tt_codecheck == "1")
                    {
                        tt_flag17 = true;
                        setRichtexBox("17、该单板所有站位都测试，没有漏测站位，全部流程:" + tt_allprocesses + ",检验流程:" + tt_partprocesses + ",1920id:" + tt_int1920id.ToString() + ",goon");
                    }
                    else
                    {
                        if (tt_codecheck == "0")
                        {
                            setRichtexBox("17、NG05,单板站位全流程检查数据集有问题,MAC" + tt_oldshortmac + ",全部流程:" + tt_allprocesses + ",检验流程:" + tt_partprocesses + ",1920id:" + tt_int1920id.ToString() + ",over");
                            PutLableInfor("NG05,单板站位全流程检查数据集有问题");
                        }
                        else
                        {
                            setRichtexBox("17、NG05,该单板这个站位没有测试:" + tt_codecheck + "，请仔细检查MAC:" + tt_oldshortmac + ",的流程:全流程为:" + tt_allprocesses + ",检测流程为:" + tt_partprocesses + ",是否有漏测站位，over");
                            PutLableInfor("NG05,该单板这个站位没有测试:" + tt_codecheck + "，请检查是否漏测");
                        }
                    }
                }
                #endregion



                //第十八步 NG06 虚拟MAC 流程顺序检查
                #region
                Boolean tt_flag18 = false;
                if (tt_flag17)
                {
                    tt_flag18 = true;
                    setRichtexBox("18、NG06 流程顺序检查跳过，over");

                    //string tt_codeserialcheck = Dataset2.getCodeSerialCheck(tt_dataset1, tt_int1920id);
                    //if (tt_codeserialcheck.Equals("1"))
                    //{
                    //    tt_flag18 = true;
                    //    setRichtexBox("18、MAC全顺序检查OK没有问题，返回值：" + tt_codeserialcheck + ",检查起始ID:" + tt_int1920id.ToString() + ",goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("18、NG06,MAC全顺序检查Fail有问题，返回值：" + tt_codeserialcheck + ",over");
                    //    PutLableInfor("NG06," + tt_codeserialcheck);
                    //}
                }
                #endregion


                //第十九步 NG07 虚拟MAC 流程前后项检查信息
                #region
                Boolean tt_flag19 = false;
                if (tt_flag18)
                {
                    tt_flag19 = true;
                    setRichtexBox("19、NG07 流程顺序检查跳过，over");

                    //string tt_nearcodecheck = Dataset2.getNearCodeCheck2(tt_dataset1, tt_int1920id, tt_allroutdataset);
                    //if (tt_nearcodecheck.Equals("1"))
                    //{
                    //    tt_flag19 = true;
                    //    setRichtexBox("19、过站前后站位检查OK没有问题，返回值：" + tt_nearcodecheck + ",检查起始ID:" + tt_int1920id.ToString() + ",goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("19、NG07,过站前后站位检查Fail有问题，返回值：" + tt_nearcodecheck + ",over");
                    //    PutLableInfor("NG07," + tt_nearcodecheck);
                    //}
                }
                #endregion


                //第二十步 NG08 虚拟MAC 流程上下项检查信息
                #region
                Boolean tt_flag20 = false;
                if (tt_flag19)
                {
                    tt_flag20 = true;
                    setRichtexBox("20、NG08 流程顺序检查跳过，over");

                    //string tt_updowncodecheck = Dataset2.getUpdownCodeCheck(tt_dataset1, tt_int1920id, tt_allroutdataset);
                    //if (tt_updowncodecheck.Equals("1"))
                    //{
                    //    tt_flag20 = true;
                    //    setRichtexBox("20、过站上下站位检查OK没有问题，返回值：" + tt_updowncodecheck + ",检查起始ID:" + tt_int1920id.ToString() + ",goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("20、NG08,过站上下站位检查Fail有问题，返回值：" + tt_updowncodecheck + ",over");
                    //    PutLableInfor("NG08," + tt_updowncodecheck);
                    //}
                }
                #endregion


                //第二十一步 NG09 虚拟MAC 检查预留2
                #region
                Boolean tt_flag21 = false;
                if (tt_flag20)
                {
                    tt_flag21 = true;
                    setRichtexBox("21、NG09虚拟MAC检查预留二，over");
                }
                #endregion


                //第二十二步 NG10 虚拟MAC 检查预留3
                #region
                Boolean tt_flag22 = false;
                if (tt_flag21)
                {
                    tt_flag22 = true;
                    setRichtexBox("22、NG10虚拟MAC检查预留三，over");
                }
                #endregion


                //第二十三步  其他预留
                #region
                Boolean tt_flag23 = false;
                if (tt_flag22)
                {
                    tt_flag23 = true;
                    setRichtexBox("23、其他预留，over");
                }
                #endregion


                //第二十四步 获取工单1剩余MAC
                #region
                Boolean tt_flag24 = false;
                string tt_langmac = "";
                string tt_shortmac = "";
                string tt_barcode = "";
                string tt_gpsn = "";
                string tt_user = "";
                string tt_password = "";
                string tt_gpsn0 = "";
                String tt_onumac = "";
                string tt_gpsn1 = this.label29.Text;
                String tt_ponname = this.label89.Text;
                if(tt_flag23)
                {
                    string tt_sql24 = "select top 10 mac,barcode,sn,username,password  from odc_macinfo " +
                                      " where taskscode = '" + tt_task1 + "'  and fusestate is null ";
                    DataSet ds24 = Dataset1.GetDataSet(tt_sql24, tt_conn);

                    if (ds24.Tables.Count > 0 && ds24.Tables[0].Rows.Count > 0)
                    {
                        //取随机数
                        int tt_rowcount = ds24.Tables[0].Rows.Count;
                        Random ran = new Random();
                        int n = ran.Next(0, tt_rowcount - 1);

                        tt_langmac = ds24.Tables[0].Rows[n].ItemArray[0].ToString();
                        tt_barcode = ds24.Tables[0].Rows[n].ItemArray[1].ToString();
                        tt_gpsn0 = ds24.Tables[0].Rows[n].ItemArray[2].ToString();
                        tt_user = ds24.Tables[0].Rows[n].ItemArray[3].ToString();
                        tt_password = ds24.Tables[0].Rows[n].ItemArray[4].ToString();
                                                
                        tt_flag24 = true;
                        tt_shortmac = GetShortMac(tt_langmac);
                        tt_getmac = tt_shortmac;

                        if (tt_gpsn1 == "HG6201T")
                        {
                            tt_gpsn = Regex.Replace(tt_gpsn0, "FHTT", "46485454");
                        }
                        else
                        {
                            tt_gpsn = tt_gpsn0;
                        }

                        if (tt_ponname == "EPON")
                        {
                            tt_onumac = Regex.Replace(tt_gpsn0, "-", "");
                        }
                        else
                        {
                            tt_onumac = tt_gpsn;
                        }
                        
                        setRichtexBox("24.1、该工单还有剩余AMC：共获取MAC数:" + tt_rowcount.ToString() + "，随机数：" + n.ToString() + ",goon");
                        setRichtexBox("24.2、该工单还有剩余AMC：已获取一个长MAC为:" + tt_langmac + "，短MAC为：" + tt_shortmac +
                            ",32位移动编码:" + tt_barcode + ",GPSN/OUN MAC号码：" + tt_gpsn + ",可以关联,goon");
                        setRichtexBox("18.3、用户名检验，已获取一个用户为:" + tt_user + "，密码为：" + tt_password);

                    }
                    else
                    {
                        setRichtexBox("24、该工单已"+tt_task1+"没有有剩余AMC，不能再关联,over");
                        PutLableInfor("该工单"+tt_task1+"已没有有剩余AMC");
                    }
                }
                #endregion



                //第二十五步 检查MAC是否用过
                #region
                Boolean tt_flag25 = false;
                if(tt_flag24)
                {
                    string tt_sql25 = "select count(1),0,0 from odc_alllable where maclable = '" + tt_getmac + "' ";
                    string[] tt_array25 = new string[3];
                    tt_array25 = Dataset1.GetDatasetArray(tt_sql25, tt_conn);

                    if (tt_array25[0] == "0")
                    {
                        tt_flag25 = true;
                        setRichtexBox("25、该MAC:" + tt_getmac + "在关联表alllable中没有找到，可以关联,goon");
                    }
                    else
                    {
                        //把MAC置为已用状态
                        string tt_sql251 = "update odc_macinfo set fusestate ='1'  where taskscode='" + tt_task1 + "' and MAC ='" + tt_langmac + "' ";

                        int tt_int251 = Dataset1.ExecCommand(tt_sql251, tt_conn);
                        if (tt_int251 > 0)
                        {

                            setRichtexBox("25、该MAC:" + tt_getmac + "在关联表alllable中有一个重复，不可以关联,已把这个状态值改为1，over");
                            PutLableInfor("获取MAC已用过,请重新扫描BOSA，重新获取MAC");
                        }
                        else
                        {
                            setRichtexBox("25、该MAC:" + tt_getmac + "在关联表alllable中有一个重复，不可以关联,状态值没有改为1，over");
                            PutLableInfor("获取MAC已用过,请重新扫描BOSA，重新获取MAC");
                        }

                    }
                }
                #endregion



                //第二十六步 对获取的MAC进行站位检查
                #region
                Boolean tt_flag26 = false;
                if(tt_flag25)
                {
                    string tt_sql26 = "select count(1),min(Ncode),0  from odc_routingtasklist " +
                                     "where pcba_pn = '" + tt_getmac + "'  and napplytype is null ";

                    string[] tt_array26 = new string[3];
                    tt_array26 = Dataset1.GetDatasetArray(tt_sql26, tt_conn);

                    if (tt_array26[0] == "0")
                    {
                        tt_flag26 = true;
                        setRichtexBox("26、该MAC：" + tt_getmac + "没有待测站位，可以关联，goon");
                    }
                    else
                    {
                        setRichtexBox("26、该MAC:" + tt_getmac + "已有待测站位：" + tt_array26[1] + "，请再次扫描");
                        PutLableInfor("该MAC:" + tt_getmac + "已有待测站位：" + tt_array26[1] + "，请再次扫描!");
                    }
                }
                #endregion



                //第二十七步 用户名检查
                #region
                Boolean tt_flag27 = false;
                if (tt_flag26)
                {
                    string tt_setuser = this.label40.Text;
                    if (tt_setuser.Equals("0"))
                    {
                        tt_flag27 = true;
                        setRichtexBox("27、设定的用户名为0，不需要进行用户名检验，goon");
                    }
                    else
                    {
                        if (tt_setuser == tt_user)
                        {
                            tt_flag27 = true;
                            setRichtexBox("27、获取MAC用户名与设定的用户一致，都是:" + tt_user + "，goon");
                        }
                        else
                        {
                            setRichtexBox("27、该MAC的用户名:" + tt_user + ",与设定的用户名不一致：" + tt_setuser + "，请检查MAC导入信息,over");
                            PutLableInfor("获取MAC用户名" + tt_user + "与设定的用户不一致，请检查MAC导入信息！");
                        }

                    }
                }
                #endregion


                //第二十八步 密码位数检查
                #region
                Boolean tt_flag28 = false;
                if(tt_flag27)
                {
                    string tt_userlength = this.label42.Text;
                    if (tt_userlength.Equals("0"))
                    {
                        tt_flag28 = true;
                        setRichtexBox("28、密码位数设置为0,不需要位数判断，goon");
                    }
                    else
                    {
                        string tt_passwordlen = tt_password.Length.ToString();
                        if (tt_userlength == tt_passwordlen)
                        {
                            tt_flag28 = true;
                            setRichtexBox("28、获取MAC密码" + tt_password + "的位数与设定的密码位数一致，都是:" + tt_passwordlen + "位，goon");
                        }
                        else
                        {
                            setRichtexBox("28、获取MAC密码" + tt_password + "的位数与设定的密码位数不一致，不是:" + tt_passwordlen + "位，goon");
                            PutLableInfor("28、获取MAC密码" + tt_password + "的位数与设定的密码位数不一致");
                        }
                    }
                }
                #endregion


                //第二十九步 密码大小写判断
                #region
                Boolean tt_flag29 = false;
                if(tt_flag28)
                {
                    string tt_lettersize = this.label2.Text;
                    if (tt_lettersize.Equals("0"))
                    {
                        tt_flag29 = true;
                        setRichtexBox("29、密码大小写设置为0,不需要大小判断，goon");
                    }
                    else
                    {
                        bool tt_flag291 = getStrChar(tt_password, tt_lettersize);
                        if (tt_flag291)
                        {
                            tt_flag29 = true;
                            setRichtexBox("29、密码大小写判断正确，goon");
                        }
                        else
                        {
                            setRichtexBox("29、该MAC的密码:" + tt_password + "，大小写判定不正确，1为小写2为大写");
                            setRichtexBox("该MAC的密码:" + tt_password + "，大小写判定不正确");
                        }
                    }
                }
                #endregion



                //第三十步 开始进站
                #region
                Boolean tt_flag30 = false;
                if( tt_flag29)
                {
                    tt_flag30 = Dataset1.FhYcMadeinStation(tt_task1, tt_task2,  //真虚工单
                                                           tt_oldshortmac, tt_task2endcode, tt_gyid2, //虚拟MAC及站位流程
                                                           tt_id, tt_scanpcba,   //单板行ID
                                                           tt_getmac, tt_langmac,  //长MAC短MAC
                                                           tt_gpsn, tt_barcode,    //GPSN,移动串号
                                                           tt_username, tt_gyid,  //用户名及流程
                                                           tt_ccode, tt_ncode,    //待测站位及下一站位
                                                           tt_conn);
                    if (tt_flag30)
                    {
                        setRichtexBox("30、单板MAC关联成功，请继续扫描");
                    }
                    else
                    {
                        setRichtexBox("30、单板MAC关联不成功，事务已回滚");
                        PutLableInfor("单板MAC关联不成功，请检查或再次扫描");
                    }

                }
                #endregion
                

                //最后判断
                #region
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10 && 
                    tt_flag11 && tt_flag12 && tt_flag13 && tt_flag14 && tt_flag15 && tt_flag16 && tt_flag17 && tt_flag18 && tt_flag19 && tt_flag20 &&
                    tt_flag21 && tt_flag22 && tt_flag23 && tt_flag24 && tt_flag25 && tt_flag26 && tt_flag27 && tt_flag28 && tt_flag29 && tt_flag30)
                {
                    //数据显示
                    this.label56.Text = tt_scanpcba;  //单板号
                    this.label55.Text = tt_shortmac;  //主机条码
                    this.label54.Text = tt_getmac;    //短MAC
                    this.label53.Text = tt_barcode;   //移动串码
                    this.label51.Text = tt_langmac;   //长MAC
                    this.label80.Text = tt_gpsn;      //GPSN
                    this.label102.Text = "";    //GPSN原始码
                    this.label103.Text = tt_onumac;   //PON MAC暗码


                    if (tt_gpsn0.Substring(0, 8) == "46485454")
                    {
                        this.label102.Text = Regex.Replace(tt_gpsn0, "46485454", "FHTT");
                    }
                    else
                    {
                        this.label102.Text = tt_gpsn0;
                    }

                    //打印记录
                    Dataset1.lablePrintRecord(tt_task1, tt_getmac, tt_oldshortmac, "延迟铭牌标签", str, tt_computermac, "", tt_conn);

                    //打印
                    GetParaDataPrint(1);
                    CheckStation2(tt_getmac);
                    GetMacUseNumber();
                    PutListViewData(tt_scanpcba, tt_oldshortmac, tt_getmac, tt_gpsn, tt_barcode);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    setRichtexBox("88，OK进站成功，可以重打标签或修改模板，或继续扫描");
                    PutLableInfor("OK,继续");
                    getProductRhythm("1");
                }
                else
                {
                    getProductRhythm("0");
                    this.richTextBox1.BackColor = Color.Red;
                }
                #endregion


                //继续扫描
                textBox2.Focus();
                textBox2.SelectAll();
                //-----以上PCBA扫描------
            }
        }
        #endregion



        #region 11、数据采集及模板打印
        //获取参数
        private void GetParaDataPrint(int tt_itemtype)
        {
            string tt_fdata = this.label32.Text;

            //mp01---数据类型一
            if (tt_fdata == "MP01")
            {
                GetParaDataPrint_MP01(tt_itemtype);
            }

            //mp01---数据类型一
            if (tt_fdata == "MC01")
            {
                GetParaDataPrint_MC01(tt_itemtype);
            }

            //mp01---数据类型一
            if (tt_fdata == "MF01")
            {
                GetParaDataPrint_MF01(tt_itemtype);
            }

        }


        //----以下是MP01数据采集----
        private void GetParaDataPrint_MP01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();
            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");


            DataRow row1 = dt.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "产品型号";
            row1["内容"] = doReportParaCheck("产品型号",this.label29.Text);
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "CMITID";
            row2["内容"] = doReportParaCheck("CMITID", this.label61.Text);
            dt.Rows.Add(row2);



            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "GPSN\\OUN MAC";
            row3["内容"] = doReportParaCheck("GPSN", this.label80.Text);
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "物料编码";
            row4["内容"] = doReportParaCheck("物料编码", this.label49.Text);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "短MAC";
            row5["内容"] = doReportParaCheck("短MAC", this.label54.Text);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "长MAC";
            row6["内容"] = doReportParaCheck("长MAC", this.label51.Text);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "地区码";
            row7["内容"] = this.label88.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "PON类型";
            row8["内容"] = this.label89.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "GPSN\\ONU MAC暗码";
            row9["内容"] = this.label103.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "文字变量01";
            row10["内容"] = this.label100.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "文字变量03";
            row11["内容"] = this.label97.Text;
            dt.Rows.Add(row11);

            DataRow row12 = dt.NewRow();
            row12["参数"] = "S12";
            row12["名称"] = "电压";
            row12["内容"] = this.label90.Text;
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "S13";
            row13["名称"] = "电流";
            row13["内容"] = this.label92.Text;
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "S14";
            row14["名称"] = "文字变量02";
            row14["内容"] = this.label98.Text;
            dt.Rows.Add(row14);

            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 60;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 200;


            //第四步 打印或预览
            //单板打印
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path);
                report.SetParameterValue("S01", dst.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("S02", dst.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("S03", dst.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("S04", dst.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("S05", dst.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("S06", dst.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("S07", dst.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("S08", dst.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("S09", dst.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("S10", dst.Tables[0].Rows[9][2].ToString());
                report.SetParameterValue("S11", dst.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("S12", dst.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("S13", dst.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("S14", dst.Tables[0].Rows[13][2].ToString());


                for (int i = 0; i < 500; ++i)
                {
                    string s = string.Format("Text{0}", i + 1);
                    TextObject p1 = report.FindObject(s) as TextObject;
                    if (p1 != null)
                    {
                        p1.Top += tt_top;
                        p1.Left += tt_left;
                    }
                    s = string.Format("Barcode{0}", i + 1);
                    BarcodeObject p2 = report.FindObject(s) as BarcodeObject;
                    if (p2 != null)
                    {
                        p2.Top += tt_top;
                        p2.Left += tt_left;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    report.Print();
                    report.Save(tt_path);
                    tt_top = 0;
                    tt_left = 0;
                    PutLableInfor("打印完毕");
                }

                //--预览
                if (tt_itemtype == 2)
                {
                    report.Design();
                    PutLableInfor("预览完毕");
                }




                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");


            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }


        }

        //----以下是MC01数据采集----
        private void GetParaDataPrint_MC01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();
            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");


            DataRow row1 = dt.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "产品型号";
            row1["内容"] = doReportParaCheck("产品型号", this.label29.Text);
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "CMITID";
            row2["内容"] = doReportParaCheck("CMITID", this.label61.Text);
            dt.Rows.Add(row2);



            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "GPSN\\OUN MAC";
            row3["内容"] = doReportParaCheck("GPSN", this.label80.Text);
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "物料编码";
            row4["内容"] = doReportParaCheck("物料编码", this.label49.Text);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "短MAC";
            row5["内容"] = doReportParaCheck("短MAC", this.label54.Text);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "长MAC";
            row6["内容"] = doReportParaCheck("长MAC", this.label51.Text);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "地区码";
            row7["内容"] = this.label88.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "PON类型";
            row8["内容"] = this.label89.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "GPSN\\ONU MAC暗码";
            row9["内容"] = this.label103.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "文字变量01";
            row10["内容"] = this.label100.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "文字变量03";
            row11["内容"] = this.label98.Text;
            dt.Rows.Add(row11);

            DataRow row12 = dt.NewRow();
            row12["参数"] = "S12";
            row12["名称"] = "电压";
            row12["内容"] = this.label90.Text;
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "S13";
            row13["名称"] = "电流";
            row13["内容"] = this.label92.Text;
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "S14";
            row14["名称"] = "文字变量02";
            row14["内容"] = this.label99.Text;
            dt.Rows.Add(row14);

            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 60;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 200;


            //第四步 打印或预览
            //单板打印
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path);
                report.SetParameterValue("S01", dst.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("S02", dst.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("S03", dst.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("S04", dst.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("S05", dst.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("S06", dst.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("S07", dst.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("S08", dst.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("S09", dst.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("S10", dst.Tables[0].Rows[9][2].ToString());
                report.SetParameterValue("S11", dst.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("S12", dst.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("S13", dst.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("S14", dst.Tables[0].Rows[13][2].ToString());


                for (int i = 0; i < 500; ++i)
                {
                    string s = string.Format("Text{0}", i + 1);
                    TextObject p1 = report.FindObject(s) as TextObject;
                    if (p1 != null)
                    {
                        p1.Top += tt_top;
                        p1.Left += tt_left;
                    }
                    s = string.Format("Barcode{0}", i + 1);
                    BarcodeObject p2 = report.FindObject(s) as BarcodeObject;
                    if (p2 != null)
                    {
                        p2.Top += tt_top;
                        p2.Left += tt_left;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    report.Print();
                    report.Save(tt_path);
                    tt_top = 0;
                    tt_left = 0;
                    PutLableInfor("打印完毕");
                }

                //--预览
                if (tt_itemtype == 2)
                {
                    report.Design();
                    PutLableInfor("预览完毕");
                }




                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");


            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }


        }

        //----以下是MF01数据采集----
        private void GetParaDataPrint_MF01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();
            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");


            DataRow row1 = dt.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "产品型号";
            row1["内容"] = this.label29.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "COMITID";
            row2["内容"] = this.label61.Text;
            dt.Rows.Add(row2);
            

            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "GPSN\\ONU MAC";
            row3["内容"] = this.label80.Text + "(" + this.label102.Text + ")";
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "物料编码";
            row4["内容"] = this.label49.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "短MAC";
            row5["内容"] = this.label54.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "长MAC";
            row6["内容"] = this.label51.Text;
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "地区码";
            row7["内容"] = this.label88.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "PON类型";
            row8["内容"] = this.label89.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "GPSN\\ONU MAC暗码";
            row9["内容"] = this.label103.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "文字变量01";
            row10["内容"] = this.label100.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "文字变量02";
            row11["内容"] = this.label98.Text;
            dt.Rows.Add(row11);

            DataRow row12 = dt.NewRow();
            row12["参数"] = "S12";
            row12["名称"] = "电压";
            row12["内容"] = this.label90.Text;
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "S13";
            row13["名称"] = "电流";
            row13["内容"] = this.label92.Text;
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "S14";
            row14["名称"] = "文字变量03";
            row14["内容"] = this.label97.Text;
            dt.Rows.Add(row14);

            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 60;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 200;


            //第四步 打印或预览
            //单板打印
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path);
                report.SetParameterValue("S01", dst.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("S02", dst.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("S03", dst.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("S04", dst.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("S05", dst.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("S06", dst.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("S07", dst.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("S08", dst.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("S09", dst.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("S10", dst.Tables[0].Rows[9][2].ToString());
                report.SetParameterValue("S11", dst.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("S12", dst.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("S13", dst.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("S14", dst.Tables[0].Rows[13][2].ToString());

                for (int i = 0; i < 500; ++i)
                {
                    string s = string.Format("Text{0}", i + 1);
                    TextObject p1 = report.FindObject(s) as TextObject;
                    if (p1 != null)
                    {
                        p1.Top += tt_top;
                        p1.Left += tt_left;
                    }
                    s = string.Format("Barcode{0}", i + 1);
                    BarcodeObject p2 = report.FindObject(s) as BarcodeObject;
                    if (p2 != null)
                    {
                        p2.Top += tt_top;
                        p2.Left += tt_left;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    report.Print();
                    report.Save(tt_path);
                    tt_top = 0;
                    tt_left = 0;
                    PutLableInfor("打印完毕");
                }

                //--预览
                if (tt_itemtype == 2)
                {
                    report.Design();
                    PutLableInfor("预览完毕");
                }




                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");


            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }


        }
        
        //参数检验为空值就弹窗
        private string doReportParaCheck(string tt_datatype,string tt_str)
        {
            string tt_outinfo = "";
            if (!tt_str.Equals(""))
            {
                tt_outinfo = tt_str;
            }
            else
            {
                MessageBox.Show("模板参数数据项" + tt_datatype+",为空，请检查");
            }
            return tt_outinfo;
        }


        #endregion


        //--------------

    }
}
