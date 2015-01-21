using System;
using System.Collections.Generic;

namespace rift.net.rest
{
	public class CharacterData
	{
		public string playerId {
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

		public string shardName {
			get;
			set;
		}

		public long guildId {
			get;
			set;
		}

		public string guildName {
			get;
			set;
		}

		public bool guildTalk {
			get;
			set;
		}

		public bool guildListen {
			get;
			set;
		}

		public bool guildOfficerChat {
			get;
			set;
		}

		public bool onlineGame {
			get;
			set;
		}

		public bool onlineWeb {
			get;
			set;
		}
	}
}