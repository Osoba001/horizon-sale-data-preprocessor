using HorizonPreProcessor.Api.DTOs;
using System.ComponentModel.DataAnnotations;

namespace HorizonPreProcessor.Api.Services
{
    public interface IDataParser
    {
        (DateTime purchasedOn, DateOnly purchasedOnDate) ParseDateTime(string? dateString);
    }
    public class DataParser : IDataParser
    {
        private static readonly string[] _exactFormats =
        {
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-dd HH:mm:ss",
        "MM/dd/yyyy HH:mm:ss",
        "dd/MM/yyyy HH:mm:ss",
        "yyyy-MM-dd",
        "MM/dd/yyyy",
        "dd/MM/yyyy"
    };

        public (DateTime purchasedOn, DateOnly purchasedOnDate) ParseDateTime(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return Fallback();

            if (DateTime.TryParseExact(
                dateString,
                _exactFormats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal |
                System.Globalization.DateTimeStyles.AdjustToUniversal,
                out var exactParsed))
            {
                return ConvertToResults(exactParsed);
            }

            if (DateTime.TryParse(
                dateString,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal |
                System.Globalization.DateTimeStyles.AdjustToUniversal,
                out var looselyParsed))
            {
                return ConvertToResults(looselyParsed);
            }


            return Fallback();
        }

        private static (DateTime, DateOnly) ConvertToResults(DateTime dt)
        {
            if (dt.Kind != DateTimeKind.Utc)
                dt = DateTime.SpecifyKind(dt.ToUniversalTime(), DateTimeKind.Utc);

            return (dt, DateOnly.FromDateTime(dt));
        }

        private static (DateTime, DateOnly) Fallback()
        {
            var now = DateTime.UtcNow;
            return (now, DateOnly.FromDateTime(now));
        }
    }

}