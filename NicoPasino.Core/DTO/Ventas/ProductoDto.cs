namespace NicoPasino.Core.DTO.Ventas
{
    public class ProductoDto
    {
        public int? IdPublica { get; set; }

        public int IdCategoria { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public int Cantidad { get; set; }

        public decimal Precio { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }

        public string? Categoria { get; set; } // propiedad calculada

        //public virtual ICollection<Ventaporproducto> Ventaporproducto { get; set; } = new List<Ventaporproducto>();

    }
}
