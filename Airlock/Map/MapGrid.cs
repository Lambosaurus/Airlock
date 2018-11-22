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
        protected Dictionary<int, MapRoom> RoomDict;
        public IEnumerable<MapRoom> Rooms { get { return RoomDict.Values; } }
        
        public MapGrid()
        {
            RoomDict = new Dictionary<int, MapRoom>();
        }

        private int LastFreeRoomID = 0;
        private int GetNextFreeRoomID()
        {
            int id = LastFreeRoomID++;
            while( RoomDict.ContainsKey(LastFreeRoomID) ) { LastFreeRoomID++; }
            return id;
        }

        private int[,] HitMesh;
        Point HitMeshCenter;
        private void BuildMesh()
        {
            Point min = new Point(0, 0);
            Point max = new Point(0,0);
            foreach (MapRoom room in Rooms)
            {
                if (room.Origin.X < min.X ) { min.X = room.Origin.X; }
                if (room.Origin.Y < min.Y) { min.Y = room.Origin.Y; }
                if (room.Origin.X + room.Size.X > max.X) { min.X = room.Origin.X + room.Size.X; }
                if (room.Origin.Y + room.Size.Y > max.Y) { min.Y = room.Origin.Y + room.Size.Y; }
            }

            int dx = max.X - min.X;
            int dy = max.Y - min.Y;

            HitMeshCenter = new Point(-min.X, -min.Y);

        }
        
        public void AddRoom( MapRoom room )
        {
            room.RoomID = GetNextFreeRoomID();
            RoomDict[room.RoomID] = room;
        }

        public void RemoveRoom(MapRoom room)
        {
            RoomDict.Remove(room.RoomID);
        }
        
        public static MapGrid StartingMap()
        {
            MapGrid grid = new MapGrid();
            grid.AddRoom(new MapRoom(new Point(-4, -2), new Point(3, 4)));
            grid.AddRoom(new MapRoom(new Point(0, 0), new Point(2, 2)));
            grid.AddRoom(new MapRoom(new Point(-1, 0), new Point(1, 1)));
            grid.AddRoom(new MapRoom(new Point(1, -3), new Point(1, 3)));
            grid.AddRoom(new MapRoom(new Point(2, -4), new Point(3, 3)));
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
