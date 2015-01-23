using System;
using rift.net.Models;
using RestSharp;

namespace rift.net
{
	public class RiftRestClientSecured : RiftRestClient
	{
		protected Session session;

		public RiftRestClientSecured (Session session) : base()
		{
			this.session = session;
		}

		protected override RestRequest CreateRequest( string url, Method method = Method.POST)
		{
			var request = base.CreateRequest (url, method);

			request.AddCookie ("SESSIONID", session.Id);

			return request;
		}
	}
}

