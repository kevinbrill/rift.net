using System;

namespace rift.net
{
	public class ChatData
	{
		/*
		 * "messageId":"300811425",
		 * "recipientId":218846794414042822,
		 * "messageTime":1385959510,
		 * "senderId":240098155572218997,
		 * "senderName":"Thamin",
		 * "message":"I\u0026apos;d talk more but we\u0026apos;re powering some folks through a dungeon. Once you have access to Rift member\u0026apos;s section, look for my \u0026quot;pocket guide to grievance/rift\u0026quot;, answers a lot of questions"
		 */
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

