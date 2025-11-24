using System.Text;
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["JWT:Secret"])) ,
        ValidAudience = conf["JWT:Audience"],
        ValidIssuer = conf["JWT:Issuer"]
    };
});

builder.Services.AddTransient<ThemeRepository>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<CourseRepository>();
builder.Services.AddTransient<RoleRepository>();

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
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
