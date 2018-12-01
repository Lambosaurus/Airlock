using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using NetCode;

namespace Airlock.Client
{
    [EnumerateSynchEntity]
    public class PlayerMotionRequest
    {
        [Synchronisable]
        public Vector2 Position { get; set; }

        [Synchronisable]
        public Vector2 Velocity { get; set; }
    }
}
