using System;

namespace rift.net
{
	public class ChatData
	{
		public string messageId {
			get;
			set;
		}

		public long recipientId {
			get;
			set;
		}

		public long messageTime {
			get;
			set;
		}

		public long senderId {
			get;
			set;
		}

		public string senderName {
			get;
			set;
		}

		public string message {
			get;
			set;
		}
	}
}

