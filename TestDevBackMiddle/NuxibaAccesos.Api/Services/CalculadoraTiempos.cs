namespace NuxibaAccesos.Api.Services;

public record Movimiento(int User_id, int TipoMov, DateTime fecha);

public static class CalculadoraTiempos
{
    // Suma, por usuario, el tiempo entre cada login (1) y el logout (0) que le sigue inmediatamente.
    // Un login sin logout después, o un logout sin login antes, no aporta tiempo.
    public static Dictionary<int, TimeSpan> TiempoPorUsuario(IEnumerable<Movimiento> movimientos)
    {
        var totales = new Dictionary<int, TimeSpan>();

        foreach (var grupo in movimientos.GroupBy(m => m.User_id))
        {
            var ordenados = grupo.OrderBy(m => m.fecha).ToList();
            var total = TimeSpan.Zero;

            for (var i = 0; i < ordenados.Count - 1; i++)
            {
                if (ordenados[i].TipoMov == 1 && ordenados[i + 1].TipoMov == 0)
                {
                    total += ordenados[i + 1].fecha - ordenados[i].fecha;
                }
            }

            totales[grupo.Key] = total;
        }

        return totales;
    }
}
