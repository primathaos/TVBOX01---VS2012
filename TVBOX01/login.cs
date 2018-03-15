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
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_databasecheck = "";
        static string tt_conn;
        static string tt_version = "20180115";
        static string tt_progranname = "PR001";
        static string tt_prodescrib = "自动升级";

        private void login_Load(object sender, EventArgs e)
        {
            this.radioButton1.Checked = true;
            tt_databasecheck = "172.18.201.2";
            this.toolStripStatusLabel2.Text = tt_databasecheck;
            tt_conn = "server=" + tt_databasecheck + ";database=oracle;uid=sa;pwd=adminsa";

            this.Text = this.Text + "-" + tt_progranname + "-" + tt_version + "-" + tt_prodescrib;

        }

        #endregion

        

        #region 2、按钮功能

        //重置
        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = null;
        }


        //确定
        private void button1_Click(object sender, EventArgs e)
        {

            string tt_username = this.comboBox1.Text.Substring(0, 5);
            

            #region 一、烽火电信移动进站
            //1 烽火只是进站产生待测站位
            #region
            if (tt_username == "FH001" )
            {

               
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    //1、大版本判断
                     bool tt_flag1 = false;
                     string tt_newversion = GetProgramVersion(tt_progranname);
                     if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                     {
                         tt_flag1 = true;
                     }
                     else
                     {
                         MessageBox.Show("当前大版本为:" + tt_version + "，不是最新版本：" + tt_newversion);
                     }

                    //2、小版本判断
                     bool tt_flag2 = false;
                     if (tt_flag1)
                     {
                         string tt_uiversion = "20171021";
                         string tt_setuiversion = GetProgramVersion2(tt_username);
                         if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                         {
                             tt_flag2 = true;
                         }
                         else
                         {
                             MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                         }
                     }



                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();
                        Form2_zg form1 = new Form2_zg();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }


                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion

            #endregion

            
            #region 二、烽火电信wifi

            //2.1 烽火电信wifi打印MAC
            #region
            if ((tt_username == "MC001" || tt_username == "MC101") )
            {
                string tt_password = GetUserPassword(tt_username);

                if (this.textBox2.Text == tt_password)
                {

                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form3_mp form2 = new Form3_mp();
                        form2.STR = tt_username;
                        form2.SIP = this.toolStripStatusLabel2.Text;
                        form2.ShowDialog();
                        form2.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }
                    
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }

            }
            #endregion
            

            //2.2 烽火电信wifi打印铭牌标签及二维码标签(已用)
            #region
            if (tt_username == "MP001" || tt_username == "MP101" || tt_username == "MP002" || tt_username == "MP102" || tt_username == "MP003" || tt_username == "MP103") 
            {
                string tt_password = GetUserPassword(tt_username);

                if (this.textBox2.Text == tt_password)
                {

                    //1、大版本判断
                    bool tt_flag1 = false;
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        tt_flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show("当前大版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }

                    //2、小版本判断
                    bool tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_uiversion = "20171128";
                        string tt_setuiversion = GetProgramVersion2(tt_username);
                        if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                        {
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                        }
                    }



                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();
                        Form12_ot form3 = new Form12_ot();
                        form3.STR = tt_username;
                        form3.SIP = this.toolStripStatusLabel2.Text;
                        form3.ShowDialog();
                        form3.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                                        

                    
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion
            

            //2.3 烽火电信wifi打印生产序列号
            #region
            if (tt_username == "SN001" || tt_username == "SN101" )
            {
                string tt_password = GetUserPassword(tt_username);

                if (this.textBox2.Text == tt_password)
                {
                     string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form11_we form4 = new Form11_we();
                        form4.STR = tt_username;
                        form4.SIP = this.toolStripStatusLabel2.Text;
                        form4.ShowDialog();
                        form4.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }

                    
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion
            

            //2.4 烽火电信wifi:打印彩盒标签
            #region
            if ( tt_username == "CH001" || tt_username == "CH101")
            {

                string tt_password = GetUserPassword(tt_username);

                if (this.textBox2.Text == tt_password)
                {
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form4_ch form4 = new Form4_ch();
                        form4.STR = tt_username;
                        form4.SIP = this.toolStripStatusLabel2.Text;
                        form4.ShowDialog();
                        form4.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }

                    
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion
            

            //2.5  烽火电信wifi打印中箱标签
            #region
            if (tt_username == "ZX001" || tt_username == "ZX101" )
            {
                string tt_password = GetUserPassword(tt_username);

                if (this.textBox2.Text == tt_password)
                {

                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form7_zx form7 = new Form7_zx();
                        form7.STR = tt_username;
                        form7.SIP = this.toolStripStatusLabel2.Text;
                        form7.ShowDialog();
                        form7.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }

                    
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion

            #endregion
            

            #region 三、烽火移动wifi
            //-------------以下烽火移动-------------
            //3.1 烽火移动wifi 设备标签 //通用设备标签 //杨浩
            #region
            if (tt_username == "FH011" || tt_username == "FH111")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    //1、大版本判断
                    bool tt_flag1 = false;
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        tt_flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show("当前大版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }

                    //2、小版本判断
                    bool tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_uiversion = "20171128";
                        string tt_setuiversion = GetProgramVersion2(tt_username);
                        if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                        {
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                        }
                    }

                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();                        
                        Form13_asb form1 = new Form13_asb();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;                        
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }

                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion

            //3.2 烽火移动wifi  移动标签  //通用移动、电信标签 //杨浩
            #region
            if (tt_username == "FH002" || tt_username == "FH102")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    //1、大版本判断
                    bool tt_flag1 = false;
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        tt_flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show("当前大版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }

                    //2、小版本判断
                    bool tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_uiversion = "20171128";
                        string tt_setuiversion = GetProgramVersion2(tt_username);
                        if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                        {
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                        }
                    }

                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();
                        Form14_ayd form1 = new Form14_ayd();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }

                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion


            //3.3 烽火移动wifi 生产序列号标签 //通用 //杨浩
            #region
            if (tt_username == "FH003" || tt_username == "FH103")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    #region 大小版本控制
                    //1、大版本判断
                    bool tt_flag1 = false;
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        tt_flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show("当前大版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }

                    //2、小版本判断
                    bool tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_uiversion = "20171016";
                        string tt_setuiversion = GetProgramVersion2(tt_username);
                        if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                        {
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                        }
                    }



                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();
                        Form15_ayx form1 = new Form15_ayx();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }

                    #endregion

                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion


            //3.4 烽火移动wifi 彩盒标签 //通用 //杨浩
            #region
            if (tt_username == "FH004" || tt_username == "FH104" || tt_username == "FH204" || tt_username == "FH214")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form16_ach form1 = new Form16_ach();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion


            //3.5 烽火移动wifi 中箱标签一/二 //通用 //杨浩
            #region
            if (tt_username == "FH005" || tt_username == "FH105" || tt_username == "FH006" || tt_username == "FH106")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    //1、大版本判断
                    bool tt_flag1 = false;
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        tt_flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show("当前大版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }

                    //2、小版本判断
                    bool tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_uiversion = "20171128";
                        string tt_setuiversion = GetProgramVersion2(tt_username);
                        if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                        {
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                        }
                    }



                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();
                        Form17_azx1 form1 = new Form17_azx1();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }





                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion


            //15 烽火移动 中箱标签二
            #region
            //if (tt_username == "FH006" || tt_username == "FH106")
            //{
            //    string tt_password = GetUserPassword(tt_username);
            //    if (this.textBox2.Text == tt_password)
            //    {
            //        string tt_newversion = GetProgramVersion(tt_progranname);
            //        if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
            //        {
            //            this.Hide();
            //            Form18_azx2 form1 = new Form18_azx2();
            //            form1.STR = tt_username;
            //            form1.SIP = this.toolStripStatusLabel2.Text;
            //            form1.ShowDialog();
            //            form1.Dispose();
            //            this.Show();
            //            this.textBox2.Text = null;
            //        }
            //        else
            //        {
            //            MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("密码不对，请确认");
            //    }
            //}
            #endregion
            

            #endregion


            #region 四、LOG相关

            //14 烽火  wifi校准
            if (tt_username == "FH901" )
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form19_abf form1 = new Form19_abf();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }


                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }


            //15 烽火  吞吐量日志过站
            if (tt_username == "FH902")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form19_abf form1 = new Form19_abf();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }



            //16 烽火  LOG日志文件上传
            if (tt_username == "FH909")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form10_lg form1 = new Form10_lg();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }




            //17 烽火  LOG数据上传及日志上传
            if (tt_username == "FH908")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form22_alog form1 = new Form22_alog();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }



            //18 烽火  LOG数据上传
            if (tt_username == "FH907")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form22_alog form1 = new Form22_alog();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }



            //19 烽火  LOG日志过站优化
            if (tt_username == "FH903")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    //1、大版本判断
                    bool tt_flag1 = false;
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        tt_flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }

                    //2、小版本判断
                    bool tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_uiversion = "20170828";
                        string tt_setuiversion = GetProgramVersion2(tt_username);
                        if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                        {
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                        }
                    }



                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();
                        Form23_alg2 form1 = new Form23_alg2();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }


                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }



            #endregion


            #region 五、其他功能

            //5.1 烽火  良率电子看板
            if (tt_username == "FH910")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form5_cc form1 = new Form5_cc();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }


            //5.2 烽火  站位跳转
            if (tt_username == "FH911")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    //1、大版本判断
                    bool tt_flag1 = false;
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        tt_flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show("当前大版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }


                    //2、小版本判断
                    bool tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_uiversion = "20180115";
                        string tt_setuiversion = GetProgramVersion2(tt_username);
                        if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                        {
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                        }
                    }


                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();
                        Form20_abd form1 = new Form20_abd();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }





                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }

            #endregion
            

            #region 六、延迟制造


            //6.1 烽火延迟制造 设备标签离线打印（暂时不用）
            #region
            if (tt_username == "FH007" || tt_username == "FH107")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        this.Hide();
                        Form21_als form1 = new Form21_als();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("当前版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion




            //6.2   延迟制造进站\铭牌
            #region
            if (tt_username == "FH008" || tt_username == "FH108")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    //1、大版本判断
                    bool tt_flag1 = false;
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        tt_flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show("当前大版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }


                    //2、小版本判断
                    bool tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_uiversion = "20171128";
                        string tt_setuiversion = GetProgramVersion2(tt_username);
                        if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                        {
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                        }
                    }


                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();
                        Form24_ama form1 = new Form24_ama();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }


                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion




            //6.3   延迟制造 单板临时标签打印
            #region
            if (tt_username == "FH009" || tt_username == "FH109")
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    //1、大版本判断
                    bool tt_flag1 = false;
                    string tt_newversion = GetProgramVersion(tt_progranname);
                    if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    {
                        tt_flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show("当前大版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    }


                    //2、小版本判断
                    bool tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_uiversion = "20171128";
                        string tt_setuiversion = GetProgramVersion2(tt_username);
                        if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                        {
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                        }
                    }


                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();
                        Form25_amb form1 = new Form25_amb();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }





                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }
            }
            #endregion


            #endregion


            #region 七、ZG相关

            //10 Log日志上传
            if (tt_username == "LG001" && this.textBox2.Text == "123456")
            {
                this.Hide();
                Form10_lg form10 = new Form10_lg();
                form10.STR = tt_username;
                form10.SIP = this.toolStripStatusLabel2.Text;
                form10.ShowDialog();
                form10.Dispose();
                this.Show();
                this.textBox2.Text = null;
            }


            


            //12 其他备用
            if (tt_username == "OT001" && this.textBox2.Text == "123456")
            {
                this.Hide();
                Form12_ot form12 = new Form12_ot();
                form12.STR = tt_username;
                form12.SIP = this.toolStripStatusLabel2.Text;
                form12.ShowDialog();
                form12.Dispose();

            }

            #endregion


            #region 八 临时加的扫描进2111
            if ((tt_username == "FH211" || tt_username == "FH212"))
            {
                string tt_password = GetUserPassword(tt_username);
                if (this.textBox2.Text == tt_password)
                {
                    //1、大版本判断
                    bool tt_flag1 = true;
                    //string tt_newversion = GetProgramVersion(tt_progranname);
                    //if (getStringToInt(tt_version) >= getStringToInt(tt_newversion))
                    //{
                    //    tt_flag1 = true;
                    //}
                    //else
                    //{
                    //    MessageBox.Show("当前大版本为:" + tt_version + "，小于最新版本：" + tt_newversion);
                    //}


                    //2、小版本判断
                    bool tt_flag2 = false;
                    if (tt_flag1)
                    {
                        string tt_uiversion = "20180205";
                        string tt_setuiversion = GetProgramVersion2(tt_username);
                        if (getStringToInt(tt_uiversion) >= getStringToInt(tt_setuiversion))
                        {
                            tt_flag2 = true;
                        }
                        else
                        {
                            MessageBox.Show("当前小版本为:" + tt_uiversion + "，不是最新版本：" + tt_setuiversion);
                        }
                    }

                    if (tt_flag1 && tt_flag2)
                    {
                        this.Hide();
                        Form18_azx2 form1 = new Form18_azx2();
                        form1.STR = tt_username;
                        form1.SIP = this.toolStripStatusLabel2.Text;
                        form1.ShowDialog();
                        form1.Dispose();
                        this.Show();
                        this.textBox2.Text = null;
                    }
                }
                else
                {
                    MessageBox.Show("密码不对，请确认");
                }

            }
            #endregion

        }

        #endregion

        

        #region 3、网络选择
        //生产网选择
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            tt_databasecheck = "172.18.201.2";
            this.toolStripStatusLabel2.Text = tt_databasecheck;
            tt_conn = "server=" + tt_databasecheck + ";database=oracle;uid=sa;pwd=adminsa";
        }

        //办公网选择
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            tt_databasecheck = "172.16.30.2";
            this.toolStripStatusLabel2.Text = tt_databasecheck;
            tt_conn = "server=" + tt_databasecheck + ";database=oracle;uid=sa;pwd=adminsa";
        }

        #endregion

        

        #region 4、辅助功能

        //获取密码
        private string GetUserPassword(string tt_username)
        {
            string tt_password = "123";

            string tt_sql = "select count(1),min(fpassword),min(Fcode) " +
                            " from odc_fhpassword where Fname = '" + tt_username + "' ";

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


        //获取大版本
        private string GetProgramVersion(string tt_programname)
        {
            string tt_newversion = "";

            string tt_sql = "select count(1),min(fpassword),min(Fcode) " +
                            " from odc_fhpassword where Fname = '" + tt_programname + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArrayTwo(tt_sql, tt_conn);

            if (tt_array[0] == "1")
            {
                tt_newversion = tt_array[1];
            }
            else
            {
                MessageBox.Show("网络连接失败，或没有" + tt_programname + "此程序账号，请确认");
            }


            return tt_newversion;
        }



        //获取小版本
        private string GetProgramVersion2(string tt_programname)
        {
            string tt_newversion = "";

            string tt_sql = "select count(1),min(fremark),0 " +
                            " from odc_fhpassword where Fname = '" + tt_programname + "' ";

            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArrayTwo(tt_sql, tt_conn);

            if (tt_array[0] == "1")
            {
                tt_newversion = tt_array[1];
            }
            else
            {
                MessageBox.Show("网络连接失败，或没有" + tt_programname + "此程序账号，请确认");
            }


            return tt_newversion;
        }




        //字符串转换
        private int getStringToInt(string tt_str)
        {
            int tt_int = 0;

            try
            {

                tt_int = Convert.ToInt32(tt_str);
            }
            catch
            {
                MessageBox.Show("字符串转换成int失败："+tt_str);
            }

            return tt_int;
        }

        #endregion


    }
}
