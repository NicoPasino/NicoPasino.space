#nullable disable
namespace NicoPasino.Core.DTO.Ventas;

public partial class ClienteDto
{
    //public int Id { get; set; }

    public string Nombre { get; set; }

    public string Correo { get; set; }

    public int Documento { get; set; }

    public int? NroCompras { get; set; } // prop calculada

    //public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}