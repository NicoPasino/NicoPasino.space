using Microsoft.AspNetCore.Mvc;
using NicoPasino.Core.DTO.Notas;
using NicoPasino.Core.Errores;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Modelos.Notas;

namespace NicoPasino.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Definir la ruta base para todo el controlador
    public class NotesController : Controller
    {
        private readonly IServicioGenerico<Cards, CardsDto> _notasServicio;
        public NotesController(IServicioGenerico<Cards, CardsDto> nServicio) {
            _notasServicio = nServicio;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll() {
            var objs = await _notasServicio.GetAll(true);
            return Ok(objs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id) {
            var obj = await _notasServicio.GetById(id);
            if (obj?.Id != null) return Ok(obj);
            else return NotFound(new { message = "Elemento no encontrado" }); // 404
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CardsDto objeto) {
            try {
                if (objeto == null) throw new DataException("Datos inválidos, por favor revisar.");

                var ok = await _notasServicio.Create(objeto);
                if (ok) return StatusCode(202, "Elemento Creado");
                else throw new Exception();
            }
            catch (DataException ex) {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (Exception ex) {
                return new ObjectResult("Error de servidor: StatusCode 500") { StatusCode = 500 };
            }
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] CardsDto objeto) {
            try {
                if (objeto == null) throw new DataException("Datos inválidos, por favor revisar.");

                var ok = await _notasServicio.Update(objeto);
                if (ok) return new ObjectResult("Actualizado") { StatusCode = 202 };
                else throw new Exception();
            }
            catch (DataException ex) {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (Exception ex) {
                return new ObjectResult("Error de servidor: StatusCode 500. " /*+ ex.Message*/) { StatusCode = 500 };
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id) {
            var res = await _notasServicio.Enable(id, false);
            if (res) return Ok();
            else return new ObjectResult("Error de servidor: StatusCode 500. ") { StatusCode = 500 };
        }
    }
}
