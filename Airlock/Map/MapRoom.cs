using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using NetCode;
using Airlock.Util;
using Airlock.Render;

namespace Airlock.Map
{
    [EnumerateSynchEntity]
    public class MapRoom
    {
        [Synchronisable]
        public Point2 Origin { get; private set; }
        [Synchronisable]
        public Point2 Size { get; private set; }
        public Point2 End { get { return Origin + Size; } }
        
        public Vector2 Center { get { return Origin.ToVector2() + Size.ToVector2() / 2; } }
        public ushort RoomID { get; set; }

        public MapRoom()
        {
        }

        public MapRoom(Point2 origin, Point2 size)
        {
            Origin = origin;
            Size = size;
        }

        public void Render(Camera camera)
        {
            Drawing.DrawSquare(camera.Batch, camera.Map(Center * MapGrid.TileSize), (Size.ToVector2() * MapGrid.TileSize * camera.Scale), 0, Color.LightGray);
        }

        public bool Passable()
        {
            return true;
        }
    }
}
