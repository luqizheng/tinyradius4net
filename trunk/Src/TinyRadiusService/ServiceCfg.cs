using System;
using System.IO;
using System.ServiceModel.Channels;
using log4net;
using log4net.Config;
using Microsoft.Win32;
using TinyRadius.Net.Cfg;
[assembly: XmlConfigurator(Watch = true)]
namespace TinyRadiusService
{
    public class ServiceCfg
    {
        private const string RegistryPath = @"SYSTEM\CurrentControlSet\services\TinyRadius.Net Server";
        public static readonly ServiceCfg Instance = new ServiceCfg();
        private Config _tinyConfig;
        private DateTime _fileModifyTime;
        private readonly static ILog Log = LogManager.GetLogger(typeof(ServiceCfg));
        private ServiceCfg()
        {
        }

        public string GetServicePath()
        {
            Log.Debug("Found TinyRadius.net server's ImagePath in Registry by path " + RegistryPath);
            RegistryKey registry =
                Registry.LocalMachine.OpenSubKey(RegistryPath);
            try
            {
                if (registry == null)
                {
                    throw new ApplicationException("TinyRadius Server没有安装");
                }
                var path = registry.GetValue("ImagePath").ToString();
                Log.Debug("Found TinyRadius.net server's ImagePath is " + path);
                if (path.StartsWith("\""))
                {
                    path = path.Substring(1);
                }
                if (path.EndsWith("\""))
                {
                    path = path.Substring(0, path.Length - 1);
                }

                int lastBackslash = path.LastIndexOf('\\');
                return path.Substring(0, lastBackslash);
            }
            finally
            {
                if (registry != null)
                    registry.Close();
            }
        }


        public Config TinyConfig
        {
            get
            {
                string servicePath = GetServicePath();
                Log.Debug("Setting filePath is " + servicePath);
                var time = File.GetLastWriteTime(servicePath);
                if (_tinyConfig == null || _fileModifyTime != time)
                {
                    lock (this)
                    {
                        if (_tinyConfig == null || _fileModifyTime != time)
                        {
                            _tinyConfig = new Config(servicePath);
                            _fileModifyTime = time;
                        }
                    }
                }
                return _tinyConfig;
            }
        }
    }
}