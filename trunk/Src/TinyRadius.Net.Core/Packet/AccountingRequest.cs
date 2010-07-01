/**
 * $Id: AccountingRequest.java,v 1.2 2006/02/17 18:14:54 wuttke Exp $
 * Created on 09.04.2005
 * @author Matthias Wuttke
 * @version $Revision: 1.2 $
 */
using TinyRadius.Net.Packet;
using System;
namespace TinyRadius.Net.packet{



/**
 * This class represents a Radius packet of the type
 * "Accounting-Request".
 */
public class AccountingRequest : RadiusPacket {
	
	/**
	 * Acct-Status-Type: Start
	 */
	public static readonly int ACCT_STATUS_TYPE_START = 1;

	/**
	 * Acct-Status-Type: Stop
	 */
	public static readonly int ACCT_STATUS_TYPE_STOP = 2;

	/**
	 * Acct-Status-Type: Interim Update/Alive
	 */
	public static readonly int ACCT_STATUS_TYPE_INTERIM_UPDATE = 3;

	/**
	 * Acct-Status-Type: Accounting-On
	 */
	public static readonly int ACCT_STATUS_TYPE_ACCOUNTING_ON = 7;

	/**
	 * Acct-Status-Type: Accounting-Off
	 */
	public static readonly int ACCT_STATUS_TYPE_ACCOUNTING_OFF = 8;
	
	/**
	 * Constructs an Accounting-Request packet to be sent to a Radius server.
	 * @param userName user name
	 * @param acctStatusType ACCT_STATUS_TYPE_*
	 */
	public AccountingRequest(String userName, int acctStatusType) 
        :base(ACCOUNTING_REQUEST, GetNextPacketIdentifier())
    {		
		setUserName(userName);
		setAcctStatusType(acctStatusType);
	}
	
	/**
	 * Constructs an empty Accounting-Request to be received by a
	 * Radius client.
	 */
	public AccountingRequest() {
		
	}
	
	/**
	 * Sets the User-Name attribute of this Accountnig-Request.
	 * @param userName user name to set
	 */
	public void setUserName(String userName) {
		if (userName == null)
			throw new ArgumentNullException("user name not set");
		if (userName.Length == 0)
			throw new ArgumentException("empty user name not allowed");
		
		RemoveAttributes(USER_NAME);
		AddAttribute(new StringAttribute(USER_NAME, userName));		
	}
	
	/**
	 * Retrieves the user name from the User-Name attribute.
	 * @return user name
	 */
	public String getUserName() 
	{
		ArrayList attrs = GetAttributes(USER_NAME);
		if (attrs.size() < 1 || attrs.size() > 1)
			throw new NotImplementedException("exactly one User-Name attribute required");
		
		RadiusAttribute ra = (RadiusAttribute)attrs.get(0);
		return ((StringAttribute)ra).getAttributeValue();
	}

	/**
	 * Sets the Acct-Status-Type attribute of this Accountnig-Request.
	 * @param acctStatusType ACCT_STATUS_TYPE_* to set
	 */
	public void setAcctStatusType(int acctStatusType) {
		if (acctStatusType < 1 || acctStatusType > 15)
			throw new ArgumentException("bad Acct-Status-Type");
		RemoveAttributes(ACCT_STATUS_TYPE);
		AddAttribute(new IntegerAttribute(ACCT_STATUS_TYPE, acctStatusType));
	}

	/**
	 * Retrieves the user name from the User-Name attribute.
	 * @return user name
	 */
	public int getAcctStatusType() 
	{
		RadiusAttribute ra = GetAttribute(ACCT_STATUS_TYPE);
		if (ra == null)
			return -1;
		else
			return ((IntegerAttribute)ra).getAttributeValueInt();
	}

	/**
	 * Calculates the request authenticator as specified by RFC 2866.
	 * @see TinyRadius.packet.RadiusPacket#updateRequestAuthenticator(java.lang.String, int, byte[])
	 */
	protected byte[] updateRequestAuthenticator(String sharedSecret, int packetLength, byte[] attributes) {
		byte[] authenticator = new byte[16];
		for (int i = 0; i < 16; i++)
			authenticator[i] = 0;
		
		MessageDigest md5 = GetMd5Digest();
        md5.reset();
        md5.update((byte)Type);
        md5.update((byte)Identifier);
        md5.update((byte)(packetLength >> 8));
        md5.update((byte)(packetLength & 0xff));
        md5.update(authenticator, 0, authenticator.Length);
        md5.update(attributes, 0, attributes.Length);
        md5.update(RadiusUtil.getUtf8Bytes(sharedSecret));
        return md5.digest();
	}
	
	/**
	 * Checks the received request authenticator as specified by RFC 2866.
	 */
	protected void checkRequestAuthenticator(String sharedSecret, int packetLength, byte[] attributes){
		byte[] expectedAuthenticator = updateRequestAuthenticator(sharedSecret, packetLength, attributes);
		byte[] receivedAuth = getAuthenticator();
		for (int i = 0; i < 16; i++)
			if (expectedAuthenticator[i] != receivedAuth[i])
				throw new RadiusException("request authenticator invalid");
	}
	
	/**
	 * Radius User-Name attribute type
	 */
	private static readonly int USER_NAME = 1;
	
	/**
	 * Radius Acct-Status-Type attribute type
	 */
	private static readonly int ACCT_STATUS_TYPE = 40;
	
}
}