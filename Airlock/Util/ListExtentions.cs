using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
