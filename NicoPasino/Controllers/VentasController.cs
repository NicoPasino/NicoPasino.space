using Microsoft.AspNetCore.Mvc;
using NicoPasino.Core.Interfaces.Ventas;

namespace NicoPasino.Controllers
{
    [Route("api/[controller]")] // Definir la ruta base para todo el controlador
    [ApiController]
    public partial class VentasController : ControllerBase
    {
        private readonly IProductoServicio _productoServicio;
        private readonly IProductoServicio _ventaServicio;
        public VentasController(IProductoServicio pServicio, IProductoServicio vServicio) {
            _productoServicio = pServicio;
            _ventaServicio = vServicio;
        }


        // Nota: (Métodos y Rutas en otros archivos).

        // Controllers/
        // └── Ventas/
        //     ├── VentasController.cs
        //     ├── Ventas.Productos.cs
        //     └── Ventas.Clientes.cs
    }
}
