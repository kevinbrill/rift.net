using System;

namespace rift.net
{
	public class GuildMate : Character
	{
		public int DisplayId {
			get;
			set;
		}

		public bool IsOfficer {
			get;
			set;
		}

		public int CountOfUnreadMessageFrom {
			get;
			set;
		}
	}
}

