using NuxibaAccesos.Api.Services;

namespace NuxibaAccesos.Tests;

public class CalculadoraTiemposTests
{
    [Fact]
    public void SumaLasSesionesCompletasDeCadaUsuario()
    {
        var movimientos = new[]
        {
            new Movimiento(1, 1, new DateTime(2024, 1, 1, 8, 0, 0)),
            new Movimiento(1, 0, new DateTime(2024, 1, 1, 10, 0, 0)),
            new Movimiento(1, 1, new DateTime(2024, 1, 2, 8, 0, 0)),
            new Movimiento(1, 0, new DateTime(2024, 1, 2, 9, 30, 0)),
            new Movimiento(2, 1, new DateTime(2024, 1, 1, 8, 0, 0)),
            new Movimiento(2, 0, new DateTime(2024, 1, 3, 8, 0, 0)),
        };

        var totales = CalculadoraTiempos.TiempoPorUsuario(movimientos);

        Assert.Equal(TimeSpan.FromHours(3.5), totales[1]);
        Assert.Equal(TimeSpan.FromHours(48), totales[2]);
    }

    [Fact]
    public void IgnoraElLoginQueNoTieneLogout()
    {
        var movimientos = new[]
        {
            new Movimiento(1, 1, new DateTime(2024, 1, 1, 8, 0, 0)),
            new Movimiento(1, 0, new DateTime(2024, 1, 1, 9, 0, 0)),
            new Movimiento(1, 1, new DateTime(2024, 1, 2, 8, 0, 0)),
        };

        var totales = CalculadoraTiempos.TiempoPorUsuario(movimientos);

        Assert.Equal(TimeSpan.FromHours(1), totales[1]);
    }

    [Fact]
    public void IgnoraElLogoutQueNoTieneLogin()
    {
        var movimientos = new[]
        {
            new Movimiento(1, 0, new DateTime(2024, 1, 1, 8, 0, 0)),
            new Movimiento(1, 1, new DateTime(2024, 1, 1, 9, 0, 0)),
            new Movimiento(1, 0, new DateTime(2024, 1, 1, 11, 0, 0)),
        };

        var totales = CalculadoraTiempos.TiempoPorUsuario(movimientos);

        Assert.Equal(TimeSpan.FromHours(2), totales[1]);
    }

    [Fact]
    public void OrdenaPorFechaAunqueLleguenDesordenados()
    {
        var movimientos = new[]
        {
            new Movimiento(1, 0, new DateTime(2024, 1, 1, 10, 0, 0)),
            new Movimiento(1, 1, new DateTime(2024, 1, 1, 8, 0, 0)),
        };

        var totales = CalculadoraTiempos.TiempoPorUsuario(movimientos);

        Assert.Equal(TimeSpan.FromHours(2), totales[1]);
    }
}
