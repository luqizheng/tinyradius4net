using System;
using System.Net;
using System.ServiceProcess;
using log4net;
using TinyRadius.Net.Cfg;

namespace TinyRadiusService
{
    public partial class TinyRadiusService : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TinyRadiusService));
        private RadiusServer _server;

        public TinyRadiusService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var config = ServiceCfg.Instance.TinyConfig;

                _server = new RadiusServer
                              {
                                  AcctPort = config.AcctPort,
                                  AuthPort = config.AuthPort,
                                  ListenAccountIp = IPAddress.Parse(config.AccountListentIp),
                                  ListenAuthIp = IPAddress.Parse(config.AuthListentIp)
                              };
                _server.Start(config.EnableAuthentication, config.EnableAccount);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;

            }
        }

        protected override void OnStop()
        {
            try
            {
                _server.Stop();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;

            }
        }
    }
}