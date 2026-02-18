using Microsoft.AspNetCore.Mvc;
using NicoPasino.Core.DTO.Ventas;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Modelos.Ventas;

namespace NicoPasino.Controllers
{
    [Route("api/[controller]")] // Definir la ruta base para todo el controlador
    [ApiController]
    public partial class VentasController : ControllerBase
    {
        private readonly IServicioGenerico<Producto, ProductoDto> _productoServicio;
        private readonly IServicioGenerico<Venta, VentaDto> _ventaServicio;
        private readonly IServicioGenerico<Cliente, ClienteDto> _clienteServicio;
        private readonly IServicioGenerico<Categoria, CategoriaDto> _categoriaServicio;

        public VentasController(IServicioGenerico<Producto, ProductoDto> pServicio,
                                IServicioGenerico<Venta, VentaDto> vServicio,
                                IServicioGenerico<Cliente, ClienteDto> clienteServicio,
                                IServicioGenerico<Categoria, CategoriaDto> categoriaServicio) {
            _productoServicio = pServicio;
            _ventaServicio = vServicio;
            _clienteServicio = clienteServicio;
            _categoriaServicio = categoriaServicio;
        }


        // Nota: (Métodos y Rutas en otros archivos).

        // Controllers/
        // └── (Ventas)
        //     ├── VentasController.cs
        //     ├── Ventas.Productos.cs + (categorías)
        //     └── Ventas.Clientes.cs
        //     └── Ventas.Ventas.cs
    }
}
