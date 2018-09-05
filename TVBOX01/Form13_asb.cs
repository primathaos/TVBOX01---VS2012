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
    public partial class Form13_asb : Form
    {
        public Form13_asb()
        {
            InitializeComponent();
            this.label55.Text = tt_uplip.ToString();
            this.label56.Text = tt_downlip.ToString();
        }

        #region 1、属性设置
        static string tt_conn;
        static string tt_code = "0000";
        static string tt_path1 = "";
        static string tt_path2 = "";
        //static string tt_md5 = "";
        string tt_productname = "";
        string tt_telecustomer = "";
        string tt_setusername = "";
        string tt_setpasswordlen = "";
        string tt_setpasswordAa = "";
        int tt_yield = 0;  //产量
        int tt_uplip = 0;  //上盖数量
        int tt_downlip = 0; //下盖数量
        static int tt_reprinttime = 0; //重打次数
        //标签微调
        static float tt_top1 = 0; //铭牌上下偏移量
        static float tt_left1 = 0; //铭牌左右偏移量
        static float tt_top2 = 0; //运营商上下偏移量
        static float tt_left2 = 0; //运营商左右偏移量
        //读取的打印设置
        static string PlatePrintPattern = "";
        //重打限制标识
        string tt_reprintmark = "1";
        //重打限数
        int tt_reprintchang1 = 0;
        int tt_reprintchang2 = 0;
        //重打计时
        DateTime tt_reprintstattime;
        DateTime tt_reprintendtime;

        DateTime tt_productstarttime = DateTime.Now; //开始时间
        DateTime tt_productprimtime; //上一次时间

        //其它参数

        //打印铭牌时，电源选择1.5A显示标识（正常HG6201M产品为1.0A）
        string tt_power_old = "";
        //1.5A电源物料不足问题重打标识 //同时也是小型化标识
        string tt_parenttask = "";
        //四川地区时间参数
        string tt_SichuanTime = "";
        string tt_Sichuanlongmac = "";
        //本机MAC
        static string tt_computermac = "";
        //小型化批量打印中断标识
        int Prints_Stop = 0;

        private void Form13_asb_Load(object sender, EventArgs e)
        {
            //FastReport环境变量设置（打印时不提示 "正在准备../正在打印..",一个程序只需设定一次，故一般写在程序入口）
            (new FastReport.EnvironmentSettings()).ReportSettings.ShowProgress = false;

            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";

            this.toolStripStatusLabel6.Text = tt_productstarttime.ToString();

            //初始不显示身份验证栏
            this.groupBox14.Visible = false;

            //初始不显示微调栏
            this.groupBox15.Visible = false;            

            //隐藏线长调试按钮
            this.LineManger.Visible = false;
            this.tabPage10.Parent = null;

            //通常不显示批量打印栏位
            this.tabPage11.Parent = null;

            //员工账号分离
            if (str.Contains("FH011"))
            {
                this.Plate_View.Visible = false;
                this.Plate_Print.Visible = false;
                this.Operator_View.Visible = false;
                this.Operator_Print.Visible = false;
                this.tabPage4.Parent = null;
                this.LineManger.Visible = true;
            }

            //烽火小型化铭牌批量打印员工账号
            if (str.Contains("FH012"))
            {
                this.Plate_View.Visible = false;
                this.Plate_Print.Visible = false;
                this.Operator_View.Visible = false;
                this.Operator_Print.Visible = false;
                this.tabPage3.Parent = null;
                this.tabPage4.Parent = null;
                this.tabPage11.Parent = tabControl2;
                this.LineManger.Visible = true;
                this.label120.Text = "";
                this.tabPage10.Text = "铭牌";
                this.label73.Text = "";
                this.label118.Text = "打印参数";
                this.label1.Text = "已打印铭牌数量";
                this.label46.Text = "";
                this.label45.Text = "";

            }

            //烽火小型化铭牌批量打印工程账号
            if (str.Contains("FH112"))
            {
                this.Plate_View.Visible = false;
                this.Plate_Print.Visible = false;
                this.Operator_Print.Visible = false;
                this.tabPage3.Parent = null;
                this.tabPage4.Parent = null;
                this.groupBox15.Visible = true;
                this.tabPage9.Parent = null;
                this.tabPage10.Parent = tabControl5;
                this.tabPage11.Parent = tabControl2;
                this.label120.Text = "";
                this.tabPage10.Text = "铭牌";
                this.label73.Text = "";
                this.label118.Text = "打印参数";
                this.label1.Text = "已打印铭牌数量";
                this.label46.Text = "";
                this.label45.Text = "";
            }

            ClearLabelInfo();
            //生产节拍
            this.label7.Text = tt_yield.ToString();
            this.label8.Text = null;
            this.label9.Text = null;
            this.label10.Text = null;

            //生产信息
            this.label46.Text = null;
            this.label47.Text = null;

            this.textBox2.Visible = false;
            this.textBox3.Visible = false;

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


        //信息清除
        private void ClearLabelInfo()
        {
            //清除工单信息
            this.label27.Text = null;
            this.label28.Text = null;
            this.label29.Text = null;
            this.label30.Text = null;
            this.label31.Text = null;
            this.label32.Text = null;
            this.label33.Text = null;
            this.label34.Text = null;
            this.label49.Text = null;
            this.label61.Text = null;
            this.label67.Text = null;
            this.label69.Text = null;
            this.label87.Text = null;
            this.label88.Text = null;
            this.label90.Text = null;
            this.label92.Text = null;
            this.label97.Text = null;
            this.label121.Text = null;
            this.label124.Text = null;
            this.textBox6.Text = null;

            //流程信息
            this.label76.Text = null;
            this.label77.Text = null;
            this.label79.Text = null;
            this.label85.Text = null;
            this.label65.Text = null;
            this.label66.Text = null;


            //提示信息
            this.label12.Text = null;


            //生产信息
            this.label46.Text = null;
            this.label47.Text = null;

            //条码信息
            this.label39.Text = null;
            this.label40.Text = null;
            this.label41.Text = null;
            this.label42.Text = null;
            this.label44.Text = null;
            this.label80.Text = null;
            this.label71.Text = null;
            this.label95.Text = null;
            this.label108.Text = null;
            this.label112.Text = null;
            this.label111.Text = null;
            this.label110.Text = null;
            this.label106.Text = null;
            this.label104.Text = null;
            this.label101.Text = null;

        }


        //扫描前数据初始化
        private void ScanDataInitial()
        {
            //条码信息清除
            this.label39.Text = null;
            this.label40.Text = null;
            this.label41.Text = null;
            this.label42.Text = null;
            this.label44.Text = null;
            this.label80.Text = null;
            this.label71.Text = null;
            this.label95.Text = null;
            this.label108.Text = null;
            this.label112.Text = null;
            this.label111.Text = null;
            this.label110.Text = null;
            this.label106.Text = null;
            this.label104.Text = null;
            this.label101.Text = null;
            this.label125.Text = null;

            //提示信息
            this.label12.Text = null;

            //流程信息清除
            this.label85.Text = null;
            
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
        //单板号扫描位数锁定
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
                this.textBox7.Enabled = false;
            }
            else
            {
                this.textBox7.Enabled = true;
            }
        }

        //工单锁定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if( this.checkBox1.Checked)
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
                    if (line.Contains("PlatePrintPattern"))
                    {
                        PlatePrintPattern = line.Substring(line.IndexOf("=") + 1).Trim();
                    }
                }

                if (PlatePrintPattern == "1")
                {
                    this.tabPage10.Parent = tabControl5;
                }

                if (str.Contains("FH111"))
                {
                    this.Plate_Print.Visible = true;
                    this.Operator_Print.Visible = true;
                    this.tabPage4.Parent = tabControl2;
                    //获取调试开始时间
                    tt_reprintstattime = DateTime.Now;
                }

                tt_computermac = Dataset1.GetHostIpName();

                string tt_sql1 = "select  tasksquantity,product_name,areacode,fec,convert(varchar, taskdate, 102) fdate," +
                                 "customer,flhratio,Gyid,Tasktype,Vendorid,Teamgroupid,pon_name,id,fhcode,parenttask " +
                                 "from odc_tasks where taskscode = '" + this.textBox1.Text + "' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1,tt_conn);
                                
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {

                    this.label27.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    tt_productname = ds1.Tables[0].Rows[0].ItemArray[1].ToString(); //产品名称
                    this.label30.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区
                    this.label31.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString(); //EC编码
                    this.label28.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //生产日期

                    this.label79.Text = ds1.Tables[0].Rows[0].ItemArray[7].ToString();  //流程配置
                    this.label49.Text = ds1.Tables[0].Rows[0].ItemArray[8].ToString();  //物料编码
                    this.label61.Text = ds1.Tables[0].Rows[0].ItemArray[9].ToString();  //COMMID
                    this.label67.Text = ds1.Tables[0].Rows[0].ItemArray[10].ToString();  //地区代码
                    this.label69.Text = ds1.Tables[0].Rows[0].ItemArray[11].ToString();  //PON类型

                    string tt_idnum = ds1.Tables[0].Rows[0].ItemArray[12].ToString();//制造单ID

                    tt_power_old = ds1.Tables[0].Rows[0].ItemArray[13].ToString().Trim();//旧电源适配器标识
                    tt_parenttask = ds1.Tables[0].Rows[0].ItemArray[14].ToString().Trim();//旧电源适配器标识(重打时检查) //同时也是小型化产品标识参数

                    tt_SichuanTime = (this.label28.Text.Replace(".", "/")).Substring(2, 5); //四川电信铭牌时间参数

                    int tt_idnum1 = Convert.ToInt32(tt_idnum);

                    if (tt_productname == "HG6201G" || tt_productname == "HG6201GW" || tt_productname == "HG6201GS") //广电产品型号转换
                    {
                        this.label29.Text = "HG6201M";
                    }
                    else
                    {
                        this.label29.Text = tt_productname;
                    }

                    if (this.label30.Text == "河南") //移动地区定制化区分
                    {
                        this.label124.Text = "10086-5";
                        this.label121.Text = "设备二维码信息";
                    }
                    else if (this.label30.Text == "江西")
                    {
                        this.label124.Text = "10086";
                        this.label121.Text = "零配置";
                    }
                    else
                    {
                        this.label124.Text = "10086";
                        this.label121.Text = "";
                    }

                    //锁死MAC特征码输入框
                    this.textBox6.Enabled = false;
                    
                    //第一步 流程检查
                    Boolean tt_flag1 = false;
                    if (!this.label79.Text.Equals(""))
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
                        if (this.label69.Text == "GPON") //文字变量
                        {
                            if (tt_idnum1 >= 915 && tt_productname == "HG6201T") 
                            {
                                this.label87.Text = "S/N";
                            }
                            else
                            {
                                this.label87.Text = "GPON SN";
                            }
                            this.label97.Text = "MAC";   //增加文字变量03，2017/10/6 聂江斌
                            this.label88.Text = "吉比特";
                            tt_flag2 = true;
                        }
                        else if (this.label69.Text == "EPON")
                        {
                            this.label87.Text = "ONU MAC";
                            this.label97.Text = "WAN MAC";   //增加文字变量03，2017/10/6 聂江斌
                            this.label88.Text = "以太网";
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("文字变量无法匹配，请确认制造单下单信息，或产品是否为PON产品");
                        }
                    }

                    //第三步 电源规格查询
                    Boolean tt_flag3 = false;
                    if (tt_flag2)
                    {
                        string tt_change = "not like '小型化%'";
                        if (tt_parenttask.Contains("小型化")) //如果小型化产品
                        {
                            tt_change = "= '" + tt_parenttask + "'";
                        }
                        string tt_sql3 = "select volt,ampere from odc_dypowertype where ftype = '" + tt_productname + "' and fdesc " + tt_change;

                        DataSet ds3 = Dataset1.GetDataSetTwo(tt_sql3, tt_conn);

                        if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                        {
                            this.label90.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString(); //电压
                            this.label92.Text = ds3.Tables[0].Rows[0].ItemArray[1].ToString(); //电流
                            if (tt_productname == "HG6201M" && tt_power_old == "1.5") //旧电源兼容
                            {
                                this.label92.Text = "1.5A";
                            }

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
                            this.textBox6.Text = tt_maccheck.Substring(0, 6); //获取该制造单MAC特征码

                            tt_flag4 = true;
                        }
                        else
                        {
                            string tt_sql4_1 = "select top(1)maclable from odc_alllable where hprintman = '" + this.textBox1.Text + "' ";
                            DataSet ds4_1 = Dataset1.GetDataSetTwo(tt_sql4_1, tt_conn);

                            if (ds4_1.Tables.Count > 0 && ds4_1.Tables[0].Rows.Count > 0)
                            {
                                tt_maccheck = ds4_1.Tables[0].Rows[0].ItemArray[0].ToString(); //随机取该制造单关联的一个MAC
                                this.textBox6.Text = tt_maccheck.Substring(0, 6); //获取该制造单MAC特征码

                                tt_flag4 = true;
                            }
                            else
                            {
                                MessageBox.Show("没有MAC相关信息，请确认该制造单是否有关联产品");
                            }
                        }
                    }

                    //第五步 EC表信息查询
                    Boolean tt_flag5 = false;
                    if (tt_flag4)
                    {
                        string tt_sql5 = "select  docdesc,Fpath01,Fdata01,Fmd01,Fpath02,Fdata02,Fmd02   from odc_ec where zjbm = '" + this.label31.Text + "' ";

                        DataSet ds5 = Dataset1.GetDataSet(tt_sql5, tt_conn);
                        if (ds5.Tables.Count > 0 && ds5.Tables[0].Rows.Count > 0)
                        {
                            this.label34.Text = ds5.Tables[0].Rows[0].ItemArray[0].ToString(); //EC描述
                            this.label33.Text = ds5.Tables[0].Rows[0].ItemArray[1].ToString(); //铭牌模板路径
                            this.label32.Text = ds5.Tables[0].Rows[0].ItemArray[2].ToString(); //铭牌数据类型
                            this.label58.Text = ds5.Tables[0].Rows[0].ItemArray[4].ToString(); //运营商模板路径
                            this.label113.Text = ds5.Tables[0].Rows[0].ItemArray[5].ToString(); //运营商数据类型
                            //this.label59.Text = ds5.Tables[0].Rows[0].ItemArray[3].ToString(); //MD5码
                            tt_path1 = Application.StartupPath + ds5.Tables[0].Rows[0].ItemArray[1].ToString();
                            tt_path2 = Application.StartupPath + ds5.Tables[0].Rows[0].ItemArray[4].ToString();
                            //tt_md5 = ds5.Tables[0].Rows[0].ItemArray[3].ToString();
                            tt_flag5 = true;
                        }
                        else
                        {
                            MessageBox.Show("没有找到工单表的EC表配置信息，请确认！");
                        }
                    }

                    //第六步 运营商检查
                    Boolean tt_flag6 = false;
                    if (tt_flag5)
                    {
                        string tt_product = this.label29.Text;
                        tt_telecustomer = GetTelecomOperator(tt_product, tt_parenttask);
                        if (tt_telecustomer == "0")
                        {
                            MessageBox.Show("运营商获取失败，无法确定是电信还是移动产品");
                        }
                        else
                        {
                            tt_flag6 = true;                           
                        }
                    }

                    //第七步 物料编码检查
                    Boolean tt_flag7 = false;
                    if (tt_flag6)
                    {
                        if (this.label49.Text != "")
                        {
                            string tt_tasktype = SetMetrialCheck(this.label30.Text, this.label29.Text, tt_telecustomer, this.label49.Text);
                            if (tt_tasktype == this.label49.Text)
                            {
                                tt_flag7 = true;
                            }
                            else
                            {
                                MessageBox.Show("该工单物料编码:" + this.label49.Text + ",与设定物料编码:" + tt_tasktype + ",不一致，请确认");
                            }
                        }
                        else
                        {
                            MessageBox.Show("该工单物料编码为空，请检查工单设置！");
                        }
                    }

                    //第八步 获取用户名密码设定
                    Boolean tt_flag8 = false;
                    if (tt_flag7)
                    {
                        string tt_sql8 = "select username,digits,format from odc_fhuser " +
                                         "where aear = '" + this.label30.Text + "' and  operator = '" + tt_telecustomer + "' ";
                        DataSet ds8 = Dataset1.GetDataSetTwo(tt_sql8, tt_conn);
                        if (ds8.Tables.Count > 0 && ds8.Tables[0].Rows.Count > 0)
                        {
                            tt_setusername = ds8.Tables[0].Rows[0].ItemArray[0].ToString(); //用户名
                            tt_setpasswordlen = ds8.Tables[0].Rows[0].ItemArray[1].ToString(); //密码位数
                            tt_setpasswordAa = ds8.Tables[0].Rows[0].ItemArray[2].ToString();  //密码大小写

                            if (tt_setusername == "" || tt_setpasswordlen =="" || tt_setpasswordAa == "")
                            {
                                MessageBox.Show("用户名，或密码设定值为空，请检查数据");
                            }
                            else
                            {
                                tt_flag8 = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("没有找到地区:" + this.label30.Text + "，的用户名及密码设定，请确认！");
                        }
                    }

                    //第九步 查看模板是否存在
                    Boolean tt_flag9 = false;
                    if( tt_flag8)
                    {
                        tt_flag9 = (getPathIstrue(tt_path1) && getPathIstrue(tt_path2));

                        if (!getPathIstrue(tt_path1))
                        {
                            MessageBox.Show(" 找不到模板文件：" + tt_path1 + "，请确认！");
                        }

                        if (!getPathIstrue(tt_path2))
                        {
                            MessageBox.Show(" 找不到模板文件：" + tt_path2 + "，请确认！");
                        }
                    }


                    //第七步 检验MD5码    2017/10/6  取消MD5码校验，聂江斌
                    //Boolean tt_flag7 = false;
                    //if (tt_flag6 )
                    //{
                    //    string tt_md6 = GetMD5HashFromFile(tt_path);

                    //    if (tt_md5 == tt_md6)
                    //    {
                    //        tt_flag7 = true;
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("系统设定模板MD5码: '"+tt_md5+"'与你使用模板的MD5码：'"+tt_md6+"'不一致，请确认！");
                    //    }
                    //}

                    //最后验证
                    if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9)
                    {
                        this.textBox1.Enabled = false;
                        this.textBox2.Visible = true;
                        this.textBox3.Visible = true;
                        if (!str.Contains("FH012") && !str.Contains("FH112")) //小型化批量打印不检查
                        {
                            GetProductNumInfo();  //生产信息  
                        }
                    }
                    else
                    {
                        this.checkBox1.Checked = false;
                    }

                }
                else
                {
                    MessageBox.Show("没有查询此工单，请确认！");
                    this.checkBox1.Checked = false;
                }
            }
            else
            {
                this.textBox1.Enabled = true;
                this.textBox2.Visible = false;
                this.textBox3.Visible = false;
                this.checkBox1.Checked = false;
                this.comboBox2.Text = "";
                this.textBox21.Text = "";
                this.textBox22.Text = "";
                this.comboBox2.Enabled = true;
                this.textBox21.Enabled = true;
                this.textBox22.Enabled = true;
                this.groupBox14.Visible = false;
                this.groupBox15.Visible = false;
                this.groupBox12.Visible = true;
                this.groupBox5.Visible = true;
                this.dataGridView1.Visible = true;
                this.Plate_Print.Visible = false;
                this.tabPage4.Parent = null;
                this.tabPage3.Parent = tabControl2;
                ClearLabelInfo();
                ScanDataInitial();
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
            this.label12.Text = tt_lableinfo;
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
            this.label7.Text = tt_yield.ToString();   //本班产量
            this.label8.Text = tt_time;               //生产时间
            this.label9.Text = tt_avgtime.ToString();  //平均节拍
            this.label10.Text = tt_differtime2;        //实时节拍

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
                setRichtexBox("1、位数判断不正确，不是" + tt_snlength.ToString() + "位,实际为：" + tt_checkstr.Length.ToString() );
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
        private bool GetStrChar(string tt_longstr, string tt_chartype)
        {
            Boolean tt_flag = false;

            String tt_chars = "";

            for (int i = 0; i < tt_longstr.Length; i++)
            {
                tt_chars = tt_longstr.Substring(i, 1);
                tt_flag = GetCharsCheck(tt_chars, tt_chartype);
                if (!tt_flag) break;


            }

            return tt_flag;
        }


        //字符大小判断
        private bool GetCharsCheck(string tt_char, string chartype)
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


        #region 5、数据辅助功能

        //获取生产信息
        private void GetProductNumInfo()
        {
            string tt_sql = "select  count(1),count(case when mprintman is not null then 1 end),0 " +
                            "from odc_alllable  where taskscode = '" + this.textBox1.Text + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label46.Text = tt_array[0];
            this.label47.Text = tt_array[1];
        }

        //刷新站位
        private void CheckStation(string tt_mac)
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

                    this.label13.Text = "站位:" + tt_code;
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

        //获取工单号(重打站位变更用)
        private string Gettasks(string tt_maclable)
        {
            string tt_tasks = "";

            string tt_sql = "select count(1), min(taskscode), min(maclable) " +
                            "from odc_alllable where maclable = '" + tt_maclable + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArrayTwo(tt_sql, tt_conn);

            if (tt_array[0] == "1")
            {
                tt_tasks = tt_array[1];
            }
            else
            {
                MessageBox.Show("网络连接失败，或此MAC" + tt_maclable + "未关联，请确认");
            }

            return tt_tasks;
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

        //获取运营商
        private string GetTelecomOperator(string tt_peoductname, string tt_parenttask)
        {
            string tt_teleplan = "0";

            if (tt_parenttask.Contains("小型化")) //如果小型化产品
            {
                tt_teleplan = tt_parenttask;
            }
            else
            {
                string tt_sql = "select count(1),min(Fdesc),0 from odc_dypowertype where Ftype = '" + tt_peoductname + "' and Fdesc not like '小型化%'";
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
            }

            return tt_teleplan;
        }

        //删除条码比对数据
        private int Delete_Check(string tt_mac)
        {
            string tt_deletesql = "delete from odc_check_barcode where maclable = '" + tt_mac + "'";
            int tt_Checknum = Dataset1.ExecCommand(tt_deletesql, tt_conn);
            return tt_Checknum;
        }
        #endregion


        #region 6、SN条码查询
        //SN条码查询 确定
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

        //SN条码查询 重置
        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox11.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;

        }
        #endregion


        #region 7、获取文件MD5码
        // 文件名获取
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
                this.textBox12.Text = file;
            }
        }

        //获取MD5码
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


        #region 8、扫描事件

        //扫描MAC重打
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描--------");
                string tt_task = this.textBox1.Text.Trim().ToUpper();
                string tt_scanmac = this.textBox3.Text.Trim().ToUpper();
                string tt_shortmac = tt_scanmac.Replace(":", "");

                if (tt_shortmac.Contains("FHTT"))
                {
                    tt_shortmac = this.textBox3.Text.Trim() + tt_shortmac.Substring((tt_shortmac.Length - 6), 6);
                }

                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox7.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1 && this.textBox6.Enabled == false)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox6.Text.Trim());
                }
                else
                {
                    setRichtexBox("2、MAC条码约束字段未锁死,：" + this.textBox6.Text + ",over");
                    PutLableInfor("MAC条码约束字段未锁死，请输入约束字段后回车！");
                }


                //第三步 判断路径
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    tt_flag3 = getPathIstrue(tt_path1);
                    if (tt_flag3)
                    {
                        setRichtexBox("3、已找到一个铭牌模板,：" + tt_path1 + ",goon");
                    }
                    else
                    {
                        setRichtexBox("3、没有找到铭牌模板,：" + tt_path1 + ",over");
                        PutLableInfor("没有找到铭牌模板，请检查！");
                    }

                }

                //第四步查找信息
                Boolean tt_flag4 = false;
                string tt_longmac = "";
                string tt_gpsn0 = "";
                string tt_ponname = this.label69.Text;
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    string tt_sql3 = "select pcbasn,hostlable,maclable,smtaskscode,bprintuser,shelllable from odc_alllable " +
                                     "where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";


                    DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
                    if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        this.label39.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();
                        this.label40.Text = ds3.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();
                        this.label41.Text = ds3.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();
                        this.label42.Text = ds3.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();
                        this.label44.Text = ds3.Tables[0].Rows[0].ItemArray[4].ToString().ToUpper();
                        tt_gpsn0 = ds3.Tables[0].Rows[0].ItemArray[5].ToString().ToUpper();

                        if (tt_productname == "HG6201T" || tt_productname == "HG6543C1")
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
                            this.label95.Text = "";
                        }

                        if (tt_ponname == "EPON")
                        {
                            this.label71.Text = Regex.Replace(tt_gpsn0, "-", "");
                        }
                        else
                        {
                            this.label71.Text = this.label80.Text;
                        }

                        if ((tt_productname == "HG2201T" || tt_productname == "HG6201T") && this.label30.Text == "四川")
                        {
                            tt_Sichuanlongmac = this.label44.Text.Replace("-", ":");
                        }

                        tt_longmac = this.label44.Text;
                        setRichtexBox("3、关联表查询到一条数据，goon");

                    }
                    else
                    {

                        string tt_sql3_1 = "select pcbasn,hostlable,maclable,smtaskscode,bprintuser,shelllable  from odc_alllable " +
                                     "where taskscode like '" + tt_task + "%' and maclable = '" + tt_shortmac + "' ";


                        DataSet ds3_1 = Dataset1.GetDataSet(tt_sql3_1, tt_conn);
                        if (ds3_1.Tables.Count > 0 && ds3_1.Tables[0].Rows.Count > 0)
                        {
                            tt_flag4 = true;
                            this.label39.Text = ds3_1.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();
                            this.label40.Text = ds3_1.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();
                            this.label41.Text = ds3_1.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();
                            this.label42.Text = ds3_1.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();
                            this.label44.Text = ds3_1.Tables[0].Rows[0].ItemArray[4].ToString().ToUpper();
                            tt_gpsn0 = ds3_1.Tables[0].Rows[0].ItemArray[5].ToString().ToUpper();

                            if (tt_productname == "HG6201T" || tt_productname == "HG6543C1")
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
                                this.label95.Text = "";
                            }

                            if (tt_ponname == "EPON")
                            {
                                this.label71.Text = Regex.Replace(tt_gpsn0, "-", "");
                            }
                            else
                            {
                                this.label71.Text = this.label80.Text;
                            }                        

                            if ((tt_productname == "HG2201T" || tt_productname == "HG6201T") && this.label30.Text == "四川")
                            {
                                tt_Sichuanlongmac = this.label44.Text.Replace("-", ":");
                            }

                            tt_longmac = this.label44.Text;
                            setRichtexBox("3、关联表查询到一条数据，goon");

                        }
                        else
                        {
                            setRichtexBox("3、关联表没有查询到数据，over");
                            PutLableInfor("关联表没有查询到数据，请检查！");
                        }
                        
                    }

                }

                //第五步查询macinfo表信息
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    tt_flag5 = true;
                    string tt_sql5 = "select ssid,username,password,Wlanpas,ssid_5G,wlanpas_5G,barcode1 from odc_macinfo " +
                                      "where taskscode = '" + tt_task + "' and mac = '" + tt_longmac + "' ";

                    DataSet ds5 = Dataset1.GetDataSet(tt_sql5, tt_conn);
                    if (ds5.Tables.Count > 0 && ds5.Tables[0].Rows.Count > 0)
                    {
                        tt_flag5 = true;
                        this.label108.Text = ds5.Tables[0].Rows[0].ItemArray[0].ToString();  //2G用户名
                        this.label112.Text = ds5.Tables[0].Rows[0].ItemArray[1].ToString();  //用户名
                        this.label111.Text = ds5.Tables[0].Rows[0].ItemArray[2].ToString();  //密码
                        this.label110.Text = ds5.Tables[0].Rows[0].ItemArray[3].ToString();  //2G密码
                        this.label106.Text = ds5.Tables[0].Rows[0].ItemArray[4].ToString();  //5G账号
                        this.label104.Text = ds5.Tables[0].Rows[0].ItemArray[5].ToString();  //5G密码
                        this.label101.Text = ds5.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper();  //设备标示号暗码


                        if (this.label30.Text == "河南")
                        {
                            this.label125.Text = "FIBER|" + this.label29.Text + "|" + this.label101.Text + "|" + tt_shortmac;
                        }
                        else if (this.label30.Text == "浙江" || this.label30.Text == "江西")
                        {
                            this.label125.Text = "厂家:烽火通信科技股份有限公司,型号:" + this.label29.Text + ",SN:" + this.label101.Text +
                                                 ",生产日期:" + this.label28.Text.Replace("/", ".") + ",用户无线默认SSID:" + this.label108.Text +
                                                 ",用户无线默认SSID密码:" + this.label110.Text + ",用户登陆默认账号:" + this.label112.Text +
                                                 ",用户登陆默认密码:" + this.label111.Text + ",设备网卡MAC:" + tt_shortmac;
                        }
                        else
                        {
                            this.label125.Text = "";
                        }

                        setRichtexBox("5、Macinfo表找到一条数据,goon");

                    }
                    else
                    {
                        setRichtexBox("5、Macinfo表没有找到一条数据，over");
                        PutLableInfor("Macinfo表没有找到数据，请检查！");
                    }
                }

                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                {
                    GetParaDataPrint1(0);
                    if (PlatePrintPattern == "1")
                    {
                        GetParaDataPrint2(0);
                    }
                    GetProductNumInfo();
                    CheckStation(tt_shortmac);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    setRichtexBox("6、查询完毕，可以重打标签或修改模板，over");
                    PutLableInfor("MAC查询完毕");
                    textBox3.Focus();
                    textBox3.SelectAll();
                    if (tt_reprintmark == "0")
                    {
                        this.textBox3.Enabled = false;
                    }
                }
                else if (tt_flag2 == false && this.textBox6.Enabled == true)
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox6.Focus();
                    textBox6.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox3.Focus();
                    textBox3.SelectAll();
                }
            }
        }

        //扫描单板过站
        private void tabControl2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始单板扫描--------");
                string tt_scanpcba = this.textBox2.Text.Trim().ToUpper();
                string tt_task = this.textBox1.Text.Trim().ToUpper();
                string tt_uplips = this.textBox8.Text.Trim();
                string tt_downlips = this.textBox9.Text.Trim();
                string tt_tin = this.textBox10.Text.Trim();
                string tt_id = "0";


                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanpcba, this.textBox4.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanpcba, this.textBox5.Text.Trim());
                }


                //第三步 检查模板
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {

                    tt_flag3 = getPathIstrue(tt_path1);
                    if (tt_flag3)
                    {
                        setRichtexBox("3、已找到一个铭牌模板,：" + tt_path1 + ",goon");
                    }
                    else
                    {
                        setRichtexBox("3、没有找到铭牌模板,：" + tt_path1 + ",over");
                        PutLableInfor("没有找到铭牌模板，请检查！");
                    }

                }

                //第四步扣数检查
                Boolean tt_flag4 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    //上盖数量检查
                    if (tt_uplip > 1 && tt_downlip > 1)
                    {
                        tt_flag4 = true;
                        setRichtexBox("4、物料扣数都大于1，goon");
                    }
                    else
                    {
                        setRichtexBox("4、有物料数小于1，请换料,over");
                        PutLableInfor("有物料数小于1，请换料！");
                    }
                }



                //第五步物料检查
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    if (textBox8.Text.Equals("") || textBox9.Text.Equals("") || textBox10.Text.Equals(""))
                    {
                        setRichtexBox("5、物料追溯都有空值，请填写物料,over");
                        PutLableInfor("物料追溯都有空值，请检查！");
                    }
                    else
                    {
                        tt_flag5 = true;
                        setRichtexBox("5、物料追溯都不为空，,goon");
                    }
                }


                //第六步流程检查
                Boolean tt_flag6 = false;
                string tt_gyid = this.label79.Text;
                string tt_ccode = this.label76.Text;
                string tt_ncode = this.label77.Text;
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
                        setRichtexBox("6、该工单已配置流程,待测站位：" + tt_ccode + ",进站站位：" + tt_ncode + ",goon");
                    }
                }

                //第七步查找关联表数据
                Boolean tt_flag7 = false;
                string tt_hostlable = "";
                string tt_shortmac = "";
                string tt_smtaskscode = "";
                string tt_longmac = "";
                string tt_gpsn0 = "";
                string tt_gpsn = "";
                string tt_onumac = "";
                string tt_taskscheck = "";
                //string tt_fsegment2 = "";//小型化EPON信息写入临时措施
                string tt_ponname = this.label69.Text;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {
                    string tt_sql7 = "select hostlable,maclable,smtaskscode,bprintuser,id,shelllable,taskscode,fsegment2 from odc_alllable " +
                                     "where hprintman = '" + tt_task + "' and pcbasn = '" + tt_scanpcba + "' ";

                    DataSet ds7 = Dataset1.GetDataSet(tt_sql7, tt_conn);
                    if (ds7.Tables.Count > 0 && ds7.Tables[0].Rows.Count > 0)
                    {
                        tt_flag7 = true;
                        tt_hostlable = ds7.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();  //主机条码
                        tt_shortmac = ds7.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();    //短MAC
                        tt_smtaskscode = ds7.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();  //移动串号
                        tt_longmac = ds7.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();     //长MAC
                        tt_id = ds7.Tables[0].Rows[0].ItemArray[4].ToString();      //行ID
                        tt_gpsn0 = ds7.Tables[0].Rows[0].ItemArray[5].ToString().ToUpper();   //GPONSN
                        tt_taskscheck = ds7.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper();   //子工单判断
                        //tt_fsegment2 = ds7.Tables[0].Rows[0].ItemArray[7].ToString().ToUpper();   //小型化EPON信息写入临时措施

                        ////小型化EPON信息写入临时措施
                        //if (tt_fsegment2 == "" && tt_ponname == "EPON" && tt_power_re.Contains("小型化"))
                        //{
                        //    string tt_update1 = "update odc_alllable set fsegment2 = '育辰飞EPON BOSA' where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "'";
                        //    int tt_int = Dataset1.ExecCommand(tt_update1, tt_conn);
                        //    if (tt_int != 1)
                        //    {
                        //        tt_flag7 = false;
                        //        MessageBox.Show("小型化EPON信息更新失败!");
                        //    }
                        //}

                        if (tt_productname == "HG6201T" || tt_productname == "HG6543C1")
                        {
                            if (tt_gpsn0.Substring(0, 8) == "46485454")
                            {
                                tt_gpsn = tt_gpsn0;
                            }
                            else
                            {
                                tt_gpsn = Regex.Replace(tt_gpsn0, "FHTT", "46485454");
                            }                           
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

                        if ((tt_productname == "HG2201T" || tt_productname == "HG6201T") && this.label30.Text == "四川")
                        {
                            tt_Sichuanlongmac = tt_longmac.Replace("-", ":");
                        }

                        setRichtexBox("7、关联表查询到一条数据，hostlable=" + tt_hostlable + ",mac=" + tt_shortmac + ",smtaskscode=" + tt_smtaskscode + ",id=" + tt_id + ",GPSN=" + tt_gpsn + ",goon");

                    }
                    else
                    {
                        setRichtexBox("7、关联表没有查询到数据，over");
                        PutLableInfor("关联表没有查询到数据，请检查！");
                    }
                }

                //第八步 查找站位信息
                Boolean tt_flag8 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7)
                {
                    string tt_sql8 = "select count(1),min(ccode),min(ncode) from odc_routingtasklist " +
                                     "where  pcba_pn = '" + tt_shortmac + "' and napplytype is null ";


                    string[] tt_array8 = new string[3];
                    tt_array8 = Dataset1.GetDatasetArray(tt_sql8, tt_conn);
                    if (tt_array8[0] == "1")
                    {
                        if (tt_array8[2] == tt_ccode)
                        {
                            tt_flag8 = true;
                            setRichtexBox("8、该单板有待测站位，站位：" + tt_array8[1] + "，" + tt_array8[2] + ",可以过站 goon");
                        }
                        else
                        {
                            setRichtexBox("8、该单板待测站位不在" + tt_code + "，站位：" + tt_array8[1] + "，" + tt_array8[2] + ",不可以过站 goon");
                            PutLableInfor("该单板当前站位：" + tt_array8[2] + "不在" + tt_code + "站位！");
                        }
                    }
                    else
                    {
                        setRichtexBox("8、没有找到待测站位，或有多条待测站位，流程异常，over");
                        PutLableInfor("没有找到待测站位，或有多条待测站位，流程异常！");
                    }
                }

                //第九步查询MACINFO信息
                Boolean tt_flag9 = false;
                string tt_ssid = null;
                string tt_macusername = null;
                string tt_password = null;
                string tt_wlanpas = null;
                string tt_5guser = null;
                string tt_5gpassword = null;
                string tt_barcode1 = null;

                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8)
                {
                    tt_flag9 = true;
                    string tt_sql9 = "select ssid,username,password,Wlanpas,ssid_5G,wlanpas_5G,barcode1  from odc_macinfo " +
                                    "where taskscode = '" + tt_task + "' and mac = '" + tt_longmac + "' ";

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

                        if (this.label30.Text == "河南")
                        {
                            this.label125.Text = "FIBER|" + this.label29.Text + "|" + tt_gpsn + "|" + tt_shortmac;
                        }
                        else if (this.label30.Text == "浙江" || this.label30.Text == "江西")
                        {
                            this.label125.Text = "厂家:烽火通信科技股份有限公司,型号:" + this.label29.Text + ",SN:" + tt_gpsn +
                                                 ",生产日期:" + this.label28.Text.Replace("/", ".") + ",用户无线默认SSID:" + tt_ssid +
                                                 ",用户无线默认SSID密码:" + tt_wlanpas + ",用户登陆默认账号:" + tt_macusername +
                                                 ",用户登陆默认密码:" + tt_password + ",设备网卡MAC:" + tt_shortmac;
                        }
                        else
                        {
                            this.label125.Text = "";
                        }

                        setRichtexBox("9、Macinfo表找到一条数据，SSID=" + tt_ssid + ",username=" + tt_macusername + ",password=" + tt_password + ",wanlaps=" + tt_wlanpas + ",goon");
                    }
                    else
                    {
                        setRichtexBox("9、Macinfo表没有找到一条数据，over");
                        PutLableInfor("Macinfo表没有找到条数据，请检查！");
                    }
                }

                //第九步附一 用户名检查
                Boolean tt_flag9_1 = false;
                if (tt_flag9)
                {
                    if (tt_setusername == "0")
                    {
                        tt_flag9_1 = true;
                        setRichtexBox("9.1、设定的用户名为0，不需要进行用户名检验，goon");
                    }
                    else
                    {
                        if (tt_setusername == tt_macusername)
                        {
                            tt_flag9_1 = true;
                            setRichtexBox("9.1、获取MAC用户名与设定的用户一致，都是:" + tt_macusername + "，goon");
                        }
                        else
                        {
                            setRichtexBox("9.1、该MAC的用户名:" + tt_macusername + ",与设定的用户名不一致：" + tt_setusername + "，请检查MAC导入信息,over");
                            PutLableInfor("获取MAC用户名" + tt_macusername + "与设定的用户不一致，请检查MAC导入信息！");
                        }
                    }
                }

                //第九步附二 密码位数检查
                Boolean tt_flag9_2 = false;
                if (tt_flag9_1)
                {
                    if (tt_setpasswordlen == "0")
                    {
                        tt_flag9_2 = true;
                        setRichtexBox("9.2、密码位数设置为0,不需要位数判断，goon");
                    }
                    else
                    {
                        string tt_passwordlen = tt_password.Length.ToString();
                        if (tt_setpasswordlen == tt_passwordlen)
                        {
                            tt_flag9_2 = true;
                            setRichtexBox("9.2、获取MAC密码" + tt_password + "的位数与设定的密码位数一致，都是:" + tt_passwordlen + "位，goon");
                        }
                        else
                        {
                            setRichtexBox("9.2、获取MAC密码" + tt_password + "的位数与设定的密码位数不一致，不是:" + tt_passwordlen + "位，goon");
                            PutLableInfor("9.2、获取MAC密码" + tt_password + "的位数与设定的密码位数不一致");
                        }
                    }

                }

                //第九步附三 密码大小写检查
                Boolean tt_flag9_3 = false;
                if (tt_flag9_2)
                {
                    if (tt_setpasswordAa == "0")
                    {
                        tt_flag9_3 = true;
                        setRichtexBox("9.3、密码大小写设置为0,不需要大小判断，goon");
                    }
                    else
                    {
                        bool tt_flag101 = GetStrChar(tt_password, tt_setpasswordAa);
                        if (tt_flag101)
                        {
                            tt_flag9_3 = true;
                            setRichtexBox("9.3、密码大小写判断正确，goon");
                        }
                        else
                        {
                            setRichtexBox("9.3、该MAC的密码:" + tt_password + "，大小写判定不正确，1为小写2为大写");
                            setRichtexBox("该MAC的密码:" + tt_password + "，大小写判定不正确");
                        }
                    }
                }

                //第十步物料追溯添加
                Boolean tt_flag10 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag9_1 && tt_flag9_2 && tt_flag9_3)
                {
                    Boolean tt_idinfo = GetMaterialIdinfor(tt_id);

                    if (tt_idinfo)
                    {
                        string tt_insert = "insert into odc_traceback(fid,fmpdate,Fsegment1,Fsegment2,Fsegment3,Ftaskcode,Fpcba,Fhostlable,Fmaclable) " +
                                           "values(" + tt_id + ",getdate(),'" + tt_uplip + "','" + tt_downlip + "','" + tt_tin + "','"
                                            + tt_task + "','" + tt_scanpcba + "','" + tt_hostlable + "','" + tt_shortmac + "')";

                        int tt_int1 = Dataset1.ExecCommand(tt_insert, tt_conn);

                        if (tt_int1 > 0)
                        {
                            tt_flag10 = true;
                            setRichtexBox("10、物料追溯已成功追加到物料表odc_traceback，id号：" + tt_id + ",goon");
                        }
                        else
                        {
                            setRichtexBox("10、物料追溯没有成功追加物料表！,over");
                            PutLableInfor("物料追溯没有成功追加物料表!请继续扫描！");
                        }
                    }
                    else
                    {
                        string tt_update = "update odc_traceback set Fsegment1='" + tt_uplip + "',Fsegment2='" + tt_downlip + "',Fsegment3='" + tt_tin + "' " +
                                           "where Fid = " + tt_id;
                        int tt_int2 = Dataset1.ExecCommand(tt_update, tt_conn);

                        if (tt_int2 > 0)
                        {
                            tt_flag10 = true;
                            setRichtexBox("10、物料追溯已成功更新到物料表odc_traceback，id号：" + tt_id + ",goon");
                        }
                        else
                        {
                            setRichtexBox("10、物料追溯没有成功更新到物料表！,over");
                            PutLableInfor("物料追溯没有成功更新到物料表!请继续扫描！");
                        }

                    }

                }
                
                //第十一步老化判断
                Boolean tt_flag11 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag9_1 && tt_flag9_2 && tt_flag9_3 && tt_flag10)
                {
                     //不是华为产品就不用判断
                     tt_flag11 = true;
                    setRichtexBox("11、不是要判别的产品,不需老化判断！,goon");
                          
                }
                
                //第十二步开始过站

                Boolean tt_flag12 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag9_1 && tt_flag9_2 && tt_flag9_3 && tt_flag10 && tt_flag11)
                {
                    if (tt_taskscheck != tt_task)
                    {
                        tt_task = tt_taskscheck;
                    }
                    string tt_username = STR;
                    tt_flag12 = Dataset1.FhUnPassStation(tt_task, tt_username, tt_shortmac, tt_gyid, tt_code, tt_ncode, tt_conn);
                    if (tt_flag12)
                    {
                        setRichtexBox("12、单板过站成功，请继续扫描,ok");
                    }
                    else
                    {
                        setRichtexBox("12、单板关联不成功，事务已回滚");
                        PutLableInfor("单板过站不成功，请检查或再次扫描！");
                    }
                }
                
                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag9_1 && tt_flag9_2 && tt_flag9_3 && tt_flag10 && tt_flag11 && tt_flag12)
                {

                    this.label39.Text = tt_scanpcba;     //单板号
                    this.label40.Text = tt_hostlable;    //主机条码
                    this.label41.Text = tt_shortmac;     //短MAC
                    this.label42.Text = tt_smtaskscode;  //移动串号
                    this.label44.Text = tt_longmac;      //长MAC
                    this.label80.Text = tt_gpsn;         //GPSN
                    this.label95.Text = "";              //GPSN原始码
                    this.label71.Text = tt_onumac;       //PON MAC暗码
                    this.label108.Text = tt_ssid;        //2G用户名
                    this.label112.Text = tt_macusername; //用户名
                    this.label111.Text = tt_password;    //密码
                    this.label110.Text = tt_wlanpas;     //2G密码
                    this.label106.Text = tt_5guser;      //5G用户名
                    this.label104.Text = tt_5gpassword;  //5G密码
                    this.label101.Text = tt_barcode1;    //移动标识暗码

                    if (tt_gpsn0.Substring(0, 8) == "46485454")
                    {
                        this.label95.Text = Regex.Replace(tt_gpsn0, "46485454", "FHTT");
                    }
                    else
                    {
                        this.label95.Text = tt_gpsn0;
                    }

                    //扣数
                    tt_uplip--;  //上盖数量
                    tt_downlip--; //下盖数量
                    this.label55.Text = tt_uplip.ToString();
                    this.label56.Text = tt_downlip.ToString();

                    //生产节拍
                    getProductRhythm();

                    //打印记录

                    string tt_remark = "";
                    if (tt_parenttask == "1.5A" && tt_power_old != "1.5")
                    {
                        tt_remark = "原1.5A产品改为打印1.0A铭牌";
                    }

                    Dataset1.lablePrintRecord(tt_task,tt_shortmac,tt_hostlable,"设备铭牌",str,tt_computermac,tt_remark,tt_conn);

                    //打印
                    GetParaDataPrint1(1);
                    if (PlatePrintPattern == "1") //如果多打，调用
                    {
                        GetParaDataPrint2(1);
                    }
                    GetProductNumInfo();
                    CheckStation(tt_shortmac);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    PutLableInfor("过站成功，请继续扫描！");
                    textBox2.Focus();
                    textBox2.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox2.Focus();
                    textBox2.SelectAll();
                }

            }
        }

        //解锁MAC特征码输入框
        private void textBox14_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.textBox14.Text == "*963.")
                {
                    this.textBox6.Enabled = true;
                    this.textBox14.Text = null;
                }
            }
        }

        //锁定MAC特征码输入框
        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bool tt_flag1 = false;

                tt_flag1 = CheckStrLengh(this.textBox6.Text, "6");
                if (tt_flag1)
                {
                    this.textBox6.Enabled = false;
                    this.textBox2.Text = "";
                    this.textBox3.Text = "";
                    this.richTextBox1.BackColor = Color.White;
                    this.richTextBox1.Text = "";
                    setRichtexBox("输入框信息已清除，特征码输入框已锁定，over");
                    PutLableInfor("");
                }
                else
                {
                    string tt_maccheck = "";
                    string tt_sql1 = "select top(1)maclable from odc_alllable where taskscode = '" + this.textBox1.Text + "' ";
                    DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);

                    if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        tt_maccheck = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //随机取该制造单关联的一个MAC
                        this.textBox6.Text = tt_maccheck.Substring(0, 6); //获取该制造单MAC特征码

                        PutLableInfor("MAC锁定约束输入位数不正确，位数小于6，特征码已还原");
                        this.textBox6.Enabled = false;
                        this.textBox2.Text = "";
                        this.textBox3.Text = "";
                        this.richTextBox1.BackColor = Color.White;
                        this.richTextBox1.Text = "";
                        setRichtexBox("MAC锁定约束输入位数不正确，位数小于6，特征码已还原，over");
                    }
                    else
                    {
                        MessageBox.Show("网络连接异常，请重新锁定工单");
                    }
                }
            }

        }

        #endregion


        #region 9、物料追溯

        //上盖物料
        private void textBox8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string tt_input = this.textBox8.Text;

                string tt_pattern = @"\s(\d+)pcs";
                string tt_str = "";

                foreach (
                    Match match in Regex.Matches(tt_input, tt_pattern))
                    tt_str = match.Value;


                try
                {
                    tt_uplip = int.Parse(tt_str.Replace("pcs", ""));
                    this.label55.Text = tt_uplip.ToString();
                }
                catch
                {
                    MessageBox.Show("上盖转换为数字失败");
                }
            }
        }

        //下盖物料
        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string tt_input = this.textBox9.Text;

                string tt_pattern = @"\s(\d+)pcs";
                string tt_str = "";

                foreach (
                    Match match in Regex.Matches(tt_input, tt_pattern))
                    tt_str = match.Value;


                try
                {
                    tt_downlip = int.Parse(tt_str.Replace("pcs", ""));
                    this.label56.Text = tt_downlip.ToString();
                }
                catch
                {
                    MessageBox.Show("下盖转换为数字失败");
                }
            }
        }

        //物料信息锁定
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked)
            {
                this.textBox8.ReadOnly = true;
                this.textBox9.ReadOnly = true;
                this.textBox10.ReadOnly = true;

            }
            else
            {
                this.textBox8.ReadOnly = false;
                this.textBox9.ReadOnly = false;
                this.textBox10.ReadOnly = false;
            }
        }

        #endregion


        #region 10、按钮功能
        //重置按钮
        private void button1_Click(object sender, EventArgs e)
        {
            ScanDataInitial();
            this.textBox2.Text = null;
            this.textBox3.Text = null;
            Prints_Stop = 1;
            textBox2.Focus();
            textBox2.SelectAll();
        }

        //铭牌模板预览
        private void Plate_View_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {

                string tt_prientcode = this.label65.Text;
                string tt_checkcode = this.label66.Text;

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

            textBox2.Focus();
            textBox2.SelectAll();
        }

        //铭牌打印
        private void Plate_Print_Click(object sender, EventArgs e)
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
                if (str.Contains("FH011"))
                {
                    tt_info = "，包装产品会被退回check站位";
                }
                DialogResult dr = MessageBox.Show("确定要重打铭牌吗，打印信息被记录"+ tt_info, "铭牌重打", messButton);

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
                        string tt_remark = "";
                        if (tt_parenttask == "1.5A" && tt_power_old != "1.5")
                        {
                            tt_remark = "原1.5A产品改为打印1.0A铭牌";
                        }
                        else
                        {
                            Reprint form1 = new Reprint();
                            form1.StartPosition = FormStartPosition.CenterScreen;
                            form1.ShowDialog();

                            tt_remark = Dataset1.Context.ContextData["Key1"].ToString();
                        }

                        GetParaDataPrint1(1);  //打印
                        string tt_host = Gethostlable(tt_recordmac);
                        string tt_taskscode = Gettasks(tt_recordmac);
                        string tt_local = "铭牌标签";
                        string tt_username = "";
                        if (str.Contains("FH011"))
                        {
                            tt_username = this.comboBox2.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }

                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);

                        if (str.Contains("FH011"))
                        {
                            if (int.Parse(tt_nowcode) >= 3000)
                            {
                                string tt_gyid = this.label79.Text;
                                string tt_ccode = this.label85.Text;
                                string tt_ncode = "2230";
                                bool tt_flag1 = Dataset1.FhUnPassStationI(tt_taskscode, tt_username, tt_recordmac, tt_gyid, tt_ccode, tt_ncode, tt_conn);
                                if (tt_flag1 && tt_nowcode == "3201")
                                {
                                    int delete_checknum = Delete_Check(tt_recordmac);
                                    setRichtexBox("重打完成，产品属于待装箱产品，已退回check站位，产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对,ok");
                                    PutLableInfor("重打完成，产品属于待装箱产品，已退回check站位，条码比对数据已删除");
                                }
                                else if (tt_flag1)
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
                            tt_reprintchang1++;

                            if (tt_reprintchang1 >= 5)
                            {
                                this.checkBox1.Checked = false;
                                MessageBox.Show("非认证打印电脑，已达到打印上限，退出打印模式");
                                tt_reprintchang1 = 0;
                            }
                            else
                            {
                                MessageBox.Show("非认证打印电脑，已打印" + tt_reprintchang1 + "次，本次打印次数剩余" + (5 - tt_reprintchang1) +"次");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("当前站位或序号：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",或装箱产品已打散,才能重打标签");
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

        //运营商模板预览
        private void Operator_View_Click(object sender, EventArgs e)
        {
            if (this.dataGridView6.RowCount > 0)
            {

                string tt_prientcode = this.label65.Text;
                string tt_checkcode = this.label66.Text;

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
            else if (str.Contains("FH012") || str.Contains("FH112")) //小型化批量打印不检查
            {
                GetParaDataPrint2(2);  //预览
            }
            else
            {
                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再预览模板");
            }

            textBox2.Focus();
            textBox2.SelectAll();
        }

        //运营商打印
        private void Operator_Print_Click(object sender, EventArgs e)
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

            if (this.dataGridView6.RowCount > 0)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                string tt_info = "";
                if (str.Contains("FH011"))
                {
                    tt_info = "，包装产品会被退回check站位";
                }
                DialogResult dr = MessageBox.Show("确定要重打运营商标签吗，打印信息被记录" + tt_info, "运营商重打", messButton);

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

                        GetParaDataPrint2(1);  //打印
                        string tt_host = Gethostlable(tt_recordmac);
                        string tt_taskscode = Gettasks(tt_recordmac);
                        string tt_local = "运营商标签";
                        string tt_username = "";
                        if (str.Contains("FH011"))
                        {
                            tt_username = this.comboBox2.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }

                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);

                        if (str.Contains("FH011"))
                        {
                            if (int.Parse(tt_nowcode) >= 3000)
                            {
                                string tt_gyid = this.label79.Text;
                                string tt_ccode = this.label85.Text;
                                string tt_ncode = "2230";
                                bool tt_flag1 = Dataset1.FhUnPassStationI(tt_taskscode, tt_username, tt_recordmac, tt_gyid, tt_ccode, tt_ncode, tt_conn);
                                if (tt_flag1 && tt_nowcode == "3201")
                                {
                                    int delete_checknum = Delete_Check(tt_recordmac);
                                    setRichtexBox("重打完成，产品属于待装箱产品，已退回check站位，产品为待装箱产品，比对数据" + delete_checknum + "条已删除，需要重新条码比对,ok");
                                    PutLableInfor("重打完成，产品属于待装箱产品，已退回check站位，条码比对数据已删除");
                                }
                                else if (tt_flag1)
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
                            tt_reprintchang2++;

                            if (tt_reprintchang2 >= 5)
                            {
                                this.checkBox1.Checked = false;
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
                        MessageBox.Show("当前站位或序号：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",或装箱产品已打散,才能重打标签");
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

        //小型化铭牌批量打印(小型化产品的铭牌打印在软件中调用的是运营商标签打印)
        private void Minitype_Plate_Prints_Click(object sender, EventArgs e)
        {
            if (this.print_num.Text != "" && int.Parse(this.print_num.Text) > 0)
            {
                this.label47.Text = "";
                this.Minitype_Plate_Prints.Visible = false;
                Prints_Stop = 0;
                int printnum = int.Parse(this.print_num.Text);
                for (int i = 0; i < printnum; i++)
                {
                    if (Prints_Stop == 1) break;
                    GetParaDataPrint2(1);  //打印
                    this.richTextBox1.Text = null;
                    this.richTextBox1.BackColor = Color.White;
                    this.label47.Text = (i + 1).ToString();
                    Application.DoEvents(); //强制刷新UI
                }
                this.Minitype_Plate_Prints.Visible = true;
                if (int.Parse(this.label47.Text) < printnum)
                {
                    PutLableInfor("批量打印被强行终止，已打印完成" + this.label47.Text + "PCS");
                    this.richTextBox1.BackColor = Color.Red;
                }
                else
                {
                    PutLableInfor("批量打印" + this.print_num.Text + "PCS，打印完成");
                    this.richTextBox1.BackColor = Color.Chartreuse;
                }
            }

            print_num.Focus();
            print_num.SelectAll();
        }

        //批量打印输入限制
        private void print_num_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (Char)8)
            {
                e.Handled = true;
            }
        }

        //小型化铭牌批量打印中断
        private void Minitype_Prints_Stop_Click(object sender, EventArgs e)
        {
            Prints_Stop = 1;
            this.Minitype_Plate_Prints.Visible = true;
        }

        //线长调试模式
        private void LineManger_Click(object sender, EventArgs e)
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
                    comboBox2.DataSource = ds1.Tables[0];
                    comboBox2.DisplayMember = "fusername";
                    this.groupBox14.Visible = true;
                    this.comboBox1.Text = "0.3";
                    this.comboBox2.Text = "下拉选择";
                    this.textBox21.Text = "";
                    this.textBox22.Text = "";
                    this.comboBox2.Enabled = true;
                    this.textBox21.Enabled = true;
                    this.textBox22.Enabled = true;
                    this.groupBox15.Visible = false;
                    this.Plate_Print.Visible = false;
                    if (str.Contains("FH012"))
                    {
                        this.print_num.Text = "1";
                        this.print_num.Enabled = false;
                    }
                    else
                    {
                        this.tabPage4.Parent = null;
                        this.tabPage3.Parent = tabControl2;
                        this.textBox3.Enabled = true;
                        this.textBox3.Text = "";
                    }
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

        //线长工号输入限制
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
                    if (str.Contains("FH012"))
                    {
                        this.tabPage9.Parent = null;
                        this.tabPage10.Parent = tabControl5;
                    }
                    else
                    {
                        this.Plate_Print.Visible = true;
                        if (PlatePrintPattern == "1")
                        {
                            this.Operator_Print.Visible = true;
                        }
                        this.tabPage3.Parent = null;
                        this.tabPage4.Parent = tabControl2;
                    }
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
            if (str.Contains("FH012"))
            {
                this.print_num.Text = "";
                this.print_num.Enabled = true;
            }
            else
            {
                this.Plate_Print.Visible = false;
                this.tabPage4.Parent = null;
                this.tabPage3.Parent = tabControl2;
            }
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
            if (str.Contains("FH012"))
            {
                this.print_num.Text = "";
                this.print_num.Enabled = true;
            }
            else
            {
                this.Plate_Print.Visible = false;
                this.tabPage4.Parent = null;
                this.tabPage3.Parent = tabControl2;
            }
        }

        //上移按钮
        private void Plate_Up_Click(object sender, EventArgs e)
        {
            tt_top1 -= float.Parse(this.comboBox1.Text);
        }

        //下移按钮
        private void Plate_Down_Click(object sender, EventArgs e)
        {
            tt_top1 += float.Parse(this.comboBox1.Text);
        }

        //左移按钮
        private void Plate_Left_Click(object sender, EventArgs e)
        {
            tt_left1 -= float.Parse(this.comboBox1.Text);
        }

        //右移按钮
        private void Plate_Right_Click(object sender, EventArgs e)
        {
            tt_left1 += float.Parse(this.comboBox1.Text);
        }

        //上移按钮
        private void Operator_Up_Click(object sender, EventArgs e)
        {
            tt_top2 -= float.Parse(this.comboBox1.Text);
        }

        //下移按钮
        private void Operator_Down_Click(object sender, EventArgs e)
        {
            tt_top2 += float.Parse(this.comboBox1.Text);
        }

        //左移按钮
        private void Operator_Left_Click(object sender, EventArgs e)
        {
            tt_left2 -= float.Parse(this.comboBox1.Text);
        }

        //右移按钮
        private void Operator_Right_Click(object sender, EventArgs e)
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
            if (str.Contains("FH012"))
            {
                this.print_num.Text = "";
                this.print_num.Enabled = true;
            }
            else
            {
                this.Plate_Print.Visible = false;
                this.tabPage4.Parent = null;
                this.tabPage3.Parent = tabControl2;
            }
        }

        #endregion


        #region 11、铭牌数据采集及模板打印
        //获取参数
        private void GetParaDataPrint1(int tt_itemtype)
        {
            string tt_fdata1 = this.label32.Text;

            //mp01---数据类型一
            if (tt_fdata1 == "MP01")
            {
                GetParaDataPrint1_MP01(tt_itemtype);
            }

            //mp01---数据类型一
            if (tt_fdata1 == "MC01")
            {
                GetParaDataPrint1_MC01(tt_itemtype);
            }

            //md01---数据类型一
            if (tt_fdata1 == "MD01")
            {
                GetParaDataPrint1_MD01(tt_itemtype);
            }

            //me01---数据类型一
            if (tt_fdata1 == "MF01")
            {
                GetParaDataPrint1_MF01(tt_itemtype);
            }

        }


        //获取参数信息及打印
        private void GetParameter()
        {
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();
            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            DataRow row1 = dt.NewRow();
            row1["参数"] = "P01";
            row1["名称"] = "产品型号";
            row1["内容"] = this.label29.Text;
            dt.Rows.Add(row1);

            DataRow row2 = dt.NewRow();
            row2["参数"] = "P01";
            row2["名称"] = "主机条码";
            row2["内容"] = this.label40.Text;
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "P01";
            row3["名称"] = "MAC";
            row3["内容"] = this.label41.Text;
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "P01";
            row4["名称"] = "移动号码";
            row4["内容"] = this.label42.Text;
            dt.Rows.Add(row4);

            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 60;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 200;

        }


        //----以下是MP01数据采集----
        private void GetParaDataPrint1_MP01(int tt_itemtype)
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
            row3["名称"] = "GPSN\\OUN MAC";
            row3["内容"] = this.label80.Text;
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "物料编码";
            row4["内容"] = this.label49.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "短MAC";
            row5["内容"] = this.label41.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "长MAC";
            row6["内容"] = this.label44.Text;
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "地区码";
            row7["内容"] = this.label67.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "PON类型";
            row8["内容"] = this.label69.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "GPSN\\ONU MAC暗码";
            row9["内容"] = this.label71.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "文字变量01";
            row10["内容"] = this.label87.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "文字变量02";
            row11["内容"] = this.label88.Text;
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
            
            DataRow row15 = dt.NewRow();
            row15["参数"] = "S15";
            row15["名称"] = "用户名";
            row15["内容"] = this.label112.Text;
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "S16";
            row16["名称"] = "密码";
            row16["内容"] = this.label111.Text;
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "S17";
            row17["名称"] = "2G账号";
            row17["内容"] = this.label108.Text;
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "S18";
            row18["名称"] = "2G密码";
            row18["内容"] = this.label110.Text;
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "S19";
            row19["名称"] = "5G账号";
            row19["内容"] = this.label106.Text;
            dt.Rows.Add(row19);

            DataRow row20 = dt.NewRow();
            row20["参数"] = "S20";
            row20["名称"] = "5G密码";
            row20["内容"] = this.label104.Text;
            dt.Rows.Add(row20);

            DataRow row21 = dt.NewRow();
            row21["参数"] = "S21";
            row21["名称"] = "设备标识";
            row21["内容"] = this.label42.Text;
            dt.Rows.Add(row21);

            DataRow row22 = dt.NewRow();
            row22["参数"] = "S22";
            row22["名称"] = "标识暗码";
            row22["内容"] = this.label101.Text;
            dt.Rows.Add(row22);

            DataRow row23 = dt.NewRow();
            row23["参数"] = "S23";
            row23["名称"] = "四川电信日期";
            row23["内容"] = tt_SichuanTime;
            dt.Rows.Add(row23);

            DataRow row24 = dt.NewRow();
            row24["参数"] = "S24";
            row24["名称"] = "四川电信LongMAC";
            row24["内容"] = tt_Sichuanlongmac;
            dt.Rows.Add(row24);

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
                report.Load(tt_path1);
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
                report.SetParameterValue("S15", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("S16", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("S17", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("S18", dst.Tables[0].Rows[17][2].ToString());
                report.SetParameterValue("S19", dst.Tables[0].Rows[18][2].ToString());
                report.SetParameterValue("S20", dst.Tables[0].Rows[19][2].ToString());
                report.SetParameterValue("S21", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("S22", dst.Tables[0].Rows[21][2].ToString());
                report.SetParameterValue("S23", dst.Tables[0].Rows[22][2].ToString());
                report.SetParameterValue("S24", dst.Tables[0].Rows[23][2].ToString());

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
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top1;
                        p3.Left += tt_left1;
                    }
                    s = string.Format("Line{0}", i + 1);
                    LineObject p4 = report.FindObject(s) as LineObject;
                    if (p4 != null)
                    {
                        p4.Top += tt_top1;
                        p4.Left += tt_left1;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    if (PlatePrintPattern == "1")
                    {
                        report.PrintSettings.Printer = "铭牌";
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

                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }


        }

        //----以下是MC01数据采集----
        private void GetParaDataPrint1_MC01(int tt_itemtype)
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
            row3["内容"] = this.label80.Text;
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "物料编码";
            row4["内容"] = this.label49.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "短MAC";
            row5["内容"] = this.label41.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "长MAC";
            row6["内容"] = this.label44.Text;
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "地区码";
            row7["内容"] = this.label67.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "PON类型";
            row8["内容"] = this.label69.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "GPSN\\ONU MAC暗码";
            row9["内容"] = this.label71.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "文字变量01";
            row10["内容"] = this.label87.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "文字变量02";
            row11["内容"] = this.label88.Text;
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

            DataRow row15 = dt.NewRow();
            row15["参数"] = "S15";
            row15["名称"] = "设备标识";
            row15["内容"] = this.label42.Text;
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "S16";
            row16["名称"] = "设备标示暗码";
            row16["内容"] = this.label101.Text;
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "S17";
            row17["名称"] = "用户名";
            row17["内容"] = this.label112.Text;
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "S18";
            row18["名称"] = "密码";
            row18["内容"] = this.label111.Text;
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "S19";
            row19["名称"] = "2G账号";
            row19["内容"] = this.label108.Text;
            dt.Rows.Add(row19);

            DataRow row20 = dt.NewRow();
            row20["参数"] = "S20";
            row20["名称"] = "2G密码";
            row20["内容"] = this.label110.Text;
            dt.Rows.Add(row20);

            DataRow row21 = dt.NewRow();
            row21["参数"] = "S21";
            row21["名称"] = "5G账号";
            row21["内容"] = this.label106.Text;
            dt.Rows.Add(row21);

            DataRow row22 = dt.NewRow();
            row22["参数"] = "S22";
            row22["名称"] = "5G密码";
            row22["内容"] = this.label104.Text;
            dt.Rows.Add(row22);

            DataRow row23 = dt.NewRow();
            row23["参数"] = "S23";
            row23["名称"] = "GPSN//ONU MAC原码";
            row23["内容"] = this.label95.Text;
            dt.Rows.Add(row23);

            DataRow row24 = dt.NewRow();
            row24["参数"] = "S24";
            row24["名称"] = "四川电信日期";
            row24["内容"] = tt_SichuanTime;
            dt.Rows.Add(row24);

            DataRow row25 = dt.NewRow();
            row25["参数"] = "S25";
            row25["名称"] = "四川电信LongMAC";
            row25["内容"] = tt_Sichuanlongmac;
            dt.Rows.Add(row25);

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
                report.Load(tt_path1);
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
                report.SetParameterValue("S15", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("S16", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("S17", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("S18", dst.Tables[0].Rows[17][2].ToString());
                report.SetParameterValue("S19", dst.Tables[0].Rows[18][2].ToString());
                report.SetParameterValue("S20", dst.Tables[0].Rows[19][2].ToString());
                report.SetParameterValue("S21", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("S22", dst.Tables[0].Rows[21][2].ToString());
                report.SetParameterValue("S23", dst.Tables[0].Rows[22][2].ToString());
                report.SetParameterValue("S24", dst.Tables[0].Rows[23][2].ToString());
                report.SetParameterValue("S25", dst.Tables[0].Rows[24][2].ToString());

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
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top1;
                        p3.Left += tt_left1;
                    }
                    s = string.Format("Line{0}", i + 1);
                    LineObject p4 = report.FindObject(s) as LineObject;
                    if (p4 != null)
                    {
                        p4.Top += tt_top1;
                        p4.Left += tt_left1;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    if (PlatePrintPattern == "1")
                    {
                        report.PrintSettings.Printer = "铭牌";
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

                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }


        }

        //----以下是MD01数据采集----
        private void GetParaDataPrint1_MD01(int tt_itemtype)
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
            row3["内容"] = this.label95.Text;
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "物料编码";
            row4["内容"] = this.label49.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "短MAC";
            row5["内容"] = this.label41.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "长MAC";
            row6["内容"] = this.label44.Text;
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "地区码";
            row7["内容"] = this.label67.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "PON类型";
            row8["内容"] = this.label69.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "GPSN\\ONU MAC暗码";
            row9["内容"] = this.label71.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "文字变量01";
            row10["内容"] = this.label87.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "文字变量02";
            row11["内容"] = this.label88.Text;
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
                report.Load(tt_path1);
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
                    if (PlatePrintPattern == "1")
                    {
                        report.PrintSettings.Printer = "铭牌";
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

                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }


        }

        //----以下是MF01数据采集----
        private void GetParaDataPrint1_MF01(int tt_itemtype)
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
            row3["内容"] = this.label80.Text + "(" + this.label95.Text + ")";
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "物料编码";
            row4["内容"] = this.label49.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "短MAC";
            row5["内容"] = this.label41.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "长MAC";
            row6["内容"] = this.label44.Text;
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "地区码";
            row7["内容"] = this.label67.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "PON类型";
            row8["内容"] = this.label69.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "GPSN\\ONU MAC暗码";
            row9["内容"] = this.label71.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "文字变量01";
            row10["内容"] = this.label87.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "文字变量02";
            row11["内容"] = this.label88.Text;
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

            DataRow row15 = dt.NewRow();
            row15["参数"] = "S15";
            row15["名称"] = "设备标识";
            row15["内容"] = this.label42.Text;
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "S16";
            row16["名称"] = "设备标示暗码";
            row16["内容"] = this.label101.Text;
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "S17";
            row17["名称"] = "用户名";
            row17["内容"] = this.label112.Text;
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "S18";
            row18["名称"] = "密码";
            row18["内容"] = this.label111.Text;
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "S19";
            row19["名称"] = "2G账号";
            row19["内容"] = this.label108.Text;
            dt.Rows.Add(row19);

            DataRow row20 = dt.NewRow();
            row20["参数"] = "S20";
            row20["名称"] = "2G密码";
            row20["内容"] = this.label110.Text;
            dt.Rows.Add(row20);
            
            DataRow row21 = dt.NewRow();
            row21["参数"] = "S21";
            row21["名称"] = "5G账号";
            row21["内容"] = this.label106.Text;
            dt.Rows.Add(row21);

            DataRow row22 = dt.NewRow();
            row22["参数"] = "S22";
            row22["名称"] = "5G密码";
            row22["内容"] = this.label104.Text;
            dt.Rows.Add(row22);

            DataRow row23 = dt.NewRow();
            row23["参数"] = "S23";
            row23["名称"] = "GPSN//ONU MAC原码";
            row23["内容"] = this.label95.Text;
            dt.Rows.Add(row23);

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
                report.Load(tt_path1);
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
                report.SetParameterValue("S15", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("S16", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("S17", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("S18", dst.Tables[0].Rows[17][2].ToString());
                report.SetParameterValue("S19", dst.Tables[0].Rows[18][2].ToString());
                report.SetParameterValue("S20", dst.Tables[0].Rows[19][2].ToString());
                report.SetParameterValue("S21", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("S22", dst.Tables[0].Rows[21][2].ToString());
                report.SetParameterValue("S23", dst.Tables[0].Rows[22][2].ToString());

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
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top1;
                        p3.Left += tt_left1;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    if (PlatePrintPattern == "1")
                    {
                        report.PrintSettings.Printer = "铭牌";
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

                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }


        }


        ////----以下是MC01数据采集（电信双频）----
        //private void GetParaDataPrint_MC01(int tt_itemtype)
        //{
        //    //第一步数据准备
        //    DataSet dst = new DataSet();
        //    DataTable dt = new DataTable();
        //    dst.Tables.Add(dt);
        //    dt.Columns.Add("参数");
        //    dt.Columns.Add("名称");
        //    dt.Columns.Add("内容");


        //    DataRow row1 = dt.NewRow();
        //    row1["参数"] = "S01";
        //    row1["名称"] = "WAN-MAC短";
        //    row1["内容"] = this.label41.Text;
        //    dt.Rows.Add(row1);


        //    DataRow row2 = dt.NewRow();
        //    row2["参数"] = "S02";
        //    row2["名称"] = "WAN-MAC长";
        //    row2["内容"] = this.label44.Text;
        //    dt.Rows.Add(row2);



        //    DataRow row3 = dt.NewRow();
        //    row3["参数"] = "S03";
        //    row3["名称"] = "ONU-MAC短";
        //    row3["内容"] = this.label41.Text;
        //    dt.Rows.Add(row3);


        //    DataRow row4 = dt.NewRow();
        //    row4["参数"] = "S04";
        //    row4["名称"] = "ONU-MAC长";
        //    row4["内容"] = this.label44.Text;
        //    dt.Rows.Add(row4);




        //    this.dataGridView2.DataSource = null;
        //    this.dataGridView2.Rows.Clear();

        //    this.dataGridView2.DataSource = dst.Tables[0];
        //    this.dataGridView2.Update();

        //    this.dataGridView2.Columns[0].Width = 50;
        //    this.dataGridView2.Columns[1].Width = 90;
        //    this.dataGridView2.Columns[2].Width = 200;


        //    //第四步 打印或预览
        //    //单板打印
        //    if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
        //    {
        //        FastReport.Report report = new FastReport.Report();

        //        report.Prepare();
        //        report.Load(tt_path);
        //        report.SetParameterValue("S01", dst.Tables[0].Rows[0][2].ToString());
        //        report.SetParameterValue("S02", dst.Tables[0].Rows[1][2].ToString());
        //        report.SetParameterValue("S03", dst.Tables[0].Rows[2][2].ToString());
        //        report.SetParameterValue("S04", dst.Tables[0].Rows[3][2].ToString());


        //        report.PrintSettings.ShowDialog = false;

        //        //--打印
        //        if (tt_itemtype == 1)
        //        {
        //            report.Print();
        //            PutLableInfor("打印完毕");
        //        }

        //        //--预览
        //        if (tt_itemtype == 2)
        //        {
        //            report.Design();
        //            PutLableInfor("预览完毕");
        //        }




        //        setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");


        //    }
        //    else
        //    {
        //        setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
        //        PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
        //    }


        //}


        #endregion

        #region 10、运营商数据采集及模板打印
        //获取参数
        private void GetParaDataPrint2(int tt_itemtype)
        {
            string tt_fdata2 = this.label113.Text;

            //MP01---数据类型一
            if (tt_fdata2 == "YD01")
            {
                GetParaDataPrint2_YD01(tt_itemtype);
            }

            //DX01---数据类型一
            if (tt_fdata2 == "DX01")
            {
                GetParaDataPrint2_DX01(tt_itemtype);
            }

            //CG01---数据类型一
            if (tt_fdata2 == "CG01")
            {
                GetParaDataPrint2_CG01(tt_itemtype);
            }
        }

        //----以下是YD01数据采集----
        private void GetParaDataPrint2_YD01(int tt_itemtype)
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
            row1["名称"] = "设备型号";
            row1["内容"] = this.label29.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "设备标识";
            row2["内容"] = this.label42.Text;
            dt.Rows.Add(row2);


            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "配置账号";
            row3["内容"] = this.label112.Text;
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "配置密码";
            row4["内容"] = this.label111.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "网络名称";
            row5["内容"] = this.label108.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "网络密匙";
            row6["内容"] = this.label110.Text;
            dt.Rows.Add(row6);


            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "5G账号";
            row7["内容"] = this.label106.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "5G密码";
            row8["内容"] = this.label104.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "设备标示暗码";
            row9["内容"] = this.label101.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "PON类型";
            row10["内容"] = this.label69.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "移动服务热线";
            row11["内容"] = this.label124.Text;
            dt.Rows.Add(row11);

            DataRow row12 = dt.NewRow();
            row12["参数"] = "S12";
            row12["名称"] = "移动文字变量";
            row12["内容"] = this.label121.Text;
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "S13";
            row13["名称"] = "移动二维码";
            row13["内容"] = this.label125.Text;
            dt.Rows.Add(row13);

            this.dataGridView6.DataSource = null;
            this.dataGridView6.Rows.Clear();

            this.dataGridView6.DataSource = dst.Tables[0];
            this.dataGridView6.Update();

            this.dataGridView6.Columns[0].Width = 50;
            this.dataGridView6.Columns[1].Width = 80;
            this.dataGridView6.Columns[2].Width = 200;


            //第四步 打印或预览
            //单板打印
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path2);
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
                    s = string.Format("Line{0}", i + 1);
                    LineObject p4 = report.FindObject(s) as LineObject;
                    if (p4 != null)
                    {
                        p4.Top += tt_top2;
                        p4.Left += tt_left2;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    if (PlatePrintPattern == "1")
                    {
                        report.PrintSettings.Printer = "运营商";
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

                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }


        }

        //----以下是DX01数据采集----
        private void GetParaDataPrint2_DX01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();
            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            string tt_ltponword = "";
            if (this.label69.Text == "GPON")
            {
                tt_ltponword = "&sn=";
            }
            else if (this.label69.Text == "EPON")
            {
                tt_ltponword = "&mac=";
            }

            string tt_LTQR = "http://op.smartont.net/app/download?ssid1=" + this.label108.Text + "&password=" + this.label110.Text +
                             "&username=" + this.label112.Text + "&pwd=" + this.label111.Text + "&model=" + this.label69.Text +
                             "&type=" + this.label29.Text + tt_ltponword + this.label71.Text + "&serialnumber=" + this.label101.Text +
                             "&ip=192.168.1.1";

            string tt_LTQR_2G_Min = "http://op.smartont.net/app/download?ssid1=" + this.label108.Text + "&password=" + this.label110.Text + 
                                    "&username=" + this.label108.Text + "&pwd=" + this.label110.Text;

            string tt_XJDZ = "";
            if ((this.label29.Text == "HG6201T" || this.label29.Text == "HG2201T") && this.label30.Text == "新疆")
            {
                tt_XJDZ = "此终端所有权归新疆电信公司所有";
            }

            string tt_HNDZ = "";
            if ((this.label29.Text == "HG6201T" || this.label29.Text == "HG2201T") && this.label30.Text == "海南")
            {
                tt_HNDZ = "　双协议\r\n防雷4KV";
            }

            DataRow row1 = dt.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "设备型号";
            row1["内容"] = this.label29.Text;
            dt.Rows.Add(row1);

            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "设备标识";
            row2["内容"] = this.label42.Text;
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "配置账号";
            row3["内容"] = this.label112.Text;
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "配置密码";
            row4["内容"] = this.label111.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "网络名称";
            row5["内容"] = this.label108.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "网络密匙";
            row6["内容"] = this.label109.Text;
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "5G账号";
            row7["内容"] = this.label106.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "5G密码";
            row8["内容"] = this.label104.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "设备标示暗码";
            row9["内容"] = this.label101.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "PON类型";
            row10["内容"] = this.label69.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "SN";
            row11["内容"] = this.label71.Text;
            dt.Rows.Add(row11);

            DataRow row12 = dt.NewRow();
            row12["参数"] = "S12";
            row12["名称"] = "QR";
            row12["内容"] = tt_LTQR;
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "S13";
            row13["名称"] = "XJDZ";
            row13["内容"] = tt_XJDZ;
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "S14";
            row14["名称"] = "HNDZ";
            row14["内容"] = tt_HNDZ;
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "S15";
            row15["名称"] = "网络类型";
            row15["内容"] = this.label88.Text;
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "S16";
            row16["名称"] = "电流";
            row16["内容"] = this.label92.Text;
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "S17";
            row17["名称"] = "电压";
            row17["内容"] = this.label90.Text;
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "S18";
            row18["名称"] = "COMITID";
            row18["内容"] = this.label61.Text;
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "S19";
            row19["名称"] = "联通单频小型化二维码";
            row19["内容"] = tt_LTQR_2G_Min;
            dt.Rows.Add(row19);

            this.dataGridView6.DataSource = null;
            this.dataGridView6.Rows.Clear();

            this.dataGridView6.DataSource = dst.Tables[0];
            this.dataGridView6.Update();

            this.dataGridView6.Columns[0].Width = 50;
            this.dataGridView6.Columns[1].Width = 80;
            this.dataGridView6.Columns[2].Width = 200;


            //第四步 打印或预览
            //单板打印
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path2);
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
                report.SetParameterValue("S15", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("S16", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("S17", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("S18", dst.Tables[0].Rows[17][2].ToString());
                report.SetParameterValue("S19", dst.Tables[0].Rows[18][2].ToString());

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
                    s = string.Format("Line{0}", i + 1);
                    LineObject p4 = report.FindObject(s) as LineObject;
                    if (p4 != null)
                    {
                        p4.Top += tt_top2;
                        p4.Left += tt_left2;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    if (PlatePrintPattern == "1" && !(str.Contains("FH012") || str.Contains("FH112")))
                    {
                        report.PrintSettings.Printer = "运营商";
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

                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }
        }

        //----以下是CG01数据采集----
        private void GetParaDataPrint2_CG01(int tt_itemtype)
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
            row1["名称"] = "设备型号";
            row1["内容"] = this.label29.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "设备标识";
            row2["内容"] = this.label42.Text;
            dt.Rows.Add(row2);


            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "配置账号";
            row3["内容"] = this.label112.Text;
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "配置密码";
            row4["内容"] = this.label111.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "网络名称";
            row5["内容"] = this.label108.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "网络密匙";
            row6["内容"] = this.label110.Text;
            dt.Rows.Add(row6);


            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "5G账号";
            row7["内容"] = this.label106.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "5G密码";
            row8["内容"] = this.label104.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "设备标示暗码";
            row9["内容"] = this.label101.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "PON类型";
            row10["内容"] = this.label69.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "移动服务热线";
            row11["内容"] = this.label124.Text;
            dt.Rows.Add(row11);

            DataRow row12 = dt.NewRow();
            row12["参数"] = "S12";
            row12["名称"] = "移动文字变量";
            row12["内容"] = this.label121.Text;
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "S13";
            row13["名称"] = "移动二维码";
            row13["内容"] = this.label125.Text;
            dt.Rows.Add(row13);

            this.dataGridView6.DataSource = null;
            this.dataGridView6.Rows.Clear();

            this.dataGridView6.DataSource = dst.Tables[0];
            this.dataGridView6.Update();

            this.dataGridView6.Columns[0].Width = 50;
            this.dataGridView6.Columns[1].Width = 80;
            this.dataGridView6.Columns[2].Width = 200;


            //第四步 打印或预览
            //单板打印
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path2);
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
                    s = string.Format("Line{0}", i + 1);
                    LineObject p4 = report.FindObject(s) as LineObject;
                    if (p4 != null)
                    {
                        p4.Top += tt_top2;
                        p4.Left += tt_left2;
                    }
                }

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    if (PlatePrintPattern == "1")
                    {
                        report.PrintSettings.Printer = "运营商";
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

                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");
            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印");
            }
        }


        #endregion


    }
}
