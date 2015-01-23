using System;
using NUnit.Framework;
using System.Linq;

namespace rift.net.tests
{
	public class ScratchCardTests
	{
		private RiftClientSecured client;
		private long guildId;
		private string characterId;

		[TestFixtureSetUp()]
		public void SetUp()
		{
			var username = System.Configuration.ConfigurationManager.AppSettings ["username"];
			var password = System.Configuration.ConfigurationManager.AppSettings ["password"];
			var guildSetting = System.Configuration.ConfigurationManager.AppSettings ["guildId"];
			characterId = System.Configuration.ConfigurationManager.AppSettings ["characterId"];

			guildId = long.Parse (guildSetting);

			var sessionFactory = new SessionFactory ();

			var session = sessionFactory.Login (username, password);

			client = new RiftClientSecured (session);
		}

		[Test()]
		public void List_Scratch_Cards_Should_Return_Back_A_Non_Null_Non_Empty_List()
		{
			var cards = client.ListScratchCards ();

			Assert.That (cards, Is.Not.Null.And.Not.Empty);
		}

		[Test()]
		public void Scratch_Cards_Should_Have_At_Least_Three_Games()
		{
			var cards = client.ListScratchCards ();

			Assert.That (cards, Is.Not.Null.And.Not.Empty);
			Assert.That (cards.Count, Is.AtLeast (3));		
		}

		[TestCase("Planar Treasure")]
		[TestCase("Crafty Critters")]
		[TestCase("Shinies")]
		public void Verify_That_Standard_Card_Games_Exist( string gameName )
		{
			var cards = client.ListScratchCards ();

			Assert.That (cards.FirstOrDefault (x => x.Name == gameName), Is.Not.Null);
		}
	}
}

