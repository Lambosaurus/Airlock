using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Airlock.Render;
using Airlock.Util;
using NetCode;

namespace Airlock.Map
{
    public class MapGrid
    {
        public const int TileSize = 80;
        public List<MapRoom> Rooms;
        
        public MapGrid()
        {
            Rooms = new List<MapRoom>();
        }
        
        public static MapGrid StartingMap()
        {
            MapGrid grid = new MapGrid();
            grid.Rooms.Add(new MapRoom(new Point(-4, -2), new Point(3, 4)));
            grid.Rooms.Add(new MapRoom(new Point(0, 0), new Point(2, 2)));
            grid.Rooms.Add(new MapRoom(new Point(-1, 0), new Point(1, 1)));
            grid.Rooms.Add(new MapRoom(new Point(1, -3), new Point(1, 3)));
            grid.Rooms.Add(new MapRoom(new Point(2, -4), new Point(3, 3)));
            return grid;
        }

        public void Render(Camera camera)
        {
            foreach(MapRoom room in Rooms)
            {
                room.Render(camera);
            }
        }
    }
}
