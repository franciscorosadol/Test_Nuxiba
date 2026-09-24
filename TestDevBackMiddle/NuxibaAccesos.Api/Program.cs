using Microsoft.EntityFrameworkCore;
using NuxibaAccesos.Api.Data;
using NuxibaAccesos.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    // Se conservan los nombres de propiedad tal como están en el modelo (User_id, TipoMov, fecha)
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddDbContext<AccesosDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("Accesos")));

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IReporteService, ReporteService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
