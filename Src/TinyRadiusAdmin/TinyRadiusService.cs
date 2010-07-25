using System;
using System.ServiceProcess;
using log4net;

namespace TinyRadiusServer
{
    public class TinyRadiusService
    {
        private readonly ILog _log;
        private readonly ServiceController serviceController;

        public TinyRadiusService(string serverName)
        {
            ServiceName = serverName;
            _log = LogManager.GetLogger(typeof(TinyRadiusService));
            serviceController = new ServiceController(ServiceName);
        }

        public string ServiceName { get; set; }

        public bool Start()
        {
            try
            {
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                    serviceController.Start();
                return true;
            }
            catch (Exception ex)
            {
                this._log.Error("Start Service fail", ex);
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                serviceController.Stop();
                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Stop service fail", ex);
                return false;
            }
        }
    }
}