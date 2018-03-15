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
    public partial class Form10_lg : Form
    {
        public Form10_lg()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_conn;
        static string tt_pcname;
        static string tt_bpath = @"D:\\bak\\log";
        private string tt_path;
        private int tt_interval = 100;

        private void Form10_lg_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";
            tt_pcname = System.Net.Dns.GetHostName();

            //清除标签数据
            ClearLabelone();

            //工单信息不显示
            this.label7.Text = null;
            this.label9.Text = null;
            
            //按钮先不显示
            this.button4.Visible = false;
            this.button5.Visible = false;
            this.button6.Visible = false;

            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label19.Text = tt_interval.ToString();


            //listview设置
            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.LabelEdit = true; //是否可编辑,ListView只可编辑第一列。
            this.listView1.Scrollable = true;//有滚动条
            this.listView1.HeaderStyle = ColumnHeaderStyle.Clickable;//对表头进行设置
            this.listView1.FullRowSelect = true;//是否可以选择行

            //添加表头
            this.listView1.Columns.Add("NO", 30);
            this.listView1.Columns.Add("文件路径及名称", 250);
            this.listView1.Columns.Add("文件名", 200);


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



        #region 2、单个文件上传
        //单个文件上传 文件选择
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string tt_file = fileDialog.FileName;
                string tt_fpath = fileDialog.SafeFileName;

                // MessageBox.Show("已选择文件:" + file, "选择文件提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.textBox1.Text = tt_file;
                this.textBox2.Text = tt_fpath;
                this.textBox3.Text = GetNewName(tt_fpath);
                this.textBox4.Text = "E:\\02 ZGLOGLOAD\\TEST";
            }
        }

        //单个文件上传 重置
        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = null;
            this.textBox2.Text = null;
            this.textBox3.Text = null;
            this.textBox4.Text = null;
        }

        //单个文件上传 上传
        private void button3_Click(object sender, EventArgs e)
        {

            if (sip == "172.18.201.2")
            {

                Boolean tt_flag1 = UploadImage4(this.textBox1.Text,this.textBox4.Text, this.textBox3.Text);
                if (tt_flag1)
                {
                    MessageBox.Show("上传成功");
                }
                else
                {
                    MessageBox.Show("上传失败");
                }

            }
            else if (sip == "172.16.30.2")
            {
                Boolean tt_flag2 = UploadImage5(this.textBox1.Text, this.textBox4.Text, this.textBox3.Text);
                if (tt_flag2)
                {
                    MessageBox.Show("上传成功");
                }
                else
                {
                    MessageBox.Show("上传失败");
                }
            }
            else
            {
                MessageBox.Show("找不到网络！");
            }




        }
        #endregion



        #region 3、上传辅助功能
        //获取数据流
        public static byte[] GetBytesByPath(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((int)fs.Length);
            fs.Flush();
            fs.Close();
            return bytes;
        }


        //上传文件 
        private bool UploadImage3(string tt_path, string tt_filname)
        {
            bool flag = true;
            //string path = @"D:\ZGBY1701001\picture\阮景欣作文.jpg";//本地路径

            //path = tt_path;


            byte[] tt_bytes = GetBytesByPath(tt_path);//获取文件byte[]
            string tt_uploadPath = "ZGlog";//上传服务器文件夹路径
            string fileName = "img18.jpg";//文件名称

            fileName = tt_filname;


            try
            {

                ServiceReference1.WebServiceSoapClient client = new ServiceReference1.WebServiceSoapClient();

                if (client.UploadFile(tt_bytes, tt_uploadPath, tt_filname))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }


            }
            catch
            {
                flag = false;
            }
            return flag;
        }




        //上传文件 生产网服务
        private bool UploadImage4(string tt_path, string tt_uploadpath, string tt_filname)
        {
            bool flag = true;
            byte[] tt_bytes = GetBytesByPath(tt_path);//获取文件byte[]
            try
            {

                ServiceReference2.WebServiceSoapClient client = new ServiceReference2.WebServiceSoapClient();

                if (client.UploadFile(tt_bytes, tt_uploadpath, tt_filname))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }


            }
            catch
            {
                flag = false;
            }
            return flag;
        }


        //上传文件 办公网服务
        private bool UploadImage5(string tt_path, string tt_uploadpath, string tt_filname)
        {
            bool flag = true;
            byte[] tt_bytes = GetBytesByPath(tt_path);//获取文件byte[]
            try
            {

                ServiceReference1.WebServiceSoapClient client = new ServiceReference1.WebServiceSoapClient();

                if (client.UploadFile(tt_bytes, tt_uploadpath, tt_filname))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }


            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        //获取新的文件名
        private string GetNewName(string tt_oldname)
        {
            string tt_newname = "";
            int i = tt_oldname.LastIndexOf(".");//获取。的索引顺序号，在这里。代表图片名字与后缀的间隔
            string tt_kzm = tt_oldname.Substring(i);//获取文件扩展名的另一种方法 string fileExtension = System.IO.Path.GetExtension(FileUpload1.FileName).ToLower();
            string tt_beforename = tt_oldname.Substring(0, i);  //获取前面的文件名

            //时间随机数
            string tt_time = DateTime.Now.ToLongDateString().ToString();
            Random ran = new Random();
            int RandKey = ran.Next(100, 999);

            DateTime now = DateTime.Now;
            string tt_millscend = now.Millisecond.ToString();

            //获取新的文件名
            //tt_newname = tt_beforename + "_" + tt_time + "_" + RandKey.ToString() + tt_kzm;
            tt_newname = tt_beforename + "_" + tt_time + "_" + tt_millscend + tt_kzm;
            return tt_newname;
        }





        #endregion



        #region 4、锁定事件
        //工单锁定事件
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                string tt_sql1 = "select  tasksquantity,product_name " +
                                "from odc_tasks where taskscode = '" + this.textBox5.Text + "' ";
                DataSet ds1 = Dataset1.GetDataSetTwo(tt_sql1, tt_conn);

                if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    this.label7.Text = ds1.Tables[0].Rows[0].ItemArray[0].ToString(); //工单数量
                    this.label9.Text = ds1.Tables[0].Rows[0].ItemArray[1].ToString(); //工单数量

                    this.textBox5.Enabled = false;

                    this.button4.Visible = true;
                    

                }
                else
                {
                    MessageBox.Show("没有查询此工单，请确认！");

                }
            }
            else
            {
                this.textBox5.Enabled = true;
                this.label7.Text = null;
                this.label9.Text = null;

                this.button4.Visible = false;
                
            }
        }


        //目录锁定事件
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.textBox6.Enabled = false;
                this.comboBox1.Enabled = false;

                this.button5.Visible = true;
                this.button6.Visible = true;
            }
            else
            {
                this.textBox6.Enabled = true;
                this.comboBox1.Enabled = true;

                this.button5.Visible = false;
                this.button6.Visible = false;
            }

            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label19.Text = tt_interval.ToString();
        }
        #endregion


        #region 5、清除事件

        //清除标签数据
        private void ClearLabelone()
        {
            this.label13.Text = null;
            this.label15.Text = null;
            this.label17.Text = null;
            this.label21.Text = null;
            this.label23.Text = null;
        }

        //清除所有文本框数据
        private void ClearRichText()
        {
            //this.richTextBox1.Text = null;
            this.richTextBox2.Text = null;
            this.richTextBox3.Text = null;
            this.richTextBox2.BackColor = Color.White;
        }



        #endregion


        #region 6、按钮事件

        //目录选择
        private void button4_Click(object sender, EventArgs e)
        {
            tt_path = "";
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (fbd.SelectedPath != "")
                {
                    tt_path = fbd.SelectedPath;
                    textBox6.Text = tt_path;
                }
            }
        }

        //开始
        private void button5_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                button5.Text = "开始";

                timer1.Stop();
            }
            else
            {

                try
                {

                    button5.Text = "停止";
                    timer1.Start();
                }
                catch (Exception)
                {

                }
            }
        }


        //上传
        private void button6_Click(object sender, EventArgs e)
        {
            if (this.button5.Text == "停止")
            {
                MessageBox.Show("请先停止自动执行");


                
            }
            else
            {
                LogUploadMain();

            }
        }

        //重置
        private void button7_Click(object sender, EventArgs e)
        {
            ClearLabelone();
            ClearRichText();
            CleatListView();
        }


        //周期选择按钮
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label19.Text = tt_interval.ToString();
        }


        #endregion


        #region 7、辅助功能
        //时间控件
        private void timer1_Tick(object sender, EventArgs e)
        {
            tt_interval--;
            this.label19.Text = tt_interval.ToString();
            if (tt_interval == 0)
            {
                LogUploadMain();

                //System.Threading.Thread.Sleep(1000);//暂停1秒
                tt_interval = Convert.ToInt32(this.comboBox1.Text);
                this.label19.Text = tt_interval.ToString();
            }
        }


        //或取工作目录文件
        private string GetLogText()
        {

            string tt_path = this.textBox6.Text;
            DirectoryInfo folder = new DirectoryInfo(tt_path);
            int tt_textnumber = 0;
            foreach (FileInfo file in folder.GetFiles("*.txt"))
            {
                //this.richTextBox1.Text = file.FullName + "\n" + this.richTextBox1.Text;
                tt_textnumber++;
            }

            this.label13.Text = tt_textnumber.ToString();
            return tt_textnumber.ToString();
        }



        //遍历获取取工作目录文件
        private string GetLogText2( string tt_path)
        {
            string tt_listnumber = "";
            CleatListView();
            Director(tt_path);
            tt_listnumber = this.listView1.Items.Count.ToString();
            this.label13.Text = tt_listnumber;
            return  tt_listnumber;
        }



        //获取已过站的文件数量
        private void GetLogUploadFile()
        {
            //string tt_path = this.textBox6.Text + @"\log";
            DirectoryInfo folder = new DirectoryInfo(tt_bpath);
            int tt_textnumber = 0;
            foreach (FileInfo file in folder.GetFiles("*.*"))
            {
                this.richTextBox3.Text = file.FullName + "\n" + this.richTextBox3.Text;
                tt_textnumber++;
            }

            this.label17.Text = tt_textnumber.ToString();
        }


        //获取上传目录
        private string GetUploadPath()
        {
            string tt_uploadpath = "E:\\02 ZGLOGLOAD\\";
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now; 
            string strYM = currentTime.ToString("y");
            tt_uploadpath = tt_uploadpath  + strYM;
            this.label21.Text = tt_uploadpath;
            return tt_uploadpath;

        }


        //获取上传目录
        private string GetUploadPath2(string tt_pcname)
        {
            string tt_uploadpath = "E:\\02 ZGLOGLOAD\\";

            //年月
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string strYM = currentTime.ToString("y");



            //日期
            string tt_date = DateTime.Now.ToString("yyyy-MM-dd");        // 2008-09-04


            tt_uploadpath = tt_uploadpath + "\\" + strYM + "\\LOG\\" + tt_date + "\\" + tt_pcname;

            this.label21.Text = tt_uploadpath;
            return tt_uploadpath;

        }


        //richtext加记录
        private void setRichtexBox(string tt_textinfor)
        {
            this.richTextBox2.Text = this.richTextBox2.Text + tt_textinfor + "\n";
        }


        //目录文件遍历
        private void Director(string dir)
        {
            DirectoryInfo d = new DirectoryInfo(dir);
            FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                if (fsinfo is DirectoryInfo)     //判断是否为文件夹  
                {
                    Director(fsinfo.FullName);//递归调用  
                }
                else
                {
                    PutListViewData(fsinfo.FullName, fsinfo.Name);

                }
            }
        }



        #endregion



        #region 8、上传方法
        //主要方法
        private void LogUploadMain()
        {
            //数据初始化
            setRichtexBox("---------开始LOG日志上传----------");
            ClearRichText();
            


            //第一步 查看是否填写日志目录
            Boolean tt_flag1 = false;
            string tt_userselectpath = this.textBox6.Text;
            string tt_frompath = "";
            if (!tt_userselectpath.Equals(""))
            {
                setRichtexBox("第一步：已选择LOG存储目录，" + tt_userselectpath + ",goon");
                tt_flag1 = true;
            }
            else
            {
                setRichtexBox("第一步：没有选择LOG存储目录,over");
            }




            //第二步 查看是否存在bak目录,不存在就创建一个目录
            Boolean tt_flag2 = false;
            if (tt_flag1)
            {
                if (!Directory.Exists(tt_bpath))
                {
                    Directory.CreateDirectory(tt_bpath);
                    setRichtexBox("第二步：没有log目录，新建一个目录" + tt_bpath);

                }
                else
                {
                    setRichtexBox("第二步：已存在log目录" + tt_bpath);
                }
                tt_flag2 = true;
            }



            //第三步获取x需要上传目录信息
            Boolean tt_flag3 = false;
            if (tt_flag1 && tt_flag2)
            {
                string tt_filenumber = GetLogText2(tt_userselectpath);
                tt_flag3 = true;
                setRichtexBox("第三步：获取到上传文件数目:" + tt_filenumber);
            }


            //第四步获取上传目录
            Boolean tt_flag4 = false;
            string tt_sevicepath = "";
            if (tt_flag1 && tt_flag2 && tt_flag3)
            {
                tt_sevicepath = GetUploadPath2(tt_pcname);
                tt_flag4 = true;
                setRichtexBox("第四步：获取到上传文件目录名:" + tt_sevicepath);
            }




            //第五步在在老的方法，目录中循环 不需要用
            #region
            Boolean tt_flag5 = false;
            string tt_task = this.textBox5.Text;
            int tt_loadsuccess = 0;
            int tt_movefile = 0;
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
            {

                setRichtexBox("第五步：开始文件上传操作" );

                
                string tt_logpath;
                string tt_newname;
                string tt_file;
                string tt_path = this.textBox6.Text;
                int tt_textnumber = 0;
                DirectoryInfo folder = new DirectoryInfo(tt_path);
                foreach (FileInfo file in folder.GetFiles("*.txt"))
                {

                    
                    setRichtexBox("--- " + tt_textnumber.ToString()+" ----");

                    //第1步，获取文件名及路径
                    tt_logpath = file.FullName;
                    tt_frompath = tt_logpath;
                    setRichtexBox("1、文件路径：" + tt_logpath);


                    //第2步 获取老的文件名
                    tt_file = file.Name;
                    setRichtexBox("2、老文件名：" + tt_file);

                    //第3步 获取新的文件名
                    tt_newname = GetNewName(tt_file);
                    setRichtexBox("3、新文件名：" + tt_newname);

                    

                    //第四步 上传文件
                    bool tt_uploadflag = AutoUploadFile(sip, tt_logpath, tt_sevicepath, tt_newname);
                    if ( tt_uploadflag )
                    {
                        setRichtexBox("4、该文件上传成功." );
                        tt_loadsuccess++;
                    }
                    else
                    {
                        setRichtexBox("4、该文件上传失败-----!!!!");
                    }


                    //第五步 记录数据
                    if (tt_uploadflag)
                    {
                       string tt_sql = "insert odc_logupload (Ftaskcode,Flogname,Fnewname,Fpath,Fdate) " +
                              "values( '" + tt_task + "','" + tt_file + "','" + tt_newname + "','" + tt_sevicepath + "',getdate() )";
                       int tt_dbrecord = Dataset1.ExecCommand(tt_sql,tt_conn);
                       if (tt_dbrecord > 0)
                        {
                            setRichtexBox("5、该记录到数据库成功");
                        }
                        else
                        {
                            setRichtexBox("5、该记录到数据库失败");
                        }
                    }

                    //第6步获取转移路径及文件名
                    string tt_movepath = tt_bpath + @"\" + tt_file;
                    if (tt_uploadflag)
                    {

                        setRichtexBox("6、转移的文件路径名:" + tt_movepath);
                    }


                    //第7步 将数据移到log目录中
                    if (tt_uploadflag)
                   {
                       Boolean tt_moveflag = fileMove(tt_frompath, tt_movepath);
                       if (tt_moveflag)
                       {
                           setRichtexBox("7、文件转移成功:");
                           tt_movefile++;
                       }
                       else
                       {
                           setRichtexBox("7、文件转移不成功:");
                       }
                   }
                 



                    tt_textnumber++;


                   
                }




                if (tt_textnumber == tt_loadsuccess)
                {
                    tt_flag5 = true;
                    setRichtexBox("第五步、文件上全部上传成功");
                }
                else
                {
                    setRichtexBox("第五步、文件上没有全部上传成功，请检查");
                }

            }

            #endregion



            //第六步查看ListView是否有信息
            Boolean tt_flag6 = false;
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
            {
                int tt_listviewitem = this.listView1.Items.Count;
                if ( tt_listviewitem > 0)
                {
                    tt_flag6 = true;
                    setRichtexBox("第六步、有需要上传的文件数："+tt_listviewitem.ToString()+",goon");
                }
                else
                {
                    tt_flag6 = false;
                    setRichtexBox("第六步、没有有需要上传的文件数：" + tt_listviewitem.ToString() + ",goon");
                }
            }



            //第七步 上传文件
            Boolean tt_flag7 = false;
            string tt_task2 = this.textBox5.Text;
            int tt_loadsuccess2 = 0;
            int tt_movefile2 = 0;
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag6)
            {
                setRichtexBox("第五步：开始文件上传操作");


                string tt_logpath;
                string tt_newname;
                string tt_file;
                string tt_path = this.textBox6.Text;
                int tt_textnumber2 = 0;

                //ListView内循环
                int tt_count = this.listView1.Items.Count;
                for (int i = 0; i < tt_count; i++)
                {

                    #region listview循环
                    setRichtexBox("--- " + i.ToString() + " ----");


                    //第1步，获取文件名及路径
                    tt_logpath = this.listView1.Items[i].SubItems[1].Text;
                    setRichtexBox("1、文件路径：" + tt_logpath);

                    //第2步 获取原文件名
                    tt_file = this.listView1.Items[i].SubItems[2].Text;
                    setRichtexBox("2、原文件名：" + tt_file);


                    //第3步 获取新的文件名
                    tt_newname = GetNewName(tt_file);
                    setRichtexBox("3、新文件名：" + tt_newname);


                    //第四步 上传文件
                    bool tt_uploadflag = AutoUploadFile(sip, tt_logpath, tt_sevicepath, tt_newname);
                    if (tt_uploadflag)
                    {
                        setRichtexBox("4、该文件上传成功.");
                        tt_loadsuccess2++;
                    }
                    else
                    {
                        setRichtexBox("4、该文件上传失败-----!!!!");
                    }



                    //第五步 记录数据
                    if (tt_uploadflag)
                    {
                        string tt_sql = "insert odc_logupload (Ftaskcode,Flogname,Fnewname,Fpath,Fdate) " +
                               "values( '" + tt_task2 + "','" + tt_file + "','" + tt_newname + "','" + tt_sevicepath + "',getdate() )";
                        int tt_dbrecord = Dataset1.ExecCommand(tt_sql, tt_conn);
                        if (tt_dbrecord > 0)
                        {
                            setRichtexBox("5、该记录到数据库成功");
                        }
                        else
                        {
                            setRichtexBox("5、该记录到数据库失败");
                        }
                    }

                    //第6步获取转移路径及文件名
                    string tt_movepath = tt_bpath + @"\" + tt_file;
                    if (tt_uploadflag)
                    {

                        setRichtexBox("6、转移的文件路径名:" + tt_movepath);
                    }


                    //第7步 将数据移到log目录中
                    if (tt_uploadflag)
                    {
                        Boolean tt_moveflag = fileMove(tt_logpath, tt_movepath);
                        if (tt_moveflag)
                        {
                            setRichtexBox("7、文件转移成功:");
                            tt_movefile2++;
                        }
                        else
                        {
                            setRichtexBox("7、文件转移不成功:");
                        }
                    }


                    tt_textnumber2++;

                    #endregion

                }


                if (tt_textnumber2 == tt_loadsuccess2)
                {
                    tt_flag7 = true;
                    setRichtexBox("第七步、文件上全部上传成功");
                }
                else
                {
                    setRichtexBox("第七步、文件上没有全部上传成功，请检查");
                }

            }




            //最后总结
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag6 && tt_flag7)
            {
                this.label15.Text = tt_loadsuccess2.ToString();
                this.label23.Text = tt_movefile2.ToString();
                GetLogUploadFile();
                this.richTextBox2.BackColor = Color.Chartreuse;

            }
            else
            {
                this.label15.Text = tt_loadsuccess2.ToString();
                this.label23.Text = tt_movefile2.ToString();
                this.richTextBox2.BackColor = Color.Red;
            }


        }


        //循环上传方法
        private bool AutoUploadFile(string tt_ip, string tt_path, string tt_uploadpath, string tt_filname)
       {
           bool tt_flag = false;
           if (tt_ip == "172.18.201.2")
           {

               Boolean tt_flag1 = UploadImage4(tt_path, tt_uploadpath, tt_filname);
               if (tt_flag1)
               {
                   tt_flag = true;
               }
               else
               {
                   
               }

           }
           else if (tt_ip == "172.16.30.2")
           {
               Boolean tt_flag2 = UploadImage5(tt_path, tt_uploadpath, tt_filname);
               if (tt_flag2)
               {
                   tt_flag = true;
               }
               else
               {
                   
               }
           }
           else
           {
               
           }




           return tt_flag;

       }


        //移动文件
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


        #region 9、上传数据查询

        //重置
        private void button9_Click(object sender, EventArgs e)
        {
            this.textBox7.Text = null;
            this.textBox8.Text = null;
            this.dataGridView1.DataSource = null;
        }



        //确定
        private void button8_Click(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = null;
            //-----日期----
            string tt_date1 = this.dateTimePicker1.Text;
            string tt_date2 = this.dateTimePicker2.Text;

            //----工单----
            string tt_task = "";
            if ( !this.textBox7.Text.Equals(""))
            {
                tt_task = " and Ftaskcode = '" + this.textBox7.Text + "' ";
            }

            //-----文件名----
            string tt_filename = "";
            if (!this.textBox8.Text.Equals(""))
            {
                tt_filename = " and Flogname like '%" + this.textBox8.Text + "%' ";
            }

            string tt_sql = "select Ftaskcode 工单,Flogname 文件名,Fnewname 新文件名,Fpath 保存路径, Fdate 记录时间 " +
                            "from ODC_LOGUPLOAD "+
                            "where Fdate between '" + tt_date1 + "' and '" + tt_date2 + "' " + tt_task + tt_filename;

            DataSet ds1 = Dataset1.GetDataSet(tt_sql, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView1.DataSource = ds1;
                dataGridView1.DataMember = "Table";
            }
            else
            {
                MessageBox.Show("sorry,没有查询到数据！");
            }


        }

        //显示行号
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //序号
            SolidBrush brushOne = new SolidBrush(Color.Red);
            e.Graphics.DrawString(Convert.ToString(e.RowIndex + 1, System.Globalization.CultureInfo.CurrentUICulture), e.InheritedRowStyle.Font, brushOne, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
        }


        #endregion


        #region 10、ListView方法
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
        private void PutListViewData(string tt_fullname, string tt_name)
        {
            int i = this.listView1.Items.Count + 1;
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { i.ToString(), tt_fullname, tt_name });
            this.listView1.Items.AddRange(p);
            this.listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
        }
        #endregion


    }
}
