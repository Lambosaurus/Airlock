using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using NetCode;
using NetCode.Connection;
using NetCode.Connection.UDP;
using NetCode.SyncPool;

using Airlock.Server;
using Airlock.Map;
using Airlock.Render;
using Airlock.Entities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Airlock.Client
{
    public class AirlockClient
    {
        private NetworkClient Network;
        private OutgoingSyncPool ClientContent;
        private IncomingSyncPool MapContent;

        private PlayerMotionRequest MotionRequest;
        private ClientInputs Inputs;

        private MapGrid Grid;

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

            Grid = new MapGrid();
        }

        private double SyncTimer = 0;

        public void Update(float elapsed)
        {
            SyncTimer += elapsed;
            if (SyncTimer > AirlockServer.SynchPeriod)
            {
                SyncTimer -= AirlockServer.SynchPeriod;
                ClientContent.Synchronise();
            }

            HandleInputs(elapsed);
            
            Network.Update();
            MapContent.Synchronise();
            
            foreach ( SyncHandle handle in MapContent.NewHandles )
            {
                if (handle.Obj is MapRoom room)
                {
                    Grid.AddRoom(room);
                }
            }
            foreach ( SyncHandle handle in MapContent.RemovedHandles )
            {
                if (handle.Obj is MapRoom room)
                {
                    Grid.RemoveRoom(room);
                }
            }
        }

        public void HandleInputs(float elapsed)
        {
            Inputs.Update();

            MotionRequest.SetVelocity(Inputs.GetWASDVector() * 120f);
            
            MotionRequest.Update(elapsed);
            Grid.StaticCollide(MotionRequest);
            MotionRequest.UpdateNetMotion(NetTime.Now());
            
            if (Inputs.KeyPressed(Keys.F1))
            {
                DebugState ^= DebugStateFlags.Network;
            }
        }

        public void Render(Camera camera)
        {
            long timestamp = NetTime.Now();
            
            Grid.Render(camera);

            foreach ( SyncHandle handle in MapContent.Handles )
            {
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
