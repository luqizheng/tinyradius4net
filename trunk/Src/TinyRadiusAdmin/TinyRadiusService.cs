using System;
using System.ServiceProcess;
using log4net;

namespace TinyRadiusAdmin
{
    public class TinyRadiusService
    {
        private readonly ILog _log;
        private readonly ServiceController _serviceController;
        public ServiceControllerStatus Status
        {
            get
            {
                return _serviceController.Status;
            }
        }
        public TinyRadiusService()
        {
            ServiceName = "TinyRadius.Net Server";

            _log = LogManager.GetLogger(typeof(TinyRadiusService));
            _serviceController = new ServiceController(ServiceName);

        }

        public void Restart()
        {
            if (_serviceController.Status == ServiceControllerStatus.Running)
                _serviceController.Stop();
            _serviceController.Start();
        }

        public string ServiceName { get; set; }

        public bool Start()
        {
            try
            {
                if (_serviceController.Status == ServiceControllerStatus.Stopped)
                {
                    _serviceController.Start();
                    _log.Debug("Start success success");
                    return false;
                }
                else
                {
                    return true;
                }

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
                if (_serviceController.Status == ServiceControllerStatus.Running)
                {
                    _serviceController.Stop();
                    _log.Debug("Stop Servce success");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Stop service fail", ex);
                return false;
            }
        }
    }
}