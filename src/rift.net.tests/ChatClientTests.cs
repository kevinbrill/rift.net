using System;
using NUnit.Framework;
using System.Configuration;
using System.Linq;
using rift.net.Models;

namespace rift.net.tests
{
	[TestFixture()]
	public class ChatClientTests
	{
		private RiftChatClient client;
		private string characterId;
		private Character character;

		[TestFixtureSetUp()]
		public void SetUp()
		{
			var username = ConfigurationManager.AppSettings ["username"];
			var password = ConfigurationManager.AppSettings ["password"];
			characterId = ConfigurationManager.AppSettings ["characterId"];

			var sessionFactory = new SessionFactory ();

			var session = sessionFactory.Login (username, password);

			client = new RiftChatClient (session, characterId);

			var securedClient = new RiftClientSecured (session);

			character = securedClient.ListCharacters ().FirstOrDefault (x => x.Id == characterId);
		}

		[Test()]
		public void Verify_Starting_Chat_Client()
		{
			client.Start ();
		}

		[Test()]
		public void Verify_That_Character_Has_Chat_History()
		{
			var chatHistory = client.ListChatHistory(characterId);

			Assert.That (chatHistory, Is.Not.Null.And.Not.Empty);
			Assert.That (chatHistory.All (x => x.RecipientId.ToString() == characterId), Is.True);
		}

		[Test()]
		public void Verify_That_Character_Guild_Has_Chat_History()
		{
			Assume.That (character, Is.Not.Null);
			Assume.That (character.Guild, Is.Not.Null);

			var chatHistory = client.ListGuildChatHistory(characterId);

			Assert.That (chatHistory, Is.Not.Null.And.Not.Empty);
			Assert.That (chatHistory.All (x => x.RecipientId == character.Guild.Id), Is.True);
		}
	}
}

