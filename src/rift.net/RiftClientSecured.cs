using System;
using System.Collections.Generic;
using AutoMapper;
using RestSharp;
using rift.net.rest;
using rift.net.Models;

namespace rift.net
{
	public class RiftClientSecured : RiftRestClient
	{
		private Session session;

		static RiftClientSecured()
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

			Mapper.CreateMap<ContactData, Contact> ()
				.IncludeBase<CharacterData, Character> ()
				.ForMember (x => x.IsOfficer, y => y.MapFrom (src => src.isOfficer))
				.ForMember(x=>x.Guild, opt => {
					opt.Condition( src => src.guildId > 0 );
					opt.MapFrom( src=> new Guild{ Id = src.guildId, 
						Name = src.guildName,
						ChatPermissions = new ChatPermissions {CanListen = src.canListen, CanTalk = src.canTalk, CanTalkInOfficer = src.isOfficer }});
				});

		}

		public RiftClientSecured (Session session) : base()
		{
			this.session = session;
		}

		public List<Character> ListCharacters()
		{
			var request = CreateRequest ("/chat/characters");

			return ExecuteAndWrap<CharacterData, Character> (request);
		}

		public List<Contact> ListFriends( string characterId )
		{
			var request = CreateRequest ("/friends");
			request.AddQueryParameter ("characterId", characterId);

			return ExecuteAndWrap<ContactData, Contact> (request);
		}

		public List<Contact> ListGuildmates( long guildId )
		{
			var request = CreateRequest ("/guild/members");
			request.AddQueryParameter ("guildId", guildId.ToString());

			return ExecuteAndWrap<ContactData, Contact> (request);
		}
			
		protected override RestRequest CreateRequest( string url, Method method = Method.POST)
		{
			var request = base.CreateRequest (url, method);

			request.AddCookie ("SESSIONID", session.Id);

			return request;
		}
	}
}