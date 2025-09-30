using System.Text.Json;

namespace MyEngine.Utils.Serialization;

public class SerializeSettings
{
    public static readonly JsonSerializerOptions Default = new JsonSerializerOptions
            { Converters = { new RectangleConverter(), new Vector2Converter() }, IgnoreReadOnlyFields = true, WriteIndented = true };
}