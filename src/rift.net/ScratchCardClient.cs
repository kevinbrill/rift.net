using System;
using rift.net.Models;
using AutoMapper;
using System.Collections.Generic;

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

		public ScratchCard GetAccountScratchCardSummary()
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
	}
}