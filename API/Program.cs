using System.Text;
using System.Text.Json.Serialization;
using API.Repositories;
using API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;

var builder = WebApplication.CreateBuilder(args);
var conf = builder.Configuration;
builder.Services.AddControllers();
builder.Services.AddDbContext<CoursesDbContext>(options =>
{
    options.UseNpgsql(conf.GetConnectionString("Default"));
});
builder.Services.AddSwaggerGen();
builder.Services.AddMvc().AddJsonOptions(op=>op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["JWT:Secret"])) ,
        ValidAudience = conf["JWT:Audience"],
        ValidIssuer = conf["JWT:Issuer"]
    };
});

builder.Services.AddTransient<ThemeRepository>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<ModuleRepository>();
builder.Services.AddTransient<CourseRepository>();
builder.Services.AddTransient<RoleRepository>();
builder.Services.AddTransient<LessonRepository>();
builder.Services.AddTransient<TaskAnswerRepository>();
builder.Services.AddTransient<ContentBlockRepository>();
builder.Services.AddTransient<TaskRepository>();
builder.Services.AddTransient<CorrelationRepository>();
builder.Services.AddExceptionHandler<GeneralExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy =>
{
    policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:5173");
});
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
