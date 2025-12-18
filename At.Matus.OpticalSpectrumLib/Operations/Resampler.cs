using System;
using System.Collections.Generic;
using System.Linq;

namespace At.Matus.OpticalSpectrumLib
{
    public static partial class Resampler
    {
        // overload that creates equidistant target wavelengths and uses them for resampling
        public static OpticalSpectrum ResampleSpectrum(this IOpticalSpectrum inputSpec, double startWavelength, double endWavelength, double step)
        {
            double[] targetWavelengths = CreateEquidistantWavelengths(startWavelength, endWavelength, step);
            return ResampleSpectrum(inputSpec, targetWavelengths);
        }

        // overload that uses a list of target wavelengths for resampling
        public static OpticalSpectrum ResampleSpectrum(this IOpticalSpectrum inputSpec, List<double> unsortedTargetWavelengths)
        {
            return ResampleSpectrum(inputSpec, unsortedTargetWavelengths.ToArray()); // sorting is performed in the core method
        }

        // overload that uses the domain of another spectrum as target
        public static OpticalSpectrum ResampleSpectrum(this IOpticalSpectrum inputSpec, IOpticalSpectrum domainSpec)
        {
            double[] targetWavelengths = domainSpec.Wavelengths;
            return ResampleSpectrum(inputSpec, targetWavelengths);
        }

        // core resampling method that performs linear interpolation
        public static OpticalSpectrum ResampleSpectrum(this IOpticalSpectrum inputSpec, double[] targetWavelengths)
        {
            double minWl = inputSpec.GetMinimumWavelength();
            double maxWl = inputSpec.GetMaximumWavelength();
            double[] sortedTargetWavelengths = SortAndClipArray(minWl, maxWl, inputSpec.Wavelengths);
            double[] inputWavelengths = inputSpec.Wavelengths;
            double[] inputIntensities = inputSpec.Signals;
            double[] inputStdErr = inputSpec.StdErrValues;
            double[] outputIntensities = new double[sortedTargetWavelengths.Length];
            double[] outputStdErr = new double[sortedTargetWavelengths.Length];
            for (int i = 0; i < sortedTargetWavelengths.Length; i++)
            {
                double targetWl = sortedTargetWavelengths[i];
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
                }
            }
            OpticalSpectrum spec = new OpticalSpectrum(targetWavelengths, outputIntensities, outputStdErr);
            return spec;
        }
    }
}
