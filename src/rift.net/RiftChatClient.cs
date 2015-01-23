using System;
using rift.net.Models;
using RestSharp;
using System.Net;
using System.IO;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace rift.net
{
	public class RiftChatClient : RiftRestClientSecured
	{
		private Character character;
		private List<Contact> guildMates = new List<Contact>();
		private List<Contact> friends = new List<Contact>();
		private Stream stream;

		static RiftChatClient()
		{
			Mapper.CreateMap<ChatData, Message> ()
				.ForMember (x => x.Id, y => y.MapFrom (src => src.messageId))
				.ForMember (x => x.Sender, y => y.MapFrom (src => new Sender{ Id = src.senderId, Name = src.senderName }))
				.ForMember (x => x.Text, y => y.MapFrom (src => src.message))
				.ForMember (x => x.ReceiveDateTime, y => y.MapFrom (src => new DateTime (1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds (src.messageTime).ToLocalTime ()));
		}

		public RiftChatClient (Session session, Character character) : base(session)
		{
			this.character = character;

			var securedClient = new RiftClientSecured (session);

			// Get the character's friends
			this.friends = securedClient.ListFriends (this.character.Id);

			// If the character is valid, then grab the guild mates
			if (this.character.Guild != null) 
			{
				guildMates = securedClient.ListGuildmates (this.character.Guild.Id);
			}
		}

		public List<Message> ListChatHistory()
		{
			var request = CreateRequest ("/chat/list");
			request.AddQueryParameter ("characterId", character.Id);

			return ExecuteAndWrap<List<ChatData>, List<Message>> (request);
		}

		public List<Message> ListGuildChatHistory()
		{
			var request = CreateRequest ("/guild/listChat");
			request.AddQueryParameter ("characterId", character.Id);

			return ExecuteAndWrap<List<ChatData>, List<Message>> (request);
		}

		public void Start()
		{
			var parser = new ChatMessageParser ();
			var selectCharacterRequest = CreateRequest ("/selectCharacter", Method.GET);
			selectCharacterRequest.AddQueryParameter ("characterId", character.Id);

			var response = Client.Execute (selectCharacterRequest);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Client.BaseUrl + "/servlet/chatlisten" );
			request.UserAgent = "trion/mobile";
			request.ProtocolVersion = HttpVersion.Version11;
			request.Method = WebRequestMethods.Http.Get;
			request.KeepAlive = true;
			request.Timeout = int.MaxValue;

			request.CookieContainer = new CookieContainer ();
			request.CookieContainer.Add (new Cookie ("SESSIONID", session.Id, "", Client.BaseUrl.Host));

			stream = request.GetResponse().GetResponseStream();

			var encode = System.Text.Encoding.GetEncoding ("utf-8");

			var readStream = new StreamReader (stream, encode);

			var buffer = new char[4096];
			var bytesRead = readStream.Read (buffer, 0, buffer.Length);

			while (bytesRead > 0) {
				System.Diagnostics.Debug.WriteLine ("Message Received");

				// Convert the contents of the buffer into a string
				var stringified = new String (buffer, 0, buffer.Length);

				// Parse the response
				var parsedResponse = parser.Parse (stringified);

				// Handle the message type
				if (parsedResponse is LoginLogoutData) {
					HandleLoginLogout (parsedResponse as LoginLogoutData);
				} else if (parsedResponse is ChatData) {
					HandleMessage (parsedResponse as ChatData);
				}

				// Message received.  Clear the buffer
				Array.Clear (buffer, 0, buffer.Length);

				// Wait for the next message
				bytesRead = readStream.Read (buffer, 0, buffer.Length);
			}
		}

		public void Stop()
		{
			if (this.stream != null) {
				this.stream.Dispose ();
				this.stream = null;
			}
		}

		protected override RestRequest CreateRequest( string url, Method method = Method.GET)
		{
			var request = base.CreateRequest (url, method);

			request.AddCookie ("SESSIONID", session.Id);

			return request;
		}

		private rift.net.Models.Action HandleLoginLogout( LoginLogoutData data )
		{
			var action = new rift.net.Models.Action ();

			var characterId = data.characterId.ToString ();

			action.State = data.login ? StateAction.Login : StateAction.Logout;
			action.InGame = data.game;
			action.Character = guildMates.FirstOrDefault (x => x.Id == characterId) ?? friends.FirstOrDefault (x => x.Id == characterId);

			return action;
		}

		private Message HandleMessage( ChatData data )
		{
			var message = Mapper.Map<ChatData, Message> (data);

			return message;
		}
	}
}

