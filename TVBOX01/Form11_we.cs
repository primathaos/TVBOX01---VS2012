using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TVBOX01
{
    public partial class Form11_we : Form
    {
        public Form11_we()
        {
            InitializeComponent();
        }

        #region 1、属性设置

        static string tt_conn;
        static string tt_path = "";
        int tt_yield = 0;  //产量
        DateTime tt_productstarttime = DateTime.Now; //开始时间
        DateTime tt_productprimtime; //上一次时间

        private void Form11_we_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";
            this.toolStripStatusLabel6.Text = tt_productstarttime.ToString();

            //员工账号分离
            if ( str.Contains("SN001") )
            {
                this.button2.Visible = false;
                this.button3.Visible = false;
            }


            ClearLabelInfo();

            //生产节拍
            this.label25.Text = tt_yield.ToString();
            this.label26.Text = null;
            this.label27.Text = null;
            this.label28.Text = null;

            
            

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



        #region 2、按钮事件
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


                string tt_prientcode = this.label55.Text;
                string tt_checkcode = this.label56.Text;

                Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                if (tt_flag)
                {
                    GetParaDataPrint(2);  //预览
                }
                else
                {
                    MessageBox.Show("当前站位：" + tt_prientcode + "必须大于待测站位：" + tt_checkcode + ",才能重打标签");
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
            if (this.dataGridView2.RowCount > 0)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("确定要重打铭牌吗，打印信息被记录", "铭牌重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {

                    string tt_prientcode = this.label55.Text;
                    string tt_checkcode = this.label56.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                    if (tt_flag)
                    {
                        GetParaDataPrint(1);  //打印
                    }
                    else
                    {
                        MessageBox.Show("当前站位：" + tt_prientcode + "必须大于待测站位：" + tt_checkcode + ",才能重打标签");
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

            textBox7.Focus();
            textBox7.SelectAll();

        }

        #endregion


        #region 3、锁定事件

        //子工单锁定事件
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {

                string tt_sql1 = "select  tasksquantity,product_name,areacode,fec,convert(varchar, taskdate, 102) fdate,customer,flhratio,Gyid,Tasktype " +
                                "from odc_tasks where taskscode = '" + this.textBox1.Text + "' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);

                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {

                    this.label12.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label13.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString(); //产品名称
                    this.label14.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区
                    this.label17.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString(); //EC编码
                    this.label16.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //生产日期

                    this.label54.Text = ds1.Tables[0].Rows[0].ItemArray[7].ToString();  //流程配置
                    this.label15.Text = ds1.Tables[0].Rows[0].ItemArray[8].ToString();  //物料编码



                    //第一步、流程检查
                    Boolean tt_flag1 = false;
                    if (!this.label54.Text.Equals(""))
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

                    //第二步、查找模板路径
                    Boolean tt_flag2 = false;
                    if( tt_flag1)
                    {
                        string tt_eccode = this.label17.Text;
                        string tt_sql2 = "select  docdesc,Fpath04,Fdata04,Macxp  from odc_ec where zjbm = '" + tt_eccode + "' ";
                    
                        DataSet ds2 = Dataset1.GetDataSet(tt_sql2, tt_conn);
                        if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                        {
                            this.label20.Text = ds2.Tables[0].Rows[0].ItemArray[0].ToString(); //EC描述
                            this.label18.Text = ds2.Tables[0].Rows[0].ItemArray[2].ToString(); //数据类型
                            tt_path = Application.StartupPath + ds2.Tables[0].Rows[0].ItemArray[1].ToString(); //模板路径
                            this.label19.Text = tt_path;
                            tt_flag2 = true;

                        }
                        else
                        {
                            MessageBox.Show("没有找到工单表的EC表配置信息，请确认！");
                        }
                    }


                    //第三步、总工单与子工单包容性检查
                    Boolean tt_flag3 = false;
                    string tt_sontask = this.textBox1.Text;
                    string tt_fathertask = this.textBox9.Text;
                    if( tt_flag1 && tt_flag2)
                    {
                        if (tt_sontask.Contains(tt_fathertask))
                        {
                            tt_flag3 = true;
                        }
                        else
                        {
                            MessageBox.Show("总工单："+tt_fathertask+",与子工单："+tt_sontask+",不一致,请检查！");
                        }
                    }



                    //第四步、查找总工单是否存在
                    Boolean tt_flag4 = false;
                    if (tt_flag1 && tt_flag2 && tt_flag3 )
                    {
                        string tt_sql4 = "select count(1),0,0 from odc_tasks "+
                                         "where taskscode = '"+tt_fathertask+"' ";

                        string[] tt_array4 = new string[3];
                        tt_array4 = Dataset1.GetDatasetArray(tt_sql4, tt_conn);
                        if (tt_array4[0] == "1")
                        {
                            tt_flag4 = true;
                        }
                        else
                        {
                            MessageBox.Show("工单表中没有找到该总工单："+tt_fathertask+"，请确认！");
                        }

                    }



                    //第五步、查找流水号配置信息
                    Boolean tt_flag5 = false;
                    if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                    {
                        string tt_sql5 = "select count(1),min(hostqzwh),0 from ODC_HOSTLABLEOPTIOAN " +
                                     "where taskscode = '" + this.textBox1.Text + "' ";
                        string[] tt_array5 = new string[3];
                        tt_array5 = Dataset1.GetDatasetArray(tt_sql5, tt_conn);
                        if (tt_array5[0] == "1")
                        {
                            tt_flag5 = true;
                            this.label65.Text = tt_array5[1];
                        }
                        else
                        {
                            MessageBox.Show("没有找到工单的串号表配置信息，请确认！");
                        }
                    }




                    //第六步 最后判断
                    if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                    {
                        this.textBox1.Enabled = false;
                        this.textBox9.Enabled = false;

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
                this.textBox9.Enabled = true;

                this.textBox2.Visible = false;
                this.textBox7.Visible = false;
                ClearLabelInfo();
                ScanDataInitial();
            }
        }

        //总工单锁定事件
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        //MAC过站位数锁定
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
                this.textBox4.Enabled = true;
            }
        }

        //MAC重打位数锁定
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked)
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

        
        #endregion


        #region 4、数据查询

        //重置
        private void button4_Click(object sender, EventArgs e)
        {
            this.textBox8.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;
        }

        //确定
        private void button5_Click(object sender, EventArgs e)
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

        //订单查询重置
        private void button7_Click(object sender, EventArgs e)
        {
            this.textBox10.Text = null;
            this.dataGridView6.DataSource = null;
        }

        //订单查询确定
        private void button6_Click(object sender, EventArgs e)
        {
            this.dataGridView6.DataSource = null;

            string tt_task = this.textBox10.Text.Trim();
            

            string tt_sql1 = "select hprintman 总工单,taskscode 子工单, pcbasn 单板号,hostlable 主机条码,maclable MAC, " +
                             "boxlable 生产序列号,Bosasn BOSA, shelllable GPSN, Smtaskscode 串号, Dystlable 电源号, " +
                             "sprinttime 关联时间 " +

                            "from odc_alllable " +
                            "where taskscode = '" + tt_task + "'   order by  hostlable ";

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


        //显示行号
        private void dataGridView6_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SolidBrush brushOne = new SolidBrush(Color.Red);
            e.Graphics.DrawString(Convert.ToString(e.RowIndex + 1, System.Globalization.CultureInfo.CurrentUICulture), e.InheritedRowStyle.Font, brushOne, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
        }

        #endregion


        #region 5、清除事件
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


            //流程信息
            this.label54.Text = null;
            this.label55.Text = null;
            this.label56.Text = null;
            this.label57.Text = null;


            //提示信息
            this.label35.Text = null;


            //生产信息
            this.label58.Text = null;
            this.label59.Text = null;


            //条码信息
            this.label42.Text = null;
            this.label43.Text = null;
            this.label44.Text = null;
            this.label45.Text = null;
            this.label46.Text = null;
            this.label47.Text = null;

            this.label67.Text = null;


            //扫描框
            this.textBox2.Visible = false;
            this.textBox7.Visible = false;

        }


        //扫描前数据初始化
        private void ScanDataInitial()
        {
            //条码信息清除
            this.label42.Text = null;
            this.label43.Text = null;
            this.label44.Text = null;
            this.label45.Text = null;
            this.label46.Text = null;
            this.label47.Text = null;
            this.label67.Text = null;

            //提示信息
            this.label35.Text = null;

            //当前站位
            this.label55.Text = null;


            //表格
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

            //richtext
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;


        }



        #endregion


        #region 6、辅助功能
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
                setRichtexBox("1、位数判断不正确，不是" + tt_snlength.ToString() + "位");
                PutLableInfor("位数判断不正确，不是" + tt_snlength.ToString() + "位,请确认！");
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
                if (tt_pricode > tt_passcde )
                {
                    tt_flag = true;
                }
            }

            return tt_flag;
        }


        //下拉列表绑定数据
        private void GetCommboxDrapList(string tt_task)
        {
        }


        #endregion


        #region 7、扫描事件

        //MAC查询重打
        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描查询--------");
                string tt_task = this.textBox1.Text.Trim();
                string tt_scanmac = this.textBox7.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace(":", "");

                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox6.Text);

                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox5.Text.Trim());
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
                        this.label42.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString();  //单板号
                        this.label43.Text = ds3.Tables[0].Rows[0].ItemArray[1].ToString();  //主机条码
                        this.label45.Text = ds3.Tables[0].Rows[0].ItemArray[2].ToString();  //短MAC
                        this.label44.Text = ds3.Tables[0].Rows[0].ItemArray[3].ToString();  //移动串号
                        this.label46.Text = ds3.Tables[0].Rows[0].ItemArray[4].ToString();  //长MAC
                        this.label47.Text = ds3.Tables[0].Rows[0].ItemArray[5].ToString();  //GPSN
                        tt_longmac = this.label39.Text;
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
                    setRichtexBox("6、查找站位信息,goon");


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
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox7.Focus();
                    textBox7.SelectAll();
                }




            }
        }

        //MAC扫描过站
        private void tabControl2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC过站扫描--------");
                string tt_smalltask = this.textBox1.Text.Trim();
                string tt_bigtask = this.textBox9.Text.Trim();
                string tt_scanmac = this.textBox2.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace(":", "");


                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanmac, this.textBox3.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanmac, this.textBox4.Text.Trim());
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


                //第四步第一次数量检查（同时生产的话会出现问题）
                Boolean tt_flag4 = false;
                int tt_tasknumber = int.Parse(this.label12.Text);
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    int tt_productnum = int.Parse(this.label59.Text);
                    if (tt_productnum < tt_tasknumber)
                    {
                        tt_flag4 = true;
                        setRichtexBox("4、第一次数量检查，已获取序列号生产数量：" + tt_productnum.ToString() + "，小于计划数量：" + tt_tasknumber.ToString() + ",还可以再生产gong");
                    }
                    else
                    {
                        setRichtexBox("4、第一次数量检查，已获取序列号生产数量：" + tt_productnum.ToString() + "，大于等于计划数量：" + tt_tasknumber.ToString() + ",不能再生产gong");
                        PutLableInfor("生产数量已满不能再生产了！");
                    }
                }



                //第五步第二次数量检查
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    string tt_sql5 = "select count(1),0,0 from odc_alllable " +
                                     "where taskscode = '"+tt_smalltask+"' ";

                    string[] tt_array5 = new string[3];
                    tt_array5 = Dataset1.GetDatasetArray(tt_sql5, tt_conn);

                    int tt_productnum1 = int.Parse(tt_array5[0]);
                    if (tt_productnum1 < tt_tasknumber)
                    {
                        tt_flag5 = true;
                        setRichtexBox("5、第二次数量检查，已获取序列号生产数量：" + tt_productnum1.ToString() + "，小于计划数量：" + tt_tasknumber.ToString() + ",还可以再生产gong");
                    }
                    else
                    {
                        setRichtexBox("5、第二次数量检查，已获取序列号生产数量：" + tt_productnum1.ToString() + "，大于等于计划数量：" + tt_tasknumber.ToString() + ",不能再生产gong");
                        PutLableInfor("生产数量已满不能再生产了！");
                    }

                }



                //第六步流程检查
                Boolean tt_flag6 = false;
                string tt_gyid = this.label54.Text;
                string tt_ccode = this.label56.Text;
                string tt_ncode = this.label57.Text;
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
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {
                    string tt_sql7 = "select hostlable,maclable,smtaskscode,bprintuser,id,ageing,shelllable,pcbasn from odc_alllable " +
                                     "where hprintman = '" + tt_bigtask + "' and taskscode = '"+tt_smalltask+"'    and maclable = '" + tt_shortmac + "' ";

                    DataSet ds7 = Dataset1.GetDataSet(tt_sql7, tt_conn);
                    if (ds7.Tables.Count > 0 && ds7.Tables[0].Rows.Count > 0)
                    {
                        tt_flag7 = true;
                        tt_hostlable = ds7.Tables[0].Rows[0].ItemArray[0].ToString();  //主机条码
                        tt_shortmac = ds7.Tables[0].Rows[0].ItemArray[1].ToString();    //短MAC
                        tt_smtaskscode = ds7.Tables[0].Rows[0].ItemArray[2].ToString();  //移动串号
                        tt_longmac = ds7.Tables[0].Rows[0].ItemArray[3].ToString();     //长MAC
                        tt_id = ds7.Tables[0].Rows[0].ItemArray[4].ToString();      //行ID
                        tt_oldtype = ds7.Tables[0].Rows[0].ItemArray[5].ToString();   //老化状态
                        tt_gpsn = ds7.Tables[0].Rows[0].ItemArray[6].ToString();   //GPSN
                        tt_pcba = ds7.Tables[0].Rows[0].ItemArray[7].ToString();   //单板号
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
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8)
                {
                        tt_flag9 = true;
                        setRichtexBox("9、查找Macinfo表信息过 goon");
                }



                //第十步物料追溯添加
                Boolean tt_flag10 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9)
                {
                    tt_flag10 = true;
                    setRichtexBox("10、物料追溯记录过，gong");
                }



                //第十一步是否获取主机条码判断
                Boolean tt_flag11 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10)
                {
                    if( tt_hostlable == tt_shortmac)
                    {

                        tt_flag11 = true;
                        setRichtexBox("10、该条码主机条码："+tt_hostlable+",数据与MAC:"+tt_shortmac+"一致，还没有获取主机条码，gong");
                    }
                    else
                    {
                        setRichtexBox("10、该条码主机条码："+tt_hostlable+",数据与MAC:"+tt_shortmac+"不一致，可能已获取主机条码，ober");
                        setRichtexBox("10、可能已获取主机条码:"+tt_hostlable+"，请确认");
                    }
                }



                //第十二步开始过站

                Boolean tt_flag12 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag10 && tt_flag11)
                {
                    string tt_username = STR;
                    tt_flag12 = Dataset1.FhwifSnInStation(tt_smalltask,tt_bigtask, tt_username, 
                                                          tt_hostlable,tt_shortmac, 
                                                          tt_gyid, tt_ccode, tt_ncode,
                                                          tt_conn);
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




                //第十三站：查询身程序序列号
                Boolean tt_flag13 = false;
                string tt_boxlable = "";
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag10 && tt_flag11 && tt_flag12)
                {
                    string tt_sql13 = "select count(1), min(boxlable),0 from odc_alllable " +
                                "where taskscode = '"+tt_smalltask+"' and hprintman = '"+tt_bigtask+"' and maclable = '"+tt_shortmac+"' ";



                    string[] tt_array13 = new string[3];
                    tt_array13 = Dataset1.GetDatasetArray(tt_sql13, tt_conn);
                    if (tt_array13[0] == "1")
                    {
                         tt_flag13 = true;
                         tt_boxlable = tt_array13[1];
                         this.label67.Text = tt_boxlable;
                         setRichtexBox("13、生产序列号获取成功，已获取序列号：" + tt_boxlable + ", goon");
                      

                    }
                    else
                    {
                        setRichtexBox("13、生产序列号获取不成功，序列号：" + tt_boxlable + ", over");
                        PutLableInfor("生产序列号获取不成功，请检查！");
                    }


                }




                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag10 && tt_flag11 && tt_flag12 && tt_flag13)
                {
                    //条码信息
                    this.label42.Text = tt_pcba;    //单板号
                    this.label43.Text = tt_boxlable;   //主机条码
                    this.label44.Text = tt_smtaskscode;  //移动串号
                    this.label45.Text = tt_shortmac;    //短MAC
                    this.label46.Text = tt_longmac;      //长MAC
                    this.label47.Text = tt_gpsn;         //GPSN
                    


                    //生产节拍
                    getProductRhythm();

                    //打印
                    GetParaDataPrint(1);
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

        #endregion


        #region 8、信息获取
        //获取生产信息
        private void GetProductNumInfo()
        {
            string tt_sql = "select  count(1),count(case when hprinttime is not null then 1 end),0 " +
                            "from odc_alllable  where taskscode = '" + this.textBox1.Text + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label58.Text = tt_array[0];
            this.label59.Text = tt_array[1];
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

                this.label55.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //当前站位
            }

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
            this.label25.Text = tt_yield.ToString();   //本班产量
            this.label26.Text = tt_time;               //生产时间
            this.label27.Text = tt_avgtime.ToString();  //平均节拍
            this.label28.Text = tt_differtime2;        //实时节拍

        }


        #endregion



        #region 9、其他功能

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
                this.label56.Text = tt_ccode;
                this.label57.Text = tt_ncode;
            }




            return tt_flag;
        }

        #endregion


        



        #region 10、铭牌打印

        //获取参数
        private void GetParaDataPrint(int tt_itemtype)
        {
            string tt_fdata = this.label18.Text;

            //MP01---数据类型一数据模板
            if (tt_fdata == "MP01")
            {
                GetParaDataPrint_MP01(tt_itemtype);
            }


            //mp01---数据类型一
            if (tt_fdata == "SN01")
            {
                GetParaDataPrint_SN01(tt_itemtype);
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
            row1["内容"] = this.label13.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "主机条码";
            row2["内容"] = this.label43.Text;
            dt.Rows.Add(row2);



            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "短MAC";
            row3["内容"] = this.label45.Text;
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "移动号码";
            row4["内容"] = this.label44.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "长MAC";
            row5["内容"] = this.label46.Text;
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


        //----以下是MP01数据采集----
        private void GetParaDataPrint_SN01(int tt_itemtype)
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
            row1["名称"] = "物料编码";
            row1["内容"] = this.label15.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "序列号";
            row2["内容"] = this.label43.Text;
            dt.Rows.Add(row2);



           

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

       


        

        //------------end------------

    }
}
