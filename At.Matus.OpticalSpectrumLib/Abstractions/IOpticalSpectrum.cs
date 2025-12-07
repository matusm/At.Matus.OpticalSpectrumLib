using At.Matus.MetaData;

namespace At.Matus.OpticalSpectrumLib
{
    public interface IOpticalSpectrum
    {
        MeasurementMetaData MetaData { get; }
        double[] Wavelengths { get; }
        double[] Signals { get; }
        double[] StdErrValues { get; }
        ISpectralPoint[] DataPoints { get; }
        int NumberOfPoints { get; }
        double MaximumSignal { get; }
        double MinimumSignal { get; }

        void AddMetaDataRecord(string key, string value);
    }
}
