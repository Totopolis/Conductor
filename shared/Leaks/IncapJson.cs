using ErrorOr;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Leaks;

public sealed class IncapJson : IEquatable<IncapJson>
{
    private static readonly IncapJson _empty = IncapJson.CreateOrThrow("{}");

    // TODO: use safe TextEncoderSettings()
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false,
        Converters = { new SortedJsonObjectConverter() }
    };

    private IncapJson(string body)
    {
        Body = body;

        if (this == Empty || Body == "{}")
        {
            HashCode = 0;
        }
        else
        {
            HashCode = Body.GetHashCode();
        }
    }

    public static IncapJson Empty => _empty;

    public string Body { get; init; }

    public int HashCode { get; init; }

    public static ErrorOr<IncapJson> CreateOrError(
        IReadOnlyDictionary<string, object> pairs)
    {
        if (!pairs.Select(x => x.Value.GetType()).All(IsSupported))
        {
            return Error.Validation(
                code: "Leaks.IncapJson.BadParamType",
                description: "Unsupported param type");
        }

        pairs = pairs.OrderBy(x => x.Key).ToDictionary();

        try
        {
            var body = JsonSerializer.Serialize(pairs, _options);
            var canonicalBody = ToCanonicalJson(body);
            var obj = new IncapJson(canonicalBody);
            return obj;
        }
        catch (JsonException)
        {
            return Error.Unexpected(
                code: "Leaks.IncapJson.UnableCreate",
                description: "Some error on build incapsulated json");
        }
    }

    public static IncapJson CreateOrThrow(string body)
    {
        var canonicalBody = ToCanonicalJson(body);
        var obj = new IncapJson(canonicalBody);
        return obj;
    }

    public override int GetHashCode() => HashCode;

    public override bool Equals(object? obj)
    {
        return Equals(obj as IncapJson);
    }

    public bool Equals(IncapJson? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (HashCode != other.HashCode)
        {
            return false;
        }

        using JsonDocument otherDoc = JsonDocument.Parse(other.Body);
        using JsonDocument thisDoc = JsonDocument.Parse(this.Body);

        JsonElement otherRoot = otherDoc.RootElement;
        JsonElement thisRoot = thisDoc.RootElement;

        return JsonElement.DeepEquals(otherRoot, thisRoot);
    }

    private static bool IsSupported(Type type) =>
        type == typeof(int) ||
        type == typeof(double) ||
        type == typeof(Guid) ||
        type == typeof(string) ||
        type == typeof(bool) ||
        type == typeof(int[]) ||
        type == typeof(DateOnly);

    private static string ToCanonicalJson(string json)
    {
        var node = JsonNode.Parse(json);

        if (node is null)
        {
            throw new JsonException("Unexpected json body");
        }

        return node.ToJsonString(_options);
    }

    private sealed class SortedJsonObjectConverter : JsonConverter<JsonObject>
    {
        public override JsonObject Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            JsonNode.Parse(ref reader)!.AsObject();

        public override void Write(
            Utf8JsonWriter writer,
            JsonObject value,
            JsonSerializerOptions options)
        {
            var pairs = value
                .OrderBy(kv => kv.Key)
                .Select(kv => KeyValuePair.Create<string, JsonNode?>(kv.Key, kv.Value));
            
            var sorted = new JsonObject(pairs);

            sorted.WriteTo(writer, options);
        }
    }
}
