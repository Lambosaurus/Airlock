using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airlock.Util
{
    public class PeriodicTimer
    {
        public float Period { get; protected set; }
        private float Elapsed = 0;

        public PeriodicTimer(float period)
        {
            Period = period;
        }

        public bool IsElapsed(float elapsed)
        {
            Elapsed += elapsed;
            if (Elapsed > Period)
            {
                Elapsed -= Period;
                return true;
            }
            return false;
        }
    }
}
