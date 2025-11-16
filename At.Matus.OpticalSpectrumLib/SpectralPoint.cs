namespace At.Matus.OpticalSpectrumLib
{
    public class SpectralPoint : ISpectralPoint
    {
        public double Wavelength { get; }
        public double Signal { get; }
        public double StdErr { get; }
        public double StdDev { get; }

        public SpectralPoint(double wavelength, double signal) : this(wavelength, signal, 0, 0) { }

        public SpectralPoint(double wavelength, double signal, double stdErr, double stdDev)
        {
            Wavelength = wavelength;
            Signal = signal;
            StdErr = stdErr;
            StdDev = stdDev;
        }

        public string ToCsvLine() => $"{Wavelength:F2},{Signal:F6},{StdErr:F6},{StdDev:F6}";
        public string GetCsvHeader() => "Wavelength,Signal,SEM,StdDev";
    }
}
