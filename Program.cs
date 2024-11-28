using Microsoft.EntityFrameworkCore;
using BETempleOfInk.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext si usas Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar soporte para controladores API
builder.Services.AddControllers();

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Permite cualquier origen
              .AllowAnyMethod()   // Permite cualquier método (GET, POST, PUT, DELETE, etc.)
              .AllowAnyHeader();  // Permite cualquier encabezado
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty; 
    });
}

// Habilitar CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
