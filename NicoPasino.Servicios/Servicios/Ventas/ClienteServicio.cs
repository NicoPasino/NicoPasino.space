using Mapster;
using NicoPasino.Core.DTO.Ventas;
using NicoPasino.Core.Errores;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Interfaces.Ventas;
using NicoPasino.Core.Modelos.Ventas;

namespace NicoPasino.Servicios.Servicios.Ventas
{
    public class ClienteServicio : IServicioGenerico<Cliente, ClienteDto>
    {
        private readonly IRepositorioGenericoVentas<Cliente> _repoG;
        public ClienteServicio(IRepositorioGenericoVentas<Cliente> repoG) {
            _repoG = repoG ?? throw new ArgumentNullException(nameof(repoG));
        }

        public async Task<IEnumerable<ClienteDto>> GetAll(bool activo) {
            try {
                var objsDb = await _repoG.ListarAsync(
                //filtro: m => m.Activo == activo
                //, orden: q => q.OrderByDescending(m => m.FechaCreacion)
                incluir: "Venta"
                );

                // si hay...
                if (objsDb != null && objsDb.Any()) {
                    var objsDto = objsDb.Adapt<IEnumerable<ClienteDto>>();
                    return objsDto;
                }
                return Enumerable.Empty<ClienteDto>();
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ClienteDto> GetById(int dni) {
            if (dni <= 9999999 || dni > 999999999) throw new DataException("Documento no válido.");
            var objDb = await _repoG.GetAsync(filtro: m => m.Documento == dni, incluir: "Venta");

            if (objDb != null) {
                var objDto = objDb.Adapt<ClienteDto>();
                return objDto;
            }
            else return new ClienteDto();
        }

        public async Task<bool> Create(ClienteDto obj) {
            ValidarDatos(obj);

            var objeto = obj.Adapt<Cliente>();
            var res = await _repoG.Add(objeto);

            return (res != null);
        }

        public async Task<bool> Update(ClienteDto obj) {
            ValidarDatos(obj);

            // obtener obj original
            var objDb = await _repoG.GetAsync(filtro: x => x.Documento == obj.Documento, incluir: "Venta");
            if (objDb == null) throw new DataException("Objeto original no encontrado.");

            // TODO: comparar con datos originales
            //if (obj == objDb) throw new MovieDataException("Se recibieron datos sin cambios, No se actualizó."); // FIXME:

            // mapear
            var objeto = obj.Adapt<Cliente>();
            //objeto.Id = objDb.Id;

            // subir
            var res = await _repoG.Update(objeto);
            if (res > 0) return true;
            else throw new UpdateException("No se pudo actualizar en la base de datos.");
        }


        private void ValidarDatos(ClienteDto obj) {
            if (obj == null) throw new DataException("No se recibió ningún dato.");
            if (obj.Documento <= 9999999 || obj.Documento > 999999999) throw new DataException("Documento no válido.");
            if (obj.Nombre == null || obj.Nombre.Trim().Length <= 3) throw new DataException("Nombre no válido.");
            if (obj.Correo == null || obj.Correo.Trim().Length <= 8) throw new DataException("Correo no válido.");
            // TODO: otras validaciones + expresiones regulares
        }




        public Task<bool> Enable(int id, bool estado) {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<ClienteDto>> GetAll(int cod) {
            throw new NotImplementedException("Método GetAll(int) de Cliente no configurado");
        }
        public Task<IEnumerable<ClienteDto>> GetAll(string Busqueda) {
            throw new NotImplementedException("Método GetAll(string) de Cliente no configurado");
        }

        /*public async Task<bool> Enable(int id, bool estado) {
            if (id <= 0) throw new ArgumentException("Id no válido");
            var objDb = await _repoG.GetAsync(filtro: m => m.Id == id);

            if (objDb != null) {
                //objDb.FechaModificacion = DateTime.UtcNow;
                objDb.Activo = estado;

                await _repoG.Update(objDb);
                //await _uow.SaveChangesAsync();
                return true;
            }
            else throw new ArgumentException("Id no válido");
        }*/
    }
}