﻿using System;
using RestSharp;
using System.Linq;
using System.Net;

namespace rift.net
{
	public class SessionFactory
	{
		const string loginUrl = "https://chat-us.riftgame.com/chatservice/loginByTicket?os=iOS&osVersion=5.100000&vendor=Apple";

		public SessionFactory ()
		{
		}

		public Session Login(string username, string password)
		{
			var restClient = new RestClient ();
			restClient.BaseUrl = new Uri("https://auth.trionworlds.com/auth");

			var request = new RestRequest (Method.POST);
			request.AddHeader ("UserAgent", "trion/mobile");

			request.AddParameter ("username", username, ParameterType.GetOrPost);
			request.AddParameter ("password", password, ParameterType.GetOrPost);
			request.AddParameter ("channel", 1, ParameterType.GetOrPost);
			request.AddHeader ("Accept", "application/json");

			var response = restClient.Execute (request);

			if (response.StatusCode != System.Net.HttpStatusCode.OK)
				throw new AccessViolationException ();

			restClient.BaseUrl = new Uri (loginUrl);

			request = new RestRequest (Method.POST);
			request.AddParameter ("ticket", response.Content);

			response = restClient.Execute(request);

			var cookie = response.Cookies.FirstOrDefault (x => x.Name == "SESSIONID");

			if (cookie == null)
				throw new AccessViolationException ();

			var session = new Session (cookie.Value);

			return session;
		}
	}
}