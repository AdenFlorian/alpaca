using Newtonsoft.Json;

namespace GameServerNet
{
    public class NatPunchResponse
    {
        [JsonProperty("ip")]
        public string PublicIpAddress;
        
        [JsonProperty("port")]
        public int PublicPort;
    }
}