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
    }
}
