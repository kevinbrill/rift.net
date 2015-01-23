using System;
using System.Collections.Generic;
using AutoMapper;
using RestSharp;
using rift.net.rest;
using rift.net.Models;
using rift.net.Models.Guilds;

namespace rift.net
{
	public class RiftClientSecured : RiftRestClient
	{
		private Session session;

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

			Mapper.CreateMap<ScratchCardData, Card> ();
			Mapper.CreateMap<AccountScratchCardData, ScratchCard> ()
				.ForMember (x => x.MaximumPoints, y => y.MapFrom (src => src.maxPoints));
		}

		public RiftClientSecured (Session session) : base()
		{
			this.session = session;
		}

		public List<Character> ListCharacters()
		{
			var request = CreateRequest ("/chat/characters");

			return ExecuteAndWrap<List<CharacterData>, List<Character>> (request);
		}

		public List<Contact> ListFriends( string characterId )
		{
			var request = CreateRequest ("/friends");
			request.AddQueryParameter ("characterId", characterId);

			return ExecuteAndWrap<List<ContactData>, List<Contact>> (request);
		}

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

		public ScratchCard GetAccountScratchCardSummary()
		{
			var request = CreateRequest ("/scratch/cards");

			return ExecuteAndWrap<AccountScratchCardData, ScratchCard> (request);
		}

		public List<Card> ListScratchCards()
		{
			var request = CreateRequest ("/scratch/cards");

			var scratchSummary = ExecuteAndWrap<AccountScratchCardData, ScratchCard> (request);

			return scratchSummary.Cards;
		}

		protected override RestRequest CreateRequest( string url, Method method = Method.POST)
		{
			var request = base.CreateRequest (url, method);

			request.AddCookie ("SESSIONID", session.Id);

			return request;
		}
	}
}