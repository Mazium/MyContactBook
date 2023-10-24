using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyContactBook.Core.Services.Implementation;
using MyContactBook.Core.Services.Interface;
using MyContactBook.Data.Context;
using MyContactBook.Data.Repository.Implementation;
using MyContactBook.Data.Repository.Interface;
using MyContactBook.Utilities;
using System.Text;

namespace MyContactAPI.Extension
{
    public static class DbRegistryExtension
    {
        public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ContactBookDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MyContactBookConnectionString"));
            });

            services.AddScoped<IContactBookService, ContactBookService>();
            services.AddScoped<IContactBookRepository, ContactBookRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthService, AuthService>();

            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));


            services.AddSingleton(provider =>
            {
                var cloudinarySettings = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;

                Account cloudinaryAccount = new(
                    cloudinarySettings.CloudName,
                    cloudinarySettings.APIKey,
                    cloudinarySettings.APISecret);

                return new Cloudinary(cloudinaryAccount);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = configuration["Authentication:Issuer"],
                     ValidAudience = configuration["Authentication:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.ASCII.GetBytes(configuration["Authentication:SecretForKey"]))
                 };
             });

            
        }


        
    }
}
  