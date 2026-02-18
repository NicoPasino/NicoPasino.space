using Mapster;
using NicoPasino.Core.DTO.Notas;
using NicoPasino.Core.Errores;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Modelos.Notas;
using NicoPasino.Core.Utils;
using System.Linq.Expressions;

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

        public async Task<IEnumerable<CardsDto>> GetAll(string campo, string? valor) {
            if (string.IsNullOrWhiteSpace(campo)) throw new ArgumentException("Campo de búsqueda no válido.");
            campo = campo.Trim().ToLowerInvariant();
            valor = valor?.Trim();

            Expression<Func<Cards, bool>> filtro = null;
            Func<IQueryable<Cards>, IOrderedQueryable<Cards>> orden = null;

            bool valorIsNull = string.IsNullOrEmpty(valor);
            var vLower = valorIsNull ? string.Empty : valor!.ToLowerInvariant();

            switch (campo) {
                case "header":
                case "headerdesc":
                    orden = (campo == "header")
                        ? new Func<IQueryable<Cards>, IOrderedQueryable<Cards>>(q => q.OrderBy(m => m.Header))
                        : new Func<IQueryable<Cards>, IOrderedQueryable<Cards>>(q => q.OrderByDescending(m => m.Header));
                    if (!valorIsNull) {
                        filtro = m => m.Header != null
                                    && m.Header.ToLower().Contains(vLower);
                    }
                    break;

                case "text":
                case "textdesc":
                    orden = (campo == "text")
                        ? new Func<IQueryable<Cards>, IOrderedQueryable<Cards>>(q => q.OrderBy(m => m.Text))
                        : new Func<IQueryable<Cards>, IOrderedQueryable<Cards>>(q => q.OrderByDescending(m => m.Text));
                    if (!valorIsNull) {
                        filtro = m => m.Text != null
                                    && m.Text.ToLower().Contains(vLower);
                    }
                    break;

                case "color":
                case "colordesc":
                    orden = (campo == "color")
                        ? new Func<IQueryable<Cards>, IOrderedQueryable<Cards>>(q => q.OrderBy(m => m.Color))
                        : new Func<IQueryable<Cards>, IOrderedQueryable<Cards>>(q => q.OrderByDescending(m => m.Color));
                    if (!valorIsNull) {
                        filtro = m => m.Color != null
                                    && m.Color != null
                                    && m.Color.ToLower().Contains(vLower);
                    }
                    break;

                default:
                    throw new DataException($"Campo de búsqueda '{campo}' no soportado. Campos soportados: header, text, color.");
            }

            var objsDb = await _repoG.ListarAsync(filtro: filtro, orden: orden);

            if (objsDb != null && objsDb.Any()) {
                var objsDto = objsDb.Adapt<IEnumerable<CardsDto>>();
                return objsDto;
            }
            else return Enumerable.Empty<CardsDto>();
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
    }
}