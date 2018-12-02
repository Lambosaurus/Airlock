using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Airlock.Util
{
    public struct Point2
    {
        public int X;
        public int Y;

        public Point2(int x)
        {
            X = x;
            Y = x;
        }

        public Point2(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public Vector2 ToVector2(float scalar)
        {
            return new Vector2(X * scalar, Y * scalar);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", X, Y);
        }

        public static Point2 operator +(Point2 a, Point2 b)
        {
            return new Point2(a.X + b.X, a.Y + b.Y);
        }

        public static Point2 operator -(Point2 a, Point2 b)
        {
            return new Point2(a.X - b.X, a.Y - b.Y);
        }

        public static Point2 operator *(Point2 p, int v)
        {
            return new Point2(p.X * v, p.Y * v);
        }

        public static Point2 operator /(Point2 p, int v)
        {
            return new Point2(p.X / v, p.Y / v);
        }

        public static Point2 operator -(Point2 p)
        {
            return new Point2(-p.X, -p.Y);
        }

        public static bool operator ==(Point2 a, Point2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point2 a, Point2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override bool Equals(object ob)
        {
            if (ob != null && ob is Point2 p)
            {
                return X == p.X && Y == p.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (Y << 16) + X;
        }
    }
}
