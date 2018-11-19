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
        public List<MapRoom> Rooms;

        public MapGrid()
        {
            Rooms = new List<MapRoom>();
        }

        public static MapGrid StartingMap()
        {
            MapGrid grid = new MapGrid();
            grid.Rooms.Add(new MapRoom( new Point(0,0), new Point(1,1) ));
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
