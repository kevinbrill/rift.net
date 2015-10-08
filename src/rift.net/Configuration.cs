using System;
using Models;

namespace rift.net
{
	public static class Configuration
	{
		const string US_URL = "http://web-api-us.riftgame.com:8080/chatservice";
		const string EU_URL = "http://web-api-eu.riftgame.com:8080/chatservice";
			
		static Configuration()
		{
			Region = Region.UnitedStates;
		}

		public static Region Region { get; set; }

		public static string Url {
			get { 
				return Region == Region.UnitedStates ? US_URL : EU_URL;
			}
		}
	}
}

