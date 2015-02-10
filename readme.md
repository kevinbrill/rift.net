##Rift.Net

###Thanks

Before I get started, I like to send out big thanks to the following for inspiration and snippets of code:

* **zachhowe** ([https://github.com/zachhowe/rift-ruby]())
* **Archaic Binary** ([https://www.archaicbinary.net/general/automatic-rift-scratch-off.html]())

###About

Rift.net is a managed wrapper around [Trion World's](http://www.trionworlds.com) MMO [Rift](http://www.riftgame.com).  Its goal is to encapsulate all of the functionality provided via the public chat APIs into a structured assembly that is consumable from .net.

###Features

Currently, the following features of the API are supported:

#####Unauthenticated
* Viewing zone information and zone events
* Viewing a list of shards

#####Authenticated
* Viewing a list of characters and their details for an account
* Viewing a list of friends for a particular character
* Viewing the guild membership and information for a particular character
* Viewing a list of all scratch card games that can be played
* Viewing an account's game status, including the number of games that can currently be played
* Play any of the scratch card games
* Mark a character as online or offline
* Connect and talk in guild chat
* Connect and talk in officer chat
* Send whispers to other players

###Limitations
Due to limitations in the underlying REST API, you currently cannot receive whispers while not logged into the game.  This means that you most likely will not be able to use this to communicate with people not in your guild.

###Install
From the package manager console, type in

    Install-Package rift.net

###Using
The usage pattern varies slightly, depending on if you're interested in using general, non-secured data or if you'd like account and character data.

#####Unsecured

	// Create a new unsecured rift client
	var client = new RiftClient();
	
	// List all shards
	var shards = client.ListShards();
	
	// Find Wolfsbane
	var wolfsbane = shards.FirstOrDefault( x=>x.Name == "Wolfsbane" );
	
	// List all the zones on Wolfsbane
	var zones = client.ListZones(wolfsbane.Id);

#####Secured
Making calls against the secured client has an extra step in the process, since all calls need to have a SESSIONID cookie set when performing the calls.  Rift.net handles ensuring that the cookie is set, as long as you authenticate and provide a session to the RiftClientSecured.

	// Create a new session factory
	var sessionFactory = new SessionFactory ();

	// Login using provided username and password
	var session = sessionFactory.Login (username, password);
	
	// Create a new secured client 
	var client = new RiftClientSecured (session);

Once a secured client has been created, you can interact with that client and perform account and character sensitive API calls.

	// List all characters on the account
	var chars = client.ListCharacters();
	
	// Which characters aren't in a guild?
	var guildless = chars.Where(x=>x.Guild == null);
	
	// Get the first character in a guild
	var guildedChar = chars.FirstOrDefault( x=>x.Guild != null );
	
	// Get the guild information for that character
	var guildInfo = client.GetGuildInfo( guildedChar.Guild.Id );
	
	// List that guild's members
	var guildies = client.ListGuildmates( guildInfo.Id );
	
	// Show the last 5 wall posts
	var posts = guildInfo.Wall.OrderByDescending( x=>x.PostDate ).Take(5);

#####Scratch Games
The mobile scratch games are also surfaced through the Rift API, and have been wrapped by Rift.net as well.  There are two main components to the games:

* The account information, including the maximum number of points available, the current number of points available, and the time to the next game
* A list of games that are available

The games are wrapped in the ScratchCardClient class, which is constructed in the same fashion as the RiftSecuredClient.
	
	// Create a new session factory
	var sessionFactory = new SessionFactory ();

	// Login using provided username and password
	var session = sessionFactory.Login (username, password);
	
	// Create a new secured client 
	var client = new ScratchGameClient (session);
	
	// Get the user's game information
	var info = client.GetAccountGameInfo();
	
	Debug.WriteLine( info.MaximumPoints );
	Debug.WriteLine( info.AvailablePoints );
	Debug.WriteLine( info.SecondsUntilNextPoint );
	Debug.WriteLine( info.Games.Count );
	
	// List all games
	var games = client.ListGames();
	
	foreach( var game in games ) 
	{
		Debug.WriteLine( game.Name );
		Debug.WriteLine( game.Description );
	}
	
Seeing the available games is fun, but playing them is even better.  Once you have a secured client, you can play games this way:

	// Who doesn't love Shinies?
	var shinies = games.FirstOrDefault( x => x.Name == "Shinies" );
	
	try
	{
		// Would you like to play a game?
		var prizes = client.Play( shinies, charactedId );
		
		if( prizes.Count == 0 ) 
		{
			Debug.WriteLine("Sorry you didn't win");
		}
		else
		{
			foreach( var prize in prizes )
			{
				Debug.WriteLine( string.Format("You won {0} copies of {1}!", prize.Quantity, prize.Name);
			}
		}
	}
	catch( NoCardsAvailableException ex )
	{
		Debug.WriteLine( "No cards are available" );
	}
	catch( InvalidGameException ex )
	{
		Debug.WriteLine( "That's an invalid game" );
	}
	
The actual playing of a game and providing rewards is all done by Trion on their servers, so this wrapper doesn't allow you to cheat or game the system (which was never the intent anyways).

#####Chat
The core of connecting to the chat infrastructure is encapsulated in the *RiftChatClient*.  The client manages connecting to the server, and communicating all messages received via custom events.

To instantiate a new instance of the chat client, you'll need to provide a session, and an instance of the character to whom this client will apply.

	// Create a new session factory
	var sessionFactory = new SessionFactory ();

	// Login using provided username and password
	var session = sessionFactory.Login (username, password);

	// Create a new secured client 
	var client = new RiftClientSecured (session);
	
	// Hello handsome
	var bruun = client.ListCharacters().FirstOrDefault( x=>x.FullName == "Bruun@Wolfsbane" );
	
	// Create a chat client
	var chatClient = new RiftChatClient( session, bruun );
	
Once, you get a chat client, the next step is to connect, which will mark your character online, and pull a list of friends and guildies.

    // Connect
    chatClient.Connect();
    
    // Hook into the Connecting and Connected events
	chatClient.Connecting += (object sender, EventArgs e) => {
		Debug.WriteLine( "Connecting to chat server..." );
	};

	chatClient.Connected += (object sender, EventArgs e) => {
		Debug.WriteLine( "Connected!" );
	};

Next, before we start listening for events, let's wire up to receive the events.

	chatClient.GuildChatReceived += (object sender, Message e) => {
		Debug.WriteLine(string.Format("{0}: {1}", e.Sender.Name, e.Text));
	};

	chatClient.WhisperReceived += (object sender, Message e) => {
        Debug.WriteLine(string.Format("{0}: {1}", e.Sender.Name, e.Text));
	};

	chatClient.OfficerChatReceived += (object sender, Message e) => {
        Debug.WriteLine(string.Format("{0}: {1}", e.Sender.Name, e.Text));
	};

	chatClient.Login += (object sender, rift.net.Models.Action e) => {
		Debug.WriteLine(string.Format("{0} has come online.", e.Character.Name));
	};

	chatClient.Logout += (object sender, rift.net.Models.Action e) => {
		Debug.WriteLine(string.Format("{0} has gone offline.", e.Character.Name));
	};
	
Next, start listening.

    // Listen
    chatClient.Listen();
    
This call spins up a background thread that will connect to the Rift chat server, and listen for incoming messages.  These messages are then relayed back via events.

To talk in guild chat, use the chat client and call the **SendGuildMessage** method:

    // Say hello
    chatClient.SendGuildMessage( "Hello everyone.  Hope you're doing fabulous!" );
    
Once you're finished with the chat client, the proper way to dispose is to call the **Stop** method.  This will disconnect from the chat server, and take that character offline.

    // All done!
    chatClient.Stop();

###Contributing
If there's a feature that you'd like to see, you can create an issue.  If there's something that you've fixed or improved, then create a pull request.  Pull requests are great!

You can reach me in game at **Bruun@Wolfsbane**.  If you're looking for a home in Telara, check out **Grievance** at [http://grievancegaming.org/](http://grievancegaming.org/)

##Legal Stuff
I am in no way affiliated or employed by Trion Worlds, nor do I receive any compensation from them for my work on this project or any other project.

I am also not responsible for any damages that are incurred by using this library.  Any usage should be done at your own risk.