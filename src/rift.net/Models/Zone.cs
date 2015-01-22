using System;

namespace rift.net.Models
{
	public class Zone
	{
		public int Id {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public ZoneLocation Location {
			get {
				switch (Id) {
				case 26:
				case 6:
				case 26580443:
				case 19:
				case 20:
				case 1992854106:
				case 24:
				case 12:
				case 1481781477:
				case 22:
				case 27:
				case 336995470:
					return ZoneLocation.Mathosia;
				case 282584906:
				case 790513416:
				case 1300766935:
				case 1967477725:
				case 1213399942:
				case 1446819710:
				case 1770829751:
					return ZoneLocation.Brevane;
				case 479431687:
				case 1494372221:
				case 956914599:
				case 798793247:
					return ZoneLocation.Dusken;
				case 302:
				case 301:
				case 303:
					return ZoneLocation.PlaneOfWater;
				default:
					return ZoneLocation.Unknown;
				}
			}
		}

		public ZoneEvent Event {
			get;
			set;
		}
	}
}