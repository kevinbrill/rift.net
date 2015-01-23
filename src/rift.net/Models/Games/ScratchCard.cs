using System;
using System.Collections.Generic;

namespace rift.net.Models.Games
{
	public class ScratchCard
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

		public List<Card> Cards {
			get;
			set;
		}
	}
}

