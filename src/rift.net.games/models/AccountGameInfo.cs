using System;
using System.Collections.Generic;

namespace rift.net.games.models
{
	public class AccountGameInfo
	{
		public int AvailablePoints {
			get;
			set;
		}

		public int MaximumPoints {
			get;
			set;
		}

		public int SecondsUntilNextPoint {
			get;
			set;
		}

		public long UserId {
			get;
			set;
		}

		public List<Game> Games {
			get;
			set;
		}
	}
}

