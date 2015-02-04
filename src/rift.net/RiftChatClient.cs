using System;
using rift.net.Models;
using RestSharp;
using System.Net;
using System.IO;
using AutoMapper;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace rift.net
{
	public class RiftChatClient : RiftRestClientSecured
	{
		private readonly Character character;
		private List<Contact> guildMates = new List<Contact>();
		private List<Contact> friends = new List<Contact>();
		private Stream stream;

	    private event EventHandler Disconnected;

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

		public void Listen()
		{
			var parser = new ChatMessageParser ();

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Client.BaseUrl + "/servlet/chatlisten" );
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

		        // Open up the response stream
		        stream = request.GetResponse().GetResponseStream();

		        // Create a stream reader and...
		        var readStream = new StreamReader(stream, Encoding.GetEncoding("utf-8"));

		        // Read it into a buffer
		        var bytesRead = readStream.Read(buffer, 0, buffer.Length);

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
		        Debug.WriteLine(exception.ToString());
		    }
		    finally
		    {
                // If we've reached this point, then we've timed out or had an exception.  
                //  Go ahead and reconnect
                if (Disconnected != null)
                    Disconnected(this, new EventArgs());
		    }
		}

		public void Stop()
		{
		    if (Disconnected != null)
		        Disconnected -= OnDisconnected;

		    if (stream == null) return;

            Debug.WriteLine("All done listening.  Have a nice day!");

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

	    protected override RestRequest CreateRequest( string url, Method method = Method.GET)
		{
			var request = base.CreateRequest (url, method);

			request.AddCookie ("SESSIONID", session.Id);

			return request;
		}

		private rift.net.Models.Action HandleLoginLogout( LoginLogoutData data )
		{
			var action = new Models.Action ();

			var characterId = data.characterId.ToString ();

			action.State = data.login ? StateAction.Login : StateAction.Logout;
			action.InGame = data.game;
			action.Character = guildMates.FirstOrDefault (x => x.Id == characterId) ?? friends.FirstOrDefault (x => x.Id == characterId);

		    Debug.WriteLine("{0}\t{1}{2} has just {3}", DateTime.Now.ToShortTimeString(),
		        action.Character.FullName,
		        action.Character.Guild != null ? string.Format(" <{0}>", action.Character.Guild.Name) : "",
		        action.State == StateAction.Login ? "logged in" : "logged out");

			return action;
		}

		private Message HandleMessage( ChatData data )
		{
			var message = Mapper.Map<ChatData, Message> (data);

		    Debug.WriteLine("{0}\t{1}: {2}", DateTime.Now.ToShortTimeString(), message.Sender.Name, message.Text);

			return message;
		}
	}
}

