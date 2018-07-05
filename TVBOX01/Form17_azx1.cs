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
    public partial class Form17_azx1 : Form
    {
        public Form17_azx1()
        {
            InitializeComponent();
            SetFpathFdataIsnotVisable();
        }

        #region 1、属性设置
        static string tt_conn;
        int tt_yield = 0;
        int tt_yieldbox = 0;
        DateTime tt_productstarttime = DateTime.Now; //开始时间
        //DateTime tt_productprimtime; //上一次时间

        //static string tt_delepath = Application.StartupPath + @"\LABLE";
        //static string tt_copypath = @"D:\\LABLE";

        //string tt_gyid = "";
        string tt_ccode = "";
        string tt_ncode = "";
        string tt_pon_name = "";
        string tt_areacode = "";
        string tt_beforstranhui = "";
        int tt_QRDZ = 0;
        int tt_scanboxnum = 0;
        //标签微调
        static float tt_top = 0; //上下偏移量
        static float tt_left = 0; //左右偏移量

        static int tt_reprinttime = 0; //重打次数

        //重打限制标识
        string tt_reprintmark = "1";
        //重打限数
        int tt_reprintchang1 = 0;
        int tt_reprintchang2 = 0;
        //重打计时
        DateTime tt_reprintstattime;
        DateTime tt_reprintendtime;

        //全流程检验
        static string tt_allprocesses = null;
        static string tt_partprocesses = null;
        static DataSet tt_routdataset = null;
        static DataSet tt_allroutdataset = null;

        //流程兼容用中间变量
        static string tt_gyid_Old = "";
        static string tt_gyid_Use = "";

        //读取的打印设置
        static string BoxPrintMode = "";

        //小型化方案装箱兼容用
        static string tt_MiniType = "";

        //临时参数

        //打印铭牌时，电源选择1.5A显示标识（正常HG6201M产品为1.0A）
        string tt_power_old = "";
        //1.5A电源物料不足问题重打标识
        string tt_power_re = "";

        //移动产品新二维码用判断参数
        static int tt_CMCCQR_DateCheck = 30000000;

        //本机MAC
        static string tt_computermac = "";
        private void Form17_azx1_Load(object sender, EventArgs e)
        {
            //FastReport环境变量设置（打印时不提示 "正在准备../正在打印..",一个程序只需设定一次，故一般写在程序入口）
            (new FastReport.EnvironmentSettings()).ReportSettings.ShowProgress = false;

            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";
            this.toolStripStatusLabel6.Text = tt_productstarttime.ToString();

            //初始不显示身份验证栏
            this.groupBox23.Visible = false;

            //初始不显示微调栏
            this.groupBox22.Visible = false;

            //隐藏线长调试按钮
            this.button20.Visible = false;

            //不勾选模板
            this.checkBox5.Checked = false;
            this.checkBox6.Checked = false;

            //员工账号分离
            if (str.Contains("FH005") || str.Contains("FH006"))
            {
                this.button2.Visible = false;
                this.button3.Visible = false;
                this.button4.Visible = false;
                this.button6.Visible = false;
                this.button7.Visible = false;
                this.tabPage4.Parent = null;
                this.button20.Visible = true;
            }

            ClearLabelInfo1();
            this.label32.Text = tt_yield.ToString();
            this.label30.Text = tt_yieldbox.ToString();
            this.label29.Text = "0";

            this.textBox4.Visible = false;
            this.textBox9.Visible = false;

            //ListView添加表头
            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.LabelEdit = true; //是否可编辑,ListView只可编辑第一列。
            this.listView1.Scrollable = true;//有滚动条
            this.listView1.HeaderStyle = ColumnHeaderStyle.Clickable;//对表头进行设置
            this.listView1.FullRowSelect = true;//是否可以选择行


            this.listView1.Columns.Add("序号", 40);
            this.listView1.Columns.Add("SN", 120);
            this.listView1.Columns.Add("PCBA", 130);
            this.listView1.Columns.Add("MAC", 100);
            this.listView1.Columns.Add("设备标示", 250);
            this.listView1.Columns.Add("GPSN", 150);
            this.listView1.Columns.Add("设备标示暗码", 250);

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


        #region 2、数据清除
        //锁定工单清除
        private void ClearLabelInfo1()
        {
            //工单信息
            this.label9.Text = null;
            this.label10.Text = null;
            this.label11.Text = null;
            this.label12.Text = null;
            this.label13.Text = null;
            this.label15.Text = null;
            this.label25.Text = null;
            this.label57.Text = null;
            this.label77.Text = null;

            //流程信息
            this.label52.Text = null;
            this.label53.Text = null;
            this.label54.Text = null;
            this.label55.Text = null;
            this.label3.Text = null;
            this.label63.Text = null;

            //生产信息
            this.label69.Text = null;
            this.label70.Text = null;


            //模板信息
            this.label40.Text = null;
            this.label41.Text = null;
            this.label42.Text = null;
            this.label43.Text = null;
            this.label44.Text = null;
            this.label45.Text = null;


            //装箱信息
            this.label46.Text = null;
            this.label47.Text = null;
            this.label48.Text = null;
            this.label78.Text = null;

            //错误显示
            this.label39.Text = null;

            //Datagridview
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

            //流程表
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;


        }

        //重置请除
        private void ClearLabelInfo2()
        {
            //装箱信息
            this.label46.Text = null;
            this.label47.Text = null;
            this.label48.Text = null;

            //流程信息
            this.label53.Text = null;
            this.label3.Text = null;

            //SN条框
            this.textBox4.Text = null;
            this.textBox9.Text = null;

            //错误显示
            this.label39.Text = null;

            //Datagridview
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

            //流程表
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;

            //扫描产品
            tt_scanboxnum = 0;
            this.textBox3.Text = tt_scanboxnum.ToString();

            //listview清除
            ClearListView();

        }

        //装箱后的清除，再装新的一箱
        private void ClearLabelInfo3()
        {

            //listview清除
            ClearListView();

            //扫描产品
            tt_scanboxnum = 0;
            this.textBox3.Text = tt_scanboxnum.ToString();

            //提示信息
            PutLableInfor("开始装新的一箱");
            this.label32.Text = tt_yield.ToString();
            this.label30.Text = tt_yieldbox.ToString();
        }


        //装第一箱得数据初始化
        private void ClearLabelInfo4()
        {
            //装箱信息
            this.label46.Text = null;
            this.label47.Text = null;
            this.label48.Text = null;


            //错误显示
            this.label39.Text = null;

            //Datagridview
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

            //流程表
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;


        }


        //模板三先不要用了，等需要时候再用了
        private void SetFpathFdataIsnotVisable()
        {
            //this.checkBox7.Visible = false;
            //this.label19.Visible = false;
            //this.label42.Visible = false;
            //this.label45.Visible = false;
            //this.button8.Visible = false;
            //this.button9.Visible = false;


        }



        //扫描前数据初始化
        private void ScanDataInitial()
        {
            //装箱数据
            this.textBox3.Text = null;
            this.label46.Text = null;
            this.label47.Text = null;
            this.label48.Text = null;


            //提示信息
            this.label39.Text = null;

            //表格
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

            //richtext
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;

            ClearListView();
        }

        #endregion


        #region 3、ListView操作

        //listview情空
        private void ClearListView()
        {
            int count = this.listView1.Items.Count;
            for (int i = 0; i < count; i++)
            {
                listView1.Items[0].Remove();
            }
            this.label78.Text = null;
        }

        //添加listview数据
        private void PutListViewData(string tt_boxsn, string tt_pcba, string tt_mac, string tt_smtbarcode, string tt_fhttgpsn, string tt_smtbarcode1)
        {
            int i = this.listView1.Items.Count + 1;
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { i.ToString(), tt_boxsn, tt_pcba, tt_mac, tt_smtbarcode, tt_fhttgpsn, tt_smtbarcode1 });
            this.listView1.Items.AddRange(p);
            this.listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
            setListviewSerialShow(tt_boxsn);
        }



        //listview数据过站
        private Boolean ListViewStatioPass(string tt_task,
                                           string tt_gyid,
                                           string tt_code,
                                           string tt_ncode,
                                           string tt_package,
                                           string tt_conn)
        {
            Boolean tt_flag = false;


            //第一步数据初始化
            string tt_boxsn = "";
            string tt_listpcba = "";
            string tt_listmac = "";
            int count = this.listView1.Items.Count;
            int tt_passcount = 0;


            //第二步 循环过站
            for (int i = 0; i < count; i++)
            {
                tt_boxsn = this.listView1.Items[i].SubItems[1].Text;
                tt_listpcba = this.listView1.Items[i].SubItems[2].Text;
                tt_listmac = this.listView1.Items[i].SubItems[3].Text;

                Boolean tt_flag11 = false;
                Boolean tt_flag22 = false;


                tt_flag11 = Dataset1.FhPackageInStation(tt_task, STR, tt_listpcba, tt_listmac, tt_gyid, tt_code, tt_ncode, tt_package, tt_conn);

                if (tt_flag11)
                {

                    setRichtexBox("14." + i.ToString() + "、SN：" + tt_boxsn + ",第一次过站成功，ok");

                }
                else
                {
                    setRichtexBox("14." + i.ToString() + "、SN：" + tt_boxsn + ",第一次过站失败，开始第二次过站");
                    tt_flag22 = Dataset1.FhPackageInStation(tt_task, STR, tt_listpcba, tt_listmac, tt_gyid, tt_code, tt_ncode, tt_package, tt_conn);
                    if (tt_flag22)
                    {
                        setRichtexBox("14." + i.ToString() + "、SN：" + tt_boxsn + ",第二次过站成功,ok");
                    }
                    else
                    {
                        setRichtexBox("14." + i.ToString() + "、SN：" + tt_boxsn + ",第二次过站失败,end");
                    }

                }

                //记录过站次数
                if (tt_flag11 || tt_flag22)
                {

                    tt_passcount++;
                    tt_yield++;
                }


            }


            //第三步确定过程结果
            if (tt_passcount == count)
            {
                tt_flag = true;
                tt_yieldbox++;
                setRichtexBox("14、全部过站成功，成功次数：" + tt_passcount.ToString() + ",ok");
                PutLableInfor("14、全部过站成功，成功次数：" + tt_passcount.ToString());
            }
            else
            {
                setRichtexBox("14、糟糕、没有全部过站成功，成功次数：" + tt_passcount.ToString() + ",ok");
                PutLableInfor("糟糕、没有全部过站成功，成功次数：" + tt_passcount.ToString());
            }


            return tt_flag;
        }




        //获取ListView数据
        private string GetListViewItem(int tt_itemtype, int tt_itemnumber)
        {
            string tt_item = "";

            int tt_count = this.listView1.Items.Count;

            if (tt_count >= tt_itemnumber)
            {
                if (tt_itemtype == 1)
                {

                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[1].Text;  //SN 生产序列号
                }
                else if (tt_itemtype == 2)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[3].Text;  //MAC 
                }
                else if (tt_itemtype == 3)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[4].Text;  //设备标示
                }
                else if (tt_itemtype == 4)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[5].Text;  //GPSN 
                }
                //else if (tt_itemtype == 5) //朝歌用
                //{
                //    //tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[5].Text.Substring(0, 17);
                //    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[5].Text;
                //}
                else if (tt_itemtype == 6)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[6].Text; //设备标示暗码
                }
            }



            return tt_item;
        }



        //ListView重复性检查
        private Boolean CheckNumberRepeat(string tt_newitem)
        {
            Boolean tt_flag = false;

            foreach (ListViewItem item in this.listView1.Items)
            {
                string tt_sn = item.SubItems[1].Text.ToString().Trim();
                if (String.Compare(tt_newitem, tt_sn) == 0)
                {
                    tt_flag = true;
                    break;
                }
            }

            return tt_flag;

        }




        //显示ListView序号
        private void setListviewSerialShow(string tt_str)
        {
            if (tt_str.Length > 2)
                this.label78.Text = tt_str.Substring(tt_str.Length - 2, 2);

        }


        #endregion


        #region 4、辅助功能
        //richtext加记录
        private void setRichtexBox(string tt_textinfor)
        {
            this.richTextBox1.Text = this.richTextBox1.Text + tt_textinfor + "\n";
        }


        //lable提示信息
        private void PutLableInfor(string tt_lableinfo)
        {
            this.label39.Text = tt_lableinfo;
        }


        //lable提示信息并记录NG信息
        private void PutLableInfor2(string tt_lableinfo, string tt_task, string tt_mac)
        {
            int tt_int = Dataset2.getNgreasonRecord(tt_task, tt_mac, "中箱标签", tt_lableinfo, "3201", tt_conn);
            this.label39.Text = tt_lableinfo + "," + tt_int.ToString();
        }


        //获取验证铭牌路径
        private Boolean GetPathIstrue(string tt_file)
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




        //包含字符判断2  包含符必填的
        private Boolean CheckStrContain2(string tt_scansn, string tt_containstr)
        {
            Boolean tt_flag = false;

            if (tt_containstr.Length > 3)
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
                setRichtexBox("2、字符包含符必须填写至少4位,over");
                PutLableInfor("字符包含符必须填写至少4位,请确认！");
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


        //--生产节拍
        private void GetProductRhythm()
        {
            DateTime tt_productendtime = DateTime.Now;  //当前时间
            //计算时间差
            TimeSpan tt_diff;
            tt_diff = tt_productendtime - tt_productstarttime;

            decimal tt_difftime = tt_diff.Hours * 3600 + tt_diff.Minutes * 60 + tt_diff.Seconds;
            string tt_millsecnds = tt_diff.Milliseconds.ToString();
            string tt_differtime2 = tt_difftime.ToString() + "." + tt_millsecnds;

            TimeSpan tt_ts = tt_productendtime - tt_productstarttime;  //耗用时间
            int tt_second = tt_ts.Hours * 3600 + tt_ts.Minutes * 60 + tt_ts.Seconds;
            string tt_time = tt_ts.Hours.ToString() + "小时" + tt_ts.Minutes.ToString() + "分" + tt_ts.Seconds.ToString() + "秒";
            this.label29.Text = tt_time;               //生产时间
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

        //联通产品判断附加字段
        private string GetGPSN_WORD(string tt_pon_name, string tt_telecustomer, string gpsn)
        {
            string tt_WORD = "";

            if (tt_pon_name == "GPON" && tt_telecustomer == "联通" && gpsn != "")
            {
                tt_WORD = "SN:";
            }
            else if (tt_pon_name == "EPON" && tt_telecustomer == "联通" && gpsn != "")
            {
                tt_WORD = "MAC:";
            }

            return tt_WORD;
        }

        //通用尾箱二维码逗号处理
        private string GetQR_COMMA(string ListViewItem)
        {
            string tt_WORD = "";

            if (ListViewItem != "")
            {
                tt_WORD = ",";
            }
            else
            {
                tt_WORD = "";
            }

            return tt_WORD;
        }

        //通用尾箱二维码换行处理
        private string GetQR_LINE_BREAK(string ListViewItem)
        {
            string tt_WORD = "";

            if (ListViewItem != "")
            {
                tt_WORD = "\r";
            }
            else
            {
                tt_WORD = "";
            }

            return tt_WORD;
        }

        //通用尾箱二维码分隔符处理
        private string GetQR_SEPARATOR(string ListViewItem)
        {
            string tt_WORD = "";

            if (ListViewItem != "")
            {
                tt_WORD = "|";
            }
            else
            {
                tt_WORD = "";
            }

            return tt_WORD;
        }

        //通用二维码文字处理
        private string GetQR_NORMAL(string productname, string ListViewItem)
        {
            string tt_WORD = "";

            if (ListViewItem != "")
            {
                tt_WORD = "FIBER|" + productname + "|";
            }
            else
            {
                tt_WORD = "";
            }

            return tt_WORD;
        }

        //四川二维码文字处理
        private string GetQR_SICHUAN(string ListViewItem)
        {
            string tt_WORD = "";

            if (ListViewItem != "")
            {
                tt_WORD = "FIBER|";
            }
            else
            {
                tt_WORD = "";
            }

            return tt_WORD;
        }

        //安徽移动符号处理
        private string GetQR_ANHUI(string ListViewItem)
        {
            string tt_WORD = "";

            if (ListViewItem != "")
            {
                tt_WORD = "#";
            }
            else
            {
                tt_WORD = "";
            }

            return tt_WORD;
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

        //生产年份-数字-字母转换
        static string Host_Year_Num_AZ(int num)
        {
            string HostAZ = "";
            string[] HostAZ_Temp = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');

            if (num >= 15 && num < 40)
            {
                HostAZ = HostAZ_Temp[num - 15];
            }
            else
            {
                HostAZ = "0";
            }

            return HostAZ;
        }

        //生产月-数字-字母转换
        static string Host_Month_Num_AZ(int num)
        {
            string HostAZ = "";
            string[] HostAZ_Temp = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');

            if (num >= 10 && num < 36)
            {
                HostAZ = HostAZ_Temp[num - 10];
            }
            else
            {
                HostAZ = num.ToString();
            }

            return HostAZ;
        }

        #endregion


        #region 5、数据辅助功能

        //刷新站位
        private void CheckStation(string tt_task, string tt_gesn)
        {
            string tt_sql = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime " +
                            "from odc_routingtasklist " +

                            "where taskscode = '" + tt_task + "' and napplytype is null " +

                                "and pcba_pn in ( select maclable from odc_package T1 , odc_alllable T2 " +
                                                " where T1.pasn = T2.pcbasn and " +
                                                " T1.taskcode = '" + tt_task + "' and T1.pagesn = '" + tt_gesn + "' ) ";

            DataSet ds1 = Dataset1.GetDataSet(tt_sql, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView1.DataSource = ds1;
                dataGridView1.DataMember = "Table";

                this.label53.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //当前站位
            }

        }


        ////获取箱号  青岛获取箱号
        //private string GetBoxNumber(string tt_beforstr, string tt_fromsn, string tt_setunitnum)
        //{
        //    string tt_boxnumber = "";
        //    decimal tt_unitint = decimal.Parse(tt_setunitnum);
        //    decimal tt_snnumber = int.Parse(tt_fromsn.Substring(tt_fromsn.Length - 5, 5));
        //    decimal tt_boxnum2 = Math.Ceiling(tt_snnumber / tt_unitint);
        //    string tt_boxnum3 = tt_boxnum2.ToString();
        //    tt_boxnumber = tt_beforstr + tt_boxnum3.PadLeft(6, '0');
        //    return tt_boxnumber;
        //}


        ////贵州获取箱号
        //private string GetBoxNumber2(string tt_beforstr, string tt_task)
        //{
        //    string tt_boxnumber = "99999";
        //    string tt_boxnumber2 = "99999";
        //    string tt_boxnumbernext = "";


        //    string tt_sql1 = "select count(1),min(hostmode),0 from ODC_HOSTLABLEOPTIOAN " +
        //                     " where taskscode = '" + tt_task + "' ";
        //    string[] tt_array = new string[3];
        //    tt_array = Dataset1.GetDatasetArray(tt_sql1, tt_conn);
        //    if (tt_array[0] == "1")
        //    {

        //        tt_boxnumber = tt_array[1];
        //        int A = Convert.ToInt32(tt_boxnumber);
        //        tt_boxnumbernext = (A + 1).ToString();


        //        string tt_update = "update ODC_HOSTLABLEOPTIOAN set hostmode = '" + tt_boxnumbernext + "' " +
        //                           "where taskscode = '" + tt_task + "' ";


        //        int tt_int = Dataset1.ExecCommand(tt_update, tt_conn);
        //        if (tt_int > 0)
        //        {
        //            tt_boxnumber2 = tt_boxnumber;
        //            tt_boxnumber2 = string.Format("{0:d5}", A);

        //        }
        //        else
        //        {
        //            MessageBox.Show("箱号设置更新失败！");
        //        }



        //    }
        //    else
        //    {
        //        MessageBox.Show("没有找到该箱号的设置信息");
        //    }

        //    return tt_beforstr + tt_boxnumber2;

        //}


        //获取箱号  烽火wifi箱号 正常装箱
        private string GetBoxNumber3(string tt_beforstr, string tt_tosn, string tt_setunitnum)//获取箱号  烽火wifi箱号 正常装箱
        {
            string tt_boxnumber = "";
            decimal tt_unitint = decimal.Parse(tt_setunitnum);
            decimal tt_snnumber = int.Parse(tt_tosn.Substring(tt_tosn.Length - 4, 4));
            //decimal tt_boxnum2 = Math.Ceiling(tt_snnumber / tt_unitint);
            decimal tt_boxnum2 = (tt_snnumber / tt_unitint);
            string tt_boxnum3 = tt_boxnum2.ToString();
            if (tt_boxnum3.Contains(".") == false)
            {
                tt_boxnumber = tt_beforstr + "C" + tt_boxnum3.PadLeft(3, '0');
            }
            return tt_boxnumber;
        }


        //获取箱号  烽火wifi箱号 生成尾箱分箱
        private string GetBoxNumber4(string tt_beforstr, string tt_fromsn, string tt_setunitnum)
        {
            string tt_boxnumber = "";

            //第一步获取前七位箱号
            string tt_beforstr7 = "";
            bool tt_flag1 = false;
            if (tt_beforstr.Length >= 7)
            {
                tt_flag1 = true;
                tt_beforstr7 = tt_beforstr.Substring(0, 7);

            }
            else
            {
                MessageBox.Show("箱号的串号设置位数小于7位数，请确定");
            }

            //第二步获取箱号流水号
            decimal tt_unitint = 0;
            decimal tt_snnumber = 0;
            decimal tt_boxnum2 = 0;
            string tt_boxnum3 = "";
            bool tt_flag2 = false;
            if (tt_flag1)
            {
                try
                {
                    tt_unitint = decimal.Parse(tt_setunitnum);
                    tt_snnumber = int.Parse(tt_fromsn.Substring(tt_fromsn.Length - 4, 4));
                    tt_boxnum2 = Math.Ceiling(tt_snnumber / tt_unitint);
                    tt_boxnum3 = tt_boxnum2.ToString();
                    tt_flag2 = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("生成尾箱，计算箱号错误：" + ex.Message);
                }
            }

            //第三步 获取分箱的批次号
            string tt_middlelot = "Z";
            int tt_model = 0;
            int tt_unitint2 = Convert.ToInt32(tt_unitint);
            int tt_snnumber2 = Convert.ToInt32(tt_snnumber);
            Boolean tt_flag3 = false;
            if (tt_flag2)
            {
                tt_model = (int)tt_snnumber2 % tt_unitint2;

                switch (tt_model)
                {
                    case 0: tt_middlelot = "E"; break;
                    case 1: tt_middlelot = "F"; break;
                    case 2: tt_middlelot = "G"; break;
                    case 3: tt_middlelot = "H"; break;
                    case 4: tt_middlelot = "I"; break;
                    case 5: tt_middlelot = "J"; break;
                    case 6: tt_middlelot = "K"; break;
                    case 7: tt_middlelot = "L"; break;
                    case 8: tt_middlelot = "M"; break;
                    case 9: tt_middlelot = "N"; break;
                    case 10: tt_middlelot = "O"; break;
                    case 11: tt_middlelot = "P"; break;
                    case 12: tt_middlelot = "Q"; break;
                    case 13: tt_middlelot = "R"; break;
                    case 14: tt_middlelot = "S"; break;
                    case 15: tt_middlelot = "T"; break;
                    case 16: tt_middlelot = "U"; break;
                    case 17: tt_middlelot = "V"; break;
                    case 18: tt_middlelot = "W"; break;
                    case 19: tt_middlelot = "X"; break;
                    case 20: tt_middlelot = "Y"; break;
                    default: tt_middlelot = "Z"; break;
                }
                tt_flag3 = true;

            }

            if (tt_flag3)
            {
                tt_boxnumber = tt_beforstr7 + tt_middlelot + "C" + tt_boxnum3.PadLeft(3, '0');
            }


            return tt_boxnumber;
        }


        //获取箱号  烽火wifi箱号 安徽移动装箱
        private string GetBoxNumber5(string tt_beforstr, string tt_tosn, string tt_setunitnum)//获取箱号  烽火wifi箱号 正常装箱
        {
            string tt_boxnumber = "";
            decimal tt_unitint = decimal.Parse(tt_setunitnum);
            decimal tt_snnumber = int.Parse(tt_tosn.Substring(tt_tosn.Length - 4, 4));
            //decimal tt_boxnum2 = Math.Ceiling(tt_snnumber / tt_unitint);
            decimal tt_boxnum2 = (tt_snnumber / tt_unitint);
            string tt_boxnum3 = tt_boxnum2.ToString();
            if (tt_boxnum3.Contains(".") == false)
            {
                tt_boxnumber = tt_beforstr + tt_boxnum3.PadLeft(3, '0') + "-" + tt_setunitnum;
            }
            return tt_boxnumber;
        }


        //获取箱号  烽火wifi箱号 生成尾箱分箱 安徽移动分箱
        private string GetBoxNumber6(string tt_beforstr, string tt_taskscode, string tt_setunitnum)
        {
            string tt_boxnumber = "";
            decimal tt_unitint = 0;
            decimal tt_snnumber = 0;
            decimal tt_boxnum2 = 0;
            string tt_boxnum3 = "";
            bool tt_flag = false;

            try
            {
                tt_unitint = decimal.Parse(tt_setunitnum);
                tt_snnumber = int.Parse(GetAnhuiHOSTLAST(tt_taskscode));
                tt_boxnum2 = Math.Ceiling(tt_snnumber / tt_unitint);
                tt_boxnum3 = tt_boxnum2.ToString();
                tt_flag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成尾箱，计算箱号错误：" + ex.Message);
            }
            if (tt_flag)
            {
                tt_boxnumber = tt_beforstr + tt_boxnum3.PadLeft(3, '0') + "-" + tt_setunitnum;
            }

            return tt_boxnumber;
        }


        //获取、生成安徽移动尾箱序号
        private string GetAnhuiHOSTLAST(string tt_taskscode)
        {
            string tt_hostlastnow = "";
            string tt_hostlast = "";
            string tt_id = "";
            string tt_tasksquantity = "";

            string tt_sql = "select count(1),min(hostlast),min(id) from ODC_HOSTLABLEOPTIOAN where taskscode = '" + tt_taskscode + "' ";
            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            if (tt_array[0] == "1")
            {
                tt_hostlast = tt_array[1];
                tt_id = tt_array[2];
            }

            if (tt_hostlast != "")
            {
                string tt_update1 = "update ODC_HOSTLABLEOPTIOAN set hostlast = '" + (int.Parse(tt_hostlast) + 10).ToString() + "' " +
                                    "where taskscode = '" + tt_taskscode + "' and id = " + tt_id;
                int tt_int = Dataset1.ExecCommand(tt_update1, tt_conn);
                if (tt_int == 1)
                {
                    string tt_sql0 = "select count(1),min(hostlast),min(id) from ODC_HOSTLABLEOPTIOAN where taskscode = '" + tt_taskscode + "' ";
                    string[] tt_array0 = new string[3];
                    tt_array0 = Dataset1.GetDatasetArray(tt_sql0, tt_conn);
                    if (tt_array0[0] == "1")
                    {
                        tt_hostlastnow = tt_array0[1];
                    }
                }
                else
                {
                    MessageBox.Show("更新尾箱数据失败!");
                }
            }
            else
            {
                string tt_sql1 = "select count(1),min(tasksquantity),min(taskscode) from odc_tasks where taskscode = '" + tt_taskscode + "' ";
                string[] tt_array1 = new string[3];
                tt_array1 = Dataset1.GetDatasetArray(tt_sql1, tt_conn);

                if (tt_array1[0] == "1")
                {
                    tt_tasksquantity = tt_array1[1];
                }

                string tt_update2 = "update ODC_HOSTLABLEOPTIOAN set hostlast = '" + (int.Parse(tt_tasksquantity) + 10).ToString() + "' " +
                                    "where taskscode = '" + tt_taskscode + "' and id = " + tt_id;
                int tt_int = Dataset1.ExecCommand(tt_update2, tt_conn);
                if (tt_int == 1)
                {
                    string tt_sql2 = "select count(1),min(hostlast),min(id) from ODC_HOSTLABLEOPTIOAN where taskscode = '" + tt_taskscode + "' ";
                    string[] tt_array2 = new string[3];
                    tt_array2 = Dataset1.GetDatasetArray(tt_sql2, tt_conn);
                    if (tt_array2[0] == "1")
                    {
                        tt_hostlastnow = tt_array2[1];
                    }
                    else
                    {
                        MessageBox.Show("更新尾箱数据失败!");
                    }
                }
            }

            return tt_hostlastnow;
        }

        //获取箱号  烽火wifi箱号 超大工单尾箱分箱
        private string GetBoxNumber7(string tt_beforstr, string tt_taskscode, string tt_setunitnum)
        {
            string tt_boxnumber = "";

            //第一步获取前七位箱号
            string tt_beforstr7 = "";
            bool tt_flag = false;
            bool tt_flag1 = false;
            if (tt_beforstr.Length >= 7)
            {
                tt_flag = true;
                tt_beforstr7 = tt_beforstr.Substring(0, 7);
            }
            else
            {
                MessageBox.Show("箱号的串号设置位数小于7位数，请确定");
            }

            if (tt_flag)
            {
                string tt_middlelot = "";
                string tt_boxnum = "";
                string tt_middlelot_old = "";
                string tt_boxnum_old = "";
                string tt_Year = Host_Year_Num_AZ(int.Parse(this.label12.Text.Replace("-", "").Substring(2, 2)));
                string tt_Month = Host_Month_Num_AZ(int.Parse(this.label12.Text.Replace("-", "").Substring(4, 2)));

                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string AZ = "";
                        if (i == 0) AZ = "Z";
                        if (i == 1) AZ = "Y";
                        if (i == 2) AZ = "X";

                        string tt_sql0 = "select count(1),max(pagesn),max(fid) from odc_package where taskcode in " +
                                         "(select taskscode from dbo.odc_tasks where areacode = '" + tt_areacode + "' " +
                                         "and product_name = '" + this.label10.Text + "') " +
                                         "and pagesn like '" + tt_beforstr7.Substring(0,5) + tt_Year + tt_Month + AZ + "C___' ";
                        string[] tt_array0 = new string[3];
                        tt_array0 = Dataset1.GetDatasetArray(tt_sql0, tt_conn);
                        if (tt_array0[0] != "0")
                        {
                            tt_middlelot_old = tt_array0[1].Substring(tt_array0[1].Length - 5, 1);
                            tt_boxnum_old = tt_array0[1].Substring(tt_array0[1].Length - 3, 3);
                        }
                        else
                        {
                            tt_middlelot_old = AZ;
                            tt_boxnum_old = "0";
                        }
                        if (int.Parse(tt_boxnum_old) < 999) break;
                    }

                    tt_middlelot = tt_middlelot_old;
                    tt_boxnum = (int.Parse(tt_boxnum_old) + 1).ToString();
                    tt_flag1 = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("生成尾箱，计算箱号错误：" + ex.Message);
                }

                if (tt_flag1)
                {
                    tt_boxnumber = tt_beforstr7 + tt_middlelot + "C" + tt_boxnum.PadLeft(3, '0');
                }
            }

            return tt_boxnumber;
        }

        //流程检查，获取下一流程
        private bool GetNextCode(string tt_task, string tt_username)
        {
            Boolean tt_flag = false;

            //第一步获取当前站位
            #region
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
            #endregion


            //第二步获取当前站位
            #region
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
            #endregion


            //第三步检查第一站位与设定的站位是否一致
            #region
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
            #endregion


            //第四步 获取下一站位
            #region
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
            #endregion


            //最后判断
            #region
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
            {
                tt_flag = true;
                this.label54.Text = tt_ccode;
                this.label55.Text = tt_ncode;
                this.label63.Text = tt_ccodenumber;
            }
            #endregion


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


        //获取装箱数量
        private void getPackageNunber(string tt_taskcode)
        {
            string tt_sql = "select count(1), count(distinct pagesn),0 from odc_package " +
                            "where taskcode = '" + tt_taskcode + "' ";

            string[] tt_array3 = new string[3];
            tt_array3 = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label69.Text = tt_array3[0];
            this.label70.Text = tt_array3[1];


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


        //检查MAC或单板，获取工单
        private string getSnRealTask(string tt_datatype, string tt_sn)
        {
            string tt_taskcode = "";
            string tt_sql = "Select 1,'不确定',1 ";
            string tt_sql1 = "select count(1),min(taskscode),0 from odc_alllable where pcbasn = '" + tt_sn + "' ";
            string tt_sql2 = "select count(1),min(taskscode),0 from odc_alllable where maclable = '" + tt_sn + "' ";
            string tt_sql3 = "select count(1),min(taskscode),0 from odc_alllable where hostlable = '" + tt_sn + "' ";
            if (tt_datatype == "1") tt_sql = tt_sql1;  //单板
            if (tt_datatype == "2") tt_sql = tt_sql2;  //MAC
            if (tt_datatype == "3") tt_sql = tt_sql3;  //主机条码

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

        //获取MAC
        private string Getmaclable(string tt_hostlable)
        {
            string tt_maclable = "";

            string tt_sql = "select count(1), min(hostlable), min(maclable) " +
                            "from odc_alllable where hostlable = '" + tt_hostlable + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArrayTwo(tt_sql, tt_conn);

            if (tt_array[0] == "1")
            {
                tt_maclable = tt_array[2];
            }
            else
            {
                MessageBox.Show("网络连接失败，或此生产序列号" + tt_hostlable + "不存在，请确认");
            }

            return tt_maclable;
        }

        //打印数记录
        private void SetPrintRecord(string tt_task, string tt_mac, string tt_host, string tt_local, string tt_user, string tt_computername, string tt_remark)
        {
            string tt_insertsql = "insert into odc_lablereprint (Ftaskcode,Fmaclable,Fhostlable,Flocal,Fname,Fdate,Fcomputername,Fremark) " +
                       "values('" + tt_task + "','" + tt_mac + "','" + tt_host + "','" + tt_local + "','" + tt_user + "',getdate(),'" + tt_computername + "','" + tt_remark + "') ";

            int tt_intcount = Dataset1.ExecCommand(tt_insertsql, tt_conn);

            if (tt_intcount > 0)
            {
                tt_reprinttime++;
            }
        }

        //查询是否存在栈板号和出货数据
        private bool Getaddress(string tt_pagesn, out string tt_polletsn, out string tt_faddress)
        {
            tt_polletsn = "";
            tt_faddress = "";

            string tt_sql = "select count(1), min(polletsn), min(faddress) " +
                            "from odc_package where pagesn = '" + tt_pagesn + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArrayTwo(tt_sql, tt_conn);

            if (int.Parse(tt_array[0]) >= 1)
            {
                tt_polletsn = tt_array[1];
                tt_faddress = tt_array[2];
                if (tt_polletsn == "" && tt_faddress == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                MessageBox.Show("网络连接失败，或此箱号" + tt_pagesn + "不存在，请确认");
                return false;
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

        //查询重打记录2
        private bool CheckPrintRecordII(string tt_maclable, string tt_flocal)
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

        //获取运营商
        private string GetTelecomOperator(string tt_peoductname)
        {
            string tt_teleplan = "0";

            string tt_sql = "select count(1),min(Fdesc),0 from odc_dypowertype where Ftype = '" + tt_peoductname + "' ";

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

        //1.5A 电源替换1.0A 铭牌防呆查询
        private bool CheckPowerLable(string tt_Dataname,string tt_shortmac,string tt_scanboxsn)
        {
            bool tt_flag = false;
            string tt_sql = "select fremark from " + tt_Dataname + " where fmaclable = '" + tt_shortmac + "'";

            DataSet ds = Dataset1.GetDataSet(tt_sql, tt_conn);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string tt_1A_remark = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                    if (tt_1A_remark.Trim() == "原1.5A产品改为打印1.0A铭牌")
                    {
                        tt_flag = true;
                        setRichtexBox("附加检查：查询到1.5A电源铭牌重打1.0A铭牌的记录,goon");
                        PutLableInfor("");
                        break;
                    }
                }
            }

            if (!tt_flag)
            {
                setRichtexBox("附加检查：没有查询到1.5A电源铭牌重打1.0A铭牌的记录,over");
                PutLableInfor("该产品:" + tt_scanboxsn + "需要重打 1.0A 铭牌！");
            }

            return tt_flag;
        }

        #endregion


        #region 6、数据查询
        //数据查询 确定
        private void button10_Click(object sender, EventArgs e)
        {
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;

            string tt_task = "";
            string tt_pcba = "";
            string tt_mac = "";
            Boolean tt_flag = false;

            string tt_sn1 = this.textBox10.Text.Trim();
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
        private void button11_Click(object sender, EventArgs e)
        {
            this.textBox10.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;
        }
        #endregion


        #region 7、MD5码计算

        //获取文件
        private void button14_Click(object sender, EventArgs e)
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
        private void button13_Click(object sender, EventArgs e)
        {
            string tt_fliename = this.textBox12.Text;

            string tt_md5 = GetMD5HashFromFile(tt_fliename);

            this.textBox13.Text = tt_md5;
        }

        //重置
        private void button12_Click(object sender, EventArgs e)
        {
            this.textBox12.Text = null;
            this.textBox13.Text = null;
        }
        #endregion


        #region 8、订单查询
        //订单查询 确定
        private void button16_Click(object sender, EventArgs e)
        {
            this.dataGridView6.DataSource = null;

            string tt_task = this.textBox11.Text.Trim();

            string tt_page = "";

            if ( this.textBox14.Text.Trim() != "")
            {
                tt_page = " and T2.pagesn = '" + this.textBox14.Text.Trim() + "' ";
            }


            string tt_sql1 = "select hprintman 总工单,taskscode 子工单, pagesn 箱号,pcbasn 单板号,hostlable 主机条码,maclable MAC, " +
                             "boxlable 生产序列号,Bosasn BOSA, shelllable GPSN, Smtaskscode 串号, Dystlable 电源号, " +
                             "sprinttime 关联时间, pagetime 装箱时间 " +

                            "from odc_alllable T1 " +

                            "left outer join odc_package T2 " +
                            "on T1.pcbasn = T2.pasn " +


                            "where T1.taskscode = '" + tt_task + "' " + tt_page;

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

        //订单查询 重置
        private void button15_Click(object sender, EventArgs e)
        {
            this.textBox11.Text = null;
            this.textBox14.Text = null;
            this.dataGridView6.DataSource = null;
        }

        //显示行号
        private void dataGridView6_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SolidBrush brushOne = new SolidBrush(Color.Red);
            e.Graphics.DrawString(Convert.ToString(e.RowIndex + 1, System.Globalization.CultureInfo.CurrentUICulture), e.InheritedRowStyle.Font, brushOne, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
        }
        #endregion
        

        #region 9、锁定功能
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
                    if (line.Contains("BoxPrintMode"))
                    {
                        BoxPrintMode = line.Substring(line.IndexOf("=") + 1).Trim();
                    }
                }

                if (str.Contains("FH105") || str.Contains("FH106"))
                {
                    this.button3.Visible = true;
                    this.button4.Visible = true;
                    this.button7.Visible = true;
                    this.tabPage4.Parent = tabControl2;
                    //获取调试开始时间
                    tt_reprintstattime = DateTime.Now;
                }

                tt_computermac = Dataset1.GetHostIpName();
				
                string tt_sql1 = "select tasksquantity,product_name,fec,convert(varchar, taskdate, 23) fdate,tasktype,softwareversion,gyid,pon_name,areacode,gyid2,parenttask,fhcode " +
                                 "from odc_tasks where taskscode = '" + this.textBox1.Text + "' ";                

                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);

                tt_gyid_Old = "";

                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    
                    this.label9.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    string tt_productname = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //产品名称
                    this.label11.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //EC代码
                    this.label12.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString();  //生产日期
                    this.label25.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //物料编码
                    this.label57.Text = ds1.Tables[0].Rows[0].ItemArray[5].ToString();  //软件版本
                    this.label52.Text = ds1.Tables[0].Rows[0].ItemArray[6].ToString();  //流程编码
                    tt_pon_name = ds1.Tables[0].Rows[0].ItemArray[7].ToString();  //产品类型
                    tt_areacode = ds1.Tables[0].Rows[0].ItemArray[8].ToString();  //生产地区
                    tt_gyid_Old = ds1.Tables[0].Rows[0].ItemArray[9].ToString();  //次级流程配置
                    tt_MiniType = ds1.Tables[0].Rows[0].ItemArray[10].ToString().Trim();  //小型化配置方案

                    tt_power_old = ds1.Tables[0].Rows[0].ItemArray[11].ToString().Trim();  //旧电源适配器标识
                    tt_power_re = ds1.Tables[0].Rows[0].ItemArray[10].ToString().Trim();  //旧电源适配器标识(需重打检查)

                    tt_CMCCQR_DateCheck = int.Parse(this.label12.Text.Replace("-", ""));//移动产品新二维码用判断参数

                    tt_gyid_Use = "";

                    tt_allprocesses = null;
                    tt_partprocesses = null;
                    tt_routdataset = null;
                    tt_allroutdataset = null;

                    if (tt_productname == "HG6201G" || tt_productname == "HG6201GW" || tt_productname == "HG6201GS")
                    {
                        this.label10.Text = "HG6201M";
                    }
                    else
                    {
                        this.label10.Text = tt_productname;
                    }

                    string tt_eccode = this.label11.Text;

                    if ((tt_productname == "HG6201T" || tt_productname == "HG2201T") && tt_areacode == "海南")
                    {
                        tt_QRDZ = 1;
                    }

                    if ((tt_productname == "HG6201T" || tt_productname == "HG2201T") && tt_areacode == "四川")
                    {
                        tt_QRDZ = 1;
                    }

                    if ((tt_productname == "HG6201M") && tt_areacode == "浙江")
                    {
                        tt_QRDZ = 1;
                    }

                    if ((tt_productname == "HG6201U" || tt_productname == "HG2201U") && tt_areacode == "天津")
                    {
                        tt_QRDZ = 1;
                    }

                    //第一步、流程检查
                    #region
                    Boolean tt_flag1 = false;
                    string tt_gyid = this.label52.Text;
                    if (!tt_gyid.Equals(""))
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
                    #endregion                   

                    //第二步、查找模板路径
                    #region
                    Boolean tt_flag2 = false;
                    string tt_checkpath1 = "";
                    string tt_checkpath2 = "";
                    string tt_md51 = "";
                    string tt_md52 = "";
                    if (tt_flag1)
                    {
                        string tt_sql2 = "select Docdesc, Fpath05,Fdata05, Fmd05, "+
                                                         "Fpath06,Fdata06, Fmd06  " +
                                          " from odc_ec  where zjbm = '" + tt_eccode + "' ";
                        DataSet ds2 = Dataset1.GetDataSet(tt_sql2, tt_conn);
                        if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                        {
                            tt_flag2 = true;

                            this.label13.Text = ds2.Tables[0].Rows[0].ItemArray[0].ToString();  //EC描述

                            this.label40.Text = ds2.Tables[0].Rows[0].ItemArray[1].ToString();  //中箱标签一
                            this.label43.Text = ds2.Tables[0].Rows[0].ItemArray[2].ToString();
                            this.label42.Text = ds2.Tables[0].Rows[0].ItemArray[3].ToString();

                            this.label41.Text = ds2.Tables[0].Rows[0].ItemArray[4].ToString();  //中箱标签二
                            this.label44.Text = ds2.Tables[0].Rows[0].ItemArray[5].ToString();
                            this.label45.Text = ds2.Tables[0].Rows[0].ItemArray[6].ToString();

                            tt_checkpath1 = Application.StartupPath + this.label40.Text;
                            tt_checkpath2 = Application.StartupPath + this.label41.Text;
                            tt_md51 = this.label42.Text;
                            tt_md52 = this.label45.Text;

                        }
                        else
                        {
                            MessageBox.Show("没有找到工表的EC表配置信息，请确认！");
                        }

                    }
                    #endregion

                    //第三步串号设置
                    #region
                    Boolean tt_flag3 = false;
                    if (tt_flag2)
                    {

                        string tt_sql4 = "select count(1),min(hostqzwh),0 from ODC_HOSTLABLEOPTIOAN " +
                                         "where taskscode = '" + this.textBox1.Text + "' ";
                        string[] tt_array4 = new string[3];
                        tt_array4 = Dataset1.GetDatasetArray(tt_sql4, tt_conn);
                        if (tt_array4[0] == "1")
                        {
                            tt_flag3 = true;
                            this.label15.Text = tt_array4[1];

                        }
                        else
                        {
                            MessageBox.Show("没有找到工单的主机表配置表ODC_HOSTLABLEOPTIOAN信息，请确认！");
                        }

                    }
                    #endregion

                    //第三步附一 模板选择
                    #region
                    Boolean tt_flag11 = false;
                    if (tt_flag3)
                    {
                        string tt_sbxh = "";
                        string tt_sql11 = "select wifi from odc_dypowertype where ftype = '" + tt_productname + "' ";
                        DataSet ds11 = Dataset1.GetDataSetTwo(tt_sql11, tt_conn);

                        if (ds11.Tables.Count > 0 && ds11.Tables[0].Rows.Count > 0)
                        {
                            tt_sbxh = ds11.Tables[0].Rows[0].ItemArray[0].ToString(); //设备特征

                            bool tt_flag11_1 = false;
                            bool tt_flag11_2 = false;
                            tt_flag11_1 = CheckStrContain(tt_sbxh, "单频");
                            tt_flag11_2 = CheckStrContain(tt_sbxh, "常规");
                            PutLableInfor("");

                            if (BoxPrintMode == "1")
                            {
                                this.checkBox8.Checked = true;
                                this.checkBox5.Visible = true;
                                this.checkBox5.Checked = true;
                                this.checkBox6.Visible = true;
                                this.checkBox6.Checked = true;
                                tt_flag11 = true;
                            }
                            else if (str.Contains("FH005") || str.Contains("FH105"))
                            {
                                this.checkBox8.Checked = true;

                                if (tt_flag11_1 || tt_flag11_2)
                                {
                                    this.checkBox5.Checked = true;
                                    this.checkBox6.Checked = false;
                                    this.checkBox6.Visible = false;
                                }
                                else if (tt_sbxh == "")
                                {
                                    this.checkBox5.Checked = true;
                                    this.checkBox6.Checked = false;
                                }
                                else
                                {
                                    this.checkBox5.Checked = true;
                                    this.checkBox6.Visible = true;
                                    this.checkBox6.Checked = true;
                                }

                                tt_flag11 = true;
                            }
                            else if (str.Contains("FH006") || str.Contains("FH106"))
                            {
                                this.checkBox8.Checked = true;

                                if (tt_flag11_1 || tt_flag11_2)
                                {
                                    this.checkBox5.Checked = false;
                                    this.checkBox5.Visible = false;
                                    this.checkBox6.Checked = true;
                                }
                                else if (tt_sbxh == "")
                                {
                                    this.checkBox5.Checked = false;
                                    this.checkBox6.Checked = true;
                                }
                                else
                                {
                                    this.checkBox5.Visible = false;
                                    this.checkBox6.Visible = false;
                                }

                                tt_flag11 = true;
                            }

                        }
                        else
                        {
                            MessageBox.Show("没有设备形态，请确认数据库设置");
                        }
                    } 
                     #endregion

                    //第三步附二 生产序列号特征码查询
                    #region
                    Boolean tt_flag12 = false;
                    if (tt_flag11)
                    {
                        bool tt_flag12_1 = false;
                        string tt_sql12 = "select hostqzwh from odc_hostlableoptioan where taskscode = '" + this.textBox1.Text + "' ";
                        DataSet ds12 = Dataset1.GetDataSetTwo(tt_sql12, tt_conn);

                        if (ds12.Tables.Count > 0 && ds12.Tables[0].Rows.Count > 0)
                        {
                            this.textBox6.Text = ds12.Tables[0].Rows[0].ItemArray[0].ToString(); //生产序列号特征码
                            this.textBox7.Text = this.textBox6.Text;

                            this.checkBox3.Checked = true;
                            this.checkBox4.Checked = true;
                            tt_flag12_1 = true;
                        }
                        else
                        {
                            MessageBox.Show("无法获取生产序列号，请确认制造单填写是否正确");
                        }

                        if (tt_flag12_1)
                        {
                            if ((tt_productname == "HG6201M" || tt_productname == "HG6821M") && tt_areacode == "安徽")
                            {
                                string tt_sql12_2 = "select count(1),min(hostmode),min(hostmax) from ODC_HOSTLABLEOPTIOAN " +
                                                    "where taskscode = '" + this.textBox1.Text + "' ";
                                string[] tt_array12_2 = new string[3];
                                tt_array12_2 = Dataset1.GetDatasetArray(tt_sql12_2, tt_conn);
                                if (tt_array12_2[0] == "1")
                                {
                                    tt_beforstranhui = tt_array12_2[1].ToUpper().Trim();
                                    string tt_beforstrCheck = tt_beforstranhui.Substring(tt_beforstranhui.Length - 10, 8);
                                    if (tt_beforstrCheck != this.label12.Text.Replace("-", ""))
                                    {
                                        MessageBox.Show("子工单" + this.textBox1.Text + "的安徽箱号前段号信息" + tt_beforstranhui + "日期与制造单日期不一致，请确认！");
                                    }
                                    else
                                    {
                                        tt_flag12 = true;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("没有找到子工单" + this.textBox1.Text + "的安徽箱号前段号信息，或有子工单号配置表重复,查询结果返回值" + tt_array12_2[0] + "请确认！");
                                }
                            }
                            else
                            {
                                tt_flag12 = true;
                            }
                        }
                    }
                    #endregion

                    //第四步 模板一路径检查
                    #region
                    Boolean tt_flag4 = false;
                    if (tt_flag12)
                    {
                        tt_flag4 = GetPathIstrue(tt_checkpath1);
                        if (!tt_flag4)
                        {
                            MessageBox.Show(" 找不到模板一文件：" + tt_checkpath1 + "，请确认！");
                        }
                    }
                    #endregion

                    //第五步 模板二路径检查
                    #region
                    Boolean tt_flag5 = false;
                    if (tt_flag4 && this.checkBox6.Checked == true)
                    {
                        tt_flag5 = GetPathIstrue(tt_checkpath2);
                        if (!tt_flag5)
                        {
                            MessageBox.Show(" 找不到模板二文件：" + tt_checkpath2 + "，请确认！");
                        }
                    }
                    else if (tt_flag4 && this.checkBox6.Checked == false)
                    {
                        tt_flag5 = true;
                    }
                   #endregion

                    //第六步 模板一特征码检验
                    #region
                    Boolean tt_flag6 = false;
                    if (tt_flag5)
                    {
                        string tt_md61 = GetMD5HashFromFile(tt_checkpath1);

                        //if (tt_md51 == tt_md61)
                        //{
                            tt_flag6 = true;
                        //}
                        //else
                        //{
                        //    MessageBox.Show("系统设定模板一MD5码: '" + tt_md51 + "'与你使用模板的MD5码：'" + tt_md61 + "'不一致，请确认！");
                        //}
                    }
                    #endregion

                    //第七步 模板二特征码检验
                    #region
                    Boolean tt_flag7 = false;
                    if (tt_flag6 && this.checkBox6.Checked == true)
                    {
                        string tt_md62 = GetMD5HashFromFile(tt_checkpath2);

                        //if (tt_md52 == tt_md62)
                        //{
                            tt_flag7 = true;
                        //}
                    //else
                    //{
                    //    MessageBox.Show("系统设定模板二MD5码: '" + tt_md51 + "'与你使用模板的MD5码：'" + tt_md62 + "'不一致，请确认！");
                    //}
                }
                    else if (tt_flag6 && this.checkBox6.Checked == false)
                    {
                        tt_flag7 = true;
                    }
                    #endregion
                    
                    //第八步 待测站位及序列号检查
                    #region 
                    bool tt_flag8 = false;
                    string tt_testcode = this.label54.Text;
                    string tt_codeserial = this.label63.Text;
                    if (tt_flag7)
                    {
                        if (tt_testcode.Equals("") || tt_codeserial.Equals(""))
                        {
                            MessageBox.Show("流程的待测站位，或流程的序列号为空，请检查流程设置");
                        }
                        else
                        {
                            tt_flag8 = true;
                        }
                    }
                    #endregion
                    
                    //第九步 获取站位流程集
                    #region 
                    bool tt_flag9 = false;
                    if (tt_flag8)
                    {
                        string tt_sql14 = "select pxid from odc_routing  where pid = " + tt_gyid + "  and LCBZ > 1 and LCBZ < '" + tt_codeserial + "' ";
                        tt_routdataset = Dataset1.GetDataSetTwo(tt_sql14, tt_conn);
                        if (tt_routdataset.Tables.Count > 0 && tt_routdataset.Tables[0].Rows.Count > 0)
                        {
                            tt_flag9 = true;
                            tt_allprocesses = Dataset2.getGyidAllProcess(tt_gyid, tt_conn);
                            tt_partprocesses = Dataset2.getGyidPartProcess(tt_routdataset);
                            tt_allroutdataset = Dataset2.getGyidAllProcessDt(tt_gyid, tt_conn);
                        }
                        else
                        {
                            MessageBox.Show("没有找到流程:" + tt_gyid + "，的流程数据集Dataset，请流程设置！");
                        }
                    }
                    #endregion
                    
                    //第十步获取装箱设定
                    #region
                    bool tt_flag10 = false;
                    if (tt_flag9)
                    {
                        string tt_sql10 = "select count(1),min(fboxset),0 from odc_dypowertype " +
                                          "where ftype = '" + tt_productname + "' ";

                        string[] tt_array10 = new string[3];
                        tt_array10 = Dataset1.GetDatasetArray(tt_sql10, tt_conn);
                        if (tt_array10[0] == "1")
                        {
                            tt_flag10 = true;
                            if ((tt_productname == "HG6201T" || tt_productname == "HG2201T") && tt_areacode == "安徽")
                            {
                                this.label77.Text = "20";
                            }
                            else if (tt_MiniType == "小型化方案")
                            {
                                this.label77.Text = "20";
                            }
                            else
                            {
                                this.label77.Text = tt_array10[1];
                            }
                            this.textBox2.Text = this.label77.Text;
                            this.checkBox2.Checked = true;
                        }
                        else
                        {
                            MessageBox.Show("没有找到产品型号:" + tt_productname + "的配置表odc_dypowertype对应的装箱设定信息，对应字段:fboxset,请确认！");
                        }

                    }
                    #endregion
                    
                    //最后判断
                    #region
                    if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10 && tt_flag11 && tt_flag12)
                    {
                        this.textBox1.Enabled = false;
                        this.textBox4.Visible = true;
                        this.textBox9.Visible = true;
                        //this.button17.Visible = false;
                        getPackageNunber(this.textBox1.Text);
                        //MessageBox.Show("---OK---，这是烽火移动双频装箱，模板文件不同步，可以去点击左上方的文件按钮进行文件的同步");
                    }
                    #endregion
                    
                }
                else
                {
                    MessageBox.Show("没有查询此工单，请确认！");
                }
            }
            else
            {
                ClearLabelInfo1();
                ClearLabelInfo4();
                this.textBox1.Enabled = true;
                this.textBox4.Visible = false;
                this.textBox9.Visible = false;
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
                this.dataGridView1.Visible = true;
                this.button3.Visible = false;
                this.button4.Visible = false;
                this.button7.Visible = false;
                this.tabPage4.Parent = null;
                this.tabPage3.Parent = tabControl2;
                tt_QRDZ = 0;//重置是否检查二维码定制标签
                //this.button17.Visible = true;
                tt_MiniType = "";//小型化方案用
                tt_CMCCQR_DateCheck = 30000000; //移动产品新二维码用判断参数
            }
        }

         //工单选择
        private bool getChoiceTaskcode()
        {
            Boolean tt_flag = false;

            return tt_flag;
        }


        //装箱设定锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {

                MessageBox.Show("确定装箱数量，是20箱还是10箱，不要随意改变装箱设定，否则会导致箱号计算错误，不满一箱装箱，点击生成尾箱");
                this.textBox2.Enabled = false;
            }
            else
            {
                this.textBox2.Enabled = true;
            }
        }


        //装箱打印 位数锁定
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked)
            {
                this.textBox5.Enabled = false;
                this.textBox6.Enabled = false;
            }
            else
            {
                this.textBox5.Enabled = true;
                this.textBox6.Enabled = true;
            }
        }

        //标签重打 位数锁定
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked)
            {
                this.textBox8.Enabled = false;
                this.textBox7.Enabled = false;
            }
            else
            {
                this.textBox8.Enabled = true;
                this.textBox7.Enabled = true;
            }
        }

        #endregion


        #region 10、按钮功能
        //重置
        private void button1_Click(object sender, EventArgs e)
        {
            ClearLabelInfo2();
            textBox4.Focus();
            textBox4.SelectAll();
            setRichtexBox("");
        }

        //装箱打印与标签重打页签切换
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //装箱打印
            if (tabControl2.SelectedTab == tabPage3)
            {
                ClearLabelInfo2();
                textBox4.Focus();
                textBox4.SelectAll();
            }

            //标签重打
            if (tabControl2.SelectedTab == tabPage4)
            {
                ClearLabelInfo2();
                textBox9.Focus();
                textBox9.SelectAll();
            }
        }


        ////文件同步
        //private void button17_Click(object sender, EventArgs e)
        //{
        //    MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
        //    DialogResult dr = MessageBox.Show("确定要删除:" + tt_delepath + "文件，并复制目录:" + tt_copypath + ",中的文件吗?，复制点击确定", "文件复制", messButton);

        //    if (dr == DialogResult.OK)//如果点击“确定”按钮
        //    {
        //        int tt_delint = DelectDir3(tt_delepath);
        //        int tt_copyint = CopyFolderTo2(tt_copypath, tt_delepath);
        //        MessageBox.Show("已删除了：" + tt_delint.ToString() + "个文件，已复制了：" + tt_copyint.ToString() + "个文件");

        //    }
        //    else
        //    {

        //    }
        //}


    private void button5_Click(object sender, EventArgs e)//尾箱
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("需装箱数量：" + this.textBox2.Text + "，已装箱数量：" + this.textBox3.Text + ",确定生成尾箱码", "生成尾箱", messButton);

            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {
                //第一步获取箱号
                string tt_package = "";
                //第一步获取箱号
                if ((this.label10.Text == "HG6201M" || this.label10.Text == "HG6821M") && tt_areacode == "安徽")
                {
                    tt_package = GetBoxNumber6(tt_beforstranhui, this.textBox1.Text, this.textBox2.Text);
                }
                else if ((this.label10.Text == "HG6201T" || this.label10.Text == "HG2201T") && tt_areacode == "安徽")
                {
                    tt_package = GetBoxNumber7(label15.Text, this.textBox1.Text, this.textBox2.Text);
                }
                else
                {
                    tt_package = GetBoxNumber4(label15.Text, this.label47.Text, this.textBox2.Text);  //中箱分箱
                }

                label46.Text = tt_package;

                //第二步 装箱过站
                Boolean tt_passflage = false;

                if (tt_package != "")
                {
                    tt_passflage = ListViewStatioPass(this.textBox1.Text, tt_gyid_Use, tt_ccode, tt_ncode, tt_package, tt_conn);
                }

                //第三步数据清理
                if (tt_passflage)
                {
                    GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                    //如果是勾选打印3站，那么再打印两张
                    if (this.checkBox8.Checked)
                    {
                        GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                        GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                    }
                    ClearLabelInfo3();
                }

                CheckStation(this.textBox1.Text, tt_package);
                this.richTextBox1.BackColor = Color.Chartreuse;
            }
            else
            {
                textBox4.Focus();
                textBox4.SelectAll();
            }
        }

        //打散
        private void button4_Click(object sender, EventArgs e)
        {
            string tt_polletsn = "";
            string tt_faddress = "";

            if (this.label46.Text.Length > 0 && Getaddress(this.label46.Text,out tt_polletsn,out tt_faddress))
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("确要打散箱号：" + this.label46.Text + ",吗", "装箱打散", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_taskcode = this.textBox1.Text;
                    string tt_pagesnbreak = this.label46.Text;
                    string tt_ccodebreak = this.label54.Text;
                    Boolean tt_flag = Dataset1.FhBreakupPackage(tt_taskcode, tt_pagesnbreak, tt_ccodebreak, tt_conn);

                    if (tt_flag)
                    {
                        MessageBox.Show("OK 打散成功");
                        ClearListView();
                    }
                    else
                    {
                        MessageBox.Show("fail 打散失败请检查");
                    }
                }
            }
            else if (tt_faddress != "")
            {
                MessageBox.Show("此箱已存在出货数据：" + tt_faddress + "，不允许打散");
            }
            else if (tt_polletsn != "")
            {
                MessageBox.Show("此箱未打散栈板：" + tt_polletsn + "，不允许打散");
            }
            else
            {
                MessageBox.Show("箱号为空，无法打散，请确认");
            }
        }


        //模板一 预览
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {
               

                string tt_prientcode = this.label53.Text;
                string tt_checkcode = this.label54.Text;

                Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                if (tt_flag)
                {
                    GetParaDataPrint(2, true, false, false); ;  //预览
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
        }

        //模板一 打印
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
                DialogResult dr = MessageBox.Show("确定要重打标签吗，打印信息被记录", "标签重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label53.Text;
                    string tt_checkcode = this.label54.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                    if (tt_flag)
                    {
					    Reprint form1 = new Reprint();
                        form1.StartPosition = FormStartPosition.CenterScreen;
                        form1.ShowDialog();

                        string tt_remark = Dataset1.Context.ContextData["Key1"].ToString();
                        GetParaDataPrint(1, true, false, false);  //打印
                        string tt_taskscode = this.textBox1.Text.Trim().ToUpper();
                        string tt_host = this.label46.Text;
                        string tt_recordmac = this.label46.Text;
                        string tt_local = "中箱I型标签";
                        string tt_username = "";
                        if (str.Contains("FH005") || str.Contains("FH006"))
                        {
                            tt_username = this.comboBox2.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }
                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);
                    }
                    else
                    {
                        MessageBox.Show("当前站位：" + tt_prientcode + "必须大于待测站位：" + tt_checkcode + ",才能重打标签");
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
            else
            {
                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再重打");
            }

            tt_reprintstattime = DateTime.Now;
        }

        //模板二 预览
        private void button6_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {


                string tt_prientcode = this.label53.Text;
                string tt_checkcode = this.label54.Text;

                Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                if (tt_flag)
                {
                    GetParaDataPrint(2, false, true, false);  //预览
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
        }

        //模板二 打印
        private void button7_Click(object sender, EventArgs e)
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
                DialogResult dr = MessageBox.Show("确定要重打标签吗，打印信息被记录", "标签重打", messButton);
                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label53.Text;
                    string tt_checkcode = this.label54.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                    if (tt_flag)
                    {
                        Reprint form1 = new Reprint();
                        form1.StartPosition = FormStartPosition.CenterScreen;
                        form1.ShowDialog();

                        string tt_remark = Dataset1.Context.ContextData["Key1"].ToString();
                        GetParaDataPrint(1, false, true, false);  //打印
                        string tt_taskscode = this.textBox1.Text.Trim().ToUpper();
                        string tt_host = this.label46.Text;
                        string tt_recordmac = this.label46.Text;
                        string tt_local = "中箱II型标签";
                        string tt_username = "";
                        if (str.Contains("FH005") || str.Contains("FH006"))
                        {
                            tt_username = this.comboBox2.Text;
                        }
                        else
                        {
                            tt_username = "工程账号重打";
                        }
                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);
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
                else
                {

                }
            }
            else
            {
                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再预重打");
            }

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
                    comboBox2.DataSource = ds1.Tables[0];
                    comboBox2.DisplayMember = "fusername";
                    this.groupBox22.Visible = true;
                    this.groupBox8.Visible = false;
                    this.groupBox9.Visible = false;
                    this.dataGridView1.Visible = false;
                    this.comboBox1.Text = "0.3";
                    this.comboBox2.Text = "下拉选择";
                    this.textBox27.Text = "";
                    this.textBox28.Text = "";
                    this.comboBox2.Enabled = true;
                    this.textBox27.Enabled = true;
                    this.textBox28.Enabled = true;
                    this.groupBox23.Visible = false;
                    this.button3.Visible = false;
                    this.button4.Visible = false;
                    this.button7.Visible = false;
                    this.tabPage4.Parent = null;
                    this.tabPage3.Parent = tabControl2;
                    this.textBox9.Enabled = true;
                    this.textBox9.Text = "";
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
                    //if (str.Contains("FH005"))
                    //{
                    //    this.button3.Visible = true;
                    //}
                    this.button4.Visible = true;
                    if (str.Contains("FH006") || BoxPrintMode == "1")
                    {
                        this.button7.Visible = true;
                    }
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
            this.button4.Visible = false;
            this.button7.Visible = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
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
            this.dataGridView1.Visible = true;
            this.button3.Visible = false;
            this.button4.Visible = false;
            this.button7.Visible = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
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
            this.dataGridView1.Visible = true;
            this.button3.Visible = false;
            this.button4.Visible = false;
            this.button7.Visible = false;
            this.tabPage4.Parent = null;
            this.tabPage3.Parent = tabControl2;
        }

        #endregion


        #region 11、扫描功能
        //标签重打扫描
        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                //开始SN扫描
                #region
                ScanDataInitial();
                setRichtexBox("------开始装箱重打扫描--------");
                string tt_scanboxsn = this.textBox9.Text.Trim();
                string tt_task = this.textBox1.Text.Trim();
                string tt_gesn = "";
                string tt_pcba = "";
                string tt_maclable = "";
                string tt_boxsn = "";
                string tt_barcode = "";
                string tt_barcode1 = "";
                string tt_longmac = "";
                #endregion


                //第一步位数判断
                #region
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanboxsn, this.textBox8.Text);
                #endregion


                //第二步包含符判断
                #region
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanboxsn, this.textBox7.Text.Trim());
                }
                #endregion


                //第三步判断是否有箱号
                #region
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    string tt_sql3 = "select count(1), max(T2.pagesn),0 " +
                                     "from odc_alllable T1 " +
                                     "left outer join odc_package T2 on T1.pcbasn = T2.pasn " +
                                     "where T1.taskscode = '" + tt_task + "' and T1.hostlable = '" + tt_scanboxsn + "' ";
                    string[] tt_array3 = new string[3];
                    tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                    if (tt_array3[0] == "1")
                    {
                        tt_flag3 = true;
                        tt_gesn = tt_array3[1].ToUpper();
                        this.label46.Text = tt_gesn;
                        setRichtexBox("3、找到一个箱号：" + tt_gesn + ", goon");
                    }
                    else
                    {
                        string tt_querytask = getSnRealTask("3", tt_scanboxsn);
                        setRichtexBox("3、该SN包装表中没有找到箱号或有多个箱号，可能该产品工单是：" + tt_querytask + ",返回箱号值：" + tt_array3[0] + ",over");
                        PutLableInfor("该产品工单可能是:" + tt_querytask + ",获取箱号返回值：" + tt_array3[0]);
                    }
                }
                #endregion
                

                //第四步查找整箱数据
                #region
                Boolean tt_flag4 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    string tt_sql4 = "select  T2.hostlable,T2.pcbasn,T2.maclable,T2.SMtaskscode, T2.shelllable " +
                                     "from odc_package T1 " +
                                     "left outer join odc_alllable T2 on T1.pasn = T2.pcbasn " +
                                     "where T1.taskcode = '" + tt_task + "'  and T1.pagesn = '" + tt_gesn + "'  order by T2.hostlable";

                    DataSet ds4 = Dataset1.GetDataSet(tt_sql4, tt_conn);
                    if (ds4.Tables.Count > 0 && ds4.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        setRichtexBox("4、关联表查询到:" + ds4.Tables[0].Rows.Count.ToString() + "条数据，,goon");
                        this.textBox3.Text = ds4.Tables[0].Rows.Count.ToString();

                        for (int i = 0; i < ds4.Tables[0].Rows.Count; i++)
                        {
                            tt_boxsn = ds4.Tables[0].Rows[i].ItemArray[0].ToString().ToUpper();
                            tt_pcba = ds4.Tables[0].Rows[i].ItemArray[1].ToString().ToUpper();
                            tt_maclable = ds4.Tables[0].Rows[i].ItemArray[2].ToString().ToUpper();
                            tt_barcode = ds4.Tables[0].Rows[i].ItemArray[3].ToString().ToUpper();
                            tt_longmac = ds4.Tables[0].Rows[i].ItemArray[4].ToString().ToUpper();
                            tt_barcode1 = Regex.Replace(tt_barcode," ", "").ToUpper();
                            PutListViewData(tt_boxsn, tt_pcba, tt_maclable, tt_barcode, tt_longmac, tt_barcode1);

                            if (i == 0)
                            {
                                label47.Text = tt_boxsn;
                            }

                            if (i == ds4.Tables[0].Rows.Count - 1)
                            {
                                label48.Text = tt_boxsn;
                            }
                        }
                    }
                    else
                    {
                        setRichtexBox("4、该箱号:" + tt_gesn + ",没找到数据关联表没有查询到数据，over");
                        PutLableInfor("该箱号:" + tt_gesn + ",没找到数据，请检查！");
                    }
                }
                #endregion


                //最后判断
                #region
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    GetParaDataPrint(0, true, false, false);
                    CheckStation(tt_task, tt_gesn);
                    this.richTextBox1.BackColor = Color.Chartreuse;

                    if (str.Contains("FH006") || str.Contains("FH106"))
                    {
                        if (tt_reprintmark == "0")
                        {
                            this.textBox9.Enabled = false;
                        }
                    }
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;

                }
                #endregion


                //移动光标
                GetProductRhythm();
                textBox9.Focus();
                textBox9.SelectAll();
            }
        }

        //SN过站扫描
        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {

                //开始SN过站扫描,数据准备
                #region
                this.label39.Text = null;
                if (tt_scanboxnum == 0)
                ClearLabelInfo4();
                setRichtexBox("------开始装箱过站扫描--------");
                string tt_scanboxsn = this.textBox4.Text.Trim().ToUpper();
                string tt_task = this.textBox1.Text.Trim().ToUpper();
                string tt_gesn = "";
                string tt_pcba = "";
                string tt_shortmac = "";
                string tt_smtaskscode = "";
                string tt_barcode1 = "";
                string tt_longmac = "";
                #endregion


                //第一步位数判断
                #region
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanboxsn, this.textBox5.Text);
                #endregion


                //第二步包含符判断
                #region
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain2(tt_scanboxsn, this.textBox6.Text.Trim());
                }
                #endregion


                //第三步装箱数量锁定
                #region
                Boolean tt_flag3 = false;
                if (tt_flag2)
                {
                    if (this.checkBox2.Checked)
                    {
                        string tt_boxsysset = this.label77.Text;
                        string tt_boxmanset = this.textBox2.Text;
                        if (tt_boxsysset.Contains(tt_boxmanset))
                        {
                            tt_flag3 = true;
                            setRichtexBox("3、工单装箱数量已锁定，且与系统设定的装箱数一致,都是:" + tt_boxsysset + ",goon");
                        }
                        else
                        {
                            setRichtexBox("3、工单装箱数量已锁定，且与系统设定的装箱数不一致,系统设定数为:"+tt_boxsysset+",人工设定数为:"+tt_boxmanset+",over");
                            PutLableInfor("装箱数量不一致，系统设定:" + tt_boxsysset + ",人工设定数为:" + tt_boxmanset);
                        }

                    }
                    else
                    {
                        setRichtexBox("3、工单装箱数量没有锁定，over");
                        PutLableInfor("工单装箱数量没有锁定，请检查！");
                    }
                }
                #endregion


                //第四步是否重复扫描
                #region
                Boolean tt_flag4 = false;
                if (tt_flag3)
                {
                    Boolean tt_repeat = CheckNumberRepeat(tt_scanboxsn);
                    if (tt_repeat && (str.Contains("FH005") || str.Contains("FH105")))
                    {
                        setRichtexBox("4、装箱有重复扫描了，end");
                        PutLableInfor("不能重复扫描此产品！");
                    }
                    else if (tt_repeat && (str.Contains("FH006") || str.Contains("FH106")))
                    {
                        tt_flag4 = true;
                        setRichtexBox("4、GPSN不检查此项，goon");
                    }
                    else
                    {
                        tt_flag4 = true;
                        setRichtexBox("4、没有重复扫描了，goon");
                    }
                }
                #endregion
                

                //第五步是否按顺序扫描
                #region
                Boolean tt_flag5 = false;
                if (tt_flag4)
                {
                    if (tt_scanboxnum == 0 && (str.Contains("FH005") || str.Contains("FH105")))
                    {
                        tt_flag5 = true;
                        setRichtexBox("5、这个是第一个产品，不用检查数据，goon");
                    }
                    else if (str.Contains("FH005") || str.Contains("FH105"))
                    {
                        int tt_count = this.listView1.Items.Count;
                        if (tt_count > 0)
                        {
                            string tt_box1 = this.listView1.Items[tt_count - 1].SubItems[1].Text;
                            int tt_boxnumber1 = getTransmitStrToInt(tt_box1.Substring(tt_box1.Length - 4, 4));

                            string tt_box2 = this.textBox4.Text;
                            int tt_boxnumber2 = getTransmitStrToInt(tt_box2.Substring(tt_box1.Length - 4, 4));

                            if (tt_boxnumber2 - tt_boxnumber1 == 1)
                            {
                                tt_flag5 = true;
                                setRichtexBox("5、这不是第一个产品，但是按顺序扫描了，goon");
                            }
                            else
                            {
                                setRichtexBox("5、这不是第一个产品，没有按顺序扫描了，goon");
                                PutLableInfor("请按顺序扫描！");
                            }

                        }
                    }
                    else if (str.Contains("FH006") || str.Contains("FH106"))
                    {
                        tt_flag5 = true;
                        setRichtexBox("5、GPSN站位，不用检查数据，goon");
                    }
                }
                #endregion


                //第六步查找关联表数据
                #region
                Boolean tt_flag6 = false;
                if (tt_flag5)
                {
                    string tt_sql6 = "select pcbasn,maclable,smtaskscode,shelllable from odc_alllable " +
                                     "where taskscode = '" + this.textBox1.Text + "' and hostlable = '" + tt_scanboxsn + "' ";

                    DataSet ds6 = Dataset1.GetDataSet(tt_sql6, tt_conn);
                    if (ds6.Tables.Count > 0 && ds6.Tables[0].Rows.Count > 0)
                    {
                        tt_flag6 = true;
                        tt_pcba = ds6.Tables[0].Rows[0].ItemArray[0].ToString().ToUpper();
                        tt_shortmac = ds6.Tables[0].Rows[0].ItemArray[1].ToString().ToUpper();
                        tt_smtaskscode = ds6.Tables[0].Rows[0].ItemArray[2].ToString().ToUpper();
                        tt_longmac = ds6.Tables[0].Rows[0].ItemArray[3].ToString().ToUpper();
                        tt_barcode1 = Regex.Replace(tt_smtaskscode, " ", "");
                        setRichtexBox("6、关联表查询到一条数据，PCBA=" + tt_pcba + ",MAC=" + tt_shortmac + ",smtaskscode=" + tt_smtaskscode + ",goon");
                    }
                    else
                    {
                        string tt_querytask = getSnRealTask("3", tt_scanboxsn);
                        setRichtexBox("6、关联表没有查询到数据，或工单不对，该产品工单可能是:" + tt_querytask + ",over");
                        PutLableInfor("该产品工单是:" + tt_querytask + ",与工单:" + tt_task+",不符");
                    }

                }
                #endregion


                //临时附加检查 查找原1.5A铭牌标签是否已重打为1.0A
                #region
                Boolean tt_flag6_1 = false;
                if (tt_flag6)
                {
                    if (tt_power_re == "1.5A" && tt_power_old != "1.5")
                    {
                        if (CheckPowerLable("odc_lablereprint", tt_shortmac, tt_scanboxsn) || CheckPowerLable("odc_lableprint", tt_shortmac, tt_scanboxsn))
                        {
                            tt_flag6_1 = true;
                        }
                    }
                    else
                    {
                        tt_flag6_1 = true;
                    }
                }
                #endregion


                //第七步  流程检查
                #region
                Boolean tt_flag7 = false;
                tt_ccode = this.label54.Text;
                tt_ncode = this.label55.Text;
                if (tt_flag6 && tt_flag6_1)
                {
                    if (tt_ccode == "" || tt_ncode == "")
                    {
                        setRichtexBox("7、该工单没有配置流程," + tt_ccode + "," + tt_ncode + ",over");
                        PutLableInfor("没有获取到当前待测站位，及下一站位，请检查");
                    }
                    else
                    {
                        tt_flag7 = true;
                        setRichtexBox("7、该工单已配置流程," + tt_ccode + "," + tt_ncode + ",goon");
                    }
                }
                #endregion


                //第八步 装箱数检验(原查找站位信息)
                #region
                Boolean tt_flag8 = false;
                if (tt_flag7)
                {
                    int tt_setboxcount = getTransmitStrToInt(this.textBox2.Text.Trim());
                    int tt_listviewcount = this.listView1.Items.Count;
                    if (tt_listviewcount <= tt_setboxcount)
                    {
                        tt_flag8 = true;
                        setRichtexBox("8、装箱扫描数:" + tt_listviewcount.ToString() + "小于等于装箱设定数:" + tt_setboxcount.ToString() + ",goon");
                    }
                    else
                    {
                        setRichtexBox("8、装箱扫描数:" + tt_listviewcount.ToString() + "大于于等于装箱设定数:" + tt_setboxcount.ToString() + ",over");
                        PutLableInfor("装箱扫描数大于装箱设定数,需要重置画面重新扫描");
                    }


                }
                #endregion


                //第九步是否装箱判断
                #region
                Boolean tt_flag9 = false;
                if (tt_flag8)
                {
                    if (str.Contains("FH005") || str.Contains("FH105"))
                    {
                        string tt_sql9 = "select  count(1),min(pagesn),min(pagetime) from odc_package " +
                                         "where taskcode = '" + tt_task + "' and pasn = '" + tt_pcba + "' ";

                        string[] tt_array9 = new string[3];
                        tt_array9 = Dataset1.GetDatasetArray(tt_sql9, tt_conn);
                        if (tt_array9[0] == "0")
                        {
                            tt_flag9 = true;
                            setRichtexBox("9、该产品还没有装箱，可以装箱,goon");
                        }
                        else
                        {
                            tt_gesn = tt_array9[1].ToUpper();
                            setRichtexBox("9、该产品已装箱，箱号：" + tt_gesn + ",装箱时间：" + tt_array9[2] + "");
                            PutLableInfor("该产品已装箱，箱号：" + tt_gesn);
                        }
                    }
                    if (str.Contains("FH006") || str.Contains("FH106"))
                    {
                        string tt_sql9 = "select  count(1),min(pagesn),min(pagetime) from odc_package " +
                                         "where taskcode = '" + tt_task + "' and pasn = '" + tt_pcba + "' ";

                        string[] tt_array9 = new string[3];
                        tt_array9 = Dataset1.GetDatasetArray(tt_sql9, tt_conn);
                        if (tt_array9[0] == "0")
                        {
                            setRichtexBox("9、该产品还没有装箱，可以装箱,over");
                            PutLableInfor("该产品未装箱");
                        }
                        else
                        {
                            tt_gesn = tt_array9[1];
                            tt_flag9 = true;
                            setRichtexBox("9、该产品已装箱，箱号：" + tt_gesn + ",装箱时间：" + tt_array9[2] + "");
                        }
                    }

                }
                #endregion
                

                //第十步模板检查
                #region
                Boolean tt_flag10 = false;
                if (tt_flag9)
                {

                    Boolean tt_flag91 = checkBox5.Checked;
                    if (tt_flag91)
                    {
                        string tt_path1 = Application.StartupPath + this.label40.Text;
                        tt_flag91 = GetPathIstrue(tt_path1);
                        if (tt_flag91)
                        {
                            setRichtexBox("10.1、模板一检查OK,goon");
                        }
                        else
                        {
                            setRichtexBox("10.1、模板一检查fail,请确认，over");
                            PutLableInfor(this.label39.Text + "模板一检查fail, ");
                        }
                    }

                    Boolean tt_flag92 = checkBox6.Checked;
                    if (tt_flag92)
                    {
                        string tt_path2 = Application.StartupPath + this.label41.Text;
                        tt_flag92 = GetPathIstrue(tt_path2);
                        if (tt_flag92)
                        {
                            setRichtexBox("10.2、模板二检查OK,goon");
                        }
                        else
                        {
                            setRichtexBox("10.2、模板二检查fail,请确认，over");
                            PutLableInfor(this.label39.Text + "模板二检查fail； ");
                        }
                    }

                    if (tt_flag91 || tt_flag92 )
                    {
                        tt_flag10 = true;
                        setRichtexBox("10、总之模板路径检查OK，至少有一个模板可以打印,goon");
                    }
                    else
                    {
                        if (str.Contains("FH005") || str.Contains("FH105"))
                        {
                            setRichtexBox("10、总之模板路径检查失败，没有一个模板可以打印,goon");
                            PutLableInfor(this.label39.Text + "没有一个模板可以使用，请检查确认");
                        }
                        else
                        {
                            setRichtexBox("10、总之模板路径检查失败，没有一个模板可以打印,goon");
                            PutLableInfor("该产品不需要打印GPSN标签，请检查确认");
                        }
                    }



                }
                #endregion

                
                //第十一步信息比对检查
                #region
                Boolean tt_flag11 = false;
                if (tt_flag10)
                {
                    string tt_sql11 = "select count(1),0,0 as Fcount from ODC_CHECK_Barcode t where maclable='" + tt_shortmac + "'  and pass = 'Y' ";

                    string[] tt_array11 = new string[3];
                    tt_array11 = Dataset1.GetDatasetArray(tt_sql11, tt_conn);

                    if (tt_array11[0] == "0")
                    {
                        setRichtexBox("11、该产品还没有信息比对，over");
                        PutLableInfor("该产品还没有信息比对");
                    }
                    else
                    {
                        tt_flag11 = true;
                        setRichtexBox("11、该条码已信息比对,比对次数：" + tt_array11[0] + ",goon");
                    }

                }
                #endregion


                //第十二步 称重检验
                #region
                Boolean tt_flag12 = false;
                if (tt_flag11)
                {
                    string tt_sql12 = "select COUNT(1),0,0 from ODC_WEIGHT " +
                                    "where MAC = '" + tt_shortmac + "' ";
                    string[] tt_array12 = new string[3];
                    tt_array12 = Dataset1.GetDatasetArray(tt_sql12, tt_conn);
                    if (tt_array12[0] == "0")
                    {
                        setRichtexBox("12、该产品没有称重,over");
                        PutLableInfor("该产品没有称重,请确认");
                    }
                    else
                    {
                        tt_flag12 = true;
                        setRichtexBox("12、该产品已称重,goon");
                    }
                }
                #endregion


                //第十三步 NG01  获取MAC站位信息
                #region
                Boolean tt_flag13 = false;
                DataSet tt_dataset1 = null;
                if( tt_flag12)
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
                        PutLableInfor2("NG01,站位表没有找MAC:" + tt_shortmac + "，站位信息", tt_task, tt_shortmac);
                    }

                }
                #endregion

                //第十四步 NG02  的待测站位
                #region
                Boolean tt_flag14 = false;
                string tt_testcode = this.label54.Text;
                if (tt_flag13)
                {
                    string tt_nowcode = Dataset2.getPcbaNowCode(tt_dataset1);
                    if (tt_nowcode == tt_testcode && (str.Contains("FH005") || str.Contains("FH105")))
                    {
                        tt_flag14 = true;
                        setRichtexBox("14、NG02过,该单板的最后站位与流程设置的最后站位一致，都是:" + tt_nowcode + ",goon");
                    }
                    else if (tt_nowcode == "9990" && (str.Contains("FH006") || str.Contains("FH106")))
                    {
                        tt_flag14 = true;
                        setRichtexBox("14、NG02过,该单板的最后站位为9990，可以打GPSN标签，goon");
                    }
                    else
                    {
                        if (tt_nowcode == "0")
                        {
                            setRichtexBox("14、NG02,当前单板MAC:" + tt_shortmac + ",没有待测站位，请检查，over");
                            PutLableInfor2("NG02,当前单板MAC:" + tt_shortmac + ",没有待测站位", tt_task, tt_shortmac);
                        }
                        else
                        {
                            if (tt_nowcode == "2")
                            {
                                setRichtexBox("14、NG02,当前单板MAC:" + tt_shortmac + ",有多个待测待测站位，流程异常，over");
                                PutLableInfor2("NG02,单板MAC:" + tt_shortmac + ",有多个待测站位,流程异常", tt_task, tt_shortmac);
                            }
                            else
                            {
                                setRichtexBox("14、NG02,当前单板MAC:" + tt_shortmac + "，站位不对" + tt_nowcode + "，与设定站位" + tt_testcode + "不符，不能使用,over");
                                PutLableInfor2("NG02,单板MAC:" + tt_shortmac + ",当前站位" + tt_nowcode + ",与" + tt_testcode + ",不符", tt_task, tt_shortmac);
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
                                PutLableInfor2("NG03,查找起始站位1902数据集有问题，为空值", tt_task, tt_shortmac);
                                break;

                            case -1:
                                setRichtexBox("15、NG03,查找起始站位1902数据集排序有问题，不是从大到小的顺序排序，id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor2("NG03,查找起始站位1902数据集排序有问题，不是顺序排序", tt_task, tt_shortmac);
                                break;

                            case -2:
                                setRichtexBox("15、NG03,查找起始站位1902数据集有问题，没有找到起始1920站位，id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor2("NG03,过站没有找到1920站位", tt_task, tt_shortmac);
                                break;

                            default:
                                setRichtexBox("15、NG03,查找起始站位1902数据集有问题，出现异常情况，id=" + tt_int1920id.ToString() + ",goon");
                                PutLableInfor2("NG03,查找起始站位1902数据集出现异常情况", tt_task, tt_shortmac);
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
                        setRichtexBox("16、NG04过,3350跳出检查不检查直接过 ,goon");

                }
                #endregion

                //第十七步  NG05  全部流程检查
                #region
                Boolean tt_flag17 = false;
               if (tt_flag16)
                {  
                    bool tt_flag16_1 = false;

                    if (tt_QRDZ == 1)
                    {
                        tt_flag16_1 = CheckPrintRecordII(tt_shortmac, "地区定制二维码");
                    }
                    else
                    {
                        tt_flag16_1 = true;
                    }

                    string tt_gyid = "";

                    if (tt_gyid_Use == this.label52.Text || tt_gyid_Use == "")
                    {
                        tt_gyid = this.label52.Text;
                    }
                    else
                    {
                        tt_gyid = tt_gyid_Old;
                    }


                    int tt_productname_check = 0;

                    if (this.label10.Text.Trim() == "HG6201M"
                        || ("HG6201T,HG2201T".Contains(this.label10.Text.Trim())
                        && tt_areacode != "安徽"
                        && tt_MiniType != "小型化方案"))
                    {
                        tt_productname_check = 1;
                    }

                    string tt_codecheck = Dataset2.getPcbaAllCheck2(tt_routdataset, tt_dataset1, tt_int1920id, tt_productname_check);

                    if (tt_codecheck == "1" && tt_flag16_1)
                    {
                        tt_flag17 = true;
                        tt_gyid_Use = tt_gyid;
                        setRichtexBox("17、NG05过,该单板所有站位都测试，没有漏测站位，全部流程:"+ tt_gyid +"号" + tt_allprocesses + ",检验流程:" + tt_partprocesses + ",1920id:" + tt_int1920id.ToString() + ",goon");
                    }
                    else if (tt_flag16_1 == false)
                    {
                        setRichtexBox("17、NG05,该单板没有打印定制二维码标签，请仔细检查是否有漏打，over");
                        PutLableInfor2("NG05,没有打印定制二维码标签，请检查是否漏打", tt_task, tt_shortmac);
                    }
                    else if (tt_codecheck == "0")
                    {
                        setRichtexBox("17、NG05,单板站位全流程检查数据集有问题,MAC" + tt_shortmac + ",全部流程:" + tt_gyid + "号" + tt_allprocesses + ",检验流程:" + tt_partprocesses + ",1920id:" + tt_int1920id.ToString() + ",over");
                        PutLableInfor2("NG05,单板站位全流程检查数据集有问题", tt_task, tt_shortmac);
                    }
                    else if (tt_gyid_Old != "")
                    {
                        string tt_gyid1 = "";

                        if (tt_gyid_Use == this.label52.Text || tt_gyid_Use == "")
                        {
                            tt_gyid1 = tt_gyid_Old;
                        }
                        else
                        {
                            tt_gyid1 = this.label52.Text;
                        }

                        string tt_codeserial = this.label63.Text;

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
                            PutLableInfor2("NG05,单板站位全流程检查数据集有问题", tt_task, tt_shortmac);
                        }
                        else
                        {
                            setRichtexBox("17、NG05,该单板这个站位没有测试:" + tt_codecheck + "，请仔细检查MAC:" + tt_shortmac + ",的流程:全流程为:" + tt_allprocesses + ",检测流程为:" + tt_partprocesses + ",是否有漏测站位，over");
                            PutLableInfor2("NG05,该单板这个站位没有测试:" + tt_codecheck + "，请检查是否漏测", tt_task, tt_shortmac);
                        }
                    }
                    else
                    {
                        setRichtexBox("17、NG05,该单板这个站位没有测试:" + tt_codecheck + "，请仔细检查MAC:" + tt_shortmac + ",的流程:全流程为:" + tt_allprocesses + ",检测流程为:" + tt_partprocesses + ",是否有漏测站位，over");
                        PutLableInfor2("NG05,该单板这个站位没有测试:" + tt_codecheck + "，请检查是否漏测", tt_task, tt_shortmac);
                    }

                }

                //第十八布 II型标签箱号查询
                Boolean tt_flag18 = false;
                if (tt_flag17 && (str.Contains("FH006") || str.Contains("FH106")))
                {
                    bool tt_flag18_1 = CheckPrintRecord(tt_gesn, "中箱II型标签");

                    if (tt_flag18_1)
                    {
                        setRichtexBox("18、该箱产品已有II型标签生成记录,over");
                        PutLableInfor("该箱产品已有II型标签生成记录，如需重打请使用线长权限！");
                    }
                    else
                    {
                        string tt_sql3 = "select count(1), max(T2.pagesn),0 " +
                                         "from odc_alllable T1 " +
                                         "left outer join odc_package T2 on T1.pcbasn = T2.pasn " +
                                         "where T1.taskscode = '" + tt_task + "' and T1.hostlable = '" + tt_scanboxsn + "' ";
                        string[] tt_array3 = new string[3];
                        tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                        if (tt_array3[0] == "1")
                        {
                            tt_flag18 = true;   
                            tt_gesn = tt_array3[1];
                            this.label46.Text = tt_gesn;
                            setRichtexBox("18、找到一个箱号：" + tt_gesn + ", goon");
                        }
                        else
                        {
                            string tt_querytask = getSnRealTask("3", tt_scanboxsn);
                            setRichtexBox("18、该SN包装表中没有找到箱号或有多个箱号，可能该产品工单是：" + tt_querytask + ",返回箱号值：" + tt_array3[0] + ",over");
                            PutLableInfor("该产品工单可能是:" + tt_querytask + ",获取箱号返回值：" + tt_array3[0]);
                        }
                    }
                }
                else if (tt_flag17 && (str.Contains("FH005") || str.Contains("FH105")))
                {
                    tt_flag18 = true;
                }

                //第十九步  II型标签装箱数据检查
                Boolean tt_flag19 = false;
                if (tt_flag18 && (str.Contains("FH006") || str.Contains("FH106")))
                {
                    string tt_sql19 = "select  T2.hostlable,T2.pcbasn,T2.maclable,T2.SMtaskscode, T2.shelllable " +
                                     "from odc_package T1 " +
                                     "left outer join odc_alllable T2 on T1.pasn = T2.pcbasn " +
                                     "where T1.taskcode = '" + tt_task + "'  and T1.pagesn = '" + tt_gesn + "'  order by T2.hostlable";

                    DataSet ds19 = Dataset1.GetDataSet(tt_sql19, tt_conn);
                    if (ds19.Tables.Count > 0 && ds19.Tables[0].Rows.Count > 0)
                    {
                        tt_flag19 = true;
                        setRichtexBox("19、关联表查询到:" + ds19.Tables[0].Rows.Count.ToString() + "条数据，,goon");
                        this.textBox3.Text = ds19.Tables[0].Rows.Count.ToString();

                        for (int i = 0; i < ds19.Tables[0].Rows.Count; i++)
                        {
                            string tt_boxsn = ds19.Tables[0].Rows[i].ItemArray[0].ToString().ToUpper();
                            tt_pcba = ds19.Tables[0].Rows[i].ItemArray[1].ToString().ToUpper();
                            string tt_maclable = ds19.Tables[0].Rows[i].ItemArray[2].ToString().ToUpper();
                            string tt_barcode = ds19.Tables[0].Rows[i].ItemArray[3].ToString().ToUpper();
                            tt_longmac = ds19.Tables[0].Rows[i].ItemArray[4].ToString().ToUpper();
                            tt_barcode1 = Regex.Replace(tt_barcode, " ", "");
                            PutListViewData(tt_boxsn, tt_pcba, tt_maclable, tt_barcode, tt_longmac, tt_barcode1);

                            if (i == 0)
                            {
                                label47.Text = tt_boxsn;
                            }

                            if (i == ds19.Tables[0].Rows.Count - 1)
                            {
                                label48.Text = tt_boxsn;
                            }
                        }
                    }
                    else
                    {
                        setRichtexBox("19、该箱号:" + tt_gesn + ",没找到数据关联表没有查询到数据，over");
                        PutLableInfor("该箱号:" + tt_gesn + ",没找到数据，请检查！");
                    }
                }
                else if (tt_flag18 && (str.Contains("FH005") || str.Contains("FH105")))
                {
                    tt_flag19 = true;
                }

                #endregion

                //最后判断
                #region
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10 && 
                    tt_flag11 && tt_flag12 && tt_flag13 && tt_flag14 && tt_flag15 && tt_flag16 && tt_flag17 && tt_flag18 && tt_flag19)
                {
                    if (str.Contains("FH005") || str.Contains("FH105"))
                    {
                        PutListViewData(tt_scanboxsn, tt_pcba, tt_shortmac, tt_smtaskscode, tt_longmac, tt_barcode1);
                        tt_scanboxnum++;
                        this.textBox3.Text = tt_scanboxnum.ToString();
                        //SN号起始范围
                        if (tt_scanboxnum == 1)
                        {
                            label47.Text = tt_scanboxsn;
                            label48.Text = tt_scanboxsn;
                        }
                        else
                        {
                            label48.Text = tt_scanboxsn;
                        }

                        //自动装箱
                        string tt_package = "";
                        if (this.textBox2.Text == this.textBox3.Text)
                        {
                            //第一步获取箱号
                            if ((this.label10.Text == "HG6201M" || this.label10.Text == "HG6821M") && tt_areacode == "安徽")
                            {
                                tt_package = GetBoxNumber5(tt_beforstranhui, this.label48.Text, this.textBox2.Text);
                            }
                            else
                            {
                                tt_package = GetBoxNumber3(label15.Text, this.label48.Text, this.textBox2.Text);
                            }

                            if (tt_package != "")
                            {
                                this.label46.Text = tt_package.ToUpper();
                            }
                            else
                            {
                                setRichtexBox("20、装箱的第一台产品不是规定的首台产品，请检查，over");
                                PutLableInfor("装箱的第一台产品不是规定的首台产品，请检查！");
                                return;
                            }

                            //第二步 装箱过站
                            Boolean tt_passflage = ListViewStatioPass(tt_task, tt_gyid_Use, tt_ccode, tt_ncode, tt_package, tt_conn);

                            //第三步打印标签,清理数据
                            if (tt_passflage)
                            {
                                //打印记录
                                Dataset1.lablePrintRecord(tt_task, tt_package, tt_package, "中箱I型标签", str, tt_computermac, "", tt_conn);

                                GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                                //如果是勾选打印3站，那么再打印两张
                                if (this.checkBox8.Checked)
                                {
                                    GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                                    GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                                    if (this.label10.Text == "HG6201U" || this.label10.Text == "HG6821U") //如果是联通，那么再打印一张
                                    {
                                        GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                                    }
                                }
                                ClearLabelInfo3();
                            }
                        }
                        CheckStation(tt_task, tt_package);
                        getPackageNunber(tt_task);
                        PutLableInfor(this.label39.Text + " OK,继续");

                    }
                    else if (str.Contains("FH006") || str.Contains("FH106"))
                    {
                        GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                        //如果是勾选打印3站，那么再打印两张
                        if (this.checkBox8.Checked)
                        {
                            GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                            GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                            if (this.label10.Text == "HG6201U" || this.label10.Text == "HG6821U") //如果是联通，那么再打印一张
                            {
                                GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, false);
                            }
                        }
                        string tt_taskscode = this.textBox1.Text.Trim().ToUpper();
                        string tt_host = tt_gesn;
                        string tt_recordmac = tt_gesn;
                        string tt_local = "中箱II型标签";
                        string tt_username = "";
                        tt_username = "FH006";
                        string tt_remark = "GPSN电脑打印记录";
                        SetPrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark);
                        //打印记录
                        Dataset1.lablePrintRecord(tt_taskscode, tt_recordmac, tt_host, tt_local, tt_username, tt_computermac, tt_remark, tt_conn);
                        ClearLabelInfo2();
                        ClearLabelInfo3();
                        ClearLabelInfo4();
                        CheckStation(tt_task, tt_gesn);
                        PutLableInfor("II型标签打印OK,继续");
                    }

                    this.richTextBox1.BackColor = Color.Chartreuse;
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                }
                #endregion

                //移动光标
                GetProductRhythm();
                textBox4.Focus();
                textBox4.SelectAll();
            }


        }

        #endregion

                
        #region 12、打印标签

        //获取参数
        private void GetParaDataPrint(int tt_itemtype, Boolean tt_flag1, Boolean tt_flag2, Boolean tt_flag3)
        {
            //模板二打印
            #region
            if (tt_flag2)
            {
                setRichtexBox("30、开始模板二打印");
                string tt_path2 = Application.StartupPath + this.label41.Text;
                string tt_datatype2 = this.label44.Text;

                #region 朝歌模板
                //if (tt_datatype2 == "ZGZX01")
                //{
                //    GetParaDataPrint_ZGZX01(tt_path2, tt_itemtype);
                //}
                //else if (tt_datatype2 == "ZGZX02")
                //{
                //    GetParaDataPrint_ZGZX02(tt_path2, tt_itemtype);
                //}
                //else if (tt_datatype2 == "ZGZX03")
                //{
                //    GetParaDataPrint_ZGZX03(tt_path2, tt_itemtype);
                //}
                //else if (tt_datatype2 == "ZGZX04")
                //{
                //    GetParaDataPrint_ZGZX04(tt_path2, tt_itemtype);
                //}
                //else if (tt_datatype2 == "ZGZX05")
                //{
                //    GetParaDataPrint_ZGZX05(tt_path2, tt_itemtype);
                //}
                //else if (tt_datatype2 == "ZGZX06")
                //{
                //    GetParaDataPrint_ZGZX06(tt_path2, tt_itemtype);
                //}
                #endregion

                if (tt_datatype2 == "ZX01")
                {
                    GetParaDataPrint_ZX01(tt_path2, tt_itemtype, "中箱二");
                }
                else if (tt_datatype2 == "ZX02")
                {
                    GetParaDataPrint_ZX02(tt_path2, tt_itemtype, "中箱二");
                }
                else if (tt_datatype2 == "ZX03")   //烽火移动双频中箱模板一
                {
                    GetParaDataPrint_ZX03(tt_path2, tt_itemtype, "中箱二");
                }
                else if (tt_datatype2 == "GP03")   //烽火移动双频中箱模板一
                {
                    GetParaDataPrint_GP03(tt_path2, tt_itemtype, "中箱二");
                }

            }
            #endregion

            //模板一打印
            #region
            if (tt_flag1)
            {
                setRichtexBox("20、开始模板一打印");

                string tt_path1 = Application.StartupPath + this.label40.Text;
                string tt_datatype1 = this.label43.Text;

                #region 朝歌参考模板
                //if (tt_datatype1 == "ZGZX01")    
                //{
                //    GetParaDataPrint_ZGZX01(tt_path1, tt_itemtype);
                //}
                //else if (tt_datatype1 == "ZGZX02")   //朝歌参考模板
                //{
                //    GetParaDataPrint_ZGZX02(tt_path1, tt_itemtype);
                //}
                //else if (tt_datatype1 == "ZGZX03")   //朝歌参考模板
                //{
                //    GetParaDataPrint_ZGZX03(tt_path1, tt_itemtype);
                //}
                //else if (tt_datatype1 == "ZGZX04")  //朝歌参考模板
                //{
                //    GetParaDataPrint_ZGZX04(tt_path1, tt_itemtype);
                //}
                //else if (tt_datatype1 == "ZGZX05")  //朝歌参考模板
                //{
                //    GetParaDataPrint_ZGZX05(tt_path1, tt_itemtype);
                //}
                //else if (tt_datatype1 == "ZGZX06")   //朝歌参考模板
                //{
                //    GetParaDataPrint_ZGZX06(tt_path1, tt_itemtype);
                //}
                #endregion

                if (tt_datatype1 == "ZX01")   //烽火天翼中箱模板一
                {
                    GetParaDataPrint_ZX01(tt_path1, tt_itemtype, "中箱一");
                }
                else if (tt_datatype1 == "ZX02")
                {
                    GetParaDataPrint_ZX02(tt_path1, tt_itemtype, "中箱一");
                }
                else if (tt_datatype1 == "ZX03")   //烽火移动双频中箱模板一
                {
                    GetParaDataPrint_ZX03(tt_path1, tt_itemtype, "中箱一");
                }
                else if (tt_datatype1 == "GP03")   //烽火移动双频中箱模板一
                {
                    GetParaDataPrint_GP03(tt_path1, tt_itemtype, "中箱一");
                }
                else if (tt_datatype1 == "ZX04")   //烽火天翼中箱模板新
                {
                    GetParaDataPrint_ZX04(tt_path1, tt_itemtype, "中箱一");
                }
                else if (tt_datatype1 == "ZX05")   //烽火广电中箱模板
                {
                    GetParaDataPrint_ZX05(tt_path1, tt_itemtype, "中箱一");
                }

            }
            #endregion

            //模板三打印
            #region
            if (tt_flag3)
            {
                setRichtexBox("40、开始模板三打印");
                string tt_path3 = Application.StartupPath + this.label42.Text;
                string tt_datatype3 = this.label45.Text;

                #region 朝歌模板
                //if (tt_datatype3 == "ZGZX01")
                //{
                //    GetParaDataPrint_ZGZX01(tt_path3, tt_itemtype);
                //}
                //else if (tt_datatype3 == "ZGZX02")
                //{
                //    GetParaDataPrint_ZGZX02(tt_path3, tt_itemtype);
                //}
                //else if (tt_datatype3 == "ZGZX03")
                //{
                //    GetParaDataPrint_ZGZX03(tt_path3, tt_itemtype);
                //}
                //else if (tt_datatype3 == "ZGZX04")
                //{
                //    GetParaDataPrint_ZGZX04(tt_path3, tt_itemtype);
                //}
                //else if (tt_datatype3 == "ZGZX05")
                //{
                //    GetParaDataPrint_ZGZX05(tt_path3, tt_itemtype);
                //}
                //else if (tt_datatype3 == "ZGZX06")
                //{
                //    GetParaDataPrint_ZGZX06(tt_path3, tt_itemtype);
                //}
                //else 
                #endregion

                if (tt_datatype3 == "ZX01")
                {
                    GetParaDataPrint_ZX01(tt_path3, tt_itemtype, "中箱一");
                }
                else if (tt_datatype3 == "ZX02")
                {
                    GetParaDataPrint_ZX02(tt_path3, tt_itemtype, "中箱一");
                }


            }
            #endregion

        }
        
        #region 机顶盒模板

        ////----以下是ZX01数据采集----朝歌中箱青岛模板一---
        //private void GetParaDataPrint_ZGZX01(string tt_path, int tt_itemtype)
        //{

        //    //第一步数据准备
        //    DataSet dst = new DataSet();
        //    DataTable dt = new DataTable();

        //    //加二维码数据
        //    int count = this.listView1.Items.Count;
        //    string tt_twodimsn = "";
        //    string tt_twodimmac = "";
        //    string tt_twodimbarcode = "";

        //    for (int i = 0; i < count; i++)
        //    {
        //        tt_twodimsn = tt_twodimsn + this.listView1.Items[i].SubItems[1].Text + "\n\r";
        //        tt_twodimmac = tt_twodimmac + this.listView1.Items[i].SubItems[5].Text.Substring(0, 17) + "\n\r";
        //        tt_twodimbarcode = tt_twodimbarcode + this.listView1.Items[i].SubItems[4].Text + "\n\r";
        //    }


        //    dst.Tables.Add(dt);
        //    dt.Columns.Add("参数");
        //    dt.Columns.Add("名称");
        //    dt.Columns.Add("内容");

        //    DataRow row1 = dt.NewRow();
        //    row1["参数"] = "N01";
        //    row1["名称"] = "箱号";
        //    row1["内容"] = this.label46.Text;
        //    dt.Rows.Add(row1);


        //    DataRow row2 = dt.NewRow();
        //    row2["参数"] = "N02";
        //    row2["名称"] = "EC编码";
        //    row2["内容"] = this.label11.Text;
        //    dt.Rows.Add(row2);

        //    DataRow row3 = dt.NewRow();
        //    row3["参数"] = "N03";
        //    row3["名称"] = "对外型号";
        //    row3["内容"] = this.label10.Text;
        //    dt.Rows.Add(row3);

        //    DataRow row4 = dt.NewRow();
        //    row4["参数"] = "N04";
        //    row4["名称"] = "起始SN";
        //    row4["内容"] = this.label47.Text;
        //    dt.Rows.Add(row4);

        //    DataRow row5 = dt.NewRow();
        //    row5["参数"] = "N05";
        //    row5["名称"] = "结束SN";
        //    row5["内容"] = this.label48.Text;
        //    dt.Rows.Add(row5);

        //    DataRow row6 = dt.NewRow();
        //    row6["参数"] = "N06";
        //    row6["名称"] = "数量";
        //    row6["内容"] = this.textBox3.Text;
        //    dt.Rows.Add(row6);

        //    DataRow row7 = dt.NewRow();
        //    row7["参数"] = "N07";
        //    row7["名称"] = "SN条码";
        //    row7["内容"] = tt_twodimsn;
        //    dt.Rows.Add(row7);

        //    DataRow row8 = dt.NewRow();
        //    row8["参数"] = "N08";
        //    row8["名称"] = "MAC条吗";
        //    row8["内容"] = tt_twodimbarcode;
        //    dt.Rows.Add(row8);

        //    DataRow row9 = dt.NewRow();
        //    row9["参数"] = "N09";
        //    row9["名称"] = "移动条码";
        //    row9["内容"] = tt_twodimmac;
        //    dt.Rows.Add(row9);


        //    DataRow row10 = dt.NewRow();
        //    row10["参数"] = "N10";
        //    row10["名称"] = "EC描述";
        //    row10["内容"] = this.label13.Text;
        //    dt.Rows.Add(row10);


        //    DataRow row11 = dt.NewRow();
        //    row11["参数"] = "N11";
        //    row11["名称"] = "生产日期";
        //    row11["内容"] = label12.Text.Replace(".", "");
        //    dt.Rows.Add(row11);

        //    //第二步加载到表格显示
        //    this.dataGridView2.DataSource = null;
        //    this.dataGridView2.Rows.Clear();

        //    this.dataGridView2.DataSource = dst.Tables[0];
        //    this.dataGridView2.Update();

        //    this.dataGridView2.Columns[0].Width = 40;
        //    this.dataGridView2.Columns[1].Width = 80;
        //    this.dataGridView2.Columns[2].Width = 300;


        //    //第三步 打印或预览
        //    if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
        //    {
        //        FastReport.Report report = new FastReport.Report();

        //        report.Prepare();
        //        report.Load(tt_path);
        //        report.SetParameterValue("N01", dst.Tables[0].Rows[0][2].ToString());
        //        report.SetParameterValue("N02", dst.Tables[0].Rows[1][2].ToString());
        //        report.SetParameterValue("N03", dst.Tables[0].Rows[2][2].ToString());
        //        report.SetParameterValue("N04", dst.Tables[0].Rows[3][2].ToString());
        //        report.SetParameterValue("N05", dst.Tables[0].Rows[4][2].ToString());
        //        report.SetParameterValue("N06", dst.Tables[0].Rows[5][2].ToString());
        //        report.SetParameterValue("N07", dst.Tables[0].Rows[6][2].ToString());
        //        report.SetParameterValue("N08", dst.Tables[0].Rows[7][2].ToString());
        //        report.SetParameterValue("N09", dst.Tables[0].Rows[8][2].ToString());
        //        report.SetParameterValue("N10", dst.Tables[0].Rows[9][2].ToString());
        //        report.SetParameterValue("N11", dst.Tables[0].Rows[10][2].ToString());


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
        //        setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
        //        PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
        //    }





        //}


        ////----以下是ZX02数据采集----朝歌中箱青岛模板二---
        //private void GetParaDataPrint_ZGZX02(string tt_path, int tt_itemtype)
        //{
        //    //第一步数据准备
        //    DataSet dst = new DataSet();
        //    DataTable dt = new DataTable();


        //    dst.Tables.Add(dt);
        //    dt.Columns.Add("参数");
        //    dt.Columns.Add("名称");
        //    dt.Columns.Add("内容");

        //    DataRow row1 = dt.NewRow();
        //    row1["参数"] = "N01";
        //    row1["名称"] = "MAC1_1";
        //    row1["内容"] = GetListViewItem(5, 1);
        //    dt.Rows.Add(row1);


        //    DataRow row2 = dt.NewRow();
        //    row2["参数"] = "N02";
        //    row2["名称"] = "MAC1_2";
        //    row2["内容"] = GetListViewItem(5, 2);
        //    dt.Rows.Add(row2);

        //    DataRow row3 = dt.NewRow();
        //    row3["参数"] = "N03";
        //    row3["名称"] = "MAC1_3";
        //    row3["内容"] = GetListViewItem(5, 3);
        //    dt.Rows.Add(row3);

        //    DataRow row4 = dt.NewRow();
        //    row4["参数"] = "N04";
        //    row4["名称"] = "MAC1_4";
        //    row4["内容"] = GetListViewItem(5, 4);
        //    dt.Rows.Add(row4);

        //    DataRow row5 = dt.NewRow();
        //    row5["参数"] = "N05";
        //    row5["名称"] = "MAC1_5";
        //    row5["内容"] = GetListViewItem(5, 5);
        //    dt.Rows.Add(row5);

        //    DataRow row6 = dt.NewRow();
        //    row6["参数"] = "N06";
        //    row6["名称"] = "MAC1_6";
        //    row6["内容"] = GetListViewItem(5, 6);
        //    dt.Rows.Add(row6);

        //    DataRow row7 = dt.NewRow();
        //    row7["参数"] = "N07";
        //    row7["名称"] = "MAC1_7";
        //    row7["内容"] = GetListViewItem(5, 7);
        //    dt.Rows.Add(row7);

        //    DataRow row8 = dt.NewRow();
        //    row8["参数"] = "N08";
        //    row8["名称"] = "MAC1_8";
        //    row8["内容"] = GetListViewItem(5, 8);
        //    dt.Rows.Add(row8);

        //    DataRow row9 = dt.NewRow();
        //    row9["参数"] = "N09";
        //    row9["名称"] = "MAC1_9";
        //    row9["内容"] = GetListViewItem(5, 9);
        //    dt.Rows.Add(row9);


        //    DataRow row10 = dt.NewRow();
        //    row10["参数"] = "N10";
        //    row10["名称"] = "MAC1_10";
        //    row10["内容"] = GetListViewItem(5, 10);
        //    dt.Rows.Add(row10);


        //    DataRow row11 = dt.NewRow();
        //    row11["参数"] = "N11";
        //    row11["名称"] = "MAC1_11";
        //    row11["内容"] = GetListViewItem(5, 11);
        //    dt.Rows.Add(row11);


        //    DataRow row12 = dt.NewRow();
        //    row12["参数"] = "N12";
        //    row12["名称"] = "MAC1_12";
        //    row12["内容"] = GetListViewItem(5, 12);
        //    dt.Rows.Add(row12);

        //    DataRow row13 = dt.NewRow();
        //    row13["参数"] = "N13";
        //    row13["名称"] = "MAC1_13";
        //    row13["内容"] = GetListViewItem(5, 13);
        //    dt.Rows.Add(row13);

        //    DataRow row14 = dt.NewRow();
        //    row14["参数"] = "N14";
        //    row14["名称"] = "MAC1_14";
        //    row14["内容"] = GetListViewItem(5, 14);
        //    dt.Rows.Add(row14);

        //    DataRow row15 = dt.NewRow();
        //    row15["参数"] = "N15";
        //    row15["名称"] = "MAC1_15";
        //    row15["内容"] = GetListViewItem(5, 15);
        //    dt.Rows.Add(row15);

        //    DataRow row16 = dt.NewRow();
        //    row16["参数"] = "N16";
        //    row16["名称"] = "MAC1_16";
        //    row16["内容"] = GetListViewItem(5, 16);
        //    dt.Rows.Add(row16);

        //    DataRow row17 = dt.NewRow();
        //    row17["参数"] = "N17";
        //    row17["名称"] = "MAC1_17";
        //    row17["内容"] = GetListViewItem(5, 17);
        //    dt.Rows.Add(row17);

        //    DataRow row18 = dt.NewRow();
        //    row18["参数"] = "N18";
        //    row18["名称"] = "MAC1_18";
        //    row18["内容"] = GetListViewItem(5, 18);
        //    dt.Rows.Add(row18);

        //    DataRow row19 = dt.NewRow();
        //    row19["参数"] = "N19";
        //    row19["名称"] = "MAC1_19";
        //    row19["内容"] = GetListViewItem(5, 19);
        //    dt.Rows.Add(row19);


        //    DataRow row20 = dt.NewRow();
        //    row20["参数"] = "N20";
        //    row20["名称"] = "MAC1_20";
        //    row20["内容"] = GetListViewItem(5, 20);
        //    dt.Rows.Add(row20);


        //    //-----------

        //    DataRow row21 = dt.NewRow();
        //    row21["参数"] = "P01";
        //    row21["名称"] = "MAC2_1";
        //    row21["内容"] = GetListViewItem(4, 1);
        //    dt.Rows.Add(row21);


        //    DataRow row22 = dt.NewRow();
        //    row22["参数"] = "P02";
        //    row22["名称"] = "MAC2_2";
        //    row22["内容"] = GetListViewItem(4, 2);
        //    dt.Rows.Add(row22);

        //    DataRow row23 = dt.NewRow();
        //    row23["参数"] = "P03";
        //    row23["名称"] = "MAC2_3";
        //    row23["内容"] = GetListViewItem(4, 3);
        //    dt.Rows.Add(row23);

        //    DataRow row24 = dt.NewRow();
        //    row24["参数"] = "P04";
        //    row24["名称"] = "MAC2_4";
        //    row24["内容"] = GetListViewItem(4, 4);
        //    dt.Rows.Add(row24);

        //    DataRow row25 = dt.NewRow();
        //    row25["参数"] = "P05";
        //    row25["名称"] = "MAC2_5";
        //    row25["内容"] = GetListViewItem(4, 5);
        //    dt.Rows.Add(row25);

        //    DataRow row26 = dt.NewRow();
        //    row26["参数"] = "P06";
        //    row26["名称"] = "MAC2_6";
        //    row26["内容"] = GetListViewItem(4, 6);
        //    dt.Rows.Add(row26);

        //    DataRow row27 = dt.NewRow();
        //    row27["参数"] = "P07";
        //    row27["名称"] = "MAC2_7";
        //    row27["内容"] = GetListViewItem(4, 7);
        //    dt.Rows.Add(row27);

        //    DataRow row28 = dt.NewRow();
        //    row28["参数"] = "P08";
        //    row28["名称"] = "MAC2_8";
        //    row28["内容"] = GetListViewItem(4, 8);
        //    dt.Rows.Add(row28);

        //    DataRow row29 = dt.NewRow();
        //    row29["参数"] = "P09";
        //    row29["名称"] = "MAC2_9";
        //    row29["内容"] = GetListViewItem(4, 9);
        //    dt.Rows.Add(row29);


        //    DataRow row30 = dt.NewRow();
        //    row30["参数"] = "P10";
        //    row30["名称"] = "MAC2_10";
        //    row30["内容"] = GetListViewItem(4, 10);
        //    dt.Rows.Add(row30);


        //    DataRow row31 = dt.NewRow();
        //    row31["参数"] = "P11";
        //    row31["名称"] = "MAC2_11";
        //    row31["内容"] = GetListViewItem(4, 11);
        //    dt.Rows.Add(row31);


        //    DataRow row32 = dt.NewRow();
        //    row32["参数"] = "P12";
        //    row32["名称"] = "MAC2_12";
        //    row32["内容"] = GetListViewItem(4, 12);
        //    dt.Rows.Add(row32);

        //    DataRow row33 = dt.NewRow();
        //    row33["参数"] = "P13";
        //    row33["名称"] = "MAC2_13";
        //    row33["内容"] = GetListViewItem(4, 13);
        //    dt.Rows.Add(row33);

        //    DataRow row34 = dt.NewRow();
        //    row34["参数"] = "P14";
        //    row34["名称"] = "MAC2_14";
        //    row34["内容"] = GetListViewItem(4, 14);
        //    dt.Rows.Add(row34);

        //    DataRow row35 = dt.NewRow();
        //    row35["参数"] = "P15";
        //    row35["名称"] = "MAC2_15";
        //    row35["内容"] = GetListViewItem(4, 15);
        //    dt.Rows.Add(row35);

        //    DataRow row36 = dt.NewRow();
        //    row36["参数"] = "P16";
        //    row36["名称"] = "MAC2_16";
        //    row36["内容"] = GetListViewItem(4, 16);
        //    dt.Rows.Add(row36);

        //    DataRow row37 = dt.NewRow();
        //    row37["参数"] = "P17";
        //    row37["名称"] = "MAC2_17";
        //    row37["内容"] = GetListViewItem(4, 17);
        //    dt.Rows.Add(row37);

        //    DataRow row38 = dt.NewRow();
        //    row38["参数"] = "P18";
        //    row38["名称"] = "MAC2_18";
        //    row38["内容"] = GetListViewItem(4, 18);
        //    dt.Rows.Add(row38);

        //    DataRow row39 = dt.NewRow();
        //    row39["参数"] = "P19";
        //    row39["名称"] = "MAC2_19";
        //    row39["内容"] = GetListViewItem(4, 19);
        //    dt.Rows.Add(row39);


        //    DataRow row40 = dt.NewRow();
        //    row40["参数"] = "P20";
        //    row40["名称"] = "MAC2_20";
        //    row40["内容"] = GetListViewItem(4, 20);
        //    dt.Rows.Add(row40);















        //    //第二步加载到表格显示
        //    this.dataGridView2.DataSource = null;
        //    this.dataGridView2.Rows.Clear();

        //    this.dataGridView2.DataSource = dst.Tables[0];
        //    this.dataGridView2.Update();

        //    this.dataGridView2.Columns[0].Width = 40;
        //    this.dataGridView2.Columns[1].Width = 80;
        //    this.dataGridView2.Columns[2].Width = 300;




        //    //第三步 打印或预览
        //    if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
        //    {
        //        FastReport.Report report = new FastReport.Report();

        //        report.Prepare();
        //        report.Load(tt_path);
        //        report.SetParameterValue("N01", dst.Tables[0].Rows[0][2].ToString());
        //        report.SetParameterValue("N02", dst.Tables[0].Rows[1][2].ToString());
        //        report.SetParameterValue("N03", dst.Tables[0].Rows[2][2].ToString());
        //        report.SetParameterValue("N04", dst.Tables[0].Rows[3][2].ToString());
        //        report.SetParameterValue("N05", dst.Tables[0].Rows[4][2].ToString());
        //        report.SetParameterValue("N06", dst.Tables[0].Rows[5][2].ToString());
        //        report.SetParameterValue("N07", dst.Tables[0].Rows[6][2].ToString());
        //        report.SetParameterValue("N08", dst.Tables[0].Rows[7][2].ToString());
        //        report.SetParameterValue("N09", dst.Tables[0].Rows[8][2].ToString());
        //        report.SetParameterValue("N10", dst.Tables[0].Rows[9][2].ToString());

        //        report.SetParameterValue("N11", dst.Tables[0].Rows[10][2].ToString());
        //        report.SetParameterValue("N12", dst.Tables[0].Rows[11][2].ToString());
        //        report.SetParameterValue("N13", dst.Tables[0].Rows[12][2].ToString());
        //        report.SetParameterValue("N14", dst.Tables[0].Rows[13][2].ToString());
        //        report.SetParameterValue("N15", dst.Tables[0].Rows[14][2].ToString());
        //        report.SetParameterValue("N16", dst.Tables[0].Rows[15][2].ToString());
        //        report.SetParameterValue("N17", dst.Tables[0].Rows[16][2].ToString());
        //        report.SetParameterValue("N18", dst.Tables[0].Rows[17][2].ToString());
        //        report.SetParameterValue("N19", dst.Tables[0].Rows[18][2].ToString());
        //        report.SetParameterValue("N20", dst.Tables[0].Rows[19][2].ToString());


        //        report.SetParameterValue("P01", dst.Tables[0].Rows[20][2].ToString());
        //        report.SetParameterValue("P02", dst.Tables[0].Rows[21][2].ToString());
        //        report.SetParameterValue("P03", dst.Tables[0].Rows[22][2].ToString());
        //        report.SetParameterValue("P04", dst.Tables[0].Rows[23][2].ToString());
        //        report.SetParameterValue("P05", dst.Tables[0].Rows[24][2].ToString());
        //        report.SetParameterValue("P06", dst.Tables[0].Rows[25][2].ToString());
        //        report.SetParameterValue("P07", dst.Tables[0].Rows[26][2].ToString());
        //        report.SetParameterValue("P08", dst.Tables[0].Rows[27][2].ToString());
        //        report.SetParameterValue("P09", dst.Tables[0].Rows[28][2].ToString());
        //        report.SetParameterValue("P10", dst.Tables[0].Rows[29][2].ToString());

        //        report.SetParameterValue("P11", dst.Tables[0].Rows[30][2].ToString());
        //        report.SetParameterValue("P12", dst.Tables[0].Rows[31][2].ToString());
        //        report.SetParameterValue("P13", dst.Tables[0].Rows[32][2].ToString());
        //        report.SetParameterValue("P14", dst.Tables[0].Rows[33][2].ToString());
        //        report.SetParameterValue("P15", dst.Tables[0].Rows[34][2].ToString());
        //        report.SetParameterValue("P16", dst.Tables[0].Rows[35][2].ToString());
        //        report.SetParameterValue("P17", dst.Tables[0].Rows[36][2].ToString());
        //        report.SetParameterValue("P18", dst.Tables[0].Rows[37][2].ToString());
        //        report.SetParameterValue("P19", dst.Tables[0].Rows[38][2].ToString());
        //        report.SetParameterValue("P20", dst.Tables[0].Rows[39][2].ToString());



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
        //        setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
        //        PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
        //    }


        //}


        ////----以下是ZX03数据采集----
        //private void GetParaDataPrint_ZGZX03(string tt_path, int tt_itemtype)
        //{
        //}


        ////----以下是ZX04数据采集----贵州中箱标签1
        //private void GetParaDataPrint_ZGZX04(string tt_path, int tt_itemtype)
        //{
        //    //第一步数据准备
        //    DataSet dst = new DataSet();
        //    DataTable dt = new DataTable();

        //    //加二维码数据
        //    int count = this.listView1.Items.Count;
        //    int tt_forint = 0;
        //    if (count > 10)
        //    {
        //        tt_forint = 10;
        //    }
        //    else
        //    {
        //        tt_forint = count;
        //    }


        //    string tt_twodimsn = "";
        //    string tt_twodimmac = "";

        //    for (int i = 0; i < tt_forint; i++)
        //    {
        //        tt_twodimsn = tt_twodimsn + this.listView1.Items[i].SubItems[4].Text + " ";
        //        tt_twodimmac = tt_twodimmac + this.listView1.Items[i].SubItems[5].Text.Substring(0, 17) + " ";
        //    }






        //    dst.Tables.Add(dt);
        //    dt.Columns.Add("参数");
        //    dt.Columns.Add("名称");
        //    dt.Columns.Add("内容");

        //    DataRow row1 = dt.NewRow();
        //    row1["参数"] = "N01";
        //    row1["名称"] = "移动码1";
        //    row1["内容"] = GetListViewItem(3, 1);
        //    dt.Rows.Add(row1);


        //    DataRow row2 = dt.NewRow();
        //    row2["参数"] = "N02";
        //    row2["名称"] = "移动码2";
        //    row2["内容"] = GetListViewItem(3, 2);
        //    dt.Rows.Add(row2);

        //    DataRow row3 = dt.NewRow();
        //    row3["参数"] = "N03";
        //    row3["名称"] = "移动码3";
        //    row3["内容"] = GetListViewItem(3, 3);
        //    dt.Rows.Add(row3);

        //    DataRow row4 = dt.NewRow();
        //    row4["参数"] = "N04";
        //    row4["名称"] = "移动码4";
        //    row4["内容"] = GetListViewItem(3, 4);
        //    dt.Rows.Add(row4);

        //    DataRow row5 = dt.NewRow();
        //    row5["参数"] = "N05";
        //    row5["名称"] = "移动码5";
        //    row5["内容"] = GetListViewItem(3, 5);
        //    dt.Rows.Add(row5);

        //    DataRow row6 = dt.NewRow();
        //    row6["参数"] = "N06";
        //    row6["名称"] = "移动码6";
        //    row6["内容"] = GetListViewItem(3, 6);
        //    dt.Rows.Add(row6);

        //    DataRow row7 = dt.NewRow();
        //    row7["参数"] = "N07";
        //    row7["名称"] = "移动码7";
        //    row7["内容"] = GetListViewItem(3, 7);
        //    dt.Rows.Add(row7);

        //    DataRow row8 = dt.NewRow();
        //    row8["参数"] = "N08";
        //    row8["名称"] = "移动码8";
        //    row8["内容"] = GetListViewItem(3, 8);
        //    dt.Rows.Add(row8);

        //    DataRow row9 = dt.NewRow();
        //    row9["参数"] = "N09";
        //    row9["名称"] = "移动码9";
        //    row9["内容"] = GetListViewItem(3, 9);
        //    dt.Rows.Add(row9);


        //    DataRow row10 = dt.NewRow();
        //    row10["参数"] = "N10";
        //    row10["名称"] = "移动码10";
        //    row10["内容"] = GetListViewItem(3, 10);
        //    dt.Rows.Add(row10);



        //    DataRow row11 = dt.NewRow();
        //    row11["参数"] = "P01";
        //    row11["名称"] = "MAC1";
        //    row11["内容"] = GetListViewItem(4, 1);
        //    dt.Rows.Add(row11);


        //    DataRow row12 = dt.NewRow();
        //    row12["参数"] = "P02";
        //    row12["名称"] = "MAC2";
        //    row12["内容"] = GetListViewItem(4, 2);
        //    dt.Rows.Add(row12);

        //    DataRow row13 = dt.NewRow();
        //    row13["参数"] = "P03";
        //    row13["名称"] = "MAC3";
        //    row13["内容"] = GetListViewItem(4, 3);
        //    dt.Rows.Add(row13);

        //    DataRow row14 = dt.NewRow();
        //    row14["参数"] = "P04";
        //    row14["名称"] = "MAC4";
        //    row14["内容"] = GetListViewItem(4, 4);
        //    dt.Rows.Add(row14);

        //    DataRow row15 = dt.NewRow();
        //    row15["参数"] = "P05";
        //    row15["名称"] = "MAC5";
        //    row15["内容"] = GetListViewItem(4, 5);
        //    dt.Rows.Add(row15);

        //    DataRow row16 = dt.NewRow();
        //    row16["参数"] = "P06";
        //    row16["名称"] = "MAC6";
        //    row16["内容"] = GetListViewItem(4, 6);
        //    dt.Rows.Add(row16);

        //    DataRow row17 = dt.NewRow();
        //    row17["参数"] = "P07";
        //    row17["名称"] = "MAC7";
        //    row17["内容"] = GetListViewItem(4, 7);
        //    dt.Rows.Add(row17);

        //    DataRow row18 = dt.NewRow();
        //    row18["参数"] = "P08";
        //    row18["名称"] = "MAC8";
        //    row18["内容"] = GetListViewItem(4, 8);
        //    dt.Rows.Add(row18);

        //    DataRow row19 = dt.NewRow();
        //    row19["参数"] = "P09";
        //    row19["名称"] = "MAC9";
        //    row19["内容"] = GetListViewItem(4, 9);
        //    dt.Rows.Add(row19);


        //    DataRow row20 = dt.NewRow();
        //    row20["参数"] = "P10";
        //    row20["名称"] = "MAC10";
        //    row20["内容"] = GetListViewItem(4, 10);
        //    dt.Rows.Add(row20);


        //    DataRow row21 = dt.NewRow();
        //    row21["参数"] = "N11";
        //    row21["名称"] = "二维SN";
        //    row21["内容"] = tt_twodimsn;
        //    dt.Rows.Add(row21);


        //    DataRow row22 = dt.NewRow();
        //    row22["参数"] = "P11";
        //    row22["名称"] = "二维MAC";
        //    row22["内容"] = tt_twodimmac;
        //    dt.Rows.Add(row22);




        //    //第二步加载到表格显示
        //    this.dataGridView2.DataSource = null;
        //    this.dataGridView2.Rows.Clear();

        //    this.dataGridView2.DataSource = dst.Tables[0];
        //    this.dataGridView2.Update();

        //    this.dataGridView2.Columns[0].Width = 40;
        //    this.dataGridView2.Columns[1].Width = 80;
        //    this.dataGridView2.Columns[2].Width = 300;


        //    //第三步 打印或预览
        //    if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
        //    {
        //        FastReport.Report report = new FastReport.Report();

        //        report.Prepare();
        //        report.Load(tt_path);
        //        report.SetParameterValue("N01", dst.Tables[0].Rows[0][2].ToString());
        //        report.SetParameterValue("N02", dst.Tables[0].Rows[1][2].ToString());
        //        report.SetParameterValue("N03", dst.Tables[0].Rows[2][2].ToString());
        //        report.SetParameterValue("N04", dst.Tables[0].Rows[3][2].ToString());
        //        report.SetParameterValue("N05", dst.Tables[0].Rows[4][2].ToString());
        //        report.SetParameterValue("N06", dst.Tables[0].Rows[5][2].ToString());
        //        report.SetParameterValue("N07", dst.Tables[0].Rows[6][2].ToString());
        //        report.SetParameterValue("N08", dst.Tables[0].Rows[7][2].ToString());
        //        report.SetParameterValue("N09", dst.Tables[0].Rows[8][2].ToString());
        //        report.SetParameterValue("N10", dst.Tables[0].Rows[9][2].ToString());

        //        report.SetParameterValue("P01", dst.Tables[0].Rows[10][2].ToString());
        //        report.SetParameterValue("P02", dst.Tables[0].Rows[11][2].ToString());
        //        report.SetParameterValue("P03", dst.Tables[0].Rows[12][2].ToString());
        //        report.SetParameterValue("P04", dst.Tables[0].Rows[13][2].ToString());
        //        report.SetParameterValue("P05", dst.Tables[0].Rows[14][2].ToString());
        //        report.SetParameterValue("P06", dst.Tables[0].Rows[15][2].ToString());
        //        report.SetParameterValue("P07", dst.Tables[0].Rows[16][2].ToString());
        //        report.SetParameterValue("P08", dst.Tables[0].Rows[17][2].ToString());
        //        report.SetParameterValue("P09", dst.Tables[0].Rows[18][2].ToString());
        //        report.SetParameterValue("P10", dst.Tables[0].Rows[19][2].ToString());

        //        report.SetParameterValue("N11", dst.Tables[0].Rows[20][2].ToString());
        //        report.SetParameterValue("P11", dst.Tables[0].Rows[21][2].ToString());

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
        //        setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
        //        PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
        //    }



        //}


        ////----以下是ZX05数据采集----贵州中箱标签2
        //private void GetParaDataPrint_ZGZX05(string tt_path, int tt_itemtype)
        //{
        //    //第一步数据准备
        //    DataSet dst = new DataSet();
        //    DataTable dt = new DataTable();

        //    //加二维码数据
        //    int count = this.listView1.Items.Count;
        //    int tt_forint = 0;
        //    if (count > 10)
        //    {
        //        tt_forint = count;
        //    }
        //    else
        //    {
        //        tt_forint = 0;
        //    }


        //    string tt_twodimsn = "";
        //    string tt_twodimmac = "";

        //    for (int i = 10; i < tt_forint; i++)
        //    {
        //        tt_twodimsn = tt_twodimsn + this.listView1.Items[i].SubItems[4].Text + " ";
        //        tt_twodimmac = tt_twodimmac + this.listView1.Items[i].SubItems[3].Text + " ";
        //    }






        //    dst.Tables.Add(dt);
        //    dt.Columns.Add("参数");
        //    dt.Columns.Add("名称");
        //    dt.Columns.Add("内容");

        //    DataRow row1 = dt.NewRow();
        //    row1["参数"] = "N01";
        //    row1["名称"] = "移动码11";
        //    row1["内容"] = GetListViewItem(3, 11);
        //    dt.Rows.Add(row1);


        //    DataRow row2 = dt.NewRow();
        //    row2["参数"] = "N02";
        //    row2["名称"] = "移动码12";
        //    row2["内容"] = GetListViewItem(3, 12);
        //    dt.Rows.Add(row2);

        //    DataRow row3 = dt.NewRow();
        //    row3["参数"] = "N03";
        //    row3["名称"] = "移动码13";
        //    row3["内容"] = GetListViewItem(3, 13);
        //    dt.Rows.Add(row3);

        //    DataRow row4 = dt.NewRow();
        //    row4["参数"] = "N04";
        //    row4["名称"] = "移动码14";
        //    row4["内容"] = GetListViewItem(3, 14);
        //    dt.Rows.Add(row4);

        //    DataRow row5 = dt.NewRow();
        //    row5["参数"] = "N05";
        //    row5["名称"] = "移动码15";
        //    row5["内容"] = GetListViewItem(3, 15);
        //    dt.Rows.Add(row5);

        //    DataRow row6 = dt.NewRow();
        //    row6["参数"] = "N06";
        //    row6["名称"] = "移动码16";
        //    row6["内容"] = GetListViewItem(3, 16);
        //    dt.Rows.Add(row6);

        //    DataRow row7 = dt.NewRow();
        //    row7["参数"] = "N07";
        //    row7["名称"] = "移动码17";
        //    row7["内容"] = GetListViewItem(3, 17);
        //    dt.Rows.Add(row7);

        //    DataRow row8 = dt.NewRow();
        //    row8["参数"] = "N08";
        //    row8["名称"] = "移动码18";
        //    row8["内容"] = GetListViewItem(3, 18);
        //    dt.Rows.Add(row8);

        //    DataRow row9 = dt.NewRow();
        //    row9["参数"] = "N09";
        //    row9["名称"] = "移动码19";
        //    row9["内容"] = GetListViewItem(3, 19);
        //    dt.Rows.Add(row9);


        //    DataRow row10 = dt.NewRow();
        //    row10["参数"] = "N10";
        //    row10["名称"] = "移动码20";
        //    row10["内容"] = GetListViewItem(3, 20);
        //    dt.Rows.Add(row10);



        //    DataRow row11 = dt.NewRow();
        //    row11["参数"] = "P01";
        //    row11["名称"] = "MAC11";
        //    row11["内容"] = GetListViewItem(4, 11);
        //    dt.Rows.Add(row11);


        //    DataRow row12 = dt.NewRow();
        //    row12["参数"] = "P02";
        //    row12["名称"] = "MAC12";
        //    row12["内容"] = GetListViewItem(4, 12);
        //    dt.Rows.Add(row12);

        //    DataRow row13 = dt.NewRow();
        //    row13["参数"] = "P03";
        //    row13["名称"] = "MAC13";
        //    row13["内容"] = GetListViewItem(4, 13);
        //    dt.Rows.Add(row13);

        //    DataRow row14 = dt.NewRow();
        //    row14["参数"] = "P04";
        //    row14["名称"] = "MAC14";
        //    row14["内容"] = GetListViewItem(4, 14);
        //    dt.Rows.Add(row14);

        //    DataRow row15 = dt.NewRow();
        //    row15["参数"] = "P05";
        //    row15["名称"] = "MAC15";
        //    row15["内容"] = GetListViewItem(4, 15);
        //    dt.Rows.Add(row15);

        //    DataRow row16 = dt.NewRow();
        //    row16["参数"] = "P06";
        //    row16["名称"] = "MAC16";
        //    row16["内容"] = GetListViewItem(4, 16);
        //    dt.Rows.Add(row16);

        //    DataRow row17 = dt.NewRow();
        //    row17["参数"] = "P07";
        //    row17["名称"] = "MAC17";
        //    row17["内容"] = GetListViewItem(4, 17);
        //    dt.Rows.Add(row17);

        //    DataRow row18 = dt.NewRow();
        //    row18["参数"] = "P08";
        //    row18["名称"] = "MAC18";
        //    row18["内容"] = GetListViewItem(4, 18);
        //    dt.Rows.Add(row18);

        //    DataRow row19 = dt.NewRow();
        //    row19["参数"] = "P09";
        //    row19["名称"] = "MAC19";
        //    row19["内容"] = GetListViewItem(4, 19);
        //    dt.Rows.Add(row19);


        //    DataRow row20 = dt.NewRow();
        //    row20["参数"] = "P10";
        //    row20["名称"] = "MAC20";
        //    row20["内容"] = GetListViewItem(4, 20);
        //    dt.Rows.Add(row20);


        //    DataRow row21 = dt.NewRow();
        //    row21["参数"] = "N11";
        //    row21["名称"] = "二维SN";
        //    row21["内容"] = tt_twodimsn;
        //    dt.Rows.Add(row21);


        //    DataRow row22 = dt.NewRow();
        //    row22["参数"] = "P11";
        //    row22["名称"] = "二维MAC";
        //    row22["内容"] = tt_twodimmac;
        //    dt.Rows.Add(row22);


        //    //第二步加载到表格显示
        //    this.dataGridView2.DataSource = null;
        //    this.dataGridView2.Rows.Clear();

        //    this.dataGridView2.DataSource = dst.Tables[0];
        //    this.dataGridView2.Update();

        //    this.dataGridView2.Columns[0].Width = 40;
        //    this.dataGridView2.Columns[1].Width = 80;
        //    this.dataGridView2.Columns[2].Width = 300;


        //    //第三步 打印或预览
        //    if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
        //    {
        //        FastReport.Report report = new FastReport.Report();

        //        report.Prepare();
        //        report.Load(tt_path);
        //        report.SetParameterValue("N01", dst.Tables[0].Rows[0][2].ToString());
        //        report.SetParameterValue("N02", dst.Tables[0].Rows[1][2].ToString());
        //        report.SetParameterValue("N03", dst.Tables[0].Rows[2][2].ToString());
        //        report.SetParameterValue("N04", dst.Tables[0].Rows[3][2].ToString());
        //        report.SetParameterValue("N05", dst.Tables[0].Rows[4][2].ToString());
        //        report.SetParameterValue("N06", dst.Tables[0].Rows[5][2].ToString());
        //        report.SetParameterValue("N07", dst.Tables[0].Rows[6][2].ToString());
        //        report.SetParameterValue("N08", dst.Tables[0].Rows[7][2].ToString());
        //        report.SetParameterValue("N09", dst.Tables[0].Rows[8][2].ToString());
        //        report.SetParameterValue("N10", dst.Tables[0].Rows[9][2].ToString());

        //        report.SetParameterValue("P01", dst.Tables[0].Rows[10][2].ToString());
        //        report.SetParameterValue("P02", dst.Tables[0].Rows[11][2].ToString());
        //        report.SetParameterValue("P03", dst.Tables[0].Rows[12][2].ToString());
        //        report.SetParameterValue("P04", dst.Tables[0].Rows[13][2].ToString());
        //        report.SetParameterValue("P05", dst.Tables[0].Rows[14][2].ToString());
        //        report.SetParameterValue("P06", dst.Tables[0].Rows[15][2].ToString());
        //        report.SetParameterValue("P07", dst.Tables[0].Rows[16][2].ToString());
        //        report.SetParameterValue("P08", dst.Tables[0].Rows[17][2].ToString());
        //        report.SetParameterValue("P09", dst.Tables[0].Rows[18][2].ToString());
        //        report.SetParameterValue("P10", dst.Tables[0].Rows[19][2].ToString());

        //        report.SetParameterValue("N11", dst.Tables[0].Rows[20][2].ToString());
        //        report.SetParameterValue("P11", dst.Tables[0].Rows[21][2].ToString());

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
        //        setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
        //        PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
        //    }
        //}

        ////----以下是ZX06数据采集----贵州中箱标签3
        //private void GetParaDataPrint_ZGZX06(string tt_path, int tt_itemtype)
        //{
        //    //第一步数据准备
        //    DataSet dst = new DataSet();
        //    DataTable dt = new DataTable();

        //    //加二维码数据
        //    int count = this.listView1.Items.Count;
        //    string tt_twodimsn = "";


        //    for (int i = 0; i < count; i++)
        //    {
        //        tt_twodimsn = tt_twodimsn + this.listView1.Items[i].SubItems[1].Text + " ";

        //    }




        //    dst.Tables.Add(dt);
        //    dt.Columns.Add("参数");
        //    dt.Columns.Add("名称");
        //    dt.Columns.Add("内容");

        //    DataRow row1 = dt.NewRow();
        //    row1["参数"] = "N01";
        //    row1["名称"] = "SN1";
        //    row1["内容"] = GetListViewItem(1, 1);
        //    dt.Rows.Add(row1);


        //    DataRow row2 = dt.NewRow();
        //    row2["参数"] = "N02";
        //    row2["名称"] = "SN2";
        //    row2["内容"] = GetListViewItem(1, 2);
        //    dt.Rows.Add(row2);

        //    DataRow row3 = dt.NewRow();
        //    row3["参数"] = "N03";
        //    row3["名称"] = "SN3";
        //    row3["内容"] = GetListViewItem(1, 3);
        //    dt.Rows.Add(row3);

        //    DataRow row4 = dt.NewRow();
        //    row4["参数"] = "N04";
        //    row4["名称"] = "SN4";
        //    row4["内容"] = GetListViewItem(1, 4);
        //    dt.Rows.Add(row4);

        //    DataRow row5 = dt.NewRow();
        //    row5["参数"] = "N05";
        //    row5["名称"] = "SN5";
        //    row5["内容"] = GetListViewItem(1, 5);
        //    dt.Rows.Add(row5);

        //    DataRow row6 = dt.NewRow();
        //    row6["参数"] = "N06";
        //    row6["名称"] = "SN6";
        //    row6["内容"] = GetListViewItem(1, 6);
        //    dt.Rows.Add(row6);

        //    DataRow row7 = dt.NewRow();
        //    row7["参数"] = "N07";
        //    row7["名称"] = "SN7";
        //    row7["内容"] = GetListViewItem(1, 7);
        //    dt.Rows.Add(row7);

        //    DataRow row8 = dt.NewRow();
        //    row8["参数"] = "N08";
        //    row8["名称"] = "SN8";
        //    row8["内容"] = GetListViewItem(1, 8);
        //    dt.Rows.Add(row8);

        //    DataRow row9 = dt.NewRow();
        //    row9["参数"] = "N09";
        //    row9["名称"] = "SN9";
        //    row9["内容"] = GetListViewItem(1, 9);
        //    dt.Rows.Add(row9);


        //    DataRow row10 = dt.NewRow();
        //    row10["参数"] = "N10";
        //    row10["名称"] = "SN10";
        //    row10["内容"] = GetListViewItem(1, 10);
        //    dt.Rows.Add(row10);


        //    DataRow row11 = dt.NewRow();
        //    row11["参数"] = "N11";
        //    row11["名称"] = "SN11";
        //    row11["内容"] = GetListViewItem(1, 11);
        //    dt.Rows.Add(row11);


        //    DataRow row12 = dt.NewRow();
        //    row12["参数"] = "N12";
        //    row12["名称"] = "SN12";
        //    row12["内容"] = GetListViewItem(1, 12);
        //    dt.Rows.Add(row12);

        //    DataRow row13 = dt.NewRow();
        //    row13["参数"] = "N13";
        //    row13["名称"] = "SN13";
        //    row13["内容"] = GetListViewItem(1, 13);
        //    dt.Rows.Add(row13);

        //    DataRow row14 = dt.NewRow();
        //    row14["参数"] = "N14";
        //    row14["名称"] = "SN14";
        //    row14["内容"] = GetListViewItem(1, 14);
        //    dt.Rows.Add(row14);

        //    DataRow row15 = dt.NewRow();
        //    row15["参数"] = "N15";
        //    row15["名称"] = "SN15";
        //    row15["内容"] = GetListViewItem(1, 15);
        //    dt.Rows.Add(row15);

        //    DataRow row16 = dt.NewRow();
        //    row16["参数"] = "N16";
        //    row16["名称"] = "SN16";
        //    row16["内容"] = GetListViewItem(1, 16);
        //    dt.Rows.Add(row16);

        //    DataRow row17 = dt.NewRow();
        //    row17["参数"] = "N17";
        //    row17["名称"] = "SN17";
        //    row17["内容"] = GetListViewItem(1, 17);
        //    dt.Rows.Add(row17);

        //    DataRow row18 = dt.NewRow();
        //    row18["参数"] = "N18";
        //    row18["名称"] = "SN18";
        //    row18["内容"] = GetListViewItem(1, 18);
        //    dt.Rows.Add(row18);

        //    DataRow row19 = dt.NewRow();
        //    row19["参数"] = "N19";
        //    row19["名称"] = "SN19";
        //    row19["内容"] = GetListViewItem(1, 19);
        //    dt.Rows.Add(row19);


        //    DataRow row20 = dt.NewRow();
        //    row20["参数"] = "N20";
        //    row20["名称"] = "SN20";
        //    row20["内容"] = GetListViewItem(1, 20);
        //    dt.Rows.Add(row20);


        //    DataRow row21 = dt.NewRow();
        //    row21["参数"] = "N21";
        //    row21["名称"] = "箱号";
        //    row21["内容"] = label46.Text;
        //    dt.Rows.Add(row21);


        //    DataRow row22 = dt.NewRow();
        //    row22["参数"] = "N22";
        //    row22["名称"] = "二维SN";
        //    row22["内容"] = tt_twodimsn;
        //    dt.Rows.Add(row22);


        //    //第二步加载到表格显示
        //    this.dataGridView2.DataSource = null;
        //    this.dataGridView2.Rows.Clear();

        //    this.dataGridView2.DataSource = dst.Tables[0];
        //    this.dataGridView2.Update();

        //    this.dataGridView2.Columns[0].Width = 40;
        //    this.dataGridView2.Columns[1].Width = 80;
        //    this.dataGridView2.Columns[2].Width = 300;




        //    //第三步 打印或预览
        //    if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
        //    {
        //        FastReport.Report report = new FastReport.Report();

        //        report.Prepare();
        //        report.Load(tt_path);
        //        report.SetParameterValue("N01", dst.Tables[0].Rows[0][2].ToString());
        //        report.SetParameterValue("N02", dst.Tables[0].Rows[1][2].ToString());
        //        report.SetParameterValue("N03", dst.Tables[0].Rows[2][2].ToString());
        //        report.SetParameterValue("N04", dst.Tables[0].Rows[3][2].ToString());
        //        report.SetParameterValue("N05", dst.Tables[0].Rows[4][2].ToString());
        //        report.SetParameterValue("N06", dst.Tables[0].Rows[5][2].ToString());
        //        report.SetParameterValue("N07", dst.Tables[0].Rows[6][2].ToString());
        //        report.SetParameterValue("N08", dst.Tables[0].Rows[7][2].ToString());
        //        report.SetParameterValue("N09", dst.Tables[0].Rows[8][2].ToString());
        //        report.SetParameterValue("N10", dst.Tables[0].Rows[9][2].ToString());

        //        report.SetParameterValue("N11", dst.Tables[0].Rows[10][2].ToString());
        //        report.SetParameterValue("N12", dst.Tables[0].Rows[11][2].ToString());
        //        report.SetParameterValue("N13", dst.Tables[0].Rows[12][2].ToString());
        //        report.SetParameterValue("N14", dst.Tables[0].Rows[13][2].ToString());
        //        report.SetParameterValue("N15", dst.Tables[0].Rows[14][2].ToString());
        //        report.SetParameterValue("N16", dst.Tables[0].Rows[15][2].ToString());
        //        report.SetParameterValue("N17", dst.Tables[0].Rows[16][2].ToString());
        //        report.SetParameterValue("N18", dst.Tables[0].Rows[17][2].ToString());
        //        report.SetParameterValue("N19", dst.Tables[0].Rows[18][2].ToString());
        //        report.SetParameterValue("N20", dst.Tables[0].Rows[19][2].ToString());

        //        report.SetParameterValue("N21", dst.Tables[0].Rows[20][2].ToString());
        //        report.SetParameterValue("N22", dst.Tables[0].Rows[21][2].ToString());


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
        //        setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
        //        PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
        //    }
        //}

        #endregion


        //----以下是ZX01数据采集----烽火天翼标签一
        private void GetParaDataPrint_ZX01(string tt_path, int tt_itemtype, string tt_printname)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();

            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            DataRow row1 = dt.NewRow();
            row1["参数"] = "N01";
            row1["名称"] = "移动码1";
            row1["内容"] = GetListViewItem(3, 1);
            dt.Rows.Add(row1);

            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "移动码2";
            row2["内容"] = GetListViewItem(3, 2);
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "移动码3";
            row3["内容"] = GetListViewItem(3, 3);
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "移动码4";
            row4["内容"] = GetListViewItem(3, 4);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "移动码5";
            row5["内容"] = GetListViewItem(3, 5);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N06";
            row6["名称"] = "移动码6";
            row6["内容"] = GetListViewItem(3, 6);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N07";
            row7["名称"] = "移动码7";
            row7["内容"] = GetListViewItem(3, 7);
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N08";
            row8["名称"] = "移动码8";
            row8["内容"] = GetListViewItem(3, 8);
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "N09";
            row9["名称"] = "移动码9";
            row9["内容"] = GetListViewItem(3, 9);
            dt.Rows.Add(row9);


            DataRow row10 = dt.NewRow();
            row10["参数"] = "N10";
            row10["名称"] = "移动码10";
            row10["内容"] = GetListViewItem(3, 10);
            dt.Rows.Add(row10);

            //装箱序列号

            DataRow row11 = dt.NewRow();
            row11["参数"] = "P01";
            row11["名称"] = "序列号1";
            row11["内容"] = GetListViewItem(1, 1);
            dt.Rows.Add(row11);

            DataRow row12 = dt.NewRow();
            row12["参数"] = "P02";
            row12["名称"] = "序列号2";
            row12["内容"] = GetListViewItem(1, 2);
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "P03";
            row13["名称"] = "序列号3";
            row13["内容"] = GetListViewItem(1, 3);
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "P04";
            row14["名称"] = "序列号4";
            row14["内容"] = GetListViewItem(1, 4);
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "P05";
            row15["名称"] = "序列号5";
            row15["内容"] = GetListViewItem(1, 5);
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "P06";
            row16["名称"] = "序列号6";
            row16["内容"] = GetListViewItem(1, 6);
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "P07";
            row17["名称"] = "序列号7";
            row17["内容"] = GetListViewItem(1, 7);
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "P08";
            row18["名称"] = "序列号8";
            row18["内容"] = GetListViewItem(1, 8);
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "P09";
            row19["名称"] = "序列号9";
            row19["内容"] = GetListViewItem(1, 9);
            dt.Rows.Add(row19);
            
            DataRow row20 = dt.NewRow();
            row20["参数"] = "P10";
            row20["名称"] = "序列号10";
            row20["内容"] = GetListViewItem(1, 10);
            dt.Rows.Add(row20);

            //------表头参数------

            DataRow row21 = dt.NewRow();
            row21["参数"] = "S01";
            row21["名称"] = "设备型号";
            row21["内容"] = this.label10.Text;
            dt.Rows.Add(row21);


            DataRow row22 = dt.NewRow();
            row22["参数"] = "S02";
            row22["名称"] = "软件版本";
            row22["内容"] = this.label57.Text;
            dt.Rows.Add(row22);

            DataRow row23 = dt.NewRow();
            row23["参数"] = "S03";
            row23["名称"] = "装箱数量";
            row23["内容"] = this.textBox3.Text;
            dt.Rows.Add(row23);

            DataRow row24 = dt.NewRow();
            row24["参数"] = "S04";
            row24["名称"] = "物料编码";
            row24["内容"] = this.label25.Text;
            dt.Rows.Add(row24);

            DataRow row25 = dt.NewRow();
            row25["参数"] = "S05";
            row25["名称"] = "生产日期";
            row25["内容"] = this.label12.Text;
            dt.Rows.Add(row25);

            DataRow row26 = dt.NewRow();
            row26["参数"] = "S06";
            row26["名称"] = "序列号1";
            row26["内容"] = this.label47.Text;
            dt.Rows.Add(row26);

            DataRow row27 = dt.NewRow();
            row27["参数"] = "S07";
            row27["名称"] = "序列号2";
            row27["内容"] = this.label48.Text;
            dt.Rows.Add(row27);

            DataRow row28 = dt.NewRow();
            row28["参数"] = "S08";
            row28["名称"] = "外箱条码";
            row28["内容"] = this.label46.Text;
            dt.Rows.Add(row28);
      

            //第二步加载到表格显示
            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 40;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 300;


            //第三步 打印或预览
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path);
                report.SetParameterValue("N01", dst.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("N02", dst.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("N03", dst.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("N04", dst.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("N05", dst.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("N06", dst.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("N07", dst.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("N08", dst.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("N09", dst.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("N10", dst.Tables[0].Rows[9][2].ToString());

                report.SetParameterValue("P01", dst.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("P02", dst.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("P03", dst.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("P04", dst.Tables[0].Rows[13][2].ToString());
                report.SetParameterValue("P05", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("P06", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("P07", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("P08", dst.Tables[0].Rows[17][2].ToString());
                report.SetParameterValue("P09", dst.Tables[0].Rows[18][2].ToString());
                report.SetParameterValue("P10", dst.Tables[0].Rows[19][2].ToString());

                report.SetParameterValue("S01", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("S02", dst.Tables[0].Rows[21][2].ToString());
                report.SetParameterValue("S03", dst.Tables[0].Rows[22][2].ToString());
                report.SetParameterValue("S04", dst.Tables[0].Rows[23][2].ToString());
                report.SetParameterValue("S05", dst.Tables[0].Rows[24][2].ToString());
                report.SetParameterValue("S06", dst.Tables[0].Rows[25][2].ToString());
                report.SetParameterValue("S07", dst.Tables[0].Rows[26][2].ToString());
                report.SetParameterValue("S08", dst.Tables[0].Rows[27][2].ToString());


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
                    if (BoxPrintMode == "1")
                    {
                        report.PrintSettings.Printer = tt_printname;
                    }
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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }
        }
        
        //----以下是ZX01数据采集----烽火天翼标签二
        private void GetParaDataPrint_ZX02(string tt_path, int tt_itemtype, string tt_printname)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();


            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            DataRow row1 = dt.NewRow();
            row1["参数"] = "N11";
            row1["名称"] = "移动码11";
            row1["内容"] = GetListViewItem(3, 11);
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "N12";
            row2["名称"] = "移动码12";
            row2["内容"] = GetListViewItem(3, 12);
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "N13";
            row3["名称"] = "移动码13";
            row3["内容"] = GetListViewItem(3, 13);
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "N14";
            row4["名称"] = "移动码14";
            row4["内容"] = GetListViewItem(3, 14);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N15";
            row5["名称"] = "移动码15";
            row5["内容"] = GetListViewItem(3, 15);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N16";
            row6["名称"] = "移动码16";
            row6["内容"] = GetListViewItem(3, 16);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N17";
            row7["名称"] = "移动码17";
            row7["内容"] = GetListViewItem(3, 17);
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N18";
            row8["名称"] = "移动码18";
            row8["内容"] = GetListViewItem(3, 18);
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "N19";
            row9["名称"] = "移动码19";
            row9["内容"] = GetListViewItem(3, 19);
            dt.Rows.Add(row9);


            DataRow row10 = dt.NewRow();
            row10["参数"] = "N20";
            row10["名称"] = "移动码20";
            row10["内容"] = GetListViewItem(3, 20);
            dt.Rows.Add(row10);

            //装箱序列号

            DataRow row11 = dt.NewRow();
            row11["参数"] = "P11";
            row11["名称"] = "序列号11";
            row11["内容"] = GetListViewItem(1, 11);
            dt.Rows.Add(row11);


            DataRow row12 = dt.NewRow();
            row12["参数"] = "P12";
            row12["名称"] = "序列号12";
            row12["内容"] = GetListViewItem(1, 12);
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "P13";
            row13["名称"] = "序列号13";
            row13["内容"] = GetListViewItem(1, 13);
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "P14";
            row14["名称"] = "序列号14";
            row14["内容"] = GetListViewItem(1, 14);
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "P15";
            row15["名称"] = "序列号15";
            row15["内容"] = GetListViewItem(1, 15);
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "P16";
            row16["名称"] = "序列号16";
            row16["内容"] = GetListViewItem(1, 16);
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "P17";
            row17["名称"] = "序列号17";
            row17["内容"] = GetListViewItem(1, 17);
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "P18";
            row18["名称"] = "序列号18";
            row18["内容"] = GetListViewItem(1, 18);
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "P19";
            row19["名称"] = "序列号19";
            row19["内容"] = GetListViewItem(1, 19);
            dt.Rows.Add(row19);

            DataRow row20 = dt.NewRow();
            row20["参数"] = "P20";
            row20["名称"] = "序列号20";
            row20["内容"] = GetListViewItem(1, 20);
            dt.Rows.Add(row20);


            //第二步加载到表格显示
            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 40;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 300;


            //第三步 打印或预览
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path);
                report.SetParameterValue("N11", dst.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("N12", dst.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("N13", dst.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("N14", dst.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("N15", dst.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("N16", dst.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("N17", dst.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("N18", dst.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("N19", dst.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("N20", dst.Tables[0].Rows[9][2].ToString());

                report.SetParameterValue("P11", dst.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("P12", dst.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("P13", dst.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("P14", dst.Tables[0].Rows[13][2].ToString());
                report.SetParameterValue("P15", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("P16", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("P17", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("P18", dst.Tables[0].Rows[17][2].ToString());
                report.SetParameterValue("P19", dst.Tables[0].Rows[18][2].ToString());
                report.SetParameterValue("P20", dst.Tables[0].Rows[19][2].ToString());

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
                    if (BoxPrintMode == "1")
                    {
                        report.PrintSettings.Printer = tt_printname;
                    }
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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }



        }

        //----以下是ZX04数据采集----烽火天翼标签新/移动单频中箱
        private void GetParaDataPrint_ZX04(string tt_path, int tt_itemtype, string tt_printname)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();

            string shelllable_QR = GetListViewItem(6, 1) + GetQR_LINE_BREAK(GetListViewItem(6, 2))
                                 + GetListViewItem(6, 2) + GetQR_LINE_BREAK(GetListViewItem(6, 3)) 
                                 + GetListViewItem(6, 3) + GetQR_LINE_BREAK(GetListViewItem(6, 4))
                                 + GetListViewItem(6, 4) + GetQR_LINE_BREAK(GetListViewItem(6, 5)) 
                                 + GetListViewItem(6, 5) + GetQR_LINE_BREAK(GetListViewItem(6, 6)) 
                                 + GetListViewItem(6, 6) + GetQR_LINE_BREAK(GetListViewItem(6, 7))
                                 + GetListViewItem(6, 7) + GetQR_LINE_BREAK(GetListViewItem(6, 8)) 
                                 + GetListViewItem(6, 8) + GetQR_LINE_BREAK(GetListViewItem(6, 9)) 
                                 + GetListViewItem(6, 9) + GetQR_LINE_BREAK(GetListViewItem(6, 10))
                                 + GetListViewItem(6, 10) + GetQR_LINE_BREAK(GetListViewItem(6, 11)) 
                                 + GetListViewItem(6, 11) + GetQR_LINE_BREAK(GetListViewItem(6, 12)) 
                                 + GetListViewItem(6, 12) + GetQR_LINE_BREAK(GetListViewItem(6, 13))
                                 + GetListViewItem(6, 13) + GetQR_LINE_BREAK(GetListViewItem(6, 14)) 
                                 + GetListViewItem(6, 14) + GetQR_LINE_BREAK(GetListViewItem(6, 15)) 
                                 + GetListViewItem(6, 15) + GetQR_LINE_BREAK(GetListViewItem(6, 16))
                                 + GetListViewItem(6, 16) + GetQR_LINE_BREAK(GetListViewItem(6, 17)) 
                                 + GetListViewItem(6, 17) + GetQR_LINE_BREAK(GetListViewItem(6, 18)) 
                                 + GetListViewItem(6, 18) + GetQR_LINE_BREAK(GetListViewItem(6, 19))
                                 + GetListViewItem(6, 19) + GetQR_LINE_BREAK(GetListViewItem(6, 20)) 
                                 + GetListViewItem(6, 20);

            string hostlable_QR = GetListViewItem(1, 1) + GetQR_LINE_BREAK(GetListViewItem(1, 2))
                                + GetListViewItem(1, 2) + GetQR_LINE_BREAK(GetListViewItem(1, 3)) 
                                + GetListViewItem(1, 3) + GetQR_LINE_BREAK(GetListViewItem(1, 4))
                                + GetListViewItem(1, 4) + GetQR_LINE_BREAK(GetListViewItem(1, 5)) 
                                + GetListViewItem(1, 5) + GetQR_LINE_BREAK(GetListViewItem(1, 6)) 
                                + GetListViewItem(1, 6) + GetQR_LINE_BREAK(GetListViewItem(1, 7))
                                + GetListViewItem(1, 7) + GetQR_LINE_BREAK(GetListViewItem(1, 8)) 
                                + GetListViewItem(1, 8) + GetQR_LINE_BREAK(GetListViewItem(1, 9)) 
                                + GetListViewItem(1, 9) + GetQR_LINE_BREAK(GetListViewItem(1, 10))
                                + GetListViewItem(1, 10) + GetQR_LINE_BREAK(GetListViewItem(1, 11)) 
                                + GetListViewItem(1, 11) + GetQR_LINE_BREAK(GetListViewItem(1, 12)) 
                                + GetListViewItem(1, 12) + GetQR_LINE_BREAK(GetListViewItem(1, 13))
                                + GetListViewItem(1, 13) + GetQR_LINE_BREAK(GetListViewItem(1, 14)) 
                                + GetListViewItem(1, 14) + GetQR_LINE_BREAK(GetListViewItem(1, 15)) 
                                + GetListViewItem(1, 15) + GetQR_LINE_BREAK(GetListViewItem(1, 16))
                                + GetListViewItem(1, 16) + GetQR_LINE_BREAK(GetListViewItem(1, 17)) 
                                + GetListViewItem(1, 17) + GetQR_LINE_BREAK(GetListViewItem(1, 18)) 
                                + GetListViewItem(1, 18) + GetQR_LINE_BREAK(GetListViewItem(1, 19))
                                + GetListViewItem(1, 19) + GetQR_LINE_BREAK(GetListViewItem(1, 20)) 
                                + GetListViewItem(1, 20);

            string HOST_SN_QR = GetListViewItem(1, 1) + GetQR_COMMA(GetListViewItem(1, 1)) + GetListViewItem(6, 1) + GetQR_LINE_BREAK(GetListViewItem(1, 2))
                              + GetListViewItem(1, 2) + GetQR_COMMA(GetListViewItem(1, 2)) + GetListViewItem(6, 2) + GetQR_LINE_BREAK(GetListViewItem(1, 3))
                              + GetListViewItem(1, 3) + GetQR_COMMA(GetListViewItem(1, 3)) + GetListViewItem(6, 3) + GetQR_LINE_BREAK(GetListViewItem(1, 4))
                              + GetListViewItem(1, 4) + GetQR_COMMA(GetListViewItem(1, 4)) + GetListViewItem(6, 4) + GetQR_LINE_BREAK(GetListViewItem(1, 5))
                              + GetListViewItem(1, 5) + GetQR_COMMA(GetListViewItem(1, 5)) + GetListViewItem(6, 5) + GetQR_LINE_BREAK(GetListViewItem(1, 6))
                              + GetListViewItem(1, 6) + GetQR_COMMA(GetListViewItem(1, 6)) + GetListViewItem(6, 6) + GetQR_LINE_BREAK(GetListViewItem(1, 7))
                              + GetListViewItem(1, 7) + GetQR_COMMA(GetListViewItem(1, 7)) + GetListViewItem(6, 7) + GetQR_LINE_BREAK(GetListViewItem(1, 8))
                              + GetListViewItem(1, 8) + GetQR_COMMA(GetListViewItem(1, 8)) + GetListViewItem(6, 8) + GetQR_LINE_BREAK(GetListViewItem(1, 9))
                              + GetListViewItem(1, 9) + GetQR_COMMA(GetListViewItem(1, 9)) + GetListViewItem(6, 9) + GetQR_LINE_BREAK(GetListViewItem(1, 10))
                              + GetListViewItem(1, 10) + GetQR_COMMA(GetListViewItem(1, 10)) + GetListViewItem(6, 10) + GetQR_LINE_BREAK(GetListViewItem(1, 11))
                              + GetListViewItem(1, 11) + GetQR_COMMA(GetListViewItem(1, 11)) + GetListViewItem(6, 11) + GetQR_LINE_BREAK(GetListViewItem(1, 12))
                              + GetListViewItem(1, 12) + GetQR_COMMA(GetListViewItem(1, 12)) + GetListViewItem(6, 12) + GetQR_LINE_BREAK(GetListViewItem(1, 13))
                              + GetListViewItem(1, 13) + GetQR_COMMA(GetListViewItem(1, 13)) + GetListViewItem(6, 13) + GetQR_LINE_BREAK(GetListViewItem(1, 14))
                              + GetListViewItem(1, 14) + GetQR_COMMA(GetListViewItem(1, 14)) + GetListViewItem(6, 14) + GetQR_LINE_BREAK(GetListViewItem(1, 15))
                              + GetListViewItem(1, 15) + GetQR_COMMA(GetListViewItem(1, 15)) + GetListViewItem(6, 15) + GetQR_LINE_BREAK(GetListViewItem(1, 16))
                              + GetListViewItem(1, 16) + GetQR_COMMA(GetListViewItem(1, 16)) + GetListViewItem(6, 16) + GetQR_LINE_BREAK(GetListViewItem(1, 17))
                              + GetListViewItem(1, 17) + GetQR_COMMA(GetListViewItem(1, 17)) + GetListViewItem(6, 17) + GetQR_LINE_BREAK(GetListViewItem(1, 18))
                              + GetListViewItem(1, 18) + GetQR_COMMA(GetListViewItem(1, 18)) + GetListViewItem(6, 18) + GetQR_LINE_BREAK(GetListViewItem(1, 19))
                              + GetListViewItem(1, 19) + GetQR_COMMA(GetListViewItem(1, 19)) + GetListViewItem(6, 19) + GetQR_LINE_BREAK(GetListViewItem(1, 20))
                              + GetListViewItem(1, 20) + GetQR_COMMA(GetListViewItem(1, 20)) + GetListViewItem(6, 20);

            string CMCC_QR = "";

            if (tt_areacode == "四川" && tt_CMCCQR_DateCheck <= 20180424)
            {
                CMCC_QR = GetQR_SICHUAN(GetListViewItem(2, 1)) + GetListViewItem(2, 1) + GetQR_SEPARATOR(GetListViewItem(4, 1)) + GetListViewItem(4, 1) + GetQR_LINE_BREAK(GetListViewItem(2, 2))
                        + GetQR_SICHUAN(GetListViewItem(2, 2)) + GetListViewItem(2, 2) + GetQR_SEPARATOR(GetListViewItem(4, 2)) + GetListViewItem(4, 2) + GetQR_LINE_BREAK(GetListViewItem(2, 3))
                        + GetQR_SICHUAN(GetListViewItem(2, 3)) + GetListViewItem(2, 3) + GetQR_SEPARATOR(GetListViewItem(4, 3)) + GetListViewItem(4, 3) + GetQR_LINE_BREAK(GetListViewItem(2, 4))
                        + GetQR_SICHUAN(GetListViewItem(2, 4)) + GetListViewItem(2, 4) + GetQR_SEPARATOR(GetListViewItem(4, 4)) + GetListViewItem(4, 4) + GetQR_LINE_BREAK(GetListViewItem(2, 5))
                        + GetQR_SICHUAN(GetListViewItem(2, 5)) + GetListViewItem(2, 5) + GetQR_SEPARATOR(GetListViewItem(4, 5)) + GetListViewItem(4, 5) + GetQR_LINE_BREAK(GetListViewItem(2, 6))
                        + GetQR_SICHUAN(GetListViewItem(2, 6)) + GetListViewItem(2, 6) + GetQR_SEPARATOR(GetListViewItem(4, 6)) + GetListViewItem(4, 6) + GetQR_LINE_BREAK(GetListViewItem(2, 7))
                        + GetQR_SICHUAN(GetListViewItem(2, 7)) + GetListViewItem(2, 7) + GetQR_SEPARATOR(GetListViewItem(4, 7)) + GetListViewItem(4, 7) + GetQR_LINE_BREAK(GetListViewItem(2, 8))
                        + GetQR_SICHUAN(GetListViewItem(2, 8)) + GetListViewItem(2, 8) + GetQR_SEPARATOR(GetListViewItem(4, 8)) + GetListViewItem(4, 8) + GetQR_LINE_BREAK(GetListViewItem(2, 9))
                        + GetQR_SICHUAN(GetListViewItem(2, 9)) + GetListViewItem(2, 9) + GetQR_SEPARATOR(GetListViewItem(4, 9)) + GetListViewItem(4, 9) + GetQR_LINE_BREAK(GetListViewItem(2, 10))
                        + GetQR_SICHUAN(GetListViewItem(2, 10)) + GetListViewItem(2, 10) + GetQR_SEPARATOR(GetListViewItem(4, 10)) + GetListViewItem(4, 10);
            }
            else if (tt_areacode == "四川" && tt_CMCCQR_DateCheck > 20180424)
            {
                CMCC_QR = GetQR_SICHUAN(GetListViewItem(2, 1)) + GetQR_LINE_BREAK(GetListViewItem(2, 1))
                        + GetListViewItem(2, 1) + GetQR_SEPARATOR(GetListViewItem(4, 1)) + GetListViewItem(4, 1) + GetQR_LINE_BREAK(GetListViewItem(2, 2))
                        + GetListViewItem(2, 2) + GetQR_SEPARATOR(GetListViewItem(4, 2)) + GetListViewItem(4, 2) + GetQR_LINE_BREAK(GetListViewItem(2, 3))
                        + GetListViewItem(2, 3) + GetQR_SEPARATOR(GetListViewItem(4, 3)) + GetListViewItem(4, 3) + GetQR_LINE_BREAK(GetListViewItem(2, 4))
                        + GetListViewItem(2, 4) + GetQR_SEPARATOR(GetListViewItem(4, 4)) + GetListViewItem(4, 4) + GetQR_LINE_BREAK(GetListViewItem(2, 5))
                        + GetListViewItem(2, 5) + GetQR_SEPARATOR(GetListViewItem(4, 5)) + GetListViewItem(4, 5) + GetQR_LINE_BREAK(GetListViewItem(2, 6))
                        + GetListViewItem(2, 6) + GetQR_SEPARATOR(GetListViewItem(4, 6)) + GetListViewItem(4, 6) + GetQR_LINE_BREAK(GetListViewItem(2, 7))
                        + GetListViewItem(2, 7) + GetQR_SEPARATOR(GetListViewItem(4, 7)) + GetListViewItem(4, 7) + GetQR_LINE_BREAK(GetListViewItem(2, 8))
                        + GetListViewItem(2, 8) + GetQR_SEPARATOR(GetListViewItem(4, 8)) + GetListViewItem(4, 8) + GetQR_LINE_BREAK(GetListViewItem(2, 9))
                        + GetListViewItem(2, 9) + GetQR_SEPARATOR(GetListViewItem(4, 9)) + GetListViewItem(4, 9) + GetQR_LINE_BREAK(GetListViewItem(2, 10))
                        + GetListViewItem(2, 10) + GetQR_SEPARATOR(GetListViewItem(4, 10)) + GetListViewItem(4, 10);
            }
            else if (tt_areacode == "安徽")
            {
                CMCC_QR = this.label46.Text + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                        + GetQR_ANHUI(GetListViewItem(4, 1)) + GetListViewItem(4, 1) + GetQR_ANHUI(GetListViewItem(4, 1)) + GetQR_LINE_BREAK(GetListViewItem(4, 2))
                        + GetQR_ANHUI(GetListViewItem(4, 2)) + GetListViewItem(4, 2) + GetQR_ANHUI(GetListViewItem(4, 2)) + GetQR_LINE_BREAK(GetListViewItem(4, 3))
                        + GetQR_ANHUI(GetListViewItem(4, 3)) + GetListViewItem(4, 3) + GetQR_ANHUI(GetListViewItem(4, 3)) + GetQR_LINE_BREAK(GetListViewItem(4, 4))
                        + GetQR_ANHUI(GetListViewItem(4, 4)) + GetListViewItem(4, 4) + GetQR_ANHUI(GetListViewItem(4, 4)) + GetQR_LINE_BREAK(GetListViewItem(4, 5))
                        + GetQR_ANHUI(GetListViewItem(4, 5)) + GetListViewItem(4, 5) + GetQR_ANHUI(GetListViewItem(4, 5)) + GetQR_LINE_BREAK(GetListViewItem(4, 6))
                        + GetQR_ANHUI(GetListViewItem(4, 6)) + GetListViewItem(4, 6) + GetQR_ANHUI(GetListViewItem(4, 6)) + GetQR_LINE_BREAK(GetListViewItem(4, 7))
                        + GetQR_ANHUI(GetListViewItem(4, 7)) + GetListViewItem(4, 7) + GetQR_ANHUI(GetListViewItem(4, 7)) + GetQR_LINE_BREAK(GetListViewItem(4, 8))
                        + GetQR_ANHUI(GetListViewItem(4, 8)) + GetListViewItem(4, 8) + GetQR_ANHUI(GetListViewItem(4, 8)) + GetQR_LINE_BREAK(GetListViewItem(4, 9))
                        + GetQR_ANHUI(GetListViewItem(4, 9)) + GetListViewItem(4, 9) + GetQR_ANHUI(GetListViewItem(4, 9)) + GetQR_LINE_BREAK(GetListViewItem(4, 10))
                        + GetQR_ANHUI(GetListViewItem(4, 10)) + GetListViewItem(4, 10) + GetQR_ANHUI(GetListViewItem(4, 10));
            }
            else if (tt_areacode == "浙江")
            {
                CMCC_QR = "厂家:烽火通信科技股份有限公司,型号:" + this.label10.Text
                        + GetQR_COMMA(GetListViewItem(4, 1)) + GetListViewItem(4, 1)
                        + GetQR_COMMA(GetListViewItem(4, 2)) + GetListViewItem(4, 2)
                        + GetQR_COMMA(GetListViewItem(4, 3)) + GetListViewItem(4, 3)
                        + GetQR_COMMA(GetListViewItem(4, 4)) + GetListViewItem(4, 4)
                        + GetQR_COMMA(GetListViewItem(4, 5)) + GetListViewItem(4, 5)
                        + GetQR_COMMA(GetListViewItem(4, 6)) + GetListViewItem(4, 6)
                        + GetQR_COMMA(GetListViewItem(4, 7)) + GetListViewItem(4, 7)
                        + GetQR_COMMA(GetListViewItem(4, 8)) + GetListViewItem(4, 8)
                        + GetQR_COMMA(GetListViewItem(4, 9)) + GetListViewItem(4, 9)
                        + GetQR_COMMA(GetListViewItem(4, 10)) + GetListViewItem(4, 10);
            }
            else if (tt_CMCCQR_DateCheck <= 20180424)
            {
                CMCC_QR = GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 1)) + GetListViewItem(4, 1) + GetQR_LINE_BREAK(GetListViewItem(4, 2))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 2)) + GetListViewItem(4, 2) + GetQR_LINE_BREAK(GetListViewItem(4, 3))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 3)) + GetListViewItem(4, 3) + GetQR_LINE_BREAK(GetListViewItem(4, 4))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 4)) + GetListViewItem(4, 4) + GetQR_LINE_BREAK(GetListViewItem(4, 5))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 5)) + GetListViewItem(4, 5) + GetQR_LINE_BREAK(GetListViewItem(4, 6))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 6)) + GetListViewItem(4, 6) + GetQR_LINE_BREAK(GetListViewItem(4, 7))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 7)) + GetListViewItem(4, 7) + GetQR_LINE_BREAK(GetListViewItem(4, 8))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 8)) + GetListViewItem(4, 8) + GetQR_LINE_BREAK(GetListViewItem(4, 9))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 9)) + GetListViewItem(4, 9) + GetQR_LINE_BREAK(GetListViewItem(4, 10))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 10)) + GetListViewItem(4, 10);
            }
            else
            {
                CMCC_QR = GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 1)) + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                        + GetListViewItem(4, 1) + GetQR_LINE_BREAK(GetListViewItem(4, 2))
                        + GetListViewItem(4, 2) + GetQR_LINE_BREAK(GetListViewItem(4, 3))
                        + GetListViewItem(4, 3) + GetQR_LINE_BREAK(GetListViewItem(4, 4))
                        + GetListViewItem(4, 4) + GetQR_LINE_BREAK(GetListViewItem(4, 5))
                        + GetListViewItem(4, 5) + GetQR_LINE_BREAK(GetListViewItem(4, 6))
                        + GetListViewItem(4, 6) + GetQR_LINE_BREAK(GetListViewItem(4, 7))
                        + GetListViewItem(4, 7) + GetQR_LINE_BREAK(GetListViewItem(4, 8))
                        + GetListViewItem(4, 8) + GetQR_LINE_BREAK(GetListViewItem(4, 9))
                        + GetListViewItem(4, 9) + GetQR_LINE_BREAK(GetListViewItem(4, 10))
                        + GetListViewItem(4, 10);
            }


            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            //设备标识码

            DataRow row1 = dt.NewRow();
            row1["参数"] = "N01";
            row1["名称"] = "移动码1";
            row1["内容"] = GetListViewItem(3, 1);
            dt.Rows.Add(row1);
            
            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "移动码2";
            row2["内容"] = GetListViewItem(3, 2);
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "移动码3";
            row3["内容"] = GetListViewItem(3, 3);
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "移动码4";
            row4["内容"] = GetListViewItem(3, 4);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "移动码5";
            row5["内容"] = GetListViewItem(3, 5);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N06";
            row6["名称"] = "移动码6";
            row6["内容"] = GetListViewItem(3, 6);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N07";
            row7["名称"] = "移动码7";
            row7["内容"] = GetListViewItem(3, 7);
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N08";
            row8["名称"] = "移动码8";
            row8["内容"] = GetListViewItem(3, 8);
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "N09";
            row9["名称"] = "移动码9";
            row9["内容"] = GetListViewItem(3, 9);
            dt.Rows.Add(row9);
        
            DataRow row10 = dt.NewRow();
            row10["参数"] = "N10";
            row10["名称"] = "移动码10";
            row10["内容"] = GetListViewItem(3, 10);
            dt.Rows.Add(row10);

            //装箱序列号

            DataRow row11 = dt.NewRow();
            row11["参数"] = "P01";
            row11["名称"] = "序列号1";
            row11["内容"] = GetListViewItem(1, 1);
            dt.Rows.Add(row11);


            DataRow row12 = dt.NewRow();
            row12["参数"] = "P02";
            row12["名称"] = "序列号2";
            row12["内容"] = GetListViewItem(1, 2);
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "P03";
            row13["名称"] = "序列号3";
            row13["内容"] = GetListViewItem(1, 3);
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "P04";
            row14["名称"] = "序列号4";
            row14["内容"] = GetListViewItem(1, 4);
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "P05";
            row15["名称"] = "序列号5";
            row15["内容"] = GetListViewItem(1, 5);
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "P06";
            row16["名称"] = "序列号6";
            row16["内容"] = GetListViewItem(1, 6);
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "P07";
            row17["名称"] = "序列号7";
            row17["内容"] = GetListViewItem(1, 7);
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "P08";
            row18["名称"] = "序列号8";
            row18["内容"] = GetListViewItem(1, 8);
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "P09";
            row19["名称"] = "序列号9";
            row19["内容"] = GetListViewItem(1, 9);
            dt.Rows.Add(row19);
            
            DataRow row20 = dt.NewRow();
            row20["参数"] = "P10";
            row20["名称"] = "序列号10";
            row20["内容"] = GetListViewItem(1, 10);
            dt.Rows.Add(row20);

            //------表头参数------

            DataRow row21 = dt.NewRow();
            row21["参数"] = "S01";
            row21["名称"] = "设备型号";
            row21["内容"] = this.label10.Text;
            dt.Rows.Add(row21);
            
            DataRow row22 = dt.NewRow();
            row22["参数"] = "S02";
            row22["名称"] = "软件版本";
            row22["内容"] = this.label57.Text;
            dt.Rows.Add(row22);

            DataRow row23 = dt.NewRow();
            row23["参数"] = "S03";
            row23["名称"] = "装箱数量";
            row23["内容"] = this.textBox3.Text;
            dt.Rows.Add(row23);

            DataRow row24 = dt.NewRow();
            row24["参数"] = "S04";
            row24["名称"] = "物料编码";
            row24["内容"] = this.label25.Text;
            dt.Rows.Add(row24);

            DataRow row25 = dt.NewRow();
            row25["参数"] = "S05";
            row25["名称"] = "生产日期";
            row25["内容"] = this.label12.Text;
            dt.Rows.Add(row25);

            DataRow row26 = dt.NewRow();
            row26["参数"] = "S06";
            row26["名称"] = "序列号1";
            row26["内容"] = this.label47.Text;
            dt.Rows.Add(row26);

            DataRow row27 = dt.NewRow();
            row27["参数"] = "S07";
            row27["名称"] = "序列号2";
            row27["内容"] = this.label48.Text;
            dt.Rows.Add(row27);

            DataRow row28 = dt.NewRow();
            row28["参数"] = "S08";
            row28["名称"] = "外箱条码";
            row28["内容"] = this.label46.Text;
            dt.Rows.Add(row28);

            //------设备标示暗码------

            DataRow row29 = dt.NewRow();
            row29["参数"] = "A01";
            row29["名称"] = "移动暗码1";
            row29["内容"] = GetListViewItem(6, 1);
            dt.Rows.Add(row29);
            
            DataRow row30 = dt.NewRow();
            row30["参数"] = "A02";
            row30["名称"] = "移动暗码2";
            row30["内容"] = GetListViewItem(6, 2);
            dt.Rows.Add(row30);

            DataRow row31 = dt.NewRow();
            row31["参数"] = "A03";
            row31["名称"] = "移动暗码3";
            row31["内容"] = GetListViewItem(6, 3);
            dt.Rows.Add(row31);

            DataRow row32 = dt.NewRow();
            row32["参数"] = "A04";
            row32["名称"] = "移动暗码4";
            row32["内容"] = GetListViewItem(6, 4);
            dt.Rows.Add(row32);

            DataRow row33 = dt.NewRow();
            row33["参数"] = "A05";
            row33["名称"] = "移动暗码5";
            row33["内容"] = GetListViewItem(6, 5);
            dt.Rows.Add(row33);

            DataRow row34 = dt.NewRow();
            row34["参数"] = "A06";
            row34["名称"] = "移动暗码6";
            row34["内容"] = GetListViewItem(6, 6);
            dt.Rows.Add(row34);

            DataRow row35 = dt.NewRow();
            row35["参数"] = "A07";
            row35["名称"] = "移动暗码7";
            row35["内容"] = GetListViewItem(6, 7);
            dt.Rows.Add(row35);

            DataRow row36 = dt.NewRow();
            row36["参数"] = "A08";
            row36["名称"] = "移动暗码8";
            row36["内容"] = GetListViewItem(6, 8);
            dt.Rows.Add(row36);

            DataRow row37 = dt.NewRow();
            row37["参数"] = "A09";
            row37["名称"] = "移动暗码9";
            row37["内容"] = GetListViewItem(6, 9);
            dt.Rows.Add(row37);


            DataRow row38 = dt.NewRow();
            row38["参数"] = "A10";
            row38["名称"] = "移动暗码10";
            row38["内容"] = GetListViewItem(6, 10);
            dt.Rows.Add(row38);
           
            //------二维码------

            DataRow row39 = dt.NewRow();
            row39["参数"] = "R01";
            row39["名称"] = "生产序列QR";
            row39["内容"] = hostlable_QR;
            dt.Rows.Add(row39);

            DataRow row40 = dt.NewRow();
            row40["参数"] = "R02";
            row40["名称"] = "设备标识QR";
            row40["内容"] = shelllable_QR;
            dt.Rows.Add(row40);

            DataRow row41 = dt.NewRow();
            row41["参数"] = "R03";
            row41["名称"] = "标识+序列QR";
            row41["内容"] = HOST_SN_QR;
            dt.Rows.Add(row41);

            DataRow row42 = dt.NewRow();
            row42["参数"] = "R05";
            row42["名称"] = "移动QR";
            row42["内容"] = CMCC_QR;
            dt.Rows.Add(row42);

            //------PON类型------

            DataRow row43 = dt.NewRow();
            row43["参数"] = "S09";
            row43["名称"] = "PON型号";
            row43["内容"] = tt_pon_name;
            dt.Rows.Add(row43);

            //第二步加载到表格显示
            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 40;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 300;


            //第三步 打印或预览
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path);
                report.SetParameterValue("N01", dst.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("N02", dst.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("N03", dst.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("N04", dst.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("N05", dst.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("N06", dst.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("N07", dst.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("N08", dst.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("N09", dst.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("N10", dst.Tables[0].Rows[9][2].ToString());

                report.SetParameterValue("P01", dst.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("P02", dst.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("P03", dst.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("P04", dst.Tables[0].Rows[13][2].ToString());
                report.SetParameterValue("P05", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("P06", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("P07", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("P08", dst.Tables[0].Rows[17][2].ToString());
                report.SetParameterValue("P09", dst.Tables[0].Rows[18][2].ToString());
                report.SetParameterValue("P10", dst.Tables[0].Rows[19][2].ToString());

                report.SetParameterValue("S01", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("S02", dst.Tables[0].Rows[21][2].ToString());
                report.SetParameterValue("S03", dst.Tables[0].Rows[22][2].ToString());
                report.SetParameterValue("S04", dst.Tables[0].Rows[23][2].ToString());
                report.SetParameterValue("S05", dst.Tables[0].Rows[24][2].ToString());
                report.SetParameterValue("S06", dst.Tables[0].Rows[25][2].ToString());
                report.SetParameterValue("S07", dst.Tables[0].Rows[26][2].ToString());
                report.SetParameterValue("S08", dst.Tables[0].Rows[27][2].ToString());

                report.SetParameterValue("A01", dst.Tables[0].Rows[28][2].ToString());
                report.SetParameterValue("A02", dst.Tables[0].Rows[29][2].ToString());
                report.SetParameterValue("A03", dst.Tables[0].Rows[30][2].ToString());
                report.SetParameterValue("A04", dst.Tables[0].Rows[31][2].ToString());
                report.SetParameterValue("A05", dst.Tables[0].Rows[32][2].ToString());
                report.SetParameterValue("A06", dst.Tables[0].Rows[33][2].ToString());
                report.SetParameterValue("A07", dst.Tables[0].Rows[34][2].ToString());
                report.SetParameterValue("A08", dst.Tables[0].Rows[35][2].ToString());
                report.SetParameterValue("A09", dst.Tables[0].Rows[36][2].ToString());
                report.SetParameterValue("A10", dst.Tables[0].Rows[37][2].ToString());

                report.SetParameterValue("R01", dst.Tables[0].Rows[38][2].ToString());
                report.SetParameterValue("R02", dst.Tables[0].Rows[39][2].ToString());
                report.SetParameterValue("R03", dst.Tables[0].Rows[40][2].ToString());
                report.SetParameterValue("R05", dst.Tables[0].Rows[41][2].ToString());

                report.SetParameterValue("S09", dst.Tables[0].Rows[42][2].ToString());


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
                    if (BoxPrintMode == "1")
                    {
                        report.PrintSettings.Printer = tt_printname;
                    }
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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }


        }

        //----以下是ZX05数据采集----烽火广电标签/联通单频标签
        private void GetParaDataPrint_ZX05(string tt_path, int tt_itemtype, string tt_printname)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();

            string tt_telecustomer = GetTelecomOperator(this.label10.Text);

            string GPSN_QR = Regex.Replace(GetListViewItem(4, 1), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                           + Regex.Replace(GetListViewItem(4, 2), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                           + Regex.Replace(GetListViewItem(4, 3), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                           + Regex.Replace(GetListViewItem(4, 4), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                           + Regex.Replace(GetListViewItem(4, 5), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                           + Regex.Replace(GetListViewItem(4, 6), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                           + Regex.Replace(GetListViewItem(4, 7), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                           + Regex.Replace(GetListViewItem(4, 8), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                           + Regex.Replace(GetListViewItem(4, 9), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                           + Regex.Replace(GetListViewItem(4, 10), "-", "");


            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            DataRow row1 = dt.NewRow();
            row1["参数"] = "N01";
            row1["名称"] = "移动码1";
            row1["内容"] = GetListViewItem(3, 1);
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "移动码2";
            row2["内容"] = GetListViewItem(3, 2);
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "移动码3";
            row3["内容"] = GetListViewItem(3, 3);
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "移动码4";
            row4["内容"] = GetListViewItem(3, 4);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "移动码5";
            row5["内容"] = GetListViewItem(3, 5);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N06";
            row6["名称"] = "移动码6";
            row6["内容"] = GetListViewItem(3, 6);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N07";
            row7["名称"] = "移动码7";
            row7["内容"] = GetListViewItem(3, 7);
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N08";
            row8["名称"] = "移动码8";
            row8["内容"] = GetListViewItem(3, 8);
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "N09";
            row9["名称"] = "移动码9";
            row9["内容"] = GetListViewItem(3, 9);
            dt.Rows.Add(row9);


            DataRow row10 = dt.NewRow();
            row10["参数"] = "N10";
            row10["名称"] = "移动码10";
            row10["内容"] = GetListViewItem(3, 10);
            dt.Rows.Add(row10);

            //装箱序列号

            DataRow row11 = dt.NewRow();
            row11["参数"] = "P01";
            row11["名称"] = "序列号1";
            row11["内容"] = GetListViewItem(1, 1);
            dt.Rows.Add(row11);


            DataRow row12 = dt.NewRow();
            row12["参数"] = "P02";
            row12["名称"] = "序列号2";
            row12["内容"] = GetListViewItem(1, 2);
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "P03";
            row13["名称"] = "序列号3";
            row13["内容"] = GetListViewItem(1, 3);
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "P04";
            row14["名称"] = "序列号4";
            row14["内容"] = GetListViewItem(1, 4);
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "P05";
            row15["名称"] = "序列号5";
            row15["内容"] = GetListViewItem(1, 5);
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "P06";
            row16["名称"] = "序列号6";
            row16["内容"] = GetListViewItem(1, 6);
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "P07";
            row17["名称"] = "序列号7";
            row17["内容"] = GetListViewItem(1, 7);
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "P08";
            row18["名称"] = "序列号8";
            row18["内容"] = GetListViewItem(1, 8);
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "P09";
            row19["名称"] = "序列号9";
            row19["内容"] = GetListViewItem(1, 9);
            dt.Rows.Add(row19);


            DataRow row20 = dt.NewRow();
            row20["参数"] = "P10";
            row20["名称"] = "序列号10";
            row20["内容"] = GetListViewItem(1, 10);
            dt.Rows.Add(row20);

            //------表头参数------

            DataRow row21 = dt.NewRow();
            row21["参数"] = "S01";
            row21["名称"] = "设备型号";
            row21["内容"] = this.label10.Text;
            dt.Rows.Add(row21);


            DataRow row22 = dt.NewRow();
            row22["参数"] = "S02";
            row22["名称"] = "软件版本";
            row22["内容"] = this.label57.Text;
            dt.Rows.Add(row22);

            DataRow row23 = dt.NewRow();
            row23["参数"] = "S03";
            row23["名称"] = "装箱数量";
            row23["内容"] = this.textBox3.Text;
            dt.Rows.Add(row23);

            DataRow row24 = dt.NewRow();
            row24["参数"] = "S04";
            row24["名称"] = "物料编码";
            row24["内容"] = this.label25.Text;
            dt.Rows.Add(row24);

            DataRow row25 = dt.NewRow();
            row25["参数"] = "S05";
            row25["名称"] = "生产日期";
            row25["内容"] = this.label12.Text;
            dt.Rows.Add(row25);

            DataRow row26 = dt.NewRow();
            row26["参数"] = "S06";
            row26["名称"] = "序列号1";
            row26["内容"] = this.label47.Text;
            dt.Rows.Add(row26);

            DataRow row27 = dt.NewRow();
            row27["参数"] = "S07";
            row27["名称"] = "序列号2";
            row27["内容"] = this.label48.Text;
            dt.Rows.Add(row27);

            DataRow row28 = dt.NewRow();
            row28["参数"] = "S08";
            row28["名称"] = "外箱条码";
            row28["内容"] = this.label46.Text;
            dt.Rows.Add(row28);

            //------GPSN标签------

            DataRow row29 = dt.NewRow();
            row29["参数"] = "G01";
            row29["名称"] = "GPSN1";
            row29["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 1)) + GetListViewItem(4, 1);
            dt.Rows.Add(row29);

            DataRow row30 = dt.NewRow();
            row30["参数"] = "G02";
            row30["名称"] = "GPSN2";
            row30["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 2)) + GetListViewItem(4, 2);
            dt.Rows.Add(row30);

            DataRow row31 = dt.NewRow();
            row31["参数"] = "G03";
            row31["名称"] = "GPSN3";
            row31["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 3)) + GetListViewItem(4, 3);
            dt.Rows.Add(row31);

            DataRow row32 = dt.NewRow();
            row32["参数"] = "G04";
            row32["名称"] = "GPSN4";
            row32["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 4)) + GetListViewItem(4, 4);
            dt.Rows.Add(row32);

            DataRow row33 = dt.NewRow();
            row33["参数"] = "G05";
            row33["名称"] = "GPSN5";
            row33["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 5)) + GetListViewItem(4, 5);
            dt.Rows.Add(row33);

            DataRow row34 = dt.NewRow();
            row34["参数"] = "G06";
            row34["名称"] = "GPSN6";
            row34["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 6)) + GetListViewItem(4, 6);
            dt.Rows.Add(row34);

            DataRow row35 = dt.NewRow();
            row35["参数"] = "G07";
            row35["名称"] = "GPSN7";
            row35["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 7)) + GetListViewItem(4, 7);
            dt.Rows.Add(row35);

            DataRow row36 = dt.NewRow();
            row36["参数"] = "G08";
            row36["名称"] = "GPSN8";
            row36["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 8)) + GetListViewItem(4, 8);
            dt.Rows.Add(row36);

            DataRow row37 = dt.NewRow();
            row37["参数"] = "G09";
            row37["名称"] = "GPSN9";
            row37["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 9)) + GetListViewItem(4, 9);
            dt.Rows.Add(row37);
            
            DataRow row38 = dt.NewRow();
            row38["参数"] = "G10";
            row38["名称"] = "GPSN10";
            row38["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 10)) + GetListViewItem(4, 10);
            dt.Rows.Add(row38);               

            //------GPSN标签暗码------

            DataRow row39 = dt.NewRow();
            row39["参数"] = "GN01";
            row39["名称"] = "GPSN暗码1";
            row39["内容"] = Regex.Replace(GetListViewItem(4, 1), "-", "");
            dt.Rows.Add(row39);

            DataRow row40 = dt.NewRow();
            row40["参数"] = "GN02";
            row40["名称"] = "GPSN暗码2";
            row40["内容"] = Regex.Replace(GetListViewItem(4, 2), "-", "");
            dt.Rows.Add(row40);

            DataRow row41 = dt.NewRow();
            row41["参数"] = "GN03";
            row41["名称"] = "GPSN暗码3";
            row41["内容"] = Regex.Replace(GetListViewItem(4, 3), "-", "");
            dt.Rows.Add(row41);

            DataRow row42 = dt.NewRow();
            row42["参数"] = "GN04";
            row42["名称"] = "GPSN暗码4";
            row42["内容"] = Regex.Replace(GetListViewItem(4, 4), "-", "");
            dt.Rows.Add(row42);

            DataRow row43 = dt.NewRow();
            row43["参数"] = "GN05";
            row43["名称"] = "GPSN暗码5";
            row43["内容"] = Regex.Replace(GetListViewItem(4, 5), "-", "");
            dt.Rows.Add(row43);

            DataRow row44 = dt.NewRow();
            row44["参数"] = "GN06";
            row44["名称"] = "GPSN暗码6";
            row44["内容"] = Regex.Replace(GetListViewItem(4, 6), "-", "");
            dt.Rows.Add(row44);

            DataRow row45 = dt.NewRow();
            row45["参数"] = "GN07";
            row45["名称"] = "GPSN暗码7";
            row45["内容"] = Regex.Replace(GetListViewItem(4, 7), "-", "");
            dt.Rows.Add(row45);

            DataRow row46 = dt.NewRow();
            row46["参数"] = "GN08";
            row46["名称"] = "GPSN暗码8";
            row46["内容"] = Regex.Replace(GetListViewItem(4, 8), "-", "");
            dt.Rows.Add(row46);

            DataRow row47 = dt.NewRow();
            row47["参数"] = "GN09";
            row47["名称"] = "GPSN暗码9";
            row47["内容"] = Regex.Replace(GetListViewItem(4, 9), "-", "");
            dt.Rows.Add(row47);


            DataRow row48 = dt.NewRow();
            row48["参数"] = "GN10";
            row48["名称"] = "GPSN暗码10";
            row48["内容"] = Regex.Replace(GetListViewItem(4, 10), "-", "");
            dt.Rows.Add(row48);

            //------二维码------

            DataRow row49 = dt.NewRow();
            row49["参数"] = "R01";
            row49["名称"] = "GPSNQR";
            row49["内容"] = GPSN_QR;
            dt.Rows.Add(row49);
                        
            //------PON类型------

            DataRow row50 = dt.NewRow();
            row50["参数"] = "S09";
            row50["名称"] = "PON型号";
            row50["内容"] = tt_pon_name;
            dt.Rows.Add(row50);

            //------地区------

            DataRow row51 = dt.NewRow();
            row51["参数"] = "D01";
            row51["名称"] = "地区";
            row51["内容"] = tt_areacode;
            dt.Rows.Add(row51);

            //第二步加载到表格显示
            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 40;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 300;


            //第三步 打印或预览
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path);
                report.SetParameterValue("N01", dst.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("N02", dst.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("N03", dst.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("N04", dst.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("N05", dst.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("N06", dst.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("N07", dst.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("N08", dst.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("N09", dst.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("N10", dst.Tables[0].Rows[9][2].ToString());

                report.SetParameterValue("P01", dst.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("P02", dst.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("P03", dst.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("P04", dst.Tables[0].Rows[13][2].ToString());
                report.SetParameterValue("P05", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("P06", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("P07", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("P08", dst.Tables[0].Rows[17][2].ToString());
                report.SetParameterValue("P09", dst.Tables[0].Rows[18][2].ToString());
                report.SetParameterValue("P10", dst.Tables[0].Rows[19][2].ToString());

                report.SetParameterValue("S01", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("S02", dst.Tables[0].Rows[21][2].ToString());
                report.SetParameterValue("S03", dst.Tables[0].Rows[22][2].ToString());
                report.SetParameterValue("S04", dst.Tables[0].Rows[23][2].ToString());
                report.SetParameterValue("S05", dst.Tables[0].Rows[24][2].ToString());
                report.SetParameterValue("S06", dst.Tables[0].Rows[25][2].ToString());
                report.SetParameterValue("S07", dst.Tables[0].Rows[26][2].ToString());
                report.SetParameterValue("S08", dst.Tables[0].Rows[27][2].ToString());

                report.SetParameterValue("G01", dst.Tables[0].Rows[28][2].ToString());
                report.SetParameterValue("G02", dst.Tables[0].Rows[29][2].ToString());
                report.SetParameterValue("G03", dst.Tables[0].Rows[30][2].ToString());
                report.SetParameterValue("G04", dst.Tables[0].Rows[31][2].ToString());
                report.SetParameterValue("G05", dst.Tables[0].Rows[32][2].ToString());
                report.SetParameterValue("G06", dst.Tables[0].Rows[33][2].ToString());
                report.SetParameterValue("G07", dst.Tables[0].Rows[34][2].ToString());
                report.SetParameterValue("G08", dst.Tables[0].Rows[35][2].ToString());
                report.SetParameterValue("G09", dst.Tables[0].Rows[36][2].ToString());
                report.SetParameterValue("G10", dst.Tables[0].Rows[37][2].ToString());

                report.SetParameterValue("GN01", dst.Tables[0].Rows[38][2].ToString());
                report.SetParameterValue("GN02", dst.Tables[0].Rows[39][2].ToString());
                report.SetParameterValue("GN03", dst.Tables[0].Rows[40][2].ToString());
                report.SetParameterValue("GN04", dst.Tables[0].Rows[41][2].ToString());
                report.SetParameterValue("GN05", dst.Tables[0].Rows[42][2].ToString());
                report.SetParameterValue("GN06", dst.Tables[0].Rows[43][2].ToString());
                report.SetParameterValue("GN07", dst.Tables[0].Rows[44][2].ToString());
                report.SetParameterValue("GN08", dst.Tables[0].Rows[45][2].ToString());
                report.SetParameterValue("GN09", dst.Tables[0].Rows[46][2].ToString());
                report.SetParameterValue("GN10", dst.Tables[0].Rows[47][2].ToString());

                report.SetParameterValue("R01", dst.Tables[0].Rows[48][2].ToString());

                report.SetParameterValue("S09", dst.Tables[0].Rows[49][2].ToString());

                report.SetParameterValue("D01", dst.Tables[0].Rows[50][2].ToString());


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
                    if (BoxPrintMode == "1")
                    {
                        report.PrintSettings.Printer = tt_printname;
                    }
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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }

        }

        //----以下是ZX03数据采集----烽火移动标签一
        private void GetParaDataPrint_ZX03(string tt_path, int tt_itemtype, string tt_printname)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();

            string tt_telecustomer = GetTelecomOperator(this.label10.Text);

            string GPSN_QR = Regex.Replace(GetListViewItem(4, 1), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 2))
                           + Regex.Replace(GetListViewItem(4, 2), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 3))
                           + Regex.Replace(GetListViewItem(4, 3), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 4))
                           + Regex.Replace(GetListViewItem(4, 4), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 5))
                           + Regex.Replace(GetListViewItem(4, 5), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 6))
                           + Regex.Replace(GetListViewItem(4, 6), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 7))
                           + Regex.Replace(GetListViewItem(4, 7), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 8))
                           + Regex.Replace(GetListViewItem(4, 8), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 9))
                           + Regex.Replace(GetListViewItem(4, 9), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 10))
                           + Regex.Replace(GetListViewItem(4, 10), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 11))
                           + Regex.Replace(GetListViewItem(4, 11), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 12))
                           + Regex.Replace(GetListViewItem(4, 12), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 13))
                           + Regex.Replace(GetListViewItem(4, 13), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 14))
                           + Regex.Replace(GetListViewItem(4, 14), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 15))
                           + Regex.Replace(GetListViewItem(4, 15), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 16))
                           + Regex.Replace(GetListViewItem(4, 16), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 17))
                           + Regex.Replace(GetListViewItem(4, 17), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 18))
                           + Regex.Replace(GetListViewItem(4, 18), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 19))
                           + Regex.Replace(GetListViewItem(4, 19), "-", "") + GetQR_LINE_BREAK(GetListViewItem(4, 20))
                           + Regex.Replace(GetListViewItem(4, 20), "-", "");

            string CMCC_QR = "";

            if (tt_areacode == "四川" && tt_CMCCQR_DateCheck <= 20180424)
            {
                CMCC_QR = GetQR_SICHUAN(GetListViewItem(2, 1)) + GetListViewItem(2, 1) + GetQR_SEPARATOR(GetListViewItem(4, 1)) + GetListViewItem(4, 1) + GetQR_LINE_BREAK(GetListViewItem(2, 2))
                        + GetQR_SICHUAN(GetListViewItem(2, 2)) + GetListViewItem(2, 2) + GetQR_SEPARATOR(GetListViewItem(4, 2)) + GetListViewItem(4, 2) + GetQR_LINE_BREAK(GetListViewItem(2, 3))
                        + GetQR_SICHUAN(GetListViewItem(2, 3)) + GetListViewItem(2, 3) + GetQR_SEPARATOR(GetListViewItem(4, 3)) + GetListViewItem(4, 3) + GetQR_LINE_BREAK(GetListViewItem(2, 4))
                        + GetQR_SICHUAN(GetListViewItem(2, 4)) + GetListViewItem(2, 4) + GetQR_SEPARATOR(GetListViewItem(4, 4)) + GetListViewItem(4, 4) + GetQR_LINE_BREAK(GetListViewItem(2, 5))
                        + GetQR_SICHUAN(GetListViewItem(2, 5)) + GetListViewItem(2, 5) + GetQR_SEPARATOR(GetListViewItem(4, 5)) + GetListViewItem(4, 5) + GetQR_LINE_BREAK(GetListViewItem(2, 6))
                        + GetQR_SICHUAN(GetListViewItem(2, 6)) + GetListViewItem(2, 6) + GetQR_SEPARATOR(GetListViewItem(4, 6)) + GetListViewItem(4, 6) + GetQR_LINE_BREAK(GetListViewItem(2, 7))
                        + GetQR_SICHUAN(GetListViewItem(2, 7)) + GetListViewItem(2, 7) + GetQR_SEPARATOR(GetListViewItem(4, 7)) + GetListViewItem(4, 7) + GetQR_LINE_BREAK(GetListViewItem(2, 8))
                        + GetQR_SICHUAN(GetListViewItem(2, 8)) + GetListViewItem(2, 8) + GetQR_SEPARATOR(GetListViewItem(4, 8)) + GetListViewItem(4, 8) + GetQR_LINE_BREAK(GetListViewItem(2, 9))
                        + GetQR_SICHUAN(GetListViewItem(2, 9)) + GetListViewItem(2, 9) + GetQR_SEPARATOR(GetListViewItem(4, 9)) + GetListViewItem(4, 9) + GetQR_LINE_BREAK(GetListViewItem(2, 10))
                        + GetQR_SICHUAN(GetListViewItem(2, 10)) + GetListViewItem(2, 10) + GetQR_SEPARATOR(GetListViewItem(4, 10)) + GetListViewItem(4, 10) + GetQR_LINE_BREAK(GetListViewItem(2, 11))
                        + GetQR_SICHUAN(GetListViewItem(2, 11)) + GetListViewItem(2, 11) + GetQR_SEPARATOR(GetListViewItem(4, 11)) + GetListViewItem(4, 11) + GetQR_LINE_BREAK(GetListViewItem(2, 12))
                        + GetQR_SICHUAN(GetListViewItem(2, 12)) + GetListViewItem(2, 12) + GetQR_SEPARATOR(GetListViewItem(4, 12)) + GetListViewItem(4, 12) + GetQR_LINE_BREAK(GetListViewItem(2, 13))
                        + GetQR_SICHUAN(GetListViewItem(2, 13)) + GetListViewItem(2, 13) + GetQR_SEPARATOR(GetListViewItem(4, 13)) + GetListViewItem(4, 13) + GetQR_LINE_BREAK(GetListViewItem(2, 14))
                        + GetQR_SICHUAN(GetListViewItem(2, 14)) + GetListViewItem(2, 14) + GetQR_SEPARATOR(GetListViewItem(4, 14)) + GetListViewItem(4, 14) + GetQR_LINE_BREAK(GetListViewItem(2, 15))
                        + GetQR_SICHUAN(GetListViewItem(2, 15)) + GetListViewItem(2, 15) + GetQR_SEPARATOR(GetListViewItem(4, 15)) + GetListViewItem(4, 15) + GetQR_LINE_BREAK(GetListViewItem(2, 16))
                        + GetQR_SICHUAN(GetListViewItem(2, 16)) + GetListViewItem(2, 16) + GetQR_SEPARATOR(GetListViewItem(4, 16)) + GetListViewItem(4, 16) + GetQR_LINE_BREAK(GetListViewItem(2, 17))
                        + GetQR_SICHUAN(GetListViewItem(2, 17)) + GetListViewItem(2, 17) + GetQR_SEPARATOR(GetListViewItem(4, 17)) + GetListViewItem(4, 17) + GetQR_LINE_BREAK(GetListViewItem(2, 18))
                        + GetQR_SICHUAN(GetListViewItem(2, 18)) + GetListViewItem(2, 18) + GetQR_SEPARATOR(GetListViewItem(4, 18)) + GetListViewItem(4, 18) + GetQR_LINE_BREAK(GetListViewItem(2, 19))
                        + GetQR_SICHUAN(GetListViewItem(2, 19)) + GetListViewItem(2, 19) + GetQR_SEPARATOR(GetListViewItem(4, 19)) + GetListViewItem(4, 19) + GetQR_LINE_BREAK(GetListViewItem(2, 20))
                        + GetQR_SICHUAN(GetListViewItem(2, 20)) + GetListViewItem(2, 20) + GetQR_SEPARATOR(GetListViewItem(4, 20)) + GetListViewItem(4, 20);
            }
            else if (tt_areacode == "四川" && tt_CMCCQR_DateCheck > 20180424)
            {
                CMCC_QR = GetQR_SICHUAN(GetListViewItem(2, 1)) + GetQR_LINE_BREAK(GetListViewItem(2, 1)) 
                        + GetListViewItem(2, 1) + GetQR_SEPARATOR(GetListViewItem(4, 1)) + GetListViewItem(4, 1) + GetQR_LINE_BREAK(GetListViewItem(2, 2))
                        + GetListViewItem(2, 2) + GetQR_SEPARATOR(GetListViewItem(4, 2)) + GetListViewItem(4, 2) + GetQR_LINE_BREAK(GetListViewItem(2, 3))
                        + GetListViewItem(2, 3) + GetQR_SEPARATOR(GetListViewItem(4, 3)) + GetListViewItem(4, 3) + GetQR_LINE_BREAK(GetListViewItem(2, 4))
                        + GetListViewItem(2, 4) + GetQR_SEPARATOR(GetListViewItem(4, 4)) + GetListViewItem(4, 4) + GetQR_LINE_BREAK(GetListViewItem(2, 5))
                        + GetListViewItem(2, 5) + GetQR_SEPARATOR(GetListViewItem(4, 5)) + GetListViewItem(4, 5) + GetQR_LINE_BREAK(GetListViewItem(2, 6))
                        + GetListViewItem(2, 6) + GetQR_SEPARATOR(GetListViewItem(4, 6)) + GetListViewItem(4, 6) + GetQR_LINE_BREAK(GetListViewItem(2, 7))
                        + GetListViewItem(2, 7) + GetQR_SEPARATOR(GetListViewItem(4, 7)) + GetListViewItem(4, 7) + GetQR_LINE_BREAK(GetListViewItem(2, 8))
                        + GetListViewItem(2, 8) + GetQR_SEPARATOR(GetListViewItem(4, 8)) + GetListViewItem(4, 8) + GetQR_LINE_BREAK(GetListViewItem(2, 9))
                        + GetListViewItem(2, 9) + GetQR_SEPARATOR(GetListViewItem(4, 9)) + GetListViewItem(4, 9) + GetQR_LINE_BREAK(GetListViewItem(2, 10))
                        + GetListViewItem(2, 10) + GetQR_SEPARATOR(GetListViewItem(4, 10)) + GetListViewItem(4, 10) + GetQR_LINE_BREAK(GetListViewItem(2, 11))
                        + GetListViewItem(2, 11) + GetQR_SEPARATOR(GetListViewItem(4, 11)) + GetListViewItem(4, 11) + GetQR_LINE_BREAK(GetListViewItem(2, 12))
                        + GetListViewItem(2, 12) + GetQR_SEPARATOR(GetListViewItem(4, 12)) + GetListViewItem(4, 12) + GetQR_LINE_BREAK(GetListViewItem(2, 13))
                        + GetListViewItem(2, 13) + GetQR_SEPARATOR(GetListViewItem(4, 13)) + GetListViewItem(4, 13) + GetQR_LINE_BREAK(GetListViewItem(2, 14))
                        + GetListViewItem(2, 14) + GetQR_SEPARATOR(GetListViewItem(4, 14)) + GetListViewItem(4, 14) + GetQR_LINE_BREAK(GetListViewItem(2, 15))
                        + GetListViewItem(2, 15) + GetQR_SEPARATOR(GetListViewItem(4, 15)) + GetListViewItem(4, 15) + GetQR_LINE_BREAK(GetListViewItem(2, 16))
                        + GetListViewItem(2, 16) + GetQR_SEPARATOR(GetListViewItem(4, 16)) + GetListViewItem(4, 16) + GetQR_LINE_BREAK(GetListViewItem(2, 17))
                        + GetListViewItem(2, 17) + GetQR_SEPARATOR(GetListViewItem(4, 17)) + GetListViewItem(4, 17) + GetQR_LINE_BREAK(GetListViewItem(2, 18))
                        + GetListViewItem(2, 18) + GetQR_SEPARATOR(GetListViewItem(4, 18)) + GetListViewItem(4, 18) + GetQR_LINE_BREAK(GetListViewItem(2, 19))
                        + GetListViewItem(2, 19) + GetQR_SEPARATOR(GetListViewItem(4, 19)) + GetListViewItem(4, 19) + GetQR_LINE_BREAK(GetListViewItem(2, 20))
                        + GetListViewItem(2, 20) + GetQR_SEPARATOR(GetListViewItem(4, 20)) + GetListViewItem(4, 20);
            }
            else if (tt_areacode == "浙江")
            {
                CMCC_QR = "厂家:烽火通信科技股份有限公司,型号:" + this.label10.Text
                        + GetQR_COMMA(GetListViewItem(4, 1)) + GetListViewItem(4, 1)
                        + GetQR_COMMA(GetListViewItem(4, 2)) + GetListViewItem(4, 2)
                        + GetQR_COMMA(GetListViewItem(4, 3)) + GetListViewItem(4, 3)
                        + GetQR_COMMA(GetListViewItem(4, 4)) + GetListViewItem(4, 4)
                        + GetQR_COMMA(GetListViewItem(4, 5)) + GetListViewItem(4, 5)
                        + GetQR_COMMA(GetListViewItem(4, 6)) + GetListViewItem(4, 6)
                        + GetQR_COMMA(GetListViewItem(4, 7)) + GetListViewItem(4, 7)
                        + GetQR_COMMA(GetListViewItem(4, 8)) + GetListViewItem(4, 8)
                        + GetQR_COMMA(GetListViewItem(4, 9)) + GetListViewItem(4, 9)
                        + GetQR_COMMA(GetListViewItem(4, 10)) + GetListViewItem(4, 10)
                        + GetQR_COMMA(GetListViewItem(4, 11)) + GetListViewItem(4, 11)
                        + GetQR_COMMA(GetListViewItem(4, 12)) + GetListViewItem(4, 12)
                        + GetQR_COMMA(GetListViewItem(4, 13)) + GetListViewItem(4, 13)
                        + GetQR_COMMA(GetListViewItem(4, 14)) + GetListViewItem(4, 14)
                        + GetQR_COMMA(GetListViewItem(4, 15)) + GetListViewItem(4, 15)
                        + GetQR_COMMA(GetListViewItem(4, 16)) + GetListViewItem(4, 16)
                        + GetQR_COMMA(GetListViewItem(4, 17)) + GetListViewItem(4, 17)
                        + GetQR_COMMA(GetListViewItem(4, 18)) + GetListViewItem(4, 18)
                        + GetQR_COMMA(GetListViewItem(4, 19)) + GetListViewItem(4, 19)
                        + GetQR_COMMA(GetListViewItem(4, 20)) + GetListViewItem(4, 20);
            }
            else if (tt_CMCCQR_DateCheck <= 20180424)
            {
                CMCC_QR = GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 1)) + GetListViewItem(4, 1) + GetQR_LINE_BREAK(GetListViewItem(4, 2))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 2)) + GetListViewItem(4, 2) + GetQR_LINE_BREAK(GetListViewItem(4, 3))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 3)) + GetListViewItem(4, 3) + GetQR_LINE_BREAK(GetListViewItem(4, 4))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 4)) + GetListViewItem(4, 4) + GetQR_LINE_BREAK(GetListViewItem(4, 5))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 5)) + GetListViewItem(4, 5) + GetQR_LINE_BREAK(GetListViewItem(4, 6))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 6)) + GetListViewItem(4, 6) + GetQR_LINE_BREAK(GetListViewItem(4, 7))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 7)) + GetListViewItem(4, 7) + GetQR_LINE_BREAK(GetListViewItem(4, 8))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 8)) + GetListViewItem(4, 8) + GetQR_LINE_BREAK(GetListViewItem(4, 9))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 9)) + GetListViewItem(4, 9) + GetQR_LINE_BREAK(GetListViewItem(4, 10))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 10)) + GetListViewItem(4, 10) + GetQR_LINE_BREAK(GetListViewItem(4, 11))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 11)) + GetListViewItem(4, 11) + GetQR_LINE_BREAK(GetListViewItem(4, 12))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 12)) + GetListViewItem(4, 12) + GetQR_LINE_BREAK(GetListViewItem(4, 13))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 13)) + GetListViewItem(4, 13) + GetQR_LINE_BREAK(GetListViewItem(4, 14))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 14)) + GetListViewItem(4, 14) + GetQR_LINE_BREAK(GetListViewItem(4, 15))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 15)) + GetListViewItem(4, 15) + GetQR_LINE_BREAK(GetListViewItem(4, 16))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 16)) + GetListViewItem(4, 16) + GetQR_LINE_BREAK(GetListViewItem(4, 17))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 17)) + GetListViewItem(4, 17) + GetQR_LINE_BREAK(GetListViewItem(4, 18))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 18)) + GetListViewItem(4, 18) + GetQR_LINE_BREAK(GetListViewItem(4, 19))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 19)) + GetListViewItem(4, 19) + GetQR_LINE_BREAK(GetListViewItem(4, 20))
                        + GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 20)) + GetListViewItem(4, 20);
            }
            else
            {
                CMCC_QR = GetQR_NORMAL(this.label10.Text, GetListViewItem(4, 1)) + GetQR_LINE_BREAK(GetListViewItem(4, 1))
                        + GetListViewItem(4, 1) + GetQR_LINE_BREAK(GetListViewItem(4, 2))
                        + GetListViewItem(4, 2) + GetQR_LINE_BREAK(GetListViewItem(4, 3))
                        + GetListViewItem(4, 3) + GetQR_LINE_BREAK(GetListViewItem(4, 4))
                        + GetListViewItem(4, 4) + GetQR_LINE_BREAK(GetListViewItem(4, 5))
                        + GetListViewItem(4, 5) + GetQR_LINE_BREAK(GetListViewItem(4, 6))
                        + GetListViewItem(4, 6) + GetQR_LINE_BREAK(GetListViewItem(4, 7))
                        + GetListViewItem(4, 7) + GetQR_LINE_BREAK(GetListViewItem(4, 8))
                        + GetListViewItem(4, 8) + GetQR_LINE_BREAK(GetListViewItem(4, 9))
                        + GetListViewItem(4, 9) + GetQR_LINE_BREAK(GetListViewItem(4, 10))
                        + GetListViewItem(4, 10) + GetQR_LINE_BREAK(GetListViewItem(4, 11))
                        + GetListViewItem(4, 11) + GetQR_LINE_BREAK(GetListViewItem(4, 12))
                        + GetListViewItem(4, 12) + GetQR_LINE_BREAK(GetListViewItem(4, 13))
                        + GetListViewItem(4, 13) + GetQR_LINE_BREAK(GetListViewItem(4, 14))
                        + GetListViewItem(4, 14) + GetQR_LINE_BREAK(GetListViewItem(4, 15))
                        + GetListViewItem(4, 15) + GetQR_LINE_BREAK(GetListViewItem(4, 16))
                        + GetListViewItem(4, 16) + GetQR_LINE_BREAK(GetListViewItem(4, 17))
                        + GetListViewItem(4, 17) + GetQR_LINE_BREAK(GetListViewItem(4, 18))
                        + GetListViewItem(4, 18) + GetQR_LINE_BREAK(GetListViewItem(4, 19))
                        + GetListViewItem(4, 19) + GetQR_LINE_BREAK(GetListViewItem(4, 20))
                        + GetListViewItem(4, 20);
            }

            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            DataRow row1 = dt.NewRow();
            row1["参数"] = "N01";
            row1["名称"] = "GPSN1";
            row1["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 1)) + GetListViewItem(4, 1);
            dt.Rows.Add(row1);
            
            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "GPSN2";
            row2["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 2)) + GetListViewItem(4, 2);
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "GPSN3";
            row3["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 3)) + GetListViewItem(4, 3);
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "GPSN4";
            row4["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 4)) + GetListViewItem(4, 4);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "GPSN5";
            row5["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 5)) + GetListViewItem(4, 5);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N06";
            row6["名称"] = "GPSN6";
            row6["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 6)) + GetListViewItem(4, 6);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N07";
            row7["名称"] = "GPSN7";
            row7["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 7)) + GetListViewItem(4, 7);
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N08";
            row8["名称"] = "GPSN8";
            row8["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 8)) + GetListViewItem(4, 8);
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "N09";
            row9["名称"] = "GPSN9";
            row9["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 9)) + GetListViewItem(4, 9);
            dt.Rows.Add(row9);
            
            DataRow row10 = dt.NewRow();
            row10["参数"] = "N10";
            row10["名称"] = "GPSN10";
            row10["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 10)) + GetListViewItem(4, 10);
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "N11";
            row11["名称"] = "GPSN11";
            row11["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 11)) + GetListViewItem(4, 11);
            dt.Rows.Add(row11);
            
            DataRow row12 = dt.NewRow();
            row12["参数"] = "N12";
            row12["名称"] = "GPSN12";
            row12["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 12)) + GetListViewItem(4, 12);
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "N13";
            row13["名称"] = "GPSN13";
            row13["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 13)) + GetListViewItem(4, 13);
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "N14";
            row14["名称"] = "GPSN14";
            row14["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 14)) + GetListViewItem(4, 14);
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "N15";
            row15["名称"] = "GPSN15";
            row15["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 15)) + GetListViewItem(4, 15);
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "N16";
            row16["名称"] = "GPSN16";
            row16["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 16)) + GetListViewItem(4, 16);
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "N17";
            row17["名称"] = "GPSN17";
            row17["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 17)) + GetListViewItem(4, 17);
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "N18";
            row18["名称"] = "GPSN18";
            row18["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 18)) + GetListViewItem(4, 18);
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "N19";
            row19["名称"] = "GPSN19";
            row19["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 19)) + GetListViewItem(4, 19);
            dt.Rows.Add(row19);
            
            DataRow row20 = dt.NewRow();
            row20["参数"] = "N20";
            row20["名称"] = "GPSN20";
            row20["内容"] = GetGPSN_WORD(tt_pon_name, tt_telecustomer, GetListViewItem(4, 20)) + GetListViewItem(4, 20);
            dt.Rows.Add(row20);

            //------表头参数------

            DataRow row21 = dt.NewRow();
            row21["参数"] = "S01";
            row21["名称"] = "设备型号";
            row21["内容"] = this.label10.Text;
            dt.Rows.Add(row21);


            DataRow row22 = dt.NewRow();
            row22["参数"] = "S02";
            row22["名称"] = "软件版本";
            row22["内容"] = this.label57.Text;
            dt.Rows.Add(row22);

            DataRow row23 = dt.NewRow();
            row23["参数"] = "S03";
            row23["名称"] = "装箱数量";
            row23["内容"] = this.textBox3.Text;
            dt.Rows.Add(row23);

            DataRow row24 = dt.NewRow();
            row24["参数"] = "S04";
            row24["名称"] = "物料编码";
            row24["内容"] = this.label25.Text;
            dt.Rows.Add(row24);

            DataRow row25 = dt.NewRow();
            row25["参数"] = "S05";
            row25["名称"] = "生产日期";
            row25["内容"] = this.label12.Text;
            dt.Rows.Add(row25);

            DataRow row26 = dt.NewRow();
            row26["参数"] = "S06";
            row26["名称"] = "序列1";
            row26["内容"] = this.label47.Text;
            dt.Rows.Add(row26);

            DataRow row27 = dt.NewRow();
            row27["参数"] = "S07";
            row27["名称"] = "序列2";
            row27["内容"] = this.label48.Text;
            dt.Rows.Add(row27);

            DataRow row28 = dt.NewRow();
            row28["参数"] = "S08";
            row28["名称"] = "外箱条码";
            row28["内容"] = this.label46.Text;
            dt.Rows.Add(row28);

            //------GPSN标签暗码------

            DataRow row29 = dt.NewRow();
            row29["参数"] = "GN01";
            row29["名称"] = "GPSN暗码1";
            row29["内容"] = Regex.Replace(GetListViewItem(4, 1), "-", "");
            dt.Rows.Add(row29);

            DataRow row30 = dt.NewRow();
            row30["参数"] = "GN02";
            row30["名称"] = "GPSN暗码2";
            row30["内容"] = Regex.Replace(GetListViewItem(4, 2), "-", "");
            dt.Rows.Add(row30);

            DataRow row31 = dt.NewRow();
            row31["参数"] = "GN03";
            row31["名称"] = "GPSN暗码3";
            row31["内容"] = Regex.Replace(GetListViewItem(4, 3), "-", "");
            dt.Rows.Add(row31);

            DataRow row32 = dt.NewRow();
            row32["参数"] = "GN04";
            row32["名称"] = "GPSN暗码4";
            row32["内容"] = Regex.Replace(GetListViewItem(4, 4), "-", "");
            dt.Rows.Add(row32);

            DataRow row33 = dt.NewRow();
            row33["参数"] = "GN05";
            row33["名称"] = "GPSN暗码5";
            row33["内容"] = Regex.Replace(GetListViewItem(4, 5), "-", "");
            dt.Rows.Add(row33);

            DataRow row34 = dt.NewRow();
            row34["参数"] = "GN06";
            row34["名称"] = "GPSN暗码6";
            row34["内容"] = Regex.Replace(GetListViewItem(4, 6), "-", "");
            dt.Rows.Add(row34);

            DataRow row35 = dt.NewRow();
            row35["参数"] = "GN07";
            row35["名称"] = "GPSN暗码7";
            row35["内容"] = Regex.Replace(GetListViewItem(4, 7), "-", "");
            dt.Rows.Add(row35);

            DataRow row36 = dt.NewRow();
            row36["参数"] = "GN08";
            row36["名称"] = "GPSN暗码8";
            row36["内容"] = Regex.Replace(GetListViewItem(4, 8), "-", "");
            dt.Rows.Add(row36);

            DataRow row37 = dt.NewRow();
            row37["参数"] = "GN09";
            row37["名称"] = "GPSN暗码9";
            row37["内容"] = Regex.Replace(GetListViewItem(4, 9), "-", "");
            dt.Rows.Add(row37);
            
            DataRow row38 = dt.NewRow();
            row38["参数"] = "GN10";
            row38["名称"] = "GPSN暗码10";
            row38["内容"] = Regex.Replace(GetListViewItem(4, 10), "-", "");
            dt.Rows.Add(row38);

            DataRow row39 = dt.NewRow();
            row39["参数"] = "GN11";
            row39["名称"] = "GPSN暗码11";
            row39["内容"] = Regex.Replace(GetListViewItem(4, 11), "-", "");
            dt.Rows.Add(row39);

            DataRow row40 = dt.NewRow();
            row40["参数"] = "GN12";
            row40["名称"] = "GPSN暗码12";
            row40["内容"] = Regex.Replace(GetListViewItem(4, 12), "-", "");
            dt.Rows.Add(row40);

            DataRow row41 = dt.NewRow();
            row41["参数"] = "GN13";
            row41["名称"] = "GPSN暗码13";
            row41["内容"] = Regex.Replace(GetListViewItem(4, 13), "-", "");
            dt.Rows.Add(row41);

            DataRow row42 = dt.NewRow();
            row42["参数"] = "GN14";
            row42["名称"] = "GPSN暗码14";
            row42["内容"] = Regex.Replace(GetListViewItem(4, 14), "-", "");
            dt.Rows.Add(row42);

            DataRow row43 = dt.NewRow();
            row43["参数"] = "GN15";
            row43["名称"] = "GPSN暗码15";
            row43["内容"] = Regex.Replace(GetListViewItem(4, 15), "-", "");
            dt.Rows.Add(row43);

            DataRow row44 = dt.NewRow();
            row44["参数"] = "GN16";
            row44["名称"] = "GPSN暗码16";
            row44["内容"] = Regex.Replace(GetListViewItem(4, 16), "-", "");
            dt.Rows.Add(row44);

            DataRow row45 = dt.NewRow();
            row45["参数"] = "GN17";
            row45["名称"] = "GPSN暗码17";
            row45["内容"] = Regex.Replace(GetListViewItem(4, 17), "-", "");
            dt.Rows.Add(row45);

            DataRow row46 = dt.NewRow();
            row46["参数"] = "GN18";
            row46["名称"] = "GPSN暗码18";
            row46["内容"] = Regex.Replace(GetListViewItem(4, 18), "-", "");
            dt.Rows.Add(row46);

            DataRow row47 = dt.NewRow();
            row47["参数"] = "GN19";
            row47["名称"] = "GPSN暗码19";
            row47["内容"] = Regex.Replace(GetListViewItem(4, 19), "-", "");
            dt.Rows.Add(row47);
            
            DataRow row48 = dt.NewRow();
            row48["参数"] = "GN20";
            row48["名称"] = "GPSN暗码20";
            row48["内容"] = Regex.Replace(GetListViewItem(4, 20), "-", "");
            dt.Rows.Add(row48);

            //QR
            DataRow row49 = dt.NewRow();
            row49["参数"] = "QR01";
            row49["名称"] = "联通二维码";
            row49["内容"] = GPSN_QR;
            dt.Rows.Add(row49);

            DataRow row50 = dt.NewRow();
            row50["参数"] = "QR02";
            row50["名称"] = "移动二维码";
            row50["内容"] = CMCC_QR;
            dt.Rows.Add(row50);

            //第二步加载到表格显示
            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 40;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 300;


            //第三步 打印或预览
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path);
                report.SetParameterValue("N01", dst.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("N02", dst.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("N03", dst.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("N04", dst.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("N05", dst.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("N06", dst.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("N07", dst.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("N08", dst.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("N09", dst.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("N10", dst.Tables[0].Rows[9][2].ToString());

                report.SetParameterValue("N11", dst.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("N12", dst.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("N13", dst.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("N14", dst.Tables[0].Rows[13][2].ToString());
                report.SetParameterValue("N15", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("N16", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("N17", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("N18", dst.Tables[0].Rows[17][2].ToString());
                report.SetParameterValue("N19", dst.Tables[0].Rows[18][2].ToString());
                report.SetParameterValue("N20", dst.Tables[0].Rows[19][2].ToString());

                report.SetParameterValue("S01", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("S02", dst.Tables[0].Rows[21][2].ToString());
                report.SetParameterValue("S03", dst.Tables[0].Rows[22][2].ToString());
                report.SetParameterValue("S04", dst.Tables[0].Rows[23][2].ToString());
                report.SetParameterValue("S05", dst.Tables[0].Rows[24][2].ToString());
                report.SetParameterValue("S06", dst.Tables[0].Rows[25][2].ToString());
                report.SetParameterValue("S07", dst.Tables[0].Rows[26][2].ToString());
                report.SetParameterValue("S08", dst.Tables[0].Rows[27][2].ToString());

                report.SetParameterValue("GN01", dst.Tables[0].Rows[28][2].ToString());
                report.SetParameterValue("GN02", dst.Tables[0].Rows[29][2].ToString());
                report.SetParameterValue("GN03", dst.Tables[0].Rows[30][2].ToString());
                report.SetParameterValue("GN04", dst.Tables[0].Rows[31][2].ToString());
                report.SetParameterValue("GN05", dst.Tables[0].Rows[32][2].ToString());
                report.SetParameterValue("GN06", dst.Tables[0].Rows[33][2].ToString());
                report.SetParameterValue("GN07", dst.Tables[0].Rows[34][2].ToString());
                report.SetParameterValue("GN08", dst.Tables[0].Rows[35][2].ToString());
                report.SetParameterValue("GN09", dst.Tables[0].Rows[36][2].ToString());
                report.SetParameterValue("GN10", dst.Tables[0].Rows[37][2].ToString());

                report.SetParameterValue("GN11", dst.Tables[0].Rows[38][2].ToString());
                report.SetParameterValue("GN12", dst.Tables[0].Rows[39][2].ToString());
                report.SetParameterValue("GN13", dst.Tables[0].Rows[40][2].ToString());
                report.SetParameterValue("GN14", dst.Tables[0].Rows[41][2].ToString());
                report.SetParameterValue("GN15", dst.Tables[0].Rows[42][2].ToString());
                report.SetParameterValue("GN16", dst.Tables[0].Rows[43][2].ToString());
                report.SetParameterValue("GN17", dst.Tables[0].Rows[44][2].ToString());
                report.SetParameterValue("GN18", dst.Tables[0].Rows[45][2].ToString());
                report.SetParameterValue("GN19", dst.Tables[0].Rows[46][2].ToString());
                report.SetParameterValue("GN20", dst.Tables[0].Rows[47][2].ToString());

                report.SetParameterValue("QR01", dst.Tables[0].Rows[48][2].ToString());
                report.SetParameterValue("QR02", dst.Tables[0].Rows[49][2].ToString());

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
                    if (BoxPrintMode == "1")
                    {
                        report.PrintSettings.Printer = tt_printname;
                    }
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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }



        }

        //----以下是GP03数据采集----烽火移动标签\安徽电信标签二
        private void GetParaDataPrint_GP03(string tt_path, int tt_itemtype, string tt_printname)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();


            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            //设备标识码

            DataRow row1 = dt.NewRow();
            row1["参数"] = "N01";
            row1["名称"] = "设备01";
            row1["内容"] = GetListViewItem(3, 1);
            dt.Rows.Add(row1);

            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "设备02";
            row2["内容"] = GetListViewItem(3, 2);
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "设备03";
            row3["内容"] = GetListViewItem(3, 3);
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "设备04";
            row4["内容"] = GetListViewItem(3, 4);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "设备05";
            row5["内容"] = GetListViewItem(3, 5);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N06";
            row6["名称"] = "设备06";
            row6["内容"] = GetListViewItem(3, 6);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N07";
            row7["名称"] = "设备07";
            row7["内容"] = GetListViewItem(3, 7);
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N08";
            row8["名称"] = "设备08";
            row8["内容"] = GetListViewItem(3, 8);
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "N09";
            row9["名称"] = "设备09";
            row9["内容"] = GetListViewItem(3, 9);
            dt.Rows.Add(row9);

            DataRow row10 = dt.NewRow();
            row10["参数"] = "N10";
            row10["名称"] = "设备10";
            row10["内容"] = GetListViewItem(3, 10);
            dt.Rows.Add(row10);

            DataRow row11 = dt.NewRow();
            row11["参数"] = "N11";
            row11["名称"] = "设备11";
            row11["内容"] = GetListViewItem(3, 11);
            dt.Rows.Add(row11);

            DataRow row12 = dt.NewRow();
            row12["参数"] = "N12";
            row12["名称"] = "设备12";
            row12["内容"] = GetListViewItem(3, 12);
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "N13";
            row13["名称"] = "设备13";
            row13["内容"] = GetListViewItem(3, 13);
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "N14";
            row14["名称"] = "设备14";
            row14["内容"] = GetListViewItem(3, 14);
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "N15";
            row15["名称"] = "设备15";
            row15["内容"] = GetListViewItem(3, 15);
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "N16";
            row16["名称"] = "设备16";
            row16["内容"] = GetListViewItem(3, 16);
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "N17";
            row17["名称"] = "设备17";
            row17["内容"] = GetListViewItem(3, 17);
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "N18";
            row18["名称"] = "设备18";
            row18["内容"] = GetListViewItem(3, 18);
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "N19";
            row19["名称"] = "设备19";
            row19["内容"] = GetListViewItem(3, 19);
            dt.Rows.Add(row19);

            DataRow row20 = dt.NewRow();
            row20["参数"] = "N20";
            row20["名称"] = "设备20";
            row20["内容"] = GetListViewItem(3, 20);
            dt.Rows.Add(row20);
            
            //装箱序列号

            DataRow row21 = dt.NewRow();
            row21["参数"] = "P01";
            row21["名称"] = "序列号01";
            row21["内容"] = GetListViewItem(1, 1);
            dt.Rows.Add(row21);

            DataRow row22 = dt.NewRow();
            row22["参数"] = "P02";
            row22["名称"] = "序列号02";
            row22["内容"] = GetListViewItem(1, 2);
            dt.Rows.Add(row22);

            DataRow row23 = dt.NewRow();
            row23["参数"] = "P03";
            row23["名称"] = "序列号03";
            row23["内容"] = GetListViewItem(1, 3);
            dt.Rows.Add(row23);

            DataRow row24 = dt.NewRow();
            row24["参数"] = "P04";
            row24["名称"] = "序列号04";
            row24["内容"] = GetListViewItem(1, 4);
            dt.Rows.Add(row24);

            DataRow row25 = dt.NewRow();
            row25["参数"] = "P05";
            row25["名称"] = "序列号05";
            row25["内容"] = GetListViewItem(1, 5);
            dt.Rows.Add(row25);

            DataRow row26 = dt.NewRow();
            row26["参数"] = "P06";
            row26["名称"] = "序列号06";
            row26["内容"] = GetListViewItem(1, 6);
            dt.Rows.Add(row26);

            DataRow row27 = dt.NewRow();
            row27["参数"] = "P07";
            row27["名称"] = "序列号07";
            row27["内容"] = GetListViewItem(1, 7);
            dt.Rows.Add(row27);

            DataRow row28 = dt.NewRow();
            row28["参数"] = "P08";
            row28["名称"] = "序列号08";
            row28["内容"] = GetListViewItem(1, 8);
            dt.Rows.Add(row28);

            DataRow row29 = dt.NewRow();
            row29["参数"] = "P09";
            row29["名称"] = "序列号09";
            row29["内容"] = GetListViewItem(1, 9);
            dt.Rows.Add(row29);

            DataRow row30 = dt.NewRow();
            row30["参数"] = "P10";
            row30["名称"] = "序列号10";
            row30["内容"] = GetListViewItem(1, 10);
            dt.Rows.Add(row30);

            DataRow row31 = dt.NewRow();
            row31["参数"] = "P11";
            row31["名称"] = "序列号11";
            row31["内容"] = GetListViewItem(1, 11);
            dt.Rows.Add(row31);
        
            DataRow row32 = dt.NewRow();
            row32["参数"] = "P12";
            row32["名称"] = "序列号12";
            row32["内容"] = GetListViewItem(1, 12);
            dt.Rows.Add(row32);

            DataRow row33 = dt.NewRow();
            row33["参数"] = "P13";
            row33["名称"] = "序列号13";
            row33["内容"] = GetListViewItem(1, 13);
            dt.Rows.Add(row33);

            DataRow row34 = dt.NewRow();
            row34["参数"] = "P14";
            row34["名称"] = "序列号14";
            row34["内容"] = GetListViewItem(1, 14);
            dt.Rows.Add(row34);

            DataRow row35 = dt.NewRow();
            row35["参数"] = "P15";
            row35["名称"] = "序列号15";
            row35["内容"] = GetListViewItem(1, 15);
            dt.Rows.Add(row35);

            DataRow row36 = dt.NewRow();
            row36["参数"] = "P16";
            row36["名称"] = "序列号16";
            row36["内容"] = GetListViewItem(1, 16);
            dt.Rows.Add(row36);

            DataRow row37 = dt.NewRow();
            row37["参数"] = "P17";
            row37["名称"] = "序列号17";
            row37["内容"] = GetListViewItem(1, 17);
            dt.Rows.Add(row37);

            DataRow row38 = dt.NewRow();
            row38["参数"] = "P18";
            row38["名称"] = "序列号18";
            row38["内容"] = GetListViewItem(1, 18);
            dt.Rows.Add(row38);

            DataRow row39 = dt.NewRow();
            row39["参数"] = "P19";
            row39["名称"] = "序列号19";
            row39["内容"] = GetListViewItem(1, 19);
            dt.Rows.Add(row39);


            DataRow row40 = dt.NewRow();
            row40["参数"] = "P20";
            row40["名称"] = "序列号20";
            row40["内容"] = GetListViewItem(1, 20);
            dt.Rows.Add(row40);

            //设备标识暗码

            DataRow row41 = dt.NewRow();
            row41["参数"] = "A01";
            row41["名称"] = "移动暗码1";
            row41["内容"] = GetListViewItem(6, 1);
            dt.Rows.Add(row41);


            DataRow row42 = dt.NewRow();
            row42["参数"] = "A02";
            row42["名称"] = "移动暗码2";
            row42["内容"] = GetListViewItem(6, 2);
            dt.Rows.Add(row42);

            DataRow row43 = dt.NewRow();
            row43["参数"] = "A03";
            row43["名称"] = "移动暗码3";
            row43["内容"] = GetListViewItem(6, 3);
            dt.Rows.Add(row43);

            DataRow row44 = dt.NewRow();
            row44["参数"] = "A04";
            row44["名称"] = "移动暗码4";
            row44["内容"] = GetListViewItem(6, 4);
            dt.Rows.Add(row44);

            DataRow row45 = dt.NewRow();
            row45["参数"] = "A05";
            row45["名称"] = "移动暗码5";
            row45["内容"] = GetListViewItem(6, 5);
            dt.Rows.Add(row45);

            DataRow row46 = dt.NewRow();
            row46["参数"] = "A06";
            row46["名称"] = "移动暗码6";
            row46["内容"] = GetListViewItem(6, 6);
            dt.Rows.Add(row46);

            DataRow row47 = dt.NewRow();
            row47["参数"] = "A07";
            row47["名称"] = "移动暗码7";
            row47["内容"] = GetListViewItem(6, 7);
            dt.Rows.Add(row47);

            DataRow row48 = dt.NewRow();
            row48["参数"] = "A08";
            row48["名称"] = "移动暗码8";
            row48["内容"] = GetListViewItem(6, 8);
            dt.Rows.Add(row48);

            DataRow row49 = dt.NewRow();
            row49["参数"] = "A09";
            row49["名称"] = "移动暗码9";
            row49["内容"] = GetListViewItem(6, 9);
            dt.Rows.Add(row49);
            
            DataRow row50 = dt.NewRow();
            row50["参数"] = "A10";
            row50["名称"] = "移动暗码10";
            row50["内容"] = GetListViewItem(6, 10);
            dt.Rows.Add(row50);

            DataRow row51 = dt.NewRow();
            row51["参数"] = "A11";
            row51["名称"] = "移动暗码11";
            row51["内容"] = GetListViewItem(6, 11);
            dt.Rows.Add(row51);

            DataRow row52 = dt.NewRow();
            row52["参数"] = "A12";
            row52["名称"] = "移动暗码12";
            row52["内容"] = GetListViewItem(6, 12);
            dt.Rows.Add(row52);

            DataRow row53 = dt.NewRow();
            row53["参数"] = "A13";
            row53["名称"] = "移动暗码13";
            row53["内容"] = GetListViewItem(6, 13);
            dt.Rows.Add(row53);

            DataRow row54 = dt.NewRow();
            row54["参数"] = "A14";
            row54["名称"] = "移动暗码14";
            row54["内容"] = GetListViewItem(6, 14);
            dt.Rows.Add(row54);

            DataRow row55 = dt.NewRow();
            row55["参数"] = "A15";
            row55["名称"] = "移动暗码15";
            row55["内容"] = GetListViewItem(6, 15);
            dt.Rows.Add(row55);

            DataRow row56 = dt.NewRow();
            row56["参数"] = "A16";
            row56["名称"] = "移动暗码16";
            row56["内容"] = GetListViewItem(6, 16);
            dt.Rows.Add(row56);

            DataRow row57 = dt.NewRow();
            row57["参数"] = "A17";
            row57["名称"] = "移动暗码17";
            row57["内容"] = GetListViewItem(6, 17);
            dt.Rows.Add(row57);

            DataRow row58 = dt.NewRow();
            row58["参数"] = "A18";
            row58["名称"] = "移动暗码18";
            row58["内容"] = GetListViewItem(6, 18);
            dt.Rows.Add(row58);

            DataRow row59 = dt.NewRow();
            row59["参数"] = "A19";
            row59["名称"] = "移动暗码19";
            row59["内容"] = GetListViewItem(6, 19);
            dt.Rows.Add(row59);

            DataRow row60 = dt.NewRow();
            row60["参数"] = "A20";
            row60["名称"] = "移动暗码20";
            row60["内容"] = GetListViewItem(6, 20);
            dt.Rows.Add(row60);

            //GPSN条码（常规产品用）

            DataRow row61 = dt.NewRow();
            row61["参数"] = "GP01";
            row61["名称"] = "GPSN01";
            row61["内容"] = GetListViewItem(4, 1);
            dt.Rows.Add(row61);


            DataRow row62 = dt.NewRow();
            row62["参数"] = "GP02";
            row62["名称"] = "GPSN02";
            row62["内容"] = GetListViewItem(4, 2);
            dt.Rows.Add(row62);

            DataRow row63 = dt.NewRow();
            row63["参数"] = "GP03";
            row63["名称"] = "GPSN03";
            row63["内容"] = GetListViewItem(4, 3);
            dt.Rows.Add(row63);

            DataRow row64 = dt.NewRow();
            row64["参数"] = "GP04";
            row64["名称"] = "GPSN04";
            row64["内容"] = GetListViewItem(4, 4);
            dt.Rows.Add(row64);

            DataRow row65 = dt.NewRow();
            row65["参数"] = "GP05";
            row65["名称"] = "GPSN05";
            row65["内容"] = GetListViewItem(4, 5);
            dt.Rows.Add(row65);

            DataRow row66 = dt.NewRow();
            row66["参数"] = "GP06";
            row66["名称"] = "GPSN06";
            row66["内容"] = GetListViewItem(4, 6);
            dt.Rows.Add(row66);

            DataRow row67 = dt.NewRow();
            row67["参数"] = "GP07";
            row67["名称"] = "GPSN07";
            row67["内容"] = GetListViewItem(4, 7);
            dt.Rows.Add(row67);

            DataRow row68 = dt.NewRow();
            row68["参数"] = "GP08";
            row68["名称"] = "GPSN08";
            row68["内容"] = GetListViewItem(4, 8);
            dt.Rows.Add(row68);

            DataRow row69 = dt.NewRow();
            row69["参数"] = "GP09";
            row69["名称"] = "GPSN09";
            row69["内容"] = GetListViewItem(4, 9);
            dt.Rows.Add(row69);

            DataRow row70 = dt.NewRow();
            row70["参数"] = "GP10";
            row70["名称"] = "GPSN10";
            row70["内容"] = GetListViewItem(4, 10);
            dt.Rows.Add(row70);

            //第二步加载到表格显示
            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 40;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 300;


            //第三步 打印或预览
            if (dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && tt_itemtype > 0)
            {
                FastReport.Report report = new FastReport.Report();

                report.Prepare();
                report.Load(tt_path);
                report.SetParameterValue("N01", dst.Tables[0].Rows[0][2].ToString());
                report.SetParameterValue("N02", dst.Tables[0].Rows[1][2].ToString());
                report.SetParameterValue("N03", dst.Tables[0].Rows[2][2].ToString());
                report.SetParameterValue("N04", dst.Tables[0].Rows[3][2].ToString());
                report.SetParameterValue("N05", dst.Tables[0].Rows[4][2].ToString());
                report.SetParameterValue("N06", dst.Tables[0].Rows[5][2].ToString());
                report.SetParameterValue("N07", dst.Tables[0].Rows[6][2].ToString());
                report.SetParameterValue("N08", dst.Tables[0].Rows[7][2].ToString());
                report.SetParameterValue("N09", dst.Tables[0].Rows[8][2].ToString());
                report.SetParameterValue("N10", dst.Tables[0].Rows[9][2].ToString());

                report.SetParameterValue("N11", dst.Tables[0].Rows[10][2].ToString());
                report.SetParameterValue("N12", dst.Tables[0].Rows[11][2].ToString());
                report.SetParameterValue("N13", dst.Tables[0].Rows[12][2].ToString());
                report.SetParameterValue("N14", dst.Tables[0].Rows[13][2].ToString());
                report.SetParameterValue("N15", dst.Tables[0].Rows[14][2].ToString());
                report.SetParameterValue("N16", dst.Tables[0].Rows[15][2].ToString());
                report.SetParameterValue("N17", dst.Tables[0].Rows[16][2].ToString());
                report.SetParameterValue("N18", dst.Tables[0].Rows[17][2].ToString());
                report.SetParameterValue("N19", dst.Tables[0].Rows[18][2].ToString());
                report.SetParameterValue("N20", dst.Tables[0].Rows[19][2].ToString());

                report.SetParameterValue("P01", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("P02", dst.Tables[0].Rows[21][2].ToString());
                report.SetParameterValue("P03", dst.Tables[0].Rows[22][2].ToString());
                report.SetParameterValue("P04", dst.Tables[0].Rows[23][2].ToString());
                report.SetParameterValue("P05", dst.Tables[0].Rows[24][2].ToString());
                report.SetParameterValue("P06", dst.Tables[0].Rows[25][2].ToString());
                report.SetParameterValue("P07", dst.Tables[0].Rows[26][2].ToString());
                report.SetParameterValue("P08", dst.Tables[0].Rows[27][2].ToString());
                report.SetParameterValue("P09", dst.Tables[0].Rows[28][2].ToString());
                report.SetParameterValue("P10", dst.Tables[0].Rows[29][2].ToString());

                report.SetParameterValue("P11", dst.Tables[0].Rows[30][2].ToString());
                report.SetParameterValue("P12", dst.Tables[0].Rows[31][2].ToString());
                report.SetParameterValue("P13", dst.Tables[0].Rows[32][2].ToString());
                report.SetParameterValue("P14", dst.Tables[0].Rows[33][2].ToString());
                report.SetParameterValue("P15", dst.Tables[0].Rows[34][2].ToString());
                report.SetParameterValue("P16", dst.Tables[0].Rows[35][2].ToString());
                report.SetParameterValue("P17", dst.Tables[0].Rows[36][2].ToString());
                report.SetParameterValue("P18", dst.Tables[0].Rows[37][2].ToString());
                report.SetParameterValue("P19", dst.Tables[0].Rows[38][2].ToString());
                report.SetParameterValue("P20", dst.Tables[0].Rows[39][2].ToString());

                report.SetParameterValue("A01", dst.Tables[0].Rows[40][2].ToString());
                report.SetParameterValue("A02", dst.Tables[0].Rows[41][2].ToString());
                report.SetParameterValue("A03", dst.Tables[0].Rows[42][2].ToString());
                report.SetParameterValue("A04", dst.Tables[0].Rows[43][2].ToString());
                report.SetParameterValue("A05", dst.Tables[0].Rows[44][2].ToString());
                report.SetParameterValue("A06", dst.Tables[0].Rows[45][2].ToString());
                report.SetParameterValue("A07", dst.Tables[0].Rows[46][2].ToString());
                report.SetParameterValue("A08", dst.Tables[0].Rows[47][2].ToString());
                report.SetParameterValue("A09", dst.Tables[0].Rows[48][2].ToString());
                report.SetParameterValue("A10", dst.Tables[0].Rows[49][2].ToString());

                report.SetParameterValue("A11", dst.Tables[0].Rows[50][2].ToString());
                report.SetParameterValue("A12", dst.Tables[0].Rows[51][2].ToString());
                report.SetParameterValue("A13", dst.Tables[0].Rows[52][2].ToString());
                report.SetParameterValue("A14", dst.Tables[0].Rows[53][2].ToString());
                report.SetParameterValue("A15", dst.Tables[0].Rows[54][2].ToString());
                report.SetParameterValue("A16", dst.Tables[0].Rows[55][2].ToString());
                report.SetParameterValue("A17", dst.Tables[0].Rows[56][2].ToString());
                report.SetParameterValue("A18", dst.Tables[0].Rows[57][2].ToString());
                report.SetParameterValue("A19", dst.Tables[0].Rows[58][2].ToString());
                report.SetParameterValue("A20", dst.Tables[0].Rows[59][2].ToString());

                report.SetParameterValue("GP01", dst.Tables[0].Rows[60][2].ToString());
                report.SetParameterValue("GP02", dst.Tables[0].Rows[61][2].ToString());
                report.SetParameterValue("GP03", dst.Tables[0].Rows[62][2].ToString());
                report.SetParameterValue("GP04", dst.Tables[0].Rows[63][2].ToString());
                report.SetParameterValue("GP05", dst.Tables[0].Rows[64][2].ToString());
                report.SetParameterValue("GP06", dst.Tables[0].Rows[65][2].ToString());
                report.SetParameterValue("GP07", dst.Tables[0].Rows[66][2].ToString());
                report.SetParameterValue("GP08", dst.Tables[0].Rows[67][2].ToString());
                report.SetParameterValue("GP09", dst.Tables[0].Rows[68][2].ToString());
                report.SetParameterValue("GP10", dst.Tables[0].Rows[69][2].ToString());

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
                    if (BoxPrintMode == "1")
                    {
                        report.PrintSettings.Printer = tt_printname;
                    }
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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }

        }
        

        #endregion

        
        #region 13、打散记录查询
        //确定
        private void button8_Click(object sender, EventArgs e)
        {
            this.dataGridView7.DataSource = null;

            string tt_task = this.textBox15.Text.Trim();
            if (this.textBox15.Text != "")
            {
                tt_task = " and Taskcode =  '" + this.textBox15.Text + "' ";
            }

            string tt_mac = "";
            if (this.textBox16.Text.Trim() != "")
            {
                tt_mac = " and Maclable = '" + this.textBox16.Text.Trim() + "' ";
            }

            string tt_sn = "";
            if (this.textBox17.Text.Trim() != "")
            {
                tt_sn = " and Hostlable = '" + this.textBox17.Text.Trim() + "' ";
            }



            //-----查询时间
            string tt_date1 = this.dateTimePicker1.Text;
            string tt_date2 = this.dateTimePicker2.Text;

            string tt_sql1 = "select Taskcode 工单, Pagesn 箱号, Maclable MAC, Hostlable SN条码, Pagetime 装箱时间,Fdate 打散时间 " +
                             "from odc_pagebreakup  " +
                            "where  Fdate  between '" + tt_date1 + "' and '" + tt_date2 + "' " + tt_task + tt_mac + tt_sn;

            DataSet ds1 = Dataset1.GetDataSet(tt_sql1, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView7.DataSource = ds1;
                dataGridView7.DataMember = "Table";

            }
            else
            {
                MessageBox.Show("sorry,没有查询到数据");
            }
        }


        //重置
        private void button9_Click(object sender, EventArgs e)
        {
            this.textBox15.Text = null;
            this.textBox16.Text = null;
            this.textBox17.Text = null;
            this.dataGridView7.DataSource = null;
        }
        
        //显示行号
        private void dataGridView7_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SolidBrush brushOne = new SolidBrush(Color.Red);
            e.Graphics.DrawString(Convert.ToString(e.RowIndex + 1, System.Globalization.CultureInfo.CurrentUICulture), e.InheritedRowStyle.Font, brushOne, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
        }
        #endregion 

        //-----------end---------


    }
}
