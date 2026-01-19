using System.IO;
using System.Text;

namespace At.Matus.OpticalSpectrumLib.Domain
{
    public static class FileTools
    {
        public static void SaveAsResultFile(this IOpticalSpectrum spectrum, string filePath, string commentPrefix = "# ", string delimiter = ",")
        {
            var content = spectrum.ToFriendlyString(commentPrefix, delimiter);
            File.WriteAllText(filePath, content);
        }

        public static void SaveAsSimpleCsvFile(this IOpticalSpectrum spectrum, string filePath, bool writeHeader = true)
        {
            StreamWriter csvFile = new StreamWriter(filePath);
            if (writeHeader)
                csvFile.WriteLine(spectrum.DataPoints[0].GetCsvHeader());
            foreach (ISpectralPoint item in spectrum.DataPoints)
            {
                csvFile.WriteLine(item.ToCsvLine());
            }
            csvFile.Close();
        }

        public static string ToFriendlyString(this IOpticalSpectrum spectrum, string commentPrefix = "# ", string delimiter = ",")
        {
            var sb = new StringBuilder();
            // metadata
            foreach (var record in spectrum.MetaData.Records)
            {
                sb.AppendLine($"{commentPrefix}{record.Key}: {record.Value}");
            }
            // data header
            sb.AppendLine($"{commentPrefix}Wavelength{delimiter}Signal");
            // data points
            foreach (var point in spectrum.DataPoints)
            {
                sb.AppendLine($"{point.Wavelength.ToString("F2")}{delimiter}{point.Signal.ToString("F2")}");
            }
            // final line
            sb.AppendLine($"{commentPrefix}End of Data");
            return sb.ToString();
        }

    }
}
