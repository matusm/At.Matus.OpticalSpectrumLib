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
            diff.AddMetaDataRecord("Origin", "SpecMathSubtract");
            diff.AddMetaDataRecordsWithPrefix("Minuend_", minuend.MetaData);
            diff.AddMetaDataRecordsWithPrefix("Subtrahend_", subtrahend.MetaData);
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
            diff.AddMetaDataRecord("Origin", "SpecMathAdd");
            diff.AddMetaDataRecordsWithPrefix("First_", first.MetaData);
            diff.AddMetaDataRecordsWithPrefix("Second_", second.MetaData);
            return diff;
        }

        public static OpticalSpectrum Multiply(IOpticalSpectrum first, IOpticalSpectrum second)
        {
            SpectralPoint[] newDataPoints = new SpectralPoint[first.NumberOfPoints];
            for (int i = 0; i < newDataPoints.Length; i++)
            {
                ISpectralPoint firstPoint = first.DataPoints[i];
                ISpectralPoint secondPoint = second.DataPoints[i];
                newDataPoints[i] = Multiply(firstPoint, secondPoint);
            }
            OpticalSpectrum productSpectrum = new OpticalSpectrum(newDataPoints);
            productSpectrum.AddMetaDataRecord("Origin", "SpecMathMultiply");
            productSpectrum.AddMetaDataRecordsWithPrefix("First_", first.MetaData);
            productSpectrum.AddMetaDataRecordsWithPrefix("Second_", second.MetaData);
            return productSpectrum;
        }

        public static OpticalSpectrum Ratio(IOpticalSpectrum numerator, IOpticalSpectrum denominator)
        {
            SpectralPoint[] newDataPoints = new SpectralPoint[numerator.NumberOfPoints];
            for (int i = 0; i < newDataPoints.Length; i++)
            {
                ISpectralPoint numPoint = numerator.DataPoints[i];
                ISpectralPoint denPoint = denominator.DataPoints[i];
                newDataPoints[i] = Ratio(numPoint, denPoint);
            }
            OpticalSpectrum ratioSpectrum = new OpticalSpectrum(newDataPoints);
            ratioSpectrum.AddMetaDataRecord("Origin", "SpecMathRatio");
            ratioSpectrum.AddMetaDataRecordsWithPrefix("Numerator_", numerator.MetaData);
            ratioSpectrum.AddMetaDataRecordsWithPrefix("Denominator_", denominator.MetaData);
            return ratioSpectrum;
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
            ratio.AddMetaDataRecord("Origin", "SpecMathComputeBiasCorrectedRatio");
            ratio.AddMetaDataRecordsWithPrefix("Signal_", signal.MetaData);
            ratio.AddMetaDataRecordsWithPrefix("Reference_", reference.MetaData);
            ratio.AddMetaDataRecordsWithPrefix("Bckgnd_", bckgnd.MetaData);
            return ratio;
        }

        #region Private Methods
        private static SpectralPoint Subtract(ISpectralPoint minuend, ISpectralPoint subtrahend)
        {
            double newSignal = minuend.Signal - subtrahend.Signal;
            double newStdErr = SqSum(minuend.StdErr, subtrahend.StdErr);
            return new SpectralPoint(minuend.Wavelength, newSignal, newStdErr);
        }

        private static SpectralPoint Add(ISpectralPoint first, ISpectralPoint second)
        {
            double newSignal = first.Signal + second.Signal;
            double newStdErr = SqSum(first.StdErr, second.StdErr);
            return new SpectralPoint(first.Wavelength, newSignal, newStdErr);
        }

        private static SpectralPoint Multiply(ISpectralPoint first, ISpectralPoint second)
        {
            double newSignal = first.Signal * second.Signal;
            double newStdErr = SqSum(second.Signal*first.StdErr, first.Signal*second.StdErr);
            return new SpectralPoint(first.Wavelength, newSignal, newStdErr);
        }

        private static SpectralPoint Ratio(ISpectralPoint nominator, ISpectralPoint denominator)
        {
            double newSignal = nominator.Signal / denominator.Signal;
            double newStdErr = RatioUncertainty(nominator.Signal, denominator.Signal, nominator.StdErr, denominator.StdErr);
            newSignal = FixNaN(newSignal);
            newStdErr = FixNaN(newStdErr);
            return new SpectralPoint(nominator.Wavelength, newSignal, newStdErr);
        }

        private static SpectralPoint ComputeBiasCorrectedRatio(ISpectralPoint signal, ISpectralPoint reference, ISpectralPoint bckgnd)
        {
            double correctedSignal = signal.Signal - bckgnd.Signal;
            double correctedReference = reference.Signal - bckgnd.Signal;
            double ratio = correctedSignal / correctedReference;
            double newSem = BiasCorrectedRatioUncertainty(signal.Signal, reference.Signal, bckgnd.Signal, signal.StdErr, reference.StdErr, bckgnd.StdErr);
            ratio = FixNaN(ratio);
            newSem = FixNaN(newSem);
            return new SpectralPoint(signal.Wavelength, ratio, newSem);
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

        private static double RatioUncertainty(double x, double xr, double ux, double uxr)
        {
            double u1 = ux / xr;
            double u2 = x * uxr / (xr * xr);
            return Math.Sqrt((u1 * u1) + (u2 * u2));
        }

        private static double FixNaN(double value)
        {
            double replaceValue = 0; // set to 'value' for no operation
            if (double.IsInfinity(value)) return replaceValue;
            if (double.IsNaN(value)) return replaceValue;
            return value;
        }

        #endregion

    }
}
