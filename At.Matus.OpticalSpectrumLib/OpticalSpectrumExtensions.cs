using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace At.Matus.OpticalSpectrumLib
{
    public static class OpticalSpectrumExtensions
    {
        public static double GetMaximumSignal(this IOpticalSpectrum spectrum)
        {
            double maxSignal = double.NegativeInfinity;
            foreach (var dp in spectrum.DataPoints)
            {
                if (dp.Signal > maxSignal)
                {
                    maxSignal = dp.Signal;
                }
            }
            return maxSignal;
        }

        public static double GetMinimumSignal(this IOpticalSpectrum spectrum)
        {
            double minSignal = double.PositiveInfinity;
            foreach (var dp in spectrum.DataPoints)
            {
                if (dp.Signal < minSignal)
                {
                    minSignal = dp.Signal;
                }
            }
            return minSignal;
        }
    }
}
