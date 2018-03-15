using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace TVBOX01
{
    public partial class Form22_alog : Form
    {
        public Form22_alog()
        {
            InitializeComponent();
        }

        #region 1、属性设置
        static string tt_conn;
        static string tt_pcname;
        private string tt_path;
        DataTable tt_dt;
        private int tt_interval = 100;
        static string tt_saveorup = ""; //上传设定



        //加载
        private void Form22_alog_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = str;
            this.toolStripStatusLabel4.Text = sip;
            tt_conn = "server=" + sip + ";database=oracle;uid=sa;pwd=adminsa";
            tt_pcname = System.Net.Dns.GetHostName();
            tt_saveorup = getSaveorUploadRemark(str);
            this.label35.Text = tt_saveorup;

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
            this.listView1.Columns.Add("CONTENT", 500);
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

        //清除标签数据
        private void ClearLabelone()
        {
            this.label13.Text = null;
            this.label15.Text = null;
            this.label17.Text = null;
            this.label21.Text = null;
            this.label23.Text = null;
            this.label33.Text = null;
        }

        //清除所有文本框数据
        private void ClearRichText()
        {
            this.richTextBox1.Text = null;
            CleatListView();
            //this.richTextBox2.Text = null;
            this.richTextBox3.Text = null;
            this.listView1.BackColor = Color.White;
            //this.richTextBox2.BackColor = Color.White;
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


        #region 4、单个文件上传
        //文件选
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

        //重置
        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = null;
            this.textBox2.Text = null;
            this.textBox3.Text = null;
            this.textBox4.Text = null;
        }

        //上传
        private void button3_Click(object sender, EventArgs e)
        {
            if (sip == "172.18.201.2")
            {

                Boolean tt_flag1 = UploadImage4(this.textBox1.Text, this.textBox4.Text, this.textBox3.Text);
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



        #region 5、日志文件上传查询
        //查询确定
        private void button8_Click(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = null;
            //-----日期----
            string tt_date1 = this.dateTimePicker1.Text;
            string tt_date2 = this.dateTimePicker2.Text;

            //----工单----
            string tt_task = "";
            if (!this.textBox7.Text.Equals(""))
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
                            "from ODC_LOGUPLOAD " +
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


        //重置
        private void button9_Click(object sender, EventArgs e)
        {
            this.textBox7.Text = null;
            this.textBox8.Text = null;
            this.dataGridView1.DataSource = null;
        }


        //显示行号
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //序号
            SolidBrush brushOne = new SolidBrush(Color.Red);
            e.Graphics.DrawString(Convert.ToString(e.RowIndex + 1, System.Globalization.CultureInfo.CurrentUICulture), e.InheritedRowStyle.Font, brushOne, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
        }

        #endregion




        #region 6、辅助功能1 读取CSV文件

        //转化为数据集
        public static DataTable Csv2DataSet(string fileFullPath)
        {
            int intColCount = 0;
            bool blnFlag = true;
            DataTable mydt = new DataTable("myTableName");
            DataColumn mydc;
            DataRow mydr;
            string strpath = fileFullPath; //cvs文件路径
            string strline;
            string[] aryline;
            System.IO.StreamReader mysr = new System.IO.StreamReader(strpath, Encoding.Default);
            while ((strline = mysr.ReadLine()) != null)
            {
                aryline = strToAry(strline);//请注意：此处变了
                if (blnFlag)
                {
                    blnFlag = false;
                    intColCount = aryline.Length;
                    for (int i = 0; i < aryline.Length; i++)
                    {
                        mydc = new DataColumn(aryline[i]);
                        mydt.Columns.Add(mydc);
                    }
                }
                mydr = mydt.NewRow();
                for (int i = 0; i < intColCount; i++)
                {
                    try
                    {
                        mydr[i] = aryline[i];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Csv2DataSet转换失败:" + ex.Message);
                    }

                }
                mydt.Rows.Add(mydr);
            }

            mysr.Close(); //解除文件占用
            return mydt;
        }


        //转化为数组
        private static string[] strToAry(string strLine)
        {
            string strItem = "";
            int iFenHao = 0;
            System.Collections.ArrayList lstStr = new System.Collections.ArrayList();
            for (int i = 0; i < strLine.Length; i++)
            {
                string strA = strLine.Substring(i, 1);
                if (strA == "\"")
                {
                    iFenHao = iFenHao + 1;
                }
                if (iFenHao == 2)
                {
                    iFenHao = 0;
                }
                if (strA == "," && iFenHao == 0)
                {
                    lstStr.Add(strItem);
                    strItem = "";
                }
                else
                {
                    strItem = strItem + strA;
                }
            }
            if (strItem.Length > 0)
                lstStr.Add(strItem);
            return (String[])lstStr.ToArray(typeof(string));
        }



        #endregion




        #region 7、数据功能

        private string getSaveorUploadRemark(string tt_user)
        {
            string tt_remark = "";
            string tt_sql = "select count(1),min(Fcode),0 from odc_fhpassword " +
                            "where fname = '"+tt_user+"' ";
            string[] tt_array3 = new string[3];
            tt_array3 = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            tt_remark = tt_array3[1];
            return tt_remark;

        }



        #endregion




        #region 8、CSV文件查看

        //文件选择
        private void button10_Click(object sender, EventArgs e)
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
                this.textBox9.Text = file;
            }
        }


        //数据读取
        private void button11_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            dt = Csv2DataSet(this.textBox9.Text);

            this.dataGridView2.DataSource = dt;

            tt_dt = dt;

            MessageBox.Show("转换完毕");
        }


        //重置
        private void button12_Click(object sender, EventArgs e)
        {
            this.textBox9.Text = null;
            this.dataGridView2.DataSource = null;
        }



        


        #endregion




        #region 9、LOG数据上传数据查看
        //查询确定
        private void button15_Click(object sender, EventArgs e)
        {
            this.dataGridView3.DataSource = null;
            //-----日期----
            string tt_date1 = this.dateTimePicker4.Text;
            string tt_date2 = this.dateTimePicker3.Text;

            //----工单----
            string tt_ftask = "";
            if (!this.textBox10.Text.Equals(""))
            {
                tt_ftask = " and Ftask = '" + this.textBox10.Text + "' ";
            }

            //-----文件名----
            string tt_fsn = "";
            if (!this.textBox11.Text.Equals(""))
            {
                tt_fsn = " and Fsn = '" + this.textBox11.Text + "' ";
            }

            string tt_sql = "select ID,Fsn,Fpc,Fcode,"+
                            "FN01,FN02,FN03,FN04,FN05,FN06,FN07,FN08,FN09,FN10,FN11,FN12,FN13,FN14,FN15,FN16,FN17,FN18,FN19,FN20,"+
                            "Fid,Ftask,Ffilename,Fdate "+
                            "from odc_wifilog "+
                            "where Fdate between '" + tt_date1 + "' and '" + tt_date2 + "' " + tt_ftask + tt_fsn;

            DataSet ds1 = Dataset1.GetDataSet(tt_sql, tt_conn);

            if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                dataGridView3.DataSource = ds1;
                dataGridView3.DataMember = "Table";
            }
            else
            {
                MessageBox.Show("sorry,没有查询到数据！");
            }
        }


        //重置
        private void button14_Click(object sender, EventArgs e)
        {
            this.textBox10.Text = null;
            this.textBox11.Text = null;
            this.dataGridView3.DataSource = null;
        }


        //导出
        private void button13_Click(object sender, EventArgs e)
        {
            //导出EXCEL
            //导出
            #region
            //--------以下数据导出导出---------
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl files (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "Export Excel File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == "")
                return;
            Stream myStream;
            myStream = saveFileDialog.OpenFile();
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));

            string str = "";


            try
            {
                for (int i = 0; i < dataGridView3.ColumnCount; i++)
                {
                    if (i > 0)
                    {
                        str += "\t";
                    }
                    str += dataGridView3.Columns[i].HeaderText;
                }
                sw.WriteLine(str);
                for (int j = 0; j < dataGridView3.Rows.Count; j++)
                {
                    string tempStr = "";
                    for (int k = 0; k < dataGridView3.Columns.Count; k++)
                    {
                        if (k > 0)
                        {
                            tempStr += "\t";
                        }
                        tempStr += dataGridView3.Rows[j].Cells[k].Value.ToString();
                    }
                    sw.WriteLine(tempStr);
                }
                sw.Close();
                myStream.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }

            //--------以下数据导出导出---------
            #endregion
        }

        //显示行号
        private void dataGridView3_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //序号
            SolidBrush brushOne = new SolidBrush(Color.Red);
            e.Graphics.DrawString(Convert.ToString(e.RowIndex + 1, System.Globalization.CultureInfo.CurrentUICulture), e.InheritedRowStyle.Font, brushOne, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
        }


        #endregion




        #region 10、锁定事件
        //工单锁定
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



        //目录锁定
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



        #region 11按钮功能
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
        }


        //选择周期
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tt_interval = Convert.ToInt32(this.comboBox1.Text);
            this.label19.Text = tt_interval.ToString();
        }

        #endregion



        #region 12、主要方法
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


        //主要方法
        private void LogUploadMain()
        {
            //数据初始化
            setRichtexBox("---------开始LOG日志上传----------");
            ClearRichText();



            //第一步 查看是否填写日志目录
            Boolean tt_flag1 = false;
            string tt_frompath = "";
            if (!this.textBox6.Text.Equals(""))
            {
                PutListViewData("第一步：已选择LOG存储目录，goon");
                tt_flag1 = true;
            }
            else
            {
                PutListViewData("第一步：没有选择LOG存储目录,over");
            }




            //第二步 查看是否存在bak目录,不存在就创建一个目录
            Boolean tt_flag2 = false;
            string tt_bpath = this.textBox6.Text + @"\log";
            if (tt_flag1)
            {
                if (!Directory.Exists(tt_bpath))
                {
                    Directory.CreateDirectory(tt_bpath);
                    PutListViewData("第二步：没有log目录，新建一个目录");

                }
                else
                {
                    PutListViewData("第二步：已存在log目录");
                }
                tt_flag2 = true;
            }



            //第三步获取带上传目录信息
            Boolean tt_flag3 = false;
            if (tt_flag1 && tt_flag2)
            {
                string tt_filenumber = GetLogText();
                tt_flag3 = true;
                PutListViewData("第三步：获取到上传文件数目:" + tt_filenumber);
            }


            //第四步获取上传目录
            Boolean tt_flag4 = false;
            string tt_sevicepath = "";
            string tt_taskscode = "";
            if (tt_flag1 && tt_flag2 && tt_flag3)
            {
                //tt_sevicepath = GetUploadPath();
                tt_sevicepath = GetUploadPath2(tt_pcname);
                tt_taskscode = this.textBox5.Text.Trim();
                tt_flag4 = true;
                PutListViewData("第四步：获取到上传文件目录名:" + tt_sevicepath);
                PutListViewData("工单号:" + tt_taskscode);
            }




            //第五步在在目录中循环
            Boolean tt_flag5 = false;
            string tt_task = this.textBox5.Text;
            int tt_loadsuccess = 0;
            int tt_logtosave = 0;   //LOG数据保存的数据库成功数量
            int tt_movefile = 0;
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4)
            {

                PutListViewData("第五步：开始数据文件上传操作");


                string tt_logpath;
                string tt_newname;
                string tt_file;
                string tt_path = this.textBox6.Text;
                int tt_textnumber = 0;
                DirectoryInfo folder = new DirectoryInfo(tt_path);
                foreach (FileInfo file in folder.GetFiles("*.csv"))
                {


                    setRichtexBox("--- " + tt_textnumber.ToString() + " ----");

                    //第1步，获取文件名及路径
                    tt_logpath = file.FullName;
                    tt_frompath = tt_logpath;
                    PutListViewData("1、文件路径：" + tt_logpath);


                    //第2步 获取老的文件名
                    tt_file = file.Name;
                    PutListViewData("2、老文件名：" + tt_file);

                    //第3步 获取新的文件名
                    tt_newname = GetNewName(tt_file);
                    PutListViewData("3、新文件名：" + tt_newname);



                    //第4步 上传数据
                    //------以下加这个数据读取-------
                    tt_dt = Csv2DataSet(tt_logpath);

                    Boolean tt_saveflag = Dataset1.saveDataset2Database2(tt_dt, tt_taskscode, tt_newname,tt_conn);
                    if (tt_saveflag)
                    {
                        tt_logtosave++;
                        PutListViewData("4、数据上传成功.");
                    }
                    else
                    {
                        PutListViewData("4、数据保存上传失败-----!!!!");
                    }




                    //第5步 上传文件
                    Boolean tt_uploadflag = false;
                    if (tt_saveorup == "数据日志上传" && tt_saveflag )
                    {
                        tt_uploadflag = AutoUploadFile(sip, tt_logpath, tt_sevicepath, tt_newname);
                        if (tt_uploadflag )
                        {
                            PutListViewData("5、该文件上传成功.");
                            tt_loadsuccess++;
                        }
                        else
                        {
                            PutListViewData("5、该文件上传失败-----!!!!");
                        }
                    }
                    else
                    {
                        PutListViewData("5、该文件设定不需要上传.");
                    }


                    //第6步 记录数据
                    if (tt_uploadflag)
                    {
                        string tt_sql = "insert odc_logupload (Ftaskcode,Flogname,Fnewname,Fpath,Fdate) " +
                               "values( '" + tt_task + "','" + tt_file + "','" + tt_newname + "','" + tt_sevicepath + "',getdate() )";
                        int tt_dbrecord = Dataset1.ExecCommand(tt_sql, tt_conn);
                        if (tt_dbrecord > 0)
                        {
                            PutListViewData("6、该记录到数据库成功");
                        }
                        else
                        {
                            PutListViewData("6、该记录到数据库失败");
                        }
                    }

                    //第7步获取转移路径及文件名
                    string tt_movepath = tt_bpath + @"\" + tt_file;
                    if (tt_uploadflag)
                    {

                        PutListViewData("7、转移的文件路径名:" + tt_movepath);
                    }


                    //第8步 将数据移到log目录中
                    if (tt_saveflag)
                    {
                        Boolean tt_moveflag = fileMove(tt_frompath, tt_movepath);
                        if (tt_moveflag)
                        {
                            PutListViewData("8、文件转移成功:");
                            tt_movefile++;
                        }
                        else
                        {
                            PutListViewData("8、文件转移不成功:");
                        }
                    }




                    tt_textnumber++;



                }




                if (tt_textnumber == tt_loadsuccess || tt_textnumber == tt_logtosave)
                {
                    tt_flag5 = true;
                    PutListViewData("第五步、数据全部上传成功");
                }
                else
                {
                    PutListViewData("第五步、数据没有全部上传成功，请检查");
                }

            }





            //最后总结
            if (tt_flag1 && tt_flag2 && tt_flag3 && tt_flag4 && tt_flag5)
            {
                this.label15.Text = tt_loadsuccess.ToString();
                this.label23.Text = tt_movefile.ToString();
                this.label33.Text = tt_logtosave.ToString();
                GetLogUploadFile();
                this.listView1.BackColor = Color.Chartreuse;
                //this.richTextBox2.BackColor = Color.Chartreuse;

            }
            else
            {
                this.label15.Text = tt_loadsuccess.ToString();
                this.label23.Text = tt_movefile.ToString();
                this.label33.Text = tt_logtosave.ToString();
                this.listView1.BackColor = Color.Red;
                //this.richTextBox2.BackColor = Color.Red;
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



        #region 13、辅助功能2 日志文件上传

        //或取工作目录文件
        private string GetLogText()
        {

            string tt_path = this.textBox6.Text;
            DirectoryInfo folder = new DirectoryInfo(tt_path);
            int tt_textnumber = 0;
            foreach (FileInfo file in folder.GetFiles("*.csv"))
            {
                this.richTextBox1.Text = file.FullName + "\n" + this.richTextBox1.Text;
                tt_textnumber++;
            }

            this.label13.Text = tt_textnumber.ToString();
            return tt_textnumber.ToString();
        }


        //获取已过站的文件数量
        private void GetLogUploadFile()
        {
            string tt_path = this.textBox6.Text + @"\log";
            DirectoryInfo folder = new DirectoryInfo(tt_path);
            int tt_textnumber = 0;
            foreach (FileInfo file in folder.GetFiles("*.csv"))
            {
                this.richTextBox3.Text = file.FullName + "\n" + this.richTextBox3.Text;
                tt_textnumber++;
            }

            this.label17.Text = tt_textnumber.ToString();
        }


        //获取上传目录
        private string GetUploadPath()
        {
            string tt_uploadpath = "E:\\02 ZGLOGLOAD\\CSV\\";
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string strYM = currentTime.ToString("y");
            tt_uploadpath = tt_uploadpath + strYM;
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


            tt_uploadpath = tt_uploadpath + "\\" + strYM + "\\CSV\\" + tt_date+"\\"+tt_pcname;

            this.label21.Text = tt_uploadpath;
            return tt_uploadpath;

        }



        //richtext加记录
        private void setRichtexBox(string tt_textinfor)
        {
            //this.richTextBox2.Text = this.richTextBox2.Text + tt_textinfor + "\n";
        }



        #endregion


        #region 14、列表操作
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
        private void PutListViewData(string tt_content)
        {
            int i = this.listView1.Items.Count + 1;
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { i.ToString(), tt_content });
            this.listView1.Items.AddRange(p);
            this.listView1.Items[this.listView1.Items.Count - 1].EnsureVisible();
        }

        #endregion










    }
}
