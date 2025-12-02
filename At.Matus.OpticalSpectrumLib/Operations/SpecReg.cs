using System;
using System.Collections.Generic;
using System.Linq;

namespace At.Matus.OpticalSpectrumLib
{
    public static partial class SpecReg
    {
        public static OpticalSpectrum ResampleSpectrum(this IOpticalSpectrum inputSpec, double startWavelength, double endWavelength, double step)
        {
            double[] targetWavelengths = CreateEquidistantWavelengths(startWavelength, endWavelength, step);
            return ResampleSpectrum(inputSpec, targetWavelengths);
        }

        public static OpticalSpectrum ResampleSpectrum(this IOpticalSpectrum inputSpec, List<double> unsortedTargetWavelengths)
        {
            double[] targetWavelengths = unsortedTargetWavelengths.OrderBy(wl => wl).ToArray();
            return ResampleSpectrum(inputSpec, targetWavelengths);
        }

        public static OpticalSpectrum ResampleSpectrum(this IOpticalSpectrum inputSpec, double[] targetWavelengths)
        {
            double[] inputWavelengths = inputSpec.Wavelengths;
            double[] inputIntensities = inputSpec.Signals;
            double[] inputStdErr = inputSpec.StdErrValues;
            double[] inputStdDev = inputSpec.StdDevValues;
            double[] outputIntensities = new double[targetWavelengths.Length];
            double[] outputStdErr = new double[targetWavelengths.Length];
            double[] outputStdDev = new double[targetWavelengths.Length];
            for (int i = 0; i < targetWavelengths.Length; i++)
            {
                double targetWl = targetWavelengths[i];
                var (smallerIndex, largerIndex) = FindNeighbors(inputWavelengths, targetWl);
                if (smallerIndex == -1 || largerIndex == -1)
                {
                    throw new ArgumentException("target wavelength outside input spectrum range");
                }
                else
                {
                    double x0 = inputWavelengths[smallerIndex];
                    double x1 = inputWavelengths[largerIndex];
                    double y0 = inputIntensities[smallerIndex];
                    double y1 = inputIntensities[largerIndex];
                    outputIntensities[i] = LinearInterpolate(x0, y0, x1, y1, targetWl);
                    double se0 = inputStdErr[smallerIndex];
                    double se1 = inputStdErr[largerIndex];
                    outputStdErr[i] = LinearInterpolate(x0, se0, x1, se1, targetWl);
                    double sd0 = inputStdDev[smallerIndex];
                    double sd1 = inputStdDev[largerIndex];
                    outputStdDev[i] = LinearInterpolate(x0, sd0, x1, sd1, targetWl);
                }
            }
            var spec = new OpticalSpectrum(targetWavelengths, outputIntensities, outputStdErr, outputStdDev);
            return spec;
        }
    }
}
