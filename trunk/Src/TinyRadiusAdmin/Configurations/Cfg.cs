using System;
using System.IO;
using Microsoft.Win32;
using TinyRadius.Net.Cfg;

namespace TinyRadiusAdmin.Configurations
{
    public class Cfg
    {
        public static readonly Cfg Instance = new Cfg();
        private Config _tinyConfig;

        private Cfg()
        {
        }

        public string ServicePath
        {
            get
            {
                RegistryKey registry =
                    Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\TinyRadius.Net Server");
                try
                {
                    if (registry == null)
                        throw new ApplicationException("TinyRadius Server没有安装");
                    var path = registry.GetValue("ImagePath").ToString();
                    if(path.StartsWith("\""))
                    {
                        path = path.Substring(1);
                    }
                    if(path.EndsWith("\""))
                    {
                        path = path.Substring(0, path.Length - 1);
                    }
                    var info = new FileInfo(path);
                    return info.DirectoryName;
                }
                catch (Exception)
                {
                    if (registry != null)
                        registry.Close();
                    throw;
                }
            }
        }


        public Config TinyConfig
        {
            get
            {
                if (_tinyConfig == null)
                    _tinyConfig = new Config(ServicePath);
                return _tinyConfig;
            }
        }
    }
}