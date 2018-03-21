using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVBOX01
{
    public partial class Form20_abd : Form
    {
        public Form20_abd()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_conn;

        private void Form20_abd_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";

            ClearLabelInfo2();
            this.radioButton1.Checked = true;
            this.radioButton4.Checked = true;

            this.tabPage2.Parent = null;
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


        #region 2、锁定及数据清除

        //位数锁定
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                this.comboBox1.Enabled = false;
            }
            else
            {
                this.comboBox1.Enabled = true;

            }
        }


        //重置数据初始化
        private void ClearLabelInfo1()
        {
            //当前站位
            this.label6.Text = null;
            this.label8.Text = null;
            this.textBox1.Text = null;
            PutLableInfor("");
            comboBox2.DataSource = null;
            this.dataGridView1.DataSource = null;
            //流程表
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;
            //跳转按钮
            this.button1.Visible = false;
        }

        //进入界面数据初始化
        private void ClearLabelInfo2()
        {
            this.label4.Text = null;
            comboBox2.DataSource = null;
            //当前站位
            this.label6.Text = null;
            this.label8.Text = null;
            this.button1.Visible = false;
        }


        #endregion


        #region 3、辅助功能

        //lable提示信息
        private void PutLableInfor(string tt_lableinfo)
        {
            this.label4.Text = tt_lableinfo;
        }

        //lable提示信息
        private void PutLableInfor2(string tt_lableinfo)
        {
            this.label11.Text = tt_lableinfo;
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


        //richtext加记录
        private void SetRichtexBox(string tt_textinfor)
        {
            this.richTextBox1.Text = this.richTextBox1.Text + tt_textinfor + "\n";
        }

        //richtext加记录
        private void SetRichtexBox2(string tt_textinfor)
        {
            this.richTextBox2.Text = this.richTextBox2.Text + tt_textinfor + "\n";
        }


        //位数判断方法
        private Boolean CheckStrLengh(string tt_checkstr, string tt_lengthtext)
        {
            Boolean tt_flag = false;

            int tt_snlength = int.Parse(tt_lengthtext);
            if (tt_checkstr.Length == tt_snlength)
            {
                tt_flag = true;
                SetRichtexBox("1、位数判断正确，" + tt_snlength.ToString() + "位，goon");
            }
            else
            {
                SetRichtexBox("1、位数判断不正确，不是" + tt_snlength.ToString() + "位,实际为：" + tt_checkstr.Length.ToString());
                PutLableInfor("位数判断不正确，不是" + tt_snlength.ToString() + "位,实际为：" + tt_checkstr.Length.ToString());
            }


            return tt_flag;
        }


        //位数判断方法
        private Boolean CheckStrLengh2(string tt_checkstr, string tt_lengthtext)
        {
            Boolean tt_flag = false;

            int tt_snlength = int.Parse(tt_lengthtext);
            if (tt_checkstr.Length == tt_snlength)
            {
                tt_flag = true;
            }
            else
            {
                PutLableInfor("位数判断不正确，不是" + tt_snlength.ToString() + "位,实际为：" + tt_checkstr.Length.ToString());
            }


            return tt_flag;
        }


        //位数判断方法
        private Boolean CheckStrLengh3(string tt_checkstr, string tt_lengthtext)
        {
            Boolean tt_flag = false;

            int tt_snlength = int.Parse(tt_lengthtext);
            if (tt_checkstr.Length == tt_snlength)
            {
                tt_flag = true;
                SetRichtexBox2("1、位数判断正确，" + tt_snlength.ToString() + "位，goon");
            }
            else
            {
                SetRichtexBox2("1、位数判断不正确，不是" + tt_snlength.ToString() + "位,实际为：" + tt_checkstr.Length.ToString());
                PutLableInfor2("位数判断不正确，不是" + tt_snlength.ToString() + "位,实际为：" + tt_checkstr.Length.ToString());
            }


            return tt_flag;
        }

        //数字转换
        private int GetStringToInt(string tt_str)
        {
            int tt_int = 0;
            if ( tt_str == "")
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
                    MessageBox.Show("字符串站位转换为数字失败!" + tt_str);
                }
            }

            return tt_int;
        }

        #endregion


        #region 4、数据功能

        //查询站位显示站位
        private void GetMacStation(string tt_mac)
        {
            this.dataGridView1.DataSource = null;
            string tt_sql3 = "select ID,taskscode 工单号, pcba_pn MAC, ccode 前站位, Createtime 创建时间, " +
                                "Ncode 当前站位,Napplytype 状态, Enddate 完成时间, Fremark 备注 " +

                        "from ODC_ROUTINGTASKLIST " +
                        "where pcba_pn =  '" + tt_mac + "' " +
                        "order by id desc ";

            DataSet ds3 = Dataset1.GetDataSet(tt_sql3, tt_conn);
            if (ds3.Tables.Count > 0 && ds3.Tables[0].Rows.Count > 0)
            {
                dataGridView1.DataSource = ds3;
                dataGridView1.DataMember = "Table";
            }
            else
            {

                PutLableInfor("没有查询到数据，请检查！");
            }

        }

        //获取3350前的站位
        private string GetFixBeforStation( string tt_mac,string tt_process, int tt_productname_check)
        {
            string tt_fixbefor = "0";
            string tt_sql = "select id,Ncode from ODC_ROUTINGTASKLIST " +
                             "where pcba_pn = '"+tt_mac+"' and Napplytype is not null " +
                              "order by id desc ";

            DataSet ds = Dataset1.GetDataSet(tt_sql, tt_conn);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tt_fixbefor = ds.Tables[0].Rows[i][1].ToString();
                    if (tt_process.Contains(tt_fixbefor) || (tt_productname_check ==1 && tt_fixbefor == "2111")) break;                   
                }
            }
            return tt_fixbefor;
        }

        //获取站位顺序号
        private string GetNcodeSerialNo(string tt_gyid,string tt_code)
        {
            string tt_routnum = "0";

            string tt_sql = "select count(1),min(lcbz),0 from odc_routing " +
                           "where pid = " + tt_gyid + "  and pxid = " + tt_code;

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);

            if (tt_array[0] == "1")
            {
                tt_routnum = tt_array[1];
            }
            return tt_routnum;
        }

        //获取可跳站位
        private string GetNcode(string tt_code)
        {
            string tt_routnum = "0";

            string tt_sql = "select ncode from odc_routing_change where ccode = '" + tt_code + "'";

            DataSet ds = Dataset1.GetDataSet(tt_sql, tt_conn);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                tt_routnum = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            }

            return tt_routnum;
        }

        //获取产品型号
        private string Getproductname(string tt_mac)
        {
            string tt_productname = "";
            string tt_sql = "select product_name from ODC_TASKS " + 
                            "where taskscode in " +
                            "(select taskscode from odc_alllable where maclable = '"+ tt_mac +"')";

            DataSet ds = Dataset1.GetDataSet(tt_sql, tt_conn);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                tt_productname = ds.Tables[0].Rows[0][0].ToString();
            }
            return tt_productname;
        }

        //获取产品地区
        private string Getproductarea(string tt_mac)
        {
            string tt_productname = "";
            string tt_sql = "select areacode from ODC_TASKS " +
                            "where taskscode in " +
                            "(select taskscode from odc_alllable where maclable = '" + tt_mac + "')";

            DataSet ds = Dataset1.GetDataSet(tt_sql, tt_conn);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                tt_productname = ds.Tables[0].Rows[0][0].ToString();
            }
            return tt_productname;
        }

        //修复无1920问题
        private int Change_ccode1920(string tt_mac, string tt_id)
        {
            string tt_Changesql = "update odc_routingtasklist set ccode = '1920' " +
                                  "where pcba_pn = '" + tt_mac + "' and ncode = '2100' and id = '" + tt_id + "' ";
            int tt_Checknum = Dataset1.ExecCommand(tt_Changesql, tt_conn);
            return tt_Checknum;
        }

        #endregion


        #region 5、功能按钮
        //站位刷新
        private void Button4_Click(object sender, EventArgs e)
        {
            GetNowMacSattion();
        }


        //确定按钮
        private void Button5_Click(object sender, EventArgs e)
        {
            GetBeforStation();

        }


        //重置
        private void Button2_Click(object sender, EventArgs e)
        {
            ClearLabelInfo1();
        }

        //异常处理
        private void Button3_Click(object sender, EventArgs e)
        {
            //数据准备
            string tt_scan = this.textBox1.Text.Trim().ToUpper();
            string tt_scansn = GetShortMac(tt_scan);


            //第一步 条码确定
            #region
            bool tt_flag1 = false;
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确定要对条码:"+tt_scansn+",进行处理吗？这个处理主要是出来没有待测站位\\有多个待测站位\\没有1920站位的情况，请确认你输入的是单板号，还是MAC", "异常处理", messButton);
            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {
                tt_flag1 = true;
            }
            #endregion


            //第二步 确定MAC
            #region
            bool tt_flag2 = false;
            string tt_shortmac = "";
            if (tt_flag1)
            {
                //选择的是MAC
                if (this.radioButton1.Checked == true)
                {
                    tt_flag2 = true;
                    tt_shortmac = tt_scansn;
                }


                //选择的是单板号
                if (this.radioButton2.Checked == true)
                {
                    string tt_sql2 = "select count(1),min(maclable),min(taskscode)  from odc_alllable where pcbasn = '" + tt_scansn + "' ";

                    string[] tt_array2 = new string[3];
                    tt_array2 = Dataset1.GetDatasetArray(tt_sql2, tt_conn);
                    if (tt_array2[0] == "1")
                    {
                        tt_flag2 = true;
                        tt_shortmac = tt_array2[1];
                    }
                    else
                    {
                        MessageBox.Show("根据你输入的单板号:"+tt_scansn+",无法在关联表中找到对应的MAC");
                    }
                }


            }
            #endregion
            

            //第三步 确定单板号
            #region
            bool tt_flag3 = false;
            string tt_pcba = "";
            if(tt_flag2)
            {
                //选择的单板号
                if (this.radioButton2.Checked == true)
                {
                    tt_flag3 = true;
                    tt_pcba = tt_scansn;
                }

                //选择的是MAC
                if (this.radioButton1.Checked == true)
                {
                    string tt_sql3 = "select count(1),min(pcbasn),min(taskscode)  from odc_alllable where maclable = '" + tt_scansn + "' ";

                    string[] tt_array3 = new string[3];
                    tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                    if (tt_array3[0] == "1")
                    {
                        tt_flag3 = true;
                        tt_pcba = tt_array3[1];
                    }
                    else
                    {
                        MessageBox.Show("根据你输入的MAC:" + tt_scansn + ",无法在关联表中找到对应的单板号");
                    }
                }

            }

            #endregion


            //第四步 确定待测站位个数/是否缺失1920站位
            #region
            int tt_intrownum = 0;
            string tt_nullmaxid = "";
            string tt_nullminid = "";
            bool tt_flag4 = false;
            bool tt_flag4_1 = false;
            if (tt_flag3)
            {
                string tt_sql4 = "select count(1),max(id),min(id) from odc_routingtasklist " +
                                    "where  pcba_pn = '" + tt_shortmac + "' and napplytype is NULL ";

                string tt_sql4_1 = "select id,ncode,ccode from odc_routingtasklist " +
                                   "where pcba_pn  = '" + tt_shortmac + "' and ncode = '2100' and napplytype = '1' order by id";

                string tt_sql4_2 = "select count(1),max(id),min(id) from odc_routingtasklist " +
                                   "where  pcba_pn = '" + tt_shortmac + "' and ncode = '2100' and napplytype = '1'";

                DataSet tt_dataset1 = Dataset2.getMacAllCodeInfo(tt_shortmac, tt_conn);
                int tt_int1920id = Dataset2.getFirstCodeId(tt_dataset1);

                string[] tt_array4 = new string[3];
                tt_array4 = Dataset1.GetDatasetArray(tt_sql4, tt_conn);

                string[] tt_array4_2 = new string[3];
                tt_array4_2 = Dataset1.GetDatasetArray(tt_sql4_2, tt_conn);

                DataSet ds4_1 = Dataset1.GetDataSet(tt_sql4_1, tt_conn);
                if (ds4_1.Tables.Count > 0 && ds4_1.Tables[0].Rows.Count > 0 && tt_int1920id == -2)
                {
                    string tt_id_1 = ds4_1.Tables[0].Rows[0].ItemArray[0].ToString();
                    string tt_ccode_1 = ds4_1.Tables[0].Rows[0].ItemArray[1].ToString();
                    string tt_ncode_1 = ds4_1.Tables[0].Rows[0].ItemArray[2].ToString();

                    if(int.Parse(tt_ccode_1) > 1920 && tt_id_1 == tt_array4_2[2])
                    {
                        tt_flag4_1 = true;
                    }

                }

                if (tt_array4[0] == "1" && tt_flag4_1 == false)
                {
                    MessageBox.Show("该MAC:" + tt_shortmac + ",只有一个待测站位，没有异常，不需要处理");
                }
                else if (tt_flag4_1)
                {
                    int chang_ccode1920 = Change_ccode1920(tt_shortmac, tt_array4_2[2]);
                    MessageBox.Show("该MAC:" + tt_shortmac + ",无1920站位的问题已处理完毕");
                }
                else
                {
                    tt_flag4 = true;
                    tt_intrownum = GetStringToInt(tt_array4[0]);
                    tt_nullmaxid = tt_array4[1];
                    tt_nullminid = tt_array4[2];
                }
            }
            #endregion
            

            //第五步 确定异常问题
            #region
            bool tt_flag5 = false; //没有待测站位
            bool tt_flag6 = false; //多个待测站位
            if (tt_flag4)
            {
                if (tt_intrownum == 0)
                {
                    tt_flag5 = true;
                }
                else
                {
                    if (tt_intrownum > 1) tt_flag6 = true;
                }
            }
            #endregion
            

            //第五步 没有待测站位处理
            #region
            if (tt_flag5)
            {
                string tt_rowmaxid = "";
                string tt_sql5 = "select count(1),max(id),0 from odc_routingtasklist " +
                                    "where  pcba_pn = '" + tt_shortmac + "' ";

                string[] tt_array5 = new string[3];
                tt_array5 = Dataset1.GetDatasetArray(tt_sql5, tt_conn);

                //查找最大的ID值
                bool tt_flag51 = false;
                if (tt_array5[0] == "0")
                {
                    MessageBox.Show("MAC:" + tt_shortmac + ",在站位表中没有找到记录");
                }
                else
                {
                    tt_flag51 = true;
                    tt_rowmaxid = tt_array5[1];
                }

                if (tt_flag51)
                {
                    string tt_sql51 = "update odc_routingtasklist set napplytype = NULL " +
                                      "where pcba_pn = '" + tt_shortmac + "' and id = " + tt_rowmaxid;

                    int tt_intexec = Dataset1.ExecCommand(tt_sql51, tt_conn);
                    if (tt_intexec > 0)
                    {
                        MessageBox.Show("---OK---,操作1成功，该MAC:"+tt_shortmac+",没有待测站位，现在已给出一个待测站位，请再次查询");
                    }
                    else
                    {
                        MessageBox.Show("---Fail---,操作1失败，该MAC:" + tt_shortmac + "，没有待测站位，现在还是没有给出待测站位，请再次检查");
                    }
                }



            }
            #endregion


            //第六步 有多个待测站位,确定处理方法
            #region
            bool tt_flag7 = false;
            bool tt_flag8 = false;
            string tt_maxid2 = "";
            if(tt_flag6)
            {
                string tt_sql6 = "select count(1),max(id),0 from odc_routingtasklist " +
                                    "where  pcba_pn = '" + tt_shortmac + "' ";

                string[] tt_array6 = new string[3];
                tt_array6 = Dataset1.GetDatasetArray(tt_sql6, tt_conn);


                if (tt_array6[0] == "0")
                {
                    MessageBox.Show("程序逻辑有问题，这是查找有多个待测站位的最大ID值，但是MAC:" + tt_shortmac + ",在站位表中没有发现有多个待测站位");
                }
                else
                {
                    tt_maxid2 = tt_array6[1];
                    if (tt_nullmaxid == tt_maxid2 )  //MAC的最大ID值和多个待测站位的最大ID值一致
                    {
                        tt_flag7 = true;
                        
                    }
                    else
                    {
                        tt_flag8 = true;
                    }
                }
            }
            #endregion
            

            //第七步 有多个待测值 最大待测ID值和MAC的最大ID值一致（正常情况）
            #region
            if(tt_flag7)
            {
                string tt_sql7 = "update odc_routingtasklist set napplytype = 1 " +
                       "where pcba_pn = '" + tt_shortmac + "' and napplytype is NULL and ID <" + tt_nullmaxid + " and id>=" + tt_nullminid;

                int tt_intexec7 = Dataset1.ExecCommand(tt_sql7, tt_conn);
                if (tt_intexec7 > 0)
                {
                    MessageBox.Show("---OK---,操作2成功，该MAC:" + tt_shortmac + ",有多个待测站位，最大ID值就是待测站位，现在已只有一个待测站位，请再次查询");
                }
                else
                {
                    MessageBox.Show("---Fail---,操作2失败，该MAC:" + tt_shortmac + "，有多个待测站位，最大ID值就是待测站位，现在还是没有给出一个待测站位，请再次检查");
                }


            }
            #endregion
            

            //第八步 有多个待测值 最大待测ID值和MAC的最大ID值不一致（非正常情况）
            #region
            if (tt_flag8)
            {
                //第8.1步 将最大ID值的设置为待测站位
                string tt_sql81 = "update odc_routingtasklist set napplytype = NULL " +
                                      "where pcba_pn = '" + tt_shortmac + "' and id = " + tt_maxid2;

                int tt_intexec81 = Dataset1.ExecCommand(tt_sql81, tt_conn);

                //第8.2步 将其他的待测站位取消
                string tt_sql82 = "update odc_routingtasklist set napplytype = 1 " +
                       "where pcba_pn = '" + tt_shortmac + "' and napplytype is NULL and ID <=" + tt_nullmaxid + " and id>=" + tt_nullminid;

                int tt_intexec82 = Dataset1.ExecCommand(tt_sql82, tt_conn);

                //第8.3步 判断
                if (tt_intexec81 > 0 && tt_intexec82 > 0)
                {
                    MessageBox.Show("---OK---,操作3成功，该MAC:" + tt_shortmac + ",有多个待测站位，最大ID值不是待测站位，现在已只有一个待测站位，请再次查询");
                }
                else
                {
                    MessageBox.Show("---Fail---,操作4失败，该MAC:" + tt_shortmac + "，有多个待测站位，最大ID值不是待测站位，现在还是没有给出一个待测站位，请再次检查");
                }

            }
            #endregion

        }

        //站位跳转
        private void Button1_Click(object sender, EventArgs e)
        {
            //第一步 查看是否选择站位
            #region
            string tt_skipcode = "";
            tt_skipcode = this.comboBox2.Text;
            Boolean tt_flag1 = false;
            if (tt_skipcode != "")
            {
                tt_flag1 = true;
            }
            else
            {
                MessageBox.Show("没有选择要跳转的站位，请确认！");
            }
            #endregion

            //第二步 查看是否有ID号
            #region
            string tt_rowid = this.label8.Text;
            string tt_nowcode = this.label6.Text;;
            Boolean tt_flag2 = false;
            if (tt_flag1)
            {
                if (tt_rowid == "" || tt_nowcode == "")
                {
                    MessageBox.Show("没有找到当前站位ID号以及当前站位，请确认！");
                }
                else
                {
                    tt_flag2 = true;
                }
            }
            #endregion

            //第三步 跳转提示判断
            #region
            Boolean tt_flag3 = false;
            if( tt_flag2)
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("确定将当前站位：" + tt_nowcode + "，跳转到：" + tt_skipcode+",跳过去就跳不回来了", "站位跳转", messButton);

                if (dr == DialogResult.OK)//如果点击“确定”按钮
                {
                    tt_flag3 = true;

                }
                else
                {

                }
            }
            #endregion            

            //第四步 开始跳转
            #region
            Boolean tt_flag4 = false;
            if(tt_flag3)
            {
                tt_flag4 = Dataset1.FhCodeSkip(tt_skipcode, tt_rowid, tt_conn);
                if (tt_flag4)
                {
                    MessageBox.Show("跳站成功，请再次查询确认");
                    ClearLabelInfo2();
                }
                else
                {
                    MessageBox.Show("跳站不成功，请确认！");
                }
            }
            #endregion
        }


        #endregion


        #region 6、扫描功能
        //SN扫描
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GetNowMacSattion();
                if(this.checkBox3.Checked)   GetBeforStation(); //勾选后才做2合1操作
            }
        }

        #endregion


        #region 7、主要功能
        //获取前一站位
        private void GetBeforStation()
        {
            //数据初始化
            #region
            PutLableInfor("");
            comboBox2.DataSource = null;
            this.dataGridView1.DataSource = null;
            this.richTextBox1.Text = null;
            this.richTextBox1.BackColor = Color.White;
            string tt_scan1 = this.textBox1.Text.Trim();
            string tt_scan2 = tt_scan1.Replace("-", "");
            string tt_sacn3 = tt_scan2.Replace(":", "");
            string tt_scansn = tt_sacn3.Replace(" ", "");
            //扫描数据
            SetRichtexBox("----开始过站操作----");
            SetRichtexBox("扫描数据为：" + tt_scansn);
            #endregion

            
            //第一步位数判断
            #region
            Boolean tt_flag1 = false;
            tt_flag1 = CheckStrLengh(tt_scan1, this.comboBox1.Text);
            #endregion


            //第二步获取MAC号
            #region
            string tt_mac = "";
            string tt_taskcode = "";
            Boolean tt_flag2 = false;
            if (tt_flag1)
            {
                //选择的是MAC
                if (this.radioButton1.Checked == true)
                {
                    tt_flag2 = true;
                    tt_mac = tt_scansn;
                    SetRichtexBox("2、扫描MAC号为：" + tt_mac + ",goon");
                }

                //选择的是单板号
                if (this.radioButton2.Checked == true)
                {
                    string tt_sql = "select count(1),min(maclable),min(taskscode)  from odc_alllable where pcbasn = '" + tt_scansn + "' ";

                    string[] tt_array = new string[3];
                    tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
                    if (tt_array[0] == "1")
                    {
                        tt_flag2 = true;
                        tt_mac = tt_array[1];
                        tt_taskcode = tt_array[2];
                        SetRichtexBox("2、扫描MAC号为：" + tt_mac + ",工单号为:"+tt_taskcode+",goon");
                    }
                    else
                    {
                        SetRichtexBox("2、该单板在alllable表中没有找到，over");
                        PutLableInfor("该单板在alllable表中没有找到，请确认");
                    }
                }

            }
            #endregion

            
            //第三步获取单板号
            #region
            string tt_pcba = "";
            Boolean tt_flag3 = false;
            if(tt_flag2)
            {
                //选择的单板号
                if (this.radioButton2.Checked == true)
                {
                    tt_flag3 = true;
                    tt_pcba = tt_scansn;
                    SetRichtexBox("3、扫描单板号为：" + tt_pcba+",goon");
                }


                //选择的是MAC
                if (this.radioButton1.Checked == true)
                {
                    string tt_sql3 = "select count(1),min(pcbasn),min(taskscode)  from odc_alllable where maclable = '" + tt_scansn + "' ";

                    string[] tt_array3 = new string[3];
                    tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                    if (tt_array3[0] == "1")
                    {
                        tt_flag3 = true;
                        tt_pcba = tt_array3[1];
                        tt_taskcode = tt_array3[2];
                        SetRichtexBox("3、扫描单板号为：" + tt_pcba + ",工单号为:"+tt_taskcode+",goon");
                    }
                    else
                    {
                        SetRichtexBox("3、该MAC在alllable表中没有找到，over");
                        PutLableInfor("该MAC在alllable表中没有找到，请确认");
                    }
                }


            }
            #endregion

            
            //第四步查看是否已装箱
            #region
            Boolean tt_flag4 = false;
            if(tt_flag3)
            {
                string tt_sql4 = "select count(1),0,0 from odc_package where pasn = '"+tt_pcba+"' ";

                string[] tt_array4 = new string[3];
                tt_array4 = Dataset1.GetDatasetArray(tt_sql4, tt_conn);
                if (tt_array4[0] == "0")
                {
                    tt_flag4 = true;
                    SetRichtexBox("4、该产品还没有装箱" + ",goon");
                }
                else
                {
                    SetRichtexBox("4、该产品还已装箱，over");
                    PutLableInfor("该产品还已装箱，请确认");
                }

            }
            #endregion

            
            //第五步 是否正在维修状态
            #region
            Boolean tt_flag5 = false;
            if(tt_flag4)
            {
                string tt_sql5 = "select count(1),0,0 from repair where MAC='"+tt_mac+"' and type = 1 ";
                string[] tt_array5 = new string[3];
                tt_array5 = Dataset1.GetDatasetArray(tt_sql5, tt_conn);
                if (tt_array5[0] == "0")
                {
                    tt_flag5 = true;
                    SetRichtexBox("5、该产品不在维修状态" + ",goon");
                }
                else
                {
                    SetRichtexBox("5、该产品处于维修状态，over");
                    PutLableInfor("该产品处于维修状态，请确认");
                }
            }
            #endregion


            //第六步 查看工单流程
            #region
            string tt_gyid = "";
            Boolean tt_flag6 = false;
            if(tt_flag5)
            {
                string tt_sql6 = "select count(1),min(gyid),0 from odc_tasks where taskscode = '"+tt_taskcode+"' ";
                string[] tt_array6 = new string[3];
                tt_array6 = Dataset1.GetDatasetArray(tt_sql6, tt_conn);
                if (tt_array6[0] == "1")
                {
                    tt_flag6 = true;
                    tt_gyid = tt_array6[1];
                    SetRichtexBox("6、该产品流程配置为" + tt_gyid + ",goon");
                }
                else
                {
                    SetRichtexBox("6、没有找到该工单的流程配置，"+tt_taskcode+"over");
                    PutLableInfor("没有找到该工单的流程配置，请确认!" + tt_taskcode);
                }
            }
            #endregion

            
            //第七步查找流程
            #region
            string tt_proname = "";
            string tt_process = "";
            Boolean tt_flag7 = false;
            if(tt_flag6)
            {
                string tt_sql7 = "select count(1),min(name),min(process) from odc_process where id= "+tt_gyid;
                string[] tt_array7 = new string[3];
                tt_array7 = Dataset1.GetDatasetArray(tt_sql7, tt_conn);
                if (tt_array7[0] == "1")
                {
                    tt_flag7 = true;
                    tt_proname = tt_array7[1];
                    tt_process = tt_array7[2];
                    SetRichtexBox("7、该产品流程名称为" + tt_proname + ",流程顺序："+tt_process+",goon");
                }
                else
                {
                    SetRichtexBox("7、没有找到该工单的流程表顺序，");
                    PutLableInfor("没有找到该工单的流程表顺序，请确认!" +tt_taskcode);
                }
            }
            #endregion
            

            //第八步：查找当前站位
            #region
            string tt_ncode = "";
            string tt_riwid = "";
            Boolean tt_flag8 = false;
            if(tt_flag7)
            {
                string tt_sql8 = "select count(1),min(id),min(ncode) from odc_routingtasklist " +
                                     "where  pcba_pn = '" + tt_mac + "' and napplytype is null ";
                string[] tt_array8 = new string[3];
                tt_array8 = Dataset1.GetDatasetArray(tt_sql8, tt_conn);
                if (tt_array8[0] == "1")
                {
                    tt_riwid = tt_array8[1];
                    tt_ncode = tt_array8[2];

                    int tt_productname_check = 0;
                    if ("HG6201M,HG6201T,HG2201T".Contains(Getproductname(tt_mac)))
                    {
                        tt_productname_check = 1;
                    }

                    if (tt_ncode == "2111" && tt_productname_check == 1)
                    {
                        tt_ncode = "2115";
                    }

                    if (tt_ncode == "2115" && tt_productname_check == 1 && ("安徽".Contains(Getproductarea(tt_mac)) == true))
                    {
                        tt_ncode = "2111";
                    }

                    tt_flag8 = true;
                    SetRichtexBox("8、该单板有待测站位，前站位：" + tt_array8[1] + "，当前站位" + tt_array8[2] + ", goon");
                }
                else
                {
                    SetRichtexBox("8、没有找到待测站位，或有多条待测站位，流程异常，over");
                    PutLableInfor("没有找到待测站位，或有多条待测站位，流程异常！");
                }
            }
            #endregion
            

            //第九步 查找流程顺序
            #region
            string tt_prono = "0";
            Boolean tt_flag9 = false;
            if(tt_flag8)
            {
                string tt_sql9 = "select count(1),min(lcbz),0 from odc_routing " +
                            "where pid = " + tt_gyid + "  and pxid = " + tt_ncode;

                string[] tt_array9 = new string[3];
                tt_array9 = Dataset1.GetDatasetArray(tt_sql9, tt_conn);

                if (tt_array9[0] == "1")
                {
                    tt_flag9 = true;
                    tt_prono = tt_array9[1];
                    SetRichtexBox("9、找到一个流程顺序号，"+tt_prono+",goon");
                }
                else
                {
                    if (tt_ncode == "3350")
                    {
                        tt_flag9 = true;
                        SetRichtexBox("9、当前站位为3350，需要往前追溯往，goon");
                    }
                    else
                    {
                        SetRichtexBox("9、该站位：" + tt_ncode + "，没有找到流程顺序号，over");
                        PutLableInfor("没有找到流程顺序号，流程异常请确认！");
                    }
                }
            }
            #endregion
            

            //第十步查找 3350 以前的站位
            #region
            string tt_mataionstation = "";
            Boolean tt_flag10 = false;
            if(tt_flag9)
            {

                if(tt_ncode == "3350")
                {
                    int tt_productname_check = 0;
                    if ("HG6201M,HG6201T,HG2201T".Contains(Getproductname(tt_mac)))
                    {
                        tt_productname_check = 1;
                    }

                    tt_mataionstation = GetFixBeforStation(tt_mac, tt_process, tt_productname_check);

                    if (tt_mataionstation!="0")
                    {
                        tt_flag10 = true;
                        SetRichtexBox("10、找到1个3350以前站位："+tt_mataionstation+",goon");
                    }
                    else
                    {
                        SetRichtexBox("10、没有找到1个3350以前站位：" + tt_mataionstation + ",over");
                        PutLableInfor("没有找到3350以前站位，流程异常请确认！");
                    }

                }
                else
                {
                    tt_flag10 = true;
                    SetRichtexBox("10、该站位：" + tt_ncode + "，不是3350，不用去找3350以前站位，goon");
                }


            }
            #endregion


            //第十一步查询跳转站位最大值
            #region
            string tt_routmaxnum = "";
            Boolean tt_flag11 = false;
            if(tt_flag10)
            {
                if (tt_ncode == "3350")
                {
                    if (tt_mataionstation == "2111" && ("HG6201M,HG6201T,HG2201T".Contains(Getproductname(tt_mac)) == true))
                    {
                        tt_mataionstation = "2115";
                    }

                    if (tt_mataionstation == "2115" && ("HG6201T".Contains(Getproductname(tt_mac)) == true) && ("安徽".Contains(Getproductarea(tt_mac)) == true))
                    {
                        tt_mataionstation = "2111";
                    }

                    tt_routmaxnum = GetNcodeSerialNo(tt_gyid, tt_mataionstation);
                    if (tt_routmaxnum!= "0")
                    {
                        tt_flag11 = true;
                        SetRichtexBox("11、找到1个3350以前站位:" + tt_mataionstation + "顺序号：" + tt_routmaxnum + ",goon");
                    }
                    else
                    {
                        SetRichtexBox("11、没有找到1个3350以前站位:" + tt_mataionstation + "顺序号：" + tt_routmaxnum + ",over");
                        PutLableInfor("没有找到3350以前站位的顺序号，流程异常请确认！");
                    }
                }
                else
                {
                    int tt_rutnum = GetStringToInt(tt_prono)-1;
                    tt_routmaxnum = tt_rutnum.ToString();
                    tt_flag11 = true;
                    SetRichtexBox("11、找到正常站位顺序:" + tt_prono + "的前最大顺序号：" + tt_routmaxnum + ",goon");
                }
            }
            #endregion
            

            //第十二步 开始加载下拉列表
            #region
            Boolean tt_flag12 = false;
            if (tt_flag11)
            {
                //string tt_sql = "select pxid from odc_routing " +
                //                 "where pid = '"+tt_gyid+"' and lcbz >= 2 and lcbz <= " +tt_routmaxnum +
                //                "  order by lcbz desc ";

                //DataSet ds12 = Dataset1.GetDataSet(tt_sql, tt_conn);
                //if (ds12.Tables.Count > 0 && ds12.Tables[0].Rows.Count > 0)
                //{
                //    comboBox2.DataSource = ds12.Tables[0];
                //    comboBox2.DisplayMember = "pxid";
                //    comboBox2.ValueMember = "pxid";
                //    tt_flag12 = true;
                //    setRichtexBox("12、下拉列表加载完毕,goon");                    
                //}

                string tt_ncode_temp = "";

                if (tt_mataionstation != "")
                {
                    tt_ncode_temp = GetNcode(tt_mataionstation);
                }
                else
                {
                    tt_ncode_temp = GetNcode(tt_ncode);
                }

                if (tt_ncode_temp != "0")
                {
                    string[] NCODE_Temp = tt_ncode_temp.Split(',');
                    comboBox2.DataSource = NCODE_Temp;
                    comboBox2.DisplayMember = "";
                    comboBox2.ValueMember = "";
                    tt_flag12 = true;
                    SetRichtexBox("12、下拉列表加载完毕,goon");
                }
                else
                {
                    SetRichtexBox("12、下拉列表加载异常,或没有前站位over");
                    PutLableInfor("下拉列表加载异常！获没有前站位了");
                }
            }
            #endregion


            //最后显示
            #region
            if (tt_flag12)
            {
                this.label8.Text = tt_riwid;
                this.label6.Text = tt_ncode;
                this.button1.Visible = true;
                PutLableInfor("下拉列表加载完毕，可以选择站位跳转！");
                this.richTextBox1.BackColor = Color.Chartreuse;
                GetMacStation(tt_mac);
            }
            else
            {
                this.richTextBox1.BackColor = Color.Red;
                if(tt_flag3)   GetMacStation(tt_mac);

            }
            #endregion

        }


        //查询站位信息
        private void GetNowMacSattion()
        {
            //数据准备
            this.dataGridView1.DataSource = null;
            ClearLabelInfo2();
            string tt_scan1 = this.textBox1.Text.Trim();
            string tt_scan2 = tt_scan1.Replace("-", "");
            string tt_sacn3 = tt_scan2.Replace(":", "");
            string tt_scansn = tt_sacn3.Replace(" ", "");

            //第一步位数判断
            Boolean tt_flag1 = false;
            tt_flag1 = CheckStrLengh2(tt_scan1, this.comboBox1.Text);


            //第二步获取MAC,PCBA号
            string tt_mac = "";
            Boolean tt_flag2 = false;
            if (tt_flag1)
            {
                //选择的是MAC
                if (this.radioButton1.Checked == true)
                {
                    tt_flag2 = true;
                    tt_mac = tt_scansn;
                }

                //选择的是单板号
                if (this.radioButton2.Checked == true)
                {
                    string tt_sql = "select count(1),min(maclable),0  from odc_alllable where pcbasn = '" + tt_scansn + "' ";

                    string[] tt_array = new string[3];
                    tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
                    if (tt_array[0] == "1")
                    {
                        tt_flag2 = true;
                        tt_mac = tt_array[1];
                    }
                    else
                    {
                        PutLableInfor("该单板在alllable表中没有找到，请确认");
                    }
                }

            }

            //第三步开始查询
            if (tt_flag2)
            {
                GetMacStation(tt_mac);
                if (this.dataGridView1.DataSource == null)
                {
                }
                else
                {
                    PutLableInfor("查询完毕！若需要跳站，请先点击确定按钮！" );
                }

            }
        }


        #endregion


        #region 8、维修板跳转
        //维修跳转2100
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //数据初始化
                PutLableInfor2("");
                this.richTextBox2.Text = null;
                this.richTextBox2.BackColor = Color.White;
                string tt_scan1 = this.textBox2.Text.Trim();
                string tt_scan2 = tt_scan1.Replace("-", "");
                string tt_sacn3 = tt_scan2.Replace(":", "");
                string tt_scansn = tt_sacn3.Replace(" ", "");

                SetRichtexBox2("----开始站位跳转------");
                //第一步位数判断
                Boolean tt_flag1 = false;
                tt_flag1 = CheckStrLengh3(tt_scan1, this.comboBox3.Text);

                //第二步获取MAC,PCBA号
                string tt_mac = "";
                Boolean tt_flag2 = false;
                if (tt_flag1)
                {
                    //选择的是MAC
                    if (this.radioButton4.Checked == true)
                    {
                        tt_flag2 = true;
                        tt_mac = tt_scansn;
                        SetRichtexBox2("2、选择的MAC为：" + tt_mac);
                    }

                    //选择的是单板号
                    if (this.radioButton3.Checked == true)
                    {
                        string tt_sql = "select count(1),min(maclable),0  from odc_alllable where pcbasn = '" + tt_scansn + "' ";

                        string[] tt_array = new string[3];
                        tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
                        if (tt_array[0] == "1")
                        {
                            tt_flag2 = true;
                            tt_mac = tt_array[1];
                            SetRichtexBox2("2、选择的MAC为：" + tt_mac);
                        }
                        else
                        {
                            SetRichtexBox2("2、该单板在alllable表中没有找到,");
                            PutLableInfor2("该单板在alllable表中没有找到，请确认");
                        }
                    }

                }

                //第三步获取单板号
                string tt_pcba = "";
                string tt_taskcode = "";
                Boolean tt_flag3 = false;
                if (tt_flag2)
                {
                    //选择的单板号
                    if (this.radioButton3.Checked == true)
                    {
                        tt_flag3 = true;
                        tt_pcba = tt_scansn;
                        SetRichtexBox2("3、扫描单板号为：" + tt_pcba + ",goon");
                    }


                    //选择的是MAC
                    if (this.radioButton4.Checked == true)
                    {
                        string tt_sql3 = "select count(1),min(pcbasn),min(taskscode)  from odc_alllable where maclable = '" + tt_scansn + "' ";

                        string[] tt_array3 = new string[3];
                        tt_array3 = Dataset1.GetDatasetArray(tt_sql3, tt_conn);
                        if (tt_array3[0] == "1")
                        {
                            tt_flag3 = true;
                            tt_pcba = tt_array3[1];
                            tt_taskcode = tt_array3[2];
                            SetRichtexBox2("3、扫描单板号为：" + tt_pcba + ",工单号为:" + tt_taskcode + ",goon");
                        }
                        else
                        {
                            SetRichtexBox2("3、该MAC在alllable表中没有找到，over");
                            PutLableInfor2("该MAC在alllable表中没有找到，请确认");
                        }
                    }
                }

                //第四步 是否正在维修状态
                Boolean tt_flag4 = false;
                if (tt_flag3)
                {
                    string tt_sql5 = "select count(1),0,0 from repair where MAC='" + tt_mac + "' and type = 2 ";
                    string[] tt_array5 = new string[3];
                    tt_array5 = Dataset1.GetDatasetArray(tt_sql5, tt_conn);
                    if (tt_array5[0] == "1")
                    {
                        tt_flag4 = true;
                        SetRichtexBox2("4、该产品处于维修状态，且已修好，over");

                        
                    }
                    else
                    {
                       SetRichtexBox2("4、该产品不在维修状态,goon");
                        PutLableInfor2("该产品不在维修状态，或还没有修好，请确认");
                    }
                }

                //第五步：查找当前站位
                string tt_ncode = "";
                string tt_riwid = "";
                Boolean tt_flag5 = false;
                if (tt_flag4)
                {
                    string tt_sql8 = "select count(1),min(id),min(ncode) from odc_routingtasklist " +
                                         "where  pcba_pn = '" + tt_mac + "' and napplytype is null ";
                    string[] tt_array8 = new string[3];
                    tt_array8 = Dataset1.GetDatasetArray(tt_sql8, tt_conn);
                    if (tt_array8[0] == "1")
                    {
                        tt_riwid = tt_array8[1];
                        tt_ncode = tt_array8[2];
                        tt_flag5 = true;
                        SetRichtexBox2("5、该单板有待测站位，ID号：" + tt_array8[1] + "，当前站位" + tt_array8[2] + ", goon");


                    }
                    else
                    {
                        SetRichtexBox2("5、没有找到待测站位，或有多条待测站位，流程异常，over");
                        PutLableInfor2("没有找到待测站位，或有多条待测站位，流程异常！");
                    }
                }

                //第六步 开始跳站
                Boolean tt_flag6 = false;
                if (tt_flag5)
                {
                    string tt_sql = "update odc_routingtasklist set ncode = '2110' ,Fremark = 'PR001维修站位跳转' " +
                                   " where  pcba_pn = '" + tt_mac + "' and napplytype is null  and id = " + tt_riwid;

                    int tt_exec = Dataset1.ExecCommand(tt_sql, tt_conn);

                    if (tt_exec > 0)
                    {
                        tt_flag6 = true;
                        SetRichtexBox2("6、跳转成功, goon");
                    }
                    else
                    {
                        SetRichtexBox2("6、跳转不成功，over");
                        PutLableInfor2("跳转不成功，请确认！");
                    }
                }

                //最后显示
                if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5 && tt_flag6)
                {

                    this.richTextBox2.BackColor = Color.Chartreuse;
                    PutLableInfor2("OK,跳转成功，请继续");
                    SetRichtexBox2("----OK,跳转成功，请继续----");
                }
                else
                {
                    this.richTextBox2.BackColor = Color.Red;
                    SetRichtexBox2("----Sorry,跳转失败----");
                   // PutLableInfor2("Sorry,跳转失败，请确认");
                }

                this.textBox2.Focus();
                this.textBox2.SelectAll();

                //-----------end-----------

            }
        }       

        //重置
        private void button6_Click(object sender, EventArgs e)
        {
            PutLableInfor2("");
            this.richTextBox2.Text = null;
            this.richTextBox2.BackColor = Color.White;
            this.textBox2.Text = null;
        }



        #endregion


        














    }
}
