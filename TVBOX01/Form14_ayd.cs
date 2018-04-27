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
using FastReport;
using FastReport.Barcode;
using System.Text.RegularExpressions;

namespace TVBOX01
{
    public partial class Form14_ayd : Form
    {
        public Form14_ayd()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_conn;
        //static string tt_code = "0000";
        int tt_yield = 0;  //产量
        static string tt_path = "";
        static string tt_md5 = "";
        //标签微调
        static float tt_top = 0; //上下偏移量
        static float tt_left = 0; //左右偏移量

        static int tt_reprinttime = 0; //重打次数

        //天翼小型化方案参数
        static string tt_Vendorid = "";
        static string tt_Voltage = "";
        static string tt_Ampere = "";
        static string tt_NetType = "";

        DateTime tt_productstarttime = DateTime.Now; //开始时间
        DateTime tt_productprimtime; //上一次时间

        //重打限制标识
        string tt_reprintmark = "1";
        //重打限数
        int tt_reprintchang = 0;
        //重打计时
        DateTime tt_reprintstattime;
        DateTime tt_reprintendtime;

        //本机MAC
        static string tt_computermac = "";
        private void Form14_ayd_Load(object sender, EventArgs e)
        {
            //FastReport环境变量设置（打印时不提示 "正在准备../正在打印..",一个程序只需设定一次，故一般写在程序入口）
            (new FastReport.EnvironmentSettings()).ReportSettings.ShowProgress = false;

            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";

            this.toolStripStatusLabel6.Text = tt_productstarttime.ToString();

            //初始不显示身份验证栏
            this.groupBox15.Visible = false;

            //初始不显示微调栏
            this.groupBox14.Visible = false;

            //隐藏线长调试按钮
            this.button14.Visible = false;

            //员工账号分离
            if (str.Contains("FH002"))
            {
                this.button2.Visible = false;
                this.button3.Visible = false;
                this.tabPage4.Parent = null;
                this.button14.Visible = true;
            }

            ClearLabelInfo();

            //生产节拍
            this.label15.Text = tt_yield.ToString();
            this.label16.Text = null;
            this.label17.Text = null;
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


        #region 2、清除事件

        private void ClearLabelInfo()
        {
            //清除工单信息
            this.label55.Text = null;
            this.label56.Text = null;
            this.label57.Text = null;
            this.label58.Text = null;
            this.label59.Text = null;
            this.label60.Text = null;
            this.label61.Text = null;
            this.label62.Text = null;
            this.label63.Text = null;
            this.label67.Text = null;
            this.label73.Text = null;
            this.label86.Text = null;
            this.label91.Text = null;
            this.textBox4.Text = null;
            this.textBox5.Text = null;


            //流程信息
            this.label47.Text = null;
            this.label48.Text = null;
            this.label49.Text = null;
            this.label71.Text = null;
            this.label74.Text = null;
            this.label75.Text = null;



            //提示信息
            this.label25.Text = null;


            //生产信息
            this.label52.Text = null;
            this.label53.Text = null;


            //条码信息
            this.label35.Text = null;
            this.label36.Text = null;
            this.label37.Text = null;
            this.label38.Text = null;
            this.label39.Text = null;
            this.label40.Text = null;
            this.label41.Text = null;
            this.label42.Text = null;
            this.label43.Text = null;
            this.label69.Text = null;
            this.label78.Text = null;
            this.label80.Text = null;
            this.label83.Text = null;
            this.label85.Text = null;
            this.label88.Text = null;


            //扫描框
            this.textBox2.Visible = false;
            this.textBox7.Visible = false;

        }


        //扫描前数据初始化
        private void ScanDataInitial()
        {
            //条码信息清除
            this.label35.Text = null;
            this.label36.Text = null;
            this.label37.Text = null;
            this.label38.Text = null;
            this.label39.Text = null;
            this.label40.Text = null;
            this.label41.Text = null;
            this.label42.Text = null;
            this.label43.Text = null;
            this.label69.Text = null;
            this.label78.Text = null;
            this.label80.Text = null;
            this.label83.Text = null;
            this.label88.Text = null;


            //提示信息
            this.label25.Text = null;

            //当前站位
            this.label71.Text = null;


            //表格
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

            //richtext
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;


        }

        
        #endregion


        #region 3、锁定事件
        // MAC过站锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.textBox3.Enabled = false;
            }
            else
            {
                this.textBox3.Enabled = true;
            }
        }

        //MAC重打锁定
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

        //工单锁定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                if (str.Contains("FH102"))
                {
                    this.button3.Visible = true;
                    this.tabPage4.Parent = tabControl2;
                    //获取调试开始时间
                    tt_reprintstattime = DateTime.Now;
                }

                tt_computermac = Dataset1.GetHostIpName();

                string tt_sql1 = "select  tasksquantity,product_name,areacode,fec,convert(varchar, taskdate, 102) fdate,customer,flhratio,Gyid,Tasktype,pon_name,Vendorid " +
                                 "from odc_tasks where taskscode = '" + this.textBox1.Text + "' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);

                
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label55.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    string tt_productname = ds1.Tables[0].Rows[0].ItemArray[1].ToString(); //产品名称
                    this.label57.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString(); //地区
                    this.label60.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString(); //EC编码
                    this.label59.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //生产日期

                    this.label47.Text = ds1.Tables[0].Rows[0].ItemArray[7].ToString();  //流程配置
                    this.label58.Text = ds1.Tables[0].Rows[0].ItemArray[8].ToString();  //物料编码
                    this.label85.Text = ds1.Tables[0].Rows[0].ItemArray[9].ToString();  //PON类型

                    tt_Vendorid = ds1.Tables[0].Rows[0].ItemArray[10].ToString().ToUpper();  //天翼小型化测试用CMIID

                    if (tt_productname == "HG6201G" || tt_productname == "HG6201GW" || tt_productname == "HG6201GS")
                    {
                        this.label56.Text = "HG6201M";
                    }
                    else
                    {
                        this.label56.Text = tt_productname;
                    }

                    if (this.label57.Text == "河南")
                    {
                        this.label86.Text = "10086-5";
                        this.label91.Text = "设备二维码信息";
                    }
                    else if (this.label57.Text == "江西")
                    {
                        this.label86.Text = "10086";
                        this.label91.Text = "零配置";
                    }
                    else
                    {
                        this.label86.Text = "10086";
                        this.label91.Text = "";
                    }

                    if (this.label85.Text == "GPON")
                    {
                        tt_NetType = "吉比特";
                    }
                    else if (this.label85.Text == "EPON")
                    {
                        tt_NetType = "以太网";
                    }

                    //锁死MAC特征码输入框
                    this.textBox4.Enabled = false;
                    this.textBox5.Enabled = false;

                    //第一步、流程检查
                    Boolean tt_flag1 = false;
                    if (!this.label47.Text.Equals(""))
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
                   
                    //第一步附 电源规格查询
                    Boolean tt_flag1_1 = false;
                    if (tt_flag1)
                    {
                        string tt_sql3 = "select volt,ampere from odc_dypowertype where ftype = '" + tt_productname + "' ";
                        DataSet ds3 = Dataset1.GetDataSetTwo(tt_sql3, tt_conn);

                        if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                        {
                            tt_Voltage = ds3.Tables[0].Rows[0].ItemArray[0].ToString(); //电压
                            tt_Ampere = ds3.Tables[0].Rows[0].ItemArray[1].ToString(); //电流
                            tt_flag1_1 = true;
                        }
                        else
                        {
                            MessageBox.Show("没有电源适配器信息，请确认数据库电源表");
                        }
                    }

                    //第二步 MAC特征码查询
                    Boolean tt_flag2 = false;
                    if (tt_flag1_1)
                    {
                        string tt_maccheck = "";
                        string tt_sql2 = "select top(1)maclable from odc_alllable where taskscode = '" + this.textBox1.Text + "' ";
                        DataSet ds2 = Dataset1.GetDataSetTwo(tt_sql2, tt_conn);

                        if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                        {
                            tt_maccheck = ds2.Tables[0].Rows[0].ItemArray[0].ToString(); //随机取该制造单关联的一个MAC
                            this.textBox4.Text = tt_maccheck.Substring(0, 6); //获取该制造单MAC特征码
                            this.textBox5.Text = this.textBox4.Text; //重打MAC的特征获取

                            tt_flag2 = true;
                        }
                        else
                        {
                            string tt_sql2_1 = "select top(1)maclable from odc_alllable where hprintman = '" + this.textBox1.Text + "' ";
                            DataSet ds2_1 = Dataset1.GetDataSetTwo(tt_sql2_1, tt_conn);

                            if (ds2_1.Tables.Count > 0 && ds2_1.Tables[0].Rows.Count > 0)
                            {
                                tt_maccheck = ds2_1.Tables[0].Rows[0].ItemArray[0].ToString(); //随机取该制造单关联的一个MAC
                                this.textBox4.Text = tt_maccheck.Substring(0, 6); //获取该制造单MAC特征码
                                this.textBox5.Text = this.textBox4.Text; //重打MAC的特征获取

                                tt_flag2 = true;
                            }
                            else
                            {
                                MessageBox.Show("没有MAC相关信息，请确认该制造单是否有关联产品");
                            }
                        }
                    }


                    //第三步 EC表信息查询
                    Boolean tt_flag3 = false;
                    if (tt_flag2)
                    {
                        string tt_sql3 = "select  docdesc,Fpath02,Fdata02,Fmd02  from odc_ec where zjbm = '" + this.label60.Text + "' ";

                        DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
                        if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                        {
                            this.label63.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString(); //EC描述
                            this.label62.Text = ds3.Tables[0].Rows[0].ItemArray[1].ToString(); //模板路径
                            this.label61.Text = ds3.Tables[0].Rows[0].ItemArray[2].ToString(); //数据类型
                            this.label73.Text = ds3.Tables[0].Rows[0].ItemArray[3].ToString(); //MD5码
                            tt_path = Application.StartupPath + ds3.Tables[0].Rows[0].ItemArray[1].ToString();
                            tt_md5 = ds3.Tables[0].Rows[0].ItemArray[3].ToString();
                            tt_flag3 = true;

                        }
                        else
                        {
                            MessageBox.Show("没有找到工单表的EC表配置信息，请确认！");
                        }
                    }


                    //第三步 查看模板是否存在
                    Boolean tt_flag4 = false;
                    if (tt_flag3)
                    {
                        tt_flag4 = getPathIstrue(tt_path);
                        if (!tt_flag4)
                        {
                            MessageBox.Show(" 找不到模板文件：" + tt_path + "，请确认！");
                        }
                    }


                    //第四步 检验MD5码      取消MD5码校验，2017/10/6  聂江斌
                    //Boolean tt_flag5 = false;
                    //if (tt_flag4)
                    //{
                    //    string tt_md6 = GetMD5HashFromFile(tt_path);

                    //    if (tt_md5 == tt_md6)
                    //    {
                    //        tt_flag5 = true;
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("系统设定模板MD5码: '" + tt_md5 + "'与你使用模板的MD5码：'" + tt_md6 + "'不一致，请确认！");
                    //    }
                    //}


                    //最后验证
                    if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                    {
                        this.textBox1.Enabled = false;
                        this.textBox2.Visible = true;
                        this.textBox7.Visible = true;
                        GetProductNumInfo();  //生产信息

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
                this.textBox7.Visible = false;
                this.checkBox1.Checked = false;
                this.comboBox2.Text = "";
                this.textBox21.Text = "";
                this.textBox22.Text = "";
                tt_Vendorid = "";
                tt_Ampere = "";
                tt_Voltage = "";
                this.comboBox2.Enabled = true;
                this.textBox21.Enabled = true;
                this.textBox22.Enabled = true;
                this.groupBox14.Visible = false;
                this.groupBox15.Visible = false;
                this.groupBox8.Visible = true;
                this.groupBox9.Visible = true;
                this.dataGridView1.Visible = true;
                this.button3.Visible = false;
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
            this.label25.Text = tt_lableinfo;
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
            this.label15.Text = tt_yield.ToString();   //本班产量
            this.label16.Text = tt_time;               //生产时间
            this.label17.Text = tt_avgtime.ToString();  //平均节拍
            this.label18.Text = tt_differtime2;        //实时节拍

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
                this.label48.Text = tt_ccode;
                this.label49.Text = tt_ncode;
                this.label75.Text = tt_ccodenumber;
            }




            return tt_flag;
        }
        
        //获取生产信息
        private void GetProductNumInfo()
        {
            string tt_sql = "select  count(1),count(case when bprintman is not null then 1 end),0 " +
                            "from odc_alllable  where taskscode = '" + this.textBox1.Text + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label52.Text = tt_array[0];
            this.label53.Text = tt_array[1];
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
                this.label71.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //当前站位

                //获取流程的顺序值
                string tt_newcode = ds1.Tables[0].Rows[0].ItemArray[1].ToString();
                string tt_process = this.label47.Text;
                this.label74.Text = GetCodeRoutNum(tt_newcode, tt_process);
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

        //删除条码比对数据
        private int Delete_Check(string tt_mac)
        {
            string tt_deletesql = "delete from odc_check_barcode where maclable = '" + tt_mac + "'";
            int tt_Checknum = Dataset1.ExecCommand(tt_deletesql, tt_conn);
            return tt_Checknum;
        }

        #endregion


        #region 6、数据查询

        //数据查询确定
        private void button4_Click_1(object sender, EventArgs e)
        {
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;

            string tt_task = "";
            string tt_pcba = "";
            string tt_mac = "";
            Boolean tt_flag = false;

            string tt_sn1 = this.textBox8.Text.Trim();
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

        //数据查询重置
        private void button5_Click(object sender, EventArgs e)
        {

            this.textBox8.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;

        }
        #endregion


        #region 7、获取文件MD5码
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
        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描查询--------");
                string tt_task = this.textBox1.Text.Trim();
                string tt_scanmac = this.textBox7.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace("-", "");

                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox6.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1 && this.textBox5.Enabled == false)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox5.Text.Trim());
                }
                else
                {
                    setRichtexBox("2、MAC条码约束字段未锁死,：" + this.textBox5.Text + ",over");
                    PutLableInfor("MAC条码约束字段未锁死，请输入约束字段后回车！");
                }

                //第三步 判断路径
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
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
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    string tt_sql3 = "select pcbasn,hostlable,maclable,smtaskscode,bprintuser,shelllable from odc_alllable " +
                                     "where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";


                    DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
                    if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        this.label35.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();  //单板号
                        this.label36.Text = ds3.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();  //主机条码
                        this.label38.Text = ds3.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();  //短MAC
                        this.label37.Text = ds3.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();  //移动串号
                        this.label39.Text = ds3.Tables[0].Rows[0].ItemArray[4].ToString().ToUpper();  //长MAC
                        this.label40.Text = ds3.Tables[0].Rows[0].ItemArray[5].ToString().ToUpper();  //GPSN
                        tt_longmac = this.label39.Text;                      
                        
                        setRichtexBox("3、关联表查询到一条数据，goon");

                    }

                    else
                    {
                        //兼容包装产品重打
                        string tt_sql3_1 = "select pcbasn,hostlable,maclable,smtaskscode,bprintuser,shelllable from odc_alllable " +
                                     "where taskscode like '" + tt_task + "%' and maclable = '" + tt_shortmac + "' ";


                        DataSet ds3_1 = Dataset1.GetDataSet(tt_sql3_1, tt_conn);
                        if (ds3_1.Tables.Count > 0 && ds3_1.Tables[0].Rows.Count > 0)
                        {
                            tt_flag4 = true;
                            this.label35.Text = ds3_1.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();  //单板号
                            this.label36.Text = ds3_1.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();  //主机条码
                            this.label38.Text = ds3_1.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();  //短MAC
                            this.label37.Text = ds3_1.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();  //移动串号
                            this.label39.Text = ds3_1.Tables[0].Rows[0].ItemArray[4].ToString().ToUpper();  //长MAC
                            this.label40.Text = ds3_1.Tables[0].Rows[0].ItemArray[5].ToString().ToUpper();  //GPSN
                            tt_longmac = this.label39.Text;

                            setRichtexBox("3、关联表查询到一条数据，goon");

                        }

                        else
                        {
                            setRichtexBox("3、关联表没有查询到数据，over");
                            PutLableInfor("关联表没有查询到数据，请检查！");
                        }

                        //setRichtexBox("3、关联表没有查询到数据，over");
                        //PutLableInfor("关联表没有查询到数据，请检查！");
                    }
                }

                //第五步查询macinfo表信息
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    string tt_sql5 = "select ssid,username,password,Wlanpas,ssid_5G,wlanpas_5G,barcode1 from odc_macinfo " +
                                     "where taskscode = '" + tt_task + "' and mac = '" + tt_longmac + "' ";

                    DataSet ds5 = Dataset1.GetDataSet(tt_sql5, tt_conn);
                    if (ds5.Tables.Count > 0 && ds5.Tables[0].Rows.Count > 0)
                    {
                        tt_flag5 = true;
                        this.label69.Text = ds5.Tables[0].Rows[0].ItemArray[0].ToString();  //SSID
                        this.label41.Text = ds5.Tables[0].Rows[0].ItemArray[1].ToString();  //用户名
                        this.label42.Text = ds5.Tables[0].Rows[0].ItemArray[2].ToString();  //密码
                        this.label43.Text = ds5.Tables[0].Rows[0].ItemArray[3].ToString();  //wlanp
                        this.label78.Text = ds5.Tables[0].Rows[0].ItemArray[4].ToString();  //5G账号
                        this.label80.Text = ds5.Tables[0].Rows[0].ItemArray[5].ToString();  //5G密码
                        this.label83.Text = ds5.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper();  //设备标示号暗码

                        if (this.label57.Text == "河南")
                        {
                            this.label88.Text = "FIBER|" + this.label56.Text + "|" + this.label40.Text + "|" + this.label38.Text;
                        }
                        else if (this.label57.Text == "浙江" || this.label57.Text == "江西")
                        {
                            this.label88.Text = "厂家:烽火通信科技股份有限公司,型号:" + this.label56.Text + ",SN:" + this.label40.Text +
                                                ",生产日期:" + this.label59.Text.Replace("/", ".") + ",用户无线默认SSID:" + this.label69.Text +
                                                ",用户无线默认SSID密码:" + this.label43.Text + ",用户登陆默认账号:" + this.label41.Text +
                                                ",用户登陆默认密码:" + this.label42.Text + ",设备网卡MAC:" + this.label38.Text;
                        }
                        else
                        {
                            this.label88.Text = "";
                        }

                        if (this.label85.Text == "EPON")
                        {
                            this.label40.Text = Regex.Replace(this.label40.Text, "-", "");
                        }

                        setRichtexBox("5、Macinfo表找到一条数据,goon");

                    }
                    else
                    {
                        setRichtexBox("5、Macinfo表没有找到一条数据，over");
                        PutLableInfor("Macinfo表没有找到数据，请检查！");
                    }
                }                

                //第六步 查找站位信息
                Boolean tt_flag6 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                {
                    tt_flag6 = true;
                    setRichtexBox("6、查站位信息过，goon");
                }

                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {
                    GetParaDataPrint(0);

                    GetProductNumInfo();
                    CheckStation(tt_shortmac);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    setRichtexBox("6、查询完毕，可以重打标签或修改模板，over");
                    PutLableInfor("MAC查询完毕");
                    textBox7.Focus();
                    textBox7.SelectAll();

                    if (tt_reprintmark == "0")
                    {
                        this.textBox7.Enabled = false;
                    }
                }
                else if (tt_flag2 == false && this.textBox5.Enabled == true)
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox5.Focus();
                    textBox5.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox7.Focus();
                    textBox7.SelectAll();
                }                                
            }
        }

        //扫描MAC过站
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC过站扫描--------");
                string tt_task = this.textBox1.Text.Trim();
                string tt_scanmac = this.textBox2.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace("-", "");

                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox3.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1 && this.textBox4.Enabled == false)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox4.Text.Trim());
                }
                else
                {                    
                    setRichtexBox("2、MAC条码约束字段未锁死,：" + this.textBox4.Text + ",over");
                    PutLableInfor("MAC条码约束字段未锁死，请输入约束字段后回车！");
                }

                //第三步 检查模板
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
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
                    tt_flag5 = true;
                    setRichtexBox("4、物料检查过，gong");
                }

                //第六步流程检查
                Boolean tt_flag6 = false;
                string tt_gyid = this.label47.Text;
                string tt_ccode = this.label48.Text;
                string tt_ncode = this.label49.Text;
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
                string tt_smtaskscode = "";
                string tt_longmac = "";
                string tt_oldtype = "";
                string tt_id = "";
                string tt_gpsn = "";
                string tt_pcba = "";
                string tt_qrcode = "";
                string tt_taskscheck = "";
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {
                    string tt_sql7 = "select hostlable,maclable,smtaskscode,bprintuser,id,ageing,shelllable,pcbasn,taskscode from odc_alllable " +
                                     "where hprintman = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";

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
                        tt_gpsn = ds7.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper();   //GPSN
                        tt_pcba = ds7.Tables[0].Rows[0].ItemArray[7].ToString().ToUpper();   //单板号
                        tt_taskscheck = ds7.Tables[0].Rows[0].ItemArray[8].ToString().ToUpper();   //子工单判断
                        setRichtexBox("7、关联表查询到一条数据，hostlable=" + tt_hostlable + ",mac=" + tt_shortmac + ",smtaskscode=" + tt_smtaskscode + ",id=" + tt_id + ",老化ageing=" + tt_oldtype + ",goon");

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
                            setRichtexBox("8、该单板待测站位不在" + tt_ccode + "，站位：" + tt_array8[1] + "，" + tt_array8[2] + ",不可以过站 goon");
                            PutLableInfor("该单板当前站位：" + tt_array8[2] + "不在" + tt_ccode + "站位！");
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

                    string tt_sql9 = "select ssid,username,password,Wlanpas,ssid_5G,wlanpas_5G,barcode1  from odc_macinfo " +
                                     "where taskscode = '" + tt_task + "' and mac = '" + tt_longmac + "' ";

                    DataSet ds9 = Dataset1.GetDataSet(tt_sql9, tt_conn);
                    if (ds9.Tables.Count > 0 && ds9.Tables[0].Rows.Count > 0)
                    {
                        tt_flag9 = true;
                        tt_ssid = ds9.Tables[0].Rows[0].ItemArray[0].ToString();  //SSID
                        tt_macusername = ds9.Tables[0].Rows[0].ItemArray[1].ToString();  //用户名
                        tt_password = ds9.Tables[0].Rows[0].ItemArray[2].ToString();  //密码
                        tt_wlanpas = ds9.Tables[0].Rows[0].ItemArray[3].ToString();  //wlan
                        tt_5guser = ds9.Tables[0].Rows[0].ItemArray[4].ToString();  //5G账号
                        tt_5gpassword = ds9.Tables[0].Rows[0].ItemArray[5].ToString();  //5G密码
                        tt_barcode1 = ds9.Tables[0].Rows[0].ItemArray[6].ToString().ToUpper();  //设备标示号暗码


                        if (this.label57.Text == "河南")
                        {
                            tt_qrcode = "FIBER|" + this.label56.Text + "|" + tt_gpsn + "|" + tt_shortmac;
                        }
                        else if (this.label57.Text == "浙江" || this.label57.Text == "江西")
                        {
                            tt_qrcode = "厂家:烽火通信科技股份有限公司,型号:" + this.label56.Text + ",SN:" + tt_gpsn +
                                        ",生产日期:" + this.label59.Text.Replace("/", ".") + ",用户无线默认SSID:" + tt_ssid +
                                        ",用户无线默认SSID密码:" + tt_wlanpas + ",用户登陆默认账号:" + tt_macusername +
                                        ",用户登陆默认密码:" + tt_password + ",设备网卡MAC:" + tt_shortmac;
                        }
                        else
                        {
                            tt_qrcode = "";
                        }

                        if (this.label85.Text == "EPON")
                        {
                            tt_gpsn = Regex.Replace(tt_gpsn, "-", "");
                        }

                        setRichtexBox("9、Macinfo表找到一条数据，SSID=" + tt_ssid + ",username=" + tt_macusername + ",password=" + tt_password + ",wanlaps=" + tt_wlanpas + ",goon");

                    }
                    else
                    {
                        setRichtexBox("9、Macinfo表没有找到一条数据，over");
                        PutLableInfor("Macinfo表没有找到条数据，请检查！");
                    }
                }

                //第十步物料追溯添加
                Boolean tt_flag10 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9)
                {
                    tt_flag10 = true;
                    setRichtexBox("10、物料追溯记录过，gong");
                }

                //第十一步老化判断
                Boolean tt_flag11 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10)
                {
                    tt_flag11 = true;
                    setRichtexBox("10、老化判断过，gong");
                }

                //第十二步开始过站

                Boolean tt_flag12 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag10 && tt_flag11)
                {
                    if (tt_taskscheck != tt_task)
                    {
                        tt_task = tt_taskscheck;
                    }
                    string tt_username = STR;
                    tt_flag12 = Dataset1.FhYDPassStation(tt_task, tt_username, tt_shortmac, tt_gyid, tt_ccode, tt_ncode, tt_conn);
                    if (tt_flag12)
                    {
                        setRichtexBox("12、该产品过站成功，请继续扫描,ok");
                    }
                    else
                    {
                        setRichtexBox("12、过站不成功，事务已回滚");
                        PutLableInfor("过站不成功，请检查或再次扫描！");
                    }
                }

                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag10 && tt_flag11 && tt_flag12)
                {
                    //条码信息
                    this.label35.Text = tt_pcba;    //单板号
                    this.label36.Text = tt_hostlable;   //主机条码
                    this.label37.Text = tt_smtaskscode;  //移动串号
                    this.label38.Text = tt_shortmac;    //短MAC
                    this.label39.Text = tt_longmac;      //长MAC
                    this.label40.Text = tt_gpsn;         //GPSN

                    //MAC信息
                    this.label41.Text = tt_macusername;  //用户名
                    this.label42.Text = tt_password;    //密码
                    this.label43.Text = tt_wlanpas;      //WLAN
                    this.label69.Text = tt_ssid;         //SSID
                    this.label78.Text = tt_5guser;       //5G账号
                    this.label80.Text = tt_5gpassword;   //5G密码
                    this.label83.Text = tt_barcode1;     //设备标示号暗码
                    this.label88.Text = tt_qrcode;     //二维码

                    //生产节拍
                    getProductRhythm();

                    //打印记录
                    Dataset1.lablePrintRecord(tt_task, tt_shortmac, tt_hostlable, "运营商标签", str, tt_computermac, "", tt_conn);

                    //打印
                    GetParaDataPrint(1);
                    GetProductNumInfo();
                    CheckStation(tt_shortmac);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    PutLableInfor("过站成功，请继续扫描！");
                    textBox2.Focus();
                    textBox2.SelectAll();
                }
                else if (tt_flag2 == false && this.textBox4.Enabled == true)
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox4.Focus();
                    textBox4.SelectAll();
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
        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.textBox9.Text == "*963.")
                {
                    this.textBox4.Enabled = true;
                    this.textBox5.Enabled = true;
                    this.textBox9.Text = null;
                }
            }
        }

        //锁定MAC过站特征码输入框
        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bool tt_flag1 = false;

                tt_flag1 = CheckStrLengh(this.textBox4.Text, "0");
                if (tt_flag1)
                {
                    this.textBox4.Enabled = false;
                    this.textBox2.Text = "";
                    this.textBox7.Text = "";
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
                        this.textBox4.Text = tt_maccheck.Substring(0, 6); //获取该制造单MAC特征码

                        PutLableInfor("MAC锁定约束输入位数不正确，位数小于6，特征码已还原");
                        this.textBox4.Enabled = false;
                        this.textBox2.Text = "";
                        this.textBox7.Text = "";
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

        //锁定MAC过站重打输入框
        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bool tt_flag1 = false;

                tt_flag1 = CheckStrLengh(this.textBox5.Text, "6");
                if (tt_flag1)
                {
                    this.textBox5.Enabled = false;
                    this.textBox2.Text = "";
                    this.textBox7.Text = "";
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
                        this.textBox5.Text = tt_maccheck.Substring(0, 6); //获取该制造单MAC特征码

                        PutLableInfor("MAC锁定约束输入位数不正确，位数小于6，特征码已还原");
                        this.textBox5.Enabled = false;
                        this.textBox2.Text = "";
                        this.textBox7.Text = "";
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
        

        #region 9、按钮功能
        //重置
        private void button1_Click(object sender, EventArgs e)
        {
            ScanDataInitial();

            this.textBox2.Text = null;
            this.textBox7.Text = null;


            textBox2.Focus();
            textBox2.SelectAll();
        }

        //预览
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {

                string tt_prientcode = this.label74.Text;
                string tt_checkcode = this.label75.Text;

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

            textBox2.Focus();
            textBox2.SelectAll();
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
                if (str.Contains("FH002"))
                {
                    tt_info = "，包装产品会被退回check站位";
                }
                DialogResult dr = MessageBox.Show("确定要重打标签吗，打印信息被记录" + tt_info, "标签重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label74.Text;
                    string tt_checkcode = this.label75.Text;
                    string tt_recordmac = this.textBox7.Text;

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
                        string tt_host = Gethostlable(tt_recordmac);
                        string tt_taskscode = Gettasks(tt_recordmac);
                        string tt_local = "运营商标签";
                        string tt_username = "";
                        if (str.Contains("FH002"))
                        {
                            tt_username = this.comboBox2.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }

                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);

                        if (str.Contains("FH002"))
                        {
                            if (int.Parse(tt_nowcode) >= 3000)
                            {
                                string tt_gyid = this.label47.Text;
                                string tt_ccode = this.label71.Text;
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

        //线长调试模式
        private void button14_Click(object sender, EventArgs e)
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
                    this.groupBox8.Visible = false;
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
                    this.tabPage4.Parent = null;
                    this.tabPage3.Parent = tabControl2;
                    this.textBox7.Enabled = true;
                    this.textBox7.Text = "";
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
            this.groupBox8.Visible = true;
            this.groupBox9.Visible = true;
            this.dataGridView1.Visible = true;
            this.button3.Visible = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
        }

        //上移按钮
        private void button9_Click(object sender, EventArgs e)
        {
            tt_top -= float.Parse(this.comboBox1.Text);
        }

        //下移按钮
        private void button10_Click(object sender, EventArgs e)
        {
            tt_top += float.Parse(this.comboBox1.Text);
        }

        //左移按钮
        private void button11_Click(object sender, EventArgs e)
        {
            tt_left -= float.Parse(this.comboBox1.Text);
        }

        //右移按钮
        private void button12_Click(object sender, EventArgs e)
        {
            tt_left += float.Parse(this.comboBox1.Text);
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
            this.groupBox8.Visible = true;
            this.groupBox9.Visible = true;
            this.dataGridView1.Visible = true;
            this.button3.Visible = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
        }

        #endregion


        #region 10、数据采集及模板打印
        //获取参数
        private void GetParaDataPrint(int tt_itemtype)
        {
            string tt_fdata = this.label61.Text;

            //MP99---数据类型(打印模板）
            if (tt_fdata == "MP99")
            {
                GetParaDataPrint_MP99(tt_itemtype);
            }

            //MP01---数据类型一
            if (tt_fdata == "YD01")
            {
                GetParaDataPrint_YD01(tt_itemtype);
            }

            //DX01---数据类型一
            if (tt_fdata == "DX01")
            {
                GetParaDataPrint_DX01(tt_itemtype);
            }

            //EW01---数据类型一
            if (tt_fdata == "EW01")
            {
                GetParaDataPrint_EW01(tt_itemtype);
            }

            //CG01---数据类型一
            if (tt_fdata == "CG01")
            {
                GetParaDataPrint_CG01(tt_itemtype);
            }
        }

        //----以下是MP99数据采集----模板
        private void GetParaDataPrint_MP99(int tt_itemtype)
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
            row1["内容"] = this.label56.Text;
            dt.Rows.Add(row1);
            
            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "主机条码";
            row2["内容"] = this.label36.Text;
            dt.Rows.Add(row2);
            
            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "短MAC";
            row3["内容"] = this.label38.Text;
            dt.Rows.Add(row3);
            
            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "移动号码";
            row4["内容"] = this.label37.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "长MAC";
            row5["内容"] = this.label39.Text;
            dt.Rows.Add(row5);


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
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top;
                        p3.Left += tt_left;
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

        //----以下是YD01数据采集----
        private void GetParaDataPrint_YD01(int tt_itemtype)
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
            row1["内容"] = this.label56.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "设备标识";
            row2["内容"] = this.label37.Text;
            dt.Rows.Add(row2);


            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "配置账号";
            row3["内容"] = this.label41.Text;
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "配置密码";
            row4["内容"] = this.label42.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "网络名称";
            row5["内容"] = this.label69.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "网络密匙";
            row6["内容"] = this.label43.Text;
            dt.Rows.Add(row6);


            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "5G账号";
            row7["内容"] = this.label78.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "5G密码";
            row8["内容"] = this.label80.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "设备标示暗码";
            row9["内容"] = this.label83.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "PON类型";
            row10["内容"] = this.label85.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "移动服务热线";
            row11["内容"] = this.label86.Text;
            dt.Rows.Add(row11);

            DataRow row12 = dt.NewRow();
            row12["参数"] = "S12";
            row12["名称"] = "移动文字变量";
            row12["内容"] = this.label91.Text;
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "S13";
            row13["名称"] = "移动二维码";
            row13["内容"] = this.label88.Text;
            dt.Rows.Add(row13);

            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 50;
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
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top;
                        p3.Left += tt_left;
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

        //----以下是DX01数据采集----
        private void GetParaDataPrint_DX01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();
            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            string tt_ltponword = "";
            if (this.label85.Text == "GPON")
            {
                tt_ltponword = "&sn=";
            }
            else if (this.label85.Text == "EPON")
            {
                tt_ltponword = "&mac=";
            }

            string tt_LTQR = "http://op.smartont.net/app/download?ssid1=" + this.label69.Text + "&password=" + this.label43.Text +
                             "&username=" + this.label41.Text + "&pwd=" + this.label42.Text + "&model=" + this.label85.Text +
                             "&type=" + this.label56.Text + tt_ltponword + this.label40.Text + "&serialnumber=" + this.label83.Text +
                             "&ip=192.168.1.1";

            string tt_XJDZ = "";
            if ((this.label56.Text == "HG6201T" || this.label56.Text == "HG2201T") && this.label57.Text == "新疆")
            {
                tt_XJDZ = "此终端所有权归新疆电信公司所有";
            }

            string tt_HNDZ = "";
            if ((this.label56.Text == "HG6201T" || this.label56.Text == "HG2201T") && this.label57.Text == "海南")
            {
                tt_HNDZ = "　双协议\r\n防雷4KV";
            }

            DataRow row1 = dt.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "设备型号";
            row1["内容"] = this.label56.Text;
            dt.Rows.Add(row1);

            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "设备标识";
            row2["内容"] = this.label37.Text;
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "配置账号";
            row3["内容"] = this.label41.Text;
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "配置密码";
            row4["内容"] = this.label42.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "网络名称";
            row5["内容"] = this.label69.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "网络密匙";
            row6["内容"] = this.label43.Text;
            dt.Rows.Add(row6);
            
            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "5G账号";
            row7["内容"] = this.label78.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "5G密码";
            row8["内容"] = this.label80.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "设备标示暗码";
            row9["内容"] = this.label83.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "PON类型";
            row10["内容"] = this.label85.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "SN";
            row11["内容"] = this.label40.Text;
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
            row15["内容"] = tt_NetType;
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "S16";
            row16["名称"] = "电流";
            row16["内容"] = tt_Ampere;
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "S17";
            row17["名称"] = "电压";
            row17["内容"] = tt_Voltage;
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "S18";
            row18["名称"] = "COMITID";
            row18["内容"] = tt_Vendorid;
            dt.Rows.Add(row18);

            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 50;
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
                report.SetParameterValue("S15", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("S16", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("S17", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("S18", dst.Tables[0].Rows[17][2].ToString());

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
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top;
                        p3.Left += tt_left;
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

        //----以下是EW01数据采集----
        private void GetParaDataPrint_EW01(int tt_itemtype)
        {
            //第一步数据准备

            //数据收集

            string tt_http = "http://download.189cube.com/clientdownload?ssid1="; //IP地址
            string tt_username = this.label69.Text; //默认无线网络名称
            string tt_wifipassword = this.label43.Text; //默认无线网络密匙
            string tt_configpassword = this.label42.Text; //默认终端配置密码
            string tt_productname = this.label56.Text; //设备型号
            string tt_productmark = this.label37.Text; //设备标示

            string tt_twodimcode = tt_http + tt_username + "&password=" + tt_wifipassword + "&useradminpw="
                                 + tt_configpassword + "&model=" + tt_productname + "&sn=" + tt_productmark;


            DataSet dst = new DataSet();
            DataTable dt = new DataTable();
            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");


            DataRow row1 = dt.NewRow();
            row1["参数"] = "S01";
            row1["名称"] = "二维码";
            row1["内容"] = tt_twodimcode;
            dt.Rows.Add(row1);


            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 50;
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
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top;
                        p3.Left += tt_left;
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

        //----以下是CG01数据采集----
        private void GetParaDataPrint_CG01(int tt_itemtype)
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
            row1["内容"] = this.label56.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "设备标识";
            row2["内容"] = this.label37.Text;
            dt.Rows.Add(row2);


            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "配置账号";
            row3["内容"] = this.label41.Text;
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "配置密码";
            row4["内容"] = this.label42.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "网络名称";
            row5["内容"] = this.label69.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "网络密匙";
            row6["内容"] = this.label43.Text;
            dt.Rows.Add(row6);


            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "5G账号";
            row7["内容"] = this.label78.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "S08";
            row8["名称"] = "5G密码";
            row8["内容"] = this.label80.Text;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "S09";
            row9["名称"] = "设备标示暗码";
            row9["内容"] = this.label83.Text;
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "S10";
            row10["名称"] = "PON类型";
            row10["内容"] = this.label85.Text;
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "S11";
            row11["名称"] = "移动服务热线";
            row11["内容"] = this.label86.Text;
            dt.Rows.Add(row11);

            DataRow row12 = dt.NewRow();
            row12["参数"] = "S12";
            row12["名称"] = "移动文字变量";
            row12["内容"] = this.label91.Text;
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "S13";
            row13["名称"] = "移动二维码";
            row13["内容"] = this.label88.Text;
            dt.Rows.Add(row13);

            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 50;
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
                    s = string.Format("Picture{0}", i + 1);
                    PictureObject p3 = report.FindObject(s) as PictureObject;
                    if (p3 != null)
                    {
                        p3.Top += tt_top;
                        p3.Left += tt_left;
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

        #endregion

        

    }
}
