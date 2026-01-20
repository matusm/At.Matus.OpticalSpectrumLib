using At.Matus.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace At.Matus.OpticalSpectrumLib
{
    public class OpticalSpectrum : IOpticalSpectrum
    {
        public MeasurementMetaData MetaData => metaData;
        public double[] Wavelengths => dataPoints.Select(dp => dp.Wavelength).ToArray();
        public double[] Signals => dataPoints.Select(dp => dp.Signal).ToArray();
        public double[] StdErrValues => dataPoints.Select(dp => dp.StdErr).ToArray();
        public ISpectralPoint[] DataPoints => dataPoints.Cast<ISpectralPoint>().ToArray();
        public int NumberOfPoints => dataPoints.Length;
        public double MaximumSignal => this.GetMaximumSignal();
        public double MinimumSignal => this.GetMinimumSignal();

        // constructor that copies data from another Spectrum or MeasuredSpectrum
        public OpticalSpectrum(IOpticalSpectrum spectrum)
        {
            metaData.AddRecords(spectrum.MetaData);
            metaData.AddRecord("Type", "OpticalSpectrumCopied");
            dataPoints = new SpectralPoint[spectrum.NumberOfPoints];
            for (int i = 0; i < spectrum.NumberOfPoints; i++)
            {
                ISpectralPoint dp = spectrum.DataPoints[i];
                dataPoints[i] = new SpectralPoint(dp.Wavelength, dp.Signal, dp.StdErr);
            }
        }

        public OpticalSpectrum(SpectralPoint[] dataPoints)
        {
            this.dataPoints = dataPoints;
            metaData.AddRecord("Type", "OpticalSpectrumFromDataPoints");
        }

        public OpticalSpectrum(double[] wavelength, double[] signals, double[] stdErrValues)
        {
            if (wavelength.Length != signals.Length || wavelength.Length != stdErrValues.Length)
            {
                throw new ArgumentException("All input arrays must have the same length.");
            }
            dataPoints = new SpectralPoint[wavelength.Length];
            for (int i = 0; i < wavelength.Length; i++)
            {
                dataPoints[i] = new SpectralPoint(wavelength[i], signals[i], stdErrValues[i]);
            }
            metaData.AddRecord("Type", "OpticalSpectrumFromArrays");
        }

        public void AddMetaDataRecord(string key, string value)
        {
            metaData.AddRecord(key, value);
        }

        public void AddMetaDataRecord(Dictionary<string, string> records)
        {
            foreach (var record in records)
            {
                AddMetaDataRecord(record.Key, record.Value);
            }
        }

        public void AddMetaDataRecordsWithPrefix(string prefix, MeasurementMetaData records)
        {
                metaData.AddRecordsWithPrefix(prefix, records);
        }

        public void DeleteMetaDataRecords()
        {
            metaData = new MeasurementMetaData();
        }

        private readonly SpectralPoint[] dataPoints;
        private MeasurementMetaData metaData = new MeasurementMetaData();
    }
}
