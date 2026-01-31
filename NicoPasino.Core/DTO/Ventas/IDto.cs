namespace NicoPasino.Core.DTO.Ventas
{
    // sin uso

    public interface IDto
    {
        public int? IdPublica { get; set; }

        public int? IdCategoria { get; set; }

        public string? Nombre { get; set; }

        public string? Descripcion { get; set; }

        public int? Cantidad { get; set; }

        public decimal? Precio { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool? Activo { get; set; }



        public int? IdCliente { get; set; }

        //public decimal? PrecioTotal { get; set; }

        public DateTime? FechaVenta { get; set; }


        public string? Correo { get; set; }
    }
}
