using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using rift.net.rest;
using rift.net.Models;
using rift.net.Models.Guilds;
using System.Linq;

namespace rift.net
{
	public class RiftClientSecured : RiftRestClientSecured
	{
		static RiftClientSecured()
		{
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

			var characterData = Execute<List<CharacterData>> (request);

			return characterData != null ? characterData.Select(MapCharacterData).ToList() : new List<Character>();
		}

        /// <summary>
        /// Logs the supplied character in.  Performing this action will change their 
        /// web presence to online.
        /// </summary>
        /// <returns>True if the character was successfully logged in, false otherwise</returns>
	    public bool GoOnline(string characterId)
	    {
            var request = CreateRequest("/selectCharacter", Method.GET);
            request.AddQueryParameter("characterId", characterId);

            var response = Client.Execute(request);

	        return response.StatusCode == HttpStatusCode.OK;
	    }

        /// <summary>
        /// Logs off the supplied character.  Performing this action will change their 
        /// web presence to offline.
        /// </summary>
        /// <returns>True if the character was successfully logged off, false otherwise</returns>
	    public bool GoOffline(string characterId)
	    {
            var request = CreateRequest("/unselectCharacter", Method.GET);
            request.AddQueryParameter("characterId", characterId);

            var response = Client.Execute(request);

	        return response.StatusCode == HttpStatusCode.OK;	        
	    }

		/// <summary>
		/// Lists all friend for the currently authenticated acount
		/// </summary>
		public List<Contact> ListFriends( string characterId )
		{
			var request = CreateRequest ("/friends");
			request.AddQueryParameter ("characterId", characterId);

			var contactData = Execute<List<ContactData>> (request);

			return contactData != null ? contactData.Select (MapContactData).ToList () : new List<Contact> ();
		}

		/// <summary>
		/// Lists all members of the provided guild 
		/// </summary>
		public List<Contact> ListGuildmates( long guildId )
		{
			var request = CreateRequest ("/guild/members");
			request.AddQueryParameter ("guildId", guildId.ToString());

			var contactData = Execute<List<ContactData>> (request);

			return contactData != null ? contactData.Select (MapContactData).ToList () : new List<Contact> ();
		}

		/// <summary>
		/// Gets information on the guild to which the supplied character is a member.  
		/// This information includes wall posts, the message of the day, the level,
		/// and shard
		/// </summary>
		public Info GetGuildInfo( string characterId )
		{
			var request = CreateRequest ("/guild/info");
			request.AddQueryParameter ("characterId", characterId);

			var guildData = Execute<GuildData> (request);

			return MapFullGuildData (guildData);
		}

		private Character MapCharacterData( CharacterData characterData ) 
		{
			return MapCharacterData (characterData, new Character ());
		}

		private Character MapCharacterData( CharacterData characterData, Character character )
		{
			if( characterData.guildId > 0 )
			{
				character.Guild = new Guild {
					Id = characterData.guildId,
					Name = characterData.guildName,
					ChatPermissions = new ChatPermissions {
						CanListen = characterData.guildListen,
						CanTalk = characterData.guildTalk,
						CanTalkInOfficer = characterData.guildOfficerChat
					}
				};
			}

			character.Id = characterData.playerId;
			character.Name = characterData.name;

			character.Presence = new Presence {
				IsOnlineInGame = characterData.onlineGame,
				IsOnlineOnWeb = characterData.onlineWeb
			};

			character.Shard = new Shard {
				Id = characterData.shardId,
				Name = characterData.shardName
			};

			return character;
		}

		private Contact MapContactData( ContactData contactData )
		{
			var contact = new Contact ();

			MapCharacterData (contactData, contact);

			if( contact.Guild != null ) {
				contact.Guild.ChatPermissions = new ChatPermissions {
					CanListen = contactData.canListen,
					CanTalk = contactData.canTalk,
					CanTalkInOfficer = contactData.isOfficer
				};
			}

			contact.IsOfficer = contactData.isOfficer;

			return contact;
		}
		
		private Info MapFullGuildData( GuildData guildData )
		{
			var guild = new Info ();

			guild.Id = guildData.guildId;
			guild.Name = guildData.name;
			guild.Level = guildData.level;
			guild.MessageOfTheDay = guildData.motd;
			guild.ShardId = guildData.shardId;
			guild.Wall = guildData.wall != null ? guildData.wall.Select (MapWallPostData).ToList () : new List<WallPost> ();

			return guild;
		}

		private WallPost MapWallPostData(WallPostData wallPost) 
		{
			return new WallPost {
				Author = wallPost.postedBy,
				Message = wallPost.text,
				PostDate = new DateTime (1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds (wallPost.posted).ToLocalTime ()
			};
		}
	}
}