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
                //orden: q => q.OrderByDescending(m => m.FechaModificacion) // TODO: agregar timestamp fechamodificacion
                //orden: q => q.OrderByDescending(m => m.Id) // TODO: agregar timestamp fechamodificacion
                );

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

        public async Task<CardsDto> GetById(int id) {
            if (id <= 0) return null;
            var objDb = await _repoG.GetAsync(
                filtro: item => item.IdPublica == id // id -> IdPublica
            );

            if (objDb != null) {
                var objDto = objDb.Adapt<CardsDto>();
                return objDto;
            }
            else return null;
        }

        public async Task<bool> Create(CardsDto obj) {
            Random random = new Random();
            if (obj == null) throw new DataException("No se recibió ningún dato.");
            if (ValidateObj(obj)) throw new DataException("Titulo y/o Texto vacíos.");


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
            if (ValidateObj(obj)) throw new DataException("Titulo y/o Texto vacíos.");

            var objDb = await _repoG.GetAsync(filtro: x => x.IdPublica == obj.Id);
            if (objDb == null) throw new DataException("Objeto original no encontrado.");

            var objeto = obj.Adapt<Cards>();

            var tiempoActual = DateHelper.GetDate();
            objeto.Id = objDb.Id;
            objeto.IdPublica = obj.Id;
            objeto.Fecha = tiempoActual.fecha;
            objeto.Hora = tiempoActual.hora;

            Cards objetoRdy = objeto.Adapt<Cards>();

            var res = await _repoG.Update(objetoRdy);

            if (res > 0) return true;
            else throw new UpdateException("No se pudo actualizar en la base de datos.");
        }

        public async Task<bool> Enable(int id, bool estado) {
            var objDb = await _repoG.GetAsync(filtro: x => x.IdPublica == id);
            if (objDb == null) throw new DataException("Objeto original no encontrado.");

            await _repoG.Delete(objDb);

            return true;
        }

        public Task<IEnumerable<CardsDto>> GetAll(string campo, string? valor) {
            throw new NotImplementedException();
        }

        public bool ValidateObj(CardsDto card) {
            if (string.IsNullOrWhiteSpace(card.Header)) return false;
            if (string.IsNullOrWhiteSpace(card.Text)) return false;
            return true;
        }
    }
}