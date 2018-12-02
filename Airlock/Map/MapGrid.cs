using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using NetCode;
using NetCode.SyncPool;

using Airlock.Render;
using Airlock.Util;
using Airlock.Entities;

namespace Airlock.Map
{
    public class MapGrid
    {
        public const int TileSize = 80;
        protected Dictionary<ushort, MapRoom> RoomDict;
        public IEnumerable<MapRoom> Rooms { get { return RoomDict.Values; } }

        public const ushort GridSize = 80;

        public MapGrid()
        {
            RoomDict = new Dictionary<ushort, MapRoom>();
            HitGrid = new ushort[GridSize, GridSize];
        }

        private ushort LastFreeRoomID = 1;
        private ushort GetNextFreeRoomID()
        {
            ushort id = LastFreeRoomID++;
            while (RoomDict.ContainsKey(LastFreeRoomID)) { LastFreeRoomID++; }
            return id;
        }

        private ushort[,] HitGrid;
        public void AddRoom(MapRoom room)
        {
            room.RoomID = GetNextFreeRoomID();
            RoomDict[room.RoomID] = room;

            for (int x = room.Origin.X; x < room.End.X; x++)
            {
                for (int y = room.Origin.Y; y < room.End.Y; y++)
                {
                    HitGrid[x + GridSize / 2, y + GridSize / 2] = room.RoomID;
                }
            }
        }

        public void RemoveRoom(MapRoom room)
        {
            RoomDict.Remove(room.RoomID);

            for (int x = room.Origin.X; x < room.End.X; x++)
            {
                for (int y = room.Origin.Y; y < room.End.Y; y++)
                {
                    int gx = x + GridSize / 2;
                    int gy = y + GridSize / 2;
                    if (HitGrid[gx, gy] == room.RoomID)
                    {
                        // Dont clear it if it has someone elses roomID, in case rooms are added and removed out of order.
                        HitGrid[gx, gy] = 0;
                    }
                    
                }
            }
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
            return grid;
        }

        public void Render(Camera camera)
        {
            foreach (MapRoom room in Rooms)
            {
                room.Render(camera);
            }
        }

        public Point2 PositionToGrid(Vector2 position)
        {
            return new Point2( Fmath.Floor(position.X / TileSize),
                               Fmath.Floor(position.Y / TileSize) );
        }
        
        public bool Passable(Point2 pt)
        {
            ushort roomID = HitGrid[pt.X + (TileSize/2), pt.Y + (TileSize/2)];
            if (roomID != 0)
            {
                return RoomDict[roomID].Passable();
            }
            return false;
        }

        public void StaticCollide(Unit unit)
        {
            Point2 point = PositionToGrid(unit.Position);

            Vector2 overlap = new Vector2(0, 0);

            if (Passable(point))
            {
                Vector2 roomMin = point.ToVector2(TileSize);
                Vector2 roomMax = roomMin + new Vector2(TileSize, TileSize);

                if ( unit.Position.X + unit.Radius > roomMax.X )
                {
                    if ( !Passable(new Point2(point.X + 1, point.Y)) )
                    {
                        overlap.X = (unit.Position.X + unit.Radius) - roomMax.X;
                    }
                }
                else if (unit.Position.X - unit.Radius < roomMin.X)
                {
                    if (!Passable(new Point2(point.X - 1, point.Y)))
                    {
                        overlap.X = (unit.Position.X - unit.Radius) - roomMin.X;
                    }
                }

                if (unit.Position.Y + unit.Radius > roomMax.Y)
                {
                    if (!Passable(new Point2(point.X, point.Y + 1)))
                    {
                        overlap.Y = (unit.Position.Y + unit.Radius) - roomMax.Y;
                    }
                }
                else if (unit.Position.Y - unit.Radius < roomMin.Y)
                {
                    if (!Passable(new Point2(point.X, point.Y - 1)))
                    {
                        overlap.Y = (unit.Position.Y - unit.Radius) - roomMin.Y;
                    }
                }
            }
            else
            {
                // Find nearest valid tile.
            }

            if (!overlap.IsZero())
            {
                unit.StaticCollide(overlap);
            } 
        }
    }
}
