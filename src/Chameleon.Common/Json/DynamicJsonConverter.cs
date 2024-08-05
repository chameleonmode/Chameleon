using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chameleon.Common.Json;
public class DynamicJsonConverter<T1, T2> : JsonConverter<T2> where T1 : T2
{
    public override T2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
        return JsonSerializer.Deserialize<T1>(jsonObject.GetRawText(), options);
    }

    public override void Write(Utf8JsonWriter writer, T2 value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (T1)value, options);
    }
}