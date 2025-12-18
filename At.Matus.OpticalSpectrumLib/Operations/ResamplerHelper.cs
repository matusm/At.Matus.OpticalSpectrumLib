using System;
using System.Collections.Generic;
using System.Linq;

namespace At.Matus.OpticalSpectrumLib
{
    public static partial class Resampler
    {
        private static double[] SortAndClipArray(double min, double max, double[] arr)
        {
            List<double> temp = new List<double>();
            foreach (double val in arr)
                if (val >= min && val <= max)
                    temp.Add(val);
            return temp.OrderBy(wl => wl).ToArray();
        }

        // the array must be sorted in ascending order
        private static (int smallerIndex, int largerIndex) FindNeighbors(double[] arr, double target)
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

        private static double LinearInterpolate(double x0, double y0, double x1, double y1, double x)
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
