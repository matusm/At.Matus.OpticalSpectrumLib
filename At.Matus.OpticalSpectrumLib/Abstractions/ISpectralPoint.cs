namespace At.Matus.OpticalSpectrumLib
{
    public interface ISpectralPoint
    {
        double Wavelength { get; }
        double Signal { get; }
        double StdErr { get; } // standard error of the mean (SEM) or standard uncertainty
        string ToCsvLine();
        string GetCsvHeader();
    }
}
