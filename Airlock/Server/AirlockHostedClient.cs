using System;
using System.Collections.Generic;
using System.Linq;

using NetCode;
using NetCode.SyncPool;
using NetCode.Connection;

using Airlock.Entities;
using Airlock.Client;
using Airlock.Util;

namespace Airlock.Server
{
    public class AirlockHostedClient
    {
        public NetworkClient Network { get; protected set; }
        private IncomingSyncPool ReturnContent;
        private OutgoingSyncPool ClientContent;

        private ClientInfo ClientInfo;
        public UnitPlayer Player { get; protected set; }

        private PeriodicTimer SyncTimer = new PeriodicTimer(1 / 20f);

        public AirlockHostedClient(NetDefinitions netDefs, NetworkClient client, OutgoingSyncPool worldContent)
        {
            Network = client;
            ReturnContent = new IncomingSyncPool(netDefs, (ushort)AirlockServer.SyncPoolID.ReturnContent);
            ClientContent = new OutgoingSyncPool(netDefs, (ushort)AirlockServer.SyncPoolID.ClientContent);
            ClientContent.LinkedPools.Add(worldContent);
            Network.Attach(ReturnContent);
            Network.Attach(ClientContent);
            Network.SetState(NetworkClient.ConnectionState.Open);
            ClientInfo = new ClientInfo();
            ClientContent.AddEntity(ClientInfo);
        }

        public void SpawnPlayer(UnitPlayer player)
        {
            Player = player;
            ClientInfo.Player = player;
        }
        
        public void Update(float elapsed)
        {
            if (SyncTimer.IsElapsed(elapsed))
            {
                ClientContent.Synchronise();
            }
            Network.Update();
            ReturnContent.Synchronise();
            
            foreach (SyncHandle handle in ReturnContent.Handles)
            {
                if (handle.Updated)
                {
                    if (handle.Obj is LocalPlayer request)
                    {
                        Player.UpdateWithMotionRequest(request);
                    }
                }
            }
        }

        public void Close()
        {
            Network.SetState(NetworkClient.ConnectionState.Closed);
            Network.Update();
            Network.Destroy();
        }
    }
}
