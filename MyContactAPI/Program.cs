using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using MyContactAPI.Extension;
using MyContactBook.Core.Services.Implementation;
using MyContactBook.Core.Services.Interface;
using MyContactBook.Data.Context;
using MyContactBook.Data.DataInitializer;
using MyContactBook.Data.Repository.Implementation;
using MyContactBook.Data.Repository.Interface;
using MyContactBook.Model.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyContactBook", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDependencies(builder.Configuration);

//Identity registration
builder.Services.AddIdentity<AppUser, IdentityRole>()
       .AddEntityFrameworkStores<ContactBookDbContext>()
       .AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


// Get the service scope and obtain the necessary services
using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
var context = serviceProvider.GetRequiredService<ContactBookDbContext>();
var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

UserAndRoleDataInitializer.SeedData(context, userManager, roleManager).Wait();


app.MapControllers();

app.Run();
