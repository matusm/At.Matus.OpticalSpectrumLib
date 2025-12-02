using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace At.Matus.OpticalSpectrumLib
{
    public static class SpecReg
    {
        public static OpticalSpectrum RegularizeSpectrum(IOpticalSpectrum inputSpec, double startWavelength, double endWavelength, double step)
        {
            double[] targetWavelengths = CreateEquidistantWavelengths(startWavelength, endWavelength, step);
            return RegularizeSpectrum(inputSpec, targetWavelengths);
        }

        public static OpticalSpectrum RegularizeSpectrum(IOpticalSpectrum inputSpec, double[] targetWavelengths)
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

        // the array must be sorted in ascending order
        public static (int smallerIndex, int largerIndex) FindNeighbors(double[] arr, double target)
        {
            if (arr.Length == 0)
                return (-1, -1);
            int smallerIndex = -1;
            int largerIndex = -1;
            int low = 0;
            int high = arr.Length - 1;
            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                if (arr[mid] == target)
                {
                    smallerIndex = (mid > 0) ? mid - 1 : -1;
                    largerIndex = (mid < arr.Length - 1) ? mid + 1 : -1;
                    return (smallerIndex, largerIndex);
                }
                else if (arr[mid] < target)
                {
                    smallerIndex = mid;
                    low = mid + 1;
                }
                else
                {
                    largerIndex = mid;
                    high = mid - 1;
                }
            }
            return (smallerIndex, largerIndex);
        }

        public static double LinearInterpolate(double x0, double y0, double x1, double y1, double x)
        {
            if (x1 == x0)
            {
                if (x0 == x)
                {
                    return (y0 + y1) / 2;
                }
                throw new ArgumentException("x0 and x1 cannot be the same value.");
            }
            return y0 + (y1 - y0) * (x - x0) / (x1 - x0);
        }

        private static double[] CreateEquidistantWavelengths(double startWavelength, double endWavelength, double step)
        {
            if (step <= 0)
            {
                throw new ArgumentException("Step must be a positive value.");
            }
            int numPoints = (int)Math.Floor((endWavelength - startWavelength) / step) + 1;
            double[] wavelengths = new double[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                wavelengths[i] = startWavelength + i * step;
            }
            return wavelengths;
        }

    }
}
