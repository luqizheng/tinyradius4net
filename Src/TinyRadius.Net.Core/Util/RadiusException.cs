/**
 * $Id: RadiusException.java,v 1.2 2005/10/15 11:35:30 wuttke Exp $
 * Created on 10.04.2005
 * @author Matthias Wuttke
 * @version $Revision: 1.2 $
 */
using System;
namespace TinyRadius.Net.Util
{

    /**
     * An exception which occurs on Radius protocol errors like
     * invalid packets or malformed attributes.
     */
    public class RadiusException : Exception
    {

        /**
         * Constructs a RadiusException with a message.
         * @param message error message
         */
        public RadiusException(String message)
        {
            base(message);
        }

        private static readonly long serialVersionUID = 2201204523946051388L;

    }

}