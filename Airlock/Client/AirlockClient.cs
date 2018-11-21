using Airlock.Server;
using NetCode;
using NetCode.Connection;
using NetCode.Connection.UDP;
using NetCode.SyncPool;
using System.Net;

using Airlock.Map;
using Airlock.Render;
using Airlock.Entities;

namespace Airlock.Client
{
    public class AirlockClient
    {
        public NetworkClient Network;
        public OutgoingSyncPool ClientContent;
        public IncomingSyncPool MapContent;

        public AirlockClient(IPAddress address, int destport, int srcport )
        {
            Network = new NetworkClient(new UDPConnection(address, destport, srcport));
            
            NetDefinitions netDefs = new NetDefinitions();
            netDefs.LoadEntityTypes();

            MapContent = new IncomingSyncPool(netDefs, (ushort)AirlockServer.SyncPoolID.MapContent);
            ClientContent = new OutgoingSyncPool(netDefs, (ushort)AirlockServer.SyncPoolID.ClientContent);
            Network.Attach(MapContent);
            Network.Attach(ClientContent);

            Network.SetState(NetworkClient.ConnectionState.Open);
        }

        private double SyncTimer = 0;

        public void Update(double elapsed)
        {
            SyncTimer += elapsed;
            if (SyncTimer > AirlockServer.SynchPeriod)
            {
                SyncTimer -= AirlockServer.SynchPeriod;
                MapContent.Synchronise();
            }

            ClientContent.Synchronise();
            Network.Update();
            MapContent.Synchronise();
        }

        public void Render(Camera camera)
        {
            foreach ( SyncHandle handle in MapContent.Handles )
            {
                if (handle.Obj is MapRoom room)
                {
                    room.Render(camera);
                }
                if (handle.Obj is Unit unit)
                {
                    unit.Render(camera);
                }
            }
        }

        public void Close()
        {
            Network.SetState(NetworkClient.ConnectionState.Closed);
            Network.Update();
        }
    }
}
