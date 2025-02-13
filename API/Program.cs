using BusinessLayer.Services;
using DataLayer;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => 
{
    options.AddPolicy(name: "Angular", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IProductoService,ProductoService>();
builder.Services.AddScoped<IProductosRepository,ProductosRepository>();
builder.Services.AddScoped<ISesionService, SesionService>();
builder.Services.AddScoped<ISesionRepository, SesionRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();

builder.Services.AddDbContext<TiendaContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"))
        );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseCors("Angular");

app.Run();
