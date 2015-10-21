using System;
using RestSharp;
using System.Net;
using System.Linq;

namespace rift.net.Models
{
	public class Session
	{
		public Session(string sessionId, string ticket)
		{
			Id = sessionId;
			Ticket = ticket;
		}

		public string Id {
			get;
			private set;
		}

		public string Ticket {
			get;
			private set;
		}
	}
}

