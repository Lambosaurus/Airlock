using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetCode;
using Airlock.Entities;

namespace Airlock.Server
{
    [EnumerateSynchEntity]
    public class ClientInfo
    {
        [Synchronisable(SyncFlags.LinkedReference)]
        public UnitPlayer Player;
    }
}
