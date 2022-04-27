using Sense.Led;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Led.Services
{
    public interface ILedMatrix
    {
        public void SetPixels(Pixels pixels);
    }
}
