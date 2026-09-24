using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using NuxibaAccesos.Api.Data;
using NuxibaAccesos.Api.Dtos;
using NuxibaAccesos.Api.Models;

namespace NuxibaAccesos.Api.Services;

public interface ILoginService
{
    Task<List<Login>> ObtenerTodosAsync();

    Task<Login?> ObtenerPorIdAsync(int id);

    Task<Resultado<Login>> CrearAsync(LoginRequest solicitud);

    Task<Resultado<Login>> ActualizarAsync(int id, LoginRequest solicitud);

    Task<bool> EliminarAsync(int id);
}

public class LoginService : ILoginService
{
    private const int TipoLogout = 0;
    private const int TipoLogin = 1;

    private readonly AccesosDbContext _db;

    public LoginService(AccesosDbContext db)
    {
        _db = db;
    }

    public Task<List<Login>> ObtenerTodosAsync()
    {
        return _db.Logins.AsNoTracking().OrderBy(l => l.Id).ToListAsync();
    }

    public Task<Login?> ObtenerPorIdAsync(int id)
    {
        return _db.Logins.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Resultado<Login>> CrearAsync(LoginRequest solicitud)
    {
        var error = await ValidarAsync(solicitud, null);
        if (error is not null)
        {
            return error;
        }

        var login = new Login
        {
            User_id = solicitud.User_id,
            Extension = solicitud.Extension,
            TipoMov = solicitud.TipoMov,
            fecha = solicitud.fecha
        };

        _db.Logins.Add(login);
        await _db.SaveChangesAsync();
        return Resultado<Login>.Ok(login);
    }

    public async Task<Resultado<Login>> ActualizarAsync(int id, LoginRequest solicitud)
    {
        var login = await _db.Logins.FirstOrDefaultAsync(l => l.Id == id);
        if (login is null)
        {
            return Resultado<Login>.Falla(TipoError.NoEncontrado, $"No existe el registro {id}.");
        }

        var error = await ValidarAsync(solicitud, id);
        if (error is not null)
        {
            return error;
        }

        login.User_id = solicitud.User_id;
        login.Extension = solicitud.Extension;
        login.TipoMov = solicitud.TipoMov;
        login.fecha = solicitud.fecha;

        await _db.SaveChangesAsync();
        return Resultado<Login>.Ok(login);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var login = await _db.Logins.FirstOrDefaultAsync(l => l.Id == id);
        if (login is null)
        {
            return false;
        }

        _db.Logins.Remove(login);
        await _db.SaveChangesAsync();
        return true;
    }

    // Valida los datos y que el movimiento respete la secuencia login/logout del usuario.
    // idExcluido es el registro que se está actualizando, para no compararlo contra sí mismo.
    private async Task<Resultado<Login>?> ValidarAsync(LoginRequest solicitud, int? idExcluido)
    {
        if (solicitud.TipoMov != TipoLogin && solicitud.TipoMov != TipoLogout)
        {
            return Resultado<Login>.Falla(TipoError.Validacion, "TipoMov debe ser 1 (login) o 0 (logout).");
        }

        if (solicitud.fecha < SqlDateTime.MinValue.Value)
        {
            return Resultado<Login>.Falla(TipoError.Validacion, "La fecha no es válida.");
        }

        if (solicitud.fecha > DateTime.Now)
        {
            return Resultado<Login>.Falla(TipoError.Validacion, "La fecha no puede ser futura.");
        }

        var existeUsuario = await _db.Usuarios.AnyAsync(u => u.User_id == solicitud.User_id);
        if (!existeUsuario)
        {
            return Resultado<Login>.Falla(TipoError.Validacion, $"El usuario {solicitud.User_id} no existe en ccUsers.");
        }

        var movimientos = _db.Logins.Where(l => l.User_id == solicitud.User_id && l.Id != idExcluido);

        var mismaFecha = await movimientos.AnyAsync(l => l.fecha == solicitud.fecha);
        if (mismaFecha)
        {
            return Resultado<Login>.Falla(TipoError.Conflicto, "El usuario ya tiene un movimiento con esa fecha.");
        }

        var anterior = await movimientos
            .Where(l => l.fecha < solicitud.fecha)
            .OrderByDescending(l => l.fecha)
            .FirstOrDefaultAsync();

        var siguiente = await movimientos
            .Where(l => l.fecha > solicitud.fecha)
            .OrderBy(l => l.fecha)
            .FirstOrDefaultAsync();

        if (anterior is not null && anterior.TipoMov == solicitud.TipoMov)
        {
            var mensaje = solicitud.TipoMov == TipoLogin
                ? "El usuario ya tiene un login sin logout anterior."
                : "El usuario no tiene un login abierto que cerrar con este logout.";
            return Resultado<Login>.Falla(TipoError.Conflicto, mensaje);
        }

        if (siguiente is not null && siguiente.TipoMov == solicitud.TipoMov)
        {
            return Resultado<Login>.Falla(TipoError.Conflicto,
                "El movimiento dejaría dos registros iguales seguidos para el usuario.");
        }

        return null;
    }
}
