using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVBOX01
{
    public partial class Form4_ch : Form
    {
        public Form4_ch()
        {
            InitializeComponent();

            this.label64.Visible = false;
            this.label65.Visible = false;
            this.label66.Visible = false;

            this.textBox13.Visible = false;
            this.textBox14.Visible = false;
            this.textBox15.Visible = false;

        }

        #region  1、属性设置

        //加载
        private void Form4_ch_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            this.toolStripStatusLabel6.Text = tt_productstarttime.ToString();
            this.toolStripStatusLabel9.Text = tt_reprinttime.ToString();

            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";

            if (str.Contains("CH001"))
            {
                this.button2.Visible = false;
                this.button3.Visible = false;
            }



            ClearLabelInfo1();
            //生产节拍
            this.label24.Text = tt_yield.ToString();
            this.label25.Text = null;
            this.label26.Text = null;
            this.label27.Text = null;

            //扫描框
            this.textBox2.Visible = false;
            this.textBox3.Visible = false;
            this.textBox4.Visible = false;




        }

     
        static string tt_conn;
        static int tt_yield = 0; //产量
        int tt_reprinttime = 0; //重打次数
        DateTime tt_productstarttime = DateTime.Now; //开始时间
        DateTime tt_productprimtime; //上一次时间


        static string tt_path = "";
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
            this.label19.Text = null;
            this.label51.Text = null;

            //流程信息
            this.label71.Text = null;
            this.label72.Text = null;
            this.label73.Text = null;
            this.label74.Text = null;


            //错误显示
            this.label37.Text = null;

            //Datagridview
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

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
                string tt_sql1 = "select  tasksquantity,product_name,softwareversion,fec, convert(varchar, taskdate, 111) fdate, gyid, Tasktype " +
                                 "from odc_tasks where taskscode = '" + this.textBox1.Text + "' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {


                    this.label12.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label13.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //产品名称
                    this.label14.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //软件版本
                    this.label16.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString();  //EC编码
                    this.label15.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //生产日期
                    this.label71.Text = ds1.Tables[0].Rows[0].ItemArray[5].ToString();  //流程信息
                    this.label51.Text = ds1.Tables[0].Rows[0].ItemArray[6].ToString();  //物料编码


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






                    //第二步、查找模板路径
                    Boolean tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_sql2 = "select  docdesc,Fpath05,Fdata05,Macxp  from odc_ec where zjbm = '" + this.label16.Text + "' ";

                        DataSet ds2 = Dataset1.GetDataSet(tt_sql2, tt_conn);
                        if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                        {
                            tt_flag2 = true;
                            this.label19.Text = ds2.Tables[0].Rows[0].ItemArray[0].ToString();
                            this.label18.Text = ds2.Tables[0].Rows[0].ItemArray[1].ToString();
                            this.label17.Text = ds2.Tables[0].Rows[0].ItemArray[2].ToString();
                            tt_path = Application.StartupPath + this.label18.Text;

                        }
                        else
                        {
                            MessageBox.Show("没有找到工表的EC表配置信息，请确认！");
                        }

                    }



                   if (tt_flag1 && tt_flag2)
                   {
                       this.textBox1.Enabled = false;
                       this.textBox2.Visible = true;
                       this.textBox3.Visible = true;
                       this.textBox4.Visible = true;
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
                this.textBox4.Visible = false;
                ClearLabelInfo1();
                ScanDataInitial();
            }
        }


        //MAC过站位数锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if( this.checkBox2.Checked)
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

        //电源位数锁定
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked)
            {
                this.textBox6.Enabled = false;
                this.textBox8.Enabled = false;
            }
            else
            {
                this.textBox6.Enabled = true;
                this.textBox8.Enabled = true;
            }
        }

        //MAC查询位数锁定
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
                this.textBox14.ReadOnly = true;
                this.textBox15.ReadOnly = true;

            }
            else
            {
                this.textBox11.ReadOnly = false;
                this.textBox12.ReadOnly = false;
                this.textBox13.ReadOnly = false;
                this.textBox14.ReadOnly = false;
                this.textBox15.ReadOnly = false;
            }
        }



        #endregion

        #region 4、按钮事件
        //重置
        private void button1_Click(object sender, EventArgs e)
        {
            ClearLabelInfo2();
        }
        
        //预览
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {
                string tt_prientcode = this.label72.Text;
                string tt_checkcode = this.label73.Text;

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


                    string tt_prientcode = this.label72.Text;
                    string tt_checkcode = this.label73.Text;

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
        }
        #endregion


        #region 5、条码扫描
        //彩盒重打MAC扫描
        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描--------");
                string tt_scanmac = this.textBox4.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace(":", "");


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

                


                //第三步查找信息
                Boolean tt_flag4 = false;
                if (tt_flag1 && tt_flag2 )
                {
                    string tt_sql4 = "select pcbasn,hostlable,maclable,smtaskscode,boxlable,dystlable,bprintuser,shelllable from odc_alllable " +
                                     "where taskscode = '"+this.textBox1.Text+"' and maclable = '"+tt_shortmac+"' ";


                    DataSet ds4 = Dataset1.GetDataSet(tt_sql4, tt_conn);
                    if (ds4.Tables.Count > 0 && ds4.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        this.label44.Text = ds4.Tables[0].Rows[0].ItemArray[0].ToString();  //单板号
                        this.label45.Text = ds4.Tables[0].Rows[0].ItemArray[1].ToString();  //主机条码
                        this.label46.Text = ds4.Tables[0].Rows[0].ItemArray[2].ToString();  //短MAC
                        this.label47.Text = ds4.Tables[0].Rows[0].ItemArray[3].ToString();  //32位移动条码
                        this.label48.Text = ds4.Tables[0].Rows[0].ItemArray[4].ToString();  //彩盒条码
                        this.label49.Text = ds4.Tables[0].Rows[0].ItemArray[5].ToString();  //电源条码
                        this.label59.Text = ds4.Tables[0].Rows[0].ItemArray[6].ToString();  //长MAC
                        this.label61.Text = ds4.Tables[0].Rows[0].ItemArray[7].ToString();  //GPSN

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

                    GetParaDataPrint(0);

                    GetProductYield();
                    CheckStation(tt_shortmac);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    textBox4.Focus();
                    textBox4.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox4.Focus();
                    textBox4.SelectAll();
                }

            }
        }

        //彩盒过站站MAC扫描
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描--------");
                string tt_scanmac = this.textBox2.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace(":", "");
                string tt_task = this.textBox1.Text.Trim();


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
                    if (this.textBox11.Text == "" || this.textBox11.Text == "")
                    {
                        setRichtexBox("4、物料填写有空值,over");
                        PutLableInfor("物料填写有空值，请检查！");
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




                //第三步查找关联表数据
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
                        tt_hostlable = ds7.Tables[0].Rows[0].ItemArray[0].ToString(); //主机条码
                        tt_pcba = ds7.Tables[0].Rows[0].ItemArray[1].ToString();      //单板号
                        tt_smtaskscode = ds7.Tables[0].Rows[0].ItemArray[2].ToString();  //移动串号
                        tt_longmac = ds7.Tables[0].Rows[0].ItemArray[3].ToString();       //长MAC
                        tt_gpsn = ds7.Tables[0].Rows[0].ItemArray[4].ToString();       //GPSN
                        setRichtexBox("7、关联表查询到一条数据，hostlable=" + tt_hostlable + ",pcba=" + tt_pcba + ",smtaskscode=" 
                                            + tt_smtaskscode + ",mac="+tt_longmac+",Gpsn="+tt_gpsn+",goon");

                    }
                    else
                    {
                        setRichtexBox("7、关联表没有查询到数据，over");
                        PutLableInfor("关联表没有查询到数据，请检查！");
                    }

                }


                //第8步串码是否存在检查
                Boolean tt_flag8 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 )
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



                





                //第十一步开始过站

                Boolean tt_flag12 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10 && tt_flag11)
                {


                    tt_flag12 = Dataset1.FhCHPassStation(tt_task, STR, tt_shortmac, tt_gyid, tt_ccode, tt_ncode, tt_conn);

                    if (tt_flag12)
                    {
                        setRichtexBox("12、彩盒过站成功，请继续扫描");
                    }
                    else
                    {
                        setRichtexBox("12、彩盒不成功，事务已回滚");
                        PutLableInfor("彩盒过站不成功，请检查或再次扫描！");
                    }


                }







                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag10 && tt_flag11 && tt_flag12)
                {

                    
                    CheckStation(tt_shortmac);
                    this.richTextBox1.BackColor = Color.Aquamarine;
                    PutLableInfor("OK 彩盒过站成功，请扫描电源！");
                    textBox3.Focus();
                    textBox3.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox2.Focus();
                    textBox2.SelectAll();
                }


            }
        }

        //扫描电源
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始电源扫描
                setRichtexBox("-----开始电源扫描--------");
                string tt_scanshell = this.textBox3.Text.Trim();
                string tt_task = this.textBox1.Text.Trim();
                string tt_scanmac = this.textBox2.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace(":", "");




                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanshell, this.textBox6.Text);

                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanshell, this.textBox8.Text.Trim());
                }





                //第三步判断电源是否用过
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    string tt_sql3 = "select count(1),0,0 from odc_alllable where taskscode = '" + tt_task + "' and dystlable = '"+tt_scanshell+"'";
                    string[] tt_array3 = new string[3];
                    tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                    if (tt_array3[0] == "0")
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、该电源线没有使用,goon");
                    }
                    else
                    {
                        setRichtexBox("3、该电源线已关联过，over");
                        PutLableInfor("该电源线已使用，请换电源");
                    }

                }





                //第四步记录数检查
                Boolean tt_flag4 = false;
                string tt_id = "";
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {
                    string tt_sql4 = "select count(1),min(boxlable),min(id) from odc_alllable " +
                                     "where taskscode = '"+tt_task+"' and maclable = '"+tt_shortmac+"' and boxlable is not null";
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
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    Boolean tt_idinfo = GetMaterialIdinfor(tt_id);
                    if (tt_idinfo)
                    {
                        string tt_insert = "insert into odc_traceback(fid,fchdate,Fsegment11,Fsegment12) " +
                        "values(" + tt_id + ",getdate(),'" + tt_mate1 + "','" + tt_mate2 + "' )";

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
                        string tt_update = "update odc_traceback set Fsegment11='" + tt_mate1 + "',Fsegment12='" + tt_mate2 + "', Fchdate = getdate() " +
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
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 )
                {
                    string tt_update6 = "update odc_alllable set dystlable = '" + tt_scanshell + "' " +
                                        "where taskscode = '" + tt_task + "' and maclable = '"+tt_shortmac+"'";

                    int tt_execute6 = Dataset1.ExecCommand(tt_update6,tt_conn);
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
                                     "where taskscode = '" + tt_task + "' and maclable = '"+tt_shortmac+"' ";

                    DataSet ds7 = Dataset1.GetDataSet(tt_sql7, tt_conn);
                    if (ds7.Tables.Count > 0 && ds7.Tables[0].Rows.Count > 0)
                    {
                        tt_flag7 = true;
                        this.label44.Text = ds7.Tables[0].Rows[0].ItemArray[0].ToString();   //单板号
                        this.label45.Text = ds7.Tables[0].Rows[0].ItemArray[1].ToString();   //主机条码
                        this.label46.Text = ds7.Tables[0].Rows[0].ItemArray[2].ToString();   //MAC
                        this.label47.Text = ds7.Tables[0].Rows[0].ItemArray[3].ToString();   //32位移动条码
                        this.label48.Text = ds7.Tables[0].Rows[0].ItemArray[4].ToString();   //流水21条码
                        this.label49.Text = ds7.Tables[0].Rows[0].ItemArray[5].ToString();   //电源条码
                        this.label59.Text = ds7.Tables[0].Rows[0].ItemArray[6].ToString();   //长MAC
                        this.label61.Text = ds7.Tables[0].Rows[0].ItemArray[7].ToString();   //GPSN

                        setRichtexBox("7、查询到关联表的数据，已关联到电源的,goon");

                    }
                    else
                    {
                        setRichtexBox("7、关联表没有查询到数据，over");
                        PutLableInfor("关联表没有查询到数据，请检查！");
                    }

                }




                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7)
                {
                    GetParaDataPrint(1);
                    GetProductYield();
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    getProductRhythm();
                    PutLableInfor("OK 电源关联成功，请扫描MAC！");
                    textBox2.Focus();
                    textBox2.SelectAll();
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


        #region 6、辅助方法
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
            }

        }


        //生产数量
        private void GetProductYield()
        {
            string tt_sql = "select count(1), sum(case when productman is not null then 1 end ) as Fcount1,max(boxlable) " +
                            "from odc_alllable where taskscode = '"+this.textBox1.Text+"' ";

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
                this.label73.Text = tt_ccode;
                this.label74.Text = tt_ncode;
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



        #endregion



        #region 7、铭牌打印

        //获取参数
        private void GetParaDataPrint(int tt_itemtype)
        {
            string tt_fdata = this.label17.Text;

            //CH01---数据类型一 烽火widfi彩盒
            if (tt_fdata == "CH01")
            {
                GetParaDataPrint_CH01(tt_itemtype);
            }

            



        }



        //----以下是QD01数据采集----青岛数据
        private void GetParaDataPrint_QD01(int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();
            string tt_twodimcode = "[)>061P" + this.label16.Text + "S" + this.label61.Text+"18VLEHWT";



            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");


            DataRow row1 = dt.NewRow();
            row1["参数"] = "N01";
            row1["名称"] = "对外型号";
            row1["内容"] = this.label14.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "串码";
            row2["内容"] = this.label61.Text;
            dt.Rows.Add(row2);



            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "MAC1";
            row3["内容"] = this.label46.Text;
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "移动号码";
            row4["内容"] = this.label47.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "华为编码";
            row5["内容"] = this.label16.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N06";
            row6["名称"] = "二维码";
            row6["内容"] = tt_twodimcode;
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N07";
            row7["名称"] = "MAC2";
            row7["内容"] = this.label59.Text;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N08";
            row8["名称"] = "生产日期";
            row8["内容"] = this.label15.Text.Substring(0,7);
            dt.Rows.Add(row8);



            //第二步加载到表格显示
            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 60;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 200;


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
                                

                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    report.Print();
                    //this.label37.Text = "打印完毕";
                    PutLableInfor("打印完毕");
                }

                //--预览
                if (tt_itemtype == 2)
                {
                    report.Design();
                    //this.label37.Text = "预览完毕";
                    PutLableInfor("预览完毕");
                }




                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");


            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                //this.label37.Text = "获取信息失败，或不是单板扫描状态，不能打印！";
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印！");
            }


        }

        //----以下是GZ01数据采集----贵州
        private void GetParaDataPrint_GZ01(int tt_itemtype)
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
            row1["名称"] = "MAC1";
            row1["内容"] = this.label46.Text;
            dt.Rows.Add(row1);
          


            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "MAC2";
            row2["内容"] = this.label59.Text;
            dt.Rows.Add(row2);


            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "主机条码";
            row3["内容"] = this.label45.Text;
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "移动号码";
            row4["内容"] = this.label47.Text;
            dt.Rows.Add(row4);


            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "生产日期";
            row5["内容"] = this.label15.Text.Substring(0,7);
            dt.Rows.Add(row5);



            //第二步加载到表格显示
            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 60;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 200;


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
                


                report.PrintSettings.ShowDialog = false;

                //--打印
                if (tt_itemtype == 1)
                {
                    report.Print();
                    //this.label37.Text = "打印完毕";
                    PutLableInfor("打印完毕");
                }

                //--预览
                if (tt_itemtype == 2)
                {
                    report.Design();
                    //this.label37.Text = "预览完毕";
                    PutLableInfor("预览完毕");
                }




                setRichtexBox("99、打印或预览完毕，请检查铭牌，OK");


            }
            else
            {
                setRichtexBox("99、获取信息失败，或不是单板扫描状态，不能打印,over");
                //this.label37.Text = "获取信息失败，或不是单板扫描状态，不能打印！";
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印！");
            }


        }


        //----以下是CH01数据采集----烽火wifi
        private void GetParaDataPrint_CH01(int tt_itemtype)
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
            row1["内容"] = this.label13.Text;
            dt.Rows.Add(row1);



            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "物料编码";
            row2["内容"] = this.label51.Text;
            dt.Rows.Add(row2);


            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "软件版本";
            row3["内容"] = this.label14.Text;
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "出厂日期";
            row4["内容"] = this.label15.Text;
            dt.Rows.Add(row4);


            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "短MAC";
            row5["内容"] = this.label46.Text;
            dt.Rows.Add(row5);


            DataRow row6 = dt.NewRow();
            row6["参数"] = "S06";
            row6["名称"] = "设备标识";
            row6["内容"] = this.label47.Text;
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "S07";
            row7["名称"] = "序列号";
            row7["内容"] = this.label45.Text;
            dt.Rows.Add(row7);


            //第二步加载到表格显示
            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 60;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 200;


            //第三步 打印或预览

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
                PutLableInfor("获取信息失败，或不是单板扫描状态，不能打印！");
            }


        }



        #endregion


        #region 8、数据查询
        //确定
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
                string tt_sql2 = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime 创建时间,fremark 备注 " +
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
            this.textBox16.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;

        }

        #endregion



    }
}
