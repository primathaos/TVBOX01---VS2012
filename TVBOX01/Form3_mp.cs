using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;  //正则表达式

namespace TVBOX01
{
    public partial class Form3_mp : Form
    {
        public Form3_mp()
        {
            InitializeComponent();
            //物料追溯初始化
            this.label55.Text = tt_uplip.ToString();
            this.label56.Text = tt_downlip.ToString();
        }

        #region  1、属性设置
        //加载
        private void Form3_mp_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";

            this.toolStripStatusLabel6.Text = tt_productstarttime.ToString();
            this.toolStripStatusLabel9.Text = tt_reprinttime.ToString();


            //员工账号分离
             if( str.Contains("MC001"))
             {
                 this.button2.Visible = false;
                 this.button3.Visible = false;
             }



            this.label13.Text = this.label13.Text + tt_code;
            ClearLabelInfo();
            //生产节拍
            this.label7.Text = tt_yield.ToString();
            this.label8.Text = null;
            this.label9.Text = null;
            this.label10.Text = null;

            //生产信息
            this.label46.Text = null;
            this.label47.Text = null;

            this.textBox2.Visible = false;
            this.textBox3.Visible = false;



        }

        
        static string  tt_conn;
        static string  tt_code ="0000";
        static string tt_path = "";
        int tt_yield = 0;  //产量
        int tt_reprinttime = 0; //重打次数
        int tt_uplip = 0;  //上盖数量
        int tt_downlip = 0; //下盖数量

        DateTime tt_productstarttime = DateTime.Now; //开始时间
        DateTime tt_productprimtime; //上一次时间


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
            this.label28.Text = null;
            this.label29.Text = null;
            this.label30.Text = null;
            this.label31.Text = null;
            this.label32.Text = null;
            this.label33.Text = null;
            this.label34.Text = null;
            this.label49.Text = null;
            
            //流程信息
            this.label76.Text = null;
            this.label77.Text = null;
            this.label79.Text = null;
            this.label85.Text = null;

            //清除老化信息
            this.label62.Text = null;
            this.label63.Text = null;
            this.label64.Text = null;
            this.label65.Text = null;
            this.label67.Text = null;
            this.label70.Text = null;
            this.label71.Text = null;

            //提示信息
            this.label12.Text = null;
            

            //生产信息
            this.label46.Text = null;
            this.label47.Text = null;

            //条码信息
            this.label39.Text = null;
            this.label40.Text = null;
            this.label41.Text = null;
            this.label42.Text = null;
            this.label44.Text = null;

        }


        //扫描前数据初始化
        private void ScanDataInitial()
        {
            //条码信息清除
            this.label39.Text = null;
            this.label40.Text = null;
            this.label41.Text = null;
            this.label42.Text = null;
            this.label44.Text = null;
            this.label80.Text = null;

            //提示信息
            this.label12.Text = null;

            //流程信息清除
            this.label85.Text = null;


            //表格
            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;

            //richtext
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;


        }


        #endregion

        #region 3、锁定事件
        //单板位数锁定
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
        //MAC位数锁定
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

        //工单锁定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if( this.checkBox1.Checked)
            {
                string tt_sql1 = "select  tasksquantity,product_name,areacode,fec,convert(varchar, taskdate, 102) fdate,customer,flhratio,Gyid,Tasktype " +
                                 "from odc_tasks where taskscode = '"+this.textBox1.Text+"' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1,tt_conn);
                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {


                    this.label27.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label29.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString(); //产品名称
                    this.label30.Text = ds1.Tables[0].Rows[0].ItemArray[2].ToString();  //地区
                    this.label31.Text = ds1.Tables[0].Rows[0].ItemArray[3].ToString(); //EC编码
                    this.label28.Text = ds1.Tables[0].Rows[0].ItemArray[4].ToString();  //生产日期

                    this.label67.Text = ds1.Tables[0].Rows[0].ItemArray[5].ToString();  //客户名称
                    this.label62.Text = ds1.Tables[0].Rows[0].ItemArray[6].ToString();  //老化比例

                    this.label79.Text = ds1.Tables[0].Rows[0].ItemArray[7].ToString();  //流程配置
                    this.label49.Text = ds1.Tables[0].Rows[0].ItemArray[8].ToString();  //物料编码

                    //流程检查
                    Boolean tt_flag1 = false;
                    if (!this.label79.Text.Equals(""))
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



                    GetOldNumber(this.label27.Text, this.label62.Text);  //老化比例

                    Boolean tt_flag2 = false;
                    string tt_sql2 = "select  docdesc,Fpath01,Fdata01,Macxp  from odc_ec where zjbm = '" + this.label31.Text + "' ";

                    DataSet ds2 = Dataset1.GetDataSet(tt_sql2, tt_conn);
                    if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                    {
                        this.label34.Text = ds2.Tables[0].Rows[0].ItemArray[0].ToString(); //EC描述
                        this.label32.Text = ds2.Tables[0].Rows[0].ItemArray[2].ToString(); //数据类型
                        tt_path = Application.StartupPath + ds2.Tables[0].Rows[0].ItemArray[1].ToString();
                        this.label33.Text = tt_path;
                        tt_flag2 = true;

                    }
                    else
                    {
                        MessageBox.Show("没有找到工单表的EC表配置信息，请确认！");
                    }




                    if( tt_flag1 && tt_flag2)
                    {
                        this.textBox1.Enabled = false;
                        this.textBox2.Visible = true;
                        this.textBox3.Visible = true;
                        GetProductNumInfo();  //生产信息
                        GetRealOldNumber();   //老化数据
                    }







                    //查询该工单主机号的最大号最小号
                    //string tt_sql3 = "select  count(1),min(Fnameplate),max(Fnameplate) from odc_macinfo " +
                    //                 "where taskscode = '"+this.textBox1.Text+"' ";


                    //string[] tt_array3 = new string[3];
                    //tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                    //this.label49.Text = tt_array3[1];
                    //this.label50.Text = tt_array3[2];





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
                ClearLabelInfo();
                ScanDataInitial();
            }
        }

        #endregion


        #region 4、扫描事件
        //MAC条码扫描
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始MAC扫描--------");
                string tt_task = this.textBox1.Text.Trim();
                string tt_scanmac = this.textBox3.Text.Trim();
                string tt_shortmac = tt_scanmac.Replace(":", "");



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
                if (tt_flag1 && tt_flag2 )
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
                    string tt_sql3 = "select pcbasn,hostlable,maclable,smtaskscode,bprintuser from odc_alllable "+
                                     "where taskscode = '"+tt_task+"' and maclable = '"+tt_shortmac+"' ";


                    DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
                    if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
                    {
                        tt_flag4 = true;
                        this.label39.Text = ds3.Tables[0].Rows[0].ItemArray[0].ToString();
                        this.label40.Text = ds3.Tables[0].Rows[0].ItemArray[1].ToString();
                        this.label41.Text = ds3.Tables[0].Rows[0].ItemArray[2].ToString();
                        this.label42.Text = ds3.Tables[0].Rows[0].ItemArray[3].ToString();
                        this.label44.Text = ds3.Tables[0].Rows[0].ItemArray[4].ToString();
                        tt_longmac = this.label44.Text;
                        setRichtexBox("3、关联表查询到一条数据，goon");

                    }
                    else
                    {
                        setRichtexBox("3、关联表没有查询到数据，over");
                        PutLableInfor("关联表没有查询到数据，请检查！");
                    }

                }



                //第五步查询macinfo表信息
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
                {
                    string tt_sql5 = "select ssid from odc_macinfo " +
                                     "where taskscode = '" + tt_task + "' and mac = '" + tt_longmac + "' ";

                    DataSet ds5 = Dataset1.GetDataSet(tt_sql5, tt_conn);
                    if (ds5.Tables.Count > 0 && ds5.Tables[0].Rows.Count > 0)
                    {
                        tt_flag5 = true;
                        this.label80.Text = ds5.Tables[0].Rows[0].ItemArray[0].ToString();  //SSID
                        setRichtexBox("5、Macinfo表找到一条数据,goon");

                    }
                    else
                    {
                        setRichtexBox("5、Macinfo表没有找到一条数据，over");
                        PutLableInfor("Macinfo表没有找到数据，请检查！");
                    }
                }






                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
                {

                    GetParaDataPrint(0);

                    GetProductNumInfo();
                    CheckStation(tt_shortmac);
                    this.richTextBox1.BackColor = Color.Chartreuse;
                    setRichtexBox("6、查询完毕，可以重打标签或修改模板，over");
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

       

        //MAC过站扫描扫描
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                //---开始MAC扫描
                ScanDataInitial();
                setRichtexBox("-----开始单板扫描--------");
                string tt_scanpcba = this.textBox2.Text.Trim();
                string tt_task = this.textBox1.Text.Trim();
                string tt_uplips = this.textBox8.Text.Trim();
                string tt_downlips = this.textBox9.Text.Trim();
                string tt_tin = this.textBox10.Text.Trim();
                string tt_id = "0";


                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh(tt_scanpcba, this.textBox4.Text);


                //第二步包含符判断
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    tt_flag2 = CheckStrContain(tt_scanpcba, this.textBox5.Text.Trim());
                }


                //第三步 检查模板
                Boolean tt_flag3 = false;
                if (tt_flag1 && tt_flag2   )
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
                if (tt_flag1 && tt_flag2 && tt_flag3 )
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



                //第五步物料检查
                Boolean tt_flag5 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 )
                {
                    if (textBox8.Text.Equals("") || textBox9.Text.Equals("") || textBox10.Text.Equals(""))
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


                //第六步流程检查
                Boolean tt_flag6 = false;
                string tt_gyid = this.label79.Text;
                string tt_ccode = this.label76.Text;
                string tt_ncode = this.label77.Text;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5  )
                {
                    if (tt_ccode == "" || tt_ncode == "")
                    {
                        setRichtexBox("6、该工单没有配置流程," + tt_ccode + "," + tt_ncode + ",over");
                        PutLableInfor("没有获取到当前待测站位，及下一站位，请检查");
                    }
                    else
                    {
                        tt_flag6 = true;
                        setRichtexBox("6、该工单已配置流程," + this.label35.Text + "," + this.label36.Text + ",goon");
                    }



                }


                //第七步查找关联表数据
                Boolean tt_flag7 = false;
                string tt_hostlable = "";
                string tt_shortmac = "";
                string tt_smtaskscode = "";
                string tt_longmac = "";
                string tt_oldtype = "";
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 )
                {
                    string tt_sql7 = "select hostlable,maclable,smtaskscode,bprintuser,id,ageing from odc_alllable " +
                                     "where hprintman = '" + this.textBox1.Text + "' and pcbasn = '" + tt_scanpcba + "' ";

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
                            setRichtexBox("8、该单板待测站位不在" + tt_code + "，站位：" + tt_array8[1] + "，" + tt_array8[2] + ",不可以过站 goon");
                            PutLableInfor("该单板当前站位：" + tt_array8[2] + "不在" + tt_code + "站位！");
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
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8)
                {
                   
                    string tt_sql9 = "select ssid from odc_macinfo " +
                                     "where taskscode = '" + tt_task + "' and mac = '"+tt_longmac+"' ";

                    DataSet ds9 = Dataset1.GetDataSet(tt_sql9, tt_conn);
                    if (ds9.Tables.Count > 0 && ds9.Tables[0].Rows.Count > 0)
                    {
                        tt_flag9 = true;
                        tt_ssid = ds9.Tables[0].Rows[0].ItemArray[0].ToString();  //SSID


                        setRichtexBox("9、Macinfo表找到一条数据，SSID=" + tt_ssid +  ",goon");

                    }
                    else
                    {
                        setRichtexBox("9、Macinfo表没有找到一条数据，over");
                        PutLableInfor("Macinfo表没有找到条数据，请检查！");
                    }


                }






                //第十步物料追溯添加
                Boolean tt_flag10 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8  && tt_flag9)
                {
                    Boolean tt_idinfo = GetMaterialIdinfor(tt_id);

                    if (tt_idinfo)
                    {
                        string tt_insert = "insert into odc_traceback(fid,fmpdate,Fsegment1,Fsegment2,Fsegment3,Ftaskcode,Fpcba,Fhostlable,Fmaclable) " +
                                           "values(" + tt_id + ",getdate(),'" + tt_uplip + "','" + tt_downlip + "','" + tt_tin + "','"
                                            + tt_task + "','" + tt_scanpcba + "','" + tt_hostlable + "','" + tt_shortmac + "')";

                        int tt_int1 = Dataset1.ExecCommand(tt_insert, tt_conn);

                        if (tt_int1 > 0)
                        {
                            tt_flag10 = true;
                            setRichtexBox("10、物料追溯已成功追加到物料表odc_traceback，id号：" + tt_id + ",goon");
                        }
                        else
                        {
                            setRichtexBox("10、物料追溯没有成功追加物料表！,over");
                            PutLableInfor("物料追溯没有成功追加物料表!请继续扫描！");
                        }


                    }
                    else
                    {
                        string tt_update = "update odc_traceback set Fsegment1='" + tt_uplip + "',Fsegment2='" + tt_downlip + "',Fsegment3='" + tt_tin + "' " +
                                           "where Fid = " + tt_id;
                        int tt_int2 = Dataset1.ExecCommand(tt_update, tt_conn);

                        if (tt_int2 > 0)
                        {
                            tt_flag10 = true;
                            setRichtexBox("10、物料追溯已成功更新到物料表odc_traceback，id号：" + tt_id + ",goon");
                        }
                        else
                        {
                            setRichtexBox("10、物料追溯没有成功更新到物料表！,over");
                            PutLableInfor("物料追溯没有成功更新到物料表!请继续扫描！");
                        }

                    }


                }









                





                //第十一步老化判断
                Boolean tt_flag11 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8  && tt_flag9 && tt_flag10 )
                {
                    if (this.label67.Text == "HUAWEI")
                    {
                        //华为产品需要进行老化判断
                        if (this.label64.Text == "0")  //全部老化
                        {
                                                if (tt_oldtype == "1")
                                                {
                                                    tt_flag11 = true;
                                                    setRichtexBox("11、该产品已老化，goon");
                                                }
                                                else
                                                {
                                                    setRichtexBox("11、该产品还没有老化，需要先老化，over");
                                                    PutLableInfor("该产品还没有老化，需要先老化！");
                                                }

                        }
                        else
                        {
                                //部分老化,允许数量判断
                                if (int.Parse(this.label71.Text) < int.Parse(this.label64.Text)) //允许老化数量未到可不老化数量
                                {
                                                if (tt_oldtype == "1")
                                                {
                                                    tt_flag11 = true;
                                                    setRichtexBox("11、该产品已老化，goon");
                                                }
                                                else
                                                {
                                                    tt_flag11 = true;
                                                    tt_oldtype = "0";
                                                    setRichtexBox("11、该产品没有老化，但可以允许不老化，goon");
                                                }


                                }
                                else  //允许不老化数量超过了可不老化数量
                                {
                                                if (tt_oldtype == "1")
                                                {
                                                    tt_flag11 = true;
                                                    setRichtexBox("11、该产品已老化，goon");
                                                }
                                                else
                                                {
                                                    setRichtexBox("11、允许老化已够数量，该产品没有老化，需要先进行老化，over");
                                                    PutLableInfor("该产品还没有老化，需要先老化！");
                                                }


                                }


                        }

                    }
                    else
                    {
                        //不是华为产品就不用判断
                        tt_flag11 = true;
                        setRichtexBox("11、不是要判别的产品不需老化判断！,goon");
                    }
                }


     




                






                //第十二步开始过站

                Boolean tt_flag12 = false;
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8  && tt_flag10  && tt_flag11)
                {
                    string tt_username = STR;
                    tt_flag12 = Dataset1.FhMacPassStation(tt_task, tt_username, tt_shortmac, tt_gyid, tt_code, tt_ncode, tt_oldtype, tt_conn);
                    if (tt_flag12)
                    {
                        setRichtexBox("12、单板过站成功，请继续扫描,ok");
                    }
                    else
                    {
                        setRichtexBox("12、单板关联不成功，事务已回滚");
                        PutLableInfor("单板过站不成功，请检查或再次扫描！");
                    }

                }






                //最后判断
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6 && tt_flag7 && tt_flag8 && tt_flag10 && tt_flag11 &&　tt_flag12)
                {

                    this.label39.Text = tt_scanpcba;  //单板号
                    this.label40.Text = tt_hostlable;  //主机条码
                    this.label41.Text = tt_shortmac;    //短MAC
                    this.label42.Text = tt_smtaskscode;  //移动串号
                    this.label44.Text = tt_longmac;        //长MAC
                    this.label80.Text = tt_ssid;        //长MAC

                    //扣数
                    tt_uplip--;  //上盖数量
                    tt_downlip--; //下盖数量
                    this.label55.Text = tt_uplip.ToString();
                    this.label56.Text = tt_downlip.ToString();

                    //生产节拍
                    getProductRhythm();

                    //打印
                    GetParaDataPrint(1);
                    GetProductNumInfo();
                    GetRealOldNumber();  
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


        #region 5、辅助功能
        //获取生产信息
        private void GetProductNumInfo()
        {
            string tt_sql = "select  count(1),count(case when mprintman is not null then 1 end),0 "+
                            "from odc_alllable  where taskscode = '"+this.textBox1.Text+"' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label46.Text = tt_array[0];
            this.label47.Text = tt_array[1];
        }


        //计算应老化数量
        private void GetOldNumber(string tt_taskquantity,string tt_lhratio)
        {
            int tt_int1 = 0;
            int tt_int2 = 0;

            try
            {
                 tt_int1 = int.Parse(tt_taskquantity);
                 tt_int2 = int.Parse(tt_lhratio);
            }
            catch
            {
                MessageBox.Show("工单数量老化比例转换失败，请检查工单数量与老化比例的值");
            }

            int tt_shouldold = (int)Math.Ceiling(Convert.ToDouble(tt_int1) * Convert.ToDouble(tt_int2) / 100);
            int tt_notold = tt_int1 - tt_shouldold;
            this.label63.Text = tt_shouldold.ToString();
            this.label64.Text = tt_notold.ToString();

        }



        //实际老化数量计算
        private void GetRealOldNumber()
        {
            string tt_sql = "select SUM(case when AGEING = '1' then 1 else 0 end) as N01, " +
                                    "SUM(case when AGEING = '0' then 1 else 0 end) as N02, " +
                                   "SUM(case when AGEING is null then 1 else 0 end) as N03 " +
                            "from ODC_ALLLABLE where TASKSCODE = '"+this.textBox1.Text+"' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            this.label65.Text = tt_array[0];
            this.label71.Text = tt_array[1];
            this.label70.Text = tt_array[2];

        }





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
                this.label85.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString();  //当前站位
            }

        }


        //判断物料表ID值
        private bool GetMaterialIdinfor( string tt_id)
        {
            Boolean tt_flag = false;

            string tt_sql = "select COUNT(1),0,0 from odc_traceback where Fid = " + tt_id ;
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
            this.label7.Text = tt_yield.ToString();   //本班产量
            this.label8.Text = tt_time;               //生产时间
            this.label9.Text = tt_avgtime.ToString();  //平均节拍
            this.label10.Text = tt_differtime2;        //实时节拍

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
                    tt_code = tt_ccode;
                    this.label13.Text = "站位:" + tt_code;
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


        #region 6、数据采集及模板打印
        //获取参数
        private void GetParaDataPrint(int tt_itemtype)
        {
            string tt_fdata = this.label32.Text;

            //mp01---数据类型一
            if (tt_fdata == "MP01")
            {
                GetParaDataPrint_MP01(tt_itemtype);
            }

            //mp01---数据类型一
            if (tt_fdata == "MC01")
            {
                GetParaDataPrint_MC01(tt_itemtype);
            }

        }




        

        //获取参数信息及打印
        private void GetParameter()
        {
            DataSet dst = new DataSet();
            DataTable dt = new DataTable();
            dst.Tables.Add(dt);
            dt.Columns.Add("参数");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");


            DataRow row1 = dt.NewRow();
            row1["参数"] = "P01";
            row1["名称"] = "产品型号";
            row1["内容"] = this.label29.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "P01";
            row2["名称"] = "主机条码";
            row2["内容"] = this.label40.Text;
            dt.Rows.Add(row2);



            DataRow row3 = dt.NewRow();
            row3["参数"] = "P01";
            row3["名称"] = "MAC";
            row3["内容"] = this.label41.Text;
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "P01";
            row4["名称"] = "移动号码";
            row4["内容"] = this.label42.Text;
            dt.Rows.Add(row4);

            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 60;
            this.dataGridView2.Columns[1].Width = 80;
            this.dataGridView2.Columns[2].Width = 200;

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
            row1["名称"] = "对外型号";
            row1["内容"] = this.label30.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "主机条码";
            row2["内容"] = this.label40.Text;
            dt.Rows.Add(row2);



            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "MAC1";
            row3["内容"] = this.label44.Text.Substring(0,17);
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "移动号码";
            row4["内容"] = this.label42.Text;
            dt.Rows.Add(row4);

            DataRow row5 = dt.NewRow();
            row5["参数"] = "S05";
            row5["名称"] = "MAC2";
            row5["内容"] = this.label44.Text;
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


        //----以下是MC01数据采集----
        private void GetParaDataPrint_MC01(int tt_itemtype)
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
            row1["名称"] = "WAN-MAC短";
            row1["内容"] = this.label41.Text;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();
            row2["参数"] = "S02";
            row2["名称"] = "WAN-MAC长";
            row2["内容"] = this.label44.Text;
            dt.Rows.Add(row2);



            DataRow row3 = dt.NewRow();
            row3["参数"] = "S03";
            row3["名称"] = "ONU-MAC短";
            row3["内容"] = this.label41.Text;
            dt.Rows.Add(row3);


            DataRow row4 = dt.NewRow();
            row4["参数"] = "S04";
            row4["名称"] = "ONU-MAC长";
            row4["内容"] = this.label44.Text;
            dt.Rows.Add(row4);

           


            this.dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();

            this.dataGridView2.DataSource = dst.Tables[0];
            this.dataGridView2.Update();

            this.dataGridView2.Columns[0].Width = 50;
            this.dataGridView2.Columns[1].Width = 90;
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


        #region 7、按钮事件

        //重置按钮
        private void button1_Click(object sender, EventArgs e)
        {
            ScanDataInitial();
            this.textBox2.Text = null;
            this.textBox3.Text = null;
            textBox2.Focus();
            textBox2.SelectAll();
        }

        //模板预览
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView2.RowCount > 0)
            {

                string tt_prientcode = this.label85.Text;
                string tt_checkcode = this.label76.Text;

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

                  

                    string tt_prientcode = this.label85.Text;
                    string tt_checkcode = this.label76.Text;

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

            textBox3.Focus();
            textBox3.SelectAll();
        }
        #endregion


        #region 8、物料追溯
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

        //物料信息锁定
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked)
            {
                this.textBox8.ReadOnly = true;
                this.textBox9.ReadOnly = true;
                this.textBox10.ReadOnly = true;

            }
            else
            {
                this.textBox8.ReadOnly = false;
                this.textBox9.ReadOnly = false;
                this.textBox10.ReadOnly = false;
            }
        }



        #endregion


        #region 9、SN条码查询
        //重置
        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox11.Text = null;
            this.dataGridView3.DataSource = null;
            this.dataGridView4.DataSource = null;
            this.dataGridView5.DataSource = null;
        }
        
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
            if (tt_flag )
            {
                string tt_sql2 = "select ccode 前站 ,Ncode 后站,napplytype 过站,taskscode 工单,pcba_pn MAC, createtime 进站时间, enddate 出站时间, fremark 备注  " +
                            " from ODC_ROUTINGTASKLIST    where pcba_pn = '" + tt_mac + "' order by createtime desc";

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


        #endregion




    }
}
