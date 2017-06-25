using System;

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
