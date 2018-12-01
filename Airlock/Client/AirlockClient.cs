using Airlock.Server;
using NetCode;
using NetCode.Connection;
using NetCode.Connection.UDP;
using NetCode.SyncPool;
using System.Net;

using Airlock.Map;
using Airlock.Render;
using Airlock.Entities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Airlock.Client
{
    public class AirlockClient
    {
        public NetworkClient Network;
        public OutgoingSyncPool ClientContent;
        public IncomingSyncPool MapContent;

        public PlayerMotionRequest MotionRequest;
        public ClientInputs Inputs;

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
            MotionRequest = new PlayerMotionRequest();
            ClientContent.AddEntity(MotionRequest);

            Inputs = new ClientInputs();
        }

        private double SyncTimer = 0;

        public void Update(float elapsed)
        {
            SyncTimer += elapsed;
            if (SyncTimer > AirlockServer.SynchPeriod)
            {
                SyncTimer -= AirlockServer.SynchPeriod;
                MapContent.Synchronise();
            }

            HandleInputs(elapsed);
            
            ClientContent.Synchronise();
            Network.Update();
            MapContent.Synchronise();
        }

        public void HandleInputs(float elapsed)
        {
            Inputs.Update();

            MotionRequest.Velocity = Inputs.GetWASDVector() * 120f;
            MotionRequest.Position += MotionRequest.Velocity * elapsed;
        }

        public void Render(Camera camera)
        {
            long timestamp = NetTime.Now();
            foreach ( SyncHandle handle in MapContent.Handles )
            {
                if (handle.Obj is MapRoom room)
                {
                    room.Render(camera);
                }
                if (handle.Obj is Entity unit)
                {
                    unit.Predict(timestamp);
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
