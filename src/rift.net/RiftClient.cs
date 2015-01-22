using System;
using RestSharp;
using AutoMapper;
using rift.net.rest;
using System.Collections.Generic;
using rift.net.Models;
using rift.net.Models.Guilds;

namespace rift.net
{
	public class RiftClient : RiftRestClient
	{
		static RiftClient()
		{
			Mapper.CreateMap<ShardData, Shard> ()
				.ForMember (x => x.Id, y => y.MapFrom (src => src.shardId))
				.ForMember (x => x.Name, y => y.MapFrom (src => src.name));

			Mapper.CreateMap<ZoneData, Zone>() 
				.ForMember(x=>x.Id, y => y.MapFrom(src=>src.zoneId))
				.ForMember(x=>x.Name, y => y.MapFrom(src=>src.zone))
				.ForMember(x=>x.Event, opt => {
					opt.Condition( src => src.name != null );
					opt.MapFrom( src=> new ZoneEvent{ Name = src.name, 
						ActiveSince = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc).AddSeconds(src.started).ToLocalTime()});
					});
		}

		public List<Shard> ListShards()
		{
			var request = CreateRequest ("/shard/list");

			return ExecuteAndWrap<List<ShardData>, List<Shard>> (request);
		}

		public List<Zone> ListZones(int shardId)
		{
			var request = CreateRequest ("zoneevent/list");
			request.AddQueryParameter ("shardId", shardId.ToString ());

			return ExecuteAndWrap<List<ZoneData>, List<Zone>> (request);
		}
	}
}

