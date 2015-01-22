using System;
using RestSharp;
using AutoMapper;
using rift.net.rest;
using System.Collections.Generic;

namespace rift.net
{
	public class RiftClient : RiftRestClient
	{
		static RiftClient()
		{
			Mapper.CreateMap<ShardData, Shard> ()
				.ForMember (x => x.Id, y => y.MapFrom (src => src.shardId))
				.ForMember (x => x.Name, y => y.MapFrom (src => src.name));
		}

		public List<Shard> ListShards()
		{
			var request = CreateRequest ("/shard/list");

			return ExecuteAndWrap<ShardData, Shard> (request);
		}
	}
}

