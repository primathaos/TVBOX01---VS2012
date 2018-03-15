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

namespace TVBOX01
{
    public partial class Form21_als : Form
    {
        public Form21_als()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_conn;
        static string tt_path = "";
        static string tt_md5 = "";
        int tt_printtime = 0;  //打印次数

        int tt_interval = 0;

        
        private void Form21_als_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";

            //员工账号分离
            if (str.Contains("FH007"))
            {
                this.button2.Visible = false;
                this.button3.Visible = false;
            }


            //操作按钮
            this.textBox2.Visible = false;
            this.textBox3.Visible = false;
            this.button4.Visible = false;
            this.button12.Visible = false;
            this.button13.Visible = false;

            ClearLabelInfo();


            //listview设置
            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.LabelEdit = true; //是否可编辑,ListView只可编辑第一列。
            this.listView1.Scrollable = true;//有滚动条
            this.listView1.HeaderStyle = ColumnHeaderStyle.Clickable;//对表头进行设置
            this.listView1.FullRowSelect = true;//是否可以选择行


            //添加表头
            this.listView1.Columns.Add("NO", 30);
            this.listView1.Columns.Add("MAC", 150);
            this.listView1.Columns.Add("GPSN", 130);

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
            this.label29.Text = null;
            this.label30.Text = null;
            this.label31.Text = null;
            this.label32.Text = null;
            this.label33.Text = null;
            this.label34.Text = null;
            this.label49.Text = null;
            this.label59.Text = null;
            this.label61.Text = null;

            //提示信息
            this.label12.Text = null;

            //打印信息
            this.label46.Text = null;
            this.label47.Text = null;
            this.label4.Text = null;

            //条码信息
            this.label7.Text = null;
            this.label8.Text = null;
            this.label9.Text = null;

            //打印次数
            tt_printtime = 0;
            this.label36.Text = tt_printtime.ToString();

            //情况列表
            CleatListView();
        }



        //扫描前数据初始化
        private void ScanDataInitial()
        {
            //条码信息清除
            this.label7.Text = null;
            this.label8.Text = null;
            this.label9.Text = null;
           

            //提示信息
            this.label12.Text = null;


            //表格
            this.dataGridView2.DataSource = null;

            //richtext
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;


        }


        //重置数据清理
        private void ClearLabelInfo2()
        {
           //填写信息
            this.textBox2.Text = null;
            this.textBox3.Text = null;


            //提示信息
            this.label12.Text = null;
       

            //条码信息
            this.label7.Text = null;
            this.label8.Text = null;
            this.label9.Text = null;

            //打印次数
            tt_printtime = 0;
            this.label36.Text = tt_printtime.ToString();

            //情况列表
            CleatListView();

            //表格
            this.dataGridView2.DataSource = null;

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
                string tt_sql1 = "select  tasksquantity,product_name,areacode,fec,Tasktype,VENDORID " +
                                 "from odc_tasks where taskscode = '" + this.textBox1.Text + "' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);

                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label27.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label29.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString(); //产品名称
                    this.label30.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区编码
                    this.label31.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString(); //EC编码


                    this.label49.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //物料编码
                    this.label61.Text = ds1.Tables[0].Rows[0].ItemArray[5].ToString();  //COMMID

                    //第一步 EC信息检查
                    Boolean tt_flag1 = false;
                    string tt_sql2 = "select  docdesc,Fpath01,Fdata01,Fmd01  from odc_ec where zjbm = '" + this.label31.Text + "' ";

                    DataSet ds2 = Dataset1.GetDataSet(tt_sql2, tt_conn);
                    if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                    {
                        this.label34.Text = ds2.Tables[0].Rows[0].ItemArray[0].ToString(); //EC描述
                        this.label33.Text = ds2.Tables[0].Rows[0].ItemArray[1].ToString(); //模板路径
                        this.label32.Text = ds2.Tables[0].Rows[0].ItemArray[2].ToString(); //数据类型
                        this.label59.Text = ds2.Tables[0].Rows[0].ItemArray[3].ToString(); //MD5码
                        tt_path = Application.StartupPath + ds2.Tables[0].Rows[0].ItemArray[1].ToString();
                        tt_md5 = ds2.Tables[0].Rows[0].ItemArray[3].ToString();
                        tt_flag1 = true;

                    }
                    else
                    {
                        MessageBox.Show("没有找到工单表的EC表配置信息，请确认！");
                    }



                    Boolean tt_flag2 = false;
                    if (tt_flag1)
                    {
                        tt_flag2 = getPathIstrue(tt_path);
                        if (!tt_flag2)
                        {
                            MessageBox.Show(" 找不到模板文件：" + tt_path + "，请确认！");
                        }
                    }


                    Boolean tt_flag3 = false;
                    if (tt_flag2)
                    {
                        string tt_md6 = GetMD5HashFromFile(tt_path);

                        if (tt_md5 == tt_md6)
                        {
                            tt_flag3 = true;
                        }
                        else
                        {
                            MessageBox.Show("系统设定模板MD5码: '" + tt_md5 + "'与你使用模板的MD5码：'" + tt_md6 + "'不一致，请确认！");
                        }
                    }



                    //最后验证
                    if (tt_flag1 && tt_flag2 && tt_flag3 )
                    {
                        this.textBox1.Enabled = false;
                        this.textBox2.Visible = true;
                        this.textBox3.Visible = true;
                        this.button4.Visible = true;
                        this.button12.Visible = true;
                        this.button13.Visible = true;
                        GetMACPrintNumInfo();    //MAC打印信息

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
                this.button4.Visible = false;
                this.button12.Visible = false;
                this.button13.Visible = false;
                ClearLabelInfo();
                ScanDataInitial();
            }
        }

        //位数锁定
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


        //将短MAC转换长MAC
        private string getLongMac(string tt_shortmac)
        {
            string tt_longmac = "";
            string tt_mac1 = "";
            string tt_mac2 = "";
            string tt_mac3 = "";
            string tt_mac4 = "";
            string tt_mac5 = "";
            string tt_mac6 = "";

            if (tt_shortmac.Length == 12)
            {
                tt_mac1 = tt_shortmac.Substring(0,2);
                tt_mac2 = tt_shortmac.Substring(2, 2);
                tt_mac3 = tt_shortmac.Substring(4, 2);
                tt_mac4 = tt_shortmac.Substring(6, 2);
                tt_mac5 = tt_shortmac.Substring(8, 2);
                tt_mac6 = tt_shortmac.Substring(10, 2);
                tt_longmac = tt_mac1 + "-" + tt_mac2 + "-" + tt_mac3 + "-" + tt_mac4 + "-" + tt_mac5 + "-" + tt_mac6;
            }
            else
            {
                MessageBox.Show("MAC转换中，MAC位数不是12位");
            }



            return tt_longmac;
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
                    MessageBox.Show(tt_str+",转换为数字失败，请检查！");
                }
            }


            return tt_int;
        }



        #endregion


        #region 5、数据辅助功能

        //获取MAC打印信息信息
        private void GetMACPrintNumInfo()
        {
            string tt_sql = "select  count(1),count(case when state is  NULL then 1  end) Fcount1, " +
                            "count(case when state is not NULL then 1  end) Fcount2 " +
                            "from odc_macinfo  where taskscode = '" + this.textBox1.Text + "'";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label46.Text = tt_array[0];
            this.label47.Text = tt_array[1];
            this.label4.Text = tt_array[2];

        }


        //记录铭牌重打信息
        private void putMacrePrintInfo(string tt_taskscode,string tt_maclable, string tt_hostlable,string tt_user,string tt_local,string tt_remark)
        {
            string tt_sql = "insert into odc_lablereprint (Ftaskcode,Fmaclable,Fhostlable,Flocal,Fname,Fdate,Fremark) " +
                 "values('"+tt_taskscode+"','"+tt_maclable+"','"+tt_hostlable+"','"+tt_local+"','"+tt_user+"',getdate(),'"+tt_remark+"')";

            if (tt_maclable != "" && tt_hostlable != "")
            {
            int tt_execint = Dataset1.ExecCommand(tt_sql,tt_conn);
            }

        }



        #endregion



        #region 6、重置预览打印功能
        //重置
        private void button1_Click(object sender, EventArgs e)
        {
            ClearLabelInfo2();
        }

        //预览
        private void button2_Click(object sender, EventArgs e)
        {
            
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确定要预览铭牌吗，打印信息被记录", "铭牌重打", messButton);

            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {
                GetParaDataPrint(2);

                string tt_task = this.textBox1.Text;
                string tt_mac = this.label7.Text;
                string tt_gpsn = this.label9.Text;
                string tt_local = "设备铭牌";
                string tt_remark = "预览";
                putMacrePrintInfo(tt_task, tt_mac, tt_gpsn, str, tt_local, tt_remark);

            }
            else
            {

            }
        }

        //打印
        private void button3_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确定要重打铭牌吗，打印信息被记录", "铭牌重打", messButton);

            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {

                GetParaDataPrint(1);

                string tt_task = this.textBox1.Text;
                string tt_mac = this.label7.Text;
                string tt_gpsn = this.label9.Text;
                string tt_local = "设备铭牌";
                string tt_remark = "打印";
                putMacrePrintInfo(tt_task, tt_mac, tt_gpsn, str, tt_local, tt_remark);


            }
            else
            {

            }


        }
        #endregion

        

        #region 7、SN条码查询
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

        //重置
        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox11.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;
        }
        #endregion



        #region 8、获取MD5码
        //文件名
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



        #region 9、MACINFO查询
        //重置
        private void button10_Click(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = null;
            Boolean tt_flag = false;
            if ( this.textBox4.Text == "" && this.textBox5.Text == "")
            {
                MessageBox.Show("工单号和MAC不能都为空！");
            }
            else
            {
                tt_flag = true;
            }

            if( tt_flag)
            {
                //工单号
                string tt_task = "";
                if (this.textBox4.Text.Trim() != "")
                {
                    tt_task = " and taskscode = '" + this.textBox4.Text.Trim() + "' ";
                }

                //MAC
                string tt_mac = "";
                if (this.textBox5.Text.Trim() != "")
                {
                    tt_mac = " and mac = '" + this.textBox5.Text.Trim() + "' ";
                }


                string tt_sql = "select taskscode 工单号,MAC, barcode 设备标示符, SN GPSN,state 打印次数, Fusestate 是否使用, Fnameplate 打印时间 " +
                                "from odc_macinfo " +
                                "where 1=1 " + tt_task + tt_mac;


                DataSet ds = Dataset1.GetDataSet(tt_sql, tt_conn);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dataGridView1.DataSource = ds;
                    dataGridView1.DataMember = "Table";
                }
                else
                {
                    MessageBox.Show("sorry,没有查询到数据");
                }


            }


        }

        //重置
        private void button11_Click(object sender, EventArgs e)
        {
            this.textBox4.Text = null;
            this.textBox5.Text = null;
            this.dataGridView1.DataSource = null;
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);  
        }

        #endregion



        #region 10、连续打印及重打

        //开始
        private void button4_Click(object sender, EventArgs e)
        {
            Boolean tt_flag1 = false; 
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("是否要连打"+this.textBox2.Text+"个标签，请确认", "条码连打", messButton);
            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {
                tt_flag1 = true; 
            }
            else
            {

            }


            tt_interval = getTransmitStrToInt(this.comboBox1.Text);
            timer1.Interval = tt_interval; 

            Boolean tt_printflag = false;
            int tt_allowprintnum =  getTransmitStrToInt(this.label47.Text);
            int tt_applyprintnum = getTransmitStrToInt(this.textBox2.Text);

            if (tt_flag1)
            {
                //对申请数量进行判断
                if (tt_applyprintnum <= tt_allowprintnum && tt_applyprintnum <= 100)
                {
                    tt_printflag = true;
                }
                else
                {
                    MessageBox.Show("申请打印数量：" + tt_applyprintnum.ToString() + "，不能大于未出铭牌数量：" + tt_allowprintnum + ",且一次打印数量不能操作100");
                }
            }


            //开始打印
            if (tt_printflag)
            {
                        this.textBox2.Enabled = false;
                        this.comboBox1.Enabled = false;

                        if (timer1.Enabled == true)
                        {

                        }
                        else
                        {
                                try
                                {
                                    timer1.Start();
                                }
                                catch (Exception)
                                {

                                }
                        }

            }

        }


        //暂停
        private void button12_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Stop();

            }
        }

        //停止
        private void button13_Click(object sender, EventArgs e)
        {
            this.textBox2.Enabled = true;
            this.comboBox1.Enabled = true;
            this.textBox2.Text = null;
            if (timer1.Enabled == true)
            {
                timer1.Stop();

            }

            tt_printtime = 0;
            this.label36.Text = tt_printtime.ToString();

            GetMACPrintNumInfo();

        }




        
        //扫描MAC重打
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描--------");
                string tt_task = this.textBox1.Text.Trim();
                string tt_scanmac = this.textBox3.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace("-", "");

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



                //第四步 MAC位数检查
                Boolean tt_flag4 = false;
                string tt_longmac = "";
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    if (tt_shortmac.Length == 12)
                    {
                        tt_flag4 = true;
                        tt_longmac = getLongMac(tt_shortmac);
                        setRichtexBox("4、短MAC位数为12,：" + tt_shortmac + ",获取长MAC："+tt_longmac+",goon");
                    }
                    else
                    {
                        setRichtexBox("4、MAC位数不为12,：" + tt_shortmac + ",over");
                        PutLableInfor("MAC位数不为12，请确认是否为MAC！");
                    }


                }



                //第五步 alllable数据检查
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    tt_flag5 = true;
                    setRichtexBox("5、alllable数据检查过,goon");
                }


                //第六步 maninfo表检查
                Boolean tt_flag6 = false;
                string tt_gpsn = "";
                string tt_state = "";
                string tt_task1 = this.textBox1.Text;
                string tt_task2 = "";
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                {
                    string tt_sql6 =  "select  taskscode, state ,SN "+
                                      "from odc_macinfo "+
                                      "where mac = '"+tt_longmac+"' ";

                    DataSet ds = Dataset1.GetDataSet(tt_sql6, tt_conn);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        tt_task2 = ds.Tables[0].Rows[0].ItemArray[0].ToString().Trim();
                        tt_state = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                        tt_gpsn = ds.Tables[0].Rows[0].ItemArray[2].ToString();

                        if (tt_task1.Trim() == tt_task2.Trim())
                        {
                            if (tt_state.Length >0 )
                            {
                                tt_flag6 = true;
                                setRichtexBox("6、该MAC打印状态为：" + tt_state + "，可以重打过，goon");
                            }
                            else
                            {
                                setRichtexBox("6、改MAC打印状态为：" + tt_state + "，还没有打印过，over");
                                PutLableInfor("该MAC还是没有打印过，不能重打 ，请确认！");
                            }
                        }
                        else
                        {
                            setRichtexBox("6、工单不一致，该MAC工单为："+tt_task2+"，over");
                            PutLableInfor("工单不一致，该MAC工单为："+tt_task2+"，请检查！");
                        }


                    }
                    else
                    {
                        setRichtexBox("6、MACINFO没有查询到改MAC数据，over");
                        PutLableInfor("MAC表没有查询到数据，请检查！");
                    }



                }



                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {
                    this.label7.Text = tt_shortmac;
                    this.label8.Text = tt_longmac;
                    this.label9.Text = tt_gpsn;

                    GetParaDataPrint(0);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    setRichtexBox("7、查询完毕，可以重打标签或修改模板，over");
                    PutLableInfor("MAC查询完毕");
                    textBox3.Focus();
                    textBox3.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox3.Focus();
                    textBox3.SelectAll();
                }






            }
        }

        #endregion



        #region 11、列表操作
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
        private void PutListViewData(string tt_mac, string tt_gpsn)
        {
            int i = this.listView1.Items.Count + 1;
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { i.ToString(), tt_mac, tt_gpsn  });
            this.listView1.Items.AddRange(p);
            this.listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
        }

        #endregion



        #region 12、主方法
        //时间控件
        private void timer1_Tick(object sender, EventArgs e)
        {
            Boolean tt_flag = true;
            if (getTransmitStrToInt(this.label36.Text) >= getTransmitStrToInt(this.textBox2.Text))
            {
                tt_flag = false;
                timer1.Stop();
                MessageBox.Show("打印数量：" + this.label36.Text + "已到设定数量:" + this.textBox2.Text);
                GetMACPrintNumInfo();
            }

            if (tt_flag)
            {
                putPrintToStar();

                tt_printtime++;
                this.label36.Text = tt_printtime.ToString();
            }



            if (getTransmitStrToInt(this.label36.Text) >= getTransmitStrToInt(this.textBox2.Text))
            {
                timer1.Stop();
                MessageBox.Show("打印数量：" + this.label36.Text + "已到设定数量:" + this.textBox2.Text);
                GetMACPrintNumInfo();
            }
            


        }


        //开始打印
        private void putPrintToStar()
        {

            //ScanDataInitial();
            setRichtexBox("-----"+tt_printtime.ToString()+"--------");


            //第一步获取MAC
            Boolean tt_flag1 = false;
            string tt_taskscode = this.textBox1.Text.Trim();
            string tt_shortmac = "";
            string tt_longmac = "";
            string tt_gpan = "";

            string tt_sql = "select top 1 mac, sn from odc_macinfo " +
                             "where taskscode = '"+tt_taskscode+"' and state is NULL " +
                             "order by mac ";

            DataSet ds = Dataset1.GetDataSet(tt_sql, tt_conn);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                tt_longmac = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                tt_gpan = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                tt_shortmac = GetShortMac(tt_longmac);

                this.label7.Text = tt_shortmac;
                this.label8.Text = tt_longmac;
                this.label9.Text = tt_gpan;
                tt_flag1 = true;
                setRichtexBox("1、找到一个可打印的MAC："+tt_longmac+",GPSN："+tt_gpan+",over");
                PutLableInfor("MAC查询完毕");
            }
            else
            {
                
                setRichtexBox("1、没有可打印的MAC：over");
                PutLableInfor("MAC查询完毕");
            }


            //第二步 打印
            Boolean tt_flag2 = false;
            if (tt_flag1)
            {
                
                try
                {
                    GetParaDataPrint(1);
                    tt_flag2 = true;
                    setRichtexBox("2、完成MAC打印：goon");
                }
                catch
                {
                    setRichtexBox("2、打印失败请检查：over");
                }
                
                
            }


            //第三步修改MAC状态
            Boolean tt_flag3 = false;
            if (tt_flag2)
            {

                string tt_updata = "update odc_macinfo set state = 1, Fnameplate = CONVERT(varchar, getdate(),120) " +
                                   "where taskscode = '" + tt_taskscode + "' and mac = '" + tt_longmac + "' ";
                int tt_int = Dataset1.ExecCommand(tt_updata, tt_conn);

                if (tt_int > 0)
                {
                    tt_flag3 = true;
                    setRichtexBox("3、MAC状态值修改完毕：goon");
                }
                else
                {
                    setRichtexBox("3、MAC:" + tt_longmac + ",状态值修改不成功：-----over");
                    PutLableInfor("MAC:" + tt_longmac+", 状态值修改不成功");
                }
            }


            //第四步显示MAC值
            if (tt_flag3)
            {
                PutListViewData(tt_longmac, tt_gpan);
                this.richTextBox1.BackColor = Color.Chartreuse;
            }
            else
            {
                timer1.Stop();
                this.richTextBox1.BackColor = Color.Red;
            }

        }


        




        #endregion



        #region 13、数据采集及模板打印
        //获取参数
        private void GetParaDataPrint(int tt_itemtype)
        {
            //数据准备
            string tt_fdata = this.label32.Text; //数据类型
            string tt_mac = this.label8.Text; //长MAC
            string tt_gpsn = this.label9.Text; //GPSN
            this.dataGridView2.DataSource = null;

            //MP01---数据类型一  设备标签
            if (tt_mac != "" && tt_gpsn != "")
            {
                if (tt_fdata == "MP01")
                {
                    GetParaDataPrint_MP01(tt_itemtype);
                }
            }
            else
            {
                MessageBox.Show("条码信息中MAC或GPSN有为空值的，请确认！");
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
            row1["内容"] = this.label29.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "COMITID";
            row2["内容"] = this.label61.Text;
            dt.Rows.Add(row2);



            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "GPSN";
            row3["内容"] = this.label9.Text;
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "物料编码";
            row4["内容"] = this.label49.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "短MAC";
            row5["内容"] = this.label7.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "长MAC";
            row6["内容"] = this.label8.Text;
            dt.Rows.Add(row6);


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


                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    report.Print();
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
