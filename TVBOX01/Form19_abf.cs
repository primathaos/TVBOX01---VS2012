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
    public partial class Form19_abf : Form
    {
        public Form19_abf()
        {
            InitializeComponent();
        }

        #region 1、属性定义
        static string tt_conn;
        private string path;
        private int tt_interval;

        private void Form19_abf_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";

            if (str == "FH901")
            {
                this.radioButton1.Checked = true;
                this.Text = this.Text + " WIFI校准测试";
            }
            else
            {
                this.radioButton2.Checked = true;
                this.Text = this.Text + " 吞吐量测试";
            }


            this.button1.Visible = false;
            this.button3.Visible = false;

            tt_interval = Convert.ToInt32(this.comboBox1.Text);

            this.label13.Text = tt_interval.ToString();

            clearItem1();
            clearCrean();

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

        private void clearCrean()
        {
            this.richTextBox1.Text = null;
            this.richTextBox2.Text = null;
            this.richTextBox3.Text = null;
            this.richTextBox3.BackColor = Color.White;
            this.dataGridView1.DataSource = null;

            this.label9.Text = null;
            this.label10.Text = null;
            this.label11.Text = null;

            this.label18.Text = null;
            this.label19.Text = null;
            this.label21.Text = null;

        }


        #endregion



        #region 3、锁定事件及周期选择
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


                    if (tt_flag1 )
                    {
                        this.textBox1.Enabled = false;

                        this.button1.Visible = true;
                        this.button3.Visible = true;
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
            }
        }


        //目录锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.textBox3.Enabled = false;
                this.comboBox1.Enabled = false;
            }
            else
            {
                this.textBox3.Enabled = true;
                this.comboBox1.Enabled = true;
            }

            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label13.Text = tt_interval.ToString();
        }

        //周期选择
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label13.Text = tt_interval.ToString();
        }


        #endregion


        #region 4、按钮功能

        //目录选择
        private void button3_Click(object sender, EventArgs e)
        {
            path = "";
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


        //重置按钮
        private void button2_Click(object sender, EventArgs e)
        {
            clearCrean();
        }


        //开始或接受按钮
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


        //执行按钮
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
        #endregion




        #region 5、方法

        //主方法
        private void LogStationPass()
        {

            //数据初始化
            DataSet ds = null;
            clearCrean();
            setRichtexBox("---开始新的过站---");
            string tt_taskcode = this.textBox1.Text.Trim();  //工单号
            string tt_code = this.textBox2.Text.Trim();  //工单号

            if( this.radioButton1.Checked)
            {
                setRichtexBox("--按条件1文件名有单板号过站");
            }
            else
            {
                setRichtexBox("--按条件2文件名有PASS+MAC过站");
            }

            setRichtexBox("--工单号：" + tt_taskcode);
            setRichtexBox("--待测站位：" + tt_code);



            //第一步 查看是否填写日志目录
            Boolean tt_flag1 = false;
            if (!this.textBox3.Text.Equals(""))
            {
                setRichtexBox("第1步：已选择LOG存储目录，goon");
                tt_flag1 = getPathIstrue2(this.textBox3.Text.Trim());
                if (tt_flag1)
                {
                    setRichtexBox("第1.1步：选择的目录存在,goon");
                }
                else
                {
                    setRichtexBox("第1.1步：选择的目录不存在,over");
                }
                
            }
            else
            {
                setRichtexBox("第1步：没有选择LOG存储目录,over");
            }


            //第二步 查看是否存在bak目录,不存在就创建一个目录
            Boolean tt_flag2 = false;
            if (tt_flag1)
            {
                string tt_bpath = this.textBox3.Text + @"\bak";

                if (!Directory.Exists(tt_bpath))
                {
                    Directory.CreateDirectory(tt_bpath);
                    setRichtexBox("第2步：没有bak目录，新建一个目录");

                }
                else
                {
                    setRichtexBox("第2步：已存在bak目录");
                }
                tt_flag2 = true;
            }


            //第三步 获取待过站信息
            Boolean tt_flag3 = false;
            if (tt_flag1 && tt_flag2)
            {
                string tt_filenumber = GetLogText();
                tt_flag3 = true;
                setRichtexBox("第3步：获取到待测文件:" + tt_filenumber);
            }


            //第四步 获取站位信息
            Boolean tt_flag4 = false;
            if (tt_flag1 && tt_flag2 && tt_flag3)
            {
                ds = GetStationInfo();
                tt_flag4 = true;
                setRichtexBox("第4步：已获取待测站位信息");
            }


            //第五步 获取bak目录信息
            Boolean tt_flag5 = false;
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
            {
                GetStationPassFile();
                tt_flag5 = true;
                setRichtexBox("第5步：已获取bak目录信息");
            }




            //第六步开始过站操作
            Boolean tt_flag6 = false;
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
            {
                int tt_passs = 0;
                int tt_fail = 0;
                int tt_notext = 0;

                string tt_mac = "";
                string tt_pcba = "";
                string tt_hostlable = "";
                string tt_longstr = "";
                string tt_textfile = "";
                string tt_textfile2 = "";
                string tt_passinfo = "";
                string tt_logpatn = this.textBox3.Text.Trim();
                

                setRichtexBox("第6步：开始过站");

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    Boolean tt_flag01 = false;  //老的方法先不用


                    #region 老的过站循环
                    if (tt_flag01)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            tt_pcba = ds.Tables[0].Rows[i].ItemArray[1].ToString(); //单板条码
                            tt_hostlable = ds.Tables[0].Rows[i].ItemArray[2].ToString(); //主机条码
                            tt_longstr = ds.Tables[0].Rows[i].ItemArray[3].ToString(); //32位串码

                            tt_textfile = this.textBox3.Text + @"\" + tt_longstr + ".txt"; //文件名1
                            tt_textfile2 = this.textBox3.Text + @"\bak\" + tt_longstr + ".txt"; //文件名2


                            setRichtexBox("------------" + i.ToString() + "------------");
                            setRichtexBox(tt_pcba);
                            setRichtexBox(tt_hostlable);
                            setRichtexBox(tt_longstr);
                            setRichtexBox(tt_textfile);



                            //第一步 文件查找
                            Boolean tt_flag61 = false;

                            if (File.Exists(tt_textfile))
                            {
                                tt_flag61 = true;
                                setRichtexBox("6.1、文件存在");
                            }
                            else
                            {

                                setRichtexBox("6.1、文件不存在");
                                tt_notext++;
                            }

                            //第二步 过站
                            Boolean tt_flag62 = false;
                            if (tt_flag61)
                            {
                                tt_passinfo = Set_Next_Station(tt_pcba, this.textBox2.Text);
                                if (tt_passinfo == "1")
                                {
                                    tt_flag62 = true;
                                    tt_passs++;
                                    setRichtexBox("6.2、过站成功:" + tt_passinfo);
                                }
                                else
                                {
                                    tt_fail++;
                                    setRichtexBox("6.2、过站不成功:" + tt_passinfo);
                                }
                            }


                            //第三步 文件转移
                            Boolean tt_flag63 = false;
                            if (tt_flag61 && tt_flag62)
                            {
                                Boolean tt_moveflag = fileMove(tt_textfile, tt_textfile2);
                                if (tt_moveflag)
                                {
                                    setRichtexBox("6.3、文件转移成功:");
                                }
                                else
                                {
                                    setRichtexBox("6.3、文件转移不成功:");
                                }

                                tt_flag63 = true;

                            }



                            //第四步信息记录
                            if (tt_flag61 && tt_flag62 && tt_flag63)
                            {
                                string tt_sql = "insert into ODC_Analysis_Log (sn,cmcc_sn,analysisDate) " +
                                         " values('" + tt_pcba + "','" + tt_hostlable + "',getdate()) ";

                                int tt_num = Dataset1.ExecCommand(tt_sql, tt_conn);

                                if (tt_num > 0)
                                {
                                    setRichtexBox("6.4、数据记录成功:");
                                }
                                else
                                {
                                    setRichtexBox("6.4、数据记录不成功:");
                                }


                            }


                        }

                    }
                    #endregion



                    #region 新的过站方法
                    //在待过站数据里循环
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        tt_mac = ds.Tables[0].Rows[i].ItemArray[0].ToString(); //MAC
                        tt_pcba = ds.Tables[0].Rows[i].ItemArray[1].ToString(); //单板条码
                        tt_hostlable = ds.Tables[0].Rows[i].ItemArray[2].ToString(); //主机条码
                        tt_longstr = ds.Tables[0].Rows[i].ItemArray[3].ToString(); //32位串码

                        setRichtexBox("------------" + i.ToString() + "------------");
                        setRichtexBox(tt_mac);
                        setRichtexBox(tt_pcba);
                        setRichtexBox(tt_hostlable);
                        setRichtexBox(tt_longstr);

                        //在LOG目录里循环
                        DirectoryInfo folder = new DirectoryInfo(tt_logpatn);
                        string tt_logfilename = "";

                        Boolean tt_logfileflag = false;
                        foreach (FileInfo file in folder.GetFiles("*.*"))
                        {

                            //第一步 获取文件名
                            tt_textfile = file.FullName;
                            tt_logfilename = file.Name;
                            tt_textfile2 = this.textBox3.Text + @"\bak\" + tt_logfilename; //文件名2
                            //setRichtexBox(tt_logfilename);


                            //第二步 对文件名进判断
                            tt_logfileflag = false;
                            tt_logfileflag = getFileNameCheck(this.radioButton1.Checked,tt_logfilename, tt_pcba, tt_mac);


                            //第三步 如果是ture进行操作
                            if (tt_logfileflag)
                            {
                                //第6.1步提示找到一个文件
                                setRichtexBox("6.1 找到一个文件可以过站：" + tt_logfilename);
                                setRichtexBox("文件1：" + tt_textfile);
                                setRichtexBox("文件2：" + tt_textfile2);


                                //第6.2 进行过站
                                bool tt_flag61 = false;
                                tt_passinfo = Set_Next_Station(tt_pcba, this.textBox2.Text);
                                if (tt_passinfo == "1")
                                {
                                    tt_flag61 = true;
                                    tt_passs++;
                                    setRichtexBox("6.2、过站成功:" + tt_passinfo);
                                }
                                else
                                {
                                    tt_fail++;
                                    setRichtexBox("6.2、过站不成功:" + tt_passinfo);
                                }


                                
                                //第三步 文件转移
                                Boolean tt_flag62 = false;
                                if (tt_flag61 )
                                {
                                    Boolean tt_moveflag = fileMove(tt_textfile, tt_textfile2);
                                    if (tt_moveflag)
                                    {
                                        setRichtexBox("6.3、文件转移成功:");
                                    }
                                    else
                                    {
                                        setRichtexBox("6.3、文件转移不成功:");
                                    }

                                    tt_flag62 = true;

                                }




                                //第四步信息记录
                                if (tt_flag61 && tt_flag62 )
                                {
                                    string tt_sql = "insert into ODC_Analysis_Log (sn,cmcc_sn,analysisDate,Fmac,Ffilename,Ftaskscode,Fcode) " +
                                     " values('" + tt_pcba + "','" + tt_hostlable + "',getdate(),'" + tt_mac + "','" + tt_logfilename + "','"+tt_taskcode+"','"+tt_code+"') ";

                                    int tt_num = Dataset1.ExecCommand(tt_sql, tt_conn);

                                    if (tt_num > 0)
                                    {
                                        setRichtexBox("6.4、数据记录成功:");
                                    }
                                    else
                                    {
                                        setRichtexBox("6.4、数据记录不成功:");
                                    }


                                }






                            }
                           
                        }


                    }
                    #endregion


                }
                else
                {
                    setRichtexBox("没有可以过站的数据");
                }


                tt_flag6 = true;

                this.label18.Text = tt_passs.ToString();
                this.label19.Text = tt_fail.ToString();
                this.label21.Text = tt_notext.ToString();

            }






            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
            {
                this.richTextBox3.BackColor = Color.Chartreuse;
            }
            else
            {
                this.richTextBox3.BackColor = Color.Red;
            }

        }


        //获取站位的数量
        private DataSet GetStationInfo()
        {
            string tt_sql = "select  T1.pcba_pn MAC, T2.pcbasn 单板,T2.hostlable 主机码,T2.smtaskscode 移动码,T1.ccode 前站,T1.ncode 后站  " +
                            "from odc_routingtasklist  T1 " +
                            "left outer join odc_alllable T2 " +
                            "on T1.pcba_pn = T2.maclable " +
                            "where T1.taskscode = '" + this.textBox1.Text + "' and T1.Napplytype is null and T1.Ncode = '" + this.textBox2.Text + "' ";


            DataSet ds1 = Dataset1.GetDataSet(tt_sql, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView1.DataSource = ds1;
                dataGridView1.DataMember = "Table";
                this.label10.Text = ds1.Tables[0].Rows.Count.ToString();
            }

            return ds1;

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



        //对文件名进判断
        private bool getFileNameCheck(Boolean tt_radioflag, string tt_filename,string tt_pcba,string tt_mac)
        {
            Boolean tt_flag = false;

            if (tt_radioflag)   
            {
                //按文件名含有单号判断
                if (tt_filename.Contains(tt_pcba))
                {
                    tt_flag = true;
                }
            }
            else
            {
                //按文件名含有PASS+MAC判断
                if (tt_filename.Contains("PASS") && tt_filename.Contains(tt_mac))
                {
                    tt_flag = true;
                }

            }


            return tt_flag;
        }



        #endregion


        #region 6、时间控件
        private void timer1_Tick(object sender, EventArgs e)
        {
            tt_interval--;
            this.label13.Text = tt_interval.ToString();
            if (tt_interval <= 0)
            {


                LogStationPass();


                //System.Threading.Thread.Sleep(100);//暂停1秒

                tt_interval = Convert.ToInt32(this.comboBox1.Text);
                this.label13.Text = tt_interval.ToString();
            }
        }
        #endregion



        #region 7、辅助功能
        //richtext加记录
        private void setRichtexBox(string tt_textinfor)
        {
            this.richTextBox3.Text = this.richTextBox3.Text + tt_textinfor + "\n";
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


        #region 8、数据查询
        //确定
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

        //重置
        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox16.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;
        }
        #endregion




        #region 9、日志过站记录查询
        //查询确定
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
                            "where analysisDate  between '" + tt_date1 + "' and '" + tt_date2 + "' "+ tt_task + tt_pcba + tt_hostlable + tt_mac;

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

        //重置
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


        






    }
}
