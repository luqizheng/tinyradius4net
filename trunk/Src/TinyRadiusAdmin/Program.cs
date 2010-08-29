using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace TinyRadiusAdmin
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (StatusChecking())
                {
                    var tinyRadiusService = new TinyRadiusService();
                    var mainFomr = new MainForm { TinyRadiusService = tinyRadiusService };
                    Application.Run(mainFomr);
                }
            }
            catch (Exception ex)
            {
                var log = LogManager.GetLogger(typeof(Program));
                log.Error("Application Error", ex);
                MessageBox.Show("程序运行出错，请联络管理员");
                Application.Exit();
            }
        }


        private static bool StatusChecking()
        {
            try
            {
                var ServiceName = "TinyRadius.Net Server";
                var serviceController = new ServiceController(ServiceName);
                var a = serviceController.Status;
                return true;
            }
            catch (InvalidOperationException ex)
            {
                LogManager.GetLogger(typeof(Program)).Error("没有发现Tiny.Radius服务", ex);
                MessageBox.Show("没有发现TinyRadius服务，请先安装后再运行本管理程序");
                Application.Exit();
            }
            return false;
        }
    }
}