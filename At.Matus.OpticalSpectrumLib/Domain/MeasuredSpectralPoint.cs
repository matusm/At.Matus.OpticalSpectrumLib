using System;

namespace At.Matus.OpticalSpectrumLib
{
    public class MeasuredSpectralPoint : ISpectralPoint, IComparable<MeasuredSpectralPoint>
    {

        public double Wavelength { get; }
        public double Signal => sp.AverageValue;
        public double StdErr => sp.StandardDeviation / Math.Sqrt(SampleSize);
        public double MaxSignal => sp.MaximumValue;
        public double MinSignal => sp.MinimumValue;
        public int SampleSize => (int)sp.SampleSize;

        public MeasuredSpectralPoint(double wavelength) => Wavelength = wavelength;

        public void UpdateSignal(double value) => sp.Update(value);

        public void Clear() => sp.Restart();

        public string ToCsvLine() => $"{Wavelength},{Signal},{MinSignal},{MaxSignal},{StdErr},{SampleSize}";

        public string GetCsvHeader() => "Wavelength,Signal,MinSignal,MaxSignal,StdErr,Samplesize";

        private readonly StatisticPod.StatisticPod sp = new StatisticPod.StatisticPod();

        public int CompareTo(MeasuredSpectralPoint other) => Wavelength.CompareTo(other.Wavelength);
    }
}
