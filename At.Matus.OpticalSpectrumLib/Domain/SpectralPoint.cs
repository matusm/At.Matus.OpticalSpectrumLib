namespace At.Matus.OpticalSpectrumLib
{
    public class SpectralPoint : ISpectralPoint
    {
        public double Wavelength { get; }
        public double Signal { get; }
        public double StdErr { get; }

        public SpectralPoint(double wavelength, double signal) : this(wavelength, signal, 0) { }

        public SpectralPoint(double wavelength, double signal, double stdErr)
        {
            Wavelength = wavelength;
            Signal = signal;
            StdErr = stdErr;
        }

        public string ToCsvLine() => $"{Wavelength},{Signal},{StdErr}";
        public string GetCsvHeader() => "Wavelength,Signal,StdErr";
    }
}
