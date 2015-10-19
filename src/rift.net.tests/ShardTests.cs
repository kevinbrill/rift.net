using System;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace rift.net.tests
{
	[TestFixture()]
	public class ShardTests
	{
		private RiftClient client;

		[TestFixtureSetUp()]
		public void SetUp()
		{
			client = new RiftClient ();
		}

		[Test()]
		public void Verify_That_Shard_List_Returns_Shards()
		{
			var shards = client.ListShards ();

			Assert.That (shards, Is.Not.Null.And.Not.Empty);
		}

		[Test()]
		public async Task Verify_That_Shard_List_Returns_Shards_Async()
		{
			var shards = await client.ListShardsAsync();

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

