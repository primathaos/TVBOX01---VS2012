using OAUS.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace TVBOX01
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                string serverIP = ConfigurationManager.AppSettings["ServerIP"];
                int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);

                //刷新打印模板，需要svn软件安装命令行操作模组（建议32位的系统安装此版本或以上版本：TortoiseSVN-1.10.1.28295-win32-svn-1.10.2）
                //cmd指令pushd是用于重定向至执行档目录的，此指令适用范围较cd更广，请不要更改
                RunCmd("svn update pushd " + AppDomain.CurrentDomain.BaseDirectory);

                if (VersionHelper.HasNewVersion(serverIP, serverPort))
                {
                    //if (DialogResult.Yes == MessageBox.Show("检测到新版本，是否启动升级", "自动升级", MessageBoxButtons.YesNo))
                    //{
                    string updateExePath = AppDomain.CurrentDomain.BaseDirectory + "CloseTvbox01.exe"; //调用软件中止TVBOX并启用自动升级
                    System.Diagnostics.Process myProcess = System.Diagnostics.Process.Start(updateExePath);
                    return;
                    //}
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("自动升级检测失败");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new login());
        }

        static string RunCmd(string command)
        {
            // 需用引用命名空间System.Diagnostics;
            // 打开一个新进程
            Process p = new Process();
            // 指定进程程序名称
            p.StartInfo.FileName = "cmd.exe";
            // 设定要输入命令
            p.StartInfo.Arguments = "/c " + command;
            // 关闭Shell的使用
            p.StartInfo.UseShellExecute = false;
            // 重定向标准输入
            p.StartInfo.RedirectStandardInput = true;
            // 重定向标准输出
            p.StartInfo.RedirectStandardOutput = true;
            // 重定向错误输出
            p.StartInfo.RedirectStandardError = true;
            // 不显示命令提示符窗口
            p.StartInfo.CreateNoWindow = true;
            // 启动程序
            p.Start();
            // 返回执行的结果
            return p.StandardOutput.ReadToEnd();
        }

    }
}
