/*
 * Created by SharpDevelop.
 * User: Kevin
 * Date: 1/25/2015
 * Time: 1:27 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using rift.net.games.models;

namespace rift.net.games
{
	/// <summary>
	/// Description of ParseResults.
	/// </summary>
	public class ParseResults
	{
		public bool IsWinner { get; set; }

		public bool IsReplay { get; set; }

		public string FollowUpUrl { get; set; }
		
		public List<Prize> Prizes { get; set; }
	}
}
