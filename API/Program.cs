using System.Text;
using System.Text.Json.Serialization;
using API;
using API.Repositories;
using API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;
using Serilog;
using Serilog.Exceptions;


Log.Logger = new LoggerConfiguration()
.WriteTo.Console().CreateBootstrapLogger();

try
{
    Log.Information("Запуск веб-приложения");

    var builder = WebApplication.CreateBuilder(args);
    var conf = builder.Configuration;
    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Filter.ByExcluding(e => e.Properties.ContainsKey("HandledException"));
    });

    builder.Services.AddControllers();
    builder.Services.AddDbContext<CoursesDbContext>(op => op.UseNpgsql(conf.GetConnectionString("Default")));
    builder.Services.AddSwaggerGen();
    builder.Services.AddMvc().AddJsonOptions(op =>
        op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["JWT:Secret"])),
            ValidAudience = conf["JWT:Audience"],
            ValidIssuer = conf["JWT:Issuer"]
        };
    });
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddRepositories();
    builder.Services.AddExceptionHandler<GeneralExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Limits.MaxRequestBodySize = null;
    });
    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 16106127360L; 
    });
    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseCors(policy => 
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Accept-Ranges", "Content-Range", "Content-Length", "Content-Type", "Content-Disposition")
            .WithOrigins(conf["WebsiteUrl"]));
        
    app.UseSerilogRequestLogging(); 
    app.UseExceptionHandler();
    
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Аварийное выключение веб-приложения");
}
finally
{
    Log.CloseAndFlush();
}

