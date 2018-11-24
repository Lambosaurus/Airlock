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
        protected Dictionary<ushort, MapRoom> RoomDict;
        public IEnumerable<MapRoom> Rooms { get { return RoomDict.Values; } }
        
        public MapGrid()
        {
            RoomDict = new Dictionary<ushort, MapRoom>();
        }

        private ushort LastFreeRoomID = 0;
        private ushort GetNextFreeRoomID()
        {
            ushort id = LastFreeRoomID++;
            while( RoomDict.ContainsKey(LastFreeRoomID) ) { LastFreeRoomID++; }
            return id;
        }

        private ushort[,] HitMesh;
        Point2 HitMeshOffset;
        private void BuildMesh()
        {
            Point2 min = new Point2(0, 0);
            Point2 max = new Point2(0, 0);
            foreach (MapRoom room in Rooms)
            {
                if (room.Origin.X < min.X ) { min.X = room.Origin.X; }
                if (room.Origin.Y < min.Y) { min.Y = room.Origin.Y; }
                if (room.End.X > max.X) { max.X = room.End.X; }
                if (room.End.Y > max.Y) { max.Y = room.End.Y; }
            }

            HitMesh = new ushort[max.X - min.X, max.Y - min.Y];
            HitMeshOffset = -min;

            foreach ( MapRoom room in Rooms )
            {
                for( int x = room.Origin.X; x < room.End.X; x++)
                {
                    for (int y = room.Origin.Y; y < room.End.Y; y++)
                    {
                        HitMesh[x + HitMeshOffset.X, y + HitMeshOffset.Y] = room.RoomID;
                    }
                }
            }
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
            MapRoom[] newRooms = new MapRoom[]
            {
                new MapRoom(new Point2(-4, -2), new Point2(3, 4)),
                new MapRoom(new Point2(0, 0), new Point2(2, 2)),
                new MapRoom(new Point2(-1, 0), new Point2(1, 1)),
                new MapRoom(new Point2(1, -3), new Point2(1, 3)),
                new MapRoom(new Point2(2, -4), new Point2(3, 3))
            };
            foreach (MapRoom room in newRooms) { grid.AddRoom(room); }
            grid.BuildMesh();
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
