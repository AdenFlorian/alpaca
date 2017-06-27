using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AlpacaCommon
{
	public class ConnectedPayload
	{
		[JsonProperty(Required = Required.AllowNull)]
		public IEnumerable<NetObj> ExistingNetObjects;
	}
}
