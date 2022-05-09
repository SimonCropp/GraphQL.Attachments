using System.Buffers;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Json converter for reading and writing <see cref="BigInteger"/> values.
/// While it is not able to correctly write very large numbers.
/// </summary>
sealed class JsonConverterBigInteger : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => TryGetBigInteger(ref reader, out var bi) ? bi : throw new JsonException();

    /// <summary>
    /// Attempts to read a <see cref="BigInteger"/> value from a <see cref="Utf8JsonReader"/>.
    /// </summary>
    public static bool TryGetBigInteger(ref Utf8JsonReader reader, out BigInteger bi)
    {
        var byteSpan = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
        Span<char> chars = stackalloc char[byteSpan.Length];
        Encoding.UTF8.GetChars(reader.ValueSpan, chars);
        return BigInteger.TryParse(chars, out bi);
    }

    static BigInteger maxBigInteger = (BigInteger)decimal.MaxValue;
    static BigInteger minBigInteger = (BigInteger)decimal.MinValue;

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        if (minBigInteger <= value && value <= maxBigInteger)
        {
            writer.WriteNumberValue((decimal)value);
            return;
        }

        // https://stackoverflow.com/questions/64788895/serialising-biginteger-using-system-text-json
        var s = value.ToString(NumberFormatInfo.InvariantInfo);
        using var doc = JsonDocument.Parse(s);
        doc.WriteTo(writer);
    }
}