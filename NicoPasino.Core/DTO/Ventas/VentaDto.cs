namespace NicoPasino.Core.DTO.Ventas
{
    public class VentaDto
    {
        public int? Id { get; set; }

        public int? Numero { get; set; }

        public string? Detalle { get; set; }

        public int IdCliente { get; set; }

        //public decimal PrecioTotal { get; set; } // TODO: eliminar

        public DateTime? FechaVenta { get; set; }

        //public virtual Cliente? IdClienteNavigation { get; set; }

        //public virtual ICollection<Ventaporproducto>? Ventaporproducto { get; set; }



        /* ---- propiedades calculadas ---- */

        public string? Cliente { get; set; } // (solo para mostrar)


        // (para Venta Nueva)
        // Nota: no exportar porque solo recibe VentaDto (servicio genérico)
        public int? DNI { get; set; }
        public string? Nombre { get; set; }
        public IEnumerable<int>? ItemsId { get; set; }
        public IEnumerable<int>? ItemsCant { get; set; }

        //public ICollection<Ventaporproducto>? VentaPorProducto { get; set; } // Dto para evitar bucle

        public IEnumerable<VentaporproductoDto>? Productos { get; set; }
    }
}