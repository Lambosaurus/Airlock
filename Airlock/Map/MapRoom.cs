using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using NetCode;
using Airlock.Util;
using Airlock.Render;

namespace Airlock.Map
{
    [EnumerateSynchEntity]
    public class MapRoom
    {
        public const int WallSize = 4;

        [Synchronisable]
        public Point Origin { get; private set; }
        [Synchronisable]
        public Point Size { get; private set; }
        public Vector2 Center { get { return Origin.ToVector2() + Size.ToVector2() / 2; } }

        public MapRoom()
        {
        }

        public MapRoom(Point origin, Point size)
        {
            Origin = origin;
            Size = size;
        }

        public void Render(Camera camera)
        {
            Drawing.DrawSquare(camera.Batch, camera.Map(Center * MapGrid.TileSize), (Size.ToVector2() * MapGrid.TileSize * camera.Scale) - new Vector2(WallSize * 2), 0, Color.LightGray);
        }
    }
}
