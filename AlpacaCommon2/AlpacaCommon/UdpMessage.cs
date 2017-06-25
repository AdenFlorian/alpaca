using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpacaCommon
{
    public class UdpMessage
    {
		public string Event;
		public object Data;

		public UdpMessage(string eventName, object data = null)
		{
			Event = eventName;
			Data = data;
		}
    }
}
