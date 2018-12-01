using System;
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

        [Flags]
        public enum DebugStateFlags
        {
            None        = 0,
            Network     = 1,
        };

        public DebugStateFlags DebugState = DebugStateFlags.None;

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

            if (Inputs.KeyPressed(Keys.F1))
            {
                DebugState ^= DebugStateFlags.Network;
            }
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

            if ((DebugState & DebugStateFlags.Network) != 0)
            {
                ConnectionStats stats = Network.Connection.Stats;

                string text = string.Format(
                      "Status : {0}\n"
                    + "Latency: {1}ms\n"
                    + "Loss   : {2}%\n"
                    + "Up     : {3}\n"
                    + "Down   : {4}\n",
                    Network.Connection.ConnectionStatus,
                    stats.Latency,
                    (int)(stats.PacketLoss * 100),
                    NetCode.Util.Primitive.SIFormat(stats.BytesSent.PerSecond, "B/s"),
                    NetCode.Util.Primitive.SIFormat(stats.BytesRecieved.PerSecond, "B/s")
                    );

                Drawing.DrawString(camera.Batch, text, new Vector2(20, 20), Color.White);
            }
        }

        public void Close()
        {
            Network.SetState(NetworkClient.ConnectionState.Closed);
            Network.Update();
        }
    }
}
