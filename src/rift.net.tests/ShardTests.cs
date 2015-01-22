﻿using System;
using NUnit.Framework;
using System.Linq;

namespace rift.net.tests
{
	[TestFixture()]
	public class ShardTests
	{
		private RiftClient client;

		[TestFixtureSetUp()]
		public void SetUp()
		{
			var username = System.Configuration.ConfigurationManager.AppSettings ["username"];
			var password = System.Configuration.ConfigurationManager.AppSettings ["password"];

			var sessionFactory = new SessionFactory ();

			var session = sessionFactory.Login (username, password);

			client = new RiftClient (session);
		}

		[Test()]
		public void Verify_That_Shard_List_Returns_Shards()
		{
			var shards = client.ListShards ();

			Assert.That (shards, Is.Not.Null.And.Not.Empty);
		}

		[Test()]
		public void Verify_That_Wolfsbane_Shard_Id_Has_Not_Changed()
		{
			var shards = client.ListShards ();

			var wolfsbane = shards.FirstOrDefault (x => x.Id == 1706);

			Assert.That (wolfsbane, Is.Not.Null);
			Assert.That (wolfsbane.Name, Is.EqualTo ("Wolfsbane"));
		}
	}
}
