using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace TVBOX01
{
    public partial class Form1_fh : Form
    {
        public Form1_fh()
        {
            InitializeComponent();
        }

        private string path;
        private int tt_interval;
        
        //加载
        private void Form1_fh_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=Adminsa@123";

            clearItem1();

            this.button1.Visible = false;
            this.button3.Visible = false;

            tt_interval = Convert.ToInt32(this.comboBox1.Text);

            this.label13.Text = tt_interval.ToString();


            clearCrean();
        }

        
        #region    1、属性定义
        static string tt_conn;
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

            //错误信息提示

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

        #region 3、锁定事件
        //工单锁定事件
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                string tt_sql1 = "select  tasksquantity,product_name " +
                                "from odc_tasks where taskscode = '" + this.textBox1.Text + "' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);

                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label20.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量

                    this.textBox1.Enabled = false;
                    this.textBox2.Enabled = false;

                    this.button1.Visible = true;
                    this.button3.Visible = true;

                }
                else
                {
                    MessageBox.Show("没有查询此工单，请确认！");

                }
            }
            else
            {
                this.textBox1.Enabled = true;
                this.textBox2.Enabled = true;
                this.label20.Text = null;

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



        #endregion




        #region 4、按钮事件

        //选择目录
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



        //开始或结束按钮
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

        #endregion




        #region 5、方法

        //主方法
        private void LogStationPass()
        {

            //数据初始化
            setRichtexBox("---开始新的过站---");
            DataSet ds = null;
            clearCrean();

            //this.label7.Text = Set_Next_Station("XZWJ0252618V000021", "3160");



            //第一步 查看是否填写日志目录
            Boolean tt_flag1 = false;
            if (!this.textBox3.Text.Equals(""))
            {
                setRichtexBox("第1步：已选择LOG存储目录，goon");
                tt_flag1 = true;
            }
            else
            {
                setRichtexBox("第1步：没有选择LOG存储目录,over");
            }


            //第二步 查看是否存在bak目录,不存在就创建一个目录
            Boolean tt_flag2 = false;
            if (tt_flag1 )
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
                setRichtexBox("第4步：已获取待测站位信息" );
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
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 )
            {
                int tt_passs = 0;
                int tt_fail = 0;
                int tt_notext = 0;

                string tt_pcba = "";
                string tt_hostlable = "";
                string tt_longstr = "";
                string tt_textfile = "";
                string tt_textfile2 = "";
                string tt_passinfo = "";

                setRichtexBox("第6步：开始过站");

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    #region 过站循环
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
                        if (tt_flag61 )
                        {
                            tt_passinfo = Set_Next_Station(tt_pcba,this.textBox2.Text);
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
                            Boolean tt_moveflag = fileMove(tt_textfile,tt_textfile2);
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
                            string tt_sql= "insert into ODC_Analysis_Log (sn,cmcc_sn,analysisDate) "+
                                     " values('"+tt_pcba+"','"+tt_hostlable+"',getdate()) ";

                            int tt_num = Dataset1.ExecCommand(tt_sql,tt_conn);

                            if (tt_num >0 )
                            {
                                setRichtexBox("6.4、数据记录成功:");
                            }
                            else
                            {
                                setRichtexBox("6.4、数据记录不成功:");
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
                            "where T1.taskscode = '"+this.textBox1.Text+"' and T1.Napplytype is null and T1.Ncode = '"+this.textBox2.Text+"' ";


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
        private string  GetLogText()
        {
            
            string tt_path = this.textBox3.Text;
            DirectoryInfo folder = new DirectoryInfo(tt_path);
            int tt_textnumber = 0;
            foreach (FileInfo file in folder.GetFiles("*.txt"))
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
            foreach (FileInfo file in folder.GetFiles("*.txt"))
            {
                this.richTextBox1.Text = file.FullName + "\n" + this.richTextBox1.Text;
                tt_textnumber++;
            }

            this.label9.Text = tt_textnumber.ToString();
        }





        #endregion




        #region 6、时间控件
        private void timer1_Tick(object sender, EventArgs e)
        {



            tt_interval--;
            this.label13.Text = tt_interval.ToString();
            if (tt_interval == 0)
            {


                LogStationPass();


                System.Threading.Thread.Sleep(100);//暂停1秒
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



        public static string Set_Next_Station(string tt_pcba,string tt_incode)
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
                string r = Dataset1.stringExecSPCommand(com, paramers,tt_conn);

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

        #endregion



    }
}
