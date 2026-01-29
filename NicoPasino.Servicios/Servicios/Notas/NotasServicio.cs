using Mapster;
using NicoPasino.Core.DTO.Notas;
using NicoPasino.Core.Errores;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Modelos.Notas;
using NicoPasino.Core.Utils;

namespace NicoPasino.Servicios.Servicios.Notas
{
    public class NotasServicio : IServicioGenerico<Cards, CardsDto>
    {
        private readonly IRepositorioGenerico<Cards> _repoG;
        public NotasServicio(IRepositorioGenerico<Cards> repoG) {
            _repoG = repoG ?? throw new ArgumentNullException(nameof(repoG));
        }

        public async Task<IEnumerable<CardsDto>> GetAll(bool activo) {
            try {
                var objsDb = await _repoG.ListarAsync(
                //filtro: m => m.Activo == activo
                //, orden: q => q.OrderByDescending(m => m.FechaCreacion)
                //, incluir: "IdCategoriaNavigation"
                );

                // si hay...
                if (objsDb != null && objsDb.Any()) {
                    var objsDto = objsDb.Adapt<IEnumerable<CardsDto>>();
                    return objsDto;
                }
                return Enumerable.Empty<CardsDto>();
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<CardsDto>> GetAll(int idOtroTipo) {
            try {
                var objsDb = await _repoG.ListarAsync(
                //filtro: m => (m.idOtroTipo == idOtroTipo)
                //orden: q => q.OrderByDescending(m => m.FechaVenta),
                //incluir: "IdCategoriaNavigation.Nombre"
                );

                // si hay...
                if (objsDb != null && objsDb.Any()) {
                    var objsDto = objsDb.Adapt<IEnumerable<CardsDto>>();
                    return objsDto;
                }

                return Enumerable.Empty<CardsDto>();
            }
            catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex) {
                throw new Exception();
            }
        }

        public Task<IEnumerable<CardsDto>> GetAll(string Busqueda) {
            throw new NotImplementedException();
        }

        public async Task<CardsDto> GetById(int id) {
            if (id <= 0) throw new ArgumentException("Id no válido");
            var objDb = await _repoG.GetAsync(
                filtro: item => item.IdPublica == id // id -> IdPublica
            );

            if (objDb != null) {
                var objDto = objDb.Adapt<CardsDto>();
                return objDto;
            }

            return null;
        }

        public async Task<bool> Create(CardsDto obj) {
            Random random = new Random();
            if (obj == null) throw new DataException("No se recibió ningún dato.");
            // TODO: otras validaciones

            obj.Id = random.Next(1, 9999999); // id -> IdPublica
            var objeto = obj.Adapt<Cards>();

            var tiempoActual = DateHelper.GetDate();
            objeto.Fecha = tiempoActual.fecha;
            objeto.Hora = tiempoActual.hora;

            var res = await _repoG.Add(objeto);

            return (res != null);
            //return true;
        }


        public async Task<bool> Update(CardsDto obj) {
            if (obj == null) throw new DataException("No se recibió ningún dato.");
            // TODO: otras validaciones

            var objeto = obj.Adapt<Cards>();

            var tiempoActual = DateHelper.GetDate();

            objeto.Fecha = tiempoActual.fecha;
            objeto.Hora = tiempoActual.hora;

            var res = await _repoG.Update(objeto);

            return (res != null);
            //return true;
        }

        public async Task<bool> Enable(int id, bool estado) {
            var objDb = await _repoG.GetAsync(filtro: x => x.IdPublica == id);
            if (objDb == null) throw new DataException("Objeto original no encontrado.");

            await _repoG.Delete(objDb);

            return true;
        }
    }
}