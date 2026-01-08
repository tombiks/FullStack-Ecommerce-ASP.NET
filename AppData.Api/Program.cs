using Microsoft.EntityFrameworkCore;
using AppData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string is not found!");
builder.Services.AddDataExtensions(connectionString);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS yapılandırması - MVC uygulamasından gelen isteklere izin ver
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcApp", policy =>
    {
        policy.WithOrigins("https://localhost:7093")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS middleware'ini ekle
app.UseCors("AllowMvcApp");

// Static files için wwwroot klasörünü etkinleştir
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DbContext>();
    await context.Database.EnsureCreatedAsync();

}

app.Run();
