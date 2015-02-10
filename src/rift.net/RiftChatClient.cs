using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using AutoMapper;
using log4net;
using rift.net.Models;
using rift.net.rest.Chat;
using RestSharp;
using Action = rift.net.Models.Action;

namespace rift.net
{
	public class RiftChatClient : RiftRestClientSecured
	{
	    private static readonly ILog logger = LogManager.GetLogger(typeof (RiftChatClient));

		private readonly Character character;
		private List<Contact> guildMates = new List<Contact>();
		private List<Contact> friends = new List<Contact>();
		private Stream stream;
		private Thread thread;

	    private event EventHandler Disconnected;

		public event EventHandler Connecting;
		public event EventHandler Connected;

	    public event EventHandler<Message> GuildChatReceived;
	    public event EventHandler<Message> WhisperReceived;
	    public event EventHandler<Message> OfficerChatReceived;
	    public event EventHandler<Action> Login;
	    public event EventHandler<Action> Logout;

		static RiftChatClient()
		{
			Mapper.CreateMap<ChatData, Message> ()
				.ForMember (x => x.Id, y => y.MapFrom (src => src.messageId))
				.ForMember (x => x.Sender, y => y.MapFrom (src => new Sender{ Id = src.senderId, Name = src.senderName }))
				.ForMember (x => x.Text, y => y.MapFrom (src => src.message))
				.ForMember (x => x.ReceiveDateTime, y => y.MapFrom (src => new DateTime (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds (src.messageTime).ToLocalTime ()));
		}

		public RiftChatClient (Session session, Character character) : base(session)
		{
			this.character = character;
		}

	    public bool Connect()
	    {
            var securedClient = new RiftClientSecured(session);

            // Go online
	        securedClient.GoOnline(character.Id);

            // Get the character's friends
            friends = securedClient.ListFriends(character.Id);

            // If the character is valid, then grab the guild mates
            if (character.Guild != null)
            {
                guildMates = securedClient.ListGuildmates(character.Guild.Id);
            }

	        return true;
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

		public bool SendGuildMessage( string message )
		{
			var request = CreateRequest ("/guild/addChat", Method.GET );
			request.AddQueryParameter ("characterId", character.Id);
			request.AddQueryParameter ("message", message);

			var response = Client.Execute (request);

			return (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.OK);
		}

		public bool SendOfficerMessage( string message )
		{
			var request = CreateRequest ("/guild/addChat", Method.GET );
			request.AddQueryParameter ("characterId", character.Id);
			request.AddQueryParameter ("officer", "true");
			request.AddQueryParameter ("message", message);

			var response = Client.Execute (request);

			return (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.OK);
		}

		public bool SendWhisper( Character recipient, string message )
		{
			var request = CreateRequest ("/chat/whisper", Method.GET);

			request.AddQueryParameter ("senderId", character.Id);
			request.AddQueryParameter ("recipientId", recipient.Id);
			request.AddQueryParameter ("message", message);

			var response = Client.Execute (request);

			return (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.OK);
		}

		public void Listen()
		{
			Stop ();

			thread = new Thread (ListenToChatStream);

			thread.Start ();		
		}

		public void Stop()
		{
		    if (Disconnected != null)
		        Disconnected -= OnDisconnected;

		    if (stream == null) 
                return;

			stream.Close ();
		    stream.Dispose ();

		    stream = null;
		}

	    private void OnDisconnected(object sender, EventArgs eventArgs)
	    {
            // Stop the stream
            Stop();

            // Start the stream
            Listen();
        }

		private void HandleLoginLogout(LoginLogoutData data)
		{
			var action = new Action ();

			var characterId = data.characterId.ToString ();

			action.State = data.login ? StateAction.Login : StateAction.Logout;
			action.InGame = data.game;
			action.Character = guildMates.FirstOrDefault (x => x.Id == characterId) ?? friends.FirstOrDefault (x => x.Id == characterId);

		    logger.DebugFormat("{0}\t{1}{2} has just {3}", DateTime.Now.ToShortTimeString(),
		        action.Character.FullName,
		        action.Character.Guild != null ? string.Format(" <{0}>", action.Character.Guild.Name) : "",
		        action.State == StateAction.Login ? "logged in" : "logged out");

		    switch (action.State)
		    {
		        case StateAction.Login:
		            if (Login != null)
		                Login(this, action);
		            break;
		        case StateAction.Logout:
		            if (Logout != null)
		                Logout(this, action);
		            break;
		    }
		}

		private void HandleMessage(ChatData data)
		{
			var message = Mapper.Map<ChatData, Message> (data);

		    switch (data.ChatChannel)
		    {
		        case ChatChannel.Guild:
		            if (GuildChatReceived != null)
		                GuildChatReceived(this, message);
                    break;
                case ChatChannel.Officer:
                    if (OfficerChatReceived != null)
                        OfficerChatReceived(this, message);
                    break;
                case ChatChannel.Whisper:
                    if (WhisperReceived != null)
                        WhisperReceived(this, message);
                    break;
            }

            logger.DebugFormat("{0}\t{1}: {2}", DateTime.Now.ToShortTimeString(), message.Sender.Name, message.Text);
		}

		private void ListenToChatStream()
		{
			var parser = new ChatMessageParser ();

			var request = (HttpWebRequest)WebRequest.Create(Client.BaseUrl + "/servlet/chatlisten" );
			request.UserAgent = "trion/mobile";
			request.ProtocolVersion = HttpVersion.Version11;
			request.Method = WebRequestMethods.Http.Get;
			request.KeepAlive = true;

			request.CookieContainer = new CookieContainer ();
			request.CookieContainer.Add (new Cookie ("SESSIONID", session.Id, "", Client.BaseUrl.Host));

			try
			{
				var buffer = new char[4096];

				// Handle a forceful disconnect
				Disconnected += OnDisconnected;

				// Notify that we're connecting
				if( Connecting != null )
					Connecting( this, new EventArgs() );

				// Open up the response stream
				stream = request.GetResponse().GetResponseStream();

				// Create a stream reader and...
				var readStream = new StreamReader(stream, Encoding.GetEncoding("utf-8"));

				// Read it into a buffer
				var bytesRead = readStream.Read(buffer, 0, buffer.Length);

				// Notify that we've connected
				if( Connected != null )
					Connected(this, new EventArgs());

				while (bytesRead > 0)
				{
					// Convert the contents of the buffer into a string
					var stringified = new String(buffer, 0, buffer.Length);

					// Parse the response
					var parsedResponse = parser.Parse(stringified);

					// Handle the message type
					if (parsedResponse is LoginLogoutData)
					{
						HandleLoginLogout(parsedResponse as LoginLogoutData);
					}
					else if (parsedResponse is ChatData)
					{
						HandleMessage(parsedResponse as ChatData);
					}

					// Message received.  Clear the buffer
					Array.Clear(buffer, 0, buffer.Length);

					// Wait for the next message
					bytesRead = readStream.Read(buffer, 0, buffer.Length);
				}
			}
			catch (Exception exception)
			{
                logger.Error("An unknown exception occurred", exception);

			    throw;
			}
			finally
			{
				// If we've reached this point, then we've timed out or had an exception.  
				//  Go ahead and reconnect
				if (Disconnected != null)
					Disconnected(this, new EventArgs());
			}		
		}
	}
}