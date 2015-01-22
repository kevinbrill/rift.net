using System;

namespace rift.net.rest
{
	public class GuildData
	{
		public long guildId {
			get;
			set;
		}

		public int level {
			get;
			set;
		}

		public string motd {
			get;
			set;
		}

		public string name {
			get;
			set;
		}

		public int shardId {
			get;
			set;
		}

		public WallPostData[] wall {
			get;
			set;
		}
	}

	public class WallPostData
	{
		public long posted {
			get;
			set;
		}

		public string postedBy {
			get;
			set;
		}

		public string text {
			get;
			set;
		}
	}
}

