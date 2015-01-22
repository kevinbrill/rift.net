using System;
using System.Collections.Generic;
using AutoMapper;
using RestSharp;
using rift.net.rest;

namespace rift.net
{
	public class RiftClient
	{
		private const string url = "http://chat-us.riftgame.com:8080/chatservice";
		private Session session;
		private RestClient client;

		static RiftClient()
		{
			Mapper.CreateMap<CharacterData, Character> ()
				.ForMember (x => x.Id, y => y.MapFrom (src => src.playerId))
				.ForMember (x => x.Name, y => y.MapFrom (src => src.name))
				.ForMember (x => x.Shard, y => y.MapFrom (src => new Shard { Id = src.shardId, Name = src.shardName }))
				.ForMember (x => x.Presence, y => y.MapFrom (src => new Presence {
					IsOnlineInGame = src.onlineGame,
					IsOnlineOnWeb = src.onlineWeb
				}))
				.ForMember(x=>x.Guild, opt => {
					opt.Condition( src => src.guildId > 0 );
					opt.MapFrom( src=> new Guild{ Id = src.guildId, 
						Name = src.guildName,
						ChatPermissions = new ChatPermissions {CanListen = src.guildListen, CanTalk = src.guildTalk, CanTalkInOfficer = src.guildOfficerChat }});
				});

			Mapper.CreateMap<GuildMateData, GuildMate> ()
				.IncludeBase<CharacterData, Character> ()
				.ForMember (x => x.IsOfficer, y => y.MapFrom (src => src.isOfficer))
				.ForMember (x => x.DisplayId, y => y.MapFrom (src => src.displayId))
				.ForMember (x => x.CountOfUnreadMessageFrom, y => y.MapFrom (src => src.unreadMsgs));

			Mapper.CreateMap<ShardData, Shard> ()
				.ForMember (x => x.Id, y => y.MapFrom (src => src.shardId))
				.ForMember (x => x.Name, y => y.MapFrom (src => src.name));
		}

		public RiftClient (Session session)
		{
			this.session = session;
			this.client = new RestClient (new Uri (url));
		}

		public List<Character> ListCharacters()
		{
			var request = CreateRequest ("/chat/characters");

			var response = client.Execute(request);

			var content = SimpleJson.DeserializeObject<JsonResponse<CharacterData>> (response.Content);

			return Mapper.Map<List<Character>> (content.data);
		}

		public List<Shard> ListShards()
		{
			var request = CreateRequest ("/shard/list");

			var response = client.Execute(request);

			var content = SimpleJson.DeserializeObject<JsonResponse<ShardData>> (response.Content);

			return Mapper.Map<List<Shard>> (content.data);
		}

		public List<Character> ListFriends( string characterId )
		{
			var request = CreateRequest ("/friends");
			request.AddQueryParameter ("characterId", characterId);

			var response = client.Execute(request);

			var content = SimpleJson.DeserializeObject<JsonResponse<Character>> (response.Content);

			return Mapper.Map<List<Character>> (content.data);
		}

		public List<GuildMate> ListGuildmates( long guildId )
		{
			var request = CreateRequest ("/guild/members");
			request.AddQueryParameter ("guildId", guildId.ToString());

			var response = client.Execute(request);

			var content = SimpleJson.DeserializeObject<JsonResponse<GuildMateData>> (response.Content);

			return Mapper.Map<List<GuildMate>> (content.data);
		}

		private RestRequest CreateRequest( string url, Method method = Method.POST)
		{
			var request = new RestRequest (url, method);
			request.AddCookie ("SESSIONID", session.Id);

			return request;
		}
	}
}