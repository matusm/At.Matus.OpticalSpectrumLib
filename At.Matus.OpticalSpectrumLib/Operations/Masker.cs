using System;

namespace At.Matus.OpticalSpectrumLib
{
    public enum TransitionType
    {
        Linear,
        Quadratic,
        Cubic,
        Quintic
    }

    public static class Masker
    {
        public static OpticalSpectrum ApplyShortpassMask(this IOpticalSpectrum spectrum, double cutoff, double hw, TransitionType transitionType = TransitionType.Linear)
        {
            double[] mask = GetShortpassMask(spectrum.Wavelengths, cutoff, hw, transitionType);
            var weightedSpectrum = Weighting(spectrum, mask);
            weightedSpectrum.AddMetaDataRecord("MaskType", "Shortpass");
            weightedSpectrum.AddMetaDataRecord("CutoffWavelength", cutoff.ToString());
            weightedSpectrum.AddMetaDataRecord("TransitionWidth", hw.ToString());
            weightedSpectrum.AddMetaDataRecord("TransitionType", transitionType.ToString());
            return weightedSpectrum;
        }

        public static OpticalSpectrum ApplyLongpassMask(this IOpticalSpectrum spectrum, double cutoff, double hw, TransitionType transitionType = TransitionType.Linear)
        {
            double[] mask = GetLongpassMask(spectrum.Wavelengths, cutoff, hw, transitionType);
            var weightedSpectrum = Weighting(spectrum, mask);
            weightedSpectrum.AddMetaDataRecord("MaskType", "Longpass");
            weightedSpectrum.AddMetaDataRecord("CutoffWavelength", cutoff.ToString());
            weightedSpectrum.AddMetaDataRecord("TransitionWidth", hw.ToString());
            weightedSpectrum.AddMetaDataRecord("TransitionType", transitionType.ToString());
            return weightedSpectrum;
        }

        // the user must ensure that cutoffLow < cutoffHigh and the transition widths do not overlap
        public static OpticalSpectrum ApplyBandpassMask(this IOpticalSpectrum spectrum, double cutoffLow, double cutoffHigh, double hwLow, double hwHigh, TransitionType transitionType = TransitionType.Linear)
        {
            double[] mask = GetBandpassMask(spectrum.Wavelengths, cutoffLow, cutoffHigh, hwLow, hwHigh, transitionType);
            var weightedSpectrum = Weighting(spectrum, mask);
            weightedSpectrum.AddMetaDataRecord("MaskType", "Bandpass");
            weightedSpectrum.AddMetaDataRecord("CutoffLowWavelength", cutoffLow.ToString());
            weightedSpectrum.AddMetaDataRecord("CutoffHighWavelength", cutoffHigh.ToString());
            weightedSpectrum.AddMetaDataRecord("TransitionLowWidth", hwLow.ToString());
            weightedSpectrum.AddMetaDataRecord("TransitionHighWidth", hwHigh.ToString());
            weightedSpectrum.AddMetaDataRecord("TransitionType", transitionType.ToString());
            return weightedSpectrum;
        }

        public static OpticalSpectrum Weighting(this IOpticalSpectrum spectrum, double[] weights)
        {
            if (spectrum.NumberOfPoints != weights.Length)
                throw new ArgumentException("Spectrum and weights must have the same number of points.");

            SpectralPoint[] newDataPoints = new SpectralPoint[spectrum.NumberOfPoints];
            for (int i = 0; i < newDataPoints.Length; i++)
            {
                ISpectralPoint point = spectrum.DataPoints[i];
                newDataPoints[i] = new SpectralPoint(point.Wavelength, point.Signal * weights[i], point.StdErr * weights[i]);
            }
            OpticalSpectrum weightedSpectrum = new OpticalSpectrum(newDataPoints);
            return weightedSpectrum;
        }

        private static double[] GetLongpassMask(double[] wavelengths, double cutoff, double hw, TransitionType transitionType)
        {
            double[] mask = new double[wavelengths.Length];
            for (int i = 0; i < wavelengths.Length; i++)
            {
                double w = wavelengths[i] - cutoff;
                switch (transitionType)
                {
                    case TransitionType.Linear:
                        mask[i] = SigmoidLinear(w, hw);
                        break;
                    case TransitionType.Quadratic:
                        mask[i] = SigmoidQuadratic(w, hw);
                        break;
                    case TransitionType.Cubic:
                        mask[i] = SigmoidCubic(w, hw);
                        break;
                    case TransitionType.Quintic:
                        mask[i] = SigmoidQuintic(w, hw);
                        break;
                }
            }
            return mask;
        }

        private static double[] GetShortpassMask(double[] wavelengths, double cutoff, double hw, TransitionType transitionType)
        {
            return InvertMask(GetLongpassMask(wavelengths, cutoff, hw, transitionType));
        }

        // the user must ensure that cutoffLow < cutoffHigh and the transition widths do not overlap
        private static double[] GetBandpassMask(double[] wavelengths, double cutoffLow, double cutoffHigh, double hwLow, double hwHigh, TransitionType transitionType)
        {
            double[] lowPass = GetLongpassMask(wavelengths, cutoffLow, hwLow, transitionType);
            double[] highPass = GetShortpassMask(wavelengths, cutoffHigh, hwHigh, transitionType);
            return CombineMasks(lowPass, highPass);
        }

        private static double[] InvertMask(double[] f)
        {
            double[] mask = new double[f.Length];
            for (int i = 0; i < f.Length; i++)
            {
                mask[i] = 1.0 - f[i];
            }
            return mask;
        }

        private static double[] CombineMasks(double[] f1, double[] f2)
        {
            if (f1.Length != f2.Length)
                throw new ArgumentException("Filter lengths do not match");
            double[] mask = new double[f1.Length];
            for (int i = 0; i < f1.Length; i++)
            {
                mask[i] = f1[i] * f2[i];
            }
            return mask;
        }

        private static double SigmoidLinear(double w, double hw)
        {
            double m0 = 0.5 + (w / hw) * 0.5;
            return Clamp(m0);
        }

        private static double SigmoidQuadratic(double w, double hw)
        {
            double m0 = 0.0;
            if (w < -hw) m0 = 0.0;
            if (w > hw) m0 = 1.0;
            if (w >= -hw && w < 0)
            {
                m0 = w * w * 0.5 / (hw * hw) + w / hw + 0.5;
            }
            if (w >= 0 && w <= hw)
            {
                m0 = -w * w * 0.5 / (hw * hw) + w / hw + 0.5;
            }
            return Clamp(m0);
        }

        private static double SigmoidCubic(double w, double hw)
        {
            double m0 = 0.0;
            if (w <= -hw) m0 = 0.0;
            if (w >= hw) m0 = 1.0;
            if (w > -hw && w < hw)
            {
                m0 = w / (4 * hw * hw * hw) * (3 * hw * hw - w * w) + 0.5;
            }
            return Clamp(m0);
        }

        private static double SigmoidQuintic(double w, double hw)
        {
            double m0 = 0.0;
            if (w <= -hw) m0 = 0.0;
            if (w >= hw) m0 = 1.0;
            if (w > -hw && w < hw)
            {
                m0 = w / (16 * hw * hw * hw * hw * hw) * (3 * w * w * w * w - 10 * w * w * hw * hw + 15 * hw * hw * hw * hw) + 0.5;
            }
            return Clamp(m0);
        }

        private static double Clamp(double m0)
        {
            if (m0 < 0) m0 = 0.0;
            if (m0 > 1) m0 = 1.0;
            return m0;
        }
    }
}
