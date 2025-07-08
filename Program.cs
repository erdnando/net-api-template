using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using netapi_template.Data;
using netapi_template.Services;
using netapi_template.Services.Interfaces;
using netapi_template.Helpers;
using netapi_template.Middleware;
using netapi_template.Repositories;
using netapi_template.Repositories.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using DotNetEnv;
using AspNetCoreRateLimit;
using netapi_template.Filters;

// 🔧 Cargar variables de entorno desde archivo .env
if (File.Exists(".env"))
{
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers(options => {
    // Add filter to handle validation exceptions
    options.Filters.Add<ValidationExceptionFilter>();
});

// Add HttpContextAccessor for getting client IP
builder.Services.AddHttpContextAccessor();

// Configure Database - MySQL
var connectionString = builder.Environment.IsDevelopment() 
    ? builder.Configuration.GetConnectionString("DevelopmentConnection")
    : builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is required. Please configure ConnectionStrings:DefaultConnection or ConnectionStrings:DevelopmentConnection in appsettings.json");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register services
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISecurityLoggerService, SecurityLoggerService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<IUtilsService, UtilsService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register services
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

// Configure CORS for React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // TODO: PRODUCTION CORS - Uncomment and configure for production
            // policy.WithOrigins("https://yourdomain.com")
            //       .WithMethods("GET", "POST", "PUT", "DELETE")
            //       .WithHeaders("Content-Type", "Authorization")
            //       .AllowCredentials();
            
            // Temporary for production testing - CHANGE THIS
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// Configure Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT SecretKey is required. Please configure JwtSettings:SecretKey in appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "NetApiTemplate",
        ValidAudience = jwtSettings["Audience"] ?? "NetApiTemplate",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add FluentValidation but configure it for better handling of async validators
builder.Services.AddFluentValidationAutoValidation(config => {
    // Skip validation for controllers/actions with [DisableValidation] attribute
    config.DisableDataAnnotationsValidation = true;
});
builder.Services.AddFluentValidationClientsideAdapters();

// Register all validators except for problematic async validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true, 
    filter: scan => !scan.ValidatorType.Name.Equals("CreateRoleDtoValidator", StringComparison.OrdinalIgnoreCase) &&
                   !scan.ValidatorType.Name.Equals("CreateUserDtoValidator", StringComparison.OrdinalIgnoreCase) &&
                   !scan.ValidatorType.Name.Equals("UpdateUserDtoValidator", StringComparison.OrdinalIgnoreCase));

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "NetAPI Template - User Management",
        Version = "2.0.0",
        Description = "Complete API for user, role, and permission management",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Support Team",
            Email = "support@example.com"
        }
    });
    c.EnableAnnotations();

    // JWT Bearer Auth for Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = false;
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetAPI Template v2.0.0");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "NetAPI Template - User Management API";
    });
}

// Add Security Headers
app.UseSecurityHeaders(policies => policies
    .AddFrameOptionsDeny()
    .AddXssProtectionBlock()
    .AddContentTypeOptionsNoSniff()
    .AddReferrerPolicyStrictOriginWhenCrossOrigin()
    .AddContentSecurityPolicy(builder =>
    {
        builder.AddDefaultSrc().Self();
        builder.AddScriptSrc().Self().UnsafeInline(); // UnsafeInline needed for Swagger in dev
        builder.AddStyleSrc().Self().UnsafeInline();
        builder.AddFontSrc().Self().Data();
        builder.AddImgSrc().Self().Data();
        if (app.Environment.IsDevelopment())
        {
            builder.AddConnectSrc().Self().From("ws:").From("wss:"); // For Swagger WebSocket
        }
    }));

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable Rate Limiting (before CORS and Authentication)
app.UseIpRateLimiting();

// Enable CORS
app.UseCors("ReactApp");

app.UseHttpsRedirection();

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply database migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var env = app.Environment;
    
    try
    {
        if (env.IsDevelopment())
        {
            // In development, ensure database exists but don't recreate it
            // This allows data to persist between API restarts
            logger.LogInformation("Development environment detected. Ensuring database exists...");
            context.Database.EnsureCreated();
            logger.LogInformation("Database existence ensured successfully.");
        }
        else
        {
            // In production, just apply migrations
            context.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error setting up database: {Message}", ex.Message);
        // Fallback to ensure database is created if migrations fail
        context.Database.EnsureCreated();
        logger.LogInformation("Database ensured created as fallback.");
    }
}

app.Run();
