using System;
using System.Collections.Generic;
using System.Linq;
using Airlock.Render;
using Microsoft.Xna.Framework;
using NetCode;

namespace Airlock.Entities
{
    public abstract class Unit : Entity
    {
        [Synchronisable]
        public float Radius { get; protected set; }

        public override void Render(Camera camera)
        {
            Drawing.DrawCircle(camera.Batch, camera.Map(Position), camera.Scale * new Vector2(Radius * 2), 0, Color.Red);
        }
    }
}
