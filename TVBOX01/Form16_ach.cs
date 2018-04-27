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
using System.Threading;

namespace TVBOX01
{
    public partial class Form16_ach : Form
    {
        public Form16_ach()
        {
            InitializeComponent();
        }

        #region 1、属性设置

        static string tt_conn;
        int tt_yield = 0;  //产量
        static string tt_path1 = "";
        static string tt_path2 = "";
        //static string tt_md5 = "";

        //标签微调
        static float tt_top1 = 0; //彩盒上下偏移量
        static float tt_left1 = 0; //彩盒左右偏移量
        static float tt_top2 = 0; //II型上下偏移量
        static float tt_left2 = 0; //II型左右偏移量

        static int tt_reprinttime = 0; //重打次数

        static string tt_ounmac = "";//OUN MAC暗码
        DateTime tt_productstarttime = DateTime.Now; //开始时间
        DateTime tt_productprimtime; //上一次时间

        //重打限制标识
        string tt_reprintmark = "1";
        //重打限数
        int tt_reprintchang1 = 0;
        int tt_reprintchang2 = 0;
        //重打计时
        DateTime tt_reprintstattime;
        DateTime tt_reprintendtime;

        //打印模式选择
        static string PrintChange = "";
        static string IItype_PrintDelay = "";

        //本机MAC
        static string tt_computermac = "";
		
        private void Form16_ach_Load(object sender, EventArgs e)
        {
            //FastReport环境变量设置（打印时不提示 "正在准备../正在打印..",一个程序只需设定一次，故一般写在程序入口）
            (new FastReport.EnvironmentSettings()).ReportSettings.ShowProgress = false;

            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";

            //初始不显示身份验证栏
            this.groupBox15.Visible = false;

            //初始不显示微调栏
            this.groupBox14.Visible = false;

            //隐藏线长调试按钮
            this.button14.Visible = false;

            //隐藏预览按钮
            this.button2.Visible = false;
            this.button19.Visible = false;

            //隐藏打印按钮
            this.button3.Visible = false;
            this.button18.Visible = false;

            //员工账号分离
            if (str.Contains("FH004") || str.Contains("FH204"))
            {
                this.tabPage4.Parent = null;
                this.button14.Visible = true;
            }

            this.textBox6.Text = "16";
            
            ClearLabelInfo1();

            //生产节拍
            this.label24.Text = tt_yield.ToString();
            this.label25.Text = null;
            this.label26.Text = null;
            this.label27.Text = null;

            //扫描框
            this.textBox2.Visible = false;
            this.textBox3.Visible = false;
            this.textBox19.Visible = false;
            this.textBox4.Visible = false;

            this.textBox2.Enabled = false;
            this.textBox19.Enabled = false;
            this.textBox3.Enabled = false;

            //打印机设定
            this.textBox29.Text = this.label114.Text;
            this.textBox30.Text = this.label115.Text;
            this.textBox29.Enabled = false;
            this.textBox30.Enabled = false;
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


        #region  2、信息清除
        //工单锁定信息清除
        private void ClearLabelInfo1()
        {
            //工单信息
            this.label12.Text = null;
            this.label13.Text = null;
            this.label14.Text = null;
            this.label15.Text = null;
            this.label16.Text = null;
            this.label17.Text = null;
            this.label18.Text = null;
            this.label105.Text = null;
            this.label82.Text = null;
            this.label19.Text = null;
            this.label51.Text = null;
            this.label84.Text = null;
            this.label87.Text = null;
            this.label88.Text = null;
            this.label91.Text = null;
            this.label94.Text = null;
            this.label96.Text = null;
            this.label97.Text = null;
            this.textBox7.Text = null;
            this.textBox8.Text = null;
            this.textBox10.Text = null;
            this.textBox14.Text = null;

            //流程信息
            this.label71.Text = null;
            this.label72.Text = null;
            this.label73.Text = null;
            this.label74.Text = null;
            this.label78.Text = null;
            this.label80.Text = null;


            //错误显示
            this.label37.Text = null;

            //Datagridview
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;
            this.dataGridView6.DataSource = null;

            //流程表
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;

            //条码信息
            this.label44.Text = null;
            this.label45.Text = null;
            this.label46.Text = null;
            this.label47.Text = null;
            this.label48.Text = null;
            this.label49.Text = null;
            this.label59.Text = null;
            this.label61.Text = null;

            //生产数量
            this.label54.Text = null;
            this.label55.Text = null;
            this.label57.Text = null;
        }

        //重置信息清除
        private void ClearLabelInfo2()
        {
            //错误显示
            this.label37.Text = null;

            //Datagridview
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;
            this.dataGridView6.DataSource = null;

            //流程表
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;

            //条码信息
            this.label44.Text = null;
            this.label45.Text = null;
            this.label46.Text = null;
            this.label47.Text = null;
            this.label48.Text = null;
            this.label49.Text = null;
            this.label59.Text = null;
            this.label61.Text = null;

            //扫描框
            this.textBox2.Text = null;
            this.textBox3.Text = null;
            this.textBox4.Text = null;

            //流程信息
            this.label72.Text = null;

        }

        //扫描前数据初始化
        private void ScanDataInitial()
        {
            //条码信息清除
            this.label44.Text = null;
            this.label45.Text = null;
            this.label46.Text = null;
            this.label47.Text = null;
            this.label48.Text = null;
            this.label49.Text = null;
            this.label59.Text = null;
            this.label61.Text = null;

            //表格
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;
            this.dataGridView6.DataSource = null;

            //richtext
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;


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

                    if (line.Contains("IItype_PrintDelay"))
                    {
                        IItype_PrintDelay = line.Substring(line.IndexOf("=") + 1).Trim();
                    }
                }

                if (str.Contains("FH104") || str.Contains("FH214"))
                {
                    this.button2.Visible = true;
                    this.button19.Visible = true;
                    this.button3.Visible = true;
                    this.button18.Visible = true;
                    this.tabPage4.Parent = tabControl2;
                    this.textBox29.Enabled = true;
                    this.textBox30.Enabled = true;
                    //获取调试开始时间
                    tt_reprintstattime = DateTime.Now;
                }

                tt_computermac = Dataset1.GetHostIpName();
                string tt_sql1 = "select  tasksquantity,product_name,areacode,fec,convert(varchar, taskdate, 111) fdate,gyid,Tasktype,softwareversion,pon_name,fhcode " +
                                 "from odc_tasks where taskscode = '" + this.textBox1.Text + "' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);


                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label12.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    string tt_productname = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //产品名称
                    this.label14.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区编码
                    this.label16.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString();  //EC编码
                    this.label15.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //生产日期
                    this.label71.Text = ds1.Tables[0].Rows[0].ItemArray[5].ToString();  //流程信息
                    this.label51.Text = ds1.Tables[0].Rows[0].ItemArray[6].ToString();  //物料编码
                    this.label84.Text = ds1.Tables[0].Rows[0].ItemArray[7].ToString();  //软件版本
                    this.label87.Text = ds1.Tables[0].Rows[0].ItemArray[8].ToString();  //PON类型        
                    string tt_power_old = ds1.Tables[0].Rows[0].ItemArray[9].ToString();//旧电源适配器标识

                    if (tt_productname == "HG6201G" || tt_productname == "HG6201GW" || tt_productname == "HG6201GS")
                    {
                        this.label13.Text = "HG6201M";
                    }
                    else
                    {
                        this.label13.Text = tt_productname;
                    }
                    
                    //第一步、流程检查
                    Boolean tt_flag1 = false;
                    if (!this.label71.Text.Equals(""))
                    {
                        bool tt_flag = GetNextCode(this.textBox1.Text, str);
                        if (tt_flag)
                        {
                            tt_flag1 = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("该工单没有配置流程，请检查");
                    }

                    //第二步 文字变量查询
                    Boolean tt_flag2 = false;
                    if (tt_flag1)
                    {
                        if (this.label87.Text == "GPON") 
                        {

                            this.label91.Text = "吉比特";
                            tt_flag2 = true;
                        }
                        else if (this.label87.Text == "EPON")
                        {

                            this.label91.Text = "以太网";
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("文字变量无法匹配，请确认制造单下单信息，或产品是否为PON产品");
                        }
                    }

                    //第三步 电源信息查询
                    Boolean tt_flag3 = false;
                    if (tt_flag2)
                    {
                        string tt_power_search = tt_productname;
                        if (tt_productname == "HG6201M" && tt_power_old == "1.5")
                        {
                            tt_power_search = "HG6201M_OLD";
                        }

                        string tt_sql3 = "select fpwmodel,fdesc,wifi,fcolor from odc_dypowertype where ftype = '" + tt_power_search + "' ";
                        DataSet ds3 = Dataset1.GetDataSetTwo(tt_sql3, tt_conn);

                        if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                        {
                            this.label96.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString(); //电源适配器特征码
                            this.label97.Text = ds3.Tables[0].Rows[0].ItemArray[1].ToString(); //运营商
                            this.label94.Text = ds3.Tables[0].Rows[0].ItemArray[2].ToString(); //产品特征
                            this.label88.Text = ds3.Tables[0].Rows[0].ItemArray[3].ToString(); //产品颜色

                            this.textBox8.Text = this.label96.Text;

                            tt_flag3 = true;
                        }
                        else
                        {
                            MessageBox.Show("没有电源适配器信息，请确认数据库电源表");
                        }
                    }

                    //第四步 MAC特征码查询
                    Boolean tt_flag4 = false;
                    if (tt_flag3)
                    {
                        string tt_maccheck = "";
                        string tt_sql4 = "select top(1)maclable from odc_alllable where taskscode = '" + this.textBox1.Text + "' ";
                        DataSet ds4 = Dataset1.GetDataSetTwo(tt_sql4, tt_conn);

                        if (ds4.Tables.Count > 0 && ds4.Tables[0].Rows.Count > 0)
                        {
                            tt_maccheck = ds4.Tables[0].Rows[0].ItemArray[0].ToString(); //随机取该制造单关联的一个MAC
                            this.textBox7.Text = tt_maccheck.Substring(0, 6); //获取该制造单MAC特征码
                            this.textBox10.Text = this.textBox7.Text; //重打MAC的特征获取

                            this.checkBox2.Checked = true;
                            this.checkBox4.Checked = true;

                            tt_flag4 = true;
                        }
                        else
                        {
                            MessageBox.Show("没有MAC相关信息，请确认该制造单是否有包装产品");
                        }
                    }

                    //第五步 生产序列号特征码查询
                    Boolean tt_flag5 = false;
                    if (tt_flag4)
                    {
                        string tt_sql5 = "select hostqzwh from odc_hostlableoptioan where taskscode = '" + this.textBox1.Text + "' ";
                        DataSet ds5 = Dataset1.GetDataSetTwo(tt_sql5, tt_conn);

                        if (ds5.Tables.Count > 0 && ds5.Tables[0].Rows.Count > 0)
                        {
                            this.textBox14.Text = ds5.Tables[0].Rows[0].ItemArray[0].ToString(); //生产序列号特征码

                            this.checkBox6.Checked = true;

                            tt_flag5 = true;
                        }
                        else
                        {
                            MessageBox.Show("无法获取生产序列号，请确认制造单填写是否正确");
                        }
                    }

                    //第六步 EC表信息查询
                    Boolean tt_flag6 = false;
                    if (tt_flag5)
                    {
                        string tt_ec = this.label16.Text;

                        string tt_sql5_1 = "select docdesc,Fpath04,Fdata04,Fmd04 from odc_ec where zjbm = '" + tt_ec + "' ";

                        if (str.Contains("FH204") || str.Contains("FH214"))//小型化方案彩盒II
                        {
                            tt_sql5_1 = "select docdesc,Fpath09,Fdata09,Fmd09 from odc_ec where zjbm = '" + tt_ec + "' ";
                        }

                        string tt_sql5_2 = "select docdesc,Fpath10,Fdata10,Fmd10 from odc_ec where zjbm = '" + tt_ec + "' ";

                        DataSet ds5_1 = Dataset1.GetDataSet(tt_sql5_1, tt_conn);
                        DataSet ds5_2 = Dataset1.GetDataSet(tt_sql5_2, tt_conn);

                        if ((ds5_1.Tables.Count > 0 && ds5_1.Tables[0].Rows.Count > 0) && (ds5_2.Tables.Count > 0 && ds5_2.Tables[0].Rows.Count > 0))
                        {
                            this.label19.Text = ds5_1.Tables[0].Rows[0].ItemArray[0].ToString(); //EC描述
                            this.label18.Text = ds5_1.Tables[0].Rows[0].ItemArray[1].ToString(); //彩盒模板路径
                            this.label17.Text = ds5_1.Tables[0].Rows[0].ItemArray[2].ToString(); //彩盒数据类型
                            //this.label82.Text = ds5.Tables[0].Rows[0].ItemArray[3].ToString(); //MD5码
                            tt_path1 = Application.StartupPath + ds5_1.Tables[0].Rows[0].ItemArray[1].ToString();
                            //tt_md5 = ds5.Tables[0].Rows[0].ItemArray[3].ToString();

                            this.label82.Text = ds5_2.Tables[0].Rows[0].ItemArray[1].ToString(); //II型标签模板路径
                            this.label105.Text = ds5_2.Tables[0].Rows[0].ItemArray[2].ToString(); //II型标签数据类型
                            tt_path2 = Application.StartupPath + ds5_2.Tables[0].Rows[0].ItemArray[1].ToString();

                            tt_flag6 = true;
                        }
                        else
                        {
                            MessageBox.Show("没有找到工单表的EC表配置信息，请确认！");
                        }
                    }

                    //第三步 查看模板是否存在
                    Boolean tt_flag7 = false;
                    if (tt_flag6)
                    {
                        bool tt_flag7_1 = getPathIstrue(tt_path1);
                        bool tt_flag7_2 = getPathIstrue(tt_path2);
                        if (tt_flag7_1 && tt_flag7_2)
                        {
                            tt_flag7 = true;
                        }
                        else if (!tt_flag7_1 && !tt_flag7_2)
                        {
                            MessageBox.Show(" 找不到模板文件：" + tt_path1 + "，" + tt_path2 + "，请确认！");
                        }
                        else if (!tt_flag7_1)
                        {
                            MessageBox.Show(" 找不到模板文件：" + tt_path1 + "，请确认！");
                        }
                        else if (!tt_flag7_2)
                        {
                            MessageBox.Show(" 找不到模板文件：" + tt_path2 + "，请确认！");
                        }
                    }

                    //第四步 检验MD5码
                    //Boolean tt_flag8 = false;
                    //if (tt_flag7)
                    //{
                    //    string tt_md6 = GetMD5HashFromFile(tt_path);

                    //    if (tt_md5 == tt_md6)
                    //    {
                    //        tt_flag8 = true;
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("系统设定模板MD5码: '" + tt_md5 + "'与你使用模板的MD5码：'" + tt_md6 + "'不一致，请确认！");
                    //    }
                    //}


                    //最后验证
                    if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7)
                    {
                        this.textBox1.Enabled = false;
                        this.textBox2.Visible = true;
                        this.textBox3.Visible = true;
                        this.textBox19.Visible = true;
                        this.textBox4.Visible = true;

                        this.textBox2.Enabled = true;
                        this.textBox19.Enabled = false;
                        this.textBox3.Enabled = false;

                        if (str.Contains("FH204") || str.Contains("FH214"))
                        {
                            this.textBox2.Visible = false;
                            this.textBox19.Enabled = true;
                            this.textBox3.Visible = false;
                        }

                        GetProductYield();  //生产信息
                    }
                }
                else
                {
                    MessageBox.Show("没有查询此工单，请确认！");
                }
            }
            else
            {
                this.textBox1.Enabled = true;
                this.textBox2.Visible = false;
                this.textBox3.Visible = false;
                this.textBox19.Visible = false;
                this.textBox4.Visible = false;
                this.checkBox1.Checked = false;
                this.comboBox2.Text = "";
                this.textBox21.Text = "";
                this.textBox22.Text = "";
                this.comboBox2.Enabled = true;
                this.textBox21.Enabled = true;
                this.textBox22.Enabled = true;
                this.groupBox14.Visible = false;
                this.groupBox15.Visible = false;
                this.groupBox9.Visible = true;
                this.dataGridView1.Visible = true;
                this.button3.Visible = false;
                this.button18.Visible = false;
                this.tabPage4.Parent = null;
                this.tabPage3.Parent = tabControl2;
                this.textBox29.Enabled = false;
                this.textBox30.Enabled = false;
                ClearLabelInfo1();
                ScanDataInitial();
            }

        }
        
        //MAC扫描过站锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.textBox5.Enabled = false;
                this.textBox7.Enabled = false;
            }
            else
            {
                this.textBox5.Enabled = true;
                this.textBox7.Enabled = true;
            }
        }

        //生产序列号锁定 
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox6.Checked)
            {
                this.textBox15.Enabled = false;
                this.textBox14.Enabled = false;
            }
            else
            {
                this.textBox15.Enabled = true;
                this.textBox14.Enabled = true;
            }
        }

        //电源位数锁定 
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {              
            if (this.checkBox3.Checked)
            {
                this.textBox6.Enabled = false;
            }
            else
            {
                this.textBox6.Enabled = true;
            }
        }

        //MAC扫描重打锁定
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked)
            {
                this.textBox9.Enabled = false;
                this.textBox10.Enabled = false;
            }
            else
            {
                this.textBox9.Enabled = true;
                this.textBox10.Enabled = true;
            }
        }

        //物料追溯锁定
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox5.Checked)
            {
                this.textBox11.ReadOnly = true;
                this.textBox12.ReadOnly = true;
                this.textBox13.ReadOnly = true;
            }
            else
            {
                this.textBox11.ReadOnly = false;
                this.textBox12.ReadOnly = false;
                this.textBox13.ReadOnly = false;
            }
        }
        
        #endregion

        #region 4、非数据辅助功能

        //richtext加记录
        private void setRichtexBox(string tt_textinfor)
        {
            this.richTextBox1.Text = this.richTextBox1.Text + tt_textinfor + "\n";
        }

        //lable提示信息
        private void PutLableInfor(string tt_lableinfo)
        {
            this.label37.Text = tt_lableinfo;
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
        private void getProductRhythm()
        {
            tt_yield = tt_yield + 1;  //产量1

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
            this.label24.Text = tt_yield.ToString();   //本班产量
            this.label25.Text = tt_time;               //生产时间
            this.label26.Text = tt_avgtime.ToString();  //平均节拍
            this.label27.Text = tt_differtime2;        //实时节拍

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

        #endregion


        #region 5、数据辅助功能

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

                    tt_process = tt_array2[2];
                    tt_ccodenumber = GetCodeRoutNum(tt_ccode, tt_process); //获取站位顺序
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

                string tt_sql4 = "select count(1),min(z.pxid),min(z.lcbz) " +
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
                this.label73.Text = tt_ccode;
                this.label74.Text = tt_ncode;
                this.label80.Text = tt_ccodenumber;
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

        //生产数量
        private void GetProductYield()
        {
            string tt_sql = "select count(1), sum(case when productman is not null then 1 else 0 end ) as Fcount1,max(boxlable) " +
                            "from odc_alllable where taskscode = '" + this.textBox1.Text + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label54.Text = tt_array[0];
            this.label55.Text = tt_array[1];
            this.label57.Text = tt_array[2];
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

        //刷新站位
        private void CheckStation(string tt_mac)
        {
            string tt_sql = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime,fremark " +
                            "from ODC_ROUTINGTASKLIST    where pcba_pn = '" + tt_mac + "' order by createtime desc";

            DataSet ds1 = Dataset1.GetDataSet(tt_sql, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView1.DataSource = ds1;
                dataGridView1.DataMember = "Table";
                this.label72.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //当前站位

                //获取流程的顺序值
                string tt_newcode = ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                string tt_process = this.label71.Text;
                this.label78.Text = GetCodeRoutNum(tt_newcode, tt_process);
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

        //删除条码比对数据
        private int Delete_Check(string tt_mac)
        {
            string tt_deletesql = "delete from odc_check_barcode where maclable = '" + tt_mac + "'";
            int tt_Checknum = Dataset1.ExecCommand(tt_deletesql, tt_conn);
            return tt_Checknum;
        }

        //查询重打记录
        private bool CheckPrintRecord(string tt_fhostlable, string tt_flocal)
        {
            string tt_sql = "select count(1), min(Fname), min(fhostlable) " +
                            "from odc_lableprint where fhostlable = '" + tt_fhostlable + "'" +
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

        #endregion


        #region 6、数据查询

        //数据查询 确定
        private void button4_Click(object sender, EventArgs e)
        {
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;

            string tt_task = "";
            string tt_pcba = "";
            string tt_mac = "";
            Boolean tt_flag = false;

            string tt_sn1 = this.textBox16.Text.Trim();
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
                string tt_sql2 = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime,fremark " +
                            "from ODC_ROUTINGTASKLIST    where pcba_pn = '" + tt_mac + "' order by createtime desc";

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


        //数据查询 重置
        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox16.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;
        }

        #endregion


        #region 7、获取MD5码

        //获取文件名
        private void button6_Click(object sender, EventArgs e)
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
                this.textBox18.Text = file;
            }
        }

        //获取MD5码
        private void button7_Click(object sender, EventArgs e)
        {
            string tt_fliename = this.textBox18.Text;

            string tt_md5 = GetMD5HashFromFile(tt_fliename);

            this.textBox17.Text = tt_md5;
        }

        //重置
        private void button8_Click(object sender, EventArgs e)
        {
            this.textBox18.Text = null;
            this.textBox17.Text = null;
        }

        #endregion


        #region 8、按钮功能
        //按钮功能 重置
        private void button1_Click(object sender, EventArgs e)
        {
            ClearLabelInfo2();
        }

        //彩盒标签预览
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {
                string tt_prientcode = this.label78.Text;
                string tt_checkcode = this.label80.Text;

                Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                if (tt_flag)
                {
                    GetParaDataPrint1(2);  //预览
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

            textBox2.Focus();
            textBox2.SelectAll();
        }

        //打印彩盒标签
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
                if (str.Contains("FH004"))
                {
                    tt_info = "，待装箱产品需要重新条码比对";
                }
                DialogResult dr = MessageBox.Show("确定要重打标签吗，打印信息被记录" + tt_info, "标签重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label78.Text;
                    string tt_checkcode = this.label80.Text;
                    string tt_recordmac = this.textBox4.Text;

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
                        string tt_local = "彩盒标签";
                        string tt_username = "";
                        if (str.Contains("FH004"))
                        {
                            tt_username = this.comboBox2.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }
                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac ,tt_remark);

                        if (str.Contains("FH004"))
                        {
                            if (tt_nowcode == "3201")
                            {
                                int delete_checknum = Delete_Check(tt_recordmac);
                                setRichtexBox("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                                PutLableInfor("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("当前站位：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",或装箱产品已打散,才能重打标签");
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
                else
                {

                }
            }

            tt_reprintstattime = DateTime.Now;
        }

        //II型标签预览
        private void button19_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {
                string tt_prientcode = this.label78.Text;
                string tt_checkcode = this.label80.Text;

                Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                if (tt_flag)
                {
                    GetParaDataPrint2(2);  //预览
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

            textBox2.Focus();
            textBox2.SelectAll();
        }

        //打印II型标签
        private void button18_Click(object sender, EventArgs e)
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
                if (str.Contains("FH004"))
                {
                    tt_info = "，装箱产品需要重新条码比对";
                }
                DialogResult dr = MessageBox.Show("确定要重打标签吗，打印信息被记录" + tt_info, "标签重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label78.Text;
                    string tt_checkcode = this.label80.Text;
                    string tt_recordmac = this.textBox4.Text;

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
                        string tt_local = "II型标签";
                        string tt_username = "";
                        if (str.Contains("FH004"))
                        {
                            tt_username = this.comboBox2.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }
                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);

                        if (str.Contains("FH004"))
                        {
                            if (tt_nowcode == "3201")
                            {
                                int delete_checknum = Delete_Check(tt_recordmac);
                                setRichtexBox("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                                PutLableInfor("产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("当前站位：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",或装箱产品已打散,才能重打标签");
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
                else
                {

                }
            }

            tt_reprintstattime = DateTime.Now;
        }

        //线长调试模式
        private void button14_Click(object sender, EventArgs e)
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
                    comboBox2.DataSource = ds1.Tables[0];
                    comboBox2.DisplayMember = "fusername";
                    this.groupBox14.Visible = true;
                    this.groupBox9.Visible = false;
                    this.dataGridView1.Visible = false;
                    this.comboBox1.Text = "0.3";
                    this.comboBox2.Text = "下拉选择";
                    this.textBox21.Text = "";
                    this.textBox22.Text = "";
                    this.comboBox2.Enabled = true;
                    this.textBox21.Enabled = true;
                    this.textBox22.Enabled = true;
                    this.groupBox15.Visible = false;
                    this.button3.Visible = false;
                    this.button18.Visible = false;
                    this.textBox29.Enabled = false;
                    this.textBox30.Enabled = false;
                    this.tabPage4.Parent = null;
                    this.tabPage3.Parent = tabControl2;
                    this.textBox4.Enabled = true;
                    this.textBox4.Text = "";
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
        private void textBox21_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (Char)8)
            {
                e.Handled = true;
            }
        }

        //线长身份验证
        private void button15_Click(object sender, EventArgs e)
        {
            if (this.comboBox2.Text != "" && this.comboBox2.Text != "下拉选择")
            {
                string tt_usernumber_MFG = GetUserNumber(this.comboBox2.Text);
                string tt_password_MFG = GetUserPassword(this.comboBox2.Text);

                if (this.textBox21.Text == tt_usernumber_MFG && this.textBox22.Text == tt_password_MFG)
                {
                    this.groupBox15.Visible = true;
                    this.comboBox2.Enabled = false;
                    this.textBox21.Enabled = false;
                    this.textBox22.Enabled = false;
                    this.button3.Visible = true;
                    if (PrintChange == "1")
                    {
                        this.button18.Visible = true;//双打功能
                        this.tabPage6.Parent = tabControl3;
                    }
                    else
                    {
                        this.tabPage6.Parent = null;//禁用II型标签微调，双打功能暂时不启动
                    }
                    this.textBox29.Enabled = true;
                    this.textBox30.Enabled = true;
                    this.tabPage3.Parent = null;
                    this.tabPage4.Parent = tabControl2;
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
        private void button16_Click(object sender, EventArgs e)
        {
            this.comboBox1.Text = "0.3";
            this.comboBox2.Text = "下拉选择";
            this.textBox21.Text = "";
            this.textBox22.Text = "";
            this.comboBox2.Enabled = true;
            this.textBox21.Enabled = true;
            this.textBox22.Enabled = true;
            this.groupBox15.Visible = false;
            this.button3.Visible = false;
            this.button18.Visible = false;
            this.textBox29.Enabled = false;
            this.textBox30.Enabled = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
        }

        //取消身份验证过程，并结束设置
        private void button17_Click(object sender, EventArgs e)
        {
            this.comboBox1.Text = "0.3";
            this.comboBox2.Text = "下拉选择";
            this.textBox21.Text = "";
            this.textBox22.Text = "";
            this.comboBox2.Enabled = true;
            this.textBox21.Enabled = true;
            this.textBox22.Enabled = true;
            this.groupBox14.Visible = false;
            this.groupBox15.Visible = false;
            this.groupBox9.Visible = true;
            this.dataGridView1.Visible = true;
            this.button3.Visible = false;
            this.button18.Visible = false;
            this.textBox29.Enabled = false;
            this.textBox30.Enabled = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
        }

        //彩盒上移按钮
        private void button9_Click(object sender, EventArgs e)
        {
            tt_top1 -= float.Parse(this.comboBox1.Text);
        }

        //彩盒下移按钮
        private void button10_Click(object sender, EventArgs e)
        {
            tt_top1 += float.Parse(this.comboBox1.Text);
        }

        //彩盒左移按钮
        private void button11_Click(object sender, EventArgs e)
        {
            tt_left1 -= float.Parse(this.comboBox1.Text);
        }

        //彩盒右移按钮
        private void button12_Click(object sender, EventArgs e)
        {
            tt_left1 += float.Parse(this.comboBox1.Text);
        }

        //II型上移按钮
        private void button21_Click(object sender, EventArgs e)
        {
            tt_top2 -= float.Parse(this.comboBox1.Text);
        }

        //II型下移按钮
        private void button20_Click(object sender, EventArgs e)
        {
            tt_top2 += float.Parse(this.comboBox1.Text);
        }

        //II型左移按钮
        private void button22_Click(object sender, EventArgs e)
        {
            tt_left2 -= float.Parse(this.comboBox1.Text);
        }

        //II型右移按钮
        private void button23_Click(object sender, EventArgs e)
        {
            tt_left2 += float.Parse(this.comboBox1.Text);
        }

        //结束设置
        private void button13_Click(object sender, EventArgs e)
        {
            this.comboBox1.Text = "0.3";
            this.comboBox2.Text = "下拉选择";
            this.textBox21.Text = "";
            this.textBox22.Text = "";
            this.comboBox2.Enabled = true;
            this.textBox21.Enabled = true;
            this.textBox22.Enabled = true;
            this.groupBox14.Visible = false;
            this.groupBox15.Visible = false;
            this.groupBox9.Visible = true;
            this.dataGridView1.Visible = true;
            this.button3.Visible = false;
            this.button18.Visible = false;
            this.textBox29.Enabled = false;
            this.textBox30.Enabled = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
        }

        #endregion


        #region 9、条码扫描
        //彩盒重打MAC扫描
        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描--------");
                string tt_scanmac = this.textBox4.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace("-", "");


                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox9.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox10.Text.Trim());
                }


                //第三步 判断路径
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    bool tt_flag3_1 = getPathIstrue(tt_path1);
                    bool tt_flag3_2 = getPathIstrue(tt_path2);
                    if (tt_flag3_1 && tt_flag3_2)
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、已找到所有模板,：" + tt_path1 + "，" + tt_path2 + ",goon");
                    }
                    else if (!tt_flag3_1 && !tt_flag3_2)
                    {
                        setRichtexBox("3、没有找到模板,：" + tt_path1 + "," + tt_path2 + ",over");
                        PutLableInfor("没有找到模板，请检查！");
                    }
                    else if (!tt_flag3_1)
                    {
                        setRichtexBox("3、没有找到彩盒标签模板,：" + tt_path1 + ",over");
                        PutLableInfor("没有找到彩盒标签模板，请检查！");
                    }
                    else if (!tt_flag3_2)
                    {
                        setRichtexBox("3、没有找到II型标签模板,：" + tt_path2 + ",over");
                        PutLableInfor("没有找到II型标签模板，请检查！");
                    }
                }


                //第三步查找信息
                Boolean tt_flag4 = false;
                if (tt_flag1 && tt_flag2)
                {
                    string tt_sql4 = "select pcbasn,hostlable,maclable,smtaskscode,boxlable,dystlable,bprintuser,shelllable from odc_alllable " +
                                     "where taskscode = '" + this.textBox1.Text + "' and maclable = '" + tt_shortmac + "' ";


                    DataSet ds4 = Dataset1.GetDataSet(tt_sql4, tt_conn);
                    if (ds4.Tables.Count > 0 && ds4.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        this.label44.Text = ds4.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();  //单板号
                        this.label45.Text = ds4.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();  //主机条码
                        this.label46.Text = ds4.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();  //短MAC
                        this.label47.Text = ds4.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();  //设备标识码
                        this.label48.Text = ds4.Tables[0].Rows[0].ItemArray[4].ToString().ToUpper();  //彩盒条码
                        this.label49.Text = ds4.Tables[0].Rows[0].ItemArray[5].ToString();  //电源条码
                        this.label59.Text = ds4.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper();  //长MAC
                        this.label61.Text = ds4.Tables[0].Rows[0].ItemArray[7].ToString().ToUpper();  //GPSN

                        this.label98.Text = Regex.Replace(this.label47.Text, " ", "");

                        if (this.label87.Text == "EPON")
                        {
                            tt_ounmac = Regex.Replace(this.label61.Text, "-", "");
                        }
                        else
                        {
                            tt_ounmac = this.label61.Text;
                        }
                        
                        setRichtexBox("4、关联表查询到一条数据，goon");

                    }
                    else
                    {
                        setRichtexBox("4、关联表没有查询到数据，over");
                        PutLableInfor("关联表没有查询到数据，请检查！");
                    }

                }

                //第五步查询macinfo表信息
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    tt_flag5 = true;
                    setRichtexBox("5、Macinfo表查找数据过,goon");

                }


                //第六步 查找站位信息
                Boolean tt_flag6 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                {
                    tt_flag6 = true;
                    setRichtexBox("6、查找站位信息过,goon");
                }


                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {
                    GetParaDataPrint1(0);
                    if (PrintChange == "1")
                    {
                        GetParaDataPrint2(0);//双打功能
                    }
                    GetProductYield();
                    CheckStation(tt_shortmac);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    Dataset1.AddLog("彩盒标签日志", this.textBox1.Text, tt_shortmac, this.richTextBox1.Text, "侧边栏显示LOG");
                    textBox4.Focus();
                    textBox4.SelectAll();

                    if (tt_reprintmark == "0")
                    {
                        this.textBox4.Enabled = false;
                    }
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox4.Focus();
                    textBox4.SelectAll();
                }
            }
        }

        
        //MAC扫描
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描--------");
                string tt_scanmac = this.textBox2.Text.Trim().ToUpper();
                string tt_shortmac = tt_scanmac.Replace("-", "");
                string tt_task = this.textBox1.Text.Trim().ToUpper();


                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox5.Text);

                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox7.Text.Trim());
                }


                //第三步 检查模板
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    bool tt_flag3_1 = getPathIstrue(tt_path1);
                    bool tt_flag3_2 = getPathIstrue(tt_path2);
                    if (tt_flag3_1 && tt_flag3_2)
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、已找到所有模板,：" + tt_path1 + "，" + tt_path2 + ",goon");
                    }
                    else if (!tt_flag3_1 && !tt_flag3_2)
                    {
                        setRichtexBox("3、没有找到模板,：" + tt_path1 + "," + tt_path2 + ",over");
                        PutLableInfor("没有找到模板，请检查！");
                    }
                    else if (!tt_flag3_1)
                    {
                        setRichtexBox("3、没有找到彩盒标签模板,：" + tt_path1 + ",over");
                        PutLableInfor("没有找到彩盒标签模板，请检查！");
                    }
                    else if (!tt_flag3_2)
                    {
                        setRichtexBox("3、没有找到II型标签模板,：" + tt_path2 + ",over");
                        PutLableInfor("没有找到II型标签模板，请检查！");
                    }
                }

                //第四步扣数检查
                Boolean tt_flag4 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    tt_flag4 = true;
                    setRichtexBox("4、物料扣数过，gong");
                }

                //第五步物料检查
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    if (this.textBox11.Text == "" || this.textBox12.Text == "")
                    {
                        setRichtexBox("4、物料填写有空值,over");
                        PutLableInfor("物料填写有空值，请检查！");
                    }
                    else if (this.label13.Text == "HG6821T-U" && this.textBox13.Text == "")
                    {
                        setRichtexBox("4、物料填写有空值,over");
                        PutLableInfor("该产品物料有电话线，请填写物料！");
                    }
                    else
                    {
                        tt_flag5 = true;
                        setRichtexBox("4、物料填写都不为空，gong");
                    }
                }

                //第六步流程检查
                Boolean tt_flag6 = false;
                string tt_gyid = this.label71.Text;
                string tt_ccode = this.label73.Text;
                string tt_ncode = this.label74.Text;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                {
                    if (tt_ccode == "" || tt_ncode == "")
                    {
                        setRichtexBox("6、该工单没有配置流程," + tt_ccode + "," + tt_ncode + ",over");
                        PutLableInfor("没有获取到当前待测站位，及下一站位，请检查");
                    }
                    else
                    {
                        tt_flag6 = true;
                        setRichtexBox("6、该工单已配置流程," + tt_ccode + "," + tt_ncode + ",goon");
                    }

                }

                //第七步查找关联表数据
                Boolean tt_flag7 = false;
                string tt_hostlable = "";
                string tt_pcba = "";
                string tt_smtaskscode = "";
                string tt_longmac = "";
                string tt_gpsn = "";
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {
                    string tt_sql7 = "select hostlable,pcbasn,smtaskscode,bprintuser,shelllable from odc_alllable " +
                                     "where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";

                    DataSet ds7 = Dataset1.GetDataSet(tt_sql7, tt_conn);
                    if (ds7.Tables.Count > 0 && ds7.Tables[0].Rows.Count > 0)
                    {
                        tt_flag7 = true;
                        tt_hostlable = ds7.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper(); //主机条码
                        tt_pcba = ds7.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();      //单板号
                        tt_smtaskscode = ds7.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();  //移动串号
                        tt_longmac = ds7.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();       //长MAC
                        tt_gpsn = ds7.Tables[0].Rows[0].ItemArray[4].ToString().ToUpper();       //GPSN
                        setRichtexBox("7、关联表查询到一条数据，hostlable=" + tt_hostlable + ",pcba=" + tt_pcba + ",smtaskscode="
                                            + tt_smtaskscode + ",mac=" + tt_longmac + ",Gpsn=" + tt_gpsn + ",goon");
                    }
                    else
                    {
                        setRichtexBox("7、关联表没有查询到数据，over");
                        PutLableInfor("关联表没有查询到数据，请检查！");
                    }
                }

                //第八步串码是否存在检查
                Boolean tt_flag8 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7)
                {

                    if (tt_hostlable == tt_shortmac)
                    {

                        setRichtexBox("8、该MAC主机条码为：" + tt_hostlable + ",还没有获取彩盒21号，over");
                        PutLableInfor("主机条码为," + tt_hostlable + ",还没有获取获取彩盒21");
                    }
                    else
                    {
                        tt_flag8 = true;
                        setRichtexBox("8、该MAC已有有彩盒21：" + tt_hostlable + ",goon");

                    }
                }

                //第九步 查找站位信息
                Boolean tt_flag9 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8)
                {
                    string tt_sql9 = "select count(1),min(ccode),min(ncode) from odc_routingtasklist " +
                                     "where  pcba_pn = '" + tt_shortmac + "' and napplytype is null ";


                    string[] tt_array9 = new string[3];
                    tt_array9 = Dataset1.GetDatasetArray(tt_sql9, tt_conn);
                    if (tt_array9[0] == "1")
                    {
                        if (tt_array9[2] == tt_ccode)
                        {
                            tt_flag9 = true;
                            setRichtexBox("9、该单板有待测站位，站位：" + tt_array9[1] + "，" + tt_array9[2] + ",可以过站 goon");
                        }
                        else
                        {
                            setRichtexBox("9、该单板待测站位不在" + tt_ccode + "，站位：" + tt_array9[1] + "，" + tt_array9[2] + ",不可以过站 goon");
                            PutLableInfor("该单板当前站位：" + tt_array9[2] + "不在" + tt_ccode + "站位！");
                        }

                    }
                    else
                    {
                        setRichtexBox("9、没有找到待测站位，或有多条待测站位，流程异常，over");
                        PutLableInfor("没有找到待测站位，或有多条待测站位，流程异常！");
                    }
                }

                //第十步查询macinfo表信息
                Boolean tt_flag10 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9)
                {
                    tt_flag10 = true;
                    setRichtexBox("10、Macinfo表查找数据过,goon");
                }

                //第十一步物料追溯添加
                Boolean tt_flag11 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10)
                {
                    tt_flag11 = true;
                    setRichtexBox("11、物料追溯记录过，gong");
                }

                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag10 && tt_flag11)
                {
                    CheckStation(tt_shortmac);
                    this.richTextBox1.BackColor = Color.Aquamarine;
                    PutLableInfor("OK MAC扫描成功，请扫描生产序列号！");
                    this.textBox2.Enabled = false;
                    this.textBox19.Enabled = true;
                    this.textBox3.Enabled = false;
                    textBox19.Focus();
                    textBox19.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox2.Focus();
                    textBox2.SelectAll();
                }
            }
        }

        //扫描生产序列号
        private void textBox19_KeyDown(object sender, KeyEventArgs e)
        {
            if (str.Contains("FH204") || str.Contains("FH214"))
            {
                if (e.KeyCode == Keys.Enter)//小型化产品彩盒标签II，原扫描处理在else项下
                {
                    //---开始序列号扫描
                    ScanDataInitial();
                    setRichtexBox("-----开始序列号扫描--------");
                    string tt_hostlable = this.textBox19.Text.Trim();

                    //第一步位数判断
                    Boolean tt_flag1 = false;
                    tt_flag1 = CheckStrLengh(tt_hostlable, this.textBox15.Text);


                    //第二步包含符判断
                    Boolean tt_flag2 = false;
                    if (tt_flag1)
                    {
                        tt_flag2 = CheckStrContain(tt_hostlable, this.textBox14.Text.Trim());
                    }


                    //第三步 判断路径
                    Boolean tt_flag3 = false;
                    if (tt_flag1 && tt_flag2)
                    {
                        bool tt_flag3_1 = getPathIstrue(tt_path1);
                        bool tt_flag3_2 = getPathIstrue(tt_path2);
                        if (tt_flag3_1 && tt_flag3_2)
                        {
                            tt_flag3 = true;
                            setRichtexBox("3、已找到所有模板,：" + tt_path1 + "，" + tt_path2 + ",goon");
                        }
                        else if (!tt_flag3_1 && !tt_flag3_2)
                        {
                            setRichtexBox("3、没有找到模板,：" + tt_path1 + "," + tt_path2 + ",over");
                            PutLableInfor("没有找到模板，请检查！");
                        }
                        else if (!tt_flag3_1)
                        {
                            setRichtexBox("3、没有找到彩盒标签模板,：" + tt_path1 + ",over");
                            PutLableInfor("没有找到彩盒标签模板，请检查！");
                        }
                        else if (!tt_flag3_2)
                        {
                            setRichtexBox("3、没有找到II型标签模板,：" + tt_path2 + ",over");
                            PutLableInfor("没有找到II型标签模板，请检查！");
                        }
                    }


                    //第三步查找信息
                    Boolean tt_flag4 = false;
                    if (tt_flag1 && tt_flag2)
                    {
                        string tt_sql4 = "select pcbasn,hostlable,maclable,smtaskscode,boxlable,dystlable,bprintuser,shelllable from odc_alllable " +
                                         "where taskscode = '" + this.textBox1.Text + "' and hostlable = '" + tt_hostlable + "' ";


                        DataSet ds4 = Dataset1.GetDataSet(tt_sql4, tt_conn);
                        if (ds4.Tables.Count > 0 && ds4.Tables[0].Rows.Count > 0)
                        {
                            tt_flag4 = true;
                            this.label44.Text = ds4.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();  //单板号
                            this.label45.Text = ds4.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();  //主机条码
                            this.label46.Text = ds4.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();  //短MAC
                            this.label47.Text = ds4.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();  //设备标识码
                            this.label48.Text = ds4.Tables[0].Rows[0].ItemArray[4].ToString().ToUpper();  //彩盒条码
                            this.label49.Text = ds4.Tables[0].Rows[0].ItemArray[5].ToString();  //电源条码
                            this.label59.Text = ds4.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper();  //长MAC
                            this.label61.Text = ds4.Tables[0].Rows[0].ItemArray[7].ToString().ToUpper();  //GPSN

                            this.label98.Text = Regex.Replace(this.label47.Text, " ", "");

                            if (this.label87.Text == "EPON")
                            {
                                tt_ounmac = Regex.Replace(this.label61.Text, "-", "");
                            }
                            else
                            {
                                tt_ounmac = this.label61.Text;
                            }

                            setRichtexBox("4、关联表查询到一条数据，goon");

                        }
                        else
                        {
                            setRichtexBox("4、关联表没有查询到数据，over");
                            PutLableInfor("关联表没有查询到数据，请检查！");
                        }

                    }

                    //第五步查询macinfo表信息
                    Boolean tt_flag5 = false;
                    if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                    {
                        tt_flag5 = true;
                        setRichtexBox("5、Macinfo表查找数据过,goon");
                    }


                    //第六步 查找彩盒II打印信息
                    Boolean tt_flag6 = false;
                    if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                    {
                        bool tt_flag6_1 = CheckPrintRecord(tt_hostlable, "彩盒标签II");

                        if (tt_flag6_1 == true)
                        {
                            setRichtexBox("6、彩盒II标签已经打印过，over");
                            PutLableInfor("此产品彩盒II标签已经打印过，请检查！");
                        }
                        else if (tt_flag6_1 == false)
                        {
                            tt_flag6 = true;
                            setRichtexBox("6、重打检查OK,goon");
                        }
                    }


                    //最后判断
                    if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                    {
                        Dataset1.lablePrintRecord(this.textBox1.Text, this.label46.Text, this.label45.Text, "彩盒标签II", str, tt_computermac, "电信小型化产品彩盒标签II", tt_conn);
                        
                        GetParaDataPrint1(1);
                        PutLableInfor("OK 彩盒标签II打印成功，请继续扫描下一片");
                        GetProductYield();
                        CheckStation(tt_hostlable);
                        this.richTextBox1.BackColor = Color.Chartreuse;
                        Dataset1.AddLog("彩盒标签日志", this.textBox1.Text, this.label46.Text, this.richTextBox1.Text, "侧边栏显示LOG");
                        textBox19.Focus();
                        textBox19.SelectAll();
                    }
                    else
                    {
                        this.richTextBox1.BackColor = Color.Red;
                        textBox19.Focus();
                        textBox19.SelectAll();
                    }
                }
            }
            else
            {
                if (e.KeyCode == Keys.Enter)
                {

                    //---开始生产序列号扫描
                    setRichtexBox("-----开始生产序列号扫描--------");
                    string tt_hostlable = this.textBox19.Text.Trim().ToUpper();
                    string tt_task = this.textBox1.Text.Trim().ToUpper();
                    string tt_scanmac = this.textBox2.Text.Trim().ToUpper();
                    string tt_shortmac = tt_scanmac.Replace("-", "");


                    //第一步位数判断
                    Boolean tt_flag1 = false;
                    tt_flag1 = CheckStrLengh(tt_hostlable, this.textBox15.Text);


                    //第二步包含符判断
                    Boolean tt_flag2 = false;
                    if (tt_flag1)
                    {
                        tt_flag2 = CheckStrContain(tt_hostlable, this.textBox14.Text.Trim());
                    }

                    //第三步判断产品关联生产序列号是否与扫描一致
                    Boolean tt_flag3 = false;
                    string tt_hostlable_1 = "";
                    if (tt_flag1 && tt_flag2)
                    {
                        string tt_sql3 = "select hostlable from odc_alllable where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "'";

                        DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
                        if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                        {
                            tt_hostlable_1 = ds3.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper(); //主机条码

                            if (tt_hostlable_1 == this.textBox19.Text.Trim())
                            {
                                tt_flag3 = true;
                                setRichtexBox("3、关联表查询到一条数据，生产序列号与扫描一致，数据记录为" + tt_hostlable_1 + ",扫描为" + tt_hostlable + ",goon");
                            }
                            else
                            {
                                setRichtexBox("3、生产序列号与扫描不一致，over");
                                PutLableInfor("数据记录为" + tt_hostlable_1 + ",扫描为" + tt_hostlable + ",请检查!");
                            }

                        }
                        else
                        {
                            setRichtexBox("3、关联表没有查询到数据，over");
                            PutLableInfor("关联表没有查询到数据，请检查！");
                        }

                    }

                    //最后判断
                    if (tt_flag1 && tt_flag2 && tt_flag3)
                    {
                        this.richTextBox1.BackColor = Color.Aquamarine;
                        PutLableInfor("OK 生产序列号扫描成功，请继续扫描电源");
                        this.textBox2.Enabled = false;
                        this.textBox19.Enabled = false;
                        this.textBox3.Enabled = true;
                        textBox3.Focus();
                        textBox3.SelectAll();
                    }
                    else
                    {
                        this.richTextBox1.BackColor = Color.Red;
                        textBox19.Focus();
                        textBox19.SelectAll();
                    }
                }
            }
        }

        //彩盒过站,扫描电源
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始电源扫描
                setRichtexBox("-----开始电源扫描--------");
                textBox3.Enabled = false;
                string tt_scanshell = this.textBox3.Text.Trim().ToUpper();
                string tt_dyscanshell = tt_scanshell.Substring(0, 7);
                string tt_task = this.textBox1.Text.Trim().ToUpper();
                string tt_scanmac = this.textBox2.Text.Trim().ToUpper();
                string tt_shortmac = tt_scanmac.Replace("-", "");
                string tt_dy = this.textBox8.Text.Trim().ToUpper(); 


                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanshell, this.textBox6.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_dy, tt_dyscanshell);
                }

                //第三步判断电源是否用过
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    string tt_sql3 = "select maclable from odc_alllable where taskscode = '" + tt_task + "' and dystlable = '" + tt_scanshell + "'";

                    DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);

                    if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                    {
                        string tt_dy_maclable = ds3.Tables[0].Rows[0].ItemArray[0].ToString();  //电源查出的MAC

                        if (tt_dy_maclable == tt_shortmac)
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

                //第四步记录数检查
                Boolean tt_flag4 = false;
                string tt_id = "";
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    string tt_sql4 = "select count(1),min(boxlable),min(id) from odc_alllable " +
                                     "where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "' and boxlable is not null";
                    string[] tt_array4 = new string[3];
                    tt_array4 = Dataset1.GetDatasetArray(tt_sql4, tt_conn);
                    if (tt_array4[0] == "1")
                    {
                        tt_flag4 = true;
                        tt_id = tt_array4[2];
                        setRichtexBox("4、有一条可更新的记录,串码：" + tt_array4[1] + ",ID号：" + tt_array4[2] + ",goon");
                    }
                    else
                    {
                        setRichtexBox("4、没有彩盒21可以更新可更新的记录，over");
                        PutLableInfor("扫描的MAC还没有获取彩盒21，请重新扫描");
                    }
                }

                //第五步物料追溯信息
                Boolean tt_flag5 = false;
                string tt_mate1 = this.textBox11.Text.Trim();  //说明书
                string tt_mate2 = this.textBox12.Text.Trim();  //网线
                string tt_mate3 = this.textBox13.Text.Trim();  //电话线
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    Boolean tt_idinfo = GetMaterialIdinfor(tt_id);
                    if (tt_idinfo)
                    {
                        string tt_insert = "insert into odc_traceback(fid,fchdate,Fsegment11,Fsegment12,Fsegment13) " +
                        "values(" + tt_id + ",getdate(),'" + tt_mate1 + "','" + tt_mate2 + "','" + tt_mate3 + "' )";

                        int tt_int1 = Dataset1.ExecCommand(tt_insert, tt_conn);

                        if (tt_int1 > 0)
                        {
                            tt_flag5 = true;
                            setRichtexBox("5、物料追溯已成功追加到物料表odc_traceback，id号：" + tt_id + ",goon");
                        }
                        else
                        {
                            setRichtexBox("5、物料追溯没有成功追加物料表！,over");
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
                            tt_flag5 = true;
                            setRichtexBox("5、物料追溯已成功更新到物料表odc_traceback，id号：" + tt_id + ",goon");
                        }
                        else
                        {
                            setRichtexBox("5、物料追溯没有成功更新到物料表！,over");
                            PutLableInfor("物料追溯没有成功更新到物料表!请继续扫描");
                        }

                    }

                }                

                //第六步更新电源
                Boolean tt_flag6 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                {
                    string tt_update6 = "update odc_alllable set dystlable = '" + tt_scanshell + "' " +
                                        "where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "'";

                    int tt_execute6 = Dataset1.ExecCommand(tt_update6, tt_conn);
                    if (tt_execute6 > 0)
                    {
                        tt_flag6 = true;
                        setRichtexBox("6、电源更新成功 ,goon");
                    }
                    else
                    {
                        setRichtexBox("6、电源更新不成功，请重新扫描，over");
                        PutLableInfor("电源更新不成功，请重新扫描");
                    }

                }
                
                //第七步 获取信息
                Boolean tt_flag7 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {
                    string tt_sql7 = "select pcbasn,hostlable,maclable,smtaskscode,boxlable,dystlable,bprintuser,shelllable " +
                                     "from odc_alllable " +
                                     "where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";

                    DataSet ds7 = Dataset1.GetDataSet(tt_sql7, tt_conn);
                    if (ds7.Tables.Count > 0 && ds7.Tables[0].Rows.Count > 0)
                    {
                        tt_flag7 = true;
                        this.label44.Text = ds7.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();   //单板号
                        this.label45.Text = ds7.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();   //主机条码
                        this.label46.Text = ds7.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();   //MAC
                        this.label47.Text = ds7.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();   //设备标识条码
                        this.label48.Text = ds7.Tables[0].Rows[0].ItemArray[4].ToString().ToUpper();   //流水21条码
                        this.label49.Text = ds7.Tables[0].Rows[0].ItemArray[5].ToString();   //电源条码
                        this.label59.Text = ds7.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper();   //长MAC
                        this.label61.Text = ds7.Tables[0].Rows[0].ItemArray[7].ToString().ToUpper();   //GPSN

                        this.label98.Text = Regex.Replace(this.label47.Text, " ", "");

                        if (this.label87.Text == "EPON")
                        {
                            tt_ounmac = Regex.Replace(this.label61.Text, "-", "");
                        }
                        else
                        {
                            tt_ounmac = this.label61.Text;
                        }
                        setRichtexBox("7、查询到关联表的数据，已关联到电源的,goon");
                    }
                    else
                    {
                        setRichtexBox("7、关联表没有查询到数据，over");
                        PutLableInfor("关联表没有查询到数据，请检查！");
                    }
                }

                //第八步开始过站
                Boolean tt_flag8 = false;
                string tt_gyid = this.label71.Text;
                string tt_ccode = this.label73.Text;
                string tt_ncode = this.label74.Text;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 )
                {
                    tt_flag8 = Dataset1.FhYDCHPassStation(tt_task, STR, tt_shortmac, tt_gyid, tt_ccode, tt_ncode, tt_conn);

                    if (tt_flag8)
                    {
                        setRichtexBox("8、产品过站成功");
                    }
                    else
                    {
                        setRichtexBox("8、产品过站不成功，事务已回滚");
                        PutLableInfor("产品过站不成功，请检查或再次扫描！");
                    }
                }

                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8)
                {
                    //打印记录
                    Dataset1.lablePrintRecord(tt_task, tt_shortmac, this.label45.Text, "彩盒标签", str, tt_computermac, "", tt_conn);

                    GetParaDataPrint1(1);
                    if (PrintChange == "1")
                    {
                        GetParaDataPrint2(1);//双打功能
                    }
                    GetProductYield();
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    getProductRhythm();
                    PutLableInfor("OK 电源关联成功，请扫描MAC！");
                    Dataset1.AddLog("彩盒标签日志", tt_task, tt_shortmac, this.richTextBox1.Text, "侧边栏显示LOG");
                    this.textBox2.Enabled = true;
                    this.textBox19.Enabled = false;
                    this.textBox3.Enabled = false;
                    textBox2.Focus();
                    textBox2.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox3.Enabled = true;
                    textBox3.Focus();
                    textBox3.SelectAll();
                }
            }
        }
        #endregion


        #region 7、标签打印

        #region 彩盒标签打印

        //获取彩盒标签参数
        private void GetParaDataPrint1(int tt_itemtype)
        {
            string tt_fdata1 = this.label17.Text;

            //CH01---数据类型一 烽火移动彩盒
            if (tt_fdata1 == "CH01")
            {
                GetParaDataPrint1_CH01(tt_itemtype);
            }

        }

        //----以下是CH01数据采集----烽火wifi & 烽火移动
        private void GetParaDataPrint1_CH01(int tt_itemtype)
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
            row1["名称"] = "设备型号";
            row1["内容"] = this.label13.Text;
            dt1.Rows.Add(row1);

            DataRow row2 = dt1.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "物料编码";
            row2["内容"] = this.label51.Text;
            dt1.Rows.Add(row2);

            DataRow row3 = dt1.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "软件版本";
            row3["内容"] = this.label84.Text;
            dt1.Rows.Add(row3);

            DataRow row4 = dt1.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "出厂日期";
            row4["内容"] = this.label15.Text;
            dt1.Rows.Add(row4);

            DataRow row5 = dt1.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "长MAC";
            row5["内容"] = this.label59.Text;
            dt1.Rows.Add(row5);

            DataRow row6 = dt1.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "短MAC";
            row6["内容"] = this.label46.Text;
            dt1.Rows.Add(row6);

            DataRow row7 = dt1.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "设备标识";
            row7["内容"] = this.label47.Text;
            dt1.Rows.Add(row7);

            DataRow row8 = dt1.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "GPONSN";
            row8["内容"] = this.label61.Text;
            dt1.Rows.Add(row8);

            DataRow row9 = dt1.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "序列号";
            row9["内容"] = this.label45.Text;
            dt1.Rows.Add(row9);

            DataRow row10 = dt1.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "PON类型";
            row10["内容"] = this.label87.Text;
            dt1.Rows.Add(row10);

            DataRow row11 = dt1.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "文字变量";
            row11["内容"] = this.label91.Text;
            dt1.Rows.Add(row11);

            DataRow row12 = dt1.NewRow();
            row12["参数"] = "S12";
            row12["名称"] = "产品颜色";
            row12["内容"] = this.label88.Text;
            dt1.Rows.Add(row12);

            DataRow row13 = dt1.NewRow();
            row13["参数"] = "S13";
            row13["名称"] = "产品特征";
            row13["内容"] = this.label94.Text;
            dt1.Rows.Add(row13);

            DataRow row14 = dt1.NewRow();
            row14["参数"] = "S14";
            row14["名称"] = "设备标示码暗码";
            row14["内容"] = this.label98.Text;
            dt1.Rows.Add(row14);

            DataRow row15 = dt1.NewRow();
            row15["参数"] = "S15";
            row15["名称"] = "GPSN暗码";
            row15["内容"] = tt_ounmac;
            dt1.Rows.Add(row15);

            //第二步加载到表格显示
            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst1.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 60;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 200;

            //第三步 打印或预览

            if (dst1.Tables.Count > 0 && dst1.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path1);
                report.SetParameterValue("S01", dst1.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("S02", dst1.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("S03", dst1.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("S04", dst1.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("S05", dst1.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("S06", dst1.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("S07", dst1.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("S08", dst1.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("S09", dst1.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("S10", dst1.Tables[0].Rows[9][2].ToString());
                report.SetParameterValue("S11", dst1.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("S12", dst1.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("S13", dst1.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("S14", dst1.Tables[0].Rows[13][2].ToString());
                report.SetParameterValue("S15", dst1.Tables[0].Rows[14][2].ToString());

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
                if (tt_itemtype == 1)
                {
                    if (PrintChange == "1")
                    {
                        report.PrintSettings.Printer = this.textBox29.Text;//双打功能
                    }
                    report.Print();
                    report.Save(tt_path1);
                    tt_top1 = 0;
                    tt_left1 = 0;
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
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印！");
            }
        }

        #endregion

        #region II型标签打印

        //获取II型标签参数
        private void GetParaDataPrint2(int tt_itemtype)
        {
            string tt_fdata2 = this.label105.Text;

            //YX01---数据类型一
            if (tt_fdata2 == "YX01")
            {
                GetParaDataPrint2_YX01(tt_itemtype);
            }
        }

        //----以下是YX01数据采集----
        private void GetParaDataPrint2_YX01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst2 = new DataSet();
            DataTable dt2 = new DataTable();
            dst2.Tables.Add(dt2);
            dt2.Columns.Add("参数");
            dt2.Columns.Add("名称");
            dt2.Columns.Add("内容");

            DataRow row1 = dt2.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "序列号";
            row1["内容"] = this.label48.Text;
            dt2.Rows.Add(row1);

            this.dataGridView6.DataSource = null;
            this.dataGridView6.Rows.Clear();

            this.dataGridView6.DataSource = dst2.Tables[0];
            this.dataGridView6.Update();

            this.dataGridView6.Columns[0].Width = 60;
            this.dataGridView6.Columns[1].Width = 80;
            this.dataGridView6.Columns[2].Width = 200;


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
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1 && this.textBox29.Text != "")
                {
                    Thread.Sleep(int.Parse(IItype_PrintDelay));
                    report.PrintSettings.Printer = this.textBox30.Text;
                    report.Print();
                    report.Save(tt_path2);
                    tt_top2 = 0;
                    tt_left2 = 0;
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

        #endregion

        #endregion
    }
}
