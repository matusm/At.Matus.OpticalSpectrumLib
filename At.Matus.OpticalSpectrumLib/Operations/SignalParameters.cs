namespace At.Matus.OpticalSpectrumLib
{
    public static class SignalParameters
    {
        public static StatisticPod.StatisticPod GetSignalStatistics(this IOpticalSpectrum spectrum)
        {
            var stats = new StatisticPod.StatisticPod();
            foreach (var dp in spectrum.DataPoints)
                stats.Update(dp.Signal);
            return stats;
        }

        public static double GetMaximumSignal(this IOpticalSpectrum spectrum)
        {
           return spectrum.GetSignalStatistics().MaximumValue;
        }

        public static double GetMinimumSignal(this IOpticalSpectrum spectrum)
        {
            return spectrum.GetSignalStatistics().MinimumValue;
        }

        public static double GetMinimumWavelength(this IOpticalSpectrum spectrum)
        {
            if (spectrum.DataPoints.Length == 0)
                return double.NaN;
            return spectrum.DataPoints[0].Wavelength;
        }

        public static double GetMaximumWavelength(this IOpticalSpectrum spectrum)
        {
            if (spectrum.DataPoints.Length == 0)
                return double.NaN;
            return spectrum.DataPoints[spectrum.DataPoints.Length - 1].Wavelength;
        }

    }
}
