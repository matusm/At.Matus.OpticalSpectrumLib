using At.Matus.MetaData;
using System;
using System.Linq;

namespace At.Matus.OpticalSpectrumLib
{
    public class MeasuredOpticalSpectrum : IOpticalSpectrum
    {
        public MeasurementMetaData MetaData => metaData;
        public double[] Wavelengths => dataPoints.Select(dp => dp.Wavelength).ToArray();
        public double[] Signals => dataPoints.Select(dp => dp.Signal).ToArray();
        public double[] StdErrValues => dataPoints.Select(dp => dp.StdErr).ToArray();
        public double[] StdDevValues => dataPoints.Select(dp => dp.StdDev).ToArray();
        public int[] SampleSize => dataPoints.Select(dp => dp.SampleSize).ToArray();
        public double[] MaxValues => dataPoints.Select(dp => dp.MaxSignal).ToArray();
        public double[] MinValues => dataPoints.Select(dp => dp.MinSignal).ToArray();
        public ISpectralPoint[] DataPoints => dataPoints.Cast<ISpectralPoint>().ToArray();
        public double MaximumSignal => this.GetMaximumSignal();
        public double MinimumSignal => this.GetMinimumSignal();
        public int NumberOfSpectra => dataPoints[0].SampleSize;
        public int NumberOfPoints => dataPoints.Length; // this might be not correct if some points are NaN        public bool IsEmpty => NumberOfSpectra == 0;

        public MeasuredOpticalSpectrum(double[] wavelength)
        {
            dataPoints = wavelength.Select(w => new MeasuredSpectralPoint(w)).ToArray();
            metaData.AddRecord("Type", "MeasuredOpticalSpectrum");
        }

        public void UpdateSignal(double[] values)
        {
            if (values.Length != dataPoints.Length) throw new ArgumentException("Signal array length does not match data points length.");
            for (int i = 0; i < values.Length; i++)
            {
                dataPoints[i].UpdateSignal(values[i]);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < dataPoints.Length; i++)
            {
                dataPoints[i].Clear();
            }
        }

        public void AddMetaDataRecord(string key, string value)
        {
            metaData.AddRecord(key, value);
        }

        private readonly MeasuredSpectralPoint[] dataPoints;
        private readonly MeasurementMetaData metaData = new MeasurementMetaData();
    }
}
