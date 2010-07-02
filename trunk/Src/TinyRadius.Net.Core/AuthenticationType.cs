using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
