using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NuxibaAccesos.Api.Data;

namespace NuxibaAccesos.Api.Services;

public interface IReporteService
{
    Task<string> GenerarCsvHorasTrabajadasAsync();
}

public class ReporteService : IReporteService
{
    private readonly AccesosDbContext _db;

    public ReporteService(AccesosDbContext db)
    {
        _db = db;
    }

    public async Task<string> GenerarCsvHorasTrabajadasAsync()
    {
        var usuarios = await _db.Usuarios.AsNoTracking().OrderBy(u => u.User_id).ToListAsync();

        var movimientos = await _db.Logins.AsNoTracking()
            .Select(l => new Movimiento(l.User_id, l.TipoMov, l.fecha))
            .ToListAsync();

        var tiempos = CalculadoraTiempos.TiempoPorUsuario(movimientos);

        // Si un IDArea se repite en el catálogo se toma el registro más antiguo
        var areas = (await _db.Areas.AsNoTracking().ToListAsync())
            .GroupBy(a => a.IDArea)
            .ToDictionary(g => g.Key, g => g.OrderBy(a => a.CreateDate).First().AreaName);

        var csv = new StringBuilder();
        csv.AppendLine("Usuario,NombreCompleto,Area,TotalHorasTrabajadas");

        foreach (var usuario in usuarios)
        {
            var nombre = string.Join(' ',
                new[] { usuario.Nombres, usuario.ApellidoPaterno, usuario.ApellidoMaterno }
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p!.Trim()));

            areas.TryGetValue(usuario.IDArea, out var area);
            tiempos.TryGetValue(usuario.User_id, out var tiempo);

            csv.AppendLine(string.Join(',',
                Escapar(usuario.Login),
                Escapar(nombre),
                Escapar(area ?? string.Empty),
                tiempo.TotalHours.ToString("0.00", CultureInfo.InvariantCulture)));
        }

        return csv.ToString();
    }

    // Encierra el valor entre comillas cuando contiene comas, comillas o saltos de línea
    private static string Escapar(string valor)
    {
        if (valor.IndexOfAny([',', '"', '\r', '\n']) < 0)
        {
            return valor;
        }

        return "\"" + valor.Replace("\"", "\"\"") + "\"";
    }
}
