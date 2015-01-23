using System;
using rift.net.Models;
using RestSharp;
using System.Net;
using System.IO;

namespace rift.net
{
	public class RiftChatClient : RiftRestClient
	{
		private Session session;
		private string characterId;

		public RiftChatClient (Session session, string characterId)
		{
			this.session = session;
			this.characterId = characterId;
		}

		public void Start()
		{
			var selectCharacterRequest = CreateRequest ("/selectCharacter", Method.GET);
			selectCharacterRequest.AddQueryParameter ("characterId", characterId);

			var response = Client.Execute (selectCharacterRequest);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Client.BaseUrl + "/servlet/chatlisten" );
			request.UserAgent = "trion/mobile";
			request.ProtocolVersion = HttpVersion.Version11;
			request.Method = WebRequestMethods.Http.Get;
			request.KeepAlive = true;
			request.Timeout = int.MaxValue;

			request.CookieContainer = new CookieContainer ();
			request.CookieContainer.Add (new Cookie ("SESSIONID", session.Id, "", Client.BaseUrl.Host));

			var stream = request.GetResponse().GetResponseStream();

			var encode = System.Text.Encoding.GetEncoding ("utf-8");

			var readStream = new StreamReader (stream, encode);

			var buffer = new char[4096];
			var bytesRead = readStream.Read (buffer, 0, buffer.Length);

			while (bytesRead > 0) {
				var stringified = new String (buffer, 0, buffer.Length);

				System.Diagnostics.Debug.WriteLine (stringified);

				bytesRead = readStream.Read (buffer, 0, buffer.Length);
			}
		}

		public void Stop()
		{
		}

		protected override RestRequest CreateRequest( string url, Method method = Method.GET)
		{
			var request = base.CreateRequest (url, method);

			request.AddCookie ("SESSIONID", session.Id);

			return request;
		}
	}
}

