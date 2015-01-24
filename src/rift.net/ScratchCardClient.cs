using System;
using System.Linq;
using rift.net.Models;
using AutoMapper;
using System.Collections.Generic;
using rift.net.Models.Games;

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
			var request = CreateRequest (selectedGame.Url.Replace ("/chatservice", "/"));
			request.AddQueryParameter ("characterId", characterId);

			//var response = Client.Execute (request);
		}
	}
}