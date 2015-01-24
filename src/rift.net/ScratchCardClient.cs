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

		public void Play( Card card, string characterId )
		{
			var accountStatus = GetAccountScratchCardSummary ();

			if (accountStatus.AvailablePoints <= 0)
				throw new NoCardsAvailableException (accountStatus.SecondsUntilNextPoint);

			var selectedGame = accountStatus.Cards.FirstOrDefault (x => x.Url == card.Url);

			if (selectedGame == null)
				throw new InvalidGameException (card);

			// Create a new request based on the game's URL.  The base URL already includes
			//  /chatservice, so go ahead and remove it.
			var request = CreateRequest(selectedGame.Url.Replace ("/chatservice", "/"));
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

			if (IsGameAWinner (response.Content)) {
				ClaimPrize (response.Content);
			} else if (IsGameAReplay (response.Content)) {
				Replay (response.Content);
			}
		}

		private bool IsGameAWinner(string content)
		{
			var indexOfRedeem = content.IndexOf ("return '/chatservice/scratch/redeem?game=");

			return indexOfRedeem >= 0;
		}

		private bool IsGameAReplay(string content)
		{
			var indexOfReplay = content.IndexOf ("replayUUID");

			return indexOfReplay >= 0;
		}

		private Uri GetWinningUrl( string content )
		{
			var indexOfRedeem = content.IndexOf ("/chatservice/scratch/redeem?game=");
			var indexOfSemi = content.IndexOf (";", indexOfRedeem);

			var uri = new Uri (content.Substring (indexOfRedeem, indexOfSemi - indexOfRedeem - 1));

			return uri;
		}

		private Uri GetReplayUrl( string content )
		{
			var indexOfReplay = content.IndexOf ("/chatservice/scratch/");
			var indexOfSemi = content.IndexOf (";", indexOfReplay);

			var uri = new Uri (content.Substring (indexOfReplay, indexOfSemi - indexOfReplay - 1));

			return uri;
		}	

		private void ClaimPrize( string content )
		{
			var url = GetWinningUrl (content);

			var request = CreateRequest (url.ToString(), Method.GET);

			var response = Client.Execute(request);

			// Check response and handle errors
		}

		private void Replay (string content)
		{
			var url = GetReplayUrl (content);

			var request = CreateRequest (url.ToString(), Method.GET);

			var response = Client.Execute(request);

			// Check response an handle errors

			if (IsGameAWinner (response.Content)) {
				ClaimPrize (response.Content);
			} else if (IsGameAReplay (response.Content)) {
				Replay (response.Content);
			}
		}
	}
}