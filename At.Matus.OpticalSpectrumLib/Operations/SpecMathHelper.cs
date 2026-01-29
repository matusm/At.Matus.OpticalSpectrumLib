namespace At.Matus.OpticalSpectrumLib
{
    public static partial class SpecMath
    {
        public static OpticalSpectrum Average(IOpticalSpectrum spec1, IOpticalSpectrum spec2)
        {
            EnsureCompatibility(spec1, spec2);
            return Add(spec1, spec2).Scale(0.5); // The standard error is underestimated here (considered uncorrelated)
        }
    }
}
