using OAUS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Management;
using System.Configuration;
using System.Collections;
using System.IO;

namespace TVBOX01
{
    class Dataset1
    {
        public static string ConnectionString = "";


        #region 1、基本连接
        //连接设置
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, CommandType cmdType, string cmdText)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
        }



        //打开数据库连接
        private static void Open(SqlConnection connection)
        {
            try
            {

                if (connection == null)
                {
                    connection.Open();
                }
                else if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                else if (connection.State == System.Data.ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }
            }
            catch
            {
                MessageBox.Show("数据库连接异常3412948781！");
            }
        }


        //带返回值open
        public static Boolean opentwo(SqlConnection connection)
        {
            Boolean tt_conect = false;
            try
            {

                if (connection == null)
                {
                    connection.Open();
                }
                else if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                else if (connection.State == System.Data.ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                }

                tt_conect = true;
            }
            catch
            {
                tt_conect = false;
                MessageBox.Show("数据库连接异常,请查看IP网段 办公网or生产网");
            }

            return tt_conect;

        }



        //关闭数据库
        public static void Close(SqlConnection connection)
        {
            if (connection != null)
                connection.Close();

        }
        #endregion


        #region 2、基本查询
        //根据sql语句返回一个DataSet
        #region GetDataSet
        public static DataSet GetDataSet(string sql, string con)
        {
            ConnectionString = con;
            SqlCommand sqlcom = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(sqlcom, conn, CommandType.Text, sql);
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = sqlcom;
                DataSet ds = new DataSet();
                sda.Fill(ds);
                sqlcom.Parameters.Clear();
                conn.Close();
                return ds;
            }
        }
        #endregion


        //带提示信息的返回Dataset
        public static DataSet GetDataSetTwo(string sql, string con)
        {
            SqlConnection connection = new SqlConnection(con);
            Boolean tt_connect = opentwo(connection);
            DataSet ds = new DataSet();
            if (tt_connect)
            {
                using (SqlDataAdapter dap = new SqlDataAdapter(sql, connection))
                {
                    dap.Fill(ds);
                    return ds;
                }
            }
            else
            {
                return ds;
            }
        }

        //执行sql语句，返回影响行数
        #region ExecCommand
        public static int ExecCommand(string sql, string con)
        {
            ConnectionString = con;
            SqlCommand sqlcom = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                PrepareCommand(sqlcom, conn, CommandType.Text, sql);
                int rtn = sqlcom.ExecuteNonQuery();
                sqlcom.Parameters.Clear();
                return rtn;
            }
        }
        #endregion


        //获取单个值 不带连接提示
        public static string[] GetDatasetArray(string tt_sql, string con)
        {
            string[] tt_datasetinfor = new string[3];
            SqlConnection connection = new SqlConnection(con);
            Open(connection); //打开连接
            DataSet ds = new DataSet();
            using (SqlDataAdapter dap = new SqlDataAdapter(tt_sql, connection))
            {
                dap.Fill(ds);
                tt_datasetinfor[0] = ds.Tables[0].Rows[0].ItemArray[0].ToString(); //
                tt_datasetinfor[1] = ds.Tables[0].Rows[0].ItemArray[1].ToString(); //
                tt_datasetinfor[2] = ds.Tables[0].Rows[0].ItemArray[2].ToString(); //
            }
            return tt_datasetinfor;
        }

        //获取单个值 带连接提示， 选择工单使用
        public static string[] GetDatasetArrayTwo(string tt_sql, string con)
        {
            
            SqlConnection connection = new SqlConnection(con);
            Boolean tt_connect = opentwo(connection);
            string[] tt_datasetinfor = new string[3];
            if (tt_connect)
            {
                DataSet ds = new DataSet();
                using (SqlDataAdapter dap = new SqlDataAdapter(tt_sql, connection))
                {
                    dap.Fill(ds);
                    tt_datasetinfor[0] = ds.Tables[0].Rows[0].ItemArray[0].ToString(); 
                    tt_datasetinfor[1] = ds.Tables[0].Rows[0].ItemArray[1].ToString(); 
                    tt_datasetinfor[2] = ds.Tables[0].Rows[0].ItemArray[2].ToString(); 
                    return tt_datasetinfor;
                }
            }
            else
            {
                return tt_datasetinfor;
            }


        }
        #endregion

        
        #region 3、朝歌特殊操作
        //朝歌产品进站操作
        public static Boolean StarinStation(string tt_task,   
                                            string tt_pcba,
                                            string tt_longmac,
                                            string tt_shortmac,
                                            string tt_hwmac,
                                            string tt_barcode,  //32移动码
                                            string tt_nameplate,  //串码
                                            string tt_gyid,
                                            string tt_ccode,
                                            string tt_ncode,
                                            string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {

                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {

                    //第一步插入打印表数据
                    string tt_sql1 = "INSERT INTO ODC_PRUCTPRINT(TASKSCODE,PSN,ISPRINT,CDATE,PREN,ISSD,ZTBJ) " +
                                    "values('" + tt_task + "','" + tt_pcba + "','1',getdate(),'条码打印', '0','1')";

                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();


                    //第二步插入关联表数据
                    string tt_sql2 = "INSERT INTO ODC_ALLLABLE(TASKSCODE,PCBASN,HOSTLABLE,MACLABLE,SPRINTMAN,SPRINTTIME,SMTASKSCODE,BPRINTUSER) " +
                                "values('"+tt_task+"','"+tt_pcba+"','"+tt_nameplate+"','"+tt_shortmac+"','1001',getdate(),'"+tt_barcode+"','"+tt_hwmac+"')";
                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();

                    //第三步插入站位表信息
                    string tt_sql3 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE)" +
                         "values('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'"+tt_gyid+"','301921','"+tt_ncode+"')";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();

                    //第四步更新MAC表状态
                    string tt_sql4 = "update odc_macinfo set fusestate = '1' " +
                                     "where mac ='" + tt_longmac + "' ";
                    command.CommandText = tt_sql4;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    tt_flag = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }

            }
            return tt_flag;
        }





        //朝歌铭牌产品过站
        public static Boolean ZgMpInStation(string tt_task,
                                            string tt_shortmac,
                                            string tt_gyid,
                                            string tt_ccode,
                                            string tt_ncode,
                                            string tt_oldtype,
                                            string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;



                try
                {
                    string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  "+
                                     "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='"+tt_gyid+"',NUSERNAME='301800' "+
                                     "WHERE NAPPLYTYPE is null and taskscode = '"+tt_task+"' and PCBA_PN='"+tt_shortmac+"' ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();


                    string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                     "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'"+tt_gyid+"','301800','" + tt_ncode + "') ";

                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();


                    string tt_sql3 = "update odc_alllable set mprintman = 'ZG001' , mprinttime = getdate(), ageing = '"+tt_oldtype+"' " +
                                     "where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();


                    transaction.Commit();
                    tt_flag = true;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }


            }



            return tt_flag;
        }


        //朝歌彩盒过站
        public static Boolean ZgChInStation(string tt_task,
                                            string tt_hostlable,
                                            string tt_shortmac,
                                            string tt_gyid,
                                            string tt_ccode,
                                            string tt_ncode,
                                            string con)
        {
            Boolean tt_flag = false;
            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();


                //第一步获取流水号
                try
                {
                    string tt_boxlable = "";
                    string tt_beforstr = "";
                    string tt_hostmax = "";
                    string tt_nexthost = "";
                    string tt_id = "";
                    string tt_sql = "select hostqzwh,hostmax,id from ODC_HOSTLABLEOPTIOAN where taskscode = '"+tt_task+"' ";


                    DataSet ds = new DataSet();
                    using (SqlDataAdapter dap = new SqlDataAdapter(tt_sql, connection))
                    {
                        dap.Fill(ds);
                        tt_beforstr = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        tt_hostmax = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                        tt_id = ds.Tables[0].Rows[0].ItemArray[2].ToString();
                    }


                    if (tt_beforstr.Length > 3)
                    {
                    tt_nexthost = (int.Parse(tt_hostmax)+1).ToString();
                    tt_boxlable = tt_beforstr + tt_nexthost.PadLeft(6,'0');
                    }
                    else
                    {
                        tt_boxlable = tt_hostlable;
                    }





                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("SampleTransaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    try
                    {
                        string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                         "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='301800' " +
                                         "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' and PCBA_PN='" + tt_shortmac + "' ";
                        command.CommandText = tt_sql1;
                        command.ExecuteNonQuery();


                        string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                         "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','301800','" + tt_ncode + "') ";

                        command.CommandText = tt_sql2;
                        command.ExecuteNonQuery();


                        string tt_sql3 = "update odc_alllable set boxlable ='" + tt_boxlable + "', hprintman = 'CH001', hprinttime = getdate() " +
                                         "where taskscode ='" + tt_task + "'  and maclable ='" + tt_shortmac + "'  ";

                        command.CommandText = tt_sql3;
                        command.ExecuteNonQuery();


                        string tt_sql4 = "update ODC_HOSTLABLEOPTIOAN set hostmax = '" + tt_nexthost + "' " +
                                         "where taskscode = '" + tt_task + "' and id = " + tt_id;
                        command.CommandText = tt_sql4;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        tt_flag = true;


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                        tt_flag = false;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                        }
                    }




                }
                catch
                {
                    MessageBox.Show("彩盒流水21数据过站异常！");
                    Close(connection);
                }



            }

            return tt_flag;

        }


        //装箱操作
        public static Boolean ZgPackageInStation(string tt_task,
                                                 string tt_pcba,
                                                 string tt_shortmac,
                                                 string tt_gyid,
                                                 string tt_ccode,
                                                 string tt_ncode,
                                                 string tt_package,
                                                 string con)
        {
            Boolean tt_flag = false;
            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                try
                {
                    //string tt_sqlpackageid = "select isnull(max(id),0) maxid  from ODC_PACKAGE ";
                    //string tt_maxid = "";

                    //DataSet ds = new DataSet();
                    //using (SqlDataAdapter dap = new SqlDataAdapter(tt_sqlpackageid, connection))
                    //{
                    //    dap.Fill(ds);
                    //    tt_maxid = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                    //}

                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("SampleTransaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    try
                    {
                        string tt_sql1 = "INSERT INTO ODC_PACKAGE(PASN,PAGESN,TASKCODE,PAGETIME,PAGEPERSON,STATE) " +
                                        " VALUES('"+tt_pcba+"','"+tt_package+"','"+tt_task+"',getdate(),'303201','1' )";
                        command.CommandText = tt_sql1;
                        command.ExecuteNonQuery();


                        string tt_sql2 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                         "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='301800' " +
                                         "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' and PCBA_PN='" + tt_shortmac + "' ";
                        command.CommandText = tt_sql2;
                        command.ExecuteNonQuery();


                        string tt_sql3 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                         "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','301800','" + tt_ncode + "') ";

                        command.CommandText = tt_sql3;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        tt_flag = true;


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                        tt_flag = false;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                        }
                    }
                    

                }
                catch
                {
                    MessageBox.Show("装箱数据过站异常！");
                    Close(connection);
                }

            }



            return tt_flag;
        }



        //打散操作
        public static Boolean ZgBreakupPackage(string tt_task,
                                               string tt_package,
                                               string tt_code,
                                               string con)
        {
            Boolean tt_flag = false;
            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();


                try
                {
                    string tt_sql = "select T1.id, T1.PASN, T2.maclable   from odc_package T1,odc_alllable T2 " +
                                    "where T1.pasn = T2.pcbasn and T1.taskcode = '" + tt_task + 
                                    "' and T1.pagesn = '" + tt_package + "'";

                    
                     SqlDataAdapter dap = new SqlDataAdapter(tt_sql, con);

                      DataSet ds = new DataSet();
                      dap.Fill(ds, "SN");


                      SqlTransaction transaction;
                      transaction = connection.BeginTransaction("SampleTransaction");
                      command.Connection = connection;
                      command.Transaction = transaction;

                      //循环处理
                      if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                      {
                          string tt_updatsql = "";
                          string tt_deletesql = "";
                          for (int i = 0; i < ds.Tables[0].Rows.Count; i++ )
                          {
                              tt_updatsql  = "UPDATE ODC_ROUTINGTASKLIST  SET NCODE='"+tt_code+"',Fremark = '装箱打散退站' "+
                                             "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' "+
                                             "and PCBA_PN='" + ds.Tables[0].Rows[i].ItemArray[2].ToString() + "' ";

                              command.CommandText = tt_updatsql;
                              command.ExecuteNonQuery();


                              tt_deletesql = "DELETE ODC_PACKAGE " +
                                             "WHERE  TASKCODE='" + tt_task + "' and  pagesn ='" + tt_package + "' " +
                                             " and id = " + ds.Tables[0].Rows[i].ItemArray[0].ToString() +
                                             " and pasn = '" + ds.Tables[0].Rows[i].ItemArray[1].ToString() + "' ";

                              command.CommandText = tt_deletesql;
                              command.ExecuteNonQuery();


                          }

                          //提交
                          transaction.Commit();
                          tt_flag = true;

                      }



                }
                catch
                {
                    MessageBox.Show("中箱打散数据过站异常！");
                    Close(connection);
                }

            }
            return tt_flag;
        }




        //贵州获取箱号 带参数存储过程
        public static String stringExecSPCommand(string tt_sql,
                                                 IDataParameter[] paramers,
                                                 string con)
        {
            try
            {
                var sqlcom = new SqlCommand();
                using (var conn = new SqlConnection(con))
                {
                    PrepareCommand(sqlcom, conn, CommandType.StoredProcedure, tt_sql);
                    foreach (var paramer in paramers)
                    {
                        sqlcom.Parameters.Add(paramer);
                    }

                    sqlcom.ExecuteNonQuery();
                    string r = sqlcom.Parameters[sqlcom.Parameters.Count - 1].Value.ToString();
                    sqlcom.Parameters.Clear();
                    return r;
                }
            }
            catch 
            {

                return "";
            }
        }


        #endregion


        #region 4、烽火电信特殊操作
        //烽火WIF单板、BOSA产品进站操作
        public static Boolean FHStarinStation(string tt_task,
                                              string tt_username,
                                              string tt_pcba,
                                              string tt_longmac,
                                              string tt_shortmac,
                                              string tt_gpsn,
                                              string tt_barcode,  //32移动码
                                              string tt_bosa,
                                              string tt_gyid,
                                              string tt_ccode,
                                              string tt_ncode,
                                              string tt_svers,
                                              string tt_bosatype,
                                              string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {

                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {

                    //第一步插入打印表数据
                    string tt_sql1 = "INSERT INTO ODC_PRUCTPRINT(TASKSCODE,PSN,ISPRINT,CDATE,PREN,ISSD,ZTBJ) " +
                                    "values('" + tt_task + "','" + tt_pcba + "','1',getdate(),'条码打印', '0','1')";

                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();

                    //第二步插入关联表数据
                    string tt_sql2 = "INSERT INTO ODC_ALLLABLE(TASKSCODE,Hprintman,PCBASN,HOSTLABLE,MACLABLE,SPRINTMAN,SPRINTTIME,SMTASKSCODE,BPRINTUSER,BOSASN,shelllable,Fsegment1,Fsegment2) " +
                                "values('" + tt_task + "','" + tt_task + "','" + tt_pcba + "','" + tt_shortmac + "','" + tt_shortmac + "','" 
                                       + tt_username + "',getdate(),'" + tt_barcode + "','" + tt_longmac + "','"+tt_bosa+"','"+tt_gpsn+"','"
                                       + tt_svers + "','" + tt_bosatype + "')";
                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();

                    //第三步插入站位表信息
                    string tt_sql3 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE)" +
                         "values('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','"+tt_username+"','" + tt_ncode + "')";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();

                    //第四步更新MAC表状态
                    string tt_sql4 = "update odc_macinfo set fusestate = '1' " +
                                     "where mac ='" + tt_longmac + "' ";
                    command.CommandText = tt_sql4;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    tt_flag = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }

            }
            return tt_flag;
        }


        //烽火wifi过站 打MAC标签
        public static Boolean FhMacPassStation(string tt_task,
                                               string tt_username,
                                               string tt_shortmac,
                                               string tt_gyid,
                                               string tt_ccode,
                                               string tt_ncode,
                                               string tt_oldtype,
                                               string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;



                try
                {
                    string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                     "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='" + tt_username + "' " +
                                     "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' and PCBA_PN='" + tt_shortmac + "' ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();


                    string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                     "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();


                    string tt_sql3 = "update odc_alllable set mprintman = '" + tt_username + "' , mprinttime = getdate(), ageing = '" + tt_oldtype + "' " +
                                     "where hprintman = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();


                    transaction.Commit();
                    tt_flag = true;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }


            }



            return tt_flag;
        }



        //烽火wifi过站 打印铭牌以及二维码标签
        public static Boolean FhMpPassStation(string tt_task,
                                               string tt_username,
                                               string tt_shortmac,
                                               string tt_gyid,
                                               string tt_ccode,
                                               string tt_ncode,
                                               string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;



                try
                {
                    string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                     "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='" + tt_username + "' " +
                                     "WHERE  PCBA_PN='" + tt_shortmac + "' and  NAPPLYTYPE is null ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();


                    string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                     "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();


                    string tt_sql3 = "update odc_alllable set bprintman = '" + tt_username + "' , bprinttime = getdate() " +
                                     "where hprintman = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();


                    transaction.Commit();
                    tt_flag = true;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }


            }



            return tt_flag;
        }







        //烽火wifi 获取生产序列号
        public static Boolean FhwifSnInStation(string tt_smalltask,
                                               string tt_bigtask,
                                               string tt_username,
                                               string tt_hostlable,
                                               string tt_shortmac,
                                               string tt_gyid,
                                               string tt_ccode,
                                               string tt_ncode,
                                               string con)
        {
            Boolean tt_flag = false;
            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();


                //第一步获取流水号
                try
                {
                    string tt_boxlable = "";
                    string tt_beforstr = "";
                    string tt_hostmax = "";
                    string tt_nexthost = "";
                    string tt_id = "";
                    string tt_sql = "select hostqzwh,hostmax,id from ODC_HOSTLABLEOPTIOAN where taskscode = '" + tt_smalltask + "' ";


                    DataSet ds = new DataSet();
                    using (SqlDataAdapter dap = new SqlDataAdapter(tt_sql, connection))
                    {
                        dap.Fill(ds);
                        tt_beforstr = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        tt_hostmax = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                        tt_id = ds.Tables[0].Rows[0].ItemArray[2].ToString();
                    }


                    if (tt_beforstr.Length > 3)
                    {
                        tt_nexthost = (int.Parse(tt_hostmax) + 1).ToString();
                        tt_boxlable = tt_beforstr + tt_nexthost.PadLeft(4, '0');
                    }
                    else
                    {
                        tt_boxlable = tt_hostlable;
                    }





                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("SampleTransaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    try
                    {
                        string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                         "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='"+tt_username+"' " +
                                         "WHERE NAPPLYTYPE is null and PCBA_PN='" + tt_shortmac + "' ";
                        command.CommandText = tt_sql1;
                        command.ExecuteNonQuery();


                        string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                         "VALUES ('" + tt_smalltask + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                        command.CommandText = tt_sql2;
                        command.ExecuteNonQuery();


                        string tt_sql3 = "update odc_alllable set boxlable ='" + tt_boxlable + "', hprinttime = getdate(), " +
                                                             " taskscode = '" + tt_smalltask + "', hostlable = '" + tt_boxlable + "' " +
                                         "where hprintman ='" + tt_bigtask + "' and taskscode = '"+tt_smalltask+"'  and maclable ='" + tt_shortmac + "'  ";

                        command.CommandText = tt_sql3;
                        command.ExecuteNonQuery();


                        string tt_sql4 = "update ODC_HOSTLABLEOPTIOAN set hostmax = '" + tt_nexthost + "' " +
                                         "where taskscode = '" + tt_smalltask + "' and id = " + tt_id;
                        command.CommandText = tt_sql4;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        tt_flag = true;


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                        tt_flag = false;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                        }
                    }




                }
                catch
                {
                    MessageBox.Show("获取生产序列号，数据过站异常！");
                    Close(connection);
                }



            }

            return tt_flag;

        }






        //烽火wifi过站 打印彩盒标签
        public static Boolean FhCHPassStation(string tt_task,
                                               string tt_username,
                                               string tt_shortmac,
                                               string tt_gyid,
                                               string tt_ccode,
                                               string tt_ncode,
                                               string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;



                try
                {
                    string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                     "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='" + tt_username + "' " +
                                     "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' and PCBA_PN='" + tt_shortmac + "' ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();


                    string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                     "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();


                    string tt_sql3 = "update odc_alllable set productman = '" + tt_username + "' , producttime = getdate() " +
                                     "where hprintman = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();


                    transaction.Commit();
                    tt_flag = true;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }


            }



            return tt_flag;
        }





        // 烽火wifi 装箱操作
        public static Boolean FhPackageInStation(string tt_task,
                                                 string tt_username,
                                                 string tt_pcba,
                                                 string tt_shortmac,
                                                 string tt_gyid,
                                                 string tt_ccode,
                                                 string tt_ncode,
                                                 string tt_package,
                                                 string con)
        {
            Boolean tt_flag = false;
            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                try
                {
                    
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("SampleTransaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    try
                    {
                        string tt_sql1 = "INSERT INTO ODC_PACKAGE(PASN,PAGESN,TASKCODE,PAGETIME,PAGEPERSON,STATE) " +
                                        " VALUES('" + tt_pcba + "','" + tt_package + "','" + tt_task + "',getdate(),'" + tt_username + "','1' )";
                        command.CommandText = tt_sql1;
                        command.ExecuteNonQuery();


                        string tt_sql2 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                         "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='" + tt_username + "' " +
                                         "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' and PCBA_PN='" + tt_shortmac + "' ";
                        command.CommandText = tt_sql2;
                        command.ExecuteNonQuery();


                        string tt_sql3 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                         "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                        command.CommandText = tt_sql3;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        tt_flag = true;


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                        tt_flag = false;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                        }
                    }


                }
                catch
                {
                    MessageBox.Show("装箱数据过站异常！");
                    Close(connection);
                }

            }

            return tt_flag;
        }



        //烽火WIFI打散操作
        public static Boolean FhBreakupPackage(string tt_task,
                                               string tt_package,
                                               string tt_code,
                                               string con)
        {
            Boolean tt_flag = false;
            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();


                try
                {
                    string tt_sql = "select T1.Fid, T1.PASN, T2.maclable, T2.hostlable, convert(varchar, T1.pagetime, 120) pagetime  "+ 
                                     "from odc_package T1,odc_alllable T2 " +
                                    "where T1.pasn = T2.pcbasn and T1.taskcode = '" + tt_task +
                                    "' and T1.pagesn = '" + tt_package + "'";


                    SqlDataAdapter dap = new SqlDataAdapter(tt_sql, con);

                    DataSet ds = new DataSet();
                    dap.Fill(ds, "SN");


                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("SampleTransaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    //循环处理
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        string tt_updatsql = "";
                        string tt_deletesql = "";
                        string tt_insertsql = "";
                        string tt_pasn = "";
                        string tt_mac = "";
                        string tt_hostlable = "";
                        string tt_pagetime = "";

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            tt_pasn = ds.Tables[0].Rows[i].ItemArray[1].ToString();
                            tt_mac = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                            tt_hostlable = ds.Tables[0].Rows[i].ItemArray[3].ToString();
                            tt_pagetime = ds.Tables[0].Rows[i].ItemArray[4].ToString();

                            tt_updatsql = "UPDATE ODC_ROUTINGTASKLIST  SET NCODE='" + tt_code + "',Fremark = '装箱打散退站' " +
                                           "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' " +
                                           "and PCBA_PN='" + ds.Tables[0].Rows[i].ItemArray[2].ToString() + "' ";
                           

                            command.CommandText = tt_updatsql;
                            command.ExecuteNonQuery();


                            
                            tt_deletesql = "DELETE ODC_PACKAGE " +
                                           "WHERE  TASKCODE='" + tt_task + "' and  pagesn ='" + tt_package + "' " +
                                           " and Fid = " + ds.Tables[0].Rows[i].ItemArray[0].ToString() +
                                           " and pasn = '" + ds.Tables[0].Rows[i].ItemArray[1].ToString() + "' ";

                            command.CommandText = tt_deletesql;
                            command.ExecuteNonQuery();


                            tt_insertsql = "insert into  odc_pagebreakup (taskcode,pagesn,pasn,maclable,hostlable,pagetime,fdate) " +
                              "values('" + tt_task + "','" + tt_package + "','" + tt_pasn + "','" + tt_mac + "','" + tt_hostlable + "','"+tt_pagetime+"',getdate() ) ";
                            command.CommandText = tt_insertsql;
                            command.ExecuteNonQuery();

                        }

                        //提交
                        transaction.Commit();
                        tt_flag = true;

                    }



                }
                catch
                {
                    MessageBox.Show("中箱打散数据过站异常！");
                    Close(connection);
                }

            }
            return tt_flag;
        }




        #endregion

        
        #region 5、烽火移动特殊操作

        //烽火移动过站 打印设备标签
        public static Boolean FhUnPassStation(string tt_task,
                                               string tt_username,
                                               string tt_shortmac,
                                               string tt_gyid,
                                               string tt_ccode,
                                               string tt_ncode,
                                               string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;
                
                try
                {
                    string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                     "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='" + tt_username + "' " +
                                     "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' and PCBA_PN='" + tt_shortmac + "' ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();


                    string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                     "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();


                    string tt_sql3 = "update odc_alllable set mprintman = '" + tt_username + "' , mprinttime = getdate() " +
                                     "where hprintman = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();


                    transaction.Commit();
                    tt_flag = true;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }


            }



            return tt_flag;
        }



        //烽火移动过站  打印移动标签
        public static Boolean FhYDPassStation(string tt_task,
                                               string tt_username,
                                               string tt_shortmac,
                                               string tt_gyid,
                                               string tt_ccode,
                                               string tt_ncode,
                                               string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;



                try
                {
                    string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                     "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='" + tt_username + "' " +
                                     "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' and PCBA_PN='" + tt_shortmac + "' ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();


                    string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                     "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();


                    string tt_sql3 = "update odc_alllable set bprintman = '" + tt_username + "' , bprinttime = getdate() " +
                                     "where hprintman = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();


                    transaction.Commit();
                    tt_flag = true;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }


            }



            return tt_flag;
        }




        //烽火移动 获取生产序列号
        public static int FhYDSnInStation(string tt_smalltask,
                                               string tt_bigtask,
                                               string tt_username,
                                               string tt_hostlable,
                                               string tt_shortmac,
                                               string tt_shanghaicheck,
                                               string tt_gyid,
                                               string tt_ccode,
                                               string tt_ncode,
                                               string con)
        {
            int tt_intno = 0;
            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();


                //第一步获取流水号
                try
                {
                    string tt_boxlable = "";
                    string tt_beforstr = "";
                    string tt_hostmax = "";
                    string tt_hostmode = "";
                    string tt_nexthost = "";
                    string tt_shanghailabel = "";
                    string tt_shanghainexthost = "";
                    string tt_id = "";
                    string tt_sql = "select hostqzwh,hostmax,id,hostmode from ODC_HOSTLABLEOPTIOAN where taskscode = '" + tt_smalltask + "' ";


                    DataSet ds = new DataSet();
                    using (SqlDataAdapter dap = new SqlDataAdapter(tt_sql, connection))
                    {
                        dap.Fill(ds);
                        tt_beforstr = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        tt_hostmax = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                        tt_id = ds.Tables[0].Rows[0].ItemArray[2].ToString();
                        tt_hostmode = ds.Tables[0].Rows[0].ItemArray[3].ToString();
                    }


                    if (tt_beforstr.Length > 3)
                    {
                        tt_nexthost = (int.Parse(tt_hostmax) + 1).ToString();
                        tt_boxlable = tt_beforstr + tt_nexthost.PadLeft(4, '0');

                        if (tt_shanghaicheck != "")
                        {
                            tt_shanghainexthost = string.Format("{0:X}", int.Parse(tt_nexthost));
                            tt_shanghailabel = tt_hostmode + tt_shanghainexthost.PadLeft(4, '0');
                        }
                    }
                    else
                    {
                        tt_boxlable = tt_hostlable;
                    }

                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("SampleTransaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    try
                    {
                        string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                         "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='" + tt_username + "' " +
                                         "WHERE NAPPLYTYPE is null and PCBA_PN='" + tt_shortmac + "' ";
                        command.CommandText = tt_sql1;
                        command.ExecuteNonQuery();


                        string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                         "VALUES ('" + tt_smalltask + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                        command.CommandText = tt_sql2;
                        command.ExecuteNonQuery();


                        string tt_sql3 = "update odc_alllable set boxlable ='" + tt_boxlable + "', hprinttime = getdate(), " +
                                                             " taskscode = '" + tt_smalltask + "', hostlable = '" + tt_boxlable + "', productlable = '" + tt_shanghailabel + "'" +
                                         "where hprintman ='" + tt_bigtask + "' and taskscode = '" + tt_bigtask + "'  and maclable ='" + tt_shortmac + "'  ";

                        command.CommandText = tt_sql3;
                        command.ExecuteNonQuery();


                        string tt_sql4 = "update ODC_HOSTLABLEOPTIOAN set hostmax = '" + tt_nexthost + "' " +
                                         "where taskscode = '" + tt_smalltask + "' and id = " + tt_id;
                        command.CommandText = tt_sql4;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        tt_intno = int.Parse(tt_nexthost);


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                        tt_intno = -1;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("获取生产序列号，数据过站异常！");
                    Close(connection);
                }
            }

            return tt_intno;
        }

        //烽火移动过站 重打标签过站
        public static Boolean FhUnPassStationI(string tt_task,
                                               string tt_username,
                                               string tt_shortmac,
                                               string tt_gyid,
                                               string tt_ccode,
                                               string tt_ncode,
                                               string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;
                                
                try
                {
                    string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                     "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='" + tt_username + "' " +
                                     "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' and PCBA_PN='" + tt_shortmac + "' ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();


                    string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                     "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();


                    string tt_sql3 = "update odc_alllable set mprintman = '" + tt_username + "' , mprinttime = getdate() " +
                                     "where taskscode = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();


                    transaction.Commit();
                    tt_flag = true;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }


            }



            return tt_flag;
        }

        //烽火移动过站 打印彩盒标签
        public static Boolean FhYDCHPassStation(string tt_task,
                                               string tt_username,
                                               string tt_shortmac,
                                               string tt_gyid,
                                               string tt_ccode,
                                               string tt_ncode,
                                               string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;



                try
                {
                    string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                     "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='" + tt_username + "' " +
                                     "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' and PCBA_PN='" + tt_shortmac + "' ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();


                    string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                     "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();


                    string tt_sql3 = "update odc_alllable set productman = '" + tt_username + "' , producttime = getdate() " +
                                     "where hprintman = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();


                    transaction.Commit();
                    tt_flag = true;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }


            }



            return tt_flag;
        }



        //烽火站位跳转 
        public static Boolean FhCodeSkip(string tt_skipcode,
                                         string tt_rowid,
                                         string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                //开始跳站
                try
                {
                    //第一步获取当前站位信息
                    string tt_task = "";
                    string tt_mac = "";
                    string tt_ncode = "";
                    string tt_gyid = "";
                    string tt_username = "PR001";
                    string tt_remark = "PR001站位跳转";
                    string tt_sql = "select taskscode,pcba_pn,Ncode,cuserid  from  odc_routingtasklist " +
                                    "where id = "+ tt_rowid;

                    DataSet ds = new DataSet();
                    using (SqlDataAdapter dap = new SqlDataAdapter(tt_sql, connection))
                    {
                        dap.Fill(ds);
                        tt_task = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        tt_mac = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                        tt_ncode = ds.Tables[0].Rows[0].ItemArray[2].ToString();
                        tt_gyid = ds.Tables[0].Rows[0].ItemArray[3].ToString();
                        if (tt_gyid == "") tt_gyid = "1100";

                    }

                    //开始过站
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("SampleTransaction");
                    command.Connection = connection;
                    command.Transaction = transaction;




                    try
                    {
                        string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                         "SET ENDDATE = getdate(),Fremark ='"+tt_remark+"',NAPPLYTYPE=0, NUSERID=" + tt_gyid + ",NUSERNAME='" + tt_username + "' " +
                                         "WHERE  id =" + tt_rowid + " ";
                        command.CommandText = tt_sql1;
                        command.ExecuteNonQuery();


                        string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                         "VALUES ('" + tt_task + "','" + tt_mac + "','" + tt_ncode + "','0',getdate()," + tt_gyid + ",'" + tt_username + "','" + tt_skipcode + "') ";

                        command.CommandText = tt_sql2;
                        command.ExecuteNonQuery();

                        string tt_sql3 = "INSERT INTO ODC_STAJUMP (TASKSCODE,MAC,LASTNCODE,JUMPTIME,NOWCODE) " +
                                         "VALUES ('" + tt_task + "','" + tt_mac + "','" + tt_ncode + "',getdate(),'" + tt_skipcode + "') ";

                        command.CommandText = tt_sql3;
                        command.ExecuteNonQuery();


                        transaction.Commit();
                        tt_flag = true;


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                        tt_flag = false;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("烽火过站异常，数据过站异常！");
                    Close(connection);
                }
            }

            return tt_flag;
        }




        #endregion
        

        #region 6、烽火延迟制造操作
        //烽火延迟制造虚拟工单转换为真实工单，获取真MAC进站
        public static Boolean FhYcMadeinStation(string tt_task1,  //真工单
                                                string tt_task2,  //虚拟工单
                                                string tt_oldmac,  //虚拟MAC
                                                string tt_oldcode, //虚拟MAC最后站位
                                                string tt_oldgyid, //虚拟MAC流程
                                             
                                              string tt_id,   //行ID
                                              string tt_pcba,      //单板号
                                              string tt_shortmac,  //短MAC
                                              string tt_longmac,   //长MAC
                                              string tt_gpsn,      //GPSN
                                              string tt_barcode,  //32移动码

                                              string tt_username,  //软件登录用户名
                                              string tt_newgyid,  //流程
                                              string tt_newccode, //待测站位
                                              string tt_newncode,  //进站站位
                                              string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {

                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {

                    //第一步插入打印表数据
                    string tt_sql1 = "INSERT INTO ODC_PRUCTPRINT(TASKSCODE,PSN,ISPRINT,CDATE,PREN,PC,ISSD,ZTBJ)  " +
                              "values('" + tt_task1 + "','" + tt_pcba + "','1',getdate(),'延迟制造', '" + tt_shortmac + "','" + tt_task2 + "','" + tt_oldmac + "')";

                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();



                    //第二步 更新关联表数据

                    string tt_sql2 = "UPDATE ODC_ALLLABLE SET TASKSCODE ='"+tt_task1+"',Hprintman = '"+tt_task1+"', " +   //工单更新
                                     "HOSTLABLE ='" + tt_shortmac + "',MACLABLE='" + tt_shortmac + "',BPRINTUSER = '" + tt_longmac + "'," +  //MAC更新
                                     "SMTASKSCODE = '" + tt_barcode + "',shelllable='" + tt_gpsn + "'," +     //移动串号,GPSN
                                     "STATE = '"+tt_task2+"',WEIGHT='"+tt_oldmac+"'," +    //虚拟工单、MAC
                                     "SPRINTMAN = '"+tt_username+"',SPRINTTIME = getdate() "+             //用户名,时间
                                     "WHERE ID = " + tt_id + " and PCBASN = '" + tt_pcba + "' and TASKSCODE = '" + tt_task2 + "' and Hprintman = '"+tt_task2+"' ";

                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();



                    //第三步 真MAC插入站位表信息
                    string tt_remark1 = "虚拟MAC:" + tt_oldmac;
                    string tt_sql3 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE,Fremark)  " +
                         "values('" + tt_task1 + "','" + tt_shortmac + "','" + tt_newccode + "','0',getdate(),'" + tt_newgyid + "','" + tt_username + "','" + tt_newncode + "','" + tt_remark1 + "')";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();



                    //第四步更新MAC表状态
                    string tt_sql4 = "update odc_macinfo set fusestate = '1' " +
                                     "where mac ='" + tt_longmac + "' ";
                    command.CommandText = tt_sql4;
                    command.ExecuteNonQuery();


                    //第五步 虚拟MAC待测站位更新
                    string tt_remark2 = "新MAC:"+tt_shortmac;
                    string tt_sql5 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                     "SET ENDDATE = getdate(),Fremark ='" + tt_remark2 + "',NAPPLYTYPE=1, NUSERID=" + tt_oldgyid + ",NUSERNAME='" + tt_username + "' " +
                                     "WHERE  taskscode = '"+tt_task2+"' and pcba_pn = '"+tt_oldmac+"' and Napplytype is null ";
                    command.CommandText = tt_sql5;
                    command.ExecuteNonQuery();


                    //第六步 虚拟MAC新增站位
                    string tt_sql6 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                        "VALUES ('" + tt_task2 + "','" + tt_oldmac + "','" + tt_oldcode + "','0',getdate()," + tt_oldgyid + ",'" + tt_username + "','8880') ";
                    command.CommandText = tt_sql6;
                    command.ExecuteNonQuery();


                    //第七步 虚拟MAC出库记录
                    string tt_sql7 = "insert into odc_virtualmac(Taskscode,Pcba,Mac1,Mac2,Gyid1,Gyid2,Ncode1,Ncode2,Fremark,Fdate)  "+
                         "values('"+tt_task2+"','"+tt_pcba+"','"+tt_oldmac+"','"+tt_shortmac+"','"+tt_oldgyid+"','"+tt_newgyid+"','"+tt_oldcode+"','"+tt_newncode+"','"+tt_task1+"',getdate())";
                    command.CommandText = tt_sql7;
                    command.ExecuteNonQuery();
                    
                    
                    transaction.Commit();
                    tt_flag = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }

            }
            return tt_flag;
        }



        //烽火延迟制造 打印单板临时标签
        public static Boolean FhYcMadePrintPcbaLabel(string tt_task,
                                               string tt_username,
                                               string tt_shortmac,
                                               string tt_gyid,
                                               string tt_ccode,
                                               string tt_ncode,
                                               string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;



                try
                {
                    string tt_sql1 = "UPDATE ODC_ROUTINGTASKLIST  " +
                                     "SET ENDDATE = getdate(),STATUS ='0',NAPPLYTYPE='1',NUSERID='" + tt_gyid + "',NUSERNAME='" + tt_username + "' " +
                                     "WHERE NAPPLYTYPE is null and taskscode = '" + tt_task + "' and PCBA_PN='" + tt_shortmac + "' ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();


                    string tt_sql2 = "INSERT INTO ODC_ROUTINGTASKLIST (TASKSCODE,PCBA_PN,CCODE,STATUS,CREATETIME,CUSERID,CUSERNAME,NCODE) " +
                                     "VALUES ('" + tt_task + "','" + tt_shortmac + "','" + tt_ccode + "','0',getdate(),'" + tt_gyid + "','" + tt_username + "','" + tt_ncode + "') ";

                    command.CommandText = tt_sql2;
                    command.ExecuteNonQuery();


                    string tt_sql3 = "update odc_alllable set E8C = '1'  " +
                                     "where hprintman = '" + tt_task + "' and maclable = '" + tt_shortmac + "' ";
                    command.CommandText = tt_sql3;
                    command.ExecuteNonQuery();


                    transaction.Commit();
                    tt_flag = true;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }


            }



            return tt_flag;
        }




        #endregion

        
        #region 7、LOG数据上传操作

        //数据DATASET保存 测试
        public static Boolean saveDataset2Database(DataTable tt_dt, string con)
        {
            Boolean tt_flag = false;

            int tt_datablenum = tt_dt.Rows.Count;

            using (SqlConnection connection = new SqlConnection(con))
            {

                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();


                string tt_n01 = "";
                string tt_n02 = "";
                string tt_n03 = "";
                string tt_n04 = "";
                string tt_n05 = "";
                string tt_n06 = "";
                string tt_n07 = "";
                string tt_n08 = "";
                string tt_n09 = "";

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {

                    string tt_sql = "";

                    for (int i = 0; i < tt_dt.Rows.Count; i++)
                    {
                        //for (int j = 0; j < tt_dt.Columns.Count; j++)
                        //{
                        //    Console.WriteLine(tt_dt.Rows[i][j].ToString());
                        //}


                        tt_n01 = tt_dt.Rows[i][0].ToString();
                        tt_n02 = tt_dt.Rows[i][1].ToString();
                        tt_n03 = tt_dt.Rows[i][2].ToString();
                        tt_n04 = tt_dt.Rows[i][3].ToString();
                        tt_n05 = tt_dt.Rows[i][4].ToString();
                        tt_n06 = tt_dt.Rows[i][5].ToString();
                        tt_n07 = tt_dt.Rows[i][6].ToString();
                        tt_n08 = tt_dt.Rows[i][7].ToString();
                        tt_n09 = tt_dt.Rows[i][8].ToString();



                        tt_sql = "insert into Table_3 (N01,N02,N03,N04,N05,N06,N07,N08,N09) " +
                            "values('" + tt_n01 + "','" + tt_n02 + "','" + tt_n03 + "','" + tt_n04 + "','" + tt_n05 + "','" + tt_n06 + "','" + tt_n07 + "','" + tt_n08 + "','" + tt_n09 + "')";

                        command.CommandText = tt_sql;
                        command.ExecuteNonQuery();

                    }


                    //提交
                    transaction.Commit();
                    tt_flag = true;
                }
                catch
                {
                    MessageBox.Show("LOG数据上传异常！");
                    Close(connection);
                }

            }


            return tt_flag;
        }


        //数据DATASET保存
        public static Boolean saveDataset2Database2(DataTable tt_dt, string tt_taskcode, string tt_filenamme, string con)
        {
            Boolean tt_flag = false;

            int tt_datatablenum1 = tt_dt.Rows.Count;
            int tt_datatablenum2 = tt_dt.Columns.Count;


            if (tt_datatablenum1 > 0 && tt_datatablenum2 > 0)
            {

                #region  开始数据操作
                using (SqlConnection connection = new SqlConnection(con))
                {

                    opentwo(connection); //打开连接
                    SqlCommand command = connection.CreateCommand();


                    string tt_n01 = "";
                    string tt_n02 = "";
                    string tt_n03 = "";
                    string tt_n04 = "";
                    string tt_n05 = "";
                    string tt_n06 = "";
                    string tt_n07 = "";
                    string tt_n08 = "";
                    string tt_n09 = "";
                    string tt_n10 = "";

                    string tt_n11 = "";
                    string tt_n12 = "";
                    string tt_n13 = "";
                    string tt_n14 = "";
                    string tt_n15 = "";
                    string tt_n16 = "";
                    string tt_n17 = "";
                    string tt_n18 = "";
                    string tt_n19 = "";
                    string tt_n20 = "";

                    string tt_n21 = "";
                    string tt_n22 = "";
                    string tt_n23 = "";

                    int tt_rownum = 0;

                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("SampleTransaction");
                    command.Connection = connection;
                    command.Transaction = transaction;

                    try
                    {

                        string tt_sql = "";

                        for (int i = 0; i < tt_dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < tt_dt.Columns.Count; j++)
                            {
                                #region 列赋值
                                if (j == 0) tt_n01 = tt_dt.Rows[i][0].ToString();
                                if (j == 1) tt_n02 = tt_dt.Rows[i][1].ToString();
                                if (j == 2) tt_n03 = tt_dt.Rows[i][2].ToString();
                                if (j == 3) tt_n04 = tt_dt.Rows[i][3].ToString();
                                if (j == 4) tt_n05 = tt_dt.Rows[i][4].ToString();
                                if (j == 5) tt_n06 = tt_dt.Rows[i][5].ToString();
                                if (j == 6) tt_n07 = tt_dt.Rows[i][6].ToString();
                                if (j == 7) tt_n08 = tt_dt.Rows[i][7].ToString();
                                if (j == 8) tt_n09 = tt_dt.Rows[i][8].ToString();
                                if (j == 9) tt_n10 = tt_dt.Rows[i][9].ToString();

                                if (j == 10) tt_n11 = tt_dt.Rows[i][10].ToString();
                                if (j == 11) tt_n12 = tt_dt.Rows[i][11].ToString();
                                if (j == 12) tt_n13 = tt_dt.Rows[i][12].ToString();
                                if (j == 13) tt_n14 = tt_dt.Rows[i][13].ToString();
                                if (j == 14) tt_n15 = tt_dt.Rows[i][14].ToString();
                                if (j == 15) tt_n16 = tt_dt.Rows[i][15].ToString();
                                if (j == 16) tt_n17 = tt_dt.Rows[i][16].ToString();
                                if (j == 17) tt_n18 = tt_dt.Rows[i][17].ToString();
                                if (j == 18) tt_n19 = tt_dt.Rows[i][18].ToString();
                                if (j == 19) tt_n20 = tt_dt.Rows[i][19].ToString();

                                if (j == 20) tt_n01 = tt_dt.Rows[i][20].ToString();
                                if (j == 21) tt_n02 = tt_dt.Rows[i][21].ToString();
                                if (j == 22) tt_n03 = tt_dt.Rows[i][22].ToString();
                                #endregion
                            }


                            tt_rownum++;

                            #region 插入数据
                            tt_sql = "insert into odc_wifilog (Fsn,Fpc,Fcode, " +
                                      "FN01,FN02,FN03,FN04,FN05,FN06,FN07,FN08,FN09,FN10,FN11,FN12,FN13,FN14,FN15,FN16,FN17,FN18,FN19,FN20, " +
                                                              "Fid,Ftask,Ffilename,Fdate) " +
                                "values('" + tt_n01 + "','" + tt_n02 + "','" + tt_n03 + "', " +
                                "'" + tt_n04 + "','" + tt_n05 + "','" + tt_n06 + "','" + tt_n07 + "','" + tt_n08 + "','" + tt_n09 + "','" + tt_n10 + "','" + tt_n11 + "','" + tt_n12 + "','" + tt_n13 + "'," +
                                "'" + tt_n14 + "','" + tt_n15 + "','" + tt_n16 + "','" + tt_n17 + "','" + tt_n18 + "','" + tt_n19 + "','" + tt_n20 + "','" + tt_n21 + "','" + tt_n22 + "','" + tt_n23 + "'," +
                                 "'" + tt_rownum.ToString() + "','" + tt_taskcode + "','" + tt_filenamme + "',getdate() )";

                            command.CommandText = tt_sql;
                            command.ExecuteNonQuery();
                            #endregion

                        }


                        //提交
                        transaction.Commit();
                        tt_flag = true;
                    }
                    catch
                    {
                        MessageBox.Show("CSV数据保存异常！");
                        Close(connection);
                    }

                }
                #endregion
            }

            return tt_flag;
        }




        #endregion


        #region 8、产品自助分单操作

        //新增制造单
        public static Boolean Fhzztasksmade(string tt_taskscode, string tt_taskstate, string tt_taskdate, string tt_customer, string tt_pid, string tt_product_name,
                                            string tt_pon_name, string tt_tasksquantity, string tt_stardate, string tt_gyid, string tt_issd, string tt_pccount,
                                            string tt_teamgroupid, string tt_softwareversion, string tt_tasktype, string tt_areacode, string tt_sver, string tt_svert,
                                            string tt_svers, string tt_modelname, string tt_vendorid, string tt_onumodel, string tt_flhratio, string tt_flgratio,
                                            string tt_fec, string tt_remark, string tt_bosatype, string tt_gyid2 ,string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    string tt_sql1 = "insert into odc_tasks (taskscode,taskstate,taskdate,customer,pid,product_name,pon_name," +
                                     "tasksquantity,stardate,gyid,gyid2,issd,pccount,teamgroupid,softwareversion,tasktype,areacode," +
                                     "sver,svert,svers,modelname,vendorid,onumodel,flhratio,flgratio,fec,bosatype,fremark,fremark_date)" +
                                     "values ('" + tt_taskscode + "','" + tt_taskstate + "','" + tt_taskdate  + "'," +
                                             "'" + tt_customer + "','" + tt_pid + "','" + tt_product_name + "'," +
                                             "'" + tt_pon_name + "','" + tt_tasksquantity + "','" + tt_stardate + "'," +
                                             "'" + tt_gyid + "','" + tt_gyid2 + "','" + tt_issd + "','" + tt_pccount + "'," +
                                             "'" + tt_teamgroupid + "','" + tt_softwareversion + "','" + tt_tasktype + "'," +
                                             "'" + tt_areacode + "','" + tt_sver + "','" + tt_svert + "'," +
                                             "'" + tt_svers + "','" + tt_modelname + "','" + tt_vendorid + "'," +
                                             "'" + tt_onumodel + "','" + tt_flhratio + "','" + tt_flgratio + "'," + 
                                             "'" + tt_fec + "','" + tt_bosatype + "','" + tt_remark + "',getdate()) ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    tt_flag = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }
            }
            return tt_flag;
        }

        //新增产品序列号
        public static Boolean Fhzzhostlablemade(string tt_taskscode, string tt_hostqzwh, string tt_hostmode , string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    string tt_sql1 = "insert into odc_hostlableoptioan (taskscode,hostqzwh,hostvalue,hostmode,hostmax)" +
                                     "values ('" + tt_taskscode + "','" + tt_hostqzwh + "'," + "'0001','" + tt_hostmode + "','0') ";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    tt_flag = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }
            }
            return tt_flag;
        }

        //修改分单制造单数量
        public static Boolean Fhzztasksnum(string tt_taskscode, int tt_tasksquantity, string con)
        {
            Boolean tt_flag = false;

            using (SqlConnection connection = new SqlConnection(con))
            {
                opentwo(connection); //打开连接
                SqlCommand command = connection.CreateCommand();

                SqlTransaction transaction;
                transaction = connection.BeginTransaction("SampleTransaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    string tt_sql1 = "update odc_tasks set tasksquantity = '" + tt_tasksquantity + "'" +
                                     "where taskscode = '" + tt_taskscode + "'";
                    command.CommandText = tt_sql1;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    tt_flag = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("统一事务处理报错：" + ex.GetType().ToString());
                    tt_flag = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("事务回滚报错" + ex2.GetType().ToString());
                    }
                }
            }
            return tt_flag;
        }


        #endregion


        #region 9、MAC权限绑定

        //获取本机MAC
        public static string GetHostIpName()
        {
            string mac = "";
            ManagementClass mc;
            mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["IPEnabled"].ToString() == "True")
                    mac = mo["MacAddress"].ToString();
            }
            return mac;
        }

        public class Uptext
        {
            public static Hashtable UptextData = new Hashtable();
        }

        //获取是否处于MAC列表内/升级确认
        static int i = 0;
        static DateTime tt_UpChangetime;
        public static string GetComputerMAC(string con)
        {
            try
            {
                string serverIP = ConfigurationManager.AppSettings["ServerIP"];
                int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);

                string datebaseIP = "172.18.201.2";
                if (serverIP == "172.16.20.29")
                {
                    datebaseIP = "172.16.30.2";
                }

                if (VersionHelper.HasNewVersion(serverIP, serverPort))
                {
                    UpMessage form1 = new UpMessage();
                    form1.SIP = datebaseIP;
                    form1.StartPosition = FormStartPosition.CenterScreen;
                    form1.ShowDialog();
                    string Upmessage_reback = Uptext.UptextData["Key1"].ToString();
                    if (Upmessage_reback == "YES")
                    {
                        string updateExePath = AppDomain.CurrentDomain.BaseDirectory + "CloseTvbox01.exe";
                        System.Diagnostics.Process myProcess = System.Diagnostics.Process.Start(updateExePath);
                    }
                    else
                    {
                        i++;
                        if (i == 1)
                        {
                            tt_UpChangetime = DateTime.Now;
                        }
                        else if (i > 1)
                        {
                            DateTime tt_Upsaynotime = DateTime.Now;
                            TimeSpan tt_diffre;
                            tt_diffre = tt_Upsaynotime - tt_UpChangetime;
                            //if (tt_diffre.Minutes >= 1)//程序调试用
                            if (tt_diffre.Hours >= 12)
                            {
                                MessageBox.Show("据打印软件发布升级通知已超过12小时，开始强制升级");
                                string updateExePath = AppDomain.CurrentDomain.BaseDirectory + "CloseTvbox01.exe";
                                System.Diagnostics.Process myProcess = System.Diagnostics.Process.Start(updateExePath);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("自动升级检测失败，请联系工程检查");
            }

            string tt_YorN = "0";

            string tt_CMAC = GetHostIpName();

            string tt_sql = "select count(1),0,0 from odc_printcomputer where ComputerMAC = '" + tt_CMAC + "' ";

            string[] tt_array = new string[3];
            tt_array = GetDatasetArray(tt_sql, con);
            if (tt_array[0] == "1")
            {
                tt_YorN = tt_array[0];
            }
            else
            {
                MessageBox.Show("非认证打印电脑，软件处于限制重打模式，如需解除限制，请联系工程");
            }
            return tt_YorN;
        }


        #endregion


        #region 重打记录

        public class Context
        {
            public static Hashtable ContextData = new Hashtable();
        }

        //打印记录
        public static void lablePrintRecord(string tt_task, string tt_mac, string tt_host, string tt_local, string tt_user, string tt_computername, string tt_remark ,string tt_conn)
        {
            string tt_insertsql = "insert into odc_lableprint (Ftaskcode,Fmaclable,Fhostlable,Flocal,Fname,Fdate,Fcomputername,Fremark) " +
                       "values('" + tt_task + "','" + tt_mac + "','" + tt_host + "','" + tt_local + "','" + tt_user + "',getdate(),'" + tt_computername + "','" + tt_remark + "') ";

            int tt_intcount = ExecCommand(tt_insertsql, tt_conn);

        }

        //log记录
        public static void AddLog(string strName, string strTaskCode, string strMac, string strLogText, string strMode)
        {
            string folderPath = string.Format(@"\{0}\{1}\{2}\{3}", strName, strTaskCode, DateTime.Now.ToString("yyyy-MM-dd"), strMode);
            string Logpath = string.Format(folderPath + @"\{0}.txt", strMac);

            if (!Directory.Exists(folderPath))//如果不存在就创建file文件夹 
            {
                Directory.CreateDirectory(folderPath);//创建该文件夹 
            }

            if (!File.Exists(Logpath))//如果不存在就创建TxT文档 
            {
                StreamWriter log = File.CreateText(Logpath);//创建文档
                log.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：  " + strLogText);
                log.Close();
            }
            else
            {
                StreamWriter log = new StreamWriter(Logpath, true);
                log.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：  " + strLogText);
                log.Close();
            }
        }

        #endregion
    }
}
