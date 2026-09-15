using System.Text.Json;
using System.Text.Json.Serialization;

namespace SistemaGYM.Web.Services;

public static class JsonOptionsWeb
{
    public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}