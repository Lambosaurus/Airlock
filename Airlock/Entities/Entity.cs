using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NetCode;

using Airlock.Render;

namespace Airlock.Entities
{
    public abstract class Entity
    {
        [Synchronisable]
        public Vector2 NetPosition { get; protected set; }
        [Synchronisable]
        public Vector2 NetVelocity { get; protected set; }
        [Synchronisable(SyncFlags.Timestamp)]
        public long NetTimestamp { get; protected set; }

        [Synchronisable]
        public float Radius { get; protected set; }

        public Vector2 Position { get; protected set; }
        public Vector2 Velocity { get; protected set; }

        public bool MouseOver { get; set; } = false;
        
        public virtual void Update(float elapsed)
        {
            Position += Velocity * elapsed;
        }

        public void Predict(long timestamp)
        {
            Velocity = NetVelocity;
            Position = NetPosition + (NetVelocity * ((timestamp - NetTimestamp) / 1000f));
        }
        
        public void UpdateNetMotion(long timestamp)
        {
            NetPosition = Position;
            NetVelocity = Velocity;
            NetTimestamp = timestamp;
        }

        public void StaticCollide( Vector2 overlap )
        {
            Vector2 newVelocity = Velocity;
            Position -= overlap;
            if ((overlap.X > 0 && Velocity.X > 0) ||
                (overlap.X < 0 && Velocity.X < 0))
            {
                newVelocity.X = 0;
            }
            if ((overlap.Y > 0 && Velocity.Y > 0) ||
                (overlap.Y < 0 && Velocity.Y < 0))
            {
                newVelocity.Y = 0;
            }
            Velocity = newVelocity;
        }

        public virtual bool IsPointOver( Vector2 point )
        {
            Vector2 delta = point - Position;
            return delta.X > -Radius
                && delta.X < Radius
                && delta.Y > -Radius
                && delta.Y < Radius;
        }

        public abstract void Render(Camera camera);
    }
}
