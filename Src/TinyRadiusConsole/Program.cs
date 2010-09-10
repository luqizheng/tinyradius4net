using System;
using System.Net;
using log4net;
using TinyRadiusService;

namespace TinyRadiusConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var _server = new RadiusServer
                              {
                                  //10.249.195.123
                                  AcctPort = 1813,
                                  AuthPort = 1812,
                                  ListenAccountIp = IPAddress.Parse("10.249.195.123"),
                                  ListenAuthIp = IPAddress.Parse("10.249.195.123")
                              };


                _server.Start(true, false);
                while (true)
                {
                    Console.WriteLine("press exit for exit");
                    string s = Console.ReadLine();
                    if (s.ToLower() == "exit")
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Program));
                log.Debug("Failed in running.", ex);
            }
        }
    }
}