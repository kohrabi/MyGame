using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MyEngine.Utils.Serialization;

public class RectangleDto
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public static RectangleDto FromRectangle(Rectangle r) =>
        new RectangleDto { X = r.X, Y = r.Y, Width = r.Width, Height = r.Height };

    public Rectangle ToRectangle() => new Rectangle(X, Y, Width, Height);
}

public class RectangleConverter : JsonConverter<Rectangle>
{
    public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        
        int x = 0, y = 0, width = 0, height = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return new Rectangle(x, y, width, height);

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propName = reader.GetString();
                reader.Read();

                switch (propName)
                {
                    case "X": x = reader.GetInt32(); break;
                    case "Y": y = reader.GetInt32(); break;
                    case "Width": width = reader.GetInt32(); break;
                    case "Height": height = reader.GetInt32(); break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteNumber("Width", value.Width);
        writer.WriteNumber("Height", value.Height);
        writer.WriteEndObject();
    }
}