using System;
using System.Collections.Generic;

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

/*
{"status":"success","data":[{"playerId":"219691219542812657","name":"Lunestra","shardId":1706,"shardName":"Wolfsbane","guildId":0,"guildName":"",
"guildTalk":0,"guildListen":0,"guildOfficerChat":0,"onlineGame":0,"onlineWeb":1},{"playerId":"218846794414042822","name":"Bruun","shardId":1706,"shardName":"Wolfsbane","guildId":219691219334217446,"guildName":"Grievance","guildTalk":1,"guildListen":1,"guildOfficerChat":0,"onlineGame":0,"onlineWeb":1},{"playerId":"211528445430540694","name":"Revani","shardId":1707,"shardName":"Faeblight","guildId":0,"guildName":"","guildTalk":0,"guildListen":0,"guildOfficerChat":0,"onlineGame":0,"onlineWeb":1},{"playerId":"215750570089610820","name":"Tendaru","shardId":1704,"shardName":"Deepwood","guildId":0,"guildName":"","guildTalk":0,"guildListen":0,"guildOfficerChat":0,"onlineGame":0,"onlineWeb":1},{"playerId":"219691219819500799","name":"Lunestara","shardId":1706,"shardName":"Wolfsbane","guildId":0,"guildName":"","guildTalk":0,"guildListen":0,"guildOfficerChat":0,"onlineGame":0,"onlineWeb":1},{"playerId":"219691219830971257","name":"Chondara","shardId":1706,"shardName":"Wolfsbane","guildId":0,"guildName":"","guildTalk":0,"guildListen":0,"guildOfficerChat":0,"onlineGame":0,"onlineWeb":1}]}
*/