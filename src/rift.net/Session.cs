using System;
using RestSharp;
using System.Net;
using System.Linq;

namespace rift.net
{
	public class Session
	{
		public Session(string sessionId)
		{
			Id = sessionId;
		}

		public string Id {
			get;
			private set;
		}
	}
}

