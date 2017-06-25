namespace GameServerNet
{
    public class ReceivedClientMessage
    {
        public readonly ReceivedMessage Message;
        public readonly GameClient Client;

        public ReceivedClientMessage(ReceivedMessage message, GameClient client)
        {
            Message = message;
            Client = client;
        }
    }
}