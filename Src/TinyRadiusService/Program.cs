using System;
using System.IO;
using System.ServiceProcess;
using log4net;
using log4net.Config;

namespace TinyRadiusService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {

            //log4net.GlobalContext.Properties["baseDir"] = AppDomain.CurrentDomain.BaseDirectory;
            //log4net.Config.XmlConfigurator.Configure();
            var file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(file);
            var servicesToRun = new ServiceBase[]
                                    {
                                        new TinyRadiusService()
                                    };
            ServiceBase.Run(servicesToRun);


        }


    }
}