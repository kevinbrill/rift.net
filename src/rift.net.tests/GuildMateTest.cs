using System;
using NUnit.Framework;
using System.Linq;

namespace rift.net.tests
{
	[TestFixture()]
	public class GuildMateTests
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
		public void Verify_That_Guild_Has_Members()
		{
			var guildMates = client.ListGuildmates (guildId);

			Assert.That (guildMates, Is.Not.Null.And.Not.Empty);
			Assert.That (guildMates [0].Guild.Id, Is.EqualTo (guildId));
		}

		[Test()]
		public void Verify_That_Guild_Has_Officers()
		{
			var guildMates = client.ListGuildmates (guildId);

			var hasOfficers = guildMates.Any(x => x.IsOfficer);

			Assert.That (hasOfficers, Is.True);
		}

		[Test()]
		public void Verify_That_Characters_Guild_Is_Valid()
		{
			var info = client.GetGuildInfo (characterId);

			Assert.That (info, Is.Not.Null);
			Assert.That (info.Id, Is.GreaterThan (0));
		}
	}
}