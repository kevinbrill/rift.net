﻿using System;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace rift.net.tests
{
	[TestFixture()]
	public class ZoneTests
	{
		private RiftClient client;

		[TestFixtureSetUp()]
		public void SetUp()
		{
			client = new RiftClient ();
		}

		[Test()]
		public void Verify_That_Wolfsbane_Has_Zones()
		{
			var zones = client.ListZones (1706);

			Assert.That (zones, Is.Not.Null.Or.Not.Empty);		
		}

		[Test()]
		public async Task Verify_That_Wolfsbane_Has_Zones_Async()
		{
			var zones = await client.ListZonesAsync(1706);

			Assert.That (zones, Is.Not.Null.Or.Not.Empty);		
		}

		[Test()]
		public void Verify_That_A_Zone_Event_Has_A_Valid_Name()
		{
			var zones = client.ListZones (1706);

			Assert.That (zones, Is.Not.Null.Or.Not.Empty);

			var zone = zones.FirstOrDefault(z => z.Event != null);

			Assume.That ( zone, Is.Not.Null, "No zone events are currently active");

			Assert.That (zone.Event.Name, Is.Not.Null.Or.Not.Empty);
		}

		[Test()]
		public void Verify_That_A_Zone_Event_Has_Started_Within_The_Last_Eight_Hours()
		{
			var zones = client.ListZones (1706);

			Assert.That (zones, Is.Not.Null.Or.Not.Empty);

			var zone = zones.FirstOrDefault(z => z.Event != null);

			Assume.That ( zone, Is.Not.Null, "No zone events are currently active");

			Assert.That( zone.Event.ActiveSince, Is.AtLeast( DateTime.Now.AddHours(-8)));
		}
	}
}