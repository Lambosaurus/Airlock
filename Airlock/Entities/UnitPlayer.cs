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
        }

        public UnitPlayer(Vector2 position)
        {
            Position = position;
        }

        public override void Render(Camera camera)
        {
            Drawing.DrawSquare(camera.Batch, camera.Map(Position), camera.Scale * new Vector2(30,40), 0, Color.Red);
        }
    }
}
