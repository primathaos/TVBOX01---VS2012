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
    public partial class Form25_amb : Form
    {
        public Form25_amb()
        {
            InitializeComponent();
        }

        //-----------延迟制造单板临时标签打印-----------------

        #region 1、属性设置
        static string tt_conn;
        static int tt_yield = 0;
        static int tt_reprinttime = 0; //重打次数
        static string tt_setcode = "0000";
        static string tt_pcname = System.Net.Dns.GetHostName();
        static string tt_path = Application.StartupPath + @"\lable\100\type1.frx";
        //全流程检验
        static string tt_allprocesses = null;
        static string tt_partprocesses = null;
        static DataSet tt_routdataset = null;
        static DataSet tt_allroutdataset = null;
        //物料追溯
        int tt_uplip = 0;  //上盖数量
        int tt_downlip = 0; //下盖数量
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
        private void Form25_amb_Load(object sender, EventArgs e)
        {
            //FastReport环境变量设置（打印时不提示 "正在准备../正在打印..",一个程序只需设定一次，故一般写在程序入口）
            (new FastReport.EnvironmentSettings()).ReportSettings.ShowProgress = false;

            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";
            this.toolStripStatusLabel6.Text = tt_productstarttime.ToString();
            this.toolStripStatusLabel10.Text = tt_reprinttime.ToString();

            //初始不显示身份验证栏
            this.groupBox15.Visible = false;

            //初始不显示微调栏
            this.groupBox14.Visible = false;

            //隐藏线长调试按钮
            this.button14.Visible = false;

            //员工账号分离
            if (str.Contains("FH009"))
            {
                this.button2.Visible = false;
                this.button3.Visible = false;
                this.tabPage4.Parent = null;
                this.button14.Visible = true;
            }

            //页面信息清理
            ClearLabelInfo_Takscode();
            ClearLabelInfo_Code();
            ClearLabelInfo_Barcode();
            ClearLabelInfo_Material();
            ClearLabelInfo_Yield();
            ClearLabelInfo_Textbox();

            //扫描框隐藏
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

        //清除工单信息
        private void ClearLabelInfo_Takscode()
        {
            this.label27.Text = null;
            this.label29.Text = null;
            this.label30.Text = null;
            this.label33.Text = null;
            this.label49.Text = null;
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

        //生产节拍数据初始化
        private void ClearLabelInfo_Yield()
        {
            this.label7.Text = tt_yield.ToString();
            this.label10.Text = null;
            this.label8.Text = null;
            this.label9.Text = null;
        }

        //条码信息清除
        private void ClearLabelInfo_Barcode()
        {
            //条码信息
            this.label39.Text = null;
            this.label41.Text = null;

        }

        //物料信息清除
        private void ClearLabelInfo_Material()
        {
            this.label55.Text = null;
            this.label56.Text = null;
        }


        //提示信息清除
        private void ClearLabelInfo_Textbox()
        {
            //提示信息
            this.label12.Text = null;
            this.textBox2.Text = null;
            this.textBox3.Text = null;

        }


        //扫描前数据初始化
        private void ScanDataInitial()
        {

            //提示信息
            this.label12.Text = null;

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



        #endregion


        #region 4、锁定事件
        //工单选择
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                if (str.Contains("FH109"))
                {
                    this.button3.Visible = true;
                    this.tabPage4.Parent = tabControl2;
                    //获取调试开始时间
                    tt_reprintstattime = DateTime.Now;
                }

                Boolean tt_flag = getChoiceTaskcode();
                if (tt_flag)
                {
                    MessageBox.Show("---OK---,这是延迟制造模式，打印单板临时标签，请确认产品型号，不要选错工单");
                    this.textBox1.Enabled = false;
                    this.textBox2.Visible = true;
                    this.textBox3.Visible = true;
                }
                else
                {
                    MessageBox.Show("工单选择失败");
                    ClearLabelInfo_Takscode();
                    ClearLabelInfo_Code();
                    ClearLabelInfo_Barcode();
                    ClearLabelInfo_Textbox();
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
                this.dataGridView1.Visible = true;
                this.button3.Visible = false;
                this.tabPage4.Parent = null;
                this.tabPage3.Parent = tabControl2;

                ClearLabelInfo_Takscode();
                ClearLabelInfo_Code();
                ClearLabelInfo_Barcode();
                ClearLabelInfo_Textbox();
            }

        }

        //单板过站位数锁定
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

        //单板重打位数锁定
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


        //物料锁定
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked)
            {
                this.textBox8.Enabled = false;
                this.textBox9.Enabled = false;
                this.textBox10.Enabled = false;
            }
            else
            {
                this.textBox8.Enabled = true;
                this.textBox9.Enabled = true;
                this.textBox10.Enabled = true;
            }
        }
        #endregion


        #region 5、工单选择及站位检查

        //工单选择
        private bool getChoiceTaskcode()
        {
            Boolean tt_flag = false;
            string tt_task = this.textBox1.Text.Trim().ToUpper();

            string tt_productname = "";
            tt_computermac = Dataset1.GetHostIpName();


            //第一步 主工单检查
            #region
            bool tt_flag1 = false;
            string tt_sql1 = "select  tasksquantity,product_name,areacode,Gyid,Tasktype " +
                                 "from odc_tasks where  taskscode = '" + tt_task + "' ";
            DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);
            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                tt_flag1 = true;
                this.label27.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                tt_productname = ds1.Tables[0].Rows[0].ItemArray[1].ToString(); //产品名称
                this.label30.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区
                this.label79.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString();  //流程配置
                this.label49.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //物料编码
                this.label33.Text = tt_path;

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
                MessageBox.Show(this.textBox1.Text + ",没有查询此工单，请确认是否正确！");
            }
            #endregion


            //第二步 流程检查
            #region
            bool tt_flag2 = false;
            string tt_gyid = this.label79.Text;
            if (!tt_gyid.Equals(""))
            {
                bool tt_codeflag = GetNextCode(this.textBox1.Text, str);
                if (tt_codeflag)
                {
                    tt_flag2 = true;
                }
            }
            else
            {
                MessageBox.Show("该工单没有配置流程，请检查");
            }
            #endregion


            //第三步模板检查
            #region
            bool tt_flag3 = false;
            if (tt_flag2)
            {
                tt_flag3 = getPathIstrue(tt_path);
                if (!tt_flag3)
                {
                    MessageBox.Show(" 找不到模板文件：" + tt_path + "，请确认！");
                }
            }
            #endregion


            //第四步 待测站位及序列号检查
            #region
            bool tt_flag4 = false;
            string tt_testcode = this.label76.Text;
            string tt_codeserial = this.label66.Text;
            if (tt_flag3)
            {
                if (tt_testcode.Equals("") || tt_codeserial.Equals(""))
                {
                    MessageBox.Show("流程的待测站位，或流程的序列号为空，请检查流程设置");
                }
                else
                {
                    tt_flag4 = true;
                    this.label13.Text =  tt_testcode;
                }

            }
            #endregion


            //第五步 获取站位流程集
            #region
            bool tt_flag5 = false;
            if (tt_flag4)
            {
                string tt_sql14 = "select pxid from odc_routing  where pid = " + tt_gyid + "  and LCBZ > 1 and LCBZ < '" + tt_codeserial + "' ";
                tt_routdataset = Dataset1.GetDataSetTwo(tt_sql14, tt_conn);
                if (tt_routdataset.Tables.Count > 0 && tt_routdataset.Tables[0].Rows.Count > 0)
                {
                    tt_flag5 = true;
                    tt_allprocesses = getGyidAllProcess(tt_gyid);
                    tt_partprocesses = getGyidPartProcess(tt_routdataset);
                    tt_allroutdataset = getGyidAllProcessDt(tt_gyid);
                }
                else
                {
                    MessageBox.Show("没有找到流程:" + tt_gyid + "，的流程数据集Dataset，请流程设置！");
                }


            }
            #endregion

            
            //最后判断
            #region
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
            {
                tt_flag = true;
            }
            #endregion


            return tt_flag;
        }


        //获取工单全部流程
        private string getGyidAllProcess(string tt_gyid)
        {
            string tt_gyidprocess = "单板工单流程没有找到";
            string tt_sql = "select count(1),min(process),0 from odc_process where id = " + tt_gyid;
            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            if (tt_array[0] == "1") tt_gyidprocess = tt_array[1];
            return tt_gyidprocess;
        }


        //获取全流程序列号
        private DataSet getGyidAllProcessDt(string tt_gyid)
        {
            DataSet tt_dt = null;
            string tt_sql = "select pxid,lcbz from odc_routing  where pid = " + tt_gyid ;
            tt_dt = Dataset1.GetDataSetTwo(tt_sql, tt_conn);
            if (tt_dt.Tables.Count > 0 && tt_dt.Tables[0].Rows.Count > 0)
            {

            }
            else
            {
                 MessageBox.Show("没有找到流程:" + tt_gyid + "，的流程数据集Dataset，请流程设置！");
            }

            return tt_dt;
        }




        //获取工单要检查的流程
        private string getGyidPartProcess(DataSet tt_checkcodedt)
        {
            string tt_parrtprocess = "部分流程无法获取";

            if (tt_checkcodedt.Tables.Count > 0 && tt_checkcodedt.Tables[0].Rows.Count > 0)
            {
                string tt_routingncode = "";
                string tt_partcheckcode = "";
                for (int i = 0; i < tt_checkcodedt.Tables[0].Rows.Count; i++)
                {
                    tt_routingncode = tt_checkcodedt.Tables[0].Rows[i][0].ToString();
                    tt_partcheckcode = tt_partcheckcode + "," + tt_routingncode;
                }
                tt_parrtprocess = tt_partcheckcode;
            }
            else
            {
                MessageBox.Show("无法获取到流程检验的部分流程");
            }


            return tt_parrtprocess;
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
                for (int i = 0; i < tt_dt.Tables[0].Rows.Count; i++)
                {
                    tt_ncode = tt_dt.Tables[0].Rows[i][2].ToString();
                    tt_napplytype = tt_dt.Tables[0].Rows[i][3].ToString();

                    if (tt_napplytype.Equals(""))
                    {
                        tt_napplycount++;
                        tt_nowcode = tt_ncode;
                    }
                }
                //以下返回值判断
                if (tt_napplycount == 0) tt_returnncode = "0";
                if (tt_napplycount == 1) tt_returnncode = tt_nowcode;
                if (tt_napplycount > 1) tt_returnncode = "2";
            }


            return tt_returnncode;
        }




        //工单设定流程每个站位检查
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
                    if (tt_checkinfo == tt_routingncode) break;
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



        //工单设定流程每个站位检查 要对1920以上站位进行检验
        private string getPcbaAllCheck2(DataSet tt_routdt, DataSet tt_codedt, int tt_intcode)
        {
            string tt_outmessage = "0";  //数据集有问题

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0 && tt_routdt.Tables.Count > 0 && tt_routdt.Tables[0].Rows.Count > 0)
            {
                string tt_routingncode = "";
                string tt_checkinfo = "0";
                //以下数据遍历 for循环
                for (int i = 0; i < tt_routdt.Tables[0].Rows.Count; i++)
                {
                    tt_routingncode = tt_routdt.Tables[0].Rows[i][0].ToString();
                    //没有找到就返回站位，找到返回1
                    tt_checkinfo = getPcbaSinglCheck2(tt_routingncode, tt_codedt,tt_intcode);
                    if (tt_checkinfo == tt_routingncode) break;
                }
                //以上数据遍历

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


        //MAC单板数据集的循环检查
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
                    tt_ncode = tt_dt.Tables[0].Rows[i][2].ToString();
                    tt_napplytype = tt_dt.Tables[0].Rows[i][3].ToString();
                    if ((tt_napplytype.Equals("1") || tt_napplytype.Equals("")) && tt_ncode == tt_checkcode)
                    {
                        tt_checkinfo = "1";
                        break;
                    }
                }

            }

            return tt_checkinfo;
        }


        //MAC单板数据集的循环检查
        private string getPcbaSinglCheck2(string tt_checkcode, DataSet tt_codedt, int tt_intcode)
        {
            string tt_checkinfo = tt_checkcode;  //没有找到就返回站位，找到返回1

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_ncode = "";
                string tt_napplytype = "";
                int tt_introwid = 0;
                //以下数据遍历
                for (int i = 0; i < tt_codedt.Tables[0].Rows.Count; i++)
                {
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    if (tt_introwid >= tt_intcode)
                    {
                        tt_ncode = tt_codedt.Tables[0].Rows[i][2].ToString();
                        tt_napplytype = tt_codedt.Tables[0].Rows[i][3].ToString();
                        if ((tt_napplytype.Equals("1") || tt_napplytype.Equals("")) && tt_ncode == tt_checkcode)
                        {
                            tt_checkinfo = "1";
                            break;
                        }
                    }
                }

            }

            return tt_checkinfo;
        }


        //检查站位顺序以及获取1920最大值
        private int getFirstCodeId(DataSet tt_dt)
        {
            int tt_introwid = -10;

            //第一步检验数据是否有数据
            #region
            bool tt_flag1 = false;
            if (tt_dt.Tables.Count > 0 && tt_dt.Tables[0].Rows.Count > 0)
            {
                tt_flag1 = true;
            }
            else
            {
                tt_introwid = 0;
                MessageBox.Show("检查1920站位数据集，发现数据集为空");
            }
            #endregion



            //第二步 检查数据集是否按顺序排序
            #region
            bool tt_flag2 = false;
            if (tt_flag1)
            {
                int tt_intid1= 0;
                int tt_intid2 = 0;
                int tt_intallcount = tt_dt.Tables[0].Rows.Count;
                //以下for循环
                #region
                for (int i = 0; i < tt_intallcount; i++)
                {
                    if (tt_intallcount == 1)
                    {
                        tt_flag2 = true;
                    }
                    else
                    {
                        if(i>0)
                        {
                            tt_intid1 = getTransmitStrToInt(tt_dt.Tables[0].Rows[i][0].ToString());
                            tt_intid2 = getTransmitStrToInt(tt_dt.Tables[0].Rows[i - 1][0].ToString());
                            tt_flag2 = true;
                            if (tt_intid1 < tt_intid2)
                            {
                                tt_flag2 = false;
                                tt_introwid = -1;
                                MessageBox.Show("检查数据集顺序，发现不是按顺序排序，ID号:" + tt_intid1.ToString());
                                break;
                                
                            }
                        }
                        
                    }

                }
                #endregion
                //以上for循环

            }
            #endregion



            //第三步 查找1920最大值
            if (tt_flag2)
            {
                int tt_inteveryid = 0;
                int tt_intendid = 0;
                string tt_nowvode = "";
                //以下for循环
                #region
                for (int i = 0; i < tt_dt.Tables[0].Rows.Count; i++)
                {
                    tt_inteveryid = getTransmitStrToInt(tt_dt.Tables[0].Rows[i][0].ToString());
                    tt_nowvode = tt_dt.Tables[0].Rows[i][1].ToString();

                    if (tt_nowvode == "1920")
                    {
                        if (tt_inteveryid > tt_intendid)  tt_intendid = tt_inteveryid;
                    }


                }
                #endregion
                //以上for循环

                if (tt_intendid == 0)
                {
                    tt_introwid = -2;
                }
                else
                {
                    tt_introwid = tt_intendid;
                }

            }



            return tt_introwid;
        }


        //3350站位跳出检查
        private string getMaintainJumpCheck(DataSet tt_codedt, int tt_intcode)
        {
            string tt_outmessage = "0";

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_ncode = "";
                string tt_remark = "";
                int tt_introwid = 0;
                string tt_checkinfo = "0";
                 //以下for循环
                #region
                for (int i = 0; i < tt_codedt.Tables[0].Rows.Count; i++)
                {
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    if (tt_introwid >= tt_intcode)
                    {
                        tt_ncode = tt_codedt.Tables[0].Rows[i][2].ToString();      
                        tt_remark = tt_codedt.Tables[0].Rows[i][4].ToString();

                        if (tt_ncode.Equals("3350") && !tt_remark.Equals("PR001站位跳转"))
                        {
                            tt_checkinfo = "3350跳出检查Fail:站位" + tt_ncode + ",ID=" + tt_introwid.ToString();
                            break;
                        }
                    }
                }
                #endregion
                //以上for循环
                if (tt_checkinfo.Equals("0"))
                {
                    tt_outmessage = "1";
                }
                else
                {
                    tt_outmessage = tt_checkinfo;
                }

            }
            else
            {
                tt_outmessage = "350站位跳出检查数据集为空！";
                MessageBox.Show("过站全顺序检查数据集为空！");
            }

            return tt_outmessage;
        }




        //过站全顺序检查
        private string  getCodeSerialCheck(DataSet tt_codedt, int tt_intcode)
        {
            string tt_outmessage = "0";

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_code1 = "";
                string tt_code2 = "";
                int tt_introwid = 0;
                string tt_checkinfo = "0";
                //以下for循环
                #region
                for (int i = 0; i < tt_codedt.Tables[0].Rows.Count; i++)
                {
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    if (tt_introwid > tt_intcode)
                    {
                        tt_code1 = tt_codedt.Tables[0].Rows[i][1].ToString();  //当前记录前一站位
                        tt_code2 = tt_codedt.Tables[0].Rows[i-1][2].ToString();  //上一记录测试站位
                        if (!tt_code1.Equals(tt_code2))
                        {
                            tt_checkinfo = "顺序检查Fail:前站位" + tt_code1 + ",ID=" + tt_introwid.ToString();
                            break;
                        }

                    }


                }
                #endregion
                //以上for循环

                if (tt_checkinfo.Equals("0"))
                {
                    tt_outmessage = "1";
                }
                else
                {
                    tt_outmessage = tt_checkinfo;
                }
            }
            else
            {
                tt_outmessage = "过站全顺序检查数据集为空！";
                MessageBox.Show("过站全顺序检查数据集为空！");
            }


            return tt_outmessage;
        }



        //前后站位关系检查 字符串匹配
        private string getNearCodeCheck(DataSet tt_codedt, int tt_intcode,string tt_allprocess)
        {
            string tt_outmessage = "0";

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_code1 = "";
                string tt_code2 = "";
                string tt_code3 = "";
                int tt_introwid = 0;
                string tt_checkinfo = "0";
                //以下for循环
                #region
                for (int i = 0; i < tt_codedt.Tables[0].Rows.Count; i++)
                {
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    if (tt_introwid >= tt_intcode)
                    {
                        tt_code1 = tt_codedt.Tables[0].Rows[i][1].ToString();  
                        tt_code2 = tt_codedt.Tables[0].Rows[i][2].ToString();
                        tt_code3 = tt_code1 + "、" + tt_code2;

                        if (tt_code1.Equals("3350") || tt_code2.Equals("3350"))
                        {
                        }
                        else
                        {
                            if (!tt_allprocess.Contains(tt_code3))
                            {
                                tt_checkinfo = "前后项检查Fail:前后站位" + tt_code3 + ",ID=" + tt_introwid.ToString();
                                break;
                            }
                        }

                    }


                }
                #endregion
                if (tt_checkinfo.Equals("0"))
                {
                    tt_outmessage = "1";
                }
                else
                {
                    tt_outmessage = tt_checkinfo;
                }
            }
            else
            {
                tt_outmessage = "过站前后站位检查数据集为空！";
                MessageBox.Show("过站前后站位检查数据集为空！");
            }

            return tt_outmessage;
        }



        //前后站位关系检查 序列号检验
        private string getNearCodeCheck2(DataSet tt_codedt, int tt_intcode, DataSet tt_dtallprocess)
        {
            string tt_outmessage = "0";

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_code1 = "";
                string tt_code2 = "";
                int tt_intcode1 = 0;
                int tt_intcode2 = 0;
                int tt_introwid = 0;
                string tt_checkinfo = "0";
                //以下for循环
                #region
                for (int i = 0; i < tt_codedt.Tables[0].Rows.Count; i++)
                {
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    if (tt_introwid >= tt_intcode)
                    {
                        tt_code1 = tt_codedt.Tables[0].Rows[i][1].ToString();
                        tt_code2 = tt_codedt.Tables[0].Rows[i][2].ToString();

                        if (tt_code1.Equals("3350") || tt_code2.Equals("3350"))
                        {
                        }
                        else
                        {
                            tt_intcode1 = getRoutCodeDerialNo(tt_dtallprocess, tt_code1);
                            tt_intcode2 = getRoutCodeDerialNo(tt_dtallprocess, tt_code2);
                            if ((tt_intcode2 - tt_intcode1 == 1) || tt_intcode2 <= tt_intcode1)
                            {
                                
                            }
                            else
                            {
                                tt_checkinfo = "前后项检查Fail:前后站位" + tt_code1 + "/"+tt_code2+","+tt_intcode1.ToString()+"/"+tt_intcode2.ToString()+",ID=" + tt_introwid.ToString();
                                break;
                            }
                        }

                    }


                }
                #endregion
                if (tt_checkinfo.Equals("0"))
                {
                    tt_outmessage = "1";
                }
                else
                {
                    tt_outmessage = tt_checkinfo;
                }
            }
            else
            {
                tt_outmessage = "过站前后站位检查数据集为空！";
                MessageBox.Show("过站前后站位检查数据集为空！");
            }

            return tt_outmessage;
        }

        //获取站位序列号
        private int getRoutCodeDerialNo(DataSet tt_dt,string tt_code)
        {
            int tt_intcode = 0;
            if (tt_dt.Tables.Count > 0 && tt_dt.Tables[0].Rows.Count > 0)
            {
                int tt_introwserin = 0;
                string tt_rowcode = "0";
                //以下for循环
                #region
                for (int i = 0; i < tt_dt.Tables[0].Rows.Count; i++)
                {
                    tt_introwserin = getTransmitStrToInt(tt_dt.Tables[0].Rows[i][1].ToString());
                    tt_rowcode = tt_dt.Tables[0].Rows[i][0].ToString();
                    if (tt_rowcode == tt_code)
                    {
                        tt_intcode = tt_introwserin;
                        break;
                    }
                }
                #endregion

            }
            else
            {
                MessageBox.Show("获取站位顺序号Fail,数据集为空");
            }


            return tt_intcode;
        }

        #endregion


        #region 6、数据功能

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

        //刷新站位2
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
                    tt_setcode = tt_ccode;

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

        //检查MAC或单板，获取工单
        private string getSnRealTask(string tt_datatype, string tt_sn)
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

        //获取单板MAC
        private string Getmaclable(string tt_pcbasn)
        {
            string tt_hostlable = "";

            string tt_sql = "select count(1), min(hostlable), min(maclable) " +
                            "from odc_alllable where pcbasn = '" + tt_pcbasn + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArrayTwo(tt_sql, tt_conn);

            if (tt_array[0] == "1")
            {
                tt_hostlable = tt_array[1];
            }
            else
            {
                MessageBox.Show("网络连接失败，或此单板" + tt_pcbasn + "未关联，请确认");
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


        #region 7、数据查询
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
                string tt_sql2 = "select id ID,ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime 进站时间, enddate 出站时间, fremark 备注  " +
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

        
        #region 8、工单查询
        //工单查询确定
        private void button10_Click(object sender, EventArgs e)
        {
            this.dataGridView6.DataSource = null;

            string tt_task = this.textBox12.Text.Trim();


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

        //工单查询重置
        private void button9_Click(object sender, EventArgs e)
        {
            this.textBox12.Text = null;
            this.dataGridView6.DataSource = null;
        }

        //显示行号
        private void dataGridView6_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SolidBrush brushOne = new SolidBrush(Color.Red);
            e.Graphics.DrawString(Convert.ToString(e.RowIndex + 1, System.Globalization.CultureInfo.CurrentUICulture), e.InheritedRowStyle.Font, brushOne, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
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

        #endregion


        #region 10、功能按钮
        //重置
        private void button1_Click(object sender, EventArgs e)
        {
            ScanDataInitial();
            ClearLabelInfo_Barcode();
            this.textBox2.Text = null;
            this.textBox3.Text = null;
            textBox2.Focus();
            textBox2.SelectAll();
            setTaskcodeList();
        }

        //页签选择
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //单板扫描
            if (tabControl2.SelectedTab == tabPage3)
            {
                ScanDataInitial();
                ClearLabelInfo_Barcode();
                this.textBox2.Text = null;
                textBox2.Focus();
                textBox2.SelectAll();
            }

            //标签重打
            if (tabControl2.SelectedTab == tabPage4)
            {
                ScanDataInitial();
                ClearLabelInfo_Barcode();
                this.textBox3.Text = null;
                textBox3.Focus();
                textBox3.SelectAll();
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
                DialogResult dr = MessageBox.Show("确定要重打铭牌吗，打印信息被记录", "铭牌重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label65.Text;
                    string tt_checkcode = this.label66.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                    if (tt_flag)
                    {
					    Reprint form1 = new Reprint();
                        form1.StartPosition = FormStartPosition.CenterScreen;
                        form1.ShowDialog();

                        string tt_remark = Dataset1.Context.ContextData["Key1"].ToString();
						
                        GetParaDataPrint(1);  //打印
                        string tt_taskscode = this.textBox1.Text.Trim().ToUpper();
                        string tt_pcbasn = this.textBox3.Text;
                        string tt_host = Getmaclable(tt_pcbasn);
                        string tt_local = "临时标签";
                        string tt_username = "";
                        if (str.Contains("FH009"))
                        {
                            tt_username = this.comboBox2.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }
                        SetPrintRecord(tt_taskscode, tt_pcbasn, tt_host, tt_local, tt_username, tt_computermac ,tt_remark);

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
                        MessageBox.Show("当前站位或序号：" + tt_prientcode + "必须大于待测站位或序号：" + tt_checkcode + ",才能重打标签");
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
                    this.comboBox1.Text = "0.3";
                    this.comboBox2.Text = "下拉选择";
                    comboBox2.DataSource = ds1.Tables[0];
                    comboBox2.DisplayMember = "fusername";
                    this.groupBox14.Visible = true;
                    this.groupBox12.Visible = false;
                    this.dataGridView1.Visible = false;
                    this.textBox21.Text = "";
                    this.textBox22.Text = "";
                    this.comboBox2.Enabled = true;
                    this.textBox21.Enabled = true;
                    this.textBox22.Enabled = true;
                    this.groupBox15.Visible = false;
                    this.button3.Visible = false;
                    this.tabPage4.Parent = null;
                    this.tabPage3.Parent = tabControl2;
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
            this.groupBox12.Visible = true;
            this.dataGridView1.Visible = true;
            this.button3.Visible = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
        }

        //上移按钮
        private void button7_Click(object sender, EventArgs e)
        {
            tt_top -= float.Parse(this.comboBox1.Text);
        }

        //下移按钮
        private void button6_Click(object sender, EventArgs e)
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
            this.groupBox12.Visible = true;
            this.dataGridView1.Visible = true;
            this.button3.Visible = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
        }


        #endregion


        #region 11、扫描事件
        //扫描单板重打标签
        private void tabControl2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---------以下MAC扫描-------
                #region
                ScanDataInitial();
                ClearLabelInfo_Barcode();
                setRichtexBox("-----开始单板扫描重打标签--------");
                string tt_taskcode = this.textBox1.Text.Trim().ToUpper();
                string tt_scanpcba = this.textBox3.Text.Trim().ToUpper();
                #endregion

                //第一步位数判断
                #region
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanpcba, this.textBox7.Text);
                #endregion


                //第二步包含符判断
                #region
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanpcba, this.textBox6.Text.Trim());
                }
                #endregion


                //第三步 判断路径
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


                //第四步查找信息
                #region
                Boolean tt_flag4 = false;
                string tt_shortmac = "";
                if (tt_flag3)
                {
                    string tt_sql3 = "select pcbasn,maclable  from odc_alllable " +
                                     "where taskscode = '" + tt_taskcode + "' and pcbasn = '" + tt_scanpcba + "' ";


                    DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
                    if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        this.label39.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString();  //单板号
                        this.label41.Text = ds3.Tables[0].Rows[0].ItemArray[1].ToString();  //MAC
                        tt_shortmac = this.label41.Text;
                        setRichtexBox("4、在工单1:" + tt_taskcode + "的关联表查询到一条数据，goon");

                    }
                    else
                    {
                        string tt_querytask = getSnRealTask("1", tt_scanpcba);
                        setRichtexBox("4、在工单:" + tt_taskcode + "的关联表中没有查询到数据，该MAC的工单是" + tt_querytask + ",over");
                        PutLableInfor("该单板的工单为:" + tt_querytask + ",与工单:" + tt_taskcode + "不符");
                    }

                }
                #endregion


                //第五步查询macinfo表信息
                #region
                Boolean tt_flag5 = false;
                if (tt_flag4)
                {
                    tt_flag5 = true;
                    setRichtexBox("5、现在需求不需要查找Macinfo表信息，以后再说了，goon");
                }
                #endregion


                //最后判断
                #region
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
                #endregion



                //生产节拍
                getProductRhythm("0");
                textBox3.Focus();
                textBox3.SelectAll();
                //---------以上MAC扫描-------

            }
        }


        //扫描单板过站
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //-----以下PCBA过站打印扫描------
                #region 数据初始化
                ScanDataInitial();
                ClearLabelInfo_Barcode();
                setRichtexBox("-----开始单板过站扫描--------");
                string tt_scanpcba = this.textBox2.Text.Trim().ToUpper();
                string tt_taskcode = this.textBox1.Text.Trim().ToUpper(); //主工单
                string tt_uplips = this.textBox8.Text.Trim();
                string tt_downlips = this.textBox9.Text.Trim();
                string tt_tin = this.textBox10.Text.Trim();
                string tt_rowid = "0";
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
                #endregion

                
                //第五步物料检查
                #region
                Boolean tt_flag5 = false;
                if (tt_flag4)
                {
                    if (tt_uplips.Equals("") || tt_downlips.Equals("") || tt_tin.Equals(""))
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
                #endregion
                

                //第六步 物料追溯预留一
                #region
                Boolean tt_flag6 = false;
                if (tt_flag5)
                {
                    tt_flag6 = true;
                    setRichtexBox("6、物料追溯预留一,goon");
                }
                #endregion


                //第七步 物料追溯预留二
                #region
                Boolean tt_flag7 = false;
                if (tt_flag6)
                {
                    tt_flag7 = true;
                    setRichtexBox("7、物料追溯预留二,goon");
                }
                #endregion


                //第八步 物料追溯预留三
                #region
                Boolean tt_flag8 = false;
                if (tt_flag7)
                {
                    tt_flag8 = true;
                    setRichtexBox("8、物料追溯预留三,goon");
                }
                #endregion
                

                //第九步 流程检查
                #region
                Boolean tt_flag9 = false;
                string tt_gyid = this.label79.Text;
                string tt_ccode = this.label76.Text;
                string tt_ncode = this.label77.Text;
                if (tt_flag5)
                {
                    if (tt_ccode == "" || tt_ncode == "")
                    {
                        setRichtexBox("9、该工单没有好配置流程,待测站位：" + tt_ccode + ",进站站位：" + tt_ncode + ",over");
                        PutLableInfor("没有获取到当前待测站位，及下一站位，请检查");
                    }
                    else
                    {
                        tt_flag9 = true;
                        setRichtexBox("9、该工单已配置流程,待测站位：" + tt_ccode + ",进站站位：" + tt_ncode + ",goon");
                    }

                }
                #endregion


                //第十步 是否是维修板检查
                #region
                Boolean tt_flag10 = false;
                if (tt_flag9)
                {
                    string tt_sql10 = "select count(1),0,0  from repair  " +
                                      "where  Fpcba = '" + tt_scanpcba + "' and Type = 1 ";
                    string[] tt_array10 = new string[3];
                    tt_array10 = Dataset1.GetDatasetArray(tt_sql10, tt_conn);
                    if (tt_array10[0] == "0")
                    {
                        tt_flag10 = true;
                        setRichtexBox("11、该单板没有进维修库或已维修出库，可以使用,goon");
                    }
                    else
                    {
                        setRichtexBox("11、该单板已进维修库，并且没有出库，不能使用,over");
                        PutLableInfor("该单板已进维修库，并且没有修好，不能使用");

                    }

                }
                #endregion


                //第十一步 维修检测二
                #region
                Boolean tt_flag11 = false;
                if (tt_flag10)
                {
                    tt_flag11 = true;
                    setRichtexBox("11、维修检测二预留过,goon");
                }
                #endregion


                //第十二步 关联表检查
                #region
                Boolean tt_flag12 = false;
                string tt_hostlable = "";
                string tt_shortmac = "";
                if (tt_flag11)
                {
                    string tt_sql12 = "select hostlable,maclable,id  from odc_alllable " +
                        "where taskscode = '" + tt_taskcode + "' and  hprintman = '" + tt_taskcode + "' and  pcbasn = '" + tt_scanpcba + "' ";

                    DataSet ds12 = Dataset1.GetDataSet(tt_sql12, tt_conn);
                    if (ds12.Tables.Count > 0 && ds12.Tables[0].Rows.Count > 0)
                    {
                        tt_flag12 = true;
                        tt_hostlable = ds12.Tables[0].Rows[0].ItemArray[0].ToString();  //主机条码
                        tt_shortmac = ds12.Tables[0].Rows[0].ItemArray[1].ToString();    //短MAC
                        tt_rowid = ds12.Tables[0].Rows[0].ItemArray[2].ToString();   //行ID

                        setRichtexBox("12、工单中:" + tt_taskcode + "关联表查询到一条MAC数据，hostlable=" + tt_hostlable + ",mac=" + tt_shortmac + ",id=" + tt_rowid + ",goon");

                    }
                    else
                    {
                        string tt_querytask = getSnRealTask("1", tt_scanpcba);
                        setRichtexBox("12、在工单:" + tt_taskcode + "的关联表中没有查询到该单板数据，该单板工单为：" + tt_querytask + ",over");
                        PutLableInfor("该单板工单为:" + tt_querytask + ",与工单:" + tt_taskcode + "不符");
                    }

                }
                #endregion
                

                //第十三步 NG01 获取单板站位信息
                #region
                Boolean tt_flag13 = false;
                DataSet tt_dataset1 = null;
                if (tt_flag12)
                {
                    string tt_sql13 = "select Id,Ccode,Ncode,Napplytype,Fremark from odc_routingtasklist " +
                                       "where pcba_pn = '" + tt_shortmac + "' order by id ";
                    tt_dataset1 = Dataset1.GetDataSet(tt_sql13, tt_conn);

                    if (tt_dataset1.Tables.Count > 0 && tt_dataset1.Tables[0].Rows.Count > 0)
                    {
                        tt_flag13 = true;
                        setRichtexBox("13、站位表找到MAC站位信息，记录数为:" + tt_dataset1.Tables[0].Rows.Count.ToString() + ",goon");

                    }
                    else
                    {
                        setRichtexBox("13、NG01，站位表没有找MAC:" + tt_shortmac + "，站位信息，over");
                        PutLableInfor("NG01站位表没有找MAC:" + tt_shortmac + "，站位信息");
                    }


                }
                #endregion
                

                //第十四步 NG02 获取MAC的待测站位
                #region
                Boolean tt_flag14 = false;
                string tt_testcode = this.label76.Text;
                if (tt_flag13)
                {
                    string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset1);
                    if (tt_nowcode == tt_testcode)
                    {
                        tt_flag14 = true;
                        setRichtexBox("14、该单板的最后站位与流程设置的待测站位一致，都是:" + tt_nowcode + ",goon");
                    }
                    else
                    {
                        if (tt_nowcode == "0")
                        {
                            setRichtexBox("14、NG02,当前单板MAC:" + tt_shortmac + ",没有待测站位，请检查，over");
                            PutLableInfor("当前单板MAC:" + tt_shortmac + ",没有待测站位");
                        }
                        else
                        {
                            if (tt_nowcode == "2")
                            {
                                setRichtexBox("14、NG02,当前单板MAC:" + tt_shortmac + ",有多个待测待测站位，流程异常，over");
                                PutLableInfor("当前单板MAC:" + tt_shortmac + ",有多个待测站位,流程异常");
                            }
                            else
                            {
                                setRichtexBox("14、NG02，当前单板MAC:" + tt_shortmac + "，站位不对" + tt_nowcode + "，与设定站位" + tt_testcode + "不符，不过使用,over");
                                PutLableInfor("NG02当前单板MAC:" + tt_shortmac + ",当前站位" + tt_nowcode + ",与" + tt_testcode + ",不符");
                            }
                        }

                    }

                }
                #endregion


                //第十五步 NG03 1920站位检查
                #region
                Boolean tt_flag15 = false;
                int tt_int1920id = 0;
                if (tt_flag14)
                {
                    tt_int1920id = Dataset2.getFirstCodeId(tt_dataset1);
                    if (tt_int1920id >  0)
                    {
                        tt_flag15 = true;
                        setRichtexBox("15、前站位ccode找到一个最近的1920站位，id=" + tt_int1920id.ToString()+ ",goon");
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
                              PutLableInfor("NG03过站没有找到1920站位");
                           break;

                            default :
                                setRichtexBox("15、NG03,查找起始站位1902数据集有问题，出现异常情况，id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor("NG03查找起始站位1902数据集出现异常情况");
                           break;


                        }
                    }

                }
                #endregion


                //第十六步 NG04 3350跳出检验
                #region
                Boolean tt_flag16 = false;
                if(tt_flag15)
                {
                    tt_flag16 = true;
                    setRichtexBox("16、NG04过,3350跳出检查先不检验直接过,goon");

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


                //第十七步 NG05 全流程检验
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
                        setRichtexBox("17、该单板所有站位都测试，没有漏测站位，全部流程:"+tt_allprocesses+",检验流程:"+tt_partprocesses+",1920id:"+tt_int1920id.ToString()+",goon");
                    }
                    else
                    {
                        if (tt_codecheck == "0")
                        {
                            setRichtexBox("17、NG05,单板站位全流程检查数据集有问题,MAC" + tt_shortmac + ",全部流程:"+tt_allprocesses+",检验流程:"+tt_partprocesses+",1920id:"+tt_int1920id.ToString()+",over");
                            PutLableInfor("NG05,单板站位全流程检查数据集有问题");
                        }
                        else
                        {
                            setRichtexBox("17、NG05,该单板这个站位没有测试:" + tt_codecheck + "，请仔细检查MAC:" + tt_shortmac + ",的流程:全流程为:" + tt_allprocesses + ",检测流程为:"+tt_partprocesses+",是否有漏测站位，over");
                            PutLableInfor("NG05,该单板这个站位没有测试:" + tt_codecheck + "，请检查是否漏测");
                        }
                    }
                }
                #endregion


                //第十八步 NG06 流程顺序检查
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
                    //    setRichtexBox("18、MAC全顺序检查OK没有问题，返回值：" + tt_codeserialcheck + ",检查起始ID:" + tt_int1920id.ToString()+ ",goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("18、NG06,MAC全顺序检查Fail有问题，返回值：" + tt_codeserialcheck + ",over");
                    //    PutLableInfor("NG06," + tt_codeserialcheck);
                    //}
                }
                #endregion


                //第十九步 NG07 跳站检查信息 检查前后顺序
                #region   
                Boolean tt_flag19 = false;
                if ( tt_flag18)
                {
                    string tt_nearcodecheck = Dataset2.getNearCodeCheck2(tt_dataset1, tt_int1920id, tt_allroutdataset);
                    if (tt_nearcodecheck.Equals("1"))
                    {
                        tt_flag19 = true;
                        setRichtexBox("19、过站前后站位检查OK没有问题，返回值：" + tt_nearcodecheck + ",检查起始ID:" + tt_int1920id.ToString() + ",goon");
                    }
                    else
                    {
                        setRichtexBox("19、NG07,过站前后站位检查Fail有问题，返回值：" + tt_nearcodecheck + ",over");
                        PutLableInfor("NG07," + tt_nearcodecheck);
                    }
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
                    //    setRichtexBox("20、过站上下站位检查OK没有问题，返回值：" + tt_updowncodecheck + ",检查起始ID:" + tt_int1920id.ToString() + ",goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("20、NG08,过站上下站位检查Fail有问题，返回值：" + tt_updowncodecheck + ",over");
                    //    PutLableInfor("NG08," + tt_updowncodecheck);
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


                //第二十四步 物料追溯添加
                #region
                Boolean tt_flag24 = false;
                if (tt_flag23)
                {
                    Boolean tt_idinfo = GetMaterialIdinfor(tt_rowid);

                    if (tt_idinfo)
                    {
                        string tt_insert = "insert into odc_traceback(fid,fmpdate,Fsegment1,Fsegment2,Fsegment3,Ftaskcode,Fpcba,Fhostlable,Fmaclable) " +
                                           "values(" + tt_rowid + ",getdate(),'" + tt_uplip + "','" + tt_downlip + "','" + tt_tin + "','"
                                            + tt_taskcode + "','" + tt_scanpcba + "','" + tt_hostlable + "','" + tt_shortmac + "')";

                        int tt_int1 = Dataset1.ExecCommand(tt_insert, tt_conn);

                        if (tt_int1 > 0)
                        {
                            tt_flag24 = true;
                            setRichtexBox("24、物料追溯已成功追加到物料表odc_traceback，id号：" + tt_rowid + ",goon");
                        }
                        else
                        {
                            setRichtexBox("24、物料追溯没有成功追加物料表！,over");
                            PutLableInfor("物料追溯没有成功追加物料表!请继续扫描！");
                        }

                    }
                    else
                    {
                        string tt_update = "update odc_traceback set Fsegment1='" + tt_uplip + "',Fsegment2='" + tt_downlip + "',Fsegment3='" + tt_tin + "' " +
                                           "where Fid = " + tt_rowid;
                        int tt_int2 = Dataset1.ExecCommand(tt_update, tt_conn);

                        if (tt_int2 > 0)
                        {
                            tt_flag24 = true;
                            setRichtexBox("24、物料追溯已成功更新到物料表odc_traceback，id号：" + tt_rowid + ",goon");
                        }
                        else
                        {
                            setRichtexBox("24、物料追溯没有成功更新到物料表！,over");
                            PutLableInfor("物料追溯没有成功更新到物料表!请继续扫描！");
                        }

                    }

                }
                #endregion


                //第二十五步 开始过站
                #region
                Boolean tt_flag25 = false;
                if ( tt_flag24 )
                {
                    string tt_username = STR;
                    tt_flag25 = Dataset1.FhYcMadePrintPcbaLabel(tt_taskcode, tt_username, tt_shortmac, tt_gyid, tt_ccode, tt_ncode, tt_conn);
                    if (tt_flag12)
                    {
                        setRichtexBox("25、单板过站成功，请继续扫描,ok");
                    }
                    else
                    {
                        setRichtexBox("25、单板关联不成功，事务已回滚");
                        PutLableInfor("单板过站不成功，请检查或再次扫描！");
                    }

                }
                #endregion



                //最后判断
                #region
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10 &&
                    tt_flag11 && tt_flag12 && tt_flag13 && tt_flag14 && tt_flag15 && tt_flag16 && tt_flag17 && tt_flag18 && tt_flag19 && tt_flag20 &&
                    tt_flag21 && tt_flag22 && tt_flag23 && tt_flag24 && tt_flag25)
                {
                    //数据显示
                    this.label39.Text = tt_scanpcba;  //单板号
                    this.label41.Text = tt_shortmac;  //主机条码

                    //扣数
                    tt_uplip--;  //上盖数量
                    tt_downlip--; //下盖数量
                    this.label55.Text = tt_uplip.ToString();
                    this.label56.Text = tt_downlip.ToString();

                    //打印记录
                    Dataset1.lablePrintRecord(tt_taskcode, tt_shortmac, tt_scanpcba, "延迟临时标签", str, tt_computermac, "", tt_conn);

                    //打印
                    GetParaDataPrint(1);
                    CheckStation2(tt_shortmac);
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
                //-----以上PCBA过站打印扫描------
            }
        }

        #endregion



        #region 12、数据采集及模板打印
        //获取参数
        private void GetParaDataPrint(int tt_itemtype)
        {
            string tt_fdata = "MP01";

            //mp01---数据类型一
            if (tt_fdata == "MP01")
            {
                GetParaDataPrint_MP01(tt_itemtype);
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
            row1["名称"] = "单板号";
            row1["内容"] = doReportParaCheck("单板号", this.label39.Text);
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "工单号";
            row2["内容"] = doReportParaCheck("工单号", this.textBox1.Text);
            dt.Rows.Add(row2);



            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "MAC";
            row3["内容"] = doReportParaCheck("GPSN", this.label41.Text);
            dt.Rows.Add(row3);


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
        private string doReportParaCheck(string tt_datatype, string tt_str)
        {
            string tt_outinfo = "";
            if (!tt_str.Equals(""))
            {
                tt_outinfo = tt_str;
            }
            else
            {
                MessageBox.Show("模板参数数据项" + tt_datatype + ",为空，请检查");
            }
            return tt_outinfo;
        }


        #endregion


       





        



        


       














        //-------end--------
    }
}
