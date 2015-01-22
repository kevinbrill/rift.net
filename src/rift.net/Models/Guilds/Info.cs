using System;
using System.Collections.Generic;

namespace rift.net.Models.Guilds
{
	public class Info
	{
		public long Id {
			get;
			set;
		}

		public int Level {
			get;
			set;
		}

		public string MessageOfTheDay {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public int ShardId {
			get;
			set;
		}

		public List<WallPost> Wall {
			get;
			set;
		}
	}
}

