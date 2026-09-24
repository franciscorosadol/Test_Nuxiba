using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaAccesos.Api.Models;

// Registro de entrada (1) o salida (0) de un usuario. Tabla ccloglogin.
[Table("ccloglogin")]
public class Login
{
    // La tabla original no tiene llave, se agrega un identificador para poder usar PUT y DELETE por id
    [Key]
    public int Id { get; set; }

    public int User_id { get; set; }

    public int Extension { get; set; }

    // 1 = login, 0 = logout
    public int TipoMov { get; set; }

    public DateTime fecha { get; set; }
}
