namespace NicoPasino.Core.DTO.Ventas;

public partial class VentaporproductoDto
{
    //public int IdVenta { get; set; }

    //public int IdProducto { get; set; }

    public string? Producto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal SubTotal { get; set; }

    //public virtual Producto IdProductoNavigation { get; set; }

    //public virtual Venta IdVentaNavigation { get; set; }
}