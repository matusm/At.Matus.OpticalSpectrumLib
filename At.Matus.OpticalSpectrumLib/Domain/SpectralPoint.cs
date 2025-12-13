using System;

namespace At.Matus.OpticalSpectrumLib
{
    public class SpectralPoint : ISpectralPoint, IComparable<SpectralPoint>
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

        public string ToCsvLine() => $"{Wavelength:F3},{Signal:F6},{StdErr:F6}";
        
        public string GetCsvHeader() => "Wavelength,Signal,StdErr";

        public int CompareTo(SpectralPoint other) => Wavelength.CompareTo(other.Wavelength);
    }
}
