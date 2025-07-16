using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace gestionale_scalzo.Utils
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string ReadFormats = "dd/MM/yyyy";  // se devi ancora leggere italiano
        private const string WriteFormat = "yyyy-MM-dd";  // per emettere ISO

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString() ?? throw new JsonException("Expected date string");
            // prova prima italiano
            if (DateOnly.TryParseExact(s, ReadFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                return d;
            // fallback nativo ISO
            if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return d;
            throw new JsonException($"Invalid date format. Expected '{ReadFormats}' or ISO.");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            // qui cambio in ISO corto
            writer.WriteStringValue(value.ToString(WriteFormat));
        }
    }

    public class NullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
    {
        private const string ReadFormats = "dd/MM/yyyy";
        private const string WriteFormat = "yyyy-MM-dd";

        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (DateOnly.TryParseExact(s, ReadFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                return d;
            if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return d;
            throw new JsonException($"Invalid date format. Expected '{ReadFormats}' or ISO.");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString(WriteFormat));
            else
                writer.WriteNullValue();
        }
    }

}
