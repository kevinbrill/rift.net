using rift.net.Models.Guilds;

namespace rift.net.Models
{
	public class Character
	{
		public string Id {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

	    public string FullName
	    {
            get { return string.Format("{0}@{1}", Name, Shard.Name); }
	    }

		public Shard Shard {
			get;
			set;
		}

		public Presence Presence {
			get;
			set;
		}

		public Guild Guild {
			get;
			set;
		}
	}
}