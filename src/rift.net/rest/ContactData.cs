using System;

namespace rift.net.rest
{
	public class ContactData : CharacterData
	{
		public bool canTalk {
			get;
			set;
		}

		public bool canListen {
			get;
			set;
		}

		public bool isOfficer {
			get;
			set;
		}
	}
}

