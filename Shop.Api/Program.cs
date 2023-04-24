using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Shop.Api.Data;
using Shop.Api.Repository;
using AutoMapper;
using ForgotPasswordService.Repository;
using System.Security.Principal;
using StackExchange.Redis;
using Shop.Api.Abtracst;
using Shop.Api.Repository;
using MassTransit;
using Share.Message;
using MassTransit.Transports.Fabric;
using Shop.Api.MilderWare;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Shop.Api.Filter;
using ForgotPasswordService.Model;
using MySqlConnector;
using ForgotPasswordService.Message;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;
builder.Services.AddMassTransit(x =>
{
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
    {
        config.Host(new Uri(builder.Configuration["RabbitMQ:Rabbitserver"]), h =>
        {
            h.Username(builder.Configuration["RabbitMQ:User"]);
            h.Password(builder.Configuration["RabbitMQ:Password"]);
        });
    }));
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddMvc();
builder.Services.AddDbContext<NewDBContext>(options =>
options.UseSqlServer(
    builder.Configuration.GetConnectionString("ShopConnect")));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});
//protection
builder.Services.Configure<DataProtectionTokenProviderOptions>(pr => pr.TokenLifespan = TimeSpan.FromHours(10));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
    builder =>
    {
        builder.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
//Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisCacheServerUrl"];
    options.InstanceName = "Product";
});
//DI
builder.Services.AddScoped<IUserServices, UserRespository>();
builder.Services.AddScoped<IProductServices, ProductRepository>();
builder.Services.AddScoped<IPushlishService<ProductSend>,PushlishResponsitory>();
builder.Services.AddScoped<IImageServices, ImageResponsitory>();
builder.Services.AddScoped<IOrderServices, OrderResponsitory>();
//Add email config
var emailCofig = configuration.GetSection("EmailCofiguration").Get<EmailCofiuration>();
builder.Services.AddSingleton(emailCofig);
builder.Services.AddScoped<ISendMailService<TokenResetMessage>, EmailRepository>();
builder.Services.AddIdentity<UserApp, IdentityRole>()
    .AddEntityFrameworkStores<NewDBContext>()
    .AddDefaultTokenProviders();
builder.Services.AddTransient<FileFormatFilter>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Configure the HTTP request pipeline.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.JwtAuthenticationMiddleware();
// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowOrigin");
app.MapControllers();

app.Run();
