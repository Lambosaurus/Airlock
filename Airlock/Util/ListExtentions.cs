using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Airlock.Util
{
    public static class ListExtentions
    {
        public static void OnReverse<T>(this List<T> list, Action<T> action)
        {
            for (int i = list.Count-1; i >= 0; i--)
            {
                action(list[i]);
            }
        }

        public static bool IsZero(this Vector2 value)
        {
            return (value.X == 0) && (value.Y == 0);
        }
    }
}
