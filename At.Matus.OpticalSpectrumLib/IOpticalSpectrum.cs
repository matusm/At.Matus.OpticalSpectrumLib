namespace At.Matus.OpticalSpectrumLib
{
    public interface IOpticalSpectrum
    {
        string Name { get; set; }
        double[] Wavelengths { get; }
        double[] Signals { get; }
        double[] StdErrValues { get; }
        double[] StdDevValues { get; }
        ISpectralPoint[] DataPoints { get; }
        int NumberOfPoints { get; }
        double MaximumSignal { get; }
        double MinimumSignal { get; }
    }
}
