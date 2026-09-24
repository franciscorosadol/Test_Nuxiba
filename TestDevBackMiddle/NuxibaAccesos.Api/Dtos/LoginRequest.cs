namespace NuxibaAccesos.Api.Dtos;

// Datos que se reciben al crear o actualizar un login/logout
public class LoginRequest
{
    public int User_id { get; set; }

    public int Extension { get; set; }

    // 1 = login, 0 = logout
    public int TipoMov { get; set; }

    public DateTime fecha { get; set; }
}
