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
            : base(message)
        {
        }

        private static readonly long serialVersionUID = 2201204523946051388L;

    }

}