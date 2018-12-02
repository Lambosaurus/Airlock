using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using NetCode;

using Airlock.Entities;

namespace Airlock.Client
{
    [EnumerateSynchEntity]
    public class PlayerMotionRequest : Unit
    {
        public PlayerMotionRequest()
        {
            Radius = 15;
        }

        public void SetVelocity( Vector2 velocity )
        {
            Velocity = velocity;
        }
    }
}
