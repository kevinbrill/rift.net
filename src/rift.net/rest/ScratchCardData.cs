using System;

namespace rift.net
{
	public class AccountScratchCardData 
	{
		public long userId {
			get;
			set;
		}
		public int availablePoints {
			get;
			set;
		}

		public int secondsUntilNextPoint {
			get;
			set;
		}

		public int maxPoints {
			get;
			set;
		}

		public ScratchCardData[] cards {
			get;
			set;
		}
	}

	public class ScratchCardData
	{
		public string background {
			get;
			set;
		}

		public int cost {
			get;
			set;
		}

		public string instructions {
			get;
			set;
		}

		public string mask {
			get;
			set;
		}

		public string name {
			get;
			set;
		}

		public string preview {
			get;
			set;
		}

		public string url {
			get;
			set;
		}
	}
}

