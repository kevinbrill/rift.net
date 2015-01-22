using System;

namespace rift.net.Models
{
	public class Guild
	{
		public long Id {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public ChatPermissions ChatPermissions {
			get;
			set;
		}
	}

	public class ChatPermissions 
	{
		public bool CanTalk {
			get;
			set;
		}

		public bool CanListen {
			get;
			set;
		}

		public bool CanTalkInOfficer {
			get;
			set;
		}
	}
}

