using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Airlock.Util;

namespace Airlock.Render
{

    public class Camera
    {
        public Vector2 Position { get; private set; }
        public Vector2 Resolution { get; private set; }
        public float Scale { get; private set; }

        private Vector2 MappingOrigin;

        public SpriteBatch Batch;

        public Camera(SpriteBatch batch, Vector2 resolution)
        {
            Scale = 1f;
            Resolution = resolution;
            Batch = batch;

            SetPosition(new Vector2(0, 0));
        }
        
        public Vector2 Map(Vector2 point)
        {
            return (point + MappingOrigin) * Scale;
        }

        public Vector2 InverseMap(Vector2 point)
        {
            return ((point - Resolution/2f) / Scale) + Position;
        }

        protected void BuildMap()
        {
            MappingOrigin = ((Resolution / 2f) - Position) / Scale;
        }

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
            BuildMap();
        }
    }
}