using System;

namespace At.Matus.OpticalSpectrumLib
{
    public class MeasuredSpectralPoint : ISpectralPoint
    {
        public double Wavelength { get; }
        public double Signal => sp.AverageValue;
        public double StdErr => StdDev / Math.Sqrt(SampleSize);
        public double StdDev => sp.StandardDeviation;
        public double MaxSignal => sp.MaximumValue;
        public double MinSignal => sp.MinimumValue;
        public int SampleSize => (int)sp.SampleSize;

        public MeasuredSpectralPoint(double wavelength) => Wavelength = wavelength;

        public void UpdateSignal(double value) => sp.Update(value);

        public void Clear() => sp.Restart();

        public string ToCsvLine() => $"{Wavelength:F2},{Signal:F6},{MinSignal:F6},{MaxSignal:F6},{StdErr:F6},{StdDev:F6},{SampleSize}";

        public string GetCsvHeader() => "Wavelength,Signal,MinSignal,MaxSignal,SEM,StdDev,Samplesize";

        private readonly StatisticPod.StatisticPod sp = new StatisticPod.StatisticPod();

    }
}
