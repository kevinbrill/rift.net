using System;
using NUnit.Framework;
using System.IO;

namespace rift.net.tests
{
	[TestFixture()]
	public class ScratchCardParserTests
	{
		[Test()]
		public void Verify_That_A_Loser_Is_A_Loser()
		{
			var contents = LoadFile ("ScratchResults/loser.html");

			var parser = new ScratchResultParser ();

			var results = parser.Parse (contents);

			Assert.That (results.IsReplay, Is.False);
			Assert.That (results.IsWinner, Is.False);
		}

		[Test()]
		public void Verify_That_A_Winner_Sets_The_Is_A_Winner_Flag()
		{
			var contents = LoadFile ("ScratchResults/winner.html");

			var parser = new ScratchResultParser ();

			var results = parser.Parse (contents);

			Assert.That (results.IsReplay, Is.False);
			Assert.That (results.IsWinner, Is.True);
			Assert.That(results.Prizes, Is.Not.Null);
			Assert.That(results.Prizes.Count, Is.EqualTo(1));
		}

		private string LoadFile(string filePath)
		{
			using (var streamReader = new StreamReader (new FileStream (filePath, FileMode.Open))) {
				return streamReader.ReadToEnd ();
			}
		}
	}
}

