using System;
using System.Collections.Generic;
using AutoMapper;
using RestSharp;
using rift.net.rest;
using rift.net.Models;
using rift.net.Models.Guilds;

namespace rift.net
{
	public class RiftClientSecured : RiftRestClientSecured
	{
		static RiftClientSecured()
		{
			Mapper.CreateMap<CharacterData, Character> ()
				.ForMember (x => x.Id, y => y.MapFrom (src => src.playerId))
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

			Mapper.CreateMap<ContactData, Contact> ()
				.IncludeBase<CharacterData, Character> ()
				.ForMember(x=>x.Guild, opt => {
					opt.Condition( src => src.guildId > 0 );
					opt.MapFrom( src=> new Guild{ Id = src.guildId, 
						Name = src.guildName,
						ChatPermissions = new ChatPermissions {CanListen = src.canListen, CanTalk = src.canTalk, CanTalkInOfficer = src.isOfficer }});
				});


			Mapper.CreateMap<WallPostData, WallPost> ()
				.ForMember (x => x.Author, y => y.MapFrom (src => src.postedBy))
				.ForMember (x => x.Message, y => y.MapFrom (src => src.text))
				.ForMember (x => x.PostDate, y => y.MapFrom (src => new DateTime (1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds (src.posted).ToLocalTime ()));

			Mapper.CreateMap<GuildData, Info> ()
				.ForMember (x => x.Id, y => y.MapFrom (src => src.guildId))
				.ForMember (x => x.MessageOfTheDay, y => y.MapFrom (src => src.motd));
		}

		public RiftClientSecured (Session session) : base(session)
		{
		}

		/// <summary>
		/// Lists all characters across all shards for the currently authenticated account
		/// </summary>
		public List<Character> ListCharacters()
		{
			var request = CreateRequest ("/chat/characters");

			return ExecuteAndWrap<List<CharacterData>, List<Character>> (request);
		}

		/// <summary>
		/// Lists all friend for the currently authenticated acount
		/// </summary>
		public List<Contact> ListFriends( string characterId )
		{
			var request = CreateRequest ("/friends");
			request.AddQueryParameter ("characterId", characterId);

			return ExecuteAndWrap<List<ContactData>, List<Contact>> (request);
		}

		/// <summary>
		/// Lists all members of the guild 
		/// </summary>
		public List<Contact> ListGuildmates( long guildId )
		{
			var request = CreateRequest ("/guild/members");
			request.AddQueryParameter ("guildId", guildId.ToString());

			return ExecuteAndWrap<List<ContactData>, List<Contact>> (request);
		}

		public Info GetGuildInfo( string characterId )
		{
			var request = CreateRequest ("/guild/info");
			request.AddQueryParameter ("characterId", characterId);

			return ExecuteAndWrap<GuildData, Info> (request);
		}
	}
}