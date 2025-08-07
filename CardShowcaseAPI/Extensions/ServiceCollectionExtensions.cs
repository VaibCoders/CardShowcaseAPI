using CardShowcaseAPI.Configuration;
using CardShowcaseAPI.Services.Implementations;
using CardShowcaseAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CardShowcaseAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрация наших сервисов и настроек
        /// </summary>
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Настройки MongoDB
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDB"));
            // Настройки JWT
            services.Configure<JwtValidationSettings>(configuration.GetSection("JwtValidation"));
            // Сервис работы с карточками
            services.AddScoped<IShowcaseCardService, ShowcaseCardService>();
            return services;
        }

        /// <summary>
        /// Настройка JWT-аутентификации
        /// </summary>        /// <summary>
        /// Настройка JWT-аутентификации с SigningConfigurations и несколькими аудиториями
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // 1) Загружаем секцию Jwt из конфигурации
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtValidationSettings>()!;

            // 2) Регистрируем SigningConfigurations (генерация ключа из секретной строки)
            var signingConfigurations = new SigningConfigurations(jwtOptions.Secret);
            services.AddSingleton(signingConfigurations);

            // 3) Добавляем аутентификацию
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudiences = jwtOptions.AllowedAudiences,

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkew),

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingConfigurations.SecurityKey
                    };
                });

            // 4) Добавляем политику авторизации (по умолчанию)
            services.AddAuthorization();

            return services;
        }

        /// <summary>
        /// Настройка Swagger с поддержкой JWT
        /// </summary>
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CardShowcaseAPI", Version = "v1" });

                var jwtScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введите в формате: Bearer {ваш JWT}"
                };
                c.AddSecurityDefinition("Bearer", jwtScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [ jwtScheme ] = Array.Empty<string>()
                });
            });
            return services;
        }
    }
}
