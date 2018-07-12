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
    public partial class Form7_zx : Form
    {
        public Form7_zx()
        {
            InitializeComponent();
            SetFpathFdataIsnotVisable();
        }


        #region 1、属性设置
        static string tt_conn;
        int tt_yield = 0;
        string tt_gyid = "";
        string tt_ccode = "";
        string tt_ncode = "";
        int tt_scanboxnum = 0;

        //加载
        private void Form7_zx_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";


            if (str.Contains("ZX001"))
            {
                this.button2.Visible = false;
                this.button3.Visible = false;

                this.button6.Visible = false;
                this.button7.Visible = false;

                this.button8.Visible = false;
                this.button9.Visible = false;

            }



            ClearLabelInfo1();
            this.label32.Text = tt_yield.ToString();

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
            this.listView1.Columns.Add("SN",120);
            this.listView1.Columns.Add("PCBA", 130);
            this.listView1.Columns.Add("MAC", 100);
            this.listView1.Columns.Add("移动条码", 250);
            this.listView1.Columns.Add("长MAC", 150);



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

            //流程信息
            this.label52.Text = null;
            this.label53.Text = null;
            this.label54.Text = null;
            this.label55.Text = null;


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
            this.label29.Text = null;
            this.label30.Text = null;


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
            this.label29.Text = null;
            this.label30.Text = null;

            //流程信息
            this.label53.Text = null;


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
            this.checkBox7.Visible = false;
            this.label19.Visible = false;
            this.label42.Visible = false;
            this.label45.Visible = false;
            this.button8.Visible = false;
            this.button9.Visible = false;


        }


        #endregion



        #region 3、按钮事件
        //重置
        private void button1_Click(object sender, EventArgs e)
        {
            ClearLabelInfo2();
            textBox4.Focus();
            textBox4.SelectAll();

        }

        //模板一预览
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {
                GetParaDataPrint(2, true, false, false);
            }
            else
            {
                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再预览模板");
            }
        }

        //模板一打印
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("确定要重打铭牌吗，打印信息被记录", "铭牌重打", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label53.Text;
                    string tt_checkcode = this.label54.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                    if (tt_flag)
                    {
                        GetParaDataPrint(1, true, false, false);  //打印
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
                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再重打");
            }
        }


        //模板二预览
        private void button6_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {
               GetParaDataPrint(2, false, true, false);
            }
            else
            {
                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再预览模板");
            }
        }

        //模板二打印
        private void button7_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("确定要重打铭牌吗，打印信息被记录", "铭牌重打", messButton);
                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label53.Text;
                    string tt_checkcode = this.label54.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                    if (tt_flag)
                    {
                        GetParaDataPrint(1, false, true, false);  //打印
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
                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再预重打");
            }
        }

        //模板三预览
        private void button8_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {
               GetParaDataPrint(2, false, false, true);
             }
            else
            {
                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再预览模板");
            }
        }

        //模板三打印
        private void button9_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("确定要重打铭牌吗，打印信息被记录", "铭牌重打", messButton);
                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    string tt_prientcode = this.label53.Text;
                    string tt_checkcode = this.label54.Text;

                    Boolean tt_flag = CheckCodeStation(tt_prientcode, tt_checkcode);

                    if (tt_flag)
                    {
                        GetParaDataPrint(1, false, false, true);  //打印
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
                PutLableInfor("参数表数据为空，不能预览，输入21条码查询数据后，再重打");
            }
        }


        //尾箱
        private void button5_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("需装箱数量："+this.textBox2.Text+"，已装箱数量："+this.textBox3.Text+",确定生成尾箱码", "生成尾箱", messButton);

            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {
                //第一步获取箱号
                string tt_package = "";
                //if (this.label15.Text.Length < 5)
                //{
                //    tt_package = GetBoxNumber2(this.label25.Text, this.textBox1.Text);
                //    label46.Text = tt_package;
                //}
                //else
                //{
                //    tt_package = GetBoxNumber(label25.Text, this.label47.Text, this.textBox2.Text);
                //    label46.Text = tt_package;
                //}


                tt_package = GetBoxNumber3(label15.Text, this.label47.Text, this.textBox2.Text);
                label46.Text = tt_package;

                //第二步 装箱过站

                Boolean tt_passflage = ListViewStatioPass(this.textBox1.Text, tt_gyid, tt_ccode, tt_ncode, tt_package, tt_conn);


                //第三步数据清理
                if (tt_passflage)
                {
                    GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, this.checkBox7.Checked);
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

            if (this.label46.Text.Length > 0)
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
            else
            {
                MessageBox.Show("箱号为空，无法打散，请确认");
            }

        }
        #endregion


        #region 4、锁定事件
        //工单号选择
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                string tt_sql1 = "select tasksquantity,product_name,fec,convert(varchar, taskdate, 102) fdate,tasktype,softwareversion,gyid " +
                                 "from odc_tasks  where taskscode = '" + this.textBox1.Text + "' ";

                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label9.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label10.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //产品名称
                    this.label11.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //EC代码
                    this.label12.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString();  //生产日期
                    this.label25.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //物料编码
                    this.label57.Text = ds1.Tables[0].Rows[0].ItemArray[5].ToString();  //软件版本
                    this.label52.Text = ds1.Tables[0].Rows[0].ItemArray[6].ToString();  //流程编码


                    //第一步、流程检查
                    Boolean tt_flag1 = false;
                    if (!this.label52.Text.Equals(""))
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
                        string tt_eccode = this.label11.Text;
                        string tt_sql2 = "select Fpath06,Fpath07,Fpath08,Fdata06,Fdata07,Fdata08,Docdesc,Macxp " +
                                          " from odc_ec  where zjbm = '" + tt_eccode + "' ";
                        DataSet ds2 = Dataset1.GetDataSet(tt_sql2, tt_conn);
                        if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                        {
                            tt_flag2 = true;
                            this.label40.Text = ds2.Tables[0].Rows[0].ItemArray[0].ToString();
                            this.label41.Text = ds2.Tables[0].Rows[0].ItemArray[1].ToString();
                            this.label42.Text = ds2.Tables[0].Rows[0].ItemArray[2].ToString();

                            this.label43.Text = ds2.Tables[0].Rows[0].ItemArray[3].ToString();
                            this.label44.Text = ds2.Tables[0].Rows[0].ItemArray[4].ToString();
                            this.label45.Text = ds2.Tables[0].Rows[0].ItemArray[5].ToString();

                            this.label13.Text = ds2.Tables[0].Rows[0].ItemArray[6].ToString();
                        }
                        else
                        {
                            MessageBox.Show("没有找到工表的EC表配置信息，请确认！");
                        }

                    }




                    Boolean tt_flag3 = false;
                    if (tt_flag1 && tt_flag2)
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



                    if (tt_flag1 && tt_flag2 && tt_flag3)
                    {
                        this.textBox1.Enabled = false;
                        this.textBox4.Visible = true;
                        this.textBox9.Visible = true;
                    }



                    

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
            }

        }

        //装箱打印位数判断
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

        //SN查询位数判断
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

        //装箱数量锁定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if( this.checkBox2.Checked)
            {
                this.textBox2.Enabled = false;
                this.textBox3.Enabled = false;
            }
            else
            {
                this.textBox2.Enabled = true;
                this.textBox3.Enabled = true;
            }
        }

        //模板一
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        //模板2
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        //模板三
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {

        }

        #endregion


        #region 5、扫描事件

        //标签重打扫描
        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                //开始SN扫描
                ScanDataInitial();
                setRichtexBox("------开始装箱重打扫描--------");
                string tt_scanboxsn = this.textBox9.Text.Trim();
                string tt_task = this.textBox1.Text.Trim();
                string tt_gesn = "";
                string tt_pcba = "";
                string tt_maclable = "";
                string tt_boxsn = "";
                string tt_barcode = "";
                string tt_longmac = "";


                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanboxsn,this.textBox8.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if(tt_flag1)
                {
                   tt_flag2 = CheckStrContain(tt_scanboxsn, this.textBox7.Text.Trim());
                }



                //第三步判断是否有箱号
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    string tt_sql3 = "select count(1), min(T2.pagesn),0 "+
                                     "from odc_alllable T1 "+
                                     "left outer join odc_package T2 on T1.pcbasn = T2.pasn "+
                                     "where taskscode = '" + tt_task + "' and hostlable = '" + tt_scanboxsn + "' ";
                    string[] tt_array3 = new string[3];
                    tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                    if (tt_array3[0] == "1")
                    {
                        tt_flag3 = true;
                        tt_gesn = tt_array3[1];
                        this.label46.Text = tt_gesn;
                        setRichtexBox("3、找到一个箱号：" + tt_gesn + ", goon");
                    }
                    else
                    {
                        setRichtexBox("3、该SN包装表中没有找到箱号，over");
                        PutLableInfor( "该SN号包装表中没有找到箱号！");
                    }
                }




                //第四步查找数据
                Boolean tt_flag4 = false;
                if ( tt_flag1 && tt_flag2 && tt_flag3)
                {
                    string tt_sql4 = "select  T2.hostlable,T2.pcbasn,T2.maclable,T2.SMtaskscode, T2.bprintuser " +
                                     "from odc_package T1 " +
                                     "left outer join odc_alllable T2 on T1.pasn = T2.pcbasn " +
                                     "where taskcode = '" + tt_task + "'  and pagesn = '" + tt_gesn + "'  order by T2.hostlable";

                    DataSet ds4 = Dataset1.GetDataSet(tt_sql4, tt_conn);
                    if (ds4.Tables.Count > 0 && ds4.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        setRichtexBox("4、关联表查询到:" + ds4.Tables[0].Rows.Count .ToString()+ "条数据，,goon");
                        this.textBox3.Text = ds4.Tables[0].Rows.Count.ToString();
                        
                        for (int i = 0; i < ds4.Tables[0].Rows.Count; i++ )
                        {
                            tt_boxsn = ds4.Tables[0].Rows[i].ItemArray[0].ToString();
                            tt_pcba = ds4.Tables[0].Rows[i].ItemArray[1].ToString();
                            tt_maclable = ds4.Tables[0].Rows[i].ItemArray[2].ToString();
                            tt_barcode = ds4.Tables[0].Rows[i].ItemArray[3].ToString();
                            tt_longmac = ds4.Tables[0].Rows[i].ItemArray[4].ToString();
                            PutListViewData(tt_boxsn, tt_pcba, tt_maclable, tt_barcode, tt_longmac);

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
                        setRichtexBox("4、关联表没有查询到数据，over");
                        PutLableInfor ("关联表没有查询到数据，请检查！");
                    }



                }










                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    GetParaDataPrint(0, true, false, false);
                    CheckStation(tt_task,tt_gesn);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    textBox9.Focus();
                    textBox9.SelectAll();
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                    textBox9.Focus();
                    textBox9.SelectAll();
                }
               


            }
        }

        //SN过站扫描
        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                //开始SN过站扫描
                this.label39.Text = null;
                if (tt_scanboxnum ==0 )
                {
                    ClearLabelInfo4();
                }


                setRichtexBox("------开始装箱过站扫描--------");
                string tt_scanboxsn = this.textBox4.Text.Trim();
                string tt_task = this.textBox1.Text.Trim();
                string tt_gesn = "";
                string tt_pcba = "";
                string tt_maclable = "";
                string tt_smtaskscode = "";
                string tt_longmac = "";


                //string tt_b = GetBoxNumber2(this.label25.Text, this.textBox1.Text);



                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanboxsn, this.textBox5.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanboxsn, this.textBox6.Text.Trim());
                }


                //第三步装箱数量锁定
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2)
                {
                    if (this.checkBox2.Checked)
                    {
                        tt_flag3 = true;
                        setRichtexBox("3、工单装箱数量已锁定，goon");
                    }
                    else
                    {
                        setRichtexBox("3、工单装箱数量没有锁定，over");
                        PutLableInfor("工单装箱数量没有锁定，请检查！");
                    }
                }



                //第四步是否重复扫描
                Boolean tt_flag4 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3)
                {

                     Boolean tt_repeat = CheckNumberRepeat(tt_scanboxsn);

                     if (tt_repeat)
                     {
                          setRichtexBox("4、装箱有重复扫描了，end");
                          PutLableInfor("不能重复扫描此产品！");
                     }
                     else
                     {
                          tt_flag4 = true;
                          setRichtexBox("4、没有重复扫描了，goon");

                     }

                }


                //第五步是否按顺序扫描
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {

                    if (tt_scanboxnum == 0)
                    {
                        tt_flag5 = true;
                        setRichtexBox("5、这个是第一个产品，不用检查数据，goon");
                    }
                    else
                    {
                        int tt_count = this.listView1.Items.Count;
                        if (tt_count > 0)
                        {


                            string tt_box1 = this.listView1.Items[tt_count - 1].SubItems[1].Text;
                            int tt_boxnumber1 = int.Parse(tt_box1.Substring(tt_box1.Length - 4, 4));

                            string tt_box2 = this.textBox4.Text;
                            int tt_boxnumber2 = int.Parse(tt_box2.Substring(tt_box1.Length - 4, 4));


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
                }






                //第六步查找关联表数据
                Boolean tt_flag6 = false;

                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                {
                    string tt_sql6 = "select pcbasn,maclable,smtaskscode,bprintuser from odc_alllable " +
                                     "where taskscode = '" + this.textBox1.Text + "' and boxlable = '" + tt_scanboxsn + "' ";

                    DataSet ds6 = Dataset1.GetDataSet(tt_sql6, tt_conn);
                    if (ds6.Tables.Count > 0 && ds6.Tables[0].Rows.Count > 0)
                    {
                        tt_flag6 = true;
                        tt_pcba = ds6.Tables[0].Rows[0].ItemArray[0].ToString();
                        tt_maclable = ds6.Tables[0].Rows[0].ItemArray[1].ToString();
                        tt_smtaskscode = ds6.Tables[0].Rows[0].ItemArray[2].ToString();
                        tt_longmac = ds6.Tables[0].Rows[0].ItemArray[3].ToString();
                        setRichtexBox("6、关联表查询到一条数据，PCBA=" + tt_pcba + ",MAC=" + tt_pcba + ",smtaskscode=" + tt_smtaskscode + ",goon");

                    }
                    else
                    {
                        setRichtexBox("6、关联表没有查询到数据，或工单不对，over");
                        PutLableInfor("关联表没有查询到数据，或工单不对，请检查！");
                    }

                }


                //第七步流程检查
                Boolean tt_flag7 = false;
                 tt_gyid = this.label52.Text;
                 tt_ccode = this.label54.Text;
                 tt_ncode = this.label55.Text;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
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




                //第八步 查找站位信息
                Boolean tt_flag8 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7)
                {
                    string tt_sql8 = "select count(1),min(ccode),min(ncode) from odc_routingtasklist " +
                                     "where  pcba_pn = '" + tt_maclable + "' and napplytype is null ";


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
                        PutLableInfor( "没有找到待测站位，或有多条待测站位，流程异常！");
                    }


                }







                //第九步是否装箱判断
                Boolean tt_flag9 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8)
                {
                    string tt_sql9 = "select  count(1),min(pagesn),min(pagetime) from odc_package " +
                                      "where taskcode = '"+tt_task+"' and pasn = '"+tt_pcba+"' ";

                     string[] tt_array9 = new string[3];
                    tt_array9 = Dataset1.GetDatasetArray(tt_sql9, tt_conn);
                    if (tt_array9[0] == "0")
                    {
                        tt_flag9 = true;
                        setRichtexBox("9、该产品还没有装箱，可以装箱,goon");
                    }
                    else
                    {
                        tt_gesn = tt_array9[1];
                        setRichtexBox("9、该产品已装箱，箱号：" + tt_gesn + ",装箱时间：" + tt_array9[2] + "");
                        PutLableInfor("该产品已装箱，箱号：" + tt_gesn);
                    }

                }



                //第十步模板检查
                Boolean tt_flag10 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9)
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
                    

                    Boolean tt_flag93 = checkBox7.Checked;
                    if (tt_flag93)
                    {
                        string tt_path3 = Application.StartupPath + this.label42.Text;
                        tt_flag93 = GetPathIstrue(tt_path3);
                        if (tt_flag93)
                        {
                            setRichtexBox("10.3、模板三检查OK,goon");
                        }
                        else
                        {
                            setRichtexBox("10.3、模板三检查fail,请确认，over");
                            PutLableInfor(this.label39.Text + "模板三检查fail；");
                        }
                    }
                   


                    if (tt_flag91 || tt_flag92 || tt_flag93)
                    {
                        tt_flag10 = true;
                        setRichtexBox("10、总之模板路径检查OK，至少有一个模板可以打印,goon");

                    }
                    else
                    {
                        setRichtexBox("10、总之模板路径检查失败，没有一个模板可以打印,goon");
                        PutLableInfor(this.label39.Text + "没有一个模板可以使用，请检查确认");
                    }



                }





                //第十一步信息比对检查
                Boolean tt_flag11 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10)
                {
                    //string tt_sql10 = "select count(1),0,0 from odc_alllable " +
                    //                "where taskscode ='" + tt_task + "' and boxlable = '" + tt_scanboxsn + "' and bosasn is not null ";

                    //string[] tt_array10 = new string[3];
                    //tt_array10 = Dataset1.GetDatasetArray(tt_sql10, tt_conn);
                    //if (tt_array10[0] == "1")
                    //{
                    //    tt_flag10 = true;
                    //    setRichtexBox("10、该条码已信息比对,goon");
                    //}
                    //else
                    //{
                    //    setRichtexBox("10、该条码没有进行信息比对,over");
                    //    PutLableInfor("该条码没有进行信息比对,请确认" );
                    //}


                    tt_flag11 = true;
                    setRichtexBox("11、该条码已信息比对,goon");


                }





                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag9 && tt_flag10 && tt_flag11)
                {

                    PutListViewData(tt_scanboxsn, tt_pcba, tt_maclable, tt_smtaskscode, tt_longmac);
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
                    if( this.textBox2.Text == this.textBox3.Text)
                    {
                        //第一步获取箱号
                        //if (this.label15.Text.Length < 5)
                        //{
                        //    tt_package = GetBoxNumber2(this.label25.Text, this.textBox1.Text);
                        //    label46.Text = tt_package;
                        //}
                        //else
                        //{
                        //    tt_package = GetBoxNumber(label25.Text, this.label47.Text, this.textBox2.Text);
                        //    label46.Text = tt_package;
                        //}

                        tt_package = GetBoxNumber3(label15.Text, this.label47.Text, this.textBox2.Text);
                        label46.Text = tt_package;


                        //第二步 装箱过站

                        Boolean tt_passflage = ListViewStatioPass(tt_task, tt_gyid, tt_ccode, tt_ncode, tt_package, tt_conn);


                        //第三步打印标签,清理数据
                        if (tt_passflage )
                        {
                            GetParaDataPrint(1, this.checkBox5.Checked, this.checkBox6.Checked, this.checkBox7.Checked);
                            ClearLabelInfo3();
                        }

                    }

                    CheckStation(tt_task, tt_package);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                }
                else
                {
                    this.richTextBox1.BackColor = Color.Red;
                }



                textBox4.Focus();
                textBox4.SelectAll();


            }
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
            this.label39.Text = tt_lableinfo;
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
                    PutLableInfor( "包含符判断不正确，不包含字符" + tt_containstr + ",请确认！");
                }

            }
            else
            {
                tt_flag = true;
                setRichtexBox("2、包含符为空，不需判断，goon");
            }

            return tt_flag;
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


        //刷新站位
        private void CheckStation(string tt_task,string tt_gesn)
        {
            string tt_sql = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime " +
                            "from odc_routingtasklist " +

                            "where taskscode = '"+tt_task+"' and napplytype is null " +

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



        //获取箱号  青岛获取箱号
        private string GetBoxNumber(string tt_beforstr, string tt_fromsn,string tt_setunitnum)
        {
            string tt_boxnumber = "";
            decimal tt_unitint = decimal.Parse(tt_setunitnum);
            decimal tt_snnumber = int.Parse(tt_fromsn.Substring(tt_fromsn.Length - 5, 5));
            decimal tt_boxnum2 = Math.Ceiling(tt_snnumber / tt_unitint);
            string tt_boxnum3 = tt_boxnum2.ToString();
            tt_boxnumber = tt_beforstr + tt_boxnum3.PadLeft(6, '0');
            return tt_boxnumber;
        }

        
        //贵州获取箱号
        private string GetBoxNumber2(string tt_beforstr,string tt_task)
        {
            string tt_boxnumber = "99999";
            string tt_boxnumber2 = "99999";
            string tt_boxnumbernext = "";
            

            string tt_sql1 = "select count(1),min(hostmode),0 from ODC_HOSTLABLEOPTIOAN " +
                             " where taskscode = '" + tt_task + "' ";
            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql1, tt_conn);
            if (tt_array[0] == "1")
            {

                tt_boxnumber = tt_array[1];
                int A = Convert.ToInt32(tt_boxnumber);
                tt_boxnumbernext = (A + 1).ToString();

                
                string tt_update = "update ODC_HOSTLABLEOPTIOAN set hostmode = '"+tt_boxnumbernext+"' " +
                                   "where taskscode = '"+tt_task+"' ";


                int tt_int = Dataset1.ExecCommand(tt_update, tt_conn);
                if (tt_int > 0)
                {
                    tt_boxnumber2 = tt_boxnumber;
                    tt_boxnumber2 = string.Format("{0:d5}", A); 

                }
                else
                {
                    MessageBox.Show("箱号设置更新失败！");
                }



            }
            else
            {
                MessageBox.Show("没有找到该箱号的设置信息");
            }








            return tt_beforstr+tt_boxnumber2;
        }



        //获取箱号  烽火wifi箱号
        private string GetBoxNumber3(string tt_beforstr, string tt_fromsn, string tt_setunitnum)
        {
            string tt_boxnumber = "";
            decimal tt_unitint = decimal.Parse(tt_setunitnum);
            decimal tt_snnumber = int.Parse(tt_fromsn.Substring(tt_fromsn.Length - 4, 4));
            decimal tt_boxnum2 = Math.Ceiling(tt_snnumber / tt_unitint);
            string tt_boxnum3 = tt_boxnum2.ToString();
            tt_boxnumber = tt_beforstr + "C"+ tt_boxnum3.PadLeft(3, '0');
            return tt_boxnumber;
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
                this.label54.Text = tt_ccode;
                this.label55.Text = tt_ncode;
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


        #region 7、ListView操作

        //listview情空
        private void ClearListView()
        {
            int count = this.listView1.Items.Count;
            for (int i = 0; i < count; i++)
            {
                listView1.Items[0].Remove();
            }
        }

        //添加listview数据
        private void PutListViewData(string tt_boxsn, string tt_pcba, string tt_mac, string tt_smtbarcode, string tt_hwmac)
        {
            int i = this.listView1.Items.Count + 1;
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { i.ToString(), tt_boxsn, tt_pcba, tt_mac, tt_smtbarcode, tt_hwmac });
            this.listView1.Items.AddRange(p);
            this.listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
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


                tt_flag11 = Dataset1.FhPackageInStation(tt_task, STR,  tt_listpcba, tt_listmac, tt_gyid, tt_code, tt_ncode, tt_package, tt_conn);

                if (tt_flag11)
                {

                    setRichtexBox("12." + i.ToString() + "、SN：" + tt_boxsn + ",第一次过站成功，ok");

                }
                else
                {
                    setRichtexBox("12." + i.ToString() + "、SN：" + tt_boxsn + ",第一次过站失败，开始第二次过站");
                    tt_flag22 = Dataset1.FhPackageInStation(tt_task, STR, tt_listpcba, tt_listmac, tt_gyid, tt_code, tt_ncode, tt_package, tt_conn);
                    if (tt_flag22)
                    {
                        setRichtexBox("12." + i.ToString() + "、SN：" + tt_boxsn + ",第二次过站成功,ok");
                    }
                    else
                    {
                        setRichtexBox("12." + i.ToString() + "、SN：" + tt_boxsn + ",第二次过站失败,end");
                    }

                }

                //记录过站次数
                if (tt_flag11 || tt_flag22 )
                {

                    tt_passcount++;
                }


            }


            //第三步确定过程结果
            if (tt_passcount == count)
            {
                tt_flag = true;
                setRichtexBox("12、全部过站成功，成功次数：" + tt_passcount.ToString() + ",ok");
                PutLableInfor("12、全部过站成功，成功次数：" + tt_passcount.ToString());
            }
            else
            {
                setRichtexBox("12、糟糕、没有全部过站成功，成功次数：" + tt_passcount.ToString() + ",ok");
                PutLableInfor("糟糕、没有全部过站成功，成功次数：" + tt_passcount.ToString());
            }


            return tt_flag;
        }




        //获取ListView数据
        private string GetListViewItem(int tt_itemtype,int tt_itemnumber)
        {
            string tt_item = "";

            int tt_count = this.listView1.Items.Count;

            if (tt_count >= tt_itemnumber)
            {
                if (tt_itemtype == 1)
                {

                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[1].Text; 
                }
                else if(tt_itemtype == 2)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[3].Text; 
                }
                else if (tt_itemtype == 3)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[4].Text; 
                }
                else if (tt_itemtype == 4)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[5].Text;
                }
                else if (tt_itemtype == 5)
                {
                    tt_item = this.listView1.Items[tt_itemnumber - 1].SubItems[5].Text.Substring(0,17);
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
               }
            }

            return tt_flag;

        }





        #endregion

        #region 8、数据查询

        //数据查询确定
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
                string tt_sql2 = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime 创建时间,fremark 备注 " +
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
        private void button11_Click(object sender, EventArgs e)
        {
            this.textBox10.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;
        }
        #endregion

        #region 9、打印标签

        //获取参数
        private void GetParaDataPrint(int tt_itemtype,Boolean tt_flag1,Boolean tt_flag2,Boolean tt_flag3)
        {
            //模板一打印
            if (tt_flag1)
            {
                setRichtexBox("20、开始模板一打印");

                string tt_path1 = Application.StartupPath + this.label40.Text;
                string tt_datatype1 = this.label43.Text;


                if (tt_datatype1 == "ZGZX01")
                {
                    GetParaDataPrint_ZGZX01(tt_path1, tt_itemtype);
                }
                else if (tt_datatype1 == "ZGZX02")
                {
                    GetParaDataPrint_ZGZX02(tt_path1, tt_itemtype);
                }
                else if (tt_datatype1 == "ZGZX03")
                {
                    GetParaDataPrint_ZGZX03(tt_path1, tt_itemtype);
                }
                else if (tt_datatype1 == "ZGZX04")
                {
                    GetParaDataPrint_ZGZX04(tt_path1, tt_itemtype);
                }
                else if (tt_datatype1 == "ZGZX05")
                {
                    GetParaDataPrint_ZGZX05(tt_path1, tt_itemtype);
                }
                else if (tt_datatype1 == "ZGZX06")
                {
                    GetParaDataPrint_ZGZX06(tt_path1, tt_itemtype);
                }
                else if (tt_datatype1 == "ZX01")
                {
                    GetParaDataPrint_ZX01(tt_path1, tt_itemtype);
                }
                else if (tt_datatype1 == "ZX02")
                {
                    GetParaDataPrint_ZX02(tt_path1, tt_itemtype);
                }
                



            }


            //模板二打印
            if (tt_flag2)
            {
                setRichtexBox("30、开始模板二打印");
                string tt_path2 = Application.StartupPath + this.label41.Text;
                string tt_datatype2 = this.label44.Text;

                if (tt_datatype2 == "ZGZX01")
                {
                    GetParaDataPrint_ZGZX01(tt_path2, tt_itemtype);
                }
                else if (tt_datatype2 == "ZGZX02")
                {
                    GetParaDataPrint_ZGZX02(tt_path2, tt_itemtype);
                }
                else if (tt_datatype2 == "ZGZX03")
                {
                    GetParaDataPrint_ZGZX03(tt_path2, tt_itemtype);
                }
                else if (tt_datatype2 == "ZGZX04")
                {
                    GetParaDataPrint_ZGZX04(tt_path2, tt_itemtype);
                }
                else if (tt_datatype2 == "ZGZX05")
                {
                    GetParaDataPrint_ZGZX05(tt_path2, tt_itemtype);
                }
                else if (tt_datatype2 == "ZGZX06")
                {
                    GetParaDataPrint_ZGZX06(tt_path2, tt_itemtype);
                }
                else if (tt_datatype2 == "ZX01")
                {
                    GetParaDataPrint_ZX01(tt_path2, tt_itemtype);
                }
                else if (tt_datatype2 == "ZX02")
                {
                    GetParaDataPrint_ZX02(tt_path2, tt_itemtype);
                }

            }

            //模板三打印
            if (tt_flag3)
            {
                setRichtexBox("40、开始模板三打印");
                string tt_path3 = Application.StartupPath + this.label42.Text;
                string tt_datatype3 = this.label45.Text;

                if (tt_datatype3 == "ZGZX01")
                {
                    GetParaDataPrint_ZGZX01(tt_path3, tt_itemtype);
                }
                else if (tt_datatype3 == "ZGZX02")
                {
                    GetParaDataPrint_ZGZX02(tt_path3, tt_itemtype);
                }
                else if (tt_datatype3 == "ZGZX03")
                {
                    GetParaDataPrint_ZGZX03(tt_path3, tt_itemtype);
                }
                else if (tt_datatype3 == "ZGZX04")
                {
                    GetParaDataPrint_ZGZX04(tt_path3, tt_itemtype);
                }
                else if (tt_datatype3 == "ZGZX05")
                {
                    GetParaDataPrint_ZGZX05(tt_path3, tt_itemtype);
                }
                else if (tt_datatype3 == "ZGZX06")
                {
                    GetParaDataPrint_ZGZX06(tt_path3, tt_itemtype);
                }
                else if (tt_datatype3 == "ZX01")
                {
                    GetParaDataPrint_ZX01(tt_path3, tt_itemtype);
                }
                else if (tt_datatype3 == "ZX02")
                {
                    GetParaDataPrint_ZX02(tt_path3, tt_itemtype);
                }


            }


        }





        //----以下是ZX01数据采集----朝歌中箱青岛模板一---
        private void GetParaDataPrint_ZGZX01(string tt_path,int tt_itemtype)
        {

            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();

            //加二维码数据
            int count = this.listView1.Items.Count;
            string tt_twodimsn = "";
            string tt_twodimmac = "";
            string tt_twodimbarcode = "";

            for (int i = 0; i < count; i++)
            {
                tt_twodimsn = tt_twodimsn + this.listView1.Items[i].SubItems[1].Text + "\n\r";
                tt_twodimmac = tt_twodimmac + this.listView1.Items[i].SubItems[5].Text.Substring(0,17) + "\n\r";
                tt_twodimbarcode = tt_twodimbarcode + this.listView1.Items[i].SubItems[4].Text + "\n\r";
            }


            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            DataRow row1 = dt.NewRow();
            row1["参数"] = "N01";
            row1["名称"] = "箱号";
            row1["内容"] = this.label46.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "EC编码";
            row2["内容"] = this.label11.Text;
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "对外型号";
            row3["内容"] = this.label10.Text;
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "起始SN";
            row4["内容"] = this.label47.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "结束SN";
            row5["内容"] = this.label48.Text;
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N06";
            row6["名称"] = "数量";
            row6["内容"] = this.textBox3.Text;
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N07";
            row7["名称"] = "SN条码";
            row7["内容"] = tt_twodimsn;
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N08";
            row8["名称"] = "MAC条吗";
            row8["内容"] = tt_twodimbarcode;
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "N09";
            row9["名称"] = "移动条码";
            row9["内容"] = tt_twodimmac;
            dt.Rows.Add(row9);


            DataRow row10 = dt.NewRow();
            row10["参数"] = "N10";
            row10["名称"] = "EC描述";
            row10["内容"] = this.label13.Text;
            dt.Rows.Add(row10);


            DataRow row11 = dt.NewRow();
            row11["参数"] = "N11";
            row11["名称"] = "生产日期";
            row11["内容"] = label12.Text.Replace(".","");
            dt.Rows.Add(row11);

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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }





        }


        //----以下是ZX02数据采集----朝歌中箱青岛模板二---
        private void GetParaDataPrint_ZGZX02(string tt_path, int tt_itemtype)
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
            row1["名称"] = "MAC1_1";
            row1["内容"] = GetListViewItem(5,1);
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "MAC1_2";
            row2["内容"] = GetListViewItem(5, 2);
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "MAC1_3";
            row3["内容"] = GetListViewItem(5, 3);
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "MAC1_4";
            row4["内容"] = GetListViewItem(5, 4);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "MAC1_5";
            row5["内容"] = GetListViewItem(5, 5);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N06";
            row6["名称"] = "MAC1_6";
            row6["内容"] = GetListViewItem(5, 6);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N07";
            row7["名称"] = "MAC1_7";
            row7["内容"] = GetListViewItem(5, 7);
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N08";
            row8["名称"] = "MAC1_8";
            row8["内容"] = GetListViewItem(5, 8);
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "N09";
            row9["名称"] = "MAC1_9";
            row9["内容"] = GetListViewItem(5, 9);
            dt.Rows.Add(row9);


            DataRow row10 = dt.NewRow();
            row10["参数"] = "N10";
            row10["名称"] = "MAC1_10";
            row10["内容"] = GetListViewItem(5, 10);
            dt.Rows.Add(row10);


            DataRow row11 = dt.NewRow();
            row11["参数"] = "N11";
            row11["名称"] = "MAC1_11";
            row11["内容"] = GetListViewItem(5, 11);
            dt.Rows.Add(row11);


            DataRow row12 = dt.NewRow();
            row12["参数"] = "N12";
            row12["名称"] = "MAC1_12";
            row12["内容"] = GetListViewItem(5, 12);
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "N13";
            row13["名称"] = "MAC1_13";
            row13["内容"] = GetListViewItem(5, 13);
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "N14";
            row14["名称"] = "MAC1_14";
            row14["内容"] = GetListViewItem(5, 14);
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "N15";
            row15["名称"] = "MAC1_15";
            row15["内容"] = GetListViewItem(5, 15);
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "N16";
            row16["名称"] = "MAC1_16";
            row16["内容"] = GetListViewItem(5, 16);
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "N17";
            row17["名称"] = "MAC1_17";
            row17["内容"] = GetListViewItem(5, 17);
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "N18";
            row18["名称"] = "MAC1_18";
            row18["内容"] = GetListViewItem(5, 18);
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "N19";
            row19["名称"] = "MAC1_19";
            row19["内容"] = GetListViewItem(5, 19);
            dt.Rows.Add(row19);


            DataRow row20 = dt.NewRow();
            row20["参数"] = "N20";
            row20["名称"] = "MAC1_20";
            row20["内容"] = GetListViewItem(5, 20);
            dt.Rows.Add(row20);


            //-----------

            DataRow row21 = dt.NewRow();
            row21["参数"] = "P01";
            row21["名称"] = "MAC2_1";
            row21["内容"] = GetListViewItem(4, 1);
            dt.Rows.Add(row21);


            DataRow row22 = dt.NewRow();
            row22["参数"] = "P02";
            row22["名称"] = "MAC2_2";
            row22["内容"] = GetListViewItem(4, 2);
            dt.Rows.Add(row22);

            DataRow row23 = dt.NewRow();
            row23["参数"] = "P03";
            row23["名称"] = "MAC2_3";
            row23["内容"] = GetListViewItem(4, 3);
            dt.Rows.Add(row23);

            DataRow row24 = dt.NewRow();
            row24["参数"] = "P04";
            row24["名称"] = "MAC2_4";
            row24["内容"] = GetListViewItem(4, 4);
            dt.Rows.Add(row24);

            DataRow row25 = dt.NewRow();
            row25["参数"] = "P05";
            row25["名称"] = "MAC2_5";
            row25["内容"] = GetListViewItem(4, 5);
            dt.Rows.Add(row25);

            DataRow row26 = dt.NewRow();
            row26["参数"] = "P06";
            row26["名称"] = "MAC2_6";
            row26["内容"] = GetListViewItem(4, 6);
            dt.Rows.Add(row26);

            DataRow row27 = dt.NewRow();
            row27["参数"] = "P07";
            row27["名称"] = "MAC2_7";
            row27["内容"] = GetListViewItem(4, 7);
            dt.Rows.Add(row27);

            DataRow row28 = dt.NewRow();
            row28["参数"] = "P08";
            row28["名称"] = "MAC2_8";
            row28["内容"] = GetListViewItem(4, 8);
            dt.Rows.Add(row28);

            DataRow row29 = dt.NewRow();
            row29["参数"] = "P09";
            row29["名称"] = "MAC2_9";
            row29["内容"] = GetListViewItem(4, 9);
            dt.Rows.Add(row29);


            DataRow row30 = dt.NewRow();
            row30["参数"] = "P10";
            row30["名称"] = "MAC2_10";
            row30["内容"] = GetListViewItem(4, 10);
            dt.Rows.Add(row30);


            DataRow row31 = dt.NewRow();
            row31["参数"] = "P11";
            row31["名称"] = "MAC2_11";
            row31["内容"] = GetListViewItem(4, 11);
            dt.Rows.Add(row31);


            DataRow row32 = dt.NewRow();
            row32["参数"] = "P12";
            row32["名称"] = "MAC2_12";
            row32["内容"] = GetListViewItem(4, 12);
            dt.Rows.Add(row32);

            DataRow row33 = dt.NewRow();
            row33["参数"] = "P13";
            row33["名称"] = "MAC2_13";
            row33["内容"] = GetListViewItem(4, 13);
            dt.Rows.Add(row33);

            DataRow row34 = dt.NewRow();
            row34["参数"] = "P14";
            row34["名称"] = "MAC2_14";
            row34["内容"] = GetListViewItem(4, 14);
            dt.Rows.Add(row34);

            DataRow row35 = dt.NewRow();
            row35["参数"] = "P15";
            row35["名称"] = "MAC2_15";
            row35["内容"] = GetListViewItem(4, 15);
            dt.Rows.Add(row35);

            DataRow row36 = dt.NewRow();
            row36["参数"] = "P16";
            row36["名称"] = "MAC2_16";
            row36["内容"] = GetListViewItem(4, 16);
            dt.Rows.Add(row36);

            DataRow row37 = dt.NewRow();
            row37["参数"] = "P17";
            row37["名称"] = "MAC2_17";
            row37["内容"] = GetListViewItem(4, 17);
            dt.Rows.Add(row37);

            DataRow row38 = dt.NewRow();
            row38["参数"] = "P18";
            row38["名称"] = "MAC2_18";
            row38["内容"] = GetListViewItem(4, 18);
            dt.Rows.Add(row38);

            DataRow row39 = dt.NewRow();
            row39["参数"] = "P19";
            row39["名称"] = "MAC2_19";
            row39["内容"] = GetListViewItem(4, 19);
            dt.Rows.Add(row39);


            DataRow row40 = dt.NewRow();
            row40["参数"] = "P20";
            row40["名称"] = "MAC2_20";
            row40["内容"] = GetListViewItem(4, 20);
            dt.Rows.Add(row40);















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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }


        }


        //----以下是ZX03数据采集----
        private void GetParaDataPrint_ZGZX03(string tt_path, int tt_itemtype)
        {
        }


        //----以下是ZX04数据采集----贵州中箱标签1
        private void GetParaDataPrint_ZGZX04(string tt_path, int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();

            //加二维码数据
            int count = this.listView1.Items.Count;
            int tt_forint = 0;
            if (count > 10)
            {
                tt_forint = 10;
            }
            else
            {
                tt_forint = count;
            }


            string tt_twodimsn = "";
            string tt_twodimmac = "";

            for (int i = 0; i < tt_forint; i++)
            {
                tt_twodimsn = tt_twodimsn + this.listView1.Items[i].SubItems[4].Text + " ";
                tt_twodimmac = tt_twodimmac + this.listView1.Items[i].SubItems[5].Text.Substring(0,17) + " ";
            }






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



            DataRow row11 = dt.NewRow();
            row11["参数"] = "P01";
            row11["名称"] = "MAC1";
            row11["内容"] = GetListViewItem(4, 1);
            dt.Rows.Add(row11);


            DataRow row12 = dt.NewRow();
            row12["参数"] = "P02";
            row12["名称"] = "MAC2";
            row12["内容"] = GetListViewItem(4, 2);
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "P03";
            row13["名称"] = "MAC3";
            row13["内容"] = GetListViewItem(4, 3);
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "P04";
            row14["名称"] = "MAC4";
            row14["内容"] = GetListViewItem(4, 4);
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "P05";
            row15["名称"] = "MAC5";
            row15["内容"] = GetListViewItem(4, 5);
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "P06";
            row16["名称"] = "MAC6";
            row16["内容"] = GetListViewItem(4, 6);
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "P07";
            row17["名称"] = "MAC7";
            row17["内容"] = GetListViewItem(4, 7);
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "P08";
            row18["名称"] = "MAC8";
            row18["内容"] = GetListViewItem(4, 8);
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "P09";
            row19["名称"] = "MAC9";
            row19["内容"] = GetListViewItem(4, 9);
            dt.Rows.Add(row19);


            DataRow row20 = dt.NewRow();
            row20["参数"] = "P10";
            row20["名称"] = "MAC10";
            row20["内容"] = GetListViewItem(4, 10);
            dt.Rows.Add(row20);


            DataRow row21 = dt.NewRow();
            row21["参数"] = "N11";
            row21["名称"] = "二维SN";
            row21["内容"] = tt_twodimsn;
            dt.Rows.Add(row21);


            DataRow row22 = dt.NewRow();
            row22["参数"] = "P11";
            row22["名称"] = "二维MAC";
            row22["内容"] = tt_twodimmac;
            dt.Rows.Add(row22);


            

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

                report.SetParameterValue("N11", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("P11", dst.Tables[0].Rows[21][2].ToString());

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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }



        }


        //----以下是ZX05数据采集----贵州中箱标签2
        private void GetParaDataPrint_ZGZX05(string tt_path, int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();

            //加二维码数据
            int count = this.listView1.Items.Count;
            int tt_forint = 0;
            if (count > 10)
            {
                tt_forint = count;
            }
            else
            {
                tt_forint = 0;
            }


            string tt_twodimsn = "";
            string tt_twodimmac = "";

            for (int i = 10; i < tt_forint; i++)
            {
                tt_twodimsn = tt_twodimsn + this.listView1.Items[i].SubItems[4].Text + " ";
                tt_twodimmac = tt_twodimmac + this.listView1.Items[i].SubItems[3].Text + " ";
            }






            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            DataRow row1 = dt.NewRow();
            row1["参数"] = "N01";
            row1["名称"] = "移动码11";
            row1["内容"] = GetListViewItem(3, 11);
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "移动码12";
            row2["内容"] = GetListViewItem(3, 12);
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "移动码13";
            row3["内容"] = GetListViewItem(3, 13);
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "移动码14";
            row4["内容"] = GetListViewItem(3, 14);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "移动码15";
            row5["内容"] = GetListViewItem(3, 15);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N06";
            row6["名称"] = "移动码16";
            row6["内容"] = GetListViewItem(3, 16);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N07";
            row7["名称"] = "移动码17";
            row7["内容"] = GetListViewItem(3, 17);
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N08";
            row8["名称"] = "移动码18";
            row8["内容"] = GetListViewItem(3, 18);
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "N09";
            row9["名称"] = "移动码19";
            row9["内容"] = GetListViewItem(3, 19);
            dt.Rows.Add(row9);


            DataRow row10 = dt.NewRow();
            row10["参数"] = "N10";
            row10["名称"] = "移动码20";
            row10["内容"] = GetListViewItem(3, 20);
            dt.Rows.Add(row10);



            DataRow row11 = dt.NewRow();
            row11["参数"] = "P01";
            row11["名称"] = "MAC11";
            row11["内容"] = GetListViewItem(4, 11);
            dt.Rows.Add(row11);


            DataRow row12 = dt.NewRow();
            row12["参数"] = "P02";
            row12["名称"] = "MAC12";
            row12["内容"] = GetListViewItem(4, 12);
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "P03";
            row13["名称"] = "MAC13";
            row13["内容"] = GetListViewItem(4, 13);
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "P04";
            row14["名称"] = "MAC14";
            row14["内容"] = GetListViewItem(4, 14);
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "P05";
            row15["名称"] = "MAC15";
            row15["内容"] = GetListViewItem(4, 15);
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "P06";
            row16["名称"] = "MAC16";
            row16["内容"] = GetListViewItem(4, 16);
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "P07";
            row17["名称"] = "MAC17";
            row17["内容"] = GetListViewItem(4, 17);
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "P08";
            row18["名称"] = "MAC18";
            row18["内容"] = GetListViewItem(4, 18);
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "P09";
            row19["名称"] = "MAC19";
            row19["内容"] = GetListViewItem(4, 19);
            dt.Rows.Add(row19);


            DataRow row20 = dt.NewRow();
            row20["参数"] = "P10";
            row20["名称"] = "MAC20";
            row20["内容"] = GetListViewItem(4, 20);
            dt.Rows.Add(row20);


            DataRow row21 = dt.NewRow();
            row21["参数"] = "N11";
            row21["名称"] = "二维SN";
            row21["内容"] = tt_twodimsn;
            dt.Rows.Add(row21);


            DataRow row22 = dt.NewRow();
            row22["参数"] = "P11";
            row22["名称"] = "二维MAC";
            row22["内容"] = tt_twodimmac;
            dt.Rows.Add(row22);


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

                report.SetParameterValue("N11", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("P11", dst.Tables[0].Rows[21][2].ToString());

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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }
        }

        //----以下是ZX06数据采集----贵州中箱标签3
        private void GetParaDataPrint_ZGZX06(string tt_path, int tt_itemtype)
        {
            //第一步数据准备
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();

            //加二维码数据
            int count = this.listView1.Items.Count;
            string tt_twodimsn = "";


            for (int i = 0; i < count; i++)
            {
                tt_twodimsn = tt_twodimsn + this.listView1.Items[i].SubItems[1].Text + " ";

            }




            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            DataRow row1 = dt.NewRow();
            row1["参数"] = "N01";
            row1["名称"] = "SN1";
            row1["内容"] = GetListViewItem(1, 1);
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "N02";
            row2["名称"] = "SN2";
            row2["内容"] = GetListViewItem(1, 2);
            dt.Rows.Add(row2);

            DataRow row3 = dt.NewRow();
            row3["参数"] = "N03";
            row3["名称"] = "SN3";
            row3["内容"] = GetListViewItem(1, 3);
            dt.Rows.Add(row3);

            DataRow row4 = dt.NewRow();
            row4["参数"] = "N04";
            row4["名称"] = "SN4";
            row4["内容"] = GetListViewItem(1, 4);
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "N05";
            row5["名称"] = "SN5";
            row5["内容"] = GetListViewItem(1, 5);
            dt.Rows.Add(row5);

            DataRow row6 = dt.NewRow();
            row6["参数"] = "N06";
            row6["名称"] = "SN6";
            row6["内容"] = GetListViewItem(1, 6);
            dt.Rows.Add(row6);

            DataRow row7 = dt.NewRow();
            row7["参数"] = "N07";
            row7["名称"] = "SN7";
            row7["内容"] = GetListViewItem(1, 7);
            dt.Rows.Add(row7);

            DataRow row8 = dt.NewRow();
            row8["参数"] = "N08";
            row8["名称"] = "SN8";
            row8["内容"] = GetListViewItem(1, 8);
            dt.Rows.Add(row8);

            DataRow row9 = dt.NewRow();
            row9["参数"] = "N09";
            row9["名称"] = "SN9";
            row9["内容"] = GetListViewItem(1, 9);
            dt.Rows.Add(row9);


            DataRow row10 = dt.NewRow();
            row10["参数"] = "N10";
            row10["名称"] = "SN10";
            row10["内容"] = GetListViewItem(1, 10);
            dt.Rows.Add(row10);


            DataRow row11 = dt.NewRow();
            row11["参数"] = "N11";
            row11["名称"] = "SN11";
            row11["内容"] = GetListViewItem(1, 11);
            dt.Rows.Add(row11);


            DataRow row12 = dt.NewRow();
            row12["参数"] = "N12";
            row12["名称"] = "SN12";
            row12["内容"] = GetListViewItem(1, 12);
            dt.Rows.Add(row12);

            DataRow row13 = dt.NewRow();
            row13["参数"] = "N13";
            row13["名称"] = "SN13";
            row13["内容"] = GetListViewItem(1, 13);
            dt.Rows.Add(row13);

            DataRow row14 = dt.NewRow();
            row14["参数"] = "N14";
            row14["名称"] = "SN14";
            row14["内容"] = GetListViewItem(1, 14);
            dt.Rows.Add(row14);

            DataRow row15 = dt.NewRow();
            row15["参数"] = "N15";
            row15["名称"] = "SN15";
            row15["内容"] = GetListViewItem(1, 15);
            dt.Rows.Add(row15);

            DataRow row16 = dt.NewRow();
            row16["参数"] = "N16";
            row16["名称"] = "SN16";
            row16["内容"] = GetListViewItem(1, 16);
            dt.Rows.Add(row16);

            DataRow row17 = dt.NewRow();
            row17["参数"] = "N17";
            row17["名称"] = "SN17";
            row17["内容"] = GetListViewItem(1, 17);
            dt.Rows.Add(row17);

            DataRow row18 = dt.NewRow();
            row18["参数"] = "N18";
            row18["名称"] = "SN18";
            row18["内容"] = GetListViewItem(1, 18);
            dt.Rows.Add(row18);

            DataRow row19 = dt.NewRow();
            row19["参数"] = "N19";
            row19["名称"] = "SN19";
            row19["内容"] = GetListViewItem(1, 19);
            dt.Rows.Add(row19);


            DataRow row20 = dt.NewRow();
            row20["参数"] = "N20";
            row20["名称"] = "SN20";
            row20["内容"] = GetListViewItem(1, 20);
            dt.Rows.Add(row20);


            DataRow row21 = dt.NewRow();
            row21["参数"] = "N21";
            row21["名称"] = "箱号";
            row21["内容"] = label46.Text;
            dt.Rows.Add(row21);


            DataRow row22 = dt.NewRow();
            row22["参数"] = "N22";
            row22["名称"] = "二维SN";
            row22["内容"] = tt_twodimsn;
            dt.Rows.Add(row22);


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

                report.SetParameterValue("N21", dst.Tables[0].Rows[20][2].ToString());
                report.SetParameterValue("N22", dst.Tables[0].Rows[21][2].ToString());


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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }
        }




        //----以下是ZX01数据采集----烽火WIF标签一
        private void GetParaDataPrint_ZX01(string tt_path, int tt_itemtype)
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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }



        }





        //----以下是ZX01数据采集----烽火WIF标签一
        private void GetParaDataPrint_ZX02(string tt_path, int tt_itemtype)
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
                setRichtexBox("99、获取信息失败，不能打印或预览，请检查数据,over");
                PutLableInfor("获取信息失败，不能打印或预览，请检查数据！");
            }



        }



        #endregion



        


        












    }
}
