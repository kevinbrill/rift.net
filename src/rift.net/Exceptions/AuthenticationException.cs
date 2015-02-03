using System;

namespace rift.net
{
	public class AuthenticationException : Exception
	{
		public string Username {
			get;
			private set;
		}

		public AuthenticationException (string username) : base(string.Format("Unable to authenticate the user '{0}'.  Check your username and password", username ))
		{
			Username = username;
		}
	}
}