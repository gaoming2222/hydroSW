using System;
using System.Diagnostics;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Hydrology.Utils;
using System.Threading;

namespace Hydrology
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //gm
                /** 
            * 当前用户是管理员的时候，直接启动应用程序 
            * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行 
            */
                //获得当前登录的Windows用户标示  
                //System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                //System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
                ////判断当前登录用户是否为管理员  
                //if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
                //{
                //    //如果是管理员，则直接运行  
                //    Application.Run(new MainForm());
                //}
                //else
                //{
                //    //创建启动对象  
                //    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                //    startInfo.UseShellExecute = true;
                //    startInfo.WorkingDirectory = Environment.CurrentDirectory;
                //    startInfo.FileName = Application.ExecutablePath;
                //    //设置启动动作,确保以管理员身份运行  
                //    startInfo.Verb = "runas";
                //    try
                //    {
                //        System.Diagnostics.Process.Start(startInfo);
                //    }
                //    catch
                //    {
                //        return;
                //    }

                    //gm
                    Application.Run(new MainForm());
                //}
            }
            catch (System.Exception ex)
            {
                // 写入日志文件
                //CXmlRealTimeDataSerializer.Instance.DeleteFile(); //删除实时数据表文件，下次重新查询数据库
                Console.WriteLine(ex);
                CSystemInfoMgr.Instance.AddInfo(ex.ToString());
                //CSystemInfoMgr.Instance.AddInfo("系统异常退出");
                CSystemInfoMgr.Instance.Close();    //写入文件
                Application.Run(new MainForm());
               
            }

        }
    }
}
