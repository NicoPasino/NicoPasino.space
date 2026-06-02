using System.ComponentModel.DataAnnotations;

namespace NicoPasino.Core.DTO.Ventas
{
    public class VentaDto
    {
        public int? Id { get; set; }

        public int? Numero { get; set; }

        [StringLength(500, ErrorMessage = "El detalle no puede tener más de 500 caracteres.")]
        public string? Detalle { get; set; }

        public int IdCliente { get; set; }

        public DateTime? FechaVenta { get; set; }

        public string? Cliente { get; set; }

        [Required(ErrorMessage = "El DNI es requerido.")]
        [Range(10000000, 99999999, ErrorMessage = "El DNI debe tener 8 dígitos.")]
        public int? DNI { get; set; }

        [StringLength(100, MinimumLength = 4, ErrorMessage = "El nombre debe tener entre 4 y 100 caracteres.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "Los items son requeridos.")]
        [MinLength(1, ErrorMessage = "Debe haber al menos un item.")]
        public IEnumerable<int>? ItemsId { get; set; }

        [Required(ErrorMessage = "Las cantidades son requeridas.")]
        [MinLength(1, ErrorMessage = "Debe haber al menos una cantidad.")]
        public IEnumerable<int>? ItemsCant { get; set; }

        public IEnumerable<VentaporproductoDto>? Productos { get; set; }
    }
}
