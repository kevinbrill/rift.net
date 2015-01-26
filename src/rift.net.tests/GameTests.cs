using System;
using NUnit.Framework;
using System.Linq;
using rift.net.Models.Games;
using Moq;
using rift.net.Models;

namespace rift.net.tests
{
	[TestFixture()]
	public class GameTests
	{
		private ScratchCardClient client;
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

			client = new ScratchCardClient (session);
		}

		[Test()]
		public void List_Scratch_Cards_Should_Return_Back_A_Non_Null_Non_Empty_List()
		{
			var cards = client.ListGames ();

			Assert.That (cards, Is.Not.Null.And.Not.Empty);
		}

		[Test()]
		public void Scratch_Cards_Should_Have_At_Least_Three_Games()
		{
			var cards = client.ListGames ();

			Assert.That (cards, Is.Not.Null.And.Not.Empty);
			Assert.That (cards.Count, Is.AtLeast (3));		
		}

		[TestCase("Planar Treasure")]
		[TestCase("Crafty Critters")]
		[TestCase("Shinies")]
		public void Verify_That_Standard_Card_Games_Exist( string gameName )
		{
			var cards = client.ListGames ();

			Assert.That (cards.FirstOrDefault (x => x.Name == gameName), Is.Not.Null);
		}

		[Test()]
		public void Verify_That_An_Invalid_Game_Throws_An_Exception()
		{
			var game = new Game () { Name = "Test", Url = "/invalid/url" };
			var mock = new Mock<ScratchCardClient> (MockBehavior.Loose, new Session ("foo"));

			mock.Setup (x => x.GetAccountGameInfo ()).Returns (new AccountGameInfo () { AvailablePoints = 1, Games = new System.Collections.Generic.List<Game>() });

			var mockedClient = mock.Object;

			Assert.That (() => mockedClient.Play (game, characterId), Throws.TypeOf<InvalidGameException> ());
		}

		[Test()]
		public void Verify_That_Being_Out_Of_Points_Throws_An_Exception()
		{
			var game = new Game () { Name = "Test", Url = "/invalid/url" };
			var mock = new Mock<ScratchCardClient> (MockBehavior.Loose, new Session ("foo"));

			mock.Setup (x => x.GetAccountGameInfo ()).Returns (new AccountGameInfo () { AvailablePoints = 0 });

			var mockedClient = mock.Object;

			Assert.That (() => mockedClient.Play (game, characterId), Throws.TypeOf<NoCardsAvailableException> ());
		}

		[Test()]
		[Ignore()]
		public void Verify_Play()
		{
			var card = new Game () { Name = "Shinies", Url = "/chatservice/scratch/threeofsix" };

			client.Play (card, characterId);
		}
	}
}

