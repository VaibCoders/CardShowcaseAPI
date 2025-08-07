using CardShowcaseAPI.Extensions;
using CardShowcaseAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1) Регистрация сервисов
builder.Services
    .AddCustomServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerWithJwt();

// 2) Стандартные ASP.NET-компоненты
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllDev",
        policy => policy
            .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>())
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// 3) Конвейер middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CardShowcaseAPI v1"));

app.UseHttpsRedirection();
app.UseCors("AllowAllDev");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
