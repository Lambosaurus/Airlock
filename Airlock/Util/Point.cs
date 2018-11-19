using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Airlock.Util
{
    public struct Point
    {
        public int X;
        public int Y;
        
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point Zero { get { return new Point(); } }

        public Vector2 ToVector()
        {
            return new Vector2(X, Y);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator *(Point p, int alpha)
        {
            return new Point(p.X * alpha, p.Y * alpha);
        }
        
        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            else if (obj is Point p) { return this == p; }
            return false;
        }

        public override int GetHashCode()
        {
            return (X | 0x0000FFFF) + (Y << 16);
        }
    }
}
