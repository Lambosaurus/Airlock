﻿using System;
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
        
        public Vector2 Position { get; protected set; }
        public Vector2 Velocity { get; protected set; }

        public virtual void Update(float elapsed)
        {
            Position += Velocity * elapsed;
        }

        public void Predict(long timestamp)
        {
            Velocity = NetVelocity;
            Position = NetPosition + (NetVelocity * ((timestamp - NetTimestamp) / 1000f));
        }

        public void SetPosition( Vector2 position )
        {
            Position = position;
            UpdateNetMotion(NetTime.Now());
        }

        public void UpdateNetMotion(long timestamp)
        {
            NetPosition = Position;
            NetVelocity = Velocity;
            NetTimestamp = timestamp;
        }

        public abstract void Render(Camera camera);
    }
}