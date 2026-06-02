using System.ComponentModel.DataAnnotations;

namespace NicoPasino.Core.DTO.Notas;

public partial class CardsDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El header es requerido.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "El header debe tener entre 1 y 50 caracteres.")]
    public string Header { get; set; }

    [Required(ErrorMessage = "El texto es requerido.")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "El texto debe tener entre 1 y 500 caracteres.")]
    public string Text { get; set; }

    [StringLength(50, ErrorMessage = "La fecha no puede tener más de 50 caracteres.")]
    public string? Fecha { get; set; }

    [StringLength(20, ErrorMessage = "La hora no puede tener más de 20 caracteres.")]
    public string? Hora { get; set; }

    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
    public string Name { get; set; }

    [StringLength(20, ErrorMessage = "El color no puede tener más de 20 caracteres.")]
    public string Color { get; set; }
}
