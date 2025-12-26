using NicoPasino.Core.DTO.Ventas;
using NicoPasino.Core.Errores;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Interfaces.Ventas;
using NicoPasino.Core.Mapper.Ventas;
using NicoPasino.Core.Modelos.Ventas;

namespace NicoPasino.Servicios.Servicios.Ventas
{
    public class VentaServicio : IServicioGenerico<Venta, VentaDto>
    {
        private readonly IRepositorioGenericoVentas<Venta> _repoG;
        public VentaServicio(IRepositorioGenericoVentas<Venta> repoG) {
            _repoG = repoG ?? throw new ArgumentNullException(nameof(repoG));
        }

        public async Task<IEnumerable<VentaDto>> GetAll(bool activo) {
            try {
                var objsDb = await _repoG.ListarAsync(
                //filtro: m => m.Activo == activo
                //, orden: q => q.OrderByDescending(m => m.FechaCreacion)
                //, incluir: "IdCategoriaNavigation"
                );

                // si hay...
                if (objsDb != null && objsDb.Any()) {
                    var objsDto = MapperGenerico.MapList<Venta, VentaDto>(objsDb);
                    return objsDto;
                }
                return Enumerable.Empty<VentaDto>();
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<VentaDto>> GetAll(int idCliente) {
            try {
                //if (!ValidarCategoria(idCliente)) throw new ArgumentException("El ID de la Categoría no es válido.");

                var objsDb = await _repoG.ListarAsync(
                    filtro: m => (m.IdCliente == idCliente)
                    //orden: q => q.OrderByDescending(m => m.FechaVenta),
                    //incluir: "IdCategoriaNavigation.Nombre"
                );

                // si hay...
                if (objsDb != null && objsDb.Any()) {
                    var objsDto = MapperGenerico.MapList<Venta, VentaDto>(objsDb);
                    return objsDto;
                }

                return Enumerable.Empty<VentaDto>();
            }
            catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex) {
                throw new Exception();
            }
        }

        public async Task<VentaDto> GetById(int id) {
            if (id <= 0) throw new ArgumentException("Id no válido");
            var objDb = await _repoG.GetAsync(
                filtro: m => m.Id == id
            );

            if (objDb != null) {
                var objDto = MapperGenerico.Map<Venta, VentaDto>(objDb);
                return objDto;
            }

            return new VentaDto();
        }

        public async Task<bool> Create(VentaDto obj) {
            Random random = new Random();
            if (obj == null) throw new DataException("No se recibió ningún dato.");
            // TODO: otras validaciones

            obj.IdPublica = random.Next(1, 9999999);
            var movie = MapperGenerico.Map<VentaDto, Venta>(obj);

            movie.FechaVenta = DateTime.UtcNow;

            var res = await _repoG.Add(movie);

            return (res != null);
        }

        public Task<IEnumerable<VentaDto>> GetAll(string Busqueda) {
            throw new NotImplementedException();
        }

        public Task<bool> Update(VentaDto obj) {
            throw new NotImplementedException();
        }

        public Task<bool> Enable(int id, bool estado) {
            throw new NotImplementedException();
        }
    }
}