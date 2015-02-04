using System;
using RestSharp;

namespace rift.net
{
	public class ChatMessageParser
	{
		public ChatMessageParser ()
		{
		}

		public object Parse( string jsonString )
		{
			object parsedObject = null;

			//System.Diagnostics.Debug.WriteLine (jsonString);

			// Parse the contents into JSON
			dynamic json = SimpleJson.DeserializeObject( jsonString );			

			// Get the type
			string type = json["type"];

			// Get the value
			JsonObject value = json["value"];

			switch (type) {
			case "Begin":
				parsedObject = ParseBeginMessage (value);
				break;
			case "Login":
				parsedObject = ParseLoginLogOutMessage (value, true);
				break;
			case "Logout":
				parsedObject = ParseLoginLogOutMessage (value, false);
				break;
			case "GuildChat":
            case "WhisperChat":
				parsedObject = ParseChat (value);
				break;                
			}

			return parsedObject;
		}

		private BeginData ParseBeginMessage(JsonObject jsonObject)
		{
			return SimpleJson.DeserializeObject<BeginData> (jsonObject.ToString());
		}

		private LoginLogoutData ParseLoginLogOutMessage( JsonObject jsonObject, bool isLoggingIn )
		{
			var data = SimpleJson.DeserializeObject<LoginLogoutData> (jsonObject.ToString());

			data.login = isLoggingIn;

			return data;
		}

		private ChatData ParseChat( JsonObject jsonObject )
		{
			return SimpleJson.DeserializeObject<ChatData> (jsonObject.ToString());
		}
	}
}

