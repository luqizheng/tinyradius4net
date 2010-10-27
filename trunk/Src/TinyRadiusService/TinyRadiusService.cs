using System;
using System.Net;
using System.ServiceProcess;
using log4net;
using TinyRadiusService.Cfg;

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
            Config config;
            IPAddress accountListentIp = IPAddress.Parse("127.0.0.1");
            IPAddress authIp = IPAddress.Parse("127.0.0.1");
            try
            {
                config = ServiceCfg.Instance.TinyConfig;
                if (!String.IsNullOrEmpty(config.AccountListentIp))
                {
                    if (!IPAddress.TryParse(config.AccountListentIp, out accountListentIp))
                    {
                        Log.Error("accountList IP isn't validate, so auto set to 127.0.0.1");
                    }
                }

                if (!String.IsNullOrEmpty(config.AuthListentIp))
                {
                    if (!IPAddress.TryParse(config.AuthListentIp, out authIp))
                    {
                        Log.Error(("Auth Ip isn't validate, so auto set to 127.0.0.1"));
                    }
                }
                _server = new RadiusServer
                              {
                                  AcctPort = config.AcctPort,
                                  AuthPort = config.AuthPort,
                                  ListenAccountIp = accountListentIp,
                                  ListenAuthIp = authIp
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