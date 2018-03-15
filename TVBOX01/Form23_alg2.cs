using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TVBOX01
{
    public partial class Form23_alg2 : Form
    {
        public Form23_alg2()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_conn;
        private int tt_interval;

        //加载
        private void Form23_alg2_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";

            this.button1.Visible = false;
            this.button3.Visible = false;
            this.button4.Visible = false;

            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label13.Text = tt_interval.ToString();
            this.radioButton1.Checked = true;
            this.radioButton5.Checked = true;

            clearItem1();
            clearCrean();
            setDecodeFileType();
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


        #region 2、事件清除
        //清除信息
        private void clearItem1()
        {
            //工单信息
            this.label20.Text = null;
            this.label23.Text = null;
            this.label24.Text = null;
            this.label27.Text = null;

            //站位跳转信息
            this.textBox2.Text = null;
            this.textBox4.Text = null;

        }

        //执行时清除
        private void clearCrean()
        {
            this.richTextBox2.Text = null;
            this.richTextBox3.Text = null;
            this.richTextBox3.BackColor = Color.White;

            this.label6.Text = null;
            this.label9.Text = null;
            this.label11.Text = null;
            this.label18.Text = null;
            this.label19.Text = null;


        }


        //加载解析数据类型
        private void setDecodeFileType()
        {
            setRichtexBox2("解析一文件类型");
            setRichtexBox2("XZWJ03406177006350_07.24.15.37.24.csv");
            setRichtexBox2(" ");
            setRichtexBox2("解析二文件类型");
            setRichtexBox2("2017_XZRH03406177002507_7_24_13_34_50_961_SLOT_1.txt");
            setRichtexBox2(" ");
            setRichtexBox2("解析三文件类型");
            setRichtexBox2("1_XZWJ03386176039903_20170724_152923_PASS.txt");
            setRichtexBox2(" ");
            setRichtexBox2("解析四文件类型");
            setRichtexBox2("[Log]XZWJ03366176024292_6H36D5S.txt");
        }

        #endregion

        #region 3、锁定功能
        //工单锁定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                string tt_sql1 = "select  tasksquantity,product_name,areacode,Gyid " +
                                 "from odc_tasks where taskscode = '" + this.textBox1.Text + "' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label20.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label23.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString(); //产品名称
                    this.label24.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区编码
                    this.label27.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString(); //流程


                    //第一步、流程检查
                    Boolean tt_flag1 = false;
                    if (!this.label27.Text.Equals(""))
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


                    if (tt_flag1)
                    {
                        this.textBox1.Enabled = false;

                        this.button1.Visible = true;
                        this.button3.Visible = true;
                        this.button4.Visible = true;
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
                clearItem1();

                this.button1.Visible = false;
                this.button3.Visible = false;
                this.button4.Visible = false;

            }
        }

        //目录锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                string tt_delepath = this.textBox3.Text;
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("确定要目录:" + tt_delepath + ",中的文件吗?，删除点击确定", "文件删除", messButton);


                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    int tt_delint = DelectDir2(tt_delepath);
                    MessageBox.Show("已删除了：" + tt_delint.ToString()+"个文件");
                }
                else
                {

                }



                this.textBox3.Enabled = false;
                this.textBox11.Enabled = false;
                this.comboBox1.Enabled = false;
            }
            else
            {
                this.textBox3.Enabled = true;
                this.textBox11.Enabled = true;
                this.comboBox1.Enabled = true;
            }

            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label13.Text = tt_interval.ToString();
        }

        //周期下拉列表
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label13.Text = tt_interval.ToString();
        }


        #endregion


        #region 4、辅助功能
        //richtext加记录
        private void setRichtexBox(string tt_textinfor)
        {
            this.richTextBox3.Text = this.richTextBox3.Text + tt_textinfor + "\n";
        }


        //richtext2加记录
        private void setRichtexBox2(string tt_textinfor)
        {
            this.richTextBox1.Text = this.richTextBox1.Text + tt_textinfor + "\n";
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



        //获取验证铭牌路径
        private Boolean getPathIstrue2(string tt_file)
        {
            Boolean tt_flag = false;
            //if (File.Exists(@tt_file))
            if (Directory.Exists(@tt_file))
            {
                tt_flag = true;
            }
            else
            {
                tt_flag = false;
            }


            return tt_flag;
        }


        //文件移动
        private bool fileMove(string tt_path1, string tt_path2)
        {
            Boolean tt_flag = false;

            try
            {

                // Ensure that the target does not exist.
                if (File.Exists(tt_path2))
                    File.Delete(tt_path2);

                // Move the file.
                File.Move(tt_path1, tt_path2);

                tt_flag = true;


            }
            catch
            {
                tt_flag = false;
            }

            return tt_flag;
        }


        //删除文件目录及子文件（不能把这bak的文件也删除掉的）
        public static void DelectDir(string srcPath)
        {
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
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        //删除文件目录及子文件（不能把这bak的文件也删除掉的）
        public int DelectDir2(string srcPath)
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
                        //DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        //subdir.Delete(true);          //删除子目录和文件
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




     #endregion


        #region 5、数据功能

        //存储过程执行
        public static string Set_Next_Station(string tt_pcba, string tt_incode)
        {

            string com = @"dbo.PR_CODEPASS";
            IDataParameter[] paramers = new IDataParameter[4];
            paramers[0] = new SqlParameter("@tt_pcba", SqlDbType.VarChar);
            paramers[0].Value = tt_pcba;

            paramers[1] = new SqlParameter("@tt_incode", SqlDbType.VarChar);
            paramers[1].Value = tt_incode;

            paramers[2] = new SqlParameter("@tt_testinfo", SqlDbType.VarChar);
            paramers[2].Value = "1";

            paramers[3] = new SqlParameter("@tt_outmessage", SqlDbType.VarChar, 200);
            paramers[3].Direction = ParameterDirection.Output;
            try
            {
                string r = Dataset1.stringExecSPCommand(com, paramers, tt_conn);

                return r;

            }
            catch
            {
                return "存储过程执行有问题";
            }

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
                this.textBox2.Text = tt_ccode;
                this.textBox4.Text = tt_ncode;
            }




            return tt_flag;
        }






        #endregion



        #region 6、站位查询
        //数据查询确定
        private void button6_Click(object sender, EventArgs e)
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

        //数据查询重置
        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox16.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;
        }
        #endregion


        #region 7、日志记录查询

        //日志记录查询确定
        private void button7_Click(object sender, EventArgs e)
        {
            this.dataGridView2.DataSource = null;

            //-----不良时间
            string tt_date1 = this.dateTimePicker1.Text;
            string tt_date2 = this.dateTimePicker2.Text;

            //工单号
            string tt_task = "";
            if (this.textBox5.Text != "")
            {
                tt_task = " and Ftaskscode =  '" + this.textBox5.Text + "' ";
            }

            //单板号
            string tt_pcba = "";
            if (this.textBox6.Text != "")
            {
                tt_pcba = " and SN =  '" + this.textBox6.Text + "' ";
            }

            //主机条码
            string tt_hostlable = "";
            if (this.textBox7.Text != "")
            {
                tt_hostlable = " and CMCC_SN =  '" + this.textBox7.Text + "' ";
            }

            //MAC
            string tt_mac = "";
            if (this.textBox8.Text != "")
            {
                tt_hostlable = " and Fmac =  '" + this.textBox8.Text + "' ";
            }


            string tt_sql1 = "select Ftaskscode 工单号, SN 单板号, CMCC_SN 主机条码, Fmac MAC, analysisDate 过站日期,Fcode 过站站位, Ffilename 过站文件 " +
                            "from ODC_Analysis_Log " +
                            "where analysisDate  between '" + tt_date1 + "' and '" + tt_date2 + "' " + tt_task + tt_pcba + tt_hostlable + tt_mac;

            DataSet ds1 = Dataset1.GetDataSet(tt_sql1, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView2.DataSource = ds1;
                dataGridView2.DataMember = "Table";
            }
            else
            {
                MessageBox.Show("sorry,没有查询到数据");
            }
        }

        //日志记录查询重置
        private void button8_Click(object sender, EventArgs e)
        {
            this.textBox5.Text = null;
            this.textBox6.Text = null;
            this.textBox7.Text = null;
            this.textBox8.Text = null;
            this.dataGridView2.DataSource = null;
        }

        //显示行号
        private void dataGridView2_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }

        #endregion


        #region 8、时间控件
        //时间控件
        private void timer1_Tick(object sender, EventArgs e)
        {
            tt_interval--;
            this.label13.Text = tt_interval.ToString();
            if (tt_interval <= 0)
            {


                LogStationPass();


                tt_interval = Convert.ToInt32(this.comboBox1.Text);
                this.label13.Text = tt_interval.ToString();
            }
        }
        #endregion


        #region 9、按钮功能

        //目录选择
        private void button3_Click(object sender, EventArgs e)
        {
            string path = "";
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (fbd.SelectedPath != "")
                {
                    path = fbd.SelectedPath;
                    textBox3.Text = path;
                }
            }
        }

        //开始
        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                button1.Text = "开始";

                timer1.Stop();
            }
            else
            {

                try
                {

                    button1.Text = "停止";
                    timer1.Start();
                }
                catch (Exception)
                {

                }
            }
        }

        //执行
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.button1.Text == "停止")
            {
                MessageBox.Show("请先停止自动执行");
            }
            else
            {
                LogStationPass();
            }
        }

        //重置
        private void button2_Click(object sender, EventArgs e)
        {
            clearCrean();
        }

        #endregion



        #region 10、主要方法

        //只要方法
        private void LogStationPass()
        {
            //数据初始化
            clearCrean();
            setRichtexBox("---开始新的过站---");
            string tt_taskcode = this.textBox1.Text.Trim();  //工单号
            string tt_code = this.textBox2.Text.Trim();  //跳转站位

            if (this.radioButton1.Checked) setRichtexBox("--文件解析一：文件名格式：XZWJ03406177006350_07.24.15.37.24.csv");
            if (this.radioButton2.Checked) setRichtexBox("--文件解析二：文件名格式：2017_XZRH03406177002507_7_24_13_34_50_961_SLOT_1.txt");
            if (this.radioButton3.Checked) setRichtexBox("--文件解析三：文件名格式：1_XZWJ03386176039903_20170724_152923_PASS.txt");
            if (this.radioButton4.Checked) setRichtexBox("--文件解析四：文件名格式：[Log]XZWJ03366176024292_6H36D5S.txt");




            //第一步 查看是否填写日志目录
            string tt_path = this.textBox3.Text.Trim();
            Boolean tt_flag1 = false;
            if (!tt_path.Equals(""))
            {
                setRichtexBox("第一步：已选择LOG存储目录，goon" + tt_path);
                tt_flag1 = getPathIstrue2(tt_path);
                if (tt_flag1)
                {
                    setRichtexBox("第1.1步：选择的目录存在,goon," + tt_path);
                }
                else
                {
                    setRichtexBox("第1.1步：选择的目录不存在,over,"+tt_path);
                }

            }
            else
            {
                setRichtexBox("第一步：没有选择LOG存储目录,over");
            }




            //第二步 查看是否存在bak目录,不存在就创建一个目录
            Boolean tt_flag2 = false;
            if (tt_flag1)
            {
                string tt_bpath = this.textBox3.Text + @"\bak";

                if (!Directory.Exists(tt_bpath))
                {
                    Directory.CreateDirectory(tt_bpath);
                    setRichtexBox("第二步：没有bak目录，新建一个目录");

                }
                else
                {
                    setRichtexBox("第二步：已存在bak目录");
                }
                tt_flag2 = true;
            }




            //第三步 获取待过站信息
            Boolean tt_flag3 = false;
            string tt_filenumber = "0";
            if (tt_flag1 && tt_flag2)
            {
                tt_filenumber = GetLogText();
                tt_flag3 = true;
                setRichtexBox("第三步：获取到待测文件:" + tt_filenumber);
            }


            //第四步 开始在目录中循环
            Boolean tt_flag4 = false;
            string tt_fullname = "";
            string tt_file = "";
            string tt_movefilename = "";
            string tt_pcba = "";
            int tt_intfilelength = 0;
            string tt_task = this.textBox1.Text;
            string tt_passinfo = "";  //过站返回信息

            int tt_passs = 0;  //过站成功数量
            int tt_fail = 0;   //过站失败数量
            int tt_move = 0;   //转移成功数量
            int tt_record = 0;  //记录成功数
            if (tt_flag3)
            {
                setRichtexBox("第3步：开始LOG过站操作");
                setRichtexBox("文件数：" + tt_filenumber);
                int tt_textnumber = 0;
                DirectoryInfo folder = new DirectoryInfo(tt_path);
                foreach (FileInfo file in folder.GetFiles("*.*"))
                {

                    #region 文件目录循环
                    setRichtexBox("--- " + tt_textnumber.ToString() + " ----");


                    //第1步，获取文件名及路径
                    tt_fullname = file.FullName;
                    setRichtexBox("1、文件路径：" + tt_fullname);

                    //第2步，获取文件名
                    tt_file = file.Name;
                    setRichtexBox("2、文件名：" + tt_file);

                    //第3步， 移动后的文件路径
                    tt_movefilename = this.textBox3.Text + @"\bak\" + tt_file; //文件名2
                    setRichtexBox("3、移动的文件名:" + tt_movefilename);

                    //第4步 文件解析
                    tt_pcba = getDecodeFileName(tt_file);
                    setRichtexBox("4、解析单板号：" + tt_pcba);

                    //第5步 文件名长度进行判断
                    bool tt_flag5 = false;
                    tt_intfilelength = tt_pcba.Length;
                    if (tt_intfilelength.ToString() == this.textBox11.Text.Trim())
                    {
                        tt_flag5 = true;               
                        setRichtexBox("5、文件名解析长度正确,OK：" + tt_intfilelength.ToString());
                    }
                    else
                    {
                        setRichtexBox("5、文件名解析长度不正确，fail：" + tt_intfilelength.ToString());
                    }



                    //第6步 开始过站
                    bool tt_flag6 = false;
                    tt_passinfo = Set_Next_Station(tt_pcba, this.textBox2.Text);
                    if(tt_flag5)
                    {
                        if (tt_passinfo == "1")
                        {
                            tt_flag6 = true;
                            tt_passs++;
                            setRichtexBox("6、过站成功:OK," + tt_passinfo);
                        }
                        else
                        {
                            tt_fail++;
                            setRichtexBox("6、过站不成功:Fail," + tt_passinfo);
                        }
                    }


                    //第7步 文件转移
                    bool tt_flag7 = false;
                    if (tt_flag6)
                    {
                        Boolean tt_moveflag = fileMove(tt_fullname, tt_movefilename);
                        if (tt_moveflag)
                        {
                            tt_move++;
                            setRichtexBox("7、文件转移成功:OK");
                        }
                        else
                        {
                            setRichtexBox("7、文件转移不成功:Fail");
                        }

                        tt_flag7 = true;

                    }


                    //第8步信息记录
                    if (tt_flag5 && tt_flag6 && tt_flag7)
                    {
                        string tt_sql = "insert into ODC_Analysis_Log (sn,cmcc_sn,analysisDate,Fmac,Ffilename,Ftaskscode,Fcode) " +
                         " values('" + tt_pcba + "','" + tt_pcba + "',getdate(),'" + tt_pcba + "','" + tt_file + "','" + tt_taskcode + "','" + tt_code + "') ";

                        int tt_num = Dataset1.ExecCommand(tt_sql, tt_conn);

                        if (tt_num > 0)
                        {
                            tt_record++;
                            setRichtexBox("8、数据记录成功:");
                        }
                        else
                        {
                            setRichtexBox("8、数据记录不成功:");
                        }


                    }

                    tt_textnumber++;

                    #endregion

                }

                this.label18.Text = tt_passs.ToString();
                this.label19.Text = tt_fail.ToString();
                this.label9.Text = tt_move.ToString();
                this.label6.Text = tt_record.ToString();
                tt_flag4 = true;
            }



            //最后总计
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
            {
                this.richTextBox3.BackColor = Color.Chartreuse;
            }
            else
            {
                this.richTextBox3.BackColor = Color.Red;
            }




        }


        //或取工作目录文件
        private string GetLogText()
        {

            string tt_path = this.textBox3.Text;
            DirectoryInfo folder = new DirectoryInfo(tt_path);
            int tt_textnumber = 0;
            foreach (FileInfo file in folder.GetFiles("*.*"))
            {
                this.richTextBox2.Text = file.FullName + "\n" + this.richTextBox2.Text;
                tt_textnumber++;
            }

            this.label11.Text = tt_textnumber.ToString();
            return tt_textnumber.ToString();
        }


        //获取已过站的文件数量
        private void GetStationPassFile()
        {
            string tt_path = this.textBox3.Text + @"\bak";
            DirectoryInfo folder = new DirectoryInfo(tt_path);
            int tt_textnumber = 0;
            foreach (FileInfo file in folder.GetFiles("*.*"))
            {
                this.richTextBox1.Text = file.FullName + "\n" + this.richTextBox1.Text;
                tt_textnumber++;
            }

            this.label9.Text = tt_textnumber.ToString();
        }


        //文件解析
        private string getDecodeFileName(string tt_filename)
        {
            string tt_decodefile = "";
            if (this.radioButton1.Checked) tt_decodefile = getFileName1(tt_filename);
            if (this.radioButton2.Checked) tt_decodefile = getFileName2(tt_filename);
            if (this.radioButton3.Checked) tt_decodefile = getFileName2(tt_filename);
            if (this.radioButton4.Checked) tt_decodefile = getFileName3(tt_filename);
            return tt_decodefile;
        }




        #endregion


        #region 11、文件解析测试
        //文件选择
        private void button9_Click(object sender, EventArgs e)
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
                string file = fileDialog.SafeFileName;
                // MessageBox.Show("已选择文件:" + file, "选择文件提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.textBox9.Text = file;
            }
        }


        //确定
        private void button11_Click(object sender, EventArgs e)
        {
            this.textBox10.Text = null;
            if (this.radioButton5.Checked == true) this.textBox10.Text = getFileName1(this.textBox9.Text);
            if (this.radioButton6.Checked == true) this.textBox10.Text = getFileName2(this.textBox9.Text);
            if (this.radioButton7.Checked == true) this.textBox10.Text = getFileName2(this.textBox9.Text);
            if (this.radioButton8.Checked == true) this.textBox10.Text = getFileName3(this.textBox9.Text);
            MessageBox.Show("解析完毕，请确认");

        }

        //重置
        private void button10_Click(object sender, EventArgs e)
        {
            this.textBox9.Text = null;
            this.textBox10.Text = null;

        }

        #endregion



        #region 12、文件解析方法

        //单板号在最前面后面是下滑线 XZRH03406177001369_07.24.13.37.35.txt
        private string getFileName1(string tt_instr)
        {
            string tt_outstr = "";

            int tt_mark1 = tt_instr.IndexOf("_");

            if (tt_mark1 > 0)
            {
                tt_outstr = tt_instr.Substring(0, tt_mark1);
            }
            return tt_outstr;
        }


        //单板号在第一个下滑线和第二个下滑线之间 2017_XZRH03406177002507_7_24_13_34_50_961_SLOT_1.txt
        private string getFileName2(string tt_instr)
        {
            string tt_outstr = "";

            int tt_mark1 = tt_instr.IndexOf("_");

            int tt_mark2 = tt_instr.IndexOf("_", tt_mark1+1);

            if (tt_mark2 > 0)
            {
              tt_outstr = tt_instr.Substring(tt_mark1 + 1, tt_mark2 - tt_mark1-1);
            }

            return tt_outstr;
        }


        //单板号在第二个括号和第一个下滑线之间 [Log]XZWJ03366176024292_6H36D5S.txt
        private string getFileName3(string tt_instr)
        {
            string tt_outstr = "";

            int tt_mark1 = tt_instr.IndexOf("]");

            int tt_mark2 = tt_instr.IndexOf("_");

            if (tt_mark2 > 0)
            {
                tt_outstr = tt_instr.Substring(tt_mark1 + 1, tt_mark2 - tt_mark1 - 1);
            }

            return tt_outstr;
        }





        #endregion

        


















    }
}
