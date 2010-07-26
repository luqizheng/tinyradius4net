using System;
using System.IO;
using Microsoft.Win32;
using TinyRadius.Net.Cfg;

namespace TinyRadiusService
{
    public class Cfg
    {
        public static readonly Cfg Instance = new Cfg();
        private Config _tinyConfig;
        private DateTime fileModifyTime;
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
                    if (path.StartsWith("\""))
                    {
                        path = path.Substring(1);
                    }
                    if (path.EndsWith("\""))
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
                var time = File.GetLastWriteTime(ServicePath);
                if (_tinyConfig == null || fileModifyTime != time)
                {
                    lock (this)
                    {
                        if (_tinyConfig == null || fileModifyTime != time)
                        {
                            _tinyConfig = new Config(ServicePath);
                            fileModifyTime = File.GetLastWriteTime(ServicePath);
                        }
                    }
                }
                return _tinyConfig;
            }
        }
    }
}