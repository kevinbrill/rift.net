using System;

namespace rift.net.rest
{
	public class GuildMateData : CharacterData
	{
		public int displayId {
			get;
			set;
		}

		public bool isOfficer {
			get;
			set;
		}

		public int unreadMsgs {
			get;
			set;
		}
	}
}

