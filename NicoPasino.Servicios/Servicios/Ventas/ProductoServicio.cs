using Mapster;
using Microsoft.IdentityModel.Tokens;
using NicoPasino.Core.DTO.Ventas;
using NicoPasino.Core.Errores;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Interfaces.Ventas;
using NicoPasino.Core.Modelos.Ventas;


namespace NicoPasino.Servicios.Servicios.Ventas
{
    public class ProductoServicio : IServicioGenerico<Producto, ProductoDto>
    {
        private readonly IRepositorioGenericoVentas<Producto> _repoG;
        public ProductoServicio(IRepositorioGenericoVentas<Producto> repoG) {
            _repoG = repoG ?? throw new ArgumentNullException(nameof(repoG));
        }

        public async Task<IEnumerable<ProductoDto>> GetAll(bool activo) {
            try {
                var objsDb = await _repoG.ListarAsync(
                    filtro: m => m.Activo == activo
                    //, orden: q => q.OrderByDescending(m => m.FechaCreacion)
                    , incluir: "IdCategoriaNavigation"
                );

                // si hay...
                if (objsDb != null && objsDb.Any()) {
                    var objsDto = objsDb.Adapt<IEnumerable<ProductoDto>>();
                    return objsDto;
                }
                return Enumerable.Empty<ProductoDto>();
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<ProductoDto>> GetAll(string nombre) {
            if (nombre.IsNullOrEmpty()) throw new ArgumentException("Texto vacío o no válido.");
            // TODO: otras validaciones

            try {
                nombre = nombre.Trim();
                nombre = nombre.Length > 30 ? nombre = nombre.Substring(0, 30) : nombre;

                var objsDb = await _repoG.ListarAsync(
                    filtro: m => m.Activo && (string.IsNullOrEmpty(nombre) || m.Nombre.Contains(nombre))
                    //orden: q => q.OrderByDescending(m => m.FechaCreacion)
                    , incluir: "IdCategoriaNavigation"
                );

                // si hay...
                if (objsDb != null && objsDb.Any()) {
                    var objsDto = objsDb.Adapt<IEnumerable<ProductoDto>>();
                    return objsDto;
                }
                return Enumerable.Empty<ProductoDto>();
            }
            catch (Exception ex) {
                throw new Exception();
            }
        }

        public async Task<IEnumerable<ProductoDto>> GetAll(int idCategoria) {
            try {
                //if (!ValidarCategoria(idCategoria)) throw new ArgumentException("El ID de la Categoría no es válido.");

                var objsDb = await _repoG.ListarAsync(
                    filtro: m => m.Activo && (m.IdCategoriaNavigation != null && m.IdCategoriaNavigation.Id == idCategoria)
                    //orden: q => q.OrderByDescending(m => m.FechaCreacion)
                    , incluir: "IdCategoriaNavigation"
                );

                // si hay...
                if (objsDb != null && objsDb.Any()) {
                    var objsDto = objsDb.Adapt<IEnumerable<ProductoDto>>();
                    return objsDto;
                }

                return Enumerable.Empty<ProductoDto>();
            }
            catch (ArgumentException ex) {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex) {
                throw new Exception();
            }
        }

        public async Task<ProductoDto> GetById(int id) {
            if (id <= 0) throw new ArgumentException("Id no válido");
            var objDb = await _repoG.GetAsync(
                filtro: m => m.IdPublica == id
                , incluir: "IdCategoriaNavigation"
            );

            if (objDb != null) {
                var objDto = objDb.Adapt<ProductoDto>();
                return objDto;
            }
            return new ProductoDto();
        }

        public async Task<bool> Create(ProductoDto obj) {
            Random random = new Random();
            if (obj == null) throw new DataException("No se recibió ningún dato.");
            // TODO: otras validaciones

            obj.IdPublica = random.Next(1, 9999999);
            var objeto = obj.Adapt<Producto>();

            objeto.FechaCreacion = DateTime.UtcNow;
            objeto.Activo = true;

            // Asegurar que se use sólo la FK y evitar que EF intente insertar una nueva Categoria
            objeto.IdCategoria = obj.IdCategoria;
            objeto.IdCategoriaNavigation = null;

            var res = await _repoG.Add(objeto);

            return (res != null);
        }

        public async Task<bool> Update(ProductoDto obj) {
            if (obj == null) throw new DataException("No se recibió ningún dato.");
            else if (obj.IdPublica == null) throw new DataException("No se recibió ningún ID.");
            else if (obj.IdCategoria <= 0) throw new DataException("La categoría no es válida."); // TODO:
            // TODO: comparar con datos originales
            //var movieOriginal = await _servicio.GetById((int)objeto.idPublica); 
            //if (objeto == movieOriginal) throw new MovieDataException("Se recibieron datos sin cambios, No se actualizó."); // FIXME:

            // TODO: otras validaciones

            // obtener obj original
            var objDb = await _repoG.GetAsync(filtro: x => x.IdPublica == obj.IdPublica/*, incluir: "IdCategoriaNavigation"*/);
            if (objDb == null) throw new DataException("Objeto original no encontrada.");

            // mapear
            //var objMapped = ProductoMapper.ConvertToModel(obj);
            var objMapped = obj.Adapt<Producto>(); // TODO:

            // Asegurar que se use sólo la FK y evitar que EF intente insertar una nueva Categoria
            objMapped.IdCategoria = obj.IdCategoria;
            objMapped.IdCategoriaNavigation = null;

            objMapped.Id = objDb.Id;
            objMapped.FechaCreacion = objDb.FechaCreacion;
            objMapped.FechaModificacion = DateTime.UtcNow;

            // subir
            var res = await _repoG.Update(objMapped);
            if (res > 0) return true;
            else throw new UpdateException("No pudo actualizar en la base de datos.");
        }

        public async Task<bool> Enable(int id, bool estado) {
            if (id <= 0) throw new ArgumentException("Id no válido");
            var objDb = await _repoG.GetAsync(filtro: m => m.IdPublica == id);

            if (objDb != null) {
                objDb.FechaModificacion = DateTime.UtcNow;
                objDb.Activo = estado;

                await _repoG.Update(objDb);
                //await _uow.SaveChangesAsync();
                return true;
            }
            else throw new ArgumentException("Id no válido");
        }

        /// <summary> Return bool. </summary>
        /*public bool ValidarCategoria(int idCat) {
            var _repoCat = _uow.Repositorio<Categoria>();
            var AllIds = _repoCat.GetAll();
            if (idCat <= 0) return false;
            if (!AllIds.Result.Any(g => g.Id == idCat)) return false;
            return true;
        }*/

        /*public async Task GuardarProductoCategoria(int productoId, IEnumerable<int> genreIds) {
            // borrar géneros anteriores si existen
            var productoGenres = await _repoMovieGenres.ListarAsync(filtro: mg => mg.MovieId == productoId);
            if (productoGenres != null) {
                await _repoMovieGenres.DeleteRange(productoGenres);
                await _uow.SaveChangesAsync();
            }

            // agregar nuevos géneros
            foreach (var id in genreIds) {
                var gen = new Moviegenres { MovieId = productoId, GenreId = id };
                await _repoMovieGenres.Add(gen);
            }
            await _uow.SaveChangesAsync();
        }*/
    }
}