﻿using System;
using System.Collections.Generic;
using System.Linq;
using Airlock.Render;
using Microsoft.Xna.Framework;
using NetCode;

using Airlock.Client;

namespace Airlock.Entities
{
    [EnumerateSynchEntity]
    public class UnitPlayer : Unit
    {
        [Synchronisable]
        public Color PlayerColor;

        public UnitPlayer()
        {
            Radius = 15;
        }

        public UnitPlayer(Vector2 position, Color color)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Radius = 15;
            PlayerColor = color;
        }

        public void UpdateWithMotionRequest(LocalPlayer request)
        {
            long timestamp = NetTime.Now();
            request.Predict(timestamp);
            Position = request.Position;
            Velocity = request.Velocity;
            UpdateNetMotion(timestamp);
        }

        public override void Render(Camera camera)
        {
            Drawing.DrawCircle(camera.Batch, camera.Map(Position), camera.Scale * new Vector2(Radius * 2), 0, PlayerColor);
        }

        public void ShadowRender(Camera camera)
        {
            Drawing.DrawCircle(camera.Batch, camera.Map(Position), camera.Scale * new Vector2(Radius * 2), 0, PlayerColor * 0.33f);
        }
    }
}
