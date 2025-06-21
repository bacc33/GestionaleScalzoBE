using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using gestionale_scalzo.Data;
using gestionale_scalzo.Services;
using gestionale_scalzo.Model;
using Serilog;
using Microsoft.OpenApi.Models;
using gestionale_scalzo.Repository.IRepository;
using gestionale_scalzo.Repository;
using gestionale_scalzo.Utils;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurazione Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 100;
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(30);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);
});

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurazione Swagger e HttpContextAccessor
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });

    options.EnableAnnotations();

    // Personalizza l'ordinamento dei metodi nella UI di Swagger
    options.DocumentFilter<OperationOrderingDocumentFilter>();
});

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

//CONNESSIONE AL DB 
var connectionString = builder.Configuration.GetConnectionString("WebApiDatabase");

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseNpgsql(connectionString, options =>
    {
        option.EnableServiceProviderCaching();
    });
}, ServiceLifetime.Scoped);

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, optionsBuilder =>
    {
    });
}, ServiceLifetime.Scoped);

builder.Services.AddScoped<IDbContextFactory<ApplicationDbContext>, DbContextFactory<ApplicationDbContext>>();
builder.Services.AddScoped<ITokenUtility, TokenUtility>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Automapper
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Authentication (JWT)
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});


builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.MultipartBoundaryLengthLimit = int.MaxValue;
    options.MultipartHeadersCountLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});


// Middleware di caching, compressione e gestione file statici
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            var serverUrl = $"https://{httpReq.Host.Value}";
            swaggerDoc.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
        {
            new OpenApiServer { Url = serverUrl }
        };
        });
    });
    app.UseSwaggerUI();    
}

// Serve i file statici
app.UseStaticFiles();

// Fallback per la tua SPA (Angular)
app.MapFallbackToFile("/index.html");

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

