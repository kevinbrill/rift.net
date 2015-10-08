using System;
using rift.net.games.models;

namespace rift.net.games.exceptions
{
	public class InvalidGameException : Exception
	{
		public Game Game {
			get;
			private set;
		}

		public InvalidGameException (Game game)
			: base(string.Format("{0} is not a valid game.", game.Name))
		{
			Game = game;
		}
	}

}