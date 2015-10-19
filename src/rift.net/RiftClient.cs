using System;
using RestSharp;
using rift.net.rest;
using System.Collections.Generic;
using rift.net.Models;
using rift.net.Models.Guilds;
using System.Threading.Tasks;

namespace rift.net
{
	public class RiftClient : RiftRestClient
	{
		static RiftClient()
		{			
		}

		public List<Shard> ListShards()
		{
			var request = CreateRequest ("/shard/list");

			var shardData = Execute<List<ShardData>> (request);

			return MapShardData (shardData);
		}

		public async Task<List<Shard>> ListShardsAsync()
		{
			var request = CreateRequest ("/shard/list");

			var shardData = await ExecuteAsync<List<ShardData>> (request);

			return MapShardData (shardData);
		}

		public List<Zone> ListZones(int shardId)
		{
			var request = CreateRequest ("zoneevent/list");
			request.AddQueryParameter ("shardId", shardId.ToString ());

			var zoneData = Execute<List<ZoneData>> (request);

			return MapZoneData (zoneData);
		}

		public async Task<List<Zone>> ListZonesAsync(int shardId)
		{
			var request = CreateRequest ("zoneevent/list");
			request.AddQueryParameter ("shardId", shardId.ToString ());

			var zoneData = await ExecuteAsync<List<ZoneData>> (request);

			return MapZoneData (zoneData);
		}

		private List<Shard> MapShardData( List<ShardData> shardData )
		{
			var results = new List<Shard> (shardData.Count);

			foreach( var data in shardData ) {
				results.Add (new Shard {
					Id = data.shardId,
					Name = data.name
				});
			}

			return results;
		}

		private List<Zone> MapZoneData( List<ZoneData> zoneData )
		{
			var results = new List<Zone> (zoneData.Count);

			foreach( var data in zoneData )
			{
				var zone = new Zone {
					Id = data.zoneId,
					Name = data.zone,
				};

				if( data.name != null ) {
					zone.Event = new ZoneEvent {
						Name = data.name,
						ActiveSince = new DateTime (1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds (data.started).ToLocalTime ()
					};
				}

				results.Add (zone);

			}

			return results;
		}
	}
}

