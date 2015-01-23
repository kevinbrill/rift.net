using System;
using NUnit.Framework;
using System.Configuration;

namespace rift.net.tests
{
	[TestFixture()]
	public class ChatClientTests
	{
		private RiftChatClient client;
		private string characterId;

		[TestFixtureSetUp()]
		public void SetUp()
		{
			var username = ConfigurationManager.AppSettings ["username"];
			var password = ConfigurationManager.AppSettings ["password"];
			characterId = ConfigurationManager.AppSettings ["characterId"];

			var sessionFactory = new SessionFactory ();

			var session = sessionFactory.Login (username, password);

			client = new RiftChatClient (session, characterId);
		}

		[Test()]
		public void Verify_Starting_Chat_Client()
		{
			client.Start ();
		}
	}
}

