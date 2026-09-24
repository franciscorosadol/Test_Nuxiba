using System.Text;
using Microsoft.AspNetCore.Mvc;
using NuxibaAccesos.Api.Services;

namespace NuxibaAccesos.Api.Controllers;

[ApiController]
[Route("reportes")]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _servicio;

    public ReportesController(IReporteService servicio)
    {
        _servicio = servicio;
    }

    // Descarga un CSV con el total de horas trabajadas por usuario
    [HttpGet("horas-trabajadas")]
    public async Task<IActionResult> HorasTrabajadas()
    {
        var csv = await _servicio.GenerarCsvHorasTrabajadasAsync();

        // Se antepone el BOM para que Excel interprete bien los acentos
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
        return File(bytes, "text/csv", "horas_trabajadas.csv");
    }
}
