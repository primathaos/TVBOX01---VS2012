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

                if (VersionHelper.HasNewVersion(serverIP, serverPort))
                {
                    //if (DialogResult.Yes == MessageBox.Show("检测到新版本，是否启动升级", "自动升级", MessageBoxButtons.YesNo))
                    //{
                    string updateExePath = AppDomain.CurrentDomain.BaseDirectory + "CloseTvbox01.exe";
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
    }
}
