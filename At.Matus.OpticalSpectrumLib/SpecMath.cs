using System;

namespace At.Matus.OpticalSpectrumLib
{
    public static class SpecMath
    {

        public static OpticalSpectrum Subtract(IOpticalSpectrum minuend, IOpticalSpectrum subtrahend)
        {
            SpectralPoint[] newDataPoints = new SpectralPoint[minuend.NumberOfPoints];
            for (int i = 0; i < newDataPoints.Length; i++)
            {
                ISpectralPoint pointMin = minuend.DataPoints[i];
                ISpectralPoint pointSub = subtrahend.DataPoints[i];
                newDataPoints[i] = Subtract(pointMin, pointSub);
            }
            OpticalSpectrum diff = new OpticalSpectrum(newDataPoints);
            return diff;
        }

        public static OpticalSpectrum Add(IOpticalSpectrum first, IOpticalSpectrum second)
        {
            SpectralPoint[] newDataPoints = new SpectralPoint[first.NumberOfPoints];
            for (int i = 0; i < newDataPoints.Length; i++)
            {
                ISpectralPoint firstPoint = first.DataPoints[i];
                ISpectralPoint secondPoint = second.DataPoints[i];
                newDataPoints[i] = Add(firstPoint, secondPoint);
            }
            OpticalSpectrum diff = new OpticalSpectrum(newDataPoints);
            return diff;
        }

        public static OpticalSpectrum ComputeBiasCorrectedRatio(IOpticalSpectrum signal, IOpticalSpectrum reference, IOpticalSpectrum bckgnd)
        {
            SpectralPoint[] newDataPoints = new SpectralPoint[signal.NumberOfPoints];
            for (int i = 0; i < newDataPoints.Length; i++)
            {
                ISpectralPoint signalPoint = signal.DataPoints[i];
                ISpectralPoint referencePoint = reference.DataPoints[i];
                ISpectralPoint bckgndPoint = bckgnd.DataPoints[i];
                newDataPoints[i] = ComputeBiasCorrectedRatio(signalPoint, referencePoint, bckgndPoint);
            }
            OpticalSpectrum ratio = new OpticalSpectrum(newDataPoints);
            return ratio;
        }

        #region Private Methods
        private static SpectralPoint Subtract(ISpectralPoint minuend, ISpectralPoint subtrahend)
        {
            double newSignal = minuend.Signal - subtrahend.Signal;
            double newStdErr = SqSum(minuend.StdErr, subtrahend.StdErr);
            double newStdDev = SqSum(minuend.StdDev, subtrahend.StdDev);
            return new SpectralPoint(minuend.Wavelength, newSignal, newStdErr, newStdDev);
        }

        private static SpectralPoint Add(ISpectralPoint first, ISpectralPoint second)
        {
            double newSignal = first.Signal + second.Signal;
            double newStdErr = SqSum(first.StdErr, second.StdErr);
            double newStdDev = SqSum(first.StdDev, second.StdDev);
            return new SpectralPoint(first.Wavelength, newSignal, newStdErr, newStdDev);
        }

        private static SpectralPoint ComputeBiasCorrectedRatio(ISpectralPoint signal, ISpectralPoint reference, ISpectralPoint bckgnd)
        {
            double correctedSignal = signal.Signal - bckgnd.Signal;
            double correctedReference = reference.Signal - bckgnd.Signal;
            double ratio = correctedSignal / correctedReference;
            double newSem = BiasCorrectedRatioUncertainty(signal.Signal, reference.Signal, bckgnd.Signal, signal.StdErr, reference.StdErr, bckgnd.StdErr);
            double newStdDev = BiasCorrectedRatioUncertainty(signal.Signal, reference.Signal, bckgnd.Signal, signal.StdDev, reference.StdDev, bckgnd.StdDev);
            return new SpectralPoint(signal.Wavelength, ratio, newSem, newStdDev);
        }

        private static double SqSum(double u1, double u2) => Math.Sqrt(u1 * u1 + u2 * u2);

        private static double BiasCorrectedRatioUncertainty(double x, double xr, double xb, double ux, double uxr, double uxb)
        {
            double v1 = 1.0 / (xr - xb);
            double v2 = v1 * v1;
            double u1 = v1 * ux;
            double u2 = v2 * (x - xr) * uxb;
            double u3 = v2 * (xb - x) * uxr;
            return Math.Sqrt((u1 * u1) + (u2 * u2) + (u3 * u3));
        }

        #endregion

    }
}
