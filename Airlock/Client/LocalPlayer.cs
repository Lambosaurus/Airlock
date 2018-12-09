using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using NetCode;

using Airlock.Entities;
using Airlock.Render;

namespace Airlock.Client
{
    [EnumerateSynchEntity]
    public class LocalPlayer : Unit
    {
        public LocalPlayer()
        {
            Radius = 15;
        }

        public void SetVelocity( Vector2 velocity )
        {
            Velocity = velocity;
        }
    }
}
