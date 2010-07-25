using System;
using System.Net;
using System.ServiceModel;
using System.ServiceProcess;
using log4net;
using TinyRadius.Net.Cfg;

namespace TinyRadiusService
{
    public partial class TinyRadiusService : ServiceBase
    {
        private ServiceHost host;
        private static readonly ILog Log = LogManager.GetLogger(typeof(TinyRadiusService));
        private MockRadiusServer _server;

        public TinyRadiusService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _server = new MockRadiusServer
                         {
                             AcctPort = Config.Instance.AcctPort,
                             AuthPort = Config.Instance.AuthPort,
                             ListenAccountIp = IPAddress.Parse(Config.Instance.AccountListentIp),
                             ListenAuthIp = IPAddress.Parse(Config.Instance.AuthListentIp)
                         };
            _server.Start(Config.Instance.EnableAuthentication, Config.Instance.EnableAccount);
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
            }
        }
    }
}