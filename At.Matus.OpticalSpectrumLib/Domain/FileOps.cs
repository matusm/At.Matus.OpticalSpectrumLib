using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace At.Matus.OpticalSpectrumLib.Domain
{
    public static class FileOps
    {
        public static string ToFriendlyString(this IOpticalSpectrum spectrum)
        {
            if (spectrum == null)
            {
                return "Null Optical Spectrum";
            }
            var sb = new StringBuilder();
            sb.AppendLine("Optical Spectrum:");
            sb.AppendLine($"  Number of Points: {spectrum.NumberOfPoints}");
            sb.AppendLine($"  Wavelength Range: {spectrum.Wavelengths.First()} nm to {spectrum.Wavelengths.Last()} nm");
            sb.AppendLine($"  Signal Range: {spectrum.MinimumSignal} to {spectrum.MaximumSignal}");
            sb.AppendLine("  MetaData:");
            foreach (var record in spectrum.MetaData.Records)
            {
                sb.AppendLine($"    {record.Key}: {record.Value}");
            }
            return sb.ToString();
        }
    }
}
