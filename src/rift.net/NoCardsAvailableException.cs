using System;

namespace rift.net
{
	public class NoCardsAvailableException : Exception
	{
		public int SecondsUntilNextPoint {
			get;
			private set;
		}

		public NoCardsAvailableException ( int secondsUntilNextPoint ) : 
			base( string.Format( "No points are available.  You have {0} seconds until you can play again.", secondsUntilNextPoint ) )
		{
			SecondsUntilNextPoint = secondsUntilNextPoint;
		}
	}

}