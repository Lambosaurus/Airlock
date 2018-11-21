using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NetCode;

using Airlock.Render;

namespace Airlock.Entities
{
    public abstract class Unit
    {
        [Synchronisable]
        public Vector2 Position { get; protected set; }

        public abstract void Render(Camera camera);

        public virtual void Update()
        {

        }
    }
}
