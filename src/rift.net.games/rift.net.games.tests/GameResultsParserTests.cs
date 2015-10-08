using System;
using NUnit.Framework;
using System.IO;

namespace rift.net.games.tests
{
	[TestFixture()]
	public class GameResultsParserTests
	{
		[Test()]
		public void Verify_That_A_Loser_Is_A_Loser()
		{
			var contents = LoadFile ("ScratchResults/loser.html");

			var parser = new GameResultsParser ();

			var results = parser.Parse (contents);

			Assert.That (results.IsReplay, Is.False);
			Assert.That (results.IsWinner, Is.False);
		}

		[Test()]
		public void Verify_That_A_Winner_Sets_The_Is_A_Winner_Flag()
		{
			const string redeemUrl = "/chatservice/scratch/redeem?game=94cd033c-ef95-4186-9b4c-062998d7fbb5";
			var contents = LoadFile ("ScratchResults/winner.html");

			var parser = new GameResultsParser ();

			var results = parser.Parse (contents);

			Assert.That (results.IsReplay, Is.False);
			Assert.That (results.IsWinner, Is.True);
			Assert.That (results.Prizes, Is.Not.Null);
			Assert.That (results.Prizes.Count, Is.EqualTo (1));
			Assert.That (results.FollowUpUrl, Is.EqualTo (redeemUrl));
		}

		[Test()]
		public void Verify_That_A_Replay_Returns_The_Correct_Replay_Url()
		{
			const string replayUrl = "/chatservice/scratch/threeofsix?characterId=218846794414042822&replayUUID=4cc0d0f6-09eb-4ece-b24c-b22797d5ac94";

			var contents = LoadFile ("ScratchResults/replay.html");

			var parser = new GameResultsParser ();

			var results = parser.Parse (contents);

			Assert.That (results.IsReplay, Is.True);
			Assert.That (results.IsWinner, Is.False);
			Assert.That (results.Prizes, Is.Null);
			Assert.That (results.FollowUpUrl, Is.EqualTo (replayUrl));
		}

		private string LoadFile(string filePath)
		{
			using (var streamReader = new StreamReader (new FileStream (filePath, FileMode.Open))) {
				return streamReader.ReadToEnd ();
			}
		}
	}
}