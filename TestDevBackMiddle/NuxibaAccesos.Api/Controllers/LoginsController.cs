using Microsoft.AspNetCore.Mvc;
using NuxibaAccesos.Api.Dtos;
using NuxibaAccesos.Api.Models;
using NuxibaAccesos.Api.Services;

namespace NuxibaAccesos.Api.Controllers;

[ApiController]
[Route("logins")]
public class LoginsController : ControllerBase
{
    private readonly ILoginService _servicio;

    public LoginsController(ILoginService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<Login>>> Get()
    {
        return await _servicio.ObtenerTodosAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Login>> GetPorId(int id)
    {
        var login = await _servicio.ObtenerPorIdAsync(id);
        return login is null ? NotFound() : login;
    }

    [HttpPost]
    public async Task<ActionResult<Login>> Post(LoginRequest solicitud)
    {
        var resultado = await _servicio.CrearAsync(solicitud);
        if (!resultado.EsExitoso)
        {
            return RespuestaError(resultado);
        }

        return CreatedAtAction(nameof(GetPorId), new { id = resultado.Valor!.Id }, resultado.Valor);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Login>> Put(int id, LoginRequest solicitud)
    {
        var resultado = await _servicio.ActualizarAsync(id, solicitud);
        if (!resultado.EsExitoso)
        {
            return RespuestaError(resultado);
        }

        return resultado.Valor!;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var eliminado = await _servicio.EliminarAsync(id);
        return eliminado ? NoContent() : NotFound();
    }

    private ActionResult RespuestaError(Resultado<Login> resultado)
    {
        return resultado.Error switch
        {
            TipoError.NoEncontrado => NotFound(new ProblemDetails { Title = resultado.Mensaje, Status = 404 }),
            TipoError.Conflicto => Conflict(new ProblemDetails { Title = resultado.Mensaje, Status = 409 }),
            _ => BadRequest(new ProblemDetails { Title = resultado.Mensaje, Status = 400 })
        };
    }
}
