using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Interfaces.Ventas;
using System.Data;

namespace NicoPasino.Servicios.Servicios.Ventas
{
    // Sin uso
    public class ServicioGenerico<Modelo, ModeloDto> : IServicioGenerico<Modelo, ModeloDto> where Modelo : class where ModeloDto : class
    {
        private readonly IRepositorioGenericoVentas<Modelo> _repoG;
        public ServicioGenerico(IRepositorioGenericoVentas<Modelo> repoG) {
            _repoG = repoG ?? throw new ArgumentNullException(nameof(repoG));
        }

        public async Task<IEnumerable<ModeloDto>> GetAll(bool activo) {
            try {
                var objsDb = await _repoG.ListarAsync(
                //filtro: m => m.Activo == activo
                //, orden: q => q.OrderByDescending(m => m.FechaCreacion)
                //, incluir: "IdCategoriaNavigation"
                );

                // si hay...
                if (objsDb != null && objsDb.Any()) {
                    return Enumerable.Empty<ModeloDto>();
                    //var objsDto = MapperGenerico.MapList<Modelo, ModeloDto>(objsDb);
                    //return objsDto;
                }
                return Enumerable.Empty<ModeloDto>();
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<ModeloDto>> GetAll(int idCliente) {
            try {
                //if (!ValidarCategoria(idCliente)) throw new ArgumentException("El ID de la Categoría no es válido.");

                var objsDb = await _repoG.ListarAsync(
                //filtro: m => (m.IdCliente == idCliente)
                //orden: q => q.OrderByDescending(m => m.FechaVenta),
                //incluir: "IdCategoriaNavigation.Nombre"
                );

                // si hay...
                if (objsDb != null && objsDb.Any()) {
                    return Enumerable.Empty<ModeloDto>();
                    //var objsDto = MapperGenerico.MapList<Modelo, ModeloDto>(objsDb);
                    //return objsDto;
                }

                return Enumerable.Empty<ModeloDto>();
            }
            catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex) {
                throw new Exception();
            }
        }

        public async Task<ModeloDto> GetById(int id) {
            if (id <= 0) throw new ArgumentException("Id no válido");
            var objDb = await _repoG.GetAsync(
            //filtro: m => m.Id == id
            );

            if (objDb != null) {
                //var objDto = MapperGenerico.Map<Modelo, ModeloDto>(objDb);
                //return objDto;
            }

            return null;
        }

        public async Task<bool> Create(ModeloDto obj) {
            Random random = new Random();
            if (obj == null) throw new DataException("No se recibió ningún dato.");
            // TODO: otras validaciones

            //obj.IdPublica = random.Next(1, 9999999);
            //var objeto = MapperGenerico.Map<ModeloDto, Modelo>(obj);

            //objeto.FechaVenta = DateTime.UtcNow;

            //var res = await _repoG.Add(objeto);

            //return (res != null);
            return true;
        }

        public Task<IEnumerable<ModeloDto>> GetAll(string Busqueda) {
            throw new NotImplementedException();
        }

        public Task<bool> Update(ModeloDto obj) {
            throw new NotImplementedException();
        }

        public Task<bool> Enable(int id, bool estado) {
            throw new NotImplementedException();
        }
    }
}