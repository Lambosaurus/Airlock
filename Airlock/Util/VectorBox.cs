using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Airlock.Util
{
    public struct VectorBox
    {
        public Vector2 Origin;
        public Vector2 Size;
        
        public VectorBox(Vector2 origin, Vector2 size)
        {
            Origin = origin;
            Size = size;
        }

        public bool Contains(Vector2 point)
        {
            return    point.X >= Origin.X
                   && point.Y >= Origin.Y
                   && point.X <= Origin.X + Size.X
                   && point.Y <= Origin.Y + Size.X;
        }

        public bool Contains(Point point)
        {
            return    point.X >= Origin.X
                   && point.Y >= Origin.Y
                   && point.X <= Origin.X + Size.X
                   && point.Y <= Origin.Y + Size.X;
        }

        public bool Ovelaps(VectorBox box)
        {
            return    Origin.X > box.Origin.X + box.Size.X
                   && Origin.X + Size.X < box.Origin.X
                   && Origin.Y > box.Origin.Y + box.Size.Y
                   && Origin.Y + Size.Y < box.Origin.X;
        }

        public static bool operator ==(VectorBox a, VectorBox b)
        {
            return a.Origin == b.Origin && a.Size == b.Size;
        }

        public static bool operator !=(VectorBox a, VectorBox b)
        {
            return a.Origin != b.Origin || a.Size != b.Size;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            else if (obj is VectorBox v) { return this == v; }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
