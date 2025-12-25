using Microsoft.AspNetCore.Mvc;
using NicoPasino.Core.DTO.Ventas;
using NicoPasino.Core.Errores;

namespace NicoPasino.Controllers
{
    public partial class VentasController
    {
        [HttpGet("Productos")]
        public async Task<ActionResult> GetAllProductos() {
            var objs = await _productoServicio.GetAll(true);
            return Ok(objs);
        }

        [HttpGet("Productos/{id}")]
        public async Task<ActionResult> GetProducto(int id) {
            var obj = await _productoServicio.GetById(id);
            return Ok(obj);
            // return NotFound(new { mensaje = "Producto no encontrado" }); // 404
        }

        [HttpPost("Productos")]
        public async Task<IActionResult> CrearProducto([FromBody] ProductoDto objeto) {
            try {
                if (objeto == null) throw new DataException("Datos inválidos, por favor revisar.");

                var ok = await _productoServicio.Create(objeto);
                if (ok) return Ok(new { success = true, message = "Data received successfully." });
                else throw new Exception();
            }
            catch (DataException ex) {
                return BadRequest(new { error = ex.Message }); // 400
            }
            catch (Exception ex) {
                return new ObjectResult("Error de servidor: StatusCode 500") { StatusCode = 500 };
            }
        }

        [HttpPut("Productos")]
        public async Task<IActionResult> ActualizarProducto([FromBody] ProductoDto objeto) {
            try {
                if (objeto == null) throw new DataException("Datos inválidos, por favor revisar.");

                var ok = await _productoServicio.Update(objeto);
                if (ok) return Ok(new { success = true, message = "Data received successfully." });
                else throw new Exception();
            }
            catch (DataException ex) {
                return BadRequest(new { error = ex.Message }); // 400
            }
            catch (Exception ex) {
                return new ObjectResult("Error de servidor: StatusCode 500. "/* + ex.Message*/) { StatusCode = 500 };
            }
        }

        [HttpDelete("Productos/{id}")]
        public async Task<IActionResult> EliminarProducto(int id) {
            var res = await _productoServicio.Enable(id, false);
            return Ok(new { success = true, message = "Data received successfully." });
        }
    }
}
