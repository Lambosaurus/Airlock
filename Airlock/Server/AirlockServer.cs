﻿using System;
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
        public List<Entity> Units;
        
        public AirlockServer( int port )
        {
            Server = new UDPServer(port);
            Server.IncomingConnectionLimit = MaxPlayers;
            Clients = new List<AirlockHostedClient>();

            NetDefs = new NetDefinitions();
            NetDefs.LoadEntityTypes();
            MapContent = new OutgoingSyncPool(NetDefs, (ushort)SyncPoolID.MapContent);

            Units = new List<Entity>();
            Grid = MapGrid.StartingMap();

            foreach (MapRoom room in Grid.Rooms)
            {
                MapContent.AddEntity(room);
            }
        }

        public void AddUnit(Entity unit)
        {
            Units.Add(unit);
            MapContent.AddEntity(unit);
        }

        public void RemoveUnit(Entity unit)
        {
            Units.Remove(unit);
            MapContent.GetHandleByObject(unit).State = SyncHandle.SyncState.Deleted;
        }

        public void Update( float elapsed )
        {
            foreach (Entity unit in Units )
            {
                unit.Update(elapsed);
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

            UnitPlayer player = new UnitPlayer( new Vector2(0,0), Color.Red);
            client.SpawnPlayer(player);
            AddUnit(player);

            Clients.Add(client);
        }

        private void DropClient(AirlockHostedClient client)
        {
            client.Close();

            if (client.Player != null)
            {
                RemoveUnit(client.Player);
            }

            Clients.Remove(client);
        }

        public void Close()
        {
            Clients.OnReverse((client) => { DropClient(client); });
        }
    }
}
