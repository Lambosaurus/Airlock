using System;
using System.Collections.Generic;
using System.Linq;
using Airlock.Render;
using Microsoft.Xna.Framework;
using NetCode;

namespace Airlock.Entities
{
    [EnumerateSynchEntity]
    public class UnitPlayer : Unit
    {
        public UnitPlayer()
        {
            Radius = 15;
        }

        public UnitPlayer(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Radius = 15;
        }
    }
}
