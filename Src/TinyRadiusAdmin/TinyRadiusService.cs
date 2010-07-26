using System;
using System.ServiceProcess;
using System.Threading;
using log4net;

namespace TinyRadiusAdmin
{
    public class TinyRadiusService
    {
        private readonly ILog _log;
        private readonly ServiceController _serviceController;

        public TinyRadiusService()
        {
            ServiceName = "TinyRadius.Net Server";

            _log = LogManager.GetLogger(typeof(TinyRadiusService));
            _serviceController = new ServiceController(ServiceName);
            ThreadPool.QueueUserWorkItem(StatusChecking);
        }

        public ServiceControllerStatus Status
        {
            get { return _serviceController.Status; }
        }

        public string ServiceName { get; set; }
        public event EventHandler StatusChangingEvent;

        public void Restart()
        {
            if (_serviceController.Status == ServiceControllerStatus.Running)
                _serviceController.Stop();
            _serviceController.Start();
        }

        public bool Start()
        {
            try
            {
                if (_serviceController.Status == ServiceControllerStatus.Stopped)
                {
                    _serviceController.Start();
                    _serviceController.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 0, 2000));
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
                _log.Error("Start Service fail", ex);
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
                    _serviceController.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 2000));
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

        private ServiceControllerStatus previousStatus;
        private void StatusChecking(object state)
        {
            previousStatus = _serviceController.Status;
            while (true)
            {
                _serviceController.Refresh();
                if (previousStatus != _serviceController.Status)
                {
                    if (StatusChangingEvent != null)
                        StatusChangingEvent(this, EventArgs.Empty);
                    previousStatus = _serviceController.Status;
                }
                Thread.Sleep(500);
            }
        }
    }
}