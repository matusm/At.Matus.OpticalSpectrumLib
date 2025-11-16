using System;
using System.Linq;

namespace At.Matus.OpticalSpectrumLib
{
    public class OpticalSpectrum : IOpticalSpectrum
    {
        public string Name { get; set; } = "Spectrum";
        public double[] Wavelengths => dataPoints.Select(dp => dp.Wavelength).ToArray();
        public double[] Signals => dataPoints.Select(dp => dp.Signal).ToArray();
        public double[] StdErrValues => dataPoints.Select(dp => dp.StdErr).ToArray();
        public double[] StdDevValues => dataPoints.Select(dp => dp.StdDev).ToArray();
        public ISpectralPoint[] DataPoints => dataPoints.Cast<ISpectralPoint>().ToArray();
        public int NumberOfPoints => dataPoints.Length;

        public double MaximumValue => GetMaximumValue();
        public double MinimumValue => GetMinimumValue();

        // constructor that copies data from another Spectrum or MeasuredSpectrum
        public OpticalSpectrum(IOpticalSpectrum spectrum)
        {
            Name = spectrum.Name;
            dataPoints = new SpectralPoint[spectrum.NumberOfPoints];
            for (int i = 0; i < spectrum.NumberOfPoints; i++)
            {
                ISpectralPoint dp = spectrum.DataPoints[i];
                dataPoints[i] = new SpectralPoint(dp.Wavelength, dp.Signal, dp.StdErr, dp.StdDev);
            }
        }

        public OpticalSpectrum(SpectralPoint[] dataPoints) => this.dataPoints = dataPoints;

        public OpticalSpectrum(double[] wavelength, double[] signals, double[] stdErrValues, double[] stdDevValues)
        {
            if (wavelength.Length != signals.Length || wavelength.Length != stdErrValues.Length || wavelength.Length != stdDevValues.Length)
            {
                throw new ArgumentException("All input arrays must have the same length.");
            }
            dataPoints = new SpectralPoint[wavelength.Length];
            for (int i = 0; i < wavelength.Length; i++)
            {
                dataPoints[i] = new SpectralPoint(wavelength[i], signals[i], stdErrValues[i], stdDevValues[i]);
            }
        }

        public override string ToString()
        {
            return $"{Name}: computed spectrum, MinSignal={MinimumValue:0.000}, MaxSignal={MaximumValue:0.000}";
        }

        private double GetMaximumValue()
        {
            StatisticPod.StatisticPod sp = new StatisticPod.StatisticPod();
            foreach (var dp in dataPoints)
            {
                sp.Update(dp.Signal);
            }
            return sp.MaximumValue;
        }

        private double GetMinimumValue()
        {
            StatisticPod.StatisticPod sp = new StatisticPod.StatisticPod();
            foreach (var dp in dataPoints)
            {
                sp.Update(dp.Signal);
            }
            return sp.MinimumValue;
        }

        private readonly SpectralPoint[] dataPoints;
    }
}
