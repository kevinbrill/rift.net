using System;
using System.Collections.Generic;
using System.Linq;
using CsQuery;
using rift.net.Models.Games;

namespace rift.net
{
	public class ScratchResultParser
	{
		public CQ CDocument {
			get;
			private set;
		}

		public ParseResults Parse(string content)
		{
			CDocument = CQ.CreateDocument (content);

			var rewardDiv = CDocument.Select ("div#reward-layer span.reward-text");

			var rewardText = rewardDiv.FirstOrDefault ();

			// If no reward text handle here

			// Check to see if we're a loser...
			if (rewardText.InnerText == "Sorry, you did not win. Please try again later.") {
				return new ParseResults { IsWinner = false, IsReplay = false };
			}
				
			// We're a winner, now go looking for prize winnings
			var prizeDivs = CDocument.Select ("div.winning-card-prize");
			var redeemUrl = CDocument.Select ("div#reward-layer div.debug a");
			var replayDiv = CDocument.Select ("div#reward-layer a");
			
			if ((prizeDivs != null) && (prizeDivs.Any ())) {
				return HandleWinner (prizeDivs, redeemUrl);
			} else if ((replayDiv != null) && (replayDiv.Any ())) {
				return HandleReplay (replayDiv);
			}
			
			return null;
		}

		private ParseResults HandleWinner(CQ prizeDivs, CQ redeemAnchor)
		{
			var results = new ParseResults();
			
			results.IsWinner = true;
			results.IsReplay = false;
			results.Prizes = prizeDivs.Select (x => CreatePrizeFromDom (x)).ToList ();
			results.FollowUpUrl = redeemAnchor.Attr ("href");

			return results;
		}

		private ParseResults HandleReplay(CQ replayDiv)
		{
			var results = new ParseResults();
			
			results.IsWinner = false;
			results.IsReplay = true;
			
			results.FollowUpUrl = replayDiv.Attr("href");
			
			return results;
		}

		private Prize CreatePrizeFromDom(IDomObject prizeDom)
		{
			var prize = new Prize ();
			
			var dom = prizeDom.Cq ().Select ("span.prize-name").FirstOrDefault ();
			 	
			if (dom == null) {
				throw new Exception ("Unable to find prize element 'span.prize-name'");
			}
			
			prize.Name = dom.InnerText;

			dom = prizeDom.Cq ().Select ("div.multiplier-text").FirstOrDefault ();
			
			if (dom == null) {
				throw new Exception ("Unable to find prize element 'span.prize-name'");
			}
			
			prize.Quantity = int.Parse (dom.InnerText);
			
			dom = prizeDom.Cq ().Select ("img.icon").FirstOrDefault ();
			
			prize.ImageUrl = dom.GetAttribute ("src");
			
			return prize;
		}
	}
}

