using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Features;
using Trasgo.Shared.ViewModels;
using RepositoryPattern.Services.OtpService;
using RepositoryPattern.Services.AuthService;
using RepositoryPattern.Services.AttachmentService;
using RepositoryPattern.Services.RoleService;
using RepositoryPattern.Services.UserService;
using RepositoryPattern.Services.BannerService;
using RepositoryPattern.Services.SettingService;
using RepositoryPattern.Services.ChatService;
using RepositoryPattern.Services.ScraperService;
using RepositoryPattern.Services.RateCardService;
using RepositoryPattern.Services.CampaignService;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// =======================
// 🔧 Service Registrations
// =======================
builder.Services.AddSingleton<ConvertJWT>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IScraperService, ScraperService>();
builder.Services.AddScoped<IRateCardService, RateCardService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

// =======================
// 🔐 JWT Authentication
// =======================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    string secretKey = config.GetSection("AppSettings")["JwtKey"];

    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "Impact.com",
        ValidAudience = "Impact.com",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// =======================
// 🔍 Swagger + Auth Header
// =======================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Impact", Version = "v1" });
    c.OperationFilter<SwaggerFileOperationFilter>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
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
            new string[] { }
        }
    });
});

// =======================
// 🌐 CORS
// =======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// =======================
// 📁 File Upload Limits
// =======================
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 300 * 1024 * 1024; // 300 MB
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 300 * 1024 * 1024;
});

var app = builder.Build();

// =======================
// ⚠️ Global Error Handler
// =======================
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandler?.Error;

        Console.WriteLine("❌ Unhandled Exception: " + exception?.Message);
        Console.WriteLine(exception?.StackTrace);

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            code = 500,
            errorMessage = new { error = "Internal Server Error", details = exception?.Message }
        };
        var json = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(json);
    });
});

// =======================
// 📦 Middleware Order
// =======================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Impact API V1");
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.Use(async (context, next) =>
{
    var maxRequestBodySizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
    if (maxRequestBodySizeFeature != null)
    {
        maxRequestBodySizeFeature.MaxRequestBodySize = 200 * 1024 * 1024;
    }

    await next();

    if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
    {
        context.Response.ContentType = "application/json";
        var viewModel = new
        {
            code = 401,
            errorMessage = new ErrorDtoVM { error = MessageReport.Unauthorized }
        };
        var json = JsonSerializer.Serialize(viewModel);
        await context.Response.WriteAsync(json);
    }
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
