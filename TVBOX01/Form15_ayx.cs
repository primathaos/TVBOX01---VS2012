using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;  //正则表达式
using FastReport;
using FastReport.Barcode;
using System.Threading;

namespace TVBOX01
{
    public partial class Form15_ayx : Form
    {
        public Form15_ayx()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_conn;
        static string tt_path1 = "";
        static string tt_path2 = "";
        static string tt_path3 = "";
        static string tt_path4 = "";
        //static string tt_md5_1 = "";
        //static string tt_md5_2 = "";
        static string tt_ponname = "";
        int tt_yield = 0;  //产量
        static string tt_pcname = System.Net.Dns.GetHostName();
        static int tt_reprinttime = 0; //重打次数
        DateTime tt_productstarttime = DateTime.Now; //开始时间
        DateTime tt_productprimtime; //上一次时间
        //全流程检验
        static string tt_allprocesses = null;
        static string tt_partprocesses = null;
        static DataSet tt_routdataset = null;
        static DataSet tt_allroutdataset = null;
        //文件同步
        //static string tt_delepath = Application.StartupPath + @"\LABLE";
        //static string tt_copypath = @"D:\\LABLE";
        //自助分单
        string tt_taskmin = "";    //已生产数量最少的工单产品数量
        string tt_leftmax = "";    //未生产数量最多的工单产品数量
        string tt_taskinfo = "";   //未生产数量最多的工单信息及序号
        string tt_taskminname = "";//未生产数量最多的工单信息
        int tt_tasknum = 0;        //未生产数量最多的工单序号 --自动获取表值使用
        int tt_tasknum0 = 0;       //子工单数量
        int tt_ZeroTask = 0;       //0包装工单数量
        int tt_palletnum = 0;      //产品栈板包装数
        //标签微调
        static float tt_top1 = 0; //I型标签上下偏移量
        static float tt_left1 = 0; //I型标签左右偏移量
        static float tt_top2 = 0; //二维码标签上下偏移量
        static float tt_left2 = 0; //二维码标签左右偏移量
        static float tt_top3 = 0; //彩盒标签上下偏移量
        static float tt_left3 = 0; //彩盒标签左右偏移量
        static float tt_top4 = 0; //II型标签上下偏移量
        static float tt_left4 = 0; //II型标签左右偏移量
        int tt_checkflag = 0;
        
        //重打限制标识
        string tt_reprintmark = "1";
        //重打限数
        int tt_reprintchang1 = 0;
        int tt_reprintchang2 = 0;
        int tt_reprintchang3 = 0;
        int tt_reprintchang4 = 0;
        //重打计时
        DateTime tt_reprintstattime;
        DateTime tt_reprintendtime;

        //流程兼容用中间变量
        static string tt_gyid_Old = "";
        static string tt_gyid_Use = "";

        //读取的打印设置
        static string PrintChange = "";
        //static string Itype_PrintDelay = "";
        static string IItype_PrintDelay = "";
        //static string BOX_PrintDelay = "";
        //static string QR_PrintDelay = "";

        //本机MAC
        static string tt_computermac = "";

        //小型化产品用参数
        static string tt_parenttask = "";

        //上海资产编码前段号参数
        static string tt_shanghailabel = "";
        
        //联通河北日期参数
        static string tt_hebeiItypedate = "";

        //旧电源适配器标识
        static string tt_power_old = "";


        private void Form15_ayx_Load(object sender, EventArgs e)
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

            //隐藏预览按钮
            this.Itype_view.Visible = false;
            this.QR_view.Visible = false;
            this.Box_view.Visible = false;
            this.IItype_view.Visible = false;

            //隐藏打印按钮
            this.Itype_print.Visible = false;
            this.QR_print.Visible = false;
            this.Box_print.Visible = false;
            this.IItype_print.Visible = false;

            //员工账号分离
            if (str.Contains("FH003"))
            {
                this.tabPage6.Parent = null;
                this.tabPage4.Parent = null;
                this.button20.Visible = true;
            }

            ClearLabelInfo();

            //生产节拍
            this.label25.Text = tt_yield.ToString();
            this.label26.Text = null;
            this.label27.Text = null;
            this.label28.Text = null;

            //ListView添加表头
            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.LabelEdit = false; //是否可编辑,ListView只可编辑第一列。
            this.listView1.Scrollable = true;//有滚动条
            this.listView1.HeaderStyle = ColumnHeaderStyle.Clickable;//对表头进行设置
            this.listView1.FullRowSelect = true;//是否可以选择行

            this.listView2.GridLines = true; //显示表格线
            this.listView2.View = View.Details;//显示表格细节
            this.listView2.LabelEdit = false; //是否可编辑,ListView只可编辑第一列。
            this.listView2.Scrollable = true;//有滚动条
            this.listView2.HeaderStyle = ColumnHeaderStyle.Clickable;//对表头进行设置
            this.listView2.FullRowSelect = true;//是否可以选择行

            this.listView1.Columns.Add("序号", 40);
            this.listView1.Columns.Add("制造单", 80);
            this.listView1.Columns.Add("工单状态", 70);
            this.listView1.Columns.Add("下单时间", 120);
            this.listView1.Columns.Add("客户代码", 70);
            this.listView1.Columns.Add("PID", 40);
            this.listView1.Columns.Add("产品名称", 70);
            this.listView1.Columns.Add("PON类型", 70);
            this.listView1.Columns.Add("工单数量", 70);
            this.listView1.Columns.Add("已包装数量", 80);
            this.listView1.Columns.Add("开产时间", 120);
            this.listView1.Columns.Add("流程代码", 70);
            this.listView1.Columns.Add("ISSD", 40);
            this.listView1.Columns.Add("PCCOUNT", 60);
            this.listView1.Columns.Add("地区代码", 70);
            this.listView1.Columns.Add("软件版本", 100);
            this.listView1.Columns.Add("物料代码", 110);
            this.listView1.Columns.Add("地区", 50);
            this.listView1.Columns.Add("软版（程控1）", 100);
            this.listView1.Columns.Add("软版（程控2）", 100);
            this.listView1.Columns.Add("编译时间", 150);
            this.listView1.Columns.Add("产品类型", 70);
            this.listView1.Columns.Add("CMIIT ID", 80);
            this.listView1.Columns.Add("老化比例", 70);
            this.listView1.Columns.Add("漏光比例", 70);
            this.listView1.Columns.Add("EC代码", 80);
            this.listView1.Columns.Add("生产序列", 80);
            this.listView1.Columns.Add("初始号段", 80);
            this.listView1.Columns.Add("额外编码", 100);
            this.listView1.Columns.Add("延迟制造", 100);
            this.listView1.Columns.Add("BOSA TYPE", 100);
            this.listView1.Columns.Add("生产流程2", 100);
            this.listView1.Columns.Add("小型化", 100);

            this.listView2.Columns.Add("序号", 40);
            this.listView2.Columns.Add("制造单", 80);
            this.listView2.Columns.Add("工单数量", 70);
            this.listView2.Columns.Add("生产序列", 80);
            this.listView2.Columns.Add("额外编码", 100);
            this.listView2.Columns.Add("分单信息确认", 250);

            this.label101.Text = null;
            this.label100.Text = null;
            this.groupBox15.Visible = false;
            this.groupBox17.Visible = false;
            this.groupBox18.Visible = false;
            this.groupBox20.Visible = false;
            this.groupBox21.Visible = false;
            this.groupBox3.Visible = false;
            this.label164.Visible = false;
            this.label165.Visible = false;
            this.label166.Visible = false;
            this.label167.Visible = false;
            this.label168.Visible = false;
            this.textBox29.Visible = false;
            this.textBox30.Visible = false;
            this.checkBox7.Visible = false;

            //打印机设定
            this.Itype_printset.Text = this.label114.Text;
            this.QR_printset.Text = this.label115.Text;
            this.Box_printset.Text = this.label137.Text;
            this.IItype_printset.Text = this.label145.Text;

            this.Itype_printset.Enabled = false;
            this.QR_printset.Enabled = false;
            this.Box_printset.Enabled = false;
            this.IItype_printset.Enabled = false;

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
        //工单清除
        private void ClearLabelInfo()
        {
            //清除工单信息
            this.label12.Text = null;
            this.label13.Text = null;
            this.label14.Text = null;
            this.label15.Text = null;
            this.label16.Text = null;
            this.label17.Text = null;
            this.label18.Text = null;
            this.label19.Text = null;
            this.label20.Text = null;
            this.label65.Text = null;
            this.label85.Text = null;
            this.label86.Text = null;
            this.label37.Text = null;
            this.label41.Text = null;
            this.label148.Text = null;
            this.label149.Text = null;
            this.label150.Text = null;
            this.label152.Text = null;
            this.label155.Text = null;
            this.label156.Text = null;
            this.label159.Text = null;
            this.label160.Text = null;
            this.label168.Text = null;
            this.label169.Text = null;
            this.label174.Text = null;
            this.textBox4.Text = null;
            this.textBox5.Text = null;
            this.textBox8.Text = null;

            //流程信息
            this.label54.Text = null;
            this.label55.Text = null;
            this.label56.Text = null;
            this.label57.Text = null;
            this.label74.Text = null;
            this.label75.Text = null;

            //提示信息
            this.label35.Text = null;

            //生产信息
            this.label58.Text = null;
            this.label59.Text = null;

            //条码信息
            this.label42.Text = null;
            this.label43.Text = null;
            this.label45.Text = null;
            this.label46.Text = null;
            this.label47.Text = null;
            this.label67.Text = null;
            this.label77.Text = null;
            this.label136.Text = null;
            this.label134.Text = null;
            this.label128.Text = null;
            this.label132.Text = null;
            this.label124.Text = null;
            this.label120.Text = null;
            this.label131.Text = null;
            this.label116.Text = null;
            this.label162.Text = null;

            //序号箱数栈板数
            this.label44.Text = null;
            this.label87.Text = null;
            this.label89.Text = null;
            this.label93.Text = null;

            //扫描框
            this.Mac_input.Visible = false;
            this.Mac_reprint_input.Visible = false;
            this.EQP_input.Visible = false;
            this.Power_input.Visible = false;

        }


        //扫描前数据初始化
        private void ScanDataInitial()
        {
            //条码信息清除
            this.label42.Text = null;
            this.label43.Text = null;
            this.label45.Text = null;
            this.label46.Text = null;
            this.label47.Text = null;
            this.label67.Text = null;
            this.label77.Text = null;
            this.label136.Text = null;
            this.label134.Text = null;
            this.label128.Text = null;
            this.label132.Text = null;
            this.label124.Text = null;
            this.label120.Text = null;
            this.label131.Text = null;
            this.label116.Text = null;

            //提示信息
            this.label35.Text = null;

            //当前站位
            this.label55.Text = null;
            this.label74.Text = null;

            //表格
            this.dataGridView1.DataSource = null;
            this.Itype_dataGridView.DataSource = null;
            this.QR_dataGridView.DataSource = null;
            this.Box_dataGridView.DataSource = null;
            this.IItype_dataGridView.DataSource = null;

            //richtext
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;
            
        }

        //清除扫描框信息
        private void getScanTextboaClear()
        {
            //扫描框
            this.Mac_input.Text = null;
            this.Mac_reprint_input.Text = null;
            this.EQP_input.Text = null;
            this.Power_input.Text = null;
        }

        #endregion


        #region 3、锁定事件

        //工单锁定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "PrintSet.ini"))
                {
                    MessageBox.Show(AppDomain.CurrentDomain.BaseDirectory + "PrintSet.ini" + "文件不存在");
                    return;
                }

                //读取配置文件，选择打印方式
                string[] lines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "PrintSet.ini", System.Text.Encoding.GetEncoding("GB2312"));

                foreach (string line in lines)
                {
                    if (line.Contains("PrintChange"))
                    {
                        PrintChange = line.Substring(line.IndexOf("=") + 1).Trim();
                    }

                    //if (line.Contains("Itype_PrintDelay"))
                    //{
                    //    Itype_PrintDelay = line.Substring(line.IndexOf("=") + 1).Trim();
                    //}

                    if (line.Contains("IItype_PrintDelay"))
                    {
                        IItype_PrintDelay = line.Substring(line.IndexOf("=") + 1).Trim();
                    }

                    //if (line.Contains("Box_PrintDelay"))
                    //{
                    //    BOX_PrintDelay = line.Substring(line.IndexOf("=") + 1).Trim();
                    //}

                    //if (line.Contains("QR_PrintDelay"))
                    //{
                    //    QR_PrintDelay = line.Substring(line.IndexOf("=") + 1).Trim();
                    //}
                }

                this.Power_input.Visible = false;//确认打印方式前不显示电源输入

                if (str.Contains("FH103"))
                {
                    //显示预览按钮
                    this.Itype_view.Visible = true;
                    this.QR_view.Visible = true;
                    this.Box_view.Visible = true;
                    this.IItype_view.Visible = true;

                    //显示打印按钮
                    this.Itype_print.Visible = true;
                    this.QR_print.Visible = true;
                    this.Box_print.Visible = true;
                    this.IItype_print.Visible = true;

                    //打印机名称可设定
                    this.Itype_printset.Enabled = true;
                    this.QR_printset.Enabled = true;
                    this.Box_printset.Enabled = true;
                    this.IItype_printset.Enabled = true;

                    this.groupBox23.Visible = true;

                    this.tabPage4.Parent = tabControl2;

                    this.comboBox4.Text = "0.3";

                    //获取调试开始时间
                    tt_reprintstattime = DateTime.Now;
                }

                Boolean tt_flag = getChoiceTaskcode();

                if (tt_flag && PrintChange != "")
                {
                    this.textBox1.Enabled = false;
                    this.textBox9.Enabled = false;

                    if (tt_parenttask != "小型化方案")
                    {
                        this.EQP_input.Visible = true;
                        this.EQP_input.Enabled = true;
                        this.Mac_input.Visible = true;
                        this.Mac_input.Enabled = false;
                    }
                    else
                    {
                        this.Mac_input.Visible = true;
                        this.Mac_input.Enabled = true;
                    }

                    if (int.Parse(PrintChange) >= 2)
                    {
                        this.label164.Visible = true;
                        this.label165.Visible = true;
                        this.label166.Visible = true;
                        this.label167.Visible = true;
                        this.label168.Visible = true;
                        this.checkBox7.Visible = true;
                        this.textBox29.Visible = true;
                        this.textBox30.Visible = true;
                        this.Power_input.Visible = true;
                        this.Power_input.Enabled = false;
                        this.groupBox3.Visible = true;
                    }

                    this.Mac_reprint_input.Visible = true;

                    this.checkBox2.Checked = true;
                    this.checkBox3.Checked = true;
                    this.checkBox4.Checked = true;
                    this.checkBox7.Checked = true;

                    GetProductNumInfo();  //生产信息
                    getPalletBoxNo(this.label86.Text,this.label85.Text,this.label44.Text,this.label12.Text);
                    MessageBox.Show("---OK---，这是烽火生产序列号，左边是子工单，右边是总工单，不要填错");
                }
                else
                {
                    MessageBox.Show("工单选择失败 或 打印模式设置不正确");
                    ClearLabelInfo();
                    ScanDataInitial();
                }
            }
            else
            {
                this.textBox1.Enabled = true;
                this.textBox9.Enabled = true;
                this.Mac_input.Visible = false;
                this.Mac_reprint_input.Visible = false;
                this.EQP_input.Visible = false;
                this.Power_input.Visible = false;
                this.checkBox1.Checked = false;
                this.comboBox5.Text = "";
                this.textBox27.Text = "";
                this.textBox28.Text = "";
                this.comboBox5.Enabled = true;
                this.textBox27.Enabled = true;
                this.textBox28.Enabled = true;
                this.groupBox22.Visible = false;
                this.groupBox23.Visible = false;
                this.groupBox8.Visible = true;
                this.groupBox9.Visible = true;
                this.dataGridView1.Visible = true;
                //隐藏预览按钮
                this.Itype_view.Visible = false;
                this.QR_view.Visible = false;
                this.Box_view.Visible = false;
                this.IItype_view.Visible = false;
                //隐藏打印按钮
                this.Itype_print.Visible = false;
                this.QR_print.Visible = false;
                this.Box_print.Visible = false;
                this.IItype_print.Visible = false;
                //打印机名称不可设定
                this.Itype_printset.Enabled = false;
                this.QR_printset.Enabled = false;
                this.Box_printset.Enabled = false;
                this.IItype_printset.Enabled = false;
                this.tabPage4.Parent = null;
                this.tabPage3.Parent = tabControl2;
                this.groupBox3.Visible = false;
                this.checkBox2.Checked = false;
                this.checkBox3.Checked = false;
                this.checkBox4.Checked = false;
                this.checkBox7.Checked = false;
                this.label164.Visible = false;
                this.label165.Visible = false;
                this.label166.Visible = false;
                this.label167.Visible = false;
                this.label168.Visible = false;
                this.textBox29.Visible = false;
                this.textBox30.Visible = false;
                this.checkBox7.Visible = false;
                tt_shanghailabel = "";
                tt_parenttask = "";
                tt_hebeiItypedate = "";
                tt_power_old = "";
                ClearLabelInfo();
                ScanDataInitial();
                getScanTextboaClear();
            }
        }


        //MAC过站锁定
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked)
            {
                this.textBox3.Enabled = false;
                this.textBox4.Enabled = false;
            }
            else
            {
                this.textBox3.Enabled = true;
            }
        }


        //设备标示符锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.textBox10.Enabled = false;
                this.textBox8.Enabled = false;
            }
            else
            {
                this.textBox10.Enabled = true;
            }
        }


        //MAC重打锁定
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked)
            {
                this.textBox6.Enabled = false;
                this.textBox5.Enabled = false;
            }
            else
            {
                this.textBox6.Enabled = true;
            }
        }

        //电源锁定
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox7.Checked)
            {
                this.textBox30.Enabled = false;
            }
            else
            {
                this.textBox30.Enabled = true;
            }
        }

        #endregion


        #region 4、工单选择

        //工单选择
        private bool getChoiceTaskcode()
        {
            Boolean tt_flag = false;
            string tt_task1 = this.textBox1.Text.Trim().ToUpper();//子工单
            string tt_task2 = this.textBox9.Text.Trim().ToUpper();//总工单

            string tt_productname = "";
            tt_computermac = Dataset1.GetHostIpName();

            #region 第一步：子工单检查
            bool tt_flag1 = false;
            string tt_sql1 = "select  tasksquantity,product_name,areacode,fec,convert(varchar, taskdate, 102) fdate,customer,flhratio,Gyid,Tasktype,Pon_name,Gyid2,Parenttask," +
                             "convert(varchar, taskdate, 111) fdate,softwareversion,fhcode " +
                             "from odc_tasks where taskscode = '" + tt_task1 + "' ";
             DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                tt_flag1 = true;
                this.label12.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                tt_productname = ds1.Tables[0].Rows[0].ItemArray[1].ToString(); //产品名称
                this.label14.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区
                this.label17.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString(); //EC编码
                this.label54.Text = ds1.Tables[0].Rows[0].ItemArray[7].ToString();  //主要流程配置
                this.label15.Text = ds1.Tables[0].Rows[0].ItemArray[8].ToString();  //物料编码
                tt_ponname = ds1.Tables[0].Rows[0].ItemArray[9].ToString();  //PON 类型
                tt_gyid_Old = ds1.Tables[0].Rows[0].ItemArray[10].ToString();  //次级流程配置
                tt_parenttask = ds1.Tables[0].Rows[0].ItemArray[11].ToString();  //小型化产品用参数

                string tt_tasksdate = ds1.Tables[0].Rows[0].ItemArray[4].ToString(); //制造单日期
                string[] tt_datetemp = tt_tasksdate.Split('.');
                tt_hebeiItypedate = tt_datetemp[0] + "年-" + tt_datetemp[1] + "月-" + tt_datetemp[2] + "日";//河北联通定制标签用

                this.label169.Text = ds1.Tables[0].Rows[0].ItemArray[12].ToString();  //彩盒打印日期
                this.label160.Text = ds1.Tables[0].Rows[0].ItemArray[13].ToString();  //软件版本

                tt_power_old = ds1.Tables[0].Rows[0].ItemArray[14].ToString().Trim();//旧电源适配器标识

                tt_gyid_Use = "";

                tt_allprocesses = null;
                tt_partprocesses = null;
                tt_routdataset = null;
                tt_allroutdataset = null;

                if (tt_productname == "HG6201G" || tt_productname == "HG6201GW" || tt_productname == "HG6201GS")
                {
                    this.label13.Text = "HG6201M";
                }
                else
                {
                    this.label13.Text = tt_productname;
                }

                this.textBox4.Enabled = false;
                this.textBox5.Enabled = false;
                this.textBox8.Enabled = false;

            }
            else
            {
                MessageBox.Show("没有查询到子工单:"+tt_task1+"，请确认！");
            }

            #endregion

            //第一步附一 文字变量查询
            bool tt_flag1_1 = false;
            if (tt_flag1)
            {
                if (tt_ponname == "GPON")
                {

                    this.label159.Text = "吉比特";
                    tt_flag1_1 = true;
                }
                else if (tt_ponname == "EPON")
                {

                    this.label159.Text = "以太网";
                    tt_flag1_1 = true;
                }
                else
                {
                    MessageBox.Show("文字变量无法匹配，请确认制造单下单信息，或产品是否为PON产品");
                }
            }

            //第一步附二 电源信息查询
            bool tt_flag1_2 = false;
            if (tt_flag1_1)
            {
                string tt_power_search = this.label13.Text;
                if (this.label13.Text == "HG6201M" && tt_power_old == "1.5")
                {
                    tt_power_search = "HG6201M_OLD";
                }

                string tt_sql1_2 = "select fpwmodel,wifi,fcolor from odc_dypowertype where ftype = '" + tt_power_search + "' ";
                DataSet ds1_2 = Dataset1.GetDataSetTwo(tt_sql1_2, tt_conn);

                if (ds1_2.Tables.Count > 0 && ds1_2.Tables[0].Rows.Count > 0)
                {
                    this.label168.Text = ds1_2.Tables[0].Rows[0].ItemArray[0].ToString(); //电源适配器特征码
                    this.label155.Text = ds1_2.Tables[0].Rows[0].ItemArray[1].ToString(); //产品特征
                    this.label156.Text = ds1_2.Tables[0].Rows[0].ItemArray[2].ToString(); //产品颜色

                    this.textBox29.Text = this.label168.Text;

                    tt_flag1_2 = true;
                }
                else
                {
                    MessageBox.Show("没有电源适配器信息，请确认数据库电源表");
                }
            }

            #region 第二步、流程检查
            bool tt_flag2 = false;
            
            string tt_gyid1 = this.label54.Text;

            if (tt_flag1_2)
            {
                if (!tt_gyid1.Equals(""))
                {
                    bool tt_flag201 = GetNextCode(tt_task1, str, tt_gyid1);
                    if (tt_flag201)
                    {
                        tt_flag2 = true;
                    }
                }
                else
                {
                    MessageBox.Show("该子工单" + tt_task1 + "没有配置流程，请检查");
                }
            }
            #endregion
           
            #region 第三步、EC信息检查
            bool tt_flag3 = false;
            if(tt_flag1_2)
            {
                string tt_ec = this.label17.Text;
                string tt_sql3_1 = "select  docdesc,Fpath03,Fdata03,Fmd03  from odc_ec where zjbm = '" + tt_ec + "' ";
                string tt_sql3_2 = "select  docdesc,Fpath07,Fdata07,Fmd07  from odc_ec where zjbm = '" + tt_ec + "' ";
                string tt_sql3_3 = "select  docdesc,Fpath04,Fdata04,Fmd04  from odc_ec where zjbm = '" + tt_ec + "' ";
                string tt_sql3_4 = "select  docdesc,Fpath10,Fdata10,Fmd10  from odc_ec where zjbm = '" + tt_ec + "' ";

                if ((tt_productname == "HG6201M" || tt_productname == "HG6821M") && this.label14.Text == "安徽")
                {
                    tt_sql3_2 = "select  docdesc,Fpath09,Fdata09,Fmd09  from odc_ec where zjbm = '" + tt_ec + "' ";
                }

                if (tt_parenttask == "小型化方案")
                {
                    tt_sql3_4 = "select  docdesc,Fpath09,Fdata09,Fmd09  from odc_ec where zjbm = '" + tt_ec + "' ";

                    this.IItype_view.Text = "预览彩盒二";
                    this.IItype_print.Text = "打印彩盒二";
                    this.IItype_label.Text = "彩盒二";
                    this.label145.Text = " 彩盒二";
                    this.label121.Text = "彩盒二";
                }
                else
                {
                    this.IItype_view.Text = "预览II型标签";
                    this.IItype_print.Text = "打印II型标签";
                    this.IItype_label.Text = "II型标签";
                    this.label145.Text = "II型标签";
                    this.label121.Text = "II型标签";
                }

                DataSet ds3_1 = Dataset1.GetDataSet(tt_sql3_1, tt_conn);
                DataSet ds3_2 = Dataset1.GetDataSet(tt_sql3_2, tt_conn);
                DataSet ds3_3 = Dataset1.GetDataSet(tt_sql3_3, tt_conn);
                DataSet ds3_4 = Dataset1.GetDataSet(tt_sql3_4, tt_conn);

                if ((ds3_1.Tables.Count > 0 && ds3_1.Tables[0].Rows.Count > 0) && (ds3_2.Tables.Count > 0 && ds3_2.Tables[0].Rows.Count > 0)
                    && (ds3_3.Tables.Count > 0 && ds3_3.Tables[0].Rows.Count > 0) && (ds3_4.Tables.Count > 0 && ds3_4.Tables[0].Rows.Count > 0))
                {
                    this.label20.Text = ds3_1.Tables[0].Rows[0].ItemArray[0].ToString(); //EC描述
                    this.label18.Text = ds3_1.Tables[0].Rows[0].ItemArray[2].ToString(); //I型标签数据类型
                    tt_path1 = Application.StartupPath + ds3_1.Tables[0].Rows[0].ItemArray[1].ToString(); //I型标签模板路径
                    this.label19.Text = ds3_1.Tables[0].Rows[0].ItemArray[1].ToString(); //I型标签模板路径

                    this.label37.Text = ds3_2.Tables[0].Rows[0].ItemArray[2].ToString(); //二维码数据类型
                    tt_path2 = Application.StartupPath + ds3_2.Tables[0].Rows[0].ItemArray[1].ToString(); //二维码模板路径
                    this.label41.Text = ds3_2.Tables[0].Rows[0].ItemArray[1].ToString(); //二维码模板路径

                    this.label148.Text = ds3_3.Tables[0].Rows[0].ItemArray[2].ToString(); //彩盒标签数据类型
                    tt_path3 = Application.StartupPath + ds3_3.Tables[0].Rows[0].ItemArray[1].ToString(); //彩盒标签模板路径
                    this.label152.Text = ds3_3.Tables[0].Rows[0].ItemArray[1].ToString(); //彩盒标签模板路径

                    this.label149.Text = ds3_4.Tables[0].Rows[0].ItemArray[2].ToString(); //II型标签数据类型
                    tt_path4 = Application.StartupPath + ds3_4.Tables[0].Rows[0].ItemArray[1].ToString(); //II型标签模板路径
                    this.label150.Text = ds3_4.Tables[0].Rows[0].ItemArray[1].ToString(); //II型标签模板路径

                    tt_flag3 = true;
                }
                else
                {
                    MessageBox.Show("没有找到子工单:"+tt_task1+"的EC代码"+tt_ec+",的配置信息，请确认！");
                }

            }
            #endregion
            

            #region 第四步、总工单与子工单包容性检查
            bool tt_flag4 = false;
            if(tt_flag3)
            {
                if (tt_task1.Contains(tt_task2))
                {
                    tt_flag4 = true;
                }
                else
                {
                    MessageBox.Show("总工单：" + tt_task2 + ",与子工单：" + tt_task1 + ",不一致,请检查！");
                }
            }
            #endregion
            

            #region 第五步、查找总工单是否存在
            bool tt_flag5 = false;
            if(tt_flag4)
            {
                string tt_sql5 = "select count(1),0,0 from odc_tasks " +
                                         "where taskscode = '" + tt_task2 + "' ";

                string[] tt_array5 = new string[3];
                tt_array5 = Dataset1.GetDatasetArray(tt_sql5, tt_conn);
                if (tt_array5[0] == "1")
                {
                    tt_flag5 = true;
                }
                else
                {
                    MessageBox.Show("工单表中没有找到该总工单：" + tt_task2 + "，请确认！");
                }

            }
            #endregion


            #region 第六步、特征码查询
            Boolean tt_flag6 = false;
            if (tt_flag5)
            {
                string tt_sql6 = "select top(1)maclable,smtaskscode,shelllable from odc_alllable where taskscode = '" + tt_task2 + "' ";
                DataSet ds6 = Dataset1.GetDataSetTwo(tt_sql6, tt_conn);

                if (ds6.Tables.Count > 0 && ds6.Tables[0].Rows.Count > 0)
                {
                    string tt_maccheck = ds6.Tables[0].Rows[0].ItemArray[0].ToString(); //随机取该制造单关联的一个MAC
                    string tt_smtaskscodecheck = ds6.Tables[0].Rows[0].ItemArray[1].ToString(); //随机取该制造单关联的一个产品的设备标识号
                    string tt_shelllable = ds6.Tables[0].Rows[0].ItemArray[2].ToString(); //随机取该制造单关联的一个产品的GPSN

                    this.textBox4.Text = tt_maccheck.Substring(0, 6); //获取该制造单MAC特征码
                    string tt_smtaskscodecheck1 = Regex.Replace(tt_smtaskscodecheck, " ", "");
                    string tt_shelllable1 = Regex.Replace(tt_shelllable,"-","");

                    this.textBox8.Text = tt_smtaskscodecheck1.Substring(0, 6) + "," + tt_shelllable1.Substring(0, 6); //获取该制造单设备标识码特征码

                    this.textBox5.Text = this.textBox4.Text; //重打MAC的特征获取

                    tt_flag6 = true;
                }
                else
                {
                    string tt_sql6_1 = "select top(1)maclable,smtaskscode,shelllable from odc_alllable where taskscode = '" + tt_task1 + "' ";
                    DataSet ds6_1 = Dataset1.GetDataSetTwo(tt_sql6_1, tt_conn);

                    if (ds6_1.Tables.Count > 0 && ds6_1.Tables[0].Rows.Count > 0)
                    {
                        string tt_maccheck = ds6_1.Tables[0].Rows[0].ItemArray[0].ToString(); //随机取该制造单关联的一个MAC
                        string tt_smtaskscodecheck = ds6_1.Tables[0].Rows[0].ItemArray[1].ToString(); //随机取该制造单关联的一个产品的设备标识号
                        string tt_shelllable = ds6_1.Tables[0].Rows[0].ItemArray[2].ToString(); //随机取该制造单关联的一个产品的GPSN

                        this.textBox4.Text = tt_maccheck.Substring(0, 6); //获取该制造单MAC特征码
                        string tt_smtaskscodecheck1 = Regex.Replace(tt_smtaskscodecheck, " ", "");
                        string tt_shelllable1 = Regex.Replace(tt_shelllable, "-", "");

                        this.textBox8.Text = tt_smtaskscodecheck1.Substring(0, 6) + "," + tt_shelllable1.Substring(0, 6); //获取该制造单设备标识码特征码

                        this.textBox5.Text = this.textBox4.Text; //重打MAC的特征获取

                        tt_flag6 = true;
                    }
                    else
                    {
                        MessageBox.Show("没有MAC相关信息，请确认该制造单是否有关联产品");
                    }
                }
            }
            #endregion


            #region 第八步、查找流水号配置信息
            bool tt_flag8 = false;
            if(tt_flag6)
            {
                bool tt_flag8_1 = false;

                string tt_sql8 = "select count(1),min(hostqzwh),min(hostmax) from ODC_HOSTLABLEOPTIOAN " +
                                     "where taskscode = '" + tt_task1 + "' ";
                string[] tt_array8 = new string[3];
                tt_array8 = Dataset1.GetDatasetArray(tt_sql8, tt_conn);
                if (tt_array8[0] == "1")
                {
                    tt_flag8_1 = true;
                    this.label65.Text = tt_array8[1].ToUpper();
                    this.label44.Text = tt_array8[2];
                }
                else
                {
                    MessageBox.Show("没有找到子工单" + tt_task1 + "的串号表配置信息，或有子工单号配置表重复,查询结果返回值" + tt_array8[0] + "请确认！");
                }

                if (tt_productname == "HG6821M" && this.label14.Text == "上海" && tt_flag8_1)
                {
                    this.QR_view.Text = "预览资产编码";
                    this.QR_print.Text = "打印资产编码";
                    this.QR_label.Text = "资产编码";
                    this.label115.Text = "资产码";
                    this.label39.Text = "资产编码";

                    string tt_sql8_1 = "select count(1),min(hostmode),min(hostmax) from ODC_HOSTLABLEOPTIOAN " +
                                     "where taskscode = '" + tt_task1 + "' ";
                    string[] tt_array8_1 = new string[3];
                    tt_array8_1 = Dataset1.GetDatasetArray(tt_sql8_1, tt_conn);
                    if (tt_array8_1[0] == "1")
                    {
                        tt_flag8 = true;
                        tt_shanghailabel = tt_array8_1[1].ToUpper().Trim();
                    }
                    else
                    {
                        MessageBox.Show("没有找到子工单" + tt_task1 + "的上海资产编码配置信息，或有子工单号配置表重复,查询结果返回值" + tt_array8[0] + "请确认！");
                    }
                }
                else if ((tt_productname == "HG6201M" || tt_productname == "HG6821M") && this.label14.Text == "安徽" && tt_flag8_1)
                {
                    this.QR_view.Text = "预览SN码";
                    this.QR_print.Text = "打印SN码";
                    this.QR_label.Text = "SN码标签";
                    this.label115.Text = "SN 码";
                    this.label39.Text = "SN码标签";
                    tt_flag8 = true;
                }                
                else
                {
                    this.QR_view.Text = "预览二维码";
                    this.QR_print.Text = "打印二维码";
                    this.QR_label.Text = "二维码标签";
                    this.label115.Text = "二维码";
                    this.label39.Text = "二维码标签";
                    tt_flag8 = true;
                }
            }
            #endregion
            

            #region 第九步、模板路径检查
            bool tt_flag9 = false;
            if(tt_flag8)
            {
                bool tt_flag9_1 = getPathIstrue(tt_path1);
                bool tt_flag9_2 = getPathIstrue(tt_path2);
                bool tt_flag9_3 = getPathIstrue(tt_path3);
                bool tt_flag9_4 = getPathIstrue(tt_path4);
                if ((tt_flag9_1 && tt_flag9_2 && tt_flag9_3 && tt_flag9_4) || (tt_flag9_1 && this.label41.Text == "" && tt_flag9_3 && tt_flag9_4))
                {
                    tt_flag9 = true;
                }
                else if (!tt_flag9_1)
                {
                    MessageBox.Show(" 找不到模板文件：" + tt_path1 + "，请确认！");
                }
                else if (!tt_flag9_2)
                {
                    MessageBox.Show(" 找不到模板文件：" + tt_path2 + "，请确认！");
                }
                else if (!tt_flag9_3)
                {
                    MessageBox.Show(" 找不到模板文件：" + tt_path3 + "，请确认！");
                }
                else if (!tt_flag9_4)
                {
                    MessageBox.Show(" 找不到模板文件：" + tt_path4 + "，请确认！");
                }
            }
            #endregion

            
            #region 第十步、检验MD5码
            //bool tt_flag10 = false;
            //if( tt_flag9)
            //{
            //    string tt_md6 = GetMD5HashFromFile(tt_path);

            //    if (tt_md5 == tt_md6)
            //    {
            //        tt_flag10 = true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("系统设定模板MD5码: '" + tt_md5 + "'与你使用模板的MD5码：'" + tt_md6 + "'不一致，文件路径:"+tt_path+",请确认！");
            //    }
            //}
            #endregion

            
            #region 第十一步、查找串号约束条件
            bool tt_flag11 = false;
            string tt_serialsets = "";
            if(tt_flag9)
            {
                string tt_area = this.label14.Text;
                tt_serialsets = GetProductSerialSet(tt_area,tt_productname,this.label15.Text);
                if (!tt_serialsets.Equals(""))
                {
                    tt_flag11 = true;
                    this.label16.Text = tt_serialsets;
                }
                else
                {
                    MessageBox.Show("获取串号约束值失败");
                }
            }
            #endregion

            
            #region 第十二步、串号约束检查
            bool tt_flag12 = false;
            string tt_taskserial = this.label65.Text;
            if(tt_flag11)
            {
                if (tt_taskserial.Contains(tt_serialsets))
                {
                    tt_flag12 = true;
                }
                else
                {
                    MessageBox.Show("工单"+tt_task1+"串号设置为:"+tt_taskserial+"，与串号约束:"+tt_serialsets+",不匹配");
                }

            }
            #endregion

            
            #region 第十三步、待测站位及序列号检查
            bool tt_flag13 = false;
            string tt_testcode = this.label56.Text;
            string tt_codeserial = this.label75.Text;
            if (tt_flag12)
            {
                if (tt_testcode.Equals("") || tt_codeserial.Equals(""))
                {
                    MessageBox.Show("流程的待测站位，或流程的序列号为空，请检查流程设置");
                }
                else
                {
                    tt_flag13 = true;
                }
            }
            #endregion


            #region 第十四步、获取站位流程集
            bool tt_flag14 = false;
            if(tt_flag13)
            {
                string tt_sql14 = "select pxid from odc_routing  where pid = " + tt_gyid1 + "  and LCBZ > 1 and LCBZ < '" + tt_codeserial + "' ";
                tt_routdataset = Dataset1.GetDataSetTwo(tt_sql14, tt_conn);
                if (tt_routdataset.Tables.Count > 0 && tt_routdataset.Tables[0].Rows.Count > 0)
                {
                    tt_flag14 = true;
                    tt_allprocesses = Dataset2.getGyidAllProcess(tt_gyid1, tt_conn);
                    tt_partprocesses = Dataset2.getGyidPartProcess(tt_routdataset);
                    tt_allroutdataset = Dataset2.getGyidAllProcessDt(tt_gyid1, tt_conn);
                }
                else
                {
                    MessageBox.Show("没有找到流程:" + tt_gyid1 + "，的流程数据集Dataset，请流程设置！");
                }
            }
            #endregion


            #region 第十五步、获取装箱设置
            bool tt_flag15 = false;
            if (tt_flag14)
            {
                string tt_sql15 = "select count(1),min(fpliietset),min(fboxset) from odc_dypowertype " +
                                  "where Ftype = '"+tt_productname+"' ";
                string[] tt_array15 = new string[3];
                tt_array15 = Dataset1.GetDatasetArray(tt_sql15, tt_conn);
                if (tt_array15[0] == "1")
                {
                    tt_flag15 = true;
                    this.label86.Text = tt_array15[1];
                    this.label85.Text = tt_array15[2];
                }
                else
                {
                    MessageBox.Show("没有找到子产品" + tt_productname + "的箱号栈板号配置信息，请确认！");
                }

            }
            #endregion


            #region 第十六步、箱号栈板号设置检查
            bool tt_flag16 = false;
            string tt_pallletset = this.label86.Text;
            string tt_boxset = this.label85.Text;
            if (tt_flag15)
            {
                if (tt_pallletset.Equals("") || tt_boxset.Equals(""))
                {
                    MessageBox.Show("该产品型号" + tt_productname + "在电源配置表中没有配置栈板数和装箱数设置，请检查");
                }
                else
                {
                    tt_flag16 = true;
                }
            }
            #endregion
            

            #region  最后判断
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag8 &&
                 tt_flag9 && tt_flag11 && tt_flag12 && tt_flag13 && tt_flag14 && tt_flag15 && tt_flag16)
            {
                tt_flag = true;
            }
            #endregion

            return tt_flag;
        }

        #endregion


        #region 5、辅助功能

        //richtext加记录
        private void setRichtexBox(string tt_textinfor)
        {
            this.richTextBox1.Text = this.richTextBox1.Text + tt_textinfor + "\n";
        }

        //lable提示信息
        private void PutLableInfor(string tt_lableinfo)
        {
            this.label35.Text = tt_lableinfo;
        }


        //lable提示信息并记录NG信息
        private void PutLableInfor2(string tt_lableinfo,string tt_task,string tt_mac)
        {
            int tt_int = Dataset2.getNgreasonRecord(tt_task, tt_mac, "生产序号", tt_lableinfo, "3000",tt_conn);
            this.label35.Text = tt_lableinfo+","+tt_int.ToString();
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
        

        //--生产节拍
        private void getProductRhythm(string tt_input)
        {
            if (tt_input == "1") tt_yield = tt_yield + 1;  //输入为1就加1

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
            this.label25.Text = tt_yield.ToString();   //本班产量
            this.label26.Text = tt_time;               //生产时间
            this.label27.Text = tt_avgtime.ToString();  //平均节拍
            this.label28.Text = tt_differtime2;        //实时节拍

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


        //删除文件目录及子文件
        public int DelectDir3(string srcPath)
        {
            int tt_delenum = 0;
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                        tt_delenum++;
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                        tt_delenum++;
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
                MessageBox.Show(e.Message);
            }
            return tt_delenum;
        }



        /// <summary>
        /// 从一个目录将其内容复制到另一目录
        /// </summary>
        /// <param name="directorySource">源目录</param>
        /// <param name="directoryTarget">目标目录</param>
        public int CopyFolderTo2(string directorySource, string directoryTarget)
        {
            int tt_copenum = 0;
            try
            {
                //检查是否存在目的目录  
                if (!Directory.Exists(directoryTarget))
                {
                    Directory.CreateDirectory(directoryTarget);
                }
                //先来复制文件  
                DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
                FileInfo[] files = directoryInfo.GetFiles();
                //复制所有文件  
                foreach (FileInfo file in files)
                {
                    file.CopyTo(Path.Combine(directoryTarget, file.Name));
                    tt_copenum++;
                }
                //最后复制目录  
                DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
                foreach (DirectoryInfo dir in directoryInfoArray)
                {
                    CopyFolderTo2(Path.Combine(directorySource, dir.Name), Path.Combine(directoryTarget, dir.Name));
                    tt_copenum++;
                }
            }
            catch (Exception e)
            {
                //throw;
                MessageBox.Show(e.Message);
            }
            return tt_copenum;
        }



        //计算箱号栈板号
        private void getPalletBoxNo(string tt_palletset,string tt_boxset,string tt_boxlableno,string tt_taskquantity)
        {
            //计算箱号
            decimal tt_unitint = decimal.Parse(tt_boxset);
            decimal tt_snnumber = decimal.Parse(tt_boxlableno);
            decimal tt_boxnum2 = Math.Ceiling(tt_snnumber / tt_unitint);
            string tt_boxnum3 = tt_boxnum2.ToString();
            this.label89.Text = tt_boxnum3;

            //计算栈板号
            decimal tt_palletint = int.Parse(tt_palletset);
            decimal tt_palletnum = Math.Ceiling(tt_snnumber / tt_palletint);
            this.label87.Text = tt_palletnum.ToString();

            //计算欠缺量
            decimal tt_taskquantityint = int.Parse(tt_taskquantity);
            decimal tt_differquantityint = tt_taskquantityint - tt_snnumber;
            this.label93.Text = tt_differquantityint.ToString();
        }

        //取最小值
        static int GetMin(int[] num,int count)
        {
            int min = num[0];
            for (int i = 0; i < count; i++)
            {
                if (min > num[i])
                {
                    min = num[i];
                }
            }
            return min;
        }

        //取最大值
        static int GetMax(int[] num, int count)
        {
            int max = num[0];
            for (int i = 0; i < count; i++)
            {
                if (max < num[i])
                {
                    max = num[i];
                }
            }
            return max;
        }

        //通过生产数筛选工单及序号
        public string GetMinTask(string[] taskscodename, string leftnum, int count)
        {
            string tt_taskname = Regex.Match(taskscodename[0], @"(?<=Q).*?(?=E)").Groups[0].Value;

            for (int i = 0; i < count; i++)
            {
                string tt_task_num = Regex.Match(taskscodename[i], @"(?<=E).*?(?=R)").Groups[0].Value;//数值

                if (tt_task_num == leftnum)
                {
                    tt_taskname = Regex.Match(taskscodename[i], @"(?<=Q).*?(?=E)").Groups[0].Value;//工单
                }
            }
            return tt_taskname;
        }

        //获取0包装工单的数量
        static int GetZeroTask(string[] tt_taskscodename_left, int count)
        {
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                string tt_task_num = Regex.Match(tt_taskscodename_left[i], @"(?<=E).*?(?=R)").Groups[0].Value;//数值

                if (tt_task_num == "0")
                {
                    j++;
                }
            }
            return j;
        }

        //判断是否是数字
        static bool IsNumeric(string value)
        {
            try
            {
                int var1 = Convert.ToInt32(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //字母-数字转换
        static int HostAZ_Num(string AZ)
        {
            int HostNum = 0;
            string[] HostAZ_Temp = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');

            if (("A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z").Contains(AZ))
            {
                for (int i = 0; i < 26; i++)
                {
                    if (AZ == HostAZ_Temp[i])
                    {
                        HostNum = i + 10;
                    }
                }
            }
            else
            {
                HostNum = 0;
            }

            return HostNum;
        }

        //数字-字母转换
        static string HostNum_AZ(int num)
        {
            string HostAZ = "";
            string[] HostAZ_Temp = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');

            if (num >= 10 && num < 36)
            {
                HostAZ = HostAZ_Temp[num - 10];
            }
            else
            {
                HostAZ = "0";
            }

            return HostAZ;
        }

        #endregion
        

        #region 6、数据功能

        //获取生产信息
        private void GetProductNumInfo()
        {
            string tt_sql = "select count(1),count(case when hprinttime is not null then 1 end),0 " +
                            "from odc_alllable where taskscode = '" + this.textBox1.Text.Trim().ToUpper() + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label58.Text = tt_array[0];
            this.label59.Text = tt_array[1];
        }
        
        //刷新站位
        private void CheckStation(string tt_mac,string tt_process)
        {
            string tt_sql = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime,fremark " +
                            "from ODC_ROUTINGTASKLIST where pcba_pn = '" + tt_mac + "' order by createtime desc";

            DataSet ds1 = Dataset1.GetDataSet(tt_sql, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView1.DataSource = ds1;
                dataGridView1.DataMember = "Table";

                this.label55.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //当前站位

                //获取流程的顺序值
                string tt_newcode = ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                //string tt_process = this.label54.Text;//改为传入参数，方便操作
                this.label74.Text = GetCodeRoutNum(tt_newcode, tt_process);
            }
        }
                
        //流程检查，获取下一流程
        private bool GetNextCode(string tt_task, string tt_username, string tt_gyid)
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
            //string tt_process = "";
            if (tt_flag1)
            {
                string tt_sql2 = "select count(1),min(b.PXID),min(a.GYID) " +
                                     "from odc_tasks a,odc_routing b " +
                                     "WHERE b.PID = " + tt_gyid + " AND b.LCBZ=1 AND a.TASKSCODE='" + tt_task + "' ";
                string[] tt_array2 = new string[3];
                tt_array2 = Dataset1.GetDatasetArray(tt_sql2, tt_conn);
                if (tt_array2[0] == "1")
                {
                    tt_firstcode = tt_array2[1];
                    tt_ccode = tt_testcode;
                    //tt_process = tt_array2[2];
                    tt_ccodenumber = GetCodeRoutNum(tt_ccode, tt_gyid); //获取站位顺序

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
                                           " where z.pid = " + tt_gyid + " and t.taskscode='" + tt_task + "' " +
                                           " and z.lcbz in( select (lcbz+1) lcbz " +
                                                            "from odc_tasks a,odc_routing b " +
                                                            "where b.pid= " + tt_gyid + " and b.pxid='" + tt_ccode + "' " +
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
                this.label56.Text = tt_ccode;
                this.label57.Text = tt_ncode;
                this.label75.Text = tt_ccodenumber;
            }

            return tt_flag;
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

        //获取串号约束
        private string GetProductSerialSet(string tt_area,string tt_productname,string tt_tasktype)
        {
            string tt_serialset = "";

            string tt_sql = "select count(1),min(hostlable_code),0 from odc_fhspec " +
                            "where aear = '" + tt_area + "' and product_name = '" + tt_productname + "' and product_code = '" + tt_tasktype + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            if (tt_array[0] == "1")
            {
                tt_serialset = tt_array[1];
            }
            else
            {
                MessageBox.Show("串号设置表odc_fhspec中没有找到或有重复的工单对应的地区:" + tt_area + "，产品型号:" + tt_productname + "的串号约束设置,返回值:" + tt_array[0]);
            }

            return tt_serialset;
        }

        //检查MAC或单板，获取工单
        private string GetSnRealTask(string tt_datatype, string tt_sn)
        {
            string tt_taskcode = "";
            string tt_sql = "Select 1,'不确定',1 ";
            string tt_sql1 = "select count(1),min(taskscode),0 from odc_alllable where pcbasn = '" + tt_sn + "' ";
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

        //列表显示工单
        private void SetTaskcodeList()
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
                            "from odc_fhpartitionpass where Fusername = '" + tt_username + "' " ;

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

        //查询重打记录
        private bool CheckPrintRecord(string tt_maclable, string tt_flocal)
        {
            string tt_sql = "select count(1), min(Fname), min(fmaclable) " +
                            "from odc_lablereprint where fmaclable = '" + tt_maclable + "'" +
                            "and flocal = '" + tt_flocal + "'";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArrayTwo(tt_sql, tt_conn);

            if (int.Parse(tt_array[0]) >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //删除条码比对数据
        private int Delete_Check(string tt_mac)
        {
            string tt_deletesql = "delete from odc_check_barcode where maclable = '" + tt_mac + "'";
            int tt_Checknum = Dataset1.ExecCommand(tt_deletesql, tt_conn);
            return tt_Checknum;
        }

        //产品强制锁死 alllable
        private int Lock_alllable(string tt_tasks, string tt_mac)
        {
            string tt_locksql = "update odc_alllable set taskscode = replace(taskscode,'" + (tt_tasks).Substring(0, 2) + "'" + 
                                ",'双胞胎"+ (tt_tasks).Substring(0, 2) + "') where maclable = '" + tt_mac + "'";
            int tt_Checknum = Dataset1.ExecCommand(tt_locksql, tt_conn);
            return tt_Checknum;
        }

        //产品强制锁死 routing 表
        private int Lock_routing(string tt_tasks, string tt_mac)
        {
            string tt_locksql = "update odc_routingtasklist set taskscode = replace(taskscode,'" + (tt_tasks).Substring(0, 2) + "'" +
                                ",'双胞胎" + (tt_tasks).Substring(0, 2) + "') where pcba_pn = '" + tt_mac + "' and napplytype is null";
            int tt_Checknum = Dataset1.ExecCommand(tt_locksql, tt_conn);
            return tt_Checknum;
        }

        //产品强制锁死 package 表
        private int Lock_package(string tt_mac)
        {
            string tt_locksql = "update odc_package set pasn = replace(pasn,'XZ','双胞XZ') where pasn in " +
                                "(select pcbasn from odc_alllable where maclable = '" + tt_mac + "')";
            int tt_Checknum = Dataset1.ExecCommand(tt_locksql, tt_conn);
            return tt_Checknum;
        }

        //判断物料表ID值
        private bool GetMaterialIdinfor(string tt_id)
        {
            Boolean tt_flag = false;

            string tt_sql = "select COUNT(1),0,0 from odc_traceback where Fid = " + tt_id;
            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            if (tt_array[0] == "0")
            {
                tt_flag = true;
            }

            return tt_flag;
        }

        #endregion


        #region 7、MAC数据查询
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

            string tt_sn1 = this.textBox12.Text.Trim();
            string tt_sn = tt_sn1.Replace("-", "");

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
                string tt_sql2 = "select ID,ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime 创建时间,enddate 完成时间,fremark  备注 " +
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
            this.textBox12.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;
        }

        //订单查询确定(不用）
        private void button6_Click(object sender, EventArgs e)
        {
            this.dataGridView6.DataSource = null;

            string tt_task = this.textBox13.Text.Trim();


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

        //工单查询确定
        private void button6_Click_1(object sender, EventArgs e)
        {
            this.dataGridView6.DataSource = null;
            this.label79.Text = null;
            string tt_task = this.textBox13.Text.Trim();

            string tt_host = "";
            if( this.textBox16.Text != "")
            {
                tt_host = " and  hostlable = '" + this.textBox16.Text + "'";
            }

            string tt_mac = "";
            if (this.textBox17.Text != "")
            {
                tt_mac = " and maclable = '" + this.textBox17.Text + "'";
            }

            string tt_sql1 = "select hprintman 总工单,taskscode 子工单, pcbasn 单板号,hostlable 主机条码,maclable MAC, " +
                             "boxlable 生产序列号,Bosasn BOSA, shelllable GPSN, Smtaskscode 串号, Dystlable 电源号, " +
                             "sprinttime 关联时间 " +

                            "from odc_alllable " +
                            "where taskscode = '" + tt_task + "'" + tt_host + tt_mac;

            DataSet ds1 = Dataset1.GetDataSet(tt_sql1, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView6.DataSource = ds1;
                dataGridView6.DataMember = "Table";
                this.label79.Text = ds1.Tables[0].Rows.Count.ToString();
            }
            else
            {
                MessageBox.Show("sorry,没有查询到数据");
            }
        }
        
        //工单查询重置
        private void button7_Click_1(object sender, EventArgs e)
        {
            this.textBox13.Text = null;
            this.textBox16.Text = null;
            this.textBox17.Text = null;
            this.label79.Text = null;
            this.dataGridView6.DataSource = null;
        }
        
        //行号
        private void dataGridView6_RowPostPaint_1(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SolidBrush brushOne = new SolidBrush(Color.Red);
            e.Graphics.DrawString(Convert.ToString(e.RowIndex + 1, System.Globalization.CultureInfo.CurrentUICulture), e.InheritedRowStyle.Font, brushOne, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
        }

        
        #endregion        


        #region 8、获取MD5码

        //MD5码查询 文件选择
        private void button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            //fileDialog.FileName = "D:软件";
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            fileDialog.InitialDirectory = "c:\\";//获取打开选择框的初始目录;
            fileDialog.ShowDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                // MessageBox.Show("已选择文件:" + file, "选择文件提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.textBox15.Text = file;
            }
        }

        //MD5码查询 获取MD5码
        private void button9_Click(object sender, EventArgs e)
        {
            string tt_fliename = this.textBox15.Text;

            string tt_md5 = GetMD5HashFromFile(tt_fliename);

            this.textBox14.Text = tt_md5;
        }

        //MD5码查询 重置
        private void button8_Click(object sender, EventArgs e)
        {
            this.textBox14.Text = null;
            this.textBox15.Text = null;
        }

        #endregion


        #region 9、按钮事件
        //重置
        private void button1_Click(object sender, EventArgs e)
        {
            ScanDataInitial();

            this.Mac_input.Text = null;
            this.Mac_reprint_input.Text = null;
            this.EQP_input.Text = null;
            this.Power_input.Text = null;

            if (tt_parenttask != "小型化方案")
            {
                this.EQP_input.Enabled = true;
                this.Mac_input.Enabled = false;
                this.Power_input.Enabled = false;
                this.Mac_reprint_input.Enabled = true;
                EQP_input.Focus();
                EQP_input.SelectAll();
            }

            if (tt_parenttask == "小型化方案")
            {
                this.Mac_input.Enabled = true;
                this.Power_input.Enabled = false;
                this.Mac_reprint_input.Enabled = true;
                Mac_input.Focus();
                Mac_input.SelectAll();
            }

            SetTaskcodeList();
        }


        //页签切换
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MAC扫描过站
            if (tabControl2.SelectedTab == tabPage3)
            {
                ScanDataInitial();

                this.Mac_input.Text = null;
                this.Mac_reprint_input.Text = null;
                this.EQP_input.Text = null;
                this.Power_input.Text = null;

                if (tt_parenttask != "小型化方案")
                {
                    this.EQP_input.Enabled = true;
                    this.Mac_input.Enabled = false;
                    this.Power_input.Enabled = false;
                    this.Mac_reprint_input.Enabled = true;
                    EQP_input.Focus();
                    EQP_input.SelectAll();
                }

                if (tt_parenttask == "小型化方案")
                {
                    this.Mac_input.Enabled = true;
                    this.Power_input.Enabled = false;
                    this.Mac_reprint_input.Enabled = true;
                    Mac_input.Focus();
                    Mac_input.SelectAll();
                }
            }

            //MAC扫描重打
            if (tabControl2.SelectedTab == tabPage4)
            {
                ScanDataInitial();

                this.Mac_input.Text = null;
                this.Mac_reprint_input.Text = null;
                this.EQP_input.Text = null;
                this.Power_input.Text = null;

                if (tt_parenttask != "小型化方案")
                {
                    this.EQP_input.Enabled = true;
                    this.Mac_input.Enabled = false;
                    this.Power_input.Enabled = false;
                    this.Mac_reprint_input.Enabled = true;
                    EQP_input.Focus();
                    EQP_input.SelectAll();
                }

                if (tt_parenttask == "小型化方案")
                {
                    this.Mac_input.Enabled = true;
                    this.Power_input.Enabled = false;
                    this.Mac_reprint_input.Enabled = true;
                    Mac_input.Focus();
                    Mac_input.SelectAll();
                }
            }
        }

        //预览I型标签
        private void Itype_view_Click(object sender, EventArgs e)//预览
        {
            if (this.Itype_dataGridView.RowCount > 0)
            {
                string tt_prientcode = this.label74.Text;
                string tt_checkcode = this.label75.Text;

                Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                if (tt_flag)
                {
                    GetParaDataPrint1(2);  //预览
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

            Mac_reprint_input.Focus();
            Mac_reprint_input.SelectAll();
        }

        //预览二维码
        private void QR_view_Click(object sender, EventArgs e)//预览
        {
            if (this.QR_dataGridView.RowCount > 0)
            {
                string tt_prientcode = this.label74.Text;
                string tt_checkcode = this.label75.Text;

                Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                if (tt_flag)
                {
                    GetParaDataPrint2(2);  //预览
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

            Mac_reprint_input.Focus();
            Mac_reprint_input.SelectAll();
        }

        //彩盒标签预览
        private void Box_view_Click(object sender, EventArgs e)
        {
            if (this.Box_dataGridView.RowCount > 0)
            {
                string tt_prientcode = this.label74.Text;
                string tt_checkcode = this.label75.Text;

                Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                if (tt_flag)
                {
                    GetParaDataPrint3(2);  //预览
                }
                else
                {
                    MessageBox.Show("当前站位：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",才能重打标签");
                }


            }
            else
            {

                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再预览模板");
            }

            Mac_reprint_input.Focus();
            Mac_reprint_input.SelectAll();
        }

        //II型标签预览
        private void IItype_view_Click(object sender, EventArgs e)
        {
            if (this.IItype_dataGridView.RowCount > 0)
            {
                string tt_prientcode = this.label74.Text;
                string tt_checkcode = this.label75.Text;

                Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                if (tt_flag)
                {
                    GetParaDataPrint4(2);  //预览
                }
                else
                {
                    MessageBox.Show("当前站位：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",才能重打标签");
                }


            }
            else
            {

                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再预览模板");
            }

            Mac_reprint_input.Focus();
            Mac_reprint_input.SelectAll();
        }

        //打印I型标签
        private void Itype_print_Click(object sender, EventArgs e)
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

            if (this.Itype_dataGridView.RowCount > 0)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                string tt_info = "";
                if (str.Contains("FH003"))
                {
                    tt_info = "，待装箱产品需要重新条码比对";
                }
                DialogResult dr = MessageBox.Show("确定要重打I型标签吗，打印信息被记录" + tt_info, "I型标签重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label74.Text;
                    string tt_checkcode = this.label75.Text;
                    string tt_recordmac = this.Mac_reprint_input.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);
                    DataSet tt_dataset1 = Dataset2.getMacAllCodeInfo(tt_recordmac, tt_conn);
                    string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset1);

                    if (tt_flag && tt_nowcode != "9990")
                    {
					    Reprint form1 = new Reprint();
                        form1.StartPosition = FormStartPosition.CenterScreen;
                        form1.ShowDialog();

                        string tt_remark = Dataset1.Context.ContextData["Key1"].ToString();
					
                        GetParaDataPrint1(1);  //打印
                        string tt_host = Gethostlable(tt_recordmac);
                        string tt_taskscode = this.textBox1.Text.Trim().ToUpper();
                        string tt_local = "生产序列I型标签";
                        string tt_username = "";
                        if (str.Contains("FH003"))
                        {
                            tt_username = this.comboBox5.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }
                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac ,tt_remark);

                        if (str.Contains("FH003") && tt_nowcode == "3201")
                        {
                            int delete_checknum = Delete_Check(tt_recordmac);
                            setRichtexBox("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                            PutLableInfor("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                        }

                        //if (str.Contains("FH003") && int.Parse(tt_nowcode) > 3100)
                        //{
                        //    string tt_gyid = this.label54.Text;
                        //    string tt_ccode = this.label55.Text;
                        //    string tt_ncode = "3100";
                        //    bool tt_flag1 = Dataset1.FhUnPassStationI(tt_taskscode, tt_username, tt_recordmac, tt_gyid, tt_ccode, tt_ncode, tt_conn);
                        //    if (tt_flag1 && tt_nowcode == "3201")
                        //    {
                        //        int delete_checknum = Delete_Check(tt_recordmac);
                        //        setRichtexBox("重打完成，产品属于待装箱产品，已退回彩盒站位，产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对,ok");
                        //        PutLableInfor("重打完成，产品属于待装箱产品，已退回彩盒站位，条码比对数据已删除");
                        //    }
                        //    else if (tt_flag1)
                        //    {
                        //        setRichtexBox("重打完成，产品属于彩盒后产品，已退回彩盒站位,ok");
                        //        PutLableInfor("重打完成，产品属于彩盒后产品，已退回彩盒站位");
                        //    }
                        //    else
                        //    {
                        //        setRichtexBox("流程异常，产品未跳转也无法正常流线，请联系工程,NG");
                        //        PutLableInfor("流程异常，产品未跳转也无法正常流线，请联系工程");
                        //    }
                        //}
                    }
                    else
                    {
                        MessageBox.Show("当前站位或序号：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",或装箱产品已打散,才能重打标签");
                    }

                    if (tt_reprintmark == "0")
                    {
                        tt_reprintchang1++;

                        if (tt_reprintchang1 >= 5)
                        {
                            this.checkBox1.Checked = false;
                            MessageBox.Show("非认证打印电脑，已达到打印上限，退出打印模式");
                            tt_reprintchang1 = 0;
                        }
                        else
                        {
                            MessageBox.Show("非认证打印电脑，已打印" + tt_reprintchang1 + "次，本次打印次数剩余" + (5 - tt_reprintchang1) + "次");
                        }
                    }
                }
            }
            else
            {
                PutLableInfor("参数表数据为空，不能打印！");
            }

            Mac_reprint_input.Focus();
            Mac_reprint_input.SelectAll();
            tt_reprintstattime = DateTime.Now;
        }

        //打印二维码标签
        private void QR_print_Click(object sender, EventArgs e)
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

            if (this.QR_dataGridView.RowCount > 0)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                string tt_info = "";
                if (str.Contains("FH003"))
                {
                    tt_info = "，待装箱产品需要重新条码比对";
                }
                DialogResult dr = MessageBox.Show("确定要重打二维码标签吗，打印信息被记录" + tt_info, "二维码标签重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label74.Text;
                    string tt_checkcode = this.label75.Text;
                    string tt_recordmac = this.Mac_reprint_input.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);
                    DataSet tt_dataset1 = Dataset2.getMacAllCodeInfo(tt_recordmac, tt_conn);
                    string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset1);

                    if (tt_flag && tt_nowcode != "9990")
                    {
                        Reprint form1 = new Reprint();
                        form1.StartPosition = FormStartPosition.CenterScreen;
                        form1.ShowDialog();

                        string tt_remark = Dataset1.Context.ContextData["Key1"].ToString();
						
						GetParaDataPrint2(1);  //打印
                        string tt_host = Gethostlable(tt_recordmac);
                        string tt_taskscode = this.textBox1.Text.Trim().ToUpper();
                        string tt_local = "电信二维码";
                        string tt_username = "";
                        if (str.Contains("FH003"))
                        {
                            tt_username = this.comboBox5.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }
                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);

                        if (str.Contains("FH003") && tt_nowcode == "3201")
                        {
                            int delete_checknum = Delete_Check(tt_recordmac);
                            setRichtexBox("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                            PutLableInfor("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                        }                       
                    }
                    else
                    {
                        MessageBox.Show("当前站位：" + tt_prientcode + "必须大于待测站位：" + tt_checkcode + ",才能重打标签");
                    }

                    if (tt_reprintmark == "0")
                    {
                        tt_reprintchang2++;

                        if (tt_reprintchang2 >= 5)
                        {
                            this.checkBox2.Checked = false;
                            MessageBox.Show("非认证打印电脑，已达到打印上限，退出打印模式");
                            tt_reprintchang2 = 0;
                        }
                        else
                        {
                            MessageBox.Show("非认证打印电脑，已打印" + tt_reprintchang2 + "次，本次打印次数剩余" + (5 - tt_reprintchang2) + "次");
                        }
                    }
                }
            }
            else
            {
                PutLableInfor("参数表数据为空，不能打印！");

            }

            Mac_reprint_input.Focus();
            Mac_reprint_input.SelectAll();
            tt_reprintstattime = DateTime.Now;
        }

        //打印彩盒标签
        private void Box_print_Click(object sender, EventArgs e)
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

            if (this.Box_dataGridView.RowCount > 0)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                string tt_info = "";
                if (str.Contains("FH003"))
                {
                    tt_info = "，待装箱产品需要重新条码比对";
                }
                DialogResult dr = MessageBox.Show("确定要重打标签吗，打印信息被记录" + tt_info, "标签重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label74.Text;
                    string tt_checkcode = this.label75.Text;
                    string tt_recordmac = this.Mac_reprint_input.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);
                    DataSet tt_dataset1 = Dataset2.getMacAllCodeInfo(tt_recordmac, tt_conn);
                    string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset1);

                    if (tt_flag && tt_nowcode != "9990")
                    {
                        Reprint form1 = new Reprint();
                        form1.StartPosition = FormStartPosition.CenterScreen;
                        form1.ShowDialog();

                        string tt_remark = Dataset1.Context.ContextData["Key1"].ToString();

                        GetParaDataPrint3(1);  //打印
                        string tt_host = Gethostlable(tt_recordmac);
                        string tt_taskscode = this.textBox1.Text.Trim().ToUpper();
                        string tt_local = "彩盒标签";
                        string tt_username = "";
                        if (str.Contains("FH003"))
                        {
                            tt_username = this.comboBox5.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }
                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);

                        if (str.Contains("FH003") && tt_nowcode == "3201")
                        {
                            int delete_checknum = Delete_Check(tt_recordmac);
                            setRichtexBox("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                            PutLableInfor("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                        }
                    }
                    else
                    {
                        MessageBox.Show("当前站位：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",或装箱产品已打散,才能重打标签");
                    }

                    if (tt_reprintmark == "0")
                    {
                        tt_reprintchang3++;

                        if (tt_reprintchang3 >= 5)
                        {
                            this.checkBox1.Checked = false;
                            MessageBox.Show("非认证打印电脑，已达到打印上限，退出打印模式");
                            tt_reprintchang3 = 0;
                        }
                        else
                        {
                            MessageBox.Show("非认证打印电脑，已打印" + tt_reprintchang3 + "次，本次打印次数剩余" + (5 - tt_reprintchang3) + "次");
                        }
                    }
                }
            }

            Mac_reprint_input.Focus();
            Mac_reprint_input.SelectAll();
            tt_reprintstattime = DateTime.Now;
        }

        //打印II型标签
        private void IItype_print_Click(object sender, EventArgs e)
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

            if (this.IItype_dataGridView.RowCount > 0)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                string tt_info = "";
                if (str.Contains("FH003"))
                {
                    tt_info = "，装箱产品需要重新条码比对";
                }
                DialogResult dr = MessageBox.Show("确定要重打标签吗，打印信息被记录" + tt_info, "标签重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label74.Text;
                    string tt_checkcode = this.label75.Text;
                    string tt_recordmac = this.Mac_reprint_input.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);
                    DataSet tt_dataset1 = Dataset2.getMacAllCodeInfo(tt_recordmac, tt_conn);
                    string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset1);

                    if (tt_flag && tt_nowcode != "9990")
                    {
                        Reprint form1 = new Reprint();
                        form1.StartPosition = FormStartPosition.CenterScreen;
                        form1.ShowDialog();

                        string tt_remark = Dataset1.Context.ContextData["Key1"].ToString();

                        GetParaDataPrint4(1);  //打印
                        string tt_host = Gethostlable(tt_recordmac);
                        string tt_taskscode = this.textBox1.Text.Trim().ToUpper();
                        string tt_local = "II型标签";
                        string tt_username = "";
                        if (str.Contains("FH003"))
                        {
                            tt_username = this.comboBox5.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }
                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);

                        if (str.Contains("FH003") && tt_nowcode == "3201")
                        {
                            int delete_checknum = Delete_Check(tt_recordmac);
                            setRichtexBox("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                            PutLableInfor("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                        }
                    }
                    else
                    {
                        MessageBox.Show("当前站位：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",或装箱产品已打散,才能重打标签");
                    }

                    if (tt_reprintmark == "0")
                    {
                        tt_reprintchang4++;

                        if (tt_reprintchang4 >= 5)
                        {
                            this.checkBox1.Checked = false;
                            MessageBox.Show("非认证打印电脑，已达到打印上限，退出打印模式");
                            tt_reprintchang4 = 0;
                        }
                        else
                        {
                            MessageBox.Show("非认证打印电脑，已打印" + tt_reprintchang4 + "次，本次打印次数剩余" + (5 - tt_reprintchang4) + "次");
                        }
                    }
                }
            }

            Mac_reprint_input.Focus();
            Mac_reprint_input.SelectAll();
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
                string tt_sql1 = "select fusername from odc_fhpartitionpass where fdepart in ('生产','0') and fpermission in ('2','0') order by id";
                DataSet ds1 = Dataset1.GetDataSet(tt_sql1, tt_conn);
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    comboBox5.DataSource = ds1.Tables[0];
                    comboBox5.DisplayMember = "fusername";
                    this.groupBox22.Visible = true;
                    this.comboBox4.Text = "0.3";
                    this.comboBox5.Text = "下拉选择";
                    this.textBox27.Text = "";
                    this.textBox28.Text = "";
                    this.comboBox5.Enabled = true;
                    this.textBox27.Enabled = true;
                    this.textBox28.Enabled = true;
                    this.groupBox23.Visible = false;
                    this.Itype_print.Visible = false;
                    this.QR_print.Visible = false;
                    this.Box_print.Visible = false;
                    this.IItype_print.Visible = false;
                    this.tabPage4.Parent = null;
                    this.tabPage3.Parent = tabControl2;
                    this.Itype_printset.Enabled = false;
                    this.QR_printset.Enabled = false;
                    this.Box_printset.Enabled = false;
                    this.IItype_printset.Enabled = false;
                    this.Mac_reprint_input.Enabled = true;
                    this.Mac_reprint_input.Text = "";
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
            if (this.comboBox5.Text != "" && this.comboBox5.Text != "下拉选择")
            {
                string tt_usernumber_MFG = GetUserNumber(this.comboBox5.Text);
                string tt_password_MFG = GetUserPassword(this.comboBox5.Text);

                if (this.textBox28.Text == tt_usernumber_MFG && this.textBox27.Text == tt_password_MFG)
                {
                    this.groupBox23.Visible = true;
                    this.comboBox5.Enabled = false;
                    this.textBox28.Enabled = false;
                    this.textBox27.Enabled = false;
                    this.Itype_print.Visible = true;
                    if (this.label41.Text != "" && int.Parse(PrintChange) >= 1)
                    {
                        this.QR_print.Visible = true;//双打功能
                    }
                    if (int.Parse(PrintChange) >= 2)
                    {
                        this.Box_print.Visible = true;//多打功能
                        this.IItype_print.Visible = true;//多打功能
                    }
                    this.tabPage3.Parent = null;
                    this.tabPage4.Parent = tabControl2;
                    if (this.label41.Text == "")
                    {
                        this.QR_label.Parent = null;
                    }
                    else if (PrintChange == "1")
                    {
                        this.QR_label.Parent = tabControl3;//同上
                    }
                    if (int.Parse(PrintChange) < 2)
                    {
                        this.Box_label.Parent = null;
                        this.IItype_label.Parent = null;
                    }
                    else//多打功能
                    {
                        this.Box_label.Parent = tabControl3;
                        this.IItype_label.Parent = tabControl3;
                    }
                    this.Itype_printset.Enabled = true;
                    this.QR_printset.Enabled = true;
                    this.Box_printset.Enabled = true;
                    this.IItype_printset.Enabled = true;
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
            this.comboBox4.Text = "0.3";
            this.comboBox5.Text = "下拉选择";
            this.textBox27.Text = "";
            this.textBox28.Text = "";
            this.comboBox5.Enabled = true;
            this.textBox27.Enabled = true;
            this.textBox28.Enabled = true;
            this.groupBox23.Visible = false;
            this.Itype_print.Visible = false;
            this.QR_print.Visible = false;
            this.Box_print.Visible = false;
            this.IItype_print.Visible = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
            this.Itype_printset.Enabled = false;
            this.QR_printset.Enabled = false;
            this.Box_printset.Enabled = false;
            this.IItype_printset.Enabled = false;
        }

        //取消身份验证过程，并结束设置
        private void button26_Click(object sender, EventArgs e)
        {
            this.comboBox4.Text = "0.3";
            this.comboBox5.Text = "下拉选择";
            this.textBox27.Text = "";
            this.textBox28.Text = "";
            this.comboBox5.Enabled = true;
            this.textBox27.Enabled = true;
            this.textBox28.Enabled = true;
            this.groupBox22.Visible = false;
            this.groupBox23.Visible = false;
            this.Itype_print.Visible = false;
            this.QR_print.Visible = false;
            this.Box_print.Visible = false;
            this.IItype_print.Visible = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
            this.Itype_printset.Enabled = false;
            this.QR_printset.Enabled = false;
            this.Box_printset.Enabled = false;
            this.IItype_printset.Enabled = false;
        }

        //I型标签上移按钮
        private void Itype_button_up_Click(object sender, EventArgs e)
        {
            tt_top1 -= float.Parse(this.comboBox4.Text);
        }

        //I型标签下移按钮
        private void Itype_button_down_Click(object sender, EventArgs e)
        {
            tt_top1 += float.Parse(this.comboBox4.Text);
        }

        //I型标签左移按钮
        private void Itype_button_left_Click(object sender, EventArgs e)
        {
            tt_left1 -= float.Parse(this.comboBox4.Text);
        }

        //I型标签右移按钮
        private void Itype_button_right_Click(object sender, EventArgs e)
        {
            tt_left1 += float.Parse(this.comboBox4.Text);
        }

        //二维码上移按钮
        private void QR_button_up_Click(object sender, EventArgs e)
        {
            tt_top2 -= float.Parse(this.comboBox4.Text);
        }

        //二维码下移按钮
        private void QR_button_down_Click(object sender, EventArgs e)
        {
            tt_top2 += float.Parse(this.comboBox4.Text);
        }

        //二维码左移按钮
        private void QR_button_left_Click(object sender, EventArgs e)
        {
            tt_left2 -= float.Parse(this.comboBox4.Text);
        }

        //二维码右移按钮
        private void QR_button_right_Click(object sender, EventArgs e)
        {
            tt_left2 += float.Parse(this.comboBox4.Text);
        }

        //彩盒上移按钮
        private void Box_button_up_Click(object sender, EventArgs e)
        {
            tt_top3 -= float.Parse(this.comboBox4.Text);
        }

        //彩盒下移按钮
        private void Box_button_down_Click(object sender, EventArgs e)
        {
            tt_top3 += float.Parse(this.comboBox4.Text);
        }

        //彩盒左移按钮
        private void Box_button_left_Click(object sender, EventArgs e)
        {
            tt_left3 -= float.Parse(this.comboBox4.Text);
        }

        //彩盒右移按钮
        private void Box_button_right_Click(object sender, EventArgs e)
        {
            tt_left3 += float.Parse(this.comboBox4.Text);
        }

        //II型标签上移按钮
        private void IItype_button_up_Click(object sender, EventArgs e)
        {
            tt_top4 -= float.Parse(this.comboBox4.Text);
        }

        //II型标签下移按钮
        private void IItype_button_down_Click(object sender, EventArgs e)
        {
            tt_top4 += float.Parse(this.comboBox4.Text);
        }

        //II型标签左移按钮
        private void IItype_button_left_Click(object sender, EventArgs e)
        {
            tt_left4 -= float.Parse(this.comboBox4.Text);
        }

        //II型标签右移按钮
        private void IItype_button_right_Click(object sender, EventArgs e)
        {
            tt_left4 += float.Parse(this.comboBox4.Text);
        }

        //结束设置
        private void button21_Click(object sender, EventArgs e)
        {
            this.comboBox4.Text = "0.3";
            this.comboBox5.Text = "下拉选择";
            this.textBox27.Text = "";
            this.textBox28.Text = "";
            this.comboBox5.Enabled = true;
            this.textBox27.Enabled = true;
            this.textBox28.Enabled = true;
            this.groupBox22.Visible = false;
            this.groupBox23.Visible = false;
            this.Itype_print.Visible = false;
            this.QR_print.Visible = false;
            this.Box_print.Visible = false;
            this.IItype_print.Visible = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
            this.Itype_printset.Enabled = false;
            this.QR_printset.Enabled = false;
            this.Box_printset.Enabled = false;
            this.IItype_printset.Enabled = false;
        }


        #endregion


        #region 10、扫描事件
        //MAC扫描重打
        private void Mac_reprint_input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                #region
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描查询--------");
                string tt_task = this.textBox1.Text.Trim().ToUpper();
                string tt_bigtask = this.textBox9.Text.Trim().ToUpper();
                string tt_scanmac = this.Mac_reprint_input.Text.Trim();
                string tt_shortmac = GetShortMac(tt_scanmac); 
                #endregion


                //第一步位数判断
                #region
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox6.Text);
                #endregion


                //第二步包含符判断
                #region
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox5.Text.Trim());
                }
                #endregion
                

                //第三步 判断路径
                #region
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    bool tt_flag3_1 = getPathIstrue(tt_path1);
                    bool tt_flag3_2 = getPathIstrue(tt_path2);
                    bool tt_flag3_3 = getPathIstrue(tt_path3);
                    bool tt_flag3_4 = getPathIstrue(tt_path4);
                    if (tt_flag3_1 && tt_flag3_2 && tt_flag3_3 && tt_flag3_4)
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、已找到所有模板,goon");
                    }
                    else if (tt_flag3_1 && this.label41.Text == "")
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、已找到模板,产品没有二维码标签：" + tt_path1 + ",goon");
                    }
                    else if (!tt_flag3_1)
                    {
                        setRichtexBox("3、没有找到I型标签模板,：" + tt_path1 + ",over");
                        PutLableInfor("没有找到I型标签模板，请检查！");
                    }
                    else if (!tt_flag3_2)
                    {
                        setRichtexBox("3、没有找到二维码模板,：" + tt_path2 + ",over");
                        PutLableInfor("没有找到二维码模板，请检查！");
                    }
                    else if (!tt_flag3_3)
                    {
                        setRichtexBox("3、没有找到彩盒标签模板,：" + tt_path3 + ",over");
                        PutLableInfor("没有找到彩盒标签模板，请检查！");
                    }
                    else if (!tt_flag3_4)
                    {
                        setRichtexBox("3、没有找到II型标签模板,：" + tt_path4 + ",over");
                        PutLableInfor("没有找到II型标签模板，请检查！");
                    }
                }
                #endregion
                

                //第四步查找信息
                #region
                Boolean tt_flag4 = false;
                string tt_longmac = "";
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    string tt_sql3 = "select pcbasn,hostlable,maclable,smtaskscode,bprintuser,shelllable,boxlable,productlable,dystlable from odc_alllable " +
                                     "where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";


                    DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
                    if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        this.label42.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString();  //单板号
                        this.label43.Text = ds3.Tables[0].Rows[0].ItemArray[1].ToString();  //主机条码
                        this.label45.Text = ds3.Tables[0].Rows[0].ItemArray[2].ToString();  //短MAC
                        this.label131.Text = ds3.Tables[0].Rows[0].ItemArray[3].ToString();  //设备标识码
                        this.label46.Text = ds3.Tables[0].Rows[0].ItemArray[4].ToString();  //长MAC
                        this.label174.Text = ds3.Tables[0].Rows[0].ItemArray[5].ToString(); //onumac
                        this.label47.Text = ds3.Tables[0].Rows[0].ItemArray[5].ToString().Replace("-","");  //GPSN
                        this.label67.Text = ds3.Tables[0].Rows[0].ItemArray[6].ToString();  //boxlable
                        this.label77.Text = ds3.Tables[0].Rows[0].ItemArray[7].ToString();  //上海资产编码
                        this.label162.Text = ds3.Tables[0].Rows[0].ItemArray[8].ToString();  //电源条码
                        tt_longmac = this.label46.Text;
                        setRichtexBox("4、关联表查询到一条数据，goon");

                    }
                    else
                    {
                        string tt_querytask = GetSnRealTask("2", tt_shortmac);
                        setRichtexBox("4、在工单:" + tt_task + "的关联表中没有查询到数据，该MAC的工单是" + tt_querytask + ",over");
                        PutLableInfor("该单板的工单为:" + tt_querytask + ",与工单:" + tt_task + "不符");
                    }

                }
                #endregion


                //第五步查询macinfo表信息
                #region
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    tt_flag5 = true;
                    string tt_sql5 = "select ssid,username,password,Wlanpas,ssid_5G,wlanpas_5G,barcode1 from odc_macinfo " +
                                      "where taskscode = '" + tt_bigtask + "' and mac = '" + tt_longmac + "' ";

                    DataSet ds5 = Dataset1.GetDataSet(tt_sql5, tt_conn);
                    if (ds5.Tables.Count > 0 && ds5.Tables[0].Rows.Count > 0)
                    {
                        tt_flag5 = true;
                        this.label128.Text = ds5.Tables[0].Rows[0].ItemArray[0].ToString();  //2G用户名
                        this.label136.Text = ds5.Tables[0].Rows[0].ItemArray[1].ToString();  //用户名
                        this.label134.Text = ds5.Tables[0].Rows[0].ItemArray[2].ToString();  //密码
                        this.label132.Text = ds5.Tables[0].Rows[0].ItemArray[3].ToString();  //2G密码
                        this.label124.Text = ds5.Tables[0].Rows[0].ItemArray[4].ToString();  //5G账号
                        this.label120.Text = ds5.Tables[0].Rows[0].ItemArray[5].ToString();  //5G密码
                        this.label116.Text = ds5.Tables[0].Rows[0].ItemArray[6].ToString();  //设备标示号暗码

                        setRichtexBox("5、Macinfo表找到一条数据,goon");
                    }
                    else
                    {
                        setRichtexBox("5、Macinfo表没有找到一条数据，over");
                        PutLableInfor("Macinfo表没有找到数据，请检查！");
                    }
                }
                #endregion
                

                //第六步 查找站位信息
                #region
                Boolean tt_flag6 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                {
                    tt_flag6 = true;
                    setRichtexBox("6、查找站位信息,goon");
                }
                #endregion
                

                //最后判断
                #region
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {
                    GetParaDataPrint1(0);
                    if (this.label41.Text != "" && int.Parse(PrintChange) >= 1)//双打功能
                    {
                        GetParaDataPrint2(0);
                    }
                    if (int.Parse(PrintChange) >= 2)
                    {
                        GetParaDataPrint3(0);
                        GetParaDataPrint4(0);
                    }
                    GetProductNumInfo();                   

                    string tt_gyid = "";
                    if (tt_gyid_Use == this.label54.Text || tt_gyid_Use == "")
                    {
                        tt_gyid = this.label54.Text;
                    }
                    else
                    {
                        tt_gyid = tt_gyid_Old;
                    }

                    CheckStation(tt_shortmac,tt_gyid);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    setRichtexBox("6、查询完毕，可以重打标签或修改模板，over");
                    PutLableInfor("MAC查询完毕");

                    if (tt_reprintmark == "0")
                    {
                        this.Mac_reprint_input.Enabled = false;
                    }

                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                }
                #endregion
                
                //移动光标
                getProductRhythm("0");
                Mac_reprint_input.Focus();
                Mac_reprint_input.SelectAll();
            }
        }


        //设备标示符
        private void EQP_input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                #region
                ScanDataInitial();
                setRichtexBox("-----开始设备标示符扫描--------");
                string tt_unitnum = this.EQP_input.Text.Trim();
                #endregion


                //第一步位数判断
                #region
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_unitnum, this.textBox10.Text);
                #endregion


                //第二步包含符判断
                #region
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(this.textBox8.Text.Trim(), tt_unitnum.Substring(0,6));
                }
                #endregion


                //最后判断
                #region
                if (tt_flag1 && tt_flag2 )
                {
                    setRichtexBox("3、设备标示扫描完毕，可以扫描MAC，goon");
                    PutLableInfor("请扫描MAC");
                    EQP_input.Enabled = false;
                    Mac_input.Enabled = true;
                    Mac_input.Focus();
                    Mac_input.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    EQP_input.Enabled = true;
                    Mac_input.Enabled = false;
                    EQP_input.Focus();
                    EQP_input.SelectAll();
                }
                #endregion

            }
        }


        //扫描MAC过站
        private void Mac_input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                #region
                setRichtexBox("-----开始MAC过站扫描--------");
                Mac_input.Enabled = false;
                string tt_smalltask = this.textBox1.Text.Trim().ToUpper();
                string tt_bigtask = this.textBox9.Text.Trim().ToUpper();
                string tt_scanmac = this.Mac_input.Text.Trim();
                string tt_shortmac = GetShortMac(tt_scanmac);
                #endregion


                //第一步位数判断
                #region
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox3.Text);
                #endregion

                
                //第二步包含符判断
                #region
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox4.Text.Trim());
                }
                #endregion


                //第三步 检查模板
                #region
                Boolean tt_flag3 = false;
                if (tt_flag2)
                {
                    bool tt_flag3_1 = getPathIstrue(tt_path1);
                    bool tt_flag3_2 = getPathIstrue(tt_path2);
                    bool tt_flag3_3 = getPathIstrue(tt_path3);
                    bool tt_flag3_4 = getPathIstrue(tt_path4);
                    if (tt_flag3_1 && tt_flag3_2 && tt_flag3_3 && tt_flag3_4)
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、已找到所有模板,：" + tt_path1 + "，" + tt_path2 + ",goon");
                    }
                    else if (tt_flag3_1 && this.label41.Text == "")
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、已找到模板,产品没有二维码标签：" + tt_path1 + ",goon");
                    }
                    else if (!tt_flag3_1)
                    {
                        setRichtexBox("3、没有找到I型标签模板,：" + tt_path1 + ",over");
                        PutLableInfor("没有找到I型标签模板，请检查！");
                    }
                    else if (!tt_flag3_2)
                    {
                        setRichtexBox("3、没有找到二维码模板,：" + tt_path2 + ",over");
                        PutLableInfor("没有找到二维码模板，请检查！");
                    }
                    else if (!tt_flag3_3)
                    {
                        setRichtexBox("3、没有找到彩盒标签模板,：" + tt_path3 + ",over");
                        PutLableInfor("没有找到彩盒标签模板，请检查！");
                    }
                    else if (!tt_flag3_4)
                    {
                        setRichtexBox("3、没有找到II型标签模板,：" + tt_path4 + ",over");
                        PutLableInfor("没有找到II型标签模板，请检查！");
                    }
                }
                #endregion


                //第五步 其他预留
                #region
                Boolean tt_flag5 = false;
                if (tt_flag3)
                {
                    tt_flag5 = true;
                    setRichtexBox("4、其他预留，over");

                }
                #endregion
                

                //第六步流程检查
                #region
                Boolean tt_flag6 = false;
                string tt_ccode = this.label56.Text;
                string tt_ncode = this.label57.Text;
                if (tt_flag5)
                {
                    if (tt_ccode == "" || tt_ncode == "")
                    {
                        setRichtexBox("5、该工单没有配置流程," + tt_ccode + "," + tt_ncode + ",over");
                        PutLableInfor("没有获取到当前待测站位，及下一站位，请检查");
                    }
                    else
                    {
                        tt_flag6 = true;
                        setRichtexBox("5、该工单已配置流程," + tt_ccode + "," + tt_ncode + ",goon");
                    }

                }
                #endregion
                

                //第七步查找关联表数据
                #region
                Boolean tt_flag7 = false;
                string tt_hostlable = "";
                string tt_smtaskscode = "";
                string tt_longmac = "";
                string tt_oldtype = "";
                string tt_id = "";
                string tt_gpsn = "";
                string tt_onumac = "";
                string tt_pcba = "";
                if (tt_flag6)
                {
                    string tt_sql7 = "select hostlable,maclable,smtaskscode,bprintuser,id,ageing,shelllable,pcbasn from odc_alllable " +
                                     "where hprintman = '" + tt_bigtask + "' and taskscode = '" + tt_bigtask + "' and maclable = '" + tt_shortmac + "' ";

                    DataSet ds7 = Dataset1.GetDataSet(tt_sql7, tt_conn);
                    if (ds7.Tables.Count > 0 && ds7.Tables[0].Rows.Count > 0)
                    {
                        tt_flag7 = true;
                        tt_hostlable = ds7.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();  //主机条码
                        tt_shortmac = ds7.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();    //短MAC
                        tt_smtaskscode = ds7.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();  //移动串号
                        tt_longmac = ds7.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();     //长MAC
                        tt_id = ds7.Tables[0].Rows[0].ItemArray[4].ToString();      //行ID
                        tt_oldtype = ds7.Tables[0].Rows[0].ItemArray[5].ToString();   //老化状态
                        tt_onumac = ds7.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper(); //ONUMAC
                        tt_gpsn = ds7.Tables[0].Rows[0].ItemArray[6].ToString().Replace("-","").ToUpper();   //GPSN
                        tt_pcba = ds7.Tables[0].Rows[0].ItemArray[7].ToString().ToUpper();   //单板号

                        setRichtexBox("6、关联表查询到一条数据，hostlable=" + tt_hostlable + ",mac=" + tt_shortmac + ",smtaskscode=" + tt_smtaskscode + ",id=" + tt_id + ",老化ageing=" + tt_oldtype + ",goon");

                    }
                    else
                    {
                        string tt_sql7_1 = "select hostlable,maclable,smtaskscode,bprintuser,id,ageing,shelllable,pcbasn from odc_alllable " +
                                           "where hprintman = '" + tt_bigtask + "' and taskscode = '" + tt_smalltask + "' and maclable = '" + tt_shortmac + "' ";

                        DataSet ds7_1 = Dataset1.GetDataSet(tt_sql7_1, tt_conn);
                        if (ds7_1.Tables.Count > 0 && ds7_1.Tables[0].Rows.Count > 0)
                        {
                            tt_flag7 = true;
                            tt_hostlable = ds7_1.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();  //主机条码
                            tt_shortmac = ds7_1.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();    //短MAC
                            tt_smtaskscode = ds7_1.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();  //移动串号
                            tt_longmac = ds7_1.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();     //长MAC
                            tt_id = ds7_1.Tables[0].Rows[0].ItemArray[4].ToString();      //行ID
                            tt_oldtype = ds7_1.Tables[0].Rows[0].ItemArray[5].ToString();   //老化状态
                            tt_onumac = ds7_1.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper(); //ONUMAC
                            tt_gpsn = ds7_1.Tables[0].Rows[0].ItemArray[6].ToString().Replace("-", "").ToUpper();   //GPSN
                            tt_pcba = ds7_1.Tables[0].Rows[0].ItemArray[7].ToString();   //单板号

                            setRichtexBox("6、关联表查询到一条数据，hostlable=" + tt_hostlable + ",mac=" + tt_shortmac + ",smtaskscode=" + tt_smtaskscode + ",id=" + tt_id + ",老化ageing=" + tt_oldtype + ",goon");

                        }
                        else
                        {
                            string tt_querytask = GetSnRealTask("2", tt_shortmac);
                            setRichtexBox("6、在工单:" + tt_smalltask + "的关联表中没有查询到数据，该MAC的工单是" + tt_querytask + ",over");
                            PutLableInfor("该单板的工单为:" + tt_querytask + ",与大工单:" + tt_bigtask + "不符");
                        }
                    }

                }
                #endregion


                //第四步第一次数量检查（同时生产的话会出现问题）
                #region
                Boolean tt_flag4 = false;
                int tt_tasknumber = int.Parse(this.label12.Text);
                if (tt_flag7)
                {
                    int tt_productnum = int.Parse(this.label59.Text);
                    if (tt_productnum < tt_tasknumber)
                    {
                        tt_flag4 = true;
                        setRichtexBox("7、第一次数量检查，已获取序列号生产数量：" + tt_productnum.ToString() + "，小于计划数量：" + tt_tasknumber.ToString() + ",还可以再生产goon");
                    }
                    else if (tt_productnum == tt_tasknumber)
                    {
                        DataSet tt_dataset = Dataset2.getMacAllCodeInfo(tt_shortmac, tt_conn);
                        string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset);

                        if (tt_hostlable != tt_shortmac && tt_nowcode == "3000") //生产序列号已满又出现重打标签的产品过站时
                        {
                            tt_flag4 = true;
                            setRichtexBox("7、第一次数量检查，已获取序列号生产数量：" + tt_productnum.ToString() + "，等于计划数量：" + tt_tasknumber.ToString() + ",但产品在当前站位，且产品已有序列号，产品可能为正常产品重投后流至此站位，goon");
                        }
                        else
                        {
                            setRichtexBox("7、第一次数量检查，已获取序列号生产数量：" + tt_productnum.ToString() + "，等于计划数量：" + tt_tasknumber.ToString() + ",且产品没有序列号或站位不正确,不能再生产goon");
                            PutLableInfor("产品站位不正确，或生产数量已满不能再生产了！");
                        }
                    }
                    else
                    {
                        setRichtexBox("7、第一次数量检查，已获取序列号生产数量：" + tt_productnum.ToString() + "，大于等于计划数量：" + tt_tasknumber.ToString() + ",不能再生产goon");
                        PutLableInfor("生产数量已满不能再生产了！");
                    }
                }
                #endregion


                //第八步 其他预留
                #region
                Boolean tt_flag8 = false;
                if (tt_flag4)
                {
                    tt_flag8 = true;
                    setRichtexBox("8、其他预留，over");
                }
                #endregion
                

                //第九步查询MACINFO信息
                #region
                Boolean tt_flag9 = false;
                string tt_ssid = null;
                string tt_macusername = null;
                string tt_password = null;
                string tt_wlanpas = null;
                string tt_5guser = null;
                string tt_5gpassword = null;
                string tt_barcode1 = null;
                if (tt_flag8)
                {
                    tt_flag9 = true;
                    string tt_sql9 = "select ssid,username,password,Wlanpas,ssid_5G,wlanpas_5G,barcode1  from odc_macinfo " +
                                    "where taskscode = '" + tt_bigtask + "' and mac = '" + tt_longmac + "' ";

                    DataSet ds9 = Dataset1.GetDataSet(tt_sql9, tt_conn);
                    if (ds9.Tables.Count > 0 && ds9.Tables[0].Rows.Count > 0)
                    {
                        tt_flag9 = true;
                        tt_ssid = ds9.Tables[0].Rows[0].ItemArray[0].ToString();  //SSID
                        tt_macusername = ds9.Tables[0].Rows[0].ItemArray[1].ToString();  //用户名
                        tt_password = ds9.Tables[0].Rows[0].ItemArray[2].ToString();  //密码
                        tt_wlanpas = ds9.Tables[0].Rows[0].ItemArray[3].ToString();  //WIFI密码
                        tt_5guser = ds9.Tables[0].Rows[0].ItemArray[4].ToString();  //5G账号
                        tt_5gpassword = ds9.Tables[0].Rows[0].ItemArray[5].ToString();  //5G密码
                        tt_barcode1 = ds9.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper();  //设备标示号暗码

                        setRichtexBox("9、Macinfo表找到一条数据，SSID=" + tt_ssid + ",username=" + tt_macusername + ",password=" + tt_password + ",wanlaps=" + tt_wlanpas + ",goon");
                    }
                    else
                    {
                        setRichtexBox("9、Macinfo表没有找到一条数据，over");
                        PutLableInfor("Macinfo表没有找到条数据，请检查！");
                    }
                }
                #endregion
                

                //第十步验证设备标示符
                #region
                Boolean tt_flag10 = false;
                string tt_unitnum = this.EQP_input.Text.Trim().ToUpper().Replace(" ","");
                if (tt_flag9 && tt_parenttask != "小型化方案")
                {
                    if (tt_barcode1.Contains(tt_unitnum) || tt_gpsn == tt_unitnum)
                    {
                        tt_flag10 = true;
                        setRichtexBox("10、设备标示码验证正确，系统正确验证码为：" + tt_barcode1 + "，扫描设备码为:" + tt_unitnum + ",gong");
                    }
                    else
                    {
                        setRichtexBox("10、设备标示码验证不正确，系统正确验证码为：" + tt_barcode1 + "，扫描设备码为:" + tt_unitnum + ",gong");
                        PutLableInfor("设备标示码验证不正确，与系统记录不一致！");
                    }
                }
                else if (tt_flag9 && tt_parenttask == "小型化方案")
                {
                    tt_flag10 = true;
                    setRichtexBox("10、小型化方案不检查,gong");
                }
                #endregion
                

                //第十一步是否获取主机条码判断
                #region
                Boolean tt_flag11 = false;
                if (tt_flag10)
                {
                    if (tt_hostlable == tt_shortmac)
                    {
                        tt_flag11 = true;
                        tt_checkflag = 0;
                        setRichtexBox("11、该条码主机条码：" + tt_hostlable + ",数据与MAC:" + tt_shortmac + "一致，还没有获取主机条码，go");
                    }
                    else
                    {
                        bool tt_flag11_1 = CheckPrintRecord(tt_shortmac,"铭牌标签");
                        bool tt_flag11_2 = CheckPrintRecord(tt_shortmac, "运营商标签");
                        DataSet tt_dataset = Dataset2.getMacAllCodeInfo(tt_shortmac, tt_conn);
                        string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset);

                        if (tt_flag11_1 == true && tt_flag11_2 == true && int.Parse(tt_nowcode) > 3000)
                        {
                            //双胞胎产品
                            tt_checkflag = 0;
                            int tt_lockdouble = Lock_alllable(tt_smalltask, tt_shortmac);
                            int tt_lockrouting = Lock_routing(tt_smalltask, tt_shortmac);
                            int tt_lockpage = Lock_package(tt_scanmac);
                            setRichtexBox("11、该条码主机条码：" + tt_hostlable + ",数据与MAC:" + tt_shortmac + "不一致，产品已进入包装后段流程且有铭牌或设备标签重打记录，可能为双胞胎产品，over");
                            PutLableInfor("产品可能为双胞胎产品，产品" + tt_hostlable + "已锁死，请通知工程");
                        }
                        else if (tt_nowcode == "3000")
                        {
                            //复测check重投产品直接通过
                            tt_flag11 = true;
                            tt_checkflag = 1;
                            setRichtexBox("11、该条码主机条码：" + tt_hostlable + ",数据与MAC:" + tt_shortmac + "不一致，但产品在当前站位，可能为正常产品重投后流至此站位，go");
                        }
                        else
                        {
                            tt_checkflag = 0;
                            setRichtexBox("11、该条码主机条码：" + tt_hostlable + ",数据与MAC:" + tt_shortmac + "不一致，产品已有生产序列号标签，且站位不在当前站位，over");
                            PutLableInfor("产品已有序列号，当前站位为"+ tt_nowcode + "，请确认！");
                        }
                    }
                }
                #endregion


                //第十二步  其他预留
                #region
                Boolean tt_flag12 = false;
                if (tt_flag11)
                {
                    tt_flag12 = true;
                    setRichtexBox("12、其他预留，over");
                }
                #endregion


                //第十三步 NG01  获取MAC站位信息
                #region
                Boolean tt_flag13 = false;
                DataSet tt_dataset1 = null;
                if (tt_flag12)
                {
                    tt_dataset1 = Dataset2.getMacAllCodeInfo(tt_shortmac, tt_conn);
                    if (tt_dataset1.Tables.Count > 0 && tt_dataset1.Tables[0].Rows.Count > 0)
                    {
                        tt_flag13 = true;
                        setRichtexBox("13、NG01过,站位表找到MAC站位信息，记录数为:" + tt_dataset1.Tables[0].Rows.Count.ToString() + ",goon");
                    }
                    else
                    {
                        setRichtexBox("13、NG01,站位表没有找MAC:" + tt_shortmac + "，站位信息，over");
                        PutLableInfor2("NG01,站位表没有找MAC:" + tt_shortmac + "，站位信息", tt_bigtask,tt_shortmac);
                    }
                }
                #endregion


                //第十四步 NG02  的待测站位
                #region
                Boolean tt_flag14 = false;
                string tt_testcode = this.label56.Text;
                if (tt_flag13)
                {
                    string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset1);
                    if (tt_nowcode == tt_testcode)
                    {
                        tt_flag14 = true;
                        setRichtexBox("14、NG02过,该单板的最后站位与流程设置的最后站位一致，都是:" + tt_nowcode + ",goon");
                    }
                    else
                    {
                        if (tt_nowcode == "0")
                        {
                            setRichtexBox("14、NG02,当前单板MAC:" + tt_shortmac + ",没有待测站位，请检查，over");
                            PutLableInfor2("NG02,当前单板MAC:" + tt_shortmac + ",没有待测站位", tt_bigtask, tt_shortmac);
                        }
                        else
                        {
                            if (tt_nowcode == "2")
                            {
                                setRichtexBox("14、NG02,当前单板MAC:" + tt_shortmac + ",有多个待测待测站位，流程异常，over");
                                PutLableInfor2("NG02,单板MAC:" + tt_shortmac + ",有多个待测站位,流程异常", tt_bigtask, tt_shortmac);
                            }
                            else
                            {
                                setRichtexBox("14、NG02,当前单板MAC:" + tt_shortmac + "，站位不对" + tt_nowcode + "，与设定站位" + tt_testcode + "不符，不过使用,over");
                                PutLableInfor2("NG02,单板MAC:" + tt_shortmac + ",当前站位" + tt_nowcode + ",与" + tt_testcode + ",不符", tt_bigtask, tt_shortmac);
                            }
                        }

                    }

                }
                #endregion


                //第十五步 NG03  1920站位检查
                #region
                Boolean tt_flag15 = false;
                int tt_int1920id = 0;
                if (tt_flag14)
                {
                    tt_int1920id = Dataset2.getFirstCodeId(tt_dataset1);
                    if (tt_int1920id > 0)
                    {
                        tt_flag15 = true;
                        setRichtexBox("15、NG03过,前站位ccode找到一个最近的1920站位，id=" + tt_int1920id.ToString() + ",goon");
                    }
                    else
                    {
                        switch (tt_int1920id)
                        {
                            case 0:
                                setRichtexBox("15、NG03,查找起始站位1902数据集内容有问题，数据集内容为空值,id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor2("NG03,查找起始站位1902数据集有问题，为空值", tt_bigtask, tt_shortmac);
                                break;

                            case -1:
                                setRichtexBox("15、NG03,查找起始站位1902数据集排序有问题，不是从大到小的顺序排序，id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor2("NG03,查找起始站位1902数据集排序有问题，不是顺序排序", tt_bigtask, tt_shortmac);
                                break;

                            case -2:
                                setRichtexBox("15、NG03,查找起始站位1902数据集有问题，没有找到起始1920站位，id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor2("NG03,过站没有找到1920站位", tt_bigtask, tt_shortmac);
                                break;

                            default:
                                setRichtexBox("15、NG03,查找起始站位1902数据集有问题，出现异常情况，id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor2("NG03,查找起始站位1902数据集出现异常情况", tt_bigtask, tt_shortmac);
                                break;
                        }
                    }
                }
                #endregion


                //第十六步 NG04  3350跳出检验
                #region
                Boolean tt_flag16 = false;
                if (tt_flag15)
                {
                    tt_flag16 = true;
                    setRichtexBox("16、NG04过,3350跳出检查先不检验直接过,goon");


                    //string tt_maintaincheck = Dataset2.getMaintainJumpCheck(tt_dataset1, tt_int1920id);
                    //if (tt_maintaincheck.Equals("1"))
                    //{
                    //    tt_flag16 = true;
                    //    setRichtexBox("16、NG04过,3350跳出检查OK没有问题，返回值：" + tt_maintaincheck + ",检查起始ID:" + tt_int1920id.ToString() + ",goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("16、NG04,3350跳出检查Fail有问题，返回值：" + tt_maintaincheck + ",over");
                    //    PutLableInfor2("NG04," + tt_maintaincheck, tt_bigtask, tt_shortmac);
                    //}

                }
                #endregion


                //第十七步  NG05  全部流程检查
                #region
                Boolean tt_flag17 = false;
                if (tt_flag16)
                {
                    string tt_gyid = "";

                    if (tt_gyid_Use == this.label54.Text || tt_gyid_Use == "")
                    {
                        tt_gyid = this.label54.Text;
                    }
                    else
                    {
                        tt_gyid = tt_gyid_Old;
                    }

                    int tt_productname_check = 0;

                    if (this.label13.Text.Trim() == "HG6201M"
                        || ("HG6201T,HG2201T".Contains(this.label13.Text.Trim())
                        && this.label14.Text != "安徽"
                        && tt_parenttask != "小型化方案"))
                    {
                        tt_productname_check = 1;
                    }

                    string tt_codecheck = Dataset2.getPcbaAllCheck2(tt_routdataset, tt_dataset1, tt_int1920id, tt_productname_check);
                    if (tt_codecheck == "1")
                    {
                        tt_flag17 = true;
                        tt_gyid_Use = tt_gyid;
                        setRichtexBox("17、NG05过,该单板所有站位都测试，没有漏测站位，全部流程:" + tt_gyid + "号" + tt_allprocesses + ",检验流程:" + tt_partprocesses + ",1920id:" + tt_int1920id.ToString() + ",goon");
                    }
                    else if (tt_codecheck == "0")
                    {
                        setRichtexBox("17、NG05,单板站位全流程检查数据集有问题,MAC" + tt_shortmac + ",全部流程:" + tt_gyid + "号" + tt_allprocesses + ",检验流程:" + tt_partprocesses + ",1920id:" + tt_int1920id.ToString() + ",over");
                        PutLableInfor2("NG05,单板站位全流程检查数据集有问题", tt_bigtask, tt_shortmac);
                    }
                    else if (tt_gyid_Old != "")
                    {
                        string tt_gyid1 = "";

                        if (tt_gyid_Use == this.label54.Text || tt_gyid_Use == "")
                        {
                            tt_gyid1 = tt_gyid_Old;
                        }
                        else
                        {
                            tt_gyid1 = this.label54.Text;
                        }

                        string tt_codeserial = this.label75.Text;

                        string tt_sql17_1 = "select pxid from odc_routing  where pid = " + tt_gyid1 + "  and LCBZ > 1 and LCBZ < '" + tt_codeserial + "' ";
                        tt_routdataset = Dataset1.GetDataSetTwo(tt_sql17_1, tt_conn);
                        if (tt_routdataset.Tables.Count > 0 && tt_routdataset.Tables[0].Rows.Count > 0)
                        {
                            tt_allprocesses = Dataset2.getGyidAllProcess(tt_gyid1, tt_conn);
                            tt_partprocesses = Dataset2.getGyidPartProcess(tt_routdataset);
                            tt_allroutdataset = Dataset2.getGyidAllProcessDt(tt_gyid1, tt_conn);
                        }
                        else
                        {
                            MessageBox.Show("没有找到流程:" + tt_gyid1 + "，的流程数据集Dataset，请流程设置！");
                            this.richTextBox1.BackColor = Color.Chartreuse;
                            this.Mac_input.Enabled = false;
                            this.EQP_input.Enabled = true;
                            return;
                        }

                        string tt_codecheck_1 = Dataset2.getPcbaAllCheck2(tt_routdataset, tt_dataset1, tt_int1920id, tt_productname_check);
                        if (tt_codecheck_1 == "1")
                        {
                            tt_flag17 = true;
                            tt_gyid_Use = tt_gyid1;
                            setRichtexBox("17、NG05过,该单板所有站位都测试，没有漏测站位，全部流程:" + tt_gyid1 + "号" + tt_allprocesses + ",检验流程:" + tt_partprocesses + ",1920id:" + tt_int1920id.ToString() + ",goon");
                        }
                        else if (tt_codecheck_1 == "0")
                        {
                            setRichtexBox("17、NG05,单板站位全流程检查数据集有问题,MAC" + tt_shortmac + ",全部流程:" + tt_gyid1 + "号" + tt_allprocesses + ",检验流程:" + tt_partprocesses + ",1920id:" + tt_int1920id.ToString() + ",over");
                            PutLableInfor2("NG05,单板站位全流程检查数据集有问题", tt_bigtask, tt_shortmac);
                        }
                        else
                        {
                            setRichtexBox("17、NG05,该单板这个站位没有测试:" + tt_codecheck_1 + "，请仔细检查MAC:" + tt_shortmac + ",的流程:全流程为:" + tt_allprocesses + ",检测流程为:" + tt_partprocesses + ",是否有漏测站位，over");
                            PutLableInfor2("NG05,该单板这个站位没有测试:" + tt_codecheck_1 + "，请检查是否漏测", tt_bigtask, tt_shortmac);
                        }
                    }
                    else
                    {
                        setRichtexBox("17、NG05,该单板这个站位没有测试:" + tt_codecheck + "，请仔细检查MAC:" + tt_shortmac + ",的流程:全流程为:" + tt_allprocesses + ",检测流程为:" + tt_partprocesses + ",是否有漏测站位，over");
                        PutLableInfor2("NG05,该单板这个站位没有测试:" + tt_codecheck + "，请检查是否漏测", tt_bigtask, tt_shortmac);
                    }
                }
                #endregion


                //第十八步 NG06  流程顺序检查
                #region
                Boolean tt_flag18 = false;
                if (tt_flag17)
                {
                    tt_flag18 = true;
                    setRichtexBox("18、NG06过,MAC全顺序检查先不检验直接过，goon");


                    //string tt_codeserialcheck = Dataset2.getCodeSerialCheck(tt_dataset1, tt_int1920id);
                    //if (tt_codeserialcheck.Equals("1"))
                    //{
                    //    tt_flag18 = true;
                    //    setRichtexBox("18、NG06过,MAC全顺序检查OK没有问题，返回值：" + tt_codeserialcheck + ",检查起始ID:" + tt_int1920id.ToString() + ",goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("18、NG06,MAC全顺序检查Fail有问题，返回值：" + tt_codeserialcheck + ",over");
                    //    PutLableInfor2("NG06," + tt_codeserialcheck, tt_bigtask,tt_shortmac);
                    //}
                }
                #endregion


                //第十九步 NG07  流程前后项检查信息
                #region
                Boolean tt_flag19 = false;
                if (tt_flag18)
                {
                    tt_flag19 = true;
                    setRichtexBox("19、NG07过,过站前后站位检查先不检验直接过，goon");

                    //string tt_nearcodecheck = Dataset2.getNearCodeCheck2(tt_dataset1, tt_int1920id, tt_allroutdataset);
                    //if (tt_nearcodecheck.Equals("1"))
                    //{
                    //    tt_flag19 = true;
                    //    setRichtexBox("19、NG07过,过站前后站位检查OK没有问题，返回值：" + tt_nearcodecheck + ",检查起始ID:" + tt_int1920id.ToString() + ",goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("19、NG07,过站前后站位检查Fail有问题，返回值：" + tt_nearcodecheck + ",over");
                    //    PutLableInfor2("NG07," + tt_nearcodecheck, tt_bigtask, tt_shortmac);
                    //}
                }
                #endregion


                //第二十步 NG08  流程上下项检查信息
                #region
                Boolean tt_flag20 = false;
                if (tt_flag19)
                {
                    tt_flag20 = true;
                    setRichtexBox("20、NG08过,过站前后站位检查先不检验直接过，goon");

                    //string tt_updowncodecheck = Dataset2.getUpdownCodeCheck(tt_dataset1, tt_int1920id, tt_allroutdataset);
                    //if (tt_updowncodecheck.Equals("1"))
                    //{
                    //    tt_flag20 = true;
                    //    setRichtexBox("20、NG08过,过站前后站位检查OK没有问题，返回值：" + tt_updowncodecheck + ",检查起始ID:" + tt_int1920id.ToString() + ",goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("20、NG08,过站前后站位检查Fail有问题，返回值：" + tt_updowncodecheck + ",over");
                    //    PutLableInfor2("NG08," + tt_updowncodecheck, tt_bigtask, tt_shortmac);
                    //}
                }
                #endregion


                //第二十一步 NG09  MAC检查预留2
                #region
                Boolean tt_flag21 = false;
                if (tt_flag20)
                {
                    tt_flag21 = true;
                    setRichtexBox("21、NG09,过站检查预留二，over");
                }
                #endregion


                //第二十二步 NG10  MAC检查预留3
                #region
                Boolean tt_flag22 = false;
                if (tt_flag21)
                {
                    tt_flag22 = true;
                    setRichtexBox("22、NG10,过站检查预留三，over");
                }
                #endregion


                //第二十三步  其他预留1
                #region
                Boolean tt_flag23 = false;
                if (tt_flag22)
                {
                    tt_flag23 = true;
                    setRichtexBox("23、其他预留，over");
                }
                #endregion


                //第二十四步  第二次数量检查
                #region
                Boolean tt_flag24 = false;
                if (tt_flag23)
                {
                    string tt_sql24 = "select count(1),0,0 from odc_alllable " +
                                      "where taskscode = '" + tt_smalltask + "' and hostlable <> maclable ";

                    string[] tt_array24 = new string[3];
                    tt_array24 = Dataset1.GetDatasetArray(tt_sql24, tt_conn);

                    int tt_productnum1 = int.Parse(tt_array24[0]);
                    if (tt_productnum1 < tt_tasknumber)
                    {
                        tt_flag24 = true;
                        setRichtexBox("24、第二次数量检查，已获取序列号生产数量：" + tt_productnum1.ToString() + "，小于计划数量：" + tt_tasknumber.ToString() + ",还可以再生产gong");
                    }
                    else if (tt_checkflag == 1)
                    {
                        tt_flag24 = true;
                        setRichtexBox("24、第二次数量检查，已获取序列号生产数量：" + tt_productnum1.ToString() + "，大于等于计划数量：" + tt_tasknumber.ToString() + ",但产品有铭牌或设备标签重打记录，且产品在当前站位，产品可能为正常产品重打后流至此站位,gong");
                    }
                    else
                    {
                        setRichtexBox("24、第二次数量检查，已获取序列号生产数量：" + tt_productnum1.ToString() + "，大于等于计划数量：" + tt_tasknumber.ToString() + ",不能再生产gong");
                        PutLableInfor("生产数量已满不能再生产了！");
                    }
                }
                #endregion


                //第二十五步开始过站
                #region
                int tt_intgetno = 0;
                Boolean tt_flag25 = false;
                if (int.Parse(PrintChange) < 2)//非多打方案就开始过站
                {
                    if (tt_flag24 && tt_hostlable == tt_shortmac)
                    {
                        string tt_username = STR;
                        tt_intgetno = Dataset1.FhYDSnInStation(tt_smalltask, tt_bigtask, tt_username,
                                                              tt_hostlable, tt_shortmac, tt_shanghailabel,
                                                              tt_gyid_Use, tt_ccode, tt_ncode,
                                                              tt_conn);
                        if (tt_intgetno > 0)
                        {
                            tt_flag25 = true;
                            setRichtexBox("25、该产品过站成功，返回生产序号值:" + tt_intgetno.ToString() + ",请继续扫描,ok");
                        }
                        else
                        {
                            setRichtexBox("25、过站不成功，事务已回滚,返回生产序号值:" + tt_intgetno.ToString() + "");
                            PutLableInfor("过站不成功，请检查或再次扫描！返回值" + tt_intgetno.ToString());
                        }

                    }
                    else if (tt_flag24 && tt_hostlable != tt_shortmac)
                    {
                        string tt_username = "已包装";
                        bool tt_flag25_1 = Dataset1.FhUnPassStationI(tt_smalltask, tt_username, tt_shortmac, tt_gyid_Use, tt_ccode, tt_ncode, tt_conn);
                        if (tt_flag25_1)
                        {
                            tt_flag25 = true;
                            setRichtexBox("25、该产品过站成功，产品已存在生产序列号:" + tt_hostlable + ",请继续扫描,ok");
                        }
                        else
                        {
                            setRichtexBox("25、过站不成功，事务已回滚,返回生产序号值:" + tt_hostlable + "");
                            PutLableInfor("过站不成功，请检查或再次扫描！");
                        }
                    }
                }
                else
                {
                    tt_flag25 = true;
                    setRichtexBox("25、一机多打方案,此处不过站,请继续扫描,ok");
                }
                #endregion


                //第二十六站：查询生成的序列号
                #region
                Boolean tt_flag26 = false;
                string tt_boxlable = "";
                if (int.Parse(PrintChange) < 2)//非多打方案检查序列号是否生成
                {
                    if (tt_flag25)
                    {
                        string tt_sql26 = "select count(1), min(boxlable),min(productlable) from odc_alllable " +
                                    "where taskscode = '" + tt_smalltask + "' and hprintman = '" + tt_bigtask + "' and maclable = '" + tt_shortmac + "' ";



                        string[] tt_array26 = new string[3];
                        tt_array26 = Dataset1.GetDatasetArray(tt_sql26, tt_conn);
                        if (tt_array26[0] == "1")
                        {
                            tt_flag26 = true;
                            tt_boxlable = tt_array26[1];
                            this.label67.Text = tt_boxlable;
                            this.label77.Text = tt_array26[2];
                            setRichtexBox("26、生产序列号获取成功，已获取序列号：" + tt_boxlable + ", goon");
                        }
                        else
                        {
                            setRichtexBox("26、生产序列号获取不成功，序列号：" + tt_boxlable + ", over");
                            PutLableInfor("生产序列号获取不成功，请检查！");
                        }
                    }
                }
                else
                {
                    tt_flag26 = true;
                    setRichtexBox("26、一机多打方案,此处不检查,请继续扫描,ok");
                }
                #endregion


                //最后判断
                #region
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10 &&
                    tt_flag11 && tt_flag12 && tt_flag13 && tt_flag14 && tt_flag15 && tt_flag16 && tt_flag17 && tt_flag18 && tt_flag19 && tt_flag20 &&
                    tt_flag21 && tt_flag22 && tt_flag23 && tt_flag24 && tt_flag25 && tt_flag26)
                {
                    //条码信息
                    this.label42.Text = tt_pcba;         //单板号
                    this.label43.Text = tt_boxlable;     //主机条码
                    if (this.label43.Text == "")
                    {
                        this.label43.Text = tt_hostlable;     //主机条码
                    }
                    this.label45.Text = tt_shortmac;     //短MAC
                    this.label46.Text = tt_longmac;      //长MAC
                    this.label47.Text = tt_gpsn;         //GPSN
                    this.label174.Text = tt_onumac;      //ONUMAC
                    this.label136.Text = tt_macusername; //用户名
                    this.label134.Text = tt_password;    //密码
                    this.label128.Text = tt_ssid;        //2G用户名
                    this.label132.Text = tt_wlanpas;     //2G密码
                    this.label124.Text = tt_5guser;      //5G用户名
                    this.label120.Text = tt_5gpassword;  //5G密码
                    this.label131.Text = tt_smtaskscode; //移动标识码
                    this.label116.Text = tt_barcode1;    //移动标识暗码

                    if (int.Parse(PrintChange) < 2)//非多打方案此时开始打印
                    {   
                        //条码信息

                        this.label44.Text = tt_intgetno.ToString();  //过站返回的最大值

                        //生产节拍
                        getProductRhythm("1");
                        getPalletBoxNo(this.label86.Text, this.label85.Text, this.label44.Text, this.label12.Text);

                        //打印记录
                        Dataset1.lablePrintRecord(tt_smalltask, tt_shortmac, tt_boxlable, "生产序列I型标签", str, tt_computermac, "", tt_conn);

                        //打印
                        if (tt_checkflag == 1)
                        {
                            GetParaDataPrint1(0);
                            if (this.label41.Text != "" && PrintChange == "1")//双打功能
                            {
                                GetParaDataPrint2(0);
                            }
                            PutLableInfor("复测check产品过站成功，打印机不出纸，请继续扫描！");
                        }
                        else
                        {
                            GetParaDataPrint1(1);
                            if (this.label41.Text != "" && PrintChange == "1")//双打功能
                            {
                                GetParaDataPrint2(1);
                            }
                            PutLableInfor("过站成功，请继续扫描！");
                        }
                        GetProductNumInfo();
                        CheckStation(tt_shortmac, tt_gyid_Use);
                        this.richTextBox1.BackColor = Color.Chartreuse;
                    }
                    else
                    {
                        //多打方案光标转移
                        PutLableInfor("请继续扫描电源！");
                        this.Mac_input.Enabled = false;
                        this.Power_input.Enabled = true;
                        Power_input.Focus();
                        Power_input.SelectAll();
                    }
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;

                    if (int.Parse(PrintChange) >= 2)//多打方案光标转移
                    {
                        //光标转移
                        this.Mac_input.Enabled = false;
                        this.EQP_input.Enabled = true;
                        EQP_input.Focus();
                        EQP_input.SelectAll();
                    }
                }

                if (int.Parse(PrintChange) < 2)//非多打方案光标转移
                {
                    if (tt_parenttask == "小型化方案")
                    {
                        //光标转移
                        this.Mac_input.Enabled = true;
                        Mac_input.Focus();
                        Mac_input.SelectAll();
                    }
                    else
                    {
                        //光标转移
                        this.Mac_input.Enabled = false;
                        this.EQP_input.Enabled = true;
                        EQP_input.Focus();
                        EQP_input.SelectAll();
                    }
                }

                #endregion
            }

        }

        //扫描电源适配器
        private void Power_input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始电源扫描
                setRichtexBox("-----开始电源扫描--------");
                Power_input.Enabled = false;
                string tt_scanshell = this.Power_input.Text.Trim().ToUpper();
                string tt_dyscanshell = tt_scanshell.Substring(0, 7);
                string tt_smalltask = this.textBox1.Text.Trim().ToUpper();
                string tt_bigtask = this.textBox9.Text.Trim().ToUpper();
                string tt_dy = this.textBox29.Text.Trim().ToUpper();
                string tt_shortmac = this.label45.Text;
                string tt_hostlable = this.label43.Text;


                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanshell, this.textBox30.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_dy, tt_dyscanshell);
                }

                //第三步判断电源是否用过
                Boolean tt_flag3 = false;
                if (tt_flag2)
                {
                    string tt_sql3 = "select maclable from odc_alllable where taskscode = '" + tt_smalltask + "' and dystlable = '" + tt_scanshell + "'";

                    DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
                    if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                    {
                        string tt_dy_maclable = ds3.Tables[0].Rows[0].ItemArray[0].ToString();  //电源查出的MAC

                        if (tt_dy_maclable == this.label45.Text)
                        {
                            tt_flag3 = true;
                            setRichtexBox("3、该电源线有使用，但与产品关联正确，产品属于重投包装线产品,goon");
                        }
                        else
                        {
                            setRichtexBox("3、该电源线已关联过，over");
                            PutLableInfor("该电源线已使用，或并不是重投产品的原有电源，请更换没有绑定过的电源");
                        }
                    }
                    else
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、该电源线没有使用,goon");
                    }
                }
                else if (tt_flag2 == false && tt_flag1 == true)
                {
                    PutLableInfor("电源前置码与系统要求不一致,请检查电源适配器是否正确!");
                }                

                //第四步更新电源
                Boolean tt_flag4 = false;
                if (tt_flag3)
                {
                    string tt_update4 = "update odc_alllable set dystlable = '" + tt_scanshell + "' " +
                                        "where taskscode = '" + tt_bigtask + "' and maclable = '" + tt_shortmac + "'";

                    int tt_execute4 = Dataset1.ExecCommand(tt_update4, tt_conn);
                    if (tt_execute4 > 0)
                    {
                        tt_flag4 = true;
                        setRichtexBox("4、电源更新成功 ,goon");
                    }
                    else
                    {
                        string tt_update4_1 = "update odc_alllable set dystlable = '" + tt_scanshell + "' " +
                                              "where taskscode = '" + tt_smalltask + "' and maclable = '" + tt_shortmac + "'";

                        int tt_execute4_1 = Dataset1.ExecCommand(tt_update4_1, tt_conn);
                        if (tt_execute4_1 > 0)
                        {
                            tt_flag4 = true;
                            setRichtexBox("4、电源更新成功 ,goon");
                        }
                        else
                        {
                            setRichtexBox("4、电源更新不成功，请重新扫描，over");
                            PutLableInfor("电源更新不成功，请重新扫描");
                        }
                    }
                }

                //第五步 获取信息
                Boolean tt_flag5 = false;
                string tt_id = "";
                if (tt_flag4)
                {
                    string tt_sql5 = "select id,dystlable from odc_alllable where taskscode = '" + tt_bigtask + "' and maclable = '" + tt_shortmac + "' ";

                    DataSet ds5 = Dataset1.GetDataSet(tt_sql5, tt_conn);
                    if (ds5.Tables.Count > 0 && ds5.Tables[0].Rows.Count > 0)
                    {
                        tt_flag5 = true;
                        tt_id = ds5.Tables[0].Rows[0].ItemArray[0].ToString();   //条码ID
                        this.label162.Text = ds5.Tables[0].Rows[0].ItemArray[1].ToString();   //电源条码
                        setRichtexBox("5、查询到关联表的数据，已关联到电源的,goon");
                    }
                    else
                    {
                        string tt_sql5_1 = "select id,dystlable from odc_alllable where taskscode = '" + tt_smalltask + "' and maclable = '" + tt_shortmac + "' ";

                        DataSet ds5_1 = Dataset1.GetDataSet(tt_sql5_1, tt_conn);
                        if (ds5_1.Tables.Count > 0 && ds5_1.Tables[0].Rows.Count > 0)
                        {
                            tt_flag5 = true;
                            tt_id = ds5_1.Tables[0].Rows[0].ItemArray[0].ToString();   //条码ID
                            this.label162.Text = ds5_1.Tables[0].Rows[0].ItemArray[1].ToString();   //电源条码
                            setRichtexBox("5、查询到关联表的数据，已关联到电源的,goon");
                        }
                        else
                        {
                            setRichtexBox("5、关联表没有查询到数据，over");
                            PutLableInfor("关联表没有查询到数据，请检查！");
                        }
                    }
                }

                //第六步物料追溯信息
                Boolean tt_flag6 = false;
                string tt_mate1 = this.Ins_Book.Text.Trim();  //说明书
                string tt_mate2 = this.Net_Line.Text.Trim();  //网线
                string tt_mate3 = this.Call_Line.Text.Trim();  //电话线
                if (tt_flag5 && tt_mate1 != "" && tt_mate2 != "")
                {
                    Boolean tt_idinfo = GetMaterialIdinfor(tt_id);
                    if (tt_idinfo)
                    {
                        string tt_insert = "insert into odc_traceback(fid,fchdate,Fsegment11,Fsegment12,Fsegment13) " +
                        "values(" + tt_id + ",getdate(),'" + tt_mate1 + "','" + tt_mate2 + "','" + tt_mate3 + "' )";

                        int tt_int1 = Dataset1.ExecCommand(tt_insert, tt_conn);

                        if (tt_int1 > 0)
                        {
                            tt_flag6 = true;
                            setRichtexBox("6、物料追溯已成功追加到物料表odc_traceback，id号：" + tt_id + ",goon");
                        }
                        else
                        {
                            setRichtexBox("6、物料追溯没有成功追加物料表！,over");
                            PutLableInfor("物料追溯没有成功追加物料表!请继续扫描");
                        }
                    }
                    else
                    {
                        string tt_update = "update odc_traceback set Fsegment11='" + tt_mate1 + "',Fsegment12='" + tt_mate2 + "',Fsegment13='" + tt_mate3 + "', Fchdate = getdate() " +
                                           "where Fid = " + tt_id;
                        int tt_int2 = Dataset1.ExecCommand(tt_update, tt_conn);

                        if (tt_int2 > 0)
                        {
                            tt_flag6 = true;
                            setRichtexBox("6、物料追溯已成功更新到物料表odc_traceback，id号：" + tt_id + ",goon");
                        }
                        else
                        {
                            setRichtexBox("6、物料追溯没有成功更新到物料表！,over");
                            PutLableInfor("物料追溯没有成功更新到物料表!请继续扫描");
                        }
                    }
                }
                else if (tt_mate1 == "" && tt_mate2 == "")
                {
                    setRichtexBox("6、物料填写为空值！,over");
                    PutLableInfor("物料填写为空值");
                }

                //第七步开始过站
                int tt_intgetno = 0;
                Boolean tt_flag7 = false;
                string tt_gyid = tt_gyid_Use;
                string tt_ccode = this.label56.Text;
                string tt_ncode = this.label57.Text;

                if (tt_flag6 && tt_hostlable == tt_shortmac)
                {
                    string tt_username = STR;
                    tt_intgetno = Dataset1.FhYDSnInStation(tt_smalltask, tt_bigtask, tt_username,
                                                          tt_hostlable, tt_shortmac, tt_shanghailabel,
                                                          tt_gyid_Use, tt_ccode, tt_ncode,
                                                          tt_conn);
                    if (tt_intgetno > 0)
                    {
                        tt_flag7 = true;
                        setRichtexBox("7、该产品过站成功，返回生产序号值:" + tt_intgetno.ToString() + ",请继续扫描,ok");
                    }
                    else
                    {
                        setRichtexBox("7、过站不成功，事务已回滚,返回生产序号值:" + tt_intgetno.ToString() + "");
                        PutLableInfor("过站不成功，请检查或再次扫描！返回值" + tt_intgetno.ToString());
                    }

                }
                else if (tt_flag6 && tt_hostlable != tt_shortmac)
                {
                    string tt_username = "已包装";
                    bool tt_flag7_1 = Dataset1.FhUnPassStationI(tt_smalltask, tt_username, tt_shortmac, tt_gyid_Use, tt_ccode, tt_ncode, tt_conn);
                    if (tt_flag7_1)
                    {
                        tt_flag7 = true;
                        setRichtexBox("7、该产品过站成功，产品已存在生产序列号:" + tt_hostlable + ",请继续扫描,ok");
                    }
                    else
                    {
                        setRichtexBox("7、过站不成功，事务已回滚,返回生产序号值:" + tt_hostlable + "");
                        PutLableInfor("过站不成功，请检查或再次扫描！");
                    }
                }

                //第八步检查序列号是否生成
                Boolean tt_flag8 = false;
                string tt_boxlable = "";
                if (tt_flag7)
                {
                    string tt_sql8 = "select count(1), min(boxlable),min(productlable) from odc_alllable " +
                                      "where taskscode = '" + tt_smalltask + "' and hprintman = '" + tt_bigtask + "' and maclable = '" + tt_shortmac + "' ";

                    string[] tt_array8 = new string[3];
                    tt_array8 = Dataset1.GetDatasetArray(tt_sql8, tt_conn);
                    if (tt_array8[0] == "1")
                    {
                        tt_flag8 = true;
                        tt_boxlable = tt_array8[1];
                        this.label67.Text = tt_boxlable;
                        this.label77.Text = tt_array8[2];
                        setRichtexBox("8、生产序列号获取成功，已获取序列号：" + tt_boxlable + ", goon");
                    }
                    else
                    {
                        setRichtexBox("8、生产序列号获取不成功，序列号：" + tt_boxlable + ", over");
                        PutLableInfor("生产序列号获取不成功，请检查！");
                    }
                }

                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8)
                {

                    //条码信息

                    this.label44.Text = tt_intgetno.ToString();  //过站返回的最大值
                    this.label43.Text = tt_boxlable; //复制生产序列号

                    //生产节拍
                    getProductRhythm("1");
                    getPalletBoxNo(this.label86.Text, this.label85.Text, this.label44.Text, this.label12.Text);

                    //打印记录
                    Dataset1.lablePrintRecord(tt_smalltask, tt_shortmac, tt_hostlable, "一机多打", str, tt_computermac, "", tt_conn);

                    //打印
                    if (tt_checkflag == 1)
                    {
                        GetParaDataPrint1(0);
                        if (this.label41.Text != "")
                        {
                            GetParaDataPrint2(0);
                        }
                        GetParaDataPrint3(0);
                        GetParaDataPrint4(0);

                        PutLableInfor("复测check产品过站成功，打印机不出纸，请继续扫描！");
                    }
                    else
                    {
                        GetParaDataPrint1(1);
                        if (this.label41.Text != "")
                        {
                            GetParaDataPrint2(1);
                        }
                        GetParaDataPrint3(1);
                        GetParaDataPrint4(1);
                        PutLableInfor("OK 电源关联成功，请扫描下一产品！");
                    }
                    GetProductNumInfo();
                    CheckStation(tt_shortmac, tt_gyid_Use);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                }

                if (tt_parenttask == "小型化方案")
                {
                    //光标转移
                    this.Power_input.Enabled = false;
                    this.Mac_input.Enabled = true;
                    Mac_input.Focus();
                    Mac_input.SelectAll();
                }
                else
                {
                    //光标转移
                    this.Power_input.Enabled = false;
                    this.EQP_input.Enabled = true;
                    EQP_input.Focus();
                    EQP_input.SelectAll();
                }

            }
        }

        //解锁特征码输入框
        private void textBox18_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.textBox18.Text == "*963.")
                {
                    this.textBox4.Enabled = true;
                    this.textBox5.Enabled = true;
                    this.textBox8.Enabled = true;
                    this.textBox18.Text = null;
                }
            }
        }

        #endregion        


        #region 11、打印

        #region I型标签打印

        //获取I型标签参数
        private void GetParaDataPrint1(int tt_itemtype)
        {
            string tt_fdata1 = this.label18.Text;

            //YX01---数据类型一
            if (tt_fdata1 == "YX01")
            {
                GetParaDataPrint1_YX01(tt_itemtype);
            }
        }        

        //----以下是YX01数据采集----
        private void GetParaDataPrint1_YX01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst1 = new DataSet();
            DataTable dt1 = new DataTable();
            dst1.Tables.Add(dt1);
            dt1.Columns.Add("参数");
            dt1.Columns.Add("名称");
            dt1.Columns.Add("内容");


            DataRow row1 = dt1.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "序列号";
            row1["内容"] = this.label43.Text;
            dt1.Rows.Add(row1);

            DataRow row2 = dt1.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "物料代码";
            row2["内容"] = this.label15.Text;
            dt1.Rows.Add(row2);

            DataRow row3 = dt1.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "河北联通";
            row3["内容"] = tt_hebeiItypedate;
            dt1.Rows.Add(row3);

            this.Itype_dataGridView.DataSource = null;
            this.Itype_dataGridView.Rows.Clear();

            this.Itype_dataGridView.DataSource = dst1.Tables[0];
            this.Itype_dataGridView.Update();

            this.Itype_dataGridView.Columns[0].Width = 60;
            this.Itype_dataGridView.Columns[1].Width = 80;
            this.Itype_dataGridView.Columns[2].Width = 200;


            //第四步 打印或预览
            //单板打印
            if (dst1.Tables.Count > 0 && dst1.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path1);
                report.SetParameterValue("S01", dst1.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("S02", dst1.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("S03", dst1.Tables[0].Rows[2][2].ToString());

                for (int i = 0; i < 500; ++i)
                {
                    string s = string.Format("Text{0}", i + 1);
                    TextObject p1 = report.FindObject(s) as TextObject;
                    if (p1 != null)
                    {
                        p1.Top += tt_top1;
                        p1.Left += tt_left1;
                    }
                    s = string.Format("Barcode{0}", i + 1);
                    BarcodeObject p2 = report.FindObject(s) as BarcodeObject;
                    if (p2 != null)
                    {
                        p2.Top += tt_top1;
                        p2.Left += tt_left1;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1 && this.Itype_printset.Text != "")
                {
                    if (int.Parse(PrintChange) >= 1)
                    {
                        //Thread.Sleep(int.Parse(Itype_PrintDelay));
                        report.PrintSettings.Printer = this.Itype_printset.Text;//双打功能
                    }
                    report.Print();
                    report.Save(tt_path1);
                    tt_top1 = 0;
                    tt_left1 = 0;
                    PutLableInfor("打印完毕");
                }

                //--预览
                if (tt_itemtype == 2)
                {
                    report.Design();
                    PutLableInfor("预览完毕");
                }

                setRichtexBox("99、打印或预览I型标签完毕，请检查，OK");

            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }
        }

        #endregion

        #region 二维码打印

        //获取二维码参数
        private void GetParaDataPrint2(int tt_itemtype)
        {
            string tt_fdata2 = this.label37.Text;

            //MP01---数据类型一
            if (tt_fdata2 == "EW01")
            {
                GetParaDataPrint2_EW01(tt_itemtype);
            }

            //MP01---数据类型一
            if (tt_fdata2 == "SN01")
            {
                GetParaDataPrint2_SN01(tt_itemtype);
            }
        }

        //----以下是EW01数据采集----
        private void GetParaDataPrint2_EW01(int tt_itemtype)
        {
            //第一步数据准备

            //数据收集

            string tt_httpdx = "https://download.189cube.com/clientdownload?ssid1="; //电信IP地址
            string tt_ssid = this.label128.Text; //默认无线网络名称
            string tt_wifipassword = this.label132.Text; //默认无线网络密匙
            string tt_password = this.label134.Text; //默认终端配置密码
            string tt_productname = this.label13.Text; //设备型号
            string tt_productmark = this.label116.Text; //设备标示

            string tt_twodimcode = tt_httpdx + tt_ssid + "&password=" + tt_wifipassword + "&useradminpw="
                                 + tt_password + "&model=" + tt_productname + "&sn=" + tt_productmark;

            string tt_httplt = "http://op.smartont.net/app/download?ssid1="; //联通IP地址
            string tt_username = this.label136.Text;
            string tt_gpsn = this.label47.Text;

            string tt_ltponword = "";
            if (tt_ponname == "GPON")
            {
                tt_ltponword = "&sn=";
            }
            else if (tt_ponname == "EPON")
            {
                tt_ltponword = "&mac=";
            }

            string tt_LTQR = tt_httplt + tt_ssid + "&password=" + tt_wifipassword + "&username=" + tt_username +
                             "&pwd=" + tt_password + "&model=" + tt_ponname + "&type=" + tt_productname +
                             tt_ltponword + tt_gpsn + "&serialnumber=" + tt_productmark + "&ip=192.168.1.1";

            string tt_LTQR_TJ = "ssid1=" + tt_ssid + "&password=" + tt_wifipassword + "&username=" + tt_username +
                                "&pwd=" + tt_password + "&model=" + tt_ponname + "&type=" + tt_productname +
                                tt_ltponword + tt_gpsn + "&serialnumber=" + tt_productmark + "&ip=192.168.1.1";

            string tt_shortmac = this.label45.Text;//MAC

            string tt_YDQR_ZJ = "厂家:烽火通信科技股份有限公司,型号:" + this.label56.Text + ",SN:" + tt_gpsn +
                                ",生产日期:" + this.label59.Text.Replace("/", ".") + ",用户无线默认SSID:" + tt_ssid +
                                ",用户无线默认SSID密码:" + tt_wifipassword + ",用户登陆默认账号:" + tt_username +
                                ",用户登陆默认密码:" + tt_password + ",设备网卡MAC:" + tt_shortmac;

            string tt_shanghaiprint = this.label77.Text;

            DataSet dst2 = new DataSet();
            DataTable dt2 = new DataTable();
            dst2.Tables.Add(dt2);
            dt2.Columns.Add("参数");
            dt2.Columns.Add("名称");
            dt2.Columns.Add("内容");


            DataRow row1 = dt2.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "电信二维码";
            row1["内容"] = tt_twodimcode;
            dt2.Rows.Add(row1);

            DataRow row2 = dt2.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "联通二维码";
            row2["内容"] = tt_LTQR;
            dt2.Rows.Add(row2);

            DataRow row3 = dt2.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "联通天津二维码";
            row3["内容"] = tt_LTQR_TJ;
            dt2.Rows.Add(row3);

            DataRow row4 = dt2.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "SN&MAC";
            row4["内容"] = tt_gpsn;
            dt2.Rows.Add(row4);

            DataRow row5 = dt2.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "移动浙江二维码";
            row5["内容"] = tt_YDQR_ZJ;
            dt2.Rows.Add(row5);

            DataRow row6 = dt2.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "上海资产编码";
            row6["内容"] = tt_shanghaiprint;
            dt2.Rows.Add(row6);

            this.QR_dataGridView.DataSource = null;
            this.QR_dataGridView.Rows.Clear();

            this.QR_dataGridView.DataSource = dst2.Tables[0];
            this.QR_dataGridView.Update();

            this.QR_dataGridView.Columns[0].Width = 50;
            this.QR_dataGridView.Columns[1].Width = 80;
            this.QR_dataGridView.Columns[2].Width = 200;


            //第四步 打印或预览
            //单板打印
            if (dst2.Tables.Count > 0 && dst2.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path2);
                report.SetParameterValue("S01", dst2.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("S02", dst2.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("S03", dst2.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("S04", dst2.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("S05", dst2.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("S06", dst2.Tables[0].Rows[5][2].ToString());

                for (int i = 0; i < 500; ++i)
                {
                    string s = string.Format("Text{0}", i + 1);
                    TextObject p1 = report.FindObject(s) as TextObject;
                    if (p1 != null)
                    {
                        p1.Top += tt_top2;
                        p1.Left += tt_left2;
                    }
                    s = string.Format("Barcode{0}", i + 1);
                    BarcodeObject p2 = report.FindObject(s) as BarcodeObject;
                    if (p2 != null)
                    {
                        p2.Top += tt_top2;
                        p2.Left += tt_left2;
                    }
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top2;
                        p3.Left += tt_left2;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1 && this.QR_printset.Text != "")
                {
                    //Thread.Sleep(int.Parse(QR_PrintDelay));
                    if ((tt_productname == "HG6201M" || tt_productname == "HG6821M") && this.label14.Text == "安徽")
                    {
                        report.PrintSettings.Printer = this.Itype_printset.Text;
                    }
                    else
                    {
                        report.PrintSettings.Printer = this.QR_printset.Text;
                    }
                    report.Print();
                    report.Save(tt_path2);
                    tt_top2 = 0;
                    tt_left2 = 0;
                    PutLableInfor("打印完毕");
                }

                //--预览
                if (tt_itemtype == 2)
                {
                    report.Design();
                    PutLableInfor("预览完毕");
                }
                setRichtexBox("99、打印或预览二维码完毕，请检查标签，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }


        }

        //----以下是SN01数据采集----
        private void GetParaDataPrint2_SN01(int tt_itemtype)
        {
            //第一步数据准备

            //数据收集

            DataSet dst2 = new DataSet();
            DataTable dt2 = new DataTable();
            dst2.Tables.Add(dt2);
            dt2.Columns.Add("参数");
            dt2.Columns.Add("名称");
            dt2.Columns.Add("内容");

            DataRow row1 = dt2.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "SN序列号";
            row1["内容"] = this.label47.Text;
            dt2.Rows.Add(row1);

            this.QR_dataGridView.DataSource = null;
            this.QR_dataGridView.Rows.Clear();

            this.QR_dataGridView.DataSource = dst2.Tables[0];
            this.QR_dataGridView.Update();

            this.QR_dataGridView.Columns[0].Width = 50;
            this.QR_dataGridView.Columns[1].Width = 80;
            this.QR_dataGridView.Columns[2].Width = 200;


            //第四步 打印或预览
            //单板打印
            if (dst2.Tables.Count > 0 && dst2.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path2);
                report.SetParameterValue("S01", dst2.Tables[0].Rows[0][2].ToString());

                for (int i = 0; i < 500; ++i)
                {
                    string s = string.Format("Text{0}", i + 1);
                    TextObject p1 = report.FindObject(s) as TextObject;
                    if (p1 != null)
                    {
                        p1.Top += tt_top2;
                        p1.Left += tt_left2;
                    }
                    s = string.Format("Barcode{0}", i + 1);
                    BarcodeObject p2 = report.FindObject(s) as BarcodeObject;
                    if (p2 != null)
                    {
                        p2.Top += tt_top2;
                        p2.Left += tt_left2;
                    }
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top2;
                        p3.Left += tt_left2;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1 && this.QR_printset.Text != "")
                {
                    //Thread.Sleep(int.Parse(QR_PrintDelay));
                    if ((this.label13.Text == "HG6201M" || this.label13.Text == "HG6821M") && this.label14.Text == "安徽")
                    {
                        report.PrintSettings.Printer = this.Itype_printset.Text;
                    }
                    else
                    {
                        report.PrintSettings.Printer = this.QR_printset.Text;
                    }
                    report.Print();
                    report.Save(tt_path2);
                    tt_top2 = 0;
                    tt_left2 = 0;
                    PutLableInfor("打印完毕");
                }

                //--预览
                if (tt_itemtype == 2)
                {
                    report.Design();
                    PutLableInfor("预览完毕");
                }
                setRichtexBox("99、打印或预览二维码完毕，请检查标签，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }


        }
        #endregion

        #region 彩盒标签打印

        //获取彩盒标签参数
        private void GetParaDataPrint3(int tt_itemtype)
        {
            string tt_fdata3 = this.label148.Text;

            //CH01---数据类型一 烽火移动彩盒
            if (tt_fdata3 == "CH01")
            {
                GetParaDataPrint1_CH01(tt_itemtype);
            }

        }

        //----以下是CH01数据采集----烽火wifi & 烽火移动
        private void GetParaDataPrint1_CH01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst3 = new DataSet();
            DataTable dt3 = new DataTable();

            dst3.Tables.Add(dt3);
            dt3.Columns.Add("参数");
            dt3.Columns.Add("名称");
            dt3.Columns.Add("内容");

            DataRow row1 = dt3.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "设备型号";
            row1["内容"] = this.label13.Text;
            dt3.Rows.Add(row1);

            DataRow row2 = dt3.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "物料编码";
            row2["内容"] = this.label15.Text;
            dt3.Rows.Add(row2);

            DataRow row3 = dt3.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "软件版本";
            row3["内容"] = this.label160.Text;
            dt3.Rows.Add(row3);

            DataRow row4 = dt3.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "出厂日期";
            row4["内容"] = this.label169.Text;
            dt3.Rows.Add(row4);

            DataRow row5 = dt3.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "长MAC";
            row5["内容"] = this.label46.Text;
            dt3.Rows.Add(row5);

            DataRow row6 = dt3.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "短MAC";
            row6["内容"] = this.label45.Text;
            dt3.Rows.Add(row6);

            DataRow row7 = dt3.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "设备标识";
            row7["内容"] = this.label131.Text;
            dt3.Rows.Add(row7);

            DataRow row8 = dt3.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "GPONSN";
            row8["内容"] = this.label174.Text;
            dt3.Rows.Add(row8);

            DataRow row9 = dt3.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "序列号";
            row9["内容"] = this.label43.Text;
            dt3.Rows.Add(row9);

            DataRow row10 = dt3.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "PON类型";
            row10["内容"] = tt_ponname;
            dt3.Rows.Add(row10);

            DataRow row11 = dt3.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "文字变量";
            row11["内容"] = this.label159.Text;
            dt3.Rows.Add(row11);

            DataRow row12 = dt3.NewRow();
            row12["参数"] = "S12";
            row12["名称"] = "产品颜色";
            row12["内容"] = this.label156.Text;
            dt3.Rows.Add(row12);

            DataRow row13 = dt3.NewRow();
            row13["参数"] = "S13";
            row13["名称"] = "产品特征";
            row13["内容"] = this.label155.Text;
            dt3.Rows.Add(row13);

            DataRow row14 = dt3.NewRow();
            row14["参数"] = "S14";
            row14["名称"] = "设备标示码暗码";
            row14["内容"] = this.label116.Text;
            dt3.Rows.Add(row14);

            DataRow row15 = dt3.NewRow();
            row15["参数"] = "S15";
            row15["名称"] = "GPSN暗码";
            row15["内容"] = this.label47.Text;
            dt3.Rows.Add(row15);

            //第二步加载到表格显示
            this.Box_dataGridView.DataSource = null;
            this.Box_dataGridView.Rows.Clear();

            this.Box_dataGridView.DataSource = dst3.Tables[0];
            this.Box_dataGridView.Update();

            this.Box_dataGridView.Columns[0].Width = 60;
            this.Box_dataGridView.Columns[1].Width = 80;
            this.Box_dataGridView.Columns[2].Width = 200;

            //第三步 打印或预览

            if (dst3.Tables.Count > 0 && dst3.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path3);
                report.SetParameterValue("S01", dst3.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("S02", dst3.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("S03", dst3.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("S04", dst3.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("S05", dst3.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("S06", dst3.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("S07", dst3.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("S08", dst3.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("S09", dst3.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("S10", dst3.Tables[0].Rows[9][2].ToString());
                report.SetParameterValue("S11", dst3.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("S12", dst3.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("S13", dst3.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("S14", dst3.Tables[0].Rows[13][2].ToString());
                report.SetParameterValue("S15", dst3.Tables[0].Rows[14][2].ToString());

                for (int i = 0; i < 500; ++i)
                {
                    string s = string.Format("Text{0}", i + 1);
                    TextObject p1 = report.FindObject(s) as TextObject;
                    if (p1 != null)
                    {
                        p1.Top += tt_top3;
                        p1.Left += tt_left3;
                    }
                    s = string.Format("Barcode{0}", i + 1);
                    BarcodeObject p2 = report.FindObject(s) as BarcodeObject;
                    if (p2 != null)
                    {
                        p2.Top += tt_top3;
                        p2.Left += tt_left3;
                    }
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top3;
                        p3.Left += tt_left3;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    //Thread.Sleep(int.Parse(BOX_PrintDelay));
                    report.PrintSettings.Printer = this.Box_printset.Text;                    
                    report.Print();
                    report.Save(tt_path3);
                    tt_top3 = 0;
                    tt_left3 = 0;
                    PutLableInfor("打印完毕");
                    setRichtexBox("打印完毕");
                }

                //--预览
                if (tt_itemtype == 2)
                {
                    report.Design();
                    PutLableInfor("预览完毕");
                }

                setRichtexBox("99、打印或预览完毕，请检查标签，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印！");
            }
        }

        #endregion

        #region II型标签打印

        //获取II型标签参数
        private void GetParaDataPrint4(int tt_itemtype)
        {
            string tt_fdata4 = this.label149.Text;

            //YX01---数据类型一
            if (tt_fdata4 == "YX01")
            {
                GetParaDataPrint2_YX01(tt_itemtype);
            }

            //CH01---数据类型一 烽火移动彩盒
            if (tt_fdata4 == "CH01")
            {
                GetParaDataPrint2_CH01(tt_itemtype);
            }
        }

        //----以下是YX01数据采集----
        private void GetParaDataPrint2_YX01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst4 = new DataSet();
            DataTable dt2 = new DataTable();
            dst4.Tables.Add(dt2);
            dt2.Columns.Add("参数");
            dt2.Columns.Add("名称");
            dt2.Columns.Add("内容");

            DataRow row1 = dt2.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "序列号";
            row1["内容"] = this.label43.Text;
            dt2.Rows.Add(row1);

            this.IItype_dataGridView.DataSource = null;
            this.IItype_dataGridView.Rows.Clear();

            this.IItype_dataGridView.DataSource = dst4.Tables[0];
            this.IItype_dataGridView.Update();

            this.IItype_dataGridView.Columns[0].Width = 60;
            this.IItype_dataGridView.Columns[1].Width = 80;
            this.IItype_dataGridView.Columns[2].Width = 200;


            //第四步 打印或预览
            //单板打印
            if (dst4.Tables.Count > 0 && dst4.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path4);
                report.SetParameterValue("S01", dst4.Tables[0].Rows[0][2].ToString());

                for (int i = 0; i < 500; ++i)
                {
                    string s = string.Format("Text{0}", i + 1);
                    TextObject p1 = report.FindObject(s) as TextObject;
                    if (p1 != null)
                    {
                        p1.Top += tt_top4;
                        p1.Left += tt_left4;
                    }
                    s = string.Format("Barcode{0}", i + 1);
                    BarcodeObject p2 = report.FindObject(s) as BarcodeObject;
                    if (p2 != null)
                    {
                        p2.Top += tt_top4;
                        p2.Left += tt_left4;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1 && this.textBox29.Text != "")
                {
                    Thread.Sleep(int.Parse(IItype_PrintDelay));
                    report.PrintSettings.Printer = this.IItype_printset.Text;
                    report.Print();
                    report.Save(tt_path4);
                    tt_top4 = 0;
                    tt_left4 = 0;
                    PutLableInfor("打印完毕");
                    setRichtexBox("打印完毕");
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

        //----以下是CH01数据采集----小型化彩盒II
        private void GetParaDataPrint2_CH01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst4 = new DataSet();
            DataTable dt4 = new DataTable();

            dst4.Tables.Add(dt4);
            dt4.Columns.Add("参数");
            dt4.Columns.Add("名称");
            dt4.Columns.Add("内容");

            DataRow row5 = dt4.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "长MAC";
            row5["内容"] = this.label46.Text;
            dt4.Rows.Add(row5);

            DataRow row6 = dt4.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "短MAC";
            row6["内容"] = this.label45.Text;
            dt4.Rows.Add(row6);

            DataRow row7 = dt4.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "设备标识";
            row7["内容"] = this.label131.Text;
            dt4.Rows.Add(row7);

            DataRow row14 = dt4.NewRow();
            row14["参数"] = "S14";
            row14["名称"] = "设备标示码暗码";
            row14["内容"] = this.label116.Text;
            dt4.Rows.Add(row14);

            //第二步加载到表格显示
            this.IItype_dataGridView.DataSource = null;
            this.IItype_dataGridView.Rows.Clear();

            this.IItype_dataGridView.DataSource = dst4.Tables[0];
            this.IItype_dataGridView.Update();

            this.IItype_dataGridView.Columns[0].Width = 60;
            this.IItype_dataGridView.Columns[1].Width = 80;
            this.IItype_dataGridView.Columns[2].Width = 200;

            //第三步 打印或预览

            if (dst4.Tables.Count > 0 && dst4.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path4);
                report.SetParameterValue("S05", dst4.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("S06", dst4.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("S07", dst4.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("S14", dst4.Tables[0].Rows[3][2].ToString());

                for (int i = 0; i < 500; ++i)
                {
                    string s = string.Format("Text{0}", i + 1);
                    TextObject p1 = report.FindObject(s) as TextObject;
                    if (p1 != null)
                    {
                        p1.Top += tt_top4;
                        p1.Left += tt_left4;
                    }
                    s = string.Format("Barcode{0}", i + 1);
                    BarcodeObject p2 = report.FindObject(s) as BarcodeObject;
                    if (p2 != null)
                    {
                        p2.Top += tt_top4;
                        p2.Left += tt_left4;
                    }
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top4;
                        p3.Left += tt_left4;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    Thread.Sleep(int.Parse(IItype_PrintDelay));
                    report.PrintSettings.Printer = this.IItype_printset.Text;
                    report.Print();
                    report.Save(tt_path4);
                    tt_top4 = 0;
                    tt_left4 = 0;
                    PutLableInfor("打印完毕");
                    setRichtexBox("打印完毕");
                }

                //--预览
                if (tt_itemtype == 2)
                {
                    report.Design();
                    PutLableInfor("预览完毕");
                }

                setRichtexBox("99、打印或预览完毕，请检查标签，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印！");
            }
        }

        #endregion

        #endregion


        #region 12、自助分单

        #region 自助分单锁定
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox5.Checked && this.textBox19.Text != "" && this.textBox19.Text.Contains("-") == false)
            {
                this.textBox19.Enabled = false;
                string tt_tasks = this.textBox19.Text.Trim().ToUpper();
                string tt_sql1 = "select a.taskscode,a.taskstate,a.taskdate,a.customer,a.pid,a.product_name,a.pon_name," +
                                 "a.tasksquantity,b.hostmax,a.stardate,a.gyid,a.issd,a.pccount,a.teamgroupid,a.softwareversion," +
                                 "a.tasktype,a.areacode,a.sver,a.svert,a.svers,a.modelname,a.vendorid,a.flhratio,a.flgratio,a.fec," +
                                 "b.hostqzwh,b.hostvalue,b.hostmode,a.onumodel,a.bosatype,a.gyid2,a.parenttask from odc_tasks as a,odc_hostlableoptioan as b " +
                                 "where a.taskscode like '" + tt_tasks + "%' and a.taskscode = b.taskscode " +
                                 "order by a.id";

                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);

                //获取分单源数据
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    string tt_product_name = "";

                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        string tt_taskscode = ds1.Tables[0].Rows[i].ItemArray[0].ToString();
                        string tt_taskstate = ds1.Tables[0].Rows[i].ItemArray[1].ToString();
                        string tt_taskdate_st = ds1.Tables[0].Rows[i].ItemArray[2].ToString();
                        string tt_taskdate = Regex.Replace(tt_taskdate_st, "-", "/");
                        string tt_customer = ds1.Tables[0].Rows[i].ItemArray[3].ToString();
                        string tt_pid = ds1.Tables[0].Rows[i].ItemArray[4].ToString();
                               tt_product_name = ds1.Tables[0].Rows[i].ItemArray[5].ToString();
                        string tt_pon_name = ds1.Tables[0].Rows[i].ItemArray[6].ToString();
                        string tt_tasksquantity = ds1.Tables[0].Rows[i].ItemArray[7].ToString();
                        string tt_hostmax = ds1.Tables[0].Rows[i].ItemArray[8].ToString();
                        string tt_stardate_st = ds1.Tables[0].Rows[i].ItemArray[9].ToString();
                        string tt_stardate = Regex.Replace(tt_stardate_st, "-", "/");
                        string tt_gyid = ds1.Tables[0].Rows[i].ItemArray[10].ToString();
                        string tt_issd = ds1.Tables[0].Rows[i].ItemArray[11].ToString();
                        string tt_pccount = ds1.Tables[0].Rows[i].ItemArray[12].ToString();
                        string tt_teamgroupid = ds1.Tables[0].Rows[i].ItemArray[13].ToString();
                        string tt_softwareversion = ds1.Tables[0].Rows[i].ItemArray[14].ToString();
                        string tt_tasktype = ds1.Tables[0].Rows[i].ItemArray[15].ToString();
                        string tt_areacode = ds1.Tables[0].Rows[i].ItemArray[16].ToString();
                        string tt_sver = ds1.Tables[0].Rows[i].ItemArray[17].ToString();
                        string tt_svert = ds1.Tables[0].Rows[i].ItemArray[18].ToString();
                        string tt_svers = ds1.Tables[0].Rows[i].ItemArray[19].ToString();
                        string tt_modelname = ds1.Tables[0].Rows[i].ItemArray[20].ToString();
                        string tt_vendorid = ds1.Tables[0].Rows[i].ItemArray[21].ToString();
                        string tt_flhratio = ds1.Tables[0].Rows[i].ItemArray[22].ToString();
                        string tt_flgratio = ds1.Tables[0].Rows[i].ItemArray[23].ToString();
                        string tt_fec = ds1.Tables[0].Rows[i].ItemArray[24].ToString();
                        string tt_hostqzwh = ds1.Tables[0].Rows[i].ItemArray[25].ToString();
                        string tt_hostvalue = ds1.Tables[0].Rows[i].ItemArray[26].ToString();
                        string tt_hostmode = ds1.Tables[0].Rows[i].ItemArray[27].ToString();
                        string tt_onumodel = ds1.Tables[0].Rows[i].ItemArray[28].ToString();
                        string tt_bosatype = ds1.Tables[0].Rows[i].ItemArray[29].ToString();
                        string tt_gyid2 = ds1.Tables[0].Rows[i].ItemArray[30].ToString();
                        string tt_parenttask = ds1.Tables[0].Rows[i].ItemArray[31].ToString();

                        PutListViewData1(tt_taskscode, tt_taskstate, tt_taskdate, tt_customer, tt_pid, tt_product_name,
                                        tt_pon_name, tt_tasksquantity, tt_hostmax, tt_stardate, tt_gyid, tt_issd, tt_pccount, tt_teamgroupid,
                                        tt_softwareversion, tt_tasktype, tt_areacode, tt_sver, tt_svert, tt_svers, tt_modelname,
                                        tt_vendorid, tt_flhratio, tt_flgratio, tt_fec, tt_hostqzwh, tt_hostvalue, tt_hostmode, tt_onumodel, 
                                        tt_bosatype, tt_gyid2, tt_parenttask);
                    }

                    string tt_sql2 = "select count(1),min(fpliietset),0 from odc_dypowertype " +
                                         "where ftype = '" + tt_product_name + "' ";

                    string[] tt_array2 = new string[3];
                    tt_array2 = Dataset1.GetDatasetArray(tt_sql2, tt_conn);
                    if (tt_array2[0] == "1")
                    {
                        tt_palletnum = Convert.ToInt32(tt_array2[1]);

                        //获取子单数量
                        tt_tasknum0 = this.listView1.Items.Count;
                        string tt_tasknum_s = Convert.ToString(tt_tasknum0);
                        
                        //获取产品生产数量
                        int[] hostnum = new int[tt_tasknum0];
                        for (int n = 0; n < tt_tasknum0; n++)
                        {
                            hostnum[n] = Convert.ToInt32(GetListViewItem1(9, n + 1));
                        }

                        //获取产品未生产数量
                        int[] leftnum = new int[tt_tasknum0];
                        for (int n = 0; n < tt_tasknum0; n++)
                        {
                            leftnum[n] = (Convert.ToInt32(GetListViewItem1(8, n + 1)) - Convert.ToInt32(GetListViewItem1(9, n + 1)));
                        }

                        //获取产品未生产数量及相关工单及ListViewData表序号
                        string[] tt_taskscodename = new string[tt_tasknum0];
                        for (int n = 0; n < tt_tasknum0; n++)
                        {
                            tt_taskscodename[n] = "Q" + "A" + GetListViewItem1(0, n + 1) + "W" + GetListViewItem1(1, n + 1) + "Z"
                                                  + "E" + (Convert.ToInt32(GetListViewItem1(8, n + 1)) - Convert.ToInt32(GetListViewItem1(9, n + 1))) + "R";
                        }

                        //获取产品已生产数量及相关工单
                        string[] tt_taskscodename_left = new string[tt_tasknum0];
                        for (int n = 0; n < tt_tasknum0; n++)
                        {
                            tt_taskscodename_left[n] = "Q" + GetListViewItem1(1, n + 1) + "E" + Convert.ToInt32(GetListViewItem1(9, n + 1)) + "R";
                        }

                        //筛选需要的分单数据
                        tt_taskmin = Convert.ToString(GetMin(hostnum, tt_tasknum0));                                 //已生产数量最少的工单产品数量
                        tt_leftmax = Convert.ToString(GetMax(leftnum, tt_tasknum0));                                 //未生产数量最多的工单产品数量
                        tt_taskinfo = GetMinTask(tt_taskscodename, tt_leftmax, tt_tasknum0);                         //未生产数量最多的工单信息及序号
                        tt_taskminname = Regex.Match(tt_taskinfo, @"(?<=W).*?(?=Z)").Groups[0].Value;                //未生产数量最多的工单信息
                        tt_tasknum = Convert.ToInt32(Regex.Match(tt_taskinfo, @"(?<=A).*?(?=W)").Groups[0].Value);   //未生产数量最多的工单序号 --自动获取表值使用
                        tt_ZeroTask = GetZeroTask(tt_taskscodename_left, tt_tasknum0);                               //0包装工单数量

                        if (tt_ZeroTask > 1)
                        {
                            this.label101.Text = "该工单尚有" + tt_ZeroTask + "个子单未包装，不建议进行分单\n如需分单请在身份验证后输入分单数";
                            this.groupBox15.Visible = true;
                        }
                        else if (tt_ZeroTask > 3)
                        {
                            this.label101.Text = "该工单尚有" + tt_ZeroTask + "个子单未包装，未投产子单数量大于3个，不允许进行分单";
                            this.groupBox15.Visible = false;
                        }
                        else if (tt_ZeroTask == 1 && Convert.ToInt32(tt_leftmax) > (tt_palletnum * 3))
                        {
                            this.label101.Text = "该工单有" + tt_ZeroTask + "个子单未包装，如需分单请在身份验证后输入分单数";
                            this.groupBox15.Visible = true;
                        }
                        else if (tt_ZeroTask == 1 && Convert.ToInt32(tt_leftmax) > (tt_palletnum * 2))
                        {
                            this.label101.Text = "该工单有" + tt_ZeroTask + "个子单未包装，该产品栈板包装量为" + tt_palletnum + "，产品剩余数量小于3栈板，\n不建议分单，如需分单请在身份验证后输入分单数";
                            this.groupBox15.Visible = true;
                        }
                        else if (tt_ZeroTask == 1 && Convert.ToInt32(tt_leftmax) <= (tt_palletnum * 2))
                        {
                            this.label101.Text = "该工单所有子单中，剩余可包装数量为" + tt_leftmax + "\n该产品栈板包装量为" + tt_palletnum + "，产品剩余数量小于等于2个栈板，不允许分单";
                            this.groupBox15.Visible = false;
                        }
                        else if (tt_ZeroTask == 0 && Convert.ToInt32(tt_leftmax) > (tt_palletnum * 3))
                        {
                            this.label101.Text = "该工单有一个剩余" + tt_leftmax + "产品未包装的工单" + tt_taskminname + "\n如需分单请在身份验证后输入分单数";
                            this.groupBox15.Visible = true;
                        }
                        else if (tt_ZeroTask == 0 && Convert.ToInt32(tt_leftmax) > (tt_palletnum * 2))
                        {
                            this.label101.Text = "该工单有一个剩余" + tt_leftmax + "产品未包装的工单" + tt_taskminname + "，该产品栈板包装量为" + tt_palletnum + "，\n产品剩余数量小于3栈板，不建议分单，如需分单请在身份验证后输入分单数";
                            this.groupBox15.Visible = true;
                        }
                        else if (tt_ZeroTask == 0 && Convert.ToInt32(tt_leftmax) <= (tt_palletnum * 2))
                        {
                            this.label101.Text = "该工单所有子单中，剩余可包装数量为" + tt_leftmax + "\n该产品栈板包装量为" + tt_palletnum + "，产品剩余数量小于等于2个栈板，不允许分单";
                            this.groupBox15.Visible = false;
                        }

                        //获取身份验证用户名
                        if (this.groupBox15.Visible == true)
                        {
                            string tt_sql3 = "select fusername from odc_fhpartitionpass where fdepart in ('生产','0') and fpermission in ('0', '2', '9') order by id";
                            DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
                            if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                            {
                                comboBox1.DataSource = ds3.Tables[0];
                                comboBox1.DisplayMember = "fusername";
                                this.comboBox1.Text = "下拉选择";
                            }

                            string tt_sql4 = "select fusername from odc_fhpartitionpass where fdepart in ('工程','0') and fpermission in ('0','9') order by id";
                            DataSet ds4 = Dataset1.GetDataSet(tt_sql4, tt_conn);
                            if (ds4.Tables.Count > 0 && ds4.Tables[0].Rows.Count > 0)
                            {
                                comboBox2.DataSource = ds4.Tables[0];
                                comboBox2.DisplayMember = "fusername";
                                this.comboBox2.Text = "下拉选择";
                            }

                            string tt_sql5 = "select fusername from odc_fhpartitionpass where fdepart in ('品质','0') and fpermission in ('0', '2') order by id";
                            DataSet ds5 = Dataset1.GetDataSet(tt_sql5, tt_conn);
                            if (ds5.Tables.Count > 0 && ds5.Tables[0].Rows.Count > 0)
                            {
                                comboBox3.DataSource = ds5.Tables[0];
                                comboBox3.DisplayMember = "fusername";
                                this.comboBox3.Text = "下拉选择";
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("没有找到产品型号:" + tt_product_name + "的配置表odc_dypowertype对应的栈板设定信息，对应字段:fpliietset,请确认！");
                    }
                }
                else
                {
                    this.label101.Text = "该工单没有查找到，请重新确认！！";
                }
            }
            else
            {
                ClearListView1();
                this.textBox19.Enabled = true;
                this.label101.Text = null;
                this.textBox20.Text = "";
                this.checkBox6.Checked = false;
                this.groupBox15.Visible = false;
                this.groupBox17.Visible = false;
                this.groupBox18.Visible = false;
                this.groupBox20.Visible = false;
                this.groupBox21.Visible = false;               
                this.comboBox1.Text = "下拉选择";
                this.comboBox2.Text = "下拉选择";
                this.comboBox3.Text = "下拉选择";
                this.textBox21.Text = "";
                this.textBox22.Text = "";
                this.textBox23.Text = "";
                this.textBox24.Text = "";
                this.textBox25.Text = "";
                this.textBox26.Text = "";
                this.comboBox1.Enabled = true;
                this.comboBox2.Enabled = true;
                this.comboBox3.Enabled = true;
                this.textBox21.Enabled = true;
                this.textBox22.Enabled = true;
                this.textBox23.Enabled = true;
                this.textBox24.Enabled = true;
                this.textBox25.Enabled = true;
                this.textBox26.Enabled = true;
            }
        }
        #endregion


        #region 限制输入类型
        private void textBox20_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (Char)8)
            {
                e.Handled = true;
            }
        }

        private void textBox21_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (Char)8)
            {
                e.Handled = true;
            }
        }

        private void textBox24_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (Char)8)
            {
                e.Handled = true;
            }
        }
        #endregion

        
        #region 身份验证

        private void button14_Click(object sender, EventArgs e)
        {
            this.comboBox1.Text = "下拉选择";
            this.comboBox2.Text = "下拉选择";
            this.comboBox3.Text = "下拉选择";
            this.textBox21.Text = "";
            this.textBox22.Text = "";
            this.textBox23.Text = "";
            this.textBox24.Text = "";
            this.textBox25.Text = "";
            this.textBox26.Text = "";
            this.comboBox1.Enabled = true;
            this.comboBox2.Enabled = true;
            this.comboBox3.Enabled = true;
            this.textBox21.Enabled = true;
            this.textBox22.Enabled = true;
            this.textBox23.Enabled = true;
            this.textBox24.Enabled = true;
            this.textBox25.Enabled = true;
            this.textBox26.Enabled = true;
            this.groupBox20.Visible = false;
            this.groupBox21.Visible = false;
            this.groupBox17.Visible = false;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text != "" && this.comboBox1.Text != "下拉选择")
            {
                string tt_usernumber_MFG = GetUserNumber(this.comboBox1.Text);
                string tt_password_MFG = GetUserPassword(this.comboBox1.Text);

                if (this.textBox21.Text == tt_usernumber_MFG && this.textBox22.Text == tt_password_MFG)
                {
                    this.groupBox20.Visible = true;
                    this.comboBox1.Enabled = false;
                    this.textBox21.Enabled = false;
                    this.textBox22.Enabled = false;
                }
                else
                {
                    MessageBox.Show("工号或密码不对，请确认");
                }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            this.comboBox2.Text = "下拉选择";
            this.comboBox3.Text = "下拉选择";
            this.textBox23.Text = "";
            this.textBox24.Text = "";
            this.textBox25.Text = "";
            this.textBox26.Text = "";
            this.comboBox2.Enabled = true;
            this.comboBox3.Enabled = true;
            this.textBox23.Enabled = true;
            this.textBox24.Enabled = true;
            this.textBox25.Enabled = true;
            this.textBox26.Enabled = true;
            this.groupBox21.Visible = false;
            this.groupBox17.Visible = false;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (this.comboBox2.Text != "" && this.comboBox2.Text != "下拉选择")
            {
                string tt_usernumber_PE = GetUserNumber(this.comboBox2.Text);
                string tt_password_PE = GetUserPassword(this.comboBox2.Text);

                if (this.textBox24.Text == tt_usernumber_PE && this.textBox23.Text == tt_password_PE)
                {
                    this.groupBox21.Visible = true;
                    this.comboBox2.Enabled = false;
                    this.textBox23.Enabled = false;
                    this.textBox24.Enabled = false;
                }
                else
                {
                    MessageBox.Show("工号或密码不对，请确认");
                }
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            this.comboBox3.Text = "下拉选择";
            this.textBox25.Text = "";
            this.textBox26.Text = "";
            this.comboBox3.Enabled = true;
            this.textBox25.Enabled = true;
            this.textBox26.Enabled = true;
            this.groupBox17.Visible = false;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (this.comboBox3.Text != "" && this.comboBox3.Text != "下拉选择")
            {
                string tt_usernumber_QA = GetUserNumber(this.comboBox3.Text);
                string tt_password_QA = GetUserPassword(this.comboBox3.Text);

                if (this.textBox26.Text == tt_usernumber_QA && this.textBox25.Text == tt_password_QA)
                {
                    this.groupBox17.Visible = true;
                    this.comboBox3.Enabled = false;
                    this.textBox25.Enabled = false;
                    this.textBox26.Enabled = false;
                    this.checkBox6.Visible = true;
                }
                else
                {
                    MessageBox.Show("工号或密码不对，请确认");
                }
            }
        }

        #endregion


        #region 分单预览
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox6.Checked && this.textBox20.Text != "" && this.textBox20.Text != "0")
            {
                this.textBox20.Enabled = false;
                int tt_tasksnewnum = Convert.ToInt32(this.textBox20.Text);
                int tt_tasknamelength = Convert.ToInt32(Convert.ToString(GetListViewItem1(1, tt_tasknum0)).Length);
                double tt_newtasksnum_math = 0;

                for (int i = 0; i < tt_tasksnewnum; i++)
                {
                    //新子单工单号
                    int tt_taskaddnum = 0;
                    string tt_tasknewname = "";
                    string tt_tasks = this.textBox19.Text.Trim().ToUpper();

                    bool tt_IsNumeric = IsNumeric(Convert.ToString(GetListViewItem1(1, tt_tasknum0).Substring(tt_tasknamelength - 1, 1)));

                    bool tt_IsNumeric2 = IsNumeric(Convert.ToString(GetListViewItem1(1, tt_tasknum0).Substring(tt_tasknamelength - 2, 2)));

                    if (tt_IsNumeric && Convert.ToString(GetListViewItem1(1, tt_tasknum0).Substring(tt_tasknamelength - 2, 1)) == "-")
                    {
                        tt_taskaddnum = Convert.ToInt32(GetListViewItem1(1, tt_tasknum0).Substring(tt_tasknamelength - 1, 1)) + i + 1;
                        tt_tasknewname = tt_tasks + "-" + Convert.ToString(tt_taskaddnum);
                    }
                    else if (tt_IsNumeric2 && Convert.ToString(GetListViewItem1(1, tt_tasknum0).Substring(tt_tasknamelength - 3, 1)) == "-")
                    {
                        tt_taskaddnum = Convert.ToInt32(GetListViewItem1(1, tt_tasknum0).Substring(tt_tasknamelength - 2, 2)) + i + 1;
                        tt_tasknewname = tt_tasks + "-" + Convert.ToString(tt_taskaddnum);
                    }
                    else
                    {
                        tt_taskaddnum = i + 1;
                        tt_tasknewname = tt_tasks + "-" + Convert.ToString(tt_taskaddnum);
                    }                    

                    //计算各子单数量
                    double tt_leftmax_math = (Convert.ToDouble(tt_leftmax)) / tt_palletnum;
                    double tt_palletnum_math = Math.Floor(tt_leftmax_math + 0.5); //产品剩余可生产的栈板数量 
                           tt_newtasksnum_math = Math.Floor(tt_palletnum_math / (tt_tasksnewnum + 1 )); //根据栈板数量计算新子单栈板数
                    string tt_tasksquantity = Convert.ToString(tt_newtasksnum_math * tt_palletnum); //新子单产品数量

                    //新子单生产序列号特征码
                    string tt_hostqzwh_now = "";
                    string tt_sql1 = "select top (1) hostqzwh from odc_hostlableoptioan " +
                                     "where (hostqzwh like '" + GetListViewItem1(26, tt_tasknum0).Substring(0, 7) + "%') " +
                                     "order by hostqzwh desc";
                    DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);
                    if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        tt_hostqzwh_now = ds1.Tables[0].Rows[0].ItemArray[0].ToString();
                    }
                    else
                    {
                        this.label100.Text = "制造单" + tt_tasks + "生产序列号代码查找有误，请找工程确认！！";
                    }

                    bool hostqzwhnum = IsNumeric(tt_hostqzwh_now.Substring(7, 1));

                    string tt_hostqzwh = "";

                    if (hostqzwhnum && Convert.ToInt32(tt_hostqzwh_now.Substring(7, 1)) + i < 9)
                    {
                        int tt_hostqzwhnum = Convert.ToInt32(tt_hostqzwh_now.Substring(7, 1)) + i + 1;
                        tt_hostqzwh = tt_hostqzwh_now.Substring(0, 7) + Convert.ToString(tt_hostqzwhnum);
                    }
                    else if (hostqzwhnum && Convert.ToInt32(tt_hostqzwh_now.Substring(7, 1)) + i >= 9)
                    {
                        int tt_hostqzwhnum = Convert.ToInt32(tt_hostqzwh_now.Substring(7, 1)) + i + 1;
                        string tt_HostAZ = HostNum_AZ(tt_hostqzwhnum);
                        tt_hostqzwh = tt_hostqzwh_now.Substring(0, 7) + tt_HostAZ;
                    }
                    else if (hostqzwhnum == false)
                    {
                        int tt_hostqzwhnum = HostAZ_Num(tt_hostqzwh_now.Substring(7, 1)) + i + 1;
                        string tt_HostAZ = HostNum_AZ(tt_hostqzwhnum);

                        if (tt_HostAZ == "0")
                        {
                            this.label101.Text = "分单数量导致生产序列号批次数大于最大值“Z”，请重新确认分单数";
                            ClearListView2();
                            this.checkBox6.Checked = false;
                            this.textBox20.Text = "";
                            textBox20.Focus();
                            return;
                        }
                        else if (tt_hostqzwhnum == 0)
                        {
                            this.label101.Text = "数据库产品批次号不在齐套性文件要求内，请确认数据库！";
                            ClearListView2();
                            this.checkBox6.Checked = false;
                            this.textBox20.Text = "";
                            textBox20.Focus();
                            return;
                        }
                        else
                        {
                            tt_hostqzwh = tt_hostqzwh_now.Substring(0, 7) + tt_HostAZ;
                        }
                    }

                    //地区特殊编码
                    string tt_hostmode = "";
                    if (GetListViewItem1(28, tt_tasknum0).Contains("SH"))
                    {
                        bool hostmodenum = IsNumeric(GetListViewItem1(28, tt_tasknum0).Substring(11, 1));

                        if (hostmodenum && Convert.ToInt32(GetListViewItem1(28, tt_tasknum0).Substring(11, 1)) + i < 9)
                        {
                            int tt_hostmodenum = Convert.ToInt32(GetListViewItem1(28, tt_tasknum0).Substring(11, 1)) + i + 1;
                            tt_hostmode = GetListViewItem1(28, tt_tasknum0).Substring(0, 11) + Convert.ToString(tt_hostmodenum);
                        }
                        else if (hostmodenum && Convert.ToInt32(GetListViewItem1(28, tt_tasknum0).Substring(11, 1)) + i >= 9)
                        {
                            int tt_hostmodenum = Convert.ToInt32(GetListViewItem1(28, tt_tasknum0).Substring(11, 1)) + i + 1;
                            string tt_hostmodeAZ = HostNum_AZ(tt_hostmodenum);

                            if (tt_hostmodeAZ == "G")
                            {
                                this.label101.Text = "分单数量导致上海地区特殊编码批次数大于最大值“F”，请重新确认分单数";
                                ClearListView2();
                                this.checkBox6.Checked = false;
                                this.textBox20.Text = "";
                                textBox20.Focus();
                                return;
                            }
                            else
                            {
                                tt_hostmode = GetListViewItem1(28, tt_tasknum0).Substring(0, 11) + tt_hostmodeAZ;
                            }
                        }
                        else if (hostmodenum == false)
                        {
                            int tt_hostmodenum = HostAZ_Num(GetListViewItem1(28, tt_tasknum0).Substring(11, 1)) + i + 1;
                            string tt_hostmodeAZ = HostNum_AZ(tt_hostmodenum);

                            if (tt_hostmodeAZ == "G")
                            {
                                this.label101.Text = "分单数量导致上海地区特殊编码批次数大于最大值“F”，请重新确认分单数";
                                ClearListView2();
                                this.checkBox6.Checked = false;
                                this.textBox20.Text = "";
                                textBox20.Focus();
                                return;
                            }
                            else if (tt_hostmodenum == 0)
                            {
                                this.label101.Text = "数据库上海地区特殊编码批次数不在齐套性文件要求内，请确认数据库！";
                                ClearListView2();
                                this.checkBox6.Checked = false;
                                this.textBox20.Text = "";
                                textBox20.Focus();
                                return;
                            }
                            else
                            {
                                tt_hostmode = GetListViewItem1(28, tt_tasknum0).Substring(0, 11) + tt_hostmodeAZ;
                            }
                        }
                    }
                    else
                    {
                        tt_hostmode = "1"; 
                    }

                    //分单人信息
                    string tt_remark = "生产" + this.comboBox1.Text + "，工程" + this.comboBox2.Text + "，品质" + this.comboBox3.Text;
                    PutListViewData2(tt_tasknewname, tt_tasksquantity, tt_hostqzwh, tt_hostmode, tt_remark);
                }

                if (tt_newtasksnum_math >= 1)
                {
                    this.label100.Text = "根据需求，即将生成" + this.textBox20.Text + "个新子单，每个子单数量为" + GetListViewItem2(2, 1) +
                         "\n生成的工单号和数量显示如下，确认无误后，点击确认按钮开始分单";
                    this.label101.Text = "";
                    this.groupBox18.Visible = true;
                }
                else
                {
                    this.label101.Text = "各子单分单数小于1栈板，请重新确认分单数";
                    this.checkBox6.Checked = false;
                    this.textBox20.Text = "";
                    textBox20.Focus();
                }
            }
            else
            {
                ClearListView2();
                this.textBox20.Enabled = true;
                textBox20.Focus();
                this.label100.Text = null;
                this.groupBox18.Visible = false;
            }
        }

        #endregion


        #region 执行分单
        private void button13_Click(object sender, EventArgs e)
        {
            int tt_tasksnewnum = Convert.ToInt32(this.textBox20.Text);
            bool tt_intgetnotasks = false;
            bool tt_intgetnohostlable = false;
            bool tt_intgetnotasksnum_o = false;

            for (int i = 0; i < tt_tasksnewnum; i++)
            {
                //制造单表赋值
                string tt_taskscode = GetListViewItem2(1, i+1);
                string tt_taskstate = GetListViewItem1(2, tt_tasknum);
                string tt_taskdate = GetListViewItem1(3, tt_tasknum);
                string tt_customer = GetListViewItem1(4, tt_tasknum);
                string tt_pid = GetListViewItem1(5, tt_tasknum);
                string tt_product_name = GetListViewItem1(6, tt_tasknum);
                string tt_pon_name = GetListViewItem1(7, tt_tasknum);
                string tt_tasksquantity = GetListViewItem2(2, i+1);
                string tt_stardate = GetListViewItem1(10, tt_tasknum);
                string tt_gyid = GetListViewItem1(11, tt_tasknum);
                string tt_issd = GetListViewItem1(12, tt_tasknum);
                string tt_pccount = GetListViewItem1(13, tt_tasknum);
                string tt_teamgroupid = GetListViewItem1(14, tt_tasknum);
                string tt_softwareversion = GetListViewItem1(15, tt_tasknum);
                string tt_tasktype = GetListViewItem1(16, tt_tasknum);
                string tt_areacode = GetListViewItem1(17, tt_tasknum);
                string tt_sver = GetListViewItem1(18, tt_tasknum);
                string tt_svert = GetListViewItem1(19, tt_tasknum);
                string tt_svers = GetListViewItem1(20, tt_tasknum);
                string tt_modelname = GetListViewItem1(21, tt_tasknum);
                string tt_vendorid = GetListViewItem1(22, tt_tasknum);
                string tt_flhratio = GetListViewItem1(23, tt_tasknum);
                string tt_flgratio = GetListViewItem1(24, tt_tasknum);
                string tt_fec = GetListViewItem1(25, tt_tasknum);
                string tt_onumodel = GetListViewItem1(29, tt_tasknum);
                string tt_remark = GetListViewItem2(5, i + 1);
                string tt_bosatype = GetListViewItem1(30, tt_tasknum);
                string tt_gyid2 = GetListViewItem1(31, tt_tasknum);
                string tt_parenttask = GetListViewItem1(32, tt_tasknum);

                //生产序列号表赋值
                string tt_hostqzwh = GetListViewItem2(3, i + 1);
                string tt_hostmode = GetListViewItem2(4, i + 1);                              

                //分单制造单处理过程
                tt_intgetnotasks = Dataset1.Fhzztasksmade(tt_taskscode, tt_taskstate, tt_taskdate, tt_customer, tt_pid, tt_product_name,
                                            tt_pon_name, tt_tasksquantity, tt_stardate, tt_gyid, tt_issd, tt_pccount, tt_teamgroupid,
                                            tt_softwareversion, tt_tasktype, tt_areacode, tt_sver, tt_svert, tt_svers, tt_modelname,
                                            tt_vendorid, tt_onumodel, tt_flhratio, tt_flgratio, tt_fec, tt_remark, tt_bosatype, tt_gyid2, tt_parenttask ,tt_conn);
                //分单生产序列号处理过程
                if (tt_intgetnotasks)
                {
                    tt_intgetnohostlable = Dataset1.Fhzzhostlablemade(tt_taskscode, tt_hostqzwh, tt_hostmode, tt_conn);
                }
            }

            //原制造单数量赋值
            int tt_tasknum_o = Convert.ToInt32(GetListViewItem1(8,tt_tasknum)) - (Convert.ToInt32(GetListViewItem2(2, 1)) * Convert.ToInt32(this.textBox20.Text));

            //更新原制造单数量
            if (tt_intgetnotasks && tt_intgetnohostlable)
            {
                tt_intgetnotasksnum_o = Dataset1.Fhzztasksnum(tt_taskminname, tt_tasknum_o, tt_conn);
            }

            if (tt_intgetnotasks && tt_intgetnohostlable && tt_intgetnotasksnum_o)
            {
                this.label100.Text = "生成" + this.textBox20.Text + "个新子单已成功，被分割工单，数量减少为" + tt_tasknum_o + "，生成的" +
                                     "\n工单号和数量如下，请在I型标签打印页面选择需要使用的新工单开始生产";
                this.label101.Text = "";
                this.groupBox18.Visible = false;
                this.checkBox6.Visible = false;
            }
            else
            {
                this.label100.Text = "分单处理失败！请联系工程师确认数据库信息是否正确！！" ;
            } 
        }

        //重置分单数
        private void button12_Click(object sender, EventArgs e)
        {
            this.checkBox6.Checked = false;
            this.textBox20.Text = "";
            textBox20.Focus();
        }


        #endregion
        
        #endregion


        #region 13、ListView操作

        //listview1清空
        private void ClearListView1()
        {
            int count = this.listView1.Items.Count;
            for (int i = 0; i < count; i++)
            {
                listView1.Items[0].Remove();
            }
        }

        //listview2清空
        private void ClearListView2()
        {
            int count = this.listView2.Items.Count;
            for (int i = 0; i < count; i++)
            {
                listView2.Items[0].Remove();
            }
        }

        //添加listview1数据
        private void PutListViewData1(string tt_taskscode, string tt_taskstate, string tt_taskdate, string tt_customer, string tt_pid, string tt_product_name, 
                                     string tt_pon_name, string tt_tasksquantity, string tt_hostmax, string tt_stardate, string tt_gyid, string tt_issd, string tt_pccount, 
                                     string tt_teamgroupid, string tt_softwareversion, string tt_tasktype, string tt_areacode, string tt_sver, string tt_svert,
                                     string tt_svers, string tt_modelname, string tt_vendorid, string tt_flhratio, string tt_flgratio, string tt_fec,
                                     string tt_hostqzwh, string tt_hostvalue, string tt_hostmode, string tt_onumodel, string tt_bosatype, string tt_gyid2, string tt_parenttask)
        {
            int i = this.listView1.Items.Count + 1;
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { i.ToString(), tt_taskscode, tt_taskstate, tt_taskdate, tt_customer, tt_pid, tt_product_name, tt_pon_name,
                                                   tt_tasksquantity, tt_hostmax, tt_stardate, tt_gyid, tt_issd, tt_pccount, tt_teamgroupid, tt_softwareversion,
                                                   tt_tasktype, tt_areacode, tt_sver, tt_svert, tt_svers, tt_modelname, tt_vendorid, tt_flhratio, tt_flgratio,
                                                   tt_fec , tt_hostqzwh, tt_hostvalue, tt_hostmode, tt_onumodel, tt_bosatype, tt_gyid2, tt_parenttask});
            this.listView1.Items.AddRange(p);
            this.listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
        }

        //添加listview2数据
        private void PutListViewData2(string tt_taskscode, string tt_tasksquantity, string tt_hostqzwh, string tt_hostmode, string tt_remark)
        {
            int i = this.listView2.Items.Count + 1;
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { i.ToString(), tt_taskscode, tt_tasksquantity, tt_hostqzwh, tt_hostmode, tt_remark});
            this.listView2.Items.AddRange(p);
            this.listView2.Items[this.listView2.Items.Count - 1].EnsureVisible();
        }


        //获取ListView1数据
        private string GetListViewItem1(int tt_itemtype, int tt_itemnumber)
        {
            string tt_item = "";

            int tt_count = this.listView1.Items.Count;

            if (tt_count >= tt_itemnumber)
            {
                if (tt_itemtype == 0)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[0].Text;
                }
                else if (tt_itemtype == 1)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[1].Text;
                }
                else if (tt_itemtype == 2)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[2].Text;
                }
                else if (tt_itemtype == 3)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[3].Text;
                }
                else if (tt_itemtype == 4)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[4].Text;
                }
                else if (tt_itemtype == 5)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[5].Text;
                }
                else if (tt_itemtype == 6)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[6].Text;
                }
                else if (tt_itemtype == 7)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[7].Text;
                }
                else if (tt_itemtype == 8)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[8].Text;
                }
                else if (tt_itemtype == 9)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[9].Text;
                }
                else if (tt_itemtype == 10)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[10].Text;
                }
                else if (tt_itemtype == 11)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[11].Text;
                }
                else if (tt_itemtype == 12)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[12].Text;
                }
                else if (tt_itemtype == 13)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[13].Text;
                }
                else if (tt_itemtype == 14)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[14].Text;
                }
                else if (tt_itemtype == 15)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[15].Text;
                }
                else if (tt_itemtype == 16)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[16].Text;
                }
                else if (tt_itemtype == 17)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[17].Text;
                }
                else if (tt_itemtype == 18)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[18].Text;
                }
                else if (tt_itemtype == 19)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[19].Text;
                }
                else if (tt_itemtype == 20)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[20].Text;
                }
                else if (tt_itemtype == 21)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[21].Text;
                }
                else if (tt_itemtype == 22)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[22].Text;
                }
                else if (tt_itemtype == 23)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[23].Text;
                }
                else if (tt_itemtype == 24)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[24].Text;
                }
                else if (tt_itemtype == 25)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[25].Text;
                }
                else if (tt_itemtype == 26)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[26].Text;
                }
                else if (tt_itemtype == 27)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[27].Text;
                }
                else if (tt_itemtype == 28)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[28].Text;
                }
                else if (tt_itemtype == 29)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[29].Text;
                }
                else if (tt_itemtype == 30)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[30].Text;
                }
                else if (tt_itemtype == 31)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[31].Text;
                }
                else if (tt_itemtype == 32)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[32].Text;
                }
            }

            return tt_item;
        }

        //获取ListView2数据
        private string GetListViewItem2(int tt_itemtype, int tt_itemnumber)
        {
            string tt_item = "";

            int tt_count = this.listView2.Items.Count;

            if (tt_count >= tt_itemnumber)
            {
                if (tt_itemtype == 0)
                {
                    tt_item = this.listView2.Items[tt_itemnumber - 1].SubItems[0].Text;
                }
                else if (tt_itemtype == 1)
                {
                    tt_item = this.listView2.Items[tt_itemnumber - 1].SubItems[1].Text;
                }
                else if (tt_itemtype == 2)
                {
                    tt_item = this.listView2.Items[tt_itemnumber - 1].SubItems[2].Text;
                }
                else if (tt_itemtype == 3)
                {
                    tt_item = this.listView2.Items[tt_itemnumber - 1].SubItems[3].Text;
                }
                else if (tt_itemtype == 4)
                {
                    tt_item = this.listView2.Items[tt_itemnumber - 1].SubItems[4].Text;
                }
                else if (tt_itemtype == 5)
                {
                    tt_item = this.listView2.Items[tt_itemnumber - 1].SubItems[5].Text;
                }
            }

            return tt_item;
        }

        #endregion

        //---------end----------
    }
}
