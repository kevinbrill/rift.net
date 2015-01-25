using System;
using System.Linq;
using rift.net.Models;
using AutoMapper;
using System.Collections.Generic;
using rift.net.Models.Games;
using RestSharp;
using rift.net.rest;

namespace rift.net
{
	public class ScratchCardClient : RiftClientSecured
	{
		private ScratchResultParser parser = new ScratchResultParser ();

		static ScratchCardClient ()
		{
			Mapper.CreateMap<ScratchCardData, Card> ();
			Mapper.CreateMap<AccountScratchCardData, ScratchCard> ()
				.ForMember (x => x.MaximumPoints, y => y.MapFrom (src => src.maxPoints));
		}

		public ScratchCardClient (Session session) : base(session)
		{
		}

		public virtual ScratchCard GetAccountScratchCardSummary()
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

		public List<Prize> Play( Card card, string characterId )
		{
			List<Prize> prizes = new List<Prize> ();
			var accountStatus = GetAccountScratchCardSummary ();

			if (accountStatus.AvailablePoints <= 0)
				throw new NoCardsAvailableException (accountStatus.SecondsUntilNextPoint);

			var selectedGame = accountStatus.Cards.FirstOrDefault (x => x.Url == card.Url);

			if (selectedGame == null)
				throw new InvalidGameException (card);

			// Create a new request based on the game's URL.  The base URL already includes
			//  /chatservice, so go ahead and remove it.
			var request = CreateRequest (selectedGame.Url.Replace ("/chatservice", "/"));
			request.AddQueryParameter ("characterId", characterId);

			var response = Client.Execute (request);

			// Winner?
			//return '/chatservice/scratch/redeem?game=02a07cd8-f08b-4931-9488-3b44370ad2b3';
			// Calling the above route returns Content of:
			// "{\"status\":\"success\",\"data\":\"ok\"}"

			if ((response.ResponseStatus != RestSharp.ResponseStatus.Completed) ||
			   (response.StatusCode != System.Net.HttpStatusCode.OK)) {
				throw new Exception ("An error occurred calling the service", response.ErrorException);
			}

			// Parse the results
			var results = parser.Parse(response.Content);

			if (results.IsWinner) {
				// Claim Prize
				prizes = ClaimPrize (results);
			} else if (results.IsReplay) {
				// Replay
				prizes = Replay (results);
			}

			return prizes;
		}

		private List<Prize> ClaimPrize( ParseResults results )
		{
			var request = CreateRequest (results.FollowUpUrl, Method.GET);

			var response = Client.Execute (request);

			// Check response and handle errors

			// If the call was successful, then return the prizes
			return results.Prizes;
		}

		private List<Prize> Replay ( ParseResults results )
		{
			var prizes = new List<Prize> ();

			var request = CreateRequest (results.FollowUpUrl, Method.GET);

			var response = Client.Execute (request);

			// Parse the results
			var replayResults = parser.Parse(response.Content);

			if (replayResults.IsWinner) {
				// Claim Prize
				prizes = ClaimPrize (replayResults);
			} else if (replayResults.IsReplay) {
				// Replay
				prizes = Replay (replayResults);
			}

			return prizes;
		}
	}
}