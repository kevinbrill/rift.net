using System;
using System.Collections.Generic;
using AutoMapper;
using RestSharp;
using rift.net.rest;
using rift.net.Models;
using rift.net.Models.Guilds;

namespace rift.net.Models.Games
{
	public class Card
	{
		public string Background {
			get;
			set;
		}

		public int Cost {
			get;
			set;
		}

		public string Instructions {
			get;
			set;
		}

		public string Mask {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public string Preview {
			get;
			set;
		}

		public string Url {
			get;
			set;
		}
	}

}