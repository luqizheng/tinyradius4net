/**
 * $Id: StringAttribute.java,v 1.1.1.1 2005/04/17 14:51:33 wuttke Exp $
 * Created on 08.04.2005
 * @author Matthias Wuttke
 * @version $Revision: 1.1.1.1 $
 */
using System;
using System.Text;

namespace TinyRadius.Net.Attributes
{
    /**
     * This class represents a Radius attribute which only
     * contains a string.
     */

    public class StringAttribute : RadiusAttribute
    {
        /**
         * Constructs an empty string attribute.
         */

        private string _value;

        public StringAttribute()
        {
        }

        /**
         * Constructs a string attribute with the given value.
         * @param type attribute type
         * @param value attribute value
         */

        public StringAttribute(int type, String value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Returns the string value of this attribute.
        /// @return a string
        /// </summary>
        public override string Value
        {
            get { return Encoding.UTF8.GetString(Data); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Value not set");
                Data = Encoding.UTF8.GetBytes(value);

                _value = value;
            }
        }
    }
}