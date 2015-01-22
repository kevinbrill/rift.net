using System;
using System.Collections.Generic;

namespace rift.net.rest
{
	public class JsonResponse<T>
	{
		public string status {
			get;
			set;
		}

		public T data {
			get;
			set;
		}
	}
}