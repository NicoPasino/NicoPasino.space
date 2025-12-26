namespace NicoPasino.Core.DTO.Ventas
{
    public class VentaDto
    {
        public int? Id { get; set; }

        public int? IdPublica { get; set; } // TODO: agregar en la bd

        public int IdCliente { get; set; }

        public decimal PrecioTotal { get; set; }

        public DateTime? FechaVenta { get; set; }

        //public virtual Cliente IdClienteNavigation { get; set; }

        //public virtual ICollection<Ventaporproducto> Ventaporproducto { get; set; } = new List<Ventaporproducto>();
    }
}