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
            Radius = 15;
        }

        public override void Render(Camera camera)
        {
            Drawing.DrawCircle(camera.Batch, camera.Map(Position), camera.Scale * new Vector2(Radius * 2), 0, Color.Red);
        }
    }
}
