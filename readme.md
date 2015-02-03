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

###Not Yet Implemented

The following features, while on the roadmap, are not yet implemented:

* Changing a characters web/mobile presence between offline and online
* Chat of any kind, including guild, friend and whispers

###Install
Forthcoming once the package is released out to nuget.

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

###Contributing
If there's a feature that you'd like to see, you can create an issue.  If there's something that you've fixed or improved, then create a pull request.  Pull requests are great!

You can reach me in game at **Bruun@Wolfsbane**.  If you're looking for a home in Telara, check out **Grievance** at [http://grievancegaming.org/](http://grievancegaming.org/)

##Legal Stuff
I am in no way affiliated or employed by Trion Worlds, nor do I receive any compensation from them for my work on this project or any other project.

I am also not responsible for any damages that are incurred by using this library.  Any usage should be done at your own risk.