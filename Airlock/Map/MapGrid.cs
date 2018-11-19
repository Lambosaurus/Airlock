using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Airlock.Util;
using NetCode;

namespace Airlock.Map
{
    public class MapGrid
    {
        public Point Size { get; private set; }

        public MapGrid(Point size)
        {
            Size = size;
        }
    }
}
