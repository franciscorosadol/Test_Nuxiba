using Microsoft.EntityFrameworkCore;
using NuxibaAccesos.Api.Data;
using NuxibaAccesos.Api.Dtos;
using NuxibaAccesos.Api.Models;
using NuxibaAccesos.Api.Services;

namespace NuxibaAccesos.Tests;

public class LoginServiceTests
{
    private static AccesosDbContext CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<AccesosDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new AccesosDbContext(opciones);
        db.Usuarios.Add(new Usuario { User_id = 1, Login = "prueba", Nombres = "Prueba" });
        db.SaveChanges();
        return db;
    }

    private static LoginRequest Solicitud(int tipo, DateTime fecha, int usuario = 1) => new()
    {
        User_id = usuario,
        Extension = 100,
        TipoMov = tipo,
        fecha = fecha
    };

    [Fact]
    public async Task Crear_LoginValido_SeGuarda()
    {
        var servicio = new LoginService(CrearContexto());

        var resultado = await servicio.CrearAsync(Solicitud(1, new DateTime(2024, 1, 1, 8, 0, 0)));

        Assert.True(resultado.EsExitoso);
        Assert.Single(await servicio.ObtenerTodosAsync());
    }

    [Fact]
    public async Task Crear_UsuarioInexistente_FallaPorValidacion()
    {
        var servicio = new LoginService(CrearContexto());

        var resultado = await servicio.CrearAsync(Solicitud(1, new DateTime(2024, 1, 1), usuario: 99));

        Assert.Equal(TipoError.Validacion, resultado.Error);
    }

    [Fact]
    public async Task Crear_TipoMovInvalido_FallaPorValidacion()
    {
        var servicio = new LoginService(CrearContexto());

        var resultado = await servicio.CrearAsync(Solicitud(2, new DateTime(2024, 1, 1)));

        Assert.Equal(TipoError.Validacion, resultado.Error);
    }

    [Fact]
    public async Task Crear_FechaFutura_FallaPorValidacion()
    {
        var servicio = new LoginService(CrearContexto());

        var resultado = await servicio.CrearAsync(Solicitud(1, DateTime.Now.AddDays(5)));

        Assert.Equal(TipoError.Validacion, resultado.Error);
    }

    [Fact]
    public async Task Crear_FechaMuyAntigua_FallaPorValidacion()
    {
        var servicio = new LoginService(CrearContexto());

        var resultado = await servicio.CrearAsync(Solicitud(1, DateTime.MinValue));

        Assert.Equal(TipoError.Validacion, resultado.Error);
    }

    [Fact]
    public async Task Crear_LoginSinLogoutAnterior_Falla()
    {
        var servicio = new LoginService(CrearContexto());
        await servicio.CrearAsync(Solicitud(1, new DateTime(2024, 1, 1, 8, 0, 0)));

        var resultado = await servicio.CrearAsync(Solicitud(1, new DateTime(2024, 1, 2, 8, 0, 0)));

        Assert.Equal(TipoError.Conflicto, resultado.Error);
    }

    [Fact]
    public async Task Crear_LogoutSinLoginAbierto_Falla()
    {
        var servicio = new LoginService(CrearContexto());
        await servicio.CrearAsync(Solicitud(1, new DateTime(2024, 1, 1, 8, 0, 0)));
        await servicio.CrearAsync(Solicitud(0, new DateTime(2024, 1, 1, 17, 0, 0)));

        var resultado = await servicio.CrearAsync(Solicitud(0, new DateTime(2024, 1, 2, 17, 0, 0)));

        Assert.Equal(TipoError.Conflicto, resultado.Error);
    }

    [Fact]
    public async Task Crear_LoginDespuesDeLogout_Funciona()
    {
        var servicio = new LoginService(CrearContexto());
        await servicio.CrearAsync(Solicitud(1, new DateTime(2024, 1, 1, 8, 0, 0)));
        await servicio.CrearAsync(Solicitud(0, new DateTime(2024, 1, 1, 17, 0, 0)));

        var resultado = await servicio.CrearAsync(Solicitud(1, new DateTime(2024, 1, 2, 8, 0, 0)));

        Assert.True(resultado.EsExitoso);
    }

    [Fact]
    public async Task Crear_MismaFechaQueOtroMovimiento_Falla()
    {
        var servicio = new LoginService(CrearContexto());
        var fecha = new DateTime(2024, 1, 1, 8, 0, 0);
        await servicio.CrearAsync(Solicitud(1, fecha));

        var resultado = await servicio.CrearAsync(Solicitud(0, fecha));

        Assert.Equal(TipoError.Conflicto, resultado.Error);
    }

    [Fact]
    public async Task Actualizar_Inexistente_NoEncontrado()
    {
        var servicio = new LoginService(CrearContexto());

        var resultado = await servicio.ActualizarAsync(50, Solicitud(1, new DateTime(2024, 1, 1)));

        Assert.Equal(TipoError.NoEncontrado, resultado.Error);
    }

    [Fact]
    public async Task Actualizar_ConDatosValidos_CambiaElRegistro()
    {
        var servicio = new LoginService(CrearContexto());
        var creado = await servicio.CrearAsync(Solicitud(1, new DateTime(2024, 1, 1, 8, 0, 0)));

        var resultado = await servicio.ActualizarAsync(creado.Valor!.Id, Solicitud(1, new DateTime(2024, 1, 1, 9, 0, 0)));

        Assert.True(resultado.EsExitoso);
        Assert.Equal(new DateTime(2024, 1, 1, 9, 0, 0), (await servicio.ObtenerPorIdAsync(creado.Valor.Id))!.fecha);
    }

    [Fact]
    public async Task Actualizar_TipoQueRompeLaSecuencia_Falla()
    {
        var servicio = new LoginService(CrearContexto());
        await servicio.CrearAsync(Solicitud(1, new DateTime(2024, 1, 1, 8, 0, 0)));
        var logout = await servicio.CrearAsync(Solicitud(0, new DateTime(2024, 1, 1, 17, 0, 0)));

        var resultado = await servicio.ActualizarAsync(logout.Valor!.Id, Solicitud(1, new DateTime(2024, 1, 1, 17, 0, 0)));

        Assert.Equal(TipoError.Conflicto, resultado.Error);
    }

    [Fact]
    public async Task Eliminar_Existente_LoQuita()
    {
        var servicio = new LoginService(CrearContexto());
        var creado = await servicio.CrearAsync(Solicitud(1, new DateTime(2024, 1, 1, 8, 0, 0)));

        Assert.True(await servicio.EliminarAsync(creado.Valor!.Id));
        Assert.Empty(await servicio.ObtenerTodosAsync());
    }

    [Fact]
    public async Task Eliminar_Inexistente_DevuelveFalso()
    {
        var servicio = new LoginService(CrearContexto());

        Assert.False(await servicio.EliminarAsync(123));
    }
}
