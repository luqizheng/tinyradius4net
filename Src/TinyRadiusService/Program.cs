using System.ServiceProcess;
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
            var servicesToRun = new ServiceBase[]
                                    {
                                        new TinyRadiusService()
                                    };
            ServiceBase.Run(servicesToRun);
        }
    }
}