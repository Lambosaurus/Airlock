using Airlock.Entities;
using Airlock.Map;
using Airlock.Render;
using Airlock.Server;
using Airlock.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NetCode;
using NetCode.Connection;
using NetCode.Connection.UDP;
using NetCode.SyncPool;
using System;
using System.Net;

namespace Airlock.Client
{
    public class AirlockClient
    {
        private NetworkClient Network;
        private OutgoingSyncPool ReturnContent;
        private IncomingSyncPool WorldContent;
        private IncomingSyncPool ClientContent;

        private LocalPlayer LocalPlayer;
        private ClientInfo ClientInfo;
        private ClientInputs Inputs;

        private MapGrid Grid;

        private Entity EntityUnderMouse = null;

        private PeriodicTimer SyncTimer = new PeriodicTimer(1 / 20f);

        [Flags]
        public enum DebugStateFlags
        {
            None        = 0,
            Network     = (1 << 0),
            Shadow      = (1 << 1),
        };

        public DebugStateFlags DebugState = DebugStateFlags.None;

        public AirlockClient(IPAddress address, int destport, int srcport )
        {
            Network = new NetworkClient(new UDPConnection(address, destport, srcport));
            
            NetDefinitions netDefs = new NetDefinitions();
            netDefs.LoadEntityTypes();

            WorldContent = new IncomingSyncPool(netDefs, (ushort)AirlockServer.SyncPoolID.WorldContent);
            ReturnContent = new OutgoingSyncPool(netDefs, (ushort)AirlockServer.SyncPoolID.ReturnContent);
            ClientContent = new IncomingSyncPool(netDefs, (ushort)AirlockServer.SyncPoolID.ClientContent);
            ClientContent.LinkedPools.Add(WorldContent);
            Network.Attach(WorldContent);
            Network.Attach(ReturnContent);
            Network.Attach(ClientContent);

            Network.SetState(NetworkClient.ConnectionState.Open);
            LocalPlayer = new LocalPlayer();
            ReturnContent.AddEntity(LocalPlayer);

            Inputs = new ClientInputs();

            Grid = new MapGrid();
        }
        
        public void Update(Camera camera, float elapsed)
        {
            if (SyncTimer.IsElapsed(elapsed))
            {
                ReturnContent.Synchronise();
            }

            HandleInputs(camera, elapsed);
            
            Network.Update();
            WorldContent.Synchronise();
            ClientContent.Synchronise();
            
            foreach ( SyncHandle handle in WorldContent.NewHandles )
            {
                if (handle.Obj is MapRoom room)
                {
                    Grid.AddRoom(room);
                }
            }
            foreach ( SyncHandle handle in WorldContent.RemovedHandles )
            {
                if (handle.Obj is MapRoom room)
                {
                    Grid.RemoveRoom(room);
                }
            }

            foreach (SyncHandle handle in ClientContent.NewHandles)
            {
                if (handle.Obj is ClientInfo info)
                {
                    ClientInfo = info;
                }
            }
        }

        public void HandleInputs(Camera camera, float elapsed)
        {
            Inputs.Update();

            LocalPlayer.SetVelocity(Inputs.GetWASDVector() * 120f);

            LocalPlayer.Update(elapsed);
            Grid.StaticCollide(LocalPlayer);
            LocalPlayer.UpdateNetMotion(NetTime.Now());
            
            if (Inputs.KeyPressed(Keys.F1))
            {
                DebugState ^= DebugStateFlags.Network;
            }
            if (Inputs.KeyPressed(Keys.F2))
            {
                DebugState ^= DebugStateFlags.Shadow;
            }

            Vector2 cursor = camera.InverseMap(Inputs.Cursor);
            if (EntityUnderMouse != null)
            {
                if (!EntityUnderMouse.IsPointOver(cursor))
                {
                    EntityUnderMouse.MouseOver = false;
                    EntityUnderMouse = null;
                }
            }
            foreach ( SyncHandle handle in WorldContent.Handles )
            {
                if (handle.Obj is Entity entity)
                {
                    if (entity.IsPointOver(cursor))
                    {
                        if (EntityUnderMouse != null)
                        {
                            EntityUnderMouse.MouseOver = false;
                        }
                        entity.MouseOver = true;
                        EntityUnderMouse = entity;
                        break;
                    }
                }
            }
        }

        public void Render(Camera camera)
        {
            long timestamp = NetTime.Now();
            
            Grid.Render(camera);

            foreach ( SyncHandle handle in WorldContent.Handles )
            {
                if (handle.Obj is Entity entity)
                {
                    if (ClientInfo?.Player != entity)
                    {
                        entity.Predict(timestamp);
                        entity.Render(camera);
                    }
                }
            }

            if (LocalPlayer != null)
            {
                LocalPlayer.Predict(timestamp);
                LocalPlayer.Render(camera);
            }

            if ((DebugState & DebugStateFlags.Network) != 0)
            {
                ConnectionStats stats = Network.Connection.Stats;

                string text = string.Format(
                      "Status : {0}{1}\n"
                    + "Latency: {2}ms\n"
                    + "Loss   : {3}%\n"
                    + "Up     : {4}\n"
                    + "Down   : {5}\n",
                    Network.State,
                        Network.State == NetworkClient.ConnectionState.Closed
                        ? string.Format(" - {0}", Network.Connection.ConnectionStatus)
                        : "",
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
