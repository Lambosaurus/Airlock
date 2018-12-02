using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using NetCode;
using NetCode.Connection;
using NetCode.Connection.UDP;
using NetCode.SyncPool;

using Airlock.Util;
using Airlock.Map;
using Airlock.Entities;

namespace Airlock.Server
{
    public class AirlockServer
    {
        public enum SyncPoolID { MapContent, ClientContent }

        public const int MaxPlayers = 4;
        public const double SynchPeriod = 1.0/20;
        private double SyncTimer = 0;
        
        private UDPServer Server;
        private List<AirlockHostedClient> Clients;

        private OutgoingSyncPool MapContent;
        private NetDefinitions NetDefs;

        private MapGrid Grid;
        public List<Entity> Entities;

        public AirlockServer( int port )
        {
            Server = new UDPServer(port);
            Server.IncomingConnectionLimit = MaxPlayers;
            Clients = new List<AirlockHostedClient>();

            NetDefs = new NetDefinitions();
            NetDefs.LoadEntityTypes();
            MapContent = new OutgoingSyncPool(NetDefs, (ushort)SyncPoolID.MapContent);

            Entities = new List<Entity>();
            Grid = MapGrid.StartingMap();

            AddEntity(new DroppedItem( new Vector2(200,-200) ));

            foreach (MapRoom room in Grid.Rooms)
            {
                MapContent.AddEntity(room);
            }
        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
            MapContent.AddEntity(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            Entities.Remove(entity);
            MapContent.GetHandleByObject(entity).State = SyncHandle.SyncState.Deleted;
        }

        public void Update( float elapsed )
        {
            foreach (Entity entity in Entities )
            {
                entity.Update(elapsed);

                if (entity is Unit unit)
                {
                    Grid.StaticCollide(unit);
                }
            }

            UDPFeed newFeed = Server.RecieveConnection();
            if (newFeed != null)
            {
                AddClient(new AirlockHostedClient(NetDefs, new NetworkClient(newFeed)));
            }

            foreach(AirlockHostedClient client in Clients)
            {
                client.Update(elapsed);
            }
            Clients.OnReverse((client) =>
            {
                if (client.Network.State == NetworkClient.ConnectionState.Closed)
                {
                    DropClient(client);
                }
            });

            SyncTimer += elapsed;
            if (SyncTimer > SynchPeriod)
            {
                SyncTimer -= SynchPeriod;
                MapContent.Synchronise();

                foreach (AirlockHostedClient client in Clients)
                {
                    // A second opportunity to update to get any pending packets out.
                    client.Network.Update();
                }
            }
        }

        private void AddClient(AirlockHostedClient client)
        {
            client.Network.Attach(MapContent);

            Color color;
            if (Clients.Count == 0) { color = Color.Red; }
            else if (Clients.Count == 1) { color = Color.Blue; }
            else if (Clients.Count == 2) { color = Color.Lime; }
            else { color = Color.Yellow; }
            
            UnitPlayer player = new UnitPlayer( new Vector2(0,0), color);
            client.SpawnPlayer(player);
            AddEntity(player);

            Clients.Add(client);
        }

        private void DropClient(AirlockHostedClient client)
        {
            client.Close();

            if (client.Player != null)
            {
                RemoveEntity(client.Player);
            }

            Clients.Remove(client);
        }

        public void Close()
        {
            Clients.OnReverse((client) => { DropClient(client); });
        }
    }
}
