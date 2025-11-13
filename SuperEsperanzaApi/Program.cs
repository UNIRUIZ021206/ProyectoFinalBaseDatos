using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SuperEsperanzaApi;
using SuperEsperanzaApi.Data;
using SuperEsperanzaApi.Dao;
using SuperEsperanzaApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.TypeInfoResolver = AppJsonSerializerContext.Default;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // Permite nombres en minúsculas
    });
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger con soporte para JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Super Esperanza API",
        Version = "v1",
        Description = "API para Super La Esperanza con autenticación JWT"
    });

    // Configurar JWT Bearer en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

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
// Configurar JWT Authentication
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
    // Configurar cómo se lee el token del header
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Permitir HTTP en desarrollo
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Eliminar el tiempo de gracia para tokens expirados
    };

    // Agregar eventos para debugging y manejo de errores
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "Error en la autenticación JWT. Error: {Error}", context.Exception?.Message);
            
            // Agregar más información de debugging
            var authHeader = context.Request.Headers["Authorization"].ToString();
            logger.LogWarning("Header Authorization recibido: {Header}", 
                string.IsNullOrEmpty(authHeader) ? "(vacío)" : authHeader.Substring(0, Math.Min(50, authHeader.Length)));
            
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token JWT validado correctamente para el usuario: {User}", 
                context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Desafío de autenticación: {Error}, {ErrorDescription}", 
                context.Error, context.ErrorDescription);
            
            // Asegurar que se devuelva un 401 cuando falte el token
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var token = context.Token;
            logger.LogDebug("Token recibido: {TokenPresente}", !string.IsNullOrEmpty(token));
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Registrar servicios de la aplicación
builder.Services.AddScoped<ConexionDB>();
builder.Services.AddScoped<UsuarioDAO>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Configurar CORS si es necesario
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configurar el pipeline HTTP
// Swagger debe estar antes de otros middlewares
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Super Esperanza API v1");
    c.RoutePrefix = "swagger"; // Swagger disponible en /swagger
});

// IMPORTANTE: El orden de los middlewares es crítico
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication(); // Debe ir antes de UseAuthorization
app.UseAuthorization();  // Debe ir después de UseAuthentication
app.MapControllers();

app.Run();
