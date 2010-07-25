[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace TinyRadius.Net
{
    public enum AuthenticationType
    {
        /// <summary>
        ///Passphrase Authentication Protocol
        /// </summary>
        pap,
        /// <summary>
        ///Challenged Handshake Authentication Protocol
        /// </summary>
        chap
    }
}