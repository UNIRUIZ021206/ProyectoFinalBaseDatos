using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models; // <-- ARREGLA ERRORES DE SWAGGER
using SuperEsperanzaApi; // <-- ARREGLA MappingConfig
using SuperEsperanzaApi.Dao;
using SuperEsperanzaApi.Dao.Interfaces; // <-- ARREGLA IRepository
using SuperEsperanzaApi.Data;
using SuperEsperanzaApi.Models; // <-- ARREGLA Categoria
using SuperEsperanzaApi.Services;
using SuperEsperanzaApi.Services.Interfaces; // <-- ARREGLA IService y CategoriaService
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Resolver source-gen (ya generado)
var resolver = AppJsonSerializerContext.Default;

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.TypeInfoResolver = resolver;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Hacer que todos los endpoints de MVC/API requieran autenticación por defecto.
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// JWT config (asegúrese de tener Jwt:Key/Issuer/Audience en appsettings)
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key no configurada");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer no configurado");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience no configurado");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero,
        // Ajuste los Claim types según su token: "role" o ClaimTypes.Role
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = "role" // Asegúrate que tu JwtService use "role" o ClaimTypes.Role
    };
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opts =>
{
    opts.SerializerOptions.TypeInfoResolver = resolver;
});
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(opts =>
{
    opts.JsonSerializerOptions.TypeInfoResolver = resolver;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Super Esperanza API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // --- SINTAXIS CORREGIDA (Arregla CS0117) ---
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// --- Inyección de Dependencias (DI) ---
builder.Services.AddScoped<ConexionDB>();

// Servicios de Autenticación
builder.Services.AddScoped<UsuarioDAO>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Servicios de Rol
builder.Services.AddScoped<RolDAO>();
builder.Services.AddScoped<IRolService, RolService>();

// Servicios de Categoria (Añadidos)
builder.Services.AddScoped<IRepository<Categoria>, CategoriaDAO>();
builder.Services.AddScoped<IService<Categoria>, CategoriaService>();

// AutoMapper (Añadido)
builder.Services.AddAutoMapper(typeof(MappingConfig));


builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// --- Pipeline HTTP ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Super Esperanza API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();