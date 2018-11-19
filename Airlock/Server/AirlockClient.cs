using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetCode;
using NetCode.SyncPool;
using NetCode.Connection;

namespace Airlock.Server
{
    public class AirlockHostedClient
    {
        public NetworkClient Network;
        public IncomingSyncPool ClientContent;

        public AirlockHostedClient(NetDefinitions netDefs, NetworkClient client)
        {
            Network = client;
            ClientContent = new IncomingSyncPool(netDefs, (ushort)AirlockServer.SyncPoolID.ClientContent);
            Network.Attach(ClientContent);
            Network.SetState(NetworkClient.ConnectionState.Open);
        }
        
        public void Update(double elapsed)
        {
            Network.Update();
            ClientContent.Synchronise();
        }

        public void Close()
        {
            Network.SetState(NetworkClient.ConnectionState.Closed);
            Network.Update();
            Network.Destroy();
        }
    }
}
