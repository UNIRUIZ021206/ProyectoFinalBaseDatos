using System.Collections.Generic;
using System.Text.Json.Serialization; // <--- ESTA ES LA LÍNEA CRÍTICA QUE FALTABA
using Microsoft.AspNetCore.Mvc;       // <--- Necesario para ProblemDetails
using SuperEsperanzaApi.Dto;
using SuperEsperanzaApi.Models;

namespace SuperEsperanzaApi
{
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    
    // --- Autenticación y General ---
    [JsonSerializable(typeof(LoginRequest))]
    [JsonSerializable(typeof(LoginResponse))]
    [JsonSerializable(typeof(ApiStatusResponse))]
    [JsonSerializable(typeof(ErrorResponse))]
    [JsonSerializable(typeof(MensajeResponse))]
    [JsonSerializable(typeof(Usuario))]
    [JsonSerializable(typeof(Rol))]                  // <-- Añadido: metadata para Rol
    [JsonSerializable(typeof(IEnumerable<Rol>))]     // <-- Añadido: colecciones de Rol
    [JsonSerializable(typeof(List<Rol>))]
    [JsonSerializable(typeof(Rol[]))]

    // --- Errores Estándar de ASP.NET Core ---
    [JsonSerializable(typeof(ProblemDetails))]
    [JsonSerializable(typeof(ValidationProblemDetails))]
    [JsonSerializable(typeof(Dictionary<string, string[]>))]

    // ... otros registros ...

    // --- MÓDULO CATEGORIA ---
    [JsonSerializable(typeof(Categoria))]
    [JsonSerializable(typeof(IEnumerable<Categoria>))]
    [JsonSerializable(typeof(List<Categoria>))]

    [JsonSerializable(typeof(CategoriaDto))]
    [JsonSerializable(typeof(IEnumerable<CategoriaDto>))]
    [JsonSerializable(typeof(List<CategoriaDto>))]

    [JsonSerializable(typeof(CategoriaCreateDto))]
    [JsonSerializable(typeof(CategoriaUpdateDto))]

    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}