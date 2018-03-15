using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace TVBOX01
{
    class Dataset2
    {
        //-------以下专门流程检验相关的，以及数据集操作----------

        #region 1、获取信息辅助方法

        //从流程表process获取全部流程字符串
        public static string getGyidAllProcess(string tt_gyid, string tt_conn)
        {
            string tt_gyidprocess = "单板工单流程没有找到";
            string tt_sql = "select count(1),min(process),0 from odc_process where id = " + tt_gyid;
            string[] tt_array = new string[3];
            tt_array = Dataset1.GetDatasetArray(tt_sql, tt_conn);
            if (tt_array[0] == "1") tt_gyidprocess = tt_array[1];
            return tt_gyidprocess;
        }


        //从routing表获取全流程数据集
        public static DataSet getGyidAllProcessDt(string tt_gyid, string tt_conn)
        {
            DataSet tt_dt = null;
            string tt_sql = "select pxid,lcbz from odc_routing  where pid = " + tt_gyid;
            tt_dt = Dataset1.GetDataSetTwo(tt_sql, tt_conn);
            if (tt_dt.Tables.Count > 0 && tt_dt.Tables[0].Rows.Count > 0)
            {

            }
            else
            {
                MessageBox.Show("getGyidAllProcessDt,没有找到流程:" + tt_gyid + "，的流程数据集Dataset，请流程设置！");
            }

            return tt_dt;
        }


        //获取工单要检查的流程
        public static string getGyidPartProcess(DataSet tt_checkcodedt)
        {
            string tt_parrtprocess = "部分流程无法获取";

            if (tt_checkcodedt.Tables.Count > 0 && tt_checkcodedt.Tables[0].Rows.Count > 0)
            {
                string tt_routingncode = "";
                string tt_partcheckcode = "";
                for (int i = 0; i < tt_checkcodedt.Tables[0].Rows.Count; i++)
                {
                    tt_routingncode = tt_checkcodedt.Tables[0].Rows[i][0].ToString();
                    tt_partcheckcode = tt_partcheckcode + "," + tt_routingncode;
                }
                tt_parrtprocess = tt_partcheckcode;
            }
            else
            {
                MessageBox.Show("getGyidAllProcessDt,无法获取到流程检验的部分流程");
            }


            return tt_parrtprocess;
        }


        //字符串转换为int
        public static int getTransmitStrToInt(string tt_str)
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
                    MessageBox.Show(tt_str + ",转换为数字失败，请检查！getTransmitStrToInt");
                }
            }


            return tt_int;
        }



        //NG原因记录
        public static int getNgreasonRecord(string tt_taskcode,string tt_mac,string tt_local,string tt_ng,string tt_code,string tt_conn)
        {
            int tt_int = 0;
            string tt_sql = "insert into odc_ng(Taskcode,Mac,Flocal,Fng,Fcode,Fdate) " +
                            "values('"+tt_taskcode+"','"+tt_mac+"','"+tt_local+"','"+tt_ng+"','"+tt_code+"',getdate()) ";
            tt_int = Dataset1.ExecCommand(tt_sql, tt_conn);
            return tt_int;
        }


        #endregion
                


        #region 2、NG01 获取站位信息

        //获取MAC全部过站信息
        public static DataSet getMacAllCodeInfo(string tt_shortmac,string tt_conn)
        {
            DataSet tt_dt = null;
            string tt_sql1 = "select Id,Ccode,Ncode,Napplytype,Fremark from odc_routingtasklist " +
                                       "where pcba_pn = '" + tt_shortmac + "' order by id ";
            tt_dt = Dataset1.GetDataSet(tt_sql1, tt_conn);

            return tt_dt;
        }



        #endregion



        #region 3、NG02 待测站位检验

        //根据数据集获取MAC的待测站位检查
        public static string getPcbaNowCode(DataSet tt_dt)
        {
            string tt_returnncode = "0";

            if (tt_dt.Tables.Count > 0 && tt_dt.Tables[0].Rows.Count > 0)
            {
                string tt_ncode = "";
                string tt_napplytype = "";
                string tt_nowcode = "";
                int tt_napplycount = 0;
                //以下数据遍历
                for (int i = 0; i < tt_dt.Tables[0].Rows.Count; i++)
                {
                    tt_ncode = tt_dt.Tables[0].Rows[i][2].ToString();
                    tt_napplytype = tt_dt.Tables[0].Rows[i][3].ToString();

                    if (tt_napplytype.Equals(""))
                    {
                        tt_napplycount++;
                        tt_nowcode = tt_ncode;
                    }
                }
                //以下返回值判断
                if (tt_napplycount == 0) tt_returnncode = "0";
                if (tt_napplycount == 1) tt_returnncode = tt_nowcode;
                if (tt_napplycount > 1) tt_returnncode = "2";
            }
            return tt_returnncode;
        }

        #endregion



        #region 4、NG03 1920站位检查

        //检查站位顺序以及获取1920最大值
        public static int getFirstCodeId(DataSet tt_dt)
        {
            int tt_introwid = -10;

            //第一步检验数据是否有数据
            #region
            bool tt_flag1 = false;
            if (tt_dt.Tables.Count > 0 && tt_dt.Tables[0].Rows.Count > 0)
            {
                tt_flag1 = true;
            }
            else
            {
                tt_introwid = 0;
                MessageBox.Show("getFirstCodeId,检查1920站位数据集，发现数据集为空");
            }
            #endregion



            //第二步 检查数据集是否按顺序排序
            #region
            bool tt_flag2 = false;
            if (tt_flag1)
            {
                int tt_intid1 = 0;
                int tt_intid2 = 0;
                int tt_intallcount = tt_dt.Tables[0].Rows.Count;
                //以下for循环
                #region
                for (int i = 0; i < tt_intallcount; i++)
                {
                    if (tt_intallcount == 1)
                    {
                        tt_flag2 = true;
                    }
                    else
                    {
                        if (i > 0)
                        {
                            tt_intid1 = getTransmitStrToInt(tt_dt.Tables[0].Rows[i][0].ToString());
                            tt_intid2 = getTransmitStrToInt(tt_dt.Tables[0].Rows[i - 1][0].ToString());
                            tt_flag2 = true;
                            if (tt_intid1 < tt_intid2)
                            {
                                tt_flag2 = false;
                                tt_introwid = -1;
                                MessageBox.Show("检查数据集顺序，发现不是按顺序排序，ID号:" + tt_intid1.ToString());
                                break;

                            }
                        }

                    }

                }
                #endregion
                //以上for循环

            }
            #endregion



            //第三步 查找1920最大值
            if (tt_flag2)
            {
                int tt_inteveryid = 0;
                int tt_intendid = 0;
                string tt_nowvode = "";
                //以下for循环
                #region
                for (int i = 0; i < tt_dt.Tables[0].Rows.Count; i++)
                {
                    tt_inteveryid = getTransmitStrToInt(tt_dt.Tables[0].Rows[i][0].ToString());
                    tt_nowvode = tt_dt.Tables[0].Rows[i][1].ToString();

                    if (tt_nowvode == "1920")
                    {
                        if (tt_inteveryid > tt_intendid) tt_intendid = tt_inteveryid;
                    }


                }
                #endregion
                //以上for循环

                if (tt_intendid == 0)
                {
                    tt_introwid = -2;
                }
                else
                {
                    tt_introwid = tt_intendid;
                }

            }



            return tt_introwid;
        }

        #endregion

        

        #region 5、NG04 3350跳出检验

        //3350站位跳出检查
        public static string getMaintainJumpCheck(DataSet tt_codedt, int tt_intcode)
        {
            string tt_outmessage = "0";

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_ncode = "";
                string tt_remark = "";
                int tt_introwid = 0;
                string tt_checkinfo = "0";
                //以下for循环
                #region
                for (int i = 0; i < tt_codedt.Tables[0].Rows.Count; i++)
                {
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    if (tt_introwid >= tt_intcode)
                    {
                        tt_ncode = tt_codedt.Tables[0].Rows[i][2].ToString();
                        tt_remark = tt_codedt.Tables[0].Rows[i][4].ToString();

                        if (tt_ncode.Equals("3350") && !tt_remark.Equals("PR001站位跳转"))
                        {
                            tt_checkinfo = "3350跳出检查Fail:站位" + tt_ncode + ",ID=" + tt_introwid.ToString();
                            break;
                        }
                    }
                }
                #endregion
                //以上for循环
                if (tt_checkinfo.Equals("0"))
                {
                    tt_outmessage = "1";
                }
                else
                {
                    tt_outmessage = tt_checkinfo;
                }

            }
            else
            {
                tt_outmessage = "350站位跳出检查数据集为空！";
                MessageBox.Show("getMaintainJumpCheck,过站全顺序检查数据集为空！");
            }

            return tt_outmessage;
        }

        #endregion



        #region 6、NG05 全流程检查

        //工单设定流程每个站位检查 要对1920以上站位进行检验
        public static string getPcbaAllCheck2(DataSet tt_routdt, DataSet tt_codedt, int tt_intcode, int tt_productname_check)
        {
            string tt_outmessage = "0";  //数据集有问题

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0 && tt_routdt.Tables.Count > 0 && tt_routdt.Tables[0].Rows.Count > 0)
            {
                string tt_routingncode = "";
                string tt_checkinfo = "0";
                //以下数据遍历 for循环
                for (int i = 0; i < tt_routdt.Tables[0].Rows.Count; i++)
                {
                    tt_routingncode = tt_routdt.Tables[0].Rows[i][0].ToString();
                    //没有找到就返回站位，找到返回1
                    tt_checkinfo = getPcbaSinglCheck2(tt_routingncode, tt_codedt, tt_intcode, tt_productname_check);
                    if (tt_checkinfo == tt_routingncode) break;
                }
                //以上数据遍历

                if (tt_checkinfo == "0")
                {
                    tt_outmessage = "1";
                }
                else
                {
                    tt_outmessage = tt_checkinfo;
                }


            }
            return tt_outmessage;
        }


        //MAC单板数据集的循环检查
        public static string getPcbaSinglCheck2(string tt_checkcode, DataSet tt_codedt, int tt_intcode, int tt_productname_check)
        {
            string tt_checkinfo = tt_checkcode;  //没有找到就返回站位，找到返回1

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_ncode = "";
                string tt_napplytype = "";
                int tt_introwid = 0;
                //以下数据遍历
                for (int i = 0; i < tt_codedt.Tables[0].Rows.Count; i++)
                {
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    if (tt_introwid >= tt_intcode)
                    {
                        tt_ncode = tt_codedt.Tables[0].Rows[i][2].ToString();
                        if (tt_productname_check == 1 && tt_ncode == "2111")
                        {
                            tt_ncode = "2115";
                        }
                        tt_napplytype = tt_codedt.Tables[0].Rows[i][3].ToString();
                        if ((tt_napplytype.Equals("1") || tt_napplytype.Equals("")) && tt_ncode == tt_checkcode)
                        {
                            tt_checkinfo = "1";
                            break;
                        }
                    }
                }
            }

            return tt_checkinfo;
        }

        #endregion



        #region 7、NG06 全顺序检查

        //过站全顺序检查
        public static string getCodeSerialCheck(DataSet tt_codedt, int tt_intcode)
        {
            string tt_outmessage = "0";

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_code1 = "";
                string tt_code2 = "";
                int tt_introwid = 0;
                string tt_checkinfo = "0";
                //以下for循环
                #region
                for (int i = 0; i < tt_codedt.Tables[0].Rows.Count; i++)
                {
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    if (tt_introwid > tt_intcode)
                    {
                        tt_code1 = tt_codedt.Tables[0].Rows[i][1].ToString();  //当前记录前一站位
                        tt_code2 = tt_codedt.Tables[0].Rows[i - 1][2].ToString();  //上一记录测试站位
                        if (!tt_code1.Equals(tt_code2))
                        {
                            tt_checkinfo = "顺序检查Fail:前站位" + tt_code1 + ",ID=" + tt_introwid.ToString();
                            break;
                        }

                    }


                }
                #endregion
                //以上for循环

                if (tt_checkinfo.Equals("0"))
                {
                    tt_outmessage = "1";
                }
                else
                {
                    tt_outmessage = tt_checkinfo;
                }
            }
            else
            {
                tt_outmessage = "过站全顺序检查数据集为空！";
                MessageBox.Show("getCodeSerialCheck,过站全顺序检查数据集为空！");
            }


            return tt_outmessage;
        }

        #endregion



        #region 8、NG07 前后项检查

        //前后站位关系检查 序列号检验
        public static string getNearCodeCheck2(DataSet tt_codedt, int tt_intcode, DataSet tt_dtallprocess)
        {
            string tt_outmessage = "0";

            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_code1 = "";
                string tt_code2 = "";
                int tt_intcode1 = 0;
                int tt_intcode2 = 0;
                int tt_introwid = 0;
                string tt_checkinfo = "0";
                //以下for循环
                #region
                for (int i = 0; i < tt_codedt.Tables[0].Rows.Count; i++)
                {
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    if (tt_introwid >= tt_intcode)
                    {
                        tt_code1 = tt_codedt.Tables[0].Rows[i][1].ToString();
                        tt_code2 = tt_codedt.Tables[0].Rows[i][2].ToString();

                        if (tt_code1.Equals("3350") || tt_code2.Equals("3350"))
                        {
                        }
                        else
                        {
                            tt_intcode1 = getRoutCodeDerialNo(tt_dtallprocess, tt_code1);
                            tt_intcode2 = getRoutCodeDerialNo(tt_dtallprocess, tt_code2);
                            if ((tt_intcode2 - tt_intcode1 == 1) || tt_intcode2 <= tt_intcode1)
                            {

                            }
                            else
                            {
                                tt_checkinfo = "前后项检查Fail:前后站位" + tt_code1 + "/" + tt_code2 + "," + tt_intcode1.ToString() + "/" + tt_intcode2.ToString() + ",ID=" + tt_introwid.ToString();
                                break;
                            }
                        }

                    }


                }
                #endregion
                if (tt_checkinfo.Equals("0"))
                {
                    tt_outmessage = "1";
                }
                else
                {
                    tt_outmessage = tt_checkinfo;
                }
            }
            else
            {
                tt_outmessage = "过站前后站位检查数据集为空！";
                MessageBox.Show("getNearCodeCheck2,过站前后站位检查数据集为空！");
            }

            return tt_outmessage;
        }


        //获取站位序列号
        public static int getRoutCodeDerialNo(DataSet tt_dt, string tt_code)
        {
            int tt_intcode = 0;
            if (tt_dt.Tables.Count > 0 && tt_dt.Tables[0].Rows.Count > 0)
            {
                int tt_introwserin = 0;
                string tt_rowcode = "0";
                //以下for循环
                #region
                for (int i = 0; i < tt_dt.Tables[0].Rows.Count; i++)
                {
                    tt_introwserin = getTransmitStrToInt(tt_dt.Tables[0].Rows[i][1].ToString());
                    tt_rowcode = tt_dt.Tables[0].Rows[i][0].ToString();
                    if (tt_rowcode == tt_code)
                    {
                        tt_intcode = tt_introwserin;
                        break;
                    }
                }
                #endregion

            }
            else
            {
                MessageBox.Show("getNearCodeCheck2,获取站位顺序号Fail,数据集为空");
            }


            return tt_intcode;
        }

        #endregion



        #region 9、NG08 上下项检查

        //流程上下项检查，跳过3350
        public static string getUpdownCodeCheck(DataSet tt_codedt, int tt_intcode, DataSet tt_dtallprocess)
        {
            string tt_outmessage = "0";

            
            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_ncode = "";
                string tt_apply = "";
                string tt_fromcode = "";

                int tt_intncode = 0;
                int tt_intfromcode = 0;
                int tt_introwid = 0;
                string tt_checkinfo = "0";

                //以下for循环
                #region
                for (int i = 0; i < tt_codedt.Tables[0].Rows.Count; i++)
                {
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    tt_ncode = tt_codedt.Tables[0].Rows[i][2].ToString();
                    tt_apply = tt_codedt.Tables[0].Rows[i][3].ToString();
                    if (tt_introwid > tt_intcode && !tt_ncode.Equals("3350"))  //&& !tt_apply.Equals("")
                    {
                        tt_intncode = getRoutCodeDerialNo(tt_dtallprocess, tt_ncode);
                        tt_fromcode = getBeforeTestCode(tt_codedt, tt_intcode,i);
                        tt_intfromcode = getRoutCodeDerialNo(tt_dtallprocess, tt_fromcode);
                        if ((tt_intncode - tt_intfromcode == 1) || tt_intncode <= tt_intfromcode)
                        {

                        }
                        else
                        {
                            tt_checkinfo = "上下项检查Fail:上下站位" + tt_ncode + "/" + tt_fromcode + "," + tt_intncode.ToString() + "/" + tt_intfromcode.ToString() + ",ID=" + tt_introwid.ToString();
                            break;
                        }
                    }
                }
                #endregion


                if (tt_checkinfo.Equals("0"))
                {
                    tt_outmessage = "1";
                }
                else
                {
                    tt_outmessage = tt_checkinfo;
                }



            }
            else
            {
                tt_outmessage = "过站上下站位检查数据集为空！";
                MessageBox.Show("NG08,过站上下站位检查数据集为空！");
            }

            return tt_outmessage;
        }



        //获取当前站位前一个的测试站位
        public static string getBeforeTestCode(DataSet tt_codedt, int tt_intcode, int tt_intn)
        {
            string tt_fromcodeinfo = "";
            if (tt_codedt.Tables.Count > 0 && tt_codedt.Tables[0].Rows.Count > 0)
            {
                string tt_ncode = "";
                int tt_introwid = 0;
                //以下for循环
                #region
                for (int i = tt_intn-1; i >= 0; i--)
                {
                    tt_ncode = tt_codedt.Tables[0].Rows[i][2].ToString();
                    tt_introwid = getTransmitStrToInt(tt_codedt.Tables[0].Rows[i][0].ToString());
                    if (tt_introwid >= tt_intcode)
                    {
                        if (!tt_ncode.Equals("3350"))
                        {
                            tt_fromcodeinfo = tt_ncode;
                            break;
                        }
                    }
                }
                #endregion
            }
            return tt_fromcodeinfo;
        }

        #endregion

        //-------以上专门流程检验相关的，以及数据集操作----------
    }
}
