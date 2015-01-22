using System;
using NUnit.Framework;
using System.Configuration;

namespace rift.net.tests
{
	[TestFixture()]
	public class CharacterTests
	{
		private RiftClient client;

		[TestFixtureSetUp()]
		public void SetUp()
		{
			var username = ConfigurationManager.AppSettings ["username"];
			var password = ConfigurationManager.AppSettings ["password"];

			var sessionFactory = new SessionFactory ();

			var session = sessionFactory.Login (username, password);

			client = new RiftClient (session);
		}

		[Test()]
		public void Verify_That_Character_List_Has_Data()
		{
			var characters = client.ListCharacters ();

			Assert.That (characters, Is.Not.Null.Or.Empty);
		}

		[Test()]
		public void Verify_That_Each_Character_Has_A_Shard()
		{
			var characters = client.ListCharacters ();

			Assert.That (characters, Is.Not.Null.Or.Empty);

			foreach (var character in characters) 
			{
				Assert.That (character.Shard, Is.Not.Null);
				Assert.That (character.Shard.Id, Is.GreaterThan (0));
				Assert.That (character.Shard.Name, Is.Not.Null.And.Not.Empty);
			}
		}

		[Test()]
		public void Verify_That_Each_Character_Appears_Online_In_Web()
		{
			var characters = client.ListCharacters ();

			Assert.That (characters, Is.Not.Null.Or.Empty);

			foreach (var character in characters) 
			{
				Assert.That (character.Presence, Is.Not.Null);
				Assert.That (character.Presence.IsOnlineOnWeb, Is.True);
			}
		}

		[Test()]
		public void Verify_That_Character_Has_Friends()
		{
			var characterId = ConfigurationManager.AppSettings ["characterId"];

			var contacts = client.ListFriends (characterId);

			Assert.That (contacts, Is.Not.Null.Or.Empty);
		}
	}
}

