﻿using System;
using System.Collections.Generic;
using System.Linq;

using NetCode;
using NetCode.SyncPool;
using NetCode.Connection;

using Airlock.Entities;

namespace Airlock.Server
{
    public class AirlockHostedClient
    {
        public NetworkClient Network;
        public IncomingSyncPool ClientContent;

        public UnitPlayer Player;

        public AirlockHostedClient(NetDefinitions netDefs, NetworkClient client)
        {
            Network = client;
            ClientContent = new IncomingSyncPool(netDefs, (ushort)AirlockServer.SyncPoolID.ClientContent);
            Network.Attach(ClientContent);
            Network.SetState(NetworkClient.ConnectionState.Open);
        }

        public void SpawnPlayer(UnitPlayer player)
        {
            Player = player;
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
