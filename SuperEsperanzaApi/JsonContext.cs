using System.Text.Json.Serialization;
using SuperEsperanzaApi.Dto;
using SuperEsperanzaApi.Models;

namespace SuperEsperanzaApi
{
    [JsonSerializable(typeof(LoginRequest))]
    [JsonSerializable(typeof(LoginResponse))]
    [JsonSerializable(typeof(ApiStatusResponse))]
    [JsonSerializable(typeof(ErrorResponse))]
    [JsonSerializable(typeof(MensajeResponse))]
    [JsonSerializable(typeof(Usuario))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}

