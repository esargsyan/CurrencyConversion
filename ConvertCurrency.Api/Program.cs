using ConvertCurrency.Api;
using ConvertCurrency.DataLayer.Repositories;
using ConvertCurrency.Service.Implementation;
using ConvertCurrency.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true; // Include API version in responses
    options.AssumeDefaultVersionWhenUnspecified = true; // Default to a version if none is specified
    options.DefaultApiVersion = new ApiVersion(1, 0); // Set the default API version (v1.0)
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Use URL versioning (e.g., /api/v1/controller)
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ConvertCurrency API", Version = "v1" });

    //  Configure Bearer Token Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    // Apply Bearer Token globally
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
builder.Services.AddMemoryCache();

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("69c7fcbd-69b2-430c-a200-543c15420c6a")),
        ValidateIssuer = false, // Set true if you have an issuer
        ValidateAudience = false, // Set true if you have an audience
        ValidateLifetime = true, // Token expiration validation
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")); // Admin role policy
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));   // User role policy
});


//var cs = builder.Configuration.GetConnectionString("ConvertCurrencyConnection");
//builder.Services.AddDbContext<ConvertCurrencyContext>(options => options.UseSqlServer(cs));
//builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddHttpClient<ICurrencyExternalSereviceClient, CurrencyExternalSereviceClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7021");
}).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

Serilog.Debugging.SelfLog.Enable(Console.Error);

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication(); // Enable authentication

app.UseAuthorization();

app.MapControllers();

app.Run();
